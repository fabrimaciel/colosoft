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
using Colosoft.Security.CaptchaSupport;
using System.Resources;
using System.Globalization;
using System.Reflection;
using Colosoft.Properties;

namespace Colosoft.Security.Authentication
{
	/// <summary>
	/// Classe abstrata com tratamentos básicos de usuário
	/// </summary>
	public abstract class Authentication : IUserProvider
	{
		private static Dictionary<string, byte> _invalidLogin;

		private Dictionary<string, IAuthenticate> _providers;

		/// <summary>
		/// Controla os logins inválidos para verificar a necessidade ou não de captcha
		/// </summary>
		private Dictionary<string, byte> InvalidLogin
		{
			get
			{
				if(_invalidLogin == null)
					_invalidLogin = new Dictionary<string, byte>();
				return _invalidLogin;
			}
		}

		/// <summary>
		/// Retorna a instância de validação de senha
		/// </summary>
		private IPasswordValidate ValidatePassword
		{
			get
			{
				return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IPasswordValidate>();
			}
		}

		/// <summary>
		/// Construtor padrão da classe
		/// </summary>
		public Authentication()
		{
			_providers = new Dictionary<string, IAuthenticate>();
		}

		/// <summary>
		/// Inicia o processo de redefinição de senha
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <returns>Resultado do processo</returns>
		public virtual ResetPasswordProcessResult RequestPasswordReset(string userName)
		{
			var currentUser = GetUser(userName, false) as Security.Authentication.IAutheticableUser;
			if((currentUser == null) || (String.IsNullOrEmpty(currentUser.UserKey)))
			{
				return new ResetPasswordProcessResult() {
					Success = false,
					Message = Resources.Invalid_UsernameOrPassword,
				};
			}
			IdentityProvider identityProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IIdentityProviderManager>().GetProviderById(currentUser.IdentityProviderId);
			var identityProviderExists = false;
			lock (_providers)
				identityProviderExists = _providers.ContainsKey(identityProvider.FullName);
			if(!identityProviderExists)
			{
				try
				{
					var authenticateInstance = Activator.CreateInstance(identityProvider.Type) as IAuthenticate;
					lock (_providers)
					{
						if(!_providers.ContainsKey(identityProvider.FullName))
						{
							_providers.Add(identityProvider.FullName, authenticateInstance);
							authenticateInstance = null;
						}
					}
					if(authenticateInstance != null && authenticateInstance is IDisposable)
						((IDisposable)authenticateInstance).Dispose();
				}
				catch(Exception ex)
				{
					return new ResetPasswordProcessResult() {
						Success = false,
						Message = ex.Message
					};
				}
			}
			IAuthenticate provider = null;
			lock (_providers)
				provider = _providers[identityProvider.FullName];
			if(provider.CanResetPassword)
			{
				return provider.RequestPasswordReset(userName);
			}
			else
			{
				return new ResetPasswordProcessResult() {
					Success = false,
					Message = Resources.Error_UserCantResetPassword
				};
			}
		}

