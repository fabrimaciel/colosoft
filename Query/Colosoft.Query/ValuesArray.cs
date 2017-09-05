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
using Colosoft.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa um vetor de valores.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetValuesArraySchema")]
	public class ValuesArray : ConditionalTerm
	{
		private ConditionalTerm[] _values = new ConditionalTerm[0];

		/// <summary>
		/// Valores do vetor.
		/// </summary>
		public ConditionalTerm[] Values
		{
			get
			{
				return _values;
			}
			set
			{
				if(value != null && value.Any(f => f == null))
					throw new InvalidOperationException("ValuesArray cannot contains null values");
				_values = value ?? new ConditionalTerm[0];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ValuesArray()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ValuesArray(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			int count = info.GetInt32("C");
			_values = new ConditionalTerm[count];
			for(int i = 0; i < count; i++)
				_values[i] = (ConditionalTerm)info.GetValue("c" + i, Type.GetType(info.GetString("t" + i), true));
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetValuesArraySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("ValuesArray", Namespaces.Query);
		}

		/// <summary>
		/// Nome que qualifica o tipo.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("ValuesArray", Namespaces.Query);
			}
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("C", _values.Length);
			for(int i = 0; i < _values.Length; i++)
			{
				info.AddValue("t" + i, _values[i].GetType().FullName);
				info.AddValue("c" + i, _values[i]);
			}
		}

		/// <summary>
		/// Lê os dados serializados no xml.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.IsEmptyElement)
				reader.Skip();
			else
			{
				reader.ReadStartElement();
				var values = new List<ConditionalTerm>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					values.Add(GetConditionalTerm(reader));
				_values = values.ToArray();
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializa os dados no XML.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (ConditionalTerm i in _values.Where(f => f != null))
			{
				writer.WriteStartElement(i.QualifiedName.Name, i.QualifiedName.Namespace);
				var qname = i.QualifiedName;
				if(qname.Name != "ConditionalTerm")
				{
					var prefix = writer.LookupPrefix(qname.Namespace);
					if(string.IsNullOrEmpty(prefix))
						writer.WriteAttributeString("xmlns", qname.Namespace);
				}
				((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0})", string.Join(",", _values.Select(f => f.ToString()).ToArray()));
		}

		/// <summary>
		/// Clona um array de valores.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			var values = new ConditionalTerm[Values.Length];
			for(int i = 0; i < Values.Length; i++)
			{
				values[i] = (ConditionalTerm)Values[i].Clone();
			}
			return new ValuesArray {
				Values = values
			};
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_values.Length);
			foreach (ConditionalTerm i in _values)
			{
				if(i != null)
				{
					writer.Write(true);
					writer.Write(i.GetType().Name);
					((ICompactSerializable)i).Serialize(writer);
				}
				else
					writer.Write(false);
			}
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			int length = reader.ReadInt32();
			_values = new ConditionalTerm[length];
			for(int i = 0; i < length; i++)
			{
				if(reader.ReadBoolean())
					_values[i] = GetConditionalTerm(reader);
			}
		}
	}
}
