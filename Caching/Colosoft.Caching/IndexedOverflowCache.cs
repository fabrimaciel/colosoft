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
using Colosoft.Caching.Queries;
using Colosoft.Caching.Queries.Filters;
using Colosoft.Caching.Data;

namespace Colosoft.Caching.Local
{
	internal class IndexedOverflowCache : OverflowCache
	{
		public IndexedOverflowCache(IDictionary cacheClasses, CacheBase parentCache, IDictionary properties, ICacheEventsListener listener, CacheRuntimeContext context, ActiveQueryAnalyzer activeQueryAnalyzer) : base(cacheClasses, parentCache, properties, listener, context, activeQueryAnalyzer)
		{
		}

		protected override LocalCacheBase CreateLocalCache(CacheBase parentCache, IDictionary cacheClasses, IDictionary schemeProps)
		{
			return new IndexedLocalCache(cacheClasses, parentCache, schemeProps, null, base._context, base._activeQueryAnalyzer);
		}

		protected override LocalCacheBase CreateOverflowCache(IDictionary cacheClasses, IDictionary schemeProps)
		{
			return new IndexedOverflowCache(cacheClasses, this, schemeProps, null, base._context, base._activeQueryAnalyzer);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			Hashtable hashtable = base._primary.GetGroupData(group, subGroup, operationContext);
			Hashtable hashtable2 = base._secondary.GetGroupData(group, subGroup, operationContext);
			if(hashtable == null)
			{
				return hashtable2;
			}
			if(hashtable2 != null)
			{
				IDictionaryEnumerator enumerator = hashtable2.GetEnumerator();
				while (enumerator.MoveNext())
				{
					hashtable.Add(enumerator.Key, enumerator.Value);
				}
			}
			return hashtable;
		}

		public override GroupInfo GetGroupInfo(object key, OperationContext operationContext)
		{
			CacheEntry entry = this.Get(key, operationContext);
			GroupInfo info = null;
			if(entry == null)
			{
				return info;
			}
			if(entry.GroupInfo != null)
			{
				return new GroupInfo(entry.GroupInfo.Group, entry.GroupInfo.SubGroup);
			}
			return new GroupInfo(null, null);
		}

		public override Hashtable GetGroupInfoBulk(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = this.Get(keys, operationContext);
			if(hashtable2 != null)
			{
				IDictionaryEnumerator enumerator = hashtable2.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GroupInfo groupInfo = null;
					CacheEntry entry = (CacheEntry)enumerator.Value;
					if(entry != null)
					{
						groupInfo = entry.GroupInfo;
						if(groupInfo == null)
						{
							groupInfo = new GroupInfo(null, null);
						}
					}
					hashtable.Add(enumerator.Key, groupInfo);
				}
			}
			return hashtable;
		}

		public override ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			ArrayList list = base._primary.GetGroupKeys(group, subGroup, operationContext);
			ArrayList c = base._secondary.GetGroupKeys(group, subGroup, operationContext);
			if(list == null)
			{
				return c;
			}
			if(c != null)
			{
				list.AddRange(c);
			}
			return list;
		}

		public override Hashtable Remove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			object[] arrayFromCollection = MiscUtil.GetArrayFromCollection(this.GetGroupKeys(group, subGroup, operationContext));
			return this.Remove(arrayFromCollection, ItemRemoveReason.Removed, notify, operationContext);
		}

		internal override QueryContext SearchInternal(Predicate pred, IDictionary values)
		{
			return null;
		}

		public override ArrayList DataGroupList
		{
			get
			{
				ArrayList list = new ArrayList();
				if(base._primary != null)
				{
					ICollection dataGroupList = base._primary.DataGroupList;
					if(dataGroupList != null)
					{
						list.AddRange(dataGroupList);
					}
				}
				if(base._secondary != null)
				{
					ICollection c = base._secondary.DataGroupList;
					if(c == null)
					{
						return list;
					}
					IEnumerator enumerator = c.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if(!list.Contains(enumerator.Current))
						{
							list.AddRange(c);
						}
					}
				}
				return list;
			}
		}

		public sealed override TypeInfoMap TypeInfoMap
		{
			get
			{
				return base._primary.TypeInfoMap;
			}
		}
	}
}
