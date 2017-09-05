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
using Colosoft.Caching.Exceptions;
using Colosoft.Serialization.Formatters;

namespace Colosoft.Caching.Util
{
	internal class CacheHelper
	{
		public static bool CheckDataGroupsCompatibility(GroupInfo g1, GroupInfo g2)
		{
			bool flag = false;
			if((g1 == null) && (g2 == null))
			{
				return true;
			}
			if((g1 != null) && (g2 != null))
			{
				return ((g1.Group == g2.Group) && (g1.SubGroup == g2.SubGroup));
			}
			if(g1 != null)
			{
				if((g1.Group == null) && (g1.SubGroup == null))
				{
					flag = true;
				}
				return flag;
			}
			if((g2.Group == null) && (g2.SubGroup == null))
			{
				flag = true;
			}
			return flag;
		}

		public static bool CheckLockCompatibility(CacheEntry existingEntry, CacheEntry newEntry)
		{
			object lockId = null;
			DateTime lockDate = new DateTime();
			if(existingEntry.IsLocked(ref lockId, ref lockDate))
			{
				return existingEntry.LockId.Equals(newEntry.LockId);
			}
			return true;
		}

		public static Hashtable CompileInsertResult(Hashtable insertResults)
		{
			Hashtable hashtable = new Hashtable();
			if(insertResults != null)
			{
				IDictionaryEnumerator enumerator = insertResults.GetEnumerator();
				while (enumerator.MoveNext())
				{
					object key = enumerator.Key;
					if(enumerator.Value is CacheInsResultWithEntry)
					{
						switch(((CacheInsResultWithEntry)enumerator.Value).Result)
						{
						case CacheInsResult.NeedsEviction:
							hashtable.Add(key, new OperationFailedException("The cache is full and not enough items could be evicted."));
							break;
						case CacheInsResult.Failure:
							hashtable.Add(key, new OperationFailedException("Generic operation failure; not enough information is available."));
							break;
						}
					}
				}
			}
			return hashtable;
		}

		public static Hashtable GetInsertableItems(Hashtable existingItems, Hashtable newItems)
		{
			Hashtable hashtable = new Hashtable();
			if(existingItems != null)
			{
				IDictionaryEnumerator enumerator = existingItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					object key = enumerator.Key;
					GroupInfo groupInfo = ((CacheEntry)newItems[key]).GroupInfo;
					GroupInfo info2 = enumerator.Value as GroupInfo;
					if(CheckDataGroupsCompatibility(groupInfo, info2))
					{
						hashtable.Add(key, newItems[key]);
					}
				}
			}
			return hashtable;
		}

		public static object[] GetKeyDependencyTable(ExpirationHint hint)
		{
			ArrayList list = null;
			if(hint != null)
			{
				if(hint.HintType == ExpirationHintType.AggregateExpirationHint)
				{
					ExpirationHint[] hints = ((AggregateExpirationHint)hint).Hints;
					for(int i = 0; i < hints.Length; i++)
					{
						if(hints[i].HintType == ExpirationHintType.KeyDependency)
						{
							if(list == null)
							{
								list = new ArrayList();
							}
							string[] cacheKeys = ((KeyDependency)hints[i]).CacheKeys;
							if((cacheKeys != null) && (cacheKeys.Length > 0))
							{
								for(int j = 0; j < cacheKeys.Length; j++)
								{
									if(!list.Contains(cacheKeys[j]))
									{
										list.Add(cacheKeys[j]);
									}
								}
							}
						}
					}
					if((list != null) && (list.Count > 0))
					{
						object[] array = new object[list.Count];
						list.CopyTo(array, 0);
						return array;
					}
				}
				else if(hint.HintType == ExpirationHintType.KeyDependency)
				{
					return ((KeyDependency)hint).CacheKeys;
				}
			}
			return null;
		}

		public static long GetLocalCount(CacheBase cache)
		{
			if((cache != null) && (cache.InternalCache != null))
			{
				return cache.InternalCache.Count;
			}
			return 0;
		}

		public static CacheEntry MergeEntries(CacheEntry c1, CacheEntry c2)
		{
			if((c1 != null) && (c1.Value is CallbackEntry))
			{
				CallbackEntry entry = null;
				entry = c1.Value as CallbackEntry;
				if(entry.ItemRemoveCallbackListener != null)
				{
					foreach (CallbackInfo info in entry.ItemRemoveCallbackListener)
					{
						c2.AddCallbackInfo(null, info);
					}
				}
				if(entry.ItemUpdateCallbackListener != null)
				{
					foreach (CallbackInfo info2 in entry.ItemUpdateCallbackListener)
					{
						c2.AddCallbackInfo(info2, null);
					}
				}
			}
			if(((c1 != null) && (c1.EvictionHint != null)) && (c2.EvictionHint == null))
			{
				c2.EvictionHint = c1.EvictionHint;
			}
			return c2;
		}

		public static bool ReleaseLock(CacheEntry existingEntry, CacheEntry newEntry)
		{
			if(CheckLockCompatibility(existingEntry, newEntry))
			{
				existingEntry.ReleaseLock();
				newEntry.ReleaseLock();
				return true;
			}
			return false;
		}

		public static string[] GetTags(Tag[] tags)
		{
			string[] strArray = new string[tags.Length];
			for(int i = 0; i < tags.Length; i++)
			{
				strArray[i] = tags[i].TagName;
			}
			return strArray;
		}
	}
}
