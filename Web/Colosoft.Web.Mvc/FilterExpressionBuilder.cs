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
	/// Implementação do construtor de expressão de filtros.
	/// </summary>
	abstract class FilterExpressionBuilder : ExpressionBuilderBase
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameterExpression"></param>
		protected FilterExpressionBuilder(ParameterExpression parameterExpression) : base(parameterExpression.Type)
		{
			base.ParameterExpression = parameterExpression;
		}

		/// <summary>
		/// Cria a expressão do corpo.
		/// </summary>
		/// <returns></returns>
		public abstract Expression CreateBodyExpression();

		/// <summary>
		/// Cria a expressão do filtro.
		/// </summary>
		/// <returns></returns>
		public LambdaExpression CreateFilterExpression()
		{
			return Expression.Lambda(this.CreateBodyExpression(), new[] {
				base.ParameterExpression
			});
		}
	}
}
