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
	public partial class XmlProfileStore : Persistable<List<XmlProfile>>
	{
		/// <summary>
		/// Gets the profiles.
		/// </summary>
		/// <value>The profiles.</value>
		public virtual List<XmlProfile> Profiles
		{
			get
			{
				return this.Value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public XmlProfileStore(string fileName) : base(fileName)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		protected XmlProfileStore() : base(null)
		{
		}

		/// <summary>
		/// Gets the user by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public XmlProfile GetByUserKey(Guid key)
		{
			lock (SyncRoot)
			{
				return (Profiles != null) ? Profiles.Find(delegate(XmlProfile profile) {
					return (profile.UserKey.CompareTo(key) == 0);
				}) : null;
			}
		}

		/// <summary>
		/// Gets the name of the by user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		public XmlProfile GetByUserName(string userName)
		{
			MembershipUser user = Membership.GetUser(userName);
			return (user != null) ? GetByUserKey((Guid)user.ProviderUserKey) : null;
		}

		/// <summary>
		/// Removes the name of the by user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		public void RemoveByUserName(string userName)
		{
			lock (SyncRoot)
			{
				XmlProfile profile = GetByUserName(userName);
				if(profile != null)
					Profiles.Remove(profile);
			}
		}
	}
}
