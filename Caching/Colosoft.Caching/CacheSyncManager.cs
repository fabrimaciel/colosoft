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
using Colosoft.Logging;
using Colosoft;

namespace Colosoft.Caching.Synchronization
{
	/// <summary>
	/// Classe responsável por gerencia a sincronização do cache.
	/// </summary>
	internal class CacheSyncManager : IDisposable
	{
		private Cache _cache;

		private CacheRuntimeContext _context;

		private Hashtable _dependenciesKeyMap = Hashtable.Synchronized(new Hashtable());

		private Hashtable _dependenciesStatus = Hashtable.Synchronized(new Hashtable());

		private Hashtable _inactiveDependencies = Hashtable.Synchronized(new Hashtable());

		private Hashtable _listeners = Hashtable.Synchronized(new Hashtable());

		private Hashtable _synCaches = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Hash as dependencias inativas.
		/// </summary>
		public Hashtable InactiveDependencies
		{
			get
			{
				return _inactiveDependencies;
			}
		}

		/// <summary>
		/// Logger.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _cache.Logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="context">Contexto.</param>
		public CacheSyncManager(Cache cache, CacheRuntimeContext context)
		{
			_cache = cache;
			_context = context;
		}

		/// <summary>
		/// Ativa as dependencias.
		/// </summary>
		public void ActivateDependencies()
		{
			try
			{
				lock (_inactiveDependencies.SyncRoot)
				{
					IDictionaryEnumerator enumerator = _inactiveDependencies.GetEnumerator();
					while (enumerator.MoveNext())
					{
						SyncItem key = (SyncItem)enumerator.Key;
						CacheSyncDependency dependency = (CacheSyncDependency)enumerator.Value;
						this.AddDependency(key.ThisKey, dependency);
					}
					_inactiveDependencies.Clear();
				}
			}
			catch(Exception exception)
			{
				this.Logger.Error("CacheSyncManager.ActivateDependencies()".GetFormatter(), exception.GetFormatter());
			}
		}

		/// <summary>
		/// Adiciona uma dependencia para o gerenciador.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="dependency">Instancia da dependencia.</param>
		public void AddDependency(object key, CacheSyncDependency dependency)
		{
			if(_context.IsDbSyncCoordinator)
			{
				ISyncCache syncCache = dependency.SyncCache;
				SyncEventListener listener = null;
				if((dependency == null) || (dependency.CacheId == null))
					return;
				lock (_dependenciesStatus.SyncRoot)
				{
					SyncItem item = new SyncItem(key, dependency.Key, dependency.CacheId);
					ArrayList list = null;
					if(!_dependenciesKeyMap.Contains(item))
					{
						list = new ArrayList();
						list.Add(item.ThisKey);
						_dependenciesKeyMap.Add(item, list);
					}
					else
					{
						list = _dependenciesKeyMap[item] as ArrayList;
						if(!list.Contains(item.ThisKey))
							list.Add(item.ThisKey);
					}
					if(!_dependenciesStatus.Contains(item))
						_dependenciesStatus.Add(item, DependencyStatus.Unchanged);
					if(_synCaches.Contains(dependency.CacheId))
					{
						listener = _listeners[dependency.CacheId] as SyncEventListener;
						syncCache = _synCaches[dependency.CacheId] as ISyncCache;
					}
					else
					{
						syncCache.Initialize();
						_synCaches.Add(dependency.CacheId, syncCache);
						if(_listeners.Contains(dependency.CacheId))
							listener = _listeners[dependency.CacheId] as SyncEventListener;
						else
						{
							listener = new SyncEventListener(dependency.CacheId, this);
							_listeners.Add(dependency.CacheId, listener);
						}
					}
					if((list != null) && (list.Count < 2))
						syncCache.RegisterSyncKeyNotifications((string)item.Key, listener);
					return;
				}
			}
			SyncItem item2 = new SyncItem(key, dependency.Key, dependency.CacheId);
			if(!_inactiveDependencies.Contains(item2))
				_inactiveDependencies.Add(item2, dependency);
		}

