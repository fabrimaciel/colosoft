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
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação básica para um predicado.
	/// </summary>
	public abstract class Predicate : IPredicate
	{
		private bool _inverse;

		/// <summary>
		/// Identifica se é o inverso do predicado.
		/// </summary>
		public bool Inverse
		{
			get
			{
				return _inverse;
			}
		}

		/// <summary>
		/// Construtor protegido padrão.
		/// </summary>
		protected Predicate()
		{
		}

		/// <summary>
		/// Aplica o predicado sobre a instancia informada.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public abstract bool ApplyPredicate(object o);

		/// <summary>
		/// Inverte o predicado.
		/// </summary>
		public virtual void Invert()
		{
			_inverse = !_inverse;
		}

		/// <summary>
		/// Calcula o predicado para o objeto informado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool Evaluate(object o)
		{
			return (!_inverse == ApplyPredicate(o));
		}

		/// <summary>
		/// Executa o predicado sobre o contexto.
		/// </summary>
		/// <param name="queryContext">Contexto da consulta.</param>
		/// <param name="nextPredicate">Próximo predicado.</param>
		internal virtual void Execute(QueryContext queryContext, Predicate nextPredicate)
		{
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal virtual void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
		}

		/// <summary>
		/// Recalcula o predicado.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="cache"></param>
		/// <param name="attributeValues"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal virtual ArrayList ReEvaluate(AttributeIndex index, LocalCacheBase cache, IDictionary attributeValues, string cacheContext)
		{
			QueryContext queryContext = new QueryContext(cache);
			queryContext.AttributeValues = attributeValues;
			queryContext.Index = index;
			queryContext.CacheContext = cacheContext;
			this.Execute(queryContext, null);
			queryContext.Tree.Reduce();
			return queryContext.Tree.LeftList;
		}
	}
}
