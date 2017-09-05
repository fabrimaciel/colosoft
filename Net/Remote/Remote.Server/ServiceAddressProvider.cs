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

namespace Colosoft.Net.Remote.Server
{
	/// <summary>
	/// Implementação do <see cref="IServiceAddressProvider"/> que acesso
	/// a parte de negócio do Net.
	/// </summary>
	public class ServiceAddressProvider : IServiceAddressProvider
	{
		/// <summary>
		/// Recupera os endereços do serviço pelo nome informado.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="servicesContext"></param>
		/// <returns></returns>
		public ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext)
		{
			return Colosoft.Net.Business.FlowFactory.GetServiceFlow().GetServiceAddresses(serviceName, servicesContext).Select(f => f.Convert()).Where(f => f != null).ToArray();
		}

		/// <summary>
		/// Nome do endereço do provedor.
		/// </summary>
		public string ProviderAddressName
		{
			get
			{
				return "ServiceAddressProviderService2";
			}
		}
	}
}
