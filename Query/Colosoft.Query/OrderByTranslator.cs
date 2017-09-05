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
		/// Classe responsável por traduzir a ordenação.
		/// </summary>
		private class OrderByTranslator : ExpressionVisitor
		{
			private readonly List<OrderByItem> _members;

			private OrderDirection _currentDirection;

			/// <summary>
			/// Membros da ordenação.
			/// </summary>
			public List<OrderByItem> Members
			{
				get
				{
					return _members;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public OrderByTranslator()
			{
				_members = new List<OrderByItem>();
			}

			/// <summary>
			/// Recupera a clausula da ordenação.
			/// </summary>
			internal string OrderByClause
			{
				get
				{
					if(_members.Count == 0)
						return string.Empty;
					StringBuilder sb = new StringBuilder();
					bool isFirst = true;
					foreach (var m in _members.Reverse<OrderByItem>())
					{
						if(!isFirst)
							sb.Append(", ");
						else
							isFirst = false;
						sb.Append(m.DataMember.Name);
						switch(m.Direction)
						{
						case OrderDirection.Ascending:
							sb.Append(" ASC");
							break;
						case OrderDirection.Descending:
							sb.Append(" DESC");
							break;
						default:
							throw new NotSupportedException("The selected sorting direction is not supported");
						}
					}
					return sb.ToString();
				}
			}

			/// <summary>
			/// Visita a expressão de ordenação.
			/// </summary>
			/// <param name="e">Expressão que será usada na ordenação.</param>
			/// <param name="direction">Direção da ordenação.</param>
			/// <returns></returns>
			internal virtual Expression Visit(Expression e, OrderDirection direction)
			{
				_currentDirection = direction;
				return Visit(e);
			}

			/// <summary>
			/// Visita o acesso ao membro.
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				if(m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
					throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
				_members.Add(new OrderByItem {
					DataMember = m.Member,
					Direction = _currentDirection
				});
				return m;
			}
		}
	}
}
