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
	/// Armazena os dados de uma operação de acesso.
	/// </summary>
	public class AccessControlOperation
	{
		private string _name;

		private DateTime _issueDate;

		private DateTime _expiration;

		private string _userHostAddress;

		private string _userName;

		private string _ticketId;

		/// <summary>
		/// Nome da operação.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Endereço de acesso do cliente.
		/// </summary>
		public string UserHostAddress
		{
			get
			{
				return _userHostAddress;
			}
		}

		/// <summary>
		/// Nome do usuário que realizou a operação.
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
		}

		/// <summary>
		/// Identificador do bilhete que representa a autenticação do usuário.
		/// </summary>
		public string TicketId
		{
			get
			{
				return _ticketId;
			}
		}

		/// <summary>
		/// Horário de expidição da operação.
		/// </summary>
		public DateTime IssueDate
		{
			get
			{
				return _issueDate;
			}
		}

		/// <summary>
		/// Horário que a operação expira.
		/// </summary>
		public DateTime Expiration
		{
			get
			{
				return _expiration;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome da operação.</param>
		/// <param name="issueDate">Horário de início da operação.</param>
		/// <param name="expiration"></param>
		/// <param name="userHostAddress">Endereço do cliente que realizou a operação.</param>
		/// <param name="userName">Nome do usuário que realizou a operação.</param>
		/// <param name="ticketId">Identificador do bilhete associado com a autenticação do usuário.</param>
		public AccessControlOperation(string name, DateTime issueDate, DateTime expiration, string userHostAddress, string userName, string ticketId)
		{
			_name = name;
			_issueDate = issueDate;
			_expiration = expiration;
			_userHostAddress = userHostAddress;
			_userName = userName;
			_ticketId = ticketId;
		}
	}
}
