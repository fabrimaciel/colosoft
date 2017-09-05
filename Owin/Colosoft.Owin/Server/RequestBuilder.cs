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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Classe com métodos para a construção de requisições.
	/// </summary>
	static class RequestBuilder
	{
		public static Microsoft.Owin.OwinRequest CreateOwinRequest()
		{
			return CreateOwinRequest(new System.Collections.Concurrent.ConcurrentDictionary<string, object>(StringComparer.Ordinal));
		}

		public static Microsoft.Owin.OwinRequest CreateOwinRequest(IDictionary<string, object> environment)
		{
			environment[Types.OwinConstants.RequestHeaders] = new System.Collections.Concurrent.ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
			environment[Types.OwinConstants.ResponseHeaders] = new System.Collections.Concurrent.ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
			return new Microsoft.Owin.OwinRequest(environment);
		}

		public static IDictionary<string, object> FromRaw(string raw)
		{
			var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(raw));
			var request = CreateOwinRequest();
			Http.HttpParser.ParseRequest(stream, (method, path, protocol) =>  {
				request.Method = method;
				var uri = new Uri(path);
				request.Protocol = protocol;
				BuildRequestFromUri(request, uri);
			}, (key, value) =>  {
				request.Headers.Set(key, value);
			});
			request.Body = stream;
			return request.Environment;
		}

		public static void BuildGet(IDictionary<string, object> environment, string url)
		{
			var uri = new Uri(url);
			var request = CreateOwinRequest(environment);
			request.Protocol = "HTTP/1.1";
			request.Method = "GET";
			BuildRequestFromUri(request, uri);
		}

		public static IDictionary<string, object> Get(string url)
		{
			if(url == null)
			{
				throw new ArgumentNullException("url");
			}
			var request = CreateRequest(url, "GET");
			return request.Environment;
		}

		public static IDictionary<string, object> Post(string url)
		{
			var request = CreateRequest(url, "POST");
			return request.Environment;
		}

		private static Microsoft.Owin.OwinRequest CreateRequest(string url, string method)
		{
			var uri = new Uri(url);
			var request = CreateOwinRequest();
			request.Protocol = "HTTP/1.1";
			request.Method = method;
			var response = new Microsoft.Owin.OwinResponse(request.Environment);
			response.Body = new System.IO.MemoryStream();
			BuildRequestFromUri(request, uri);
			return request;
		}

		private static void BuildRequestFromUri(Microsoft.Owin.OwinRequest request, Uri uri)
		{
			request.Host = new Microsoft.Owin.HostString(uri.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped));
			request.PathBase = new Microsoft.Owin.PathString(String.Empty);
			request.Path = new Microsoft.Owin.PathString(uri.LocalPath);
			request.Scheme = uri.Scheme;
			request.QueryString = new Microsoft.Owin.QueryString(uri.Query.Length > 0 ? uri.Query.Substring(1) : String.Empty);
		}
	}
}
