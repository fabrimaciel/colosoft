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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Implementação do factory.
	/// </summary>
	public class JsonNetValueProviderFactory : ValueProviderFactory
	{
		/// <summary>
		/// Recupera a provedor de valor.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <returns></returns>
		public override IValueProvider GetValueProvider(ControllerContext controllerContext)
		{
			if(controllerContext == null)
				throw new ArgumentNullException("controllerContext");
			if(!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
				return null;
			var streamReader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
			var JSONReader = new JsonTextReader(streamReader);
			if(!JSONReader.Read())
				return null;
			var JSONSerializer = new JsonSerializer();
			JSONSerializer.Converters.Add(new Newtonsoft.Json.Converters.ExpandoObjectConverter());
			Object JSONObject;
			if(JSONReader.TokenType == JsonToken.StartArray)
				JSONObject = JSONSerializer.Deserialize<List<System.Dynamic.ExpandoObject>>(JSONReader);
			else
				JSONObject = JSONSerializer.Deserialize<System.Dynamic.ExpandoObject>(JSONReader);
			var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			AddToBackingStore(backingStore, String.Empty, JSONObject);
			return new DictionaryValueProvider<object>(backingStore, System.Globalization.CultureInfo.CurrentCulture);
		}

		private static void AddToBackingStore(Dictionary<string, object> backingStore, string prefix, object value)
		{
			var d = value as IDictionary<string, object>;
			if(d != null)
			{
				foreach (var entry in d)
				{
					AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
				}
				return;
			}
			var l = value as System.Collections.IList;
			if(l != null)
			{
				for(var i = 0; i < l.Count; i++)
				{
					AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
				}
				return;
			}
			backingStore[prefix] = value;
		}

		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "[" + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
		}
	}
}
