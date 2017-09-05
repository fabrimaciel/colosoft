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
using Colosoft.Threading;
using System.IO;
using System.Collections;
using Colosoft.Caching.Data;
using Colosoft.Serialization;
using Colosoft.Caching.Expiration;
using System.Threading;
using Colosoft.Caching.Queries;
using Colosoft.Caching.Policies;
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching
{
	/// <summary>
	/// Possíveis modos de stream.
	/// </summary>
	public enum StreamModes : short
	{
		/// <summary>
		/// Leitura.
		/// </summary>
		Read = 0,
		/// <summary>
		/// Leitura sem lock.
		/// </summary>
		ReadWithoutLock = 1,
		/// <summary>
		/// Escrita.
		/// </summary>
		Write = 2
	}
	/// <summary>
	/// Implementação básica dos métodos.
	/// </summary>
	internal partial class CacheBase : ICache, IDisposable
	{
		protected CacheRuntimeContext _context;

		private bool _isInProc;

		private bool _keepDeflattedObjects;

		private ICacheEventsListener _listener;

		private string _name;

		private Notifications _notifiers;

		protected ReaderWriterLock _syncObj;

		private StreamWriter _writer;

		/// <summary>
		/// Contexto do cache,
		/// </summary>
		public CacheRuntimeContext Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Recupera a quantidade de itens no cache.
		/// </summary>
		public virtual long Count
		{
			get
			{
				return 0;
			}
		}

		public virtual ArrayList DataGroupList
		{
			get
			{
				return null;
			}
		}

		internal virtual float EvictRatio
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		/// <summary>
		/// Instancia do cache interno.
		/// </summary>
		protected internal virtual CacheBase InternalCache
		{
			get
			{
				return null;
			}
		}

		protected bool IsCacheClearNotifier
		{
			get
			{
				return ((this.Notifiers & Notifications.CacheClear) == Notifications.CacheClear);
			}
		}

		/// <summary>
		/// Identifica se a liberação é permitida no cache.
		/// </summary>
		public virtual bool IsEvictionAllowed
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		/// <summary>
		/// Identifica se o cache está executando como processo.
		/// </summary>
		public bool IsInProc
		{
			get
			{
				return _isInProc;
			}
			set
			{
				_isInProc = value;
			}
		}

		/// <summary>
		/// Verifica se está ativa a notificação de novo item.
		/// </summary>
		protected bool IsItemAddNotifier
		{
			get
			{
				return ((this.Notifiers & Notifications.ItemAdd) == Notifications.ItemAdd);
			}
		}

		/// <summary>
		/// Identifica se o notificador de item removido está ativo.
		/// </summary>
		protected bool IsItemRemoveNotifier
		{
			get
			{
				return ((this.Notifiers & Notifications.ItemRemove) == Notifications.ItemRemove);
			}
		}

		/// <summary>
		/// Identifica se o notificador de item atualizado está ativo.
		/// </summary>
		protected bool IsItemUpdateNotifier
		{
			get
			{
				return ((this.Notifiers & Notifications.ItemUpdate) == Notifications.ItemUpdate);
			}
		}

		public bool KeepDeflattedValues
		{
			get
			{
				return _keepDeflattedObjects;
			}
			set
			{
				_keepDeflattedObjects = value;
			}
		}

		/// <summary>
		/// Chaves associadas com o cache.
		/// </summary>
		public virtual Array Keys
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Listener dos eventos.
		/// </summary>
		public virtual ICacheEventsListener Listener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		/// <summary>
		/// Tamanho máximo.
		/// </summary>
		internal virtual long MaxSize
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Nome da instancia.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public Colosoft.Logging.ILogger Logger
		{
			get
			{
				return _context.Logger;
			}
		}

		/// <summary>
		/// Notificações da instancia.
		/// </summary>
		public virtual Notifications Notifiers
		{
			get
			{
				return _notifiers;
			}
			set
			{
				_notifiers = value;
			}
		}

		public virtual ulong OperationSequenceId
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Analizador de consulta.
		/// </summary>
		public virtual ActiveQueryAnalyzer QueryAnalyzer
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Identifica se o cache requer replicação.
		/// </summary>
		internal virtual bool RequiresReplication
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Tamanho do cache.
		/// </summary>
		internal virtual long Size
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Sincronizador usado para leitura e escrita.
		/// </summary>
		public ReaderWriterLock Sync
		{
			get
			{
				return _syncObj;
			}
		}

		/// <summary>
		/// Mapa das informações dos tipos.
		/// </summary>
		public virtual TypeInfoMap TypeInfoMap
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected CacheBase()
		{
			_name = string.Empty;
		}

		/// <summary>
		/// Cria a instancia já definindo as propriedades os listener de ventos o contexto.
		/// </summary>
		/// <param name="properties">Propriedades</param>
		/// <param name="listener"></param>
		/// <param name="context">Contexto.</param>
		public CacheBase(IDictionary properties, ICacheEventsListener listener, CacheRuntimeContext context)
		{
			_name = string.Empty;
			_context = context;
			_listener = listener;
			_syncObj = new ReaderWriterLock();
		}

		/// <summary>
		/// Adiciona a entrada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="expirationHint"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual bool Add(object key, ExpirationHint expirationHint, OperationContext operationContext)
		{
			return this.Add(key, null, expirationHint, operationContext);
		}

		public virtual bool Add(object key, Colosoft.Caching.Synchronization.CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			return false;
		}

		/// <summary>
		/// Adiciona uma nova entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cacheEntry"></param>
		/// <param name="notify"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, OperationContext operationContext)
		{
			return CacheAddResult.Failure;
		}

		public virtual Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			return null;
		}

		public virtual bool Add(object key, string group, ExpirationHint eh, OperationContext operationContext)
		{
			return false;
		}

		public virtual bool Add(object key, string group, Colosoft.Caching.Synchronization.CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			return false;
		}

		public virtual CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			return CacheAddResult.Failure;
		}

		public virtual Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, string taskId, OperationContext operationContext)
		{
			return null;
		}

		public virtual CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, string taskId, OperationContext operationContext)
		{
			return CacheAddResult.Failure;
		}

		/// <summary>
		/// Baseado no <see cref="Hashtable"/> informado associa chaves de dependencia.
		/// A chave do <see cref="Hashtable"/> é a chave do item e o valor é a lista das dependencias.
		/// </summary>
		/// <param name="table">Tabela contendo os dados que serão processados.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns><see cref="Hashtable"/> dos resultados associados com as chaves informadas.</returns>
		public virtual Hashtable AddDependencyKeyList(Hashtable table, OperationContext operationContext)
		{
			return null;
		}

		public virtual void AddLoggedData(ArrayList bucketIds)
		{
		}

		internal virtual void CacheBecomeActive()
		{
		}

		internal virtual bool CanChangeCacheSize(long size)
		{
			return false;
		}

		public virtual Hashtable Cascaded_remove(Hashtable keyValues, ItemRemoveReason ir, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			return null;
		}

		public virtual void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, OperationContext operationContext)
		{
		}

		public virtual void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, string taskId, OperationContext operationContext)
		{
		}

		public virtual void CloseStream(string key, string lockHandle, OperationContext operationContext)
		{
		}

		public virtual bool Contains(object key, OperationContext operationContext)
		{
			return this.Contains(key, null, operationContext);
		}

		public virtual Hashtable Contains(object[] keys, OperationContext operationContext)
		{
			return this.Contains(keys, null, operationContext);
		}

		public virtual Hashtable Contains(object[] keys, string group, OperationContext operationContext)
		{
			return null;
		}

		public virtual bool Contains(object key, string group, OperationContext operationContext)
		{
			return false;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
		public virtual void Dispose()
		{
			if(_writer != null)
			{
				lock (_writer)
				{
					_writer.Close();
					_writer = null;
				}
			}
			_listener = null;
			_syncObj = null;
			GC.SuppressFinalize(this);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
		public virtual void DoWrite(string module, string message, OperationContext operationContext)
		{
			if(_writer != null)
				lock (_writer)
					_writer.WriteLine("[" + module + "]" + message + "\t" + DateTime.Now.ToLongTimeString());
		}

		internal virtual void EnqueueForReplication(object key, int opCode, object data)
		{
		}

		internal virtual void EnqueueForReplication(object key, int opCode, object data, int size, Array userPayLoad, long payLoadSize)
		{
		}

		/// <summary>
		/// Extrai as chave da tabela informada.
		/// </summary>
		/// <param name="table"></param>
		/// <returns>Vetor com as chaves.</returns>
		protected string[] ExtractKeys(Hashtable table)
		{
			Hashtable hashtable = new Hashtable();
			IDictionaryEnumerator enumerator = table.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if(enumerator.Value != null)
				{
					Hashtable keysDependingOnMe = null;
					if(enumerator.Value is CacheEntry)
						keysDependingOnMe = ((CacheEntry)enumerator.Value).KeysDependingOnMe;
					else if(enumerator.Value is CacheInsResultWithEntry)
					{
						CacheInsResultWithEntry entry = (CacheInsResultWithEntry)enumerator.Value;
						if(entry.Entry != null)
							keysDependingOnMe = entry.Entry.KeysDependingOnMe;
					}
					if(keysDependingOnMe != null)
					{
						IDictionaryEnumerator enumerator2 = keysDependingOnMe.GetEnumerator();
						while (enumerator2.MoveNext())
							if(!hashtable.ContainsKey(enumerator2.Key))
								hashtable.Add(enumerator2.Key, null);
					}
				}
			}
			string[] array = new string[hashtable.Count];
			hashtable.Keys.CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Recupera os itens com as chaves informadas.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual Hashtable Get(object[] keys, OperationContext operationContext)
		{
			return this.Get(keys, operationContext);
		}

		/// <summary>
		/// Recupera uma entrada com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Get(object key, OperationContext operationContext)
		{
			object lockId = null;
			DateTime now = DateTime.Now;
			ulong version = 0;
			return this.Get(key, ref version, ref lockId, ref now, null, LockAccessType.IGNORE_LOCK, operationContext);
		}

		/// <summary>
		/// Recupera a entrada com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="isUserOperation">Identifica se é uma operação do usuário.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Get(object key, bool isUserOperation, OperationContext operationContext)
		{
			object lockId = null;
			DateTime now = DateTime.Now;
			ulong version = 0;
			return this.Get(key, isUserOperation, ref version, ref lockId, ref now, null, LockAccessType.IGNORE_LOCK, operationContext);
		}

		/// <summary>
		/// Recupera a entrada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="lockId">Lock associado.</param>
		/// <param name="lockDate">Data do lock</param>
		/// <param name="lockExpiration">Data de expiração.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Get(object key, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			lockId = null;
			lockDate = DateTime.Now;
			return null;
		}

		/// <summary>
		/// Recupera a entrada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="isUserOperation"></param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="lockId">Lock associado.</param>
		/// <param name="lockDate">Data do lock</param>
		/// <param name="lockExpiration">Data de expiração.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Get(object key, bool isUserOperation, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			return null;
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return this.GetEnumerator(null);
		}

		public virtual IDictionaryEnumerator GetEnumerator(string group)
		{
			return null;
		}

		/// <summary>
		/// Recupera a lista das chaves finais.
		/// </summary>
		/// <param name="pKeys"></param>
		/// <param name="nKeys"></param>
		/// <returns></returns>
		public Hashtable GetFinalKeysList(object[] pKeys, object[] nKeys)
		{
			Hashtable hashtable = new Hashtable();
			if((pKeys == null) || (nKeys == null))
			{
				hashtable.Add("oldKeys", new object[0]);
				hashtable.Add("newKeys", new object[0]);
				return hashtable;
			}
			if((pKeys != null) && (nKeys != null))
			{
				ArrayList list = new ArrayList(pKeys);
				ArrayList list2 = new ArrayList(nKeys);
				for(int i = 0; i < pKeys.Length; i++)
				{
					for(int j = 0; j < nKeys.Length; j++)
					{
						if(pKeys[i] == nKeys[j])
						{
							list.RemoveAt(i);
							list2.RemoveAt(j);
							break;
						}
					}
				}
				hashtable.Add("oldKeys", list.ToArray());
				hashtable.Add("newKeys", list2.ToArray());
			}
			return hashtable;
		}

		public virtual Hashtable GetGroup(object[] keys, string group, string subGroup, OperationContext operationContext)
		{
			return null;
		}

		public virtual CacheEntry GetGroup(object key, string group, string subGroup, OperationContext operationContext)
		{
			object lockId = null;
			DateTime now = DateTime.Now;
			ulong version = 0;
			return this.GetGroup(key, group, subGroup, ref version, ref lockId, ref now, null, LockAccessType.IGNORE_LOCK, operationContext);
		}

		public virtual CacheEntry GetGroup(object key, string group, string subGroup, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			return null;
		}

		public virtual GroupInfo GetGroupInfo(object key, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable GetGroupInfoBulk(object[] keys, OperationContext operationContext)
		{
			return null;
		}

		public virtual ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			return null;
		}

		public virtual int GetItemSize(object key)
		{
			return 0;
		}

		public virtual ArrayList GetKeyList(int bucketId, bool startLogging)
		{
			return null;
		}

		internal virtual ArrayList GetKeysByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			return null;
		}

		protected Hashtable GetKeysTable(object key, object[] keys)
		{
			if(keys == null)
			{
				return null;
			}
			Hashtable hashtable = new Hashtable(keys.Length);
			for(int i = 0; i < keys.Length; i++)
			{
				if(!hashtable.Contains(keys[i]))
				{
					hashtable.Add(keys[i], new ArrayList());
				}
				((ArrayList)hashtable[keys[i]]).Add(key);
			}
			return hashtable;
		}

		public virtual Hashtable GetLogTable(ArrayList bucketIds, ref bool isLoggingStopped)
		{
			return null;
		}

		public virtual EnumerationDataChunk GetNextChunk(EnumerationPointer pointer, OperationContext operationContext)
		{
			return null;
		}

		public virtual long GetStreamLength(string key, string lockHandle, OperationContext operationContext)
		{
			return 0;
		}

		public virtual Hashtable GetTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			return null;
		}

		public virtual bool GroupExists(string group)
		{
			return false;
		}

		public virtual bool HasEnumerationPointer(EnumerationPointer pointer)
		{
			return false;
		}

		/// <summary>
		/// Inicializa a configuração da instancia.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="properties"></param>
		protected virtual void Initialize(IDictionary cacheClasses, IDictionary properties)
		{
			if(properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			try
			{
				this.Name = Convert.ToString(properties["id"]);
				if(properties.Contains("notifications"))
				{
					IDictionary dictionary = properties["notifications"] as IDictionary;
					if(dictionary.Contains("item-add") && Convert.ToBoolean(dictionary["item-add"]))
						_notifiers |= Notifications.ItemAdd;
					if(dictionary.Contains("item-update") && Convert.ToBoolean(dictionary["item-update"]))
						_notifiers |= Notifications.ItemUpdate;
					if(dictionary.Contains("item-remove") && Convert.ToBoolean(dictionary["item-remove"]))
						_notifiers |= Notifications.ItemRemove;
					if(dictionary.Contains("cache-clear") && Convert.ToBoolean(dictionary["cache-clear"]))
						_notifiers |= Notifications.CacheClear;
				}
				else
				{
					_notifiers |= Notifications.All;
				}
			}
			catch(Colosoft.Caching.Exceptions.ConfigurationException)
			{
				this.Dispose();
				throw;
			}
			catch(Exception exception)
			{
				this.Dispose();
				throw new ConfigurationException("Configuration Error: " + exception.ToString(), exception);
			}
		}

		/// <summary>
		/// Inicializa a configuração da instancia.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="properties"></param>
		/// <param name="userId"></param>
		/// <param name="password"></param>
		/// <param name="twoPhaseInitialization"></param>
		public virtual void Initialize(IDictionary cacheClasses, IDictionary properties, string userId, string password, bool twoPhaseInitialization)
		{
			if(properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			try
			{
				this.Name = Convert.ToString(properties["id"]);
				if(properties.Contains("notifications"))
				{
					IDictionary dictionary = properties["notifications"] as IDictionary;
					if(dictionary.Contains("item-add") && Convert.ToBoolean(dictionary["item-add"]))
						_notifiers |= Notifications.ItemAdd;
					if(dictionary.Contains("item-update") && Convert.ToBoolean(dictionary["item-update"]))
						_notifiers |= Notifications.ItemUpdate;
					if(dictionary.Contains("item-remove") && Convert.ToBoolean(dictionary["item-remove"]))
						_notifiers |= Notifications.ItemRemove;
					if(dictionary.Contains("cache-clear") && Convert.ToBoolean(dictionary["cache-clear"]))
						_notifiers |= Notifications.CacheClear;
				}
				else
				{
					_notifiers |= Notifications.All;
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
				throw new ConfigurationException("Configuration Error: " + exception.ToString(), exception);
			}
		}

		public virtual Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, string taskId, OperationContext operationContext)
		{
			return null;
		}

		public virtual CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return new CacheInsResultWithEntry();
		}

		public virtual CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, bool isUserOperation, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return new CacheInsResultWithEntry();
		}

		public virtual CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, string taskId, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return new CacheInsResultWithEntry();
		}

		internal virtual bool IsBucketFunctionalOnReplica(string key)
		{
			return false;
		}

		/// <summary>
		/// Verifica se o item do cache associado com a chave está bloqueado.
		/// </summary>
		/// <param name="key">Chave o item do cache.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public virtual LockOptions IsLocked(object key, ref object lockId, ref DateTime lockDate, OperationContext operationContext)
		{
			lockId = null;
			lockDate = DateTime.Now;
			return null;
		}

		/// <summary>
		/// Realiza um lock no item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="lockExpiration">Informações sobre a expiração do lock.</param>
		/// <param name="lockId">Identificador gerado para o lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Informações sobre o lock.</returns>
		public virtual LockOptions Lock(object key, LockExpiration lockExpiration, ref object lockId, ref DateTime lockDate, OperationContext operationContext)
		{
			lockId = null;
			lockDate = DateTime.Now;
			return null;
		}

		protected virtual void NotifyCacheCleared(bool async)
		{
			if((this.Listener != null) && this.IsCacheClearNotifier)
			{
				if(!async)
				{
					this.Listener.OnCacheCleared();
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyCacheClear(this.Listener));
				}
			}
		}

		public virtual void NotifyCustomEvent(object notifId, object data, bool async)
		{
			if(this.Listener != null)
			{
				if(!async)
				{
					this.Listener.OnCustomEvent(notifId, data);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyCustomEvent(this.Listener, notifId, data));
				}
			}
		}

		public virtual void NotifyCustomRemoveCallback(object key, object value, ItemRemoveReason reason, bool async)
		{
			if(this.Listener != null)
			{
				if(!async)
				{
					this.Listener.OnCustomRemoveCallback(key, value, reason);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyRemoveCallback(this.Listener, key, value, reason));
				}
			}
		}

		protected virtual void NotifyCustomUpdateCallback(object key, object value, bool async)
		{
			if(this.Listener != null)
			{
				if(!async)
				{
					this.Listener.OnCustomUpdateCallback(key, value);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyUpdateCallback(this.Listener, key, value));
				}
			}
		}

		protected virtual void NotifyHashmapChanged(long viewId, Hashtable newmap, ArrayList members, bool async)
		{
			ICacheEventsListener listener = this.Listener;
		}

		/// <summary>
		/// Notifica que uma item foi adicionado.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="async"></param>
		protected virtual void NotifyItemAdded(object key, bool async)
		{
			if((this.Listener != null) && this.IsItemAddNotifier)
			{
				if(!async)
				{
					this.Listener.OnItemAdded(key);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyAdd(this.Listener, key));
				}
			}
		}

		protected virtual void NotifyItemRemoved(object key, object val, ItemRemoveReason reason, bool async, OperationContext operationContext)
		{
			if((this.Listener != null) && this.IsItemRemoveNotifier)
			{
				if(!async)
				{
					this.Listener.OnItemRemoved(key, val, reason, operationContext);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyRemoval(this.Listener, key, val, reason, operationContext));
				}
			}
		}

		public virtual void NotifyItemsRemoved(object[] keys, object[] vals, ItemRemoveReason reason, bool async, OperationContext operationContext)
		{
			if((this.Listener != null) && this.IsItemRemoveNotifier)
			{
				if(!async)
				{
					this.Listener.OnItemsRemoved(keys, vals, reason, operationContext);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyRemoval(this.Listener, keys, vals, reason, operationContext));
				}
			}
		}

		protected virtual void NotifyItemUpdated(object key, bool async, OperationContext operationContext)
		{
			if((this.Listener != null) && this.IsItemUpdateNotifier)
			{
				if(!async)
				{
					this.Listener.OnItemUpdated(key, operationContext);
				}
				else
				{
					_context.AsyncProc.Enqueue(new AsyncLocalNotifyUpdate(this.Listener, key, operationContext));
				}
			}
		}

		protected virtual void NotifyWriteBehindTaskCompleted(OpCode operationCode, Hashtable result, CallbackEntry cbEntry, OperationContext operationContext)
		{
			if(this.Listener != null)
			{
				this.InternalCache.DoWrite("CacheBase.NotifyWriteBehindTaskCompleted", "", operationContext);
				this.Listener.OnWriteBehindOperationCompletedCallback(operationCode, result, cbEntry);
			}
		}

		public virtual void NotifyWriteBehindTaskStatus(OpCode operationCode, Hashtable result, CallbackEntry cbEntry, string taskId, string providerName, OperationContext operationContext)
		{
			if((cbEntry != null) && (cbEntry.WriteBehindOperationCompletedCallback != null))
			{
				this.NotifyWriteBehindTaskCompleted(operationCode, result, cbEntry, operationContext);
			}
		}

		public virtual bool OpenStream(string key, string lockHandle, StreamModes mode, string group, string subGroup, ExpirationHint hint, EvictionHint evictinHint, OperationContext operationContext)
		{
			return false;
		}

		public virtual int ReadFromStream(ref VirtualArray vBuffer, string key, string lockHandle, int offset, int length, OperationContext operationContext)
		{
			return 0;
		}

		public virtual void RegisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
		}

		public virtual void RegisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
		}

		public virtual Hashtable Remove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable Remove(object[] keys, ItemRemoveReason removalReason, bool notify, OperationContext operationContext)
		{
			return this.Remove(keys, null, removalReason, notify, operationContext);
		}

		public virtual Hashtable Remove(object[] keys, ItemRemoveReason removalReason, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable Remove(object[] keys, ItemRemoveReason removalReason, bool notify, string taskId, OperationContext operationContext)
		{
			return null;
		}

		public virtual Hashtable Remove(object[] keys, string group, ItemRemoveReason removalReason, bool notify, OperationContext operationContext)
		{
			return null;
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada do cache.</param>
		/// <param name="removalReason"></param>
		/// <param name="notify">Identifica que é para notificar a exclusão.</param>
		/// <param name="lockId"></param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Remove(object key, ItemRemoveReason removalReason, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Remove(key, null, removalReason, notify, lockId, version, accessType, operationContext);
		}

		public virtual CacheEntry Remove(object key, ItemRemoveReason removalReason, bool notify, bool isUserOperation, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return null;
		}

		public virtual CacheEntry Remove(object key, ItemRemoveReason removalReason, bool notify, string taskId, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return null;
		}

		/// <summary>
		/// Remove o entrada do cache associada com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="group"></param>
		/// <param name="removalReason"></param>
		/// <param name="notify"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual CacheEntry Remove(object key, string group, ItemRemoveReason removalReason, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return null;
		}

		public virtual void RemoveBucket(int bucket)
		{
		}

		public virtual void RemoveBucketData(ArrayList bucketIds)
		{
			if((bucketIds != null) && (bucketIds.Count > 0))
			{
				for(int i = 0; i < bucketIds.Count; i++)
				{
					this.RemoveBucketData((int)bucketIds[i]);
				}
			}
		}

		public virtual void RemoveBucketData(int bucketId)
		{
		}

		public virtual Hashtable RemoveByTag(string[] tags, TagComparisonType tagComparisonType, bool notify, OperationContext operationContext)
		{
			return null;
		}

		public void RemoveCascadingDependencies(Hashtable removedItems, OperationContext operationContext)
		{
			if((removedItems != null) && (removedItems.Count != 0))
			{
				string[] keys = this.ExtractKeys(removedItems);
				Hashtable table = null;
				while ((keys != null) && (keys.Length > 0))
				{
					table = _context.CacheImpl.Remove(keys, ItemRemoveReason.DependencyChanged, true, operationContext);
					keys = this.ExtractKeys(table);
				}
			}
		}

		/// <summary>
		/// Remove a dependencia em cascata.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="e"></param>
		/// <param name="operationContext"></param>
		public void RemoveCascadingDependencies(object key, CacheEntry e, OperationContext operationContext)
		{
			if((e != null) && (e.KeysDependingOnMe != null))
			{
				Hashtable table = new Hashtable();
				table.Add(key, e);
				string[] array = new string[e.KeysDependingOnMe.Count];
				e.KeysDependingOnMe.Keys.CopyTo(array, 0);
				while ((array != null) && (array.Length > 0))
				{
					table = _context.CacheImpl.Remove(array, ItemRemoveReason.DependencyChanged, true, operationContext);
					array = this.ExtractKeys(table);
				}
			}
		}

		public virtual Hashtable RemoveDepKeyList(Hashtable table, OperationContext operationContext)
		{
			return null;
		}

		public virtual void RemoveExtraBuckets(ArrayList bucketIds)
		{
		}

		public virtual void RemoveFromLogTbl(int bucketId)
		{
		}

		public virtual object RemoveSync(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			return null;
		}

		public virtual void ReplicateConnectionString(string connString, bool isSql)
		{
		}

		public virtual void ReplicateOperations(Array keys, Array cacheEntries, Array userPayloads, ArrayList compilationInfo, ulong seqId, long viewId)
		{
		}

		/// <summary>
		/// Realiza a pesquisa com os dados informados.
		/// </summary>
		/// <param name="query">Texto do comando da pesquisa.</param>
		/// <param name="values">Valores que serão utilizados.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual QueryResultSet Search(string query, IDictionary values, OperationContext operationContext)
		{
			return null;
		}

		/// <summary>
		/// Realiza a junção dos indices associados aos tipos informados.
		/// </summary>
		/// <param name="leftTypeName">Nome do tipo da esquerda.</param>
		/// <param name="leftFieldName">Nome do campo do tipo da esquerda.</param>
		/// <param name="rightTypeName">Nome do tipo da direita.</param>
		/// <param name="rightFieldName">Nome do campo do tipo da direita.</param>
		/// <param name="comparisonType">Tipo de comparação que será realizada.</param>
		/// <returns></returns>
		public virtual IEnumerable<object[]> JoinIndex(string leftTypeName, string leftFieldName, string rightTypeName, string rightFieldName, ComparisonType comparisonType)
		{
			return null;
		}

		/// <summary>
		/// Realiza a pesquisa de entradas.
		/// </summary>
		/// <param name="query">Texto do comando da pesquisa.</param>
		/// <param name="values">Valores que serão utilizados.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public virtual QueryResultSet SearchEntries(string query, IDictionary values, OperationContext operationContext)
		{
			return null;
		}

		public virtual void SendNotification(object notifId, object data)
		{
		}

		public virtual void StartLogging(int bucketId)
		{
		}

		internal virtual void StopServices()
		{
		}

		public static CacheBase Synchronized(CacheBase cache)
		{
			return new CacheSyncWrapper(cache);
		}

		public virtual void UnLock(object key, object lockId, bool isPreemptive, OperationContext operationContext)
		{
		}

		public virtual void UnregisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
		}

		public virtual void UnregisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
		}

		public virtual void UpdateLocalBuckets(ArrayList bucketIds)
		{
		}

		internal virtual void UpdateLockInfo(object key, object lockId, DateTime lockDate, LockExpiration lockExpiration, OperationContext operationContext)
		{
		}

		public virtual void ValidateItems(ArrayList keys, ArrayList userPayloads)
		{
		}

		public virtual void ValidateItems(object key, object userPayloads)
		{
		}

		public virtual void WriteToStream(string key, string lockHandle, VirtualArray vBuffer, int srcOffset, int dstOffset, int length, OperationContext operationContext)
		{
		}

		/// <summary>
		/// Possíveis notificações.
		/// </summary>
		[Flags]
		public enum Notifications
		{
			/// <summary>
			/// Todas notificações.
			/// </summary>
			All = 15,
			/// <summary>
			/// Limpeza do cache.
			/// </summary>
			CacheClear = 8,
			/// <summary>
			/// Item adicionado.
			/// </summary>
			ItemAdd = 1,
			/// <summary>
			/// Item removido.
			/// </summary>
			ItemRemove = 4,
			/// <summary>
			/// Item atualizado.
			/// </summary>
			ItemUpdate = 2,
			/// <summary>
			/// Nenhuma
			/// </summary>
			None = 0
		}
	}
}
