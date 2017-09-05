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
using Colosoft.Text.Parser;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado de comparação menor.
	/// </summary>
	public class FunctorLesserGeneratorPredicate : Predicate, IComparable
	{
		private IFunctor _functor;

		private IGenerator _generator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="functor"></param>
		/// <param name="generator"></param>
		public FunctorLesserGeneratorPredicate(IFunctor functor, IGenerator generator)
		{
			_functor = functor;
			_generator = generator;
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			object a = _functor.Evaluate(o);
			if(base.Inverse)
				return (Comparer.Default.Compare(a, _generator.Evaluate()) >= 0);
			return (Comparer.Default.Compare(a, _generator.Evaluate()) < 0);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is FunctorLesserGeneratorPredicate)
			{
				FunctorLesserGeneratorPredicate predicate = (FunctorLesserGeneratorPredicate)obj;
				if(base.Inverse == predicate.Inverse)
				{
					if((((IComparable)_functor).CompareTo(predicate._functor) == 0) && (((IComparable)_generator).CompareTo(predicate._generator) == 0))
						return 0;
					return -1;
				}
			}
			return -1;
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="nextPredicate"></param>
		internal override void Execute(QueryContext queryContext, Predicate nextPredicate)
		{
			AttributeIndex index = queryContext.Index;
			IIndexStore store = ((MemberFunction)_functor).GetStore(index);
			if(store == null)
			{
				if(queryContext.Cache.Count != 0)
					throw new ParserException("Index is not defined for attribute '" + ((MemberFunction)_functor).MemberName + "'");
			}
			else
			{
				var data = store.GetData(_generator.Evaluate(queryContext.AttributeValues), base.Inverse ? ComparisonType.GREATER_THAN_EQUALS : ComparisonType.LESS_THAN);
				if((data != null) && (data.Count > 0))
				{
					IEnumerator enumerator = data.GetEnumerator();
					if(!queryContext.PopulateTree)
					{
						while (enumerator.MoveNext())
							if(queryContext.Tree.LeftList.Contains(enumerator.Current))
								queryContext.Tree.Shift(enumerator.Current);
					}
					else
					{
						queryContext.Tree.RightList = data;
						queryContext.PopulateTree = false;
					}
				}
			}
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal override void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
			AttributeIndex index = queryContext.Index;
			IIndexStore store = ((MemberFunction)_functor).GetStore(index);
			if(store != null)
			{
				var data = store.GetData(_generator.Evaluate(queryContext.AttributeValues), base.Inverse ? ComparisonType.GREATER_THAN_EQUALS : ComparisonType.LESS_THAN);
				if(data != null)
					list.Add(data.Count, data);
			}
			else if(queryContext.Cache.Count != 0)
				throw new ParserException("Index is not defined for attribute '" + ((MemberFunction)_functor).MemberName + "'");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (_functor + (base.Inverse ? " >= " : " < ") + _generator);
		}
	}
}