		/// <summary>
		/// Valida o usuário no provedor de identidade
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="parameters">Demais informações necessárias</param>
		/// <returns></returns>
		public virtual IValidateUserResult ValidateUser(string userName, string password, SecurityParameter[] parameters)
		{
			IValidateUserResult result = new ValidateUserResult();
			var currentUser = GetUser(userName, false) as Security.Authentication.IAutheticableUser;
			if((currentUser == null) || (String.IsNullOrEmpty(currentUser.UserKey)))
			{
				return new ValidateUserResult() {
					Status = AuthenticationStatus.InvalidUserNameOrPassword,
					Message = Resources.Invalid_UsernameOrPassword,
					User = null
				};
			}
			var identityProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IIdentityProviderManager>().GetProviderById(currentUser.IdentityProviderId);
			if(identityProvider == null)
			{
				return new ValidateUserResult() {
					Status = AuthenticationStatus.UnknownError,
					Message = string.Format(Resources.Error_IdentityProviderNotFound, userName),
					User = null
				};
			}
			if(currentUser != null)
			{
				var captcha = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ICaptcha>();
				if(!currentUser.IgnoreCaptcha)
				{
					if((InvalidLogin.ContainsKey(userName)) && (InvalidLogin[userName] >= 3))
					{
						string captchaStr = GetParameterValue(parameters, "CaptchaString") ?? String.Empty;
						string guidStr = GetParameterValue(parameters, "CaptchaGuid") ?? String.Empty;
						if((!String.IsNullOrEmpty(captchaStr)) && (!String.IsNullOrEmpty(guidStr)))
						{
							Guid guid = new Guid(guidStr);
							if(!captcha.IsCorrect(guid, captchaStr))
							{
								result.Status = AuthenticationStatus.InvalidCaptcha;
								result.Message = Resources.Invalid_Captcha;
								result.Captcha = CreateCaptcha(parameters, captcha);
								return result;
							}
						}
						else
						{
							result.Status = AuthenticationStatus.CaptchaRequired;
							result.Message = Resources.Captcha_Necessary;
							result.Captcha = CreateCaptcha(parameters, captcha);
							return result;
						}
					}
				}
				var identityProviderExists = false;
				lock (_providers)
					identityProviderExists = _providers.ContainsKey(identityProvider.FullName);
				if(!identityProviderExists)
				{
					if(identityProvider.Type == null)
						return new ValidateUserResult() {
							Status = AuthenticationStatus.ErrorInValidate,
							Message = ResourceMessageFormatter.Create(() => Properties.Resources.Error_IdentityProviderTypeUndefined).Format()
						};
					try
					{
						var authenticateInstance = Activator.CreateInstance(identityProvider.Type) as IAuthenticate;
						lock (_providers)
						{
							if(!_providers.ContainsKey(identityProvider.FullName))
							{
								_providers.Add(identityProvider.FullName, authenticateInstance);
								authenticateInstance = null;
							}
						}
						if(authenticateInstance != null && authenticateInstance is IDisposable)
							((IDisposable)authenticateInstance).Dispose();
					}
					catch(Exception ex)
					{
						return new ValidateUserResult() {
							Status = AuthenticationStatus.ErrorInValidate,
							Message = ex.Message
						};
					}
				}
				IAuthenticate provider = null;
				lock (_providers)
					provider = _providers[identityProvider.FullName];
				result = provider.ValidateUser(userName, password, parameters);
				if(result.Status == AuthenticationStatus.Success)
				{
					if(InvalidLogin.ContainsKey(userName))
					{
						InvalidLogin.Remove(userName);
					}
					if((result.ExpireDate.HasValue) && (result.ExpireDate.Value > DateTime.Today) && (identityProvider.WarningDays > 0) && (System.DateTime.Today.AddDays(identityProvider.WarningDays) >= result.ExpireDate))
					{
						result.Message = String.Format(Resources.Warning_Password, result.ExpireDate.Value.Subtract(ServerData.GetDate()).Days);
						result.Status = AuthenticationStatus.PasswordWarning;
					}
					Security.ITokenProvider tokenFramework = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITokenProvider>();
					string oldToken = tokenFramework.GetToken(Convert.ToInt32(currentUser.UserKey));
					if(!String.IsNullOrEmpty(oldToken))
					{
						if((GetParameterValue(parameters, "ForceToken") ?? "False").ToUpper().Equals("TRUE"))
						{
							tokenFramework.Close(oldToken);
						}
						else
						{
							result.Status = AuthenticationStatus.DuplicateToken;
							result.Message = Resources.Error_TokenDuplicate;
							return result;
						}
					}
					do
					{
						result.Token = tokenFramework.Create(256);
					}
					while (tokenFramework.Check(result.Token).Success);
					result.User = currentUser;
					tokenFramework.CloseUserTokens(Convert.ToInt32(result.User.UserKey));
					if(!tokenFramework.Insert(result.Token, Convert.ToInt32(result.User.UserKey)))
					{
						result.Status = AuthenticationStatus.ErrorTokenControl;
						result.Message = Resources.Error_TokenCreate;
						result.User = null;
					}
				}
				else if(result.Status == AuthenticationStatus.InvalidUserNameOrPassword)
				{
					if(!currentUser.IgnoreCaptcha)
					{
						if(!InvalidLogin.ContainsKey(userName))
						{
							InvalidLogin.Add(userName, 1);
						}
						else
						{
							InvalidLogin[userName]++;
						}
						if(InvalidLogin[userName] >= 3)
						{
							result.Captcha = CreateCaptcha(parameters, captcha);
						}
					}
				}
			}
			else
			{
				result.Status = AuthenticationStatus.InvalidUserNameOrPassword;
				result.Message = Resources.Invalid_UsernameOrPassword;
				result.User = null;
			}
			return result;
		}

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public virtual IValidateUserResult ValidateToken(string token)
		{
			IValidateUserResult result = new ValidateUserResult();
			var currentUser = GetUserByToken(token, false) as Security.Authentication.IAutheticableUser;
			if((currentUser == null) || (String.IsNullOrEmpty(currentUser.UserKey)))
			{
				return new ValidateUserResult() {
					Status = AuthenticationStatus.InvalidUserNameOrPassword,
					Message = Resources.Invalid_UsernameOrPassword,
					User = null
				};
			}
			var identityProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IIdentityProviderManager>().GetProviderById(currentUser.IdentityProviderId);
			if(identityProvider == null)
			{
				return new ValidateUserResult() {
					Status = AuthenticationStatus.UnknownError,
					Message = string.Format(Resources.Error_IdentityProviderNotFound, token),
					User = null
				};
			}
			if(currentUser != null)
			{
				var identityProviderExists = false;
				lock (_providers)
					identityProviderExists = _providers.ContainsKey(identityProvider.FullName);
				if(!identityProviderExists)
				{
					try
					{
						var authenticateInstance = Activator.CreateInstance(identityProvider.Type) as IAuthenticate;
						lock (_providers)
						{
							if(!_providers.ContainsKey(identityProvider.FullName))
							{
								_providers.Add(identityProvider.FullName, authenticateInstance);
								authenticateInstance = null;
							}
						}
						if(authenticateInstance != null && authenticateInstance is IDisposable)
							((IDisposable)authenticateInstance).Dispose();
					}
					catch(Exception ex)
					{
						return new ValidateUserResult() {
							Status = AuthenticationStatus.ErrorInValidate,
							Message = ex.Message
						};
					}
				}
				IAuthenticate provider = null;
				lock (_providers)
					provider = _providers[identityProvider.FullName];
				result = provider.ValidateToken(token);
			}
			else
			{
				result.Status = AuthenticationStatus.InvalidUserNameOrPassword;
				result.Message = Resources.Invalid_UsernameOrPassword;
				result.User = null;
			}
			return result;
		}

