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
using System.Text;
using System.Xml.Serialization;

namespace Colosoft.Business
{
	/// <summary>
	/// Armazena o resultado de uma operação de Delete.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class DeleteResult : IXmlSerializable
	{
		/// <summary>
		/// Identifica se a operação foi executada com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem do resultado.
		/// </summary>
		public IMessageFormattable Message
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DeleteResult()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success">Identifica se a operação foi realizada com sucesso.</param>
		/// <param name="message">Mensagem do resultado.</param>
		public DeleteResult(bool success, IMessageFormattable message)
		{
			this.Success = success;
			this.Message = message;
		}

		/// <summary>
		/// Converte implicitamente para um Boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool(DeleteResult value)
		{
			if(value == null)
				return false;
			return value.Success;
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			BusinessNamespace.ResolveRolePermissionSchema(xs);
			return new System.Xml.XmlQualifiedName("DeleteResult", BusinessNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê o xml.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Success"))
			{
				bool success;
				if(bool.TryParse(reader.ReadContentAsString(), out success))
					Success = success;
			}
			reader.MoveToElement();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				var text = reader.ReadElementString("Message");
				if(!text.Equals(string.Empty))
					Message = text.GetFormatter();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Escreve o xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Success", Success.ToString());
			writer.WriteStartElement("Message");
			writer.WriteValue(Message.FormatOrNull());
			writer.WriteEndElement();
		}
	}
}
