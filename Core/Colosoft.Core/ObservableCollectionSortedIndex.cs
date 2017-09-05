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

namespace Colosoft.Collections
{
	/// <summary>
	/// Represente o indice de uma coleção observada.
	/// </summary>
	public class ObservableCollectionSortedIndex<T> : ObservableCollectionIndex<T>
	{
		/// <summary>
		/// Relação dos itens do indice.
		/// </summary>
		private SortedList2 _items;

		/// <summary>
		/// Recupera o itens pelo indice informado.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override IEnumerable<T> this[object key]
		{
			get
			{
				int[] indexes = null;
				lock (SyncRoot)
				{
					if(!_items.ContainsKey(key))
						return new T[0];
					indexes = _items[key].ToArray();
				}
				var result = new T[indexes.Length];
				for(var i = 0; i < indexes.Length; i++)
					result[i] = Collection[indexes[i]];
				return result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do indice.</param>
		/// <param name="collection">Coleção que será observada.</param>
		/// <param name="watchProperties">Relação das propriedades assistidas.</param>
		/// <param name="keyGetter">Ponteiro do método usado para recupera o valor da chave do item.</param>
		/// <param name="comparer">Comparador que será utilizado.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ObservableCollectionSortedIndex(string name, IObservableCollection<T> collection, string[] watchProperties, Func<T, object> keyGetter, System.Collections.IComparer comparer) : base(name, collection, watchProperties, keyGetter, comparer)
		{
			_items = new SortedList2(new SortedList2ItemComparer(comparer));
			Initialize();
		}

		/// <summary>
		/// Reseta o indice.
		/// </summary>
		public override void Reset()
		{
			lock (SyncRoot)
			{
				_items.Clear();
				Initialize();
			}
		}

		/// <summary>
		/// Método usado para tratar os novos itens adicionados.
		/// </summary>
		/// <param name="item">Item adicionado</param>
		/// <param name="index">Indice do item na coleção de origem.</param>
		protected override void OnAdded(T item, int index)
		{
			lock (SyncRoot)
			{
				var key = KeyGetter(item);
				if(!_items.ContainsKey(key))
				{
					var values = new List<int>();
					values.Add(index);
					_items[key] = values;
				}
				else
				{
					var values = _items[key] as List<int>;
					var i = values.BinarySearch(index);
					if(i < 0)
						values.Insert(~i, index);
				}
			}
		}

		/// <summary>
		/// Método acionado quando o item for removido.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="index"></param>
		protected override void OnRemoved(T item, int index)
		{
			lock (SyncRoot)
			{
				if(_items.ContainsKey(item))
				{
					var values = _items[item] as List<int>;
					var i = values.BinarySearch(index);
					if(i >= 0)
						values.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Representa um item da lista ordenada.
		/// </summary>
		class SortedList2Item
		{
			public object Key;

			public List<int> Indexes;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="key"></param>
			/// <param name="indexes"></param>
			public SortedList2Item(object key, List<int> indexes)
			{
				Key = key;
				Indexes = indexes;
			}
		}

		/// <summary>
		/// Implementação do comparador do item da lista ordenada.
		/// </summary>
		class SortedList2ItemComparer : IComparer<SortedList2Item>
		{
			private System.Collections.IComparer _keyComparer;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="comparer"></param>
			public SortedList2ItemComparer(System.Collections.IComparer comparer)
			{
				_keyComparer = comparer;
			}

			/// <summary>
			/// Compara os itens informados.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(SortedList2Item x, SortedList2Item y)
			{
				return _keyComparer.Compare(x.Key, y.Key);
			}
		}

		/// <summary>
		/// Implementação de uma lista ordenada customizada para o indice.
		/// </summary>
		class SortedList2
		{
			private SortedList2ItemComparer _comparer;

			private List<SortedList2Item> _items = new List<SortedList2Item>();

			/// <summary>
			/// Recupera as posição pela chave informada.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public List<int> this[object key]
			{
				get
				{
					var item = new SortedList2Item(key, null);
					var index = _items.BinarySearch(item, _comparer);
					if(index >= 0)
						return _items[index].Indexes;
					throw new IndexNotFoundException((key ?? "").ToString());
				}
				set
				{
					var item = new SortedList2Item(key, null);
					var index = _items.BinarySearch(item, _comparer);
					if(index >= 0)
						_items[index].Indexes = value;
					else
						_items.Add(new SortedList2Item(key, value));
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="comparer"></param>
			public SortedList2(SortedList2ItemComparer comparer)
			{
				_comparer = comparer;
			}

			/// <summary>
			/// Verifica se existe a chave informada.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public bool ContainsKey(object key)
			{
				var item = new SortedList2Item(key, null);
				var index = _items.BinarySearch(item, _comparer);
				return index >= 0;
			}

			/// <summary>
			/// Limpa a coleção.
			/// </summary>
			public void Clear()
			{
				_items.Clear();
			}
		}
	}
}
