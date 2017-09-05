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

namespace Colosoft.Net.Remote.Client.ServiceAddressProviderServiceReference
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "ServiceAddressProviderServiceReference.IServiceAddressProviderService")]
	internal interface IServiceAddressProviderService
	{
		[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IServiceAddressProviderService/GetServiceAddresses", ReplyAction = "http://tempuri.org/IServiceAddressProviderService/GetServiceAddressesResponse")]
		Colosoft.Net.ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext);
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
	internal interface IServiceAddressProviderServiceChannel : Colosoft.Net.Remote.Client.ServiceAddressProviderServiceReference.IServiceAddressProviderService, System.ServiceModel.IClientChannel
	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
	internal partial class ServiceAddressProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Net.Remote.Client.ServiceAddressProviderServiceReference.IServiceAddressProviderService>, Colosoft.Net.Remote.Client.ServiceAddressProviderServiceReference.IServiceAddressProviderService
	{
		public ServiceAddressProviderServiceClient()
		{
		}

		public ServiceAddressProviderServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public ServiceAddressProviderServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ServiceAddressProviderServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ServiceAddressProviderServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public Colosoft.Net.ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext)
		{
			return base.Channel.GetServiceAddresses(serviceName, servicesContext);
		}
	}
}
