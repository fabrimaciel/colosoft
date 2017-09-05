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

namespace Colosoft.Web.Mvc
{
	public static class ExpressionBuilder
	{
		public static Expression<Func<T, bool>> Expression<T>(IList<IFilterDescriptor> filterDescriptors)
		{
			return Expression<T>(filterDescriptors, true);
		}

		public static Expression<Func<TModel, TValue>> Expression<TModel, TValue>(string memberName)
		{
			return (Expression<Func<TModel, TValue>>)Lambda<TModel>(memberName);
		}

		public static Expression<Func<T, bool>> Expression<T>(IList<IFilterDescriptor> filterDescriptors, bool checkForNull)
		{
			return (Expression<Func<T, bool>>)new Infrastructure.Implementation.Expressions.FilterDescriptorCollectionExpressionBuilder(System.Linq.Expressions.Expression.Parameter(typeof(T), "item"), filterDescriptors) {
				Options =  {
					LiftMemberAccessToNull = checkForNull
				}
			}.CreateFilterExpression();
		}

		public static LambdaExpression Lambda<T>(string memberName)
		{
			return Lambda<T>(memberName, false);
		}

		public static LambdaExpression Lambda<T>(string memberName, bool checkForNull)
		{
			return Infrastructure.Implementation.Expressions.ExpressionBuilderFactory.MemberAccess(typeof(T), memberName, checkForNull).CreateLambdaExpression();
		}

		public static LambdaExpression Lambda<T>(Type memberType, string memberName, bool checkForNull)
		{
			return Infrastructure.Implementation.Expressions.ExpressionBuilderFactory.MemberAccess(typeof(T), memberType, memberName, checkForNull).CreateLambdaExpression();
		}
	}
}
