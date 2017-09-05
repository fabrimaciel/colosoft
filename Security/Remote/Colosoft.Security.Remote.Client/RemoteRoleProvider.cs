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
    /// Implementação do provedor de papéis remoto.
    /// </summary>
    public class RemoteRoleProvider : IRoleProvider
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço do provedor dos perfis.
        /// </summary>
        const string RoleProviderServiceClientConfigurationName = "RoleProviderServiceClient";

        #endregion

        #region Local Variables

        private readonly string _roleProviderClientUid = Guid.NewGuid().ToString();

        #endregion

        #region Properties

        /// <summary>
        /// Recupera a instancia do cliente do serviço.
        /// </summary>
        internal RoleProviderServiceReference.IRoleProviderService RoleProviderClient
        {
            get
            {
                return Net.ServiceClientsManager.Current
                    .Get<RoleProviderServiceReference.RoleProviderServiceClient>(_roleProviderClientUid);
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
        public RemoteRoleProvider()
        {
            Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Net.ServiceClientsManager.Current.Register(_roleProviderClientUid,
                () =>
                {
                    var serviceAddress = Net.ServicesConfiguration.Current[RoleProviderServiceClientConfigurationName];
                    var client = new RoleProviderServiceReference.RoleProviderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
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
            if (e.ServiceName == RoleProviderServiceClientConfigurationName)
                Net.ServiceClientsManager.Current.Reset(_roleProviderClientUid);
        }

        #endregion

        /// <summary>
        /// Cria um novo papel no sistema.
        /// </summary>
        /// <param name="roleName"></param>
        public void CreateRole(string roleName)
        {
            RoleProviderClient.CreateRole(roleName);
        }

        /// <summary>
        /// Apaga o papel do sistema.
        /// </summary>
        /// <param name="roleName">Nome do papel que será removido.</param>
        /// <returns></returns>
        public bool DeleteRole(string roleName)
        {
            return RoleProviderClient.DeleteRole(roleName);
        }

        /// <summary>
        /// Recupera os papéis para o usuário informado.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string[] GetRolesForUser(string username)
        {
            return RoleProviderClient.GetRolesForUser(username);
        }

        /// <summary>
        /// Recupera os papéis execlusivos do usuário, ou seja, independente de grupos.
        /// </summary>
        /// <param name="username">Nome do usuário.</param>
        /// <returns></returns>
        public string[] GetExclusiveRolesForUser(string username)
        {
            return RoleProviderClient.GetExclusiveRolesForUser(username);
        }

        /// <summary>
        /// Recupera os usuário que estão associados com a regra informada.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public string[] GetUsersInRole(string roleName)
        {
            return RoleProviderClient.GetUsersInRole(roleName);
        }

        /// <summary>
        /// Verifica se o usuário informado está inserido no papel informada.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserInRole(string username, string roleName)
        {
            return RoleProviderClient.IsUserInRole(username, roleName);
        }

        /// <summary>
        /// Verifica se o papel existe.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool RoleExists(string roleName)
        {
            return RoleProviderClient.RoleExists(roleName);
        }

        /// <summary>
        /// Adiciona os papéis para os usuário informados.
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            RoleProviderClient.AddUsersToRoles(usernames, roleNames);
        }

        /// <summary>
        /// Pesquisa os usuário dentro do papél informado.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="usernameToMatch"></param>
        /// <returns></returns>
        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return RoleProviderClient.FindUsersInRole(roleName, usernameToMatch);
        }

        /// <summary>
        /// Remove os usuários dos papéis.
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            RoleProviderClient.RemoveUsersFromRoles(usernames, roleNames);
        }

        /// <summary>
        /// Recupera todos os papéis do sistema.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles()
        {
            return RoleProviderClient.GetAllRoles();
        }

        /// <summary>
        /// Cria um grupo de papéis.
        /// </summary>
        /// <param name="roleGroupName">Nome do grupo.</param>
        /// <param name="roles">Papéis associados.</param>
        public void CreateRoleGroup(string roleGroupName, string[] roles)
        {
            RoleProviderClient.CreateRoleGroup(roleGroupName, roles);
        }

        /// <summary>
        /// Apaga um grupo de papéis.
        /// </summary>
        /// <param name="roleGroupName"></param>
        public void DeleteRoleGroup(string roleGroupName)
        {
            RoleProviderClient.DeleteRoleGroup(roleGroupName);
        }

        /// <summary>
        /// Verifica se o grupo de papéis existe.
        /// </summary>
        /// <param name="roleGroupName"></param>
        /// <returns></returns>
        public bool ExistsRoleGroup(string roleGroupName)
        {
            return RoleProviderClient.ExistsRoleGroup(roleGroupName);
        }

        /// <summary>
        /// Adiciona papéis para o grupo.
        /// </summary>
        /// <param name="roleGroupName">Nome do grupo.</param>
        /// <param name="roleNames">Nomes dos papéis que serão adicionados.</param>
        public void AddRolesToRoleGroup(string roleGroupName, string[] roleNames)
        {
            RoleProviderClient.AddRolesToRoleGroup(roleGroupName, roleNames);
        }

        /// <summary>
        /// Remove os papéis associados com os grupos de papéis.
        /// </summary>
        /// <param name="roleGroupName">Nomes dos grupos.</param>
        /// <param name="roleNames">Nomes dos papéis.</param>
        public void RemoveRolesFromRoleGroup(string[] roleGroupName, string[] roleNames)
        {
            RoleProviderClient.RemoveRolesFromRoleGroup(roleGroupName, roleNames);
        }

        /// <summary>
        /// Recupera os papéis para o grupo de papéis.
        /// </summary>
        /// <param name="roleGroupName">Nome do grupom de papéis.</param>
        /// <returns>Nomes dos papéis.</returns>
        public string[] GetRolesForRoleGroup(string roleGroupName)
        {
            return RoleProviderClient.GetRolesForRoleGroup(roleGroupName);
        }

        /// <summary>
        /// Verifica se o grupo de regras existe.
        /// </summary>
        /// <param name="roleGroupName">Nome do grupo.</param>
        /// <returns>True caso exista.</returns>
        public bool RoleGroupExists(string roleGroupName)
        {
            return RoleProviderClient.RoleGroupExists(roleGroupName);
        }

        /// <summary>
        /// Recupera todos os grupos de papéis.
        /// </summary>
        /// <returns>Nomes dos grupos.</returns>
        public string[] GetAllRoleGroups()
        {
            return RoleProviderClient.GetAllRoleGroups();
        }

        /// <summary>
        /// Adiciona usuários para os grupos de papéis.
        /// </summary>
        /// <param name="usernames">Nomes dos usuários que serão associados.</param>
        /// <param name="roleGroupNames">Nomes dos grupos que serão associados.</param>
        public void AddUsersToRoleGroup(string[] usernames, string[] roleGroupNames)
        {
            RoleProviderClient.AddUsersToRoleGroup(usernames, roleGroupNames);
        }

        /// <summary>
        /// Remove os usuários associados com os grupos de papéis.
        /// </summary>
        /// <param name="usernames">Nomes dos usuários.</param>
        /// <param name="roleGroupNames">Nomes dos grupos.</param>
        public void RemoveUsersFromRoleGroup(string[] usernames, string[] roleGroupNames)
        {
            RoleProviderClient.RemoveUsersFromRoleGroup(usernames, roleGroupNames);
        }

        /// <summary>
        /// Pesquisa os usuários qeu estão inseridos no grupo de papéis.
        /// </summary>
        /// <param name="roleGroupName">Nome do grupo.</param>
        /// <param name="usernameToMatch">Nome dos usuários que serão usados para comparação.</param>
        /// <returns></returns>
        public string[] FindUsersInRoleGroup(string roleGroupName, string usernameToMatch)
        {
            return RoleProviderClient.FindUsersInRoleGroup(roleGroupName, usernameToMatch);
        }

        /// <summary>
        /// Verifica se o usuário está no grupo de papéis.
        /// </summary>
        /// <param name="username">Nome do usuário.</param>
        /// <param name="roleGroupName">Nome do grupo de papéis.</param>
        /// <returns>True caso exista.</returns>
        public bool IsUserInRoleGroup(string username, string roleGroupName)
        {
            return RoleProviderClient.IsUserInRoleGroup(username, roleGroupName);
        }
    }
}
