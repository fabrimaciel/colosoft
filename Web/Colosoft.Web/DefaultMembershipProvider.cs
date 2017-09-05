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
using System.Web.Security;

namespace Colosoft.Web.Security
{
	/// <summary>
	/// Implementação padrão do provedor do membership.
	/// </summary>
	public class DefaultMembershipProvider : MembershipProvider
	{
		/// <summary>
		/// Converte um usuário do Colosoft para um MembershipUser.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private static MembershipUser Convert(Colosoft.Security.IUser user)
		{
			return new MembershipUser(Colosoft.Security.Membership.Provider.Name, user.UserName, user.UserKey, user.Email, user.PasswordQuestion, null, user.IsApproved, false, user.CreationDate.UtcDateTime, System.DateTime.UtcNow, System.DateTime.UtcNow, user.LastPasswordChangedDate.UtcDateTime, DateTime.MinValue);
		}

		/// <summary>
		/// Converte a situação do Colosoft para um MembershipCreateStatus
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		private static MembershipCreateStatus Convert(Colosoft.Security.UserCreateStatus status)
		{
			switch(status)
			{
			case Colosoft.Security.UserCreateStatus.DuplicateEmail:
				return MembershipCreateStatus.DuplicateEmail;
			case Colosoft.Security.UserCreateStatus.DuplicateProviderUserKey:
				return MembershipCreateStatus.DuplicateProviderUserKey;
			case Colosoft.Security.UserCreateStatus.DuplicateUserName:
				return MembershipCreateStatus.DuplicateUserName;
			case Colosoft.Security.UserCreateStatus.InvalidAnswer:
				return MembershipCreateStatus.InvalidAnswer;
			case Colosoft.Security.UserCreateStatus.InvalidEmail:
				return MembershipCreateStatus.InvalidEmail;
			case Colosoft.Security.UserCreateStatus.InvalidPassword:
				return MembershipCreateStatus.InvalidPassword;
			case Colosoft.Security.UserCreateStatus.InvalidProviderUserKey:
				return MembershipCreateStatus.InvalidProviderUserKey;
			case Colosoft.Security.UserCreateStatus.InvalidQuestion:
				return MembershipCreateStatus.InvalidQuestion;
			case Colosoft.Security.UserCreateStatus.InvalidUserName:
				return MembershipCreateStatus.InvalidUserName;
			case Colosoft.Security.UserCreateStatus.ProviderError:
				return MembershipCreateStatus.ProviderError;
			case Colosoft.Security.UserCreateStatus.Success:
				return MembershipCreateStatus.Success;
			case Colosoft.Security.UserCreateStatus.UserRejected:
				return MembershipCreateStatus.UserRejected;
			default:
				return MembershipCreateStatus.ProviderError;
			}
		}

