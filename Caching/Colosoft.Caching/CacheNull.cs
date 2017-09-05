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

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um valor nulo do cache.
	/// </summary>
	internal sealed class CacheNull : IComparable
	{
		private static CacheNull _value = new CacheNull();

		/// <summary>
		/// Representa um valor nulo.
		/// </summary>
		public static CacheNull Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Protege o construtor.
		/// </summary>
		private CacheNull()
		{
		}

		/// <summary>
		/// Verifica se valor informado é nulo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNull(object value)
		{
			return value is CacheNull || value == null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "NULL";
		}

		/// <summary>
		/// Compara com a instancia informada
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(IsNull(obj))
				return 0;
			return -1;
		}
	}
}
