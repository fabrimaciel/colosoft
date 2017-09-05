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
	/// <summary>
	/// Representa um indice de liberação.
	/// </summary>
	internal class EvictionIndex
	{
		private long _head = -1;

		private Dictionary<long, EvictionIndexEntry> _index = new Dictionary<long, EvictionIndexEntry>();

		private object _syncLock = new object();

		private long _tail = -1;

		/// <summary>
		/// Instancia de sincronização.
		/// </summary>
		internal object SyncRoot
		{
			get
			{
				return _syncLock;
			}
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave da entrada que será adicionada..</param>
		/// <param name="value">Valor do item.</param>
		internal void Add(long key, object value)
		{
			if(_index.Count == 0)
				_head = key;
			if(_index.ContainsKey(key))
			{
				var entry = (EvictionIndexEntry)_index[key];
				if(entry != null)
					entry.Insert(value);
			}
			else
			{
				var entry2 = new EvictionIndexEntry();
				entry2.Insert(value);
				_index[key] = entry2;
				var entry3 = _index[_tail] as EvictionIndexEntry;
				if(entry3 != null)
					entry3.Next = key;
				entry2.Previous = _tail;
			}
			if(key > _tail)
				_tail = key;
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		internal void Clear()
		{
			_head = _tail = -1;
			_index.Clear();
		}

		/// <summary>
		/// Verifica se existe, algum item com a chave e o valor informado.
		/// </summary>
		/// <param name="key">Chave que será verificada.</param>
		/// <param name="value">Valor que será verificado.</param>
		/// <returns></returns>
		internal bool Contains(long key, object value)
		{
			if(_index.ContainsKey(key))
			{
				EvictionIndexEntry entry = _index[key] as EvictionIndexEntry;
				return entry.Contains(value);
			}
			return false;
		}

		/// <summary>
		/// Recupera as chaves selecionadas.
		/// </summary>
		/// <param name="cache">Instancia do cache de onde os itens estão inseridos.</param>
		/// <param name="evictSize"></param>
		/// <returns></returns>
		internal ArrayList GetSelectedKeys(CacheBase cache, long evictSize)
		{
			EvictionIndexEntry entry = null;
			ArrayList list = new ArrayList();
			int num = 0;
			bool flag = false;
			long next = _head;
			if(_head != -1)
			{
				do
				{
					entry = _index[next] as EvictionIndexEntry;
					foreach (string str in entry.GetAllKeys())
					{
						int itemSize = cache.GetItemSize(str);
						if(((num + itemSize) >= evictSize) && (num > 0))
						{
							if((evictSize - num) > ((itemSize + num) - evictSize))
								list.Add(str);
							flag = true;
							break;
						}
						list.Add(str);
						num += itemSize;
					}
					next = entry.Next;
				}
				while (!flag && (next != -1));
			}
			return list;
		}

		/// <summary>
		/// Insere uma nova entrada para o indice.
		/// </summary>
		/// <param name="key">Chave que identifica a entrada.</param>
		/// <param name="value">Valor da entrada..</param>
		internal void Insert(long key, object value)
		{
			this.Insert(key, value, -1, _head);
		}

		/// <summary>
		/// Insere um nova entrada para o indice.
		/// </summary>
		/// <param name="key">Chave que identifica a entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="previous">Identificador da entrada anterior.</param>
		/// <param name="next">Identificador da próxima entrada.</param>
		internal void Insert(long key, object value, long previous, long next)
		{
			EvictionIndexEntry entry = _index[next] as EvictionIndexEntry;
			EvictionIndexEntry entry2 = _index[previous] as EvictionIndexEntry;
			if((_index.Count == 0) || (key < _head))
				_head = key;
			if(_index.ContainsKey(key))
			{
				EvictionIndexEntry entry3 = (EvictionIndexEntry)_index[key];
				if(entry3 != null)
					entry3.Insert(value);
			}
			else
			{
				EvictionIndexEntry entry4 = new EvictionIndexEntry();
				entry4.Insert(value);
				_index[key] = entry4;
				if((entry2 == null) && (entry == null))
				{
					entry4.Next = -1;
					entry4.Previous = -1;
				}
				else if((entry2 == null) && (entry != null))
				{
					entry4.Next = next;
					entry4.Previous = -1;
					entry.Previous = key;
				}
				else if((entry2 != null) && (entry == null))
				{
					entry4.Previous = previous;
					entry4.Next = -1;
					entry2.Next = key;
				}
				else
				{
					entry4.Previous = previous;
					entry4.Next = next;
					entry2.Next = key;
					entry.Previous = key;
				}
			}
			if(key > _tail)
				_tail = key;
		}

		/// <summary>
		/// Remove a entrada com a chave e valor informados.
		/// </summary>
		/// <param name="key">Chave que identifica a entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		internal void Remove(long key, object value)
		{
			EvictionIndexEntry entry = null;
			EvictionIndexEntry entry2 = null;
			if(_index.ContainsKey(key))
			{
				EvictionIndexEntry entry3 = (EvictionIndexEntry)_index[key];
				if(entry3.Remove(value))
				{
					if(entry3.Previous != -1)
						entry = (EvictionIndexEntry)_index[entry3.Previous];
					if(entry3.Next != -1)
						entry2 = (EvictionIndexEntry)_index[entry3.Next];
					if((entry != null) && (entry2 != null))
					{
						entry.Next = entry3.Next;
						entry2.Previous = entry3.Previous;
					}
					else if(entry != null)
					{
						entry.Next = entry3.Next;
						_tail = entry3.Previous;
					}
					else if(entry2 != null)
					{
						entry2.Previous = entry3.Previous;
						_head = entry3.Next;
					}
					else
						_tail = _head = -1;
					_index.Remove(key);
				}
			}
		}

		/// <summary>
		/// Remove a entrada com a chave e valor informados.
		/// </summary>
		/// <param name="key">Chave que identifica a entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="previous">Identificador da entrada anterior.</param>
		/// <param name="next">Identificador da próxima entrada.</param>
		internal void Remove(long key, object value, ref long previous, ref long next)
		{
			EvictionIndexEntry entry = null;
			EvictionIndexEntry entry2 = null;
			previous = key;
			if(_index.ContainsKey(key))
			{
				EvictionIndexEntry entry3 = (EvictionIndexEntry)_index[key];
				if(entry3.Previous != -1)
					entry = (EvictionIndexEntry)_index[entry3.Previous];
				if(entry3.Next != -1)
					entry2 = (EvictionIndexEntry)_index[entry3.Next];
				next = entry3.Next;
				if(entry3.Remove(value))
				{
					previous = entry3.Previous;
					if((entry != null) && (entry2 != null))
					{
						entry.Next = entry3.Next;
						entry2.Previous = entry3.Previous;
					}
					else if(entry != null)
					{
						entry.Next = entry3.Next;
						_tail = entry3.Previous;
					}
					else if(entry2 != null)
					{
						entry2.Previous = entry3.Previous;
						_head = entry3.Next;
					}
					else
						_tail = _head = -1;
					_index.Remove(key);
				}
			}
		}
	}
}
