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

namespace Colosoft.Globalization
{
	/// <summary>
	/// Implementação do comparador padrão do sistema.
	/// </summary>
	public class SystemComparer : System.Collections.IComparer
	{
		private static System.Collections.IComparer _default = new SystemComparer();

		/// <summary>
		/// Instancia do comparador padrão.
		/// </summary>
		public static System.Collections.IComparer Default
		{
			get
			{
				return _default;
			}
			set
			{
				_default = value;
			}
		}

		/// <summary>
		/// Compara a instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			if(x is string || y is string)
			{
				return Culture.StringComparer.Compare(x is string ? (string)x : (x != null ? x.ToString() : null), y is string ? (string)y : (y != null ? y.ToString() : null));
			}
			return System.Collections.Comparer.DefaultInvariant.Compare(x, y);
		}
	}
}
