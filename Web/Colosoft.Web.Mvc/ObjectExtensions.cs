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

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com métodos de extensão para objetos.
	/// </summary>
	internal static class ObjectExtensions
	{
		/// <summary>
		/// Converte o obketo de uma  dicionário de propriedade valor.
		/// </summary>
		/// <param name="object"></param>
		/// <returns></returns>
		public static IDictionary<string, object> ToDictionary(this object @object)
		{
			var dictionary = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
			if(@object != null)
			{
				foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(@object))
				{
					dictionary.Add(property.Name.Replace("_", "-"), property.GetValue(@object));
				}
			}
			return dictionary;
		}
	}
}
