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

namespace Colosoft.Query
{
	/// <summary>
	/// Representa o nome de uma store procedure.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetStoredProcedureName")]
	public class StoredProcedureName : System.Xml.Serialization.IXmlSerializable, Colosoft.Serialization.ICompactSerializable, ICloneable
	{
		private static readonly StoredProcedureName _empty = new StoredProcedureName();

		private string _schema;

		private string _name;

		/// <summary>
		/// Nome da stored procedure.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Nome do esquema onde a stored procedure está inserida.
		/// </summary>
		public string Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				_schema = value;
			}
		}

		/// <summary>
		/// Representa uma storedprocedure vazia.
		/// </summary>
		public static StoredProcedureName Empty
		{
			get
			{
				return StoredProcedureName._empty;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public StoredProcedureName()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		public StoredProcedureName(string name, string schema = null)
		{
			_name = name;
			_schema = schema;
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected StoredProcedureName(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_name = info.GetString("Name");
			_schema = info.GetString("Schema");
		}

		/// <summary>
		/// Recupera os dados para a serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Name", _name);
			info.AddValue("Schema", _schema);
		}

		/// <summary>
		/// Executa um parse do texto informado para o nome da stored procedure.
		/// </summary>
		/// <param name="storedProcedureName"></param>
		/// <returns></returns>
		public static StoredProcedureName Parse(string storedProcedureName)
		{
			if(string.IsNullOrEmpty(storedProcedureName))
				return null;
			var index = storedProcedureName.IndexOf('.');
			if(index >= 0)
				return new StoredProcedureName(storedProcedureName.Substring(index + 1), storedProcedureName.Substring(0, index));
			return new StoredProcedureName(storedProcedureName);
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetStoredProcedureName(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("StoredProcedureName", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados do XML.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToAttribute("Name");
			_name = reader.ReadContentAsString();
			reader.MoveToAttribute("Schema");
			_schema = reader.ReadContentAsString();
			reader.MoveToElement();
			reader.Skip();
		}

		/// <summary>
		/// Serializa os dados no xml.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", _name);
			writer.WriteAttributeString("Schema", _schema);
		}

		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_name = reader.ReadString();
			_schema = reader.ReadString();
		}

		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_name);
			writer.Write(_schema);
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new StoredProcedureName(_name, _schema);
		}
	}
}
