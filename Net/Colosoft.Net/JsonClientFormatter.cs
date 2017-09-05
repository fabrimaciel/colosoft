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
	/// Implementação do formatador de mensagem para o client json.
	/// </summary>
	public abstract class JsonClientFormatter : IClientMessageFormatter
	{
		private OperationDescription _operation;

		private Uri _operationUri;

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
		/// Uri da operação.
		/// </summary>
		protected Uri OperationUri
		{
			get
			{
				return _operationUri;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="endpoint"></param>
		public JsonClientFormatter(OperationDescription operation, ServiceEndpoint endpoint)
		{
			_operation = operation;
			var endpointAddress = endpoint.Address.Uri.ToString();
			if(!endpointAddress.EndsWith("/"))
				endpointAddress = endpointAddress + "/";
			_operationUri = new Uri(endpointAddress + operation.Name);
		}

		/// <summary>
		/// Deserializa os dados
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="objectType"></param>
		/// <returns></returns>
		protected abstract object Deserialize(TextReader reader, Type objectType);

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="messageVersion"></param>
		/// <param name="writer"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected abstract void Serialize(MessageVersion messageVersion, TextWriter writer, object[] parameters);

		/// <summary>
		/// Deserializa a resposta.
		/// </summary>
		/// <param name="message">Mensagem.</param>
		/// <param name="parameters">Parametros.</param>
		/// <returns></returns>
		public object DeserializeReply(Message message, object[] parameters)
		{
			object bodyFormatProperty;
			if(!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) || (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
			{
				throw new InvalidOperationException("Incoming messages must have a body format of Raw. Is a ContentTypeMapper set on the WebHttpBinding?");
			}
			XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
			bodyReader.ReadStartElement("Binary");
			byte[] body = bodyReader.ReadContentAsBase64();
			using (MemoryStream ms = new MemoryStream(body))
			{
				using (StreamReader sr = new StreamReader(ms))
				{
					Type returnType = this._operation.Messages[1].Body.ReturnValue.Type;
					return Deserialize(sr, returnType);
				}
			}
		}

		/// <summary>
		/// Serializa a requisição.
		/// </summary>
		/// <param name="messageVersion"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
		{
			byte[] body;
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
				{
					Serialize(messageVersion, sw, parameters);
				}
				body = ms.ToArray();
			}
			Message requestMessage = Message.CreateMessage(messageVersion, _operation.Messages[0].Action, new RawBodyWriter(body));
			requestMessage.Headers.To = _operationUri;
			requestMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
			HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
			reqProp.Headers[HttpRequestHeader.ContentType] = "application/json";
			requestMessage.Properties.Add(HttpRequestMessageProperty.Name, reqProp);
			return requestMessage;
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
