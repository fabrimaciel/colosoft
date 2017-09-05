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

namespace Colosoft
{
	/// <summary>
	/// Extensões para objetos genéricos.
	/// </summary>
	public static class ObjectExtensions
	{
		///// <summary>
		///// Recupera as informações de acesso da propriedade ou campo (públicos) solicitados.
		///// </summary>
		///// <param name="instance"></param>
		///// <param name="propertyPath"></param>
		///// <returns></returns>
		/// <summary>
		/// Recupera as informações de acesso da propriedade ou campo (públicos) solicitados.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyPath"></param>
		/// <returns></returns>
		private static Tuple<object, Type, MemberInfo> GetMemberInfo(object instance, string propertyPath)
		{
			if((instance == null) || String.IsNullOrEmpty(propertyPath))
			{
				return null;
			}
			var parts = propertyPath.Split('.');
			var index = 0;
			var size = parts.Length;
			var result = Tuple.Create<object, Type, MemberInfo>(instance, instance.GetType(), null);
			var name = String.Empty;
			MemberInfo pInfo = null;
			while (index < size)
			{
				name = parts[index++];
				pInfo = result.Item2.GetProperty(name);
				if(pInfo != null)
				{
					var asP = (PropertyInfo)pInfo;
					var source = (result.Item3 == null) ? instance : GetMemberValue(result.Item1, result.Item3);
					result = Tuple.Create<object, Type, MemberInfo>(source, asP.PropertyType, asP);
					continue;
				}
				pInfo = result.Item2.GetField(name);
				if(pInfo != null)
				{
					var asF = (FieldInfo)pInfo;
					var source = (result.Item3 == null) ? instance : GetMemberValue(result.Item1, result.Item3);
					result = Tuple.Create<object, Type, MemberInfo>(source, asF.FieldType, asF);
					continue;
				}
				else
				{
					return null;
				}
			}
			return result;
		}

		/// <summary>
		/// Recupera o tipo da propriedade ou campo solicitados.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private static Type GetMemberType(MemberInfo info)
		{
			if(info == null)
			{
				return null;
			}
			PropertyInfo asP;
			FieldInfo asF;
			if(info.TryCastAs<PropertyInfo>(out asP))
			{
				return asP.PropertyType;
			}
			else if(info.TryCastAs<FieldInfo>(out asF))
			{
				return asF.FieldType;
			}
			return null;
		}

		/// <summary>
		/// Recupera o valor do membro (público) definido pelo parâmetro.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="info"></param>
		/// <returns></returns>
		private static object GetMemberValue(object instance, MemberInfo info)
		{
			if((instance == null) || (info == null))
			{
				return null;
			}
			PropertyInfo asP;
			FieldInfo asF;
			if(info.TryCastAs<PropertyInfo>(out asP))
			{
				return asP.GetValue(instance, null);
			}
			else if(info.TryCastAs<FieldInfo>(out asF))
			{
				return asF.GetValue(instance);
			}
			return null;
		}

		/// <summary>
		/// Define o valor do membro (público) definido pelo parâmetro.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="info"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool SetMemberValue(object instance, MemberInfo info, object value)
		{
			if((instance == null) || (info == null))
			{
				return false;
			}
			PropertyInfo asP;
			FieldInfo asF;
			if(info.TryCastAs<PropertyInfo>(out asP))
			{
				asP.SetValue(instance, value, null);
				return true;
			}
			else if(info.TryCastAs<FieldInfo>(out asF))
			{
				asF.SetValue(instance, value);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Recupera o tipo do membro (público) solicitado.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyPath"></param>
		/// <returns></returns>
		public static Type GetMemberType(this object instance, string propertyPath)
		{
			if((instance == null) || String.IsNullOrEmpty(propertyPath))
			{
				return null;
			}
			var info = GetMemberInfo(instance, propertyPath);
			return (info != null) ? GetMemberType(info.Item3) : null;
		}

		/// <summary>
		/// Recupera o valor do membro (público) solicitado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyPath"></param>
		/// <returns></returns>
		public static T GetMemberValue<T>(this object instance, string propertyPath)
		{
			var info = GetMemberInfo(instance, propertyPath);
			var obj = (info != null) && (info.Item1 != null) ? GetMemberValue(info.Item1, info.Item3) : null;
			return (obj != null) && (obj is T) ? (T)obj : default(T);
		}

		/// <summary>
		/// Recupera o valor do membro (público) solicitado.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyPath"></param>
		/// <returns></returns>
		public static object GetMemberValue(this object instance, string propertyPath)
		{
			var info = GetMemberInfo(instance, propertyPath);
			return (info != null) && (info.Item1 != null) ? GetMemberValue(info.Item1, info.Item3) : null;
		}

		/// <summary>
		/// Define o valor do membro (público) solicitado.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyPath"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool SetMemberValue(this object instance, string propertyPath, object value)
		{
			var info = GetMemberInfo(instance, propertyPath);
			if((info == null) || (info.Item1 == null))
			{
				return false;
			}
			return SetMemberValue(info.Item1, info.Item3, value);
		}

		/// <summary>
		/// Tenta converter o objeto para o tipo especificado.
		/// Atribui o valor default fornecido em caso de falha.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="result"></param>
		/// <param name="stdVal"></param>
		/// <returns></returns>
		public static bool TryCastAs<T>(this object instance, out T result, T stdVal = null) where T : class
		{
			var valid = (instance != null);
			result = valid ? (instance as T) : stdVal;
			return valid && (result != null);
		}

		/// <summary>
		/// Tenta converter o valor para o tipo especificado.
		/// Atribui o valor default especificado em caso de falha.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="result"></param>
		/// <param name="stdVal"></param>
		/// <returns></returns>
		public static bool TryConvertAs<T>(this object instance, out T result, T stdVal = default(T)) where T : struct
		{
			var isValid = (instance != null) && (instance is T);
			result = isValid ? (T)instance : stdVal;
			return isValid;
		}
	}
}
