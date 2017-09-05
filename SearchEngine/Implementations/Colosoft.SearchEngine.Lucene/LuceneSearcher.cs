/* 
 * Colosoft Framework - generic framework to assist in development on the .NET platform
 * Copyright (C) 2013  <http://www.colosoft.com.br/framework> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using StandardAnalyzer = Lucene.Net.Analysis.Standard.StandardAnalyzer;
using IndexWriter = Lucene.Net.Index.IndexWriter;
using FSDirectory = Lucene.Net.Store.FSDirectory;
using Version = Lucene.Net.Util.Version;
using Document = Lucene.Net.Documents.Document;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Representa a pesquisador do Lucene.
	/// </summary>
	[Export(typeof(ISearcher))]
	[Export(typeof(ISearcherMaintenance))]
	[System.ComponentModel.Composition.PartCreationPolicy(CreationPolicy.Shared)]
	public class LuceneSearcher : ISearcher, ISearcherMaintenance, IDisposable
	{
		private bool _initialized = false;

		private readonly ISchemeRepository _structRepository;

		private readonly IRepositoryLoader _repositoryLoader;

		private readonly IDataRepository _dataRepository;

		private readonly ISearchStatistics _statisticsManager;

		private readonly Services.SyncSearchService _syncService;

		private global::Lucene.Net.Store.RAMDirectory _ramDirectory;

		private object _ramDirectoryLock = new object();

		private System.Threading.ManualResetEvent _createWriterResetEvent = new System.Threading.ManualResetEvent(true);

		private static System.Globalization.CultureInfo _currentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

		/// <summary>
		/// Dicionário onde são armazenados as informações dos segmentos usados
		/// pelo pesquisador.
		/// </summary>
		private Dictionary<string, List<int>> _segments = new Dictionary<string, List<int>>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Identifica se o escritor já foi criado.
		/// </summary>
		private bool _writerCreated = false;

		private object _processElementsLock = new object();

		/// <summary>
		/// Data de corte para recuperar os elementos que foram removidos.
		/// </summary>
		private DateTime _removedElementsCutoffDate = DateTime.Now;

		/// <summary>
		/// Data de corte para recuperar os novos elementos.
		/// </summary>
		private DateTime _newElementsCutoffDate = DateTime.Now;

		private IndexReader _reader;

		/// <summary>
		/// Instancia responsável pela manutenção do pesquisador.
		/// </summary>
		public ISearcherMaintenance Maintenance
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Diretório usado pelo indice.
		/// </summary>
		public global::Lucene.Net.Store.Directory IndexDirectory
		{
			get
			{
				lock (_ramDirectoryLock)
				{
					if(_ramDirectory == null)
						_ramDirectory = new global::Lucene.Net.Store.RAMDirectory();
					return _ramDirectory;
				}
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="structRepository"></param>
		/// <param name="repositoryLoader"></param>
		/// <param name="dataRepository"></param>
		/// <param name="statisticsManager"></param>
		[ImportingConstructor]
		public LuceneSearcher(ISchemeRepository structRepository, IRepositoryLoader repositoryLoader, IDataRepository dataRepository, ISearchStatistics statisticsManager)
		{
			_structRepository = structRepository;
			_repositoryLoader = repositoryLoader;
			_dataRepository = dataRepository;
			_statisticsManager = statisticsManager;
			_syncService = new Colosoft.SearchEngine.Services.SyncSearchService(this);
		}

		/// <summary>
		/// Reseta o diretório do indice.
		/// </summary>
		private void ResetIndexDirectory()
		{
		}

		/// <summary>
		/// Recupera o tipo de ordenação do Lucene.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private int GetLuceneSortType(Type type)
		{
			switch(type.Name)
			{
			case "Int16":
				return global::Lucene.Net.Search.SortField.SHORT;
			case "Int32":
				return global::Lucene.Net.Search.SortField.INT;
			case "Int64":
				return global::Lucene.Net.Search.SortField.LONG;
			case "Double":
				return global::Lucene.Net.Search.SortField.DOUBLE;
			case "Single":
				return global::Lucene.Net.Search.SortField.FLOAT;
			case "DateTime":
				return global::Lucene.Net.Search.SortField.STRING;
			default:
				return global::Lucene.Net.Search.SortField.STRING;
			}
		}

		/// <summary>
		/// Incrementa a quantidade para as palavras pesquisas.
		/// </summary>
		/// <param name="searchParameters"></param>
		private void IncrementCountWords(IEnumerable<SearchParameter> searchParameters)
		{
			var sb = new StringBuilder();
			Colosoft.SearchEngine.Channel channel = null;
			List<KeyValuePair<SchemeIndex, string>> rankTerms = new List<KeyValuePair<SchemeIndex, string>>();
			foreach (var parameter in searchParameters)
			{
				if(parameter.Name == "ChannelId")
				{
					var channelId = int.Parse(parameter.Value);
					channel = _structRepository.GetChannel((byte)channelId);
				}
				var field = _structRepository.GetSchemaField(parameter.Name);
				var schemeIndex = _structRepository.GetSchemaIndex(parameter.Name);
				if(field != null)
				{
					if(field.IncludeInStatistics && !string.IsNullOrEmpty(parameter.Value))
						sb.Append(parameter.Value).Append(" ");
				}
				if(schemeIndex != null)
				{
					if(schemeIndex.IsRanked)
						rankTerms.Add(new KeyValuePair<SchemeIndex, string>(schemeIndex, parameter.Value));
				}
			}
			if(channel != null && rankTerms.Count > 0)
			{
				foreach (var i in rankTerms)
					_statisticsManager.IncrementCountWords(i.Key.SchemeIndexId, channel, i.Value);
			}
			IncrementCountWords(sb.ToString(), searchParameters.Select(f => (Parameter)f));
		}

		/// <summary>
		/// Incrementa a quantidade para as palavras pesquisas.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parameters">Parametros usados na pesquisa.</param>
		private void IncrementCountWords(string text, IEnumerable<Parameter> parameters)
		{
			if(string.IsNullOrEmpty(text))
				return;
			var analyzer = new StandardAnalyzer(Version.LUCENE_29, Stopwords.PORTUGUESE_SET);
			var stringReader = new System.IO.StringReader(text);
			var tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);
			var statisticsManager2 = _statisticsManager as ISearchStatistics2;
			var token = tokenStream.Next();
			while (token != null)
			{
				var termText = token.TermText();
				if(_segments.ContainsKey(termText))
				{
					_statisticsManager.IncrementCountWords(token.TermText());
					if(statisticsManager2 != null)
						statisticsManager2.IncrementCountWords(text, parameters);
				}
				token = tokenStream.Next();
			}
		}

		/// <summary>
		/// Converte os dados de sort para serem usados pelo Lucene.
		/// </summary>
		/// <param name="sort"></param>
		/// <param name="sortFields">Campos de ordenação adicionais</param>
		/// <returns></returns>
		private global::Lucene.Net.Search.Sort Convert(Sort sort, global::Lucene.Net.Search.SortField[] sortFields)
		{
			global::Lucene.Net.Search.Sort luceneSort;
			var fields = new List<global::Lucene.Net.Search.SortField>();
			if(sort == null && sortFields != null && sortFields.Length > 0)
				fields.AddRange(sortFields);
			if(sort != null && sort.Fields.Length > 0)
				fields.AddRange(sort.Fields.Select(f =>  {
					SchemeField field = null;
					if(f.Name == "Rnd")
						field = new SchemeField("Rnd", typeof(int), "Rnd", false, false);
					else
						field = _structRepository.GetSchemaField(f.Name);
					Type fieldType = null;
					if(field == null)
					{
						if(f.Name.EndsWith("Ticks"))
						{
							var name = f.Name.Substring(0, f.Name.Length - "Ticks".Length);
							field = _structRepository.GetSchemaField(name);
							if(field == null)
								return null;
							fieldType = typeof(long);
						}
						else
							return null;
					}
					else
						fieldType = field.Type;
					return new global::Lucene.Net.Search.SortField(f.Name, GetLuceneSortType(fieldType), f.Reverse);
				}).Where(f => f != null));
			if(fields.Count > 0)
				luceneSort = new global::Lucene.Net.Search.Sort(fields.ToArray());
			else
				luceneSort = new global::Lucene.Net.Search.Sort();
			return luceneSort;
		}

		/// <summary>
		/// Verifica se as opções de pesquisa são válidas.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		private void ValidateOptions(SearchOptions options)
		{
			if(options.RandomResult && options.TotalHits == 0)
				throw new Exception("For random result is need TotalHits in options.");
		}

		/// <summary>
		/// Recupera o valor do parametro.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="schemeIndex"></param>
		/// <returns></returns>
		private static string GetParameterValue(Parameter parameter, SchemeIndex schemeIndex)
		{
			return parameter == null || parameter.Value == null ? "<<NULL>>" : RemoveAcent(parameter.Value);
		}

		/// <summary>
		/// Recupera os valores do parametro.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="schemeIndex"></param>
		/// <returns></returns>
		private static string[] GetParameterValues(Parameter parameter, SchemeIndex schemeIndex)
		{
			if(parameter == null || parameter.Values == null)
				return null;
			var isDateTime = schemeIndex != null && schemeIndex.FieldSchema != null && schemeIndex.FieldSchema.Length == 1 && (schemeIndex.FieldSchema[0].Type == typeof(DateTime) || schemeIndex.FieldSchema[0].Type == typeof(DateTime?));
			var result = new string[parameter.Values.Length];
			if(isDateTime && result.Length > 1)
				parameter.Name += "Ticks";
			for(var i = 0; i < result.Length; i++)
			{
				if(isDateTime && result.Length > 1 && parameter.Values[i] != null)
				{
					DateTime dateTime = DateTime.MinValue;
					if(DateTime.TryParse(parameter.Values[i], _currentCulture, System.Globalization.DateTimeStyles.None, out dateTime))
						result[i] = dateTime.Ticks.ToString();
					else
						result[i] = parameter.Values[i] ?? "<<NULL>>";
				}
				else
					result[i] = RemoveAcent(parameter.Values[i]) ?? "<<NULL>>";
			}
			return result;
		}

		/// <summary>
		/// Recupera a consulta dos parametros informados.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="analyzer"></param>
		/// <returns></returns>
		private static Query GetQuery(Parameter[] parameters, ISchemeRepository schemeRepository, global::Lucene.Net.Analysis.Analyzer analyzer)
		{
			if(parameters == null)
				return null;
			var queries = new List<Query>();
			foreach (var i in parameters.Distinct(new Parameter.NameEqualityComparer()))
			{
				var index = schemeRepository.GetSchemaIndex(i.Name);
				string parserValue = null;
				switch(i.SearchType)
				{
				case SearchType.Equal:
					parserValue = "\"" + GetParameterValue(i, index) + "\"";
					break;
				case SearchType.Like:
					parserValue = i.Value;
					break;
				case SearchType.Between:
					var betweenValues = GetParameterValues(i, index);
					parserValue = string.Format("[{0} TO {1}]", betweenValues[0], betweenValues[1]);
					break;
				case SearchType.In:
					parserValue = string.Join(" OR ", GetParameterValues(i, index));
					break;
				case SearchType.Or:
					parserValue = string.Join(" OR ", GetParameterValues(i, index));
					break;
				case SearchType.Greater:
				case SearchType.GreaterEqual:
					parserValue = string.Format("[{0} TO *]", GetParameterValue(i, index));
					break;
				case SearchType.Less:
				case SearchType.LessEqual:
					parserValue = string.Format("[* TO {0}]", GetParameterValue(i, index));
					break;
				default:
					throw new InvalidOperationException(string.Format("Invalid SearchParameterType ({0})", i.SearchType));
				}
				var parser = new QueryParser(Version.LUCENE_29, i.Name, analyzer);
				queries.Add(parser.Parse(parserValue));
			}
			if(queries.Count > 0)
			{
				var query = new global::Lucene.Net.Search.BooleanQuery();
				foreach (var i in queries)
					query.Add(new global::Lucene.Net.Search.BooleanClause(i, global::Lucene.Net.Search.BooleanClause.Occur.MUST));
				return query;
			}
			return null;
		}

		/// <summary>
		/// Recupera o filtro dos parametros.
		/// </summary>
		/// <param name="filters"></param>
		/// <param name="schemeRepository"></param>
		/// <param name="analyzer"></param>
		/// <returns></returns>
		private static Filter GetFilter(FilterParameter[] filters, ISchemeRepository schemeRepository, StandardAnalyzer analyzer)
		{
			if(filters != null && filters.Length > 0)
				return new QueryWrapperFilter(GetQuery(filters, schemeRepository, analyzer));
			return null;
		}

		/// <summary>
		/// Cria o escritor para o indice.
		/// </summary>
		/// <returns></returns>
		private IndexWriterAccess CreateIndexWriter()
		{
			_createWriterResetEvent.WaitOne();
			_createWriterResetEvent.Reset();
			var access = new IndexWriterAccess(new IndexWriter(IndexDirectory, new StandardAnalyzer(Version.LUCENE_29), !_writerCreated, IndexWriter.MaxFieldLength.LIMITED), _createWriterResetEvent);
			_writerCreated = true;
			access.Disposing += (sender, e) =>  {
				if(_reader != null)
					_reader.Close();
				_reader = IndexReader.Open(IndexDirectory, false);
			};
			return access;
		}

		/// <summary>
		/// Instancia do leitor dos dados
		/// </summary>
		private IndexReader CreateReader()
		{
			_createWriterResetEvent.WaitOne();
			return _reader.Clone(true);
		}

		/// <summary>
		/// Prepara os dados para a pesquisa.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="filters"></param>
		/// <param name="options"></param>
		/// <param name="query"></param>
		/// <param name="filter"></param>
		/// <param name="sortFields"></param>
		private void PrepareSearch(string text, FilterParameter[] filters, SearchOptions options, out Query query, out Filter filter, out global::Lucene.Net.Search.SortField[] sortFields)
		{
			if(options == null)
				throw new ArgumentNullException("options");
			ValidateOptions(options);
			var analyzer = new StandardAnalyzer(Version.LUCENE_29, Stopwords.PORTUGUESE_SET);
			var parser = new QueryParser(Version.LUCENE_29, "FullText", analyzer);
			var terms = new List<string>();
			using (var stringReader = new System.IO.StringReader(RemoveAcent(text)))
			{
				var tokenStream = analyzer.TokenStream("FullText", stringReader);
				var token = tokenStream.Next();
				while (token != null)
				{
					var term = token.TermText();
					terms.Add(term);
					token = tokenStream.Next();
				}
			}
			var segmentsTerms = new List<string>();
			var segmentsSortTerms = new List<string>();
			foreach (var i in terms)
				if(_segments.ContainsKey(i) && !segmentsTerms.Contains(i))
					segmentsTerms.Add(i);
				else
					segmentsSortTerms.Add(i);
			if(segmentsTerms.Count > 0)
			{
				var booleanQuery = new BooleanQuery();
				foreach (var s in segmentsTerms)
					booleanQuery.Add(parser.Parse(s), BooleanClause.Occur.MUST);
				query = booleanQuery;
				filter = GetFilter(filters, _structRepository, analyzer);
				sortFields = new global::Lucene.Net.Search.SortField[] {
					new global::Lucene.Net.Search.SortField("FullText", new FullTextFieldComparatorSource(analyzer, _dataRepository, segmentsSortTerms), true)
				};
				return;
			}
			var queries = new List<Query>();
			foreach (var i in terms)
				queries.Add(parser.Parse(i));
			if(queries.Count == 0)
				query = parser.Parse(RemoveAcent(text));
			else
			{
				var booleanQuery = new BooleanQuery();
				foreach (var q in queries)
					booleanQuery.Add(q, BooleanClause.Occur.MUST);
				query = booleanQuery;
			}
			filter = null;
			sortFields = null;
			if(query != null)
				filter = GetFilter(filters, _structRepository, analyzer);
			else
				query = GetQuery(filters, _structRepository, analyzer);
		}

		/// <summary>
		/// Prepara os parametros para a pesquisa.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="filters"></param>
		/// <param name="options"></param>
		/// <param name="query"></param>
		/// <param name="filter"></param>
		private void PrepareSearch(SearchParameter[] parameters, FilterParameter[] filters, SearchOptions options, out Query query, out Filter filter)
		{
			if(options == null)
				throw new ArgumentNullException("options");
			ValidateOptions(options);
			var analyzer = new StandardAnalyzer(Version.LUCENE_29, Stopwords.PORTUGUESE_SET);
			var queries = new List<Query>();
			query = GetQuery(parameters, _structRepository, analyzer);
			filter = null;
			if(query != null)
				filter = GetFilter(filters, _structRepository, analyzer);
			else
				query = GetQuery(filters, _structRepository, analyzer);
		}

		/// <summary>
		/// Realiza a pesquisa a recupera a quantidade de itens do resultado.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		private int SearchCount(Query query, Filter filter)
		{
			var reader = CreateReader();
			var searcher = new IndexSearcher(reader);
			try
			{
				if(query != null)
				{
					var weight = query.Weight(searcher);
					var collector = TopFieldCollector.create(new global::Lucene.Net.Search.Sort(), 1, true, false, false, !weight.ScoresDocsOutOfOrder());
					searcher.Search(weight, filter, collector);
					return collector.GetTotalHits();
				}
			}
			finally
			{
				searcher.Close();
				reader.Close();
			}
			return 0;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			if(!_initialized)
			{
				_initialized = true;
				try
				{
					_dataRepository.Initialize();
					_structRepository.Initialize();
					_statisticsManager.Initialize();
					var analyzer = new StandardAnalyzer(Version.LUCENE_29, Stopwords.PORTUGUESE_SET);
					lock (_processElementsLock)
					{
						using (var writer = CreateIndexWriter())
						{
							using (var enumerator = _dataRepository.GetElements().GetEnumerator())
							{
								while (enumerator.MoveNext())
									AddElement(writer.Writer, analyzer, enumerator.Current);
							}
							_removedElementsCutoffDate = DateTime.Now;
							_newElementsCutoffDate = DateTime.Now;
						}
					}
					_syncService.Start();
				}
				catch(Exception ex)
				{
					_initialized = false;
					throw ex;
				}
			}
		}

		/// <summary>
		/// Remove o acento do texto.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static string RemoveAcent(string text)
		{
			if(string.IsNullOrEmpty(text))
				return text;
			char[] acents = "áàâäãéèêëíìîïóòôöõúùûüçÁÀÂÄÃÉÈÊËÍÌÎÏÓÒÔÖÕÚÙÛÜÇ".ToCharArray();
			char[] noAcents = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC".ToCharArray();
			var result = "";
			for(int i = 0; i < text.Length; i++)
				if(acents.Contains(text[i]))
					result += noAcents[Array.FindIndex(acents, c => c == text[i])];
				else
					result += text[i];
			return result;
		}

		/// <summary>
		/// Adiciona o elemento para o elemento.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="analyzer"></param>
		/// <param name="element"></param>
		private void AddElement(IndexWriter writer, global::Lucene.Net.Analysis.Analyzer analyzer, Element element)
		{
			var document = new Document();
			var channel = _structRepository.GetChannel(element.ChannelId);
			if(channel == null)
				throw new InvalidOperationException(string.Format("Not found channel by channel id '{0}'", element.ChannelId));
			var random = new Random();
			document.Add(new Field("Uid", element.Uid.ToString(), Field.Store.YES, Field.Index.ANALYZED));
			document.Add(new Field("test", "1", Field.Store.NO, Field.Index.ANALYZED));
			var fieldsCount = 0;
			foreach (var index in channel.Indexes)
			{
				if(index.Name.ToLower() == "hasimage")
				{
					var valueIndex = channel.GetIndexValue(element, "HasImage");
					if(valueIndex.ToLower() == "true")
						document.Add(new Field("Rnd", random.Next(10000).ToString(), Field.Store.YES, Field.Index.ANALYZED));
				}
				if(index.FieldSchema.Length == 1 && (index.FieldSchema[0].Type == typeof(DateTime) || index.FieldSchema[0].Type == typeof(DateTime?)))
				{
					var valueDateTime = channel.GetFieldObjectValue(element, index.FieldSchema[0]);
					string valueString = "";
					if(valueDateTime != null)
					{
						if(valueDateTime is DateTime?)
							valueString = ((DateTime?)valueDateTime).GetValueOrDefault().Ticks.ToString();
						else
							valueString = ((DateTime)valueDateTime).Ticks.ToString();
					}
					document.Add(new Field(index.Name + "Ticks", valueString, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES));
				}
				var value = channel.GetIndexValue(element, index.Name);
				fieldsCount++;
				var field = new Field(index.Name, RemoveAcent(value) ?? "", Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES);
				document.Add(field);
			}
			var fullTextItems = new List<string>();
			foreach (var field in channel.Fields)
			{
				if(field.IsSegmentValue)
				{
					lock (_segments)
					{
						var value = channel.GetFieldValue(element, field);
						if(!string.IsNullOrEmpty(value))
						{
							value = RemoveAcent(value);
							using (var reader = new System.IO.StringReader(value))
							{
								var tokenStream = analyzer.TokenStream(field.Name, reader);
								List<string> terms = new List<string>();
								var token = tokenStream.Next();
								while (token != null)
								{
									var term = token.TermText();
									if(!terms.Contains(term))
										terms.Add(term);
									token = tokenStream.Next();
								}
								foreach (var i in terms)
								{
									List<int> segmentItems = null;
									if(_segments.TryGetValue(i, out segmentItems))
									{
										var index = segmentItems.BinarySearch(element.Uid);
										if(index < 0)
										{
											segmentItems.Insert(~index, element.Uid);
										}
									}
									else
										_segments.Add(i, new List<int> {
											element.Uid
										});
								}
							}
						}
					}
				}
				if(field.IncludeInFullText)
				{
					var fullTextValue = channel.GetFieldFullTextValue(element, field.Name);
					if(!string.IsNullOrEmpty(fullTextValue))
					{
						fullTextValue = RemoveAcent(fullTextValue);
						var terms = new List<string>();
						using (var stringReader = new System.IO.StringReader(fullTextValue))
						{
							var tokenStream = analyzer.TokenStream("FullText", stringReader);
							var token = tokenStream.Next();
							while (token != null)
							{
								var term = token.TermText();
								terms.Add(term);
								token = tokenStream.Next();
							}
						}
						foreach (var i in terms)
						{
							var index = fullTextItems.BinarySearch(i);
							if(index < 0)
							{
								fullTextItems.Insert(~index, i);
							}
						}
					}
				}
			}
			if(fieldsCount > 0 || fullTextItems.Count > 0)
			{
				if(fullTextItems.Count > 0)
				{
					fullTextItems.Sort();
					document.Add(new Field("FullText", string.Join(" ", fullTextItems.ToArray()), Field.Store.YES, Field.Index.ANALYZED));
				}
				writer.AddDocument(document, analyzer);
			}
			_dataRepository.Add(element);
		}

		/// <summary>
		/// Realiza uma pesquisa.
		/// </summary>
		/// <param name="parameters">Parametros que serão usados no pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação do resultado.</param>
		/// <returns></returns>
		public ISearchResult Search(SearchParameter[] parameters, FilterParameter[] filters, SearchOptions options, Sort sort)
		{
			Query query;
			Filter filter;
			PrepareSearch(parameters, filters, options, out query, out filter);
			var luceneSort = Convert(sort, null);
			return new SearchResult(query, filter, CreateReader, luceneSort, options, _dataRepository, _structRepository, (sender, e) =>  {
				IncrementCountWords(parameters);
			});
		}

		/// <summary>
		/// Realiza um pesquisa no sistema e recupera a quantidade do resultado.
		/// </summary>
		/// <param name="parameters">Parametros que serão usados no pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		public int SearchCount(SearchParameter[] parameters, FilterParameter[] filters, SearchOptions options)
		{
			Query query;
			Filter filter;
			PrepareSearch(parameters, filters, options, out query, out filter);
			return SearchCount(query, filter);
		}

		/// <summary>
		/// Realiza uma pesquisa no sistema.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="options"></param>
		/// <param name="filters"></param>
		/// <param name="sort"></param>
		/// <returns></returns>
		public ISearchResult Search(string text, FilterParameter[] filters, SearchOptions options, Sort sort)
		{
			Query query;
			Filter filter;
			global::Lucene.Net.Search.SortField[] sortFields;
			text = RemoveAcent(text);
			PrepareSearch(text, filters, options, out query, out filter, out sortFields);
			global::Lucene.Net.Search.Sort luceneSort = Convert(sort, sortFields);
			return new SearchResult(query, filter, CreateReader, luceneSort, options, _dataRepository, _structRepository, (sender, e) =>  {
				IncrementCountWords(text, filters);
			});
		}

		/// <summary>
		/// Realiza uma pesquisa no sistema e recupera a quantidade do resultado.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="filters"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public int SearchCount(string text, FilterParameter[] filters, SearchOptions options)
		{
			Query query;
			Filter filter;
			global::Lucene.Net.Search.SortField[] sortFields;
			PrepareSearch(text, filters, options, out query, out filter, out sortFields);
			return SearchCount(query, filter);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_reader != null)
				_reader.Close();
			_syncService.Dispose();
		}

		/// <summary>
		/// Processa os novos elementos que devem ser carregados para o pesquisador.
		/// </summary>
		public void ProcessNewElements()
		{
			if(!_initialized)
				Initialize();
			lock (_processElementsLock)
			{
				var startDate = DateTime.Now;
				IndexWriterAccess writerAccess = null;
				var analyzer = new StandardAnalyzer(Version.LUCENE_29, Stopwords.PORTUGUESE_SET);
				var exceptionQueue = new Queue<Exception>();
				IEnumerator<Element> elementsEnumerator = null;
				try
				{
					try
					{
						elementsEnumerator = _repositoryLoader.GetElements(_newElementsCutoffDate).GetEnumerator();
					}
					catch(Exception ex)
					{
						throw new Exception(string.Format("Fail on get enumerator for elements from repository loader using date '{0}'", _newElementsCutoffDate.ToString()), ex);
					}
					while (true)
					{
						try
						{
							if(!elementsEnumerator.MoveNext())
								break;
						}
						catch(Exception ex)
						{
							throw new Exception("Fail on get next elemento from elementsEnumerator", ex);
						}
						var element = elementsEnumerator.Current;
						try
						{
							if(writerAccess == null)
								writerAccess = CreateIndexWriter();
							writerAccess.Writer.DeleteDocuments(new Term("Uid", element.Uid.ToString()));
							AddElement(writerAccess.Writer, analyzer, element);
						}
						catch(Exception ex)
						{
							exceptionQueue.Enqueue(ex);
						}
					}
				}
				finally
				{
					if(elementsEnumerator != null)
						elementsEnumerator.Dispose();
					if(writerAccess != null)
						writerAccess.Dispose();
				}
				if(exceptionQueue.Count > 0)
				{
					throw new SearchEngineAggregateException(string.Format("there were {0} exceptions, the first '{1}'", exceptionQueue.Count, exceptionQueue.Peek().Message), exceptionQueue);
				}
				_newElementsCutoffDate = startDate;
			}
		}

		/// <summary>
		/// Processa os elementos que devem ser removidos do pesquisador.
		/// </summary>
		public void ProcessRemovedElements()
		{
			if(!_initialized)
				Initialize();
			lock (_processElementsLock)
			{
				Exception exception = null;
				var startDate = DateTime.Now;
				if(_repositoryLoader.CheckClearAllElements())
				{
					using (var writerAccess = CreateIndexWriter())
						writerAccess.Writer.DeleteAll();
					_repositoryLoader.ConfirmClearAllElements();
					_dataRepository.Clear();
					_newElementsCutoffDate = DateTime.MinValue;
				}
				else
				{
					IndexWriterAccess writerAccess = null;
					try
					{
						foreach (var uid in _repositoryLoader.GetRemoved(_removedElementsCutoffDate))
						{
							try
							{
								if(writerAccess == null)
									writerAccess = CreateIndexWriter();
								writerAccess.Writer.DeleteDocuments(new Term("Uid", uid.ToString()));
								_dataRepository.Remove(uid);
							}
							catch(Exception ex)
							{
								exception = ex;
							}
						}
					}
					finally
					{
						if(writerAccess != null)
							writerAccess.Dispose();
					}
				}
				if(exception != null)
					throw exception;
				_removedElementsCutoffDate = startDate;
			}
		}

		/// <summary>
		/// Classe responsável pelo gerenciamento
		/// da escrita no indice.
		/// </summary>
		class IndexWriterAccess : IDisposable
		{
			private IndexWriter _writer;

			private System.Threading.ManualResetEvent _resetEvent;

			/// <summary>
			/// Evento acionado quando a instancia estive sendo liberada.
			/// </summary>
			public event EventHandler Disposing;

			/// <summary>
			/// Recupera a instancia do escritor.
			/// </summary>
			public IndexWriter Writer
			{
				get
				{
					return _writer;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="writer"></param>
			/// <param name="resetEvent"></param>
			public IndexWriterAccess(IndexWriter writer, System.Threading.ManualResetEvent resetEvent)
			{
				_writer = writer;
				_resetEvent = resetEvent;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_writer.Flush(true, true, true);
				_writer.Commit();
				_writer.ExpungeDeletes(true);
				try
				{
					if(Disposing != null)
						Disposing(this, EventArgs.Empty);
				}
				finally
				{
					_writer.Close();
					_resetEvent.Set();
				}
			}
		}
	}
}
