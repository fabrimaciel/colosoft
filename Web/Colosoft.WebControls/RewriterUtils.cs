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
	/// Provides utility helper methods for the rewriting HttpModule and HttpHandler.
	/// </summary>
	/// <remarks>This class is marked as internal, meaning only classes in the same assembly will be
	/// able to access its methods.</remarks>
	public class RewriterUtils
	{
		/// <summary>
		/// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
		/// </summary>
		/// <param name="context">The HttpContext object to rewrite the URL to.</param>
		/// <param name="sendToUrl">The URL to rewrite to.</param>
		public static void RewriteUrl(HttpContext context, string sendToUrl)
		{
			string x, y;
			RewriteUrl(context, sendToUrl, out x, out y);
		}

		/// <summary>
		/// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
		/// </summary>
		/// <param name="context">The HttpContext object to rewrite the URL to.</param>
		/// <param name="sendToUrl">The URL to rewrite to.</param>
		/// <param name="sendToUrlLessQString">Returns the value of sendToUrl stripped of the querystring.</param>
		/// <param name="filePath">Returns the physical file path to the requested page.</param>
		public static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
		{
			if(context.Request.QueryString.Count > 0)
			{
				if(sendToUrl.IndexOf('?') != -1)
					sendToUrl += "&" + context.Request.QueryString.ToString();
				else
					sendToUrl += "?" + context.Request.QueryString.ToString();
			}
			string queryString = String.Empty;
			sendToUrlLessQString = sendToUrl;
			if(sendToUrl.IndexOf('?') > 0)
			{
				sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
				queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
			}
			filePath = string.Empty;
			filePath = context.Server.MapPath(sendToUrlLessQString);
			context.RewritePath(sendToUrlLessQString, String.Empty, queryString);
		}

		/// <summary>
		/// Libera a somente leitura da lista de QueryString.
		/// </summary>
		/// <param name="collection"></param>
		public static void MakeReadWriteCollection(System.Collections.Specialized.NameValueCollection collection)
		{
			if(collection == null)
				throw new ArgumentNullException("collection");
			System.Reflection.MethodInfo mi = collection.GetType().GetMethod("MakeReadWrite", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			mi.Invoke(collection, null);
		}

		/// <summary>
		/// Marca somente leitura para a lista de QueryString.
		/// </summary>
		/// <param name="collection"></param>
		public static void MakeReadOnlyCollection(System.Collections.Specialized.NameValueCollection collection)
		{
			if(collection == null)
				throw new ArgumentNullException("collection");
			System.Reflection.MethodInfo mi = collection.GetType().GetMethod("MakeReadOnly", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			mi.Invoke(collection, null);
		}

		/// <summary>
		/// Converts a URL into one that is usable on the requesting client.
		/// </summary>
		/// <remarks>Converts ~ to the requesting application path.  Mimics the behavior of the 
		/// <b>Control.ResolveUrl()</b> method, which is often used by control developers.</remarks>
		/// <param name="appPath">The application path.</param>
		/// <param name="url">The URL, which might contain ~.</param>
		/// <returns>A resolved URL.  If the input parameter <b>url</b> contains ~, it is replaced with the
		/// value of the <b>appPath</b> parameter.</returns>
		public static string ResolveUrl(string appPath, string url)
		{
			if(url.Length == 0 || url[0] != '~')
				return url;
			else
			{
				if(url.Length == 1)
					return appPath;
				if(url[1] == '/' || url[1] == '\\')
				{
					if(appPath.Length > 1)
						return appPath + "/" + url.Substring(2);
					else
						return "/" + url.Substring(2);
				}
				else
				{
					if(appPath.Length > 1)
						return appPath + "/" + url.Substring(1);
					else
						return appPath + url.Substring(1);
				}
			}
		}
	}
}
