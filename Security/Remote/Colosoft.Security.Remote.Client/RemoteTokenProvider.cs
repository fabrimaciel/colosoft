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

namespace Colosoft.Security.Remote.Client
{
    /// <summary>
    /// Implementação remota do provedor de token.
    /// </summary>
    public class RemoteTokenProvider : ITokenProvider, ITokenApplicationProvider
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço do provedor dos perfis.
        /// </summary>
        const string TokenProviderServiceClientConfigurationName = "TokenProviderService";

        #endregion

        #region Local Variables

        private readonly string _tokenProviderClientUid = Guid.NewGuid().ToString();

        #endregion

        #region Events

        /// <summary>
        /// Evento acionado quando estiver configurando o cliente do provedor de token.
        /// </summary>
        public event ConfigureRemoteClientEventHandler ConfiguringTokenProviderClient;

        #endregion

        #region Properties

        /// <summary>
        /// Recupera a instancia do cliente do serviço.
        /// </summary>
        internal TokenProviderServiceReference.ITokenProviderService TokenProviderClient
        {
            get
            {
                return Net.ServiceClientsManager.Current
                    .Get<TokenProviderServiceReference.TokenProviderServiceClient>(_tokenProviderClientUid);
            }
        }
        
        /// <summary>
        /// Nome do provedor.
        /// </summary>
        public string Name { get { return "RemoteTokenProvider"; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public RemoteTokenProvider()
        {
            Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Net.ServiceClientsManager.Current.Register(_tokenProviderClientUid,
                () =>
                {
                    Colosoft.Net.ServiceAddress serviceAddress = null;

                    //if (Net.ServicesConfiguration.Current.Contains(TokenProviderServiceClientConfigurationName))
                    serviceAddress = Net.ServicesConfiguration.Current[TokenProviderServiceClientConfigurationName];

                    /*else
                        throw new InvalidOperationException("Not found address for TokenProviderServiceClient");*/

                    var client = new TokenProviderServiceReference.TokenProviderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());

                    if (ConfiguringTokenProviderClient != null)
                        ConfiguringTokenProviderClient(this, new ConfigureRemoteClientEventArgs(client, client.Endpoint));

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
            if (e.ServiceName == TokenProviderServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_tokenProviderClientUid);
            
            /*else if (e.ServiceName == AuthenticationServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_authenticationClientUid);*/
        }

        #endregion

        #region ITokenProvider Members

        /// <summary>
        /// Cria um token com o tamanho padrão e com os caracteres padrão
        /// </summary>
        /// <returns>Token</returns>
        public string Create()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Cria um token com o tamanho padrão os caracteres informados
        /// </summary>
        /// <param name="validChars">Vetor com os caracteres válidos</param>
        /// <returns>Token</returns>
        public string Create(char[] validChars)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Cria um token do tamanho informado com os caracteres "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%¨*()"
        /// </summary>
        /// <param name="size">Tamanho do token</param>
        /// <returns>Token</returns>
        public string Create(int size)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Cria um token do tamanho informado com os caracteres informados
        /// </summary>
        /// <param name="size">Tamanho do token</param>
        /// <param name="validChars">Vetor com os caracteres válidos</param>
        /// <returns>Token</returns>
        public string Create(int size, char[] validChars)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Insere o controle do token
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userId">identificador do usuário</param>
        /// <returns>verdadeiro se a inserção foi bem sucedida</returns>
        public bool Insert(string token, int userId)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Verifica se um token está ou não válido
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>Objeto com o resultado da consulta</returns>
        public TokenConsultResult Check(string token)
        {
            /*
              System.Security.Principal.IPrincipal currentPrincipal = UserContext.Current.Principal;
              if ((currentPrincipal != null) && (currentPrincipal.Identity != null))
              {
                  var identity = currentPrincipal.Identity;

                  if (identity.IsAuthenticated)
                      return TokenProviderClient.Check(token);
                  
              }

            return new TokenConsultResult
                {
                    Success = false
                };*/

            try
            {
                return TokenProviderClient.Check(token);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.RemoteTokenProvider_CheckTokenEndpointNotFound).Format());
            }
            catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> ex)
            {
                throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.RemoteTokenProvider_CheckTokenFaultException, ex.Detail.Message).Format(), ex);
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.RemoteTokenProvider_CheckTokenCommunicationError).Format());
            }
            catch (Exception ex)
            {
                throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.RemoteTokenProvider_CheckTokenUnknownError).Format(), ex);
            }
        }

        /// <summary>
        /// Define o perfil para o token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public TokenSetProfileResult SetProfile(string token, int profileId)
        {
            try
            {
                return TokenProviderClient.SetProfile(token, profileId);
            }
            catch (Exception ex)
            {
                return new TokenSetProfileResult
                {
                    Success = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Invalida o token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>verdadeiro se conseguiu invalidar</returns>
        public bool Close(string token)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Verifica se já existe um token aberto para o usuário em questão
        /// </summary>
        /// <param name="userId">Identificador do usuário</param>
        /// <returns>token</returns>
        public string GetToken(int userId)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executa uma verificação do token no servidor.
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        public TokenPingResult Ping(string token)
        {
            try
            {
                return TokenProviderClient.Ping(token);
            }
            catch (Exception ex)
            {
                return new TokenPingResult
                {
                    Message = ex,
                    Status = TokenPingResultStatus.Error,
                };
            }
        }
        /// <summary>
        /// Marca as mensagens como lidas.
        /// </summary>
        /// <param name="dispatcherIds">Identificadores dos despachos.</param>
        public void MarkMessageAsRead(IEnumerable<int> dispatcherIds)
        {
            try
            {
                TokenProviderClient.MarkMessageAsRead(dispatcherIds.ToArray());
            }
            catch 
            {
                
            }
        }

        /// <summary>
        /// Fecha os tokens em aberto de um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário</param>
        public void CloseUserTokens(int userId)
        {
            TokenProviderClient.CloseUserTokens(userId);
        }

        #endregion

        #region ITokenApplicationProvider Members

        /// <summary>
        /// Fecha os otknes em aberto do usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="applicationName"></param>
        public void CloseUserTokens(int userId, string applicationName)
        {
            TokenProviderClient.CloseUserTokens2(userId, applicationName);
        }

        /// <summary>
        /// Recupera o token associado com o usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public string GetToken(int userId, string applicationName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Insere um token no sistema.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public bool Insert(string token, int userId, string applicationName)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
