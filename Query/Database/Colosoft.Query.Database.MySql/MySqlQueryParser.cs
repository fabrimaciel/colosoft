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
using Colosoft.Data.Schema;

namespace Colosoft.Query.Database.MySql
{
	/// <summary>
	/// Gerador de texto de query padrão
	/// </summary>
	public class MySqlQueryParser : SqlQueryParser
	{
		private StringBuilder _sqlQuery;

		private Dictionary<string, QueryInfo> _queryParameters = new Dictionary<string, QueryInfo>();

		/// <summary>
		/// Monitor usado para verificar o nome da coluna.
		/// Se ele estiver busy quer dizer não será validado o nome da coluna.
		/// </summary>
		private Colosoft.Threading.SimpleMonitor _columnNameNoCheckMonitor = new Threading.SimpleMonitor();

		private object _objLock = new object();

		/// <summary>
		/// Texto do parse.
		/// </summary>
		public override string Text
		{
			get
			{
				return _sqlQuery.ToString();
			}
		}

		/// <summary>
		/// Constutor da classe
		/// </summary>
		/// <param name="translator">Classe de tradução de dados</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		public MySqlQueryParser(IQueryTranslator translator, ITypeSchema typeSchema) : base(translator, typeSchema)
		{
		}

		/// <summary>
		/// Constrói o texto.
		/// </summary>
		/// <param name="part">Partes que serão usadas.</param>
		private void BuildText(SqlQueryPart part)
		{
			EntityAliasDictionary = null;
			if(part.HasFlag(SqlQueryPart.Select))
				SelectParser();
			if(part.HasFlag(SqlQueryPart.Joins) && Query.Joins.Length > 0)
				JoinParser();
			if(part.HasFlag(SqlQueryPart.Where))
				WhereParser();
			if(part.HasFlag(SqlQueryPart.GroupBy) && Query.GroupBy != null)
				GroupByParser();
			if(part.HasFlag(SqlQueryPart.Having) && Query.Having != null)
				HavingParser();
			if(part.HasFlag(SqlQueryPart.OrderBy) && Query.Sort != null)
				SortParser();
			if(Query.Unions != null)
			{
				var root = Query;
				foreach (var union in Query.Unions)
				{
					Append(string.Format(" UNION{0} ", union.All ? " ALL" : null));
					Query = union.Query;
					BuildText(SqlQueryPart.All);
				}
				Query = root;
			}
			if(UseTakeParameter && Query.TakeParameters != null && (Query.TakeParameters.Skip > 0 || Query.TakeParameters.Take > 0))
				FormatTakeParameter();
		}

		/// <summary>
		/// Método que gera e retorna o texto da query
		/// </summary>
		/// <returns>Retorna o texto com o comando sql</returns>
		public override string GetText()
		{
			return GetText(SqlQueryPart.All);
		}

		/// <summary>
		/// Método que gera e retorna o texto da query
		/// </summary>
		/// <returns>Retorna o texto com o comando sql</returns>
		public override string GetText(SqlQueryPart part)
		{
			_sqlQuery = new StringBuilder();
			_queryParameters.Clear();
			BuildText(part);
			return _sqlQuery.ToString();
		}

