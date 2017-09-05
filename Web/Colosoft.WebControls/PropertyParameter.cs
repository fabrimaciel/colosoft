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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa o parametro da propriedade de um controle.
	/// </summary>
	public class PropertyParameter : Parameter
	{
		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		protected override object Evaluate(HttpContext context, Control control)
		{
			Control propertyControl = GetContainingControl(control);
			if(propertyControl != null)
			{
				return DataBinder.Eval(propertyControl, PropertyName);
			}
			else
			{
				throw new ApplicationException(string.Format("Unable to find property: {0}", PropertyName));
			}
		}

		private Control GetContainingControl(Control control)
		{
			var propertyInfo = control.GetType().GetProperty(PropertyName);
			if(propertyInfo != null)
			{
				if(System.Type.GetTypeCode(propertyInfo.PropertyType).Equals(Type))
				{
					return control;
				}
			}
			if(control.Parent == null)
			{
				return null;
			}
			else
			{
				return GetContainingControl(control.Parent);
			}
		}
	}
}