		/// <summary>
		/// Altera o password do usuário
		/// </summary>
		/// <param name="userName">usuário</param>
		/// <param name="oldPassword">senha atual</param>
		/// <param name="newPassword">nova senha</param>
		/// <param name="parameters">demais parametros</param>
		/// <returns>resultado da operação</returns>
		public virtual ChangePasswordResult ChangePassword(string userName, string oldPassword, string newPassword, SecurityParameter[] parameters)
		{
			if(ValidatePassword != null)
			{
				PasswordValidateResult validateResult = ValidatePassword.IsValid(newPassword);
				if(!validateResult.IsOk)
				{
					return new ChangePasswordResult() {
						Status = ChangePasswordStatus.Error,
						Message = validateResult.Message
					};
				}
			}
			IValidateUserResult authenticateResult = ValidateUser(userName, oldPassword, parameters);
			if((authenticateResult.Status == AuthenticationStatus.Success) || (authenticateResult.Status == AuthenticationStatus.PasswordWarning) || (authenticateResult.Status == AuthenticationStatus.PasswordExpired))
			{
				if(!_providers.ContainsKey(authenticateResult.User.IdentityProvider))
				{
					try
					{
						_providers.Add(authenticateResult.User.IdentityProvider, Activator.CreateInstance(Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IIdentityProviderManager>().GetProviderByName(authenticateResult.User.IdentityProvider).Type) as IAuthenticate);
					}
					catch(Exception ex)
					{
						return new ChangePasswordResult() {
							Status = ChangePasswordStatus.Error,
							Message = ex.Message
						};
					}
				}
				return _providers[authenticateResult.User.IdentityProvider].ChangePassword(userName, oldPassword, newPassword, parameters);
			}
			else
			{
				ChangePasswordStatus status = ChangePasswordStatus.Error;
				switch(authenticateResult.Status)
				{
				case AuthenticationStatus.InvalidCaptcha:
					status = ChangePasswordStatus.InvalidCaptcha;
					break;
				case AuthenticationStatus.CaptchaRequired:
					status = ChangePasswordStatus.CaptchaRequired;
					break;
				}
				return new ChangePasswordResult() {
					Status = status,
					Message = authenticateResult.Message,
					Captcha = authenticateResult.Captcha
				};
			}
		}

