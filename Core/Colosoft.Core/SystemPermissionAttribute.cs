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
using System.Security.Permissions;

namespace Colosoft.Security.Permissions
{
	/// <summary>
	/// Atributo usado para definir permissão para execução de operações.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public class SystemPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>
		/// Tipo de acesso.
		/// </summary>
		public SystemPermissionAccess Access
		{
			get;
			set;
		}

		/// <summary>
		/// Ação de acesso.
		/// </summary>
		public System.Security.AccessControl.AccessControlActions AccessAction
		{
			get;
			set;
		}

		/// <summary>
		/// Caminho da permissão.
		/// </summary>
		public string Path
		{
			get;
			set;
		}

		/// <summary>
		/// Permissão.
		/// </summary>
		public SystemPermission Permission
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="action"></param>
		public SystemPermissionAttribute(SecurityAction action) : base(action)
		{
			Access = SystemPermissionAccess.Execute;
			AccessAction = System.Security.AccessControl.AccessControlActions.None;
		}

		/// <summary>
		/// Cria um nova permissão com os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public override System.Security.IPermission CreatePermission()
		{
			return Permission ?? new SystemPermission(Access, AccessAction, Path);
		}
	}
}
