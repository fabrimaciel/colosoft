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
	/// Implementação do comparador que usa expressões lambda para comparação.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LambdaComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _lambdaComparer;

		private readonly Func<T, int> _lambdaHash;

		/// <summary>
		/// Cria a instancia tendo uma expressão de compraração e ignorando o cálculo de hash.
		/// </summary>
		/// <param name="lambdaComparer"></param>
		public LambdaComparer(Func<T, T, bool> lambdaComparer) : this(lambdaComparer, o => 0)
		{
		}

		/// <summary>
		/// Cria a instancia tendo um expressão de compraração e de calculo da hash.
		/// </summary>
		/// <param name="lambdaComparer"></param>
		/// <param name="lambdaHash"></param>
		public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
		{
			if(lambdaComparer == null)
				throw new ArgumentNullException("lambdaComparer");
			if(lambdaHash == null)
				throw new ArgumentNullException("lambdaHash");
			_lambdaComparer = lambdaComparer;
			_lambdaHash = lambdaHash;
		}

		/// <summary>
		/// Verifica se as duas instancias informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(T x, T y)
		{
			return _lambdaComparer(x, y);
		}

		/// <summary>
		/// Calcula o hash do tipo informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(T obj)
		{
			return _lambdaHash(obj);
		}
	}
}
