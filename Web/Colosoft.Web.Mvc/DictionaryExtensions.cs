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
using System.Threading.Tasks;
using System.Web.Routing;

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com métodos para auxiliar na manipulação de dicionários.
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Merges the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
		public static void Merge(this IDictionary<string, object> instance, string key, object value, bool replaceExisting)
		{
			if(replaceExisting || !instance.ContainsKey(key))
			{
				instance[key] = value;
			}
		}

		/// <summary>
		/// Appends the in value.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="key">The key.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="value">The value.</param>
		public static void AppendInValue(this IDictionary<string, object> instance, string key, string separator, object value)
		{
			instance[key] = instance.ContainsKey(key) ? instance[key] + separator + value : value.ToString();
		}

		/// <summary>
		/// Appends the specified value at the beginning of the existing value
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="key"></param>
		/// <param name="separator"></param>
		/// <param name="value"></param>
		public static void PrependInValue(this IDictionary<string, object> instance, string key, string separator, object value)
		{
			instance[key] = instance.ContainsKey(key) ? value + separator + instance[key] : value.ToString();
		}

		/// <summary>
		/// Toes the attribute string.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public static string ToAttributeString(this IDictionary<string, object> instance)
		{
			StringBuilder attributes = new StringBuilder();
			foreach (KeyValuePair<string, object> attribute in instance)
			{
				attributes.Append(string.Format(System.Globalization.CultureInfo.CurrentCulture, " {0}=\"{1}\"", System.Web.HttpUtility.HtmlAttributeEncode(attribute.Key), System.Web.HttpUtility.HtmlAttributeEncode(attribute.Value.ToString())));
			}
			return attributes.ToString();
		}

		/// <summary>
		/// Merges the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="from">From.</param>
		/// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
		public static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from, bool replaceExisting)
		{
			foreach (KeyValuePair<string, object> pair in from)
			{
				if(!replaceExisting && instance.ContainsKey(pair.Key))
				{
					continue;
				}
				instance[pair.Key] = pair.Value;
			}
		}

		/// <summary>
		/// Merges the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="from">From.</param>
		public static void Merge(this IDictionary<string, object> instance, IDictionary<string, object> from)
		{
			Merge(instance, from, true);
		}

		/// <summary>
		/// Merges the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="values">The values.</param>
		/// <param name="replaceExisting">if set to <c>true</c> [replace existing].</param>
		public static void Merge(this IDictionary<string, object> instance, object values, bool replaceExisting)
		{
			Merge(instance, new RouteValueDictionary(values), replaceExisting);
		}

		/// <summary>
		/// Merges the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="values">The values.</param>
		public static void Merge(this IDictionary<string, object> instance, object values)
		{
			Merge(instance, values, true);
		}
	}
}
