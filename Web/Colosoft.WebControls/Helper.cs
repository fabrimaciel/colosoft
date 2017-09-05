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
using System.Text;
using System.Collections.Specialized;

namespace Colosoft.WebControls.Route.Security
{
	public class Helper
	{
		public const char DEFAULT_DATA_SEPARATOR = ',';

		/// <summary>
		/// Transforma o valor informado para sua equivalencia no Enum.
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TEnum ParseEnum<TEnum>(int value) where TEnum : struct
		{
			return ParseEnum<TEnum>(value.ToString());
		}

		/// <summary>
		/// Transforma o valor informado para sua equivalencia no Enum.
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TEnum ParseEnum<TEnum>(string value) where TEnum : struct
		{
			if(string.IsNullOrEmpty(value))
				throw new ArgumentNullException("value");
			return (TEnum)Enum.Parse(typeof(TEnum), value);
		}

		/// <summary>
		/// Recupera o valor boleano da propriedade de configuração.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="propertyName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		internal static bool GetBooleanConfigValue(NameValueCollection config, string propertyName, bool defaultValue)
		{
			if(config == null)
				throw new ArgumentNullException("config");
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			string value = config[propertyName];
			if(string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			else
			{
				bool result = false;
				if(!bool.TryParse(value, out result))
					return defaultValue;
				return result;
			}
		}

		internal static int GetIntConfigValue(NameValueCollection config, string propertyName, int defaultValue)
		{
			if(config == null)
				throw new ArgumentNullException("config");
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			string value = config[propertyName];
			if(string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			else
			{
				int result = 0;
				if(!int.TryParse(value, out result))
					return defaultValue;
				return result;
			}
		}

		public static string ConvertCollectionInString(StringCollection collection)
		{
			if(collection == null)
				throw new ArgumentNullException("collection");
			StringEnumerator se = collection.GetEnumerator();
			List<string> data = new List<string>();
			while (se.MoveNext())
				data.Add(se.Current);
			return string.Join(DEFAULT_DATA_SEPARATOR.ToString(), data.ToArray());
		}

		/// <summary>
		/// Extrái uma coleção dos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static StringCollection ExtractCollectionFromString(string data)
		{
			if(string.IsNullOrEmpty(data))
				throw new ArgumentNullException("data");
			StringCollection collection = new StringCollection();
			Array.ForEach<string>(data.Split(new char[] {
				DEFAULT_DATA_SEPARATOR
			}), s => collection.Add(s));
			return collection;
		}

		internal static void ValidateDataCollection(StringCollection data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			foreach (string item in data)
			{
				if(item.Length > 1)
				{
					if(item.IndexOfAny(new char[] {
						'*',
						'?'
					}) > -1)
					{
						throw new InvalidCaracterException();
					}
				}
				else if(item != "*" && item != "?")
				{
					throw new InvalidCaracterException();
				}
			}
		}
	}
}
