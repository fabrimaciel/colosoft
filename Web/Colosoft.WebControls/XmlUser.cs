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

namespace Colosoft.WebControls.Security.Store
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlUser
	{
		public Guid UserKey = Guid.Empty;

		public string UserName = string.Empty;

		public string Password = string.Empty;

		public string PasswordSalt = string.Empty;

		public string Email = string.Empty;

		public string PasswordQuestion = string.Empty;

		public string PasswordAnswer = string.Empty;

		public string Comment;

		public DateTime CreationDate = DateTime.Now;

		public DateTime LastActivityDate = DateTime.MinValue;

		public DateTime LastLoginDate = DateTime.MinValue;

		public DateTime LastPasswordChangeDate = DateTime.MinValue;

		public DateTime LastLockoutDate = DateTime.MaxValue;

		public bool IsApproved = true;

		public bool IsLockedOut = false;
	}
}
