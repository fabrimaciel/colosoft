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
	/// Interface que contém métodos de autenticação
	/// </summary>
	public interface IAuthenticate
	{
		/// <summary>
		/// Informa se o provedor de identificação permite ou não a criação de usuário
		/// </summary>
		bool CanCreateUser
		{
			get;
		}

		/// <summary>
		/// Informa se a senha pode ser dedefinida
		/// </summary>
		bool CanResetPassword
		{
			get;
		}

		/// <summary>
		/// Inicia o processo de redefinição de senha
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <returns>Resultado do processo</returns>
		ResetPasswordProcessResult RequestPasswordReset(string userName);

		/// <summary>
		/// Cria um novo usuário no provedor de identificação
		/// </summary>
		/// <param name="user">usuário</param>
		/// <param name="password">hash da Senha</param>
		/// <param name="parameters">Parâmetros adicionais do provider</param>
		/// <returns>Sucesso de criação</returns>
		CreateUserResult CreateNewUser(IUser user, string password, params SecurityParameter[] parameters);

		/// <summary>
		/// Valida credenciais
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="parameters">Demais parâmetros</param>
		/// <returns>Resultado da autenticação</returns>
		IValidateUserResult ValidateUser(string userName, string password, params SecurityParameter[] parameters);

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		IValidateUserResult ValidateToken(string token);

		/// <summary>
		/// Altera a senha
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="oldPassword">Senha atual</param>
		/// <param name="newPassword">Nova senha</param>
		/// <param name="parameters">Parametros adicionais de autenticação</param>
		/// <returns>Resultado da troca de senha</returns>
		ChangePasswordResult ChangePassword(string userName, string oldPassword, string newPassword, params SecurityParameter[] parameters);
	}
}
