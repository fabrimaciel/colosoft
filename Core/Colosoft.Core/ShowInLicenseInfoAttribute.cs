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

namespace Colosoft.Licensing
{
	/// <summary>
	/// This attribute defines whether the property of LicenseEntity object will be shown in LicenseInfoControl
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ShowInLicenseInfoAttribute : Attribute
	{
		public enum FormatType
		{
			String,
			Date,
			DateTime,
			EnumDescription,
		}

		protected bool _showInLicenseInfo = true;

		protected string _displayAs = string.Empty;

		protected FormatType _formatType = FormatType.String;

		public ShowInLicenseInfoAttribute()
		{
		}

		public ShowInLicenseInfoAttribute(bool showInLicenseInfo)
		{
			if(showInLicenseInfo)
			{
				throw new Exception("When ShowInLicenseInfo is True, DisplayAs MUST have a value");
			}
			_showInLicenseInfo = showInLicenseInfo;
		}

		public ShowInLicenseInfoAttribute(bool showInLicenseInfo, string displayAs)
		{
			_showInLicenseInfo = showInLicenseInfo;
			_displayAs = displayAs;
		}

		public ShowInLicenseInfoAttribute(bool showInLicenseInfo, string displayAs, FormatType dataFormatType)
		{
			_showInLicenseInfo = showInLicenseInfo;
			_displayAs = displayAs;
			_formatType = dataFormatType;
		}

		public bool ShowInLicenseInfo
		{
			get
			{
				return _showInLicenseInfo;
			}
		}

		public string DisplayAs
		{
			get
			{
				return _displayAs;
			}
		}

		public FormatType DataFormatType
		{
			get
			{
				return _formatType;
			}
		}
	}
}
