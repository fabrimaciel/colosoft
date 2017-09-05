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
using System.Collections;
using System.Linq.Expressions;

namespace Colosoft.Query.Linq
{
	/// <summary>
	/// Representa uma consultada na fonte.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class SourceQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IQueryProvider, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable
	{
		private readonly ILinqSourceContext _context;

		private readonly Expression _queryExpression;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="expression"></param>
		public SourceQuery(ILinqSourceContext context, Expression expression)
		{
			_context = context;
			_queryExpression = expression;
		}

		/// <summary>
		/// Tipo do elemento da consulta.
		/// </summary>
		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		/// <summary>
		/// Expressão associada com a consulta.
		/// </summary>
		Expression IQueryable.Expression
		{
			get
			{
				return _queryExpression;
			}
		}

		IQueryProvider IQueryable.Provider
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Recupera os itens.
		/// </summary>
		/// <returns></returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if(_context == null)
				throw new QueryInvalidOperationException(Properties.Resources.InvalidOperationException_ContextUndefined);
			if(_context.Provider == null)
				throw new QueryInvalidOperationException(Properties.Resources.InvalidOperationException_ProviderOfContextUndefined);
			return ((IEnumerable<T>)_context.Provider.Execute(_queryExpression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_context.Provider.Execute(_queryExpression)).GetEnumerator();
		}

		/// <summary>
		/// Cria a consutla para o tipo informado.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		IQueryable<TResult> IQueryProvider.CreateQuery<TResult>(Expression expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(!typeof(IQueryable<TResult>).IsAssignableFrom(expression.Type))
				throw new QueryException("ExpectedQueryableArgument");
			return new SourceQuery<TResult>(_context, expression);
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			Type elementType = TypeHelper.GetElementType(expression.Type);
			Type type = typeof(IQueryable<>).MakeGenericType(new[] {
				elementType
			});
			if(!type.IsAssignableFrom(expression.Type))
				throw new QueryException("ExpectedQueryableArgument");
			return (IQueryable)Activator.CreateInstance(typeof(SourceQuery<>).MakeGenericType(new[] {
				elementType
			}), new object[] {
				_context,
				expression
			});
		}

		object IQueryProvider.Execute(Expression expression)
		{
			return _context.Provider.Execute(expression);
		}

		TElement IQueryProvider.Execute<TElement>(Expression expression)
		{
			return (TElement)_context.Provider.Execute(expression);
		}
	}
}
