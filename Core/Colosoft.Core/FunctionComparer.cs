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

namespace Colosoft
{
	/// <summary>
	/// Implementação do comparador funcional.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FunctionalComparer<T> : IComparer<T>
	{
		private Func<T, T, int> _comparer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="comparer"></param>
		public FunctionalComparer(Func<T, T, int> comparer)
		{
			comparer.Require("comparer").NotNull();
			_comparer = comparer;
		}

		/// <summary>
		/// Cria uma instancia do comparador.
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IComparer<T> Create(Func<T, T, int> comparer)
		{
			return new FunctionalComparer<T>(comparer);
		}

		/// <summary>
		/// Compara os valor informados.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}
	}
}
