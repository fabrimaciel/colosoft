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
	/// Representa uma condicional CASE.
	/// </summary>
	public class CaseConditional : ConditionalTerm
	{
		private ConditionalTerm _inputExpression;

		private List<CaseWhenExpression> _whenExpressions;

		private ConditionalTerm _elseResultExpression;

		/// <summary>
		/// Expressão de entrada.
		/// </summary>
		public ConditionalTerm InputExpression
		{
			get
			{
				return _inputExpression;
			}
			set
			{
				_inputExpression = value;
			}
		}

		/// <summary>
		/// Expressões WHEN.
		/// </summary>
		public IList<CaseWhenExpression> WhenExpressions
		{
			get
			{
				return _whenExpressions;
			}
		}

		/// <summary>
		/// Expressão do resutlado do ELSE.
		/// </summary>
		public ConditionalTerm ElseResultExpression
		{
			get
			{
				return _elseResultExpression;
			}
			set
			{
				_elseResultExpression = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CaseConditional()
		{
			_whenExpressions = new List<CaseWhenExpression>();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="inputExpression"></param>
		/// <param name="whenExpressions"></param>
		/// <param name="elseResultExpression"></param>
		public CaseConditional(ConditionalTerm inputExpression, IEnumerable<CaseWhenExpression> whenExpressions, ConditionalTerm elseResultExpression)
		{
			_inputExpression = inputExpression;
			_whenExpressions = whenExpressions == null ? new List<CaseWhenExpression>() : new List<CaseWhenExpression>(whenExpressions);
			_elseResultExpression = elseResultExpression;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CaseConditional(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			var inputExpressionType = info.GetString("InputExpressionType");
			var elseResultExpressionType = info.GetString("ElseResultExpressionType");
			if(!string.IsNullOrEmpty(inputExpressionType))
				_inputExpression = (ConditionalTerm)info.GetValue("InputExpression", Type.GetType(inputExpressionType, true));
			if(!string.IsNullOrEmpty(elseResultExpressionType))
				_elseResultExpression = (ConditionalTerm)info.GetValue("ElseResultExpression", Type.GetType(elseResultExpressionType, true));
			var count = info.GetInt32("WhenExpressionsCount");
			for(var i = 0; i < count; i++)
				_whenExpressions.Add((CaseWhenExpression)info.GetValue(string.Format("WhenExpression{0}", i), typeof(CaseWhenExpression)));
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("Case", Namespaces.Query);
			}
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetCaseSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Case", Namespaces.Query);
		}

		/// <summary>
		/// Escreve o xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("InputExpression", Namespaces.Query);
			if(_inputExpression != null)
			{
				InsertConditionalTermType(writer, _inputExpression);
				((System.Xml.Serialization.IXmlSerializable)_inputExpression).WriteXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("WhenExpressions", Namespaces.Query);
			foreach (var i in _whenExpressions)
			{
				writer.WriteStartElement(i.QualifiedName.Name, i.QualifiedName.Namespace);
				((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("ElseReturnExpression", Namespaces.Query);
			if(_elseResultExpression != null)
			{
				InsertConditionalTermType(writer, _elseResultExpression);
				((System.Xml.Serialization.IXmlSerializable)_elseResultExpression).WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Lê os dados serializados em XML.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_inputExpression = GetConditionalTerm(reader);
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "WhenExpressions")
			{
				reader.ReadStartElement("WhenExpressions", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var caseWhenExpression = new CaseWhenExpression();
					((System.Xml.Serialization.IXmlSerializable)caseWhenExpression).ReadXml(reader);
					_whenExpressions.Add(caseWhenExpression);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_elseResultExpression = GetConditionalTerm(reader);
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InputExpressionType", _inputExpression != null ? _inputExpression.GetType().FullName : null);
			info.AddValue("InputExpression", _inputExpression);
			info.AddValue("ElseResultExpressionType", _elseResultExpression != null ? _elseResultExpression.GetType().FullName : null);
			info.AddValue("ElseResultExpression", _elseResultExpression);
			info.AddValue("WhenExpressionsCount", _whenExpressions.Count);
			for(var i = 0; i < _whenExpressions.Count; i++)
				info.AddValue(string.Format("WhenExpression{0}", i), _whenExpressions[i]);
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(_inputExpression != null)
			{
				writer.Write(true);
				((Colosoft.Serialization.ICompactSerializable)_inputExpression).Serialize(writer);
			}
			else
				writer.Write(false);
			foreach (var i in _whenExpressions)
			{
				writer.Write(true);
				((Colosoft.Serialization.ICompactSerializable)i).Serialize(writer);
			}
			writer.Write(false);
			if(_elseResultExpression != null)
			{
				writer.Write(true);
				((Colosoft.Serialization.ICompactSerializable)_elseResultExpression).Serialize(writer);
			}
			else
				writer.Write(false);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
				_inputExpression = GetConditionalTerm(reader);
			while (reader.ReadBoolean())
			{
				var caseWhenExpression = new CaseWhenExpression();
				((Colosoft.Serialization.ICompactSerializable)caseWhenExpression).Deserialize(reader);
				_whenExpressions.Add(caseWhenExpression);
			}
			if(reader.ReadBoolean())
				_elseResultExpression = GetConditionalTerm(reader);
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var text = new StringBuilder("CASE");
			if(InputExpression != null)
				text.Append(InputExpression);
			foreach (var i in WhenExpressions)
				text.Append(" WHEN ").Append(i.Expression).Append(" THEN ").Append(i.ResultExpression);
			if(ElseResultExpression != null)
				text.Append(" ELSE ").Append(ElseResultExpression);
			text.Append(" END");
			return text.ToString();
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns>Clone da instancia.</returns>
		public override object Clone()
		{
			return new CaseConditional(_inputExpression != null ? (ConditionalTerm)_inputExpression.Clone() : null, _whenExpressions.Select(f => (CaseWhenExpression)f.Clone()), _elseResultExpression != null ? (ConditionalTerm)_elseResultExpression.Clone() : null);
		}
	}
}
