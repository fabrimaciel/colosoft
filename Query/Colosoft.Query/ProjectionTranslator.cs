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
using System.Collections.ObjectModel;

namespace Colosoft.Query.Linq.Translators
{
	internal partial class QueryTranslator
	{
		/// <summary>
		/// Classe responsável por traduzir a projeção do resultado.
		/// </summary>
		private class ProjectionTranslator : ExpressionVisitor
		{
			/// <summary>
			/// Lista dos membros da projeção.
			/// </summary>
			private readonly List<Member> _members;

			private LambdaExpression _projectionLambda;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public ProjectionTranslator()
			{
				_members = new List<Member>();
			}

			/// <summary>
			/// Cria uma instancia já com os membros da projeção.
			/// </summary>
			/// <param name="dataMembers"></param>
			public ProjectionTranslator(IEnumerable<Member> dataMembers)
			{
				_members = new List<Member>(dataMembers);
			}

			/// <summary>
			/// Recupera a clausula da projeção.
			/// </summary>
			internal string ProjectionClause
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			/// <summary>
			/// Membros.
			/// </summary>
			internal IEnumerable<Member> DataMembers
			{
				get
				{
					return _members;
				}
			}

			/// <summary>
			/// Expressão Lambda da projeção.
			/// </summary>
			internal LambdaExpression ProjectionLambda
			{
				get
				{
					return _projectionLambda;
				}
			}

			/// <summary>
			/// Traduz a expressão lambda.
			/// </summary>
			/// <param name="lambda"></param>
			internal void Translate(LambdaExpression lambda)
			{
				_projectionLambda = lambda;
				base.VisitLambda(lambda);
			}

			/// <summary>
			/// Visita o acesso do membro.
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				if(m.Expression == null)
					throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
				if(m.Expression.NodeType == ExpressionType.MemberAccess)
				{
					var memberExpression = (System.Linq.Expressions.MemberExpression)m.Expression;
					_members.Add(new Member(memberExpression.Member.Name, m.Member));
				}
				else
				{
					if(m.Expression.NodeType != ExpressionType.Parameter)
						throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
					_members.Add(new Member(null, m.Member));
				}
				return m;
			}
		}
	}
}
