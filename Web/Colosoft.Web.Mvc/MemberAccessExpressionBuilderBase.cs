﻿/* 
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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação base do contrutor de expressão de acesso a membros.
	/// </summary>
	abstract class MemberAccessExpressionBuilderBase : ExpressionBuilderBase
	{
		private readonly string _memberName;

		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string MemberName
		{
			get
			{
				return _memberName;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="itemType"></param>
		/// <param name="memberName"></param>
		protected MemberAccessExpressionBuilderBase(Type itemType, string memberName) : base(itemType)
		{
			_memberName = memberName;
		}

		/// <summary>
		/// Cria a expressão lambda.
		/// </summary>
		/// <returns></returns>
		public System.Linq.Expressions.LambdaExpression CreateLambdaExpression()
		{
			return System.Linq.Expressions.Expression.Lambda(this.CreateMemberAccessExpression(), base.ParameterExpression);
		}

		/// <summary>
		/// Cria a expressão de acesso ao membro.
		/// </summary>
		/// <returns></returns>
		public abstract System.Linq.Expressions.Expression CreateMemberAccessExpression();
	}
}
