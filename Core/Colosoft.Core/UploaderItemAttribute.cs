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

namespace Colosoft.Net
{
	/// <summary>
	/// Representa o atributo de um item do uploader.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class UploaderItemAttribute : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Nome do atributo.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor do atributo.
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do atributo.</param>
		/// <param name="value">Valor do atributo.</param>
		public UploaderItemAttribute(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public UploaderItemAttribute()
		{
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} : {1}", Name, Value);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			NetNamespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("UploaderItemAttribute", NetNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema de serialização.
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
			Name = reader.GetAttribute("Name");
			reader.MoveToElement();
			Value = reader.ReadElementContentAsString();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteValue(Value);
		}
	}
}
