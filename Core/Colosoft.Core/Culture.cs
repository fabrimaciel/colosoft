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
	/// Armazena as configurações de cultura do sistema.
	/// </summary>
	public static class Culture
	{
		private static readonly CultureInfo _systemCulture;

		private static readonly StringComparer _stringComparer;

		private static readonly CultureInfo _invariantEnglishUS;

		/// <summary>
		/// Cultura invariante para inglês dos Estados Unidos.
		/// </summary>
		public static CultureInfo InvariantEnglishUS
		{
			get
			{
				return _invariantEnglishUS;
			}
		}

		/// <summary>
		/// Cultura padrão do sistema.
		/// </summary>
		public static CultureInfo SystemCulture
		{
			get
			{
				return _systemCulture;
			}
		}

		/// <summary>
		/// Instancia do comparador de string do sistema.
		/// </summary>
		public static StringComparer StringComparer
		{
			get
			{
				return _stringComparer;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static Culture()
		{
			_systemCulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
			_stringComparer = new SystemStringComparer();
			_invariantEnglishUS = CultureInfo.ReadOnly(new CultureInfo("en-US", false));
		}
	}
}
