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
using System.Threading;
using System.Runtime.Remoting.Lifetime;
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Policies;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Data;
using Colosoft.Caching.Synchronization;
using Colosoft.IO.Compression;
using Colosoft.Caching.Util;
using Colosoft.Caching.Queries;
using System.Net.Sockets;
using Colosoft.Serialization;
using Colosoft.Caching.Local;
using Colosoft.Caching.Configuration;
using System.Runtime.Remoting;
using System.Diagnostics;
using Colosoft.Caching.Loaders;
using Colosoft.Threading;
using Colosoft.Caching.Statistics;
using Colosoft.Logging;
using Colosoft.Caching.Threading;

namespace Colosoft.Caching
{
	/// <summary>
	/// Possíveis operações assincronas.
	/// </summary>
	[Serializable]
	public enum AsyncOpCode
	{
		/// <summary>
		/// Adição
		/// </summary>
		Add,
		/// <summary>
		/// Atualização.
		/// </summary>
		Update,
		/// <summary>
		/// Remoção.
		/// </summary>
		Remove,
		/// <summary>
		/// Limpeza.
		/// </summary>
		Clear
	}
	/// <summary>
	/// Implementação do cache.
	/// </summary>
	public class Cache : IEnumerable, ICacheEventsListener, IDisposable
	{
		/// <summary>
		/// Constante que representa que foi informado o limite de compressão.
		/// </summary>
		public const int COMPRESSIONTHRESHOLD = 2;

		private CacheInfo _cacheInfo;

		private string _cacheType;

		private Hashtable _cmptKnownTypes;

		private Hashtable _cmptKnownTypesforNet;

		private bool _compressionEnabled;

		private long _compressionThresholdSize;

		private CacheRuntimeContext _context;

		private Hashtable _dataSharingKnownTypes;

		private Hashtable _dataSharingKnownTypesforNet;

		private bool _inProc;

		private long _lockIdTicker;

		private TimeSpan InstantStatsUpdateInterval;

		private bool _isLoaded = false;

		private AggregateCacheObserver _observer = new AggregateCacheObserver();

		private event AsyncOperationCompletedCallback _asyncOperationCompleted;

		private event CacheClearedCallback _cacheCleared;

		private event CacheStoppedCallback _cacheStopped;

		private event CustomNotificationCallback _cusotmNotif;

		private event CustomRemoveCallback _customRemoveNotif;

		private event CustomUpdateCallback _customUpdateNotif;

		private event DataSourceUpdatedCallback _dataSourceUpdated;

		private event ItemAddedCallback _itemAdded;

		private event ItemRemovedCallback _itemRemoved;

		private event ItemUpdatedCallback _itemUpdated;

		/// <summary>
		/// Evento acionado quando cache for completamenta carregado.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Evento acionado quando ocorre um erro na carga do cache.
		/// </summary>
		public event CacheErrorEventHandler LoadError;

		/// <summary>
		/// Evento acionado quando ocorreu um erro no processamento da carga do cache.
		/// </summary>
		public event CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Evento acionado quando ocorrer um erro ao inserir uma entrada no cache.
		/// </summary>
		public event CacheInsertEntryErrorHandler InsertEntryError;

		/// <summary>
		/// Observer que será usada pela instancia.
		/// </summary>
		public AggregateCacheObserver Observer
		{
			get
			{
				return _observer;
			}
		}

		/// <summary>
		/// Identifica se o cache está carregado.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}

		/// <summary>
		/// Tipo do cache.
		/// </summary>
		public string CacheType
		{
			get
			{
				return _cacheType;
			}
		}

		/// <summary>
		/// Identifica se a compressão está ativa.
		/// </summary>
		public bool CompressionEnabled
		{
			get
			{
				return _compressionEnabled;
			}
		}

		/// <summary>
		/// Tamanho limite da compactação.
		/// </summary>
		public long CompressThresholdSize
		{
			get
			{
				return (_compressionThresholdSize * 1024);
			}
		}

		/// <summary>
		/// String de configuração da instancia.
		/// </summary>
		public string ConfigString
		{
			get
			{
				return _cacheInfo.ConfigString;
			}
			set
			{
				_cacheInfo.ConfigString = value;
			}
		}

		/// <summary>
		/// Configurações.
		/// </summary>
		public Configuration.Dom.CacheConfig Configuration
		{
			get
			{
				return _cacheInfo.Configuration;
			}
			set
			{
				_cacheInfo.Configuration = value;
			}
		}

		/// <summary>
		/// Contexto de execução.
		/// </summary>
		internal CacheRuntimeContext Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Quantidade de itens no cache.
		/// </summary>
		public long Count
		{
			get
			{
				if(!this.IsRunning)
					return 0;
				return _context.CacheImpl.Count;
			}
		}

		/// <summary>
		/// Identifica se o cache está executando como processo.
		/// </summary>
		public bool IsInProc
		{
			get
			{
				return _inProc;
			}
		}

