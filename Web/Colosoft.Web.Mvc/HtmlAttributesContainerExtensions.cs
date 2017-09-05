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

using Colosoft.Web.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com métodos para auxiliar na manipulação de atributos html.
	/// </summary>
	public static class HtmlAttributesContainerExtensions
	{
		/// <summary>
		/// Anexa uma class css no container.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="class"></param>
		public static void AppendCssClass(this IHtmlAttributesContainer container, string @class)
		{
			container.HtmlAttributes.AppendInValue("class", " ", @class);
		}

		/// <summary>
		/// PrependCssClass
		/// </summary>
		/// <param name="container"></param>
		/// <param name="class"></param>
		public static void PrependCssClass(this IHtmlAttributesContainer container, string @class)
		{
			container.HtmlAttributes.PrependInValue("class", " ", @class);
		}

		/// <summary>
		/// Dispara uma exception caso a classe não esteja presente.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="class"></param>
		/// <param name="message"></param>
		public static void ThrowIfClassIsPresent(this IHtmlAttributesContainer container, string @class, string message)
		{
			object value;
			if(container.HtmlAttributes.TryGetValue("class", out value))
			{
				if(value != null)
				{
					var classes = value.ToString().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if(Array.IndexOf(classes, @class) > -1)
					{
						throw new NotSupportedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, message, @class));
					}
				}
			}
		}
	}
}
