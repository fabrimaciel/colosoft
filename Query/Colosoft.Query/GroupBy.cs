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
	/// Representa um agrupamento.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class GroupBy : IEnumerable<GroupByEntry>, ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private List<GroupByEntry> _columns = new List<GroupByEntry>();

		/// <summary>
		/// Quantidade de colunas da instancia.
		/// </summary>
		public int Count
		{
			get
			{
				return _columns.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GroupBy()
		{
		}

		/// <summary>
		/// Cria uma instancia definidos as colunas.
		/// </summary>
		/// <param name="columns"></param>
		public GroupBy(IEnumerable<GroupByEntry> columns)
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
		private GroupBy(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("C");
			for(var i = 0; i < count; i++)
				_columns.Add((GroupByEntry)info.GetValue(i.ToString(), typeof(GroupByEntry)));
		}

		/// <summary>
		/// Adiciona uma nova coluna para a instancia.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public GroupBy Add(GroupByEntry column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			return this;
		}

		/// <summary>
		/// Adiciona uma nova coluna para a instancia.
		/// </summary>
		/// <param name="columnName">Nome da coluna.</param>
		/// <returns></returns>
		public GroupBy Add(string columnName)
		{
			if(string.IsNullOrEmpty(columnName))
				throw new ArgumentNullException("columnName");
			return this;
		}

		/// <summary>
		/// Transforma a expressão em uma instancia de agrupamento.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static GroupBy Parse(string expression)
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
			var columns = parts.Select(f => new GroupByEntry(f));
			return new GroupBy(columns);
		}

		/// <summary>
		/// Clona um GroupBy clause.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new GroupBy(this._columns.Select(f => (GroupByEntry)f.Clone()));
		}

		/// <summary>
		/// Recupera os dado da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("C", _columns.Count);
			for(var i = 0; i < _columns.Count; i++)
				info.AddValue(i.ToString(), _columns[i]);
		}

		IEnumerator<GroupByEntry> IEnumerable<GroupByEntry>.GetEnumerator()
		{
			return _columns.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _columns.GetEnumerator();
		}

		/// <summary>
		/// Recupera o esquema Xml da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("GroupBy", Namespaces.Query);
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
					if(reader.LocalName == "GroupByColumn")
					{
						var column = new GroupByEntry();
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
				writer.WriteStartElement("GroupByColumn", Namespaces.Query);
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
			if(_columns == null)
			{
				_columns = new List<GroupByEntry>();
			}
			while (reader.ReadBoolean())
			{
				var column = new GroupByEntry();
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
