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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado para a função MIN.
	/// </summary>
	public class MinPredicate : AggregateFunctionPredicate
	{
		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			return false;
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="nextPredicate"></param>
		internal override void Execute(QueryContext queryContext, Predicate nextPredicate)
		{
			base.ChildPredicate.Execute(queryContext, nextPredicate);
			queryContext.Tree.Reduce();
			bool flag = false;
			IComparable comparable = null;
			Type type = null;
			foreach (string str in queryContext.Tree.LeftList)
			{
				MetaInformation metaInformation = queryContext.Cache.GetEntryInternal(str, true).MetaInformation;
				if((metaInformation == null) || !metaInformation.IsAttributeIndexed(base.AttributeName))
					throw new Exception("Index is not defined for attribute '" + base.AttributeName + "'");
				IComparable comparable2 = (IComparable)metaInformation[base.AttributeName];
				if(comparable2 != null)
				{
					type = comparable2.GetType();
					if(type == typeof(bool))
						throw new Exception("MIN cannot be applied to Boolean data type.");
					if(!flag)
					{
						comparable = comparable2;
						flag = true;
					}
					if(comparable2.CompareTo(comparable) < 0)
						comparable = comparable2;
				}
			}
			if(((type != typeof(DateTime)) && (type != typeof(string))) && ((type != typeof(char)) && (comparable != null)))
				base.SetResult(queryContext, AggregateFunctionType.MIN, Convert.ToDecimal(comparable));
			else
				base.SetResult(queryContext, AggregateFunctionType.MIN, comparable);
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal override void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
		}
	}
}
