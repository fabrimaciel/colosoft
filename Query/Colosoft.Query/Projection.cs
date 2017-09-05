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
	/// Armazena os dados da projeção do consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class Projection : IEnumerable<ProjectionEntry>, ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private static Text.InterpreterExpression.Lexer _projectionsLexer;

		private List<ProjectionEntry> _columns = new List<ProjectionEntry>();

		private string _functionName;

		/// <summary>
		/// Instancia do analizador lexo das projeções.
		/// </summary>
		public static Text.InterpreterExpression.Lexer ProjectionsLexer
		{
			get
			{
				if(_projectionsLexer == null)
					_projectionsLexer = new Text.InterpreterExpression.Lexer(new Parser.SqlTokenParser(), new Parser.SqlDefaultLexerConfiguration());
				return _projectionsLexer;
			}
		}

		/// <summary>
		/// Quantidade de campos da projeção.
		/// </summary>
		public int Count
		{
			get
			{
				return _columns.Count;
			}
		}

		/// <summary>
		/// Recupera os dados do campo na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ProjectionEntry this[int index]
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
		/// Nome da função da projeção
		/// </summary>
		public string FunctionName
		{
			get
			{
				return _functionName;
			}
			set
			{
				_functionName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Projection()
		{
		}

		/// <summary>
		/// Cria uma instancia da projeção já com as colunas que serão usadas.
		/// </summary>
		/// <param name="columns"></param>
		public Projection(IEnumerable<ProjectionEntry> columns)
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
		private Projection(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("c");
			for(var i = 0; i < count; i++)
				_columns.Add(info.GetValue(i.ToString(), typeof(ProjectionEntry)) as ProjectionEntry);
		}

		/// <summary>
		/// Fixa o resultado do analizador lexo.
		/// </summary>
		/// <param name="lResult"></param>
		/// <returns></returns>
		internal static int FixLexerResult(Text.InterpreterExpression.LexerResult lResult)
		{
			var source = new List<Text.InterpreterExpression.Expression>(lResult.Expressions);
			var size = source.Count;
			var offset = 0;
			int i = 0;
			for(i = 0; i < size; i++)
			{
				var current = source[i];
				if(current.Token == (int)Text.InterpreterExpression.TokenID.Identifier && ((i + 2) < size) && source[i + 1].Token == (int)Text.InterpreterExpression.TokenID.Dot && source[i + 2].Token == (int)Text.InterpreterExpression.TokenID.Identifier)
				{
					var e2 = source[i + 2];
					var ee = new Text.InterpreterExpression.Expression(current.Container, current.BeginPoint, current.Line, string.Format("{0}{1}{2}", current.Text, source[i + 1].Text, e2.Text));
					lResult.Expressions.RemoveAt(i - offset);
					lResult.Expressions.RemoveAt(i - offset);
					lResult.Expressions.RemoveAt(i - offset);
					lResult.Expressions.Insert(i - offset, ee);
					offset += 2;
				}
			}
			FixGroupExpressions(lResult.Expressions, (int)Parser.SqlTokenID.kCase, (int)Parser.SqlTokenID.kEnd, false);
			FixGroupExpressions(lResult.Expressions, (int)Parser.SqlTokenID.kWhen, (int)Parser.SqlTokenID.kThen, true);
			FixGroupExpressions(lResult.Expressions, (int)Parser.SqlTokenID.kThen, (int)Parser.SqlTokenID.kElse, true);
			FixGroupExpressions(lResult.Expressions, (int)Parser.SqlTokenID.kElse, (int)Parser.SqlTokenID.kEnd, true);
			return i;
		}

		/// <summary>
		/// Fixa o grupo de expressões.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startToken"></param>
		/// <param name="endToken"></param>
		/// <param name="groupInto">Identica se é para grupar por dentro.</param>
		private static void FixGroupExpressions(List<Text.InterpreterExpression.Expression> source, int startToken, int endToken, bool groupInto)
		{
			var startTokens = new Stack<int>();
			var statements = new Queue<GroupExpressionInfo>();
			for(var i = 0; i < source.Count; i++)
			{
				var current = source[i];
				if(current.Token == startToken)
				{
					startTokens.Push(i);
					Colosoft.Text.InterpreterExpression.Expression endExpression = null;
					for(var j = i + 1; source.Count < j; j++)
					{
						if(source[j].Token == endToken)
						{
							endExpression = source[j];
							break;
						}
					}
				}
				else if(startTokens.Count > 0 && current.Token == endToken)
				{
					statements.Enqueue(new GroupExpressionInfo(startTokens.Pop(), i));
				}
			}
			while (statements.Count > 0)
			{
				var statement = statements.Dequeue();
				var startExpression = source[statement.StartIndex];
				var stopExpression = source[statement.StopIndex];
				var stopIndex = statement.StopIndex + (groupInto ? 0 : 1);
				var startIndex = statement.StartIndex + (groupInto ? 1 : 0);
				source.Insert(stopIndex, new Text.InterpreterExpression.Expression(null, stopExpression.BeginPoint, stopExpression.Line, ')'));
				source.Insert(startIndex, new Text.InterpreterExpression.Expression(null, startExpression.BeginPoint, startExpression.Line, '('));
				foreach (var j in statements)
				{
					if(groupInto)
					{
						if(j.StartIndex > startIndex)
							j.StartIndex++;
						if(j.StartIndex >= stopIndex)
							j.StartIndex++;
						if(j.StopIndex > startIndex)
							j.StopIndex++;
						if(j.StopIndex >= stopIndex)
							j.StopIndex++;
					}
					else
					{
						if(j.StartIndex >= startIndex)
							j.StartIndex++;
						if(j.StartIndex > stopIndex)
							j.StartIndex++;
						if(j.StopIndex >= startIndex)
							j.StopIndex++;
						if(j.StopIndex > stopIndex)
							j.StopIndex++;
					}
				}
			}
		}

		/// <summary>
		/// Adiciona um novo campo para a projeção.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public Projection Add(ProjectionEntry entry)
		{
			if(entry == null)
				throw new ArgumentNullException("field");
			if(_columns.Exists(f => !string.IsNullOrEmpty(f.Alias) && !string.IsNullOrEmpty(entry.Alias) && f.Alias == entry.Alias))
				throw new InvalidOperationException("A field with the same key already exists.");
			_columns.Add(entry);
			return this;
		}

		/// <summary>
		/// Adiciona um novo camp para a projeção.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Projection Add(string expression, string alias = null)
		{
			expression.Require("expression").NotNull().NotEmpty();
			var entry = new ProjectionEntry(expression, alias);
			return Add(entry);
		}

		/// <summary>
		/// Concatena a projeção.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public Projection Concat(Projection projection)
		{
			projection.Require("projection").NotNull();
			foreach (var entry in projection)
				Add(entry);
			return this;
		}

		/// <summary>
		/// Transforma a expressão em uma projeção.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static Projection Parse(string expression)
		{
			var lexer = ProjectionsLexer;
			var lResult = lexer.Execute(expression);
			int i = FixLexerResult(lResult);
			var parts = new List<List<Colosoft.Text.InterpreterExpression.Expression>>();
			i = 0;
			var start = 0;
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
			var columns = parts.Select(f => new ProjectionEntry(f));
			return new Projection(columns);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_columns.Count > 0)
				return string.Join(",", _columns.Select(f => f.ToString()).ToArray());
			return string.Empty;
		}

		/// <summary>
		/// Clona a projeção.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new Projection(this._columns.Select(f => (ProjectionEntry)f.Clone())) {
				_functionName = this._functionName
			};
		}

		IEnumerator<ProjectionEntry> IEnumerable<ProjectionEntry>.GetEnumerator()
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
		/// Recupera o esquema Xml da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Projection", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				var column = new ProjectionEntry();
				((System.Xml.Serialization.IXmlSerializable)column).ReadXml(reader);
				_columns.Add(column);
			}
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (System.Xml.Serialization.IXmlSerializable i in _columns)
			{
				writer.WriteStartElement("ProjectionEntry", Namespaces.Query);
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
				_columns = new List<ProjectionEntry>();
			while (reader.ReadBoolean())
			{
				var column = new ProjectionEntry();
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

		/// <summary>
		/// Armazena as informações da expressão case.
		/// </summary>
		class GroupExpressionInfo
		{
			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="startIndex"></param>
			/// <param name="stopIndex"></param>
			public GroupExpressionInfo(int startIndex, int stopIndex)
			{
				StartIndex = startIndex;
				StopIndex = stopIndex;
			}

			/// <summary>
			/// Indice onde está a expressão de inicio.
			/// </summary>
			public int StartIndex;

			/// <summary>
			/// Indice onde está a expressão de fim.
			/// </summary>
			public int StopIndex;
		}
	}
}
