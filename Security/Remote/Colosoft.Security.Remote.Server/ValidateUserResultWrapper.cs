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
using Colosoft.Security.CaptchaSupport;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Resultado da validação de usuário
	/// </summary>
	[DataContract(Name = "ValidateUserResult")]
	public class ValidateUserResultWrapper
	{
		/// <summary>
		/// Status
		/// </summary>
		[DataMember]
		public AuthenticationStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que a próxima tentativa de login deverá conter o captcha
		/// </summary>
		[DataMember]
		public CaptchaInfo Captcha
		{
			get;
			set;
		}

		/// <summary>
		/// Data em que o password irá expirar.
		/// </summary>
		[DataMember]
		public DateTimeOffset? ExpireDate
		{
			get;
			set;
		}

		/// <summary>
		/// Token que identifica a autenticação.
		/// </summary>
		[DataMember]
		public string Token
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem a ser apresentada
		/// </summary>
		[DataMember]
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Usuário autenticado em caso de sucesso
		/// </summary>
		[DataMember]
		public User User
		{
			get;
			set;
		}

		/// <summary>
		/// Endereço do serviço do provedor do usuário.
		/// </summary>
		[DataMember]
		public Colosoft.Net.ServiceAddress UserProviderServiceAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Endereço do serviço do provedor de perfil.
		/// </summary>
		[DataMember]
		public Colosoft.Net.ServiceAddress ProfileProviderServiceAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Endereço do serviço do provedor de endereços dos serviços.
		/// </summary>
		[DataMember]
		public Colosoft.Net.ServiceAddress ServiceAddressProviderServiceAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instance">Instancia que será adaptada.</param>
		/// <param name="userProviderServiceAddress">Endereço do serviço do provedor dos usuários.</param>
		/// <param name="profileProviderServiceAddress">Endereço do serviço do provedor dos perfis.</param>
		/// <param name="serviceAddressProviderServiceAddress">Endereço do serviço do provedor dos endereços dos serviços.</param>
		public ValidateUserResultWrapper(IValidateUserResult instance, Colosoft.Net.ServiceAddress userProviderServiceAddress, Colosoft.Net.ServiceAddress profileProviderServiceAddress, Colosoft.Net.ServiceAddress serviceAddressProviderServiceAddress)
		{
			Status = instance.Status;
			Message = instance.Message;
			User = instance.User != null ? new User(instance.User) : null;
			Captcha = instance.Captcha;
			ExpireDate = instance.ExpireDate;
			Token = instance.Token;
			UserProviderServiceAddress = userProviderServiceAddress;
			ProfileProviderServiceAddress = profileProviderServiceAddress;
			ServiceAddressProviderServiceAddress = serviceAddressProviderServiceAddress;
		}
	}
}
