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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Classe com método que auxiliam na criação de expressões.
	/// </summary>
	static class ExpressionFactory
	{
		/// <summary>
		/// Cria uma expressão condicional.
		/// </summary>
		/// <param name="instance">Instancia da expressão.</param>
		/// <param name="memberAccess">Membro que será acessado.</param>
		/// <param name="defaultValue">Valor padrão.</param>
		/// <returns></returns>
		private static Expression CreateConditionExpression(Expression instance, Expression memberAccess, Expression defaultValue)
		{
			Expression right = DefaltValueExpression(instance.Type);
			return Expression.Condition(Expression.NotEqual(instance, right), memberAccess, defaultValue);
		}

		/// <summary>
		/// Cria uma expressão se para comparação se o valor for nulo.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="memberAccess"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static Expression CreateIfNullExpression(Expression instance, Expression memberAccess, Expression defaultValue)
		{
			if(ShouldGenerateCondition(instance.Type))
				return CreateConditionExpression(instance, memberAccess, defaultValue);
			return memberAccess;
		}

		/// <summary>
		/// Extra a expressão de acesso ao membro da expressão levandata.
		/// </summary>
		/// <param name="liftedToNullExpression"></param>
		/// <returns></returns>
		private static Expression ExtractMemberAccessExpressionFromLiftedExpression(Expression liftedToNullExpression)
		{
			while (liftedToNullExpression.NodeType == ExpressionType.Conditional)
			{
				ConditionalExpression expression = (ConditionalExpression)liftedToNullExpression;
				if(expression.Test.NodeType == ExpressionType.NotEqual)
				{
					liftedToNullExpression = expression.IfTrue;
				}
				else
				{
					liftedToNullExpression = expression.IfFalse;
				}
			}
			return liftedToNullExpression;
		}

		/// <summary>
		/// Recupera a expressão da instancia a partir da expressão informada.
		/// </summary>
		/// <param name="memberAccess"></param>
		/// <returns></returns>
		private static Expression GetInstanceExpressionFromExpression(Expression memberAccess)
		{
			MemberExpression expression = memberAccess as MemberExpression;
			if(expression != null)
			{
				return expression.Expression;
			}
			MethodCallExpression expression2 = memberAccess as MethodCallExpression;
			if(expression2 != null)
			{
				return expression2.Object;
			}
			return null;
		}

		/// <summary>
		/// Levanta o acesso ao membro para nulo recursivo.
		/// </summary>
		/// <param name="memberAccess"></param>
		/// <param name="conditionalExpression"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static Expression LiftMemberAccessToNullRecursive(Expression memberAccess, Expression conditionalExpression, Expression defaultValue)
		{
			Expression instanceExpressionFromExpression = GetInstanceExpressionFromExpression(memberAccess);
			if(instanceExpressionFromExpression == null)
			{
				return conditionalExpression;
			}
			conditionalExpression = CreateIfNullExpression(instanceExpressionFromExpression, conditionalExpression, defaultValue);
			return LiftMemberAccessToNullRecursive(instanceExpressionFromExpression, conditionalExpression, defaultValue);
		}

		/// <summary>
		/// Verifica se pode gerar condicional para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool ShouldGenerateCondition(Type type)
		{
			if(type.IsValueType)
			{
				return type.IsNullableType();
			}
			return true;
		}

		/// <summary>
		/// Cria uma expressão para o valor padrão do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Expression DefaltValueExpression(Type type)
		{
			return Expression.Constant(type.DefaultValue(), type);
		}

		/// <summary>
		/// Verifica se a expressão é uma constante not null.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static bool IsNotNullConstantExpression(Expression expression)
		{
			if(expression.NodeType == ExpressionType.Constant)
			{
				ConstantExpression expression2 = (ConstantExpression)expression;
				return (expression2.Value != null);
			}
			return false;
		}

		/// <summary>
		/// Levanta o acesso ao membro para nulo.
		/// </summary>
		/// <param name="memberAccess"></param>
		/// <returns></returns>
		public static Expression LiftMemberAccessToNull(Expression memberAccess)
		{
			Expression defaultValue = DefaltValueExpression(memberAccess.Type);
			return LiftMemberAccessToNullRecursive(memberAccess, memberAccess, defaultValue);
		}

		/// <summary>
		/// Levanta a chamada do método para nulo.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="method"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static Expression LiftMethodCallToNull(Expression instance, System.Reflection.MethodInfo method, params Expression[] arguments)
		{
			return LiftMemberAccessToNull(Expression.Call(ExtractMemberAccessExpressionFromLiftedExpression(instance), method, arguments));
		}

		/// <summary>
		/// Levanta a expressão textual para vazia.
		/// </summary>
		/// <param name="stringExpression"></param>
		/// <returns></returns>
		public static Expression LiftStringExpressionToEmpty(Expression stringExpression)
		{
			if(stringExpression.Type != typeof(string))
			{
				throw new ArgumentException("Provided expression should have string type", "stringExpression");
			}
			if(IsNotNullConstantExpression(stringExpression))
			{
				return stringExpression;
			}
			return Expression.Coalesce(stringExpression, EmptyStringExpression);
		}

		/// <summary>
		/// Cria o acesso do membro.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public static Expression MakeMemberAccess(Expression instance, string memberName)
		{
			foreach (IMemberAccessToken token in MemberAccessTokenizer.GetTokens(memberName))
			{
				instance = token.CreateMemberAccessExpression(instance);
			}
			return instance;
		}

		/// <summary>
		/// Cria oo acesso do membro.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="memberName"></param>
		/// <param name="liftMemberAccessToNull"></param>
		/// <returns></returns>
		public static Expression MakeMemberAccess(Expression instance, string memberName, bool liftMemberAccessToNull)
		{
			Expression memberAccess = MakeMemberAccess(instance, memberName);
			if(liftMemberAccessToNull)
			{
				return LiftMemberAccessToNull(memberAccess);
			}
			return memberAccess;
		}

		/// <summary>
		/// Expressão que representa um texto vazio.
		/// </summary>
		public static ConstantExpression EmptyStringExpression
		{
			get
			{
				return Expression.Constant(string.Empty);
			}
		}

		/// <summary>
		/// Expressão que representa um valor zerado.
		/// </summary>
		public static ConstantExpression ZeroExpression
		{
			get
			{
				return Expression.Constant(0);
			}
		}
	}
}
