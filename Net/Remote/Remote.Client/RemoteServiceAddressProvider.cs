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
using Colosoft.Net;

namespace Colosoft.Net.Remote.Client
{
	/// <summary>
	/// Implementação do acesso remoto ao provedor dos endereços dos serviços.
	/// </summary>
	public sealed class RemoteServiceAddressProvider : IServiceAddressProvider, IDisposable
	{
		private readonly string _clientUid = Guid.NewGuid().ToString();

		/// <summary>
		/// Nome do enderelo do provedor.
		/// </summary>
		public string ProviderAddressName
		{
			get
			{
				return "ServiceAddressProviderService";
			}
		}

		/// <summary>
		/// Instancia do cliente do serviço.
		/// </summary>
		private ServiceAddressProviderServiceReference.ServiceAddressProviderServiceClient Client
		{
			get
			{
				return Colosoft.Net.ServiceClientsManager.Current.Get<ServiceAddressProviderServiceReference.ServiceAddressProviderServiceClient>(_clientUid);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public RemoteServiceAddressProvider()
		{
			Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
			Colosoft.Net.ServiceClientsManager.Current.Register(_clientUid, () =>  {
				if(!Net.ServicesConfiguration.Current.Contains("ServiceAddressProviderService", false))
					throw new InvalidOperationException("ServiceAddressProviderService address not found.");
				var serviceAddress = Net.ServicesConfiguration.Current["ServiceAddressProviderService"];
				var binding = serviceAddress.GetBinding();
				var endpoint = serviceAddress.GetEndpointAddress();
				var serviceClient = new ServiceAddressProviderServiceReference.ServiceAddressProviderServiceClient(binding, endpoint);
				Colosoft.Net.SecurityTokenBehavior.Register(serviceClient.Endpoint);
				return serviceClient;
			});
		}

		/// <summary>
		/// Método acionado quando o endereço do serviço for alterado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ServicesConfigurationUpdated(object sender, Net.ServicesConfigurationActionEventArgs e)
		{
			if(e.ServiceName == "ServiceAddressProviderService")
				Colosoft.Net.ServiceClientsManager.Current.Reset(_clientUid);
		}

		/// <summary>
		/// Recupera os endereços associados com o serviço.
		/// </summary>
		/// <param name="serviceName">Nome do serviço.</param>
		/// <param name="servicesContext">Contexto de serviços.</param>
		/// <returns></returns>
		public ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext)
		{
			if(Net.ServicesConfiguration.Current.Contains("ServiceAddressProviderService", false))
				return Client.GetServiceAddresses(serviceName, servicesContext);
			return new ServiceAddress[0];
		}

		/// <summary>
		/// Define o endereço do serviço do provedor de perfis.
		/// </summary>
		/// <param name="address"></param>
		public static void SetServiceAddress(Colosoft.Net.ServiceAddress address)
		{
			Net.ServicesConfiguration.Current["ServiceAddressProviderService"] = address;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Net.ServicesConfiguration.Current.Updated -= ServicesConfigurationUpdated;
			Colosoft.Net.ServiceClientsManager.Current.Remove(_clientUid);
		}
	}
}
