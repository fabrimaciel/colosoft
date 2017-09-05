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
using System.Security.Principal;

namespace Colosoft.Security.Principal
{
	/// <summary>
	/// Implementação padrão do IPrincipal.
	/// </summary>
	public class DefaultPrincipal : IPrincipal
	{
		private Func<IIdentity> _identityGetter;

		private IIdentity _identity;

		private string[] _roles;

		/// <summary>
		/// Cria a instancia informado o ponteiro do método para recupera a identidade.
		/// </summary>
		/// <param name="identityGetter"></param>
		public DefaultPrincipal(Func<IIdentity> identityGetter)
		{
			_identityGetter = identityGetter;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="identity"></param>
		public DefaultPrincipal(IIdentity identity)
		{
			_identity = identity;
		}

		/// <summary>
		/// Método para facilitar o acesso.
		/// </summary>
		public static DefaultPrincipal Current
		{
			get
			{
				return System.Threading.Thread.CurrentPrincipal as DefaultPrincipal;
			}
		}

		/// <summary>
		/// Instancia do Identity do principal.
		/// </summary>
		public IIdentity Identity
		{
			get
			{
				return _identityGetter != null ? _identityGetter() : _identity;
			}
		}

		/// <summary>
		/// Recupera todos os papéis associados com a instancia.
		/// </summary>
		public string[] Roles
		{
			get
			{
				EnsureRoles();
				if(_roles == null)
					return new string[0];
				return _roles;
			}
		}

		/// <summary>
		/// Verifica se está contido no papel.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public bool IsInRole(string role)
		{
			EnsureRoles();
			if(_roles != null)
				return _roles.Contains(role);
			return false;
		}

		/// <summary>
		/// Armazena os papéis no cache para requisições sobsequentes.
		/// </summary>
		protected virtual void EnsureRoles()
		{
			if(_roles == null && Identity != null)
				_roles = Security.Roles.GetRolesForUser(Identity.Name);
		}
	}
}
