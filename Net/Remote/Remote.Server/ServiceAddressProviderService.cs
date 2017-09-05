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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Colosoft.Net.Remote.Server
{
	/// <summary>
	/// Implementação da interface de serviço <see cref="IServiceAddressProviderService"/>.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
	public class ServiceAddressProviderService : IServiceAddressProviderService
	{
		private IServiceAddressProvider _serviceAddressProvider;

		/// <summary>
		/// Instancia do provedor de serviços adaptado pela instancia.
		/// </summary>
		private IServiceAddressProvider ServiceAddressProvider
		{
			get
			{
				if(_serviceAddressProvider == null)
					_serviceAddressProvider = CreateInstance();
				return _serviceAddressProvider;
			}
		}

		/// <summary>
		/// Recupera a instancia do provider de aplicações que será usado no serviço.
		/// </summary>
		/// <returns></returns>
		private IServiceAddressProvider CreateInstance()
		{
			IServiceAddressProvider provider = null;
			try
			{
				provider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IServiceAddressProvider>();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperation_ServiceProviderNotFound).Format(), ex);
			}
			return provider;
		}

		/// <summary>
		/// Recupera os endereços do serviço com base no nome informado.
		/// </summary>
		/// <param name="serviceName">Nome do serviço.</param>
		/// <param name="servicesContext">Contexto de serviços</param>
		/// <returns></returns>
		public Colosoft.Net.ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext)
		{
			return ServiceAddressProvider.GetServiceAddresses(serviceName, servicesContext);
		}
	}
}
