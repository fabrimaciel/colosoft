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
using System.Web.Script.Serialization;

namespace Colosoft.Web.Script.Serialization
{
	/// <summary>
	/// Implementação do conversor de DateTime para javascript.
	/// </summary>
	public class DateTimeConverter : JavaScriptConverter
	{
		private System.Globalization.CultureInfo _culture;

		/// <summary>
		/// Tipos suportados.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new List<Type>(new[] {
					typeof(DateTime),
					typeof(DateTime?)
				}));
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="culture">Cultura que será usada pela instancia.</param>
		public DateTimeConverter(System.Globalization.CultureInfo culture)
		{
			_culture = culture;
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="dictionary">Dicionário com os dados.</param>
		/// <param name="type">Tipo.</param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if(dictionary == null)
				throw new ArgumentNullException("dictionary");
			var value = dictionary.Select(f => f.Value as string).FirstOrDefault();
			if(string.IsNullOrEmpty(value) && type == typeof(DateTime?))
				return null;
			if(type == typeof(DateTime))
			{
				DateTime time;
				time = DateTime.Parse(value, _culture);
				return time;
			}
			return null;
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			DateTime? time = obj as DateTime?;
			if(time.HasValue)
			{
				Dictionary<string, object> result = new Dictionary<string, object>();
				result[""] = time.Value.ToString(_culture);
				return result;
			}
			return new Dictionary<string, object>();
		}
	}
}
