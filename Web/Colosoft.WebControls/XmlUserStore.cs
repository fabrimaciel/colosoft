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
using System.IO;
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
	public partial class XmlUserStore : Persistable<List<XmlUser>>
	{
		/// <summary>
		/// Gets the users.
		/// </summary>
		/// <value>The users.</value>
		public virtual List<XmlUser> Users
		{
			get
			{
				return this.Value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserStore"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public XmlUserStore(string fileName) : base(fileName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlUserStore"/> class.
		/// </summary>
		protected XmlUserStore() : base(null)
		{
		}

		/// <summary>
		/// Gets the user by email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		public virtual XmlUser GetUserByEmail(string email)
		{
			lock (SyncRoot)
			{
				return (Users != null) ? Users.Find(delegate(XmlUser user) {
					return string.Equals(email, user.Email);
				}) : null;
			}
		}

		/// <summary>
		/// Gets the user by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public virtual XmlUser GetUserByKey(Guid key)
		{
			lock (SyncRoot)
			{
				return (Users != null) ? Users.Find(delegate(XmlUser user) {
					return (user.UserKey.CompareTo(key) == 0);
				}) : null;
			}
		}

		/// <summary>
		/// Gets the name of the user by.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual XmlUser GetUserByName(string name)
		{
			lock (SyncRoot)
			{
				return (Users != null) ? Users.Find(delegate(XmlUser user) {
					return string.Equals(name, user.UserName);
				}) : null;
			}
		}
	}
}
