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
using Colosoft.Web.Mvc.Infrastructure;
using System.Linq.Expressions;
using System.Collections;
using System.ComponentModel;
using Colosoft.Web.Mvc.UI;
using System.Web.Mvc;
using Colosoft.Web.Mvc.Infrastructure.Implementation;

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Extensões para usar sobre o Queryable.
	/// </summary>
	public static class QueryableExtensions
	{
		/// <summary>
		/// Agrega o resultado da consulta para as funções de agregação informadas.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="aggregateFunctions"></param>
		/// <returns></returns>
		public static AggregateResultCollection Aggregate(this IQueryable source, IEnumerable<AggregateFunction> aggregateFunctions)
		{
			var list = aggregateFunctions.ToList<AggregateFunction>();
			if(list.Count > 0)
			{
				foreach (Infrastructure.Implementation.Expressions.AggregateFunctionsGroup group in new Infrastructure.Implementation.Expressions.QueryableAggregatesExpressionBuilder(source, list) {
					Options =  {
						LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider()
					}
				}.CreateQuery())
				{
					return group.GetAggregateResults(list);
				}
			}
			return new AggregateResultCollection();
		}

		/// <summary>
		/// Chama o método da consulta.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="methodName"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		private static IQueryable CallQueryableMethod(this IQueryable source, string methodName, LambdaExpression selector)
		{
			return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), methodName, new Type[] {
				source.ElementType,
				selector.Body.Type
			}, new Expression[] {
				source.Expression,
				Expression.Quote(selector)
			}));
		}

		/// <summary>
		/// Recupera a quantidade da consulta informada.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static int Count(this IQueryable source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.Provider.Execute<int>(Expression.Call(typeof(Queryable), "Count", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression
			}));
		}

		/// <summary>
		/// Cria o resultado da origem de dados.
		/// </summary>
		/// <typeparam name="TModel">Tipo da model.</typeparam>
		/// <typeparam name="TResult">Tipo do resultado.</typeparam>
		/// <param name="queryable">Consulta.</param>
		/// <param name="request">Requisição.</param>
		/// <param name="modelState">Estado do modelo.</param>
		/// <param name="selector">Seletor.</param>
		/// <returns></returns>
		private static Colosoft.Web.Mvc.UI.DataSourceResult CreateDataSourceResult<TModel, TResult>(this IQueryable queryable, Mvc.UI.DataSourceRequest request, System.Web.Mvc.ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			Func<AggregateDescriptor, IEnumerable<AggregateFunction>> func = null;
			Action<GroupDescriptor> action = null;
			Action<GroupDescriptor> action2 = null;
			var result = new Mvc.UI.DataSourceResult();
			IQueryable source = queryable;
			var filters = new List<IFilterDescriptor>();
			if(request.Filters != null)
				filters.AddRange(request.Filters);
			if(filters.Any())
				source = source.Where(filters);
			var sorts = new List<SortDescriptor>();
			if(request.Sorts != null)
				sorts.AddRange(request.Sorts);
			var temporarySortDescriptors = new List<SortDescriptor>();
			var instance = new List<GroupDescriptor>();
			if(request.Groups != null)
				instance.AddRange<GroupDescriptor>(request.Groups);
			var aggregates = new List<AggregateDescriptor>();
			if(request.Aggregates != null)
				aggregates.AddRange(request.Aggregates);
			if(aggregates.Any())
			{
				IQueryable queryable3 = source.AsQueryable();
				IQueryable queryable4 = queryable3;
				if(filters.Any())
					queryable4 = queryable3.Where(filters);
				if(func == null)
					func = a => a.Aggregates;
				result.AggregateResults = queryable4.Aggregate(aggregates.SelectMany(func));
				if(instance.Any() && aggregates.Any())
				{
					if(action == null)
					{
						action = delegate(GroupDescriptor g) {
							foreach (var a in aggregates)
								g.AggregateFunctions.AddRange(a.Aggregates);
						};
					}
					instance.Each(action);
				}
			}
			result.Total = source.Count();
			if(!sorts.Any() && queryable.Provider.IsEntityFrameworkProvider())
			{
				var descriptor = new SortDescriptor {
					Member = queryable.ElementType.FirstSortableProperty()
				};
				sorts.Add(descriptor);
				temporarySortDescriptors.Add(descriptor);
			}
			if(instance.Any())
			{
				if(action2 == null)
				{
					action2 = delegate(GroupDescriptor groupDescriptor) {
						var item = new SortDescriptor {
							Member = groupDescriptor.Member,
							SortDirection = groupDescriptor.SortDirection
						};
						sorts.Insert(0, item);
						temporarySortDescriptors.Add(item);
					};
				}
				instance.Reverse<GroupDescriptor>().Each<GroupDescriptor>(action2);
			}
			if(sorts.Any())
			{
				source = source.Sort(sorts);
			}
			IQueryable notPagedData = source;
			source = source.Page(request.Page - 1, request.PageSize);
			if(instance.Any())
			{
				source = source.GroupBy(notPagedData, instance);
			}
			result.Data = source.Execute<TModel, TResult>(selector);
			if((modelState != null) && !modelState.IsValid)
			{
				result.Errors = modelState.SerializeErrors();
			}
			temporarySortDescriptors.Each(sortDescriptor => sorts.Remove(sortDescriptor));
			return result;
		}

		private static UI.TreeDataSourceResult CreateTreeDataSourceResult<TModel, T1, T2, TResult>(this IQueryable queryable, UI.DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, System.Web.Mvc.ModelStateDictionary modelState, Func<TModel, TResult> selector, Expression<Func<TModel, bool>> rootSelector)
		{
			var result = new UI.TreeDataSourceResult();
			IQueryable source = queryable;
			List<IFilterDescriptor> list = new List<IFilterDescriptor>();
			if(request.Filters != null)
			{
				list.AddRange(request.Filters);
			}
			if(list.Any<IFilterDescriptor>())
			{
				source = source.Where(list).ParentsRecursive<TModel>(queryable, idSelector, parentIDSelector);
			}
			IQueryable allData = source;
			if(rootSelector != null)
			{
				source = source.Where(rootSelector);
			}
			List<SortDescriptor> list2 = new List<SortDescriptor>();
			if(request.Sorts != null)
			{
				list2.AddRange(request.Sorts);
			}
			List<AggregateDescriptor> list3 = new List<AggregateDescriptor>();
			if(request.Aggregates != null)
			{
				list3.AddRange(request.Aggregates);
			}
			if(list3.Any<AggregateDescriptor>())
			{
				IQueryable queryable4 = source;
				foreach (IGrouping<T2, TModel> grouping in queryable4.GroupBy(parentIDSelector))
				{
					result.AggregateResults.Add(Convert.ToString(grouping.Key), grouping.AggregateForLevel<TModel, T1, T2>(allData, list3, idSelector, parentIDSelector));
				}
			}
			if(list2.Any<SortDescriptor>())
			{
				source = source.Sort(list2);
			}
			result.Data = source.Execute<TModel, TResult>(selector);
			if((modelState != null) && !modelState.IsValid)
			{
				result.Errors = modelState.SerializeErrors();
			}
			return result;
		}

		/// <summary>
		/// Recupera o elemento no indice informado.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static object ElementAt(this IQueryable source, int index)
		{
			if(source == null)
				throw new ArgumentNullException("source");
			if(index < 0)
				throw new ArgumentOutOfRangeException("index");
			return source.Provider.Execute(Expression.Call(typeof(Queryable), "ElementAt", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression,
				Expression.Constant(index)
			}));
		}

		private static IEnumerable Execute<TModel, TResult>(this IQueryable source, Func<TModel, TResult> selector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(source is DataTableWrapper)
			{
				return source;
			}
			Type elementType = source.ElementType;
			if(selector != null)
			{
				var list = new List<Infrastructure.Implementation.Expressions.AggregateFunctionsGroup>();
				if(elementType == typeof(Infrastructure.Implementation.Expressions.AggregateFunctionsGroup))
				{
					foreach (Infrastructure.Implementation.Expressions.AggregateFunctionsGroup group in source)
					{
						group.Items = group.Items.AsQueryable().Execute<TModel, TResult>(selector);
						list.Add(group);
					}
					return list;
				}
				List<TResult> list2 = new List<TResult>();
				foreach (TModel local in source)
				{
					list2.Add(selector(local));
				}
				return list2;
			}
			IList list3 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] {
				elementType
			}));
			foreach (object obj2 in source)
			{
				list3.Add(obj2);
			}
			return list3;
		}

		/// <summary>
		/// Recupera o campo do tipo com base na coluna de dados.
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="memberName"></param>
		/// <returns></returns>
		private static Type GetFieldByTypeFromDataColumn(System.Data.DataTable dataTable, string memberName)
		{
			if(!dataTable.Columns.Contains(memberName))
				return null;
			return dataTable.Columns[memberName].DataType;
		}

		public static IQueryable GroupBy(this IQueryable source, LambdaExpression keySelector)
		{
			return source.CallQueryableMethod("GroupBy", keySelector);
		}

		public static IQueryable GroupBy(this IQueryable source, IEnumerable<GroupDescriptor> groupDescriptors)
		{
			return source.GroupBy(source, groupDescriptors);
		}

		public static IQueryable GroupBy(this IQueryable source, IQueryable notPagedData, IEnumerable<GroupDescriptor> groupDescriptors)
		{
			return new Infrastructure.Implementation.Expressions.GroupDescriptorCollectionExpressionBuilder(source, groupDescriptors, notPagedData) {
				Options =  {
					LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider()
				}
			}.CreateQuery();
		}

		public static IQueryable OrderBy(this IQueryable source, LambdaExpression keySelector)
		{
			return source.CallQueryableMethod("OrderBy", keySelector);
		}

		public static IQueryable OrderBy(this IQueryable source, LambdaExpression keySelector, ListSortDirection? sortDirection)
		{
			if(!sortDirection.HasValue)
			{
				return source;
			}
			if(((ListSortDirection)sortDirection.Value) == ListSortDirection.Ascending)
			{
				return source.OrderBy(keySelector);
			}
			return source.OrderByDescending(keySelector);
		}

		public static IQueryable OrderByDescending(this IQueryable source, LambdaExpression keySelector)
		{
			return source.CallQueryableMethod("OrderByDescending", keySelector);
		}

		public static IQueryable Page(this IQueryable source, int pageIndex, int pageSize)
		{
			IQueryable queryable = source;
			queryable = queryable.Skip(pageIndex * pageSize);
			if(pageSize > 0)
			{
				queryable = queryable.Take(pageSize);
			}
			return queryable;
		}

		public static IQueryable Select(this IQueryable source, LambdaExpression selector)
		{
			return source.CallQueryableMethod("Select", selector);
		}

		public static IQueryable Skip(this IQueryable source, int count)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Skip", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression,
				Expression.Constant(count)
			}));
		}

		public static IQueryable Sort(this IQueryable source, IEnumerable<SortDescriptor> sortDescriptors)
		{
			var builder = new Infrastructure.Implementation.SortDescriptorCollectionExpressionBuilder(source, sortDescriptors);
			return builder.Sort();
		}

		public static IQueryable Take(this IQueryable source, int count)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Take", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression,
				Expression.Constant(count)
			}));
		}

		private static DataSourceResult ToDataSourceResult(this DataTableWrapper enumerable, DataSourceRequest request)
		{
			List<IFilterDescriptor> source = new List<IFilterDescriptor>();
			if(request.Filters != null)
			{
				source.AddRange(request.Filters);
			}
			if(source.Any<IFilterDescriptor>())
			{
				var dataTable = enumerable.Table;
				source.SelectMemberDescriptors().Each<FilterDescriptor>(delegate(FilterDescriptor f) {
					f.MemberType = GetFieldByTypeFromDataColumn(dataTable, f.Member);
				});
			}
			List<GroupDescriptor> list2 = new List<GroupDescriptor>();
			if(request.Groups != null)
			{
				list2.AddRange(request.Groups);
			}
			if(list2.Any<GroupDescriptor>())
			{
				var dataTable = enumerable.Table;
				list2.Each<GroupDescriptor>(delegate(GroupDescriptor g) {
					g.MemberType = GetFieldByTypeFromDataColumn(dataTable, g.Member);
				});
			}
			DataSourceResult result = enumerable.AsEnumerable<System.Data.DataRowView>().ToDataSourceResult(request);
			result.Data = result.Data.SerializeToDictionary(enumerable.Table);
			return result;
		}

		public static DataSourceResult ToDataSourceResult(this IEnumerable enumerable, DataSourceRequest request)
		{
			return enumerable.AsQueryable().ToDataSourceResult(request);
		}

		public static DataSourceResult ToDataSourceResult(this System.Data.DataTable dataTable, DataSourceRequest request)
		{
			return dataTable.WrapAsEnumerable().ToDataSourceResult(request);
		}

		public static DataSourceResult ToDataSourceResult(this IQueryable enumerable, DataSourceRequest request)
		{
			return enumerable.ToDataSourceResult(request, null);
		}

		public static DataSourceResult ToDataSourceResult(this IEnumerable enumerable, DataSourceRequest request, ModelStateDictionary modelState)
		{
			return enumerable.AsQueryable().ToDataSourceResult(request, modelState);
		}

		public static DataSourceResult ToDataSourceResult(this IQueryable queryable, DataSourceRequest request, ModelStateDictionary modelState)
		{
			return queryable.CreateDataSourceResult<object, object>(request, modelState, null);
		}

		public static DataSourceResult ToDataSourceResult<TModel, TResult>(this IEnumerable<TModel> enumerable, DataSourceRequest request, Func<TModel, TResult> selector)
		{
			return enumerable.AsQueryable<TModel>().CreateDataSourceResult<TModel, TResult>(request, null, selector);
		}

		public static DataSourceResult ToDataSourceResult<TModel, TResult>(this IQueryable<TModel> enumerable, DataSourceRequest request, Func<TModel, TResult> selector)
		{
			return enumerable.CreateDataSourceResult<TModel, TResult>(request, null, selector);
		}

		public static DataSourceResult ToDataSourceResult<TModel, TResult>(this IEnumerable<TModel> enumerable, DataSourceRequest request, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return enumerable.AsQueryable<TModel>().CreateDataSourceResult<TModel, TResult>(request, modelState, selector);
		}

		public static DataSourceResult ToDataSourceResult<TModel, TResult>(this IQueryable<TModel> enumerable, DataSourceRequest request, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return enumerable.CreateDataSourceResult<TModel, TResult>(request, modelState, selector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult(this IEnumerable enumerable, DataSourceRequest request)
		{
			return enumerable.AsQueryable().ToTreeDataSourceResult(request, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, TResult>(this IEnumerable<TModel> enumerable, DataSourceRequest request, Func<TModel, TResult> selector)
		{
			return enumerable.ToTreeDataSourceResult<TModel, object, object, TResult>(request, null, null, selector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult(this IEnumerable enumerable, DataSourceRequest request, ModelStateDictionary modelState)
		{
			return enumerable.AsQueryable().CreateTreeDataSourceResult<object, object, object, object>(request, null, null, modelState, null, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, TResult>(this IQueryable<TModel> enumerable, DataSourceRequest request, Func<TModel, TResult> selector)
		{
			return enumerable.ToTreeDataSourceResult<TModel, object, object, TResult>(request, null, null, selector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IEnumerable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			return enumerable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, null, null, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IQueryable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector)
		{
			return enumerable.CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, null, null, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IEnumerable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Func<TModel, TResult> selector)
		{
			return queryable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, null, selector, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IEnumerable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector)
		{
			return enumerable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, null, null, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IEnumerable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, ModelStateDictionary modelState)
		{
			return queryable.AsQueryable<TModel>().ToTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, modelState, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IQueryable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Func<TModel, TResult> selector)
		{
			return queryable.CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, null, selector, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IQueryable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector)
		{
			return enumerable.CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, null, null, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IQueryable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, ModelStateDictionary modelState)
		{
			return queryable.ToTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, modelState, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IEnumerable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, Func<TModel, TResult> selector)
		{
			return queryable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, null, selector, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IEnumerable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, ModelStateDictionary modelState)
		{
			return enumerable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, modelState, null, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IEnumerable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return queryable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, modelState, selector, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2>(this IQueryable<TModel> enumerable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, ModelStateDictionary modelState)
		{
			return enumerable.CreateTreeDataSourceResult<TModel, T1, T2, TModel>(request, idSelector, parentIDSelector, modelState, null, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IQueryable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return queryable.CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, modelState, selector, null);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IQueryable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, Func<TModel, TResult> selector)
		{
			return queryable.CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, null, selector, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IEnumerable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return queryable.AsQueryable<TModel>().CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, modelState, selector, rootSelector);
		}

		public static TreeDataSourceResult ToTreeDataSourceResult<TModel, T1, T2, TResult>(this IQueryable<TModel> queryable, DataSourceRequest request, Expression<Func<TModel, T1>> idSelector, Expression<Func<TModel, T2>> parentIDSelector, Expression<Func<TModel, bool>> rootSelector, ModelStateDictionary modelState, Func<TModel, TResult> selector)
		{
			return queryable.CreateTreeDataSourceResult<TModel, T1, T2, TResult>(request, idSelector, parentIDSelector, modelState, selector, rootSelector);
		}

		public static IQueryable Union(this IQueryable source, IQueryable second)
		{
			return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Union", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression,
				second.Expression
			}));
		}

		public static IQueryable Where(this IQueryable source, Expression predicate)
		{
			return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new Type[] {
				source.ElementType
			}, new Expression[] {
				source.Expression,
				Expression.Quote(predicate)
			}));
		}

		public static IQueryable Where(this IQueryable source, IEnumerable<IFilterDescriptor> filterDescriptors)
		{
			if(filterDescriptors.Any<IFilterDescriptor>())
			{
				LambdaExpression predicate = new Infrastructure.Implementation.Expressions.FilterDescriptorCollectionExpressionBuilder(Expression.Parameter(source.ElementType, "item"), filterDescriptors) {
					Options =  {
						LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider()
					}
				}.CreateFilterExpression();
				return source.Where(predicate);
			}
			return source;
		}
	}
}
