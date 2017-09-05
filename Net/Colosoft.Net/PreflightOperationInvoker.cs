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
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Collections.Generic;

namespace Colosoft.Net.ServiceModel.Cors
{
	/// <summary>
	/// PreflightOperationInvoker
	/// </summary>
	class PreflightOperationInvoker : IOperationInvoker
	{
		private string _replyAction;

		List<string> _allowedHttpMethods;

		/// <summary>
		/// Identifica se é sincrono.
		/// </summary>
		public bool IsSynchronous
		{
			get
			{
				return true;
			}
		}

		public PreflightOperationInvoker(string replyAction, List<string> allowedHttpMethods)
		{
			this._replyAction = replyAction;
			this._allowedHttpMethods = allowedHttpMethods;
		}

		/// <summary>
		/// Trata o Preflight.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private Message HandlePreflight(Message input)
		{
			HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)input.Properties[HttpRequestMessageProperty.Name];
			string origin = httpRequest.Headers[CorsConstants.Origin];
			string requestMethod = httpRequest.Headers[CorsConstants.AccessControlRequestMethod];
			string requestHeaders = httpRequest.Headers[CorsConstants.AccessControlRequestHeaders];
			Message reply = Message.CreateMessage(MessageVersion.None, _replyAction);
			HttpResponseMessageProperty httpResponse = new HttpResponseMessageProperty();
			reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
			httpResponse.SuppressEntityBody = true;
			httpResponse.StatusCode = HttpStatusCode.OK;
			if(origin != null)
			{
				httpResponse.Headers.Add(CorsConstants.AccessControlAllowOrigin, origin);
			}
			if(requestMethod != null && this._allowedHttpMethods.Contains(requestMethod))
			{
				httpResponse.Headers.Add(CorsConstants.AccessControlAllowMethods, string.Join(",", this._allowedHttpMethods));
			}
			if(requestHeaders != null)
			{
				httpResponse.Headers.Add(CorsConstants.AccessControlAllowHeaders, requestHeaders);
			}
			return reply;
		}

		/// <summary>
		/// Entradas alocadas.
		/// </summary>
		/// <returns></returns>
		public object[] AllocateInputs()
		{
			return new object[1];
		}

		/// <summary>
		/// Realiza a chamada.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="inputs"></param>
		/// <param name="outputs"></param>
		/// <returns></returns>
		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			var input = (Message)inputs[0];
			outputs = null;
			return HandlePreflight(input);
		}

		/// <summary>
		/// Realiza a chamada de forma assincrona.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="inputs"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("Only synchronous invocation");
		}

		/// <summary>
		/// Finaliza a chamada assincrona.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="outputs"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		{
			throw new NotSupportedException("Only synchronous invocation");
		}
	}
}
