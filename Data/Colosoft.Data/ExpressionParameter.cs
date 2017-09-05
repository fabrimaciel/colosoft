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
	/// Representa um parametro que é uma expressão. Um instancia dessa
	/// classe pode ser informado como valor de uma parametro 
	/// para codificar uma expressão.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class ExpressionParameter : ISerializable, System.Xml.Serialization.IXmlSerializable, ICompactSerializable, ICloneable
	{
		private string _expression;

		private PersistenceParameterCollection _parameters = new PersistenceParameterCollection();

		/// <summary>
		/// Expressão.
		/// </summary>
		public string Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				_expression = value;
			}
		}

		/// <summary>
		/// Parametros da expressão.
		/// </summary>
		public PersistenceParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ExpressionParameter()
		{
		}

		/// <summary>
		/// Cria a instancia com a expressão.
		/// </summary>
		/// <param name="expression"></param>
		public ExpressionParameter(string expression)
		{
			_expression = expression;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ExpressionParameter(SerializationInfo info, StreamingContext context)
		{
			Expression = info.GetString("Expression");
			_parameters = (PersistenceParameterCollection)info.GetValue("Parameters", typeof(PersistenceParameterCollection));
		}

		/// <summary>
		/// Recupera os dados para a serialização da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Expression", Expression);
			info.AddValue("Parameters", Parameters);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSchema();
			Colosoft.Query.ConditionalContainer.GetConditionalContainerSchema(xs);
			return new System.Xml.XmlQualifiedName("ExpressionParameter", Namespaces.Data);
		}

		/// <summary>
		/// Recupera o esquema da ação.
		/// </summary>
		/// <returns></returns>
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
			reader.MoveToElement();
			reader.ReadStartElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Parameters", Namespaces.Data);
				var parameters = new List<PersistenceParameter>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Parameter")
					{
						var parameter = new PersistenceParameter();
						((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
						parameters.Add(parameter);
					}
					else
						reader.Skip();
				}
				_parameters = new PersistenceParameterCollection(parameters);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Expression");
				_expression = reader.ReadElementContentAs(typeof(string), null) as string;
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Escreve os dados para XML.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteAttributeString("xmlns", "q", null, Namespaces.Query);
			writer.WriteStartElement("Parameters", Namespaces.Data);
			if(Parameters != null)
				foreach (System.Xml.Serialization.IXmlSerializable parameter in Parameters)
				{
					writer.WriteStartElement("Parameter", Namespaces.Data);
					parameter.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			writer.WriteStartElement("Expression");
			writer.WriteValue(_expression);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			_expression = reader.ReadString();
			var parameters = new List<PersistenceParameter>();
			var count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var parameter = new PersistenceParameter();
				((ICompactSerializable)parameter).Deserialize(reader);
				parameters.Add(parameter);
			}
			_parameters = new PersistenceParameterCollection(parameters);
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(_expression);
			if(Parameters != null)
			{
				writer.Write(Parameters.Count);
				foreach (ICompactSerializable parameter in Parameters)
					parameter.Serialize(writer);
			}
			else
				writer.Write(0);
		}

		/// <summary>
		/// Clona um parâmetro.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var parameter = new ExpressionParameter(_expression);
			parameter._parameters = (PersistenceParameterCollection)_parameters.Clone();
			return parameter;
		}

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		public ExpressionParameter Add(string parameterName, object value)
		{
			Parameters.Add(parameterName, value);
			return this;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Expression: {0}]", Expression);
		}
	}
}
