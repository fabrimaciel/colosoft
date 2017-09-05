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
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Serializers
{
	class FlagEnumJsonConverter : JsonConverter
	{
		public FlagEnumJsonConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			var isEnum = typeof(Enum).IsAssignableFrom(objectType);
			if(!isEnum)
			{
				return false;
			}
			var hasFlags = objectType.IsDefined(typeof(FlagsAttribute), false);
			return hasFlags;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var flags = EnumExtensions.GetIndividualFlags((Enum)value);
			var flagValues = flags.Select(e => Convert.ToUInt64(e));
			serializer.Serialize(writer, flagValues);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			UInt64 enumValue = 0;
			var value = reader.Value;
			bool keepGoing = false;
			do
			{
				switch(reader.TokenType)
				{
				case JsonToken.StartArray:
					keepGoing = true;
					break;
				case JsonToken.EndArray:
					keepGoing = false;
					break;
				case JsonToken.Integer:
					enumValue |= Convert.ToUInt64(reader.Value);
					break;
				case JsonToken.String:
					enumValue |= Convert.ToUInt64(Enum.Parse(objectType, (string)reader.Value));
					break;
				default:
					throw new FormatException("Invalid JSON");
				}
			}
			while (keepGoing && reader.Read());
			return Enum.ToObject(objectType, enumValue);
		}
	}
}
