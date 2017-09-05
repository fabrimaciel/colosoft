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
using System.Text;
using System.Xml;

namespace Colosoft.Net.Json.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DispatchJsonMessageFormatter : MessageFormatter, IDispatchJsonMessageFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="serviceRegister"></param>
		protected DispatchJsonMessageFormatter(OperationDescription operation, IServiceRegister serviceRegister) : base(new ServiceOperation(operation, operation.Messages[1].Action), serviceRegister)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		public void DeserializeRequest(Message message, object[] parameters)
		{
			object bodyFormatProperty;
			if(!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty))
				throw new InvalidOperationException("Incoming message cannot be null.");
			WebBodyFormatMessageProperty bodyMsg = bodyFormatProperty as WebBodyFormatMessageProperty;
			if(bodyMsg == null)
				throw new InvalidCastException("The type of body message must be WebBodyFormatMessageProperty.");
			if(bodyMsg.Format != WebContentFormat.Raw)
				throw new InvalidOperationException("The body message type must be equals to WebContentFormat.Raw.");
			////
			XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
			bodyReader.ReadStartElement(BinaryRawBodyWriter.DefaultRootName);
			this.DecodeParameters(bodyReader.ReadContentAsBase64(), parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageVersion"></param>
		/// <param name="parameters"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
		{
			byte[] body = this.EncodeReply(parameters, result);
			Message replyMessage = Message.CreateMessage(messageVersion, this.Action, new BinaryRawBodyWriter(body));
			replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
			HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
			respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
			replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
			return replyMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="body"></param>
		/// <param name="parameters"></param>
		public abstract void DecodeParameters(byte[] body, object[] parameters);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public abstract byte[] EncodeReply(object[] parameters, object result);
	}
}
