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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.ServiceModel.Cors
{
	/// <summary>
	/// Implementação do inspetor do despachante.
	/// </summary>
	class CorsEnabledMessageInspector : IDispatchMessageInspector
	{
		private List<string> _corsEnabledOperationNames;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="corsEnabledOperations"></param>
		public CorsEnabledMessageInspector(List<OperationDescription> corsEnabledOperations)
		{
			_corsEnabledOperationNames = corsEnabledOperations.Select(o => o.Name).ToList();
		}

		/// <summary>
		/// Trata a requisiçãp depois de receber.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <param name="instanceContext"></param>
		/// <returns></returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			HttpRequestMessageProperty httpProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
			object operationName;
			request.Properties.TryGetValue(WebHttpDispatchOperationSelector.HttpOperationNamePropertyName, out operationName);
			if(httpProp != null && operationName != null && _corsEnabledOperationNames.Contains((string)operationName))
			{
				string origin = httpProp.Headers[CorsConstants.Origin];
				if(origin != null)
					return origin;
			}
			return null;
		}

		/// <summary>
		/// Trata a mensagem antes de enviar a resposta.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			HttpResponseMessageProperty httpProp = null;
			if(reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
			{
				httpProp = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
			}
			else
			{
				httpProp = new HttpResponseMessageProperty();
				reply.Properties.Add(HttpResponseMessageProperty.Name, httpProp);
			}
			string origin = correlationState as string;
			if(origin != null)
				httpProp.Headers.Add(CorsConstants.AccessControlAllowOrigin, origin);
			httpProp.Headers.Add(CorsConstants.AccessControlAllowCredentials, "true");
		}
	}
}
