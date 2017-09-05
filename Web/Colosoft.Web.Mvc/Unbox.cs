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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Classe com métodos para tratar unbox do tipo.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	static class UnboxT<T>
	{
		public static readonly Converter<object, T> Unbox;

		/// <summary>
		/// Construtor para o tipo T para o conversor.
		/// </summary>
		static UnboxT()
		{
			UnboxT<T>.Unbox = UnboxT<T>.Create(typeof(T));
		}

		/// <summary>
		/// Recupera o conversor do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Converter<object, T> Create(Type type)
		{
			if(!type.IsValueType)
			{
				return new Converter<object, T>(UnboxT<T>.ReferenceField);
			}
			if((type.IsGenericType && !type.IsGenericTypeDefinition) && (typeof(Nullable<>) == type.GetGenericTypeDefinition()))
			{
				var method = typeof(UnboxT<T>).GetMethod("NullableField", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type.GetGenericArguments()[0]);
				return (Converter<object, T>)Delegate.CreateDelegate(typeof(Converter<object, T>), method);
			}
			return new Converter<object, T>(UnboxT<T>.ValueField);
		}

		/// <summary>
		/// Recupera o campo nullable para o valor informado.
		/// </summary>
		/// <typeparam name="TElem"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		private static TElem? NullableField<TElem>(object value) where TElem : struct
		{
			if(DBNull.Value == value)
				return null;
			return (TElem?)value;
		}

		/// <summary>
		/// Recupera a referencia do campo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static T ReferenceField(object value)
		{
			if(DBNull.Value != value)
			{
				return (T)value;
			}
			return default(T);
		}

		/// <summary>
		/// Recupera o valor do campo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static T ValueField(object value)
		{
			if(DBNull.Value == value)
			{
				throw new InvalidCastException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Type: {0} cannot be casted to Nullable type", typeof(T)));
			}
			return (T)value;
		}
	}
}
