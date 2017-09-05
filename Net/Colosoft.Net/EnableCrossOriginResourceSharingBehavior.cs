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
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;

namespace Colosoft.Net.ServiceModel.Configuration
{
	/// <summary>
	/// Implementação do comportamento para habilitar o compartilhar recursos entre dominios.
	/// </summary>
	public class EnableCrossOriginResourceSharingBehavior : BehaviorExtensionElement, IEndpointBehavior
	{
		private Cors.EnableCorsEndpointBehavior _bahavior;

		/// <summary>
		/// Tipo do comportamento.
		/// </summary>
		public override Type BehaviorType
		{
			get
			{
				return typeof(EnableCrossOriginResourceSharingBehavior);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EnableCrossOriginResourceSharingBehavior()
		{
			_bahavior = new Cors.EnableCorsEndpointBehavior();
		}

		/// <summary>
		/// Adiciona os parametros de binding.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			_bahavior.AddBindingParameters(endpoint, bindingParameters);
		}

		/// <summary>
		/// Aplica o comportamento do cliente.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="clientRuntime"></param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			_bahavior.ApplyClientBehavior(endpoint, clientRuntime);
		}

		/// <summary>
		/// Aplica o comportamento do dispatch.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			_bahavior.ApplyDispatchBehavior(endpoint, endpointDispatcher);
		}

		/// <summary>
		/// Valida o endpoing
		/// </summary>
		/// <param name="endpoint"></param>
		public void Validate(ServiceEndpoint endpoint)
		{
			_bahavior.Validate(endpoint);
		}

		/// <summary>
		/// Cria uma instancia do comportamento.
		/// </summary>
		/// <returns></returns>
		protected override object CreateBehavior()
		{
			return new EnableCrossOriginResourceSharingBehavior();
		}
	}
}
