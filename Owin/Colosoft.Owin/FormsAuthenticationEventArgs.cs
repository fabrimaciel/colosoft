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

namespace Colosoft.Owin.Server.Security
{
	/// <summary>
	/// Representa os argumentos da FormsAuthentication.
	/// </summary>
	sealed class FormsAuthenticationEventArgs : EventArgs
	{
		private HttpContext _context;

		private System.Security.Principal.IPrincipal _user;

		/// <summary>
		/// Contexto associado.
		/// </summary>
		public HttpContext Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Usuário associado,
		/// </summary>
		public System.Security.Principal.IPrincipal User
		{
			get
			{
				return _user;
			}
			[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, ControlPrincipal = true)]
			set
			{
				_user = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		public FormsAuthenticationEventArgs(HttpContext context)
		{
			this._context = context;
		}
	}
}
