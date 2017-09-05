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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Colosoft.Web.Mvc.Infrastructure
{
	/// <summary>
	/// Implementação do inicializador de javascript.
	/// </summary>
	public class JavaScriptInitializer : IJavaScriptInitializer
	{
		private static readonly Regex EscapeRegex = new Regex(@"([;&,\.\+\*~'\:\""\!\^\$\[\]\(\)\|\/])", RegexOptions.Compiled);

		/// <summary>
		/// Inicaliza os parametros.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual string Initialize(string id, string name, IDictionary<string, object> options)
		{
			return InitializeFor(EscapeRegex.Replace(id, @"\\$1"), name, options);
		}

		/// <summary>
		/// Inicializa os parametro para.
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual string InitializeFor(string selector, string name, IDictionary<string, object> options)
		{
			return new StringBuilder().Append("jQuery(function(){jQuery(\"").Append(selector).Append("\").colosoft").Append(name).Append("(").Append(Serialize(options)).Append(");});").ToString();
		}

		/// <summary>
		/// Cria o serializador javascript.
		/// </summary>
		/// <returns></returns>
		public virtual IJavaScriptSerializer CreateSerializer()
		{
			return new DefaultJavaScriptSerializer();
		}

		/// <summary>
		/// Serializa os dados do objeto informado.
		/// </summary>
		/// <param name="object"></param>
		/// <returns></returns>
		public virtual string Serialize(IDictionary<string, object> @object)
		{
			var output = new StringBuilder();
			output.Append("{");
			foreach (var keyValuePair in @object)
			{
				output.Append(",").Append("\"" + keyValuePair.Key + "\"").Append(":");
				var value = keyValuePair.Value;
				var @string = value as string;
				if(@string != null)
				{
					output.Append(System.Web.HttpUtility.JavaScriptStringEncode(@string, true));
					continue;
				}
				var dictionary = value as IDictionary<string, object>;
				if(dictionary != null)
				{
					output.Append(Serialize(dictionary));
					continue;
				}
				var dates = value as IEnumerable<DateTime>;
				if(dates != null)
				{
					AppendDates(output, dates);
					continue;
				}
				var nested = value as IEnumerable<IDictionary<string, object>>;
				if(nested != null)
				{
					AppendArrayOfObjects(output, nested);
					continue;
				}
				var serializer = CreateSerializer();
				var enumerable = value as IEnumerable;
				if(enumerable != null)
				{
					output.Append(serializer.Serialize(enumerable));
					continue;
				}
				if(value is bool)
				{
					AppendBoolean(output, (bool)value);
					continue;
				}
				if(value is DateTime)
				{
					AppendDate(output, (DateTime)value);
					continue;
				}
				if(value is int)
				{
					output.Append((int)value);
					continue;
				}
				if(value is double)
				{
					output.Append(((double)value).ToString("r", CultureInfo.InvariantCulture));
					continue;
				}
				if(value is float)
				{
					output.Append(((float)value).ToString("r", CultureInfo.InvariantCulture));
					continue;
				}
				if(value is Guid)
				{
					output.AppendFormat("\"{0}\"", value.ToString());
					continue;
				}
				if(value == null)
				{
					output.Append("null");
					continue;
				}
				if(!(value is Char) && (value.GetType().IsPrimitive || value is decimal))
				{
					AppendConvertible(output, value);
					continue;
				}
				var @event = value as ClientHandlerDescriptor;
				if(@event != null)
				{
					AppendEvent(output, @event);
					continue;
				}
				if(value is Enum)
				{
					output.Append(System.Web.HttpUtility.JavaScriptStringEncode(value.ToString().ToLower(), true));
					continue;
				}
				output.Append(serializer.Serialize(value));
			}
			if(output.Length >= 2)
			{
				output.Remove(1, 1);
			}
			output.Append("}");
			return output.ToString();
		}

		/// <summary>
		/// Anexa um valor boolean.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="value"></param>
		private void AppendBoolean(StringBuilder output, bool value)
		{
			if(value)
			{
				output.Append("true");
			}
			else
			{
				output.Append("false");
			}
		}

		/// <summary>
		/// Anexa um evento.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="value"></param>
		private void AppendEvent(StringBuilder output, ClientHandlerDescriptor value)
		{
			if(!string.IsNullOrEmpty(value.HandlerName))
			{
				output.Append(value.HandlerName);
			}
			else if(value.TemplateDelegate != null)
			{
				output.Append(value.TemplateDelegate(value));
			}
		}

		/// <summary>
		/// Anexa datas.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="dates"></param>
		private void AppendDates(StringBuilder output, IEnumerable<DateTime> dates)
		{
			output.Append("[");
			if(dates.Any())
			{
				foreach (var date in dates)
				{
					AppendDate(output, date);
					output.Append(",");
				}
				output.Remove(output.Length - 1, 1);
			}
			output.Append("]");
		}

		/// <summary>
		/// Anaxa um vetor de objetos.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="array"></param>
		private void AppendArrayOfObjects(StringBuilder output, IEnumerable<IDictionary<string, object>> array)
		{
			output.Append("[");
			if(array.Any())
			{
				foreach (var obj in array)
				{
					output.Append(Serialize(obj));
					output.Append(",");
				}
				output.Remove(output.Length - 1, 1);
			}
			output.Append("]");
		}

		/// <summary>
		/// Anexa um data.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="value"></param>
		private void AppendDate(StringBuilder output, DateTime value)
		{
			output.Append("new Date(").Append(value.Year).Append(",").Append(value.Month - 1).Append(",").Append(value.Day).Append(",").Append(value.Hour).Append(",").Append(value.Minute).Append(",").Append(value.Second).Append(",").Append(value.Millisecond).Append(")");
		}

		/// <summary>
		/// Aneaxa um valor convertivel.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="value"></param>
		private void AppendConvertible(StringBuilder output, object value)
		{
			var convertible = value as IConvertible;
			if(convertible != null)
			{
				output.Append(convertible.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
