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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Serializador do descritor da grid.
	/// </summary>
	public class GridDescriptorSerializer
	{
		/// <summary>
		/// Deserializa os dados recebidos.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="from"></param>
		/// <returns></returns>
		public static IList<T> Deserialize<T>(string from) where T : IDescriptor, new()
		{
			List<T> list = new List<T>();
			if(from.HasValue())
			{
				foreach (string str in from.Split("~".ToArray(), StringSplitOptions.RemoveEmptyEntries))
				{
					T local2 = default(T);
					T item = (local2 == null) ? Activator.CreateInstance<T>() : default(T);
					item.Deserialize(str);
					list.Add(item);
				}
			}
			return list;
		}

		/// <summary>
		/// Serializa os descritores informados.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="descriptors"></param>
		/// <returns></returns>
		public static string Serialize<T>(IEnumerable<T> descriptors) where T : IDescriptor
		{
			if(!descriptors.Any<T>())
				return "~";
			string[] items = descriptors.Select(f => f.Serialize()).ToArray();
			return string.Join("~", items);
		}
	}
}
