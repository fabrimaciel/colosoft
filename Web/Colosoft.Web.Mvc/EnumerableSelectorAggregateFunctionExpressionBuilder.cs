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
	class EnumerableSelectorAggregateFunctionExpressionBuilder : AggregateFunctionExpressionBuilderBase
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerableExpression"></param>
		/// <param name="function"></param>
		public EnumerableSelectorAggregateFunctionExpressionBuilder(System.Linq.Expressions.Expression enumerableExpression, EnumerableSelectorAggregateFunction function) : base(enumerableExpression, function)
		{
		}

		/// <summary>
		/// Converte a expressão do membro para uma epxressão ace acesso.
		/// </summary>
		/// <param name="memberExpression"></param>
		/// <returns></returns>
		private System.Linq.Expressions.Expression ConvertMemberAccessExpression(System.Linq.Expressions.Expression memberExpression)
		{
			if((base.ItemType.IsDataRow() || base.ItemType.IsDynamicObject()) && (this.Function.MemberType != null))
			{
				memberExpression = System.Linq.Expressions.Expression.Convert(memberExpression, this.Function.MemberType);
			}
			if(ShouldConvertTypeToInteger(memberExpression.Type.GetNonNullableType()))
			{
				memberExpression = ConvertMemberExpressionToInteger(memberExpression);
			}
			return memberExpression;
		}

		/// <summary>
		/// Converte a expressão do membro para uma expressão de número inteiro.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private static System.Linq.Expressions.Expression ConvertMemberExpressionToInteger(System.Linq.Expressions.Expression expression)
		{
			Type type = expression.Type.IsNullableType() ? typeof(int?) : typeof(int);
			return System.Linq.Expressions.Expression.Convert(expression, type);
		}

		/// <summary>
		/// Cria uma expressão agregada.
		/// </summary>
		/// <returns></returns>
		public override System.Linq.Expressions.Expression CreateAggregateExpression()
		{
			var memberSelectorExpression = this.CreateMemberSelectorExpression();
			return this.CreateMethodCallExpression(memberSelectorExpression);
		}

		/// <summary>
		/// Cria uma expressão seletora do membro.
		/// </summary>
		/// <returns></returns>
		private System.Linq.Expressions.LambdaExpression CreateMemberSelectorExpression()
		{
			var base2 = ExpressionBuilderFactory.MemberAccess(base.ItemType, (Type)null, this.Function.SourceField);
			base2.Options.CopyFrom(base.Options);
			var memberExpression = base2.CreateMemberAccessExpression();
			return System.Linq.Expressions.Expression.Lambda(this.ConvertMemberAccessExpression(memberExpression), new System.Linq.Expressions.ParameterExpression[] {
				base2.ParameterExpression
			});
		}

		/// <summary>
		/// Cria uma epxressão de chamada de método.
		/// </summary>
		/// <param name="memberSelectorExpression"></param>
		/// <returns></returns>
		private System.Linq.Expressions.Expression CreateMethodCallExpression(System.Linq.Expressions.LambdaExpression memberSelectorExpression)
		{
			IEnumerable<Type> methodArgumentsTypes = this.GetMethodArgumentsTypes(memberSelectorExpression);
			return System.Linq.Expressions.Expression.Call(this.Function.ExtensionMethodsType, this.Function.AggregateMethodName, methodArgumentsTypes.ToArray<Type>(), new[] {
				base.EnumerableExpression,
				memberSelectorExpression
			});
		}

		/// <summary>
		/// Recupera os tipos do argumento do método.
		/// </summary>
		/// <param name="memberSelectorExpression"></param>
		/// <returns></returns>
		private IEnumerable<Type> GetMethodArgumentsTypes(System.Linq.Expressions.LambdaExpression memberSelectorExpression)
		{
			yield return this.ItemType;
			if(!memberSelectorExpression.Body.Type.IsNumericType())
			{
				yield return memberSelectorExpression.Body.Type;
			}
		}

		/// <summary>
		/// Verifica se o tipo pode ser convertido para um valor inteiro.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool ShouldConvertTypeToInteger(Type type)
		{
			switch(Type.GetTypeCode(type))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return true;
			}
			return false;
		}

		/// <summary>
		/// Função de agregação do seletor.
		/// </summary>
		protected EnumerableSelectorAggregateFunction Function
		{
			get
			{
				return (EnumerableSelectorAggregateFunction)base.Function;
			}
		}
	}
}
