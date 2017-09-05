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
	/// Implementação do serviço do providor de papéis.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
	public class RoleProviderService : IRoleProviderService
	{
		/// <summary>
		/// Instancia do provedor dos usuários.
		/// </summary>
		protected IRoleProvider RoleProvider
		{
			get
			{
				return Roles.Provider;
			}
		}

		/// <summary>
		/// Cria um novo papel no sistema.
		/// </summary>
		/// <param name="roleName"></param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void CreateRole(string roleName)
		{
			RoleProvider.CreateRole(roleName);
		}

		/// <summary>
		/// Apaga o papel do sistema.
		/// </summary>
		/// <param name="roleName">Nome do papel que será removido.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public bool DeleteRole(string roleName)
		{
			return RoleProvider.DeleteRole(roleName);
		}

		/// <summary>
		/// Recupera os papéis para o usuário informado.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetRolesForUser(string username)
		{
			return RoleProvider.GetRolesForUser(username);
		}

		/// <summary>
		/// Recupera os papéis execlusivos do usuário, ou seja, independente de grupos.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetExclusiveRolesForUser(string username)
		{
			return RoleProvider.GetExclusiveRolesForUser(username);
		}

		/// <summary>
		/// Recupera os usuário que estão associados com a regra informada.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetUsersInRole(string roleName)
		{
			return RoleProvider.GetUsersInRole(roleName);
		}

		/// <summary>
		/// Verifica se o usuário informado está inserido no papel informada.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public bool IsUserInRole(string username, string roleName)
		{
			return RoleProvider.IsUserInRole(username, roleName);
		}

		/// <summary>
		/// Verifica se o papel existe.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public bool RoleExists(string roleName)
		{
			return RoleProvider.RoleExists(roleName);
		}

		/// <summary>
		/// Adiciona os papéis para os usuário informados.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			RoleProvider.AddUsersToRoles(usernames, roleNames);
		}

		/// <summary>
		/// Pesquisa os usuário dentro do papél informado.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			return RoleProvider.FindUsersInRole(roleName, usernameToMatch);
		}

		/// <summary>
		/// Remove os usuários dos papéis.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			RoleProvider.RemoveUsersFromRoles(usernames, roleNames);
		}

		/// <summary>
		/// Recupera todos os papéis do sistema.
		/// </summary>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetAllRoles()
		{
			return RoleProvider.GetAllRoles();
		}

		/// <summary>
		/// Cria um grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="roles">Papéis associados.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void CreateRoleGroup(string roleGroupName, string[] roles)
		{
			RoleProvider.CreateRoleGroup(roleGroupName, roles);
		}

		/// <summary>
		/// Apaga um grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName"></param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void DeleteRoleGroup(string roleGroupName)
		{
			RoleProvider.DeleteRoleGroup(roleGroupName);
		}

		/// <summary>
		/// Verifica se o grupo de papéis existe.
		/// </summary>
		/// <param name="roleGroupName"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public bool ExistsRoleGroup(string roleGroupName)
		{
			return RoleProvider.ExistsRoleGroup(roleGroupName);
		}

		/// <summary>
		/// Adiciona papéis para o grupo.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="roleNames">Nomes dos papéis que serão adicionados.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void AddRolesToRoleGroup(string roleGroupName, string[] roleNames)
		{
			RoleProvider.AddRolesToRoleGroup(roleGroupName, roleNames);
		}

		/// <summary>
		/// Remove os papéis associados com os grupos de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nomes dos grupos.</param>
		/// <param name="roleNames">Nomes dos papéis.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void RemoveRolesFromRoleGroup(string[] roleGroupName, string[] roleNames)
		{
			RoleProvider.RemoveRolesFromRoleGroup(roleGroupName, roleNames);
		}

		/// <summary>
		/// Recupera os papéis para o grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupom de papéis.</param>
		/// <returns>Nomes dos papéis.</returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetRolesForRoleGroup(string roleGroupName)
		{
			return RoleProvider.GetRolesForRoleGroup(roleGroupName);
		}

		/// <summary>
		/// Verifica se o grupo de regras existe.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <returns>True caso exista.</returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public bool RoleGroupExists(string roleGroupName)
		{
			return RoleProvider.RoleGroupExists(roleGroupName);
		}

		/// <summary>
		/// Recupera todos os grupos de papéis.
		/// </summary>
		/// <returns>Nomes dos grupos.</returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public string[] GetAllRoleGroups()
		{
			return RoleProvider.GetAllRoleGroups();
		}

		/// <summary>
		/// Adiciona usuários para os grupos de papéis.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários que serão associados.</param>
		/// <param name="roleGroupNames">Nomes dos grupos que serão associados.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void AddUsersToRoleGroup(string[] usernames, string[] roleGroupNames)
		{
			RoleProvider.AddUsersToRoleGroup(usernames, roleGroupNames);
		}

		/// <summary>
		/// Remove os usuários associados com os grupos de papéis.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários.</param>
		/// <param name="roleGroupNames">Nomes dos grupos.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public void RemoveUsersFromRoleGroup(string[] usernames, string[] roleGroupNames)
		{
			RoleProvider.RemoveUsersFromRoleGroup(usernames, roleGroupNames);
		}

		/// <summary>
		/// Pesquisa os usuários qeu estão inseridos no grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="usernameToMatch">Nome dos usuários que serão usados para comparação.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = Constants.AdministratorRole, Authenticated = true)]
		public string[] FindUsersInRoleGroup(string roleGroupName, string usernameToMatch)
		{
			return RoleProvider.FindUsersInRoleGroup(roleGroupName, usernameToMatch);
		}

		/// <summary>
		/// Verifica se o usuário está no grupo de papéis.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="roleGroupName">Nome do grupo de papéis.</param>
		/// <returns>True caso exista.</returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public bool IsUserInRoleGroup(string username, string roleGroupName)
		{
			return RoleProvider.IsUserInRoleGroup(username, roleGroupName);
		}
	}
}
