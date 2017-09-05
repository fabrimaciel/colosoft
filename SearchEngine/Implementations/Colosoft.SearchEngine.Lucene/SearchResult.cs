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
using Lucene.Net.Search;

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Argumentos do evento acionado quando o resultado da pesquisa for carregado.
	/// </summary>
	class SearchResultLoadEventArgs : EventArgs
	{
		/// <summary>
		/// Quantidade de itens do resultado.
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SearchResultLoadEventArgs(int count)
		{
			this.Count = count;
		}
	}
	/// <summary>
	/// Representa o resulta da pesquisa.
	/// </summary>
	class SearchResult : ISearchResult, IDisposable
	{
		/// <summary>
		/// Instancia da consulta.
		/// </summary>
		private global::Lucene.Net.Search.Query _query;

		private global::Lucene.Net.Search.Filter _filter;

		private Func<global::Lucene.Net.Index.IndexReader> _readerCreator;

		private global::Lucene.Net.Index.IndexReader _reader;

		private IDataRepository _dataRepository;

		private global::Lucene.Net.Search.Sort _sort;

		private SearchOptions _options;

		private global::Lucene.Net.Search.Searcher _searcher;

		private ISchemeRepository _structRepository;

		/// <summary>
		/// Evento acionado quando o resultado for carregado.
		/// </summary>
		private EventHandler<SearchResultLoadEventArgs> _loadedHandle;

		private List<SummaryResult> _summaries;

		private ChannelFields[] _channelFields;

		private int _count = -1;

		/// <summary>
		/// Armazena os elementos do resultado pontual.
		/// </summary>
		private Element[] _result;

		/// <summary>
		/// Opções da pesquisa.
		/// </summary>
		public SearchOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Pesquisador da instancia.
		/// </summary>
		private global::Lucene.Net.Search.Searcher Searcher
		{
			get
			{
				if(_searcher == null)
				{
					_reader = _readerCreator();
					_searcher = new IndexSearcher(_reader);
				}
				return _searcher;
			}
		}

		/// <summary>
		/// Instancia do repositório de dados.
		/// </summary>
		protected IDataRepository DataRepository
		{
			get
			{
				return _dataRepository;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="filter"></param>
		/// <param name="readerCreator"></param>
		/// <param name="sort"></param>
		/// <param name="options"></param>
		/// <param name="dataRepository"></param>
		/// <param name="structRepository"></param>
		/// <param name="loadedHandle"></param>
		internal SearchResult(global::Lucene.Net.Search.Query query, global::Lucene.Net.Search.Filter filter, Func<global::Lucene.Net.Index.IndexReader> readerCreator, global::Lucene.Net.Search.Sort sort, SearchOptions options, IDataRepository dataRepository, ISchemeRepository structRepository, EventHandler<SearchResultLoadEventArgs> loadedHandle)
		{
			_query = query;
			_filter = filter;
			_readerCreator = readerCreator;
			_sort = sort;
			_options = options;
			_dataRepository = dataRepository;
			_structRepository = structRepository;
			_loadedHandle = loadedHandle;
		}

		/// <summary>
		/// Evento acionado quando o resultado for carregado.
		/// </summary>
		protected void OnLoad()
		{
			if(_loadedHandle != null)
				_loadedHandle(this, new SearchResultLoadEventArgs(_count));
		}

		/// <summary>
		/// Campos dos canais usados pelos elementos.
		/// </summary>
		public ChannelFields[] ChannelFields
		{
			get
			{
				if(_query == null && (_channelFields == null || _channelFields.Length == 0))
				{
					var channels = _structRepository.GetChannels();
					var fields = new ChannelFields[channels.Count];
					var index = 0;
					foreach (var channel in channels.Where(f => f.Fields != null))
					{
						fields[index] = new ChannelFields(channel.ChannelId, channel.Fields.ToArray());
						index++;
					}
					_channelFields = fields;
				}
				else if(_query != null && _channelFields == null)
					ExecuteSearch(0);
				return _channelFields ?? new ChannelFields[0];
			}
		}

		/// <summary>
		/// Quantidade de elementos do resultado.
		/// </summary>
		public int Count
		{
			get
			{
				if(_count < 0)
					ExecuteSearch(0);
				return _count;
			}
		}

		/// <summary>
		/// Elementos do resultado.
		/// </summary>
		public IEnumerable<Element> Elements
		{
			get
			{
				if(_options.RandomResult)
				{
					if(_result == null)
						ExecuteSearch(0);
					return _result;
				}
				var hitsPerPage = _options.HitsPerPage > 0 ? _options.HitsPerPage : 50;
				return new Colosoft.Colletions.VirtualList<Element>(hitsPerPage, (sender, e) =>  {
					if(e.NeedItemsCount)
						return new Colosoft.Colletions.LoaderResult<Element>(new Element[0], Count);
					var items = new List<Element>();
					int pageIndex = (int)Math.Floor(e.StartRow / (double)e.PageSize);
					var result = ExecuteSearch(pageIndex);
					if(result is TopDocsCollector)
					{
						var topDocsCollector = (TopDocsCollector)result;
						var docs = topDocsCollector.TopDocs(e.StartRow);
						foreach (var i in docs.scoreDocs)
						{
							var doc = _searcher.Doc(i.doc);
							items.Add(GetElementFromDocument(doc));
						}
					}
					else if(result is SummaryCollectorWrapper)
					{
						var summaryCollector = (SummaryCollectorWrapper)result;
						items.AddRange(summaryCollector.GetResult().Skip(e.StartRow).Take(e.PageSize));
					}
					return new Colosoft.Colletions.LoaderResult<Element>(items, Count);
				}, this);
			}
		}

		/// <summary>
		/// Recupera os dados do elemento com base no documento.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		protected Element GetElementFromDocument(global::Lucene.Net.Documents.Document doc)
		{
			var uidField = doc.GetField("Uid");
			var uid = int.Parse(uidField.StringValue());
			return _dataRepository[uid];
		}

		/// <summary>
		/// Executa a pesquisa
		/// </summary>
		/// <returns></returns>
		private Collector ExecuteSearch(int pageIndex)
		{
			Collector collector = null;
			TopDocsCollector result = null;
			var totalHits = _options.RandomResult ? 1 : (pageIndex + 1) * (_options.HitsPerPage > 0 ? _options.HitsPerPage : 50);
			if(totalHits == 0)
				totalHits = 1;
			global::Lucene.Net.Search.Query query = _query;
			if(_sort != null && query == null)
			{
				var parser = new global::Lucene.Net.QueryParsers.QueryParser(global::Lucene.Net.Util.Version.LUCENE_29, "test", new global::Lucene.Net.Analysis.Standard.StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_29, Stopwords.PORTUGUESE_SET));
				query = parser.Parse("test:1");
				result = TopFieldCollector.create(_sort, totalHits, true, false, false, true);
			}
			if(query != null)
			{
				var weight = query.Weight(Searcher);
				collector = result = TopFieldCollector.create(_sort, totalHits, true, false, false, !weight.ScoresDocsOutOfOrder());
				if(_options.RandomResult)
					collector = new RandomCollectorWrapper(this, this.Searcher, result);
				else if(_channelFields == null || _channelFields.Length == 0)
					collector = new SummaryCollectorWrapper(this, this.Searcher, result);
				this.Searcher.Search(weight, _filter, collector);
				if(collector is SummaryCollectorWrapper)
				{
					var sCollector = ((SummaryCollectorWrapper)collector);
					_summaries = sCollector.GetSummaries().ToList();
					_channelFields = sCollector.GetChannelsFields().ToArray();
				}
				if(collector is RandomCollectorWrapper)
				{
					var wrapper = (RandomCollectorWrapper)collector;
					var wrapperResult = wrapper.GetResult();
					_result = wrapperResult is Element[] ? (Element[])wrapperResult : wrapperResult.ToArray();
					_count = _result.Length;
					OnLoad();
					return null;
				}
				_count = result.GetTotalHits();
				OnLoad();
				return result;
			}
			else
			{
				SummaryCollectorWrapper wrapper = null;
				if(_options.RandomResult)
				{
					wrapper = new RandomCollectorWrapper(this);
					_result = ((RandomCollectorWrapper)wrapper).GetResult().ToArray();
				}
				else
				{
					collector = wrapper = new SummaryCollectorWrapper(this);
					_channelFields = wrapper.GetChannelsFields().ToArray();
				}
				_count = wrapper.GetTotalHits();
				_summaries = wrapper.GetSummaries().ToList();
				OnLoad();
				return wrapper;
			}
		}

		/// <summary>
		/// Recupera os sumarios do resultado.
		/// </summary>
		public IEnumerable<SummaryResult> Summary
		{
			get
			{
				return _summaries;
			}
		}

		public void Dispose()
		{
			if(_searcher != null)
			{
				_reader.Close();
				_searcher.Close();
				_searcher = null;
			}
		}

		private static global::Lucene.Net.Util.PriorityQueue GetPriorityQueue(TopDocsCollector collector)
		{
			return (global::Lucene.Net.Util.PriorityQueue)typeof(TopDocsCollector).GetField("pq", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(collector);
		}

		/// <summary>
		/// Adaptador do coletor usado para construir o sumário do resultado.
		/// </summary>
		class SummaryCollectorWrapper : TopDocsCollector
		{
			class SummaryItemsInfo
			{
				/// <summary>
				/// Descrição do sumário.
				/// </summary>
				public string Description
				{
					get;
					set;
				}

				/// <summary>
				/// Itens do sumário.
				/// </summary>
				public List<SummaryItem> Items
				{
					get;
					set;
				}

				/// <summary>
				/// Construtor padrão.
				/// </summary>
				/// <param name="description"></param>
				/// <param name="items"></param>
				public SummaryItemsInfo(string description, List<SummaryItem> items)
				{
					this.Description = description;
					this.Items = items;
				}
			}

			protected TopDocsCollector _instance;

			protected SearchResult _searchResult;

			private global::Lucene.Net.Search.Searcher _searcher;

			private global::Lucene.Net.Index.IndexReader _currentReader;

			private Dictionary<string, SummaryItemsInfo> _summaries = new Dictionary<string, SummaryItemsInfo>();

			protected Dictionary<byte, ChannelFields> _channelsFields = new Dictionary<byte, ChannelFields>();

			/// <summary>
			/// Instancia adaptada.
			/// </summary>
			public TopDocsCollector Collector
			{
				get
				{
					return _instance;
				}
				set
				{
					_instance = value;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="searchResult"></param>
			/// <param name="searcher"></param>
			/// <param name="instance"></param>
			public SummaryCollectorWrapper(SearchResult searchResult, global::Lucene.Net.Search.Searcher searcher, TopDocsCollector instance) : base(SearchResult.GetPriorityQueue(instance))
			{
				_searchResult = searchResult;
				_instance = instance;
				_searcher = searcher;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="searchResult"></param>
			public SummaryCollectorWrapper(SearchResult searchResult) : base(null)
			{
				_searchResult = searchResult;
			}

			/// <summary>
			/// Preenche o sumário com os dados do elemento.
			/// </summary>
			/// <param name="channel"></param>
			/// <param name="element"></param>
			protected virtual void FillSummary(Channel channel, Element element)
			{
				if(_searchResult.Options.BuildSummary)
				{
					foreach (var i in channel.Indexes.Where(f => f.IncludeInSummary))
					{
						var value = channel.GetIndexValue(element, i.Name);
						SummaryItemsInfo summaryItemsInfo = null;
						if(!_summaries.TryGetValue(i.Name, out summaryItemsInfo))
						{
							summaryItemsInfo = new SummaryItemsInfo(i.Description, new List<SummaryItem>());
							_summaries.Add(i.Name, summaryItemsInfo);
						}
						var item = summaryItemsInfo.Items.Find(f => string.Compare(f.Label, value, true) == 0);
						if(item == null)
							summaryItemsInfo.Items.Add(new SummaryItem(value == "<<NULL>>" ? null : value, 1));
						else
							item.Count++;
					}
				}
			}

			/// <summary>
			/// Coleta os dados do elemento.
			/// </summary>
			/// <param name="channel"></param>
			/// <param name="element"></param>
			protected virtual void Collect(Channel channel, Element element)
			{
				FillSummary(channel, element);
			}

			/// <summary>
			/// Recupera o total de hits.
			/// </summary>
			/// <returns></returns>
			public virtual int GetTotalHits()
			{
				if(_instance != null)
					return _instance.GetTotalHits();
				else
					return _searchResult.DataRepository.Count;
			}

			/// <summary>
			/// Recupera os sumarios gerados.
			/// </summary>
			/// <returns></returns>
			public virtual IEnumerable<SummaryResult> GetSummaries()
			{
				if(_instance == null && _searchResult.Count == 0)
				{
					foreach (var i in _searchResult.DataRepository.GetElements())
						FillSummary(_searchResult._structRepository.GetChannel(i.ChannelId), i);
				}
				return _summaries.Select(f => new SummaryResult(f.Key, f.Value.Description, f.Value.Items.ToArray()));
			}

			/// <summary>
			/// Recupera os canais dos itens do resultado.
			/// </summary>
			/// <returns></returns>
			public IEnumerable<ChannelFields> GetChannelsFields()
			{
				return _channelsFields.Values;
			}

			/// <summary>
			/// Recupera o resultado do coletor.
			/// </summary>
			/// <returns></returns>
			public virtual IEnumerable<Element> GetResult()
			{
				return _searchResult.DataRepository.GetElements();
			}

			public override TopDocs NewTopDocs(ScoreDoc[] results, int start)
			{
				return _instance.NewTopDocs(results, start);
			}

			protected override void PopulateResults(ScoreDoc[] results, int howMany)
			{
				try
				{
					typeof(TopDocsCollector).GetMethod("PopulateResults", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(_instance, new object[] {
						results,
						howMany
					});
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}

			public override bool AcceptsDocsOutOfOrder()
			{
				return _instance.AcceptsDocsOutOfOrder();
			}

			public override void Collect(int doc)
			{
				_instance.Collect(doc);
				var document = _currentReader.Document(doc);
				var element = _searchResult.GetElementFromDocument(document);
				var channel = _searchResult._structRepository.GetChannel(element.ChannelId);
				if(!_channelsFields.ContainsKey(channel.ChannelId))
					_channelsFields.Add(channel.ChannelId, channel.Fields);
				Collect(channel, element);
			}

			public override void SetNextReader(global::Lucene.Net.Index.IndexReader reader, int docBase)
			{
				_currentReader = reader;
				_instance.SetNextReader(reader, docBase);
			}

			public override void SetScorer(Scorer scorer)
			{
				_instance.SetScorer(scorer);
			}
		}

		/// <summary>
		/// Adaptador do coletor usado para recupera o resultado aleatório.
		/// </summary>
		class RandomCollectorWrapper : SummaryCollectorWrapper
		{
			private IEnumerable<Element> _result;

			private List<Element> _elements = new List<Element>();

			private int _itemsCount;

			/// <summary>
			/// Quantidade de elemento do resultado.
			/// </summary>
			private int ElementCount
			{
				get
				{
					if(base._instance == null)
						return _searchResult.DataRepository.Count;
					return _elements.Count;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="searchResult"></param>
			/// <param name="searcher"></param>
			/// <param name="instance"></param>
			public RandomCollectorWrapper(SearchResult searchResult, global::Lucene.Net.Search.Searcher searcher, TopDocsCollector instance) : base(searchResult, searcher, instance)
			{
				_itemsCount = searchResult.Options.TotalHits;
			}

			/// <summary>
			/// Construtor usado para resultado direto.
			/// </summary>
			/// <param name="searchResult"></param>
			public RandomCollectorWrapper(SearchResult searchResult) : base(searchResult)
			{
				_itemsCount = searchResult.Options.TotalHits;
			}

			public override int GetTotalHits()
			{
				return ElementCount < _itemsCount ? ElementCount : _itemsCount;
			}

			/// <summary>
			/// Recupera o elemento pela posição informada.
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			private Element GetElementByPosition(int position)
			{
				if(_instance == null)
					return _searchResult.DataRepository.GetByPosition(position);
				return _elements[position];
			}

			/// <summary>
			/// Constrói o resultado.
			/// </summary>
			private void BuildResult()
			{
				if(_result == null)
				{
					var random = new Random();
					var positions = new int[ElementCount < _itemsCount ? ElementCount : _itemsCount];
					var currentResult = new Element[positions.Length];
					for(int i = 0; i < positions.Length; i++)
					{
						var pos = random.Next(0, ElementCount);
						if(i > 0 && positions.Take(i).Any(f => f == pos))
							i--;
						else
						{
							positions[i] = pos;
							var element = GetElementByPosition(pos);
							currentResult[i] = element;
							var channel = _searchResult._structRepository.GetChannel(element.ChannelId);
							if(_searchResult.Options.BuildSummary)
								FillSummary(_searchResult._structRepository.GetChannel(element.ChannelId), element);
						}
					}
					_result = currentResult;
					if(_channelsFields.Count == 0)
					{
						foreach (var element in _result)
						{
							var channel = _searchResult._structRepository.GetChannel(element.ChannelId);
							if(!_channelsFields.ContainsKey(channel.ChannelId))
								_channelsFields.Add(channel.ChannelId, channel.Fields);
						}
					}
				}
			}

			public override IEnumerable<SummaryResult> GetSummaries()
			{
				BuildResult();
				return base.GetSummaries();
			}

			/// <summary>
			/// Recupera o resultado do coletor.
			/// </summary>
			/// <returns></returns>
			public override IEnumerable<Element> GetResult()
			{
				BuildResult();
				return _result;
			}

			/// <summary>
			/// Coleta os elementos do resultado.
			/// </summary>
			/// <param name="channel"></param>
			/// <param name="element"></param>
			protected override void Collect(Channel channel, Element element)
			{
				_elements.Add(element);
			}
		}
	}
}
