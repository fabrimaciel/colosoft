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
using Colosoft.Caching;

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Classe responsável pela carga dos dados do cache.
	/// </summary>
	public class CacheLoader : ICacheLoader
	{
		private IDataCacheManager _manager;

		private IDataEntryDownloader _downloader;

		private IDataEntriesRepository _repository;

		private Colosoft.Logging.ILogger _logger;

		private ItemsLoader _itemsLoader;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private AggregateCacheLoaderObserver _observer = new AggregateCacheLoaderObserver();

		private Colosoft.Query.IRecordKeyFactory _cacheItemKeyFactory;

		/// <summary>
		/// Nomes dos tipos que devem ser carregados.
		/// </summary>
		private Queue<Colosoft.Reflection.TypeName> _typeNames = new Queue<Colosoft.Reflection.TypeName>();

		/// <summary>
		/// Identifica se a carga foi iniciada.
		/// </summary>
		private bool _started = false;

		private System.Threading.ManualResetEvent _allDone = new System.Threading.ManualResetEvent(false);

		private DataEntryDownloadCompletedEventArgs _downloadResult;

		/// <summary>
		/// Evento acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		public event CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Instancia do gerador de chave
		/// </summary>
		public Colosoft.Query.IRecordKeyFactory RecordKeyFactory
		{
			get
			{
				if(Colosoft.Query.RecordKeyFactory.Instance == null)
				{
					if(_cacheItemKeyFactory == null)
						_cacheItemKeyFactory = new Data.Schema.RecordKeyFactory(_typeSchema);
					return _cacheItemKeyFactory;
				}
				return Colosoft.Query.RecordKeyFactory.Instance;
			}
		}

		/// <summary>
		/// Agregador dos observers da instancia.
		/// </summary>
		public AggregateCacheLoaderObserver Observer
		{
			get
			{
				return _observer;
			}
			set
			{
				if(_observer != value)
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CacheLoader()
		{
		}

		/// <summary>
		/// Método acionado quando o download for completado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Downloader_DownloadCompleted(object sender, Colosoft.Net.DownloadCompletedEventArgs e)
		{
			try
			{
				_downloadResult = (DataEntryDownloadCompletedEventArgs)e;
				_observer.OnDownloadCompleted(e);
				if(_downloadResult.Error == null && _downloadResult.Package != null)
				{
					foreach (var i in _downloadResult.Package.GetDataEntries())
					{
						try
						{
							_repository.Insert(i.Item1, i.Item2);
						}
						catch(Exception)
						{
						}
					}
				}
			}
			finally
			{
				_allDone.Set();
			}
		}

		/// <summary>
		/// Método acionado
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Downloader_ProgressChanged(object sender, Colosoft.Net.DownloadProgressChangedEventArgs e)
		{
			_observer.OnDownloadProgressChanged(e);
		}

		/// <summary>
		/// Inicializa a carga do cache.
		/// </summary>
		private void Start(object userState)
		{
			Observer.OnDoWork(userState);
			_downloader.Clear();
			var localVersions = _repository.GetEntryVersions();
			var versions = new List<DataEntryVersion>();
			var total = 0;
			var typeNames = new List<Reflection.TypeName>();
			foreach (var i in _typeNames.Concat(_manager).OrderBy(f => f.FullName).ToArray())
			{
				var version = localVersions.Where(f => Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(f.TypeName, i)).FirstOrDefault();
				if(version == null)
					version = new DataEntryVersion {
						TypeName = i,
						Version = DateTime.MinValue
					};
				_downloader.Add(version);
				typeNames.Add(i);
				total++;
			}
			_downloader.RunAsync(userState);
			_allDone.WaitOne();
			_itemsLoader = new ItemsLoader(this, typeNames);
			_started = true;
		}

		/// <summary>
		/// Método acionado quando ocorrer um erro no processamento.
		/// </summary>
		/// <param name="error"></param>
		protected virtual void OnLoadProcessingError(Exception error)
		{
			if(LoadProcessingError != null)
				LoadProcessingError(this, new CacheErrorEventArgs(error));
		}

		/// <summary>
		/// Adiciona o nome do tipo para ser carregado.
		/// </summary>
		/// <param name="typeName"></param>
		public void Add(Colosoft.Reflection.TypeName typeName)
		{
			typeName.Require("typeName").NotNull();
			_typeNames.Enqueue(typeName);
		}

		/// <summary>
		/// Inicializa a isntancia.
		/// </summary>
		/// <param name="parameters">Parametros de configuração.</param>
		public void Init(System.Collections.IDictionary parameters)
		{
			var manager = parameters["manager"] as IDataCacheManager;
			var downloader = parameters["downloader"] as IDataEntryDownloader;
			var repository = parameters["repository"] as IDataEntriesRepository;
			var logger = parameters["logger"] as Colosoft.Logging.ILogger;
			var typeSchema = parameters["typeSchema"] as Colosoft.Data.Schema.ITypeSchema;
			var cacheLoaderObserver = parameters["cacheLoaderObserver"] as ICacheLoaderObserver;
			manager.Require("manager").NotNull();
			downloader.Require("downloader").NotNull();
			repository.Require("repository").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			logger.Require("logger").NotNull();
			_manager = manager;
			_downloader = downloader;
			_repository = repository;
			_logger = logger;
			_typeSchema = typeSchema;
			if(cacheLoaderObserver != null)
				_observer += cacheLoaderObserver;
			downloader.DownloadCompleted += Downloader_DownloadCompleted;
			downloader.ProgressChanged += Downloader_ProgressChanged;
		}

		/// <summary>
		/// Carrega o próximo conjunto de dados do cache.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool LoadNext(ref System.Collections.Specialized.OrderedDictionary data, ref object index)
		{
			try
			{
				if(!_started)
					Start(null);
				Colosoft.Query.Record record = null;
				Colosoft.Query.IRecordKeyGenerator generator = null;
				while (true)
				{
					if(!_itemsLoader.MoveNext())
					{
						_itemsLoader.Dispose();
						_itemsLoader = null;
						_started = false;
						return false;
					}
					record = _itemsLoader.Current;
					generator = _itemsLoader.CurrentGenerator;
					if(record == null || generator == null)
						continue;
					break;
				}
				var key = generator.GetKey(record);
				var values = new object[record.FieldCount];
				for(var i = 0; i < record.FieldCount; i++)
					values[i] = record.GetValue(i);
				var cacheItem = new ProviderCacheItem(new CacheItemRecord(_itemsLoader.CurrentDataEntry.TypeName, values, record.Descriptor));
				cacheItem.AbsoluteExpiration = DateTime.MaxValue;
				data.Add(key, cacheItem);
				return true;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_itemsLoader != null)
			{
				_itemsLoader.Dispose();
				_itemsLoader = null;
			}
			_downloader.DownloadCompleted -= Downloader_DownloadCompleted;
			_downloader.ProgressChanged -= Downloader_ProgressChanged;
			_downloader.CancelAsync();
			if(_allDone != null)
			{
				_allDone.Dispose();
				_allDone = null;
			}
		}

		/// <summary>
		/// Classe responsável pela cargar dos itens para o cache.
		/// </summary>
		sealed class ItemsLoader : IEnumerator<Colosoft.Query.Record>
		{
			private IEnumerator<DataEntry> _dataEntryEnumerator;

			private IEnumerator<Colosoft.Query.Record> _recordEnumerator;

			private CacheLoader _loader;

			private DataEntry _currentDataEntry;

			private Colosoft.Query.Record _current;

			private Colosoft.Query.IRecordKeyGenerator _currentGenerator;

			private int _position = 0;

			private List<Reflection.TypeName> _typeNames;

			/// <summary>
			/// Instancia da atual entrada de dados.
			/// </summary>
			public DataEntry CurrentDataEntry
			{
				get
				{
					return _currentDataEntry;
				}
			}

			/// <summary>
			/// Instancia do atual gerador de chave.
			/// </summary>
			public Colosoft.Query.IRecordKeyGenerator CurrentGenerator
			{
				get
				{
					return _currentGenerator;
				}
			}

			/// <summary>
			/// Instancia do atual item.
			/// </summary>
			public Colosoft.Query.Record Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Instancia do atual item.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="loader"></param>
			/// <param name="typeNames">Nomes do tipos que serão carregados.</param>
			public ItemsLoader(CacheLoader loader, List<Reflection.TypeName> typeNames)
			{
				_typeNames = typeNames;
				_loader = loader;
				_dataEntryEnumerator = loader._repository.GetEntries().GetEnumerator();
			}

			/// <summary>
			/// Recupera o criador associado com tipo informado.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			private Colosoft.Query.IQueryResultObjectCreator GetObjectCreator(Type type)
			{
				return new Colosoft.Query.QueryResultObjectCreator(type);
			}

			/// <summary>
			/// Posiciona no próximo item.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				while (true)
				{
					if(_recordEnumerator == null)
					{
						if(!_dataEntryEnumerator.MoveNext())
						{
							_loader.Observer.OnCurrentProgressChanged(new System.ComponentModel.ProgressChangedEventArgs(100, null));
							return false;
						}
						else
						{
							_loader.Observer.OnStageChanged(new CacheLoaderStage("LoadingType", ResourceMessageFormatter.Create(() => Properties.Resources.CacheLoader_LoadingTypeName, _dataEntryEnumerator.Current.TypeName.FullName)));
							_loader.Observer.OnCurrentProgressChanged(new System.ComponentModel.ProgressChangedEventArgs(_typeNames.Count > 0 ? ((_position++) * 100 / _typeNames.Count) : 100, null));
						}
						_currentDataEntry = _dataEntryEnumerator.Current;
						if(_currentDataEntry == null)
							continue;
						_currentGenerator = _loader.RecordKeyFactory.CreateGenerator(_currentDataEntry.TypeName);
						_recordEnumerator = _currentDataEntry.GetRecords().GetEnumerator();
					}
					if(!_recordEnumerator.MoveNext())
					{
						_recordEnumerator = null;
						continue;
					}
					_current = _recordEnumerator.Current;
					return true;
				}
			}

			/// <summary>
			/// Reseta o navegador.
			/// </summary>
			public void Reset()
			{
				_dataEntryEnumerator.Reset();
				_recordEnumerator = null;
				_position = 0;
				if(_currentDataEntry != null)
				{
					_currentDataEntry.Dispose();
					_currentDataEntry = null;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_currentDataEntry != null)
				{
					_currentDataEntry.Dispose();
					_currentDataEntry = null;
				}
				_loader.Observer.OnLoadComplete(new ApplicationLoaderCompletedEventArgs(null, false, null, null, true));
			}
		}

		/// <summary>
		/// Implementação do comparador para os nomes do tipo.
		/// </summary>
		sealed class TypeNameEqualityComparer : IEqualityComparer<Colosoft.Reflection.TypeName>
		{
			public readonly static TypeNameEqualityComparer Instance = new TypeNameEqualityComparer();

			/// <summary>
			/// Compara as instancias informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(Reflection.TypeName x, Reflection.TypeName y)
			{
				return x != null && y != null && x.AssemblyQualifiedName == y.AssemblyQualifiedName;
			}

			/// <summary>
			/// Recupera o hash code do nome do tipo.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(Reflection.TypeName obj)
			{
				if(object.ReferenceEquals(obj, null))
					return 0;
				return obj.AssemblyQualifiedName.GetHashCode();
			}
		}
	}
}
