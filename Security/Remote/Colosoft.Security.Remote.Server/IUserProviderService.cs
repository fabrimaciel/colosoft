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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Assinatura do serviço do provedor de usuário.
	/// </summary>
	[ServiceContract]
	public interface IUserProviderService
	{
		/// <summary>
		/// Recupera as informações do provedor.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		UserProviderInfo GetProviderInfo();

		/// <summary>
		/// Cria um novo usuário no sistema.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Senha de acesso.</param>
		/// <param name="email">Email associado com o usuário.</param>
		/// <param name="passwordQuestion">Pergunta para a senha.</param>
		/// <param name="passwordAnswer">Resposta da pergunta para a recuperação da senha.</param>
		/// <param name="isApproved">Identifica se o usuário é aprovado.</param>
		/// <param name="userKey">Chave que identifica o usuário.</param>
		/// <param name="ignoreCaptcha">Indica se o usuário irá ignorar o controle por captcha</param>
		/// <param name="fullname">Nome do usuário</param>
		/// <param name="parameters">Parâmetros adicionais</param>
		/// <param name="identityProvider">Nome do provedor de identidade do usuário.</param>
		/// <returns></returns>
		[OperationContract]
		CreateUserResult CreateUser(string username, string password, string fullname, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string userKey, string identityProvider, bool ignoreCaptcha, Security.SecurityParameter[] parameters);

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Nova senha.</param>
		/// <param name="newPasswordQuestion">Pergunta para a nova senha.</param>
		/// <param name="newPasswordAnswer">Resposta da pergunta da nova senha.</param>
		/// <returns></returns>
		[OperationContract]
		bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer);

		/// <summary>
		/// Reseta a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="answer">Resposta para validar a operação.</param>
		/// <returns></returns>
		[OperationContract]
		string ResetPassword(string username, string answer);

		/// <summary>
		/// Recupera os dados do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		[OperationContract]
		User GetUser(string username, bool userIsOnline);

		/// <summary>
		/// Recupera os dados do usuário pela a sua chave.
		/// </summary>
		/// <param name="userKey">Valor da chave do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		[OperationContract]
		User GetUserByKey(string userKey, bool userIsOnline);

		/// <summary>
		/// Recupera os dados do usuário pelo token informado.
		/// </summary>
		/// <param name="token">Token associado com usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns>Dados do usuário.</returns>
		[OperationContract]
		User GetUserByToken(string token, bool userIsOnline);

		/// <summary>
		/// Atualiza os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		[OperationContract]
		void UpdateUser(User user);

		/// <summary>
		/// Recupera o número de usuário online.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		int GetNumberOfUsersOnline();

		/// <summary>
		/// Recupera os nomes de todos os provedores de identidade.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		string[] GetIdentityProviders();
	}
}