		/// <summary>
		/// Identifica se está em execução.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return (_context.CacheImpl != null);
			}
		}

		/// <summary>
		/// Recupera o valor da entrada do cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <returns></returns>
		public object this[object key]
		{
			get
			{
				object lockId = null;
				DateTime utcNow = DateTime.UtcNow;
				ulong version = 0;
				return this.GetGroup(key, new BitSet(), null, null, ref version, ref lockId, ref utcNow, TimeSpan.Zero, LockAccessType.IGNORE_LOCK, "", new OperationContext(OperationContextFieldName.OperationType, (OperationContextOperationType)OperationContextOperationType.CacheOperation)).Value;
			}
			set
			{
				this.Insert(key, value);
			}
		}

		/// <summary>
		/// Nome do cache.
		/// </summary>
		public string Name
		{
			get
			{
				return _cacheInfo.Name;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ILogger Logger
		{
			get
			{
				return _context.Logger;
			}
		}

		/// <summary>
		/// Identifica se a serialzação está ativa.
		/// </summary>
		public bool SerializationEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor inicial.
		/// </summary>
		static Cache()
		{
			MiscUtil.RegisterCompactTypes();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Cache()
		{
			_cacheInfo = new CacheInfo();
			_context = new CacheRuntimeContext();
			_compressionEnabled = true;
			_compressionThresholdSize = 2;
			_cmptKnownTypes = new Hashtable();
			_cmptKnownTypesforNet = new Hashtable();
			_dataSharingKnownTypes = new Hashtable();
			_dataSharingKnownTypesforNet = new Hashtable();
			this.InstantStatsUpdateInterval = new TimeSpan(0, 0, 1);
			_context.CacheRoot = this;
		}

		/// <summary>
		/// Cria a instancia com a string de configuração.
		/// </summary>
		/// <param name="configString"></param>
		protected internal Cache(string configString)
		{
			_cacheInfo = new CacheInfo();
			_context = new CacheRuntimeContext();
			_compressionEnabled = true;
			_compressionThresholdSize = 2;
			_cmptKnownTypes = new Hashtable();
			_dataSharingKnownTypes = new Hashtable();
			_dataSharingKnownTypesforNet = new Hashtable();
			this.InstantStatsUpdateInterval = new TimeSpan(0, 0, 1);
			_context.CacheRoot = this;
			_cacheInfo = ConfigHelper.GetCacheInfo(configString);
		}

		/// <summary>
		/// Método acionado quando o cache for completamente carregado.
		/// </summary>
		private void OnLoaded()
		{
			_isLoaded = true;
			Observer.OnLoaded();
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro na carga do cache.
		/// </summary>
		/// <param name="e"></param>
		private void OnLoadError(CacheErrorEventArgs e)
		{
			Observer.OnLoadError(e);
			if(LoadError != null)
				LoadError(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no processamneot do carga do cache.
		/// </summary>
		/// <param name="e"></param>
		private void OnLoadProcessingError(CacheErrorEventArgs e)
		{
			Observer.OnLoadProcessingError(e);
			if(LoadProcessingError != null)
				LoadProcessingError(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorrer um erro ao inserir ume entrada no cache.
		/// </summary>
		/// <param name="e"></param>
		private void OnInsertEntryError(CacheInsertEntryErrorEventArgs e)
		{
			Observer.OnInsertEntryError(e);
			if(InsertEntryError != null)
				InsertEntryError(this, e);
		}

		/// <summary>
		/// Verifica se a origem de dados está disponíveis para as opções informadas.
		/// </summary>
		/// <param name="updateOpts"></param>
		private void CheckDataSourceAvailabilityAndOptions(DataSourceUpdateOptions updateOpts)
		{
			if((updateOpts != DataSourceUpdateOptions.None) && ((_context.DatasourceMgr == null) || ((!_context.DatasourceMgr.IsWriteThruEnabled || (updateOpts != DataSourceUpdateOptions.WriteThru)) && (!_context.DatasourceMgr.IsWriteBehindEnabled || (updateOpts != DataSourceUpdateOptions.WriteBehind)))))
			{
				throw new OperationFailedException("Backing source not available. Verify backing source settings");
			}
		}

		/// <summary>
		/// Adiciona novas entradas para o cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="entries">Instancia das entradas.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Resultado da adição das entradas.</returns>
		private Hashtable Add(object[] keys, CacheEntry[] entries, OperationContext operationContext)
		{
			Hashtable hashtable3;
			try
			{
				var hashtable = _context.CacheImpl.Add(keys, entries, true, operationContext);
				if(hashtable != null)
				{
					IDictionaryEnumerator enumerator = ((Hashtable)hashtable.Clone()).GetEnumerator();
					while (enumerator.MoveNext())
					{
						if(enumerator.Value is CacheAddResult)
						{
							switch(((CacheAddResult)enumerator.Value))
							{
							case CacheAddResult.Success:
								hashtable.Remove(enumerator.Key);
								break;
							case CacheAddResult.KeyExists:
								hashtable[enumerator.Key] = new OperationFailedException("The specified key already exists.");
								break;
							case CacheAddResult.NeedsEviction:
								hashtable[enumerator.Key] = new OperationFailedException("The cache is full and not enough items could be evicted.");
								break;
							}
						}
					}
				}
				hashtable3 = hashtable;
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Add():".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Add():".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Add operation failed. Error : " + exception2.Message, exception2);
			}
			return hashtable3;
		}

		/// <summary>
		/// Adiciona uma nova entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="entry">Instancia da entrada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		private void Add(object key, CacheEntry entry, OperationContext operationContext)
		{
			object obj1 = entry.Value;
			try
			{
				switch(_context.CacheImpl.Add(key, entry, true, operationContext))
				{
				case CacheAddResult.Success:
					return;
				case CacheAddResult.SuccessNearEviction:
				case CacheAddResult.Failure:
					return;
				case CacheAddResult.KeyExists:
					throw new OperationFailedException("The specified key already exists.", false);
				case CacheAddResult.NeedsEviction:
					throw new OperationFailedException("The cache is full and not enough items could be evicted.", false);
				}
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Add():".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Add():".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Add operation failed. Error : " + exception2.Message, exception2);
			}
		}

		/// <summary>
		/// Limpa os callbacks da instancia
		/// </summary>
		private void ClearCallbacks()
		{
			if(_asyncOperationCompleted != null)
			{
				Delegate[] invocationList = _asyncOperationCompleted.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					AsyncOperationCompletedCallback callback = (AsyncOperationCompletedCallback)invocationList[i];
					_asyncOperationCompleted = (AsyncOperationCompletedCallback)Delegate.Remove(_asyncOperationCompleted, callback);
				}
			}
			if(_cacheStopped != null)
			{
				Delegate[] delegateArray2 = _cacheStopped.GetInvocationList();
				for(int j = delegateArray2.Length - 1; j >= 0; j--)
				{
					CacheStoppedCallback callback2 = (CacheStoppedCallback)delegateArray2[j];
					_cacheStopped = (CacheStoppedCallback)Delegate.Remove(_cacheStopped, callback2);
				}
			}
			if(_cacheCleared != null)
			{
				Delegate[] delegateArray3 = _cacheCleared.GetInvocationList();
				for(int k = delegateArray3.Length - 1; k >= 0; k--)
				{
					CacheClearedCallback callback3 = (CacheClearedCallback)delegateArray3[k];
					_cacheCleared = (CacheClearedCallback)Delegate.Remove(_cacheCleared, callback3);
				}
			}
			if(_itemUpdated != null)
			{
				Delegate[] delegateArray4 = _itemUpdated.GetInvocationList();
				for(int m = delegateArray4.Length - 1; m >= 0; m--)
				{
					ItemUpdatedCallback callback4 = (ItemUpdatedCallback)delegateArray4[m];
					_itemUpdated = (ItemUpdatedCallback)Delegate.Remove(_itemUpdated, callback4);
				}
			}
			if(_itemRemoved != null)
			{
				Delegate[] delegateArray5 = _itemRemoved.GetInvocationList();
				for(int n = delegateArray5.Length - 1; n >= 0; n--)
				{
					ItemRemovedCallback callback5 = (ItemRemovedCallback)delegateArray5[n];
					_itemRemoved = (ItemRemovedCallback)Delegate.Remove(_itemRemoved, callback5);
				}
			}
			if(_itemAdded != null)
			{
				Delegate[] delegateArray6 = _itemAdded.GetInvocationList();
				for(int num6 = delegateArray6.Length - 1; num6 >= 0; num6--)
				{
					ItemAddedCallback callback6 = (ItemAddedCallback)delegateArray6[num6];
					_itemAdded = (ItemAddedCallback)Delegate.Remove(_itemAdded, callback6);
				}
			}
			if(_customUpdateNotif != null)
			{
				Delegate[] delegateArray7 = _customUpdateNotif.GetInvocationList();
				for(int num7 = delegateArray7.Length - 1; num7 >= 0; num7--)
				{
					CustomUpdateCallback callback7 = (CustomUpdateCallback)delegateArray7[num7];
					_customUpdateNotif = (CustomUpdateCallback)Delegate.Remove(_customUpdateNotif, callback7);
				}
			}
			if(_customRemoveNotif != null)
			{
				Delegate[] delegateArray8 = _customRemoveNotif.GetInvocationList();
				for(int num8 = delegateArray8.Length - 1; num8 >= 0; num8--)
				{
					CustomRemoveCallback callback8 = (CustomRemoveCallback)delegateArray8[num8];
					_customRemoveNotif = (CustomRemoveCallback)Delegate.Remove(_customRemoveNotif, callback8);
				}
			}
			if(_cusotmNotif != null)
			{
				Delegate[] delegateArray9 = _cusotmNotif.GetInvocationList();
				for(int num9 = delegateArray9.Length - 1; num9 >= 0; num9--)
				{
					CustomNotificationCallback callback9 = (CustomNotificationCallback)delegateArray9[num9];
					_cusotmNotif = (CustomNotificationCallback)Delegate.Remove(_cusotmNotif, callback9);
				}
			}
		}

		/// <summary>
		/// Cria uma instancia interna do cache.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="twoPhaseInitialization"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void CreateInternalCache(IDictionary properties, bool twoPhaseInitialization)
		{
			properties.Require("properties").NotNull();
			try
			{
				if(!properties.Contains("class"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingAttribute, "class").Format());
				string str = Convert.ToString(properties["class"]);
				if(!properties.Contains("cache-classes"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingSection, "cache-classes").Format());
				IDictionary cacheClasses = (IDictionary)properties["cache-classes"];
				if(!cacheClasses.Contains(str.ToLower()))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_NotFoundClassCache, str).Format());
				IDictionary cacheProperties = (IDictionary)cacheClasses[str.ToLower()];
				if(!cacheProperties.Contains("type"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_CannotFindTypeOfCache, str).Format());
				if(this.Name.Length < 1)
					_cacheInfo.Name = str;
				bool flag = true;
				if(properties.Contains("perf-counters"))
					flag = Convert.ToBoolean(properties["perf-counters"]);
				_context.ExpiryMgr = new ExpirationManager(cacheProperties, _context);
				_context.ExpiryMgr.TopLevelCache = this;
				_cacheInfo.ClassName = Convert.ToString(cacheProperties["type"]).ToLower();
				_context.AsyncProc.Start();
				if(_cacheInfo.ClassName.CompareTo("overflow-cache") == 0)
				{
					LocalCacheImpl parentCache = new LocalCacheImpl();
					parentCache.Internal = CacheBase.Synchronized(new IndexedOverflowCache(cacheClasses, parentCache, cacheProperties, this, _context, null));
					_context.CacheImpl = parentCache;
				}
				else
				{
					if(_cacheInfo.ClassName.CompareTo("local-cache") != 0)
						throw new ConfigurationException("Specified cache class '" + _cacheInfo.ClassName + "' is not available in this edition of NCache.");
					LocalCacheImpl queryChangeListener = new LocalCacheImpl();
					ActiveQueryAnalyzer activeQueryAnalyzer = new ActiveQueryAnalyzer(queryChangeListener, cacheProperties, _cacheInfo.Name);
					queryChangeListener.Internal = CacheBase.Synchronized(new IndexedLocalCache(cacheClasses, queryChangeListener, cacheProperties, this, _context, activeQueryAnalyzer));
					_context.CacheImpl = queryChangeListener;
				}
				_cacheType = _cacheInfo.ClassName;
				if(_context.CacheImpl != null)
					_context.ExpiryMgr.Start();
				else
					_context.ExpiryMgr.Dispose();
			}
			catch(ConfigurationException exception)
			{
				_context.Logger.Error("Cache.CreateInternalCache()".GetFormatter(), exception.GetFormatter());
				_context.CacheImpl = null;
				this.Dispose();
				throw;
			}
			catch(Exception exception3)
			{
				_context.Logger.Error("Cache.CreateInternalCache()".GetFormatter(), exception3.GetFormatter());
				_context.CacheImpl = null;
				this.Dispose();
				throw new ConfigurationException("Configuration Error: " + exception3.ToString(), exception3);
			}
		}

		/// <summary>
		/// Cria uma instancia interna para o cache.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="userId"></param>
		/// <param name="password"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void CreateInternalCache2(IDictionary properties, string userId, string password)
		{
			properties.Require("properties").NotNull();
			try
			{
				string str = Convert.ToString(properties["name"]).Trim();
				if(this.Name.Length < 1)
					_cacheInfo.Name = str;
				bool flag = true;
				if(properties.Contains("perf-counters"))
				{
					Hashtable hashtable = properties["perf-counters"] as Hashtable;
					if((hashtable != null) && hashtable.Contains("enabled"))
						flag = Convert.ToBoolean(hashtable["enabled"]);
				}
				_context.ExpiryMgr = new ExpirationManager(properties, _context);
				_context.ExpiryMgr.TopLevelCache = this;
				_cacheInfo.ClassName = "local";
				_context.AsyncProc.Start();
				if(_cacheInfo.ClassName.CompareTo("local") != 0)
					throw new ConfigurationException("Specified cache class '" + _cacheInfo.ClassName + "' is not available in this edition of NCache.");
				LocalCacheImpl parentCache = new LocalCacheImpl();
				parentCache.Internal = CacheBase.Synchronized(new LocalCache(properties, parentCache, properties, this, _context, null));
				_context.CacheImpl = parentCache;
				_cacheType = _cacheInfo.ClassName;
				if(_context.CacheImpl != null)
					_context.ExpiryMgr.Start();
				else
					_context.ExpiryMgr.Dispose();
			}
			catch(ConfigurationException exception)
			{
				_context.Logger.Error("Cache.CreateInternalCache()".GetFormatter(), exception.ToString().GetFormatter());
				_context.CacheImpl = null;
				this.Dispose();
				throw;
			}
			catch(Exception exception3)
			{
				_context.Logger.Error("Cache.CreateInternalCache()".GetFormatter(), exception3.GetFormatter());
				_context.CacheImpl = null;
				this.Dispose();
				throw new ConfigurationException("Configuration Error: " + exception3.ToString(), exception3);
			}
		}

		/// <summary>
		/// Recupera a configuração do tamanho do limite de compressão.
		/// </summary>
		/// <param name="properties"></param>
		private void CompressionThresholdSize(IDictionary properties)
		{
			if(properties.Contains("threshold"))
			{
				try
				{
					_compressionThresholdSize = Convert.ToInt64(properties["threshold"]);
					_compressionEnabled = Convert.ToBoolean(properties["enabled"]);
				}
				catch(Exception)
				{
					_compressionThresholdSize = 2;
					_compressionEnabled = true;
				}
			}
			else
			{
				_compressionThresholdSize = 2;
				_compressionEnabled = true;
			}
		}

		/// <summary>
		/// Recupera o nome do provedor padrão.
		/// </summary>
		/// <returns></returns>
		private string GetDefaultProvider()
		{
			return "";
		}

		/// <summary>
		/// Recupera o identifiador do lock.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private object GetLockId(object key)
		{
			long num = 0;
			lock (this)
			{
				long num2;
				_lockIdTicker = (num2 = _lockIdTicker) + 1;
				num = num2;
			}
			return string.Concat(new object[] {
				Environment.MachineName,
				"-",
				key.ToString(),
				"-",
				num
			});
		}

		/// <summary>
		/// Métod acionado pelo callback de limpeza o cache.
		/// </summary>
		/// <param name="ar"></param>
		internal void CacheClearAsyncCallbackHandler(IAsyncResult ar)
		{
			CacheClearedCallback asyncState = (CacheClearedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.CacheClearAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_cacheCleared)
				{
					_cacheCleared = (CacheClearedCallback)Delegate.Remove(_cacheCleared, asyncState);
				}
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.CacheClearAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Métdo do callback de adiciona assincrona de uma nova entrada.
		/// </summary>
		/// <param name="ar"></param>
		internal void AddAsyncCallbackHandler(IAsyncResult ar)
		{
			ItemAddedCallback asyncState = (ItemAddedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.AddAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_itemAdded)
					_itemAdded = (ItemAddedCallback)Delegate.Remove(_itemAdded, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.AddAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Método usado para processar a chamada assincrona do evento customizado.
		/// </summary>
		/// <param name="ar"></param>
		internal void CustomEventAsyncCallbackHandler(IAsyncResult ar)
		{
			CustomNotificationCallback asyncState = (CustomNotificationCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.CustomEventAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_cusotmNotif)
					_cusotmNotif = (CustomNotificationCallback)Delegate.Remove(_cusotmNotif, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.CustomEventAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Método usado para processa a chamada assincrona do evento customizado de remoção.
		/// </summary>
		/// <param name="ar"></param>
		internal void CustomRemoveAsyncCallbackHandler(IAsyncResult ar)
		{
			CustomRemoveCallback asyncState = (CustomRemoveCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.CustomRemoveAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_customRemoveNotif)
					_customRemoveNotif = (CustomRemoveCallback)Delegate.Remove(_customRemoveNotif, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.CustomRemoveAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Método usado para processar
		/// </summary>
		/// <param name="ar"></param>
		internal void CustomUpdateAsyncCallbackHandler(IAsyncResult ar)
		{
			CustomUpdateCallback asyncState = (CustomUpdateCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.CustomUpdateAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_customUpdateNotif)
					_customUpdateNotif = (CustomUpdateCallback)Delegate.Remove(_customUpdateNotif, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.CustomUpdateAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		internal void DSUpdateEventAsyncCallbackHandler(IAsyncResult ar)
		{
			DataSourceUpdatedCallback asyncState = (DataSourceUpdatedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.DSUpdateEventAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				_dataSourceUpdated = (DataSourceUpdatedCallback)Delegate.Remove(_dataSourceUpdated, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.DSUpdateEventAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Adiciona um novo item para o cache.
		/// </summary>
		/// <param name="key">Chave que presenta o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(object key, object value)
		{
			this.Add(key, value, null, null, null, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Adiciona uma nova entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada que será adicionada.</param>
		/// <param name="evictionHint">Hint de liberação.</param>
		public void Add(object key, object value, EvictionHint evictionHint)
		{
			this.Add(key, value, null, null, evictionHint, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Adiciona uma nova entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Instancia da entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Add(object key, object value, ExpirationHint expiryHint, OperationContext operationContext)
		{
			this.Add(key, value, expiryHint, null, null, null, null, operationContext);
		}

		/// <summary>
		/// Adiciona uma nova entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Instancia com o valor da entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="syncDependency">Dependencia de sincronização.</param>
		/// <param name="evictionHint">Hint de liberação.</param>
		/// <param name="group">Grupo da entrada.</param>
		/// <param name="subGroup">Subgrupo da entrada.</param>
		/// <param name="queryInfo">Hash com as informações para consultas.</param>
		/// <param name="flag"></param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="resyncProviderName">Nome do provedor de resincronização.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void Add(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, Hashtable queryInfo, BitSet flag, string providerName, string resyncProviderName, OperationContext operationContext)
		{
			key.Require("key").NotNull();
			value.Require("value").NotNull();
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!value.GetType().IsSerializable)
				throw new ArgumentException("value is not serializable");
			if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
				throw new ArgumentException("expiryHint is not serializable");
			if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
				throw new ArgumentException("evictionHint is not serializable");
			if(this.IsRunning)
			{
				DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
				this.CheckDataSourceAvailabilityAndOptions(updateOpts);
				GroupInfo info = new GroupInfo(group, subGroup);
				CacheEntry entry = new CacheEntry(value, expiryHint, evictionHint);
				entry.GroupInfo = info;
				entry.ResyncProviderName = resyncProviderName;
				entry.ProviderName = providerName;
				entry.SyncDependency = syncDependency;
				entry.QueryInfo = queryInfo;
				var set1 = entry.Flag;
				set1.Data = (byte)(set1.Data | flag.Data);
				try
				{
					this.Add(key, entry, operationContext);
					if(updateOpts == DataSourceUpdateOptions.WriteThru)
						_context.DatasourceMgr.WriteThru(key as string, entry, OpCode.Add, providerName, operationContext);
					else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
						_context.DatasourceMgr.WriteBehind(_context.CacheImpl, key, entry, null, null, providerName, OpCode.Add, WriteBehindAsyncProcessor.TaskState.Execute);
				}
				catch(Exception)
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Adiciona uma entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="syncDependency">Dependencia de sincronização.</param>
		/// <param name="evictionHint">Hint de liberação.</param>
		/// <param name="group">Grupo da entrada.</param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Add(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			Hashtable queryInfo = null;
			var queryInfo1 = CacheLoaderUtil.GetQueryInfo(value, _context.CacheImpl.TypeInfoMap);
			if(queryInfo1 != null)
			{
				queryInfo = new Hashtable();
				queryInfo.Add("query-info", queryInfo1);
			}
			this.Add(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, queryInfo, operationContext);
		}

		/// <summary>
		/// Adiciona uma entrada para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="syncDependency">Dependencia de sincronização.</param>
		/// <param name="evictionHint">Hint de liberação.</param>
		/// <param name="group">Nome do grupo associado.</param>
		/// <param name="subGroup"></param>
		/// <param name="queryInfo">Informações para a consulta.</param>
		/// <param name="operationContext"></param>
		public void Add(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, Hashtable queryInfo, OperationContext operationContext)
		{
			this.Add(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, queryInfo, new BitSet(), null, null, operationContext);
		}

		/// <summary>
		/// Adiciona as entradas;
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="expiryHint"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Add(object[] keys, object[] values, ExpirationHint expiryHint, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			IDictionary dictionary;
			keys.Require("keys").NotNull();
			values.Require("values").NotNull();
			if(keys.Length != values.Length)
				throw new ArgumentException("keys count is not equals to values count");
			var entries = new CacheEntry[values.Length];
			for(int i = 0; i < values.Length; i++)
			{
				object obj2 = keys[i];
				object val = values[i];
				if(obj2 == null)
					throw new ArgumentNullException("key");
				if(val == null)
					throw new ArgumentNullException("value");
				if(!obj2.GetType().IsSerializable)
					throw new ArgumentException("key is not serializable");
				if(!val.GetType().IsSerializable)
					throw new ArgumentException("value is not serializable");
				if(expiryHint != null && !expiryHint.GetType().IsSerializable)
					throw new ArgumentException("expiryHint is not serializable");
				if(evictionHint != null && !evictionHint.GetType().IsSerializable)
					throw new ArgumentException("evictionHint is not serializable");
				if(!this.IsRunning)
					return null;
				entries[i] = new CacheEntry(val, expiryHint, evictionHint);
				GroupInfo info = new GroupInfo(group, subGroup);
				entries[i].GroupInfo = info;
			}
			try
			{
				dictionary = this.Add(keys, entries, operationContext);
			}
			catch(Exception)
			{
				throw;
			}
			return dictionary;
		}

		/// <summary>
		/// Adiciona as entradas.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="callbackEnteries"></param>
		/// <param name="expirations"></param>
		/// <param name="syncDependencies"></param>
		/// <param name="evictions"></param>
		/// <param name="groupInfos"></param>
		/// <param name="queryInfos"></param>
		/// <param name="flags"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Add(string[] keys, object[] values, CallbackEntry[] callbackEnteries, ExpirationHint[] expirations, CacheSyncDependency[] syncDependencies, EvictionHint[] evictions, GroupInfo[] groupInfos, Hashtable[] queryInfos, BitSet[] flags, string providerName, OperationContext operationContext)
		{
			keys.Require("keys").NotNull();
			values.Require("values").NotNull();
			IDictionary dictionary2;
			if(keys.Length != values.Length)
				throw new ArgumentException("keys count is not equals to values count");
			DataSourceUpdateOptions updateOpts = this.UpdateOption(flags[0]);
			this.CheckDataSourceAvailabilityAndOptions(updateOpts);
			CacheEntry[] entries = new CacheEntry[values.Length];
			for(int i = 0; i < values.Length; i++)
			{
				if(keys[i] == null)
					throw new ArgumentNullException("key");
				if(values[i] == null)
					throw new ArgumentNullException("value");
				if(!keys[i].GetType().IsSerializable)
					throw new ArgumentException("key is not serializable");
				if(!values[i].GetType().IsSerializable)
					throw new ArgumentException("value is not serializable");
				if((expirations[i] != null) && !expirations[i].GetType().IsSerializable)
					throw new ArgumentException("expiryHint is not serializable");
				if((evictions[i] != null) && !evictions[i].GetType().IsSerializable)
					throw new ArgumentException("evictionHint is not serializable");
				if(!this.IsRunning)
					return null;
				entries[i] = new CacheEntry(values[i], expirations[i], evictions[i]);
				entries[i].SyncDependency = syncDependencies[i];
				entries[i].GroupInfo = groupInfos[i];
				entries[i].QueryInfo = queryInfos[i];
				BitSet flag = entries[i].Flag;
				flag.Data = (byte)(flag.Data | flags[i].Data);
				entries[i].ProviderName = providerName;
				if(callbackEnteries[i] != null)
				{
					CallbackEntry entry = callbackEnteries[i].Clone() as CallbackEntry;
					entry.Value = values[i];
					entry.Flag = entries[i].Flag;
					entries[i].Value = entry;
				}
			}
			try
			{
				IDictionary dictionary = this.Add(keys, entries, operationContext);
				string[] strArray = null;
				object[] objArray = null;
				if((updateOpts != DataSourceUpdateOptions.None) && (keys.Length > dictionary.Count))
				{
					strArray = new string[keys.Length - dictionary.Count];
					objArray = new object[keys.Length - dictionary.Count];
					int index = 0;
					int num3 = 0;
					while (index < keys.Length)
					{
						if(!dictionary.Contains(keys[index]))
						{
							strArray[num3] = keys[index];
							objArray[num3] = CompressionUtil.Decompress((values[index] as UserBinaryObject).GetFullObject(), entries[index].Flag);
							num3++;
						}
						index++;
					}
					if(updateOpts == DataSourceUpdateOptions.WriteThru)
						_context.DatasourceMgr.WriteThru(strArray, objArray, null, dictionary as Hashtable, OpCode.Add, providerName, operationContext);
					else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
					{
						CacheEntry[] entryArray2 = new CacheEntry[] {
							entries[0]
						};
						_context.DatasourceMgr.WriteBehind(_context.CacheImpl, strArray, objArray, entryArray2, null, null, providerName, OpCode.Add, WriteBehindAsyncProcessor.TaskState.Execute);
					}
				}
				dictionary2 = dictionary;
			}
			catch(Exception)
			{
				throw;
			}
			return dictionary2;
		}

		/// <summary>
		/// Adiciona uma nova entrada de forma assincrona.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		public void AddAsync(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			this.AddAsync(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, new BitSet(), null, operationContext);
		}

		/// <summary>
		/// Adiciona uma nova entrada de forma assicrona.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="Flag"></param>
		/// <param name="queryInfo"></param>
		/// <param name="operationContext"></param>
		public void AddAsync(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, BitSet Flag, Hashtable queryInfo, OperationContext operationContext)
		{
			key.Require("key").NotNull();
			value.Require("value").NotNull();
			if(!_inProc)
			{
				if(key is byte[])
					key = SerializationUtil.CompactDeserialize((byte[])key, _context.SerializationContext);
				if(value is byte[])
					value = SerializationUtil.CompactDeserialize((byte[])value, _context.SerializationContext);
			}
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!value.GetType().IsSerializable)
				throw new ArgumentException("value is not serializable");
			if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
				throw new ArgumentException("expiryHint is not not serializable");
			if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
				throw new ArgumentException("evictionHint is not serializable");
			if(this.IsRunning)
				_context.AsyncProc.Enqueue(new AsyncAdd(this, key, value, expiryHint, syncDependency, evictionHint, group, subGroup, Flag, queryInfo, operationContext));
		}

		/// <summary>
		/// Adiciona um nova entrada de forma assincrona.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="operationContext"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void AddAsyncEntry(object entry, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				CompactCacheEntry entry2 = (CompactCacheEntry)SerializationUtil.CompactDeserialize(entry, _context.SerializationContext);
				bool isAbsolute = false;
				bool flag2 = false;
				int num = 0;
				int options = entry2.Options;
				if(options != 0xff)
				{
					isAbsolute = Convert.ToBoolean((int)(options & 1));
					options = options >> 1;
					flag2 = Convert.ToBoolean((int)(options & 1));
					options = options >> 1;
					num = options - 2;
				}
				ExpirationHint expiryHint = ConvHelper.MakeExpirationHint(entry2.Expiration, isAbsolute);
				if((expiryHint != null) && (entry2.Dependency != null))
					expiryHint = new AggregateExpirationHint(new ExpirationHint[] {
						entry2.Dependency,
						expiryHint
					});
				if(expiryHint == null)
					expiryHint = entry2.Dependency;
				if((expiryHint != null) && flag2)
					expiryHint.SetBit(2);
				this.AddAsync(entry2.Key, entry2.Value, expiryHint, entry2.SyncDependency, new PriorityEvictionHint((CacheItemPriority)num), entry2.Group, entry2.SubGroup, entry2.Flag, entry2.QueryInfo, operationContext);
			}
		}

		/// <summary>
		/// Adiciona várias entradas.
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary AddEntries(object[] entries, OperationContext operationContext)
		{
			if(!this.IsRunning)
			{
				return null;
			}
			string[] keys = new string[entries.Length];
			object[] values = new object[entries.Length];
			CallbackEntry[] callbackEnteries = new CallbackEntry[entries.Length];
			ExpirationHint[] expirations = new ExpirationHint[entries.Length];
			EvictionHint[] evictions = new EvictionHint[entries.Length];
			CacheSyncDependency[] syncDependencies = new CacheSyncDependency[entries.Length];
			BitSet[] flags = new BitSet[entries.Length];
			Hashtable[] queryInfos = new Hashtable[entries.Length];
			GroupInfo[] groupInfos = new GroupInfo[entries.Length];
			CallbackEntry entry = null;
			for(int i = 0; i < entries.Length; i++)
			{
				CompactCacheEntry cce = (CompactCacheEntry)SerializationUtil.CompactDeserialize(entries[i], _context.SerializationContext);
				keys[i] = cce.Key as string;
				CacheEntry entry3 = this.MakeCacheEntry(cce);
				if(entry3 != null)
				{
					if(entry3.Value is CallbackEntry)
					{
						entry = entry3.Value as CallbackEntry;
					}
					else
					{
						entry = null;
					}
					callbackEnteries[i] = entry;
					object obj2 = entry3.Value as CallbackEntry;
					values[i] = (obj2 == null) ? entry3.Value : ((CallbackEntry)entry3.Value).Value;
					expirations[i] = entry3.ExpirationHint;
					evictions[i] = entry3.EvictionHint;
					syncDependencies[i] = entry3.SyncDependency;
					queryInfos[i] = entry3.QueryInfo;
					flags[i] = entry3.Flag;
					groupInfos[i] = new GroupInfo(cce.Group, cce.SubGroup);
				}
			}
			return this.Add(keys, values, callbackEnteries, expirations, syncDependencies, evictions, groupInfos, queryInfos, flags, null, operationContext);
		}

		/// <summary>
		/// Adiciona os dados de uma entrada compacta.
		/// </summary>
		/// <param name="entry">Instancia da entrada que será adicionada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void AddEntry(CompactCacheEntry entry, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				CompactCacheEntry cce = null;
				cce = (CompactCacheEntry)entry;
				CacheEntry entry3 = this.MakeCacheEntry(cce);
				if(entry3 != null)
					entry3.SyncDependency = cce.SyncDependency;
				string group = null;
				string subGroup = null;
				if((entry3.GroupInfo != null) && (entry3.GroupInfo.Group != null))
				{
					group = entry3.GroupInfo.Group;
					subGroup = entry3.GroupInfo.SubGroup;
				}
				this.Add(cce.Key, entry3.Value, entry3.ExpirationHint, entry3.SyncDependency, entry3.EvictionHint, group, subGroup, entry3.QueryInfo, entry3.Flag, cce.ProviderName, entry3.ResyncProviderName, operationContext);
			}
		}

		/// <summary>
		/// Adiciona um <see cref="ExpirationHint"/> para a entrada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="hint">Hint que será adicionado.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public bool AddExpirationHint(object key, ExpirationHint hint, OperationContext operationContext)
		{
			bool flag;
			if(!this.IsRunning)
				return false;
			try
			{
				flag = _context.CacheImpl.Add(key, hint, operationContext);
			}
			catch(Exception exception)
			{
				_context.Logger.Error(("Add operation failed. Error: " + exception.ToString()).GetFormatter());
				throw;
			}
			return flag;
		}

		/// <summary>
		/// Adiciona uma <see cref="CacheSyncDependency"/> para a entrada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="syncDependency">Dependencia de sincronização.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public bool AddSyncDependency(object key, CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			if(!this.IsRunning)
				return false;
			try
			{
				if(!_context.CacheImpl.Add(key, syncDependency, operationContext))
					return false;
			}
			catch(Exception exception)
			{
				_context.Logger.Error(("Add operation failed. Error: " + exception.ToString()).GetFormatter());
				throw;
			}
			return true;
		}

		/// <summary>
		/// Método acionado pela operação assincrona.
		/// </summary>
		/// <param name="ar"></param>
		internal void AsyncOpAsyncCallbackHandler(IAsyncResult ar)
		{
			AsyncOperationCompletedCallback asyncState = (AsyncOperationCompletedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.AsyncOpAsyncCallbackHandler".GetFormatter(), exception.ToString().GetFormatter());
				lock (_customUpdateNotif)
					_asyncOperationCompleted = (AsyncOperationCompletedCallback)Delegate.Remove(_asyncOperationCompleted, asyncState);
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.AsyncOpAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Insere as entradas em cascata.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="cacheEntries">Instancia das entradas.</param>
		/// <param name="notify">True para dispara notificação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		internal Hashtable CascadedInsert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			Hashtable removedItems = _context.CacheImpl.Insert(keys, cacheEntries, notify, operationContext);
			_context.CacheImpl.RemoveCascadingDependencies(removedItems, operationContext);
			return removedItems;
		}

		/// <summary>
		/// Realiza uma inserção cascadeada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="entry">Instancia da entrada.</param>
		/// <param name="notify">True para notificar.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		internal CacheInsResultWithEntry CascadedInsert(object key, CacheEntry entry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			CacheInsResultWithEntry entry2 = _context.CacheImpl.Insert(key, entry, notify, lockId, version, accessType, operationContext);
			if((entry2.Entry != null) && (entry2.Result != CacheInsResult.IncompatibleGroup))
				_context.CacheImpl.RemoveCascadingDependencies(key, entry2.Entry, operationContext);
			return entry2;
		}

		/// <summary>
		/// Remove em castata as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves que serão removidas.</param>
		/// <param name="reason">Razão para a remoção.</param>
		/// <param name="notify"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal Hashtable CascadedRemove(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			Hashtable removedItems = _context.CacheImpl.Remove(keys, reason, notify, operationContext);
			_context.CacheImpl.RemoveCascadingDependencies(removedItems, operationContext);
			return removedItems;
		}

		/// <summary>
		/// Remove as entrada emc cascata.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="notify"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal Hashtable CascadedRemove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			Hashtable removedItems = _context.CacheImpl.Remove(group, subGroup, notify, operationContext);
			_context.CacheImpl.RemoveCascadingDependencies(removedItems, operationContext);
			return removedItems;
		}

		/// <summary>
		/// Remove as entradas em cascata.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="comaprisonType"></param>
		/// <param name="notify"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal Hashtable CascadedRemove(string[] tags, TagComparisonType comaprisonType, bool notify, OperationContext operationContext)
		{
			Hashtable removedItems = _context.CacheImpl.RemoveByTag(tags, comaprisonType, notify, operationContext);
			_context.CacheImpl.RemoveCascadingDependencies(removedItems, operationContext);
			return removedItems;
		}

		/// <summary>
		/// Remove as entradas em cascata.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="pack"></param>
		/// <param name="reason"></param>
		/// <param name="notify"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal CacheEntry CascadedRemove(object key, object pack, ItemRemoveReason reason, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry e = _context.CacheImpl.Remove(pack, reason, notify, lockId, version, accessType, operationContext);
			if(e != null)
				_context.CacheImpl.RemoveCascadingDependencies(key, e, operationContext);
			return e;
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		public void Clear()
		{
			this.Clear(new BitSet(), null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Clear(OperationContext operationContext)
		{
			this.Clear(new BitSet(), null, operationContext);
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		/// <param name="flag">Conjunto com informações adicionais.</param>
		/// <param name="callbackEntry"></param>
		/// <param name="operationContext">Contexto da operação.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void Clear(BitSet flag, CallbackEntry callbackEntry, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
				this.CheckDataSourceAvailabilityAndOptions(updateOpts);
				try
				{
					string providerName = null;
					if(operationContext.Contains(OperationContextFieldName.ReadThruProviderName))
						providerName = (string)operationContext.GetValueByField(OperationContextFieldName.ReadThruProviderName);
					_context.CacheImpl.Clear(callbackEntry, updateOpts, operationContext);
					if(updateOpts == DataSourceUpdateOptions.WriteThru)
						_context.DatasourceMgr.WriteThru(null, null, OpCode.Clear, providerName, operationContext);
					else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
					{
						CacheEntry entry = null;
						if(callbackEntry != null)
							entry = new CacheEntry(callbackEntry, null, null);
						_context.DatasourceMgr.WriteBehind(_context.CacheImpl, null, entry, null, null, providerName, OpCode.Clear, WriteBehindAsyncProcessor.TaskState.Execute);
					}
				}
				catch(Exception exception)
				{
					_context.Logger.Error("Cache.Clear()".GetFormatter(), exception.GetFormatter());
					throw new OperationFailedException("Clear operation failed. Error: " + exception.Message, exception);
				}
			}
		}

		/// <summary>
		/// Cria um processo assincrono para a limpeza da instancia.
		/// </summary>
		/// <param name="flagMap"></param>
		/// <param name="callbackEntry"></param>
		/// <param name="operationContext"></param>
		public void ClearAsync(BitSet flagMap, CallbackEntry callbackEntry, OperationContext operationContext)
		{
			if(this.IsRunning)
				_context.AsyncProc.Enqueue(new AsyncClear(this, callbackEntry, flagMap, operationContext));
		}

		/// <summary>
		/// Fecha a stream da instanencia.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="lockHandle"></param>
		/// <param name="operationContext"></param>
		public void CloseStream(string key, string lockHandle, OperationContext operationContext)
		{
			_context.CacheImpl.CloseStream(key, lockHandle, operationContext);
		}

		/// <summary>
		/// Verifica se existe algum item no cache com chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será pesquisado.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public bool Contains(object key, OperationContext operationContext)
		{
			bool flag;
			if(key == null)
				throw new ArgumentNullException("key");
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!this.IsRunning)
				return false;
			try
			{
				flag = _context.CacheImpl.Contains(key, operationContext);
			}
			catch(Exception exception)
			{
				_context.Logger.Error("Cache.Contains()".GetFormatter(), exception.GetFormatter());
				throw new OperationFailedException("Contains operation failed. Error : " + exception.Message, exception);
			}
			return flag;
		}

		/// <summary>
		/// Apaga as entradas do cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas que serão apagadas.</param>
		/// <param name="flagMap"></param>
		/// <param name="cbEntry">Callback.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Delete(object[] keys, BitSet flagMap, CallbackEntry cbEntry, string providerName, OperationContext operationContext)
		{
			if(keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if(this.IsRunning)
			{
				DataSourceUpdateOptions updateOpts = this.UpdateOption(flagMap);
				this.CheckDataSourceAvailabilityAndOptions(updateOpts);
				try
				{
					IDictionary dictionary = this.CascadedRemove(keys, ItemRemoveReason.Removed, true, operationContext);
					if((((updateOpts != DataSourceUpdateOptions.None) && (dictionary != null)) && (dictionary.Count > 0)))
					{
						string[] array = null;
						CacheEntry[] entryArray = null;
						array = new string[dictionary.Count];
						entryArray = new CacheEntry[dictionary.Count];
						int index = 0;
						for(int i = 0; i < keys.Length; i++)
						{
							if(dictionary[keys[i]] is CacheEntry)
							{
								array[index] = keys[i] as string;
								entryArray[index++] = dictionary[keys[i]] as CacheEntry;
							}
						}
						if(dictionary.Count > index)
						{
							Resize(ref array, index);
							Resize(ref entryArray, index);
						}
						if(updateOpts == DataSourceUpdateOptions.WriteThru)
						{
							_context.DatasourceMgr.WriteThru(array, null, entryArray, dictionary as Hashtable, OpCode.Remove, providerName, operationContext);
						}
						else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
						{
							if(cbEntry != null)
							{
								if(entryArray[0].Value is CallbackEntry)
									((CallbackEntry)entryArray[0].Value).WriteBehindOperationCompletedCallback = cbEntry.WriteBehindOperationCompletedCallback;
								else
								{
									cbEntry.Value = entryArray[0].Value;
									entryArray[0].Value = cbEntry;
								}
							}
							_context.DatasourceMgr.WriteBehind(_context.CacheImpl, array, null, entryArray, null, null, providerName, OpCode.Remove, WriteBehindAsyncProcessor.TaskState.Execute);
						}
					}
				}
				catch(Exception exception)
				{
					_context.Logger.Error("Cache.Delete()".GetFormatter(), exception.GetFormatter());
					throw new OperationFailedException("Delete operation failed. Error : " + exception.Message, exception);
				}
			}
		}

		/// <summary>
		/// Apaga uma entrada do cache.
		/// </summary>
		/// <param name="key">Chave da entrada que será apagada.</param>
		/// <param name="flag"></param>
		/// <param name="cbEntry"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Delete(string key, BitSet flag, CallbackEntry cbEntry, object lockId, ulong version, LockAccessType accessType, string providerName, OperationContext operationContext)
		{
			if(key == null)
			{
				throw new ArgumentNullException("key");
			}
			if(!key.GetType().IsSerializable)
			{
				throw new ArgumentException("key is not serializable");
			}
			if(this.IsRunning)
			{
				DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
				this.CheckDataSourceAvailabilityAndOptions(updateOpts);
				try
				{
					object pack = key;
					CacheEntry val = this.CascadedRemove(key, pack, ItemRemoveReason.Removed, true, lockId, version, accessType, operationContext);
					if(val != null)
					{
						if(updateOpts == DataSourceUpdateOptions.WriteThru)
							_context.DatasourceMgr.WriteThru(key, val, OpCode.Remove, providerName, operationContext);
						else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
						{
							if(cbEntry != null)
							{
								if(val.Value is CallbackEntry)
									((CallbackEntry)val.Value).WriteBehindOperationCompletedCallback = cbEntry.WriteBehindOperationCompletedCallback;
								else
								{
									cbEntry.Value = val.Value;
									val.Value = cbEntry;
								}
							}
							_context.DatasourceMgr.WriteBehind(_context.CacheImpl, key, val, null, null, providerName, OpCode.Remove, WriteBehindAsyncProcessor.TaskState.Execute);
						}
					}
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.Delete()".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.Delete()".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("Delete operation failed. Error : " + exception2.Message, exception2);
				}
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
			lock (this)
			{
				if((_cacheStopped != null) && !this.CacheType.Equals("mirror-server"))
				{
					foreach (Delegate delegate2 in _cacheStopped.GetInvocationList())
					{
						CacheStoppedCallback callback = (CacheStoppedCallback)delegate2;
						try
						{
							callback(_cacheInfo.Name);
						}
						catch(Exception exception)
						{
							this.Logger.Error("Cache.Dispose".GetFormatter(), ("Error occured while invoking cache stopped event: " + exception.ToString()).GetFormatter());
						}
						finally
						{
							_cacheStopped = (CacheStoppedCallback)Delegate.Remove(_cacheStopped, callback);
						}
					}
				}
				if(_context.CacheImpl != null)
					_context.CacheImpl.StopServices();
				if(_context.DatasourceMgr != null)
				{
					_context.DatasourceMgr.Dispose();
					_context.DatasourceMgr = null;
				}
				if(_context.CSLManager != null)
				{
					_context.CSLManager.Dispose();
					_context.CSLManager = null;
				}
				if((_cacheStopped != null) && this.CacheType.Equals("mirror-server"))
				{
					_cacheStopped(_cacheInfo.Name);
				}
				if(_context.SyncManager != null)
				{
					_context.SyncManager.Dispose();
					_context.SyncManager = null;
				}
				this.ClearCallbacks();
				_cacheStopped = null;
				_cacheCleared = null;
				_itemAdded = null;
				_itemUpdated = null;
				_itemRemoved = null;
				_cusotmNotif = null;
				if(this.Logger != null)
					this.Logger.CriticalInfo("Cache.Dispose".GetFormatter(), "Cache stopped successfully".GetFormatter());
				GC.Collect();
				if(disposing)
					GC.SuppressFinalize(this);
				if(_context != null)
					_context.Dispose();
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~Cache()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Recupera uma entrada com base na chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public CompressedValueEntry Get(object key)
		{
			return this.GetGroup(key, new BitSet(), null, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Recupera o pacote dos dados.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="flagMap"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary GetBulk(object[] keys, BitSet flagMap, string providerName, OperationContext operationContext)
		{
			if(keys == null)
				throw new ArgumentNullException("keys");
			if(!this.IsRunning)
				return null;
			Hashtable orginalTable = null;
			try
			{
				orginalTable = _context.CacheImpl.Get(keys, operationContext);
				if(orginalTable == null)
					return orginalTable;
				bool flag = (_context.DatasourceMgr != null) && _context.DatasourceMgr.IsReadThruEnabled;
				int[] numArray = null;
				int num = 0;
				if(flag)
					numArray = new int[keys.Length];
				for(int i = 0; i < keys.Length; i++)
				{
					if(orginalTable.ContainsKey(keys[i]))
					{
						CacheEntry entry = orginalTable[keys[i]] as CacheEntry;
						CompressedValueEntry entry2 = new CompressedValueEntry();
						entry2.Value = (entry.Value is CallbackEntry) ? ((CallbackEntry)entry.Value).Value : entry.Value;
						entry2.Flag = entry.Flag;
						orginalTable[keys[i]] = entry2;
					}
					else if(flag && flagMap.IsBitSet(0x10))
					{
						numArray[num++] = i;
					}
				}
				if(flag && (num > 0))
				{
					if((providerName == null) || (providerName == string.Empty))
						providerName = _context.DatasourceMgr.DefaultReadThruProvider;
					string[] strArray = new string[num];
					CacheEntry[] e = new CacheEntry[num];
					BitSet[] setArray = new BitSet[num];
					for(int j = 0; j < num; j++)
					{
						int index = numArray[j];
						strArray[j] = keys[index] as string;
						e[j] = orginalTable[keys[index]] as CacheEntry;
						setArray[j] = (e[j] == null) ? new BitSet() : e[j].Flag;
					}
					_context.DatasourceMgr.ResyncCacheItem(orginalTable, strArray, e, setArray, providerName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
				}
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
				{
					_context.Logger.Error("Cache.Get()".GetFormatter(), exception.GetFormatter());
				}
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Get()".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Get operation failed. Error : " + exception2.Message, exception2);
			}
			return orginalTable;
		}

		/// <summary>
		/// Recupera as entradas associadas com as tags informadas.
		/// </summary>
		/// <param name="tags">Tags que serão usadas na comparação.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public Hashtable GetByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			if(!this.IsRunning)
				return null;
			Hashtable hashtable = null;
			try
			{
				hashtable = _context.CacheImpl.GetTag(tags, comparisonType, operationContext);
				if(hashtable == null)
					return hashtable;
				IDictionaryEnumerator enumerator = ((Hashtable)hashtable.Clone()).GetEnumerator();
				while (enumerator.MoveNext())
				{
					object key = enumerator.Key;
					CacheEntry entry = enumerator.Value as CacheEntry;
					CompressedValueEntry entry2 = new CompressedValueEntry();
					entry2.Value = (entry.Value is CallbackEntry) ? ((CallbackEntry)entry.Value).Value : entry.Value;
					entry2.Flag = entry.Flag;
					hashtable[key] = entry2;
				}
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.GetByTag()".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.GetByTag()".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("GetByTag operation failed. Error : " + exception2.Message, exception2);
			}
			return hashtable;
		}

		/// <summary>
		/// Recupera uma entrda do cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="group">Grupo da entrada.</param>
		/// <param name="subGroup">Subgrupo da entrada.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="lockTimeout"></param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public object GetCacheEntry(object key, string group, string subGroup, ref object lockId, ref DateTime lockDate, TimeSpan lockTimeout, LockAccessType accessType, OperationContext operationContext)
		{
			object obj3;
			key.Require("key").NotNull();
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!this.IsRunning)
				return null;
			try
			{
				CacheEntry entry = null;
				ulong version = 0;
				LockExpiration lockExpiration = null;
				if((((lockId != null) && (lockId.ToString() == "")) || (lockId == null)) && (accessType == LockAccessType.ACQUIRE))
				{
					lockId = this.GetLockId(key);
					lockDate = DateTime.Now;
					if(!TimeSpan.Equals(lockTimeout, TimeSpan.Zero))
						lockExpiration = new LockExpiration(lockTimeout);
				}
				object obj2 = lockId;
				if(((group == null) && (subGroup == null)) && ((accessType == LockAccessType.IGNORE_LOCK) || (accessType == LockAccessType.DONT_ACQUIRE)))
					entry = _context.CacheImpl.Get(key, operationContext);
				else if((group != null) && (subGroup != null))
					entry = _context.CacheImpl.GetGroup(key, group, subGroup, ref version, ref lockId, ref lockDate, null, LockAccessType.IGNORE_LOCK, operationContext);
				else
					entry = _context.CacheImpl.Get(key, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
				if(((entry == null) && (accessType == LockAccessType.ACQUIRE)) && ((lockId == null) || obj2.Equals(lockId)))
				{
					lockId = null;
					lockDate = new DateTime();
				}
				if(operationContext.Contains(OperationContextFieldName.ReadThru))
				{
					bool flag = Convert.ToBoolean(operationContext.GetValueByField(OperationContextFieldName.ReadThru));
					string providerName = null;
					if(operationContext.Contains(OperationContextFieldName.ReadThruProviderName))
					{
						providerName = Convert.ToString(operationContext.GetValueByField(OperationContextFieldName.ReadThruProviderName));
					}
					bool flag2 = false;
					if(accessType == LockAccessType.DONT_ACQUIRE)
					{
						if(((obj2 == null) && (lockId != null)) && (((string)lockId).CompareTo(string.Empty) != 0))
							flag2 = true;
					}
					else if((((accessType == LockAccessType.ACQUIRE) && (obj2 != null)) && ((lockId != null) && (((string)lockId).CompareTo(string.Empty) != 0))) && !obj2.Equals(lockId))
						flag2 = true;
					if((((entry == null) && !flag2) && (flag && (_context.DatasourceMgr != null))) && _context.DatasourceMgr.IsReadThruEnabled)
					{
						BitSet set = new BitSet();
						_context.DatasourceMgr.ResyncCacheItem(key as string, out entry, ref set, group, subGroup, providerName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					}
				}
				obj3 = entry;
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
				{
					_context.Logger.Error("Cache.Get()".GetFormatter(), exception.GetFormatter());
				}
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Get()".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Get operation failed. Error : " + exception2.Message, exception2);
			}
			return obj3;
		}

		/// <summary>
		/// Recupera o nome do provedor de leitura.
		/// </summary>
		/// <returns></returns>
		internal string GetDefaultReadThruProvider()
		{
			string providerName = null;
			foreach (var provider in _cacheInfo.Configuration.BackingSource.Readthru.Providers)
			{
				if(provider.IsDefaultProvider)
					providerName = provider.ProviderName;
			}
			return providerName;
		}

		/// <summary>
		/// Recupera o nome do provedor de escrita padrão.
		/// </summary>
		/// <returns></returns>
		internal string GetDefaultWriteThruProvider()
		{
			string providerName = null;
			foreach (var provider in _cacheInfo.Configuration.BackingSource.Writethru.Providers)
			{
				if(provider.IsDefaultProvider)
				{
					providerName = provider.ProviderName;
				}
			}
			return providerName;
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			IEnumerator enumerator;
			if(!this.IsRunning)
				return null;
			try
			{
				enumerator = new CacheEnumerator(_context.SerializationContext, _context.CacheImpl.GetEnumerator());
			}
			catch(Exception exception)
			{
				_context.Logger.Error("Cache.GetEnumerator()".GetFormatter(), exception.GetFormatter());
				throw new OperationFailedException("GetEnumerator failed. Error : " + exception.Message, exception);
			}
			return enumerator;
		}

		/// <summary>
		/// Recupera os dados do grupo.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="flagMap"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public CompressedValueEntry GetGroup(object key, BitSet flagMap, string group, string subGroup, string providerName, OperationContext operationContext)
		{
			object lockId = null;
			DateTime utcNow = DateTime.UtcNow;
			ulong version = 0;
			return this.GetGroup(key, flagMap, group, subGroup, ref version, ref lockId, ref utcNow, TimeSpan.Zero, LockAccessType.IGNORE_LOCK, providerName, operationContext);
		}

		/// <summary>
		/// Recupera a entrada do grupo.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="flagMap"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="version"></param>
		/// <param name="lockId"></param>
		/// <param name="lockDate"></param>
		/// <param name="lockTimeout"></param>
		/// <param name="accessType"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public CompressedValueEntry GetGroup(object key, BitSet flagMap, string group, string subGroup, ref ulong version, ref object lockId, ref DateTime lockDate, TimeSpan lockTimeout, LockAccessType accessType, string providerName, OperationContext operationContext)
		{
			if(!this.IsRunning)
				return null;
			CompressedValueEntry entry = new CompressedValueEntry();
			CacheEntry entry2 = null;
			try
			{
				LockExpiration lockExpiration = null;
				if(accessType == LockAccessType.ACQUIRE)
				{
					lockId = this.GetLockId(key);
					lockDate = DateTime.UtcNow;
					if(!TimeSpan.Equals(lockTimeout, TimeSpan.Zero))
					{
						lockExpiration = new LockExpiration(lockTimeout);
					}
				}
				object obj2 = lockId;
				if((group == null) && (subGroup == null))
					entry2 = _context.CacheImpl.Get(key, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
				else
					entry2 = _context.CacheImpl.GetGroup(key, group, subGroup, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
				if(((entry2 == null) && (accessType == LockAccessType.ACQUIRE)) && ((lockId == null) || obj2.Equals(lockId)))
				{
					lockId = null;
					lockDate = new DateTime();
				}
				if(flagMap != null)
				{
					bool flag = flagMap.IsBitSet(0x10);
					if(entry2 != null)
					{
						entry.Value = entry2.Value;
						entry.Flag = entry2.Flag;
					}
					bool flag2 = false;
					if(accessType == LockAccessType.DONT_ACQUIRE)
					{
						if(((obj2 == null) && (lockId != null)) && (((string)lockId).CompareTo(string.Empty) != 0))
							flag2 = true;
					}
					else if((((accessType == LockAccessType.ACQUIRE) && (obj2 != null)) && ((lockId != null) && (((string)lockId).CompareTo(string.Empty) != 0))) && !obj2.Equals(lockId))
					{
						flag2 = true;
					}
					if((((entry2 == null) && !flag2) && (flag && (_context.DatasourceMgr != null))) && _context.DatasourceMgr.IsReadThruEnabled)
					{
						entry.Flag = new BitSet();
						entry.Value = _context.DatasourceMgr.ResyncCacheItem(key as string, out entry2, ref entry.Flag, group, subGroup, providerName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					}
				}
				if(entry.Value is CallbackEntry)
					entry.Value = ((CallbackEntry)entry.Value).Value;
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error : " + exception.ToString()).GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error : " + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("Get operation failed. Error :" + exception2.Message, exception2);
			}
			return entry;
		}

		/// <summary>
		/// Recupera os dados de um grupo.
		/// </summary>
		/// <param name="group">Nome do grupo;</param>
		/// <param name="subGroup">Nome do subgrupo.</param>
		/// <param name="operationContext">Contexto da operação</param>
		/// <returns>Hashtable contendo os dados do grupo.</returns>
		public Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			group.Require("group").NotNull();
			Hashtable hashtable2;
			if(!this.IsRunning)
				return null;
			try
			{
				Hashtable hashtable = _context.CacheImpl.GetGroupData(group, subGroup, operationContext);
				if(hashtable != null)
				{
					object[] array = new object[hashtable.Count];
					hashtable.Keys.CopyTo(array, 0);
					IEnumerator enumerator = array.GetEnumerator();
					CompressedValueEntry entry = null;
					while (enumerator.MoveNext())
					{
						entry = new CompressedValueEntry();
						CacheEntry entry2 = (CacheEntry)hashtable[enumerator.Current];
						entry.Value = entry2.Value;
						if(entry.Value is CallbackEntry)
							entry.Value = ((CallbackEntry)entry.Value).Value;
						entry.Flag = entry2.Flag;
						hashtable[enumerator.Current] = entry;
					}
				}
				hashtable2 = hashtable;
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error : " + exception.ToString()).GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error : " + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("Get operation failed. Error : " + exception2.Message, exception2);
			}
			return hashtable2;
		}

		/// <summary>
		/// Recupera as chaves associadas com o grupo informado.
		/// </summary>
		/// <param name="group">Nome do grupo.</param>
		/// <param name="subGroup">Nome do subgrupo.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Lista das chaves.</returns>
		public ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			group.Require("group").NotNull();
			ArrayList list2;
			if(!this.IsRunning)
				return null;
			try
			{
				list2 = _context.CacheImpl.GetGroupKeys(group, subGroup, operationContext);
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
				{
					_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error :" + exception.ToString()).GetFormatter());
				}
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Get()".GetFormatter(), ("Get operation failed. Error :" + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("Get operation failed. Error :" + exception2.Message, exception2);
			}
			return list2;
		}

		/// <summary>
		/// Recupera as chaves das entradas com base na tags informadas.
		/// </summary>
		/// <param name="tags">Tags que serão usadas na pesquisa.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Lista das chaves.</returns>
		public ArrayList GetKeysByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			ArrayList list;
			if(!this.IsRunning)
				return null;
			try
			{
				list = _context.CacheImpl.GetKeysByTag(tags, comparisonType, operationContext);
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.GetKeysByTag()".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.GetKeysByTag()".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("GetKeysByTag operation failed. Error : " + exception2.Message, exception2);
			}
			return list;
		}

		/// <summary>
		/// Recupera o próximo pedaço.
		/// </summary>
		/// <param name="pointer">Ponteiro de navegação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public EnumerationDataChunk GetNextChunk(EnumerationPointer pointer, OperationContext operationContext)
		{
			EnumerationDataChunk nextChunk = null;
			if(!this.IsRunning)
				return null;
			try
			{
				nextChunk = _context.CacheImpl.GetNextChunk(pointer, operationContext);
			}
			catch(Exception exception)
			{
				_context.Logger.Error("Cache.GetNextChunk()".GetFormatter(), exception.GetFormatter());
				throw new OperationFailedException("GetNextChunk failed. Error : " + exception.Message, exception);
			}
			return nextChunk;
		}

		/// <summary>
		/// Recupera o tamanho a stream do item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada do cache.</param>
		/// <param name="lockHandle"></param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public long GetStreamLength(string key, string lockHandle, OperationContext operationContext)
		{
			return _context.CacheImpl.GetStreamLength(key, lockHandle, operationContext);
		}

		/// <summary>
		/// Recupera o mapeamento das informações do tipo.
		/// </summary>
		/// <returns></returns>
		public TypeInfoMap GetTypeInfoMap()
		{
			if(!this.IsRunning)
				return null;
			return _context.CacheImpl.TypeInfoMap;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties">Propriedades com as configurações.</param>
		/// <param name="inProc"></param>
		internal void Initialize(IDictionary properties, bool inProc)
		{
			this.Initialize(properties, inProc, false);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties">Propriedades de configuraçã.</param>
		/// <param name="inProc"></param>
		/// <param name="twoPhaseInitialization">Identifica se é para executar a segunda fase de inicialização.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void Initialize(IDictionary properties, bool inProc, bool twoPhaseInitialization)
		{
			_inProc = inProc;
			if(properties == null)
				throw new ArgumentNullException("properties");
			try
			{
				lock (this)
				{
					if(!properties.Contains("cache"))
						throw new ConfigurationException("Missing configuration attribute 'cache'");
					IDictionary config = (IDictionary)properties["cache"];
					if(config.Contains("name"))
						_cacheInfo.Name = Convert.ToString(config["name"]).Trim();
					if(config.Contains("log"))
						_context.Logger = new TextLogger();
					else
						_context.Logger = new DebugLogger();
					if(config.Contains("observer"))
					{
						var observer = config["observer"] as ICacheObserver;
						if(observer != null)
							Observer.Add(observer);
					}
					if(config.Contains("compression"))
					{
						this.CompressionThresholdSize(config["compression"] as IDictionary);
						_context.CompressionEnabled = _compressionEnabled;
						_context.CompressionThreshold = _compressionThresholdSize;
					}
					_context.SerializationContext = _cacheInfo.Name;
					_context.TimeSched = new TimeScheduler();
					_context.TimeSched.AddTask(new SystemMemoryTask(_context.Logger));
					_context.AsyncProc = new AsyncProcessor("Cache.Context", _context.Logger);
					_context.SyncManager = new CacheSyncManager(this, _context);
					if(config.Contains("backing-source"))
					{
						try
						{
							long timeout = 10000;
							IDictionary dictionary3 = (IDictionary)config["cache-classes"];
							if(dictionary3 != null)
							{
								dictionary3 = (IDictionary)dictionary3[_cacheInfo.Name.ToLower()];
								if((dictionary3 != null) && dictionary3.Contains("op-timeout"))
								{
									timeout = Convert.ToInt64(dictionary3["op-timeout"]);
									if(timeout < 100)
										timeout = 100;
									if(timeout > 30000)
										timeout = 30000;
								}
							}
							IDictionary dictionary4 = (IDictionary)config["backing-source"];
							_context.DatasourceMgr = new DatasourceMgr(this.Name, dictionary4, _context, timeout);
							if(_context.DatasourceMgr.IsReadThruEnabled)
								_context.DatasourceMgr.DefaultReadThruProvider = this.GetDefaultReadThruProvider();
							if(_context.DatasourceMgr.IsWriteThruEnabled)
								_context.DatasourceMgr.DefaultWriteThruProvider = this.GetDefaultWriteThruProvider();
						}
						catch(Exception exception)
						{
							if(exception is ConfigurationException)
							{
								_context.Logger.Error("Cache.Initialize()".GetFormatter(), exception.GetFormatter());
								string str2 = string.Format("Datasource provider (ReadThru/WriteThru) could not be initialized because of the following error: {0}", exception.Message);
								throw new Exception(str2);
							}
							_context.Logger.Error("Cache.Initialize()".GetFormatter(), exception.GetFormatter());
							string msg = string.Format("Failed to initialize datasource sync. read-through/write-through will not be available, Error {0}", exception.Message);
							throw new Exception(msg, exception);
						}
					}
					if(config.Contains("cache-loader"))
					{
						try
						{
							IDictionary properties2 = (IDictionary)config["cache-loader"];
							_context.CSLManager = new CacheStartupLoader(properties2, this, _context.Logger);
						}
						catch(ConfigurationException exception2)
						{
							_context.Logger.Error("Cache.Initialize()".GetFormatter(), exception2.GetFormatter());
						}
						catch(Exception exception3)
						{
							_context.Logger.Error("Cache.Initialize()".GetFormatter(), exception3.GetFormatter());
						}
					}
					this.CreateInternalCache(config, twoPhaseInitialization);
					if(_context.DatasourceMgr != null)
						_context.DatasourceMgr.StartWriteBehindProcessor();
					if(inProc && (_context.CacheImpl != null))
						_context.CacheImpl.KeepDeflattedValues = _context.CacheImpl is LocalCacheImpl;
					_cacheInfo.ConfigString = ConfigHelper.CreatePropertyString(properties);
					if((_context.CSLManager != null) && _context.CSLManager.IsCacheloaderEnabled)
					{
						try
						{
							LoadCacheTask task = new LoadCacheTask(_context.CSLManager);
							_context.CSLManager.Task = task;
							_context.CSLManager.Loaded += (sender, e) => OnLoaded();
							_context.CSLManager.InsertEntryError += (sender, e) => OnInsertEntryError(e);
							task.LoadError += (sender, e) => OnLoadError(e);
							task.LoadProcessingError += (sender, e) => OnLoadProcessingError(e);
							task.Start();
						}
						catch(Exception)
						{
						}
					}
				}
				if(_context.CSLManager == null || !_context.CSLManager.IsCacheloaderEnabled)
					OnLoaded();
				_context.Logger.CriticalInfo(("Cache '" + _context.CacheRoot.Name + "' started succesfully.").GetFormatter());
			}
			catch(ConfigurationException)
			{
				this.Dispose();
				throw;
			}
			catch(Exception exception4)
			{
				this.Dispose();
				throw new ConfigurationException("Configuration Error: " + exception4.Message, exception4);
			}
		}

		/// <summary>
		/// Inicializa a instancia
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="inProc"></param>
		/// <param name="userId"></param>
		/// <param name="password"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void Initialize2(IDictionary properties, bool inProc, string userId, string password)
		{
			properties.Require("properties").NotNull();
			_inProc = inProc;
			try
			{
				lock (this)
				{
					if(properties.Contains("name"))
						_cacheInfo.Name = Convert.ToString(properties["name"]).Trim();
					_context.Logger = new Colosoft.Logging.DebugLogger();
					_context.SerializationContext = _cacheInfo.Name;
					_context.TimeSched = new TimeScheduler();
					_context.TimeSched.AddTask(new SystemMemoryTask(_context.Logger));
					_context.AsyncProc = new AsyncProcessor("Cache.Context", _context.Logger);
					_context.SyncManager = new CacheSyncManager(this, _context);
					this.CreateInternalCache2(properties, userId, password);
					_cacheInfo.ConfigString = ConfigHelper.CreatePropertyString(properties);
				}
			}
			catch(ConfigurationException)
			{
				this.Dispose();
				throw;
			}
			catch(Exception exception)
			{
				this.Dispose();
				throw new ConfigurationException("Configuration Error: " + exception.Message, exception);
			}
		}

		/// <summary>
		/// Insere um novo item para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Instancia do item que será inserido.</param>
		/// <returns>Versão da entrada.</returns>
		public ulong Insert(object key, object value)
		{
			return this.Insert(key, value, null, null, null, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Insere um novo item para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Instancia do item que será inserido.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <returns></returns>
		public ulong Insert(object key, object value, ExpirationHint expiryHint)
		{
			return this.Insert(key, value, expiryHint, null, null, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Insere varias entrada para o cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas que serão inseridas.</param>
		/// <param name="entries">Instancia das entradas.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		private Hashtable Insert(object[] keys, CacheEntry[] entries, OperationContext operationContext)
		{
			Hashtable hashtable3;
			try
			{
				Hashtable hashtable = this.CascadedInsert(keys, entries, true, operationContext);
				if(hashtable != null)
				{
					IDictionaryEnumerator enumerator = ((Hashtable)hashtable.Clone()).GetEnumerator();
					while (enumerator.MoveNext())
					{
						CacheInsResultWithEntry entry = null;
						if(enumerator.Value is CacheInsResultWithEntry)
						{
							entry = (CacheInsResultWithEntry)enumerator.Value;
							switch(entry.Result)
							{
							case CacheInsResult.Success:
								hashtable.Remove(enumerator.Key);
								break;
							case CacheInsResult.SuccessOverwrite:
								hashtable.Remove(enumerator.Key);
								break;
							case CacheInsResult.NeedsEviction:
								hashtable[enumerator.Key] = new OperationFailedException("The cache is full and not enough items could be evicted.");
								break;
							case CacheInsResult.IncompatibleGroup:
								hashtable[enumerator.Key] = new OperationFailedException("Data group of the inserted item does not match the existing item's data group");
								break;
							}
						}
					}
				}
				hashtable3 = hashtable;
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Insert()".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Insert()".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Insert operation failed. Error : " + exception2.Message, exception2);
			}
			return hashtable3;
		}

		/// <summary>
		/// Insere uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="evictionHint"><see cref="EvictionHint"/> associado.</param>
		/// <returns></returns>
		public ulong Insert(object key, object value, EvictionHint evictionHint)
		{
			return this.Insert(key, value, null, null, evictionHint, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
		}

		/// <summary>
		/// Insere vários itens no cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas que serão inseridas.</param>
		/// <param name="values">Vetor com as entradas que serão inseridas.</param>
		/// <param name="expiryHint"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Insert(object[] keys, object[] values, ExpirationHint expiryHint, OperationContext operationContext)
		{
			return this.Insert(keys, values, expiryHint, null, null, null, operationContext);
		}

		/// <summary>
		/// Insere vários itens no cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas que serão inseridas.</param>
		/// <param name="values">Valor dos entradas.</param>
		/// <param name="evictionHint"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Insert(object[] keys, object[] values, EvictionHint evictionHint, OperationContext operationContext)
		{
			return this.Insert(keys, values, null, evictionHint, null, null, operationContext);
		}

		/// <summary>
		/// Insere uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="e">Instancia de entrada.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		private ulong Insert(object key, CacheEntry e, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			object obj1 = e.Value;
			try
			{
				CacheInsResultWithEntry entry = this.CascadedInsert(key, e, true, lockId, version, accessType, operationContext);
				switch(entry.Result)
				{
				case CacheInsResult.Success:
					return ((entry.Entry == null) ? ((ulong)1) : entry.Entry.Version);
				case CacheInsResult.SuccessOverwrite:
					return ((entry.Entry == null) ? ((ulong)1) : (entry.Entry.Version + 1));
				case CacheInsResult.SuccessNearEvicition:
				case CacheInsResult.SuccessOverwriteNearEviction:
				case CacheInsResult.Failure:
				case CacheInsResult.BucketTransfered:
					return 0;
				case CacheInsResult.NeedsEviction:
				case CacheInsResult.NeedsEvictionNotRemove:
					throw new OperationFailedException("The cache is full and not enough items could be evicted.", false);
				case CacheInsResult.IncompatibleGroup:
					throw new OperationFailedException("Data group of the inserted item does not match the existing item's data group.");
				case CacheInsResult.ItemLocked:
					throw new LockingException("Item is locked.");
				case CacheInsResult.VersionMismatch:
					throw new LockingException("Item does not exist at the specified version.");
				}
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable)
					_context.Logger.Error("Cache.Insert():".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.Insert():".GetFormatter(), exception2.GetFormatter());
				throw new OperationFailedException("Insert operation failed. Error : " + exception2.Message, exception2);
			}
			return 0;
		}

		/// <summary>
		/// Insere novas entradas no cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="values">Valores das entradas.</param>
		/// <param name="expiryHint"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Insert(object[] keys, object[] values, ExpirationHint expiryHint, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			IDictionary dictionary;
			if(keys == null)
				throw new ArgumentNullException("keys");
			if(values == null)
				throw new ArgumentNullException("items");
			if(keys.Length != values.Length)
				throw new ArgumentException("keys count is not equals to values count");
			CacheEntry[] entries = new CacheEntry[values.Length];
			for(int i = 0; i < values.Length; i++)
			{
				object obj2 = keys[i];
				object val = values[i];
				if(obj2 == null)
					throw new ArgumentNullException("key");
				if(val == null)
					throw new ArgumentNullException("value");
				if(!obj2.GetType().IsSerializable)
					throw new ArgumentException("key is not serializable");
				if(!val.GetType().IsSerializable)
					throw new ArgumentException("value is not serializable");
				if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
					throw new ArgumentException("expiryHint is not not serializable");
				if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
					throw new ArgumentException("evictionHint is not serializable");
				if(!this.IsRunning)
					return null;
				entries[i] = new CacheEntry(val, expiryHint, evictionHint);
				GroupInfo info = new GroupInfo(group, subGroup);
				entries[i].GroupInfo = info;
			}
			try
			{
				dictionary = this.Insert(keys, entries, operationContext);
			}
			catch(Exception)
			{
				throw;
			}
			return dictionary;
		}

		/// <summary>
		/// Insere uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public ulong Insert(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			return this.Insert(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, null, operationContext);
		}

		/// <summary>
		/// Insere uma nova entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="queryInfo"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public ulong Insert(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, Hashtable queryInfo, OperationContext operationContext)
		{
			return this.Insert(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, queryInfo, new BitSet(), operationContext);
		}

		/// <summary>
		/// Insere um nova entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="queryInfo"></param>
		/// <param name="flag"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public ulong Insert(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, Hashtable queryInfo, BitSet flag, OperationContext operationContext)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			if(value == null)
				throw new ArgumentNullException("value");
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!value.GetType().IsSerializable)
				throw new ArgumentException("value is not serializable");
			if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
				throw new ArgumentException("expiryHint is not not serializable");
			if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
				throw new ArgumentException("evictionHint is not serializable");
			if(!this.IsRunning)
				return 0;
			DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
			this.CheckDataSourceAvailabilityAndOptions(updateOpts);
			GroupInfo info = new GroupInfo(group, subGroup);
			CacheEntry e = new CacheEntry(value, expiryHint, evictionHint);
			e.GroupInfo = info;
			e.SyncDependency = syncDependency;
			e.QueryInfo = queryInfo;
			BitSet set1 = e.Flag;
			set1.Data = (byte)(set1.Data | flag.Data);
			ulong num = 0;
			try
			{
				num = this.Insert(key, e, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
				if(updateOpts == DataSourceUpdateOptions.WriteThru)
				{
					_context.DatasourceMgr.WriteThru(key as string, e, OpCode.Update, null, operationContext);
					return num;
				}
				if(updateOpts == DataSourceUpdateOptions.WriteBehind)
					_context.DatasourceMgr.WriteBehind(_context.CacheImpl, key, e, null, null, null, OpCode.Update, WriteBehindAsyncProcessor.TaskState.Execute);
			}
			catch(Exception)
			{
				throw;
			}
			return num;
		}

		/// <summary>
		/// Insere novas entradas no cache.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="callbackEnteries"></param>
		/// <param name="expirations"></param>
		/// <param name="syncDependencies"></param>
		/// <param name="evictions"></param>
		/// <param name="groupInfos"></param>
		/// <param name="queryInfos"></param>
		/// <param name="flags"></param>
		/// <param name="providername"></param>
		/// <param name="resyncProviderName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary Insert(object[] keys, object[] values, CallbackEntry[] callbackEnteries, ExpirationHint[] expirations, CacheSyncDependency[] syncDependencies, EvictionHint[] evictions, GroupInfo[] groupInfos, Hashtable[] queryInfos, BitSet[] flags, string providername, string resyncProviderName, OperationContext operationContext)
		{
			IDictionary dictionary2;
			if(keys == null)
				throw new ArgumentNullException("keys");
			if(values == null)
				throw new ArgumentNullException("items");
			if(keys.Length != values.Length)
				throw new ArgumentException("keys count is not equals to values count");
			DataSourceUpdateOptions updateOpts = this.UpdateOption(flags[0]);
			this.CheckDataSourceAvailabilityAndOptions(updateOpts);
			CacheEntry[] entries = new CacheEntry[values.Length];
			for(int i = 0; i < values.Length; i++)
			{
				if(keys[i] == null)
					throw new ArgumentNullException("key");
				if(values[i] == null)
					throw new ArgumentNullException("value");
				if(!keys[i].GetType().IsSerializable)
					throw new ArgumentException("key is not serializable");
				if(!values[i].GetType().IsSerializable)
					throw new ArgumentException("value is not serializable");
				if((expirations[i] != null) && !expirations[i].GetType().IsSerializable)
					throw new ArgumentException("expiryHint is not not serializable");
				if((evictions[i] != null) && !evictions[i].GetType().IsSerializable)
					throw new ArgumentException("evictionHint is not serializable");
				if(!this.IsRunning)
					return null;
				entries[i] = new CacheEntry(values[i], expirations[i], evictions[i]);
				entries[i].SyncDependency = syncDependencies[i];
				entries[i].GroupInfo = groupInfos[i];
				entries[i].QueryInfo = queryInfos[i];
				BitSet flag = entries[i].Flag;
				flag.Data = (byte)(flag.Data | flags[i].Data);
				entries[i].ProviderName = providername;
				if(callbackEnteries[i] != null)
				{
					CallbackEntry entry = callbackEnteries[i].Clone() as CallbackEntry;
					entry.Value = values[i];
					entry.Flag = entries[i].Flag;
					entries[i].Value = entry;
				}
			}
			try
			{
				IDictionary dictionary = this.Insert(keys, entries, operationContext);
				string[] strArray = null;
				object[] objArray = null;
				if((updateOpts != DataSourceUpdateOptions.None) && (keys.Length > dictionary.Count))
				{
					strArray = new string[keys.Length - dictionary.Count];
					objArray = new object[keys.Length - dictionary.Count];
					int index = 0;
					int num3 = 0;
					while (index < keys.Length)
					{
						if(!dictionary.Contains(keys[index]))
						{
							strArray[num3] = keys[index] as string;
							objArray[num3] = CompressionUtil.Decompress((values[index] as UserBinaryObject).GetFullObject(), entries[index].Flag);
							num3++;
						}
						index++;
					}
					if(updateOpts == DataSourceUpdateOptions.WriteThru)
						_context.DatasourceMgr.WriteThru(strArray, objArray, null, dictionary as Hashtable, OpCode.Update, null, operationContext);
					else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
					{
						CacheEntry[] entryArray2 = new CacheEntry[] {
							entries[0]
						};
						_context.DatasourceMgr.WriteBehind(_context.CacheImpl, strArray, objArray, entryArray2, null, null, providername, OpCode.Update, WriteBehindAsyncProcessor.TaskState.Execute);
					}
				}
				dictionary2 = dictionary;
			}
			catch(Exception)
			{
				throw;
			}
			return dictionary2;
		}

		/// <summary>
		/// Insere um nova entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="queryInfo"></param>
		/// <param name="flag"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="providerName"></param>
		/// <param name="resyncProviderName"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public ulong Insert(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, Hashtable queryInfo, BitSet flag, object lockId, ulong version, LockAccessType accessType, string providerName, string resyncProviderName, OperationContext operationContext)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			if(value == null)
				throw new ArgumentNullException("value");
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!value.GetType().IsSerializable)
				throw new ArgumentException("value is not serializable");
			if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
				throw new ArgumentException("expiryHint is not not serializable");
			if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
				throw new ArgumentException("evictionHint is not serializable");
			if(!this.IsRunning)
				return 0;
			DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
			this.CheckDataSourceAvailabilityAndOptions(updateOpts);
			CacheEntry e = new CacheEntry(value, expiryHint, evictionHint);
			GroupInfo info = new GroupInfo(group, subGroup);
			e.GroupInfo = info;
			e.SyncDependency = syncDependency;
			e.QueryInfo = queryInfo;
			BitSet set1 = e.Flag;
			set1.Data = (byte)(set1.Data + flag.Data);
			e.ResyncProviderName = resyncProviderName;
			e.ProviderName = providerName;
			ulong num = 0;
			try
			{
				num = this.Insert(key, e, lockId, version, accessType, operationContext);
				if(updateOpts == DataSourceUpdateOptions.WriteThru)
				{
					_context.DatasourceMgr.WriteThru(key as string, e, OpCode.Update, providerName, operationContext);
					return num;
				}
				if(updateOpts == DataSourceUpdateOptions.WriteBehind)
				{
					_context.DatasourceMgr.WriteBehind(_context.CacheImpl, key, e, null, null, providerName, OpCode.Update, WriteBehindAsyncProcessor.TaskState.Execute);
				}
			}
			catch(Exception)
			{
				throw;
			}
			return num;
		}

		/// <summary>
		/// Insere uma nova entrada de forma assincrona.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		public void InsertAsync(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, OperationContext operationContext)
		{
			this.InsertAsync(key, value, expiryHint, syncDependency, evictionHint, group, subGroup, new BitSet(), null, operationContext);
		}

		/// <summary>
		/// Insere um nova entrada de for a assincrona.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryHint"></param>
		/// <param name="syncDependency"></param>
		/// <param name="evictionHint"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="Flag"></param>
		/// <param name="queryInfo"></param>
		/// <param name="operationContext"></param>
		public void InsertAsync(object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, BitSet Flag, Hashtable queryInfo, OperationContext operationContext)
		{
			key.Require("key").NotNull();
			value.Require("value").NotNull();
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(!value.GetType().IsSerializable)
				throw new ArgumentException("value is not serializable");
			if((expiryHint != null) && !expiryHint.GetType().IsSerializable)
				throw new ArgumentException("expiryHint is not not serializable");
			if((evictionHint != null) && !evictionHint.GetType().IsSerializable)
				throw new ArgumentException("evictionHint is not serializable");
			if(this.IsRunning)
				_context.AsyncProc.Enqueue(new AsyncInsert(this, key, value, expiryHint, syncDependency, evictionHint, group, subGroup, Flag, queryInfo, operationContext));
		}

		/// <summary>
		/// Insere várias entradas de forma assincrona.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="operationContext"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void InsertAsyncEntry(object entry, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				CompactCacheEntry entry2 = (CompactCacheEntry)SerializationUtil.CompactDeserialize(entry, _context.SerializationContext);
				bool isAbsolute = false;
				bool flag2 = false;
				int num = 0;
				int options = entry2.Options;
				if(options != 0xff)
				{
					isAbsolute = Convert.ToBoolean((int)(options & 1));
					options = options >> 1;
					flag2 = Convert.ToBoolean((int)(options & 1));
					options = options >> 1;
					num = options - 2;
				}
				ExpirationHint expiryHint = ConvHelper.MakeExpirationHint(entry2.Expiration, isAbsolute);
				if((expiryHint != null) && (entry2.Dependency != null))
					expiryHint = new AggregateExpirationHint(new ExpirationHint[] {
						entry2.Dependency,
						expiryHint
					});
				if(expiryHint == null)
					expiryHint = entry2.Dependency;
				if((expiryHint != null) && flag2)
					expiryHint.SetBit(2);
				this.InsertAsync(entry2.Key, entry2.Value, expiryHint, entry2.SyncDependency, new PriorityEvictionHint((CacheItemPriority)num), entry2.Group, entry2.SubGroup, entry2.Flag, entry2.QueryInfo, operationContext);
			}
		}

		/// <summary>
		/// Insere novas entradas no cache.
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public IDictionary InsertEntries(object[] entries, OperationContext operationContext)
		{
			if(!this.IsRunning)
			{
				return null;
			}
			string[] keys = new string[entries.Length];
			object[] values = new object[entries.Length];
			CallbackEntry[] callbackEnteries = new CallbackEntry[entries.Length];
			ExpirationHint[] expirations = new ExpirationHint[entries.Length];
			EvictionHint[] evictions = new EvictionHint[entries.Length];
			CacheSyncDependency[] syncDependencies = new CacheSyncDependency[entries.Length];
			BitSet[] flags = new BitSet[entries.Length];
			Hashtable[] queryInfos = new Hashtable[entries.Length];
			GroupInfo[] groupInfos = new GroupInfo[entries.Length];
			CallbackEntry entry = null;
			for(int i = 0; i < entries.Length; i++)
			{
				CompactCacheEntry cce = (CompactCacheEntry)SerializationUtil.CompactDeserialize(entries[i], _context.SerializationContext);
				keys[i] = cce.Key as string;
				CacheEntry entry3 = this.MakeCacheEntry(cce);
				if(entry3 != null)
				{
					if(entry3.Value is CallbackEntry)
					{
						entry = entry3.Value as CallbackEntry;
					}
					else
					{
						entry = null;
					}
					callbackEnteries[i] = entry;
					object obj2 = entry3.Value as CallbackEntry;
					values[i] = (obj2 == null) ? entry3.Value : ((CallbackEntry)entry3.Value).Value;
					expirations[i] = entry3.ExpirationHint;
					evictions[i] = entry3.EvictionHint;
					syncDependencies[i] = entry3.SyncDependency;
					queryInfos[i] = entry3.QueryInfo;
					groupInfos[i] = entry3.GroupInfo;
					flags[i] = entry3.Flag;
				}
			}
			return this.Insert(keys, values, callbackEnteries, expirations, syncDependencies, evictions, groupInfos, queryInfos, flags, null, null, operationContext);
		}

		/// <summary>
		/// Insere um nova entrada.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public ulong InsertEntry(object entry, OperationContext operationContext)
		{
			if(!this.IsRunning)
				return 0;
			CompactCacheEntry cce = null;
			cce = (CompactCacheEntry)entry;
			CacheEntry entry3 = this.MakeCacheEntry(cce);
			string group = null;
			string subGroup = null;
			if((entry3.GroupInfo != null) && (entry3.GroupInfo.Group != null))
			{
				group = entry3.GroupInfo.Group;
				subGroup = entry3.GroupInfo.SubGroup;
			}
			return this.Insert(cce.Key, entry3.Value, entry3.ExpirationHint, entry3.SyncDependency, entry3.EvictionHint, group, subGroup, entry3.QueryInfo, entry3.Flag, entry3.LockId, entry3.Version, entry3.LockAccessType, null, entry3.ResyncProviderName, operationContext);
		}

		/// <summary>
		/// Verifica se o item do cache associado com a chave está bloqueado.
		/// </summary>
		/// <param name="key">Chave o item do cache.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public bool IsLocked(object key, ref object lockId, ref DateTime lockDate, OperationContext operationContext)
		{
			if(!this.IsRunning)
				return false;
			object objB = lockId;
			LockOptions options = _context.CacheImpl.IsLocked(key, ref lockId, ref lockDate, operationContext);
			if(options == null)
				return false;
			if(options.LockId == null)
				return false;
			lockId = options.LockId;
			lockDate = options.LockDate;
			return !object.Equals(options.LockId, objB);
		}

		/// <summary>
		/// Realiza um lock no item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="lockTimeout">Timeout do lock.</param>
		/// <param name="lockId">Identificador gerado para o lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Informações sobre o lock.</returns>
		public bool Lock(object key, TimeSpan lockTimeout, out object lockId, out DateTime lockDate, OperationContext operationContext)
		{
			lockId = null;
			lockDate = DateTime.UtcNow;
			LockExpiration lockExpiration = null;
			if(!TimeSpan.Equals(lockTimeout, TimeSpan.Zero))
				lockExpiration = new LockExpiration(lockTimeout);
			if(this.IsRunning)
			{
				object obj2 = lockId = this.GetLockId(key);
				LockOptions options = _context.CacheImpl.Lock(key, lockExpiration, ref lockId, ref lockDate, operationContext);
				if(options != null)
				{
					lockId = options.LockId;
					lockDate = options.LockDate;
					return obj2.Equals(options.LockId);
				}
				lockId = null;
			}
			return false;
		}

		/// <summary>
		/// Constrói uma entrada do cache.
		/// </summary>
		/// <param name="cce"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private CacheEntry MakeCacheEntry(CompactCacheEntry cce)
		{
			bool isAbsolute = false;
			bool flag2 = false;
			int num = 0;
			int options = cce.Options;
			if(options != 0xff)
			{
				isAbsolute = Convert.ToBoolean((int)(options & 1));
				options = options >> 1;
				flag2 = Convert.ToBoolean((int)(options & 1));
				options = options >> 1;
				num = options - 2;
			}
			ExpirationHint expiryHint = ConvHelper.MakeExpirationHint(cce.Expiration, isAbsolute);
			if((expiryHint != null) && (cce.Dependency != null))
				expiryHint = new AggregateExpirationHint(new ExpirationHint[] {
					cce.Dependency,
					expiryHint
				});
			if(expiryHint == null)
				expiryHint = cce.Dependency;
			if((expiryHint != null) && flag2)
				expiryHint.SetBit(2);
			CacheEntry entry = new CacheEntry(cce.Value, expiryHint, new PriorityEvictionHint((CacheItemPriority)num));
			if(cce.Group != null)
				entry.GroupInfo = new GroupInfo(cce.Group, cce.SubGroup);
			entry.QueryInfo = cce.QueryInfo;
			entry.Flag = cce.Flag;
			entry.SyncDependency = cce.SyncDependency;
			entry.LockId = cce.LockId;
			entry.LockAccessType = cce.LockAccessType;
			entry.Version = cce.Version;
			entry.ResyncProviderName = cce.ResyncProviderName;
			return entry;
		}

		/// <summary>
		/// Método acioando quando operação assincronna for completada.
		/// </summary>
		/// <param name="opCode"></param>
		/// <param name="result"></param>
		internal void OnAsyncOperationCompleted(AsyncOpCode opCode, object result)
		{
			if(_asyncOperationCompleted != null)
			{
				Delegate[] invocationList = _asyncOperationCompleted.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					AsyncOperationCompletedCallback callback = (AsyncOperationCompletedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(opCode, result, new AsyncCallback(this.AsyncOpAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnAsyncOperationCompletedCallback()".GetFormatter(), exception.GetFormatter());
						_asyncOperationCompleted = (AsyncOperationCompletedCallback)Delegate.Remove(_asyncOperationCompleted, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnAsyncOperationCompletedCallback".GetFormatter(), exception2.GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// Método acionado quando um item é removido.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="reason"></param>
		/// <param name="operationContext"></param>
		public void OnItemRemoved(object key, object value, ItemRemoveReason reason, OperationContext operationContext)
		{
			object obj2 = null;
			if(value != null)
				obj2 = ((CacheEntry)value).Value;
			key = SerializationUtil.CompactSerialize(key, _context.SerializationContext);
			if(_itemRemoved != null)
			{
				Delegate[] invocationList = _itemRemoved.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					ItemRemovedCallback callback = (ItemRemovedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(key, obj2, reason, ((CacheEntry)value).Flag, new AsyncCallback(this.RemoveAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnItemUpdated()".GetFormatter(), exception.GetFormatter());
						_itemRemoved = (ItemRemovedCallback)Delegate.Remove(_itemRemoved, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnItemUpdated".GetFormatter(), exception2.GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// Método acionado quando vários itens são removidos.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="value"></param>
		/// <param name="reason"></param>
		/// <param name="operationContext"></param>
		public void OnItemsRemoved(object[] keys, object[] value, ItemRemoveReason reason, OperationContext operationContext)
		{
			try
			{
				if(_itemRemoved != null)
				{
					for(int i = 0; i < keys.Length; i++)
						this.OnItemRemoved(keys[i], value[i], reason, operationContext);
				}
			}
			catch(Exception exception)
			{
				_context.Logger.Error("Cache.OnItemsRemoved()".GetFormatter(), exception.GetFormatter());
			}
		}

		/// <summary>
		/// Abre a stream para o cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="mode"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="expHint"></param>
		/// <param name="evictionHint"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public string OpenStream(string key, StreamModes mode, string group, string subGroup, ExpirationHint expHint, EvictionHint evictionHint, OperationContext operationContext)
		{
			string lockHandle = Guid.NewGuid().ToString() + DateTime.Now.Ticks;
			return this.OpenStream(key, lockHandle, mode, group, subGroup, expHint, evictionHint, operationContext);
		}

		/// <summary>
		/// Abre a stream para o cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="lockHandle"></param>
		/// <param name="mode"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="expHint"></param>
		/// <param name="evictionHint"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public string OpenStream(string key, string lockHandle, StreamModes mode, string group, string subGroup, ExpirationHint expHint, EvictionHint evictionHint, OperationContext operationContext)
		{
			if(_context.CacheImpl.OpenStream(key, lockHandle, mode, group, subGroup, expHint, evictionHint, operationContext))
				return lockHandle;
			return null;
		}

		/// <summary>
		/// Lê os dados da stream do cache.
		/// </summary>
		/// <param name="vBuffer"></param>
		/// <param name="key"></param>
		/// <param name="lockHandle"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public int ReadFromStream(ref VirtualArray vBuffer, string key, string lockHandle, int offset, int length, OperationContext operationContext)
		{
			return _context.CacheImpl.ReadFromStream(ref vBuffer, key, lockHandle, offset, length, operationContext);
		}

		/// <summary>
		/// Regista callbacks para notificações associadas com as chaves.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="updateCallback"></param>
		/// <param name="removeCallback"></param>
		/// <param name="operationContext"></param>
		public void RegisterKeyNotificationCallback(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				keys.Require("keys").NotNull();
				if(keys.Length == 0)
					throw new ArgumentException("Keys count can not be zero");
				if((updateCallback == null) && (removeCallback == null))
					throw new ArgumentNullException();
				try
				{
					_context.CacheImpl.RegisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.RegisterKeyNotificationCallback() ".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.RegisterKeyNotificationCallback() ".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("RegisterKeyNotification failed. Error : " + exception2.Message, exception2);
				}
			}
		}

		/// <summary>
		/// Registra uma chave de notificação para uma entrada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="updateCallback"></param>
		/// <param name="removeCallback"></param>
		/// <param name="operationContext"></param>
		public void RegisterKeyNotificationCallback(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				key.Require("key").NotNull();
				if((updateCallback == null) && (removeCallback == null))
					throw new ArgumentNullException();
				try
				{
					_context.CacheImpl.RegisterKeyNotification(key, updateCallback, removeCallback, operationContext);
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.RegisterKeyNotificationCallback() ".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.RegisterKeyNotificationCallback() ".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("RegisterKeyNotification failed. Error : " + exception2.Message, exception2);
				}
			}
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Valor da entrada removida.</returns>
		public CompressedValueEntry Remove(object key, OperationContext operationContext)
		{
			return this.Remove(key as string, new BitSet(), null, null, 0, LockAccessType.IGNORE_LOCK, null, operationContext);
		}

		/// <summary>
		/// Remove as entradas pelo grupo informado.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		public void Remove(string group, string subGroup, OperationContext operationContext)
		{
			group.Require("group").NotNull();
			if(this.IsRunning)
			{
				try
				{
					Hashtable hashtable = this.CascadedRemove(group, subGroup, true, operationContext);
				}
				catch(Exception exception)
				{
					_context.Logger.Error("Cache.Remove()".GetFormatter(), exception.GetFormatter());
					throw new OperationFailedException("Remove operation failed. Error : " + exception.Message, exception);
				}
			}
		}

		/// <summary>
		/// Remove as entradas associadas com as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="flagMap"></param>
		/// <param name="cbEntry"></param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="operationContext">Contexto da opera~çao.</param>
		/// <returns></returns>
		public IDictionary Remove(object[] keys, BitSet flagMap, CallbackEntry cbEntry, string providerName, OperationContext operationContext)
		{
			IDictionary dictionary2;
			if(keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if(!this.IsRunning)
			{
				return null;
			}
			DataSourceUpdateOptions updateOpts = this.UpdateOption(flagMap);
			this.CheckDataSourceAvailabilityAndOptions(updateOpts);
			try
			{
				IDictionary dictionary = this.CascadedRemove(keys, ItemRemoveReason.Removed, true, operationContext);
				if((((updateOpts != DataSourceUpdateOptions.None) && (dictionary != null)) && (dictionary.Count > 0)))
				{
					string[] array = null;
					CacheEntry[] entryArray = null;
					array = new string[dictionary.Count];
					entryArray = new CacheEntry[dictionary.Count];
					int index = 0;
					for(int i = 0; i < keys.Length; i++)
					{
						if(dictionary[keys[i]] is CacheEntry)
						{
							array[index] = keys[i] as string;
							entryArray[index++] = dictionary[keys[i]] as CacheEntry;
						}
					}
					if(dictionary.Count > index)
					{
						Resize(ref array, index);
						Resize(ref entryArray, index);
					}
					if(updateOpts == DataSourceUpdateOptions.WriteThru)
					{
						_context.DatasourceMgr.WriteThru(array, null, entryArray, dictionary as Hashtable, OpCode.Remove, providerName, operationContext);
					}
					else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
					{
						if(cbEntry != null)
						{
							if(entryArray[0].Value is CallbackEntry)
							{
								((CallbackEntry)entryArray[0].Value).WriteBehindOperationCompletedCallback = cbEntry.WriteBehindOperationCompletedCallback;
							}
							else
							{
								cbEntry.Value = entryArray[0].Value;
								entryArray[0].Value = cbEntry;
							}
						}
						_context.DatasourceMgr.WriteBehind(_context.CacheImpl, array, null, entryArray, null, null, providerName, OpCode.Remove, WriteBehindAsyncProcessor.TaskState.Execute);
					}
				}
				CompressedValueEntry entry = null;
				if(dictionary != null)
				{
					object[] objArray = new object[dictionary.Count];
					dictionary.Keys.CopyTo(objArray, 0);
					IEnumerator enumerator = objArray.GetEnumerator();
					while (enumerator.MoveNext())
					{
						CacheEntry entry2 = dictionary[enumerator.Current] as CacheEntry;
						entry = new CompressedValueEntry();
						entry.Value = entry2.Value;
						if(entry.Value is CallbackEntry)
						{
							entry.Value = ((CallbackEntry)entry.Value).Value;
						}
						entry.Flag = entry2.Flag;
						dictionary[enumerator.Current] = entry;
					}
				}
				dictionary2 = dictionary;
			}
			catch(Exception exception)
			{
				_context.Logger.Error("Cache.Remove()".GetFormatter(), exception.GetFormatter());
				throw new OperationFailedException("Remove operation failed. Error : " + exception.Message, exception);
			}
			return dictionary2;
		}

		/// <summary>
		/// Remove uma entrada do cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="flag"></param>
		/// <param name="cbEntry">Callback que será usado.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="operationContext">Contexto da opera~çao.</param>
		/// <returns></returns>
		public CompressedValueEntry Remove(string key, BitSet flag, CallbackEntry cbEntry, object lockId, ulong version, LockAccessType accessType, string providerName, OperationContext operationContext)
		{
			key.Require("key").NotNull();
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(this.IsRunning)
			{
				DataSourceUpdateOptions updateOpts = this.UpdateOption(flag);
				this.CheckDataSourceAvailabilityAndOptions(updateOpts);
				try
				{
					object pack = key;
					CacheEntry val = this.CascadedRemove(key, pack, ItemRemoveReason.Removed, true, lockId, version, accessType, operationContext);
					if(val != null)
					{
						if(updateOpts == DataSourceUpdateOptions.WriteThru)
						{
							_context.DatasourceMgr.WriteThru(key, val, OpCode.Remove, providerName, operationContext);
						}
						else if(updateOpts == DataSourceUpdateOptions.WriteBehind)
						{
							if(cbEntry != null)
							{
								if(val.Value is CallbackEntry)
									((CallbackEntry)val.Value).WriteBehindOperationCompletedCallback = cbEntry.WriteBehindOperationCompletedCallback;
								else
								{
									cbEntry.Value = val.Value;
									val.Value = cbEntry;
								}
							}
							_context.DatasourceMgr.WriteBehind(_context.CacheImpl, key, val, null, null, providerName, OpCode.Remove, WriteBehindAsyncProcessor.TaskState.Execute);
						}
					}
					if(val != null)
					{
						CompressedValueEntry entry2 = new CompressedValueEntry();
						entry2.Value = val.Value;
						entry2.Flag = val.Flag;
						if(entry2.Value is CallbackEntry)
						{
							entry2.Value = ((CallbackEntry)entry2.Value).Value;
						}
						return entry2;
					}
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.Remove()".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.Remove()".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("Remove operation failed. Error : " + exception2.Message, exception2);
				}
			}
			return null;
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada, de forma assincrona.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="operationContext"></param>
		public void RemoveAsync(object key, OperationContext operationContext)
		{
			key.Require("key").NotNull();
			if(!key.GetType().IsSerializable)
				throw new ArgumentException("key is not serializable");
			if(this.IsRunning)
				_context.AsyncProc.Enqueue(new AsyncRemove(this, key, operationContext));
		}

		internal void RemoveAsyncCallbackHandler(IAsyncResult ar)
		{
			ItemRemovedCallback asyncState = (ItemRemovedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.RemoveAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_itemRemoved)
				{
					_itemRemoved = (ItemRemovedCallback)Delegate.Remove(_itemRemoved, asyncState);
				}
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.RemoveAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Remove as entradas palas tags informadas.
		/// </summary>
		/// <param name="sTags"></param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void RemoveByTag(string[] sTags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				try
				{
					this.CascadedRemove(sTags, comparisonType, true, operationContext);
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
					{
						_context.Logger.Error("Cache.RemoveByTag()".GetFormatter(), exception.GetFormatter());
					}
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.RemoveByTag()".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("RemoveByTag operation failed. Error : " + exception2.Message, exception2);
				}
			}
		}

		/// <summary>
		/// Redimeciona o vetor informado.
		/// </summary>
		/// <param name="array">Vetor.</param>
		/// <param name="newLength">Novo tamanho.</param>
		internal static void Resize(ref CacheEntry[] array, int newLength)
		{
			if((array != null) && (array.Length != newLength))
			{
				CacheEntry[] entryArray = new CacheEntry[newLength];
				for(int i = 0; i < newLength; i++)
				{
					if(i >= array.Length)
					{
						break;
					}
					entryArray[i] = array[i];
				}
				array = entryArray;
			}
		}

		/// <summary>
		/// Redimeciona o vetor informado.
		/// </summary>
		/// <param name="array">Vetor.</param>
		/// <param name="newLength">Novo tamanho.</param>
		internal static void Resize(ref object[] array, int newLength)
		{
			if((array != null) && (array.Length != newLength))
			{
				object[] objArray = new object[newLength];
				for(int i = 0; i < newLength; i++)
				{
					if(i >= array.Length)
					{
						break;
					}
					objArray[i] = array[i];
				}
				array = objArray;
			}
		}

		/// <summary>
		/// Redimeciona o vetor informado.
		/// </summary>
		/// <param name="array">Vetor.</param>
		/// <param name="newLength">Novo tamanho.</param>
		internal static void Resize(ref string[] array, int newLength)
		{
			if((array != null) && (array.Length != newLength))
			{
				string[] strArray = new string[newLength];
				for(int i = 0; i < newLength; i++)
				{
					if(i >= array.Length)
					{
						break;
					}
					strArray[i] = array[i];
				}
				array = strArray;
			}
		}

		/// <summary>
		/// Pesquisa os dados com base na consulta informada.
		/// </summary>
		/// <param name="query">Texto da consulta.</param>
		/// <param name="values">Parametros da consulta.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public QueryResultSet Search(string query, IDictionary values, OperationContext operationContext = null)
		{
			query.Require("query").NotNull().NotEmpty();
			QueryResultSet set;
			if(!this.IsRunning)
				return null;
			try
			{
				set = _context.CacheImpl.Search(query, values, operationContext);
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable && _context.Logger.IsErrorEnabled)
					_context.Logger.Error(("search operation failed. Error: " + exception.ToString()).GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				if(_context.Logger.IsErrorEnabled)
					_context.Logger.Error(("search operation failed. Error: " + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("search operation failed. Error: " + exception2.Message, exception2);
			}
			return set;
		}

		/// <summary>
		/// Realiza uma pesquisa nas entradas do cache.
		/// </summary>
		/// <param name="query">Texto da consulta.</param>
		/// <param name="values">Valores do consulta.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public QueryResultSet SearchEntries(string query, IDictionary values, OperationContext operationContext)
		{
			QueryResultSet set;
			if(!this.IsRunning)
				return null;
			query.Require("query").NotNull().NotEmpty();
			try
			{
				set = _context.CacheImpl.SearchEntries(query, values, operationContext);
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable && _context.Logger.IsErrorEnabled)
					_context.Logger.Error(("search operation failed. Error: " + exception.ToString()).GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				if(_context.Logger.IsErrorEnabled)
					_context.Logger.Error(("search operation failed. Error: " + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("search operation failed. Error: " + exception2.Message, exception2);
			}
			return set;
		}

		/// <summary>
		/// Realiza a junção dos indices associados aos tipos informados.
		/// </summary>
		/// <param name="leftTypeName">Nome do tipo da esquerda.</param>
		/// <param name="leftFieldName">Nome do campo do tipo da esquerda.</param>
		/// <param name="rightTypeName">Nome do tipo da direita.</param>
		/// <param name="rightFieldName">Nome do campo do tipo da direita.</param>
		/// <param name="comparisonType">Tipo de comparação que será realizado.</param>
		/// <returns></returns>
		public IEnumerable<object[]> JoinIndex(string leftTypeName, string leftFieldName, string rightTypeName, string rightFieldName, ComparisonType comparisonType)
		{
			IEnumerable<object[]> set;
			if(!this.IsRunning)
				return null;
			try
			{
				set = _context.CacheImpl.JoinIndex(leftTypeName, leftFieldName, rightTypeName, rightFieldName, comparisonType);
			}
			catch(OperationFailedException exception)
			{
				if(exception.IsTracable && _context.Logger.IsErrorEnabled)
					_context.Logger.Error(("join index operation failed. Error: " + exception.ToString()).GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				if(_context.Logger.IsErrorEnabled)
					_context.Logger.Error(("join index operation failed. Error: " + exception2.ToString()).GetFormatter());
				throw new OperationFailedException("join index operation failed. Error: " + exception2.Message, exception2);
			}
			return set;
		}

		/// <summary>
		/// Inicia o cache.
		/// </summary>
		/// <param name="twoPhaseInitialization"></param>
		protected internal virtual void Start(bool twoPhaseInitialization)
		{
			try
			{
				if(this.IsRunning)
					this.Stop();
				ConfigReader reader = new PropsConfigReader(this.ConfigString);
				this.Initialize(reader.Properties, false, twoPhaseInitialization);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Para a exeução do cache.
		/// </summary>
		protected internal virtual void Stop()
		{
			this.Dispose();
		}

		/// <summary>
		/// Remove o lock da entrada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="isPreemptive"></param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void Unlock(object key, object lockId, bool isPreemptive, OperationContext operationContext)
		{
			if(this.IsRunning)
				_context.CacheImpl.UnLock(key, lockId, isPreemptive, operationContext);
		}

		/// <summary>
		/// Remove o registro de notificação para as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="updateCallback">Callback de atualização.</param>
		/// <param name="removeCallback">Callback de remoção.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void UnregisterKeyNotificationCallback(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				keys.Require("keys").NotNull();
				if(keys.Length == 0)
					throw new ArgumentException("Keys count can not be zero");
				if((updateCallback == null) && (removeCallback == null))
					throw new ArgumentNullException();
				try
				{
					_context.CacheImpl.UnregisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.UnregisterKeyNotificationCallback() ".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.UnregisterKeyNotificationCallback()".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("UnregisterKeyNotification failed. Error : " + exception2.Message, exception2);
				}
			}
		}

		/// <summary>
		/// Remove o registro de notificação para a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="updateCallback">Callback de atualização.</param>
		/// <param name="removeCallback">Callback de remoção.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void UnregisterKeyNotificationCallback(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(this.IsRunning)
			{
				key.Require("key").NotNull();
				if((updateCallback == null) && (removeCallback == null))
					throw new ArgumentNullException();
				try
				{
					_context.CacheImpl.UnregisterKeyNotification(key, updateCallback, removeCallback, operationContext);
				}
				catch(OperationFailedException exception)
				{
					if(exception.IsTracable)
						_context.Logger.Error("Cache.UnregisterKeyNotificationCallback() ".GetFormatter(), exception.GetFormatter());
					throw;
				}
				catch(Exception exception2)
				{
					_context.Logger.Error("Cache.UnregisterKeyNotificationCallback()".GetFormatter(), exception2.GetFormatter());
					throw new OperationFailedException("UnregisterKeyNotification failed. Error : " + exception2.Message, exception2);
				}
			}
		}

		/// <summary>
		/// Método acionado pelo callback de atualização.
		/// </summary>
		/// <param name="ar"></param>
		internal void UpdateAsyncCallbackHandler(IAsyncResult ar)
		{
			ItemUpdatedCallback asyncState = (ItemUpdatedCallback)ar.AsyncState;
			try
			{
				asyncState.EndInvoke(ar);
			}
			catch(SocketException exception)
			{
				_context.Logger.Error("Cache.UpdateAsyncCallbackHandler".GetFormatter(), exception.GetFormatter());
				lock (_itemUpdated)
				{
					_itemUpdated = (ItemUpdatedCallback)Delegate.Remove(_itemUpdated, asyncState);
				}
			}
			catch(Exception exception2)
			{
				_context.Logger.Error("Cache.UpdateAsyncCallbackHandler".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Atualiza as opções da instancia.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private DataSourceUpdateOptions UpdateOption(BitSet flag)
		{
			if(flag.IsBitSet(4))
				return DataSourceUpdateOptions.WriteThru;
			if(flag.IsBitSet(8))
				return DataSourceUpdateOptions.WriteBehind;
			return DataSourceUpdateOptions.None;
		}

		/// <summary>
		/// Escreve para a stream do cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="lockHandle">Nome do manipulador do lock.</param>
		/// <param name="vBuffer">Buffer dos dados.</param>
		/// <param name="srcOffset">Offset da origem.</param>
		/// <param name="dstOffset">Offset do destino.</param>
		/// <param name="length">Tamanho dos dados.</param>
		/// <param name="operationContext">Contexto da opera~çao.</param>
		public void WriteToStream(string key, string lockHandle, VirtualArray vBuffer, int srcOffset, int dstOffset, int length, OperationContext operationContext)
		{
			try
			{
				_context.CacheImpl.WriteToStream(key, lockHandle, vBuffer, srcOffset, dstOffset, length, operationContext);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Notifica que oi cache foi limpo.
		/// </summary>
		public void OnCacheCleared()
		{
			if(_cacheCleared != null)
			{
				Delegate[] invocationList = _cacheCleared.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					CacheClearedCallback callback = (CacheClearedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(new AsyncCallback(this.CacheClearAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnCacheCleared()".GetFormatter(), exception.GetFormatter());
						_cacheCleared = (CacheClearedCallback)Delegate.Remove(_cacheCleared, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnCacheCleared".GetFormatter(), exception2.GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// Dispara um evento customizado.
		/// </summary>
		/// <param name="notifId"></param>
		/// <param name="data"></param>
		public void OnCustomEvent(object notifId, object data)
		{
			if(_cusotmNotif != null)
			{
				Delegate[] invocationList = _cusotmNotif.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					CustomNotificationCallback callback = (CustomNotificationCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(notifId, data, new AsyncCallback(this.CustomEventAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnCustomEvent()".GetFormatter(), exception.ToString().GetFormatter());
						_cusotmNotif = (CustomNotificationCallback)Delegate.Remove(_cusotmNotif, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnCustomEvent".GetFormatter(), exception2.ToString().GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// Dispara o callback de remoção customizado.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value"></param>
		/// <param name="reason">Razão da remoção.</param>
		public void OnCustomRemoveCallback(object key, object value, ItemRemoveReason reason)
		{
			CallbackEntry entry = value as CallbackEntry;
			if(((entry != null) && (entry.ItemRemoveCallbackListener != null)) && (entry.ItemRemoveCallbackListener.Count > 0))
			{
				foreach (CallbackInfo info in entry.ItemRemoveCallbackListener)
				{
				}
			}
		}

		/// <summary>
		/// Dispara o callback de atualização customizada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor.</param>
		public void OnCustomUpdateCallback(object key, object value)
		{
			ArrayList list = value as ArrayList;
			if((list != null) && (list.Count > 0))
			{
				list = list.Clone() as ArrayList;
				foreach (CallbackInfo info in list)
				{
				}
			}
		}

		/// <summary>
		/// Dispara o callback que identifica que uma nova entrada foi adicionada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		public void OnItemAdded(object key)
		{
			if(_itemAdded != null)
			{
				key = SerializationUtil.CompactSerialize(key, _context.SerializationContext);
				Delegate[] invocationList = _itemAdded.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					ItemAddedCallback callback = (ItemAddedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(key, new AsyncCallback(this.AddAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnItemAdded()".GetFormatter(), exception.GetFormatter());
						_itemAdded = (ItemAddedCallback)Delegate.Remove(_itemAdded, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnItemAdded".GetFormatter(), exception2.GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// Dispara o callback que identifica que o item foi atualizado.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		public void OnItemUpdated(object key, OperationContext operationContext)
		{
			if(_itemUpdated != null)
			{
				key = SerializationUtil.CompactSerialize(key, _context.SerializationContext);
				Delegate[] invocationList = _itemUpdated.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					ItemUpdatedCallback callback = (ItemUpdatedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(key, new AsyncCallback(this.UpdateAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnItemUpdated()".GetFormatter(), exception.ToString().GetFormatter());
						_itemUpdated = (ItemUpdatedCallback)Delegate.Remove(_itemUpdated, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnItemUpdated".GetFormatter(), exception2.ToString().GetFormatter());
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operationCode"></param>
		/// <param name="result"></param>
		/// <param name="cbEntry"></param>
		public void OnWriteBehindOperationCompletedCallback(OpCode operationCode, object result, CallbackEntry cbEntry)
		{
			if((cbEntry.WriteBehindOperationCompletedCallback != null) && (_dataSourceUpdated != null))
			{
				Delegate[] invocationList = _dataSourceUpdated.GetInvocationList();
				for(int i = invocationList.Length - 1; i >= 0; i--)
				{
					DataSourceUpdatedCallback callback = (DataSourceUpdatedCallback)invocationList[i];
					try
					{
						callback.BeginInvoke(result, cbEntry, operationCode, new AsyncCallback(this.DSUpdateEventAsyncCallbackHandler), callback);
					}
					catch(SocketException exception)
					{
						_context.Logger.Error("Cache.OnWriteBehindOperationCompletedCallback".GetFormatter(), exception.ToString().GetFormatter());
						_dataSourceUpdated = (DataSourceUpdatedCallback)Delegate.Remove(_dataSourceUpdated, callback);
					}
					catch(Exception exception2)
					{
						_context.Logger.Error("Cache.OnWriteBehindOperationCompletedCallback".GetFormatter(), exception2.ToString().GetFormatter());
					}
				}
			}
		}
	}
}
