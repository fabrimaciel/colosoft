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
using System.Text;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Representa uma coleção de colunas.
	/// </summary>
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class ColumnCollection : BaseItemCollection<GridView, Column>
	{
		protected override object CreateKnownType(int index)
		{
			return new Column();
		}

		/// <summary>
		/// Recupera a coluna com base no campo de dados.
		/// </summary>
		/// <param name="dataField"></param>
		/// <returns></returns>
		public Column FromDataField(string dataField)
		{
			for(int i = 0; i < base.Count; i++)
			{
				var column = base[i];
				if(column.QualifiedName == dataField)
					return column;
			}
			return null;
		}
	}
}
