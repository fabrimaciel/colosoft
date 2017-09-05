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
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using System.ComponentModel.Composition;

namespace Colosoft.Security.Remote.Client
{
    /// <summary>
    /// Implementação do provedor de usuário remoto.
    /// </summary>
    [Export(typeof(IUserProvider))]
    public class RemoteUserProvider : IUserProvider, IDisposable
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço do provedor do usuário.
        /// </summary>
        const string UserProviderServiceClientConfigurationName = "UserProviderService";

        /// <summary>
        /// Nome da configuração do serviço de autenticaçã.
        /// </summary>
        const string AuthenticationServiceClientConfigurationName = "AuthenticationService";

        #endregion

        #region Local Variables

        private readonly string _userProviderClientUid = Guid.NewGuid().ToString();
        private readonly string _authenticationClientUid = Guid.NewGuid().ToString();

        private UserProviderInfoRefreshAction _infoAction;

        /// <summary>
        /// Dados das credenciais usadas no UserProviderService.
        /// </summary>
        private string _userProviderClientUserName = "anonymous";
        private string _userProviderClientPassword = "anonymous";

        #endregion

        #region Events

        /// <summary>
        /// Evento acionado quando estiver configurando o cliente de autenticação.
        /// </summary>
        public event ConfigureRemoteClientEventHandler ConfiguringAuthenticationClient;

        /// <summary>
        /// Evento acionado quando estiver configurando o cliente do provedor de usuário.
        /// </summary>
        public event ConfigureRemoteClientEventHandler ConfiguringUserProviderClient;

        #endregion

        #region Properties

        /// <summary>
        /// Recupera a instancia do cliente do serviço.
        /// </summary>
        internal ServerHost.UserProviderServiceClient UserProviderClient
        {
            get 
            {
                return Net.ServiceClientsManager.Current.Get<ServerHost
                    .UserProviderServiceClient>(_userProviderClientUid);
            }
        }

        /// <summary>
        /// Recupera a instancia do cliente do serviço de authenticação.
        /// </summary>
        internal AuthenticationHost.AuthenticationServiceClient AuthenticationClient
        {
            get
            {
                return Net.ServiceClientsManager.Current
                    .Get<AuthenticationHost.AuthenticationServiceClient>(_authenticationClientUid);
            }
        }

        /// <summary>
        /// Recupera as informações do provedor.
        /// </summary>
        internal ServerHost.UserProviderInfo Info
        {
            get
            {
                if (_infoAction == null)
                    _infoAction = new UserProviderInfoRefreshAction(this);

                return _infoAction.UserProviderInfo;
            }
        }

        /// <summary>
        /// Identifica se o provedor suporta resetar a senha.
        /// </summary>
        public bool EnablePasswordReset
        {
            get { return Info.EnablePasswordReset; }
        }

        /// <summary>
        /// Identifica se o provedor suporte recupera a senha.
        /// </summary>
        public bool EnablePasswordRetrieval
        {
            get { return Info.EnablePasswordRetrieval; }
        }

