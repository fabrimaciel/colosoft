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

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Parametro do tipo de validação.
	/// </summary>
	public class ValidationTypeParameter : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor padrão.
		/// </summary>
		public string DefaultValue
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do parametro.
		/// </summary>
		public Reflection.TypeName Type
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			Name = reader.GetAttribute("name");
			var type = reader.GetAttribute("type");
			if(!string.IsNullOrEmpty(type))
				Type = new Reflection.TypeName(type);
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					switch(reader.LocalName)
					{
					case "DefaultValue":
						DefaultValue = reader.ReadElementContentAsString();
						break;
					default:
						reader.Skip();
						break;
					}
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("type", Type.AssemblyQualifiedName);
			writer.WriteElementString("DefaultValue", DefaultValue);
		}
	}
}
