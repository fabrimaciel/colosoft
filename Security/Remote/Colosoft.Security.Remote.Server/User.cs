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
using System.Runtime.Serialization;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Classe usada para adaptar a interface do usuário.
	/// </summary>
	[DataContract]
	public class User : Colosoft.Security.Authentication.IAutheticableUser
	{
		/// <summary>
		/// Id do usuário
		/// </summary>
		public int UserId
		{
			get;
			set;
		}

		/// <summary>
		/// Email do usuário.
		/// </summary>
		[DataMember]
		public string Email
		{
			get;
			set;
		}

		/// <summary>
		/// Nome completo do usuário.
		/// </summary>
		[DataMember]
		public string FullName
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se o usuário está online.
		/// </summary>
		[DataMember]
		public bool IsOnline
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se os dados do usuário foram aprovados.
		/// </summary>
		[DataMember]
		public bool IsApproved
		{
			get;
			set;
		}

		/// <summary>
		/// Data de criação do usuário.
		/// </summary>
		[DataMember]
		public DateTimeOffset CreationDate
		{
			get;
			set;
		}

		/// <summary>
		/// Data da ultima atividade registrada.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastActivityDate
		{
			get;
			set;
		}

		/// <summary>
		/// Data do ultimo login registrado.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastLoginDate
		{
			get;
			set;
		}

		/// <summary>
		/// Data da última modificação registrada.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastPasswordChangedDate
		{
			get;
			set;
		}

		/// <summary>
		/// Pergunta para descobrir a senha.
		/// </summary>
		[DataMember]
		public string PasswordQuestion
		{
			get;
			set;
		}

		/// <summary>
		/// Chave única do usuário.
		/// </summary>
		[DataMember]
		public string UserKey
		{
			get
			{
				return UserId.ToString();
			}
			set
			{
				UserId = Convert.ToInt32(value);
			}
		}

		/// <summary>
		/// Nome do usuário para acesso.
		/// </summary>
		[DataMember]
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do provedor de identidade do usuário.
		/// </summary>
		[DataMember]
		public string IdentityProvider
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que o usuário irá ignorar o captcha do sistema
		/// </summary>
		[DataMember]
		public bool IgnoreCaptcha
		{
			get;
			set;
		}

		/// <summary>
		/// Resposta para descobrir a senha.
		/// </summary>
		public string PasswordAnswer
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do provedor de identidade associado.
		/// </summary>
		public int IdentityProviderId
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se o usuário está ativo.
		/// </summary>
		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public User()
		{
		}

		/// <summary>
		/// Cria uma nova instancia adaptando a instancia do usuário informada.
		/// </summary>
		/// <param name="user"></param>
		public User(IUser user)
		{
			this.Email = user.Email;
			this.FullName = user.FullName;
			this.LastPasswordChangedDate = user.LastPasswordChangedDate;
			this.PasswordQuestion = user.PasswordQuestion;
			this.UserKey = user.UserKey;
			this.UserName = user.UserName;
			this.IsApproved = user.IsApproved;
			this.CreationDate = user.CreationDate;
			this.IgnoreCaptcha = user.IgnoreCaptcha;
			this.IsActive = user.IsActive;
		}
	}
}