		/// <summary>
		/// Altera o password do usuário
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="newPasswordQuestion"></param>
		/// <param name="newPasswordAnswer"></param>
		/// <returns></returns>
		public virtual bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			IValidateUserResult authenricateResult = ValidateUser(username, password, null);
			if((authenricateResult.Status == AuthenticationStatus.Success) || (authenricateResult.Status == AuthenticationStatus.PasswordWarning) || (authenricateResult.Status == AuthenticationStatus.PasswordExpired))
			{
				var changedUser = authenricateResult.User as Security.Authentication.IAutheticableUser;
				changedUser.PasswordAnswer = newPasswordAnswer;
				changedUser.PasswordQuestion = newPasswordQuestion;
				UpdateUser(changedUser);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Informa se o password pode ou não ser resetado
		/// </summary>
		public abstract bool EnablePasswordReset
		{
			get;
		}

		/// <summary>
		/// Informa se o password pode ou não ser recuperado
		/// </summary>
		public abstract bool EnablePasswordRetrieval
		{
			get;
		}

		/// <summary>
		/// Retorna o número de usuários online
		/// </summary>
		/// <returns></returns>
		public abstract int GetNumberOfUsersOnline();

		/// <summary>
		/// Retorna o usuário
		/// </summary>
		/// <param name="username">Nome do usuário</param>
		/// <param name="userIsOnline">Flag que indica se o usuário está online</param>
		/// <returns></returns>
		public abstract Security.IUser GetUser(string username, bool userIsOnline);

		/// <summary>
		/// Número máximo de tentativas de autenticaçãoi
		/// </summary>
		public abstract int MaxInvalidPasswordAttempts
		{
			get;
		}

		/// <summary>
		/// Número mínimo de caracteres alfanuméricos na senha
		/// </summary>
		public abstract int MinRequiredNonAlphanumericCharacters
		{
			get;
		}

		/// <summary>
		/// Tamanho mínimo necessário para a senha
		/// </summary>
		public abstract int MinRequiredPasswordLength
		{
			get;
		}

		/// <summary>
		/// Formatador da senha
		/// </summary>
		public abstract PasswordFormat PasswordFormat
		{
			get;
		}

		/// <summary>
		/// Expreessão regular para validar senha
		/// </summary>
		public abstract string PasswordStrengthRegularExpression
		{
			get;
		}

		/// <summary>
		/// Informa se é obrigatório o preenchimento da resposta da pergunta secreta
		/// </summary>
		public abstract bool RequiresQuestionAndAnswer
		{
			get;
		}

		/// <summary>
		/// Limpa a senha
		/// </summary>
		/// <param name="username">Nome de usuário</param>
		/// <param name="answer">Resposta da pergunta secreta</param>
		/// <returns></returns>
		public string ResetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Desloga o usuário do sistema
		/// </summary>
		/// <param name="token">Token do usuário</param>
		/// <returns>Sucesso da operação</returns>
		public virtual bool LogOut(string token)
		{
			return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITokenProvider>().Close(token);
		}

		/// <summary>
		/// Atualiza o cadastro do usuário
		/// </summary>
		/// <param name="user">Usuário</param>
		public abstract void UpdateUser(Security.IUser user);

		/// <summary>
		/// Cria um novo usuário no sistema.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="password">Senha de acesso.</param>
		/// <param name="email">Email associado com o usuário.</param>
		/// <param name="passwordQuestion">Pergunta para a senha.</param>
		/// <param name="passwordAnswer">Resposta da pergunta para a recuperação da senha.</param>
		/// <param name="isApproved">Identifica se o usuário é aprovado.</param>
		/// <param name="identityProvider">Nome do provedor de identidade do usuário.</param>
		/// <param name="userKey">Chave que identifica o usuário.</param>
		/// <param name="ignoreCaptcha">Indica se o usuário irá ignorar o controle por captcha</param>
		/// <param name="status">Situação do usuário.</param>
		/// <param name="fullname">Nome do usuário</param>
		/// <param name="parameters">Parametros adicionais para a criação do usuário</param>
		/// <returns></returns>
		public virtual Security.IUser CreateUser(string username, string password, string fullname, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string identityProvider, string userKey, bool ignoreCaptcha, out UserCreateStatus status, params SecurityParameter[] parameters)
		{
			IIdentityProviderManager providerFlow = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IIdentityProviderManager>();
			var result = GetUser(username, false) as Security.Authentication.IAutheticableUser;
			if((result != null) && (!String.IsNullOrEmpty(result.UserKey)))
			{
				status = UserCreateStatus.DuplicateUserName;
				result = null;
			}
			else
			{
				result.UserName = username;
				result.Email = email;
				result.FullName = fullname;
				result.IdentityProvider = identityProvider;
				result.LastPasswordChangedDate = ServerData.GetDateTime();
				result.PasswordAnswer = passwordAnswer;
				result.PasswordQuestion = passwordQuestion;
				result.UserName = username;
				result.IdentityProviderId = providerFlow.GetProviderByName(identityProvider).IdentityProviderId;
				if(InsertUser(result))
				{
					status = UserCreateStatus.Success;
					result = GetUser(username, false) as Security.Authentication.IAutheticableUser;
					IAuthenticate provider = Activator.CreateInstance((providerFlow.GetProviderByName(identityProvider).Type)) as IAuthenticate;
					if(provider.CanCreateUser)
					{
						if(provider.CreateNewUser(result, password, parameters).Success)
						{
							status = UserCreateStatus.Success;
						}
						else
						{
							status = UserCreateStatus.DuplicateProviderUserKey;
						}
					}
				}
				else
				{
					status = UserCreateStatus.ProviderError;
				}
			}
			return result;
		}

		/// <summary>
		/// Busca o usuário pela chave
		/// </summary>
		/// <param name="userKey">Chave</param>
		/// <param name="userIsOnline"></param>
		/// <returns>Objeto</returns>
		public abstract Security.IUser GetUserByKey(string userKey, bool userIsOnline);

		/// <summary>
		/// Recupera os dados do usuário pelo token informado.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="userIsOnline"></param>
		/// <returns></returns>
		public Security.IUser GetUserByToken(string token, bool userIsOnline)
		{
			var result = Tokens.Check(token);
			if(result == null || !result.Success)
				return null;
			return GetUserByKey(result.UserId.ToString(), true);
		}

		/// <summary>
		/// Nome do usuário
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Busca os provedores de acesso disponíveis
		/// </summary>
		/// <returns>Lista com os nomes</returns>
		public abstract string[] GetIdentityProviders();

		/// <summary>
		/// Persiste o usuário na base
		/// </summary>
		/// <param name="user">Usuário a persistir</param>
		/// <returns></returns>
		protected abstract bool InsertUser(Security.IUser user);

		private string GetParameterValue(SecurityParameter[] parameters, string key)
		{
			try
			{
				string result = parameters.Where(f => f.Name.ToUpper().Equals(key.ToUpper())).Select(f => f.Value).FirstOrDefault();
				return result;
			}
			catch
			{
				return null;
			}
		}

		private CaptchaInfo CreateCaptcha(SecurityParameter[] parameters, ICaptcha provider)
		{
			int captchaHeight = Convert.ToInt32(GetParameterValue(parameters, "CaptchaHeight") ?? "100");
			int captchaWidth = Convert.ToInt32(GetParameterValue(parameters, "CaptchaWidth") ?? "100");
			int captchaNumChars = Convert.ToInt32(GetParameterValue(parameters, "CaptchaNumChars") ?? "5");
			CaptchaSettings settings = new CaptchaSettings(captchaHeight, captchaWidth, "GenericSerif", captchaNumChars, false);
			return provider.Generate(settings);
		}
	}
}
