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
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena os dados de um parametro de referencia para 
	/// uma consulta pai.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class ReferenceParameter : ISerializable, Colosoft.Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string ColumnName
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReferenceParameter()
		{
		}

		/// <summary>
		/// Cria a instancia com o nome da coluna na qual referencia.
		/// </summary>
		/// <param name="columnName"></param>
		public ReferenceParameter(string columnName)
		{
			this.ColumnName = columnName;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ReferenceParameter(SerializationInfo info, StreamingContext context)
		{
			this.ColumnName = info.GetString("ColumnName");
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ColumnName", ColumnName);
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="reader"></param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			this.ColumnName = reader.ReadString();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(ColumnName);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("ReferenceParameter", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("ColumnName"))
				this.ColumnName = reader.ReadContentAsString();
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("FullName", this.ColumnName);
		}
	}
}
