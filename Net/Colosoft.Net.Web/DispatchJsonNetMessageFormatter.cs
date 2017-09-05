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
using System.IO;
using System.ServiceModel.Description;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Colosoft.Net.Json.Formatters;
using Colosoft.Net.Json;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Class DispatchJsonNetMessageFormatter.
	/// </summary>
	public class DispatchJsonNetMessageFormatter : DispatchJsonMessageFormatter
	{
		private readonly JsonSerializer serializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchJsonNetMessageFormatter"/> class.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="serializer">The serializer.</param>
		/// <param name="serviceRegister">The service register.</param>
		public DispatchJsonNetMessageFormatter(OperationDescription operation, JsonSerializer serializer, IServiceRegister serviceRegister) : base(operation, serviceRegister)
		{
			this.serializer = serializer;
		}

		/// <summary>
		/// Decodes the parameters.
		/// </summary>
		/// <param name="body">The body.</param>
		/// <param name="parameters">The parameters.</param>
		public override void DecodeParameters(byte[] body, object[] parameters)
		{
			using (MemoryStream ms = new MemoryStream(body))
			{
				using (StreamReader sr = new StreamReader(ms))
				{
					using (JsonReader reader = new JsonTextReader(sr))
					{
						JObject wrappedParameters = serializer.Deserialize<JObject>(reader);
						int indexParam = -1;
						foreach (var parameter in this.OperationParameters)
						{
							JProperty property = wrappedParameters.Property(parameter.Name);
							if(property != null)
							{
								Type type = this.ServiceRegister.GetTypeByName(JsonFormatterUtility.GetTypeNameFromJObject(property.Value as JObject), false) ?? parameter.NormalizedType;
								parameters[++indexParam] = property.Value.ToObject(type, serializer);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Encodes the reply.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="result">The result.</param>
		/// <returns>System.Byte[].</returns>
		public override byte[] EncodeReply(object[] parameters, object result)
		{
			byte[] body;
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
				{
					using (JsonWriter writer = new JsonTextWriter(sw))
					{
						if(result == null)
						{
							serializer.Serialize(writer, null);
						}
						else
						{
							JToken token = JToken.FromObject(result, serializer);
							JsonFormatterUtility.JTokenToSerialize(token);
							serializer.Serialize(writer, token);
						}
						writer.Flush();
					}
				}
				body = ms.ToArray();
			}
			return body;
		}
	}
}
