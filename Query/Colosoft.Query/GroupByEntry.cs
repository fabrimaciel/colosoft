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
	/// Armazena os dados de uma coluna que será usada no agrupamento.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class GroupByEntry : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private ConditionalTerm _term;

		/// <summary>
		/// Termo associado.
		/// </summary>
		public ConditionalTerm Term
		{
			get
			{
				return _term;
			}
			set
			{
				_term = value;
			}
		}

		/// <summary>
		/// Recupera os dados da coluna de projeção com base nas expressões informadas
		/// </summary>
		/// <param name="expressions"></param>
		internal GroupByEntry(List<Colosoft.Text.InterpreterExpression.Expression> expressions)
		{
			Fill(expressions.ToList());
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GroupByEntry()
		{
		}

		/// <summary>
		/// Cria a instancia.
		/// </summary>
		/// <param name="expression"></param>
		public GroupByEntry(string expression)
		{
			var lexer = Projection.ProjectionsLexer;
			var lResult = lexer.Execute(expression);
			Projection.FixLexerResult(lResult);
			Fill(lResult.Expressions);
		}

		/// <summary>
		/// Cria uma instancia.
		/// </summary>
		/// <param name="term"></param>
		public GroupByEntry(ConditionalTerm term)
		{
			_term = term;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private GroupByEntry(SerializationInfo info, StreamingContext context)
		{
			var termType = info.GetString("TermType");
			if(!string.IsNullOrEmpty(termType))
				_term = (ConditionalTerm)info.GetValue("Term", Type.GetType(termType, true));
		}

		/// <summary>
		/// Recupera os dados da instância anteriormente serializada.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("TermType", _term != null ? _term.GetType().FullName : null);
			if(_term != null)
				info.AddValue("Term", _term);
		}

		/// <summary>
		/// Clona uma projeção.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new GroupByEntry(_term != null ? (ConditionalTerm)_term.Clone() : null);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Term: '{0}']", _term);
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
				_term = ConditionalTerm.GetConditionalTerm(reader);
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(_term != null)
			{
				writer.Write(true);
				writer.Write(_term.GetType().Name);
				((ICompactSerializable)_term).Serialize(writer);
			}
			else
				writer.Write(false);
		}

		/// <summary>
		/// Preenche os dados da instancia com a expressões informadas.
		/// </summary>
		/// <param name="expressions"></param>
		private void Fill(List<Colosoft.Text.InterpreterExpression.Expression> expressions)
		{
			var enumerator = (IEnumerator<Text.InterpreterExpression.Expression>)expressions.GetEnumerator();
			if(enumerator.MoveNext())
				_term = ConditionalParser.GetContainer(ref enumerator, new Text.InterpreterExpression.ContainerChars('(', ')'));
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("GroupByEntry", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement || reader.HasAttributes)
			{
				reader.ReadStartElement();
				_term = ConditionalTerm.GetConditionalTerm(reader);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Term", Namespaces.Query);
			if(_term != null)
			{
				writer.WriteStartElement(_term.GetType().Name, Namespaces.Query);
				((System.Xml.Serialization.IXmlSerializable)_term).WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
