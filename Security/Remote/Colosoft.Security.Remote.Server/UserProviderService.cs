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
	/// Implementação do serviço do provedor de usuário.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
	public class UserProviderService : IUserProviderService
	{
		/// <summary>
		/// Instancia do provedor dos usuários.
		/// </summary>
		protected IUserProvider UserProvider
		{
			get
			{
				return Membership.Provider;
			}
		}

		/// <summary>
		/// Recupera as informações do provedor.
		/// </summary>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public UserProviderInfo GetProviderInfo()
		{
			return new UserProviderInfo(UserProvider);
		}

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
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public CreateUserResult CreateUser(string username, string password, string fullname, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string userKey, string identityProvider, bool ignoreCaptcha, Security.SecurityParameter[] parameters)
		{
			UserCreateStatus status = UserCreateStatus.ProviderError;
			var user = UserProvider.CreateUser(username, password, fullname, email, passwordQuestion, passwordAnswer, isApproved, userKey, identityProvider, ignoreCaptcha, out status, parameters);
			return new CreateUserResult {
				User = user != null ? new User(user) : null,
				Status = status
			};
		}

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="oldPassword">Antiga senha.</param>
		/// <param name="newPassword">Nova senha.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword, SecurityParameter[] parameters)
		{
			return UserProvider.ChangePassword(username, oldPassword, newPassword);
		}

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Nova senha.</param>
		/// <param name="newPasswordQuestion">Pergunta para a nova senha.</param>
		/// <param name="newPasswordAnswer">Resposta da pergunta da nova senha.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			return UserProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
		}

		/// <summary>
		/// Reseta a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="answer">Resposta para validar a operação.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public string ResetPassword(string username, string answer)
		{
			return UserProvider.ResetPassword(username, answer);
		}

		/// <summary>
		/// Recupera os dados do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public User GetUser(string username, bool userIsOnline)
		{
			var user = UserProvider.GetUser(username, userIsOnline);
			return user == null ? null : new User(user);
		}

		/// <summary>
		/// Recupera os dados do usuário pela a sua chave.
		/// </summary>
		/// <param name="userKey">Valor da chave do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public User GetUserByKey(string userKey, bool userIsOnline)
		{
			var user = UserProvider.GetUserByKey(userKey, userIsOnline);
			return user == null ? null : new User(user);
		}

		/// <summary>
		/// Recupera os dados do usuário pelo token informado.
		/// </summary>
		/// <param name="token">Token associado com usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns>Dados do usuário.</returns>
		public User GetUserByToken(string token, bool userIsOnline)
		{
			var user = UserProvider.GetUserByToken(token, userIsOnline);
			return user == null ? null : new User(user);
		}

		/// <summary>
		/// Atualiza os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void UpdateUser(User user)
		{
			UserProvider.UpdateUser(user);
		}

		/// <summary>
		/// Recupera o número de usuário online.
		/// </summary>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public int GetNumberOfUsersOnline()
		{
			return UserProvider.GetNumberOfUsersOnline();
		}

		/// <summary>
		/// Recupera os nomes de todos os provedores de identidade.
		/// </summary>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public string[] GetIdentityProviders()
		{
			return UserProvider.GetIdentityProviders();
		}

		/// <summary>
		/// Valida credenciais
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="parameters">Demais parâmetros</param>
		/// <returns>Resultado da autenticação</returns>
		public ValidateUserResultWrapper ValidateUser(string userName, string password, SecurityParameter[] parameters)
		{
			throw new NotSupportedException();
		}
	}
}
