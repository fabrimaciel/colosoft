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

using Colosoft.Web.Mvc.Infrastructure;
using Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	public static class TreeDataSourceExtensions
	{
		private static System.Reflection.MethodInfo anyMethod = typeof(Queryable).GetMethods().First<System.Reflection.MethodInfo>(method => ((method.Name == "Any") && (method.GetParameters().Length == 1)));

		internal static IEnumerable<AggregateResult> AggregateForLevel<TModel, T1, T2>(this IEnumerable data, IQueryable allData, List<AggregateDescriptor> aggregates, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			ParameterExpression expression;
			return allData.Where(Expression.Lambda(CreateOrExpression<TModel, T1>(data, idSelector, expression = Expression.Parameter(typeof(TModel), "item")), new ParameterExpression[] {
				expression
			})).AggregateForLevel<TModel, T1, T2>(allData, aggregates, idSelector, parentIDSelector);
		}

		internal static IEnumerable<AggregateResult> AggregateForLevel<TModel, T1, T2>(this IQueryable data, IQueryable allData, List<AggregateDescriptor> aggregates, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			data = data.ChildrenRecursive<TModel, T1, T2>(allData, idSelector, parentIDSelector);
			IEnumerable<AggregateFunction> a2 = new AggregateFunction[0];
			foreach (var a in aggregates)
				a2 = a2.Concat(a.Aggregates);
			return data.Aggregate(a2);
		}

		private static MethodInfo AnyMethod(Type type)
		{
			return anyMethod.MakeGenericMethod(new Type[] {
				type
			});
		}

		internal static IQueryable Children<TModel, T1, T2>(this IQueryable roots, IQueryable allData, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			Type elementType = allData.ElementType;
			ParameterExpression instance = Expression.Parameter(elementType, "allItem");
			ParameterExpression expression2 = Expression.Parameter(elementType, "rootItem");
			Expression left = ExpressionFactory.MakeMemberAccess(expression2, idSelector.MemberWithoutInstance());
			Expression right = Expression.Convert(ExpressionFactory.MakeMemberAccess(instance, parentIDSelector.MemberWithoutInstance()), left.Type);
			LambdaExpression expression = Expression.Lambda(Expression.Equal(left, right), new ParameterExpression[] {
				instance
			});
			MethodCallExpression body = Expression.Call(typeof(Queryable), "Where", new Type[] {
				elementType
			}, new Expression[] {
				allData.Expression,
				Expression.Quote(expression)
			});
			LambdaExpression expression8 = Expression.Lambda(typeof(Func<TModel, IEnumerable<TModel>>), body, new ParameterExpression[] {
				expression2
			});
			MethodCallExpression expression9 = Expression.Call(typeof(Queryable), "SelectMany", new Type[] {
				elementType,
				elementType
			}, new Expression[] {
				roots.Expression,
				Expression.Quote(expression8)
			});
			return allData.Provider.CreateQuery(expression9);
		}

		internal static IQueryable ChildrenRecursive<TModel, T1, T2>(this IQueryable roots, IQueryable allData, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			IQueryable queryable = roots.Children<TModel, T1, T2>(allData, idSelector, parentIDSelector);
			if((bool)AnyMethod(typeof(TModel)).Invoke(null, new IQueryable[] {
				queryable
			}))
			{
				return roots.Union(queryable.ChildrenRecursive<TModel, T1, T2>(allData, idSelector, parentIDSelector));
			}
			return roots;
		}

		internal static Expression CreateOrExpression<TModel, T1>(IEnumerable data, Expression<Func<TModel, T1>> idSelector, Expression e)
		{
			Func<TModel, T1> func = idSelector.Compile();
			Expression right = null;
			Expression left = ExpressionFactory.MakeMemberAccess(e, idSelector.MemberWithoutInstance());
			foreach (TModel local in data)
			{
				if(right != null)
				{
					right = Expression.Or(Expression.Equal(left, Expression.Constant(func(local))), right);
				}
				else
				{
					right = Expression.Equal(left, Expression.Constant(func(local)));
				}
			}
			return right;
		}

		internal static IQueryable Parents(this IQueryable matches, IQueryable allData, LambdaExpression idSelector, LambdaExpression parentIDSelector)
		{
			Type elementType = allData.ElementType;
			ParameterExpression instance = Expression.Parameter(elementType, "allItem");
			ParameterExpression expression2 = Expression.Parameter(elementType, "matchedItem");
			Expression right = ExpressionFactory.MakeMemberAccess(instance, idSelector.MemberWithoutInstance());
			LambdaExpression expression = Expression.Lambda(Expression.Equal(Expression.Convert(ExpressionFactory.MakeMemberAccess(expression2, parentIDSelector.MemberWithoutInstance()), right.Type), right), new ParameterExpression[] {
				expression2
			});
			LambdaExpression expression8 = Expression.Lambda(Expression.Call(typeof(Queryable), "Any", new Type[] {
				elementType
			}, new Expression[] {
				matches.Expression,
				Expression.Quote(expression)
			}), new ParameterExpression[] {
				instance
			});
			MethodCallExpression expression9 = Expression.Call(typeof(Queryable), "Where", new Type[] {
				elementType
			}, new Expression[] {
				allData.Expression,
				Expression.Quote(expression8)
			});
			return allData.Provider.CreateQuery(expression9);
		}

		internal static IQueryable ParentsRecursive<TModel>(this IQueryable matches, IQueryable allData, LambdaExpression idSelector, LambdaExpression parentIDSelector)
		{
			IQueryable source = matches.Parents(allData, idSelector, parentIDSelector);
			if((bool)AnyMethod(matches.ElementType).Invoke(null, new IQueryable[] {
				source
			}))
			{
				source = source.Union(source.ParentsRecursive<TModel>(allData, idSelector, parentIDSelector));
			}
			return matches.Union(source);
		}
	}
}
