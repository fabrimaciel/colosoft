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

namespace Colosoft.Query.Linq.Translators
{
	/// <summary>
	/// Representa um membro da projeção.
	/// </summary>
	class Member : IEquatable<Member>
	{
		/// <summary>
		/// Nome do pai do membro.
		/// </summary>
		public string Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do membro da projeção.
		/// </summary>
		public System.Reflection.MemberInfo Info
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="member"></param>
		public Member(string owner, System.Reflection.MemberInfo member)
		{
			Owner = owner;
			Info = member;
		}

		/// <summary>
		/// Verifica se o membro é igual ao membro informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Member other)
		{
			if(other == null)
				return false;
			return this.Owner == other.Owner && this.Info == other.Info;
		}
	}
}
