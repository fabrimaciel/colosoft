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

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Representa uma coleção de campos de grupo.
	/// </summary>
	[System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal), System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal)]
	public class GroupFieldCollection : BaseItemCollection<GridView, GroupField>
	{
		/// <summary>
		/// Cria um tipo conhecido.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override object CreateKnownType(int index)
		{
			return new GroupField();
		}
	}
}
