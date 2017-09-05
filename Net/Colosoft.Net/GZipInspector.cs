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
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação do inspetor da mensagens para o GZip.
	/// </summary>
	public class GZipInspector : IDispatchMessageInspector, IClientMessageInspector
	{
		/// <summary>
		/// Método acionado depois de receber o requisição.
		/// </summary>
		/// <param name="request">Mensagem de requisição.</param>
		/// <param name="channel">Canal.</param>
		/// <param name="instanceContext">Contexto da instancia.</param>
		/// <returns></returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
		{
			try
			{
				var prop = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
				var accept = prop.Headers[HttpRequestHeader.AcceptEncoding];
				var contentType = prop.Headers[HttpRequestHeader.ContentType];
				if(!string.IsNullOrEmpty(accept) && accept.Contains("gzip"))
					OperationContext.Current.Extensions.Add(new DoCompressExtension());
				if(!string.IsNullOrEmpty(contentType) && contentType.Contains("+gzip"))
					OperationContext.Current.Extensions.Add(new DoCompressPlusExtension());
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Método acionado antes de enviar a resposta.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if(OperationContext.Current.Extensions.OfType<DoCompressExtension>().Any() && !OperationContext.Current.Extensions.OfType<DoCompressPlusExtension>().Any())
			{
				HttpResponseMessageProperty httpResponseProperty = new HttpResponseMessageProperty();
				httpResponseProperty.Headers.Add(HttpResponseHeader.ContentEncoding, "gzip");
				reply.Properties[HttpResponseMessageProperty.Name] = httpResponseProperty;
			}
		}

		/// <summary>
		/// Processa a respota recebida.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if(reply.IsFault)
			{
				var buffer = reply.CreateBufferedCopy(Int32.MaxValue);
				var copy = buffer.CreateMessage();
				reply = buffer.CreateMessage();
				var messageFault = MessageFault.CreateFault(copy, 0x10000);
				throw FaultException.CreateFault(messageFault);
			}
		}

		/// <summary>
		/// Processa a requisição antes de enviar.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
		{
			object httpRequestMessageObject;
			if(request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
			{
				var httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
				httpRequestMessage.Headers[System.Net.HttpRequestHeader.ContentEncoding] = "gzip";
			}
			else
			{
				var httpRequestMessage = new HttpRequestMessageProperty();
				httpRequestMessage.Headers.Add(System.Net.HttpRequestHeader.ContentEncoding, "gzip");
				request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
			}
			return null;
		}
	}
}
