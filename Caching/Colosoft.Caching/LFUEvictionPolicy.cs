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

namespace Colosoft.Caching.Policies
{
	/// <summary>
	/// Implementação da política de liberação LFU (Least-Frequently Used) (item usado com a menor frequencia.)
	/// </summary>
	internal class LFUEvictionPolicy : IEvictionPolicy
	{
		private EvictionIndex _index;

		private float _ratio;

		private int _removeThreshhold;

		private int _sleepInterval;

		private long totalEvicted;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal LFUEvictionPolicy()
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			this.Initialize();
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="properties">Propriedades de configuração.</param>
		/// <param name="ratio"></param>
		public LFUEvictionPolicy(IDictionary properties, float ratio)
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			_ratio = ratio / 100f;
			this.Initialize();
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		void IEvictionPolicy.Clear()
		{
			lock (_index.SyncRoot)
				_index.Clear();
		}

		void IEvictionPolicy.Execute(CacheBase cache, CacheRuntimeContext context, long evictSize)
		{
			if(context.Logger.IsInfoEnabled)
				context.Logger.Info(("Cache Size: {0}" + cache.Count.ToString()).GetFormatter());
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			DateTime now = DateTime.Now;
			ArrayList selectedKeys = this.GetSelectedKeys(cache, (long)Math.Ceiling((double)(evictSize * _ratio)));
			DateTime time2 = DateTime.Now;
			if(context.Logger.IsInfoEnabled)
				context.Logger.Info(string.Format("Time Span for {0} Items: " + ((TimeSpan)(time2 - now)), selectedKeys.Count).GetFormatter());
			now = DateTime.Now;
			Cache cacheRoot = context.CacheRoot;
			ArrayList list2 = new ArrayList();
			ArrayList list3 = new ArrayList();
			ArrayList c = null;
			this.totalEvicted += selectedKeys.Count;
			IEnumerator enumerator = selectedKeys.GetEnumerator();
			int num = _removeThreshhold / 300;
			int num2 = 0;
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				list2.Add(current);
				if((list2.Count % 300) == 0)
				{
					try
					{
						c = cache.RemoveSync(list2.ToArray(), ItemRemoveReason.Underused, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
					}
					catch(Exception exception)
					{
						context.Logger.Error("LfuEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing items. Error " + exception.ToString()).GetFormatter());
					}
					list2.Clear();
					if((c != null) && (c.Count > 0))
						list3.AddRange(c);
					num2++;
					if(num2 >= num)
					{
						Thread.Sleep((int)(_sleepInterval * 0x3e8));
						num2 = 0;
					}
				}
			}
			if(list2.Count > 0)
			{
				try
				{
					c = cache.RemoveSync(list2.ToArray(), ItemRemoveReason.Underused, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
					if((c != null) && (c.Count > 0))
						list3.AddRange(c);
				}
				catch(Exception exception2)
				{
					context.Logger.Error("LfuEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing items. Error " + exception2.ToString()).GetFormatter());
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
									context.Logger.Error("LfuEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing dependent items. Error " + exception3.ToString()).GetFormatter());
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
							context.Logger.Error("LfuEvictionPolicy.Execute".GetFormatter(), ("an error occured while removing dependent items. Error " + exception4.ToString()).GetFormatter());
						}
						list5.Clear();
					}
				}
			}
		}

		void IEvictionPolicy.Notify(object key, EvictionHint oldhint, EvictionHint newHint)
		{
			lock (_index.SyncRoot)
			{
				EvictionHint hint = (oldhint == null) ? newHint : oldhint;
				if(((_index != null) && (key != null)) && (hint != null))
				{
					int count = ((CounterHint)hint).Count;
					if(_index.Contains((long)count, key))
					{
						long previous = -1;
						long next = -1;
						_index.Remove((long)count, key, ref previous, ref next);
						hint = (newHint == null) ? oldhint : newHint;
						hint.Update();
						count = ((CounterHint)hint).Count;
						_index.Insert((long)count, key, previous, next);
					}
					else
					{
						_index.Insert((long)count, key);
					}
				}
			}
		}

		void IEvictionPolicy.Remove(object key, EvictionHint hint)
		{
			if(hint != null)
			{
				lock (_index.SyncRoot)
				{
					long count = ((CounterHint)hint).Count;
					_index.Remove(count, key);
				}
			}
		}

		public EvictionHint CompatibleHint(EvictionHint eh)
		{
			if((eh != null) && (eh is CounterHint))
			{
				return eh;
			}
			return new CounterHint();
		}

		private ArrayList GetSelectedKeys(CacheBase cache, long evictSize)
		{
			return _index.GetSelectedKeys(cache, evictSize);
		}

		private void Initialize()
		{
			_index = new EvictionIndex();
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
