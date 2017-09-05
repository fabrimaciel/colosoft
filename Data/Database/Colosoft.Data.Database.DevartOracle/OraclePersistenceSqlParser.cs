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

using Colosoft.Data.Schema;
using Colosoft.Query;
using Colosoft.Query.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Data.Database.Oracle
{
	/// <summary>
	/// Implementação do Parser para oracle
	/// </summary>
	public class OraclePersistenceSqlParser : PersistenceSqlParser
	{
		private ITakeParametersParser _takeParametersParser;

		/// <summary>
		/// Tipo de operação.
		/// </summary>
		public ITakeParametersParser TakeParametersParser
		{
			get
			{
				return _takeParametersParser;
			}
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="translator">Interface de mapeamento de entidade-tabela no banco de dados</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		/// <param name="takeParametersParser">Parser dos parametros take.</param>
		public OraclePersistenceSqlParser(IQueryTranslator translator, ITypeSchema typeSchema, ITakeParametersParser takeParametersParser) : base(translator, typeSchema)
		{
			_takeParametersParser = takeParametersParser;
		}

		/// <summary>
		/// Método que retorna o texto do comando sql a ser executado
		/// </summary>
		/// <returns>Retorna o texto do comando sql a ser executado</returns>
		public override string GetPersistenceCommandText()
		{
			switch(Action.Type)
			{
			case PersistenceActionType.Insert:
				return InsertParser();
			case PersistenceActionType.Update:
				return UpdateParser();
			case PersistenceActionType.Delete:
				return DeleteParser();
			default:
				throw new NotSupportedException(ResourceMessageFormatter.Create(() => Database.Properties.Resources.NotSupportedException_TypeOfConditionalTermNotSupported, Enum.GetName(typeof(PersistenceActionType), Action.Type)).Format());
			}
		}

		/// <summary>
		/// Método responsável por gerar comandos de inserção
		/// </summary>
		private string InsertParser()
		{
			var sqlCommand = new StringBuilder();
			var propertyPart = new StringBuilder();
			var valuePart = new StringBuilder();
			StringBuilder generateIdentityPart = null;
			EntityInfo info = GetEntityInfo(Action.EntityFullName);
			var typeMetadata = TypeSchema.GetTypeMetadata(Action.EntityFullName);
			sqlCommand.AppendLine("BEGIN");
			bool isFirst = true;
			bool isPosCommand = PrimaryKeyRepository.IsPosCommand(typeMetadata.FullName);
			for(int i = 0; i < Action.Parameters.Count; i++)
			{
				var propertyMetadata = typeMetadata[Action.Parameters[i].Name];
				SetParameterType(propertyMetadata, Action.Parameters[i]);
				if(propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey)
				{
					if(isPosCommand)
					{
						generateIdentityPart = new StringBuilder();
						Append("SELECT ", generateIdentityPart).AppendQuoteExpression(typeMetadata.TableName.Schema, generateIdentityPart).Append(".\"", generateIdentityPart).Append(typeMetadata.TableName.Name, generateIdentityPart).Append("_seq\".nextval INTO ?", generateIdentityPart).Append(Action.Parameters[i].Name, generateIdentityPart).Append(" FROM dual;", generateIdentityPart);
					}
				}
				if(!isFirst)
				{
					Append(',', valuePart);
					Append(',', propertyPart);
				}
				else
					isFirst = false;
				var translatedName = Translator.GetName(info, propertyMetadata.Name);
				if(translatedName is TranslatedColumnName)
					AppendTranslatedName(translatedName, propertyPart);
				else if(propertyMetadata.Name == Query.DataAccessConstants.RowVersionPropertyName)
					AppendQuoteExpression(Query.DataAccessConstants.RowVersionColumnName.ToUpper(), propertyPart);
				else
					throw new InvalidOperationException("Expected ColumnName for property \"" + propertyMetadata.Name + "\"");
				Append('?', valuePart).Append(propertyMetadata.ColumnName, valuePart);
			}
			Append(')', valuePart);
			Append(')', propertyPart);
			if(generateIdentityPart != null)
				sqlCommand.AppendLine(generateIdentityPart.ToString());
			Append("INSERT INTO ", sqlCommand).AppendTranslatedName(Translator.GetName(info), sqlCommand).Append(" (", sqlCommand).Append(propertyPart, sqlCommand).Append(" VALUES (", sqlCommand).Append(valuePart, sqlCommand);
			sqlCommand.AppendLine(";");
			AppendGetRowsAffectedExpression(sqlCommand);
			sqlCommand.AppendLine("END;");
			return sqlCommand.ToString();
		}

		/// <summary>
		/// Método responsável por gerar comandos de atualização
		/// </summary>
		private string UpdateParser()
		{
			var sqlCommand = new StringBuilder();
			var typeMetadata = TypeSchema.GetTypeMetadata(Action.EntityFullName);
			var keyProperties = typeMetadata.GetKeyProperties();
			var wherePart = new StringBuilder();
			var setPart = new StringBuilder();
			EntityInfo info = GetEntityInfo(Action.EntityFullName);
			sqlCommand.AppendLine("BEGIN");
			Append("UPDATE ", sqlCommand).AppendTranslatedName(Translator.GetName(info), sqlCommand).Append(" SET ", sqlCommand);
			bool isFirstWhere = true;
			bool isFirstSet = true;
			Tuple<int, IPropertyMetadata> identityKeyIndex = null;
			var keysIndex = new List<Tuple<int, IPropertyMetadata>>();
			for(int i = 0; i < Action.Parameters.Count; i++)
			{
				var parameter = Action.Parameters[i];
				if(string.IsNullOrEmpty(parameter.Name) || parameter.Name.StartsWith("?"))
					continue;
				var propertyMetadata = typeMetadata[parameter.Name];
				if(propertyMetadata == null)
					throw new Exception(string.Format("Property {0} not found", parameter.Name));
				var translatedName = Translator.GetName(info, propertyMetadata.Name);
				SetParameterType(propertyMetadata, parameter);
				if(propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey || propertyMetadata.ParameterType == PersistenceParameterType.Key)
				{
					if(Action.Conditional != null)
						continue;
					if(propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey)
						identityKeyIndex = new Tuple<int, IPropertyMetadata>(i, propertyMetadata);
					else if(propertyMetadata.ParameterType == PersistenceParameterType.Key)
						keysIndex.Add(new Tuple<int, IPropertyMetadata>(i, propertyMetadata));
					if(!isFirstWhere)
						Append(" AND ", wherePart);
					else
						isFirstWhere = false;
					if(translatedName is TranslatedColumnName)
					{
						if(!string.IsNullOrEmpty(info.Alias))
							AppendQuoteExpression(info.Alias, sqlCommand).Append('.', sqlCommand);
						AppendTranslatedName(translatedName, wherePart);
					}
					else if(propertyMetadata.Name == Query.DataAccessConstants.RowVersionPropertyName)
						AppendQuoteExpression(Query.DataAccessConstants.RowVersionColumnName, wherePart);
					else
						throw new InvalidOperationException("Expected ColumnName for property \"" + propertyMetadata.Name + "\"");
					Append("=?", wherePart).Append(propertyMetadata.ColumnName, wherePart);
					continue;
				}
				if(!isFirstSet)
					Append(',', setPart);
				else
					isFirstSet = false;
				if(translatedName is TranslatedColumnName)
				{
					if(!string.IsNullOrEmpty(info.Alias))
						AppendQuoteExpression(info.Alias, sqlCommand).Append('.', sqlCommand);
					AppendTranslatedName(translatedName, setPart);
				}
				else if(propertyMetadata.Name == Query.DataAccessConstants.RowVersionPropertyName)
					AppendQuoteExpression(Query.DataAccessConstants.RowVersionColumnName, setPart);
				else
					throw new InvalidOperationException("Expected ColumnName for property \"" + propertyMetadata.Name + "\"");
				Append("=", setPart);
				if(parameter.Value is PropertyReference)
				{
					var propertyReference = (PropertyReference)parameter.Value;
					var propertyTranslateName = Translator.GetName(info, propertyReference.PropertyName);
					if(translatedName is TranslatedColumnName)
						AppendTranslatedName(translatedName, setPart);
					else
						throw new InvalidOperationException("Expected ColumnName for property \"" + propertyReference.PropertyName + "\"");
				}
				else if(parameter.Value is ExpressionParameter)
				{
					var expressionParameter = (ExpressionParameter)parameter.Value;
					var expressionContainer = Colosoft.Query.ConditionalContainer.Parse(expressionParameter.Expression, expressionParameter.Parameters.Select(f => new QueryParameter(f.Name, f.Value)).ToArray());
					Format(expressionContainer, setPart);
				}
				else if(parameter.Value is Colosoft.Query.Queryable)
				{
					var queryInfo = ((Colosoft.Query.Queryable)parameter.Value).CreateQueryInfo();
					Format(queryInfo, setPart);
				}
				else if(parameter.Value is QueryInfo)
				{
					var queryInfo = (QueryInfo)parameter.Value;
					Format(queryInfo, setPart);
				}
				else
					Append("?", setPart).Append(propertyMetadata.ColumnName, setPart);
			}
			if(Action.Conditional != null)
				Format(Action.Conditional, wherePart);
			else if(Action.Query != null)
				Format(Action.Query.WhereClause, wherePart);
			Append(' ', setPart);
			Append(setPart, sqlCommand).Append(" WHERE ", sqlCommand).Append(wherePart, sqlCommand);
			if(Action.Conditional == null && Action.Query == null)
			{
				if(identityKeyIndex != null)
				{
					keysIndex.Clear();
					keysIndex.Add(identityKeyIndex);
				}
				if(typeMetadata.IsVersioned && keysIndex.Count > 0)
					AppendVerifyRowVersionExpression(sqlCommand);
			}
			sqlCommand.AppendLine(";");
			AppendGetRowsAffectedExpression(sqlCommand);
			sqlCommand.AppendLine("END;");
			return sqlCommand.ToString();
		}

		/// <summary>
		/// Método responsável por gerar comandos de deleção
		/// </summary>
		private string DeleteParser()
		{
			var sqlCommand = new StringBuilder();
			var typeMetadata = TypeSchema.GetTypeMetadata(Action.EntityFullName);
			EntityInfo info = GetEntityInfo(Action.EntityFullName);
			Append("DELETE FROM ", sqlCommand).AppendTranslatedName(Translator.GetName(info), sqlCommand).Append(" WHERE ", sqlCommand);
			if(Action.Conditional == null)
			{
				bool isFirst = true;
				for(int i = 0; i < Action.Parameters.Count; i++)
				{
					if(!isFirst)
						Append(" AND ", sqlCommand);
					else
						isFirst = false;
					var propertyMetadata = typeMetadata[Action.Parameters[i].Name];
					if(propertyMetadata == null)
						throw new Exception(string.Format("Property {0} not found", Action.Parameters[i].Name));
					AppendTranslatedName(Translator.GetName(info, Action.Parameters[i].Name), sqlCommand).Append("=?", sqlCommand).Append(propertyMetadata.ColumnName, sqlCommand);
				}
				if(typeMetadata.IsVersioned)
					AppendVerifyRowVersionExpression(sqlCommand);
			}
			else
				Format(Action.Conditional, sqlCommand);
			return sqlCommand.ToString();
		}

		/// <summary>
		/// Formata o texto de uma <see cref="QueryInfo" />
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <param name="sqlCommand"></param>
		/// <returns></returns>
		private OraclePersistenceSqlParser Format(QueryInfo queryInfo, StringBuilder sqlCommand)
		{
			var parser = new Colosoft.Query.Database.Oracle.OracleQueryParser(this.Translator, this.TypeSchema, this.TakeParametersParser);
			parser.Query = queryInfo;
			foreach (var parameter in Query.QueryParameter.GetParameters(queryInfo, true))
				if(!this.Action.Parameters.Any(f => f.Name == parameter.Name))
					this.Action.Parameters.Add(new PersistenceParameter(parameter.Name, parameter.Value, parameter.Direction));
			Append('(', sqlCommand).Append(parser.GetText(), sqlCommand).Append(')', sqlCommand);
			return this;
		}

		/// <summary>
		/// Formata texto de um <see cref="QueryTerm"/>
		/// </summary>
		/// <param name="queryTerm">Sub query a ser formatada.</param>
		/// <param name="sqlCommand"></param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Format(QueryTerm queryTerm, StringBuilder sqlCommand)
		{
			var queryInfo = queryTerm.QueryInfo;
			return Format(queryInfo, sqlCommand);
		}

		/// <summary>
		/// Formata texto de um <see cref="ConditionalTerm"/>
		/// </summary>
		/// <param name="conditionalTerm">Termo condicional</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Format(ConditionalTerm conditionalTerm, StringBuilder sqlCommand)
		{
			if(conditionalTerm is Constant)
				Append(((Constant)conditionalTerm).Text, sqlCommand);
			else if(conditionalTerm is Column)
				Format((Column)conditionalTerm, sqlCommand);
			else if(conditionalTerm is Variable)
			{
				var variable = (Variable)conditionalTerm;
				var parameter = Action != null ? Action.Parameters.Where(f => f.Name == variable.Name).FirstOrDefault() : null;
				Colosoft.Query.QueryParameter queryParameter = null;
				if(parameter == null && Action != null && Action.Conditional != null)
				{
					queryParameter = Action.Conditional.ParameterContainer.FirstOrDefault(f => f.Name == variable.Name);
					if(queryParameter != null)
						parameter = new PersistenceParameter(queryParameter.Name, queryParameter.Value);
				}
				if(parameter != null && (parameter.Value is QueryInfo || parameter.Value is Colosoft.Query.Queryable))
				{
					if(queryParameter == null)
						this.Action.Parameters.Remove(parameter);
					else if(Action.Conditional.ParameterContainer is Colosoft.Query.IQueryParameterContainerExt)
						((Colosoft.Query.IQueryParameterContainerExt)Action.Conditional.ParameterContainer).Remove(queryParameter);
					var value = parameter.Value;
					if(value is Colosoft.Query.Queryable)
						value = ((Colosoft.Query.Queryable)value).CreateQueryInfo();
					Format(new QueryTerm((QueryInfo)value), sqlCommand);
				}
				else
					Append(((Variable)conditionalTerm).Name, sqlCommand);
			}
			else if(conditionalTerm is ValuesArray)
			{
				var values = (ValuesArray)conditionalTerm;
				if(values.Values != null && values.Values.Length == 1 && values.Values[0] is Variable)
				{
					var variable = (Variable)values.Values[0];
					var parameter = Action != null ? Action.Parameters.Where(f => f.Name == variable.Name).FirstOrDefault() : null;
					Colosoft.Query.QueryParameter queryParameter = null;
					if(parameter == null && Action != null && Action.Conditional != null)
					{
						queryParameter = Action.Conditional.ParameterContainer.FirstOrDefault(f => f.Name == variable.Name);
						if(queryParameter != null)
							parameter = new PersistenceParameter(queryParameter.Name, queryParameter.Value);
					}
					if(parameter != null && (parameter.Value is QueryInfo || parameter.Value is Colosoft.Query.Queryable))
					{
						if(queryParameter == null)
							this.Action.Parameters.Remove(parameter);
						else if(Action.Conditional.ParameterContainer is Colosoft.Query.IQueryParameterContainerExt)
							((Colosoft.Query.IQueryParameterContainerExt)Action.Conditional.ParameterContainer).Remove(queryParameter);
						var value = parameter.Value;
						if(value is Colosoft.Query.Queryable)
							value = ((Colosoft.Query.Queryable)value).CreateQueryInfo();
						Format(new QueryTerm((QueryInfo)value), sqlCommand);
						values = null;
					}
				}
				if(values != null)
					Append(conditionalTerm.ToString(), sqlCommand);
			}
			else if(conditionalTerm is ConditionalContainer)
				Format((ConditionalContainer)conditionalTerm, sqlCommand);
			else if(conditionalTerm is QueryTerm)
				Format((QueryTerm)conditionalTerm, sqlCommand);
			else if(conditionalTerm is FunctionCall)
			{
				var f = (FunctionCall)conditionalTerm;
				var name = f.Call.ToString().Replace("'", String.Empty);
				Append(name, sqlCommand);
				Append('(', sqlCommand);
				var gone = false;
				foreach (var paramTerm in f.Parameters)
				{
					if(gone)
						Append(", ", sqlCommand);
					else
						gone = true;
					if(paramTerm != null)
						Format(paramTerm, sqlCommand);
					else
						Append("NULL", sqlCommand);
				}
				Append(')', sqlCommand);
			}
			else if(conditionalTerm is Formula)
				Format((Formula)conditionalTerm, sqlCommand);
			else if(conditionalTerm is MinusTerm)
			{
				sqlCommand.Append("-");
				Format(((MinusTerm)conditionalTerm).Term, sqlCommand);
			}
			else if(conditionalTerm is Conditional)
				Format((Conditional)conditionalTerm, sqlCommand);
			else
				throw new NotSupportedException(ResourceMessageFormatter.Create(() => Database.Properties.Resources.NotSupportedException_TypeOfConditionalTermNotSupported, conditionalTerm.GetType().ToString()).Format());
			return this;
		}

		/// <summary>
		/// Formata o texto de um <see cref="Column"/>
		/// </summary>
		/// <param name="column">coluna</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Format(Column column, StringBuilder sqlCommand)
		{
			AppendTranslatedName(Translator.GetName(GetEntityInfo(Action.EntityFullName), column.Name), sqlCommand);
			return this;
		}

		/// <summary>
		/// Formata o texto de uma <see cref="Conditional"/>
		/// </summary>
		/// <param name="conditional">Condicional a ser formatada</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Format(Conditional conditional, StringBuilder sqlCommand)
		{
			Format((ConditionalTerm)conditional.Left, sqlCommand);
			Append(' ', sqlCommand).AppendSqlOperator(conditional.Operator.Op, sqlCommand).Append(' ', sqlCommand);
			Format((ConditionalTerm)conditional.Right, sqlCommand);
			return this;
		}

		/// <summary>
		/// Formata o texto de um <see cref="ConditionalContainer"/>
		/// </summary>
		/// <param name="container">Container a ser formatado</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Format(ConditionalContainer container, StringBuilder sqlCommand)
		{
			int counter = 1;
			var listLogicalOperators = container.LogicalOperators.ToList();
			foreach (var conditionalItem in container.Conditionals)
			{
				if(conditionalItem is ConditionalContainer)
				{
					var containerItem = (ConditionalContainer)conditionalItem;
					Append('(', sqlCommand).Format(containerItem, sqlCommand).Append(')', sqlCommand);
				}
				else
				{
					var conditional = (Conditional)conditionalItem;
					Format(conditional, sqlCommand);
				}
				if(counter != container.ConditionalsCount)
				{
					if(listLogicalOperators[counter - 1] == LogicalOperator.And)
						Append(" AND ", sqlCommand);
					else
						Append(" OR ", sqlCommand);
					counter++;
				}
				else
					Append(' ', sqlCommand);
			}
			return this;
		}

		/// <summary>
		/// Define o tipo do parâmetero.
		/// </summary>
		/// <param name="propertyMetadata">Metadados da propriedade.</param>
		/// <param name="parameter">Parâmetro de persistência.</param>
		protected override void SetParameterType(IPropertyMetadata propertyMetadata, PersistenceParameter parameter)
		{
			if(StringComparer.InvariantCultureIgnoreCase.Equals(propertyMetadata.PropertyType, "System.Boolean"))
			{
				parameter.DbType = System.Data.DbType.Int16;
				if((bool)parameter.Value)
					parameter.Value = 1;
				else
					parameter.Value = 0;
			}
			base.SetParameterType(propertyMetadata, parameter);
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="character">Caracter a ser adicionado</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Append(char character, StringBuilder sqlCommand)
		{
			sqlCommand.Append(character);
			sqlCommand.Append(new StringBuilder());
			return this;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="text">Texto a ser adicionado</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Append(string text, StringBuilder sqlCommand)
		{
			sqlCommand.Append(text);
			return this;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="text"><see cref="StringBuilder"/> a ser adicinado</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser Append(StringBuilder text, StringBuilder sqlCommand)
		{
			sqlCommand.Append(text);
			return this;
		}

		/// <summary>
		/// Adiciona a a tradução do nome.
		/// </summary>
		/// <param name="name">Nome a ser traduzido</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser AppendTranslatedName(ITranslatedName name, StringBuilder sqlCommand)
		{
			if(name is TranslatedTableName)
			{
				var tableName = (TranslatedTableName)name;
				if(!string.IsNullOrEmpty(tableName.Schema))
					AppendQuoteExpression(tableName.Schema, sqlCommand).Append(".", sqlCommand);
				AppendQuoteExpression(tableName.Name, sqlCommand);
			}
			else if(name is TranslatedColumnName)
			{
				var columnName = (TranslatedColumnName)name;
				AppendQuoteExpression(columnName.Name, sqlCommand);
			}
			else
			{
				Append(name.ToString(), sqlCommand);
			}
			return this;
		}

		/// <summary>
		/// Adiciona a palvra recebida entre colchetes
		/// </summary>
		/// <param name="word">Palavra a ser adicionada</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser AppendQuoteExpression(string word, StringBuilder sqlCommand)
		{
			return Append('"', sqlCommand).Append(word, sqlCommand).Append('"', sqlCommand);
		}

		/// <summary>
		/// Receve um operador c# e adiciona um sql
		/// </summary>
		/// <param name="operatorString">operador c#</param>
		/// <param name="sqlCommand">Objeto <see cref="StringBuilder"/> no qual será adicionado texto</param>
		/// <returns>Retorna o próprio objeto</returns>
		private OraclePersistenceSqlParser AppendSqlOperator(string operatorString, StringBuilder sqlCommand)
		{
			switch(operatorString)
			{
			case "==":
				Append('=', sqlCommand);
				return this;
			case "!=":
				Append("<>", sqlCommand);
				return this;
			}
			if(String.Compare("like", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				Append("LIKE", sqlCommand);
			else
				Append(operatorString, sqlCommand);
			return this;
		}

		/// <summary>
		/// Anexa o comando usado para recuepra a quantidade de linhas afetadas.
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <returns></returns>
		private OraclePersistenceSqlParser AppendGetRowsAffectedExpression(StringBuilder sqlCommand)
		{
			sqlCommand.Append("?").Append(DataAccessConstants.AffectedRowsParameterName).AppendLine(":=sql%rowcount;");
			return this;
		}

		/// <summary>
		/// Anexa no comando a verificação do RowVersion.
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <returns></returns>
		private OraclePersistenceSqlParser AppendVerifyRowVersionExpression(StringBuilder sqlCommand)
		{
			Append(" AND CAST(ORA_ROWSCN AS NUMBER(18,0))=?", sqlCommand).Append(DataAccessConstants.RowVersionPropertyName, sqlCommand);
			return this;
		}
	}
}
