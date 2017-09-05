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
using Colosoft.Text.Parser;
using System.Collections;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado de comparação de valores nulos.
	/// </summary>
	public class IsNullPredicate : Predicate, IComparable
	{
		private IFunctor _functor;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="functor"></param>
		public IsNullPredicate(IFunctor functor)
		{
			_functor = functor;
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			return (o == null);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is IsNullPredicate)
				return base.Inverse.CompareTo(((Predicate)obj).Inverse);
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
			var memberFunction = (IMemberFunction)_functor;
			IIndexStore store = memberFunction.GetStore(index);
			if(store == null)
			{
				if(queryContext.Cache.Count != 0)
					throw new ParserException("Index is not defined for attribute '" + ((IMemberFunction)_functor).MemberName + "'");
			}
			else
			{
				var data = memberFunction.GetData(store, queryContext.AttributeValues, base.Inverse ? ComparisonType.NOT_EQUALS : ComparisonType.EQUALS, CacheNull.Value);
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
			var memberFunction = (IMemberFunction)_functor;
			IIndexStore store = memberFunction.GetStore(index);
			if(store != null)
			{
				var data = memberFunction.GetData(store, queryContext.AttributeValues, base.Inverse ? ComparisonType.NOT_EQUALS : ComparisonType.EQUALS, CacheNull.Value);
				if(data != null)
					list.Add(data.Count, data);
			}
			else if(queryContext.Cache.Count != 0)
				throw new ParserException("Index is not defined for attribute '" + ((IMemberFunction)_functor).MemberName + "'");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(!base.Inverse)
				return "is null";
			return "is not null";
		}
	}
}
