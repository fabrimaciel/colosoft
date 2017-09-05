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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Colosoft.Collections
{
	/// <summary>
	/// Assinatura que repreesnta um coleção ordenável.
	/// </summary>
	public interface ISortableCollection : IEnumerable, IList, ICollection
	{
		/// <summary>
		/// Aplica a ordenação a coleção.
		/// </summary>
		/// <param name="sortExpression">Expressão de ordenação.</param>
		void ApplySort(string sortExpression);

		/// <summary>
		/// Aplica a ordenação a coleção.
		/// </summary>
		/// <param name="property">Descritor da propriedade que será ordenada.</param>
		/// <param name="direction">Direção da ordenação.</param>
		void ApplySort(PropertyDescriptor property, ListSortDirection direction);

		/// <summary>
		/// Remove a ordenação aplicada.
		/// </summary>
		void RemoveSort();
	}
}
