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

namespace Colosoft.Domain
{
	/// <summary>
	/// Representa um token com os dados da inscrição de um evento no domínio.
	/// </summary>
	public class SubscriptionToken : IEquatable<SubscriptionToken>
	{
		private readonly Guid _token;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SubscriptionToken()
		{
			_token = Guid.NewGuid();
		}

		/// <summary>
		/// Verifica se o atual objeto é igual a instancia informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(SubscriptionToken other)
		{
			if(other == null)
				return false;
			return Equals(_token, other._token);
		}

		///<summary>
		/// Determina se a instancia informado é igual a atual instancia.
		///</summary>
		///<returns></returns>
		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this, obj))
				return true;
			return Equals(obj as SubscriptionToken);
		}

		/// <summary>
		/// Recupera o hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _token.GetHashCode();
		}
	}
}
