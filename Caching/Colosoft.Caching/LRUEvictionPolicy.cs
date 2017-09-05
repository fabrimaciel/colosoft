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
using System.Threading;

namespace Colosoft.Caching.Policies
{
	/// <summary>
	/// Implementação da política de liberação LRU (Least Recently Used) (item menos utilizado recentemente)
	/// </summary>
	internal class LRUEvictionPolicy : IEvictionPolicy
	{
		private EvictionIndex _index;

		private DateTime _initTime;

		private float _ratio;

		private int _removeThreshhold;

		private int _sleepInterval;

		private long totalEvicted;

		/// <summary>
		/// Recupera a taxa de liberação.
		/// </summary>
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

		/// <summary>
		/// Construtro padrão.
		/// </summary>
		internal LRUEvictionPolicy()
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			this.Initialize();
		}

		/// <summary>
		/// Cria a instancia já definindo os valore iniciais.
		/// </summary>
		/// <param name="properties">Propriedades de configuração.</param>
		/// <param name="ratio">Taxa de liberação.</param>
		public LRUEvictionPolicy(IDictionary properties, float ratio)
		{
			_ratio = 0.25f;
			_removeThreshhold = 10;
			_ratio = ratio / 100f;
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			this.Initialize();
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		private void Initialize()
		{
			_index = new EvictionIndex();
			_initTime = DateTime.UtcNow;
		}

		/// <summary>
		/// Recupera a chave o indice.
		/// </summary>
		/// <param name="diffTime"></param>
		/// <returns></returns>
		private long GetIndexKey(TimeSpan diffTime)
		{
			return (long)diffTime.TotalSeconds;
		}

		/// <summary>
		/// Recupera as chaves selecionadas.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="evictSize"></param>
		/// <returns></returns>
		private ArrayList GetSelectedKeys(CacheBase cache, long evictSize)
		{
			return _index.GetSelectedKeys(cache, evictSize);
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		void IEvictionPolicy.Clear()
		{
			lock (_index.SyncRoot)
				_index.Clear();
		}

		/// <summary>
		/// Executa a política de liberação.
		/// </summary>
		/// <param name="cache">Instancia do cache onde a execução será realizada.</param>
		/// <param name="context">Contexto de execução.</param>
		/// <param name="evictSize">Tamanho da liberação.</param>
		void IEvictionPolicy.Execute(CacheBase cache, CacheRuntimeContext context, long evictSize)
		{
			ILogger Logger = cache.Context.Logger;
			if(Logger.IsInfoEnabled)
				Logger.Info(("Cache Size: {0}" + cache.Count.ToString()).GetFormatter());
			_sleepInterval = ServiceConfiguration.EvictionBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.EvictionBulkRemoveSize;
			DateTime now = DateTime.Now;
			ArrayList selectedKeys = this.GetSelectedKeys(cache, (long)Math.Ceiling((double)(evictSize * _ratio)));
			DateTime time2 = DateTime.Now;
			if(Logger.IsInfoEnabled)
				Logger.Info(string.Format("Time Span for {0} Items: " + ((TimeSpan)(time2 - now)), selectedKeys.Count).GetFormatter());
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
						Logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_AnErrorOccuredWhileRemovingItems), exception);
					}
					list2.Clear();
					if(c != null && c.Count > 0)
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
					Logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_AnErrorOccuredWhileRemovingItems), exception2);
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
									Logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_AnErrorOccuredWhileRemovingItems), exception3);
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
							Logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_AnErrorOccuredWhileRemovingItems), exception4);
						}
						list5.Clear();
					}
				}
			}
		}

		/// <summary>
		/// Realiza a notificação de alteração do <see cref="EvictionHint"/> do item associado
		/// com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="oldhint">Antigo <see cref="EvictionHint"/>.</param>
		/// <param name="newHint">Novo <see cref="EvictionHint"/>.</param>
		void IEvictionPolicy.Notify(object key, EvictionHint oldhint, EvictionHint newHint)
		{
			lock (_index.SyncRoot)
			{
				var hint = (oldhint == null) ? newHint : oldhint;
				if(((_index != null) && (key != null)) && (hint != null))
				{
					TimeSpan diffTime = ((TimestampHint)hint).TimeStamp.Subtract(_initTime);
					long indexKey = this.GetIndexKey(diffTime);
					if(_index.Contains(indexKey, key))
					{
						_index.Remove(indexKey, key);
						hint = (newHint == null) ? oldhint : newHint;
						hint.Update();
						diffTime = ((TimestampHint)hint).TimeStamp.Subtract(_initTime);
						indexKey = this.GetIndexKey(diffTime);
						_index.Add(indexKey, key);
					}
					else
					{
						_index.Add(indexKey, key);
					}
				}
			}
		}

		/// <summary>
		/// Remove o <see cref="EvictionHint"/> do item associado
		/// com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="hint"><see cref="EvictionHint"/> que será removido.</param>
		void IEvictionPolicy.Remove(object key, EvictionHint hint)
		{
			if(hint != null)
			{
				lock (_index.SyncRoot)
				{
					TimeSpan diffTime = ((TimestampHint)hint).TimeStamp.Subtract(_initTime);
					long indexKey = this.GetIndexKey(diffTime);
					_index.Remove(indexKey, key);
				}
			}
		}

		/// <summary>
		/// Recupera um <see cref="EvictionHint"/> compatível com o informado.
		/// </summary>
		/// <param name="evictionHint"></param>
		/// <returns>Instancia compatível.</returns>
		public EvictionHint CompatibleHint(EvictionHint evictionHint)
		{
			if((evictionHint != null) && (evictionHint is TimestampHint))
				return evictionHint;
			return new TimestampHint();
		}

		/// <summary>
		/// Comparador para o Timestamp.
		/// </summary>
		internal class TimestampComparer : IComparer
		{
			private Hashtable _unsortedList;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="unsortedList"></param>
			public TimestampComparer(Hashtable unsortedList)
			{
				_unsortedList = unsortedList;
			}

			/// <summary>
			/// Compara as duas insntacias informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			int IComparer.Compare(object x, object y)
			{
				var obj2 = _unsortedList[x];
				var obj3 = _unsortedList[y];
				DateTime time = (DateTime)obj2;
				return time.CompareTo((DateTime)obj3);
			}
		}
	}
}
