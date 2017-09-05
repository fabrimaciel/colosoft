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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação base de um construtor de expressão de função de agregada.
	/// </summary>
	abstract class AggregateFunctionExpressionBuilderBase : ExpressionBuilderBase
	{
		private readonly System.Linq.Expressions.Expression _enumerableExpression;

		private readonly AggregateFunction _function;

		/// <summary>
		/// Expressão de enumeração.
		/// </summary>
		protected System.Linq.Expressions.Expression EnumerableExpression
		{
			get
			{
				return _enumerableExpression;
			}
		}

		/// <summary>
		/// Função agregada.
		/// </summary>
		protected AggregateFunction Function
		{
			get
			{
				return _function;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerableExpression">Expressão de enumeração.</param>
		/// <param name="function"></param>
		protected AggregateFunctionExpressionBuilderBase(System.Linq.Expressions.Expression enumerableExpression, AggregateFunction function) : base(ExtractItemTypeFromEnumerableType(enumerableExpression.Type))
		{
			this._enumerableExpression = enumerableExpression;
			this._function = function;
		}

		/// <summary>
		/// Cria a expressão agregada.
		/// </summary>
		/// <returns></returns>
		public abstract System.Linq.Expressions.Expression CreateAggregateExpression();

		/// <summary>
		/// Extrai tipo do item do tipo de enumeração.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type ExtractItemTypeFromEnumerableType(Type type)
		{
			Type type2 = type.FindGenericType(typeof(IEnumerable<>));
			if(type2 == null)
			{
				throw new ArgumentException("Provided type is not IEnumerable<>", "type");
			}
			return type2.GetGenericArguments().First<Type>();
		}
	}
}
