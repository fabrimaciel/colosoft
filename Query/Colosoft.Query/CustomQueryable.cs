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
using System.Text;

namespace Colosoft.Query.Linq
{
	/// <summary>
	/// Implementação do Queryable para a consulta.
	/// </summary>
	class CustomQueryable : IQueryable, System.Collections.IEnumerable
	{
		private Type _elementType;

		private ILinqSourceContext _sourceContext;

		private Colosoft.Query.Queryable _queryable;

		private QueryProvider _queryProvider;

		/// <summary>
		/// Instancia do contexto associado.
		/// </summary>
		public ISourceContext Context
		{
			get
			{
				return _sourceContext;
			}
		}

		/// <summary>
		/// Cria a instancia a partir de uma consulta.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="elementType">Tipo do elemento do resultado da consulta.</param>
		public CustomQueryable(Query.Queryable queryable, Type elementType) : this(queryable, elementType, null)
		{
		}

		/// <summary>
		/// Cria a instancia a partir de uma consulta.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="elementType"></param>
		/// <param name="executeSelect">Referencia do método que será usado para executar a seleção dos dados.</param>
		public CustomQueryable(Query.Queryable queryable, Type elementType, Func<Query.Queryable, System.Collections.IEnumerable> executeSelect)
		{
			queryable.Require("queryable").NotNull();
			elementType.Require("elementType").NotNull();
			_elementType = elementType;
			_sourceContext = queryable.SourceContext as ILinqSourceContext;
			_queryable = queryable;
			if(_sourceContext == null)
				_sourceContext = new LinqSourceContext(queryable.SourceContext, new QueryableSourceProvider(queryable, executeSelect));
			_queryProvider = new QueryProvider(_sourceContext);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context">Contexto usado pela instancia.</param>
		public CustomQueryable(ILinqSourceContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			_sourceContext = context;
			_queryProvider = new QueryProvider(context);
		}

		/// <summary>
		/// Tipo do elemento trabalhado.
		/// </summary>
		public Type ElementType
		{
			get
			{
				return _elementType;
			}
		}

		/// <summary>
		/// Expressão associada com a instancia.
		/// </summary>
		public Expression Expression
		{
			get
			{
				return Expression.Constant(this);
			}
		}

		/// <summary>
		/// Recupera a instancia do provedor de consulta.
		/// </summary>
		public System.Linq.IQueryProvider Provider
		{
			get
			{
				return _queryProvider;
			}
		}

		/// <summary>
		/// Recupera o enumerator.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)_sourceContext.Provider.Execute(Expression.Constant(this))).GetEnumerator();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("Entity(" + ElementType.Name + ")");
		}
	}
}
