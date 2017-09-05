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

namespace Colosoft.Text
{
	/// <summary>
	/// Implementação do comparador de string.
	/// </summary>
	public class StringEqualityComparer : IEqualityComparer<String>
	{
		private readonly StringComparison _comparison;

		private static readonly Dictionary<StringComparison, StringEqualityComparer> _comparerMap = new Dictionary<StringComparison, StringEqualityComparer>();

		private static readonly Object _compareInitSync = new Object();

		/// <summary>
		/// Tipo de comparação utilizado.
		/// </summary>
		public StringComparison Comparison
		{
			get
			{
				return _comparison;
			}
		}

		/// <summary>
		/// Recupera o comparador.
		/// </summary>
		/// <param name="comparison"></param>
		/// <returns></returns>
		public static StringEqualityComparer GetComparer(StringComparison comparison)
		{
			StringEqualityComparer comparer;
			if(!_comparerMap.TryGetValue(comparison, out comparer))
			{
				lock (_compareInitSync)
				{
					if(!_comparerMap.TryGetValue(comparison, out comparer))
					{
						comparer = new StringEqualityComparer(comparison);
						_comparerMap[comparison] = comparer;
					}
				}
			}
			return comparer;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="comparison"></param>
		public StringEqualityComparer(StringComparison comparison)
		{
			_comparison = comparison;
		}

		/// <summary>
		/// Compara as duas instancias.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(string x, string y)
		{
			return string.Compare(x, y, Comparison) == 0;
		}

		/// <summary>
		/// Recupera o HashCode da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public int GetHashCode(string obj)
		{
			if(obj == null)
				throw new ArgumentNullException(obj);
			switch(_comparison)
			{
			case StringComparison.CurrentCultureIgnoreCase:
				return obj.ToLower(System.Globalization.CultureInfo.CurrentCulture).GetHashCode();
			case StringComparison.InvariantCultureIgnoreCase:
				return obj.ToLowerInvariant().GetHashCode();
			case StringComparison.OrdinalIgnoreCase:
				return obj.ToLower().GetHashCode();
			case StringComparison.Ordinal:
			case StringComparison.InvariantCulture:
			case StringComparison.CurrentCulture:
				return obj.GetHashCode();
			}
			throw new InvalidOperationException("Unknown 'Comparison' value: " + Comparison);
		}
	}
}
