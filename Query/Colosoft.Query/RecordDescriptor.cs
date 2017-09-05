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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Colosoft.Query
{
	public partial class Record
	{
		/// <summary>
		/// Descrição do registro.
		/// </summary>        
		[Serializable]
		[XmlSchemaProvider("GetSchema")]
		public sealed class RecordDescriptor : ISerializable, IEnumerable<Field>, IXmlSerializable, Colosoft.Serialization.ICompactSerializable
		{
			private string _name;

			private System.Globalization.CultureInfo _locale;

			private string _namespace;

			private List<Field> _fields = new List<Field>();

			private bool _cultureUserSet = false;

			private string _encodedTableName;

			/// <summary>
			/// Nome do descritor.
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
			/// Nome codificado.
			/// </summary>
			internal string EncodedName
			{
				get
				{
					string encodedTableName = this._encodedTableName;
					if(encodedTableName == null)
					{
						encodedTableName = System.Xml.XmlConvert.EncodeLocalName(this._name);
						this._encodedTableName = encodedTableName;
					}
					return encodedTableName;
				}
			}

			/// <summary>
			/// Dados de localização do descritor.
			/// </summary>
			public System.Globalization.CultureInfo Locale
			{
				get
				{
					return _locale;
				}
				set
				{
					_locale = value;
				}
			}

			/// <summary>
			/// Quantidades de campos do descritor.
			/// </summary>
			public int Count
			{
				get
				{
					return _fields.Count;
				}
			}

			/// <summary>
			/// Recupera o define o campo no indice informado.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Field this[int index]
			{
				get
				{
					return _fields[index];
				}
				set
				{
					_fields[index] = value;
				}
			}

			/// <summary>
			/// Construtor vazio.
			/// </summary>
			public RecordDescriptor()
			{
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name">Nome do descritor.</param>
			/// <param name="fields">Nome dos fields</param>
			public RecordDescriptor(string name, IEnumerable<Field> fields)
			{
				name.Require("name").NotNull().NotEmpty();
				fields.Require("fields").NotNull();
				_name = name;
				_fields.AddRange(fields);
			}

			/// <summary>
			/// Construtor usado na deserialização dos dados.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			private RecordDescriptor(SerializationInfo info, StreamingContext context)
			{
				var localeName = info.GetString("Locale");
				if(!string.IsNullOrEmpty(localeName))
					_locale = System.Globalization.CultureInfo.GetCultureInfo(localeName);
				_name = info.GetString("Name");
				_cultureUserSet = info.GetBoolean("CultureUserSet");
				_namespace = info.GetString("Namespace");
				var count = info.GetInt32("c");
				for(var i = 0; i < count; i++)
					_fields.Add(info.GetValue(i.ToString(), typeof(Field)) as Field);
			}

			/// <summary>
			/// Verifica se existe o campo com o nome informado.
			/// </summary>
			/// <param name="fieldName"></param>
			/// <returns></returns>
			public bool Contains(string fieldName)
			{
				return _fields.Any(f => Record.FieldNameComparer.Equals(f.Name, fieldName));
			}

			/// <summary>
			/// Recupera a posição do campo com o nome informado.
			/// </summary>
			/// <param name="name"></param>
			/// <returns>Posição do campo no descritor ou -1 caso não encontre.</returns>
			public int GetFieldPosition(string name)
			{
				return _fields.FindIndex(f => Record.FieldNameComparer.Equals(f.Name, name));
			}

			/// <summary>
			/// Adiciona um campo para o descritor.
			/// </summary>
			/// <param name="field">Campo a ser adicionado</param>
			public void Add(Field field)
			{
				field.Require("field").NotNull();
				_fields.Add(field);
			}

			/// <summary>
			/// Adiciona vários campos ao descritor.
			/// </summary>
			/// <param name="fields">Campos a serem adicionados.</param>
			public void AddRange(IEnumerable<Field> fields)
			{
				fields.Require("fields").NotNull();
				_fields.AddRange(fields);
			}

			/// <summary>
			/// Cria um registro apartir do descritor.
			/// </summary>
			/// <returns></returns>
			public Record CreateRecord(object[] values)
			{
				return new Record(this) {
					_values = values
				};
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("[{0}]", string.Join("; ", _fields.Select(f => f.Name).ToArray()));
			}

			/// <summary>
			/// Recupera os dados do objeto.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("Name", _name);
				info.AddValue("CultureUserSet", _cultureUserSet);
				info.AddValue("Locale", _locale == null ? null : _locale.Name);
				info.AddValue("Namespace", _namespace);
				info.AddValue("c", Count);
				for(var i = 0; i < _fields.Count; i++)
					info.AddValue(i.ToString(), _fields[i]);
			}

			/// <summary>
			/// Recupera o enumerador dos campos da descritor.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Field> GetEnumerator()
			{
				return _fields.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _fields.GetEnumerator();
			}

			private static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
			{
				xs.ResolveQuerySchema();
				return new System.Xml.XmlQualifiedName("RecordDescriptor", Namespaces.Query);
			}

			System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Recupera os dados serializados no XML.
			/// </summary>
			/// <param name="reader"></param>
			void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
			{
				string locale = null;
				if(reader.MoveToAttribute("Name"))
					Name = reader.ReadContentAsString();
				if(reader.MoveToAttribute("Locale"))
					locale = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(locale))
					_locale = System.Globalization.CultureInfo.GetCultureInfo(locale);
				reader.MoveToElement();
				if(!reader.IsEmptyElement)
				{
					reader.ReadStartElement();
					reader.ReadStartElement("Fields", Namespaces.Query);
					while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					{
						if(reader.LocalName == "Field")
						{
							var field = new Field();
							((System.Xml.Serialization.IXmlSerializable)field).ReadXml(reader);
							_fields.Add(field);
						}
						else
							reader.Skip();
					}
					reader.ReadEndElement();
					reader.ReadEndElement();
				}
				else
					reader.Skip();
			}

			/// <summary>
			/// Serializa os dados em XML.
			/// </summary>
			/// <param name="writer"></param>
			void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
			{
				writer.WriteAttributeString("Name", Name);
				writer.WriteAttributeString("Locale", Locale == null ? null : Locale.Name);
				writer.WriteStartElement("Fields", Namespaces.Query);
				foreach (IXmlSerializable field in _fields)
				{
					writer.WriteStartElement("Field", Namespaces.Query);
					field.WriteXml(writer);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}

			/// <summary>
			/// Deserializa os dados para a instancia.
			/// </summary>
			/// <param name="reader"></param>
			void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
			{
				Name = reader.ReadString();
				var locale = reader.ReadString();
				if(!string.IsNullOrEmpty(locale))
					_locale = System.Globalization.CultureInfo.GetCultureInfo(locale);
				var count = reader.ReadInt32();
				_fields = new List<Field>(count);
				for(var i = 0; i < count; i++)
				{
					var field = new Field();
					((Colosoft.Serialization.ICompactSerializable)field).Deserialize(reader);
					_fields.Add(field);
				}
			}

			/// <summary>
			/// Serializa os dados da isntancia.
			/// </summary>
			/// <param name="writer"></param>
			void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
			{
				writer.Write(Name);
				writer.Write(Locale == null ? null : Locale.Name);
				writer.Write(_fields.Count);
				foreach (Colosoft.Serialization.ICompactSerializable i in _fields)
					i.Serialize(writer);
			}
		}
	}
}
