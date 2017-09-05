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
using System.Text.RegularExpressions;

namespace Colosoft.Caching.Util
{
	/// <summary>
	/// Representa uma carta coringa para compara com expressão regular.
	/// </summary>
	public class WildcardEnabledRegex
	{
		private Regex _regex;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="pattern">Padrão que será usado.</param>
		public WildcardEnabledRegex(string pattern)
		{
			_regex = new Regex(WildcardToRegex(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Verifica se o texto informado é compatível com a expressão regular.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public bool IsMatch(string text)
		{
			return _regex.IsMatch(RemoveDiacritics(text));
		}

		/// <summary>
		/// Recupera um coringa para o padrão informado.
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static string WildcardToRegex(string pattern)
		{
			pattern = RemoveDiacritics(pattern);
			return ("^" + Regex.Escape(pattern).Replace("%", ".*").Replace(@"\*", ".*").Replace(@"\?", "."));
		}

		/// <summary>
		/// Remove a Diacritics da string informada.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string RemoveDiacritics(string str)
		{
			if(str == null)
				return null;
			var chars = from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
			let uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
			where uc != System.Globalization.UnicodeCategory.NonSpacingMark
			select c;
			var cleanStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
			return cleanStr;
		}
	}
}
