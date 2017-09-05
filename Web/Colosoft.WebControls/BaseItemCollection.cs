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
using System.Web;
using System.Security.Permissions;
using System.Collections;
using System.Web.UI;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa um coleção de item basicos.
	/// </summary>
	/// <typeparam name="OwnerType"></typeparam>
	/// <typeparam name="ItemType"></typeparam>
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public abstract class BaseItemCollection<OwnerType, ItemType> : StateManagedCollection
	{
		private static readonly Type[] _knownTypes;

		private OwnerType _owner;

		private int capacity;

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
			set
			{
				this.capacity = value;
			}
		}

		public ItemType this[int i]
		{
			get
			{
				return (ItemType)((IList)this)[i];
			}
			set
			{
				this[i] = value;
			}
		}

		/// <summary>
		/// Construtor estático para a inicializa das variáveis.
		/// </summary>
		static BaseItemCollection()
		{
			BaseItemCollection<OwnerType, ItemType>._knownTypes = new Type[] {
				typeof(ItemType)
			};
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BaseItemCollection()
		{
		}

		/// <summary>
		/// Constrói a instancia informando a instancia do tipo pai.
		/// </summary>
		/// <param name="owner"></param>
		public BaseItemCollection(OwnerType owner)
		{
			_owner = owner;
		}

		protected abstract override object CreateKnownType(int index);

		protected override Type[] GetKnownTypes()
		{
			return BaseItemCollection<OwnerType, ItemType>._knownTypes;
		}

		/// <summary>
		/// Método acionado quando a coleção for limpa.
		/// </summary>
		protected override void OnClear()
		{
			base.OnClear();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);
		}

		protected override void OnValidate(object value)
		{
			base.OnValidate(value);
		}

		/// <summary>
		/// Marca o objeto com sujo.
		/// </summary>
		/// <param name="o"></param>
		protected override void SetDirtyObject(object o)
		{
			if(o is BaseItem)
			{
				((BaseItem)o).SetDirty();
			}
		}

		public int Add(ItemType item)
		{
			return ((IList)this).Add(item);
		}

		public bool Contains(ItemType item)
		{
			return ((IList)this).Contains(item);
		}

		public void CopyTo(ItemType[] array, int index)
		{
			this.CopyTo(array, index);
		}

		public int IndexOf(ItemType value)
		{
			return ((IList)this).IndexOf(value);
		}

		public void Insert(int index, ItemType item)
		{
			((IList)this).Insert(index, item);
		}

		public void Remove(ItemType item)
		{
			this.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.RemoveAt(index);
		}
	}
}
