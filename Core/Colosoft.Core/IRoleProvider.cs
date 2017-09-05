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
	/// Provedor dos papéis do sistema.
	/// </summary>
	public interface IRoleProvider
	{
		/// <summary>
		/// Nome do provedor.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Cria um novo papel no sistema.
		/// </summary>
		/// <param name="roleName"></param>
		void CreateRole(string roleName);

		/// <summary>
		/// Apaga o papel do sistema.
		/// </summary>
		/// <param name="roleName">Nome do papel que será removido.</param>
		/// <returns></returns>
		bool DeleteRole(string roleName);

		/// <summary>
		/// Recupera os papéis para o usuário informado.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		string[] GetRolesForUser(string username);

		/// <summary>
		/// Recupera os papéis execlusivos do usuário, ou seja, independente de grupos.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <returns></returns>
		string[] GetExclusiveRolesForUser(string username);

		/// <summary>
		/// Recupera os usuário que estão associados com a regra informada.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		string[] GetUsersInRole(string roleName);

		/// <summary>
		/// Verifica se o usuário informado está inserido no papel informada.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		bool IsUserInRole(string username, string roleName);

		/// <summary>
		/// Verifica se o papel existe.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		bool RoleExists(string roleName);

		/// <summary>
		/// Adiciona os papéis para os usuário informados.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		void AddUsersToRoles(string[] usernames, string[] roleNames);

		/// <summary>
		/// Pesquisa os usuário dentro do papél informado.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		string[] FindUsersInRole(string roleName, string usernameToMatch);

		/// <summary>
		/// Remove os usuários dos papéis.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

		/// <summary>
		/// Recupera todos os papéis do sistema.
		/// </summary>
		/// <returns></returns>
		string[] GetAllRoles();

		/// <summary>
		/// Cria um grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="roles">Papéis associados.</param>
		void CreateRoleGroup(string roleGroupName, string[] roles);

		/// <summary>
		/// Apaga um grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName"></param>
		void DeleteRoleGroup(string roleGroupName);

		/// <summary>
		/// Verifica se o grupo de papéis existe.
		/// </summary>
		/// <param name="roleGroupName"></param>
		/// <returns></returns>
		bool ExistsRoleGroup(string roleGroupName);

		/// <summary>
		/// Adiciona papéis para o grupo.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="roleNames">Nomes dos papéis que serão adicionados.</param>
		void AddRolesToRoleGroup(string roleGroupName, string[] roleNames);

		/// <summary>
		/// Remove os papéis associados com os grupos de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nomes dos grupos.</param>
		/// <param name="roleNames">Nomes dos papéis.</param>
		void RemoveRolesFromRoleGroup(string[] roleGroupName, string[] roleNames);

		/// <summary>
		/// Recupera os papéis para o grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupom de papéis.</param>
		/// <returns>Nomes dos papéis.</returns>
		string[] GetRolesForRoleGroup(string roleGroupName);

		/// <summary>
		/// Verifica se o grupo de regras existe.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <returns>True caso exista.</returns>
		bool RoleGroupExists(string roleGroupName);

		/// <summary>
		/// Recupera todos os grupos de papéis.
		/// </summary>
		/// <returns>Nomes dos grupos.</returns>
		string[] GetAllRoleGroups();

		/// <summary>
		/// Adiciona usuários para os grupos de papéis.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários que serão associados.</param>
		/// <param name="roleGroupNames">Nomes dos grupos que serão associados.</param>
		void AddUsersToRoleGroup(string[] usernames, string[] roleGroupNames);

		/// <summary>
		/// Remove os usuários associados com os grupos de papéis.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários.</param>
		/// <param name="roleGroupNames">Nomes dos grupos.</param>
		void RemoveUsersFromRoleGroup(string[] usernames, string[] roleGroupNames);

		/// <summary>
		/// Pesquisa os usuários qeu estão inseridos no grupo de papéis.
		/// </summary>
		/// <param name="roleGroupName">Nome do grupo.</param>
		/// <param name="usernameToMatch">Nome dos usuários que serão usados para comparação.</param>
		/// <returns></returns>
		string[] FindUsersInRoleGroup(string roleGroupName, string usernameToMatch);

		/// <summary>
		/// Verifica se o usuário está no grupo de papéis.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="roleGroupName">Nome do grupo de papéis.</param>
		/// <returns>True caso exista.</returns>
		bool IsUserInRoleGroup(string username, string roleGroupName);
	}
}
