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
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Policies;
using Colosoft.Caching.Util;
using Colosoft.Serialization;
using Colosoft.Caching.Queries;
using Colosoft.Caching.Statistics;
using Colosoft.Caching.Synchronization;
using Colosoft.Caching.Data;

namespace Colosoft.Caching.Local
{
	/// <summary>
	/// Implementação do cache local.
	/// </summary>
	internal class LocalCacheImpl : CacheBase, ICacheEventsListener
	{
		private CacheBase _cache;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public LocalCacheImpl()
		{
		}

		/// <summary>
		/// Cria a instancia com base no cache informado.
		/// </summary>
		/// <param name="cache"></param>
		public LocalCacheImpl(CacheBase cache)
		{
			cache.Require("cache").NotNull();
			_cache = cache;
			base._context = cache.InternalCache.Context;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override bool Add(object key, ExpirationHint eh, OperationContext operationContext)
		{
			bool flag = false;
			if(this.Internal != null)
			{
				CacheEntry entry = new CacheEntry();
				entry.ExpirationHint = eh;
				object[] keysIAmDependingOn = entry.KeysIAmDependingOn;
				if(keysIAmDependingOn != null)
				{
					Hashtable hashtable = this.Contains(keysIAmDependingOn, operationContext);
					if(!hashtable.ContainsKey("items-found"))
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if(hashtable["items-found"] == null)
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if(((ArrayList)hashtable["items-found"]).Count != keysIAmDependingOn.Length)
						throw new OperationFailedException("One of the dependency keys does not exist.");
				}
				flag = this.Internal.Add(key, eh, operationContext);
				if(!flag || (keysIAmDependingOn == null))
					return flag;
				var table = new Hashtable();
				for(int i = 0; i < keysIAmDependingOn.Length; i++)
				{
					if(table[keysIAmDependingOn[i]] == null)
						table.Add(keysIAmDependingOn[i], new ArrayList());
					((ArrayList)table[keysIAmDependingOn[i]]).Add(key);
				}
				table = this.Internal.AddDependencyKeyList(table, operationContext);
				if(table == null)
					return flag;
				IDictionaryEnumerator enumerator = table.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if((enumerator.Value is bool) && !((bool)enumerator.Value))
						throw new OperationFailedException("One of the dependency keys does not exist.");
				}
			}
			return flag;
		}

		public override bool Add(object key, CacheSyncDependency syncDependency, OperationContext operationContext)
		{
			return this.Internal.Add(key, syncDependency, operationContext);
		}

		public override Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			ArrayList list = new ArrayList();
			ArrayList list2 = new ArrayList();
			ArrayList list3 = new ArrayList();
			if(this.Internal != null)
			{
				for(int i = 0; i < cacheEntries.Length; i++)
				{
					object[] keysIAmDependingOn = cacheEntries[i].KeysIAmDependingOn;
					if(keysIAmDependingOn != null)
					{
						Hashtable hashtable2 = this.Contains(keysIAmDependingOn, operationContext);
						if((hashtable2.ContainsKey("items-found") && (hashtable2["items-found"] != null)) && (keysIAmDependingOn.Length == ((ArrayList)hashtable2["items-found"]).Count))
						{
							list.Add(keys[i]);
							list3.Add(cacheEntries[i]);
						}
						else
						{
							list2.Add(keys[i]);
						}
					}
					else
					{
						list.Add(keys[i]);
						list3.Add(cacheEntries[i]);
					}
				}
				CacheEntry[] array = new CacheEntry[list3.Count];
				list3.CopyTo(array);
				hashtable = this.Internal.Add(list.ToArray(), array, notify, operationContext);
				for(int j = 0; j < list.Count; j++)
				{
					if(!(hashtable[list[j]] is Exception))
					{
						CacheAddResult result = (CacheAddResult)hashtable[list[j]];
						object[] objArray2 = array[j].KeysIAmDependingOn;
						if((result == CacheAddResult.Success) && (objArray2 != null))
						{
							Hashtable table = new Hashtable();
							for(int m = 0; m < objArray2.Length; m++)
							{
								if(table[objArray2[m]] == null)
								{
									table.Add(objArray2[m], new ArrayList());
								}
								((ArrayList)table[objArray2[m]]).Add(list[j]);
							}
							this.Internal.AddDependencyKeyList(table, operationContext);
						}
					}
				}
				for(int k = 0; k < list2.Count; k++)
				{
					hashtable.Add(list2[k], new OperationFailedException("One of the dependency keys does not exist."));
				}
			}
			return hashtable;
		}

