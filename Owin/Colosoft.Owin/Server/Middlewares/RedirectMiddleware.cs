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

namespace Colosoft.Owin.Server.Middlewares
{
	/// <summary>
	/// Implementação do middleware para redirecionamento.
	/// </summary>
	class RedirectMiddleware
	{
		private readonly Func<IDictionary<string, object>, Task> _next;

		private readonly int _maxRedirects;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="next"></param>
		/// <param name="maxRedirects"></param>
		public RedirectMiddleware(Func<IDictionary<string, object>, Task> next, int maxRedirects)
		{
			_next = next;
			_maxRedirects = maxRedirects;
		}

		/// <summary>
		/// Invoke.
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public async Task Invoke(IDictionary<string, object> environment)
		{
			int maxRedirects = _maxRedirects;
			while (maxRedirects >= 0)
			{
				await _next(environment);
				var response = new Microsoft.Owin.OwinResponse(environment);
				if(response.StatusCode == 302 || response.StatusCode == 301)
				{
					string url = BuildRedirectUrl(response);
					environment.Clear();
					RequestBuilder.BuildGet(environment, url);
				}
				else
				{
					break;
				}
				maxRedirects--;
			}
		}

		/// <summary>
		/// Constrói a url para redirecionamento.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private string BuildRedirectUrl(Microsoft.Owin.OwinResponse response)
		{
			string location = response.Headers.Get("Location");
			Uri uri;
			if(Uri.TryCreate(location, UriKind.Relative, out uri))
			{
				var previousRequest = new Microsoft.Owin.OwinRequest(response.Environment);
				var uriBuilder = new UriBuilder(previousRequest.Uri);
				uriBuilder.Path = location;
				return uriBuilder.ToString();
			}
			return location;
		}
	}
}
