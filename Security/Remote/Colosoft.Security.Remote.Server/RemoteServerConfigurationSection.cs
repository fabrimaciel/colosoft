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
using System.Configuration;
using System.Diagnostics;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Representa uma seção de configuração do servidor remoto.
	/// </summary>
	public class RemoteServerConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Nome do serviço do provedor de usuários.
		/// </summary>
		[ConfigurationProperty("userProviderServiceName", DefaultValue = "UserProviderService", IsRequired = true)]
		public string UserProviderServiceName
		{
			[DebuggerStepThrough]
			get
			{
				return base["userProviderServiceName"] as string;
			}
		}

		/// <summary>
		/// Nome do serviço do providor de perfis.
		/// </summary>
		[ConfigurationProperty("profileProviderServiceName", DefaultValue = "ProfileProviderService", IsRequired = true)]
		public string ProfileProviderServiceName
		{
			[DebuggerStepThrough]
			get
			{
				return base["profileProviderServiceName"] as string;
			}
		}

		/// <summary>
		/// Nome do serviço do provedor de endereços dos serviços.
		/// </summary>
		[ConfigurationProperty("serviceAddressProviderServiceName", DefaultValue = "ServiceAddressProviderService", IsRequired = true)]
		public string ServiceAddressProviderServiceName
		{
			[DebuggerStepThrough]
			get
			{
				return base["serviceAddressProviderServiceName"] as string;
			}
		}
	}
}
