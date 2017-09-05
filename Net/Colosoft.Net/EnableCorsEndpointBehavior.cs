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
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.ServiceModel.Cors
{
	/// <summary>
	/// Implementação do comportamento de endpoint para habilitar o CORS.
	/// </summary>
	class EnableCorsEndpointBehavior : IEndpointBehavior
	{
		/// <summary>
		/// Adiciona os parametros de vinculação.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento do cliente.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="clientRuntime"></param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		/// <summary>
		/// Aplica o comportamento do dispatchante.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			List<OperationDescription> corsEnabledOperations = endpoint.Contract.Operations.Where(o => o.Behaviors.Find<EnableCorsAttribute>() != null).ToList();
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsEnabledMessageInspector(corsEnabledOperations));
		}

		/// <summary>
		/// Valida o endpoint.
		/// </summary>
		/// <param name="endpoint"></param>
		public void Validate(ServiceEndpoint endpoint)
		{
			var corsEnabledOperations = endpoint.Contract.Operations.Where(o => o.Behaviors.Find<EnableCorsAttribute>() != null).ToList();
			AddPreflightOperations(endpoint, corsEnabledOperations);
		}

		/// <summary>
		/// Adiciona a operação Preflight.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="corsOperations"></param>
		private void AddPreflightOperations(ServiceEndpoint endpoint, List<OperationDescription> corsOperations)
		{
			Dictionary<string, PreflightOperationBehavior> uriTemplates = new Dictionary<string, PreflightOperationBehavior>(StringComparer.OrdinalIgnoreCase);
			foreach (var operation in corsOperations)
			{
				if(operation.Behaviors.Find<System.ServiceModel.Web.WebGetAttribute>() != null)
				{
					continue;
				}
				if(operation.IsOneWay)
				{
					continue;
				}
				string originalUriTemplate;
				var originalWia = operation.Behaviors.Find<System.ServiceModel.Web.WebInvokeAttribute>();
				if(originalWia != null && originalWia.UriTemplate != null)
				{
					originalUriTemplate = NormalizeTemplate(originalWia.UriTemplate);
				}
				else
				{
					originalUriTemplate = operation.Name;
				}
				string originalMethod = originalWia != null && originalWia.Method != null ? originalWia.Method : "POST";
				if(uriTemplates.ContainsKey(originalUriTemplate))
				{
					PreflightOperationBehavior operationBehavior = uriTemplates[originalUriTemplate];
					operationBehavior.AddAllowedMethod(originalMethod);
				}
				else
				{
					ContractDescription contract = operation.DeclaringContract;
					OperationDescription preflightOperation = new OperationDescription(operation.Name + CorsConstants.PreflightSuffix, contract);
					MessageDescription inputMessage = new MessageDescription(operation.Messages[0].Action + CorsConstants.PreflightSuffix, MessageDirection.Input);
					inputMessage.Body.Parts.Add(new MessagePartDescription("input", contract.Namespace) {
						Index = 0,
						Type = typeof(Message)
					});
					preflightOperation.Messages.Add(inputMessage);
					MessageDescription outputMessage = new MessageDescription(operation.Messages[1].Action + CorsConstants.PreflightSuffix, MessageDirection.Output);
					outputMessage.Body.ReturnValue = new MessagePartDescription(preflightOperation.Name + "Return", contract.Namespace) {
						Type = typeof(Message)
					};
					preflightOperation.Messages.Add(outputMessage);
					var wia = new System.ServiceModel.Web.WebInvokeAttribute();
					wia.UriTemplate = originalUriTemplate;
					wia.Method = "OPTIONS";
					preflightOperation.Behaviors.Add(wia);
					preflightOperation.Behaviors.Add(new DataContractSerializerOperationBehavior(preflightOperation));
					PreflightOperationBehavior preflightOperationBehavior = new PreflightOperationBehavior(preflightOperation);
					preflightOperationBehavior.AddAllowedMethod(originalMethod);
					preflightOperation.Behaviors.Add(preflightOperationBehavior);
					uriTemplates.Add(originalUriTemplate, preflightOperationBehavior);
					contract.Operations.Add(preflightOperation);
				}
			}
		}

		private string NormalizeTemplate(string uriTemplate)
		{
			int queryIndex = uriTemplate.IndexOf('?');
			if(queryIndex >= 0)
			{
				uriTemplate = uriTemplate.Substring(0, queryIndex);
			}
			int paramIndex;
			while ((paramIndex = uriTemplate.IndexOf('{')) >= 0)
			{
				int endParamIndex = uriTemplate.IndexOf('}', paramIndex);
				if(endParamIndex >= 0)
				{
					uriTemplate = uriTemplate.Substring(0, paramIndex) + '*' + uriTemplate.Substring(endParamIndex + 1);
				}
			}
			return uriTemplate;
		}
	}
}