		/// <summary>
		/// Adiciona varias dependencias da entradas para o gerenciador.
		/// </summary>
		/// <param name="keys">Chaves das dependencias.</param>
		/// <param name="entries">Entradas do cache.</param>
		public void AddDependency(object[] keys, CacheEntry[] entries)
		{
			for(int i = 0; i < keys.Length; i++)
				if(entries[i].SyncDependency != null)
					this.AddDependency(keys[i], entries[i].SyncDependency);
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		public void Clear()
		{
			try
			{
				if(_synCaches != null)
				{
					object[] array = new object[_synCaches.Values.Count];
					_synCaches.Values.CopyTo(array, 0);
					if(array != null)
					{
						foreach (ISyncCache cache in array)
						{
							this.RemoveDependentItems(cache.CacheId, true, false);
							cache.Dispose();
						}
					}
				}
				lock (_synCaches.SyncRoot)
					_synCaches.Clear();
				lock (_dependenciesStatus.SyncRoot)
					_dependenciesStatus.Clear();
				lock (_listeners.SyncRoot)
					_listeners.Clear();
				lock (_dependenciesKeyMap.SyncRoot)
					_dependenciesKeyMap.Clear();
			}
			catch(Exception exception)
			{
				Logger.Error(("CacheSyncManager:" + exception.ToString()).GetFormatter());
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_synCaches != null)
			{
				object[] array = new object[_synCaches.Values.Count];
				_synCaches.Values.CopyTo(array, 0);
				if(array != null)
				{
					foreach (ISyncCache cache in array)
						this.RemoveDependentItems(cache.CacheId, false, false);
				}
				_synCaches.Clear();
				_dependenciesKeyMap.Clear();
				_dependenciesStatus.Clear();
			}
		}

		/// <summary>
		/// Recupera a situação do item.
		/// </summary>
		/// <param name="syncItem"></param>
		/// <returns></returns>
		public DependencyStatus GetDependencyStatus(SyncItem syncItem)
		{
			DependencyStatus expired = DependencyStatus.Expired;
			if(_dependenciesStatus.Contains(syncItem))
			{
				expired = (DependencyStatus)_dependenciesStatus[syncItem];
			}
			return expired;
		}

		/// <summary>
		/// Recupera o instancia associado com o item de sincronização.
		/// </summary>
		/// <param name="item">Item de sincronização associado.</param>
		/// <param name="version">Versão do item recuperado.</param>
		/// <param name="flag"></param>
		/// <returns></returns>
		private object GetItemFromSyncCache(SyncItem item, ref ulong version, ref BitSet flag)
		{
			if(item != null)
			{
				ISyncCache cache = _synCaches[item.CacheId] as ISyncCache;
				if(cache != null)
				{
					try
					{
						return cache.Get((string)item.Key, ref version, ref flag);
					}
					catch(Exception exception)
					{
						this.Logger.Error(("CacheSyncManager:" + exception.ToString()).GetFormatter());
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Remove a dependencia.
		/// </summary>
		/// <param name="key">Chave.</param>
		/// <param name="dependency">Instancia da dependencia.</param>
		public void RemoveDependency(object key, CacheSyncDependency dependency)
		{
			if(_context.IsDbSyncCoordinator)
			{
				if(dependency != null)
				{
					try
					{
						SyncItem item = new SyncItem(key, dependency.Key, dependency.CacheId);
						lock (_dependenciesStatus.SyncRoot)
						{
							if(_dependenciesKeyMap.Contains(item))
							{
								ArrayList list = _dependenciesKeyMap[item] as ArrayList;
								if(list != null)
								{
									list.Remove(key);
									if(list.Count > 0)
										return;
								}
								_dependenciesKeyMap.Remove(item);
							}
							if(_dependenciesStatus.Contains(item))
								_dependenciesStatus.Remove(item);
							else
								return;
						}
						ISyncCache cache = _synCaches[item.CacheId] as ISyncCache;
						ISyncCacheEventsListener eventListener = _listeners[item.CacheId] as ISyncCacheEventsListener;
						if((cache != null) && (eventListener != null))
							cache.UnRegisterSyncKeyNotifications((string)item.Key, eventListener);
					}
					catch(Exception exception)
					{
						this.Logger.Error(("CacheSyncManager:" + exception.ToString()).GetFormatter());
					}
				}
			}
			else
			{
				_inactiveDependencies.Remove(key);
			}
		}

		/// <summary>
		/// Recupera as dependencia das entradas informadas.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="entries"></param>
		/// <param name="failedKeys">Chaves de falha</param>
		public void RemoveDependency(object[] keys, CacheEntry[] entries, IDictionary failedKeys)
		{
			for(int i = 0; i < keys.Length; i++)
			{
				if((entries[i].SyncDependency != null) && failedKeys.Contains(keys[i]))
				{
					if(failedKeys[keys[i]] is CacheAddResult)
					{
						if(((CacheAddResult)failedKeys[keys[i]]) != CacheAddResult.Success)
							this.RemoveDependency(keys[i], entries[i].SyncDependency);
					}
					else if(((failedKeys[keys[i]] is CacheInsResult) && (((CacheInsResult)failedKeys[keys[i]]) != CacheInsResult.Success)) && (((CacheInsResult)failedKeys[keys[i]]) != CacheInsResult.SuccessOverwrite))
					{
						this.RemoveDependency(keys[i], entries[i].SyncDependency);
					}
				}
			}
		}

		/// <summary>
		/// Remove os itens de dependencia.
		/// </summary>
		/// <param name="cacheId">Identificador do cache.</param>
		/// <param name="unregisterCallbacks">True para remove o registro dos callbacks.</param>
		/// <param name="removeFromCache">True para remover do cache.</param>
		public void RemoveDependentItems(string cacheId, bool unregisterCallbacks, bool removeFromCache)
		{
			object[] array = null;
			lock (_dependenciesStatus.SyncRoot)
			{
				if(!unregisterCallbacks)
				{
					ISyncCache cache = _synCaches[cacheId.ToLower()] as ISyncCache;
					if(cache != null)
					{
						cache.Dispose();
						_synCaches.Remove(cacheId.ToLower());
					}
				}
				if(removeFromCache && (_dependenciesStatus.Count > 0))
				{
					array = new object[_dependenciesStatus.Keys.Count];
					_dependenciesStatus.Keys.CopyTo(array, 0);
				}
			}
			if(array != null)
			{
				foreach (SyncItem item in array)
				{
					if(item.CacheId == cacheId.ToLower())
						this.RemoveSyncItem(item, unregisterCallbacks);
				}
			}
		}

		/// <summary>
		/// Remove um item de sincronização.
		/// </summary>
		/// <param name="item">Instancia do item.</param>
		/// <param name="unregisterCallbacks">True para remover o registro dos callbacks.</param>
		private void RemoveSyncItem(SyncItem item, bool unregisterCallbacks)
		{
			if(_cache != null)
			{
				try
				{
					ArrayList list = null;
					lock (_dependenciesStatus.SyncRoot)
					{
						_dependenciesStatus.Remove(item);
						if(_dependenciesKeyMap.Contains(item))
							list = _dependenciesKeyMap[item] as ArrayList;
						_dependenciesKeyMap.Remove(item);
					}
					ISyncCache cache = _synCaches[item.CacheId] as ISyncCache;
					ISyncCacheEventsListener eventListener = _listeners[item.CacheId] as ISyncCacheEventsListener;
					if(((cache != null) && (eventListener != null)) && unregisterCallbacks)
					{
						cache.UnRegisterSyncKeyNotifications((string)item.Key, eventListener);
					}
					if(list != null)
					{
						foreach (object obj2 in list)
						{
							_cache.Remove(obj2, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						}
					}
				}
				catch(Exception exception)
				{
					this.Logger.Error(("CacheSyncManager:" + exception.ToString()).GetFormatter());
				}
			}
		}

		/// <summary>
		/// Define a situação para o item.
		/// </summary>
		/// <param name="syncItem">Instancia do item de sincronização.</param>
		/// <param name="status">Nova situação.</param>
		/// <returns></returns>
		private bool SetItemStatus(SyncItem syncItem, DependencyStatus status)
		{
			bool flag = false;
			lock (_dependenciesStatus.SyncRoot)
			{
				if(_dependenciesStatus.ContainsKey(syncItem))
				{
					_dependenciesStatus[syncItem] = status;
					flag = true;
				}
			}
			return flag;
		}

		/// <summary>
		/// Sincroniza o item informado.
		/// </summary>
		/// <param name="syncItem">Instancia do item que será sincronizado.</param>
		/// <param name="status">Situação.</param>
		public void Synchronize(SyncItem syncItem, DependencyStatus status)
		{
			if(this.SetItemStatus(syncItem, status))
			{
				switch(status)
				{
				case DependencyStatus.Expired:
					this.RemoveSyncItem(syncItem, true);
					return;
				case DependencyStatus.HasChanged:
					if(_cache != null)
					{
						ulong version = 0;
						BitSet flag = new BitSet();
						object obj2 = this.GetItemFromSyncCache(syncItem, ref version, ref flag);
						if(obj2 != null)
						{
							ArrayList list = _dependenciesKeyMap[syncItem] as ArrayList;
							try
							{
								if(list != null)
								{
									foreach (object obj3 in list)
									{
										_cache.Insert(obj3, obj2, null, null, null, null, null, null, flag, null, version, LockAccessType.PRESERVE_VERSION, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
									}
								}
								this.SetItemStatus(syncItem, DependencyStatus.Unchanged);
							}
							catch(Exception)
							{
								this.RemoveSyncItem(syncItem, true);
							}
						}
					}
					return;
				default:
					return;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private class SyncEventListener : ISyncCacheEventsListener
		{
			private string _cacheid;

			private CacheSyncManager _synchronizer;

			public SyncEventListener(string cacheId, CacheSyncManager synchronizer)
			{
				_cacheid = cacheId;
				_synchronizer = synchronizer;
			}

			public void CacheCleared()
			{
				if(_synchronizer != null)
					_synchronizer.RemoveDependentItems(_cacheid, true, true);
			}

			public void CacheStopped(string cacheId)
			{
				if(_synchronizer != null)
					_synchronizer.RemoveDependentItems(_cacheid, false, true);
			}

			public override bool Equals(object obj)
			{
				return ((obj is CacheSyncManager.SyncEventListener) && (_cacheid == ((CacheSyncManager.SyncEventListener)obj)._cacheid));
			}

			/// <summary>
			/// Hash.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public void SyncItemRemoved(string key)
			{
				if(_synchronizer != null)
					_synchronizer.Synchronize(new CacheSyncManager.SyncItem(key, _cacheid), DependencyStatus.Expired);
			}

			public void SyncItemUpdated(string key)
			{
				if(_synchronizer != null)
					_synchronizer.Synchronize(new CacheSyncManager.SyncItem(key, _cacheid), DependencyStatus.HasChanged);
			}
		}

		/// <summary>
		/// Representa um item de sincronização.
		/// </summary>
		public class SyncItem
		{
			private string _cacheId;

			private object _key;

			private object _thisKey;

			/// <summary>
			/// Identificador do cache.
			/// </summary>
			public string CacheId
			{
				get
				{
					return _cacheId;
				}
				set
				{
					if(value != null)
						_cacheId = value.ToLower();
					else
						_cacheId = value;
				}
			}

			/// <summary>
			/// Chave associada.
			/// </summary>
			public object Key
			{
				get
				{
					return _key;
				}
				set
				{
					_key = value;
				}
			}

			/// <summary>
			/// Chave THIS.
			/// </summary>
			public object ThisKey
			{
				get
				{
					return _thisKey;
				}
				set
				{
					_thisKey = value;
				}
			}

			/// <summary>
			/// Cria uma instancia com a chave e o id do cache.
			/// </summary>
			/// <param name="key"></param>
			/// <param name="cacheId">Identificador do cache.</param>
			public SyncItem(object key, string cacheId)
			{
				_key = key;
				_cacheId = cacheId;
			}

			/// <summary>
			/// Cria uma instancia com a chave this, chave e o id do cache.
			/// </summary>
			/// <param name="thisKey"></param>
			/// <param name="key"></param>
			/// <param name="cacheid"></param>
			public SyncItem(object thisKey, object key, string cacheid)
			{
				_key = key;
				_cacheId = cacheid;
				_thisKey = thisKey;
			}

			/// <summary>
			/// Compara com outra instancia.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				if(obj is CacheSyncManager.SyncItem)
				{
					CacheSyncManager.SyncItem item = obj as CacheSyncManager.SyncItem;
					if(item._cacheId == _cacheId)
					{
						string str = item._key as string;
						string str2 = _key as string;
						if(str == str2)
							return true;
					}
				}
				return false;
			}

			/// <summary>
			/// Hash code da instancia.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				if((_cacheId != null) && (_key != null))
					return (_cacheId + _key).GetHashCode();
				return base.GetHashCode();
			}
		}
	}
}