		/// <summary>
		/// Converte o formato do password do Colosoft para um MembershipPasswordFormat.
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		private static MembershipPasswordFormat Convert(Colosoft.Security.PasswordFormat format)
		{
			switch(format)
			{
			case Colosoft.Security.PasswordFormat.Clear:
				return MembershipPasswordFormat.Clear;
			case Colosoft.Security.PasswordFormat.Encrypted:
				return MembershipPasswordFormat.Encrypted;
			case Colosoft.Security.PasswordFormat.Hashed:
				return MembershipPasswordFormat.Hashed;
			default:
				return MembershipPasswordFormat.Clear;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ApplicationName
		{
			get
			{
				return "Default Membership Provider";
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Altera a senha do usuário associado com o username informado.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			var result = Colosoft.Security.Membership.Provider.ChangePassword(username, oldPassword, newPassword);
			return result.Status == Colosoft.Security.ChangePasswordStatus.Success;
		}

		/// <summary>
		/// Altera a senha do usuário associado com o nome informado usando a resposta da pergunta cadastrada no sistema.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="newPasswordQuestion"></param>
		/// <param name="newPasswordAnswer"></param>
		/// <returns></returns>
		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			return Colosoft.Security.Membership.Provider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
		}

		/// <summary>
		/// Cria um novo usuário.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="email"></param>
		/// <param name="passwordQuestion"></param>
		/// <param name="passwordAnswer"></param>
		/// <param name="isApproved"></param>
		/// <param name="providerUserKey"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			Colosoft.Security.UserCreateStatus userCreateStatus = Colosoft.Security.UserCreateStatus.ProviderError;
			var user = Colosoft.Security.Membership.Provider.CreateUser(username, password, username, email, passwordQuestion, passwordAnswer, isApproved, null, providerUserKey != null ? providerUserKey.ToString() : null, false, out userCreateStatus);
			status = Convert(userCreateStatus);
			return user != null ? Convert(user) : null;
		}

		/// <summary>
		/// Apaga o usuário do sistema.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="deleteAllRelatedData"></param>
		/// <returns></returns>
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera se o sistema possui suporte para resetar a senha.
		/// </summary>
		public override bool EnablePasswordReset
		{
			get
			{
				return Colosoft.Security.Membership.Provider.EnablePasswordReset;
			}
		}

		/// <summary>
		/// Recupera se o sistema possui suporte para recupera a senha do usuário.
		/// </summary>
		public override bool EnablePasswordRetrieval
		{
			get
			{
				return Colosoft.Security.Membership.Provider.EnablePasswordRetrieval;
			}
		}

		/// <summary>
		/// Pesquisa os usuários por email.
		/// </summary>
		/// <param name="emailToMatch"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Pesquisa os usuário pelo nome.
		/// </summary>
		/// <param name="usernameToMatch"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera todos os usuário cadastrados no sistema.
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o número de usuário online.
		/// </summary>
		/// <returns></returns>
		public override int GetNumberOfUsersOnline()
		{
			return Colosoft.Security.Membership.Provider.GetNumberOfUsersOnline();
		}

		/// <summary>
		/// Recupera a senha do usuário com base no nome e na resposta da pergunta secreta.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="answer"></param>
		/// <returns></returns>
		public override string GetPassword(string username, string answer)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera os dados do usuário.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="userIsOnline"></param>
		/// <returns></returns>
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			var user = Colosoft.Security.Membership.Provider.GetUser(username, userIsOnline);
			return user != null ? Convert(user) : null;
		}

		/// <summary>
		/// Recupera os dados do usuário.
		/// </summary>
		/// <param name="providerUserKey"></param>
		/// <param name="userIsOnline"></param>
		/// <returns></returns>
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			var user = Colosoft.Security.Membership.Provider.GetUserByKey(providerUserKey == null ? null : providerUserKey.ToString(), userIsOnline);
			return user != null ? Convert(user) : null;
		}

		/// <summary>
		/// Rucupera o nome do usuário pelo email informado.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public override string GetUserNameByEmail(string email)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o número máximo de tentativas inválidas de senha.
		/// </summary>
		public override int MaxInvalidPasswordAttempts
		{
			get
			{
				return Colosoft.Security.Membership.Provider.MaxInvalidPasswordAttempts;
			}
		}

		/// <summary>
		/// Recupera o minimo de caracteres não alfa númerico.
		/// </summary>
		public override int MinRequiredNonAlphanumericCharacters
		{
			get
			{
				return Colosoft.Security.Membership.Provider.MinRequiredNonAlphanumericCharacters;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int MinRequiredPasswordLength
		{
			get
			{
				return Colosoft.Security.Membership.Provider.MinRequiredPasswordLength;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int PasswordAttemptWindow
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Formato da senha.
		/// </summary>
		public override MembershipPasswordFormat PasswordFormat
		{
			get
			{
				return Convert(Colosoft.Security.Membership.Provider.PasswordFormat);
			}
		}

		/// <summary>
		/// Pessoa da senha.
		/// </summary>
		public override string PasswordStrengthRegularExpression
		{
			get
			{
				return Colosoft.Security.Membership.Provider.PasswordStrengthRegularExpression;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool RequiresQuestionAndAnswer
		{
			get
			{
				return Colosoft.Security.Membership.Provider.RequiresQuestionAndAnswer;
			}
		}

		/// <summary>
		/// Identfica se é requerido um email único.
		/// </summary>
		public override bool RequiresUniqueEmail
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="answer"></param>
		/// <returns></returns>
		public override string ResetPassword(string username, string answer)
		{
			return Colosoft.Security.Membership.Provider.ResetPassword(username, answer);
		}

		/// <summary>
		/// Desbloqueia o usuário.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public override bool UnlockUser(string userName)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Atualiza os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		public override void UpdateUser(MembershipUser user)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Valida os dados do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Senha de acesso.</param>
		/// <returns></returns>
		public override bool ValidateUser(string username, string password)
		{
			var result = Colosoft.Security.Membership.Provider.ValidateUser(username, password);
			return result.Status == Colosoft.Security.AuthenticationStatus.Success;
		}
	}
}
