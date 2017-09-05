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
using System.Web.UI.WebControls;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa o parametro do item de dados associado a página.
	/// </summary>
	public class PageDataItemParameter : Parameter
	{
		/// <summary>
		/// Nome da propriedade do item de dados.
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o valor.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		protected override object Evaluate(System.Web.HttpContext context, System.Web.UI.Control control)
		{
			return System.Web.UI.DataBinder.Eval(control.Page.GetDataItem(), PropertyName);
		}
	}
}
