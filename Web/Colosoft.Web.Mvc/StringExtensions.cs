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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Class com métodos de extensão para strings.
	/// </summary>
	public static class StringExtensions
	{
		private static readonly Regex EntityExpression = new Regex("(&amp;|&)#([0-9]+;)", RegexOptions.Compiled);

		private static readonly Regex NameExpression = new Regex("([A-Z]+(?=$|[A-Z][a-z])|[A-Z]?[a-z]+)", RegexOptions.Compiled);

		/// <summary>
		/// Verifica se o valor informado é um titulo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string AsTitle(this string value)
		{
			if(value == null)
			{
				return string.Empty;
			}
			int num = value.LastIndexOf(".", StringComparison.Ordinal);
			if(num > -1)
			{
				value = value.Substring(num + 1);
			}
			return value.SplitPascalCase();
		}

		/// <summary>
		/// Remove as entidades html
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string EscapeHtmlEntities(this string html)
		{
			return EntityExpression.Replace(html, @"$1\\#$2");
		}

		/// <summary>
		/// Formata o texto com os argumentos informados.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string FormatWith(this string instance, params object[] args)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, instance, args);
		}

		/// <summary>
		/// Verifica se o texto informado possui um valor.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasValue(this string value)
		{
			return !string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// Verifica se os dois texto informados são iguais com caseinsensitive.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="comparing"></param>
		/// <returns></returns>
		public static bool IsCaseInsensitiveEqual(this string instance, string comparing)
		{
			return (string.Compare(instance, comparing, StringComparison.OrdinalIgnoreCase) == 0);
		}

		/// <summary>
		/// Verifica se os texto são iguais com case sensitive.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="comparing"></param>
		/// <returns></returns>
		public static bool IsCaseSensitiveEqual(this string instance, string comparing)
		{
			return (string.CompareOrdinal(instance, comparing) == 0);
		}

		/// <summary>
		/// Quebra o textom como pascal.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SplitPascalCase(this string value)
		{
			return NameExpression.Replace(value, " $1").Trim();
		}

		/// <summary>
		/// ToCamelCase.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string ToCamelCase(this string instance)
		{
			char ch = instance[0];
			return (ch.ToString().ToLowerInvariant() + instance.Substring(1));
		}

		/// <summary>
		/// Recupera o valor do enum com base no texto informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value, T defaultValue)
		{
			if(!value.HasValue())
			{
				return defaultValue;
			}
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}
			catch(ArgumentException)
			{
				return defaultValue;
			}
		}
	}
}
