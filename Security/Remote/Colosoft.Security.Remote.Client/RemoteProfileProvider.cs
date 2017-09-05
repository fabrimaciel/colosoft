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
using Colosoft.Security.Profile;
using System.ComponentModel.Composition;

namespace Colosoft.Security.Remote.Client
{
    /// <summary>
    /// Implementação do provedor de perfis remoto.
    /// </summary>
    [Export(typeof(IProfileProvider))]
    public class RemoteProfileProvider : IProfileProvider, IDisposable
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço do provedor dos perfis.
        /// </summary>
        const string ProfileProviderServiceClientConfigurationName = "ProfileProviderService";

        #endregion

        #region Local Variables

        private readonly string _profileProviderClientUid = Guid.NewGuid().ToString();

        #endregion

        #region Properties

        /// <summary>
        /// Recupera a instancia do cliente do serviço.
        /// </summary>
        internal ProfileProviderServiceReference.ProfileProviderServiceClient ProfileProviderClient
        {
            get
            {
                return Net.ServiceClientsManager.Current
                    .Get<ProfileProviderServiceReference.ProfileProviderServiceClient>(_profileProviderClientUid);
            }
        }

        /// <summary>
        /// Nome do provedor.
        /// </summary>
        public string Name
        {
            get { return "RemoteProfileProvider"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public RemoteProfileProvider()
        {
            Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Net.ServiceClientsManager.Current.Register(_profileProviderClientUid,
                () =>
                {
                    var serviceAddress = Net.ServicesConfiguration.Current[ProfileProviderServiceClientConfigurationName];
                    var client = new ProfileProviderServiceReference.ProfileProviderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
                    Colosoft.Net.SecurityTokenBehavior.Register(client.Endpoint);

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
            if (e.ServiceName == ProfileProviderServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_profileProviderClientUid);
        }

        private static ProfileInfo Convert(ProfileProviderServiceReference.ProfileInfo info)
        {
            return new ProfileInfo
                (info.ProfileId, info.UserName, info.FullName, info.IsAnonymous, info.SearchMode, info.Source == null ? null : new Wrappers.AuthenticationSourceWrapper(info.Source), 
                 info.LastActivityDate, info.LastUpdatedDate, info.MarkGroupId, info.SellerTreeId, info.IntermediateId);
        }

        /// <summary>
        /// Converte um ProfileInfo do sistema para o ProfileInfo do WebService.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static ProfileProviderServiceReference.ProfileInfo Convert(ProfileInfo info)
        {
            return new ProfileProviderServiceReference.ProfileInfo
            {
                IsAnonymous = info.IsAnonymous,
                FullName = info.FullName,
                ProfileId = info.ProfileId,
                LastActivityDate = info.LastActivityDate,
                LastUpdatedDate = info.LastUpdatedDate,
                Source = info.Source != null ? new ProfileProviderServiceReference.AuthenticationSource
                {
                    FullName = info.Source.FullName,
                    Uid = info.Source.Uid
                } : null,
                UserName = info.UserName
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Apaga os perfis dos usuários informados.
        /// </summary>
        /// <param name="usernames">Nomes dos usuários que terão seus perfis apagados.</param>
        /// <returns></returns>
        public int DeleteProfiles(string[] usernames)
        {
            return ProfileProviderClient.DeleteProfiles(usernames);
        }

        /// <summary>
        /// Apaga os perfis informados.
        /// </summary>
        /// <param name="profiles">Informações dos perfis que serão apagados.</param>
        /// <returns></returns>
        public int DeleteProfiles(ProfileInfo[] profiles)
        {
            if (profiles == null || profiles.Length == 0) return 0;
            return ProfileProviderClient.DeleteProfilesByProfileInfo(profiles.Select(f => Convert(f)).ToArray());
        }

        /// <summary>
        /// Pesquisa os perfis associados com o nome do usuário informado.
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <returns></returns>
        public IList<ProfileInfo> FindProfilesByUserName(string usernameToMatch)
        {
            return ProfileProviderClient.FindProfilesByUserName(usernameToMatch)
                .Select(f => Convert(f)).ToList();
        }

        /// <summary>
        /// Recupera os perfis associados com o usuário informado.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<ProfileInfo> GetUserProfiles(string userName)
        {
            try
            {
                return ProfileProviderClient.GetUserProfiles(userName)
                    .Select(f => Convert(f)).ToList();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new ProfileProviderException(
                    ResourceMessageFormatter.Create(
                        () => Properties.Resources.RemoteProfileProvider_GetUserProfilesCommunicationError, ProfileProviderClient.Endpoint.Address.Uri).Format(), ex);
            }
            catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> ex)
            {
                throw new ProfileProviderException(
                    ResourceMessageFormatter.Create(() => Properties.Resources.RemoteProfileProvider_GetUserProfilesFaultException, ex.Message).Format(), ex);
            }
            catch (System.ServiceModel.FaultException ex)
            {
                throw new ProfileProviderException(
                    ResourceMessageFormatter.Create(() => Properties.Resources.RemoteProfileProvider_GetUserProfilesFaultException, ex.Message).Format(), ex);
            }
        }

        /// <summary>
        /// Recupera os dados do perfil.
        /// </summary>
        /// <param name="info">Informações usadas para recuperar o perfil.</param>
        /// <returns></returns>
        public IProfile GetProfile(ProfileInfo info)
        {
            if (info == null) return null; 
            ProfileProviderServiceReference.Profile result = null;

            try
            {
                result = ProfileProviderClient.GetProfile(Convert(info));
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new ProfileProviderException(
                    ResourceMessageFormatter.Create(() => Properties.Resources.RemoteProfileProvider_GetProfileCommunicationError).Format(), ex);
            }
            catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> ex)
            {
                throw new ProfileProviderException(
                    ResourceMessageFormatter.Create(() => Properties.Resources.RemoteProfileProvider_GetProfileFaultException, ex.Message).Format(), ex);
            }

            return result == null ? null : new Wrappers.ProfileWrapper(result, this);
        }

        /// <summary>
        /// Recupera a origem do perfil.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public IAuthenticationSource GetSource(int sourceId)
        {
            var source = ProfileProviderClient.GetSource(sourceId);
            if (source == null) return null;
            return new Wrappers.AuthenticationSourceWrapper(source);
        }

        /// <summary>
        /// Define o valor da propriedade do perfil.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        public void SetProfilePropertyValue(IProfile profile, ProfilePropertyDefinition property, string propertyValue)
        {
            profile.Require("profile").NotNull();
            property.Require("property").NotNull();
            var info = profile.GetInfo();
            ProfileProviderClient.SetProfilePropertyValue(Convert(info), property, propertyValue);
        }

        /// <summary>
        /// Recupera as definições das proprietadades dos perfis do sistema.
        /// </summary>
        /// <returns></returns>
        public ProfilePropertyDefinition[] GetProfilePropertyDefinitions()
        {
            return this.ProfileProviderClient.GetProfilePropertyDefinitions();
        }

        /// <summary>
        /// Não suportado.
        /// </summary>
        /// <param name="profile"></param>
        public void SaveProfileProperties(IProfile profile)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Define o endereço do serviço do provedor de perfis.
        /// </summary>
        /// <param name="address"></param>
        public static void SetServiceAddress(Colosoft.Net.ServiceAddress address)
        {
            Net.ServicesConfiguration
                    .Current[ProfileProviderServiceClientConfigurationName] = address;
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
            Net.ServiceClientsManager.Current.Remove(_profileProviderClientUid);
        }

        #endregion
    }
}
