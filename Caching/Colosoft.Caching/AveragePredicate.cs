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
	/// Implementação do predicado para calcular o valor médio.
	/// </summary>
	public class AveragePredicate : AggregateFunctionPredicate
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
			decimal num = 0M;
			if(queryContext.Tree.LeftList.Count > 0)
			{
				foreach (string str in queryContext.Tree.LeftList)
				{
					var metaInformation = queryContext.Cache.GetEntryInternal(str, true).MetaInformation;
					if((metaInformation == null) || !metaInformation.IsAttributeIndexed(base.AttributeName))
						throw new Exception("Index is not defined for attribute '" + base.AttributeName + "'");
					object obj2 = metaInformation[base.AttributeName];
					if(obj2 != null)
					{
						Type type = obj2.GetType();
						if(((type == typeof(bool)) || (type == typeof(DateTime))) || ((type == typeof(string)) || (type == typeof(char))))
							throw new Exception("AVG can only be applied to integral data types.");
						num += Convert.ToDecimal(obj2);
					}
				}
				AverageResult result = new AverageResult();
				result.Sum = num;
				result.Count = queryContext.Tree.LeftList.Count;
				base.SetResult(queryContext, AggregateFunctionType.AVG, result);
			}
			else
			{
				base.SetResult(queryContext, AggregateFunctionType.AVG, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal override void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
		}
	}
}
