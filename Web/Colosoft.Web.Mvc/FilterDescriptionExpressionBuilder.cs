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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação do construtor de expressão de descrição do filtro.
	/// </summary>
	class FilterDescriptionExpressionBuilder : FilterExpressionBuilder
	{
		private readonly FilterDescription _filterDescription;

		/// <summary>
		/// Descrição do filtro.
		/// </summary>
		public FilterDescription FilterDescription
		{
			get
			{
				return _filterDescription;
			}
		}

		/// <summary>
		/// Expressão da descrição do filtro.
		/// </summary>
		private System.Linq.Expressions.Expression FilterDescriptionExpression
		{
			get
			{
				return System.Linq.Expressions.Expression.Constant(_filterDescription);
			}
		}

		/// <summary>
		/// Informações do método que satisfaz o filtro.
		/// </summary>
		private System.Reflection.MethodInfo SatisfiesFilterMethodInfo
		{
			get
			{
				return _filterDescription.GetType().GetMethod("SatisfiesFilter", new[] {
					typeof(object)
				});
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <param name="filterDescription"></param>
		public FilterDescriptionExpressionBuilder(System.Linq.Expressions.ParameterExpression parameterExpression, FilterDescription filterDescription) : base(parameterExpression)
		{
			_filterDescription = filterDescription;
		}

		/// <summary>
		/// Cria a expressão do filtro ativo.
		/// </summary>
		/// <returns></returns>
		protected virtual System.Linq.Expressions.Expression CreateActiveFilterExpression()
		{
			return CreateSatisfiesFilterExpression();
		}

		/// <summary>
		/// Cria a expressão do corpo.
		/// </summary>
		/// <returns></returns>
		public override System.Linq.Expressions.Expression CreateBodyExpression()
		{
			if(_filterDescription.IsActive)
			{
				return CreateActiveFilterExpression();
			}
			return ExpressionConstants.TrueLiteral;
		}

		/// <summary>
		/// Cria a expressão que satisfaz o filtro.
		/// </summary>
		/// <returns></returns>
		private System.Linq.Expressions.MethodCallExpression CreateSatisfiesFilterExpression()
		{
			System.Linq.Expressions.Expression parameterExpression = base.ParameterExpression;
			if(parameterExpression.Type.IsValueType)
			{
				parameterExpression = System.Linq.Expressions.Expression.Convert(parameterExpression, typeof(object));
			}
			return System.Linq.Expressions.Expression.Call(FilterDescriptionExpression, SatisfiesFilterMethodInfo, new[] {
				parameterExpression
			});
		}
	}
}
