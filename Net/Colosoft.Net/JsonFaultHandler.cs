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
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Manipulador das falhas para o formato JSON.
	/// </summary>
	public class JsonFaultHandler : IErrorHandler
	{
		/// <summary>
		/// Trata o erro informado.
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public bool HandleError(Exception error)
		{
			return true;
		}

		/// <summary>
		/// Prove as informações da falha.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="version"></param>
		/// <param name="fault"></param>
		public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
		{
			var prop = new System.ServiceModel.Channels.HttpResponseMessageProperty();
			prop.Headers[System.Net.HttpResponseHeader.ContentType] = "application/json; charset=utf-8";
			fault = System.ServiceModel.Channels.Message.CreateMessage(version, null, new RawBodyWriter(error));
			fault.Properties.Add(System.ServiceModel.Channels.HttpResponseMessageProperty.Name, prop);
			fault.Properties.Add(System.ServiceModel.Channels.WebBodyFormatMessageProperty.Name, new System.ServiceModel.Channels.WebBodyFormatMessageProperty(System.ServiceModel.Channels.WebContentFormat.Json));
		}

		class RawBodyWriter : System.ServiceModel.Channels.BodyWriter
		{
			private Exception _exception;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="exception"></param>
			public RawBodyWriter(Exception exception) : base(true)
			{
				_exception = exception;
			}

			/// <summary>
			/// Método acionad quando for escreve o contúdo do corpo.
			/// </summary>
			/// <param name="writer"></param>
			protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
			{
				writer.WriteStartElement("root");
				writer.WriteAttributeString("type", "object");
				WriteException(writer, _exception);
				writer.WriteEndElement();
			}

			private void WriteException(System.Xml.XmlDictionaryWriter writer, Exception exception)
			{
				writer.WriteStartElement("type");
				writer.WriteAttributeString("type", "string");
				writer.WriteString(exception.GetType().Name);
				writer.WriteEndElement();
				writer.WriteStartElement("error");
				writer.WriteAttributeString("type", "string");
				writer.WriteString(exception.Message);
				writer.WriteEndElement();
				if(exception.InnerException != null)
				{
					writer.WriteStartElement("inner");
					writer.WriteAttributeString("type", "object");
					WriteException(writer, exception.InnerException);
					writer.WriteEndElement();
				}
			}
		}
	}
}
