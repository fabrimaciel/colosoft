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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Implementação do visitador dos nós do filtro.
	/// </summary>
	public class FilterNodeVisitor : IFilterNodeVisitor
	{
		private Stack<IFilterDescriptor> _context = new Stack<IFilterDescriptor>();

		/// <summary>
		/// Atual descritor.
		/// </summary>
		private IFilterDescriptor CurrentDescriptor
		{
			get
			{
				if(_context.Count > 0)
					return _context.Peek();
				return null;
			}
		}

		/// <summary>
		/// Resultado.
		/// </summary>
		public IFilterDescriptor Result
		{
			get
			{
				return _context.Pop();
			}
		}

		/// <summary>
		/// Finaliza a visita.
		/// </summary>
		public void EndVisit()
		{
			if(_context.Count > 1)
				_context.Pop();
		}

		/// <summary>
		/// Inicia a visita.
		/// </summary>
		/// <param name="logicalNode">Nó lógico.</param>
		public void StartVisit(ILogicalNode logicalNode)
		{
			var item = new CompositeFilterDescriptor {
				LogicalOperator = logicalNode.LogicalOperator
			};
			CompositeFilterDescriptor currentDescriptor = this.CurrentDescriptor as CompositeFilterDescriptor;
			if(currentDescriptor != null)
			{
				currentDescriptor.FilterDescriptors.Add(item);
			}
			this._context.Push(item);
		}

		/// <summary>
		/// Inicia a visita.
		/// </summary>
		/// <param name="operatorNode"></param>
		public void StartVisit(IOperatorNode operatorNode)
		{
			FilterDescriptor item = new FilterDescriptor {
				Operator = operatorNode.FilterOperator
			};
			CompositeFilterDescriptor currentDescriptor = this.CurrentDescriptor as CompositeFilterDescriptor;
			if(currentDescriptor != null)
			{
				currentDescriptor.FilterDescriptors.Add(item);
			}
			this._context.Push(item);
		}

		/// <summary>
		/// Visita um nó de valor.
		/// </summary>
		/// <param name="valueNode"></param>
		public void Visit(IValueNode valueNode)
		{
			((FilterDescriptor)CurrentDescriptor).Value = valueNode.Value;
		}

		/// <summary>
		/// Visita um nó de propriedade.
		/// </summary>
		/// <param name="propertyNode"></param>
		public void Visit(PropertyNode propertyNode)
		{
			((FilterDescriptor)CurrentDescriptor).Member = propertyNode.Name;
		}
	}
}
