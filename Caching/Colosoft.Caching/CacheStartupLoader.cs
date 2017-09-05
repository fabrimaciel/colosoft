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
using System.Collections;
using System.Collections.Specialized;
using Colosoft.Caching.Policies;
using Colosoft.Caching.Exceptions;

namespace Colosoft.Caching.Loaders
{
	/// <summary>
	/// Classe responsável pela carga do cache.
	/// </summary>
	public class CacheStartupLoader : IDisposable
	{
		private Cache _cache;

		private ICacheLoader _cacheLoader;

		private bool _enabled;

		private bool _loadCache;

		private int _noOfRetries;

		private IDictionary _properties;

		private int _retryInterval;

		private LoadCacheTask _task;

		private Logging.ILogger _logger;

		/// <summary>
		/// Evento acionado quando o cache for completamente carregado.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Evento acionado quando ocorreu um erro ao processa uma entrada do cache.
		/// </summary>
		public event ProcessCacheEntryErrorEventHandler ProcessEntryError;

		/// <summary>
		/// Evento acionado quando ocorreu um erro ao inserir uma entrada no cache.
		/// </summary>
		public event CacheInsertEntryErrorHandler InsertEntryError;

		/// <summary>
		/// Evento acionado quando ocorre um erro ao carregar o próximo conjunto de registros.
		/// </summary>
		public event LoadNextErrorEventHandler LoadNextError;

		/// <summary>
		/// Evento acionado quando ocorrer um erro no processamento da carga.
		/// </summary>
		public event CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Evento acionado quando uma entrada estiver sendo inserida.
		/// </summary>
		public event InsertingCacheEntryEventHandler InsertingEntry;

		/// <summary>
		/// Evento acioando quando uma entrada estiver sido inserida no cache.
		/// </summary>
		public event InsertedCacheEntryEventHandler InsertedEntry;

		/// <summary>
		/// Identifica se a já foi executado.
		/// </summary>
		public bool ExecuteCacheLoader
		{
			get
			{
				return _loadCache;
			}
			set
			{
				_loadCache = value;
			}
		}

		/// <summary>
		/// Identifica se a instancia está abilitada.
		/// </summary>
		public bool IsCacheloaderEnabled
		{
			get
			{
				return _enabled;
			}
		}

		/// <summary>
		/// Número de tentativas de carga.
		/// </summary>
		public int NoOfRetries
		{
			get
			{
				return _noOfRetries;
			}
			set
			{
				_noOfRetries = value;
			}
		}

		/// <summary>
		/// Dicionário com as propriedades da instancia.
		/// </summary>
		public IDictionary Properties
		{
			get
			{
				return _properties;
			}
			set
			{
				_properties = value;
			}
		}

		/// <summary>
		/// Intervalo de tentativas.
		/// </summary>
		public int RetryInterval
		{
			get
			{
				return _retryInterval * 1000;
			}
			set
			{
				_retryInterval = value;
			}
		}

