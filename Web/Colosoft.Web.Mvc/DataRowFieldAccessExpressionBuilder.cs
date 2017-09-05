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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Construtor de expressão para acesso ao campo de um linha de dados.
	/// </summary>
	class DataRowFieldAccessExpressionBuilder : MemberAccessExpressionBuilderBase
	{
		private readonly Type columnDataType;

		/// <summary>
		/// Método usado para recupera o campo.
		/// </summary>
		private static readonly System.Reflection.MethodInfo DataRowFieldMethod = typeof(System.Data.DataRowExtensions).GetMethod("Field", new Type[] {
			typeof(System.Data.DataRow),
			typeof(string)
		});

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="memberType">Tipo do membro.</param>
		/// <param name="memberName">Nome do membro.</param>
		public DataRowFieldAccessExpressionBuilder(Type memberType, string memberName) : base(typeof(System.Data.DataRow), memberName)
		{
			if(memberType.IsValueType && !memberType.IsNullableType())
			{
				this.columnDataType = typeof(Nullable<>).MakeGenericType(new Type[] {
					memberType
				});
			}
			else
			{
				this.columnDataType = memberType;
			}
		}

		/// <summary>
		/// Cria a expressão de acesso ao membro.
		/// </summary>
		/// <returns></returns>
		public override System.Linq.Expressions.Expression CreateMemberAccessExpression()
		{
			var expression = System.Linq.Expressions.Expression.Constant(base.MemberName);
			return System.Linq.Expressions.Expression.Call(DataRowFieldMethod.MakeGenericMethod(new Type[] {
				this.columnDataType
			}), base.ParameterExpression, expression);
		}

		/// <summary>
		/// Tipo de dado da coluna.
		/// </summary>
		public Type ColumnDataType
		{
			get
			{
				return this.columnDataType;
			}
		}
	}
}
