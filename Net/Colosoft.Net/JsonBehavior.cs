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
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação base para o comportamento para Json.
	/// </summary>
	public abstract class JsonBehavior : System.ServiceModel.Description.WebHttpBehavior
	{
		/// <summary>
		/// Recupera o formatador do despachante.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="isRequest"></param>
		/// <returns></returns>
		protected abstract JsonDispatchFormatter GetDispatchFormatter(OperationDescription operation, bool isRequest);

		/// <summary>
		/// Recupera o formatador do cliente.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected abstract JsonClientFormatter GetClientFormatter(OperationDescription operation, ServiceEndpoint endpoint);

		/// <summary>
		/// Recupera o formatador do despachante  da requisição.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			if(this.IsGetOperation(operationDescription))
			{
				return base.GetRequestDispatchFormatter(operationDescription, endpoint);
			}
			if(operationDescription.Messages[0].Body.Parts.Count == 0)
			{
				return base.GetRequestDispatchFormatter(operationDescription, endpoint);
			}
			return GetDispatchFormatter(operationDescription, true);
		}

		/// <summary>
		/// Recupera o formatador do despachante da resposta.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			if(operationDescription.Messages.Count == 1 || operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void))
			{
				return base.GetReplyDispatchFormatter(operationDescription, endpoint);
			}
			else
			{
				return GetDispatchFormatter(operationDescription, false);
			}
		}

		/// <summary>
		/// Recupera o formatador do cliente da requisição.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			if(operationDescription.Behaviors.Find<WebGetAttribute>() != null)
			{
				return base.GetRequestClientFormatter(operationDescription, endpoint);
			}
			else
			{
				var wia = operationDescription.Behaviors.Find<WebInvokeAttribute>();
				if(wia != null)
				{
					if(wia.Method == "HEAD")
					{
						return base.GetRequestClientFormatter(operationDescription, endpoint);
					}
				}
			}
			if(operationDescription.Messages[0].Body.Parts.Count == 0)
			{
				return base.GetRequestClientFormatter(operationDescription, endpoint);
			}
			return GetClientFormatter(operationDescription, endpoint);
		}

		/// <summary>
		/// Recupera o formatador do cliente da resposta.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			if(operationDescription.Messages.Count == 1 || operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void))
			{
				return base.GetReplyClientFormatter(operationDescription, endpoint);
			}
			else
			{
				return GetClientFormatter(operationDescription, endpoint);
			}
		}

		/// <summary>
		/// Valida o endpoint.
		/// </summary>
		/// <param name="endpoint"></param>
		public override void Validate(ServiceEndpoint endpoint)
		{
			base.Validate(endpoint);
			var elements = endpoint.Binding.CreateBindingElements();
			var webEncoder = elements.Find<System.ServiceModel.Channels.WebMessageEncodingBindingElement>();
			if(webEncoder == null)
			{
				throw new InvalidOperationException("This behavior must be used in an endpoint with the WebHttpBinding (or a custom binding with the WebMessageEncodingBindingElement).");
			}
			foreach (OperationDescription operation in endpoint.Contract.Operations)
			{
				this.ValidateOperation(operation);
			}
		}

		/// <summary>
		/// Valida a operação informada.
		/// </summary>
		/// <param name="operation"></param>
		private void ValidateOperation(OperationDescription operation)
		{
			if(operation.Messages.Count > 1)
			{
				if(operation.Messages[1].Body.Parts.Count > 0)
				{
					throw new InvalidOperationException("Operations cannot have out/ref parameters.");
				}
			}
			WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operation);
			int inputParameterCount = operation.Messages[0].Body.Parts.Count;
			if(!this.IsGetOperation(operation))
			{
				bool wrappedRequest = bodyStyle == WebMessageBodyStyle.Wrapped || bodyStyle == WebMessageBodyStyle.WrappedRequest;
				if(inputParameterCount == 1 && wrappedRequest)
				{
					throw new InvalidOperationException("Wrapped body style for single parameters not implemented in this behavior.");
				}
			}
			bool wrappedResponse = bodyStyle == WebMessageBodyStyle.Wrapped || bodyStyle == WebMessageBodyStyle.WrappedResponse;
			bool isVoidReturn = operation.Messages.Count == 1 || operation.Messages[1].Body.ReturnValue.Type == typeof(void);
			if(!isVoidReturn && wrappedResponse)
			{
				throw new InvalidOperationException("Wrapped response not implemented in this behavior.");
			}
		}

		/// <summary>
		/// Recupera a uri modelo.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		private string GetUriTemplate(OperationDescription operation)
		{
			WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
			if(wga != null)
			{
				return wga.UriTemplate;
			}
			WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
			if(wia != null)
			{
				return wia.UriTemplate;
			}
			return null;
		}

		/// <summary>
		/// Recupera o estilo do corpo.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		private WebMessageBodyStyle GetBodyStyle(OperationDescription operation)
		{
			WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
			if(wga != null)
			{
				return wga.BodyStyle;
			}
			WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
			if(wia != null)
			{
				return wia.BodyStyle;
			}
			return this.DefaultBodyStyle;
		}

		/// <summary>
		/// Verifica se é uma operação Get.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		private bool IsGetOperation(OperationDescription operation)
		{
			WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
			if(wga != null)
			{
				return true;
			}
			WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
			if(wia != null)
			{
				return wia.Method == "HEAD";
			}
			return false;
		}
	}
}
