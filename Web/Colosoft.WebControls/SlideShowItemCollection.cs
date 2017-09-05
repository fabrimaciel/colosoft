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
using System.Text;
using System.Collections;

namespace Colosoft.WebControls
{
	public sealed class SlideShowItemCollection : IList, ICollection, IEnumerable, IEnumerable<SlideShowItem>
	{
		private ArrayList listItems = new ArrayList();

		private bool marked = false;

		private bool saveAll = false;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SlideShowItemCollection()
		{
		}

		/// <summary>
		/// Constrói a instancia com base em dados já existentes
		/// </summary>
		/// <param name="items"></param>
		public SlideShowItemCollection(IEnumerable<SlideShowItem> items)
		{
		}

		/// <summary>
		/// Cria e adiciona um novo item.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="description"></param>
		/// <param name="imageUrl"></param>
		public void Add(string title, string description, string imageUrl)
		{
			this.Add(new SlideShowItem(title, description, imageUrl));
		}

		public void Add(SlideShowItem item)
		{
			this.listItems.Add(item);
		}

		public void AddRange(SlideShowItem[] items)
		{
			if(items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (SlideShowItem item in items)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// Limpa a colecao.
		/// </summary>
		public void Clear()
		{
			this.listItems.Clear();
			if(this.marked)
			{
				this.saveAll = true;
			}
		}

		public bool Contains(SlideShowItem item)
		{
			return this.listItems.Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			this.listItems.CopyTo(array, index);
		}

		/// <summary>
		/// Localiza o item com o titulo informado.
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public SlideShowItem FindByTitle(string title)
		{
			int num = 0;
			foreach (SlideShowItem item in this.listItems)
			{
				if(item.Title == title)
				{
					break;
				}
				num++;
			}
			if(num != -1)
			{
				return (SlideShowItem)this.listItems[num];
			}
			return null;
		}

		public IEnumerator GetEnumerator()
		{
			return this.listItems.GetEnumerator();
		}

		public int IndexOf(SlideShowItem item)
		{
			return this.listItems.IndexOf(item);
		}

		/// <summary>
		/// Cria e adiciona um novo item na colecao na posicao informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="title"></param>
		/// <param name="description"></param>
		/// <param name="imageUrl"></param>
		public void Insert(int index, string title, string description, string imageUrl)
		{
			this.Insert(index, new SlideShowItem(title, description, imageUrl));
		}

		public void Insert(int index, SlideShowItem item)
		{
			this.listItems.Insert(index, item);
			if(this.marked)
			{
				this.saveAll = true;
			}
		}

		public void Remove(string title)
		{
			int index = this.IndexOf(new SlideShowItem(title));
			if(index >= 0)
			{
				this.RemoveAt(index);
			}
		}

		public void Remove(SlideShowItem item)
		{
			int index = this.IndexOf(item);
			if(index >= 0)
			{
				this.RemoveAt(index);
			}
		}

		public void RemoveAt(int index)
		{
			this.listItems.RemoveAt(index);
			if(this.marked)
			{
				this.saveAll = true;
			}
		}

		int IList.Add(object item)
		{
			SlideShowItem item2 = (SlideShowItem)item;
			int num = this.listItems.Add(item2);
			return num;
		}

		bool IList.Contains(object item)
		{
			return this.Contains((SlideShowItem)item);
		}

		int IList.IndexOf(object item)
		{
			return this.IndexOf((SlideShowItem)item);
		}

		void IList.Insert(int index, object item)
		{
			this.Insert(index, (SlideShowItem)item);
		}

		void IList.Remove(object item)
		{
			this.Remove((SlideShowItem)item);
		}

		public bool SaveAll
		{
			get
			{
				return saveAll;
			}
			set
			{
				saveAll = value;
			}
		}

		public int Capacity
		{
			get
			{
				return this.listItems.Capacity;
			}
			set
			{
				this.listItems.Capacity = value;
			}
		}

		public int Count
		{
			get
			{
				return this.listItems.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.listItems.IsReadOnly;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.listItems.IsSynchronized;
			}
		}

		public SlideShowItem this[int index]
		{
			get
			{
				return (SlideShowItem)this.listItems[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.listItems[index];
			}
			set
			{
				this.listItems[index] = (SlideShowItem)value;
			}
		}

		IEnumerator<SlideShowItem> IEnumerable<SlideShowItem>.GetEnumerator()
		{
			return GetEnumerable().GetEnumerator();
		}

		private IEnumerable<SlideShowItem> GetEnumerable()
		{
			foreach (SlideShowItem i in this.listItems)
				yield return i;
		}
	}
}
