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
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Colosoft.Query.Dynamic
{
	/// <summary>
	/// Classe método para auxiliar a manipulação de expressões dinamicas.
	/// </summary>
	public static class DynamicExpression
	{
		/// <summary>
		/// Realiza o parser sobre a expressão informada.
		/// </summary>
		/// <param name="resultType"></param>
		/// <param name="expression"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static Expression Parse(Type resultType, string expression, params object[] values)
		{
			ExpressionParser parser = new ExpressionParser(null, expression, values);
			return parser.Parse(resultType);
		}

		/// <summary>
		/// Executa o parser sobre a expressão informada.
		/// </summary>
		/// <param name="itType">Tipo do origem onde será realizado o parser.</param>
		/// <param name="resultType"></param>
		/// <param name="expression"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
		{
			return ParseLambda(new ParameterExpression[] {
				Expression.Parameter(itType, "")
			}, resultType, expression, values);
		}

		/// <summary>
		/// Executa o parser sobre a expressão informada.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="resultType"></param>
		/// <param name="expression"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
		{
			ExpressionParser parser = new ExpressionParser(parameters, expression, values);
			return Expression.Lambda(parser.Parse(resultType), parameters);
		}

		/// <summary>
		/// Executa o parser sobre a expressão informada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="S"></typeparam>
		/// <param name="expression"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
		{
			return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, values);
		}
	}
}
