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
	/// Implementa do predicado de comparação maior.
	/// </summary>
	public class FunctorGreaterFunctorPredicate : Predicate, IComparable
	{
		private IFunctor _functor;

		private IFunctor _functor2;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="functor"></param>
		/// <param name="functor2"></param>
		public FunctorGreaterFunctorPredicate(IFunctor functor, IFunctor functor2)
		{
			_functor = functor;
			_functor2 = functor2;
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			object a = _functor.Evaluate(o);
			object b = _functor2.Evaluate(o);
			return (System.Collections.Comparer.Default.Compare(a, b) > 0);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is FunctorGreaterFunctorPredicate)
			{
				FunctorGreaterFunctorPredicate predicate = (FunctorGreaterFunctorPredicate)obj;
				if(base.Inverse == predicate.Inverse)
				{
					if((((IComparable)_functor).CompareTo(predicate._functor) == 0) && (((IComparable)_functor2).CompareTo(predicate._functor2) == 0))
						return 0;
					return -1;
				}
			}
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (_functor + (base.Inverse ? " <= " : " > ") + _functor2);
		}
	}
}
