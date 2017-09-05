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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Representa o construtor de expressão de acesso a propriedade de objetos dinamicos.
	/// </summary>
	class DynamicPropertyAccessExpressionBuilder : MemberAccessExpressionBuilderBase
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="itemType"></param>
		/// <param name="memberName"></param>
		public DynamicPropertyAccessExpressionBuilder(Type itemType, string memberName) : base(itemType, memberName)
		{
		}

		/// <summary>
		/// Cria a expressão de acesso ao indexador.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="indexerToken"></param>
		/// <returns></returns>
		private Expression CreateIndexerAccessExpression(Expression instance, IndexerToken indexerToken)
		{
			return Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None, typeof(DynamicPropertyAccessExpressionBuilder), new[] {
				Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null),
				Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.Constant | Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.UseCompileTimeType, null)
			}), typeof(object), new Expression[] {
				instance,
				indexerToken.Arguments.Select<object, ConstantExpression>(new Func<object, ConstantExpression>(Expression.Constant)).First<ConstantExpression>()
			});
		}

		/// <summary>
		/// Cria a expressão ade acesso a propriedade.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private Expression CreatePropertyAccessExpression(Expression instance, string propertyName)
		{
			return Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None, propertyName, typeof(DynamicPropertyAccessExpressionBuilder), new[] {
				Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null)
			}), typeof(object), new Expression[] {
				instance
			});
		}

		/// <summary>
		/// Cria a expressão para acesso ao membro.
		/// </summary>
		/// <returns></returns>
		public override Expression CreateMemberAccessExpression()
		{
			if(string.IsNullOrEmpty(base.MemberName))
			{
				return base.ParameterExpression;
			}
			Expression parameterExpression = base.ParameterExpression;
			foreach (IMemberAccessToken token in MemberAccessTokenizer.GetTokens(base.MemberName))
			{
				if(token is PropertyToken)
				{
					string propertyName = ((PropertyToken)token).PropertyName;
					parameterExpression = this.CreatePropertyAccessExpression(parameterExpression, propertyName);
				}
				else if(token is IndexerToken)
				{
					parameterExpression = this.CreateIndexerAccessExpression(parameterExpression, (IndexerToken)token);
				}
			}
			return parameterExpression;
		}
	}
}
