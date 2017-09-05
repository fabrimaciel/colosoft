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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Representa o construtor de expressão para a coleção de descritor de filtros.
	/// </summary>
	class FilterDescriptorCollectionExpressionBuilder : FilterExpressionBuilder
	{
		private readonly IEnumerable<IFilterDescriptor> _filterDescriptors;

		private readonly FilterCompositionLogicalOperator _logicalOperator;

		/// <summary>
		/// Cria a instancia com a expressão do parametro e os descritores dos filtros.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <param name="filterDescriptors"></param>
		public FilterDescriptorCollectionExpressionBuilder(ParameterExpression parameterExpression, IEnumerable<IFilterDescriptor> filterDescriptors) : this(parameterExpression, filterDescriptors, FilterCompositionLogicalOperator.And)
		{
		}

		/// <summary>
		/// Cria a instancia com a epxressão o parametro, os descritores dos filtros e o operador lógico.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <param name="filterDescriptors"></param>
		/// <param name="logicalOperator"></param>
		public FilterDescriptorCollectionExpressionBuilder(ParameterExpression parameterExpression, IEnumerable<IFilterDescriptor> filterDescriptors, FilterCompositionLogicalOperator logicalOperator) : base(parameterExpression)
		{
			_filterDescriptors = filterDescriptors;
			_logicalOperator = logicalOperator;
		}

		/// <summary>
		/// Compóe as expressões informadas.
		/// </summary>
		/// <param name="left">Expressão de esquerda.</param>
		/// <param name="right">Expressão da direita.</param>
		/// <param name="logicalOperator">Operador lógico.</param>
		/// <returns></returns>
		private static Expression ComposeExpressions(Expression left, Expression right, FilterCompositionLogicalOperator logicalOperator)
		{
			switch(logicalOperator)
			{
			case FilterCompositionLogicalOperator.Or:
				return Expression.OrElse(left, right);
			}
			return Expression.AndAlso(left, right);
		}

		/// <summary>
		/// Inicializa as op~ções do contrutor de expressão.
		/// </summary>
		/// <param name="filterDescriptor"></param>
		private void InitilializeExpressionBuilderOptions(IFilterDescriptor filterDescriptor)
		{
			FilterDescriptorBase base2 = filterDescriptor as FilterDescriptorBase;
			if(base2 != null)
			{
				base2.ExpressionBuilderOptions.CopyFrom(base.Options);
			}
		}

		/// <summary>
		/// Cria a expressão do corpo.
		/// </summary>
		/// <returns></returns>
		public override Expression CreateBodyExpression()
		{
			Expression left = null;
			foreach (IFilterDescriptor descriptor in this._filterDescriptors)
			{
				this.InitilializeExpressionBuilderOptions(descriptor);
				Expression right = descriptor.CreateFilterExpression(base.ParameterExpression);
				if(left == null)
				{
					left = right;
				}
				else
				{
					left = ComposeExpressions(left, right, this._logicalOperator);
				}
			}
			if(left == null)
			{
				return ExpressionConstants.TrueLiteral;
			}
			return left;
		}
	}
}
