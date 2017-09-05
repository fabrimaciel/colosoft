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
using Colosoft.Query;
using System.Collections;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe que gera comandos oql.
	/// </summary>
	public class CacheQueryParser
	{
		private StringBuilder _oqlQuery = new StringBuilder();

		private QueryInfo _queryInfo;

		private Hashtable _cacheParameters = new Hashtable();

		private List<QueryParameter> _constantParameters = new List<QueryParameter>();

		private IQueryTranslator _translator;

		/// <summary>
		/// Parâmetros do cache.
		/// </summary>
		public Hashtable CacheParameters
		{
			get
			{
				return _cacheParameters;
			}
			set
			{
				_cacheParameters = value;
			}
		}

		/// <summary>
		/// Tradutor associado.
		/// </summary>
		public IQueryTranslator Translator
		{
			get
			{
				return _translator;
			}
			set
			{
				_translator = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="translator"></param>
		/// <param name="info">Informações da query.</param>
		public CacheQueryParser(IQueryTranslator translator, QueryInfo info)
		{
			translator.Require("translator").NotNull();
			info.Require("info").NotNull();
			_translator = translator;
			_queryInfo = info;
		}

		/// <summary>
		/// Gera e retorna o texto para o comando oql.
		/// </summary>
		/// <returns>Texto do comando oql.</returns>
		public string GetText()
		{
			Append("SELECT ");
			var entity = _queryInfo.Entities[0];
			Append(entity.FullName);
			if(_queryInfo.WhereClause != null && _queryInfo.WhereClause.ConditionalsCount > 0)
				Append(" WHERE ").Format(_queryInfo.WhereClause);
			return _oqlQuery.ToString();
		}

		/// <summary>
		/// Formata texto de um <see cref="ConditionalTerm"/>.
		/// </summary>
		/// <param name="conditionalTerm">Termo condicional.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser Format(ConditionalTerm conditionalTerm)
		{
			if(conditionalTerm is Column)
				Format((Column)conditionalTerm);
			else if(conditionalTerm is Variable)
				Append(((Variable)conditionalTerm).Name);
			else if(conditionalTerm is Constant)
			{
				var constantText = ((Constant)conditionalTerm).Text;
				if(StringComparer.InvariantCultureIgnoreCase.Equals(constantText, "NULL"))
					Append("NULL");
				else
				{
					var constantValue = GetConstantValue(constantText);
					var pName = string.Format("?v{0}", _cacheParameters.Count.ToString("00"));
					_cacheParameters[pName] = constantValue;
					Append(pName);
				}
			}
			else if(conditionalTerm is ValuesArray)
			{
				var valuesArray = (ValuesArray)conditionalTerm;
				Append('(');
				bool isFirst = true;
				;
				foreach (var value in valuesArray.Values)
				{
					if(isFirst)
						isFirst = false;
					else
						Append(',');
					if(value is Constant)
					{
						var pName = string.Format("?v{0}", _cacheParameters.Count.ToString("00"));
						var constantText = ((Constant)value).Text;
						if(StringComparer.InvariantCultureIgnoreCase.Equals(constantText, "NULL"))
							_cacheParameters[pName] = null;
						else
						{
							var constantValue = GetConstantValue(constantText);
							_cacheParameters[pName] = constantValue;
						}
						Append(pName);
					}
					else if(value is Variable)
					{
						var variable = (Variable)value;
						Append(variable.Name);
					}
					else
						throw new NotSupportedException("ValuesArray accept only Constant and parameter");
				}
				Append(')');
			}
			else if(conditionalTerm is ConditionalContainer)
				Format((ConditionalContainer)conditionalTerm);
			else
				throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.NotSupportedException_TypeOfConditionalTermNotSupported, conditionalTerm.GetType().ToString()).Format());
			return this;
		}

		/// <summary>
		/// Formata o texto de um <see cref="Column"/>.
		/// </summary>
		/// <param name="column">Coluna.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser Format(Column column)
		{
			var propertyName = (CacheQueryTranslator.TranslatedPropertyName)Translator.GetName(_queryInfo.Entities[0], column.Name);
			Append("this.").Append(propertyName.Name);
			return this;
		}

		/// <summary>
		/// Formata o texto de uma <see cref="Conditional"/>.
		/// </summary>
		/// <param name="conditional">Condicional a ser formatada.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser Format(Conditional conditional)
		{
			Format(conditional.Left);
			var variable = conditional.Left as Variable;
			var valuesArray = conditional.Left as ValuesArray;
			var constant = conditional.Left as Constant;
			Append(' ').AppendSqlOperator(conditional.Operator.Op).Append(' ');
			Format(conditional.Right);
			if(variable == null)
				variable = conditional.Right as Variable;
			if(valuesArray == null)
				valuesArray = conditional.Right as ValuesArray;
			if(constant == null)
				constant = conditional.Right as Constant;
			IEnumerable<string> parameters = null;
			var constantValues = new List<object>();
			if(variable != null)
				parameters = new string[] {
					variable.Name
				};
			else if(valuesArray != null)
			{
				var parameters2 = new List<string>();
				foreach (var name in valuesArray.Values.Where(p => p is Variable).Select(f => ((Variable)f).Name))
				{
					var value = _queryInfo.Parameters.Where(f => f.Name == name).Select(f => f.Value).FirstOrDefault();
					if(!(value is string) && !(value is byte[]) && value is IEnumerable)
					{
						var enumerable = (IEnumerable)value;
						foreach (var i in enumerable)
							constantValues.Add(i);
					}
					else
						parameters2.Add(name);
				}
				parameters = parameters2;
				constantValues.AddRange(valuesArray.Values.Where(p => p is Constant).Select(f => GetConstantValue(((Constant)f).Text)));
			}
			else if(constant != null)
				constantValues.Add(GetConstantValue(constant.Text));
			string colunmName = null;
			if(conditional.Right is Column)
				colunmName = ((Column)conditional.Right).Name;
			else if(conditional.Left is Column)
				colunmName = ((Column)conditional.Left).Name;
			else
			{
				Column column = null;
				if(conditional.Left is ConditionalContainer)
					column = FindColumn((ConditionalContainer)conditional.Left);
				if(column == null && conditional.Right is ConditionalContainer)
					column = FindColumn((ConditionalContainer)conditional.Right);
				if(column != null)
					colunmName = column.Name;
			}
			if(string.IsNullOrEmpty(colunmName))
				throw new NotImplementedException("Not found column name");
			var arrayList = new ArrayList();
			if(parameters != null)
			{
				foreach (var parameter in parameters)
				{
					var queryParameter = _queryInfo.Parameters.Where(p => p.Name == parameter).FirstOrDefault();
					if(queryParameter == null)
						throw new Exception(string.Format("Not found parameter {0}", parameter));
					arrayList.Add(queryParameter.Value);
				}
				arrayList.AddRange(constantValues);
			}
			if(constantValues.Count > 0)
				arrayList.AddRange(constantValues);
			var baseColumnName = colunmName;
			for(var i = 0; _cacheParameters.Contains(colunmName); i++)
				colunmName = string.Format("{0}{1}", baseColumnName, i.ToString("00"));
			_cacheParameters.Add(colunmName, arrayList);
			return this;
		}

		/// <summary>
		/// Localiza a coluna no container
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private Column FindColumn(ConditionalContainer container)
		{
			foreach (var part in container.Conditionals)
				if(part is Column)
					return (Column)part;
				else if(part is Conditional)
				{
					var conditional = (Conditional)part;
					if(conditional.Left is Column)
						return (Column)conditional.Left;
					if(conditional.Right is Column)
						return (Column)conditional.Right;
					{
						Column column = null;
						if(conditional.Left is ConditionalContainer)
							column = FindColumn((ConditionalContainer)conditional.Left);
						if(column != null)
							return column;
						if(conditional.Right is ConditionalContainer)
							column = FindColumn((ConditionalContainer)conditional.Right);
						if(column != null)
							return column;
					}
				}
				else if(part is ConditionalContainer)
				{
					var column = FindColumn((ConditionalContainer)part);
					if(column != null)
						return column;
				}
			return null;
		}

		/// <summary>
		/// Formata o texto de um <see cref="ConditionalContainer"/>.
		/// </summary>
		/// <param name="container">Container a ser formatado.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser Format(ConditionalContainer container)
		{
			Append('(');
			int counter = 1;
			var listLogicalOperators = container.LogicalOperators.ToList();
			foreach (var conditionalItem in container.Conditionals)
			{
				if(conditionalItem is ConditionalContainer)
				{
					var containerItem = (ConditionalContainer)conditionalItem;
					Format(containerItem);
				}
				else
				{
					var conditional = (Conditional)conditionalItem;
					Format(conditional);
				}
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
			Append(')');
			return this;
		}

		/// <summary>
		/// Substitui os nomes parâmetros do Queryable para parâmetros compatíveis com o cache.
		/// </summary>
		/// <param name="parameters">Nomes dos parâmetros do Queryable a serem substituídos.</param>
		/// <param name="cacheParameterName">Nome do parâmetro para o cache.</param>
		private void ProvideCacheParameter(IEnumerable<string> parameters, string cacheParameterName)
		{
			if(parameters != null)
			{
				foreach (var parameter in parameters)
				{
					var queryParameter = _queryInfo.Parameters.Where(p => p.Name == parameter).FirstOrDefault();
					queryParameter.Name = cacheParameterName;
				}
			}
		}

		/// <summary>
		/// Recupera o valor de uma constante.
		/// </summary>
		/// <param name="constantString">String da constante.</param>
		/// <returns>Valor da constante fornecida.</returns>
		private object GetConstantValue(string constantString)
		{
			object value = null;
			int auxInt;
			double auxDouble;
			if(String.Compare("NULL", constantString, StringComparison.CurrentCultureIgnoreCase) == 0)
				value = null;
			if(constantString.StartsWith("'") && constantString.EndsWith("'"))
				value = constantString.Substring(1, constantString.Length - 1);
			else if(int.TryParse(constantString, out auxInt))
				value = auxInt;
			else if(double.TryParse(constantString, out auxDouble))
				value = auxDouble;
			return value;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="character">Caracter a se anexada</param>
		/// <returns>Retorna o próprio objeto</returns>
		private CacheQueryParser Append(char character)
		{
			_oqlQuery.Append(character);
			return this;
		}

		/// <summary>
		/// Anexa o texto a composição da consulta.
		/// </summary>
		/// <param name="text">String a ser anexada.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser Append(string text)
		{
			_oqlQuery.Append(text);
			return this;
		}

		/// <summary>
		/// Receve um operador c# e adiciona um oql.
		/// </summary>
		/// <param name="operatorString">Operador c#.</param>
		/// <returns>Retorna o próprio objeto.</returns>
		private CacheQueryParser AppendSqlOperator(string operatorString)
		{
			switch(operatorString)
			{
			case "==":
				_oqlQuery.Append("==");
				return this;
			case "!=":
				_oqlQuery.Append("!=");
				return this;
			}
			if(String.Compare("like", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				_oqlQuery.Append("LIKE");
			else if(String.Compare("not like", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				_oqlQuery.Append("NOT LIKE");
			else if(String.Compare("IS", operatorString, StringComparison.CurrentCultureIgnoreCase) == 0)
				_oqlQuery.Append("==");
			else
				_oqlQuery.Append(operatorString);
			return this;
		}
	}
}
