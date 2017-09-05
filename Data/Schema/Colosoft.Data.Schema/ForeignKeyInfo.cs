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

namespace Colosoft.Data.Schema.Local
{
	/// <summary>
	/// Armazena as informações da chave estrangeira.
	/// </summary>
	public class ForeignKeyInfo : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Nome do tipo associado.
		/// </summary>
		public string TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Namespace do tipo associado.
		/// </summary>
		public string Namespace
		{
			get;
			set;
		}

		/// <summary>
		/// Assembly do tipo associado.
		/// </summary>
		public string Assembly
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da propriedade de ligação.
		/// </summary>
		public string Property
		{
			get;
			set;
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			TypeName = reader.GetAttribute("typeName");
			Namespace = reader.GetAttribute("namespace");
			Assembly = reader.GetAttribute("assembly");
			Property = reader.GetAttribute("property");
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("typeName", TypeName);
			writer.WriteAttributeString("namespace", Namespace);
			writer.WriteAttributeString("assembly", Assembly);
			writer.WriteAttributeString("property", Property);
		}
	}
}
