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
using System.Threading.Tasks;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	class QueryableAggregatesExpressionBuilder : GroupDescriptorExpressionBuilder
	{
		public QueryableAggregatesExpressionBuilder(IQueryable queryable, IEnumerable<AggregateFunction> aggregateFunctions) : base(queryable, CreateDescriptor(aggregateFunctions))
		{
		}

		private static GroupDescriptor CreateDescriptor(IEnumerable<AggregateFunction> aggregateFunctions)
		{
			GroupDescriptor descriptor = new GroupDescriptor();
			descriptor.AggregateFunctions.AddRange<AggregateFunction>(aggregateFunctions);
			return descriptor;
		}

		protected override LambdaExpression CreateGroupByExpression()
		{
			return Expression.Lambda(Expression.Constant(1), new ParameterExpression[] {
				base.ParameterExpression
			});
		}

		protected override IEnumerable<MemberBinding> CreateMemberBindings()
		{
			yield return this.CreateKeyMemberBinding();
			yield return this.CreateCountMemberBinding();
			yield return this.CreateHasSubgroupsMemberBinding();
			if(this.GroupDescriptor.AggregateFunctions.Count > 0)
			{
				yield return this.CreateAggregateFunctionsProjectionMemberBinding();
			}
			yield return this.CreateFieldNameMemberBinding();
		}
	}
}
