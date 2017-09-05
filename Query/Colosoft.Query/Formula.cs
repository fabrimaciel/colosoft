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
using System.Text;

namespace Colosoft.Query
{
	/// <summary>
	/// Possíveis operadores matemáticos.
	/// </summary>
	public enum MathematicalOperator
	{
		/// <summary>
		/// Subtração.
		/// </summary>
		Subtraction,
		/// <summary>
		/// Adição.
		/// </summary>
		Addition,
		/// <summary>
		/// Divisão.
		/// </summary>
		Division,
		/// <summary>
		/// Multiplicação.
		/// </summary>
		Multiplication,
		/// <summary>
		/// Exponenciação.
		/// </summary>
		Exponentiation,
		/// <summary>
		/// Módulo.
		/// </summary>
		Module
	}
	/// <summary>
	/// Representa uma equação.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetFormulaSchema")]
	public class Formula : ConditionalTerm, System.Xml.Serialization.IXmlSerializable, ICompactSerializable
	{
		private List<ConditionalTerm> _parts = new List<ConditionalTerm>();

		private List<MathematicalOperator> _operators = new List<MathematicalOperator>();

		/// <summary>
		/// Partes da formula.
		/// </summary>
		public List<ConditionalTerm> Parts
		{
			get
			{
				return _parts;
			}
		}

		/// <summary>
		/// Operadores da formula.
		/// </summary>
		public List<MathematicalOperator> Operators
		{
			get
			{
				return _operators;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Formula()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Formula(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			var partsCount = info.GetInt32("PC");
			var operatorsCount = info.GetInt32("OC");
			for(var i = 0; i < partsCount; i++)
				_parts.Add((ConditionalTerm)info.GetValue("p" + i, Type.GetType(info.GetString("pt" + i), true)));
			for(var i = 0; i < operatorsCount; i++)
				_operators.Add((MathematicalOperator)info.GetInt32("o" + i));
		}

		/// <summary>
		/// Recupera os dados
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OC", _operators.Count);
			for(var i = 0; i < _operators.Count; i++)
				info.AddValue("o" + i, (int)_operators[i]);
			info.AddValue("PC", _parts.Count);
			for(var i = 0; i < _parts.Count; i++)
			{
				info.AddValue("pt" + i, _parts[i].GetType().FullName);
				info.AddValue("p" + i, _parts[i]);
			}
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			string position = reader.ReadString();
			while (position.Equals("ConditionalTerm"))
			{
				_parts.Add(GetConditionalTerm(reader));
				position = reader.ReadString();
			}
			while (position.Equals("Operator"))
			{
				_operators.Add((MathematicalOperator)Enum.Parse(typeof(MathematicalOperator), reader.ReadString()));
				position = reader.ReadString();
			}
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			foreach (ConditionalTerm i in _parts)
			{
				writer.Write("ConditionalTerm");
				writer.Write(i.GetType().Name);
				((ICompactSerializable)i).Serialize(writer);
			}
			foreach (var i in _operators)
			{
				writer.Write("Operator");
				writer.Write(i.ToString());
			}
			writer.Write("End");
		}

		/// <summary>
		/// Clona um container condicional.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			var formula = new Formula();
			foreach (var term in _parts)
				formula._parts.Add((ConditionalTerm)term.Clone());
			formula._operators = new List<MathematicalOperator>(_operators);
			return formula;
		}

		/// <summary>
		/// Recupera a string que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_parts.Count > 0)
			{
				var sb = new StringBuilder("(");
				for(int i = 0, j = -1; i < _parts.Count; i++, j++)
				{
					if(_operators.Count > j && j >= 0)
					{
						switch(_operators[j])
						{
						case MathematicalOperator.Addition:
							sb.Append(" + ");
							break;
						case MathematicalOperator.Division:
							sb.Append(" / ");
							break;
						case MathematicalOperator.Exponentiation:
							sb.Append(" ^ ");
							break;
						case MathematicalOperator.Module:
							sb.Append(" % ");
							break;
						case MathematicalOperator.Multiplication:
							sb.Append(" * ");
							break;
						case MathematicalOperator.Subtraction:
							sb.Append(" - ");
							break;
						}
					}
					sb.Append(_parts[i].ToString());
				}
				sb.Append(")");
				return sb.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// Recupera o operador matemático representado pela expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		internal static MathematicalOperator GetMathematicalOperator(Text.InterpreterExpression.Expression expression)
		{
			var token = (Text.InterpreterExpression.TokenID)expression.Token;
			switch(token)
			{
			case Text.InterpreterExpression.TokenID.Plus:
				return MathematicalOperator.Addition;
			case Text.InterpreterExpression.TokenID.Minus:
				return MathematicalOperator.Subtraction;
			case Text.InterpreterExpression.TokenID.Star:
				return MathematicalOperator.Multiplication;
			case Text.InterpreterExpression.TokenID.Slash:
				return MathematicalOperator.Division;
			case Text.InterpreterExpression.TokenID.Percent:
				return MathematicalOperator.Module;
			}
			throw new InvalidOperationException("Invalid mathematical operator");
		}

		/// <summary>
		/// Verifica se a expressão informada é um operador de 
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		internal static bool IsArithmeticOperator(Text.InterpreterExpression.Expression expression)
		{
			var token = (Text.InterpreterExpression.TokenID)expression.Token;
			switch(token)
			{
			case Text.InterpreterExpression.TokenID.Plus:
			case Text.InterpreterExpression.TokenID.Minus:
			case Text.InterpreterExpression.TokenID.Star:
			case Text.InterpreterExpression.TokenID.Slash:
			case Text.InterpreterExpression.TokenID.Percent:
				return true;
			}
			return false;
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetFormulaSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Formula", Namespaces.Query);
		}

		/// <summary>
		/// Nome XML que irá representa a o tipo na serialização.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("Formula", Namespaces.Query);
			}
		}

		/// <summary>
		/// Lê os dados serializados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement && reader.LocalName == "Parts")
			{
				reader.ReadStartElement("Parts", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					_parts.Add(GetConditionalTerm(reader));
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "Operators")
			{
				reader.ReadStartElement("Operators", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					_operators.Add((MathematicalOperator)Enum.Parse(typeof(MathematicalOperator), reader.ReadElementString()));
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Salva os dados da instancia no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteStartElement("Parts", Namespaces.Query);
			foreach (ConditionalTerm i in _parts)
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
			writer.WriteEndElement();
			writer.WriteStartElement("Operators", Namespaces.Query);
			foreach (var i in _operators)
				writer.WriteElementString("MathematicalOperator", Namespaces.Query, i.ToString());
			writer.WriteEndElement();
		}
	}
}
