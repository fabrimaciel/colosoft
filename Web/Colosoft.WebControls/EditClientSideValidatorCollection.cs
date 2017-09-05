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
	public class EditClientSideValidatorCollection : BaseItemCollection<Column, EditClientSideValidator>
	{
		private static readonly Type[] _knownTypes = new Type[] {
			typeof(DateValidator),
			typeof(CustomValidator),
			typeof(IntegerValidator),
			typeof(MaxValueValidator),
			typeof(MinValueValidator),
			typeof(NumberValidator),
			typeof(RequiredValidator),
			typeof(TimeValidator),
			typeof(UrlValidator),
			typeof(CustomValidator)
		};

		protected override object CreateKnownType(int index)
		{
			switch(index)
			{
			case 0:
				return new DateValidator();
			case 1:
				return new CustomValidator();
			case 2:
				return new IntegerValidator();
			case 3:
				return new MaxValueValidator();
			case 4:
				return new MinValueValidator();
			case 5:
				return new NumberValidator();
			case 6:
				return new RequiredValidator();
			case 7:
				return new TimeValidator();
			case 8:
				return new UrlValidator();
			case 9:
				return new CustomValidator();
			}
			throw new ArgumentOutOfRangeException("index");
		}

		/// <summary>
		/// Recupera os tipos conhecidos.
		/// </summary>
		/// <returns></returns>
		protected override Type[] GetKnownTypes()
		{
			return _knownTypes;
		}
	}
}
