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

namespace Colosoft.Security
{
	/// <summary>
	/// Representa um origem do sistema.
	/// </summary>
	public interface IAuthenticationSource
	{
		/// <summary>
		/// Identificador único da origem de autenticação.
		/// </summary>
		int Uid
		{
			get;
		}

		/// <summary>
		/// Nome da origem.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Identifica se a origem está ativa.
		/// </summary>
		bool IsActive
		{
			get;
		}
	}
	/// <summary>
	/// Implementação Fake do IAutheticationSource.
	/// </summary>
	class AuthenticationSource : IAuthenticationSource, System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Identificador único da origem.
		/// </summary>
		public int Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da origem.
		/// </summary>
		public string FullName
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a origem está ativa.
		/// </summary>
		public bool IsActive
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AuthenticationSource()
		{
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AuthenticationSource(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			Uid = info.GetInt32("Uid");
			FullName = info.GetString("FullName");
			IsActive = info.GetBoolean("IsActive");
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Uid", Uid);
			info.AddValue("FullName", FullName);
			info.AddValue("IsActive", IsActive);
		}
	}
}
