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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado que é a composição de duas funções.
	/// </summary>
	public class CompositeFunction : IFunctor, IComparable
	{
		private IFunctor _func;

		private IFunctor _func2;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="func">Primeira função da composição.</param>
		/// <param name="func2">Segunda função da composição.</param>
		public CompositeFunction(IFunctor func, IFunctor func2)
		{
			_func = func;
			_func2 = func2;
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is CompositeFunction)
			{
				CompositeFunction function = (CompositeFunction)obj;
				if((((IComparable)_func).CompareTo(function._func) == 0) && (((IComparable)_func2).CompareTo(function._func2) == 0))
					return 0;
			}
			return -1;
		}

		/// <summary>
		/// Calcula o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public object Evaluate(object o)
		{
			return _func.Evaluate(_func2.Evaluate(o));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (_func + "." + _func2);
		}
	}
}
