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

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.AspNet.Identity
{
	/// <summary>
	/// Implementação do usuário para identidade.
	/// </summary>
	public class IdentityUser : IUser
	{
		private Security.IUser _user;

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="user"></param>
		public IdentityUser(Security.IUser user)
		{
			_user = user;
		}

		/// <summary>
		/// Usuário associado.
		/// </summary>
		public Security.IUser User
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		///     Is two factor enabled for the user
		/// </summary>
		public virtual bool TwoFactorEnabled
		{
			get;
			set;
		}

		/// <summary>
		///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
		/// </summary>
		public virtual DateTime? LockoutEndDateUtc
		{
			get;
			set;
		}

		/// <summary>
		///     Is lockout enabled for this user
		/// </summary>
		public virtual bool LockoutEnabled
		{
			get;
			set;
		}

		/// <summary>
		///     Used to record failures for the purposes of lockout
		/// </summary>
		public virtual int AccessFailedCount
		{
			get;
			set;
		}

		/// <summary>
		///     User ID (Primary Key)
		/// </summary>
		public virtual string Id
		{
			get
			{
				return _user.UserKey;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		///     User name
		/// </summary>
		public virtual string UserName
		{
			get
			{
				return _user.UserName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Gera a identidade do usuário.
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public async virtual Task<System.Security.Claims.ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			return userIdentity;
		}
	}
}
