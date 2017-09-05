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
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class ColumnFormatterCollection : BaseItemCollection<Column, ColumnFormatter>
	{
		private static readonly Type[] _knownTypes = new Type[] {
			typeof(IntegerFormatter),
			typeof(LinkFormatter),
			typeof(EmailFormatter),
			typeof(CheckBoxFormatter),
			typeof(CurrencyFormatter),
			typeof(NumberFormatter),
			typeof(CustomFormatter)
		};

		protected override object CreateKnownType(int index)
		{
			switch(index)
			{
			case 0:
				return new IntegerFormatter();
			case 1:
				return new LinkFormatter();
			case 2:
				return new EmailFormatter();
			case 3:
				return new CheckBoxFormatter();
			case 4:
				return new CurrencyFormatter();
			case 5:
				return new NumberFormatter();
			case 6:
				return new CustomFormatter();
			}
			throw new ArgumentOutOfRangeException("index");
		}

		protected override Type[] GetKnownTypes()
		{
			return _knownTypes;
		}
	}
}
