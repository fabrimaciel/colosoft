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
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Colosoft.Net
{
	/// <summary>
	/// Representa o comportamento dos serviços para dar suporta ao token de segunrança.
	/// </summary>
	public class SecurityTokenBehavior : Attribute, IEndpointBehavior, IServiceBehavior
	{
		/// <summary>
		/// Registra o comportamento para o endpoint informado.
		/// </summary>
		/// <param name="endpoint"></param>
		public static void Register(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
			endpoint.Behaviors.Add(new SecurityTokenBehavior());
		}

		/// <summary>
		/// Registra as credenciais para o serviço.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="service"></param>
		public static void RegisterCredentials<TService>(TService service) where TService : System.ServiceModel.ICommunicationObject
		{
			System.Security.Principal.IPrincipal currentPrincipal = Colosoft.Security.UserContext.Current.Principal;
			if((currentPrincipal != null) && (currentPrincipal.Identity != null))
			{
				var identity = currentPrincipal.Identity;
				var credentials = (System.ServiceModel.Description.ClientCredentials)typeof(TService).GetProperty("ClientCredentials").GetValue(service, null);
				credentials.UserName.UserName = identity.Name;
			}
		}

		/// <summary>
		/// Método usado para criar a inspetor de mensagens que será usado.
		/// </summary>
		/// <returns></returns>
		protected virtual SecurityTokenMessageInspector CreateMessageInspector()
		{
			return new SecurityTokenMessageInspector();
		}

		/// <summary>
		/// Adiciona parametros do binding do EndPoint.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento do cliente.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="clientRuntime"></param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(CreateMessageInspector());
		}

		/// <summary>
		/// Aplica o comportamento do despachante das mensagens.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(CreateMessageInspector());
		}

		/// <summary>
		/// Valida o ponto do serviço.
		/// </summary>
		/// <param name="endpoint"></param>
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		/// <summary>
		/// Aplica os parametros para o serviço.
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		/// <param name="endpoints"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento do despachante do serviço.
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
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
		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
		}
	}
}
