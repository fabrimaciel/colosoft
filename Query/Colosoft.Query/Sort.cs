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
	/// Armazena os dados da ordenação.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class Sort : IEnumerable<SortEntry>, ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private List<SortEntry> _columns = new List<SortEntry>();

		/// <summary>
		/// Quantidade de campos de ordenação.
		/// </summary>
		public int Count
		{
			get
			{
				return _columns.Count;
			}
		}

		/// <summary>
		/// Recupera e definie o campo de ordenação na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SortEntry this[int index]
		{
			get
			{
				return _columns[index];
			}
			set
			{
				_columns[index] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Sort()
		{
		}

		/// <summary>
		/// Cria uma instancia de ordenação com os campos.
		/// </summary>
		/// <param name="columns"></param>
		public Sort(IEnumerable<SortEntry> columns)
		{
			if(columns == null)
				throw new ArgumentNullException("columns");
			_columns.AddRange(columns);
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private Sort(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("c");
			for(var i = 0; i < count; i++)
				_columns.Add(info.GetValue(i.ToString(), typeof(SortEntry)) as SortEntry);
		}

		/// <summary>
		/// Adiciona um campo de ordenação.
		/// </summary>
		/// <param name="column"></param>
		public Sort Add(SortEntry column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			if(_columns.Exists(f => f.Equals(column)))
				throw new InvalidOperationException("A field with the same key already exists.");
			_columns.Add(column);
			return this;
		}

		/// <summary>
		/// Transforma a expressão informação em uma instancia de ordenação.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static Sort Parse(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				throw new ArgumentNullException("expression");
			var lexer = Projection.ProjectionsLexer;
			var lResult = lexer.Execute(expression);
			Projection.FixLexerResult(lResult);
			var parts = new List<List<Colosoft.Text.InterpreterExpression.Expression>>();
			int i = 0, start = 0;
			for(; i < lResult.Expressions.Count; i++)
			{
				var expr = lResult.Expressions[i];
				if(expr.Container == null && expr.Token == (int)Text.InterpreterExpression.TokenID.Comma)
				{
					parts.Add(lResult.Expressions.Skip(start).Take(i - start).ToList());
					start = i + 1;
				}
			}
			if((i - start) >= 1)
				parts.Add(lResult.Expressions.Skip(start).Take(i - start).ToList());
			return new Sort(parts.Select(f => new SortEntry(f)));
		}

		/// <summary>
		/// Clona um Sort
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new Sort(this._columns.Select(f => (SortEntry)f.Clone()));
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<SortEntry> GetEnumerator()
		{
			return _columns.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _columns.GetEnumerator();
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("c", _columns.Count);
			for(var i = 0; i < _columns.Count; i++)
				info.AddValue(i.ToString(), _columns[i]);
		}

		/// <summary>
		/// Recupera o esquema da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Sort", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "SortColumn")
					{
						var column = new SortEntry();
						((System.Xml.Serialization.IXmlSerializable)column).ReadXml(reader);
						_columns.Add(column);
					}
					else
						reader.Skip();
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (System.Xml.Serialization.IXmlSerializable i in _columns)
			{
				writer.WriteStartElement("SortColumn", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			while (reader.ReadBoolean())
			{
				var column = new SortEntry();
				((ICompactSerializable)column).Deserialize(reader);
				_columns.Add(column);
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			foreach (ICompactSerializable i in _columns)
			{
				writer.Write(true);
				i.Serialize(writer);
			}
			writer.Write(false);
		}
	}
}
