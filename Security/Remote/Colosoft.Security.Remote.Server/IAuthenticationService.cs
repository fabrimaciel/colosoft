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
using System.ServiceModel;
using Colosoft.Security;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Interface com o contrato do serviço de autenticação
	/// </summary>
	[ServiceContract]
	public interface IAuthenticationService
	{
		/// <summary>
		/// Valida o usuário no provedor de identidade
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="servicesContext">Nome do contexto de serviços que será usado na autenticação.</param>
		/// <param name="parameters">Demais informações necessárias</param>
		/// <returns></returns>
		[OperationContract]
		ValidateUserResultWrapper ValidateUser(string userName, string password, string servicesContext, SecurityParameter[] parameters);

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token">Token que será validado.</param>
		/// <param name="servicesContext">Nome do contexto de serviços que será usado na autenticação.</param>
		/// <returns></returns>
		[OperationContract]
		ValidateUserResultWrapper ValidateToken(string token, string servicesContext);

		/// <summary>
		/// Desloga o usuário do sistema
		/// </summary>
		/// <param name="token">Token do usuário</param>
		/// <returns>Sucesso da operação</returns>
		[OperationContract]
		bool LogOut(string token);

		/// <summary>
		/// Verifica se um token está ou não válido
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="token">token</param>
		/// <returns>Objeto com o resultado da consulta</returns>
		[OperationContract]
		TokenConsultResult Check(string userName, string token);

		/// <summary>
		/// Altera o password do usuário
		/// </summary>
		/// <param name="userName">usuário</param>
		/// <param name="oldPassword">senha atual</param>
		/// <param name="newPassword">nova senha</param>
		/// <param name="parameters">demais parametros</param>
		/// <returns>resultado da operação</returns>
		[OperationContract]
		ChangePasswordResult ChangePassword(string userName, string oldPassword, string newPassword, SecurityParameter[] parameters);

		/// <summary>
		/// Inicia o processo de redefinição de senha
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <returns>Resultado do processo</returns>
		[OperationContract]
		ResetPasswordProcessResult RequestPasswordReset(string userName);
	}
}
