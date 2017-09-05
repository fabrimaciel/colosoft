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

namespace Colosoft.Net.Share
{
	/// <summary>
	/// Armazena os dados da identifidade de acesso para o 
	/// compartilhamento.
	/// </summary>
	public class NetworkShareIdentity
	{
		/// <summary>
		/// Usuário de acesso.
		/// </summary>
		public string Username
		{
			get;
			set;
		}

		/// <summary>
		/// Senha de acesso.
		/// </summary>
		public string Password
		{
			get;
			set;
		}

		/// <summary>
		/// Domínio de acesso.
		/// </summary>
		public string Domain
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="domain">Domínio de autenticação.</param>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Senha de acesso.</param>
		public NetworkShareIdentity(string domain, string username, string password)
		{
			this.Domain = domain;
			this.Username = username;
			this.Password = password;
		}
	}
}
