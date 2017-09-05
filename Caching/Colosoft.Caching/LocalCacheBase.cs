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
using Colosoft.Caching.Queries.Filters;
using System.Collections;
using Colosoft.Caching.Queries;
using Colosoft.Caching.Expiration;
using Colosoft.Text.Parser;
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Locking;
using Colosoft.Caching.Util;
using Colosoft.Caching.Data;
using Colosoft.Caching.Policies;
using Colosoft.Serialization;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching.Local
{
	/// <summary>
	/// Implementação base de um cache local.
	/// </summary>
	internal class LocalCacheBase : CacheBase
	{
		protected ActiveQueryAnalyzer _activeQueryAnalyzer;

		internal bool _allowAsyncEviction;

		protected CacheBase _parentCache;

		private int _preparedQueryEvictionPercentage;

		private Hashtable _preparedQueryTable;

		private int _preparedQueryTableSize;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public LocalCacheBase()
		{
			_allowAsyncEviction = true;
			_preparedQueryTable = Hashtable.Synchronized(new Hashtable());
			_preparedQueryTableSize = 0x3e8;
			_preparedQueryEvictionPercentage = 10;
		}

		/// <summary>
		/// Cria a instancia já com os parametros iniciais.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="parentCache"></param>
		/// <param name="listener"></param>
		/// <param name="context"></param>
		/// <param name="activeQueryAnalyzer"></param>
		public LocalCacheBase(IDictionary properties, CacheBase parentCache, ICacheEventsListener listener, CacheRuntimeContext context, ActiveQueryAnalyzer activeQueryAnalyzer) : base(properties, listener, context)
		{
			_allowAsyncEviction = true;
			_preparedQueryTable = Hashtable.Synchronized(new Hashtable());
			_preparedQueryTableSize = 1000;
			_preparedQueryEvictionPercentage = 10;
			_activeQueryAnalyzer = activeQueryAnalyzer;
			_parentCache = parentCache;
		}

		public sealed override bool Add(object key, ExpirationHint expirationHint, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			bool flag = this.AddInternal(key, expirationHint, operationContext);
			if(flag)
			{
				expirationHint.CacheKey = (string)key;
				if(!expirationHint.Reset(base._context))
				{
					this.RemoveInternal(key, expirationHint);
					throw new OperationFailedException("Unable to initialize expiration hint");
				}
				bool hasDependentKeys = (entry.KeysDependingOnMe != null) && (entry.KeysDependingOnMe.Count > 0);
				base._context.ExpiryMgr.UpdateIndex(key, expirationHint, hasDependentKeys);
			}
			return flag;
		}

		public sealed override bool Add(object key, Colosoft.Caching.Synchronization.CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			base._context.SyncManager.AddDependency(key, syncDependency);
			bool flag = this.AddInternal(key, syncDependency);
			if(!flag)
			{
				base._context.SyncManager.RemoveDependency(key, syncDependency);
			}
			return flag;
		}

		public sealed override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, OperationContext operationContext)
		{
			return this.Add(key, cacheEntry, notify, true, operationContext);
		}

		public sealed override Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					CacheAddResult result = this.Add(keys[i], cacheEntries[i], notify, operationContext);
					hashtable.Add(keys[i], result);
				}
				catch(StateTransferException exception)
				{
					hashtable.Add(keys[i], exception);
				}
				catch(Exception exception2)
				{
					hashtable.Add(keys[i], new OperationFailedException(exception2.Message, exception2));
				}
			}
			return hashtable;
		}

		public sealed override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			CacheAddResult failure = CacheAddResult.Failure;
			if(cacheEntry.SyncDependency != null)
			{
				base._context.SyncManager.AddDependency(key, cacheEntry.SyncDependency);
			}
			failure = this.AddInternal(key, cacheEntry, isUserOperation);
			if((failure == CacheAddResult.Failure) && (cacheEntry.SyncDependency != null))
			{
				base._context.SyncManager.RemoveDependency(key, cacheEntry.SyncDependency);
			}
			if((failure == CacheAddResult.NeedsEviction) || (failure == CacheAddResult.SuccessNearEviction))
			{
				this.Evict();
				if(failure == CacheAddResult.SuccessNearEviction)
				{
					failure = CacheAddResult.Success;
				}
			}
			if(failure == CacheAddResult.KeyExists)
			{
				if(cacheEntry.SyncDependency != null)
				{
					base._context.SyncManager.RemoveDependency(key, cacheEntry.SyncDependency);
				}
				CacheEntry entry = this.GetInternal(key, isUserOperation, operationContext);
				if((entry.ExpirationHint != null) && entry.ExpirationHint.CheckExpired(base._context))
				{
					this.Remove(key, ItemRemoveReason.Expired, true, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
				}
			}
			if(failure == CacheAddResult.Success)
			{
				if(cacheEntry.ExpirationHint != null)
				{
					cacheEntry.ExpirationHint.CacheKey = (string)key;
					try
					{
						base._context.ExpiryMgr.ResetHint(null, cacheEntry.ExpirationHint);
					}
					catch
					{
						this.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
						throw;
					}
					base._context.ExpiryMgr.UpdateIndex(key, cacheEntry);
				}
				if(notify)
				{
					this.NotifyItemAdded(key, false);
				}
				if(operationContext.Contains(OperationContextFieldName.RaiseCQNotification))
				{
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemAdded(key, cacheEntry, this, base._context.CacheRoot.Name, (bool)operationContext.GetValueByField(OperationContextFieldName.RaiseCQNotification));
				}
				else
				{
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemAdded(key, cacheEntry, this, base._context.CacheRoot.Name, false);
				}
			}
			if((failure == CacheAddResult.NeedsEviction) && (cacheEntry.SyncDependency != null))
				base._context.SyncManager.RemoveDependency(key, cacheEntry.SyncDependency);
			return failure;
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
			var result = new Hashtable();
			IDictionaryEnumerator enumerator = table.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CacheEntry cacheEntry = this.Get(enumerator.Key, operationContext);
				if(cacheEntry == null)
					result.Add(enumerator.Key, false);
				else
				{
					if(cacheEntry.KeysDependingOnMe == null)
						cacheEntry.KeysDependingOnMe = new Hashtable();
					ArrayList list = (ArrayList)enumerator.Value;
					for(int i = 0; i < list.Count; i++)
					{
						if(!cacheEntry.KeysDependingOnMe.Contains(list[i]))
							cacheEntry.KeysDependingOnMe.Add(list[i], null);
					}
					try
					{
						if(this.InsertInternal(enumerator.Key, cacheEntry, false, cacheEntry, operationContext) != CacheInsResult.SuccessOverwrite)
							result.Add(enumerator.Key, false);
						else
						{
							result.Add(enumerator.Key, true);
							base._context.ExpiryMgr.UpdateIndex(enumerator.Key, cacheEntry);
						}
						continue;
					}
					catch(Exception exception)
					{
						result.Add(enumerator.Key, exception);
						continue;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Adiciona uma dependencia de sincronização para o item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="syncDependency">Instancia da dependencia de sincronização.</param>
		/// <returns>True caso a operação tenha sido executada com sucesso,</returns>
		internal virtual bool AddInternal(object key, Colosoft.Caching.Synchronization.CacheSyncDependency syncDependency)
		{
			return false;
		}

		/// <summary>
		/// Adiciona um <see cref="ExpirationHint"/> para o item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="expirationHint"><see cref="ExpirationHint"/> que será adicionado.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>True caso a operação tenha sido executada com sucesso.</returns>
		internal virtual bool AddInternal(object key, ExpirationHint expirationHint, OperationContext operationContext)
		{
			return false;
		}

		/// <summary>
		/// Método interno acionado para adicionar um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada que está sendo adicionada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <returns>Resultado da operação.</returns>
		internal virtual CacheAddResult AddInternal(object key, CacheEntry cacheEntry, bool isUserOperation)
		{
			return CacheAddResult.Failure;
		}

		private void AddPreparedReduction(string query, Reduction currentReduction)
		{
			_preparedQueryTable.Add(new QueryIdentifier(query), currentReduction);
			if(_preparedQueryTable.Count > _preparedQueryTableSize)
			{
				ArrayList list = new ArrayList(_preparedQueryTable.Keys);
				list.Sort();
				int num = (_preparedQueryTable.Count * _preparedQueryEvictionPercentage) / 100;
				for(int i = 0; i < num; i++)
				{
					_preparedQueryTable.Remove(list[i]);
				}
			}
		}

		public sealed override void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, OperationContext operationContext)
		{
			this.ClearInternal();
			base._context.SyncManager.Clear();
			if(_activeQueryAnalyzer != null)
			{
				_activeQueryAnalyzer.Clear();
			}
			if(this.IsSelfInternal)
				base._context.ExpiryMgr.Clear();
			GC.Collect();
			this.NotifyCacheCleared(false);
		}

		internal virtual void ClearInternal()
		{
		}

		public override void CloseStream(string key, string lockHandle, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry != null)
			{
				switch(entry.RWLockManager.Mode)
				{
				case LockMode.Reader:
					if((lockHandle != null) && !entry.RWLockManager.ValidateLock(LockMode.Reader, lockHandle))
					{
						throw new StreamInvalidLockException();
					}
					entry.RWLockManager.ReleaseReaderLock(lockHandle);
					return;
				case LockMode.Write:
					if((lockHandle != null) && !entry.RWLockManager.ValidateLock(LockMode.Write, lockHandle))
					{
						throw new StreamInvalidLockException();
					}
					entry.RWLockManager.ReleaseWriterLock(lockHandle);
					return;
				default:
					return;
				}
			}
		}

		public sealed override bool Contains(object key, OperationContext operationContext)
		{
			if(!this.ContainsInternal(key))
			{
				return false;
			}
			CacheEntry entry = this.GetInternal(key, true, operationContext);
			if(entry == null)
			{
				return false;
			}
			if((entry.ExpirationHint != null) && entry.ExpirationHint.CheckExpired(base._context))
			{
				this.Remove(key, ItemRemoveReason.Expired, true, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
				return false;
			}
			return true;
		}

		public sealed override Hashtable Contains(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			ArrayList list = new ArrayList();
			ArrayList list2 = new ArrayList();
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					if(this.Contains(keys[i], operationContext))
					{
						list.Add(keys[i]);
					}
				}
				catch(StateTransferException)
				{
					list2.Add(keys[i]);
				}
			}
			if(list.Count > 0)
			{
				hashtable["items-found"] = list;
			}
			if(list2.Count > 0)
			{
				hashtable["items-transfered"] = list2;
			}
			return hashtable;
		}

		internal virtual bool ContainsInternal(object key)
		{
			return false;
		}

		public override void Dispose()
		{
			if(_activeQueryAnalyzer != null)
			{
				_activeQueryAnalyzer.Dispose();
				_activeQueryAnalyzer = null;
			}
			base.Dispose();
		}

		public virtual void Evict()
		{
		}

		public sealed override Hashtable Get(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			CacheEntry entry = null;
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					entry = this.Get(keys[i], operationContext);
					if(entry != null)
					{
						hashtable.Add(keys[i], entry);
					}
				}
				catch(StateTransferException exception)
				{
					hashtable.Add(keys[i], exception);
				}
			}
			return hashtable;
		}

		public sealed override CacheEntry Get(object key, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Get(key, true, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
		}

		public sealed override CacheEntry Get(object key, bool isUserOperation, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, isUserOperation, operationContext);
			if(accessType != LockAccessType.IGNORE_LOCK)
			{
				if(entry != null)
				{
					if(accessType == LockAccessType.DONT_ACQUIRE)
					{
						bool flag = entry.CompareLock(lockId);
						if(flag)
						{
							lockDate = entry.LockDate;
						}
						else
						{
							flag = !entry.IsLocked(ref lockId, ref lockDate);
						}
						if(!flag)
						{
							entry = null;
						}
					}
					else if((accessType == LockAccessType.ACQUIRE) && !entry.Lock(lockExpiration, ref lockId, ref lockDate))
					{
						entry = null;
					}
					else if(accessType == LockAccessType.GET_VERSION)
					{
						version = entry.Version;
					}
					else if(accessType == LockAccessType.COMPARE_VERSION)
					{
						if(entry.IsNewer(version))
						{
							version = entry.Version;
						}
						else
						{
							version = 0;
							entry = null;
						}
					}
					else if((accessType == LockAccessType.MATCH_VERSION) && !entry.CompareVersion(version))
					{
						entry = null;
					}
				}
				else
				{
					lockId = null;
				}
			}
			ExpirationHint hint = (entry == null) ? null : entry.ExpirationHint;
			if(hint != null)
			{
				if(hint.CheckExpired(base._context) && !hint.NeedsReSync)
				{
					this.Remove(key, ItemRemoveReason.Expired, true, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
					entry = null;
				}
				if(hint.IsVariant && isUserOperation)
				{
					try
					{
						base._context.ExpiryMgr.ResetVariant(hint);
					}
					catch
					{
						this.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
						throw;
					}
				}
			}
			return entry;
		}

		private Hashtable GetEntries(object[] keys)
		{
			Hashtable hashtable = new Hashtable();
			CacheEntry entryInternal = null;
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					entryInternal = this.GetEntryInternal(keys[i], true);
					if(entryInternal != null)
					{
						hashtable[keys[i]] = entryInternal;
					}
				}
				catch(Exception exception)
				{
					hashtable[keys[i]] = exception;
				}
			}
			return hashtable;
		}

		internal virtual CacheEntry GetEntryInternal(object key, bool isUserOperation)
		{
			return null;
		}

		private Hashtable GetFromCache(ArrayList keys, OperationContext operationContext)
		{
			if(keys == null)
			{
				return null;
			}
			return this.GetEntries(keys.ToArray());
		}

		internal virtual CacheEntry GetInternal(object key, bool isUserOperation, OperationContext operationContext)
		{
			return null;
		}

		public override int GetItemSize(object key)
		{
			return 0;
		}

		/// <summary>
		/// Recupera a redução para a consulta informada.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private Reduction GetPreparedReduction(string query)
		{
			Reduction currentReduction = null;
			lock (_preparedQueryTable.SyncRoot)
			{
				if(!_preparedQueryTable.ContainsKey(query.ToLower()))
				{
					var helper = new ParserHelper(this.InternalCache.Logger);
					if(helper.Parse(query) != ParseMessage.Accept)
						throw new ParserException("Incorrect query format");
					currentReduction = helper.CurrentReduction;
					this.AddPreparedReduction(query, currentReduction);
					return currentReduction;
				}
				return (Reduction)_preparedQueryTable[query.ToLower()];
			}
		}

		/// <summary>
		/// Recupera o tamanho da stream associada com a entrada do cache
		/// </summary>
		/// <param name="key"></param>
		/// <param name="lockHandle"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override long GetStreamLength(string key, string lockHandle, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry == null)
				throw new StreamNotFoundException();
			if(!string.IsNullOrEmpty(lockHandle) && !entry.RWLockManager.ValidateLock(lockHandle))
				throw new StreamInvalidLockException();
			return (long)entry.Length;
		}

		public override Hashtable GetTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			return this.GetFromCache(this.GetKeysByTag(tags, comparisonType, operationContext), operationContext);
		}

		public sealed override Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					CacheInsResultWithEntry entry = this.Insert(keys[i], cacheEntries[i], notify, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
					hashtable.Add(keys[i], entry);
				}
				catch(Exception exception)
				{
					hashtable.Add(keys[i], exception);
				}
			}
			return hashtable;
		}

		public sealed override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Insert(key, cacheEntry, notify, true, lockId, version, accessType, operationContext);
		}

		public sealed override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, bool isUserOperation, object lockId, ulong version, LockAccessType access, OperationContext operationContext)
		{
			CacheInsResultWithEntry entry = new CacheInsResultWithEntry();
			CacheEntry entry2 = null;
			CallbackEntry entry3 = null;
			entry2 = this.GetInternal(key, false, operationContext);
			entry.Entry = entry2;
			if((entry2 != null) && (access != LockAccessType.IGNORE_LOCK))
			{
				if(access == LockAccessType.COMPARE_VERSION)
				{
					if(!entry2.CompareVersion(version))
					{
						entry.Result = CacheInsResult.VersionMismatch;
						entry.Entry = null;
						return entry;
					}
				}
				else
				{
					if(((access == LockAccessType.RELEASE) || (access == LockAccessType.DONT_RELEASE)) && (entry2.IsItemLocked() && !entry2.CompareLock(lockId)))
					{
						entry.Result = CacheInsResult.ItemLocked;
						entry.Entry = null;
						return entry;
					}
					if(access == LockAccessType.DONT_RELEASE)
						cacheEntry.CopyLock(entry2.LockId, entry2.LockDate, entry2.LockExpiration);
					else
						cacheEntry.ReleaseLock();
				}
			}
			ExpirationHint oldHint = (entry2 == null) ? null : entry2.ExpirationHint;
			if((entry2 != null) && (entry2.Value is CallbackEntry))
			{
				entry3 = entry2.Value as CallbackEntry;
				cacheEntry = CacheHelper.MergeEntries(entry2, cacheEntry);
			}
			if(cacheEntry.SyncDependency != null)
				base._context.SyncManager.AddDependency(key, cacheEntry.SyncDependency);
			if(access == LockAccessType.PRESERVE_VERSION)
			{
				cacheEntry.Version = version;
				isUserOperation = false;
			}
			entry.Result = this.InsertInternal(key, cacheEntry, isUserOperation, entry2, operationContext);
			if(((entry.Result == CacheInsResult.Failure) || (entry.Result == CacheInsResult.IncompatibleGroup)) && (cacheEntry.SyncDependency != null))
				base._context.SyncManager.RemoveDependency(key, cacheEntry.SyncDependency);
			if(((entry.Result == CacheInsResult.NeedsEviction) || (entry.Result == CacheInsResult.SuccessNearEvicition)) || (entry.Result == CacheInsResult.SuccessOverwriteNearEviction))
			{
				this.Evict();
				if(entry.Result == CacheInsResult.SuccessNearEvicition)
					entry.Result = CacheInsResult.Success;
				if(entry.Result == CacheInsResult.SuccessOverwriteNearEviction)
					entry.Result = CacheInsResult.SuccessOverwrite;
			}
			if(!((entry.Result != CacheInsResult.Success) && (entry.Result != CacheInsResult.SuccessOverwrite)))
			{
				if(oldHint != null)
					base._context.ExpiryMgr.RemoveFromIndex(key);
				if(cacheEntry.ExpirationHint != null)
				{
					cacheEntry.ExpirationHint.CacheKey = (string)key;
					if(isUserOperation)
					{
						try
						{
							base._context.ExpiryMgr.ResetHint(oldHint, cacheEntry.ExpirationHint);
						}
						catch
						{
							this.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
							throw;
						}
					}
					else
						cacheEntry.ExpirationHint.ReInitializeHint(base.Context);
					base._context.ExpiryMgr.UpdateIndex(key, cacheEntry);
				}
				if(operationContext.Contains(OperationContextFieldName.RaiseCQNotification))
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemUpdated(key, cacheEntry, this, base._context.CacheRoot.Name, (bool)operationContext.GetValueByField(OperationContextFieldName.RaiseCQNotification));
				else
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemUpdated(key, cacheEntry, this, base._context.CacheRoot.Name, false);
			}
			if(((entry.Result == CacheInsResult.NeedsEviction) || (entry.Result == CacheInsResult.NeedsEvictionNotRemove)) && (cacheEntry.SyncDependency != null))
				base._context.SyncManager.AddDependency(key, cacheEntry.SyncDependency);
			switch(entry.Result)
			{
			case CacheInsResult.Success:
				if(notify)
					this.NotifyItemAdded(key, false);
				return entry;
			case CacheInsResult.SuccessOverwrite:
				if(notify)
				{
					if(((entry3 != null) && (entry3.ItemUpdateCallbackListener != null)) && (entry3.ItemUpdateCallbackListener.Count > 0))
						this.NotifyCustomUpdateCallback(key, entry3.ItemUpdateCallbackListener, false);
					this.NotifyItemUpdated(key, false, operationContext);
				}
				return entry;
			}
			return entry;
		}

		/// <summary>
		/// Método interno usado para inserir uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <param name="oldEntry">Valor da antiga entrada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Resulta da operação.</returns>
		internal virtual CacheInsResult InsertInternal(object key, CacheEntry cacheEntry, bool isUserOperation, CacheEntry oldEntry, OperationContext operationContext)
		{
			return CacheInsResult.Failure;
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
			LockOptions options = new LockOptions();
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry != null)
			{
				entry.IsLocked(ref lockId, ref lockDate);
				options.LockDate = lockDate;
				options.LockId = lockId;
				return options;
			}
			lockId = null;
			return options;
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
			LockOptions options = new LockOptions();
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry != null)
			{
				entry.Lock(lockExpiration, ref lockId, ref lockDate);
				options.LockDate = lockDate;
				options.LockId = lockId;
				return options;
			}
			options.LockId = lockId = null;
			return options;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override bool OpenStream(string key, string lockHandle, StreamModes mode, string group, string subGroup, ExpirationHint hint, EvictionHint evictinHint, OperationContext operationContext)
		{
			bool flag = false;
			CacheEntry cacheEntry = this.GetInternal(key, false, operationContext);
			if((cacheEntry != null) && !CacheHelper.CheckDataGroupsCompatibility(new GroupInfo(group, subGroup), cacheEntry.GroupInfo))
			{
				throw new OperationFailedException("Data group of the stream does not match the existing stream's data group");
			}
			switch(mode)
			{
			case StreamModes.Read:
				if(cacheEntry == null)
				{
					throw new StreamNotFoundException();
				}
				flag = cacheEntry.RWLockManager.AcquireReaderLock(lockHandle);
				break;
			case StreamModes.ReadWithoutLock:
				if(cacheEntry == null)
				{
					throw new StreamNotFoundException();
				}
				flag = true;
				break;
			case StreamModes.Write:
				if(cacheEntry != null)
				{
					flag = cacheEntry.RWLockManager.AcquireWriterLock(lockHandle);
					break;
				}
				cacheEntry = new CacheEntry(new byte[0], hint, evictinHint);
				cacheEntry.GroupInfo = new GroupInfo(group, subGroup);
				flag = cacheEntry.RWLockManager.AcquireWriterLock(lockHandle);
				if(flag)
				{
					this.Insert(key, cacheEntry, true, true, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
				}
				return flag;
			}
			if(!flag)
			{
				throw new StreamAlreadyLockedException();
			}
			return flag;
		}

		/// <summary>
		/// Prepara a pesquisa para ser executada.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		private QueryContext PrepareSearch(string query, IDictionary values)
		{
			Reduction preparedReduction = null;
			QueryContext context;
			try
			{
				preparedReduction = this.GetPreparedReduction(query);
				context = this.SearchInternal(preparedReduction.Tag as Predicate, values);
			}
			catch(ParserException exception)
			{
				this.RemoveReduction(query);
				throw new ParserException(exception.Message, exception);
			}
			return context;
		}

		public override int ReadFromStream(ref VirtualArray vBuffer, string key, string lockHandle, int offset, int length, OperationContext operationContext)
		{
			CacheEntry entry = this.Get(key, true, operationContext);
			if(entry == null)
			{
				throw new StreamNotFoundException();
			}
			if(!string.IsNullOrEmpty(lockHandle) && !entry.RWLockManager.ValidateLock(LockMode.Reader, lockHandle))
			{
				throw new StreamInvalidLockException();
			}
			vBuffer = entry.Read(offset, length);
			return (int)vBuffer.Size;
		}

		public override void RegisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(keys != null)
			{
				foreach (string str in keys)
				{
					this.RegisterKeyNotification(str, updateCallback, removeCallback, operationContext);
				}
			}
		}

		public override void RegisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			CacheEntry entry = this.Get(key, operationContext);
			if(entry != null)
			{
				entry.AddCallbackInfo(updateCallback, removeCallback);
			}
		}

		public sealed override Hashtable Remove(object[] keys, ItemRemoveReason removalReason, bool notify, OperationContext operationContext)
		{
			return this.Remove(keys, removalReason, notify, true, operationContext);
		}

		public override Hashtable Remove(object[] keys, ItemRemoveReason removalReason, bool notify, bool isUserOperation, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					CacheEntry entry = this.Remove(keys[i], removalReason, notify, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
					if(entry != null)
					{
						hashtable.Add(keys[i], entry);
					}
				}
				catch(Exception exception)
				{
					hashtable.Add(keys[i], exception);
				}
			}
			return hashtable;
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada que será removida.</param>
		/// <param name="removalReason"></param>
		/// <param name="notify"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public sealed override CacheEntry Remove(object key, ItemRemoveReason removalReason, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			return this.Remove(key, removalReason, notify, true, lockId, version, accessType, operationContext);
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="removalReason"></param>
		/// <param name="notify"></param>
		/// <param name="isUserOperation"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override CacheEntry Remove(object key, ItemRemoveReason removalReason, bool notify, bool isUserOperation, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry val = null;
			CacheEntry entry2 = null;
			object obj2 = key;
			if(key is object[])
			{
				obj2 = ((object[])key)[0];
			}
			if(accessType == LockAccessType.COMPARE_VERSION)
			{
				if(!this.GetInternal(obj2, false, operationContext).CompareVersion(version))
					throw new LockingException("Item in the cache does not exist at the specified version.");
			}
			else if(accessType != LockAccessType.IGNORE_LOCK)
			{
				entry2 = this.GetInternal(obj2, false, operationContext);
				if(((entry2 != null) && entry2.IsItemLocked()) && !entry2.CompareLock(lockId))
					throw new LockingException("Item is locked.");
			}
			val = this.RemoveInternal(obj2, removalReason, isUserOperation, operationContext);
			if(val != null)
			{
				try
				{
					base._context.ExpiryMgr.ResetHint(val.ExpirationHint, null);
					if(val.ExpirationHint != null)
					{
						base._context.ExpiryMgr.RemoveFromIndex(key);
					}
				}
				catch(Exception exception)
				{
					base.Logger.Error(("LocalCacheBase.Remove(object, ItemRemovedReason, bool):" + exception.ToString()).GetFormatter());
				}
				if(val.SyncDependency != null)
				{
					base._context.SyncManager.RemoveDependency(obj2, val.SyncDependency);
				}
				if(this.IsSelfInternal)
					((IDisposable)val).Dispose();
				if(notify)
				{
					CallbackEntry entry3 = val.Value as CallbackEntry;
					if(((entry3 != null) && (entry3.ItemRemoveCallbackListener != null)) && (entry3.ItemRemoveCallbackListener.Count > 0))
						this.NotifyCustomRemoveCallback(obj2, entry3, removalReason, false);
					this.NotifyItemRemoved(obj2, val, removalReason, false, operationContext);
				}
				if(operationContext.Contains(OperationContextFieldName.RaiseCQNotification))
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemRemoved(key, val, this, base._context.CacheRoot.Name, (bool)operationContext.GetValueByField(OperationContextFieldName.RaiseCQNotification));
				else
					((IQueryOperationsObserver)_activeQueryAnalyzer).OnItemRemoved(key, val, this, base._context.CacheRoot.Name, false);
			}
			return val;
		}

		public override Hashtable RemoveByTag(string[] tags, TagComparisonType tagComparisonType, bool notify, OperationContext operationContext)
		{
			return this.RemoveFromCache(this.GetKeysByTag(tags, tagComparisonType, operationContext), notify, operationContext);
		}

		public override Hashtable RemoveDepKeyList(Hashtable table, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			if(table == null)
			{
				return null;
			}
			IDictionaryEnumerator enumerator = table.GetEnumerator();
			while (enumerator.MoveNext())
			{
				try
				{
					CacheEntry cacheEntry = this.GetInternal(enumerator.Key, true, operationContext);
					if(cacheEntry != null)
					{
						if(cacheEntry.KeysDependingOnMe != null)
						{
							ArrayList list = (ArrayList)enumerator.Value;
							for(int i = 0; i < list.Count; i++)
							{
								cacheEntry.KeysDependingOnMe.Remove(list[i]);
							}
						}
						if(this.InsertInternal(enumerator.Key, cacheEntry, false, cacheEntry, operationContext) != CacheInsResult.SuccessOverwrite)
						{
							hashtable.Add(enumerator.Key, false);
						}
						else
						{
							hashtable.Add(enumerator.Key, true);
							base._context.ExpiryMgr.UpdateIndex(enumerator.Key, cacheEntry);
						}
					}
					else
					{
						hashtable.Add(enumerator.Key, false);
					}
					continue;
				}
				catch(Exception exception)
				{
					hashtable.Add(enumerator.Key, exception);
					continue;
				}
			}
			return hashtable;
		}

		private Hashtable RemoveFromCache(ArrayList keys, bool notify, OperationContext operationContext)
		{
			if(keys == null)
			{
				return null;
			}
			return this.Remove(keys.ToArray(), ItemRemoveReason.Removed, notify, operationContext);
		}

		internal virtual bool RemoveInternal(object key, ExpirationHint eh)
		{
			return false;
		}

		/// <summary>
		/// Remove a entrada. (Método interno)
		/// </summary>
		/// <param name="key"></param>
		/// <param name="removalReason"></param>
		/// <param name="isUserOperation"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal virtual CacheEntry RemoveInternal(object key, ItemRemoveReason removalReason, bool isUserOperation, OperationContext operationContext)
		{
			return null;
		}

		private void RemoveReduction(string query)
		{
			lock (_preparedQueryTable.SyncRoot)
			{
				_preparedQueryTable.Remove(query.ToLower());
			}
		}

		public override object RemoveSync(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			if(_parentCache != null)
			{
				return _parentCache.RemoveSync(keys, reason, notify, operationContext);
			}
			return null;
		}

		/// <summary>
		/// Realiza a pesquisa informada;
		/// </summary>
		/// <param name="query">Texto da pesquisa que será realizada.</param>
		/// <param name="values">Valores que serão utilizados na pesquisa.</param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public sealed override QueryResultSet Search(string query, IDictionary values, OperationContext operationContext)
		{
			QueryResultSet resultSet;
			try
			{
				QueryContext context = this.PrepareSearch(query, values);
				if(context.ResultSet.Type != QueryType.AggregateFunction)
				{
					context.Tree.Reduce();
					context.CacheContext = base._context.SerializationContext;
					context.ResultSet.SearchKeysResult = context.Tree.LeftList;
				}
				resultSet = context.ResultSet;
			}
			catch(ParserException exception)
			{
				this.RemoveReduction(query);
				throw new ParserException(exception.Message, exception);
			}
			return resultSet;
		}

		/// <summary>
		/// Realiza a pesquisa das entradas.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public sealed override QueryResultSet SearchEntries(string query, IDictionary values, OperationContext operationContext)
		{
			QueryResultSet resultSet;
			try
			{
				QueryContext context = this.PrepareSearch(query, values);
				if(context.ResultSet.Type != QueryType.AggregateFunction)
				{
					Hashtable hashtable = new Hashtable();
					ICollection leftList = null;
					context.Tree.Reduce();
					context.CacheContext = base._context.SerializationContext;
					if(context.Tree.LeftList.Count > 0)
					{
						leftList = context.Tree.LeftList;
					}
					if((leftList != null) && (leftList.Count > 0))
					{
						object[] array = new object[leftList.Count];
						leftList.CopyTo(array, 0);
						IDictionaryEnumerator enumerator = this.GetEntries(array).GetEnumerator();
						CompressedValueEntry entry = null;
						while (enumerator.MoveNext())
						{
							CacheEntry entry2 = enumerator.Value as CacheEntry;
							if(entry2 != null)
							{
								entry = new CompressedValueEntry();
								entry.Value = entry2.Value;
								if(entry.Value is CallbackEntry)
								{
									entry.Value = ((CallbackEntry)entry.Value).Value;
								}
								entry.Flag = ((CacheEntry)enumerator.Value).Flag;
								hashtable[enumerator.Key] = entry;
							}
						}
					}
					context.ResultSet.Type = QueryType.SearchEntries;
					context.ResultSet.SearchEntriesResult = hashtable;
				}
				resultSet = context.ResultSet;
			}
			catch(ParserException exception)
			{
				this.RemoveReduction(query);
				throw new ParserException(exception.Message, exception);
			}
			return resultSet;
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
		public sealed override IEnumerable<object[]> JoinIndex(string leftTypeName, string leftFieldName, string rightTypeName, string rightFieldName, ComparisonType comparisonType)
		{
			var leftQueryContext = new QueryContext(this) {
				TypeName = leftTypeName
			};
			IIndexStore leftRBStore = null;
			var enumerator = leftQueryContext.Index.GetEnumerator();
			while (enumerator.MoveNext())
				if((string)enumerator.Key == leftFieldName)
				{
					leftRBStore = (IIndexStore)enumerator.Value;
					break;
				}
			if(leftRBStore == null)
				throw new CacheException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_NotFoundIndexForAttributeException, leftFieldName, leftTypeName).Format());
			var rightQueryContext = new QueryContext(this) {
				TypeName = rightTypeName
			};
			IIndexStore rightStore = null;
			enumerator = rightQueryContext.Index.GetEnumerator();
			while (enumerator.MoveNext())
				if((string)enumerator.Key == rightFieldName)
				{
					rightStore = (IIndexStore)enumerator.Value;
					break;
				}
			if(rightStore == null)
				throw new CacheException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_NotFoundIndexForAttributeException, rightFieldName, rightTypeName).Format());
			var leftItemsEnumerator = leftRBStore.GetEnumerator();
			while (leftItemsEnumerator.MoveNext())
			{
				var rightItems = rightStore.GetData(leftItemsEnumerator.Key, comparisonType);
				if(rightItems.Count > 0)
				{
					foreach (DictionaryEntry i in (System.Collections.Hashtable)leftItemsEnumerator.Value)
					{
						foreach (var j in rightItems)
							yield return new object[] {
								i.Key,
								j
							};
					}
				}
			}
		}

		internal virtual IDictionary SearchEntriesInternal(Predicate pred, IDictionary values)
		{
			return null;
		}

		/// <summary>
		/// Realiza a pesquisa utilizando o predicado informado.
		/// </summary>
		/// <param name="pred"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		internal virtual QueryContext SearchInternal(Predicate pred, IDictionary values)
		{
			return null;
		}

		public sealed override void SendNotification(object notifId, object data)
		{
			base.NotifyCustomEvent(notifId, data, false);
		}

		public override void UnLock(object key, object lockId, bool isPreemptive, OperationContext operationContext)
		{
			DateTime now = DateTime.Now;
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry != null)
			{
				if(isPreemptive)
				{
					entry.ReleaseLock();
				}
				else if(entry.CompareLock(lockId))
				{
					entry.ReleaseLock();
				}
			}
		}

		public override void UnregisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			CacheEntry entry = this.Get(key, operationContext);
			if(entry != null)
			{
				entry.RemoveCallbackInfo(updateCallback, removeCallback);
			}
		}

		public override void UnregisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			if(keys != null)
			{
				foreach (string str in keys)
				{
					this.UnregisterKeyNotification(str, updateCallback, removeCallback, operationContext);
				}
			}
		}

		internal override void UpdateLockInfo(object key, object lockId, DateTime lockDate, LockExpiration lockExpiration, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry != null)
			{
				entry.CopyLock(lockId, lockDate, lockExpiration);
			}
		}

		public override void WriteToStream(string key, string lockHandle, VirtualArray vBuffer, int srcOffset, int dstOffset, int length, OperationContext operationContext)
		{
			CacheEntry entry = this.GetInternal(key, false, operationContext);
			if(entry == null)
				throw new StreamNotFoundException();
			if((lockHandle == null) || !entry.RWLockManager.ValidateLock(LockMode.Write, lockHandle))
				throw new StreamInvalidLockException();
			CacheEntry cacheEntry = entry.Clone() as CacheEntry;
			this.Remove(key, ItemRemoveReason.Removed, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
			cacheEntry.Write(vBuffer, srcOffset, dstOffset, length);
			CacheInsResultWithEntry entry3 = this.Insert(key, cacheEntry, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
			if((entry3 != null) && (entry3.Result == CacheInsResult.NeedsEviction))
				throw new CacheException("The cache is full and not enough items could be evicted.");
		}

		protected internal override CacheBase InternalCache
		{
			get
			{
				return this;
			}
		}

		protected bool IsSelfInternal
		{
			get
			{
				if(base._context.CacheInternal is CacheSyncWrapper)
				{
					return object.ReferenceEquals(this, ((CacheSyncWrapper)base._context.CacheInternal).Internal);
				}
				return object.ReferenceEquals(this, base._context.CacheInternal);
			}
		}

		public Hashtable PreparedQueryTable
		{
			get
			{
				return _preparedQueryTable;
			}
		}

		public override ActiveQueryAnalyzer QueryAnalyzer
		{
			get
			{
				return _activeQueryAnalyzer;
			}
		}
	}
}
