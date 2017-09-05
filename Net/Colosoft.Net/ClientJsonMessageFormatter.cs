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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Colosoft.Net.Json.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ClientJsonMessageFormatter : MessageFormatter, IClientJsonMessageFormatter
	{
		private readonly Uri operationUri;

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientJsonMessageFormatter"/> class.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="serviceRegister">The service register.</param>
		protected ClientJsonMessageFormatter(OperationDescription operation, ServiceEndpoint endpoint, IServiceRegister serviceRegister) : base(new ServiceOperation(operation, operation.Messages[0].Action), serviceRegister)
		{
			string endpointAddress = endpoint.Address.Uri.ToString();
			if(!endpointAddress.EndsWith("/"))
				endpointAddress = endpointAddress + "/";
			this.operationUri = new Uri(endpointAddress + operation.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageVersion"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
		{
			byte[] body = this.EncodeParameters(parameters);
			Message requestMessage = Message.CreateMessage(messageVersion, this.Action, new BinaryRawBodyWriter(body));
			requestMessage.Headers.To = operationUri;
			requestMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
			HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
			reqProp.Headers[HttpRequestHeader.ContentType] = "application/json";
			requestMessage.Properties.Add(HttpRequestMessageProperty.Name, reqProp);
			return requestMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object DeserializeReply(Message message, object[] parameters)
		{
			object bodyFormatProperty;
			if(!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty))
				throw new InvalidOperationException("Incoming message cannot be null.");
			WebBodyFormatMessageProperty bodyMsg = bodyFormatProperty as WebBodyFormatMessageProperty;
			if(bodyMsg == null)
				throw new InvalidCastException("The type of body message must be WebBodyFormatMessageProperty.");
			if(bodyMsg.Format != WebContentFormat.Raw)
				throw new InvalidOperationException("The body message type must be equals to WebContentFormat.Raw.");
			XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
			bodyReader.ReadStartElement(BinaryRawBodyWriter.DefaultRootName);
			return this.DecodeReply(bodyReader.ReadContentAsBase64(), parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public abstract byte[] EncodeParameters(object[] parameters);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="body"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public abstract object DecodeReply(byte[] body, object[] parameters);
	}
}
