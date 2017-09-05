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
using System.Linq.Expressions;

namespace Colosoft.Query.Linq
{
	/// <summary>
	/// Implementação do provedor de consulta.
	/// </summary>
	public class QueryProvider : System.Linq.IQueryProvider
	{
		private ILinqSourceContext _sourceContext;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sourceContext"></param>
		public QueryProvider(ILinqSourceContext sourceContext)
		{
			_sourceContext = sourceContext;
		}

		/// <summary>
		/// Cria uma instancia do provedor para a consulta informada.
		/// </summary>
		/// <param name="queryable"></param>
		/// <returns></returns>
		public static IQueryProvider Create<T>(Queryable queryable)
		{
			queryable.Require("queryable").NotNull();
			return Create<T>(queryable, null);
		}

		/// <summary>
		/// Cria uma instancia do provedor para a consulta informada.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="executeSelect">Método acionado quando for executada uma seleção de valores da consulta.</param>
		/// <returns></returns>
		public static IQueryProvider Create<T>(Queryable queryable, Func<Query.Queryable, IEnumerable<T>> executeSelect)
		{
			queryable.Require("queryable").NotNull();
			return Create<T>(queryable, null, null, executeSelect);
		}

		/// <summary>
		/// Cria uma instancia do provedor para a consulta informada.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator">Criador de objetos.</param>
		/// <param name="executeSelect">Método acionado quando for executada uma seleção de valores da consulta.</param>
		/// <returns></returns>
		public static IQueryProvider Create<T>(Queryable queryable, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator, Func<Query.Queryable, IEnumerable<T>> executeSelect)
		{
			queryable.Require("queryable").NotNull();
			var sourceProvider = new QueryableSourceProvider<T>(queryable, bindStrategy, objectCreator, executeSelect);
			var sourceContext = new LinqSourceContext(queryable.SourceContext, sourceProvider);
			return new QueryProvider(sourceContext);
		}

		/// <summary>
		/// Cria uma consultada.
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
				throw new QueryException("ExpectedQueryableArgument");
			return new SourceQuery<TElement>(_sourceContext, expression);
		}

		/// <summary>
		/// Cria um estrutura de consulta.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public IQueryable CreateQuery(Expression expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			Type elementType = TypeHelper.GetElementType(expression.Type);
			Type type2 = typeof(IQueryable<>).MakeGenericType(new[] {
				elementType
			});
			if(!type2.IsAssignableFrom(expression.Type))
				throw new QueryException("ExpectedQueryableArgument");
			return (IQueryable)Activator.CreateInstance(typeof(SourceQuery<>).MakeGenericType(new[] {
				elementType
			}), new object[] {
				_sourceContext,
				expression
			});
		}

		/// <summary>
		/// Executa a consulta com a expressão informada.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)_sourceContext.Provider.Execute(expression);
		}

		/// <summary>
		/// Executa a consulta com a expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public object Execute(Expression expression)
		{
			return _sourceContext.Provider.Execute(expression);
		}
	}
}
