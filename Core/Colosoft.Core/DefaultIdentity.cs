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

namespace Colosoft.Security.Principal
{
	/// <summary>
	/// Implementação padrão do <see cref="System.Security.Principal.IIdentity"/> usado pelo sistema.
	/// </summary>
	public class DefaultIdentity : System.Security.Principal.IIdentity
	{
		private string _token;

		private string _name;

		private string _authenticationType;

		private bool _isAutenticated;

		private System.Collections.Hashtable _parameters = new System.Collections.Hashtable();

		/// <summary>
		/// Token associado.
		/// </summary>
		public string Token
		{
			get
			{
				return _token;
			}
			internal set
			{
				_token = value;
			}
		}

		/// <summary>
		/// Nome do usuário da identidade.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			internal set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Tipo de autenticação da identidade;
		/// </summary>
		public string AuthenticationType
		{
			get
			{
				return _authenticationType;
			}
			internal set
			{
				_authenticationType = value;
			}
		}

		/// <summary>
		/// Identifica o usuário da identidade está autenticado.
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return _isAutenticated;
			}
			internal set
			{
				_isAutenticated = value;
			}
		}

		/// <summary>
		/// Parametros.
		/// </summary>
		public System.Collections.Hashtable Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do usuário cujo código está rodando.</param>
		/// <param name="token">Token do usuário.</param>
		/// <param name="isAuthenticated">Usuário autenticado.</param>
		/// <param name="type">Tipo da autenticação.</param>
		public DefaultIdentity(string name, string token, bool isAuthenticated = false, string type = null)
		{
			_token = token;
			_name = name;
			_authenticationType = type;
			_isAutenticated = isAuthenticated;
		}

		/// <summary>
		/// Redefine os dados.
		/// </summary>
		/// <param name="name">Nome do usuário cujo código está rodando.</param>
		/// <param name="token">Token do usuário.</param>
		/// <param name="isAuthenticated">Usuário autenticado.</param>
		/// <param name="type">Tipo da autenticação.</param>
		public void SetValues(string name, string token, bool isAuthenticated, string type = null)
		{
			_name = name;
			_token = token;
			_authenticationType = type;
			_isAutenticated = isAuthenticated;
		}
	}
}
