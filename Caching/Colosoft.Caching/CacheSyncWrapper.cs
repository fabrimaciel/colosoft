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
using Colosoft.Caching.Expiration;
using System.Collections;
using Colosoft.Caching.Data;
using Colosoft.Serialization;
using Colosoft.Caching.Queries;
using Colosoft.Caching.Statistics;
using Colosoft.Caching.Policies;

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação do cache como classe adaptadora para garantir sincronismo de acesso.
	/// </summary>
	internal class CacheSyncWrapper : CacheBase
	{
		/// <summary>
		/// Instancia da classe adaptada.
		/// </summary>
		private CacheBase _cache;

		/// <summary>
		/// Quantidade de itens na instancia.
		/// </summary>
		public override long Count
		{
			get
			{
				return this.Internal.Count;
			}
		}

		public override ArrayList DataGroupList
		{
			get
			{
				ArrayList dataGroupList;
				base.Sync.AcquireWriterLock(-1);
				try
				{
					dataGroupList = this.Internal.DataGroupList;
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
				return dataGroupList;
			}
		}

		internal override float EvictRatio
		{
			get
			{
				return this.Internal.EvictRatio;
			}
			set
			{
				base.Sync.AcquireWriterLock(-1);
				try
				{
					this.Internal.EvictRatio = value;
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
			}
		}

		public CacheBase Internal
		{
			get
			{
				return _cache;
			}
		}

		protected internal override CacheBase InternalCache
		{
			get
			{
				return _cache.InternalCache;
			}
		}

		public override ICacheEventsListener Listener
		{
			get
			{
				return this.Internal.Listener;
			}
			set
			{
				this.Internal.Listener = value;
			}
		}

		/// <summary>
		/// Tamanho máximo do cache.
		/// </summary>
		internal override long MaxSize
		{
			get
			{
				long maxSize;
				base.Sync.AcquireReaderLock(-1);
				try
				{
					maxSize = this.Internal.MaxSize;
				}
				finally
				{
					base.Sync.ReleaseReaderLock();
				}
				return maxSize;
			}
			set
			{
				base.Sync.AcquireWriterLock(-1);
				try
				{
					this.Internal.MaxSize = value;
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
			}
		}

		/// <summary>
		/// Nome do cache.
		/// </summary>
		public override string Name
		{
			get
			{
				return this.Internal.Name;
			}
			set
			{
				this.Internal.Name = value;
			}
		}

		public override CacheBase.Notifications Notifiers
		{
			get
			{
				return this.Internal.Notifiers;
			}
			set
			{
				this.Internal.Notifiers = value;
			}
		}

		/// <summary>
		/// Instancia do analizador de consulta.
		/// </summary>
		public override ActiveQueryAnalyzer QueryAnalyzer
		{
			get
			{
				base.Sync.AcquireReaderLock(-1);
				try
				{
					return this.Internal.QueryAnalyzer;
				}
				finally
				{
					base.Sync.ReleaseReaderLock();
				}
			}
		}

		/// <summary>
		/// Tamanho do cache.
		/// </summary>
		internal override long Size
		{
			get
			{
				return this.Internal.Size;
			}
		}

		public override TypeInfoMap TypeInfoMap
		{
			get
			{
				return _cache.TypeInfoMap;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache">Instancia da classe que será adaptada.</param>
		public CacheSyncWrapper(CacheBase cache)
		{
			cache.Require("cache").NotNull();
			_cache = cache;
			base._context = cache.Context;
			base._syncObj = _cache.Sync;
		}

		public override bool Add(object key, ExpirationHint eh, OperationContext operationContext)
		{
			bool flag = false;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				flag = this.Internal.Add(key, eh, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return flag;
		}

		public override bool Add(object key, Colosoft.Caching.Synchronization.CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			bool flag = false;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				flag = this.Internal.Add(key, syncDependency, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return flag;
		}

		public override Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			return this.Add(keys, cacheEntries, notify, null, operationContext);
		}

		public override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, OperationContext operationContext)
		{
			return this.Add(key, cacheEntry, notify, (string)null, operationContext);
		}

		public override Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, string taskId, OperationContext operationContext)
		{
			Hashtable hashtable = null;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				hashtable = this.Internal.Add(keys, cacheEntries, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return hashtable;
		}

		public override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, bool IsUserOperation, OperationContext operationContext)
		{
			CacheAddResult result2;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				result2 = this.Internal.Add(key, cacheEntry, notify, IsUserOperation, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return result2;
		}

		/// <summary>
		/// Adiciona um nova entrada no cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cacheEntry"></param>
		/// <param name="notify"></param>
		/// <param name="taskId"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, string taskId, OperationContext operationContext)
		{
			CacheAddResult result2;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				result2 = this.Internal.Add(key, cacheEntry, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return result2;
		}

		/// <summary>
		/// Baseado no <see cref="Hashtable"/> informado associa chaves de dependencia.
		/// A chave do <see cref="Hashtable"/> é a chave do item e o valor é a lista das dependencias.
		/// </summary>
		/// <param name="table">Tabela contendo os dados que serão processados.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns><see cref="Hashtable"/> dos resultados associados com as chaves informadas.</returns>
		public override Hashtable AddDependencyKeyList(Hashtable table, OperationContext operationContext)
		{
			Hashtable hashtable = null;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				hashtable = this.Internal.AddDependencyKeyList(table, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return hashtable;
		}

		public override void AddLoggedData(ArrayList bucketIds)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.AddLoggedData(bucketIds);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		internal override bool CanChangeCacheSize(long size)
		{
			return this.Internal.CanChangeCacheSize(size);
		}

		public override Hashtable Cascaded_remove(Hashtable keyValues, ItemRemoveReason ir, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			ArrayList list = new ArrayList();
			if(keyValues != null)
			{
				base.Sync.AcquireWriterLock(-1);
				try
				{
					IDictionaryEnumerator enumerator = keyValues.GetEnumerator();
					while (enumerator.MoveNext())
					{
						CacheEntry entry = enumerator.Value as CacheEntry;
						if((entry != null) && (entry.KeysDependingOnMe != null))
						{
							Hashtable table = new Hashtable();
							string[] array = new string[entry.KeysDependingOnMe.Count];
							entry.KeysDependingOnMe.Keys.CopyTo(array, 0);
							while ((array != null) && (array.Length > 0))
							{
								table = this.Remove(array, ir, notify, operationContext);
								if(table != null)
								{
									IDictionaryEnumerator enumerator2 = table.GetEnumerator();
									if(enumerator2.MoveNext() && (enumerator2.Value != null))
									{
										hashtable[enumerator2.Key] = enumerator2.Value;
										list.Add(enumerator2.Key);
									}
								}
								array = base.ExtractKeys(table);
							}
						}
					}
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
			}
			return hashtable;
		}

		public override void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, OperationContext operationContext)
		{
			this.Clear(cbEntry, updateOptions, null, operationContext);
		}

		public override void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, string taskId, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.Clear(cbEntry, updateOptions, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void CloseStream(string key, string lockHandle, OperationContext operationContext)
		{
			try
			{
				base.Sync.AcquireWriterLock(-1);
				this.InternalCache.CloseStream(key, lockHandle, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override bool Contains(object key, OperationContext operationContext)
		{
			bool flag;
			if(!base.Sync.IsWriterLockHeld)
				base.Sync.AcquireReaderLock(-1);
			try
			{
				flag = this.Internal.Contains(key, operationContext);
			}
			finally
			{
				if(!base.Sync.IsWriterLockHeld)
					base.Sync.ReleaseReaderLock();
			}
			return flag;
		}

		public override Hashtable Contains(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable;
			if(!base.Sync.IsWriterLockHeld)
				base.Sync.AcquireReaderLock(-1);
			try
			{
				hashtable = this.Internal.Contains(keys, operationContext);
			}
			finally
			{
				if(!base.Sync.IsWriterLockHeld)
					base.Sync.ReleaseReaderLock();
			}
			return hashtable;
		}

		public override void Dispose()
		{
			if(_cache != null)
			{
				_cache.Dispose();
				_cache = null;
			}
			base.Dispose();
		}

		public override Hashtable Get(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				hashtable = this.Internal.Get(keys, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return hashtable;
		}

		public override CacheEntry Get(object key, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType access, OperationContext operationContext)
		{
			CacheEntry entry = null;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				entry = this.Internal.Get(key, ref version, ref lockId, ref lockDate, lockExpiration, access, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return entry;
		}

		public override CacheEntry Get(object key, bool IsUserOperation, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = null;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				entry = this.Internal.Get(key, IsUserOperation, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return entry;
		}

		public override IDictionaryEnumerator GetEnumerator()
		{
			IDictionaryEnumerator enumerator;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				enumerator = this.Internal.GetEnumerator();
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return enumerator;
		}

		public override CacheEntry GetGroup(object key, string group, string subGroup, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = null;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				entry = this.Internal.GetGroup(key, group, subGroup, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return entry;
		}

		public override Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			Hashtable hashtable;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				hashtable = this.Internal.GetGroupData(group, subGroup, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return hashtable;
		}

		public override GroupInfo GetGroupInfo(object key, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.GetGroupInfo(key, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override Hashtable GetGroupInfoBulk(object[] keys, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.GetGroupInfoBulk(keys, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.GetGroupKeys(group, subGroup, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override int GetItemSize(object key)
		{
			return _cache.GetItemSize(key);
		}

		public override ArrayList GetKeyList(int bucketId, bool startLogging)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return _cache.GetKeyList(bucketId, startLogging);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		internal override ArrayList GetKeysByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.GetKeysByTag(tags, comparisonType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override Hashtable GetLogTable(ArrayList bucketIds, ref bool isLoggingStopped)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return _cache.GetLogTable(bucketIds, ref isLoggingStopped);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override EnumerationDataChunk GetNextChunk(EnumerationPointer pointer, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.GetNextChunk(pointer, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override long GetStreamLength(string key, string lockHandle, OperationContext operationContext)
		{
			try
			{
				base.Sync.AcquireReaderLock(-1);
				return this.InternalCache.GetStreamLength(key, lockHandle, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override Hashtable GetTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.GetTag(tags, comparisonType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override bool HasEnumerationPointer(EnumerationPointer pointer)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.HasEnumerationPointer(pointer);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			return this.Insert(keys, cacheEntries, notify, null, operationContext);
		}

		public override Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, string taskId, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Insert(keys, cacheEntries, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Insert(key, cacheEntry, notify, (string)null, lockId, version, accessType, operationContext);
		}

		public override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, bool IsUserOperation, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Insert(key, cacheEntry, notify, IsUserOperation, lockId, version, accessType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, string taskId, object lockId, ulong version, LockAccessType access, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Insert(key, cacheEntry, notify, lockId, version, access, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Verifica se o item do cache associado com a chave está bloqueado.
		/// </summary>
		/// <param name="key">Chave o item do cache.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public override LockOptions IsLocked(object key, ref object lockId, ref DateTime lockDate, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.IsLocked(key, ref lockId, ref lockDate, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
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
		public override LockOptions Lock(object key, LockExpiration lockExpiration, ref object lockId, ref DateTime lockDate, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Lock(key, lockExpiration, ref lockId, ref lockDate, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override bool OpenStream(string key, string lockHandle, StreamModes mode, string group, string subGroup, ExpirationHint hint, EvictionHint evictinHint, OperationContext operationContext)
		{
			try
			{
				base.Sync.AcquireWriterLock(-1);
				return this.InternalCache.OpenStream(key, lockHandle, mode, group, subGroup, hint, evictinHint, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override int ReadFromStream(ref VirtualArray vBuffer, string key, string lockHandle, int offset, int length, OperationContext operationContext)
		{
			try
			{
				base.Sync.AcquireReaderLock(-1);
				return this.InternalCache.ReadFromStream(ref vBuffer, key, lockHandle, offset, length, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override void RegisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.RegisterKeyNotification(key, updateCallback, removeCallback, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void RegisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.RegisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable Remove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Remove(group, subGroup, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable Remove(object[] keys, ItemRemoveReason ir, bool notify, OperationContext operationContext)
		{
			return this.Remove(keys, ir, notify, (string)null, operationContext);
		}

		public override Hashtable Remove(object[] keys, ItemRemoveReason ir, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Remove(keys, ir, notify, isUserOperation, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable Remove(object[] keys, ItemRemoveReason ir, bool notify, string taskId, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Remove(keys, ir, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada que será apagada.</param>
		/// <param name="ir"></param>
		/// <param name="notify"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override CacheEntry Remove(object key, ItemRemoveReason ir, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Remove(key, ir, notify, (string)null, lockId, version, accessType, operationContext);
		}

		public override CacheEntry Remove(object key, ItemRemoveReason ir, bool notify, bool isUserOperation, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Remove(key, ir, notify, isUserOperation, lockId, version, accessType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada que será apagada.</param>
		/// <param name="ir"></param>
		/// <param name="notify"></param>
		/// <param name="taskId"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override CacheEntry Remove(object key, ItemRemoveReason ir, bool notify, string taskId, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.Remove(key, ir, notify, lockId, version, accessType, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void RemoveBucket(int bucket)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.RemoveBucket(bucket);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void RemoveBucketData(int bucketId)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.RemoveBucketData(bucketId);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable RemoveByTag(string[] tags, TagComparisonType tagComparisonType, bool notify, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.RemoveByTag(tags, tagComparisonType, notify, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override Hashtable RemoveDepKeyList(Hashtable table, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				return this.Internal.RemoveDepKeyList(table, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void RemoveExtraBuckets(ArrayList bucketIds)
		{
			try
			{
				base.Sync.AcquireWriterLock(-1);
				_cache.RemoveExtraBuckets(bucketIds);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void RemoveFromLogTbl(int bucketId)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.RemoveFromLogTbl(bucketId);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override object RemoveSync(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			return this.Internal.RemoveSync(keys, reason, notify, operationContext);
		}

		/// <summary>
		/// Realiza a pesquisa.
		/// </summary>
		/// <param name="query">Texto do comando de pesquisa.</param>
		/// <param name="values"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override QueryResultSet Search(string query, IDictionary values, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.Search(query, values, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override QueryResultSet SearchEntries(string query, IDictionary values, OperationContext operationContext)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.SearchEntries(query, values, operationContext);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Realiza a junção dos indices associados aos tipos informados.
		/// </summary>
		/// <param name="leftTypeName">Nome do tipo da esquerda.</param>
		/// <param name="leftFieldName">Nome do campo do tipo da esquerda.</param>
		/// <param name="rightTypeName">Nome do tipo da direita.</param>
		/// <param name="rightFieldName">Nome do campo do tipo da direita.</param>
		/// <param name="comparisonType">Tipo de comparação que será utilizada.</param>
		/// <returns></returns>
		public override IEnumerable<object[]> JoinIndex(string leftTypeName, string leftFieldName, string rightTypeName, string rightFieldName, ComparisonType comparisonType)
		{
			base.Sync.AcquireReaderLock(-1);
			try
			{
				return this.Internal.JoinIndex(leftTypeName, leftFieldName, rightTypeName, rightFieldName, comparisonType);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
		}

		public override void SendNotification(object notifId, object data)
		{
			this.Internal.SendNotification(notifId, data);
		}

		public override void StartLogging(int bucketId)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.StartLogging(bucketId);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void UnLock(object key, object lockId, bool isPreemptive, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.UnLock(key, lockId, isPreemptive, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void UnregisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.UnregisterKeyNotification(key, updateCallback, removeCallback, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void UnregisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.UnregisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void UpdateLocalBuckets(ArrayList bucketIds)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_cache.UpdateLocalBuckets(bucketIds);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		internal override void UpdateLockInfo(object key, object lockId, DateTime lockDate, LockExpiration lockExpiration, OperationContext operationContext)
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				this.Internal.UpdateLockInfo(key, lockId, lockDate, lockExpiration, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		public override void WriteToStream(string key, string lockHandle, VirtualArray vBuffer, int srcOffset, int dstOffset, int length, OperationContext operationContext)
		{
			try
			{
				base.Sync.AcquireWriterLock(-1);
				this.InternalCache.WriteToStream(key, lockHandle, vBuffer, srcOffset, dstOffset, length, operationContext);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}
	}
}
