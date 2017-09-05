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

namespace Colosoft.Security.Authentication
{
	/// <summary>
	/// Interface mínima para que um usuário seja autenticado
	/// </summary>
	public interface IAutheticableUser : Colosoft.Security.IUser
	{
		/// <summary>
		/// Identificador do provedor de identidade associado.
		/// </summary>
		int IdentityProviderId
		{
			get;
			set;
		}

		/// <summary>
		/// Resposta à pergunta para relembrar senha
		/// </summary>
		string PasswordAnswer
		{
			get;
			set;
		}

		/// <summary>
		/// Questão usada para recuperação da senha.
		/// </summary>
		new string PasswordQuestion
		{
			get;
			set;
		}

		/// <summary>
		/// Nome de usuário
		/// </summary>
		new string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// Email
		/// </summary>
		new string Email
		{
			get;
			set;
		}

		/// <summary>
		/// Nome ompleto
		/// </summary>
		new string FullName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do provedor de identidade
		/// </summary>
		new string IdentityProvider
		{
			get;
			set;
		}

		/// <summary>
		/// Data da ultima alteraçã de senha
		/// </summary>
		new DateTimeOffset LastPasswordChangedDate
		{
			get;
			set;
		}
	}
}
