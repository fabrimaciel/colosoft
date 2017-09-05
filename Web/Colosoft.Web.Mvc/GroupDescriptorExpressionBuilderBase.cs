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
using System.Threading.Tasks;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação base do construtor de expressão para o descritor de grupo.
	/// </summary>
	abstract class GroupDescriptorExpressionBuilderBase : ExpressionBuilderBase
	{
		private IQueryable _queryable;

		/// <summary>
		/// Construto padrão.
		/// </summary>
		/// <param name="queryable">Consulta do grupo.</param>
		protected GroupDescriptorExpressionBuilderBase(IQueryable queryable) : base(queryable.ElementType)
		{
			_queryable = queryable;
		}

		/// <summary>
		/// Cria a expressão do group by.
		/// </summary>
		/// <returns></returns>
		protected abstract System.Linq.Expressions.LambdaExpression CreateGroupByExpression();

		/// <summary>
		/// Cria a expressão do order by.
		/// </summary>
		/// <returns></returns>
		protected abstract System.Linq.Expressions.LambdaExpression CreateOrderByExpression();

		/// <summary>
		/// Cria a expressão de projeção.
		/// </summary>
		/// <returns></returns>
		protected abstract System.Linq.Expressions.LambdaExpression CreateSelectExpression();

		/// <summary>
		/// Cria a consulta para a expressão.
		/// </summary>
		/// <returns></returns>
		public IQueryable CreateQuery()
		{
			return _queryable.GroupBy(CreateGroupByExpression()).OrderBy(CreateOrderByExpression(), SortDirection).Select(CreateSelectExpression());
		}

		public virtual IQueryable Queryable
		{
			get
			{
				return this._queryable;
			}
			protected set
			{
				this._queryable = value;
			}
		}

		protected virtual System.ComponentModel.ListSortDirection? SortDirection
		{
			get
			{
				return null;
			}
		}
	}
}
