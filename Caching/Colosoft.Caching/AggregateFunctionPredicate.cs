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
	/// Implementação do predicado para uma função de agregação.
	/// </summary>
	public class AggregateFunctionPredicate : Predicate, IComparable
	{
		/// <summary>
		/// Nome do atributo.
		/// </summary>
		public string AttributeName
		{
			get;
			set;
		}

		/// <summary>
		/// Predicado filho.
		/// </summary>
		public Predicate ChildPredicate
		{
			get;
			set;
		}

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
		/// Realiza a comparação com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			AggregateFunctionPredicate predicate = obj as AggregateFunctionPredicate;
			if(predicate != null)
				return ((IComparable)this.ChildPredicate).CompareTo(predicate.ChildPredicate);
			return -1;
		}

		/// <summary>
		/// Define o resultado.
		/// </summary>
		/// <param name="queryContext">Contexto da pesquisa.</param>
		/// <param name="functionType">Tipo de função.</param>
		/// <param name="result">Resultado.</param>
		internal void SetResult(QueryContext queryContext, AggregateFunctionType functionType, object result)
		{
			QueryResultSet set = new QueryResultSet();
			set.Type = QueryType.AggregateFunction;
			set.AggregateFunctionType = functionType;
			set.AggregateFunctionResult = new DictionaryEntry(functionType, result);
			queryContext.ResultSet = set;
		}
	}
}