        /// <summary>
        /// Recupera a quantidade maxima de tentativas de senha inválida.
        /// </summary>
        public int MaxInvalidPasswordAttempts
        {
            get { return Info.MaxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Recupera a quantidade minima requerida de caracteres alfanumericos.
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters
        {
            get { return Info.MinRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Recupera o tamanho minimo da senha.
        /// </summary>
        public int MinRequiredPasswordLength
        {
            get { return Info.MinRequiredPasswordLength; }
        }

        /// <summary>
        /// Formatação da senha usada pelo provedor.
        /// </summary>
        public PasswordFormat PasswordFormat
        {
            get { return Convert(Info.PasswordFormat); }
        }

        /// <summary>
        /// Expressão regular usada para verificar se a senha é forte.
        /// </summary>
        public string PasswordStrengthRegularExpression
        {
            get { return Info.PasswordStrengthRegularExpression; }
        }

        /// <summary>
        /// Identifica que o provedor exige pergunta de sergurança para seus usuário.
        /// </summary>
        public bool RequiresQuestionAndAnswer
        {
            get { return Info.RequiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Nome do provedor.
        /// </summary>
        public string Name
        {
            get { return "RemoteUserProvider"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public RemoteUserProvider()
        {
            Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Net.ServiceClientsManager.Current.Register(_userProviderClientUid,
                () =>
                {
                    var serviceAddress = Net.ServicesConfiguration.Current[UserProviderServiceClientConfigurationName];
                    var client = new ServerHost.UserProviderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());

                    //client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                    client.ClientCredentials.UserName.UserName = _userProviderClientUserName;
                    client.ClientCredentials.UserName.Password = _userProviderClientPassword;

                    Colosoft.Net.SecurityTokenBehavior.Register(client.Endpoint);

                    if (ConfiguringUserProviderClient != null)
                        ConfiguringUserProviderClient(this, new ConfigureRemoteClientEventArgs(client, client.Endpoint));

                    return client;
                });

            Net.ServiceClientsManager.Current.Register(_authenticationClientUid,
                () =>
                {
                    var serviceAddress = Net.ServicesConfiguration.Current[AuthenticationServiceClientConfigurationName];
                    var client = new AuthenticationHost.AuthenticationServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());

                    if (ConfiguringAuthenticationClient != null)
                        ConfiguringAuthenticationClient(this, new ConfigureRemoteClientEventArgs(client, client.Endpoint));

                    return client;
                });
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Método acionado quando o endereço do serviço for alterado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServicesConfigurationUpdated(object sender, Net.ServicesConfigurationActionEventArgs e)
        {
            if (e.ServiceName == UserProviderServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_userProviderClientUid);
            else if (e.ServiceName == AuthenticationServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_authenticationClientUid);
        }

        /// <summary>
        /// Converte os dados do usuário do sistema para o usuário do serviço.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static ServerHost.User Convert(IUser user)
        {
            return new ServerHost.User
            {
                Email = user.Email,
                FullName = user.FullName,
                //IsOnline = user.IsOnline,
                //LastActivityDate = user.LastActivityDate,
                //LastLoginDate = user.LastLoginDate,
                LastPasswordChangedDate = user.LastPasswordChangedDate,
                PasswordQuestion = user.PasswordQuestion,
                UserKey = user.UserKey,
                UserName = user.UserName
            };
        }

        /// <summary>
        /// Converte a situação vinda do serviço para a do sistema.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static UserCreateStatus Convert(ServerHost.UserCreateStatus status)
        {
            UserCreateStatus result = UserCreateStatus.ProviderError;
            Enum.TryParse<UserCreateStatus>(status.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Converte a situação vinda do serviço para a do sistema.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static AuthenticationStatus Convert(AuthenticationHost.AuthenticationStatus status)
        {
            AuthenticationStatus result = AuthenticationStatus.ErrorInValidate;
            Enum.TryParse<AuthenticationStatus>(status.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Converte o resultado vindo do serviço para o do sistema.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static ChangePasswordResult Convert(AuthenticationHost.ChangePasswordResult result)
        {
            if (result == null) return null;
            return new ChangePasswordResult
            {
                Message = result.Message,
                Status = result.Status
            };
        }

        /// <summary>
        /// Converte os dados do serviço para os dados do sistema.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static Colosoft.Security.CaptchaSupport.CaptchaInfo Convert(AuthenticationHost.CaptchaInfo info)
        {
            if (info == null) return null;
            return new CaptchaSupport.CaptchaInfo
            {
                Image = info.Image,
                Uid = info.Uid
            };
        }

        /// <summary>
        /// Converte o resultado vindo do serviço para o do sistema.
        /// </summary>
        /// <param name="validateUserResult"></param>
        /// <returns></returns>
        private static IValidateUserResult Convert(AuthenticationHost.ValidateUserResult validateUserResult)
        {
            if (validateUserResult == null) return null;
            return new Wrappers.ValidateUserResultWrapper
            {
                Captcha = Convert(validateUserResult.Captcha),
                ExpireDate = validateUserResult.ExpireDate,
                Message = validateUserResult.Message,
                Status = Convert(validateUserResult.Status),
                Token = validateUserResult.Token,
                User = validateUserResult.User != null ? new Wrappers.UserWrapper(validateUserResult.User) : null,
                UserProviderServiceAddress = validateUserResult.UserProviderServiceAddress,
                ProfileProviderServiceAddress = validateUserResult.ProfileProviderServiceAddress,
                ServiceAddressProviderServiceAddress = validateUserResult.ServiceAddressProviderServiceAddress,
                Error = validateUserResult is ValidateUserResultError ? ((ValidateUserResultError)validateUserResult).Error : null
            };
        }

        /// <summary>
        /// Converte
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static PasswordFormat Convert(ServerHost.PasswordFormat format)
        {
            PasswordFormat result = PasswordFormat.Clear;
            Enum.TryParse<PasswordFormat>(format.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Gera o hash da senha informada.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string HashPassword(string password)
        {
            return password;

            /*if (string.IsNullOrEmpty(password)) return null;
            StringBuilder result = new StringBuilder();
            using (System.Security.Cryptography.MD5CryptoServiceProvider csp = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] msg = Encoding.Default.GetBytes(password);
                byte[] hash = csp.ComputeHash(msg);
                for (int i = 0; i < hash.Length; i++)
                    result.Append(hash[i].ToString("x2"));
            }
            return result.ToString();*/
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Altera se senha do usuário informado.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword, params SecurityParameter[] parameters)
        {
            return Convert(AuthenticationClient.ChangePassword(username, HashPassword(oldPassword), HashPassword(newPassword), parameters));
        }

        /// <summary>
        /// Altera a senha a pergunta de segurança do usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return UserProviderClient.ChangePasswordQuestionAndAnswer(username, HashPassword(password), newPasswordQuestion, newPasswordAnswer);
        }

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
        public IUser CreateUser(string username, string password, string fullName, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string identityProvider, string userKey, bool ignoreCaptcha, out UserCreateStatus status, params SecurityParameter[] parameters)
        {
            var result = UserProviderClient.CreateUser(username, HashPassword(password), fullName, email, passwordQuestion, passwordAnswer, isApproved, userKey, identityProvider, ignoreCaptcha, parameters);

            status = Convert(result.Status);
            return result.User == null ? null : new Wrappers.UserWrapper(result.User);
        }

        /// <summary>
        /// Recupera os dados do usuário pelo nome informado.
        /// </summary>
        /// <param name="username">Nome do usuário.</param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public IUser GetUser(string username, bool userIsOnline)
        {
            // Recupera os dados do usuário
            var user = UserProviderClient.GetUser(username, userIsOnline);
            return user == null ? null : new Wrappers.UserWrapper(user);
        }

        /// <summary>
        /// Recupera os dados do usuário pela chave informada.
        /// </summary>
        /// <param name="userKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public IUser GetUserByKey(string userKey, bool userIsOnline)
        {
            return null;
        }

        /// <summary>
        /// Recupera os dados do usuário pelo token informado.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public IUser GetUserByToken(string token, bool userIsOnline)
        {
            if (!Net.ServicesConfiguration.Current.Contains(UserProviderServiceClientConfigurationName))
                return null;

            // Verifica se o token é válido
            var result = Tokens.Check(token);
            if (result == null || !result.Success) return null;

            // Recupera os dados do usuário com base no token informado
            var user = UserProviderClient.GetUserByToken(token, userIsOnline);

            return user == null ? null : new Wrappers.UserWrapper(user);
        }

        /// <summary>
        /// Recupera o número de usuários online no sistema.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfUsersOnline()
        {
            return UserProviderClient.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// Recupera os nomes dos provedores de identidade do sistema.
        /// </summary>
        /// <returns></returns>
        public string[] GetIdentityProviders()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reseta a senha do usuário informado.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public string ResetPassword(string username, string answer)
        {
            return UserProviderClient.ResetPassword(username, answer);
        }

        /// <summary>
        /// Atualiza os dados do usuário informado.
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            UserProviderClient.UpdateUser(Convert(user));
        }

        /// <summary>
        /// Valida credenciais
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <param name="password">Senha</param>
        /// <param name="parameters">Demais parâmetros</param>
        /// <returns>Resultado da autenticação</returns>
        public IValidateUserResult ValidateUser(string userName, string password, params SecurityParameter[] parameters)
        {
            // Calcula o Hash da senha
            var passwordHash = HashPassword(password);

            var servicesContext = Colosoft.Net.ServicesConfiguration.Current != null ?
                Colosoft.Net.ServicesConfiguration.Current.ServicesContext : null;

            AuthenticationHost.ValidateUserResult result = null;

            try
            {
                // Realiza a validação do usuário
                result = AuthenticationClient.ValidateUser(userName, passwordHash, servicesContext, parameters);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                result = new ValidateUserResultError
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserErrorEndpointNotFound).Format(),
                    Error = ex
                };
            }
            catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> ex)
            {
                result = new ValidateUserResultError
                {
                    Status = AuthenticationHost.AuthenticationStatus.UnknownError,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserFaultException, ex.Message).Format(),
                    Error = ex
                };
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                result = new ValidateUserResultError
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserErrorInCommunication).Format(),
                    Error = ex
                };
            }
            catch (TimeoutException ex)
            {
                result = new ValidateUserResultError
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserErrorInCommunicationTimeout).Format(),
                    Error = ex
                };
            }
            catch (Exception ex)
            {
                result = new ValidateUserResultError
                {
                    Status = AuthenticationHost.AuthenticationStatus.UnknownError,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserUnknownError).Format(),
                    Error = ex
                };
            }

            if (result.Status == AuthenticationHost.AuthenticationStatus.Success)
            {
                _userProviderClientUserName = userName;
                _userProviderClientPassword = password;
            }
            else
            {
                _userProviderClientUserName = "anonymous";
                _userProviderClientPassword = "anonymous";
            }

            Net.ServiceClientsManager.Current.Reset(_authenticationClientUid);

            return Convert(result);
        }

        /// <summary>
        /// Valida os dados do token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IValidateUserResult ValidateToken(string token)
        {
            var servicesContext = Colosoft.Net.ServicesConfiguration.Current != null ?
                Colosoft.Net.ServicesConfiguration.Current.ServicesContext : null;

            AuthenticationHost.ValidateUserResult result = null;

            try
            {
                result = AuthenticationClient.ValidateToken(token, servicesContext);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                result = new AuthenticationHost.ValidateUserResult
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateTokenErrorEndpointNotFound).Format()
                };
            }
            catch (System.ServiceModel.FaultException<DetailsException>)
            {
                result = new AuthenticationHost.ValidateUserResult
                {
                    Status = AuthenticationHost.AuthenticationStatus.UnknownError,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateTokenFaultException).Format()
                };
            }
            catch (System.ServiceModel.CommunicationException)
            {
                result = new AuthenticationHost.ValidateUserResult
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserErrorInCommunication).Format()
                };
            }
            catch (TimeoutException)
            {
                result = new AuthenticationHost.ValidateUserResult
                {
                    Status = AuthenticationHost.AuthenticationStatus.ErrorInCommunication,
                    Message = ResourceMessageFormatter.Create(() => Properties.Resources.RemoteUserProvider_ValidateUserErrorInCommunicationTimeout).Format()
                };
            }
            catch (Exception ex)
            {
                result = new AuthenticationHost.ValidateUserResult
                {
                    Status = AuthenticationHost.AuthenticationStatus.UnknownError,
                    Message = ResourceMessageFormatter.Create(() => 
                        Properties.Resources.RemoteUserProvider_ValidateTokenUnknownError, 
                        Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true)).Format()
                };
            }
            return Convert(result);
        }

        /// <summary>
        /// Desloga o usuário do sistema
        /// </summary>
        /// <param name="token">Token do usuário</param>
        /// <returns>Sucesso da operação</returns>
        public bool LogOut(string token)
        {
            return AuthenticationClient.LogOut(token);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Define o endereço do serviço do provedor de usuário.
        /// </summary>
        /// <param name="address"></param>
        public static void SetUserProviderServiceAddress(Colosoft.Net.ServiceAddress address)
        {
            Net.ServicesConfiguration
                    .Current[UserProviderServiceClientConfigurationName] = address;
        }

        /// <summary>
        /// Define o endereço do serviço de autenticação.
        /// </summary>
        /// <param name="address"></param>
        public static void SetAuthenticationServiceAddress(Colosoft.Net.ServiceAddress address)
        {
            Net.ServicesConfiguration
                    .Current[AuthenticationServiceClientConfigurationName] = address;
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Represenat o resultado da validação do usuário com o erro associado.
        /// </summary>
        class ValidateUserResultError : AuthenticationHost.ValidateUserResult
        {
            /// <summary>
            /// Error associado.
            /// </summary>
            public Exception Error { get; set; }
        }

        /// <summary>
        /// Classe responsável por tratar a ação de atualização do item do cache.
        /// </summary>
        class UserProviderInfoRefreshAction : Microsoft.Practices.EnterpriseLibrary.Caching.ICacheItemRefreshAction
        {
            #region Local Variables

            private RemoteUserProvider _provider;
            private Exception _lastException;

            /// <summary>
            /// Armazena o identificador unico da instancia.
            /// </summary>
            private readonly string _uid = Guid.NewGuid().ToString();

            #endregion

            #region Properties

            /// <summary>
            /// Instancia com as informações do provedor do usuário.
            /// </summary>
            public ServerHost.UserProviderInfo UserProviderInfo
            {
                get
                {
                    if (_lastException != null || !Cache.Instance.Contains(_uid))
                    {
                        LoadUserProvideInfo();

                        if (_lastException != null)
                            throw _lastException;
                    }

                    return (ServerHost.UserProviderInfo)Cache.Instance[_uid];
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="provider"></param>
            public UserProviderInfoRefreshAction(RemoteUserProvider provider)
            {
                _provider = provider;
                LoadUserProvideInfo();
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Carrega as informações do provedor.
            /// </summary>
            private void LoadUserProvideInfo()
            {
                Colosoft.Security.Remote.Client.ServerHost.UserProviderInfo info = null;

                try
                {
                    info = _provider.UserProviderClient.GetProviderInfo();
                }
                catch (Exception ex)
                {
                    _lastException = ex;
                    return;
                }

                // Adiciona a instancia o cache
                Cache.Instance.Add(_uid, info, Microsoft.Practices.EnterpriseLibrary.Caching.CacheItemPriority.Normal,
                                    this,
                                    new SlidingTime(new TimeSpan(0, 5, 0)));
            }

            #endregion

            #region Public Methods

            public void Refresh(string removedKey, object expiredValue, Microsoft.Practices.EnterpriseLibrary.Caching.CacheItemRemovedReason removalReason)
            {
                // Atualiza o cache se o item expirou, mas se não sido removido explicitamente
                if (removalReason == Microsoft.Practices.EnterpriseLibrary.Caching.CacheItemRemovedReason.Expired)
                    LoadUserProvideInfo();
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            Net.ServicesConfiguration.Current.Updated -= ServicesConfigurationUpdated;
            Net.ServiceClientsManager.Current.Remove(_userProviderClientUid);
            Net.ServiceClientsManager.Current.Remove(_authenticationClientUid);
        }

        #endregion

        #region IUserProvider Members

        /// <summary>
        /// Inicia o processo de redefinição de senha
        /// </summary>
        /// <param name="userName">Nome do usuário</param>
        /// <returns>Resultado do processo</returns>
        public ResetPasswordProcessResult RequestPasswordReset(string userName)
        {
            return AuthenticationClient.RequestPasswordReset(userName);
        }

        /// <summary>
        /// Redefine a senha do usuário
        /// </summary>
        /// <param name="userName">Nome do usuário</param>
        /// <param name="resetCode">Código de redefinição</param>
        /// <param name="newPassword">Nova senha</param>
        /// <param name="parameters">Parâmetros adicionais</param>
        /// <returns>Resultado da troca de senha</returns>
        public ChangePasswordResult ResetPassword(string userName, string resetCode, string newPassword, params SecurityParameter[] parameters)
        {
            return Convert(AuthenticationClient.ResetPassword(userName, resetCode, newPassword, parameters));
        }

        #endregion
    }
}