		public override CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, OperationContext operationContext)
		{
			CacheAddResult failure = CacheAddResult.Failure;
			if(this.Internal != null)
			{
				object[] keysIAmDependingOn = cacheEntry.KeysIAmDependingOn;
				if(keysIAmDependingOn != null)
				{
					Hashtable hashtable = this.Contains(keysIAmDependingOn, operationContext);
					if(!hashtable.ContainsKey("items-found"))
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if(hashtable["items-found"] == null)
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if((hashtable["items-found"] == null) || (((ArrayList)hashtable["items-found"]).Count != keysIAmDependingOn.Length))
						throw new OperationFailedException("One of the dependency keys does not exist.");
				}
				failure = this.Internal.Add(key, cacheEntry, notify, operationContext);
				if((failure != CacheAddResult.Success) || (keysIAmDependingOn == null))
					return failure;
				Hashtable table = new Hashtable();
				for(int i = 0; i < keysIAmDependingOn.Length; i++)
				{
					if(table[keysIAmDependingOn[i]] == null)
						table.Add(keysIAmDependingOn[i], new ArrayList());
					((ArrayList)table[keysIAmDependingOn[i]]).Add(key);
				}
				this.Internal.AddDependencyKeyList(table, operationContext);
			}
			return failure;
		}

		void ICacheEventsListener.OnCacheCleared()
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnCustomEvent(object notifId, object data)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnCustomRemoveCallback(object key, object value, ItemRemoveReason reason)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnCustomUpdateCallback(object key, object value)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnItemAdded(object key)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnItemRemoved(object key, object val, ItemRemoveReason reason, OperationContext operationContext)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnItemsRemoved(object[] keys, object[] vals, ItemRemoveReason reason, OperationContext operationContext)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnItemUpdated(object key, OperationContext operationContext)
		{
			throw new NotImplementedException();
		}

		void ICacheEventsListener.OnWriteBehindOperationCompletedCallback(OpCode operationCode, object result, CallbackEntry cbEntry)
		{
			throw new NotImplementedException();
		}

		public override void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, OperationContext operationContext)
		{
			this.Internal.Clear(cbEntry, updateOptions, operationContext);
		}

		public override void CloseStream(string key, string lockHandle, OperationContext operationContext)
		{
			this.Internal.CloseStream(key, lockHandle, operationContext);
		}

		public override bool Contains(object key, OperationContext operationContext)
		{
			return this.Internal.Contains(key, operationContext);
		}

		public override Hashtable Contains(object[] keys, OperationContext operationContext)
		{
			return this.Internal.Contains(keys, operationContext);
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
			Hashtable hashtable = this.Internal.Get(keys, operationContext);
			if((hashtable != null) && base.KeepDeflattedValues)
			{
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CacheEntry entry = enumerator.Value as CacheEntry;
					if(entry != null)
					{
						entry.KeepDeflattedValue(base._context.SerializationContext);
					}
				}
			}
			return hashtable;
		}

		public override CacheEntry Get(object key, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = this.Internal.Get(key, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
			if((entry != null) && base.KeepDeflattedValues)
			{
				entry.KeepDeflattedValue(base._context.SerializationContext);
			}
			return entry;
		}

		public override IDictionaryEnumerator GetEnumerator()
		{
			return this.Internal.GetEnumerator();
		}

		public override CacheEntry GetGroup(object key, string group, string subGroup, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = this.Internal.GetGroup(key, group, subGroup, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
			if((entry != null) && base.KeepDeflattedValues)
			{
				entry.KeepDeflattedValue(base._context.SerializationContext);
			}
			return entry;
		}

		public override Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			Hashtable hashtable = this.Internal.GetGroupData(group, subGroup, operationContext);
			if((hashtable != null) && base.KeepDeflattedValues)
			{
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CacheEntry entry = enumerator.Value as CacheEntry;
					if(entry != null)
					{
						entry.KeepDeflattedValue(base._context.SerializationContext);
					}
				}
			}
			return hashtable;
		}

		public override GroupInfo GetGroupInfo(object key, OperationContext operationContext)
		{
			return this.Internal.GetGroupInfo(key, operationContext);
		}

		public override Hashtable GetGroupInfoBulk(object[] keys, OperationContext operationContext)
		{
			return this.Internal.GetGroupInfoBulk(keys, operationContext);
		}

		public override ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			return this.Internal.GetGroupKeys(group, subGroup, operationContext);
		}

		internal override ArrayList GetKeysByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			return this.Internal.GetKeysByTag(tags, comparisonType, operationContext);
		}

		public override EnumerationDataChunk GetNextChunk(EnumerationPointer pointer, OperationContext operationContext)
		{
			return this.Internal.GetNextChunk(pointer, operationContext);
		}

		public override long GetStreamLength(string key, string lockHandle, OperationContext operationContext)
		{
			return this.Internal.GetStreamLength(key, lockHandle, operationContext);
		}

		public override Hashtable GetTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			return this.Internal.GetTag(tags, comparisonType, operationContext);
		}

		public override Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext)
		{
			Hashtable hashtable = null;
			ArrayList list = new ArrayList();
			ArrayList list2 = new ArrayList();
			ArrayList list3 = new ArrayList();
			if(this.Internal != null)
			{
				for(int i = 0; i < cacheEntries.Length; i++)
				{
					object[] keysIAmDependingOn = cacheEntries[i].KeysIAmDependingOn;
					if(keysIAmDependingOn != null)
					{
						Hashtable hashtable2 = this.Contains(keysIAmDependingOn, operationContext);
						if((hashtable2.ContainsKey("items-found") && (hashtable2["items-found"] != null)) && (keysIAmDependingOn.Length == ((ArrayList)hashtable2["items-found"]).Count))
						{
							list.Add(keys[i]);
							list2.Add(cacheEntries[i]);
						}
						else
						{
							list3.Add(keys[i]);
						}
					}
					else
					{
						list.Add(keys[i]);
						list2.Add(cacheEntries[i]);
					}
				}
				CacheEntry[] array = new CacheEntry[list2.Count];
				list2.CopyTo(array);
				hashtable = this.Internal.Insert(list.ToArray(), array, notify, operationContext);
				for(int j = 0; j < list.Count; j++)
				{
					CacheInsResultWithEntry entry = hashtable[list[j]] as CacheInsResultWithEntry;
					if((entry != null) && ((entry.Result == CacheInsResult.Success) || (entry.Result == CacheInsResult.SuccessOverwrite)))
					{
						Hashtable finalKeysList = null;
						if((entry.Entry != null) && (entry.Entry.KeysIAmDependingOn != null))
						{
							finalKeysList = base.GetFinalKeysList(entry.Entry.KeysIAmDependingOn, array[j].KeysIAmDependingOn);
							object[] objArray2 = (object[])finalKeysList["oldKeys"];
							Hashtable table = new Hashtable();
							for(int m = 0; m < objArray2.Length; m++)
							{
								if(!table.Contains(objArray2[m]))
								{
									table.Add(objArray2[m], new ArrayList());
								}
								((ArrayList)table[objArray2[m]]).Add(list[j]);
							}
							this.Internal.RemoveDepKeyList(table, operationContext);
							object[] objArray3 = (object[])finalKeysList["newKeys"];
							table.Clear();
							for(int n = 0; n < objArray3.Length; n++)
							{
								if(!table.Contains(objArray3[n]))
								{
									table.Add(objArray3[n], new ArrayList());
								}
								((ArrayList)table[objArray3[n]]).Add(list[j]);
							}
							this.Internal.AddDependencyKeyList(table, operationContext);
						}
						else if(array[j].KeysIAmDependingOn != null)
						{
							object[] objArray4 = array[j].KeysIAmDependingOn;
							Hashtable hashtable5 = new Hashtable();
							for(int num5 = 0; num5 < objArray4.Length; num5++)
							{
								if(!hashtable5.Contains(objArray4[num5]))
								{
									hashtable5.Add(objArray4[num5], new ArrayList());
								}
								((ArrayList)hashtable5[objArray4[num5]]).Add(list[j]);
							}
							this.Internal.AddDependencyKeyList(hashtable5, operationContext);
						}
					}
				}
				for(int k = 0; k < list3.Count; k++)
				{
					hashtable.Add(list3[k], new OperationFailedException("One of the dependency keys does not exist."));
				}
			}
			return hashtable;
		}

		public override CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			CacheInsResultWithEntry entry = new CacheInsResultWithEntry();
			if(this.Internal != null)
			{
				object[] keysIAmDependingOn = cacheEntry.KeysIAmDependingOn;
				if(keysIAmDependingOn != null)
				{
					Hashtable hashtable = this.Contains(keysIAmDependingOn, operationContext);
					if(!hashtable.ContainsKey("items-found"))
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if(hashtable["items-found"] == null)
						throw new OperationFailedException("One of the dependency keys does not exist.");
					if(keysIAmDependingOn.Length != ((ArrayList)hashtable["items-found"]).Count)
						throw new OperationFailedException("One of the dependency keys does not exist.");
				}
				entry = this.Internal.Insert(key, cacheEntry, notify, lockId, version, accessType, operationContext);
				if((entry.Result != CacheInsResult.Success) && (entry.Result != CacheInsResult.SuccessOverwrite))
				{
					return entry;
				}
				Hashtable finalKeysList = null;
				if((entry.Entry != null) && (entry.Entry.KeysIAmDependingOn != null))
				{
					Hashtable table = null;
					finalKeysList = base.GetFinalKeysList(entry.Entry.KeysIAmDependingOn, cacheEntry.KeysIAmDependingOn);
					object[] objArray2 = (object[])finalKeysList["oldKeys"];
					if(objArray2 != null)
					{
						table = new Hashtable();
						for(int i = 0; i < objArray2.Length; i++)
						{
							if(!table.Contains(objArray2[i]))
							{
								table.Add(objArray2[i], new ArrayList());
							}
							((ArrayList)table[objArray2[i]]).Add(key);
						}
						this.Internal.RemoveDepKeyList(table, operationContext);
					}
					object[] objArray3 = (object[])finalKeysList["newKeys"];
					if(objArray3 != null)
					{
						table = new Hashtable();
						for(int j = 0; j < objArray3.Length; j++)
						{
							if(!table.Contains(objArray3[j]))
							{
								table.Add(objArray3[j], new ArrayList());
							}
							((ArrayList)table[objArray3[j]]).Add(key);
						}
						this.Internal.AddDependencyKeyList(table, operationContext);
					}
					return entry;
				}
				if(cacheEntry.KeysIAmDependingOn != null)
				{
					object[] objArray4 = cacheEntry.KeysIAmDependingOn;
					Hashtable hashtable4 = new Hashtable();
					for(int k = 0; k < objArray4.Length; k++)
					{
						if(!hashtable4.Contains(objArray4[k]))
							hashtable4.Add(objArray4[k], new ArrayList());
						((ArrayList)hashtable4[objArray4[k]]).Add(key);
					}
					this.Internal.AddDependencyKeyList(hashtable4, operationContext);
				}
			}
			return entry;
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
			return this.Internal.IsLocked(key, ref lockId, ref lockDate, operationContext);
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
			return this.Internal.Lock(key, lockExpiration, ref lockId, ref lockDate, operationContext);
		}

		public override bool OpenStream(string key, string lockHandle, StreamModes mode, string group, string subGroup, ExpirationHint hint, EvictionHint evictinHint, OperationContext operationContext)
		{
			object[] keyDependencyTable = CacheHelper.GetKeyDependencyTable(hint);
			if((keyDependencyTable != null) && (mode == StreamModes.Write))
			{
				Hashtable hashtable = this.Contains(keyDependencyTable, operationContext);
				if(!hashtable.ContainsKey("items-found"))
				{
					throw new OperationFailedException("One of the dependency keys does not exist.");
				}
				if(hashtable["items-found"] == null)
				{
					throw new OperationFailedException("One of the dependency keys does not exist.");
				}
				if(((ArrayList)hashtable["items-found"]).Count != keyDependencyTable.Length)
				{
					throw new OperationFailedException("One of the dependency keys does not exist.");
				}
			}
			bool flag = this.Internal.OpenStream(key, lockHandle, mode, group, subGroup, hint, evictinHint, operationContext);
			if((flag && (mode == StreamModes.Write)) && (keyDependencyTable != null))
			{
				Hashtable table = new Hashtable();
				for(int i = 0; i < keyDependencyTable.Length; i++)
				{
					if(table[keyDependencyTable[i]] == null)
					{
						table.Add(keyDependencyTable[i], new ArrayList());
					}
					((ArrayList)table[keyDependencyTable[i]]).Add(key);
				}
				this.Internal.AddDependencyKeyList(table, operationContext);
			}
			return flag;
		}

		public override int ReadFromStream(ref VirtualArray vBuffer, string key, string lockHandle, int offset, int length, OperationContext operationContext)
		{
			return this.Internal.ReadFromStream(ref vBuffer, key, lockHandle, offset, length, operationContext);
		}

		public override void RegisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			this.Internal.RegisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
		}

		public override void RegisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			this.Internal.RegisterKeyNotification(key, updateCallback, removeCallback, operationContext);
		}

		public override Hashtable Remove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			ArrayList col = this.GetGroupKeys(group, subGroup, operationContext);
			if((col != null) && (col.Count > 0))
			{
				object[] arrayFromCollection = MiscUtil.GetArrayFromCollection(col);
				return this.Remove(arrayFromCollection, ItemRemoveReason.Removed, notify, operationContext);
			}
			return null;
		}

		public override Hashtable Remove(object[] keys, ItemRemoveReason ir, bool notify, OperationContext operationContext)
		{
			Hashtable hashtable = this.Internal.Remove(keys, ir, notify, operationContext);
			for(int i = 0; i < keys.Length; i++)
			{
				CacheEntry entry = (CacheEntry)hashtable[keys[i]];
				if((entry != null) && (entry.KeysIAmDependingOn != null))
				{
					this.Internal.RemoveDepKeyList(base.GetKeysTable(keys[i], entry.KeysIAmDependingOn), operationContext);
				}
			}
			return hashtable;
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada que será removida.</param>
		/// <param name="ir"></param>
		/// <param name="notify"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override CacheEntry Remove(object key, ItemRemoveReason ir, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext)
		{
			CacheEntry entry = this.Internal.Remove(key, ir, notify, lockId, version, accessType, operationContext);
			if((entry != null) && (entry.KeysIAmDependingOn != null))
			{
				this.Internal.RemoveDepKeyList(base.GetKeysTable(key, entry.KeysIAmDependingOn), operationContext);
			}
			return entry;
		}

		public override Hashtable RemoveByTag(string[] tags, TagComparisonType tagComparisonType, bool notify, OperationContext operationContext)
		{
			return this.Internal.RemoveByTag(tags, tagComparisonType, notify, operationContext);
		}

		public override object RemoveSync(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			ArrayList list = new ArrayList();
			try
			{
				Hashtable hashtable = new Hashtable();
				CacheEntry entry = null;
				IDictionaryEnumerator enumerator = null;
				for(int i = 0; i < keys.Length; i++)
				{
					try
					{
						if(keys[i] != null)
						{
							entry = this.Internal.Remove(keys[i], reason, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
						}
						if(entry != null)
						{
							hashtable.Add(keys[i], entry);
							if((entry.KeysDependingOnMe != null) && (entry.KeysDependingOnMe.Count > 0))
							{
								list.AddRange(entry.KeysDependingOnMe.Keys);
							}
						}
					}
					catch(Exception)
					{
					}
				}
				enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					try
					{
						entry = enumerator.Value as CacheEntry;
						if(entry != null)
						{
							if(base.IsItemRemoveNotifier)
							{
								this.NotifyItemRemoved(enumerator.Key, entry, reason, true, operationContext);
							}
							if(entry.Value is CallbackEntry)
							{
								this.NotifyCustomRemoveCallback(enumerator.Key, entry.Value, reason, true);
							}
						}
						continue;
					}
					catch(Exception)
					{
						continue;
					}
				}
			}
			catch(Exception)
			{
				throw;
			}
			return list;
		}

		/// <summary>
		/// Realiza a pesquisa.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override QueryResultSet Search(string query, IDictionary values, OperationContext operationContext)
		{
			return this.Internal.Search(query, values, operationContext);
		}

		public override QueryResultSet SearchEntries(string query, IDictionary values, OperationContext operationContext)
		{
			QueryResultSet set = this.Internal.SearchEntries(query, values, operationContext);
			if((set.SearchEntriesResult != null) && base.KeepDeflattedValues)
			{
				IDictionaryEnumerator enumerator = set.SearchEntriesResult.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CacheEntry entry = enumerator.Value as CacheEntry;
					if(entry != null)
					{
						entry.KeepDeflattedValue(base._context.SerializationContext);
					}
				}
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
		/// <param name="comparisonType">Tipo de comparação que será utilizado.</param>
		/// <returns></returns>
		public override IEnumerable<object[]> JoinIndex(string leftTypeName, string leftFieldName, string rightTypeName, string rightFieldName, ComparisonType comparisonType)
		{
			return this.Internal.JoinIndex(leftTypeName, leftFieldName, rightTypeName, rightFieldName, comparisonType);
		}

		public override void SendNotification(object notifId, object data)
		{
			this.Internal.SendNotification(notifId, data);
		}

		public override void UnLock(object key, object lockId, bool isPreemptive, OperationContext operationContext)
		{
			this.Internal.UnLock(key, lockId, isPreemptive, operationContext);
		}

		public override void UnregisterKeyNotification(string key, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			this.Internal.UnregisterKeyNotification(key, updateCallback, removeCallback, operationContext);
		}

		public override void UnregisterKeyNotification(string[] keys, CallbackInfo updateCallback, CallbackInfo removeCallback, OperationContext operationContext)
		{
			this.Internal.UnregisterKeyNotification(keys, updateCallback, removeCallback, operationContext);
		}

		public override void WriteToStream(string key, string lockHandle, VirtualArray vBuffer, int srcOffset, int dstOffset, int length, OperationContext operationContext)
		{
			this.Internal.WriteToStream(key, lockHandle, vBuffer, srcOffset, dstOffset, length, operationContext);
		}

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
				return this.Internal.DataGroupList;
			}
		}

		public CacheBase Internal
		{
			get
			{
				return _cache;
			}
			set
			{
				_cache = value;
				base._context = value.InternalCache.Context;
			}
		}

		protected internal override CacheBase InternalCache
		{
			get
			{
				return _cache;
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

		public override TypeInfoMap TypeInfoMap
		{
			get
			{
				return _cache.TypeInfoMap;
			}
		}
	}
}
