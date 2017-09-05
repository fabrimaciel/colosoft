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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com métodos de extensão para trabalhar com tipo.
	/// </summary>
	static class TypeExtensions
	{
		/// <summary>
		/// Tipos predefinidos.
		/// </summary>
		internal static readonly Type[] PredefinedTypes = new Type[] {
			typeof(object),
			typeof(bool),
			typeof(char),
			typeof(string),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Math),
			typeof(Convert)
		};

		/// <summary>
		/// Adiciona as interfaces do tipo para a lista informada.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="type"></param>
		private static void AddInterface(List<Type> types, Type type)
		{
			if(!types.Contains(type))
			{
				types.Add(type);
				foreach (Type type2 in type.GetInterfaces())
				{
					AddInterface(types, type2);
				}
			}
		}

		/// <summary>
		/// Verifica se os argumentso são aplicáveis.
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static bool AreArgumentsApplicable(IEnumerable<Type> arguments, IEnumerable<ParameterInfo> parameters)
		{
			List<Type> list = arguments.ToList<Type>();
			List<ParameterInfo> list2 = parameters.ToList<ParameterInfo>();
			if(list.Count != list2.Count)
				return false;
			for(int i = 0; i < list.Count; i++)
			{
				if(list2[i].ParameterType != list[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Recupera o valor padrão do tipo.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object DefaultValue(this Type type)
		{
			if(type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		/// <summary>
		/// Localiza o tipo genérico.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="genericType"></param>
		/// <returns></returns>
		public static Type FindGenericType(this Type type, Type genericType)
		{
			while ((type != null) && (type != typeof(object)))
			{
				if(type.IsGenericType && (type.GetGenericTypeDefinition() == genericType))
					return type;
				if(genericType.IsInterface)
				{
					foreach (Type type2 in type.GetInterfaces())
					{
						Type type3 = type2.FindGenericType(genericType);
						if(type3 != null)
						{
							return type3;
						}
					}
				}
				type = type.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Localiza a propriedade ou o campo.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public static MemberInfo FindPropertyOrField(this Type type, string memberName)
		{
			MemberInfo info = type.FindPropertyOrField(memberName, false);
			if(info == null)
			{
				info = type.FindPropertyOrField(memberName, true);
			}
			return info;
		}

		/// <summary>
		/// Localiza a propriedade ou o campo.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberName"></param>
		/// <param name="staticAccess"></param>
		/// <returns></returns>
		public static MemberInfo FindPropertyOrField(this Type type, string memberName, bool staticAccess)
		{
			BindingFlags bindingAttr = (BindingFlags.Public | BindingFlags.DeclaredOnly) | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
			foreach (Type type2 in type.SelfAndBaseTypes())
			{
				MemberInfo[] infoArray = type2.FindMembers(MemberTypes.Property | MemberTypes.Field, bindingAttr, Type.FilterNameIgnoreCase, memberName);
				if(infoArray.Length != 0)
				{
					return infoArray[0];
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera a primeira propriedade ordenada.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string FirstSortableProperty(this Type type)
		{
			PropertyInfo info = (from property in type.GetProperties()
			where property.PropertyType.IsPredefinedType()
			select property).FirstOrDefault<PropertyInfo>();
			if(info == null)
			{
				throw new NotSupportedException("CannotFindPropertyToSortBy");
			}
			return info.Name;
		}

		/// <summary>
		/// Recupera as informação da propridade indexadora.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexerArguments"></param>
		/// <returns></returns>
		public static PropertyInfo GetIndexerPropertyInfo(this Type type, params Type[] indexerArguments)
		{
			return (from p in type.GetProperties()
			where AreArgumentsApplicable(indexerArguments, p.GetIndexParameters())
			select p).FirstOrDefault<PropertyInfo>();
		}

		/// <summary>
		/// Recupera o nome do tipo.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetName(this Type type)
		{
			return type.FullName.Replace(type.Namespace + ".", "");
		}

		/// <summary>
		/// Recupera o tipo não nullable.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetNonNullableType(this Type type)
		{
			if(!type.IsNullableType())
				return type;
			return type.GetGenericArguments()[0];
		}

		/// <summary>
		/// Tipo o tipo numérico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static int GetNumericTypeKind(this Type type)
		{
			if(type != null)
			{
				type = type.GetNonNullableType();
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
				}
			}
			return 0;
		}

		/// <summary>
		/// Recupera o nome do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetTypeName(this Type type)
		{
			Type nonNullableType = type.GetNonNullableType();
			string name = nonNullableType.Name;
			if(type != nonNullableType)
			{
				name = name + '?';
			}
			return name;
		}

		/// <summary>
		/// Verifica se os tipos são compatíveis.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static bool IsCompatibleWith(this Type source, Type target)
		{
			if(source == target)
			{
				return true;
			}
			if(!target.IsValueType)
			{
				return target.IsAssignableFrom(source);
			}
			Type nonNullableType = source.GetNonNullableType();
			Type type = target.GetNonNullableType();
			if((nonNullableType == source) || (type != target))
			{
				TypeCode code = nonNullableType.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType);
				TypeCode code2 = type.IsEnum ? TypeCode.Object : Type.GetTypeCode(type);
				switch(code)
				{
				case TypeCode.SByte:
					switch(code2)
					{
					case TypeCode.SByte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.Byte:
					switch(code2)
					{
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.Int16:
					switch(code2)
					{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.UInt16:
					switch(code2)
					{
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.Int32:
					switch(code2)
					{
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.UInt32:
					switch(code2)
					{
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.Int64:
					switch(code2)
					{
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.UInt64:
					switch(code2)
					{
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return true;
					}
					break;
				case TypeCode.Single:
					switch(code2)
					{
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					}
					break;
				default:
					if(nonNullableType == type)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		/// <summary>
		/// Verifica se o tipo é uma linha de dados.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDataRow(this Type type)
		{
			if(!type.IsCompatibleWith(typeof(System.Data.DataRow)))
			{
				return type.IsCompatibleWith(typeof(System.Data.DataRowView));
			}
			return true;
		}

		/// <summary>
		/// Verifica se o tipo é um DateTime.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDateTime(this Type type)
		{
			if(!(type == typeof(DateTime)))
			{
				return (type == typeof(DateTime?));
			}
			return true;
		}

		/// <summary>
		/// Verifica se o tipo é um objeto dinamico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsDynamicObject(this Type type)
		{
			if(!(type == typeof(object)))
			{
				return type.IsCompatibleWith(typeof(System.Dynamic.IDynamicMetaObjectProvider));
			}
			return true;
		}

		/// <summary>
		/// Verifica se o tipo é um Enum.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsEnumType(this Type type)
		{
			return type.GetNonNullableType().IsEnum;
		}

		/// <summary>
		/// Verifica se o tipo é nullable.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNullableType(this Type type)
		{
			return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
		}

		/// <summary>
		/// Verifica se é um tipo numérico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNumericType(this Type type)
		{
			return (type.GetNumericTypeKind() != 0);
		}

		/// <summary>
		/// Verifica se é um PlainType.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsPlainType(this Type type)
		{
			return ((!type.IsDynamicObject() && !type.IsDataRow()) && !type.IsCompatibleWith(typeof(System.ComponentModel.ICustomTypeDescriptor)));
		}

		/// <summary>
		/// Verifica se é um tipo predefinido.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsPredefinedType(this Type type)
		{
			foreach (Type type2 in PredefinedTypes)
			{
				if(type2 == type)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Verifica se é um tipo integra assinado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsSignedIntegralType(this Type type)
		{
			return (type.GetNumericTypeKind() == 2);
		}

		/// <summary>
		/// Verifica se é um tipo integral não assinado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsUnsignedIntegralType(this Type type)
		{
			return (type.GetNumericTypeKind() == 3);
		}

		/// <summary>
		/// Recupera o tipo e as classes base.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<Type> SelfAndBaseClasses(this Type type)
		{
			while (true)
			{
				if(type == null)
				{
					yield break;
				}
				yield return type;
				type = type.BaseType;
			}
		}

		/// <summary>
		/// Recupera o tipo e os tipos base.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<Type> SelfAndBaseTypes(this Type type)
		{
			if(type.IsInterface)
			{
				List<Type> types = new List<Type>();
				AddInterface(types, type);
				return types;
			}
			return type.SelfAndBaseClasses();
		}

		/// <summary>
		/// Recupera o tipo javascript equivalente.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToJavaScriptType(this Type type)
		{
			if(type != null)
			{
				if((type == typeof(char)) || (type == typeof(char?)))
				{
					return "String";
				}
				if(type.IsNumericType())
				{
					return "Number";
				}
				if((type == typeof(DateTime)) || (type == typeof(DateTime?)))
				{
					return "Date";
				}
				if(type == typeof(string))
				{
					return "String";
				}
				if((type == typeof(bool)) || (type == typeof(bool?)))
				{
					return "Boolean";
				}
				if(type.GetNonNullableType().IsEnum)
				{
					return "Number";
				}
				if(type.GetNonNullableType() == typeof(Guid))
				{
					return "String";
				}
			}
			return "Object";
		}
	}
}
