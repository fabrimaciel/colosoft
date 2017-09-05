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
	/// Representa o valor do parametro de referencia.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class ReferenceParameterValue : ISerializable, Colosoft.Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		private string _columnName;

		private object _value;

		/// <summary>
		/// Nome da coluna de referencia.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReferenceParameterValue()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="columnName">Instancia do parametro de referencia.</param>
		/// <param name="value">Valor do parametro.</param>
		public ReferenceParameterValue(string columnName, object value)
		{
			_columnName = columnName;
			_value = value;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ReferenceParameterValue(SerializationInfo info, StreamingContext context)
		{
			_columnName = info.GetString("ColumnName");
			var isNull = info.GetBoolean("IsNull");
			if(!isNull)
				_value = info.GetValue("Value", Type.GetType(info.GetString("Type")));
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ColumnName", _columnName);
			info.AddValue("IsNull", _value == null);
			if(_value != null)
			{
				info.AddValue("Type", _value.GetType().FullName);
				info.AddValue("Value", _value);
			}
		}

		/// <summary>
		/// Recupera o esquema Xml da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("ReferenceParameterValue", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("ColumnName"))
				_columnName = reader.ReadContentAsString();
			Type type = null;
			if(reader.MoveToAttribute("Type"))
			{
				var typeString = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(typeString))
					type = Type.GetType(typeString, true);
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				if(type == typeof(byte[]))
				{
					var content = (string)reader.ReadElementContentAs(typeof(string), null);
					_value = Convert.FromBase64String(content);
				}
				else
					_value = reader.ReadElementContentAs(type, null);
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("ColumnName", ColumnName);
			if(_value != null)
			{
				object value = null;
				var valueType = _value.GetType();
				if(valueType.IsEnum)
				{
					valueType = Enum.GetUnderlyingType(valueType);
					value = Convert.ChangeType(_value, valueType);
				}
				else
					value = _value;
				writer.WriteAttributeString("Type", valueType.FullName);
				if(valueType == typeof(byte[]))
					writer.WriteValue(Convert.ToBase64String((byte[])value));
				else
					writer.WriteValue(value);
			}
			else
				writer.WriteAttributeString("Type", "");
		}

		/// <summary>
		/// Deserializa usando o CompactSerializer.
		/// </summary>
		/// <param name="reader">Representa o compact reader.</param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_columnName = reader.ReadString();
			if(reader.ReadByte() == 1)
				_value = reader.ReadObject();
		}

		/// <summary>
		/// Serializa usando o CompactSerializer.
		/// </summary>
		/// <param name="writer">Representa o compact writer.</param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_columnName);
			if(_value != null)
			{
				writer.Write((byte)1);
				var valueType = _value.GetType();
				object value;
				if(valueType.IsEnum)
				{
					valueType = Enum.GetUnderlyingType(valueType);
					value = Convert.ChangeType(_value, valueType);
				}
				else
				{
					value = _value;
				}
				writer.WriteObject(value);
			}
			else
				writer.Write((byte)0);
		}
	}
	/// <summary>
	/// Coleção dos valores de referencia.
	/// </summary>
	[Serializable]
	public sealed class ReferenceParameterValueCollection : List<ReferenceParameterValue>, ISerializable, Colosoft.Serialization.ICompactSerializable
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReferenceParameterValueCollection() : base()
		{
		}

		/// <summary>
		/// Cria uma nova instancia a partir de uma enumeração existente.
		/// </summary>
		/// <param name="collection"></param>
		public ReferenceParameterValueCollection(IEnumerable<ReferenceParameterValue> collection) : base(collection)
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo a sua capacidade inicial.
		/// </summary>
		/// <param name="capacity"></param>
		public ReferenceParameterValueCollection(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ReferenceParameterValueCollection(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("C");
			for(var i = 0; i < count; i++)
				this.Add((ReferenceParameterValue)info.GetValue(i.ToString(), typeof(ReferenceParameterValue)));
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("C", this.Count);
			for(var i = 0; i < Count; i++)
				info.AddValue(i.ToString(), this[i]);
		}

		/// <summary>
		/// Deserializa usando o CompactSerializer.
		/// </summary>
		/// <param name="reader">Representa o compact reader.</param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			var count = reader.ReadInt32();
			for(int i = 0; i < count; i++)
			{
				var parameter = new ReferenceParameterValue();
				((Colosoft.Serialization.ICompactSerializable)parameter).Deserialize(reader);
				Add(parameter);
			}
		}

		/// <summary>
		/// Serializa usando o CompactSerializer.
		/// </summary>
		/// <param name="writer">Representa o compact writer.</param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(this.Count);
			for(var i = 0; i < Count; i++)
				((Colosoft.Serialization.ICompactSerializable)this[i]).Serialize(writer);
		}
	}
}
