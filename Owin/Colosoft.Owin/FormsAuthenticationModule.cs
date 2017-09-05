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

namespace Colosoft.Owin.Server.Security
{
	/// <summary>
	/// Assinatura do método acionado quando ocorrer um evento do FormsAuthentication.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	delegate void FormsAuthenticationEventHandler (object sender, FormsAuthenticationEventArgs e);
	/// <summary>
	/// Implementação do módulo de autenticação.
	/// </summary>
	sealed class FormsAuthenticationModule : Web.IHttpModule
	{
		private FormsAuthenticationEventHandler _eventHandler;

		private static bool _fAuthChecked;

		private static bool _fAuthRequired;

		private bool _fOnEnterCalled;

		/// <summary>
		/// Evento acionado quando for autenticado.
		/// </summary>
		public event System.Web.Security.FormsAuthenticationEventHandler Authenticate;

		/// <summary>
		/// Url de retorno.
		/// </summary>
		internal static string ReturnUrlVar
		{
			get
			{
				return "ReturnUrl";
			}
		}

		/// <summary>
		/// Inicializa o módulo.
		/// </summary>
		/// <param name="app"></param>
		public void Init(Web.IHttpApplication app)
		{
			if(!_fAuthChecked)
			{
				_fAuthRequired = Configuration.AuthenticationConfig.Mode == System.Web.Configuration.AuthenticationMode.Forms;
				_fAuthChecked = true;
			}
			if(_fAuthRequired)
			{
				System.Web.Security.FormsAuthentication.Initialize();
				app.AuthenticateRequest += new EventHandler(this.OnEnter);
				app.EndRequest += new EventHandler(this.OnLeave);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Recupera o ticket.
		/// </summary>
		/// <param name="version"></param>
		/// <param name="name"></param>
		/// <param name="issueDateUtc"></param>
		/// <param name="expirationUtc"></param>
		/// <param name="isPersistent"></param>
		/// <param name="userData"></param>
		/// <param name="cookiePath"></param>
		/// <returns></returns>
		private static System.Web.Security.FormsAuthenticationTicket FromUtc(int version, string name, DateTime issueDateUtc, DateTime expirationUtc, bool isPersistent, string userData, string cookiePath)
		{
			return new System.Web.Security.FormsAuthenticationTicket(version, name, issueDateUtc.ToLocalTime(), expirationUtc.ToLocalTime(), isPersistent, userData, cookiePath);
		}

		/// <summary>
		/// Remove a variável da querystring.
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="posQ"></param>
		/// <param name="token"></param>
		/// <param name="sep"></param>
		/// <param name="lenAtStartToLeave"></param>
		private static void RemoveQSVar(ref string strUrl, int posQ, string token, string sep, int lenAtStartToLeave)
		{
			for(int i = strUrl.LastIndexOf(token, StringComparison.Ordinal); i >= posQ; i = strUrl.LastIndexOf(token, StringComparison.Ordinal))
			{
				int startIndex = strUrl.IndexOf(sep, i + token.Length, StringComparison.Ordinal) + sep.Length;
				if((startIndex < sep.Length) || (startIndex >= strUrl.Length))
				{
					strUrl = strUrl.Substring(0, i);
				}
				else
				{
					strUrl = strUrl.Substring(0, i + lenAtStartToLeave) + strUrl.Substring(startIndex);
				}
			}
		}

		/// <summary>
		/// Remove a variável da querystring da url.
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="QSVar"></param>
		/// <returns></returns>
		private static string RemoveQueryStringVariableFromUrl(string strUrl, string QSVar)
		{
			int index = strUrl.IndexOf('?');
			if(index >= 0)
			{
				string sep = "&";
				string str2 = "?";
				string token = sep + QSVar + "=";
				RemoveQSVar(ref strUrl, index, token, sep, sep.Length);
				token = str2 + QSVar + "=";
				RemoveQSVar(ref strUrl, index, token, sep, str2.Length);
				sep = System.Web.HttpUtility.UrlEncode("&");
				str2 = System.Web.HttpUtility.UrlEncode("?");
				token = sep + System.Web.HttpUtility.UrlEncode(QSVar + "=");
				RemoveQSVar(ref strUrl, index, token, sep, sep.Length);
				token = str2 + System.Web.HttpUtility.UrlEncode(QSVar + "=");
				RemoveQSVar(ref strUrl, index, token, sep, str2.Length);
			}
			return strUrl;
		}

		/// <summary>
		/// Extrai o ticket do cookie.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <param name="cookielessTicket"></param>
		/// <returns></returns>
		private static System.Web.Security.FormsAuthenticationTicket ExtractTicketFromCookie(HttpContext context, string name, out bool cookielessTicket)
		{
			System.Web.Security.FormsAuthenticationTicket ticket = null;
			string encryptedTicket = null;
			System.Web.Security.FormsAuthenticationTicket ticket2;
			bool flag = false;
			bool flag2 = false;
			try
			{
				try
				{
					cookielessTicket = false;
					if(cookielessTicket)
					{
						encryptedTicket = context.CookielessHelper.GetCookieValue('F');
					}
					else
					{
						var cookie = context.Request.Cookies[name];
						if(cookie != null)
						{
							encryptedTicket = cookie.Value;
						}
					}
					if((encryptedTicket != null) && (encryptedTicket.Length > 1))
					{
						try
						{
							ticket = System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);
						}
						catch
						{
							if(cookielessTicket)
							{
								context.CookielessHelper.SetCookieValue('F', null);
							}
							else
							{
								context.Request.Cookies.Remove(name);
							}
							flag2 = true;
						}
						if(ticket == null)
						{
							flag2 = true;
						}
						if(((ticket != null) && !ticket.Expired) && ((cookielessTicket || !System.Web.Security.FormsAuthentication.RequireSSL) || context.Request.IsSecureConnection))
						{
							return ticket;
						}
						if((ticket != null) && ticket.Expired)
						{
							flag = true;
						}
						ticket = null;
						if(cookielessTicket)
						{
							context.CookielessHelper.SetCookieValue('F', null);
						}
						else
						{
							context.Request.Cookies.Remove(name);
						}
					}
					if(System.Web.Security.FormsAuthentication.EnableCrossAppRedirects)
					{
						encryptedTicket = context.Request.QueryString[name];
						if((encryptedTicket != null) && (encryptedTicket.Length > 1))
						{
							if(!cookielessTicket && (System.Web.Security.FormsAuthentication.CookieMode == System.Web.HttpCookieMode.AutoDetect))
							{
								cookielessTicket = Security.CookielessHelperClass.UseCookieless(context, true, System.Web.Security.FormsAuthentication.CookieMode);
							}
							try
							{
								ticket = System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);
							}
							catch
							{
								flag2 = true;
							}
							if(ticket == null)
							{
								flag2 = true;
							}
						}
						if((ticket == null) || ticket.Expired)
						{
							encryptedTicket = context.Request.Form[name];
							if((encryptedTicket != null) && (encryptedTicket.Length > 1))
							{
								if(!cookielessTicket && (System.Web.Security.FormsAuthentication.CookieMode == System.Web.HttpCookieMode.AutoDetect))
								{
									cookielessTicket = Security.CookielessHelperClass.UseCookieless(context, true, System.Web.Security.FormsAuthentication.CookieMode);
								}
								try
								{
									ticket = System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);
								}
								catch
								{
									flag2 = true;
								}
								if(ticket == null)
								{
									flag2 = true;
								}
							}
						}
					}
					if((ticket == null) || ticket.Expired)
					{
						if((ticket != null) && ticket.Expired)
						{
							flag = true;
						}
						return null;
					}
					if(System.Web.Security.FormsAuthentication.RequireSSL && !context.Request.IsSecureConnection)
					{
						throw new System.Web.HttpException("Connection not secure creating secure cookie");
					}
					if(cookielessTicket)
					{
						if(ticket.CookiePath != "/")
						{
							ticket = FromUtc(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, ticket.UserData, "/");
							encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(ticket);
						}
						context.CookielessHelper.SetCookieValue('F', encryptedTicket);
						string url = RemoveQueryStringVariableFromUrl(context.Request.RawUrl, name);
						context.Response.Redirect(url);
					}
					else
					{
						var cookie2 = new System.Web.HttpCookie(name, encryptedTicket) {
							HttpOnly = true,
							Path = ticket.CookiePath
						};
						if(ticket.IsPersistent)
						{
							cookie2.Expires = ticket.Expiration;
						}
						cookie2.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
						if(System.Web.Security.FormsAuthentication.CookieDomain != null)
						{
							cookie2.Domain = System.Web.Security.FormsAuthentication.CookieDomain;
						}
						context.Response.Cookies.Remove(cookie2.Name);
						context.Response.Cookies.Add(cookie2);
					}
					ticket2 = ticket;
				}
				finally
				{
				}
			}
			catch
			{
				throw;
			}
			return ticket2;
		}

