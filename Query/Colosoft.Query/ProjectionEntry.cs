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
	/// Armazena as informações de uma coluna na projeção.
	/// </summary>
	public class ProjectionColumnInfo
	{
		/// <summary>
		/// Proprietário da coluna.
		/// </summary>
		public string Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Alias da coluna.
		/// </summary>
		public string Alias
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="name"></param>
		/// <param name="alias"></param>
		public ProjectionColumnInfo(string owner, string name, string alias)
		{
			Owner = owner;
			Name = name;
			Alias = alias;
		}
	}
	/// <summary>
	/// Armazena os dados do campo da projeção.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class ProjectionEntry : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private ConditionalTerm _term;

		private string _alias;

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
		/// Apelido do campo de projeção.
		/// </summary>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public ProjectionEntry()
		{
		}

		/// <summary>
		/// Cria a instancia com o termo associado e o alias da entrada.
		/// </summary>
		/// <param name="term"></param>
		/// <param name="alias"></param>
		public ProjectionEntry(ConditionalTerm term, string alias)
		{
			_term = term;
			_alias = alias;
		}

		/// <summary>
		/// Cria a instancia.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		public ProjectionEntry(string expression, string alias)
		{
			var lexer = Projection.ProjectionsLexer;
			var lResult = lexer.Execute(expression);
			Projection.FixLexerResult(lResult);
			Fill(lResult.Expressions);
			_alias = alias;
		}

		/// <summary>
		/// Recupera os dados da coluna de projeção com base nas expressões informadas
		/// </summary>
		/// <param name="expressions"></param>
		internal ProjectionEntry(List<Colosoft.Text.InterpreterExpression.Expression> expressions)
		{
			Fill(expressions.ToList());
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ProjectionEntry(SerializationInfo info, StreamingContext context)
		{
			_alias = info.GetString("Alias");
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
			info.AddValue("Alias", _alias);
			info.AddValue("TermType", _term != null ? _term.GetType().FullName : null);
			if(_term != null)
				info.AddValue("Term", _term);
		}

		/// <summary>
		/// Preenche os dados da instancia com a expressões informadas.
		/// </summary>
		/// <param name="expressions"></param>
		private void Fill(List<Text.InterpreterExpression.Expression> expressions)
		{
			int ownerIndex;
			int aliasIndex;
			GetParameters(expressions, out ownerIndex, out aliasIndex);
			IEnumerator<Text.InterpreterExpression.Expression> enumerator = null;
			if(aliasIndex >= 0)
				enumerator = (IEnumerator<Text.InterpreterExpression.Expression>)expressions.Take(aliasIndex).GetEnumerator();
			else
				enumerator = (IEnumerator<Text.InterpreterExpression.Expression>)expressions.GetEnumerator();
			if(enumerator.MoveNext())
				_term = ConditionalParser.GetContainer(ref enumerator, new Text.InterpreterExpression.ContainerChars('(', ')'));
			if(aliasIndex >= 0 && _alias == null)
				_alias = expressions[aliasIndex + 1].Text;
		}

		/// <summary>
		/// Recupera os indice os parametros contidos nas expressões.
		/// </summary>
		/// <param name="expressions"></param>
		/// <param name="ownerIndex"></param>
		/// <param name="aliasIndex"></param>
		private static void GetParameters(List<Colosoft.Text.InterpreterExpression.Expression> expressions, out int ownerIndex, out int aliasIndex)
		{
			ownerIndex = -1;
			aliasIndex = -1;
			if(expressions.Count > 2)
			{
				if(expressions[1].Token == (int)Text.InterpreterExpression.TokenID.Dot)
					ownerIndex = 1;
			}
			for(var i = 0; i < expressions.Count; i++)
			{
				if(aliasIndex < 0 && string.Compare(expressions[i].Text, "as", true) == 0 && (i + 1) < expressions.Count)
					aliasIndex = i;
				if(aliasIndex >= 0)
					break;
			}
			if(aliasIndex < 0 && expressions.Count > 1)
			{
				var expr1 = expressions[expressions.Count - 2];
				var expr2 = expressions[expressions.Count - 1];
				if((expr2.BeginPoint - (expr1.BeginPoint + expr1.Length)) > 0 && !Formula.IsArithmeticOperator(expr1) && expr2.Token != (int)Colosoft.Text.InterpreterExpression.TokenID.RParen)
				{
					aliasIndex = expressions.Count - 1;
					expressions.Insert(aliasIndex, new Text.InterpreterExpression.Expression(expr1.Container, expr2.BeginPoint, expr2.Line, "AS"));
				}
			}
		}

		/// <summary>
		/// Verifica se é uma entrada simples, ou seja possui apenas uma coluna.
		/// </summary>
		/// <returns></returns>
		public bool CheckSimpleEntry()
		{
			var term = _term;
			if(term is ConditionalContainer)
			{
				var container = (ConditionalContainer)term;
				if(container.ConditionalsCount != 1)
					return false;
				term = container.Conditionals.First();
			}
			return (term is Column);
		}

		/// <summary>
		/// Recupera as informações da coluna associada.
		/// </summary>
		/// <remarks>Os dados da coluna serão recuperados somente se a entrada for considerada simples.</remarks>
		/// <returns></returns>
		public ProjectionColumnInfo GetColumnInfo()
		{
			var term = _term;
			if(term is ConditionalContainer)
			{
				var container = (ConditionalContainer)term;
				if(container.ConditionalsCount != 1)
					return null;
				term = container.Conditionals.First();
			}
			if(term is Column)
			{
				var column = (Column)term;
				return new ProjectionColumnInfo(column.Owner, column.Name, Alias);
			}
			return null;
		}

		/// <summary>
		/// Clona uma projeção.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ProjectionEntry(_term != null ? (ConditionalTerm)_term.Clone() : null, _alias);
		}

		/// <summary>
		/// Transforma a expressão em um campo de projeção.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ProjectionEntry Parse(string expression)
		{
			var lexer = Projection.ProjectionsLexer;
			var lResult = lexer.Execute(expression);
			Projection.FixLexerResult(lResult);
			return new ProjectionEntry(lResult.Expressions);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Term: {0}, Alias:'{1}']", _term, _alias);
		}

		private static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("ProjectionEntry", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Alias"))
				_alias = reader.ReadContentAsString();
			reader.MoveToElement();
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
			if(!string.IsNullOrEmpty(_alias))
				writer.WriteAttributeString("Alias", _alias);
			writer.WriteStartElement("Term", Namespaces.Query);
			if(_term != null)
			{
				writer.WriteStartElement(_term.GetType().Name, Namespaces.Query);
				((System.Xml.Serialization.IXmlSerializable)_term).WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_alias = reader.ReadString();
			if(reader.ReadBoolean())
				_term = ConditionalTerm.GetConditionalTerm(reader);
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_alias);
			if(_term != null)
			{
				writer.Write(true);
				writer.Write(_term.GetType().Name);
				((ICompactSerializable)_term).Serialize(writer);
			}
			else
				writer.Write(false);
		}
	}
}
