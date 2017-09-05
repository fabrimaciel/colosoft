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

namespace Colosoft.Media.Drawing
{
	/// <summary>
	/// Armazena um tamanho.
	/// </summary>
	[Serializable, System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential), System.Runtime.InteropServices.ComVisible(true)]
	public struct Size
	{
		private int _width;

		private int _height;

		/// <summary>
		/// Identifica se o tamanho está vazio.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return ((_width == 0) && (_height == 0));
			}
		}

		/// <summary>
		/// Largura.
		/// </summary>
		public int Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		/// <summary>
		/// Altura.
		/// </summary>
		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Size(int width, int height)
		{
			_width = width;
			_height = height;
		}

		/// <summary>
		/// Soma os tamanhos.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static Size operator +(Size sz1, Size sz2)
		{
			return Add(sz1, sz2);
		}

		/// <summary>
		/// Subtraí os tamanhos.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static Size operator -(Size sz1, Size sz2)
		{
			return Subtract(sz1, sz2);
		}

		/// <summary>
		/// Compara os tamanhos.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static bool operator ==(Size sz1, Size sz2)
		{
			return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
		}

		/// <summary>
		/// Verifica se os tamanhos são diferentes.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static bool operator !=(Size sz1, Size sz2)
		{
			return !(sz1 == sz2);
		}

		/// <summary>
		/// Soma os tamanhos.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static Size Add(Size sz1, Size sz2)
		{
			return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		/// <summary>
		/// Subtraí os tamanhos.
		/// </summary>
		/// <param name="sz1"></param>
		/// <param name="sz2"></param>
		/// <returns></returns>
		public static Size Subtract(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		/// <summary>
		/// Verifica se a instancia informada é igual atual.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(!(obj is Size))
				return false;
			var size = (Size)obj;
			return ((size._width == _width) && (size._height == _height));
		}

		/// <summary>
		/// Recupera o HashCode.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_width ^ _height);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("{Width=" + _width.ToString(System.Globalization.CultureInfo.CurrentCulture) + ", Height=" + _height.ToString(System.Globalization.CultureInfo.CurrentCulture) + "}");
		}
	}
}
