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
using System.ServiceModel.Description;
using System.Text;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Implementação do comportamento para tratar falhas no formato json.
	/// </summary>
	public class JsonFaultBehavior : IEndpointBehavior
	{
		/// <summary>
		/// Adiciona os parametros do binding.
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
		}

		/// <summary>
		/// Aplica o comportamento do dispatcher.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			var handler = new JsonFaultHandler();
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(handler);
		}

		/// <summary>
		/// Valida o endpoint.
		/// </summary>
		/// <param name="endpoint"></param>
		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
