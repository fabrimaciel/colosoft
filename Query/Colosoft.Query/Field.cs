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
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	public partial class Record
	{
		/// <summary>
		/// Representa os dados do campo do registro.
		/// </summary>
		[Serializable]
		[XmlSchemaProvider("GetMySchema")]
		public sealed class Field : ISerializable, IXmlSerializable, Colosoft.Serialization.ICompactSerializable
		{
			private string _encodedTableName;

			private Type _nullableType;

			/// <summary>
			/// Nome do campo.
			/// </summary>
			public string Name
			{
				get;
				internal set;
			}

			/// <summary>
			/// Tipo do campo.
			/// </summary>
			public Type Type
			{
				get;
				internal set;
			}

			/// <summary>
			/// Nome codificado.
			/// </summary>
			internal string EncodedName
			{
				get
				{
					string encodedTableName = _encodedTableName;
					if(encodedTableName == null)
					{
						encodedTableName = System.Xml.XmlConvert.EncodeLocalName(this.Name);
						_encodedTableName = encodedTableName;
					}
					return encodedTableName;
				}
			}

			/// <summary>
			/// Identifica o tipo implementa um INullable.
			/// </summary>
			public bool ImplementsINullable
			{
				get
				{
					return TypeHelper.IsNullableType(Type);
				}
			}

			/// <summary>
			/// Tipo com suporte para nulo.
			/// </summary>
			public Type NullableType
			{
				get
				{
					if(_nullableType == null)
						if(Type.IsValueType)
							_nullableType = typeof(Nullable<>).MakeGenericType(Type);
						else
							_nullableType = Type;
					return _nullableType;
				}
			}

			/// <summary>
			/// Construtor vazio.
			/// </summary>
			public Field()
			{
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="type"></param>
			public Field(string name, Type type)
			{
				name.Require("name").NotNull();
				type.Require("type").NotNull();
				this.Name = name;
				this.Type = type;
			}

			/// <summary>
			/// Construtor usado na deserialização dos dados.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			private Field(SerializationInfo info, StreamingContext context)
			{
				Name = info.GetString("Name");
				Type = Type.GetType(info.GetString("Type"), true);
			}

			/// <summary>
			/// Altera o tipo do campo.
			/// </summary>
			/// <param name="newType"></param>
			public void ChangeFieldType(Type newType)
			{
				this.Type = newType;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("{0} : {1}", Name, Type.FullName);
			}

			/// <summary>
			/// Recupera os dados da instancia.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("Name", Name, typeof(string));
				info.AddValue("Type", Type.FullName, typeof(string));
			}

			/// <summary>
			/// Recupera o esquema que representa o tipo.
			/// </summary>
			/// <param name="xs"></param>
			/// <returns></returns>
			public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
			{
				xs.ResolveQuerySchema();
				return new System.Xml.XmlQualifiedName("Field", Namespaces.Query);
			}

			System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
			{
				throw new NotImplementedException();
			}

			void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
			{
				reader.ReadStartElement();
				Name = reader.ReadElementString("Name", Namespaces.Query);
				Type = Type.GetType(reader.ReadElementString("Type", Namespaces.Query), true);
				reader.ReadEndElement();
			}

			void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
			{
				writer.WriteElementString("Name", Namespaces.Query, Name);
				writer.WriteElementString("Type", Namespaces.Query, Type.FullName);
			}

			void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
			{
				var name = reader.ReadString();
				var typeFullName = reader.ReadString();
				Name = name;
				Type = Type.GetType(typeFullName, true);
			}

			void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
			{
				writer.Write(Name);
				writer.Write(Type.FullName);
			}
		}
	}
}
