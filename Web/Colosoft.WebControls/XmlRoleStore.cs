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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace Colosoft.WebControls.Security.Store
{
	/// <summary>
	/// 
	/// </summary>
	public partial class XmlRoleStore : Persistable<List<XmlRole>>
	{
		/// <summary>
		/// Gets the roles.
		/// </summary>
		/// <value>The roles.</value>
		public virtual List<XmlRole> Roles
		{
			get
			{
				return this.Value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRoleStore"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public XmlRoleStore(string fileName) : base(fileName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRoleStore"/> class.
		/// </summary>
		protected XmlRoleStore() : base(null)
		{
		}

		/// <summary>
		/// Gets the role.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		public virtual XmlRole GetRole(string roleName)
		{
			lock (SyncRoot)
			{
				return (Roles != null) ? Roles.Find(delegate(XmlRole role) {
					return role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase);
				}) : null;
			}
		}

		/// <summary>
		/// Gets the roles for user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		public virtual List<XmlRole> GetRolesForUser(string userName)
		{
			lock (SyncRoot)
			{
				List<XmlRole> results = new List<XmlRole>();
				List<XmlRole> roles = this.Roles;
				if(roles != null)
				{
					foreach (XmlRole r in roles)
					{
						if(r.Users.Contains(userName))
							results.Add(r);
					}
				}
				return results;
			}
		}

		/// <summary>
		/// Gets the users in role.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		public virtual string[] GetUsersInRole(string roleName)
		{
			lock (SyncRoot)
			{
				XmlRole role = GetRole(roleName);
				if(role != null)
				{
					string[] Results = new string[role.Users.Count];
					role.Users.CopyTo(Results, 0);
					return Results;
				}
				else
					throw new Exception(string.Format("Role with name {0} does not exist!", roleName));
			}
		}
	}
}
