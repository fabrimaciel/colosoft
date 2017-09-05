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
	/// Representa um termo uma consulta.
	/// </summary>
	[Serializable]
	public abstract class ConditionalTerm : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private static Text.InterpreterExpression.Lexer _conditionalLexer;

		/// <summary>
		/// Recupera o analizador lexo das condicionais do sistema.
		/// </summary>
		internal static Text.InterpreterExpression.Lexer Lexer
		{
			get
			{
				if(_conditionalLexer == null)
				{
					_conditionalLexer = new Text.InterpreterExpression.Lexer(new Text.InterpreterExpression.DefaultTokenParser(), new Text.InterpreterExpression.DefaultLexerConfiguration());
				}
				return _conditionalLexer;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ConditionalTerm()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ConditionalTerm(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// Recupera o termo condicional contido no reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		internal static protected ConditionalTerm GetConditionalTerm(Colosoft.Serialization.IO.CompactReader reader)
		{
			var type = reader.ReadString();
			ConditionalTerm term = null;
			if(string.IsNullOrEmpty(type))
				term = new Constant();
			else
			{
				if(type == "Conditional")
					term = new Conditional();
				else if(type == "ConditionalContainer")
					term = new ConditionalContainer();
				else if(type == "Operator")
					term = new Operator();
				else if(type == "Constant")
					term = new Constant();
				else if(type == "Column")
					term = new Column();
				else if(type == "Variable")
					term = new Variable();
				else if(type == "ValuesArray")
					term = new ValuesArray();
				else if(type == "QueryTerm")
					term = new QueryTerm();
				else if(type == "FunctionCall")
					term = new FunctionCall();
				else if(type == "MinusTerm")
					term = new MinusTerm();
				else if(type == "Formula")
					term = new Formula();
				else if(type == "Empty")
					return null;
				else
					throw new QueryInvalidOperationException("Invalid conditional type");
			}
			((ICompactSerializable)term).Deserialize(reader);
			return term;
		}

		/// <summary>
		/// Clona o ConditionalTerm.
		/// </summary>
		/// <returns></returns>
		public abstract object Clone();

		/// <summary>
		/// Determina se não existe chamada de função na avaliação do termo.
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		public static bool HasNoFunctionCall(ConditionalTerm term)
		{
			if(term == null)
			{
				return true;
			}
			if(term is FunctionCall)
			{
				return false;
			}
			var cond = term as Conditional;
			if(cond == null)
			{
				return true;
			}
			return HasNoFunctionCall(cond.Left) && HasNoFunctionCall(cond.Right);
		}

		/// <summary>
		/// Recuperea os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		/// <summary>
		/// Recupera o termo condicional contido no reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		internal protected static ConditionalTerm GetConditionalTerm(System.Xml.XmlReader reader)
		{
			var type = reader.GetAttribute("type", Namespaces.SchemaInstance);
			if(string.IsNullOrEmpty(type))
				type = reader.LocalName;
			ConditionalTerm term = null;
			if(string.IsNullOrEmpty(type))
				term = new Constant();
			else
			{
				var index1 = type.IndexOf(":");
				if(index1 >= 0)
					type = type.Substring(index1 + 1);
				var startPoint = type.IndexOf('.');
				if(startPoint >= 0)
					type = type.Substring(startPoint + 1);
				if(type == "Conditional")
					term = new Conditional();
				else if(type == "ConditionalContainer")
					term = new ConditionalContainer();
				else if(type == "Operator")
					term = new Operator();
				else if(type == "Constant")
					term = new Constant();
				else if(type == "Column")
					term = new Column();
				else if(type == "Variable")
					term = new Variable();
				else if(type == "ValuesArray")
					term = new ValuesArray();
				else if(type == "QueryTerm")
					term = new QueryTerm();
				else if(type == "FunctionCall")
					term = new FunctionCall();
				else if(type == "MinusTerm")
					term = new MinusTerm();
				else if(type == "Formula")
					term = new Formula();
				else if(type == "Empty")
					return null;
				else if(type == "Case")
					term = new CaseConditional();
				else
					throw new QueryInvalidOperationException("Invalid conditional type");
			}
			((System.Xml.Serialization.IXmlSerializable)term).ReadXml(reader);
			return term;
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <returns></returns>
		public virtual System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("ConditionalTerm", Namespaces.Query);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			ReadXml(reader);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			WriteXml(writer);
		}

		/// <summary>
		/// Recupera os dados do termo do leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void ReadXml(System.Xml.XmlReader reader);

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected abstract void Serialize(Colosoft.Serialization.IO.CompactWriter writer);

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void Deserialize(Colosoft.Serialization.IO.CompactReader reader);

		/// <summary>
		/// Salva os dados do termo no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		protected abstract void WriteXml(System.Xml.XmlWriter writer);

		/// <summary>
		/// Insere o tipo do termo condicional
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="term"></param>
		internal protected static void InsertConditionalTermType(System.Xml.XmlWriter writer, ConditionalTerm term)
		{
			var qname = term.QualifiedName;
			var prefix = writer.LookupPrefix(qname.Namespace);
			if(string.IsNullOrEmpty(prefix))
				writer.WriteAttributeString("xmlns", qname.Namespace);
			writer.WriteAttributeString("type", Namespaces.SchemaInstance, string.Format("{0}{1}{2}", prefix, string.IsNullOrEmpty(prefix) ? "" : ":", qname.Name));
		}

		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			Deserialize(reader);
		}

		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			Serialize(writer);
		}

		/// <summary>
		/// Insere o tip do termo condicional
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="term"></param>
		protected void InsertConditionalTermType(Colosoft.Serialization.IO.CompactWriter writer, ConditionalTerm term)
		{
			writer.Write(term.GetType().Name);
		}
	}
}
