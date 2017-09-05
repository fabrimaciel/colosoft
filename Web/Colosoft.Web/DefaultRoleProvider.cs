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
	/// Implementação padrão do provedor de papel do sistema.
	/// </summary>
	public class DefaultRoleProvider : RoleProvider
	{
		/// <summary>
		/// Adiciona vários usuário para os papéis informados.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			Colosoft.Security.Roles.Provider.AddUsersToRoles(usernames, roleNames);
		}

		/// <summary>
		/// Nome da aplicação.
		/// </summary>
		public override string ApplicationName
		{
			get
			{
				return "Default Role Provider";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Cria um novo papel.
		/// </summary>
		/// <param name="roleName"></param>
		public override void CreateRole(string roleName)
		{
			Colosoft.Security.Roles.Provider.CreateRole(roleName);
		}

		/// <summary>
		/// Apaga um papél.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="throwOnPopulatedRole"></param>
		/// <returns></returns>
		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			return Colosoft.Security.Roles.Provider.DeleteRole(roleName);
		}

		/// <summary>
		/// Pequisa os usuário no papel informado.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			return Colosoft.Security.Roles.Provider.FindUsersInRole(roleName, usernameToMatch);
		}

		/// <summary>
		/// Recupera todos os papéis cadastrados no sistema.
		/// </summary>
		/// <returns></returns>
		public override string[] GetAllRoles()
		{
			return Colosoft.Security.Roles.Provider.GetAllRoles();
		}

		/// <summary>
		/// Recupera os papéis para o usuário.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public override string[] GetRolesForUser(string username)
		{
			return Colosoft.Security.Roles.Provider.GetRolesForUser(username);
		}

		/// <summary>
		/// Recupera os usuários inseridos no papel informado.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public override string[] GetUsersInRole(string roleName)
		{
			return Colosoft.Security.Roles.Provider.GetUsersInRole(roleName);
		}

		/// <summary>
		/// Verifica se o usuário está inserido no papel.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public override bool IsUserInRole(string username, string roleName)
		{
			return Colosoft.Security.Roles.Provider.IsUserInRole(username, roleName);
		}

		/// <summary>
		/// Remove os usuário dos papéis.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			Colosoft.Security.Roles.Provider.RemoveUsersFromRoles(usernames, roleNames);
		}

		/// <summary>
		/// Verifica se o papel já existe.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public override bool RoleExists(string roleName)
		{
			return Colosoft.Security.Roles.Provider.RoleExists(roleName);
		}
	}
}
