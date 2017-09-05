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

namespace Colosoft.Security
{
	/// <summary>
	/// Armazena os dados da identidade de o usuário do sistema.
	/// </summary>
	public class UserIdentity : IIdentity, IEnumerable<KeyValuePair<string, object>>
	{
		/// <summary>
		/// Instancia do IIdentity adaptada.
		/// </summary>
		private readonly IIdentity _identity;

		private readonly IUser _user;

		/// <summary>
		/// Dicionário de informações agregadas para a identificação.
		/// </summary>
		private readonly Dictionary<string, object> _items;

		/// <summary>
		/// Instancia do usuário relacionado.
		/// </summary>
		public IUser User
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		/// Recupera o valor do item pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do item que será recuperado.</param>
		/// <returns></returns>
		public object this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				object result = null;
				if(_items.TryGetValue(name, out result))
					return result;
				return null;
			}
			set
			{
				if(name == null)
					throw new ArgumentNullException("name");
				_items.Remove(name);
				_items.Add(name, value);
			}
		}

		/// <summary>
		/// Quantidade de itens armazenados.
		/// </summary>
		public int ItemsCount
		{
			get
			{
				return _items.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="identity">Instancia que será adaptada.</param>
		public UserIdentity(IIdentity identity)
		{
			if(identity == null)
				throw new ArgumentNullException("identity");
			_items = new Dictionary<string, object>();
			_identity = identity;
			_user = Membership.GetUser(identity.Name);
		}

		/// <summary>
		/// Verifica se existem o item com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			return _items.ContainsKey(name);
		}

		/// <summary>
		/// Apaga 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>True caso o item tenha sido removido.</returns>
		public bool Remove(string name)
		{
			return _items.Remove(name);
		}

		/// <summary>
		/// Tipo de autenticação.
		/// </summary>
		public string AuthenticationType
		{
			get
			{
				return _identity.AuthenticationType;
			}
		}

		/// <summary>
		/// Identifica se a identidade está autenticada.
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return _identity.IsAuthenticated;
			}
		}

		/// <summary>
		/// Nome associado com a identidade.
		/// </summary>
		public string Name
		{
			get
			{
				return _identity.Name;
			}
		}

		/// <summary>
		/// Recupera o enumerador dos itens da identidade.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}
