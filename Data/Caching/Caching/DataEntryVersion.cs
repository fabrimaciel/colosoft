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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Armazena os dados da versão de uma entrada de dados do cache.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class DataEntryVersion : System.Runtime.Serialization.ISerializable, Colosoft.Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		private Colosoft.Reflection.TypeName _typeName;

		private DateTime _version;

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public Colosoft.Reflection.TypeName TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				_typeName = value;
			}
		}

		/// <summary>
		/// Versão da entrada.
		/// </summary>
		public DateTime Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DataEntryVersion()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DataEntryVersion(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_typeName = info.GetValue("typeName", typeof(Colosoft.Reflection.TypeName)) as Colosoft.Reflection.TypeName;
			_version = info.GetDateTime("version");
		}

		void Serialization.ICompactSerializable.Deserialize(Serialization.IO.CompactReader reader)
		{
			if(reader.ReadByte() == 1)
			{
				_typeName = new Reflection.TypeName();
				_typeName.Deserialize(reader);
			}
			else
				_typeName = null;
			_version = reader.ReadDateTime();
		}

		void Serialization.ICompactSerializable.Serialize(Serialization.IO.CompactWriter writer)
		{
			if(_typeName == null)
				writer.Write((byte)0);
			else
			{
				writer.Write((byte)1);
				_typeName.Serialize(writer);
			}
			writer.Write(_version);
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("typeName", _typeName);
			info.AddValue("version", _version);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			Namespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("DataEntryVersion", Namespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Type"))
				_typeName = new Colosoft.Reflection.TypeName(reader.ReadContentAsString());
			if(reader.MoveToAttribute("Version"))
			{
				var v = reader.ReadContentAsString();
				_version = DateTime.Parse(v, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
			}
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Type", TypeName != null ? TypeName.AssemblyQualifiedName : "");
			writer.WriteAttributeString("Version", Version.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));
		}

		/// <summary>
		/// Classe com método que auxiliam no namespace do DataEntryVersion.
		/// </summary>
		public static class Namespace
		{
			/// <summary>
			/// Namespace definido para o XML
			/// </summary>
			public const string Data = "http://Colosoft.com.br/2012/webservices/data/caching";

			/// <summary>
			/// Namespace do arquivo de schema
			/// </summary>
			public const string SchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

			private static System.Xml.Schema.XmlSchema _schema;

			/// <summary>
			/// Instancia do esquema da consulta.
			/// </summary>
			public static System.Xml.Schema.XmlSchema Schema
			{
				get
				{
					if(_schema == null)
					{
						var path = "Colosoft.Data.Caching.Xsd.DataCaching.xsd";
						System.Xml.Schema.XmlSchema schema = null;
						var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));
						using (var stream = typeof(Namespace).Assembly.GetManifestResourceStream(path))
						{
							if(stream == null)
								return null;
							schema = (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(new System.Xml.XmlTextReader(stream), null);
							_schema = schema;
						}
					}
					return _schema;
				}
			}

			/// <summary>
			/// Resolve o esquema da consulta.
			/// </summary>
			/// <param name="xs"></param>
			public static void ResolveSchema(System.Xml.Schema.XmlSchemaSet xs)
			{
				var querySchema = Schema;
				if(!xs.Contains(querySchema))
				{
					xs.XmlResolver = new System.Xml.XmlUrlResolver();
					xs.Add(querySchema);
				}
			}
		}
	}
}
