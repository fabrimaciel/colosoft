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

namespace Colosoft.WebControls.Route
{
	/// <summary>
	/// The SSLHelper class provides static methods for ensuring that a page is rendered 
	/// securely via SSL or unsecurely.
	/// </summary>
	public class SSLHelper
	{
		/// <summary>
		/// Intializes an instance of this class.
		/// </summary>
		public SSLHelper()
		{
		}

		/// <summary>
		/// Requests the current page over a secure connection, if it is not already.
		/// </summary>
		public static void RequestSecurePage(HttpContext context)
		{
			string RequestPath = context.Request.Url.ToString();
			if(RequestPath.StartsWith("http://"))
			{
				RequestPath = RequestPath.Replace("http://", "https://");
				context.Response.Redirect(RequestPath, true);
			}
		}

		/// <summary>
		/// Requests the current page over an unsecure connection, if it is not already.
		/// </summary>
		public static void RequestUnsecurePage(HttpContext context)
		{
			string RequestPath = context.Request.Url.ToString();
			if(RequestPath.StartsWith("https://"))
			{
				RequestPath = RequestPath.Replace("https://", "http://");
				context.Response.Redirect(RequestPath, true);
			}
		}
	}
}
