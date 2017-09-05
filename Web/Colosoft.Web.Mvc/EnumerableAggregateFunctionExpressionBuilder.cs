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
	/// Implementação do construtor de expressão para funções de agregação.
	/// </summary>
	class EnumerableAggregateFunctionExpressionBuilder : AggregateFunctionExpressionBuilderBase
	{
		/// <summary>
		/// Instancia da função de agregação associada.
		/// </summary>
		protected EnumerableAggregateFunction Function
		{
			get
			{
				return (EnumerableAggregateFunction)base.Function;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerableExpression"></param>
		/// <param name="function">Função de agregação.</param>
		public EnumerableAggregateFunctionExpressionBuilder(System.Linq.Expressions.Expression enumerableExpression, EnumerableAggregateFunction function) : base(enumerableExpression, function)
		{
		}

		/// <summary>
		/// Cria a expressão de agregação.
		/// </summary>
		/// <returns></returns>
		public override System.Linq.Expressions.Expression CreateAggregateExpression()
		{
			return System.Linq.Expressions.Expression.Call(Function.ExtensionMethodsType, Function.AggregateMethodName, new Type[] {
				base.ItemType
			}, new[] {
				base.EnumerableExpression
			});
		}
	}
}
