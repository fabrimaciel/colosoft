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
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Class QueryStringJsonConverter.
	/// </summary>
	public class QueryStringJsonConverter : QueryStringConverter
	{
		private JsonSerializer serializer;

		private readonly JsonSerializerSettings settings;

		private readonly IServiceRegister serviceRegister;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStringJsonConverter"/> class.
		/// </summary>
		/// <param name="serializer">The serializer.</param>
		/// <param name="serviceRegister">The service register.</param>
		public QueryStringJsonConverter(JsonSerializer serializer, IServiceRegister serviceRegister)
		{
			this.serializer = serializer;
			this.serviceRegister = serviceRegister;
			this.settings = serializer.MakeSettings();
		}

		/// <summary>
		/// Determines whether the specified type can be converted to and from a string representation.
		/// </summary>
		/// <param name="type">The <see cref="T:System.Type" /> to convert.</param>
		/// <returns>A value that specifies whether the type can be converted.</returns>
		public override bool CanConvert(Type type)
		{
			return true;
		}

		/// <summary>
		/// Converts a query string parameter to the specified type.
		/// </summary>
		/// <param name="parameter">The string form of the parameter and value.</param>
		/// <param name="parameterType">The <see cref="T:System.Type" /> to convert the parameter to.</param>
		/// <returns>The converted parameter.</returns>
		/// <exception cref="System.InvalidOperationException">Error when the serializer tried to deserialize the given parameter.</exception>
		public override object ConvertStringToValue(string parameter, Type parameterType)
		{
			try
			{
				if(parameter == null)
				{
					if(parameterType.IsClass || parameterType.IsInterface || parameterType.Name.Equals("Nullable`1", StringComparison.InvariantCultureIgnoreCase))
						parameter = "null";
					else
						parameter = Activator.CreateInstance(parameterType).ToString();
				}
				if(parameterType.IsInterface)
					parameterType = this.serviceRegister.TryToNormalize(parameterType);
				return JsonConvert.DeserializeObject(parameter, parameterType, this.settings);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException("Error when the serializer tried to deserialize the given parameter.", ex);
			}
		}

		/// <summary>
		/// Converts a parameter to a query string representation.
		/// </summary>
		/// <param name="parameter">The parameter to convert.</param>
		/// <param name="parameterType">The <see cref="T:System.Type" /> of the parameter to convert.</param>
		/// <returns>The parameter name and value.</returns>
		/// <exception cref="System.InvalidOperationException">Error when the serializer tried to serialize the given parameter.</exception>
		public override string ConvertValueToString(object parameter, Type parameterType)
		{
			try
			{
				return JsonConvert.SerializeObject(parameter, Formatting.None, this.settings);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException("Error when the serializer tried to serialize the given parameter.", ex);
			}
		}
	}
}
