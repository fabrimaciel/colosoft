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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Classe com método para auxiliar na manipulação do operador de filtro.
	/// </summary>
	static class FilterOperatorExtensions
	{
		/// <summary>
		/// Referencia do método para comparar textos.
		/// </summary>
		public static readonly MethodInfo StringCompareMethodInfo = typeof(string).GetMethod("Compare", new Type[] {
			typeof(string),
			typeof(string)
		});

		/// <summary>
		/// Referencia do método para verifica se um texto contém outro texto.
		/// </summary>
		public static readonly MethodInfo StringContainsMethodInfo = typeof(string).GetMethod("Contains", new Type[] {
			typeof(string)
		});

		/// <summary>
		/// Referencia do método para verifica se o texto termina com o texto informado.
		/// </summary>
		public static readonly MethodInfo StringEndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new Type[] {
			typeof(string)
		});

		/// <summary>
		/// Referencia do método para verifica se o texto começa com o texto informado.
		/// </summary>
		public static readonly MethodInfo StringStartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new Type[] {
			typeof(string)
		});

		/// <summary>
		/// Referencia do método que passar todas as letras para minusculo.
		/// </summary>
		public static readonly MethodInfo StringToLowerMethodInfo = typeof(string).GetMethod("ToLower", new Type[0]);

		/// <summary>
		/// Cria uma expressão com base nos parmatros informados.
		/// </summary>
		/// <param name="filterOperator">Operador do filtro.</param>
		/// <param name="left">Expressão da esquerda.</param>
		/// <param name="right">Expressão da direita.</param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		public static Expression CreateExpression(this FilterOperator filterOperator, Expression left, Expression right, bool liftMemberAccess)
		{
			switch(filterOperator)
			{
			case FilterOperator.IsLessThan:
				return GenerateLessThan(left, right, liftMemberAccess);
			case FilterOperator.IsLessThanOrEqualTo:
				return GenerateLessThanEqual(left, right, liftMemberAccess);
			case FilterOperator.IsEqualTo:
				return GenerateEqual(left, right, liftMemberAccess);
			case FilterOperator.IsNotEqualTo:
				return GenerateNotEqual(left, right, liftMemberAccess);
			case FilterOperator.IsGreaterThanOrEqualTo:
				return GenerateGreaterThanEqual(left, right, liftMemberAccess);
			case FilterOperator.IsGreaterThan:
				return GenerateGreaterThan(left, right, liftMemberAccess);
			case FilterOperator.StartsWith:
				return GenerateStartsWith(left, right, liftMemberAccess);
			case FilterOperator.EndsWith:
				return GenerateEndsWith(left, right, liftMemberAccess);
			case FilterOperator.Contains:
				return GenerateContains(left, right, liftMemberAccess);
			case FilterOperator.IsContainedIn:
				return GenerateIsContainedIn(left, right, liftMemberAccess);
			case FilterOperator.DoesNotContain:
				return GenerateNotContains(left, right, liftMemberAccess);
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Gera a chamada de método de texto case insensitive.
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateCaseInsensitiveStringMethodCall(MethodInfo methodInfo, Expression left, Expression right, bool liftMemberAccess)
		{
			Expression instance = GenerateToLowerCall(left, liftMemberAccess);
			Expression expression2 = GenerateToLowerCall(right, liftMemberAccess);
			if(methodInfo.IsStatic)
				return Expression.Call(methodInfo, new Expression[] {
					instance,
					expression2
				});
			return Expression.Call(instance, methodInfo, new Expression[] {
				expression2
			});
		}

		/// <summary>
		/// Gera a expressão Contains.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateContains(Expression left, Expression right, bool liftMemberAccess)
		{
			return GenerateCaseInsensitiveStringMethodCall(StringContainsMethodInfo, left, right, liftMemberAccess);
		}

		/// <summary>
		/// Gera a expressão para comparação de texto terminado com.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateEndsWith(Expression left, Expression right, bool liftMemberAccess)
		{
			return GenerateCaseInsensitiveStringMethodCall(StringEndsWithMethodInfo, left, right, liftMemberAccess);
		}

		/// <summary>
		/// Gera a expressão de comparação de duas outras expressões.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateEqual(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				left = GenerateToLowerCall(left, liftMemberAccess);
				right = GenerateToLowerCall(right, liftMemberAccess);
			}
			return Expression.Equal(left, right);
		}

		/// <summary>
		/// Gera a expressão de compara maior que.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateGreaterThan(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				return Expression.GreaterThan(GenerateCaseInsensitiveStringMethodCall(StringCompareMethodInfo, left, right, liftMemberAccess), ExpressionFactory.ZeroExpression);
			}
			return Expression.GreaterThan(left, right);
		}

		/// <summary>
		/// Gera a expressão de compara maior que ou igual.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateGreaterThanEqual(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				return Expression.GreaterThanOrEqual(GenerateCaseInsensitiveStringMethodCall(StringCompareMethodInfo, left, right, liftMemberAccess), ExpressionFactory.ZeroExpression);
			}
			return Expression.GreaterThanOrEqual(left, right);
		}

		/// <summary>
		/// Gera a expressão de comparação está contido.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateIsContainedIn(Expression left, Expression right, bool liftMemberAccess)
		{
			return GenerateCaseInsensitiveStringMethodCall(StringContainsMethodInfo, right, left, liftMemberAccess);
		}

		/// <summary>
		/// Gera a expressão de comparação menor que.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateLessThan(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				return Expression.LessThan(GenerateCaseInsensitiveStringMethodCall(StringCompareMethodInfo, left, right, liftMemberAccess), ExpressionFactory.ZeroExpression);
			}
			return Expression.LessThan(left, right);
		}

		/// <summary>
		/// Gera a expressão de comparaçã menor que ou igual.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateLessThanEqual(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				return Expression.LessThanOrEqual(GenerateCaseInsensitiveStringMethodCall(StringCompareMethodInfo, left, right, liftMemberAccess), ExpressionFactory.ZeroExpression);
			}
			return Expression.LessThanOrEqual(left, right);
		}

		/// <summary>
		/// Gera a expressão de comparação não contém.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateNotContains(Expression left, Expression right, bool liftMemberAccess)
		{
			return Expression.Not(GenerateCaseInsensitiveStringMethodCall(StringContainsMethodInfo, left, right, liftMemberAccess));
		}

		/// <summary>
		/// Gera a expressão de comparação diferente.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateNotEqual(Expression left, Expression right, bool liftMemberAccess)
		{
			if(left.Type == typeof(string))
			{
				left = GenerateToLowerCall(left, liftMemberAccess);
				right = GenerateToLowerCall(right, liftMemberAccess);
			}
			return Expression.NotEqual(left, right);
		}

		/// <summary>
		/// Gera a expressão de comparação começa com.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateStartsWith(Expression left, Expression right, bool liftMemberAccess)
		{
			return GenerateCaseInsensitiveStringMethodCall(StringStartsWithMethodInfo, left, right, liftMemberAccess);
		}

		/// <summary>
		/// Gera a expressão para deixar o texto em minusculo.
		/// </summary>
		/// <param name="stringExpression"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		private static Expression GenerateToLowerCall(Expression stringExpression, bool liftMemberAccess)
		{
			if(liftMemberAccess)
			{
				stringExpression = ExpressionFactory.LiftStringExpressionToEmpty(stringExpression);
			}
			return Expression.Call(stringExpression, StringToLowerMethodInfo);
		}
	}
}
