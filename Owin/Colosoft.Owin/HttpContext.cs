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
	/// Contexto Http associado.
	/// </summary>
	class HttpContext : System.Web.HttpContextBase
	{
		private System.Web.HttpResponseBase _response;

		private System.Web.HttpRequestBase _request;

		private System.Collections.IDictionary _items;

		private Security.CookielessHelperClass _cookielessHelper;

		private bool _skipAuthorization;

		/// <summary>
		/// Instacia atual do contexto.
		/// </summary>
		public static HttpContext Current
		{
			get
			{
				return System.Threading.Thread.GetData(System.Threading.Thread.GetNamedDataSlot("ColosoftHttpContext")) as HttpContext;
			}
			set
			{
				System.Threading.Thread.SetData(System.Threading.Thread.GetNamedDataSlot("ColosoftHttpContext"), value);
			}
		}

		/// <summary>
		/// Resposta associada.
		/// </summary>
		public override System.Web.HttpResponseBase Response
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Requisição associada.
		/// </summary>
		public override System.Web.HttpRequestBase Request
		{
			get
			{
				return _request;
			}
		}

		/// <summary>
		/// Sessão associada.
		/// </summary>
		public override System.Web.HttpSessionStateBase Session
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Recupera os dados do usuário logado.
		/// </summary>
		public override System.Security.Principal.IPrincipal User
		{
			get
			{
				return System.Threading.Thread.CurrentPrincipal;
			}
			set
			{
			}
		}

		/// <summary>
		/// Identifica se é para saltar a autorização.
		/// </summary>
		public override bool SkipAuthorization
		{
			get
			{
				return _skipAuthorization;
			}
			set
			{
				_skipAuthorization = value;
			}
		}

		/// <summary>
		/// Items.
		/// </summary>
		public override System.Collections.IDictionary Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Cookieless.
		/// </summary>
		internal Security.CookielessHelperClass CookielessHelper
		{
			get
			{
				if(_cookielessHelper == null)
					_cookielessHelper = new Security.CookielessHelperClass(this);
				return _cookielessHelper;
			}
		}

		/// <summary>
		/// Comportamento do estado da sessão.
		/// </summary>
		internal System.Web.SessionState.SessionStateBehavior SessionStateBehavior
		{
			get;
			set;
		}

		public HttpContext(Microsoft.Owin.IOwinContext context)
		{
			_request = new HttpRequest(this, context.Request);
			_response = new HttpResponse(this, context.Response, _request, System.Text.Encoding.UTF8);
			_items = new System.Collections.Hashtable();
			_items["owin.Environment"] = context;
			if(Colosoft.Runtime.PlatformHelper.IsRunningOnMono)
				System.Web.WebPages.BrowserHelpers.SetOverriddenBrowser(this, "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP)");
			else
				System.Web.WebPages.BrowserHelpers.SetOverriddenBrowser(this, System.Web.WebPages.BrowserOverride.Desktop);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		public HttpContext(Microsoft.Owin.IOwinContext context, System.Web.HttpRequestBase request, System.Web.HttpResponseBase response)
		{
			_request = request;
			_response = response;
			_items = new System.Collections.Hashtable();
			_items["owin.Environment"] = context;
			if(Colosoft.Runtime.PlatformHelper.IsRunningOnMono)
				System.Web.WebPages.BrowserHelpers.SetOverriddenBrowser(this, "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP)");
			else
				System.Web.WebPages.BrowserHelpers.SetOverriddenBrowser(this, System.Web.WebPages.BrowserOverride.Desktop);
		}

		/// <summary>
		/// Recupera o serviço pelo tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public override object GetService(Type serviceType)
		{
			return null;
		}

		/// <summary>
		/// Define o comportamento do estado da sessão.
		/// </summary>
		/// <param name="sessionStateBehavior"></param>
		public override void SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior sessionStateBehavior)
		{
			SessionStateBehavior = sessionStateBehavior;
		}

		/// <summary>
		/// Define o principal sem demanda.
		/// </summary>
		/// <param name="principal"></param>
		internal void SetPrincipalNoDemand(System.Security.Principal.IPrincipal principal)
		{
			this.SetPrincipalNoDemand(principal, true);
		}

		/// <summary>
		/// Define o principal sem damanda.
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="needToSetNativePrincipal"></param>
		internal void SetPrincipalNoDemand(System.Security.Principal.IPrincipal principal, bool needToSetNativePrincipal)
		{
			System.Threading.Thread.CurrentPrincipal = principal;
		}

		/// <summary>
		/// Identifica que é para salar o autorização.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="managedOnly"></param>
		internal void SetSkipAuthorizationNoDemand(bool value, bool managedOnly)
		{
			_skipAuthorization = value;
		}
	}
}
