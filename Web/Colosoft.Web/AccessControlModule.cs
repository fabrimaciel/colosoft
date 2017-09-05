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

namespace Colosoft.Web.Security.AccessControl
{
	/// <summary>
	/// Implementação do módulo para auditoria de segurança.
	/// </summary>
	public class AccessControlModule : System.Web.IHttpModule
	{
		private const string FormsAuthenticationTicketKey = "AuditingFormsAuthenticationTicketKey";

		private System.Security.Cryptography.RandomNumberGenerator _randgen;

		private Dictionary<string, System.Web.Security.FormsAuthenticationTicket> _tickets = new Dictionary<string, System.Web.Security.FormsAuthenticationTicket>();

		private System.Threading.Timer _validateTicketsTimer;

		private System.Threading.Timer _closePendingLoginTimer;

		/// <summary>
		/// Intervalo para verificação dos tickets expirados. (10 segundos)
		/// </summary>
		private long _validateTicketsInterval = 10000;

		/// <summary>
		/// Intervalo para fechar os logins pendentes. (1 minuto)
		/// </summary>
		private long _closePendingLoginInterval = 60000;

		private System.Threading.Thread _saveOperationsThread;

		/// <summary>
		/// Evento acionado quando o usuário for autenticado.
		/// </summary>
		public static event EventHandler<AuthenticatedUserInfoEventArgs> Authenticated;

		/// <summary>
		/// Evento acionado quando um ticket de autenticação for renovado.
		/// </summary>
		public static event EventHandler<AuthenticatedUserInfoEventArgs> TicketUpdated;

		/// <summary>
		/// Evento acionado quando um usuário é deslogado do site.
		/// </summary>
		public static event EventHandler<LogoutUserInfoEventArgs> Logout;

		/// <summary>
		/// Evento acionado quando é requisitado a fechamento de logins pendentes.
		/// </summary>
		public static event EventHandler ClosePendingLoginRequested;

		/// <summary>
		/// Nome do cookie que irá armazenar o TicketId
		/// </summary>
		public static string TicketIdCookieName
		{
			get
			{
				return string.Format("{0}_ID", System.Web.Security.FormsAuthentication.FormsCookieName);
			}
		}

		/// <summary>
		/// Nome do cookie que irá armazenar o Token
		/// </summary>
		public static string TokenCookieName
		{
			get
			{
				return string.Format("{0}_TOKEN", System.Web.Security.FormsAuthentication.FormsCookieName);
			}
		}

		/// <summary>
		/// Método acionado quando o módulo é inicializado.
		/// </summary>
		/// <param name="context"></param>
		public void Init(System.Web.HttpApplication context)
		{
			context.BeginRequest += OnBeginRequest;
			context.EndRequest += OnEndRequest;
			_validateTicketsTimer = new System.Threading.Timer(ValidateTickets, null, 0, _validateTicketsInterval);
			_closePendingLoginTimer = new System.Threading.Timer(ClosePendingLogin, null, 0, _closePendingLoginInterval);
			_saveOperationsThread = new System.Threading.Thread(SaveAccessControlOperations);
			_saveOperationsThread.Start();
		}

		/// <summary>
		/// Método acionado para libera o módulo.
		/// </summary>
		public void Dispose()
		{
			if(_validateTicketsTimer != null)
				_validateTicketsTimer.Dispose();
			if(_saveOperationsThread != null)
				_saveOperationsThread.Abort();
			if(_closePendingLoginTimer != null)
				_closePendingLoginTimer.Dispose();
			_saveOperationsThread = null;
		}

		/// <summary>
		/// Salva as operações do controle de acesso.
		/// </summary>
		private void SaveAccessControlOperations()
		{
			while (true)
			{
				try
				{
					if(!AccessControlManager.SaveOperations())
					{
						System.Threading.Thread.Sleep(3000);
						continue;
					}
				}
				catch(System.Threading.ThreadAbortException)
				{
					break;
				}
				catch(Exception)
				{
					System.Threading.Thread.Sleep(3000);
				}
			}
		}

		/// <summary>
		/// Valida o tickets.
		/// </summary>
		/// <param name="sender"></param>
		private void ValidateTickets(object sender)
		{
			var expireds = new List<KeyValuePair<string, System.Web.Security.FormsAuthenticationTicket>>();
			lock (_tickets)
			{
				foreach (var i in _tickets)
				{
					if(i.Value.Expired)
						expireds.Add(i);
				}
				foreach (var i in expireds)
					_tickets.Remove(i.Key);
			}
			foreach (var i in expireds)
			{
				if(Logout != null)
					Logout(this, new LogoutUserInfoEventArgs(i.Key, i.Value.Name, i.Value.UserData, i.Value.IssueDate, i.Value.Expiration));
			}
		}

		/// <summary>
		/// Fecha os logins pendentes.
		/// </summary>
		/// <param name="sender"></param>
		private void ClosePendingLogin(object sender)
		{
			if(ClosePendingLoginRequested != null)
				ClosePendingLoginRequested(this, EventArgs.Empty);
		}

		/// <summary>
		/// Recupera o identificador do ticket que está nos cookies informados.
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		internal static string GetTicketId(System.Web.HttpCookieCollection cookies)
		{
			if(cookies.AllKeys.Contains(TicketIdCookieName))
				return cookies[TicketIdCookieName].Value;
			return null;
		}

