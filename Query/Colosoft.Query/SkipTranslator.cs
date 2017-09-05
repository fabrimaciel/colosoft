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
using System.Linq.Expressions;

namespace Colosoft.Query.Linq.Translators
{
	internal partial class QueryTranslator
	{
		/// <summary>
		/// Classe usada para armazenar os dados da tradução da rotina de saltar registros.
		/// </summary>
		private class SkipTranslator : ExpressionVisitor
		{
			private int? _skip;

			private bool _useDefault;

			internal int? Skip
			{
				get
				{
					return _skip;
				}
			}

			internal bool UseDefault
			{
				get
				{
					return _useDefault;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			internal SkipTranslator()
			{
			}

			/// <summary>
			/// Recupera um novo tradutor.
			/// </summary>
			/// <param name="useDefault"></param>
			/// <returns></returns>
			internal static SkipTranslator GetNewFirstTranslator(bool useDefault)
			{
				return new SkipTranslator() {
					_skip = 1,
					_useDefault = useDefault
				};
			}

			/// <summary>
			/// Traduz a expressão.
			/// </summary>
			/// <param name="exp"></param>
			internal void Translate(Expression exp)
			{
				if(exp is MemberExpression)
				{
					var member = (MemberExpression)exp;
					if(member.Expression is ConstantExpression)
					{
						var constant = (ConstantExpression)member.Expression;
						if(member.Member is System.Reflection.PropertyInfo)
							exp = Expression.Constant(((System.Reflection.PropertyInfo)member.Member).GetValue(constant.Value, null));
					}
				}
				Visit(exp);
			}

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if(c.Type == typeof(Int32))
					_skip = (Int32)c.Value;
				return c;
			}
		}
	}
}
