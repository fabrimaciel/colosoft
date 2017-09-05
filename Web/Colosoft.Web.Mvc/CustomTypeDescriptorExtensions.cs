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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Classe com métodos de extensão para tratar o descritor de tipos customizados.
	/// </summary>
	static class CustomTypeDescriptorExtensions
	{
		/// <summary>
		/// Recupera a propriedade pelo nome informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeDescriptor"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static T Property<T>(this System.ComponentModel.ICustomTypeDescriptor typeDescriptor, string propertyName)
		{
			System.ComponentModel.PropertyDescriptor descriptor = System.ComponentModel.TypeDescriptor.GetProperties(typeDescriptor)[propertyName];
			if(descriptor == null)
			{
				throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Property with specified name: {0} cannot be found on type: {1}", propertyName, typeDescriptor.GetType()), "propertyName");
			}
			return UnboxT<T>.Unbox(descriptor.GetValue(typeDescriptor));
		}
	}
}
