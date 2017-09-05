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
using System.Collections;

namespace Colosoft.DataStructures
{
	/// <summary>
	/// Assinatura da classe que representa um filha com prioriedade.
	/// </summary>
	public interface IPriorityQueue : ICloneable, IList, ICollection, IEnumerable
	{
		/// <summary>
		/// Recupera a instancia do primeiro item da fila.
		/// </summary>
		/// <returns></returns>
		object Peek();

		/// <summary>
		/// Remove o primeiro item da fila.
		/// </summary>
		/// <returns></returns>
		object Pop();

		/// <summary>
		/// Adiciona um novo item na fila.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int Push(object value);

		/// <summary>
		/// Atualiza o item na posição informada.
		/// </summary>
		/// <param name="i">Posição do item.</param>
		void Update(int i);
	}
}
