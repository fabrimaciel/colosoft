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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Classe que compara elementos
	/// </summary>
	public class ElementComparer : IEqualityComparer<Element>, IComparer<Element>
	{
		/// <summary>
		/// Verifica se 2 elementos são iguais
		/// </summary>
		/// <param name="x">Elemento 1</param>
		/// <param name="y">Elemento 2</param>
		/// <returns>Verdadeiro se são iguais</returns>
		public bool Equals(Element x, Element y)
		{
			return x.Uid.Equals(y.Uid);
		}

		/// <summary>
		/// Retorna o código Hash do elemento
		/// </summary>
		/// <param name="obj">elemento</param>
		/// <returns>código hash</returns>
		public int GetHashCode(Element obj)
		{
			return obj.GetHashCode();
		}

		/// <summary>
		/// Compara 2 elementos
		/// </summary>
		/// <param name="x">Elemento 1</param>
		/// <param name="y">Elemento 2</param>
		/// <returns>int representando a comparação</returns>
		public int Compare(Element x, Element y)
		{
			return x.Uid - y.Uid;
		}
	}
}