		/// <summary>
		/// Renova o ticket.
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="ticket"></param>
		/// <returns>Identifica se a renovação foi feita com sucesso.</returns>
		private bool RenewTicket(string ticketId, System.Web.Security.FormsAuthenticationTicket ticket)
		{
			if(string.IsNullOrEmpty(ticketId) || !TicketId.IsLegit(ticketId))
				return false;
			lock (_tickets)
				if(_tickets.ContainsKey(ticketId))
					_tickets[ticketId] = ticket;
				else if(!ticket.Expired)
					_tickets.Add(ticketId, ticket);
				else
					return false;
			return true;
		}

		/// <summary>
		/// Remove o ticket.
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		private bool RemoveTicket(string ticketId)
		{
			if(string.IsNullOrEmpty(ticketId) || !TicketId.IsLegit(ticketId))
				return false;
			lock (_tickets)
				return _tickets.Remove(ticketId);
		}

		/// <summary>
		/// Recupera o texto do ticket.
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		private string GetTicketText(System.Web.HttpCookieCollection cookies)
		{
			if(cookies.AllKeys.Contains(System.Web.Security.FormsAuthentication.FormsCookieName))
				return cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value;
			return null;
		}

		/// <summary>
		/// Método acionado quando for iniciada um requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeginRequest(object sender, EventArgs e)
		{
			var application = (System.Web.HttpApplication)sender;
			var request = application.Context.Request;
			var ticketText = GetTicketText(request.Cookies);
			if(!string.IsNullOrEmpty(ticketText))
			{
				System.Web.Security.FormsAuthenticationTicket ticket = null;
				try
				{
					ticket = System.Web.Security.FormsAuthentication.Decrypt(ticketText);
				}
				catch(Exception)
				{
				}
				if(ticket != null && !ticket.Expired)
				{
					var ticketId = GetTicketId(request.Cookies);
					application.Context.Items[FormsAuthenticationTicketKey] = ticket;
				}
			}
			var tokenProvider = Colosoft.Security.Tokens.Provider;
			if(tokenProvider is Colosoft.Security.ITokenProviderExtension)
				((Colosoft.Security.ITokenProviderExtension)tokenProvider).TokenInserted += TokenInserted;
		}

		/// <summary>
		/// Método acionado quando um token é inserido no sistema.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TokenInserted(object sender, Colosoft.Security.TokenInsertedEventArgs e)
		{
			var context = System.Web.HttpContext.Current;
			if(context != null)
				context.Items[FormsAuthenticationTicketKey] = e.Token;
		}

		/// <summary>
		/// Método acionado quando for finalizada uma requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEndRequest(object sender, EventArgs e)
		{
			var application = (System.Web.HttpApplication)sender;
			var response = application.Context.Response;
			if(response.Cookies.AllKeys.Contains(System.Web.Security.FormsAuthentication.FormsCookieName))
			{
				var ticketText = response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value;
				var oldTicket = application.Context.Items[FormsAuthenticationTicketKey] as System.Web.Security.FormsAuthenticationTicket;
				System.Web.Security.FormsAuthenticationTicket ticket = null;
				if(!string.IsNullOrEmpty(ticketText) && ticketText != "NoCookie")
				{
					try
					{
						ticket = System.Web.Security.FormsAuthentication.Decrypt(ticketText);
					}
					catch(Exception)
					{
					}
				}
				if(oldTicket != null && (string.IsNullOrEmpty(ticketText) || ticketText == "NoCookie"))
				{
					response.Cookies.Add(new System.Web.HttpCookie(TicketIdCookieName, null) {
						Expires = DateTime.MinValue
					});
					RemoveTicket(GetTicketId(application.Context.Request.Cookies));
					var token = application.Context.Request.Cookies[TokenCookieName];
					if(token != null)
					{
						var tokenProvider = Colosoft.Security.Tokens.Provider;
						if(tokenProvider != null)
							tokenProvider.Close(token.Value);
					}
					if(Logout != null)
						Logout(this, new LogoutUserInfoEventArgs(GetTicketId(application.Context.Request.Cookies), oldTicket.Name, oldTicket.UserData, oldTicket.IssueDate, DateTime.Now));
				}
				else if(oldTicket == null && ticket != null && !ticket.Expired)
				{
					var id = TicketId.Create(ref _randgen);
					response.Cookies.Add(new System.Web.HttpCookie(TicketIdCookieName, id) {
						Expires = ticket.Expiration
					});
					string token = application.Context.Items[FormsAuthenticationTicketKey] as string;
					response.Cookies.Add(new System.Web.HttpCookie(TokenCookieName, token) {
						Expires = ticket.Expiration
					});
					if(Authenticated != null)
						Authenticated(this, new AuthenticatedUserInfoEventArgs(id, ticket.Name, ticket.UserData, ticket.IssueDate, ticket.Expiration));
				}
				else if(oldTicket != null && ticket != null && !ticket.Expired)
				{
					var ticketIdCookie = application.Context.Request.Cookies[TicketIdCookieName];
					string id = null;
					if(ticketIdCookie != null && !string.IsNullOrEmpty(ticketIdCookie.Value))
						id = ticketIdCookie.Value;
					else
						id = TicketId.Create(ref _randgen);
					response.Cookies.Add(new System.Web.HttpCookie(TicketIdCookieName, id) {
						Expires = ticket.Expiration
					});
					var token = application.Context.Request.Cookies[TokenCookieName];
					if(token != null)
					{
						var tokenProvider = Colosoft.Security.Tokens.Provider;
						if(tokenProvider != null)
							tokenProvider.Ping(token.Value);
					}
					if(TicketUpdated != null)
						TicketUpdated(this, new AuthenticatedUserInfoEventArgs(id, ticket.Name, ticket.UserData, ticket.IssueDate, ticket.Expiration));
				}
				if(ticket != null)
					RenewTicket(GetTicketId(application.Context.Request.Cookies), ticket);
			}
		}
	}
}
