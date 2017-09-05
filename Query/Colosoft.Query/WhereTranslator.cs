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
		private class WhereTranslator : ExpressionVisitor
		{
			private readonly StringBuilder _sb;

			/// <summary>
			/// Lista dos parametros usados
			/// </summary>
			private readonly QueryParameterCollection _parameters;

			/// <summary>
			/// Claúsula condicional.
			/// </summary>
			internal string WhereClause
			{
				get
				{
					return _sb.ToString();
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parameters"></param>
			public WhereTranslator(QueryParameterCollection parameters)
			{
				_sb = new StringBuilder();
				_parameters = parameters;
			}

			/// <summary>
			/// Traduz a expressão.
			/// </summary>
			/// <param name="lambda"></param>
			internal void Translate(LambdaExpression lambda)
			{
				base.VisitLambda(lambda);
			}

			/// <summary>
			/// Is the expression comparing a value with NULL such as:
			/// c.CustomerName = NULL in which case it has to be
			/// reworked to look like (c.CustomerName is NULL)
			/// </summary>
			/// <returns></returns>
			internal virtual bool IsComparingWithNull(BinaryExpression b)
			{
				if(b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual)
				{
					if(IsNullConstant(b.Left as ConstantExpression) || IsNullConstant(b.Right as ConstantExpression))
						return true;
				}
				return false;
			}

			protected override Expression VisitConditional(ConditionalExpression c)
			{
				var whereTranslator1 = new WhereTranslator(this._parameters);
				whereTranslator1.Visit(c.Test);
				var whereTranslator2 = new WhereTranslator(this._parameters);
				whereTranslator2.Visit(c.IfTrue);
				var whereTranslator3 = new WhereTranslator(this._parameters);
				whereTranslator3.Visit(c.IfFalse);
				_sb.AppendFormat("(CASE WHEN ({0}) THEN ({1}) ELSE ({2}) END)", whereTranslator1._sb.ToString(), whereTranslator2._sb.ToString(), whereTranslator3._sb.ToString());
				return c;
			}

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				if(m.Method.Name == "Contains" || m.Method.Name == "StartsWith" || m.Method.Name == "EndsWith")
				{
					Visit(m.Object);
					_sb.Append(" LIKE ");
					VisitExpressionList(m.Arguments);
					var val = _parameters[_parameters.Count - 1].Value as string;
					if(val != null)
						_parameters[_parameters.Count - 1].Value = m.Method.Name == "Contains" ? "%" + val + "%" : m.Method.Name == "StartsWith" ? val + "%" : "%" + val;
					return m;
				}
				return base.VisitMethodCall(m);
			}

			protected override Expression VisitUnary(UnaryExpression u)
			{
				switch(u.NodeType)
				{
				case ExpressionType.Not:
					_sb.Append(" NOT ");
					Visit(u.Operand);
					break;
				case ExpressionType.Convert:
					Visit(u.Operand);
					break;
				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
				}
				return u;
			}

			protected override Expression VisitBinary(BinaryExpression b)
			{
				_sb.Append("(");
				Visit(b.Left);
				switch(b.NodeType)
				{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					_sb.Append(" AND ");
					break;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					_sb.Append(" OR ");
					break;
				case ExpressionType.Equal:
					if(IsComparingWithNull(b))
						_sb.Append(" IS ");
					else
					{
						if(b.Right is ConstantExpression && ((ConstantExpression)b.Right).Value is bool && (bool)((ConstantExpression)b.Right).Value)
						{
							_sb.Append(")");
							return b;
						}
						_sb.Append(" = ");
					}
					break;
				case ExpressionType.NotEqual:
					if(IsComparingWithNull(b))
						_sb.Append(" IS NOT ");
					else
						_sb.Append(" <> ");
					break;
				case ExpressionType.GreaterThan:
					_sb.Append(" > ");
					break;
				case ExpressionType.GreaterThanOrEqual:
					_sb.Append(" >= ");
					break;
				case ExpressionType.LessThan:
					_sb.Append(" < ");
					break;
				case ExpressionType.LessThanOrEqual:
					_sb.Append(" <= ");
					break;
				default:
					throw new NotSupportedException(string.Format("The Where predicate does not support '{0}' binary operator", b.NodeType));
				}
				Visit(b.Right);
				_sb.Append(")");
				return b;
			}

			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				if(m.Expression == null)
					throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
				if(m.Expression.NodeType == ExpressionType.MemberAccess)
				{
					var memberExpression = (System.Linq.Expressions.MemberExpression)m.Expression;
					_sb.Append(memberExpression.Member.Name).Append('.').Append(m.Member.Name);
				}
				else
				{
					if(m.Expression.NodeType == ExpressionType.Constant)
					{
						var constant = (ConstantExpression)m.Expression;
						if(m.Member is System.Reflection.PropertyInfo)
							constant = Expression.Constant(((System.Reflection.PropertyInfo)m.Member).GetValue(constant.Value, null));
						return VisitConstant(constant);
					}
					if(m.Expression.NodeType != ExpressionType.Parameter)
						throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
					_sb.Append(m.Member.Name);
				}
				return m;
			}

			protected override Expression VisitConstant(ConstantExpression c)
			{
				IQueryable q = c.Value as IQueryable;
				if(q != null)
					throw new NotSupportedException("Sub queries are not supported");
				if(c.Value != null)
				{
					var param = new QueryParameter("?param" + (_parameters.Count + 1), c.Value);
					_sb.Append(param.Name);
					_parameters.Add(param);
				}
				else
					_sb.Append("NULL");
				return c;
			}

			/// <summary>
			/// Verifica se a expressão é uma constante nula.
			/// </summary>
			/// <param name="ce"></param>
			/// <returns></returns>
			private static bool IsNullConstant(ConstantExpression ce)
			{
				return (ce != null && ce.Value == null);
			}
		}
	}
}
