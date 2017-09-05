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
		/// Representa o tradutor da quantidade de itens q recuperar.
		/// </summary>
		private class TakeTranslator : ExpressionVisitor
		{
			private int? _count;

			private bool _useDefault;

			internal int? Count
			{
				get
				{
					return _count;
				}
			}

			internal bool UseDefault
			{
				get
				{
					return _useDefault;
				}
			}

			internal TakeTranslator()
			{
			}

			/// <summary>
			/// Recupera um novo tradutor.
			/// </summary>
			/// <param name="useDefault"></param>
			/// <returns></returns>
			internal static TakeTranslator GetNewFirstTranslator(bool useDefault)
			{
				return new TakeTranslator() {
					_count = 1,
					_useDefault = useDefault
				};
			}

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

			/// <summary>
			/// Trata a visita de uma constante.
			/// </summary>
			/// <param name="c"></param>
			/// <returns></returns>
			protected override Expression VisitConstant(ConstantExpression c)
			{
				if(c.Type == typeof(Int32))
					_count = (Int32)c.Value;
				return c;
			}
		}
	}
}
