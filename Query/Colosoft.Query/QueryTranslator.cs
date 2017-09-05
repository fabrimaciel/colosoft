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
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Colosoft.Query.Linq.Translators
{
	/// <summary>
	/// Possíveis formas de resultado.
	/// </summary>
	internal enum ResultShape
	{
		None,
		Singleton,
		Sequence
	}
	internal partial class QueryTranslator : ExpressionVisitor
	{
		private readonly OrderByTranslator _orderByTranslator;

		private WhereTranslator _whereTranslator;

		private ProjectionTranslator _selectTranslator;

		private TakeTranslator _takeTranslator;

		private SkipTranslator _skipTranslator;

		private QueryMethod _useMethod = QueryMethod.Select;

		private bool _longCount;

		/// <summary>
		/// Lista dos parametro que serão usados na consulta.
		/// </summary>
		private QueryParameterCollection _parameters;

		/// <summary>
		/// Lista dos parametro que serão usados na consulta.
		/// </summary>
		public QueryParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Método de consulta.
		/// </summary>
		public QueryMethod UseMethod
		{
			get
			{
				return _useMethod;
			}
		}

		/// <summary>
		/// Identiica se é para usar long para count.
		/// </summary>
		public bool LongCount
		{
			get
			{
				return _longCount;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal QueryTranslator()
		{
			_orderByTranslator = new OrderByTranslator();
			_parameters = new QueryParameterCollection();
		}

		/// <summary>
		/// Traduz a consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		internal QueryInfo Translate(Expression query)
		{
			Visit(query);
			return ConvertToExecutableQuery(query);
		}

		/// <summary>
		/// Aplica os parametros da expressão para a consulta informada.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="expression"></param>
		public void Apply(Query.Queryable queryable, Expression expression)
		{
			Visit(expression);
			if(_takeTranslator != null)
				queryable.Take(_takeTranslator.Count.GetValueOrDefault());
			if(_skipTranslator != null)
				queryable.Skip(_skipTranslator.Skip.GetValueOrDefault());
			if(_whereTranslator != null)
			{
				string where = _whereTranslator.WhereClause;
				if(!string.IsNullOrEmpty(where))
					queryable.WhereClause.And(ConditionalContainer.Parse(where));
			}
			foreach (var i in _parameters)
				queryable.Add(i);
			if(_orderByTranslator != null && _orderByTranslator.Members.Any())
			{
				foreach (var i in _orderByTranslator.Members)
				{
					queryable.Sort = new Sort();
					queryable.Sort.Add(new SortEntry(new Column(i.DataMember.Name), i.Direction == OrderDirection.Descending));
				}
			}
			switch(_useMethod)
			{
			case QueryMethod.Count:
				queryable.Count();
				break;
			case QueryMethod.Sum:
			{
				if(_selectTranslator == null || !_selectTranslator.DataMembers.Any())
				{
				}
				break;
			}
			}
		}

		/// <summary>
		/// Converte a expressão da consulta com uma consulta executável.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private QueryInfo ConvertToExecutableQuery(Expression query)
		{
			Type source;
			if(!GetSourceEntity(query, out source))
				throw new NotSupportedException("This query expression is not supported!");
			var queryDetails = new QueryInfo() {
				Method = _useMethod
			};
			if(_takeTranslator != null)
				queryDetails.TakeParameters = new TakeParameters(_takeTranslator.Count.GetValueOrDefault(), _skipTranslator != null ? _skipTranslator.Skip.GetValueOrDefault() : 0);
			if(_selectTranslator == null || !_selectTranslator.DataMembers.Any())
			{
				_selectTranslator = new ProjectionTranslator(source.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(f => new Member(null, f)));
			}
			if(!_selectTranslator.DataMembers.Any())
				throw new Exception("There are no items for projection in this query!");
			var projection = new Projection();
			foreach (var i in _selectTranslator.DataMembers)
				projection.Add(new ProjectionEntry(!string.IsNullOrEmpty(i.Owner) ? string.Format("{0}.{1}", i.Owner, i.Info.Name) : i.Info.Name, i.Info.Name));
			queryDetails.Projection = projection;
			IEnumerable<Member> entities = _selectTranslator.DataMembers;
			if(_whereTranslator != null)
			{
				string where = _whereTranslator.WhereClause;
				if(!string.IsNullOrEmpty(where))
					queryDetails.WhereClause = ConditionalContainer.Parse(where);
			}
			else
				queryDetails.WhereClause = new ConditionalContainer().Add(_parameters);
			var rr = entities.GroupBy(f => f.Owner, f => f).ToArray();
			if(_orderByTranslator != null && _orderByTranslator.Members.Any())
			{
				foreach (var i in _orderByTranslator.Members)
				{
					if(queryDetails.Sort == null)
						queryDetails.Sort = new Sort();
					queryDetails.Sort.Add(new SortEntry(new Column(i.DataMember.Name), i.Direction == OrderDirection.Descending));
				}
			}
			return queryDetails;
		}

		/// <summary>
		/// O que essa consulta retorna?
		/// - um entidade unica
		/// - uma sequencia de entidades
		/// - ou nada
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private ResultShape GetResultShape(Expression query)
		{
			LambdaExpression lambda = query as LambdaExpression;
			if(lambda != null)
				query = lambda.Body;
			if(query.Type == typeof(void))
				return ResultShape.None;
			MethodCallExpression methodExp = query as MethodCallExpression;
			if(methodExp != null && ((methodExp.Method.DeclaringType == typeof(System.Linq.Queryable)) || (methodExp.Method.DeclaringType == typeof(Enumerable))))
			{
				string str = methodExp.Method.Name;
				if(str != null && (str == "First" || str == "FirstOrDefault" || str == "Single" || str == "SingleOrDefault"))
					return ResultShape.Singleton;
			}
			return ResultShape.Sequence;
		}

		/// <summary>
		/// Recupera os dados da tabela fonte da consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool GetSourceEntity(Expression query, out Type source)
		{
			source = null;
			MethodCallExpression me = query as MethodCallExpression;
			ConstantExpression ce;
			if(me == null)
			{
				ce = query as ConstantExpression;
			}
			else
			{
				while (true)
				{
					if(me.Arguments[0] is MethodCallExpression)
						me = me.Arguments[0] as MethodCallExpression;
					else
						break;
				}
				ce = me.Arguments[0] as ConstantExpression;
			}
			if(ce != null)
			{
				IQueryable entity = ce.Value as IQueryable;
				if(entity != null)
				{
					source = entity.ElementType;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Método acionado toda vez que um método é visitado na arvore de expressões.
		/// </summary>
		/// <param name="mc"></param>
		/// <returns></returns>
		protected override Expression VisitMethodCall(MethodCallExpression mc)
		{
			Type declaringType = mc.Method.DeclaringType;
			if(declaringType != typeof(System.Linq.Queryable))
				throw new NotSupportedException("Invalid Sequence Operator Call. The type for the operator is not Queryable!");
			switch(mc.Method.Name)
			{
			case "Where":
				var whereLambda = GetLambdaWithParamCheck(mc);
				if(whereLambda == null)
					break;
				VisitWhere(whereLambda);
				break;
			case "OrderBy":
			case "ThenBy":
				var orderLambda = GetLambdaWithParamCheck(mc);
				if(orderLambda == null)
					break;
				VisitOrderBy(orderLambda, OrderDirection.Ascending);
				break;
			case "OrderByDescending":
			case "ThenByDescending":
				var orderDescLambda = GetLambdaWithParamCheck(mc);
				if(orderDescLambda == null)
					break;
				VisitOrderBy(orderDescLambda, OrderDirection.Descending);
				break;
			case "Select":
				var selectLambda = GetLambdaWithParamCheck(mc);
				if(selectLambda == null)
					break;
				VisitSelect(selectLambda);
				break;
			case "LongCount":
				_longCount = true;
				_useMethod = QueryMethod.Count;
				break;
			case "Count":
				_useMethod = QueryMethod.Count;
				break;
			case "Sum":
				_useMethod = QueryMethod.Sum;
				VisitSelect(GetLambdaWithParamCheck(mc));
				break;
			case "Take":
				if(mc.Arguments.Count != 2)
					break;
				VisitTake(mc.Arguments[1]);
				break;
			case "Skip":
				if(mc.Arguments.Count != 2)
					break;
				VisitSkip(mc.Arguments[1]);
				break;
			case "First":
				if(mc.Arguments.Count != 1)
					break;
				VisitFirst(false);
				break;
			case "FirstOrDefault":
				if(mc.Arguments.Count != 1)
					break;
				VisitFirst(true);
				break;
			default:
				return base.VisitMethodCall(mc);
			}
			Visit(mc.Arguments[0]);
			return mc;
		}

		/// <summary>
		/// Visita a parte da condicional.
		/// </summary>
		/// <param name="predicate">The lambda expression parameter to the Where extension method</param>
		private void VisitWhere(LambdaExpression predicate)
		{
			if(_whereTranslator != null)
				throw new NotSupportedException("You cannot have more than one Where in the expression");
			_whereTranslator = new WhereTranslator(_parameters);
			_whereTranslator.Translate(predicate);
		}

		/// <param name="predicate">The lambda expression parameter to the Select extension method</param>
		private void VisitSelect(LambdaExpression predicate)
		{
			if(_selectTranslator != null)
				throw new NotSupportedException("You cannot have more than 1 Select in the expression");
			_selectTranslator = new ProjectionTranslator();
			_selectTranslator.Translate(predicate);
		}

		/// <summary>
		/// Realiza a visita a expressão de ordenação.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="direction"></param>
		private void VisitOrderBy(LambdaExpression predicate, OrderDirection direction)
		{
			_orderByTranslator.Visit(predicate, direction);
		}

		private void VisitTake(Expression takeValue)
		{
			if(_takeTranslator != null)
				throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");
			_takeTranslator = new TakeTranslator();
			_takeTranslator.Translate(takeValue);
		}

		private void VisitSkip(Expression skipValue)
		{
			if(_skipTranslator != null)
				throw new NotSupportedException("You cannot have more than 1 Take/Skip/First/FirstOrDefault in the expression");
			_skipTranslator = new SkipTranslator();
			_skipTranslator.Translate(skipValue);
		}

		private void VisitFirst(bool useDefault)
		{
			if(_takeTranslator != null)
				throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");
			_takeTranslator = TakeTranslator.GetNewFirstTranslator(useDefault);
		}

		/// <summary>
		/// Check to see if the expression is valid for
		/// Select, Where, OrderBy, ThenBy, OrderByDescending and ThenByDescending
		/// and then return the lmbda section
		/// </summary>
		/// <returns></returns>
		private LambdaExpression GetLambdaWithParamCheck(MethodCallExpression mc)
		{
			if(mc.Arguments.Count != 2 || !IsLambda(mc.Arguments[1]))
				return null;
			var lambda = GetLambda(mc.Arguments[1]);
			return (lambda.Parameters.Count != 1) ? null : lambda;
		}

		private bool IsLambda(Expression expression)
		{
			return RemoveQuotes(expression).NodeType == ExpressionType.Lambda;
		}

		private LambdaExpression GetLambda(Expression expression)
		{
			return RemoveQuotes(expression) as LambdaExpression;
		}

		private Expression RemoveQuotes(Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression = ((UnaryExpression)expression).Operand;
			}
			return expression;
		}
	}
}
