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
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Colosoft.WebControls.Security
{
	/// <summary>
	/// Summary description for ProviderUtil
	/// </summary>
	internal static class SecurityUtil
	{
		public static readonly string DefaultFolder = "~/App_Data/";

		/// <summary>
		/// 
		/// </summary>
		public static void EnsureDataFoler()
		{
			if(HttpContext.Current != null)
			{
				string folder = HttpContext.Current.Server.MapPath("~/App_Data/");
				if(!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}
			}
		}

		/// <summary>
		/// Gets the config value.
		/// </summary>
		/// <param name="configValue">The config value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public static string GetConfigValue(string configValue, string defaultValue)
		{
			return (String.IsNullOrEmpty(configValue)) ? defaultValue : configValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public static string MapPath(string virtualPath)
		{
			return HostingEnvironment.MapPath(virtualPath);
		}
	}
}