		/// <summary>
		/// Método acionado quando for solicitada a autenticação.
		/// </summary>
		/// <param name="e"></param>
		private void OnAuthenticate(FormsAuthenticationEventArgs e)
		{
			System.Web.HttpCookie cookie = null;
			if(_eventHandler != null)
				_eventHandler(this, e);
			//// Recupera o cookie da autenticação
			var formAuthCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
			if(formAuthCookie != null && !string.IsNullOrEmpty(formAuthCookie.Value))
			{
				if(e.User != null)
				{
					e.Context.SetPrincipalNoDemand(e.User);
				}
				else
				{
					bool cookielessTicket = false;
					var tOld = ExtractTicketFromCookie(e.Context, System.Web.Security.FormsAuthentication.FormsCookieName, out cookielessTicket);
					if((tOld != null) && !tOld.Expired)
					{
						System.Web.Security.FormsAuthenticationTicket ticket = tOld;
						if(System.Web.Security.FormsAuthentication.SlidingExpiration)
						{
							ticket = System.Web.Security.FormsAuthentication.RenewTicketIfOld(tOld);
						}
						e.Context.SetPrincipalNoDemand(new System.Security.Principal.GenericPrincipal(new System.Web.Security.FormsIdentity(ticket), new string[0]));
						if(!cookielessTicket && !ticket.CookiePath.Equals("/"))
						{
							cookie = e.Context.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
							if(cookie != null)
							{
								cookie.Path = ticket.CookiePath;
							}
						}
						if(ticket != tOld)
						{
							if((cookielessTicket && (ticket.CookiePath != "/")) && (ticket.CookiePath.Length > 1))
							{
								ticket = FromUtc(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, ticket.UserData, "/");
							}
							string cookieValue = System.Web.Security.FormsAuthentication.Encrypt(ticket);
							if(cookielessTicket)
							{
								e.Context.CookielessHelper.SetCookieValue('F', cookieValue);
								e.Context.Response.Redirect(e.Context.Request.RawUrl);
							}
							else
							{
								if(cookie != null)
								{
									cookie = e.Context.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
								}
								if(cookie == null)
								{
									cookie = new System.Web.HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, cookieValue) {
										Path = ticket.CookiePath
									};
								}
								if(ticket.IsPersistent)
								{
									cookie.Expires = ticket.Expiration;
								}
								cookie.Value = cookieValue;
								cookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
								cookie.HttpOnly = true;
								if(System.Web.Security.FormsAuthentication.CookieDomain != null)
								{
									cookie.Domain = System.Web.Security.FormsAuthentication.CookieDomain;
								}
								e.Context.Response.Cookies.Remove(cookie.Name);
								e.Context.Response.Cookies.Add(cookie);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Método aciando quando entrar na requisição.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void OnEnter(object source, EventArgs eventArgs)
		{
			_fOnEnterCalled = true;
			var application = (Web.IHttpApplication)source;
			HttpContext context = application.Context;
			OnAuthenticate(new FormsAuthenticationEventArgs(context));
			CookielessHelperClass cookielessHelper = context.CookielessHelper;
			if(Configuration.AuthenticationConfig.AccessingLoginPage(context, System.Web.Security.FormsAuthentication.LoginUrl))
			{
				context.SetSkipAuthorizationNoDemand(true, false);
				cookielessHelper.RedirectWithDetectionIfRequired(null, System.Web.Security.FormsAuthentication.CookieMode);
			}
			if(!context.SkipAuthorization)
			{
				context.SetSkipAuthorizationNoDemand(false, false);
			}
		}

		/// <summary>
		/// Método acionado quando deixar a requisição.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void OnLeave(object source, EventArgs eventArgs)
		{
			if(_fOnEnterCalled)
				_fOnEnterCalled = false;
			else
				return;
			var application = (Web.IHttpApplication)source;
			HttpContext context = application.Context;
			if((context.Response.StatusCode == 0x191) && !context.Response.SuppressFormsAuthenticationRedirect)
			{
				string rawUrl = context.Request.RawUrl;
				if((rawUrl.IndexOf("?" + ReturnUrlVar + "=", StringComparison.Ordinal) == -1) && (rawUrl.IndexOf("&" + ReturnUrlVar + "=", StringComparison.Ordinal) == -1))
				{
					string str3;
					string strUrl = null;
					if(!string.IsNullOrEmpty(System.Web.Security.FormsAuthentication.LoginUrl))
					{
						strUrl = Configuration.AuthenticationConfig.GetCompleteLoginUrl(context, System.Web.Security.FormsAuthentication.LoginUrl);
					}
					if((strUrl == null) || (strUrl.Length <= 0))
					{
						throw new System.Web.HttpException("Auth_Invalid_Login_Url");
					}
					CookielessHelperClass cookielessHelper = context.CookielessHelper;
					if(strUrl.IndexOf('?') >= 0)
					{
						strUrl = RemoveQueryStringVariableFromUrl(strUrl, ReturnUrlVar);
						string[] textArray1 = new string[] {
							strUrl,
							"&",
							ReturnUrlVar,
							"=",
							System.Web.HttpUtility.UrlEncode(rawUrl, context.Request.ContentEncoding)
						};
						str3 = string.Concat(textArray1);
					}
					else
					{
						string[] textArray2 = new string[] {
							strUrl,
							"?",
							ReturnUrlVar,
							"=",
							System.Web.HttpUtility.UrlEncode(rawUrl, context.Request.ContentEncoding)
						};
						str3 = string.Concat(textArray2);
					}
					int index = rawUrl.IndexOf('?');
					if((index >= 0) && (index < (rawUrl.Length - 1)))
					{
						str3 = str3 + "&" + rawUrl.Substring(index + 1);
					}
					cookielessHelper.SetCookieValue('F', null);
					cookielessHelper.RedirectWithDetectionIfRequired(str3, System.Web.Security.FormsAuthentication.CookieMode);
					context.Response.Redirect(str3, false);
				}
			}
		}

		/// <summary>
		/// Identifica se a autenticação via forms é requerida.
		/// </summary>
		internal static bool FormsAuthRequired
		{
			get
			{
				return _fAuthRequired;
			}
		}
	}
}
