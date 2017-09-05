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
	/// Formato da senha.
	/// </summary>
	public enum PasswordFormat
	{
		/// <summary>
		/// Password sem formatação.
		/// </summary>
		Clear,
		/// <summary>
		/// Password em formato de hash.
		/// </summary>
		Hashed,
		/// <summary>
		/// Password em formto criptografado.
		/// </summary>
		Encrypted
	}
	/// <summary>
	/// Possiveis situações da criação de um novo usuário.
	/// </summary>
	public enum UserCreateStatus
	{
		/// <summary>
		/// Usuário criado com sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Nome do usuário é inválido.
		/// </summary>
		InvalidUserName,
		/// <summary>
		/// Senha inválida.
		/// </summary>
		InvalidPassword,
		/// <summary>
		/// Pergunta inválida.
		/// </summary>
		InvalidQuestion,
		/// <summary>
		/// Resposta inválida.
		/// </summary>
		InvalidAnswer,
		/// <summary>
		/// Email inválido.
		/// </summary>
		InvalidEmail,
		/// <summary>
		/// Nome de usuário duplicado.
		/// </summary>
		DuplicateUserName,
		/// <summary>
		/// Email duplicado.
		/// </summary>
		DuplicateEmail,
		/// <summary>
		/// Usuário rejeitado.
		/// </summary>
		UserRejected,
		/// <summary>
		/// Identifica que a chave do usuário é inválida no provedor.
		/// </summary>
		InvalidProviderUserKey,
		/// <summary>
		/// Identifica que a chave informada para o usuário já existe no provedor.
		/// </summary>
		DuplicateProviderUserKey,
		/// <summary>
		/// Identifica um erro do provedor.
		/// </summary>
		ProviderError
	}
	/// <summary>
	/// Provedor de acesso a manipulação dos dados do usuário.
	/// </summary>
	public interface IUserProvider
	{
		/// <summary>
		/// Identifica se está habilitado resetar a senha.
		/// </summary>
		bool EnablePasswordReset
		{
			get;
		}

		/// <summary>
		/// Identifica se a recuperação de senha está habilitada.
		/// </summary>
		bool EnablePasswordRetrieval
		{
			get;
		}

		/// <summary>
		/// Número máximo de senhas inválidas.
		/// </summary>
		int MaxInvalidPasswordAttempts
		{
			get;
		}

		/// <summary>
		/// Quantidade minima de caracteres não alfanuméricos.
		/// </summary>
		int MinRequiredNonAlphanumericCharacters
		{
			get;
		}

		/// <summary>
		/// Comprimento minimo requerido para a senha.
		/// </summary>
		int MinRequiredPasswordLength
		{
			get;
		}

		/// <summary>
		/// Formato da senha.
		/// </summary>
		PasswordFormat PasswordFormat
		{
			get;
		}

		/// <summary>
		/// Expressão regular usada para varifica se a senha é forte.
		/// </summary>
		string PasswordStrengthRegularExpression
		{
			get;
		}

		/// <summary>
		/// Identifica se é requerido pergunta e resposta.
		/// </summary>
		bool RequiresQuestionAndAnswer
		{
			get;
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		string Name
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
		/// Cria um novo usuário no sistema.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Senha de acesso.</param>
		/// <param name="fullName">Nome completo do usuário.</param>
		/// <param name="email">Email associado com o usuário.</param>
		/// <param name="passwordQuestion">Pergunta para a senha.</param>
		/// <param name="passwordAnswer">Resposta da pergunta para a recuperação da senha.</param>
		/// <param name="isApproved">Identifica se o usuário é aprovado.</param>
		/// <param name="identityProvider">Nome do provedor de identidade do usuário.</param>
		/// <param name="userKey">Chave que identifica o usuário.</param>
		/// <param name="ignoreCaptcha">Indica se o usuário deve ignorar o controle de captcha</param>
		/// <param name="status">Situação do usuário.</param>
		/// <param name="parameters">Parametros adicionais para a criação do usuário</param>
		/// <returns></returns>
		IUser CreateUser(string username, string password, string fullName, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string identityProvider, string userKey, bool ignoreCaptcha, out UserCreateStatus status, params SecurityParameter[] parameters);

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="oldPassword">Antiga senha.</param>
		/// <param name="newPassword">Nova senha.</param>
		/// <param name="parameters">Parametros da autenticação.</param>
		/// <returns></returns>
		ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword, params SecurityParameter[] parameters);

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Nova senha.</param>
		/// <param name="newPasswordQuestion">Pergunta para a nova senha.</param>
		/// <param name="newPasswordAnswer">Resposta da pergunta da nova senha.</param>
		/// <returns></returns>
		bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer);

		/// <summary>
		/// Reseta a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="answer">Resposta para validar a operação.</param>
		/// <returns></returns>
		string ResetPassword(string username, string answer);

		/// <summary>
		/// Recupera os dados do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		IUser GetUser(string username, bool userIsOnline);

		/// <summary>
		/// Recupera os dados do usuário pela a sua chave.
		/// </summary>
		/// <param name="userKey">Valor da chave do usuário.</param>
		/// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
		/// <returns></returns>
		IUser GetUserByKey(string userKey, bool userIsOnline);

		/// <summary>
		/// Recupera os dados do usuário pelo token informado.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="userIsOnline"></param>
		/// <returns></returns>
		IUser GetUserByToken(string token, bool userIsOnline);

		/// <summary>
		/// Atualiza os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		void UpdateUser(IUser user);

		/// <summary>
		/// Recupera o número de usuário online.
		/// </summary>
		/// <returns></returns>
		int GetNumberOfUsersOnline();

		/// <summary>
		/// Recupera os nomes de todos os provedores do identidade cadastrados no sistema.
		/// </summary>
		/// <returns></returns>
		string[] GetIdentityProviders();

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
		/// <param name="token">Token</param>
		/// <returns></returns>
		IValidateUserResult ValidateToken(string token);

		/// <summary>
		/// Desloga o usuário do sistema
		/// </summary>
		/// <param name="token">Token do usuário</param>
		/// <returns>Sucesso da operação</returns>
		bool LogOut(string token);
	}
}
