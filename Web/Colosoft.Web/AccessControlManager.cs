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
	/// Gernciador do controle de acesso.
	/// </summary>
	public static class AccessControlManager
	{
		private static Queue<AccessControlOperation> _operations = new Queue<AccessControlOperation>();

		private static IAccessControlProvider _provider;

		private static Func<AccessControlOperation, AccessControlOperation> _operationValidatorHandler;

		/// <summary>
		/// Evento acionado quando o 
		/// </summary>
		public static event EventHandler<AuthenticatedUserInfoEventArgs> Authenticated {
			add
			{
				AccessControlModule.Authenticated += value;
			}
			remove {
				AccessControlModule.Authenticated -= value;
			}
		}

		/// <summary>
		/// Método acionado quando ocorrer um logout.
		/// </summary>
		public static event EventHandler<LogoutUserInfoEventArgs> Logout {
			add
			{
				AccessControlModule.Logout += value;
			}
			remove {
				AccessControlModule.Logout -= value;
			}
		}

		/// <summary>
		/// Evento acionado quando um ticket de autenticação for renovado.
		/// </summary>
		public static event EventHandler<AuthenticatedUserInfoEventArgs> TicketUpdated {
			add
			{
				AccessControlModule.TicketUpdated += value;
			}
			remove {
				AccessControlModule.TicketUpdated -= value;
			}
		}

		/// <summary>
		/// Instancia do provedor associado.
		/// </summary>
		public static IAccessControlProvider Provider
		{
			get
			{
				return _provider;
			}
			set
			{
				_provider = value;
			}
		}

		/// <summary>
		/// Configura o validador das operações.
		/// </summary>
		/// <param name="handler"></param>
		public static void ConfigureOperationValidator(Func<AccessControlOperation, AccessControlOperation> handler)
		{
			_operationValidatorHandler = handler;
		}

		/// <summary>
		/// Registra uma operação para o controle de acesso.
		/// </summary>
		/// <param name="name">Nome da operação.</param>
		public static void RegisterOperation(string name)
		{
			var context = System.Web.HttpContext.Current;
			string ticketId = null;
			string userName = null;
			if(context != null)
			{
				if(context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
					userName = context.User.Identity.Name;
				ticketId = AccessControlModule.GetTicketId(context.Request.Cookies);
			}
			RegisterOperation(name, userName, ticketId, DateTime.Now, DateTime.Now);
		}

		/// <summary>
		/// Registra uma operação para o controle de acesso.
		/// </summary>
		/// <param name="name">Nome da operação.</param>
		/// <param name="userName">Nome do usuário que está realizando a operação.</param>
		/// <param name="ticketId">Identificador do bilhete associada a operação.</param>
		/// <param name="issueDate">Horário de expidição da operação.</param>
		/// <param name="endDate">Horário de fim da operação.</param>
		public static void RegisterOperation(string name, string userName, string ticketId, DateTime issueDate, DateTime endDate)
		{
			var context = System.Web.HttpContext.Current;
			string userHostAddress = "127.0.0.1";
			if(context != null)
				userHostAddress = context.Request.UserHostAddress;
			var operation = new AccessControlOperation(name, issueDate, endDate, userHostAddress, userName, ticketId);
			if(_operationValidatorHandler != null)
				operation = _operationValidatorHandler(operation);
			lock (_operations)
				_operations.Enqueue(operation);
		}

		/// <summary>
		/// Salva as operações pedentes.
		/// </summary>
		/// <returns>True caso ainda existam operações para serem salvas</returns>
		internal static bool SaveOperations()
		{
			var operations = new List<AccessControlOperation>();
			lock (_operations)
			{
				operations.AddRange(_operations);
				_operations.Clear();
			}
			if(operations.Count > 0 && _provider != null)
				_provider.SaveOperations(operations);
			return operations.Count > 0;
		}

		/// <summary>
		/// Verifica se a relação de operações está vazia.
		/// </summary>
		internal static bool IsEmpty()
		{
			return _operations.Count == 0;
		}
	}
}
