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
using System.Web;

namespace Colosoft.Web.Security
{
	/// <summary>
	/// Implementação do módulo para suprir o redirecionamento do FormsAuthentication.
	/// </summary>
	public class SuppressFormsAuthenticationRedirectModule : IHttpModule
	{
		private static readonly object SuppressAuthenticationKey = new Object();

		/// <summary>
		/// Registra para suprimir o redirecionamento da autenticação.
		/// </summary>
		/// <param name="context"></param>
		public static void SuppressAuthenticationRedirect(HttpContext context)
		{
			context.Items[SuppressAuthenticationKey] = true;
		}

		/// <summary>
		/// Registra para suprimir o redirecionamento da autenticação.
		/// </summary>
		/// <param name="context"></param>
		public static void SuppressAuthenticationRedirect(HttpContextBase context)
		{
			context.Items[SuppressAuthenticationKey] = true;
		}

		/// <summary>
		/// Inicializa o módulo.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.PostReleaseRequestState += OnPostReleaseRequestState;
			context.EndRequest += OnEndRequest;
		}

		/// <summary>
		/// Verifica se é uma requisição Ajax.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static bool IsAjaxRequest(HttpRequest request)
		{
			if(request == null)
				throw new ArgumentNullException("request");
			return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest")) || (request.Headers != null && StringComparer.InvariantCultureIgnoreCase.Equals(request.Headers["Access-Control-Request-Headers"], "x-requested-with"));
		}

		/// <summary>
		/// Método acionado após o estado da requisição for liberado.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		private void OnPostReleaseRequestState(object source, EventArgs args)
		{
			var context = (HttpApplication)source;
			var response = context.Response;
			var request = context.Request;
			if(IsAjaxRequest(request))
			{
				SuppressAuthenticationRedirect(context.Context);
			}
		}

		/// <summary>
		/// Método acionado quando a requisição for finalizada.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		private void OnEndRequest(object source, EventArgs args)
		{
			var context = (HttpApplication)source;
			var response = context.Response;
			if(context.Context.Items.Contains(SuppressAuthenticationKey))
			{
				response.TrySkipIisCustomErrors = true;
				response.ClearContent();
				response.StatusCode = 401;
				response.RedirectLocation = null;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
