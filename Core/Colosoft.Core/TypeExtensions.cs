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

namespace Colosoft
{
	/// <summary>
	/// Método de extensão para a classe Type
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Tipos predefinidos considerados no sistema.
		/// </summary>
		private static readonly Type[] PredefinedTypes =  {
			typeof(Object),
			typeof(Boolean),
			typeof(Char),
			typeof(String),
			typeof(SByte),
			typeof(Byte),
			typeof(Int16),
			typeof(UInt16),
			typeof(Int32),
			typeof(UInt32),
			typeof(Int64),
			typeof(UInt64),
			typeof(Single),
			typeof(Double),
			typeof(Decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Math),
			typeof(Convert)
		};

		/// <summary>
		/// Verifica se o tipo informado é pré-definido.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsPredefinedType(this Type type)
		{
			foreach (Type t in PredefinedTypes)
				if(t == type)
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se o tipo informado é um System.Nullable wrapper
		/// </summary>
		/// <param name="type">Tipo a ser verificado.</param>
		/// <returns>True if the type is a System.Nullable wrapper</returns>
		public static bool IsNullable(this Type type)
		{
			return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		/// <summary>
		/// Recupera o tipo não Nullable.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static Type GetNonNullableType(this Type type)
		{
			return IsNullable(type) ? type.GetGenericArguments()[0] : type;
		}

		/// <summary>
		/// Obtém o tipo de dados base. Para tipos que sejam <see cref="Nullable"/>
		/// retorna o tipo puro associado.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static Type GetRootType(this Type instance)
		{
			if(instance == null)
			{
				return null;
			}
			if(instance.IsGenericType && instance.Name.StartsWith("Nullable"))
			{
				var gPar = instance.GetGenericArguments();
				return gPar.IsNullOrEmpty() ? null : gPar[0];
			}
			return instance;
		}

		/// <summary>
		/// Identifica se o nome do tipo está na lista separada por ':' fornecida.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="nameList"></param>
		/// <returns></returns>
		private static bool MatchName(Type instance, string nameList)
		{
			if(instance == null)
			{
				return false;
			}
			var name = String.Format(":{0}:", instance.GetRootType().Name);
			return nameList.IndexOf(name) >= 0;
		}

		/// <summary>
		/// Identifica se o tipo se refere a um dos tipos inteiros do CLR.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsIntegerType(this Type instance)
		{
			return MatchName(instance, ":Int16:Int32:Int64:UInt16:UInt32:UInt64:Byte:SByte:");
		}

		/// <summary>
		/// Identifica se o tipo se refere a um dos tipos de data do CLR.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsDateType(this Type instance)
		{
			return MatchName(instance, ":DateTime:DateTimeOffset:");
		}

		/// <summary>
		/// Identifica se o tipo se refere a um dos tipos de ponto flutuante do CLR.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsFloatingPointType(this Type instance)
		{
			return MatchName(instance, ":Double:Single:Decimal:");
		}

		/// <summary>
		/// Recupera o nome do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Reflection.TypeName GetName(this Type type)
		{
			type.Require("type").NotNull();
			return Reflection.TypeName.Get(type);
		}

		/// <summary>
		/// Verifica se é um tipo numérico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsNumericType(this Type type)
		{
			return GetNumericTypeKind(type) != 0;
		}

		/// <summary>
		/// Verifica se é um tipo integral com sinal.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsSignedIntegralType(this Type type)
		{
			return GetNumericTypeKind(type) == 2;
		}

		/// <summary>
		/// Verifica se é um tipo integração sem sinal.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsUnsignedIntegralType(this Type type)
		{
			return GetNumericTypeKind(type) == 3;
		}

		/// <summary>
		/// Recupera o tipo numérico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static int GetNumericTypeKind(this Type type)
		{
			if(type == null)
			{
				return 0;
			}
			type = GetNonNullableType(type);
			if(type.IsEnum)
			{
				return 0;
			}
			switch(Type.GetTypeCode(type))
			{
			case TypeCode.Char:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return 1;
			case TypeCode.SByte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				return 2;
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return 3;
			default:
				return 0;
			}
		}
	}
}
