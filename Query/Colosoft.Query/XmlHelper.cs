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

namespace Colosoft.Query.Serialization
{
	static class XmlHelper
	{
		/// <summary>
		/// Recupera o tipo XML.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static string XmlDataTypeName(Type type)
		{
			if(type == typeof(char))
				return "_";
			if(type == typeof(byte[]))
				return "base64Binary";
			if(type == typeof(DateTime))
				return "dateTime";
			if(type == typeof(TimeSpan))
				return "duration";
			if(type == typeof(decimal))
				return "decimal";
			if(type == typeof(int))
				return "int";
			if(type == typeof(bool))
				return "boolean";
			if(type == typeof(float))
				return "float";
			if(type == typeof(double))
				return "double";
			if(type == typeof(sbyte))
				return "byte";
			if(type == typeof(byte))
				return "unsignedByte";
			if(type == typeof(short))
				return "short";
			if(type == typeof(int))
				return "int";
			if(type == typeof(long))
				return "long";
			if(type == typeof(ushort))
				return "unsignedShort";
			if(type == typeof(uint))
				return "unsignedInt";
			if(type == typeof(ulong))
				return "unsignedLong";
			if(type == typeof(Uri))
				return "anyURI";
			if((type == typeof(string)) || (type == typeof(Guid)))
				return "string";
			if((type != typeof(object)) && (type != typeof(DateTimeOffset)))
				return string.Empty;
			return "anyType";
		}
	}
}
