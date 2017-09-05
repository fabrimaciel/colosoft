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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Colosoft.Query;
using System.Data;
using Colosoft.Serialization;

namespace Colosoft.Data
{
	/// <summary>
	/// Representa um parametro que será processado em 
	/// alguma ação de persistencia.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class PersistenceParameter : ISerializable, System.Xml.Serialization.IXmlSerializable, ICompactSerializable, ICloneable
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
		/// Valor do parametro.
		/// </summary>
		public object Value
		{
			get;
			set;
		}

		/// <summary>
		/// Direção do parâmetro.
		/// </summary>
		public Colosoft.Query.ParameterDirection Direction
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo de banco de dados do parâmetro.
		/// </summary>
		public DbType DbType
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho do parâmetro.
		/// </summary>
		public int Size
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PersistenceParameter() : this(Colosoft.Query.ParameterDirection.Input)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="direction">Direção do parâmetro.</param>
		public PersistenceParameter(Colosoft.Query.ParameterDirection direction)
		{
			Direction = direction;
		}

		/// <summary>
		/// Cria uma nova instancia já definindo seus valores.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="direction">Direção do parâmetro.</param>
		public PersistenceParameter(string name, object value, Colosoft.Query.ParameterDirection direction = Colosoft.Query.ParameterDirection.Input)
		{
			Name = name;
			Value = value;
			Direction = direction;
		}

		/// <summary>
		/// Cria uma nova instancia já definindo seus valores.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="size">Tamanho do parâmtro.</param>
		/// <param name="direction">Direção do parâmetro.</param>
		public PersistenceParameter(string name, object value, int size, Colosoft.Query.ParameterDirection direction = Colosoft.Query.ParameterDirection.Input)
		{
			Name = name;
			Value = value;
			Direction = direction;
			Size = size;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private PersistenceParameter(SerializationInfo info, StreamingContext context)
		{
			Name = info.GetString("Name");
			Direction = (Colosoft.Query.ParameterDirection)info.GetInt32("Direction");
			var valueType = info.GetString("Type");
			if(!string.IsNullOrEmpty(valueType))
				Value = info.GetValue("Value", Type.GetType(valueType, true));
		}

		/// <summary>
		/// Recupera os dados para a serialização da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Direction", (int)Direction);
			info.AddValue("Type", Value == null ? null : Value.GetType().FullName);
			if(Value != null)
				info.AddValue("Value", Value);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSchema();
			return new System.Xml.XmlQualifiedName("PersistenceParameter", Namespaces.Data);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Direction"))
				Direction = (Colosoft.Query.ParameterDirection)reader.ReadContentAsInt();
			if(reader.MoveToAttribute("Size"))
				Size = reader.ReadContentAsInt();
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
					Value = Convert.FromBase64String(content);
				}
				else if(type != null)
					Value = reader.ReadElementContentAs(type, null);
				else
					Value = null;
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Direction", ((int)Direction).ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Size", Size.ToString(CultureInfo.InvariantCulture));
			if(Value != null)
			{
				object value;
				var valueType = Value.GetType();
				if(valueType.IsEnum)
				{
					valueType = Enum.GetUnderlyingType(valueType);
					value = Convert.ChangeType(Value, valueType);
				}
				else
					value = Value;
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
		/// Desserializa o parametro.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			Name = reader.ReadString();
			Direction = (Colosoft.Query.ParameterDirection)reader.ReadInt32();
			Size = reader.ReadInt32();
			this.DbType = (System.Data.DbType)reader.ReadInt32();
			if(reader.ReadBoolean())
				Value = reader.ReadObject();
		}

		/// <summary>
		/// Serializa o parâmetro.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(Name);
			writer.Write((int)Direction);
			writer.Write(Size);
			writer.Write((int)DbType);
			if(Value != null)
			{
				var valueType = Value.GetType();
				object value;
				if(valueType.IsEnum)
				{
					valueType = Enum.GetUnderlyingType(valueType);
					value = Convert.ChangeType(Value, valueType);
				}
				else
				{
					value = Value;
				}
				writer.Write(true);
				writer.WriteObject(value);
			}
			else
			{
				writer.Write(false);
			}
		}

		/// <summary>
		/// Clona um parâmetro.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new QueryParameter(Name, Value, Direction);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Name: {0}, Value: {1}]", this.Name, this.Value);
		}
	}
}
