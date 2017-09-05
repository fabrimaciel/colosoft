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
	/// Armazena os argumentos do usuário que realizou o logout.
	/// </summary>
	public class LogoutUserInfoEventArgs : EventArgs
	{
		private string _ticketId;

		private string _userName;

		private string _userData;

		private DateTime _issueDate;

		private DateTime _expiration;

		/// <summary>
		/// Identificador do bilhete associado.
		/// </summary>
		public string TicketId
		{
			get
			{
				return _ticketId;
			}
		}

		/// <summary>
		/// Nome do usuário associado.
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
		}

		/// <summary>
		/// Dados do usuário associado.
		/// </summary>
		public string UserData
		{
			get
			{
				return _userData;
			}
		}

		/// <summary>
		/// Data de emissão do Login.
		/// </summary>
		public DateTime IssueDate
		{
			get
			{
				return _issueDate;
			}
		}

		/// <summary>
		/// Data que o login expirou.
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
		/// <param name="ticketId"></param>
		/// <param name="userName"></param>
		/// <param name="userData"></param>
		/// <param name="issueDate"></param>
		/// <param name="expiration"></param>
		public LogoutUserInfoEventArgs(string ticketId, string userName, string userData, DateTime issueDate, DateTime expiration)
		{
			_ticketId = ticketId;
			_userName = userName;
			_userData = userData;
			_issueDate = issueDate;
			_expiration = expiration;
		}
	}
}
