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
	/// Implementação de uma comparador para propriedade de um tipo.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PropertiesComparer<T> : IComparer<T>
	{
		private PropertyComparer[] _comparers;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties">Propriedades do comparador.</param>
		public PropertiesComparer(System.Linq.Expressions.Expression<Func<T, object>>[] properties)
		{
			properties.Require("properties").NotNull();
			_comparers = properties.Select(f => new PropertyComparer((System.Reflection.PropertyInfo)f.GetMember(), f.Compile())).ToArray();
		}

		/// <summary>
		/// Cria o comparador com os nomes das propriedades informadas.
		/// </summary>
		/// <param name="propertyNames"></param>
		public PropertiesComparer(string[] propertyNames)
		{
			propertyNames.Require("propertyNames").NotNull().NotEmptyCollection();
			var type = typeof(T);
			_comparers = propertyNames.Select(f => type.GetProperty(f)).Where(f => f != null).Select(f => new PropertyComparer(f, CreateGetter(f))).ToArray();
		}

		/// <summary>
		/// Recupera um getter para a propriedade.
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <returns></returns>
		private Func<T, object> CreateGetter(System.Reflection.PropertyInfo propertyInfo)
		{
			var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "f");
			var expr = System.Linq.Expressions.Expression.Property(parameter, propertyInfo.Name);
			return System.Linq.Expressions.Expression.Lambda<Func<T, object>>(expr, parameter).Compile();
		}

		/// <summary>
		/// Compara as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(T x, T y)
		{
			foreach (var comparer in _comparers)
			{
				var result = comparer.Compare(x, y);
				if(result != 0)
					return result;
			}
			return 0;
		}

		class PropertyComparer : IComparer<T>
		{
			private System.Collections.IComparer _comparer;

			private Func<T, object> _getter;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="propertyInfo"></param>
			/// <param name="getter"></param>
			public PropertyComparer(System.Reflection.PropertyInfo propertyInfo, Func<T, object> getter)
			{
				propertyInfo.Require("propertyInfo").NotNull();
				_comparer = typeof(Comparer<>).MakeGenericType(propertyInfo.PropertyType).GetProperty("Default", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as System.Collections.IComparer;
				_getter = getter;
			}

			/// <summary>
			/// Compara as instancias informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(T x, T y)
			{
				if(x != null && y != null)
				{
					var x1 = _getter(x);
					var y1 = _getter(y);
					return _comparer.Compare(x1, y1);
				}
				return x != null && y == null ? 1 : y != null && x == null ? -1 : 0;
			}
		}
	}
}