		/// <summary>
		/// Tarega de carga do cache.
		/// </summary>
		public LoadCacheTask Task
		{
			get
			{
				return _task;
			}
			set
			{
				_task = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties2"></param>
		/// <param name="cache"></param>
		/// <param name="logger"></param>
		public CacheStartupLoader(IDictionary properties2, Cache cache, Logging.ILogger logger)
		{
			properties2.Require("properties2").NotNull();
			logger.Require("logger").NotNull();
			if(properties2.Contains("retries"))
				_noOfRetries = Convert.ToInt32(properties2["retries"]);
			else
				_noOfRetries = 0;
			if(properties2.Contains("retry-interval"))
				_retryInterval = Convert.ToInt32(properties2["retry-interval"]);
			else
				_retryInterval = 0;
			if(properties2.Contains("enabled"))
				_enabled = Convert.ToBoolean(properties2["enabled"]);
			_cache = cache;
			_properties = properties2;
			_logger = logger;
		}

		/// <summary>
		/// Libera a istancia.
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
			if(_cacheLoader != null)
			{
				_cacheLoader.Dispose();
				_cacheLoader = null;
			}
			if(_task != null && _task.IsAlive)
				_task.Abort();
		}

		/// <summary>
		/// Inicializa a instancia com as propriedades informadas.
		/// </summary>
		/// <param name="properties"></param>
		private void Initialize(IDictionary properties)
		{
			properties.Require("properties").NotNull();
			try
			{
				if(!properties.Contains("assembly"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => global::Colosoft.Caching.Properties.Resources.ConfigurationException_MissingAssemblyName).Format());
				if(!properties.Contains("classname"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => global::Colosoft.Caching.Properties.Resources.ConfigurationException_MissingClassName).Format());
				var assemblyName = Convert.ToString(properties["assembly"]);
				string typeName = Convert.ToString(properties["classname"]);
				string path = Convert.ToString(properties["full-name"]);
				string extension = ".dll";
				if(properties.Contains("full-name"))
					extension = System.IO.Path.GetExtension(path);
				IDictionary parameters = properties["parameters"] as IDictionary;
				if(parameters == null)
					parameters = new Hashtable();
				try
				{
					if(!string.IsNullOrEmpty(path) && extension.EndsWith(".dll") || extension.EndsWith(".exe"))
					{
						System.Reflection.Assembly assembly = null;
						string assemblyFile = CachingUtils.DeployedAssemblyDir + _cache.Name + @"\" + path;
						try
						{
							assembly = System.Reflection.Assembly.LoadFrom(assemblyFile);
						}
						catch(Exception exception)
						{
							throw new Exception(ResourceMessageFormatter.Create(() => global::Colosoft.Caching.Properties.Resources.Exception_CouldNotLoadAssembly, assemblyFile, exception.Message).Format());
						}
						if(assembly != null)
							_cacheLoader = (ICacheLoader)assembly.CreateInstance(typeName);
					}
					else
					{
						var type = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName), false);
						if(type != null)
							_cacheLoader = (ICacheLoader)Activator.CreateInstance(type);
					}
					if(_cacheLoader == null)
						throw new Exception(ResourceMessageFormatter.Create(() => global::Colosoft.Caching.Properties.Resources.Exception_UnableToInstantiate, typeName).Format());
					_cacheLoader.Init(parameters);
				}
				catch(InvalidCastException)
				{
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => global::Colosoft.Caching.Properties.Resources.ConfigurationException_ICacheLoaderNotImplemented).Format());
				}
				catch(Exception exception2)
				{
					throw new ConfigurationException(exception2.Message, exception2);
				}
			}
			catch(ConfigurationException)
			{
				throw;
			}
			catch(Exception exception3)
			{
				throw new ConfigurationException("Configuration Error: " + exception3.ToString(), exception3);
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="serializableObject"></param>
		/// <returns></returns>
		private object SafeSerialize(object serializableObject)
		{
			if(serializableObject != null)
				serializableObject = Serialization.Formatters.CompactBinaryFormatter.ToByteBuffer(serializableObject, _cache.Name);
			return serializableObject;
		}

		/// <summary>
		/// Método acionado quando cache for completamente carregado.
		/// </summary>
		protected virtual void OnLoaded()
		{
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Verifica se pode inserir a entrada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected virtual bool CanInsertEntry(object key, object value)
		{
			var args = new InsertingCacheEntryEventArgs(key, value);
			if(InsertingEntry != null)
				InsertingEntry(this, args);
			return !args.Cancel;
		}

		/// <summary>
		/// Método acionado quando uma entrada for inserida no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected virtual void OnInsertedEntry(object key, object value)
		{
			if(InsertedEntry != null)
				InsertedEntry(this, new InsertedCacheEntryEventArgs(key, value));
		}

		/// <summary>
		/// Método acionado quando ocorrer um erro ao inserir ume entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="error"></param>
		protected virtual void OnInsertEntryError(object key, object value, Exception error)
		{
			var args = new CacheInsertEntryErrorEventArgs(key, value, error);
			if(InsertEntryError != null)
				InsertEntryError(this, args);
			OnLoadProcessingError(args);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro ao carregar o próximo conjunto de registros.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="exception"></param>
		protected virtual void OnLoadNextError(object index, Exception exception)
		{
			var args = new LoadNextErrorEventArgs(index, exception);
			if(LoadNextError != null)
				LoadNextError(this, args);
			OnLoadProcessingError(args);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro ao processar uma entrada do cache.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="exception"></param>
		protected virtual void OnProcessEntryError(DictionaryEntry entry, Exception exception)
		{
			var args = new ProcessCacheEntryErrorEventArgs(entry, exception);
			if(ProcessEntryError != null)
				ProcessEntryError(this, args);
			OnLoadProcessingError(args);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnLoadProcessingError(CacheErrorEventArgs e)
		{
			if(LoadProcessingError != null)
				LoadProcessingError(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		/// <param name="exception"></param>
		protected virtual void OnLoadProcessingError(Exception exception)
		{
			if(LoadProcessingError != null)
				LoadProcessingError(this, new CacheErrorEventArgs(exception));
		}

		/// <summary>
		/// Configura a instancia com o CacheLoader informado.
		/// </summary>
		/// <param name="cacheLoader"></param>
		public void Initialize(ICacheLoader cacheLoader)
		{
			_cacheLoader = cacheLoader;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			try
			{
				Initialize(Properties);
			}
			catch
			{
				OnLoaded();
				throw;
			}
		}

		/// <summary>
		/// Carrega os dados do cache.
		/// </summary>
		public void LoadCache()
		{
			ExecuteLoadData();
			OnLoaded();
		}

		/// <summary>
		/// Executa a carga dos dados para o cache.
		/// </summary>
		private void ExecuteLoadData()
		{
			bool flag = true;
			object index = null;
			while (flag)
			{
				flag = false;
				var data = new OrderedDictionary();
				try
				{
					flag = _cacheLoader.LoadNext(ref data, ref index);
				}
				catch(Exception exception)
				{
					OnLoadNextError(index, exception);
					continue;
				}
				if(data != null)
					ProcessDataFromLoadNext(data);
			}
		}

		/// <summary>
		/// Processa os dados carregados do LoadNext.
		/// </summary>
		/// <param name="data"></param>
		private void ProcessDataFromLoadNext(OrderedDictionary data)
		{
			int num = 0;
			byte[] buffer = null;
			foreach (DictionaryEntry entry in data)
			{
				EvictionHint hint2;
				num = 0;
				object key = null;
				ProviderCacheItem item = null;
				Colosoft.Caching.Expiration.ExpirationHint expiryHint = null;
				string resyncProviderName = null;
				Hashtable queryInfo = null;
				try
				{
					if((!(entry.Key is string) || !(entry.Value is ProviderCacheItem)))
						throw new InvalidOperationException("Invalid Key/Value type specified");
					key = entry.Key;
					item = (ProviderCacheItem)entry.Value;
					if(item == null)
						continue;
					CacheLoaderUtil.EvaluateExpirationParameters(item.AbsoluteExpiration, item.SlidingExpiration);
					expiryHint = Expiration.DependencyHelper.GetExpirationHint(item.Dependency, item.AbsoluteExpiration, item.SlidingExpiration);
					if((expiryHint != null) && item.ResyncItemOnExpiration)
						expiryHint.SetBit(2);
					resyncProviderName = (item.ResyncProviderName == null) ? null : item.ResyncProviderName.ToLower();
					queryInfo = new Hashtable();
					TypeInfoMap typeInfoMap = _cache.GetTypeInfoMap();
					if(typeInfoMap != null)
						queryInfo["query-info"] = CacheLoaderUtil.GetQueryInfo(item.Value, typeInfoMap);
					if(item.Tags != null)
						queryInfo["tag-info"] = CacheLoaderUtil.GetTagInfo(item.Value, item.Tags);
					if(item.NamedTags != null)
					{
						Hashtable hashtable2 = CacheLoaderUtil.GetNamedTagsInfo(item.Value, item.NamedTags, typeInfoMap);
						if(hashtable2 != null)
							queryInfo["named-tag-info"] = hashtable2;
					}
				}
				catch(Exception exception3)
				{
					OnProcessEntryError(entry, exception3);
					continue;
				}
				var set = new BitSet();
				object currentValue = item.Value;
				try
				{
					hint2 = new PriorityEvictionHint(item.ItemPriority);
					if(!(currentValue is ICacheItemRecord))
					{
						buffer = this.SafeSerialize(currentValue) as byte[];
						if((buffer != null) && _cache.CompressionEnabled)
							buffer = IO.Compression.CompressionUtil.Compress(buffer, ref set, _cache.CompressThresholdSize);
						if(buffer != null)
							currentValue = UserBinaryObject.CreateUserBinaryObject(buffer);
					}
				}
				catch(Exception ex)
				{
					OnLoadProcessingError(ex);
					continue;
				}
				while (num <= this.NoOfRetries)
				{
					if(_cache.IsRunning)
					{
						if(!CanInsertEntry(key, currentValue))
							break;
						try
						{
							_cache.Insert(key, currentValue, expiryHint, null, hint2, item.Group, item.SubGroup, queryInfo, set, null, 0, LockAccessType.IGNORE_LOCK, null, resyncProviderName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						}
						catch(Exception exception4)
						{
							num++;
							System.Threading.Thread.Sleep(this.RetryInterval);
							if(num > this.NoOfRetries)
							{
								OnInsertEntryError(key, currentValue, exception4);
								if(exception4 is OperationFailedException)
								{
									if(((OperationFailedException)exception4).IsTracable || !_logger.IsErrorEnabled)
										continue;
									_logger.Error("CacheStartupLoader.Load()".GetFormatter(), exception4);
									break;
								}
								if(_logger.IsErrorEnabled)
								{
									_logger.Error("CacheStartupLoader.Load()".GetFormatter(), exception4);
									break;
								}
							}
							continue;
						}
						OnInsertedEntry(key, currentValue);
					}
					break;
				}
			}
		}
	}
}
