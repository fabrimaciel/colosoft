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
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementar fo Dispatch formatter para json.
	/// </summary>
	public abstract class JsonDispatchFormatter : IDispatchMessageFormatter
	{
		private OperationDescription _operation;

		private Dictionary<string, int> _parameterNames;

		/// <summary>
		/// Operação associada.
		/// </summary>
		protected OperationDescription Operation
		{
			get
			{
				return _operation;
			}
		}

		/// <summary>
		/// Nomes dos parametros.
		/// </summary>
		protected Dictionary<string, int> ParameterNames
		{
			get
			{
				return _parameterNames;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="isRequest"></param>
		public JsonDispatchFormatter(OperationDescription operation, bool isRequest)
		{
			_operation = operation;
			if(isRequest)
			{
				int operationParameterCount = operation.Messages[0].Body.Parts.Count;
				if(operationParameterCount > 1)
				{
					_parameterNames = new Dictionary<string, int>();
					for(int i = 0; i < operationParameterCount; i++)
						this._parameterNames.Add(operation.Messages[0].Body.Parts[i].Name, i);
				}
			}
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="reader"></param>
		/// <param name="parameters"></param>
		protected abstract void Deserialize(Message message, TextReader reader, object[] parameters);

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="messageVersion"></param>
		/// <param name="writer"></param>
		/// <param name="parameters"></param>
		protected abstract void Serialize(MessageVersion messageVersion, TextWriter writer, object[] parameters, object result);

		public void DeserializeRequest(Message message, object[] parameters)
		{
			object bodyFormatProperty;
			if(!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) || (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
			{
				throw new InvalidOperationException("Incoming messages must have a body format of Raw. Is a ContentTypeMapper set on the WebHttpBinding?");
			}
			XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
			bodyReader.ReadStartElement("Binary");
			byte[] rawBody = bodyReader.ReadContentAsBase64();
			MemoryStream ms = new MemoryStream(rawBody);
			StreamReader sr = new StreamReader(ms);
			Deserialize(message, sr, parameters);
			sr.Close();
			ms.Close();
		}

		public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
		{
			byte[] body;
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
					Serialize(messageVersion, sw, parameters, result);
				body = ms.ToArray();
			}
			Message replyMessage = Message.CreateMessage(messageVersion, _operation.Messages[1].Action, new RawBodyWriter(body));
			replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
			HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
			respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
			replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
			return replyMessage;
		}

		class RawBodyWriter : BodyWriter
		{
			byte[] content;

			public RawBodyWriter(byte[] content) : base(true)
			{
				this.content = content;
			}

			protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			{
				writer.WriteStartElement("Binary");
				writer.WriteBase64(content, 0, content.Length);
				writer.WriteEndElement();
			}
		}
	}
}
