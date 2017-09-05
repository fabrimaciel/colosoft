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

namespace Colosoft.Serialization
{
	/// <summary>
	/// Representa um indice virtual.
	/// </summary>
	public class VirtualIndex : IComparable
	{
		private int _x;

		private int _y;

		/// <summary>
		/// Valor do indice.
		/// </summary>
		public int IndexValue
		{
			get
			{
				return ((_y * 0x13c00) + _x);
			}
		}

		/// <summary>
		/// Valor de X;
		/// </summary>
		internal int XIndex
		{
			get
			{
				return _x;
			}
		}

		/// <summary>
		/// Valor do Y.
		/// </summary>
		internal int YIndex
		{
			get
			{
				return this._y;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public VirtualIndex()
		{
		}

		/// <summary>
		/// Cria um instancia já com o valor padrão.
		/// </summary>
		/// <param name="index"></param>
		public VirtualIndex(int index)
		{
			this.IncrementBy(index);
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public VirtualIndex Clone()
		{
			VirtualIndex index = new VirtualIndex();
			index._x = this._x;
			index._y = this._y;
			return index;
		}

		/// <summary>
		/// Compara a instancia com outro objeto.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			VirtualIndex index = null;
			if(obj is VirtualIndex)
			{
				index = obj as VirtualIndex;
			}
			else if(obj is int)
			{
				index = new VirtualIndex((int)obj);
			}
			else
			{
				return -1;
			}
			if(index.IndexValue == this.IndexValue)
			{
				return 0;
			}
			if(index.IndexValue > this.IndexValue)
			{
				return -1;
			}
			return 1;
		}

		/// <summary>
		/// Incrementa o indice.
		/// </summary>
		public void Increment()
		{
			_x++;
			if(_x == 0x13c00)
			{
				_x = 0;
				_y++;
			}
		}

		/// <summary>
		/// Incrementa a quantidade informada.
		/// </summary>
		/// <param name="count"></param>
		public void IncrementBy(int count)
		{
			int num = ((_y * 0x13c00) + _x) + count;
			_x = num % 0x13c00;
			_y = num / 0x13c00;
		}
	}
}
