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
using System.Globalization;

namespace Colosoft.Text
{
	/// <summary>
	/// Extensões para strings.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Normaliza a string para uma url.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string NormalizeStringForUrl(this string name)
		{
			if(string.IsNullOrEmpty(name))
				return name;
			String normalizedString = name.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in normalizedString)
			{
				switch(System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c))
				{
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.DecimalDigitNumber:
					stringBuilder.Append(c);
					break;
				case UnicodeCategory.SpaceSeparator:
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.DashPunctuation:
					stringBuilder.Append('_');
					break;
				}
			}
			string result = stringBuilder.ToString();
			return String.Join("_", result.Split(new char[] {
				'_'
			}, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Joins the specified items into a string separated by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public static string DelimitWith<T>(this IEnumerable<T> source, string separator)
		{
			return source.DelimitWith(separator, null);
		}

		/// <summary>
		/// Joins the specified items into a string separated by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="format">The item format string.</param>
		/// <returns></returns>
		public static string DelimitWith<T>(this IEnumerable<T> source, string separator, string format)
		{
			return source.DelimitWith(separator, format, string.Empty, string.Empty);
		}

		/// <summary>
		/// Joins the specified items into a string separated by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="format">The item format string.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="suffix">The suffix.</param>
		/// <returns></returns>
		public static string DelimitWith<T>(this IEnumerable<T> source, string separator, string format, string prefix, string suffix)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(separator == null)
			{
				throw new ArgumentNullException("separator");
			}
			using (var enumerator = source.GetEnumerator())
			{
				if(enumerator.MoveNext())
				{
					var buffer = new StringBuilder();
					if(prefix != null)
					{
						buffer.Append(prefix);
					}
					AppendItem(enumerator, buffer, format);
					while (enumerator.MoveNext())
					{
						buffer.Append(separator);
						AppendItem(enumerator, buffer, format);
					}
					if(suffix != null)
					{
						buffer.Append(suffix);
					}
					return buffer.ToString();
				}
				return string.Empty;
			}
		}

		private static void AppendItem<T>(IEnumerator<T> enumerator, StringBuilder buffer, string format)
		{
			if(format != null)
			{
				buffer.AppendFormat(format, enumerator.Current);
			}
			else
			{
				buffer.Append(enumerator.Current);
			}
		}
	}
}
