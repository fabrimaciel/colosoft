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

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena os dados do campo de ordenação.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class SortEntry : IEquatable<SortEntry>, ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private ConditionalTerm _term;

		private bool _reverse;

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
		/// Identifica se é uma ordenação invertida.
		/// </summary>
		public bool Reverse
		{
			get
			{
				return _reverse;
			}
			set
			{
				_reverse = value;
			}
		}

		/// <summary>
		/// Recupera o nome que representa a entrada de ordenação.
		/// </summary>
		public string Name
		{
			get
			{
				var term = Term;
				while (term != null)
				{
					var column = term as Column;
					if(column != null)
						return column.Name;
					if(term is ConditionalContainer && ((ConditionalContainer)term).ConditionalsCount == 1)
					{
						term = ((ConditionalContainer)term).Conditionals.First();
					}
					else
						term = null;
				}
				return null;
			}
		}

		/// <summary>
		/// Recupera os dados da coluna de projeção com base nas expressões informadas
		/// </summary>
		/// <param name="expressions"></param>
		internal SortEntry(List<Colosoft.Text.InterpreterExpression.Expression> expressions)
		{
			Fill(expressions.ToList());
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SortEntry()
		{
		}

		/// <summary>
		/// Cria a instancia já definindo os parametros.
		/// </summary>
		/// <param name="term"></param>
		/// <param name="reverse"></param>
		public SortEntry(ConditionalTerm term, bool reverse)
		{
			_term = term;
			_reverse = reverse;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private SortEntry(SerializationInfo info, StreamingContext context)
		{
			_reverse = info.GetBoolean("Reverse");
			var termType = info.GetString("TermType");
			if(!string.IsNullOrEmpty(termType))
				_term = (ConditionalTerm)info.GetValue("Term", Type.GetType(termType, true));
		}

		/// <summary>
		/// Preenche os dados da instancia com a expressões informadas.
		/// </summary>
		/// <param name="expressions"></param>
		private void Fill(List<Colosoft.Text.InterpreterExpression.Expression> expressions)
		{
			var last = expressions.Last();
			if(last.Token == (int)Parser.SqlTokenID.kDesc)
				_reverse = true;
			if(last.Token == (int)Parser.SqlTokenID.kAsc || last.Token == (int)Parser.SqlTokenID.kDesc)
				expressions.RemoveAt(expressions.Count - 1);
			var enumerator = (IEnumerator<Text.InterpreterExpression.Expression>)expressions.GetEnumerator();
			if(enumerator.MoveNext())
				_term = ConditionalParser.GetContainer(ref enumerator, new Text.InterpreterExpression.ContainerChars('(', ')'));
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Reverse", _reverse);
			info.AddValue("TermType", _term != null ? _term.GetType().FullName : null);
			if(_term != null)
				info.AddValue("Term", _term);
		}

		/// <summary>
		/// Compara com outra coluna de ordenação.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(SortEntry other)
		{
			if(other == null)
				return false;
			return this.Term == other.Term;
		}

		/// <summary>
		/// String que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Term:{0}, Reverse:{2}]", _term, _reverse);
		}

		/// <summary>
		/// Clona coluna de Sort.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new SortEntry((ConditionalTerm)Term.Clone(), Reverse);
		}

		/// <summary>
		/// Recupera o esquema da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("SortEntry", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Reverse"))
				_reverse = reader.ReadContentAsBoolean();
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
			writer.WriteAttributeString("Reverse", _reverse.ToString().ToLower());
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
			_reverse = reader.ReadBoolean();
			if(reader.ReadBoolean())
				_term = ConditionalTerm.GetConditionalTerm(reader);
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_reverse);
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
