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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Representa o nó de uma função.
	/// </summary>
	public class FunctionNode : IFilterNode, IOperatorNode
	{
		/// <summary>
		/// Nós dos argumentos da função.
		/// </summary>
		public IList<IFilterNode> Arguments
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public FilterOperator FilterOperator
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FunctionNode()
		{
			Arguments = new List<IFilterNode>();
		}

		/// <summary>
		/// Aceite.
		/// </summary>
		/// <param name="visitor"></param>
		public void Accept(IFilterNodeVisitor visitor)
		{
			visitor.StartVisit(this);
			foreach (IFilterNode node in this.Arguments)
			{
				node.Accept(visitor);
			}
			visitor.EndVisit();
		}
	}
}
