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
	/// Representa uma expressão condicional.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetConditionalSchema")]
	public class Conditional : ConditionalTerm
	{
		private ConditionalTerm _left;

		private ConditionalTerm _right;

		private Operator _operator;

		/// <summary>
		/// Expressão de comparação da esquerda.
		/// </summary>
		public ConditionalTerm Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		/// <summary>
		/// Expressão de comparação da direita.
		/// </summary>
		public ConditionalTerm Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		/// <summary>
		/// Expressão do operador de comparação.
		/// </summary>
		public Operator Operator
		{
			get
			{
				return _operator;
			}
			set
			{
				_operator = value;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public Conditional()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já com os parametros que serão usados.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="oper"></param>
		/// <param name="right"></param>
		public Conditional(ConditionalTerm left, Operator oper, ConditionalTerm right)
		{
			_left = left;
			_operator = oper;
			_right = right;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Conditional(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			var leftType = Type.GetType(info.GetString("LeftType"));
			var rightType = Type.GetType(info.GetString("RightType"));
			_left = (ConditionalTerm)info.GetValue("Left", typeof(ConditionalTerm));
			_operator = (Operator)info.GetValue("Operator", typeof(Operator));
			_right = (ConditionalTerm)info.GetValue("Right", typeof(ConditionalTerm));
		}

		/// <summary>
		/// Recupera os dados do objeto para serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("LeftType", _left.GetType().FullName);
			info.AddValue("Left", _left);
			info.AddValue("Operator", _operator);
			info.AddValue("RightType", _right.GetType().FullName);
			info.AddValue("Right", _right);
		}

		/// <summary>
		/// Executa um parse na expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ConditionalTerm ParseTerm(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				throw new ArgumentNullException("expression");
			var container = ConditionalParser.Parse(expression);
			if(container.ConditionalsCount == 1)
				return container.Conditionals.First();
			return container;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_operator != null && StringComparer.InvariantCultureIgnoreCase.Equals(_operator.Op, "EXISTS"))
				return string.Format("{0}", _left);
			return string.Format("{0} {1} {2}", _left, _operator, _right);
		}

		/// <summary>
		/// Clona um Conditional.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Conditional(Left != null ? (ConditionalTerm)Left.Clone() : null, Operator != null ? (Operator)Operator.Clone() : null, Right != null ? (ConditionalTerm)Right.Clone() : null);
		}

		private static System.Xml.XmlQualifiedName GetConditionalSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Conditional", Namespaces.Query);
		}

		/// <summary>
		/// Nome que qualifica o elemento XML.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("Conditional", Namespaces.Query);
			}
		}

		/// <summary>
		/// Recupera os dados da instancia do leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_left = GetConditionalTerm(reader);
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
				_operator = (Operator)GetConditionalTerm(reader);
			else
				reader.Skip();
			if(!reader.IsEmptyElement || reader.HasAttributes)
				_right = GetConditionalTerm(reader);
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
			writer.WriteStartElement("Left", Namespaces.Query);
			if(_left != null)
			{
				InsertConditionalTermType(writer, _left);
				((System.Xml.Serialization.IXmlSerializable)_left).WriteXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Operator", Namespaces.Query);
			if(_operator != null)
				((System.Xml.Serialization.IXmlSerializable)_operator).WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement("Right", Namespaces.Query);
			if(_right != null)
			{
				InsertConditionalTermType(writer, _right);
				((System.Xml.Serialization.IXmlSerializable)_right).WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Verifica se a expressão informada é um operador
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		internal static bool IsConditionalOperator(Text.InterpreterExpression.Expression expression)
		{
			var token = (Text.InterpreterExpression.TokenID)expression.Token;
			switch(token)
			{
			case Text.InterpreterExpression.TokenID.Greater:
			case Text.InterpreterExpression.TokenID.GreaterEqual:
			case Text.InterpreterExpression.TokenID.Less:
			case Text.InterpreterExpression.TokenID.LessEqual:
			case Text.InterpreterExpression.TokenID.Equal:
			case Text.InterpreterExpression.TokenID.EqualEqual:
			case Text.InterpreterExpression.TokenID.NotEqual:
				return true;
			}
			var token2 = (Colosoft.Query.Parser.SqlTokenID)expression.Token;
			switch(token2)
			{
			case Parser.SqlTokenID.kIs:
				return true;
			}
			return false;
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(_left != null)
			{
				writer.Write("Left");
				InsertConditionalTermType(writer, _left);
				((ICompactSerializable)_left).Serialize(writer);
			}
			else
				writer.Write("");
			if(_operator != null)
			{
				writer.Write("Operator");
				InsertConditionalTermType(writer, _operator);
				((ICompactSerializable)_operator).Serialize(writer);
			}
			else
				writer.Write("");
			if(_right != null)
			{
				writer.Write("Right");
				InsertConditionalTermType(writer, _right);
				((ICompactSerializable)_right).Serialize(writer);
			}
			else
				writer.Write("");
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			string position = reader.ReadString();
			if(position == "Left")
				_left = GetConditionalTerm(reader);
			position = reader.ReadString();
			if(position == "Operator")
				_operator = (Operator)GetConditionalTerm(reader);
			position = reader.ReadString();
			if(position == "Right")
				_right = GetConditionalTerm(reader);
		}
	}
}
