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

namespace Colosoft.Web.Mvc
{
	class TypeHelpers
	{
		public static bool IsNullableValueType(Type type)
		{
			return (Nullable.GetUnderlyingType(type) != null);
		}

		public static bool TypeAllowsNullValue(Type type)
		{
			if(type.IsValueType)
			{
				return IsNullableValueType(type);
			}
			return true;
		}

		public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
		{
			if(MatchesGenericType(queryType, interfaceType))
			{
				return queryType;
			}
			return MatchGenericTypeFirstOrDefault(queryType.GetInterfaces(), interfaceType);
		}

		private static Type MatchGenericTypeFirstOrDefault(Type[] types, Type matchType)
		{
			for(int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if(MatchesGenericType(type, matchType))
				{
					return type;
				}
			}
			return null;
		}

		private static bool MatchesGenericType(Type type, Type matchType)
		{
			return (type.IsGenericType && (type.GetGenericTypeDefinition() == matchType));
		}
	}
}