		/// <summary>
		/// Adiciona ao campo string builder o texto correspondente a parte SELECT da query
		/// </summary>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser SelectParser()
		{
			if(Query.IsSelectDistinct)
				Append("SELECT DISTINCT ");
			else
				Append("SELECT ");
			ITypeMetadata mainTypeMetadata = null;
			for(int i = 0; i < Query.Entities.Length; i++)
			{
				if(string.IsNullOrEmpty(Query.Entities[i].FullName))
					continue;
				ITypeMetadata typeMetadata = null;
				if(!Query.IgnoreTypeSchema)
				{
					typeMetadata = TypeSchema.GetTypeMetadata(Query.Entities[i].FullName);
					if(typeMetadata == null)
						throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_TypeMetadataNotFoundByFullName, Query.Entities[i].FullName).Format());
				}
				if(i == 0)
				{
					mainTypeMetadata = typeMetadata;
					if(string.IsNullOrEmpty(Query.Entities[i].Alias))
						Query.Entities[i].Alias = "main";
				}
				else if(string.IsNullOrEmpty(Query.Entities[i].Alias))
					Query.Entities[i].Alias = "main" + i.ToString();
			}
			EntityInfo entity = Query.Entities[0];
			if(!string.IsNullOrEmpty(entity.FullName))
			{
				if(Query.Projection == null)
				{
					if(mainTypeMetadata != null && mainTypeMetadata.IsVersioned)
						Query.HasRowVersion = true;
					var properties = mainTypeMetadata != null ? mainTypeMetadata.Where(f => f.Direction == Data.Schema.DirectionParameter.Input || f.Direction == Data.Schema.DirectionParameter.InputOutput).ToArray() : new IPropertyMetadata[0];
					if(properties.Length == 0)
						throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_NotFoundPropertiesForTypeMetadata, entity.FullName).Format());
					Query.Projection = new Projection();
					for(var i = 0; i < properties.Length; i++)
					{
						var prop = properties[i];
						Query.Projection.Add(new ProjectionEntry(string.Format("{0}.{1}", entity.Alias, prop.Name), prop.Name));
						var translateName = Translator.GetName(entity, prop.Name, Query.IgnoreTypeSchema);
						var translateColumnName = translateName as TranslatedColumnName;
						if(translateColumnName != null)
						{
							if(!string.IsNullOrEmpty(translateColumnName.TableAlias))
								AppendQuoteExpression(translateColumnName.TableAlias).Append('.');
							AppendQuoteExpression(translateColumnName.Name);
						}
						if(prop.ColumnName != prop.Name)
						{
							Append(" AS ").AppendQuoteExpression(FormatProjectionAlias(prop.Name));
						}
						if(i + 1 < properties.Length)
							Append(',');
					}
					Append(' ');
				}
				else
					ProjectionParser();
				if(Query.HasRowVersion)
				{
				}
				var tableName = Translator.GetName(entity, Query.IgnoreTypeSchema);
				if(tableName == null)
					throw new InvalidOperationException(string.Format("Not found table name for entity '{0}'", entity.FullName));
				Append("FROM ").AppendTranslatedName(tableName).Append(' ').AppendQuoteExpression(entity.Alias).Append(' ');
			}
			else
			{
				var sqlParser = new MySqlQueryParser(Translator, TypeSchema) {
					Query = entity.SubQuery,
					UseTakeParameter = true
				};
				var subQueryText = sqlParser.GetText();
				if(Query.Projection == null)
				{
					Query.Projection = new Projection();
					foreach (var column in entity.SubQuery.Projection)
						Query.Projection.Add(new ProjectionEntry(!string.IsNullOrEmpty(entity.Alias) ? string.Format("{0}.{1}", entity.Alias, string.IsNullOrEmpty(column.Alias) ? (string.IsNullOrEmpty(column.GetColumnInfo().Alias) ? column.GetColumnInfo().Name : column.GetColumnInfo().Alias) : column.Alias) : column.Alias, column.Alias));
				}
				ProjectionParser();
				Append("FROM (").Append(subQueryText).Append(") ").AppendQuoteExpression(entity.Alias);
			}
			return this;
		}

		/// <summary>
		/// Adiciona ao campo string builder o texto correspondente a parte JOIN da query
		/// </summary>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser JoinParser()
		{
			foreach (var join in Query.Joins)
			{
				switch(join.Type)
				{
				case JoinType.Inner:
					Append("INNER JOIN ");
					break;
				case JoinType.Right:
					Append("RIGHT JOIN ");
					break;
				case JoinType.Left:
					Append("LEFT JOIN ");
					break;
				case JoinType.Cross:
					Append("CROSS JOIN ");
					break;
				default:
					throw new InvalidOperationException("Invalid join type");
				}
				var infoEntity = GetEntity(join.Right);
				if(string.IsNullOrEmpty(infoEntity.FullName))
				{
					Format(infoEntity.SubQuery);
				}
				else
				{
					var tableName = Translator.GetName(infoEntity, Query.IgnoreTypeSchema);
					if(tableName == null)
						throw new InvalidOperationException(string.Format("Not found table name for entity '{0}'", infoEntity.FullName));
					AppendTranslatedName(tableName).Append(' ');
				}
				AppendQuoteExpression(infoEntity.Alias);
				if(join.Conditional != null)
					Append(" ON (").Format(join.Conditional).Append(')');
			}
			return this;
		}

		/// <summary>
		/// Adiciona ao campo string builder o texto correspondente a parte WHERE da query
		/// </summary>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser WhereParser()
		{
			if(Query.WhereClause.ConditionalsCount > 0)
				Append(" WHERE ").Format(Query.WhereClause);
			return this;
		}

		/// <summary>
		/// Adiciona ao campo StringBuilder local o texto corresponte a parte GROUP BY da query
		/// </summary>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser GroupByParser()
		{
			Append(" GROUP BY ");
			int counter = 1;
			foreach (var groupby in Query.GroupBy)
			{
				Format(groupby);
				if(counter != Query.GroupBy.Count)
				{
					Append(',');
					counter++;
				}
			}
			return this;
		}

		/// <summary>
		/// Adiciona ao campo StringBuilder local o texto correspondente a parte do HAVING.
		/// </summary>
		/// <returns></returns>
		private MySqlQueryParser HavingParser()
		{
			Append(" HAVING ");
			_columnNameNoCheckMonitor.Enter();
			using (_columnNameNoCheckMonitor)
				Format(Query.Having);
			return this;
		}

		/// <summary>
		/// Adiciona ao campo StringBuilder local o texto correspondente a parte SORT da query
		/// </summary>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser SortParser()
		{
			Append(" ORDER BY ");
			int counter = 1;
			foreach (var sort in Query.Sort)
			{
				Format(sort);
				if(counter != Query.Sort.Count)
				{
					Append(',');
					counter++;
				}
			}
			return this;
		}

		/// <summary>
		/// Faz o parse das projeções.
		/// </summary>
		/// <returns>Retorna a própria instância.</returns>
		private MySqlQueryParser ProjectionParser()
		{
			int counter = 1;
			foreach (var proj in Query.Projection)
			{
				Format(proj);
				if(counter != Query.Projection.Count)
				{
					Append(',');
					counter++;
				}
				else
					Append(' ');
			}
			return this;
		}

		/// <summary>
		/// Forma o parametro Take.
		/// </summary>
		/// <returns></returns>
		private MySqlQueryParser FormatTakeParameter()
		{
			Append(string.Format(" LIMIT {0},{1}", Query.TakeParameters.Skip, Query.TakeParameters.Take));
			return this;
		}

		/// <summary>
		/// Formata o texto de uma <see cref="ProjectionEntry"/>
		/// </summary>
		/// <param name="entry">Recebe a projeção de coluna</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(ProjectionEntry entry)
		{
			Format(entry.Term);
			var columnInfo = entry.GetColumnInfo();
			if(!string.IsNullOrEmpty(entry.Alias))
				Append(" AS ").AppendQuoteExpression(FormatProjectionAlias(entry.Alias));
			else if(columnInfo != null)
				Append(" AS ").AppendQuoteExpression(FormatProjectionAlias(columnInfo.Name));
			return this;
		}

		/// <summary>
		/// Formata texto de um <see cref="ConditionalTerm"/>
		/// </summary>
		/// <param name="conditionalTerm">Termo condicional</param>
		/// <param name="ignoreContainerFormat">Identifica que é para ignorar a formatação de de container para a consulta.</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(ConditionalTerm conditionalTerm, bool ignoreContainerFormat = false)
		{
			if(conditionalTerm is Constant)
				Append(((Constant)conditionalTerm).Text);
			else if(conditionalTerm is Column)
				Format((Column)conditionalTerm);
			else if(conditionalTerm is Variable)
			{
				var variable = (Variable)conditionalTerm;
				var parameter = Query != null ? Query.Parameters.Where(f => f.Name == variable.Name).FirstOrDefault() : null;
				QueryInfo queryParameter = null;
				if(parameter != null && (parameter.Value is QueryInfo || parameter.Value is Queryable))
				{
					Query.Parameters.Remove(parameter);
					var value = parameter.Value;
					if(value is Queryable)
						value = ((Queryable)value).CreateQueryInfo();
					try
					{
						_queryParameters.Add(parameter.Name, (QueryInfo)value);
					}
					catch(ArgumentException)
					{
						throw new ConditionalParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ConditionalParserException_DuplicateParameter, parameter.Name).Format());
					}
					Format(new QueryTerm((QueryInfo)value), ignoreContainerFormat);
				}
				else if(_queryParameters.TryGetValue(variable.Name, out queryParameter))
				{
					Format(new QueryTerm(queryParameter), ignoreContainerFormat);
				}
				else
					Append(((Variable)conditionalTerm).Name);
			}
			else if(conditionalTerm is ValuesArray)
			{
				var values = (ValuesArray)conditionalTerm;
				if(values.Values != null && values.Values.Length == 1 && values.Values[0] is Variable)
				{
					var variable = (Variable)values.Values[0];
					var parameter = Query != null ? Query.Parameters.Where(f => f.Name == variable.Name).FirstOrDefault() : null;
					QueryInfo queryParameter = null;
					if(parameter != null && (parameter.Value is QueryInfo || parameter.Value is Queryable))
					{
						Query.Parameters.Remove(parameter);
						var value = parameter.Value;
						if(value is Queryable)
							value = ((Queryable)value).CreateQueryInfo();
						try
						{
							_queryParameters.Add(variable.Name, (QueryInfo)value);
						}
						catch(ArgumentException)
						{
							throw new ConditionalParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ConditionalParserException_DuplicateParameter, parameter.Name).Format());
						}
						Format(new QueryTerm((QueryInfo)value), ignoreContainerFormat);
						values = null;
					}
					else if(_queryParameters.TryGetValue(variable.Name, out queryParameter))
					{
						Format(new QueryTerm(queryParameter), ignoreContainerFormat);
						values = null;
					}
				}
				if(values != null)
				{
					Append('(');
					var gone = false;
					foreach (var i in values.Values)
					{
						if(gone)
							Append(", ");
						else
							gone = true;
						if(i != null)
							Format(i, ignoreContainerFormat);
						else
							Append("NULL");
					}
					Append(')');
				}
			}
			else if(conditionalTerm is ConditionalContainer)
				Format((ConditionalContainer)conditionalTerm);
			else if(conditionalTerm is QueryTerm)
				Format((QueryTerm)conditionalTerm, ignoreContainerFormat);
			else if(conditionalTerm is FunctionCall)
			{
				var f = (FunctionCall)conditionalTerm;
				var name = f.Call.ToString().Replace("'", String.Empty);
				if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "DATEADD") && f.Parameters.Length == 3)
				{
					Append("DATE_ADD(");
					Format(f.Parameters[2]);
					Append(", INTERVAL ");
					Format(f.Parameters[1]);
					Append(' ');
					Format(f.Parameters[0]);
					Append(')');
				}
				else if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "DATEFORMAT") && f.Parameters.Length == 2)
				{
					Append("DATE_FORMAT(");
					Format(f.Parameters[0]);
					Append(",");
					Format(f.Parameters[1]);
					Append(')');
				}
				else if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "CAST") && f.Parameters.Length == 2)
				{
					Append(name).Append('(').Format(f.Parameters[0]).Append(" AS ");
					if(f.Parameters[1] is Constant)
					{
						var text = ((Constant)f.Parameters[1]).Text;
						var comparer = StringComparer.InvariantCultureIgnoreCase;
						if(comparer.Equals(text, "INT"))
							Format(new Constant("SIGNED"));
						else if(comparer.Equals(text, "VARCHAR"))
							Format(new Constant("CHAR"));
						else
							Format(f.Parameters[1]);
					}
					else
						Format(f.Parameters[1]);
					Append(')');
				}
				else
				{
					if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "ISNULL"))
						name = "COALESCE";
					else if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "CHARINDEX"))
						name = "INSTR";
					Append(name);
					Append('(');
					if(f.Options.HasFlag(FunctionCallOptions.Distinct))
						Append("DISTINCT ");
					var gone = false;
					foreach (var paramTerm in f.Parameters)
					{
						if(gone)
							Append(", ");
						else
							gone = true;
						if(paramTerm != null)
							Format(paramTerm, true);
						else
							Append("NULL");
					}
					Append(')');
				}
			}
			else if(conditionalTerm is Formula)
				Format((Formula)conditionalTerm);
			else if(conditionalTerm is MinusTerm)
			{
				Append("-");
				Format(((MinusTerm)conditionalTerm).Term);
			}
			else if(conditionalTerm is Conditional)
				Format((Conditional)conditionalTerm);
			else if(conditionalTerm is CaseConditional)
				Format((CaseConditional)conditionalTerm);
			else
				throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.NotSupportedException_TypeOfConditionalTermNotSupported, conditionalTerm.GetType().ToString()).Format());
			return this;
		}

		/// <summary>
		/// Formata a condicional do case.
		/// </summary>
		/// <param name="caseConditional"></param>
		/// <returns></returns>
		private MySqlQueryParser Format(CaseConditional caseConditional)
		{
			Append("CASE");
			if(caseConditional.InputExpression != null)
				Append(' ').Format(caseConditional.InputExpression);
			foreach (var i in caseConditional.WhenExpressions)
				Append(" WHEN ").Format(i.Expression).Append(" THEN ").Format(i.ResultExpression);
			if(caseConditional.ElseResultExpression != null)
				Append(" ELSE ").Format(caseConditional.ElseResultExpression);
			Append(" END ");
			return this;
		}

		/// <summary>
		/// Formata texto de um <see cref="QueryTerm"/>
		/// </summary>
		/// <param name="queryTerm">Sub query a ser formatada.</param>
		/// <param name="ignoreContainerFormat">Identifica que é para ignorar a formatação de de container para a consulta.</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(QueryTerm queryTerm, bool ignoreContainerFormat = false)
		{
			var queryInfo = queryTerm.QueryInfo;
			return Format(queryInfo, ignoreContainerFormat);
		}

		/// <summary>
		/// Formata o texto de uma <see cref="QueryInfo" />
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <param name="ignoreContainerFormat">Identifica que é para ignorar a formatação de de container para a consulta.</param>
		/// <returns></returns>
		private MySqlQueryParser Format(QueryInfo queryInfo, bool ignoreContainerFormat = false)
		{
			var parser = new MySqlQueryParser(this.Translator, this.TypeSchema) {
				Query = queryInfo,
				Owner = this,
				UseTakeParameter = true
			};
			foreach (var parameter in queryInfo.Parameters)
				if(!this.Query.Parameters.Any(f => f.Name == parameter.Name))
					this.Query.Parameters.Add(parameter);
			if(!ignoreContainerFormat)
				Append('(');
			Append(parser.GetText());
			if(!ignoreContainerFormat)
				Append(')');
			return this;
		}

		/// <summary>
		/// Recupera o nome da coluna.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public override string GetColumnName(Column column)
		{
			EntityInfo entity = (!string.IsNullOrEmpty(column.Owner)) ? GetEntity(column.Owner) : Query.Entities[0];
			ITranslatedName name = null;
			if(_columnNameNoCheckMonitor.Busy && string.IsNullOrEmpty(column.Owner) && !Translator.TryGetName(entity, column.Name, Query.IgnoreTypeSchema, out name))
			{
				return QuoteExpression(column.Name);
			}
			var result = new StringBuilder();
			result.Append(QuoteExpression(entity.Alias)).Append('.');
			if(!string.IsNullOrEmpty(entity.FullName))
				result.Append(GetTranslatedName(Translator.GetName(entity, column.Name, Query.IgnoreTypeSchema)));
			else
				result.Append(QuoteExpression(column.Name));
			return result.ToString();
		}

		/// <summary>
		/// Formata o texto de um <see cref="Column"/>
		/// </summary>
		/// <param name="column">coluna</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(Column column)
		{
			Append(GetColumnName(column));
			return this;
		}

		/// <summary>
		/// Formata a formula.
		/// </summary>
		/// <param name="formula"></param>
		/// <returns></returns>
		private MySqlQueryParser Format(Formula formula)
		{
			var partsEnumerator = formula.Parts.GetEnumerator();
			var operatorsEnumerator = formula.Operators.GetEnumerator();
			while (partsEnumerator.MoveNext())
			{
				Format(partsEnumerator.Current);
				if(operatorsEnumerator.MoveNext())
				{
					switch(operatorsEnumerator.Current)
					{
					case MathematicalOperator.Addition:
						Append(" + ");
						break;
					case MathematicalOperator.Division:
						Append(" / ");
						break;
					case MathematicalOperator.Exponentiation:
						Append(" ^ ");
						break;
					case MathematicalOperator.Module:
						Append(" % ");
						break;
					case MathematicalOperator.Multiplication:
						Append(" * ");
						break;
					case MathematicalOperator.Subtraction:
						Append(" - ");
						break;
					}
				}
			}
			return this;
		}

		/// <summary>
		/// Formata o texto de uma <see cref="Conditional"/>
		/// </summary>
		/// <param name="conditional">Condicional a ser formatada</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(Conditional conditional)
		{
			Format((ConditionalTerm)conditional.Left);
			if(conditional.Operator != null && (StringComparer.InvariantCultureIgnoreCase.Equals(conditional.Operator.Op, "EXISTS") || StringComparer.InvariantCultureIgnoreCase.Equals(conditional.Operator.Op, "NOT EXISTS")))
				return this;
			var sqlOperator = GetSqlOperator(conditional.Operator.Op);
			if(conditional.Right is Variable && (sqlOperator == "=" || sqlOperator == "<>"))
			{
				var variable = (Variable)conditional.Right;
				var parameter = Query != null ? Query.Parameters.Where(f => f.Name == variable.Name).FirstOrDefault() : null;
				if(parameter != null && parameter.Value == null)
				{
					if(sqlOperator == "=")
						Append(" IS NULL");
					else
						Append(" IS NOT NULL");
					return this;
				}
			}
			if(!string.IsNullOrEmpty(conditional.Operator.Op))
				Append(' ').AppendSqlOperator(conditional.Operator.Op).Append(' ');
			else
				Append(' ');
			if(conditional.Right != null)
				Format((ConditionalTerm)conditional.Right);
			return this;
		}

		/// <summary>
		/// Formata o texto de um <see cref="ConditionalContainer"/>
		/// </summary>
		/// <param name="container">Container a ser formatado</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(ConditionalContainer container)
		{
			int counter = 1;
			Append("(");
			var listLogicalOperators = container.LogicalOperators.ToList();
			foreach (var conditionalItem in container.Conditionals)
			{
				Format(conditionalItem);
				if(counter != container.ConditionalsCount)
				{
					if(listLogicalOperators[counter - 1] == LogicalOperator.And)
						Append(" AND ");
					else
						Append(" OR ");
					counter++;
				}
				else
					Append(' ');
			}
			Append(")");
			return this;
		}

		/// <summary>
		/// Formata o texto de um <see cref="GroupByEntry"/>
		/// </summary>
		/// <param name="groupBy">Entrada do group by a ser formatada</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(GroupByEntry groupBy)
		{
			if(groupBy.Term == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_FoundEmptyGroupByEntry).Format());
			return Format(groupBy.Term);
		}

		/// <summary>
		/// Formata o texto de um <see cref="SortEntry"/>
		/// </summary>
		/// <param name="sort">Coluna de sort a ser formatada</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Format(SortEntry sort)
		{
			_columnNameNoCheckMonitor.Enter();
			using (_columnNameNoCheckMonitor)
				Format(sort.Term);
			if(sort.Reverse)
				Append(" DESC");
			else
				Append(" ASC");
			return this;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="character">Caracter a se anexada</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser Append(char character)
		{
			_sqlQuery.Append(character);
			return this;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="text">String a ser anexada</param>
		/// <returns>Retorna o próprio objeto</returns>
		protected MySqlQueryParser Append(string text)
		{
			_sqlQuery.Append(text);
			return this;
		}

		/// <summary>
		/// Recuper ao nome traduzido.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private string GetTranslatedName(ITranslatedName name)
		{
			if(name is TranslatedTableName)
			{
				var tableName = (TranslatedTableName)name;
				return QuoteExpression(tableName.Name);
			}
			else if(name is TranslatedColumnName)
			{
				var columnName = (TranslatedColumnName)name;
				return QuoteExpression(columnName.Name);
			}
			else if(name is TranslatedSelectPart)
			{
				return name.ToString();
			}
			return null;
		}

		/// <summary>
		/// Adiciona a tradução do nome.
		/// </summary>
		/// <param name="name">Nome a ser adiocionado</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser AppendTranslatedName(ITranslatedName name)
		{
			var result = GetTranslatedName(name);
			if(result != null)
				Append(result);
			return this;
		}

		/// <summary>
		/// Adiciona a palavra recebida entre colchetes
		/// </summary>
		/// <param name="word">Palavra a ser adicionada</param>
		/// <returns>Retorna o próprio objeto</returns>
		protected virtual MySqlQueryParser AppendQuoteExpression(string word)
		{
			return Append('`').Append(word).Append('`');
		}

		/// <summary>
		/// Adiciona a palavra recebida entre colchetes.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		protected virtual string QuoteExpression(string word)
		{
			return string.Format("`{0}`", word);
		}

		/// <summary>
		/// Receve um operador c# e adiciona um sql
		/// </summary>
		/// <param name="operatorString">operador c#</param>
		/// <returns>Retorna o próprio objeto</returns>
		private MySqlQueryParser AppendSqlOperator(string operatorString)
		{
			Append(GetSqlOperator(operatorString));
			return this;
		}

		/// <summary>
		/// Recupera o operador do SQL.
		/// </summary>
		/// <param name="operatorString"></param>
		/// <returns></returns>
		private string GetSqlOperator(string operatorString)
		{
			switch(operatorString)
			{
			case "==":
				return "=";
			case "!=":
				return "<>";
			}
			if(String.Compare("like", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				return "LIKE";
			else if(String.Compare("not like", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				return "NOT LIKE";
			else
				return operatorString;
		}
	}
}
