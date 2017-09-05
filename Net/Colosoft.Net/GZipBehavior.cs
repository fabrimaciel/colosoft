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
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação do comportamento do GZip.
	/// </summary>
	public class GZipBehavior : IEndpointBehavior, IServiceBehavior
	{
		/// <summary>
		/// Método usado para criar a inspetor de mensagens que será usado.
		/// </summary>
		/// <returns></returns>
		protected virtual GZipInspector CreateMessageInspector()
		{
			return new GZipInspector();
		}

		/// <summary>
		/// Registra o comportamento para o endpoint informado.
		/// </summary>
		/// <param name="endpoint"></param>
		public static void Register(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
			endpoint.Behaviors.Add(new GZipBehavior());
		}

		/// <summary>
		/// Adiciona os parametros de bingind.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="bindingParameters"></param>
		void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento no lado do client.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="clientRuntime"></param>
		void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(CreateMessageInspector());
		}

		/// <summary>
		/// Aplica o comportamento do dispatcher.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(CreateMessageInspector());
		}

		/// <summary>
		/// Valida o endereço.
		/// </summary>
		/// <param name="endpoint"></param>
		void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
		{
		}

		/// <summary>
		/// Aplica os parametros para o serviço.
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		/// <param name="endpoints"></param>
		/// <param name="bindingParameters"></param>
		void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento do despachante do serviço.
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			foreach (System.ServiceModel.Dispatcher.ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
			{
				foreach (var endPointDispatcher in channelDispatcher.Endpoints)
				{
					endPointDispatcher.DispatchRuntime.MessageInspectors.Add(CreateMessageInspector());
				}
			}
		}

		/// <summary>
		/// Valida a descrição do serviço.
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		void IServiceBehavior.Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
		}
	}
}
