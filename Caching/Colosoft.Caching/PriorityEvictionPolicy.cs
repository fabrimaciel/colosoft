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

namespace Colosoft.Caching.Policies
{
	internal class PriorityEvictionPolicy : IEvictionPolicy
	{
		private Hashtable[] _index;

		private CacheItemPriority _priority;

		private float _ratio;

		private int _removeThreshhold;

		private int _sleepInterval;

		internal PriorityEvictionPolicy()
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			this.Initialize();
		}

		public PriorityEvictionPolicy(IDictionary properties, float ratio)
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			if((properties != null) && properties.Contains("default-value"))
			{
				string priority = (string)properties["default-value"];
				_priority = GetPriorityValue(priority);
			}
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			_ratio = ratio / 100f;
			this.Initialize();
		}

		void IEvictionPolicy.Clear()
		{
			lock (_index.SyncRoot)
			{
				if(_index != null)
				{
					for(int i = 0; i < 5; i++)
					{
						if(_index[i] != null)
						{
							_index[i].Clear();
						}
					}
				}
			}
		}

		void IEvictionPolicy.Execute(CacheBase cache, CacheRuntimeContext context, long evictSize)
		{
			Colosoft.Logging.ILogger Logger = cache.Context.Logger;
			if(Logger.IsInfoEnabled)
			{
				Logger.Info(("Cache Size: {0}" + cache.Count.ToString()).GetFormatter());
			}
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			DateTime now = DateTime.Now;
			ArrayList selectedKeys = this.GetSelectedKeys(cache, (long)Math.Ceiling((double)(evictSize * _ratio)));
			DateTime time2 = DateTime.Now;
			if(Logger.IsInfoEnabled)
			{
				Logger.Info(string.Format("Time Span for {0} Items: " + ((TimeSpan)(time2 - now)), selectedKeys.Count).GetFormatter());
			}
			now = DateTime.Now;
			Cache cacheRoot = context.CacheRoot;
			ArrayList list2 = new ArrayList();
			ArrayList list3 = new ArrayList();
			ArrayList c = null;
			IEnumerator enumerator = selectedKeys.GetEnumerator();
			int num = _removeThreshhold / 300;
			int num2 = 0;
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if(current != null)
				{
					list2.Add(current);
					if((list2.Count % 300) == 0)
					{
						try
						{
							c = cache.RemoveSync(list2.ToArray(), ItemRemoveReason.Underused, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
						}
						catch(Exception exception)
						{
							Logger.Error("PriorityEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing items. Error " + exception.ToString()).GetFormatter());
						}
						list2.Clear();
						if((c != null) && (c.Count > 0))
						{
							list3.AddRange(c);
						}
						num2++;
						if(num2 >= num)
						{
							System.Threading.Thread.Sleep((int)(_sleepInterval * 0x3e8));
							num2 = 0;
						}
					}
				}
			}
			if(list2.Count > 0)
			{
				try
				{
					c = cache.RemoveSync(list2.ToArray(), ItemRemoveReason.Underused, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
					if((c != null) && (c.Count > 0))
					{
						list3.AddRange(c);
					}
				}
				catch(Exception exception2)
				{
					Logger.Error("PriorityEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing items. Error " + exception2.ToString()).GetFormatter());
				}
			}
			if(list3.Count > 0)
			{
				ArrayList list5 = new ArrayList();
				if(cacheRoot != null)
				{
					foreach (object obj3 in list3)
					{
						if(obj3 != null)
						{
							list5.Add(obj3);
							if((list5.Count % 100) == 0)
							{
								try
								{
									cacheRoot.CascadedRemove(list5.ToArray(), ItemRemoveReason.Underused, true, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
								}
								catch(Exception exception3)
								{
									Logger.Error("PriorityEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing dependent items. Error " + exception3.ToString()).GetFormatter());
								}
								list5.Clear();
							}
						}
					}
					if(list5.Count > 0)
					{
						try
						{
							cacheRoot.CascadedRemove(list5.ToArray(), ItemRemoveReason.Underused, true, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						}
						catch(Exception exception4)
						{
							Logger.Error("PriorityEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing dependent items. Error " + exception4.ToString()).GetFormatter());
						}
						list5.Clear();
					}
				}
			}
		}

		void IEvictionPolicy.Notify(object key, EvictionHint oldhint, EvictionHint newHint)
		{
			EvictionHint hint = newHint;
			if(hint != null)
			{
				CacheItemPriority priority = ((PriorityEvictionHint)hint).Priority;
				if(priority == CacheItemPriority.Default)
				{
					priority = _priority;
					((PriorityEvictionHint)hint).Priority = _priority;
				}
				lock (_index.SyncRoot)
				{
					switch(priority)
					{
					case CacheItemPriority.Low:
						if(_index[0] == null)
						{
							_index[0] = new Hashtable(0x61a8, 0.7f);
						}
						_index[0][key] = hint;
						return;
					case CacheItemPriority.BelowNormal:
						if(_index[1] == null)
						{
							_index[1] = new Hashtable(0x61a8, 0.7f);
						}
						_index[1][key] = hint;
						return;
					case CacheItemPriority.Normal:
						if(_index[2] == null)
						{
							_index[2] = new Hashtable(0x61a8, 0.7f);
						}
						_index[2][key] = hint;
						return;
					case CacheItemPriority.AboveNormal:
						if(_index[3] == null)
						{
							_index[3] = new Hashtable(0x61a8, 0.7f);
						}
						_index[3][key] = hint;
						return;
					case CacheItemPriority.High:
						break;
					default:
						return;
					}
					if(_index[4] == null)
					{
						_index[4] = new Hashtable(0x61a8, 0.7f);
					}
					_index[4][key] = hint;
				}
			}
		}

		void IEvictionPolicy.Remove(object key, EvictionHint hint)
		{
			lock (_index.SyncRoot)
			{
				if(_index != null)
				{
					for(int i = 0; i < 5; i++)
					{
						if((_index[i] != null) && _index[i].Contains(key))
						{
							_index[i].Remove(key);
							if(_index[i].Count == 0)
							{
								_index[i] = null;
							}
						}
					}
				}
			}
		}

		public EvictionHint CompatibleHint(EvictionHint eh)
		{
			if((eh != null) && (eh is PriorityEvictionHint))
			{
				return eh;
			}
			return new PriorityEvictionHint(_priority);
		}

		private static CacheItemPriority GetPriorityValue(string priority)
		{
			priority = priority.ToLower();
			switch(priority)
			{
			case "notremovable":
				return CacheItemPriority.NotRemovable;
			case "high":
				return CacheItemPriority.High;
			case "above-normal":
				return CacheItemPriority.AboveNormal;
			case "below-normal":
				return CacheItemPriority.BelowNormal;
			case "low":
				return CacheItemPriority.Low;
			}
			return CacheItemPriority.Normal;
		}

		private ArrayList GetSelectedKeys(CacheBase cache, long evictSize)
		{
			ArrayList list = new ArrayList(100);
			long num = 0;
			object key = null;
			bool flag = false;
			lock (_index.SyncRoot)
			{
				for(int i = 0; i < 5; i++)
				{
					if(flag)
					{
						return list;
					}
					Hashtable hashtable = _index[i];
					if(hashtable != null)
					{
						IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
						while (enumerator.MoveNext())
						{
							key = enumerator.Key;
							if(key != null)
							{
								int itemSize = cache.GetItemSize(key);
								if(((num + itemSize) >= evictSize) && (num > 0))
								{
									if((evictSize - num) > ((itemSize + num) - evictSize))
									{
										list.Add(key);
									}
									flag = true;
									break;
								}
								list.Add(key);
								num += itemSize;
							}
						}
					}
				}
			}
			return list;
		}

		private void Initialize()
		{
			_index = new Hashtable[5];
		}

		float IEvictionPolicy.EvictRatio
		{
			get
			{
				return _ratio;
			}
			set
			{
				_ratio = value;
			}
		}
	}
}
