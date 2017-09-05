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

namespace Colosoft.Security
{
	/// <summary>
	/// Armazena os dados associados com o usuário.
	/// </summary>
	public interface IUser
	{
		/// <summary>
		/// Chave que identifica unicamente o usuário.
		/// </summary>
		string UserKey
		{
			get;
		}

		/// <summary>
		/// Nome do usuário.
		/// </summary>
		string UserName
		{
			get;
		}

		/// <summary>
		/// Nome completo do usuário.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Email associado.
		/// </summary>
		string Email
		{
			get;
		}

		/// <summary>
		/// Questão usada para recuperação da senha.
		/// </summary>
		string PasswordQuestion
		{
			get;
		}

		/// <summary>
		/// Identifica se o usuário foi aprovado.
		/// </summary>
		bool IsApproved
		{
			get;
		}

		/// <summary>
		/// Nome do provedor de identidade do usuário.
		/// </summary>
		string IdentityProvider
		{
			get;
		}

		/// <summary>
		/// Ultima data de alteração da senha de acesso.
		/// </summary>
		DateTimeOffset LastPasswordChangedDate
		{
			get;
		}

		/// <summary>
		/// Data de criação do usuário.
		/// </summary>
		DateTimeOffset CreationDate
		{
			get;
		}

		/// <summary>
		/// Indica que o usuário deve ignorar o captcha do sistema
		/// </summary>
		bool IgnoreCaptcha
		{
			get;
		}

		/// <summary>
		/// Identifica se o usuário está inativo.
		/// </summary>
		bool IsActive
		{
			get;
		}
	}
}
