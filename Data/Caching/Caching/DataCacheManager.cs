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
using Colosoft.Data.Schema;

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Classe responsável por gerenciar o cache de dados.
	/// </summary>
	public class DataCacheManager : Colosoft.Data.Caching.IDataCacheManager, Colosoft.Caching.ICacheObserver
	{
		/// <summary>
		/// Nome do tipos que deveram ser carregados para o cache.
		/// </summary>
		private List<Colosoft.Reflection.TypeName> _typeNames = new List<Colosoft.Reflection.TypeName>();

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Colosoft.Caching.Cache _cache;

		private object _objLock = new object();

		private Colosoft.Query.ISourceContext _sourceContext;

		private IDataEntryDownloader _dataEntryDownloader;

		private IDataEntriesRepository _dataEntriesRepository;

		private ICacheLoaderObserver _cacheLoaderObserver;

		private Colosoft.Logging.ILogger _logger;

		private bool _isInitialized;

		private Colosoft.Caching.Configuration.Dom.CacheLoader _cacheLoaderConfiguration;

		/// <summary>
		/// Evento acionado quando cache for completamenta carregado.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Evento acionado quando ocorre um erro na carga do cache.
		/// </summary>
		public event Colosoft.Caching.CacheErrorEventHandler LoadError;

		/// <summary>
		/// Evento acionado quando ocorreu um erro no processamento da carga do cache.
		/// </summary>
		public event Colosoft.Caching.CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Evento acionado quando ocorrer um erro ao inserir uma entrada no cache.
		/// </summary>
		public event Colosoft.Caching.CacheInsertEntryErrorHandler InsertEntryError;

		/// <summary>
		/// Instancia da atual configuração do cacheLoader.
		/// </summary>
		protected Colosoft.Caching.Configuration.Dom.CacheLoader CacheLoaderConfiguration
		{
			get
			{
				return _cacheLoaderConfiguration;
			}
			set
			{
				_cacheLoaderConfiguration = value;
			}
		}

		/// <summary>
		/// Instancia do cache de dados.
		/// </summary>
		public Colosoft.Caching.Cache Cache
		{
			get
			{
				return _cache;
			}
		}

		/// <summary>
		/// Identifica se a instancia foi inicializada.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sourceContext">Contexto de origem que será usado para fazer as consulta no banco de dados.</param>
		/// <param name="typeSchema"></param>
		/// <param name="dataEntryDownloader">Instancia do downloader da entradas de dados.</param>
		/// <param name="dataEntriesRepository">Instancia do repositório das entradas de dados.</param>
		/// <param name="cacheLoaderObverser">Observer o loader.</param>
		/// <param name="logger"></param>
		public DataCacheManager(Colosoft.Query.ISourceContext sourceContext, ITypeSchema typeSchema, IDataEntryDownloader dataEntryDownloader, IDataEntriesRepository dataEntriesRepository, ICacheLoaderObserver cacheLoaderObverser, Colosoft.Logging.ILogger logger)
		{
			sourceContext.Require("sourceContext").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			dataEntryDownloader.Require("dataEntryDownloader").NotNull();
			dataEntriesRepository.Require("dataEntriesRepository").NotNull();
			cacheLoaderObverser.Require("cacheLoaderObserver").NotNull();
			logger.Require("logger").NotNull();
			_sourceContext = sourceContext;
			_typeSchema = typeSchema;
			_dataEntryDownloader = dataEntryDownloader;
			_dataEntriesRepository = dataEntriesRepository;
			_cacheLoaderObserver = cacheLoaderObverser;
			_logger = logger;
		}

		/// <summary>
		/// Configura o cache para trabalhar com 
		/// os arquivos de dados que são armazenados
		/// local.
		/// </summary>
		public void ConfigureLocalCache()
		{
			CreateLocalCache();
		}

		/// <summary>
		/// Configura o cache para trabalhar os
		/// dados carregados diretamento do servidor.
		/// </summary>
		public void ConfigureServerCache()
		{
			CreateServerCache();
		}

		/// <summary>
		/// Registra o tipo que terá seus dados carregados para o cache na inicialização.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public virtual IDataCacheManager Register<T>()
		{
			return Register(typeof(T));
		}

		/// <summary>
		/// Registra o tipo que terá seus dados carregados para o cache na inicialização.
		/// </summary>
		/// <param name="type"></param>
		public virtual IDataCacheManager Register(Type type)
		{
			type.Require("type").NotNull();
			return Register(new Colosoft.Reflection.TypeName(type.AssemblyQualifiedName));
		}

		/// <summary>
		/// Registra o nome do tipo que terá seus dados carregados para o cache na inicialização.
		/// </summary>
		/// <param name="typeName"></param>
		public virtual IDataCacheManager Register(Reflection.TypeName typeName)
		{
			typeName.Require("typeFullName").NotNull();
			lock (_objLock)
			{
				var index = _typeNames.BinarySearch(typeName, Reflection.TypeName.TypeNameFullNameComparer.Instance);
				if(index < 0)
					_typeNames.Insert(~index, typeName);
			}
			return this;
		}

		/// <summary>
		/// Remove o registro do tipo para ser carregado para o cache.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será removido.</param>
		/// <returns></returns>
		public IDataCacheManager Unregister(Colosoft.Reflection.TypeName typeName)
		{
			typeName.Require("typeFullName").NotNull();
			lock (_objLock)
			{
				var index = _typeNames.FindIndex(f => Reflection.TypeName.TypeNameFullNameComparer.Instance.Equals(f, typeName));
				if(index >= 0)
					_typeNames.RemoveAt(index);
			}
			return this;
		}

		/// <summary>
		/// Recarrega os dados dos tipos informados para o cache.
		/// </summary>
		/// <param name="typeNames"></param>
		/// <returns></returns>
		public RealoadDataCacheResult Reload(Colosoft.Reflection.TypeName[] typeNames)
		{
			if(CacheLoaderConfiguration == null)
				throw new InvalidOperationException("CacheLoaderConfiguration undefined.");
			var loaderProvider = CacheLoaderConfiguration.Provider;
			Colosoft.Caching.ICacheLoader cacheLoader;
			try
			{
				var cacheLoaderType = Type.GetType(string.Format("{0}, {1}", loaderProvider.ClassName, loaderProvider.AssemblyName));
				cacheLoader = (Colosoft.Caching.ICacheLoader)Activator.CreateInstance(cacheLoaderType);
			}
			catch(InvalidCastException ex)
			{
				throw new Colosoft.Caching.Exceptions.ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_ICacheLoaderNotImplemented).Format(), ex);
			}
			catch(Exception ex)
			{
				throw new Colosoft.Caching.Exceptions.ConfigurationException(ex.Message, ex);
			}
			var resultErrors = new List<RealoadDataCacheResult.Error>();
			var typesMetadata = new List<ITypeMetadata>();
			foreach (var i in typeNames)
			{
				var metadata = _typeSchema.GetTypeMetadata(i.FullName);
				if(metadata == null)
				{
					resultErrors.Add(new RealoadDataCacheResult.Error {
						Type = i,
						Exception = new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheManager_Reload_TypeNotFound, i.FullName))
					});
				}
				else
					typesMetadata.Add(metadata);
			}
			var observerManager = Colosoft.Query.RecordObserverManager.Instance;
			for(var i = 0; i < typesMetadata.Count; i++)
			{
				var metadata = typesMetadata[i];
				var typeName = metadata.GetTypeName();
				Colosoft.Caching.Queries.QueryResultSet queryResult = null;
				var queryInfo = new Colosoft.Query.QueryInfo() {
					Entities = new Query.EntityInfo[] {
						new Query.EntityInfo(typeName.FullName)
					}
				};
				try
				{
					queryResult = _cache.Search(string.Format("select {0}.{1}", metadata.Namespace, metadata.Name), null);
				}
				catch(Exception ex)
				{
					resultErrors.Add(new RealoadDataCacheResult.Error {
						Type = Colosoft.Data.Schema.TypeSchemaExtensions.GetTypeName(metadata),
						Exception = new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheManager_Reload_QueryItemsToDelete, metadata.FullName), ex)
					});
					typesMetadata.RemoveAt(i--);
					continue;
				}
				var recordKeys = new Dictionary<string, Colosoft.Query.RecordKey>();
				using (var recordEnumerator = new Colosoft.Caching.Queries.QueryResultSetRecordEnumerator(_typeSchema, Cache, queryResult, queryInfo))
				{
					while (recordEnumerator.MoveNext())
						recordKeys.Add(recordEnumerator.CurrentKey is string ? (string)recordEnumerator.CurrentKey : (recordEnumerator.CurrentKey ?? "").ToString(), Colosoft.Query.RecordKeyFactory.Instance.Create(typeName, recordEnumerator.Current));
				}
				var cacheLoaderParameters = new System.Collections.Hashtable();
				cacheLoaderParameters.Add("sourceContext", _sourceContext);
				cacheLoaderParameters.Add("manager", this);
				cacheLoaderParameters.Add("logger", _logger);
				cacheLoaderParameters.Add("typeMetadata", null);
				cacheLoaderParameters.Add("cacheLoaderObserver", _cacheLoaderObserver);
				cacheLoaderParameters.Add("typesMetadata", new List<ITypeMetadata> {
					metadata
				});
				if(cacheLoader is DataCacheLoader)
					((DataCacheLoader)cacheLoader).Observer.OnTotalProgressChanged(new System.ComponentModel.ProgressChangedEventArgs(i * 100 / typesMetadata.Count, null));
				cacheLoader.Init(cacheLoaderParameters);
				using (var startupLoader = new Colosoft.Caching.Loaders.CacheStartupLoader(new System.Collections.Hashtable(), Cache, _logger))
				{
					var isNewEntry = false;
					startupLoader.Initialize(cacheLoader);
					startupLoader.InsertingEntry += (sender, e) =>  {
						var key = e.Key is string ? (string)e.Key : (e.Key ?? "").ToString();
						if(recordKeys.Remove(key))
						{
							isNewEntry = false;
							Cache.Remove(key, new Colosoft.Caching.OperationContext(Colosoft.Caching.OperationContextFieldName.OperationType, Colosoft.Caching.OperationContextOperationType.CacheOperation));
						}
						else
							isNewEntry = true;
					};
					startupLoader.InsertedEntry += (sender, e) =>  {
						if(e.Value is Colosoft.Query.IRecord)
						{
							var record = e.Value as Colosoft.Query.IRecord;
							var recordKey = Colosoft.Query.RecordKeyFactory.Instance.Create(typeName, record);
							if(!isNewEntry)
							{
								var notifier = observerManager.GetRecordChangedNotifier(typeName, recordKey);
								if(notifier.IsValid)
									notifier.Notify(record);
							}
							else
								observerManager.NotifyRecordsInserted(typeName, new Query.IRecord[] {
									record
								});
						}
					};
					startupLoader.LoadCache();
				}
				foreach (var key in recordKeys)
				{
					try
					{
						Cache.Remove(key.Key, new Colosoft.Caching.OperationContext(Colosoft.Caching.OperationContextFieldName.OperationType, Colosoft.Caching.OperationContextOperationType.CacheOperation));
					}
					catch(Exception ex)
					{
						resultErrors.Add(new RealoadDataCacheResult.Error {
							Type = Colosoft.Data.Schema.TypeSchemaExtensions.GetTypeName(metadata),
							Exception = new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheManager_Reload_ClearItems, metadata.FullName), ex)
						});
						typesMetadata.RemoveAt(i--);
						break;
					}
					observerManager.NotifyRecordDeleted(typeName, new Colosoft.Query.RecordKey[] {
						key.Value
					});
				}
			}
			return new RealoadDataCacheResult {
				Success = resultErrors.Count == 0,
				Errors = resultErrors.ToArray()
			};
		}

		/// <summary>
		/// Inicializa o processo assincrono que recarrega os dados para o cache.
		/// </summary>
		/// <param name="typeNames">Nome dos tipos que serão processados.</param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IAsyncResult BeginReload(Colosoft.Reflection.TypeName[] typeNames, AsyncCallback callback, object state)
		{
			var asyncResult = new Colosoft.Threading.AsyncResult<RealoadDataCacheResult>(callback, state);
			var args = new object[] {
				typeNames,
				asyncResult
			};
			if(!System.Threading.ThreadPool.QueueUserWorkItem(DoReload, args))
				DoReload(args);
			return asyncResult;
		}

		/// <summary>
		/// Recupera o resulta da execução assincrona da recarga dos dados para o cache.
		/// </summary>
		/// <param name="ar"></param>
		/// <rereturns></rereturns>
		public RealoadDataCacheResult EndReload(IAsyncResult ar)
		{
			var asyncResult = (Colosoft.Threading.AsyncResult<RealoadDataCacheResult>)ar;
			if(asyncResult.Exception != null)
				throw asyncResult.Exception;
			return asyncResult.Result;
		}

		/// <summary>
		/// Recupera os nomes do tipos que serão carregados.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Reflection.TypeName> GetTypesNames()
		{
			return _typeNames.ToArray();
		}

		/// <summary>
		/// Método usado pelo processo assincrono para recarregar os dados para o cache.
		/// </summary>
		/// <param name="state"></param>
		private void DoReload(object state)
		{
			var args = (object[])state;
			var typeNames = (Colosoft.Reflection.TypeName[])args[0];
			var asyncResult = (Colosoft.Threading.AsyncResult<RealoadDataCacheResult>)args[1];
			RealoadDataCacheResult result = null;
			try
			{
				result = Reload(typeNames);
			}
			catch(Exception ex)
			{
				asyncResult.HandleException(ex, false);
				return;
			}
			asyncResult.Complete(result, false);
		}

		/// <summary>
		/// Cria o cache local.
		/// </summary>
		private void CreateLocalCache()
		{
			CacheLoaderConfiguration = GetCacheLoader();
			CreateCache(CacheLoaderConfiguration);
		}

		/// <summary>
		/// Cria o cache com base nos dados direto do servidor.
		/// </summary>
		private void CreateServerCache()
		{
			CacheLoaderConfiguration = GetDataCacheLoader();
			CreateCache(CacheLoaderConfiguration);
		}

		/// <summary>
		/// Cria o cache com base no loader informado.
		/// </summary>
		/// <param name="loader"></param>
		private void CreateCache(Colosoft.Caching.Configuration.Dom.CacheLoader loader)
		{
			var config = CreateCacheConfig(loader);
			var configProperties = Colosoft.Caching.Configuration.Dom.ConfigConverter.ToHashtable(config);
			var cacheProperties = (System.Collections.Hashtable)configProperties["cache"];
			var cacheLoaderProperties = (System.Collections.Hashtable)cacheProperties["cache-loader"];
			var loaderParameters = new System.Collections.Hashtable();
			loaderParameters.Add("sourceContext", _sourceContext);
			loaderParameters.Add("manager", this);
			loaderParameters.Add("downloader", _dataEntryDownloader);
			loaderParameters.Add("repository", _dataEntriesRepository);
			loaderParameters.Add("logger", _logger);
			loaderParameters.Add("typeSchema", _typeSchema);
			loaderParameters.Add("cacheLoaderObserver", _cacheLoaderObserver);
			cacheLoaderProperties["parameters"] = loaderParameters;
			cacheProperties.Add("observer", this);
			_cache = Colosoft.Caching.CacheFactory.CreateFromProperties(configProperties, config, true);
			_cache.Loaded += CacheLoaded;
		}

		/// <summary>
		/// Método acionado quando o cache for carregado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CacheLoaded(object sender, EventArgs e)
		{
			_isInitialized = true;
			_cache.Loaded -= CacheLoaded;
		}

		/// <summary>
		/// Cria a configuração do cache.
		/// </summary>
		/// <returns></returns>
		private Colosoft.Caching.Configuration.Dom.CacheConfig CreateCacheConfig(Colosoft.Caching.Configuration.Dom.CacheLoader loader)
		{
			var config = new Colosoft.Caching.Configuration.Dom.CacheConfig();
			config.CacheType = "local-cache";
			config.InProc = false;
			config.ConfigID = 0;
			config.Name = "local-cache";
			config.CacheLoader = loader;
			config.QueryIndices = GetQueryIndices();
			config.Storage = new Colosoft.Caching.Configuration.Dom.Storage {
				Size = int.MaxValue,
				Type = "heap"
			};
			return config;
		}

		/// <summary>
		/// Recupera os indices de consulta.
		/// </summary>
		/// <returns></returns>
		private Colosoft.Caching.Configuration.Dom.QueryIndex GetQueryIndices()
		{
			var classes = new List<Colosoft.Caching.Configuration.Dom.Class>();
			foreach (var typeMetadata in _typeSchema.GetTypeMetadatas())
			{
				Colosoft.Reflection.TypeName typeName = null;
				try
				{
					typeName = typeMetadata.GetTypeName();
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.Fail("An error ocurred where get typename from TypeMetadata", Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true));
					continue;
				}
				var attributes = new List<Colosoft.Caching.Configuration.Dom.Attrib>();
				foreach (var propertyMetadata in typeMetadata)
				{
					if(propertyMetadata.IsCacheIndexed || propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey || propertyMetadata.ParameterType == PersistenceParameterType.Key)
					{
						attributes.Add(new Colosoft.Caching.Configuration.Dom.Attrib {
							ID = propertyMetadata.Name,
							Name = propertyMetadata.Name,
							Type = propertyMetadata.PropertyType
						});
					}
				}
				if(typeMetadata.IsVersioned && !attributes.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, "RowVersion")))
				{
					attributes.Add(new Colosoft.Caching.Configuration.Dom.Attrib {
						ID = "RowVersion",
						Name = "RowVersion",
						Type = typeof(long).FullName
					});
				}
				if(attributes.Count > 0)
				{
					classes.Add(new Colosoft.Caching.Configuration.Dom.Class {
						ID = typeName.FullName,
						Name = typeName.FullName,
						Attributes = attributes.ToArray()
					});
				}
				else
				{
				}
			}
			return new Colosoft.Caching.Configuration.Dom.QueryIndex {
				Classes = classes.ToArray()
			};
		}

		/// <summary>
		/// Recupera as configurações do CacheLoader.
		/// </summary>
		/// <returns></returns>
		private Colosoft.Caching.Configuration.Dom.CacheLoader GetCacheLoader()
		{
			var loader = new Colosoft.Caching.Configuration.Dom.CacheLoader();
			loader.Enabled = true;
			loader.Retries = 0;
			loader.RetryInterval = 0;
			loader.Provider = new Colosoft.Caching.Configuration.Dom.ProviderAssembly {
				FullProviderName = "CacheLoader",
				AssemblyName = "Colosoft.Data.Caching",
				ClassName = "Colosoft.Data.Caching.CacheLoader"
			};
			return loader;
		}

		/// <summary>
		/// Recupera as configurações do DataCacheLoader.
		/// </summary>
		/// <returns></returns>
		private Colosoft.Caching.Configuration.Dom.CacheLoader GetDataCacheLoader()
		{
			var loader = new Colosoft.Caching.Configuration.Dom.CacheLoader();
			loader.Enabled = true;
			loader.Retries = 0;
			loader.RetryInterval = 0;
			loader.Provider = new Colosoft.Caching.Configuration.Dom.ProviderAssembly {
				FullProviderName = "DataCacheLoader",
				AssemblyName = "Colosoft.Data.Caching",
				ClassName = "Colosoft.Data.Caching.DataCacheLoader"
			};
			return loader;
		}

		/// <summary>
		/// Recupera o enumerador do nome dos tipos.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Reflection.TypeName> GetEnumerator()
		{
			lock (_objLock)
				return _typeNames.ToList().GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador do nome dos tipos.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			lock (_objLock)
				return _typeNames.ToList().GetEnumerator();
		}

		/// <summary>
		/// Método acionado quando o cache for carregado.
		/// </summary>
		void Colosoft.Caching.ICacheObserver.OnLoaded()
		{
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado quanod ocorre um erro na carga do cache.
		/// </summary>
		/// <param name="e"></param>
		void Colosoft.Caching.ICacheObserver.OnLoadError(Colosoft.Caching.CacheErrorEventArgs e)
		{
			if(LoadError != null)
				LoadError(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		/// <param name="e"></param>
		void Colosoft.Caching.ICacheObserver.OnLoadProcessingError(Colosoft.Caching.CacheErrorEventArgs e)
		{
			if(LoadProcessingError != null)
				LoadProcessingError(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorreu um erro ao inserir uma entrada do cache.
		/// </summary>
		/// <param name="e"></param>
		void Colosoft.Caching.ICacheObserver.OnInsertEntryError(Colosoft.Caching.CacheInsertEntryErrorEventArgs e)
		{
			if(InsertEntryError != null)
				InsertEntryError(this, e);
		}
	}
}
