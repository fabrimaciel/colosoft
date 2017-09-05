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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Representa uma classe dinâmica.
	/// </summary>
	public abstract class DynamicClass
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected DynamicClass()
		{
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var properties = base.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			StringBuilder builder = new StringBuilder();
			builder.Append("{");
			for(int i = 0; i < properties.Length; i++)
			{
				if(i > 0)
				{
					builder.Append(", ");
				}
				builder.Append(properties[i].Name);
				builder.Append("=");
				builder.Append(properties[i].GetValue(this, null));
			}
			builder.Append("}");
			return builder.ToString();
		}
	}
}
