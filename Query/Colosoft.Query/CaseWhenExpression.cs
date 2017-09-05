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
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa o segumento WHEN de um condicional CASE.
	/// </summary>
	[System.Xml.Serialization.XmlSchemaProvider("GetCaseWhenExpressionSchema")]
	public class CaseWhenExpression : System.Xml.Serialization.IXmlSerializable, ICloneable, Colosoft.Serialization.ICompactSerializable, ISerializable
	{
		private ConditionalTerm _expression;

		private ConditionalTerm _resultExpression;

		/// <summary>
		/// Expressão associada.
		/// </summary>
		public ConditionalTerm Expression
		{
			get
			{
				return _expression;
			}
		}

		/// <summary>
		/// Exoressão do resultado.
		/// </summary>
		public ConditionalTerm ResultExpression
		{
			get
			{
				return _resultExpression;
			}
		}

		/// <summary>
		/// Construtor interno.
		/// </summary>
		internal CaseWhenExpression()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="resultExpression"></param>
		public CaseWhenExpression(ConditionalTerm expression, ConditionalTerm resultExpression)
		{
			_expression = expression;
			_resultExpression = resultExpression;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CaseWhenExpression(SerializationInfo info, StreamingContext context)
		{
			var expressionType = info.GetString("ExpressionType");
			var resultExpressionType = info.GetString("ResultExpressionType");
			if(!string.IsNullOrEmpty(expressionType))
				_expression = (ConditionalTerm)info.GetValue("Expression", Type.GetType(expressionType, true));
			if(!string.IsNullOrEmpty(resultExpressionType))
				_resultExpression = (ConditionalTerm)info.GetValue("ResultExpression", Type.GetType(resultExpressionType, true));
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <returns></returns>
		public virtual System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("CaseWhenExpression", Namespaces.Query);
			}
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ExpressionType", _expression != null ? _expression.GetType().FullName : null);
			info.AddValue("Expression", _expression);
			info.AddValue("ResultExpressionType", _resultExpression != null ? _resultExpression.GetType().FullName : null);
			info.AddValue("ResultExpression", _resultExpression);
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetCaseWhenExpressionSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("CaseWhenExpression", Namespaces.Query);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_expression = ConditionalTerm.GetConditionalTerm(reader);
			else
				reader.Skip();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_resultExpression = ConditionalTerm.GetConditionalTerm(reader);
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Serializa os dados no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Expression", Namespaces.Query);
			if(_expression != null)
			{
				ConditionalTerm.InsertConditionalTermType(writer, _expression);
				((System.Xml.Serialization.IXmlSerializable)_expression).WriteXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("ResultExpression", Namespaces.Query);
			if(_resultExpression != null)
			{
				ConditionalTerm.InsertConditionalTermType(writer, _resultExpression);
				((System.Xml.Serialization.IXmlSerializable)_resultExpression).WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new CaseWhenExpression(_expression != null ? (ConditionalTerm)_expression.Clone() : null, _resultExpression != null ? (ConditionalTerm)_resultExpression.Clone() : null);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
				_expression = ConditionalTerm.GetConditionalTerm(reader);
			if(reader.ReadBoolean())
				_resultExpression = ConditionalTerm.GetConditionalTerm(reader);
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(_expression != null)
			{
				writer.Write(true);
				((Colosoft.Serialization.ICompactSerializable)_expression).Serialize(writer);
			}
			else
				writer.Write(false);
			if(_resultExpression != null)
			{
				writer.Write(true);
				((Colosoft.Serialization.ICompactSerializable)_resultExpression).Serialize(writer);
			}
			else
				writer.Write(false);
		}
	}
}
