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
using System.Runtime.Serialization;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Representa a coleção de linhas.
	/// </summary>
	[Serializable]
	public class RowCollection : IList, ICollection, IEnumerable, IEnumerable<string>, ISerializable
	{
		private ArrayList listItems = new ArrayList();

		private bool marked = false;

		private bool saveAll = false;

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

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RowCollection()
		{
		}

		/// <summary>
		/// Constrói a instancia com base em dados já existentes
		/// </summary>
		/// <param name="items"></param>
		public RowCollection(IEnumerable<string> items)
		{
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected RowCollection(SerializationInfo info, StreamingContext context)
		{
			var items = (string[])info.GetValue("Items", typeof(string[]));
			if(items != null && items.Length > 0)
				this.listItems.AddRange(items);
		}

		public void Add(string item)
		{
			this.listItems.Add(item);
		}

		public void AddRange(string[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			foreach (var item in items)
				this.Add(item);
		}

		/// <summary>
		/// Limpa a colecao.
		/// </summary>
		public void Clear()
		{
			this.listItems.Clear();
			if(this.marked)
				this.saveAll = true;
		}

		public bool Contains(string item)
		{
			return this.listItems.Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			this.listItems.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.listItems.GetEnumerator();
		}

		public int IndexOf(string item)
		{
			return this.listItems.IndexOf(item);
		}

		public void Insert(int index, string item)
		{
			this.listItems.Insert(index, item);
			if(this.marked)
				this.saveAll = true;
		}

		public void Remove(string item)
		{
			int index = this.IndexOf(item);
			if(index >= 0)
				this.RemoveAt(index);
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
			var item2 = (string)item;
			int num = this.listItems.Add(item2);
			return num;
		}

		bool IList.Contains(object item)
		{
			return this.Contains((string)item);
		}

		int IList.IndexOf(object item)
		{
			return this.IndexOf((string)item);
		}

		void IList.Insert(int index, object item)
		{
			this.Insert(index, (string)item);
		}

		void IList.Remove(object item)
		{
			this.Remove((string)item);
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

		public string this[int index]
		{
			get
			{
				return (string)this.listItems[index];
			}
			set
			{
				this.listItems[index] = value;
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

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return GetEnumerable().GetEnumerator();
		}

		private IEnumerable<string> GetEnumerable()
		{
			foreach (string i in this.listItems)
				yield return i;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Items", this.listItems.ToArray(typeof(string)), typeof(string[]));
		}
	}
}
