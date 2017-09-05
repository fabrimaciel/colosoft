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

namespace Colosoft.WebControls.SiteMap
{
	public enum Frenquently
	{
		Always,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
		Never
	}
	/// <summary>
	/// Item do mapa do site.
	/// </summary>
	public class SiteMapItem
	{
		private string _localization;

		private DateTime? _lastModification;

		private Frenquently? _changeFrequently;

		private float _priority = 0.5f;

		/// <summary>
		/// URL of the page. This URL must begin with the protocol (such as http) and end with a trailing slash, 
		/// if your web server requires it. This value must be less than 2,048 characters.
		/// </summary>
		public string Localization
		{
			get
			{
				return _localization;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Localization");
				_localization = value;
			}
		}

		/// <summary>
		/// The date of last modification of the file. This date should be in  W3C Datetime format. 
		/// This format allows you to omit the time portion, if desired, and use YYYY-MM-DD.
		/// Note that this tag is separate from the If-Modified-Since (304) header the server can 
		/// return, and search engines may use the information from both sources differently.
		/// </summary>
		public DateTime? LastModification
		{
			get
			{
				return _lastModification;
			}
			set
			{
				_lastModification = value;
			}
		}

		/// <summary>
		/// How frequently the page is likely to change. This value provides general 
		/// information to search engines and may not correlate exactly to how often they crawl the page.
		/// </summary>
		public Frenquently? ChangeFrequently
		{
			get
			{
				return _changeFrequently;
			}
			set
			{
				_changeFrequently = value;
			}
		}

		/// <summary>
		/// The priority of this URL relative to other URLs on your site. 
		/// Valid values range from 0.0 to 1.0. This value does not affect 
		/// how your pages are compared to pages on other sites—it only lets 
		/// the search engines know which pages you deem most important for the crawlers.
		/// </summary>
		public float Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}

		public SiteMapItem(string localization)
		{
			Localization = localization;
		}
	}
}
