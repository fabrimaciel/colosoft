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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Colosoft.Globalization
{
	/// <summary>
	/// Representa o comparador d string padrão do sistema.
	/// </summary>
	public class SystemStringComparer : StringComparer
	{
		private static readonly SystemComparer _default = new SystemComparer();

		/// <summary>
		/// Instancia padrão do comparador.
		/// </summary>
		public static SystemComparer Default
		{
			get
			{
				return _default;
			}
		}

		/// <summary>
		/// Compara as instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static int GlobalCompare(object x, object y)
		{
			return Default.Compare(x, y);
		}

		/// <summary>
		/// Compara a duas strings informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(string x, string y)
		{
			return Culture.SystemCulture.CompareInfo.Compare(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}

		/// <summary>
		/// Verifica se as duas string informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(string x, string y)
		{
			return Culture.SystemCulture.CompareInfo.Compare(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0;
		}

		/// <summary>
		/// Recupera o hashcode da string informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override int GetHashCode(string obj)
		{
			return Culture.SystemCulture.CompareInfo.GetSortKey(obj, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth).GetHashCode();
		}
	}
}
