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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Construtor de expressões do coleção de descritores de ordenação.
	/// </summary>
	class SortDescriptorCollectionExpressionBuilder
	{
		private readonly IQueryable _queryable;

		private readonly IEnumerable<SortDescriptor> _sortDescriptors;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="sortDescriptors"></param>
		public SortDescriptorCollectionExpressionBuilder(IQueryable queryable, IEnumerable<SortDescriptor> sortDescriptors)
		{
			_queryable = queryable;
			_sortDescriptors = sortDescriptors;
		}

		/// <summary>
		/// Ordena a consulta.
		/// </summary>
		/// <returns></returns>
		public IQueryable Sort()
		{
			IQueryable queryable = this._queryable;
			bool flag = true;
			foreach (SortDescriptor descriptor in this._sortDescriptors)
			{
				Type memberType = typeof(object);
				var expression = Infrastructure.Implementation.Expressions.ExpressionBuilderFactory.MemberAccess(this._queryable, memberType, descriptor.Member).CreateLambdaExpression();
				string methodName = "";
				if(flag)
				{
					methodName = (descriptor.SortDirection == ListSortDirection.Ascending) ? "OrderBy" : "OrderByDescending";
					flag = false;
				}
				else
				{
					methodName = (descriptor.SortDirection == ListSortDirection.Ascending) ? "ThenBy" : "ThenByDescending";
				}
				queryable = queryable.Provider.CreateQuery(Expression.Call(typeof(Queryable), methodName, new Type[] {
					queryable.ElementType,
					expression.Body.Type
				}, new Expression[] {
					queryable.Expression,
					Expression.Quote(expression)
				}));
			}
			return queryable;
		}
	}
}
