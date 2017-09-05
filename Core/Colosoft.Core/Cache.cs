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

namespace Colosoft.Runtime
{
	/// <summary>
	/// Representa um cache para armazenar os itens do gerenciador.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Cache<T> : IDisposable
	{
		private int _maxCount;

		private List<Entry> _entries;

		/// <summary>
		/// Evento acionado quando o cache estiver sendo liberado.
		/// </summary>
		public event EventHandler Disposing;

		/// <summary>
		/// Evento acionado quando a coleção associada for alterada.
		/// </summary>
		public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="maxCount"></param>
		public Cache(int maxCount)
		{
			_maxCount = maxCount;
			_entries = new List<Entry>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~Cache()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o item que está no cache.
		/// </summary>
		/// <param name="predicate">Predicado usado no filtro.</param>
		/// <param name="creator"></param>
		/// <param name="item">Instancia do item encontrado.</param>
		/// <returns></returns>
		public bool Get(Predicate<T> predicate, Func<T> creator, out T item)
		{
			T removedItem = default(T);
			T addedItem = default(T);
			var removed = false;
			var added = false;
			lock (_entries)
			{
				var item1 = _entries.Find(f => predicate(f.Item));
				if(item1 != null)
				{
					foreach (var i in _entries)
						i.Notify(i == item1);
					item = item1.Item;
					return true;
				}
				else if(creator != null)
				{
					item = creator();
					if(_entries.Count > 0 && _entries.Count + 1 > _maxCount)
					{
						var item2 = _entries.OrderBy(f => f.UsageCount).First();
						removedItem = item2.Item;
						removed = true;
						_entries.Remove(item2);
					}
					addedItem = item;
					added = true;
					_entries.Add(new Entry(item));
					return true;
				}
			}
			if(CollectionChanged != null)
			{
				if(removed)
					CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, new System.Collections.ArrayList(), new List<T> {
						removedItem
					}, 0));
				if(added)
					CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, new List<T> {
						addedItem
					}, new System.Collections.ArrayList(), 0));
			}
			item = default(T);
			return false;
		}

		/// <summary>
		/// Reseta o cache.
		/// </summary>
		public void Reset()
		{
			IList<T> removedItems = null;
			lock (_entries)
			{
				removedItems = _entries.Select(f => f.Item).ToList();
				_entries.Clear();
			}
			if(CollectionChanged != null)
				CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, new System.Collections.ArrayList(), removedItems, 0));
		}

		/// <summary>
		/// Reseta os itens que atendam o predicado informado.
		/// </summary>
		/// <param name="predicate"></param>
		public void Reset(Predicate<T> predicate)
		{
			lock (_entries)
			{
				var items = new Queue<Entry>(_entries.Where(f => predicate(f.Item)));
				var removedItems = items.Select(f => f.Item).ToList();
				while (items.Count > 0)
					_entries.Remove(items.Dequeue());
				if(CollectionChanged != null)
					CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, new System.Collections.ArrayList(), removedItems, 0));
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(Disposing != null)
				Disposing(this, EventArgs.Empty);
			var removedItems = _entries.Select(f => f.Item).ToList();
			_entries.Clear();
			if(CollectionChanged != null)
				CollectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, new System.Collections.ArrayList(), removedItems, 0));
			_maxCount = 0;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Representa uma entrada do cache.
		/// </summary>
		class Entry
		{
			private T _item;

			private int _usageCount;

			/// <summary>
			/// Item
			/// </summary>
			public T Item
			{
				get
				{
					return _item;
				}
			}

			/// <summary>
			/// Contador da utilização.
			/// </summary>
			public int UsageCount
			{
				get
				{
					return _usageCount;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="item"></param>
			public Entry(T item)
			{
				_usageCount = int.MaxValue / 2;
				_item = item;
			}

			/// <summary>
			/// Notifica o uso da entrada.
			/// </summary>
			/// <param name="inUse"></param>
			public void Notify(bool inUse)
			{
				_usageCount--;
				if(_usageCount < 0)
					_usageCount = 0;
			}
		}
	}
}
