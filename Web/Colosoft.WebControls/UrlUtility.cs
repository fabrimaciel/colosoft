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

namespace Colosoft.WebControls
{
	/// <summary>
	/// Classe com métodos que auxiliam na manipulação de urls.
	/// </summary>
	public class UrlUtility
	{
		/// <summary>
		/// Recupera o nome do servidor de onde está sendo feita a requisição.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string GetUrlServerName(HttpContext context)
		{
			return context.Request.Url.AbsoluteUri.Substring(0, context.Request.Url.AbsoluteUri.Length - context.Request.RawUrl.Length);
		}

		/// <summary>
		/// Extrai a url virtual.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string ExtractVirtualUrl(string url)
		{
			return ExtractVirtualUrl(url, HttpContext.Current);
		}

		/// <summary>
		/// Extrai a url virtual.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string ExtractVirtualUrl(string url, HttpContext context)
		{
			if(!string.IsNullOrEmpty(url))
			{
				string urlServer = GetUrlServerName(context);
				if(url.IndexOf(urlServer) != 0)
					return url;
				int pos = url.IndexOf("://");
				if(pos >= 0)
				{
					pos += 3;
					pos = url.IndexOf('/', pos);
					url = url.Substring(pos + 1);
					string[] parts = url.Split('/');
					string appPath = context.Request.ApplicationPath.Substring(1);
					for(int i = 0; i < parts.Length; i++)
					{
						if(string.Compare(appPath, parts[i], true) == 0)
						{
							url = "~/";
							for(int j = i + 1; j < parts.Length; j++)
								url += parts[j] + ((j + 1) < parts.Length ? "/" : "");
							break;
						}
					}
				}
			}
			return url;
		}
	}
}
