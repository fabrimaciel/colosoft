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

using Colosoft.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Data
{
	/// <summary>
	/// Representa a referencia para uma propriedade.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class PropertyReference : ISerializable, System.Xml.Serialization.IXmlSerializable, ICompactSerializable, ICloneable
	{
		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PropertyReference()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private PropertyReference(SerializationInfo info, StreamingContext context)
		{
			PropertyName = info.GetString("PropertyName");
		}

		/// <summary>
		/// Recupera os dados para a serialização da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("PropertyName", PropertyName);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSchema();
			return new System.Xml.XmlQualifiedName("PropertyReference", Namespaces.Data);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("PropertyName"))
				PropertyName = reader.ReadContentAsString();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("PropertyName", PropertyName);
		}

		/// <summary>
		/// Desserializa o parametro.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			PropertyName = reader.ReadString();
		}

		/// <summary>
		/// Serializa o parâmetro.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(PropertyName);
		}

		/// <summary>
		/// Clona um parâmetro.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new PropertyReference {
				PropertyName = PropertyName
			};
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Property: {0}]", PropertyName);
		}
	}
}
