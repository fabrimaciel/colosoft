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
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Colosoft.Web.AspNet.Identity
{
	/// <summary>
	/// Representa o armazenamento dos dados do usuário.
	/// </summary>
	public class UserStore : IUserStore<IdentityUser, string>, IUserStore<IdentityUser>, IUserRoleStore<IdentityUser, string>
	{
		private Security.IUserProvider _userProvider;

		private Security.IRoleProvider _roleProvider;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="userProvider"></param>
		/// <param name="roleProvider"></param>
		public UserStore(Security.IUserProvider userProvider, Security.IRoleProvider roleProvider)
		{
			userProvider.Require("userProvider").NotNull();
			roleProvider.Require("roleProvider").NotNull();
			_userProvider = userProvider;
			_roleProvider = roleProvider;
		}

		/// <summary>
		/// Cria um usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task CreateAsync(IdentityUser user)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Apaga os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task DeleteAsync(IdentityUser user)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Pesquisa o usuário pelo identificador informado.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<IdentityUser> FindByIdAsync(string userId)
		{
			return new Task<IdentityUser>(() => new IdentityUser(_userProvider.GetUserByKey(userId.ToString(), false)));
		}

		/// <summary>
		/// Recupera o usuário pelo nome informado.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public Task<IdentityUser> FindByNameAsync(string userName)
		{
			return new Task<IdentityUser>(() => new IdentityUser(_userProvider.GetUser(userName, false)));
		}

		/// <summary>
		/// Atualzia os dados do usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task UpdateAsync(IdentityUser user)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Adiciona um papel para o usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public Task AddToRoleAsync(IdentityUser user, string roleName)
		{
			return new Task(() => _roleProvider.AddUsersToRoles(new[] {
				user.UserName
			}, new[] {
				roleName
			}));
		}

		/// <summary>
		/// Recupera os papéis do usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<IList<string>> GetRolesAsync(IdentityUser user)
		{
			return new Task<IList<string>>(() => _roleProvider.GetRolesForUser(user.UserName));
		}

		/// <summary>
		/// Verifica se o usuário está no papel informado.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
		{
			return new Task<bool>(() => _roleProvider.IsUserInRole(user.UserName, roleName));
		}

		/// <summary>
		/// Remove o papel do usuário.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
		{
			return new Task(() => _roleProvider.RemoveUsersFromRoles(new[] {
				user.UserName
			}, new[] {
				roleName
			}));
		}
	}
}
