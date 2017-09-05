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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação do consturtor de expressão do descritor de grupo.
	/// </summary>
	class GroupDescriptorExpressionBuilder : GroupDescriptorExpressionBuilderBase
	{
		private Expression _aggregateParameterExpression;

		private readonly GroupDescriptorExpressionBuilder _childBuilder;

		private readonly GroupDescriptor _groupDescriptor;

		private ParameterExpression _groupingParameterExpression;

		private readonly IQueryable _notPagedData;

		/// <summary>
		/// Expressão do parametro da agregação.
		/// </summary>
		private Expression AggregateParameterExpression
		{
			get
			{
				if(_aggregateParameterExpression == null)
				{
					Action<LambdaExpression> action = null;
					LambdaExpression predicate = CreateChildItemsFilterExpression();
					IQueryable items = _notPagedData;
					if(ParentBuilder != null)
					{
						if(action == null)
						{
							action = delegate(LambdaExpression expression) {
								items = items.Where(expression);
							};
						}
						this.ParentBuilder.CreateChildItemsFilterExpressionFromRecursive().Each<LambdaExpression>(action);
					}
					items = items.Where(predicate);
					_aggregateParameterExpression = items.Expression;
				}
				return this._aggregateParameterExpression;
			}
		}

		/// <summary>
		/// Construtor filho.
		/// </summary>
		public GroupDescriptorExpressionBuilder ChildBuilder
		{
			get
			{
				return _childBuilder;
			}
		}

		/// <summary>
		/// Descritor do grupo.
		/// </summary>
		public GroupDescriptor GroupDescriptor
		{
			get
			{
				return _groupDescriptor;
			}
		}

		/// <summary>
		/// Expressão do parametro de agrupamento.
		/// </summary>
		private ParameterExpression GroupingParameterExpression
		{
			get
			{
				if(_groupingParameterExpression == null)
				{
					LambdaExpression expression = CreateGroupByExpression();
					Type type = typeof(IGrouping<, >).MakeGenericType(new Type[] {
						expression.Body.Type,
						base.ItemType
					});
					_groupingParameterExpression = Expression.Parameter(type, "group" + this.GetHashCode());
				}
				return _groupingParameterExpression;
			}
		}

		/// <summary>
		/// Identifica se possui subgrupos,
		/// </summary>
		public bool HasSubgroups
		{
			get
			{
				return (_childBuilder != null);
			}
		}

		/// <summary>
		/// Construtor pai.
		/// </summary>
		public GroupDescriptorExpressionBuilder ParentBuilder
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da ordenação.
		/// </summary>
		protected override System.ComponentModel.ListSortDirection? SortDirection
		{
			get
			{
				return new System.ComponentModel.ListSortDirection?(this._groupDescriptor.SortDirection);
			}
		}

		/// <summary>
		/// Cria a instancia com a consulta e o descritor do grupo.
		/// </summary>
		/// <param name="queryable">Consulta.</param>
		/// <param name="groupDescriptor"></param>
		public GroupDescriptorExpressionBuilder(IQueryable queryable, GroupDescriptor groupDescriptor) : this(queryable, groupDescriptor, null, queryable)
		{
			_groupDescriptor = groupDescriptor;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="groupDescriptor"></param>
		/// <param name="childBuilder"></param>
		/// <param name="notPagedData"></param>
		public GroupDescriptorExpressionBuilder(IQueryable queryable, GroupDescriptor groupDescriptor, GroupDescriptorExpressionBuilder childBuilder, IQueryable notPagedData) : base(queryable)
		{
			_groupDescriptor = groupDescriptor;
			_childBuilder = childBuilder;
			_notPagedData = notPagedData;
		}

		/// <summary>
		/// Cria a expressão do filtro do itens filhos.
		/// </summary>
		/// <returns></returns>
		public LambdaExpression CreateChildItemsFilterExpression()
		{
			LambdaExpression expression = this.CreateGroupByExpression();
			Expression right = Expression.Property(this.GroupingParameterExpression, "Key");
			return Expression.Lambda(Expression.Equal(expression.Body, right), new ParameterExpression[] {
				base.ParameterExpression
			});
		}

		/// <summary>
		/// Cria a expressão do filtro do itens filhos a partir da recursividade.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<LambdaExpression> CreateChildItemsFilterExpressionFromRecursive()
		{
			List<LambdaExpression> list = new List<LambdaExpression> {
				this.CreateChildItemsFilterExpression()
			};
			if(this.ParentBuilder != null)
			{
				list.AddRange(this.ParentBuilder.CreateChildItemsFilterExpressionFromRecursive());
			}
			return list;
		}

		/// <summary>
		/// Cria o MemberBinding da projeção das funções de agregação.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateAggregateFunctionsProjectionMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("AggregateFunctionsProjection");
			Expression expression = this.CreateProjectionInitExpression();
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cria o MemberBinding para a quantidade de itens.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateCountMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("ItemCount");
			Expression expression = Expression.Call(typeof(Enumerable), "Count", new Type[] {
				base.ItemType
			}, new Expression[] {
				this.GroupingParameterExpression
			});
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cria o MemberBinding par ao nome do campo.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateFieldNameMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("Member");
			Expression expression = Expression.Constant(this.GroupDescriptor.Member ?? "");
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cria a expresão de agrupamento por.
		/// </summary>
		/// <returns></returns>
		protected override LambdaExpression CreateGroupByExpression()
		{
			MemberAccessExpressionBuilderBase base2 = ExpressionBuilderFactory.MemberAccess(this.Queryable, this._groupDescriptor.MemberType, this._groupDescriptor.Member);
			base2.ParameterExpression = base.ParameterExpression;
			return base2.CreateLambdaExpression();
		}

		/// <summary>
		/// Cria o MemberBinding da parte que identifica se possui subgrupos.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateHasSubgroupsMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("HasSubgroups");
			Expression expression = Expression.Constant(this.HasSubgroups);
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cira o MemberBinding para os itens.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateItemsMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("Items");
			Expression expression = this.CreateItemsExpression();
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cria o MemberBinding para a chave.
		/// </summary>
		/// <returns></returns>
		protected MemberBinding CreateKeyMemberBinding()
		{
			var property = typeof(AggregateFunctionsGroup).GetProperty("Key");
			Expression expression = Expression.Property(this.GroupingParameterExpression, "Key");
			if(expression.Type.IsValueType && !this.Queryable.Provider.IsEntityFrameworkProvider())
			{
				expression = Expression.Convert(expression, typeof(object));
			}
			return Expression.Bind(property, expression);
		}

		/// <summary>
		/// Cria os MemberBindings.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<MemberBinding> CreateMemberBindings()
		{
			yield return this.CreateKeyMemberBinding();
			yield return this.CreateCountMemberBinding();
			yield return this.CreateHasSubgroupsMemberBinding();
			yield return this.CreateFieldNameMemberBinding();
			if(this._groupDescriptor.AggregateFunctions.Count > 0)
			{
				yield return this.CreateAggregateFunctionsProjectionMemberBinding();
			}
			yield return this.CreateItemsMemberBinding();
		}

		/// <summary>
		/// Cria a expressão para ordenação por.
		/// </summary>
		/// <returns></returns>
		protected override LambdaExpression CreateOrderByExpression()
		{
			return Expression.Lambda(Expression.Property(this.GroupingParameterExpression, "Key"), new ParameterExpression[] {
				this.GroupingParameterExpression
			});
		}

		/// <summary>
		/// Cria a expressão para seleção.
		/// </summary>
		/// <returns></returns>
		protected override LambdaExpression CreateSelectExpression()
		{
			if(HasSubgroups)
			{
				_childBuilder.ParentBuilder = this;
			}
			return Expression.Lambda(CreateSelectBodyExpression(), new ParameterExpression[] {
				this.GroupingParameterExpression
			});
		}

		/// <summary>
		/// Cria um expressão para os itens.
		/// </summary>
		/// <returns></returns>
		private Expression CreateItemsExpression()
		{
			if(this.HasSubgroups)
			{
				return CreateItemsExpressionFromChildBuilder();
			}
			return this.GroupingParameterExpression;
		}

		/// <summary>
		/// Cria uma expressõa para os itens a parti do construtor do filho
		/// </summary>
		/// <returns></returns>
		private Expression CreateItemsExpressionFromChildBuilder()
		{
			LambdaExpression predicate = this.CreateChildItemsFilterExpression();
			IQueryable queryable = this.Queryable.Where(predicate);
			_childBuilder.Queryable = queryable;
			return _childBuilder.CreateQuery().Expression;
		}

		/// <summary>
		/// Cria a expressão para inicialização da projeção.
		/// </summary>
		/// <returns></returns>
		private Expression CreateProjectionInitExpression()
		{
			List<Expression> propertyValuesExpressions = this.ProjectionPropertyValueExpressions().ToList<Expression>();
			NewExpression newExpression = this.CreateProjectionNewExpression(propertyValuesExpressions);
			IEnumerable<MemberBinding> bindings = this.CreateProjectionMemberBindings(newExpression.Type, propertyValuesExpressions);
			return Expression.MemberInit(newExpression, bindings);
		}

		/// <summary>
		/// Cria os MemberBindings para a projeção.
		/// </summary>
		/// <param name="projectionType"></param>
		/// <param name="propertyValuesExpressions"></param>
		/// <returns></returns>
		private IEnumerable<MemberBinding> CreateProjectionMemberBindings(Type projectionType, IEnumerable<Expression> propertyValuesExpressions)
		{
			return this._groupDescriptor.AggregateFunctions.Consolidate<AggregateFunction, Expression, MemberAssignment>(propertyValuesExpressions, (f, e) => Expression.Bind(projectionType.GetProperty(f.FunctionName), e)).Cast<MemberBinding>();
		}

		/// <summary>
		/// Cria a expressão para a projeção com novo elementos.
		/// </summary>
		/// <param name="propertyValuesExpressions"></param>
		/// <returns></returns>
		private NewExpression CreateProjectionNewExpression(IEnumerable<Expression> propertyValuesExpressions)
		{
			IEnumerable<DynamicProperty> properties = _groupDescriptor.AggregateFunctions.Consolidate<AggregateFunction, Expression, DynamicProperty>(propertyValuesExpressions, (f, e) => new DynamicProperty(f.FunctionName, e.Type));
			return Expression.New(ClassFactory.Instance.GetDynamicClass(properties));
		}

		/// <summary>
		/// Cria a expressão para a seleção do corpo.
		/// </summary>
		/// <returns></returns>
		private Expression CreateSelectBodyExpression()
		{
			NewExpression newExpression = Expression.New(typeof(AggregateFunctionsGroup));
			IEnumerable<MemberBinding> bindings = CreateMemberBindings();
			return Expression.MemberInit(newExpression, bindings);
		}

		/// <summary>
		/// Cria as expressões para os valores das propriedades da expressão.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Expression> ProjectionPropertyValueExpressions()
		{
			return (from f in this._groupDescriptor.AggregateFunctions
			select f.CreateAggregateExpression(this.AggregateParameterExpression, base.Options.LiftMemberAccessToNull));
		}
	}
}
