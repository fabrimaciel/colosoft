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
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Colosoft.Net
{
	/// <summary>
	/// Representa um parametro do endereço de um serviço.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("MySchema")]
	public class ServiceAddressParameter : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Nome.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor.
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceAddressParameter()
		{
		}

		/// <summary>
		/// Cria um nova instancia definindos os valores.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public ServiceAddressParameter(string name, string value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Recupera a string que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}: {1}", this.Name, this.Value);
		}

		/// <summary>
		/// Método usado para recupera o esquema da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static XmlSchemaType MySchema(XmlSchemaSet xs)
		{
			var complexType = new XmlSchemaComplexType();
			complexType.Attributes.Add(new XmlSchemaAttribute {
				Name = "name",
				SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
				Use = XmlSchemaUse.Required
			});
			complexType.Attributes.Add(new XmlSchemaAttribute {
				Name = "value",
				SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
				Use = XmlSchemaUse.Required
			});
			return complexType;
		}

		/// <summary>
		/// Não usa;
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Recupera os dados do xml.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("name"))
			{
				Name = reader.ReadContentAsString();
				reader.MoveToElement();
			}
			if(reader.MoveToAttribute("value"))
			{
				Value = reader.ReadContentAsString();
				reader.MoveToElement();
			}
			reader.Skip();
		}

		/// <summary>
		/// Escreve os dados do parametro no xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("value", Value);
		}
	}
}
