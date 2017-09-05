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

namespace Colosoft.Query
{
	/// <summary>
	/// Resulta da consulta para um tipo especifico.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class QueryResult<TModel> : IEnumerable<TModel>, IQueryResult<TModel>
	{
		private IQueryResult _result;

		private IQueryResultBindStrategy _bindStrategy;

		private IQueryResultObjectCreator _objectCreator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result"></param>
		public QueryResult(IQueryResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");
			_result = result;
			var ts = TypeBindStrategyCache.GetItem(typeof(TModel), t => new QueryResultObjectCreator(t));
			_objectCreator = ts;
			_bindStrategy = ts;
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="result">Resultado base da consulta.</param>
		/// <param name="bindStrategy">Estratégia de vinculação que será utilizada.</param>
		/// <param name="objectCreator">Instancia responsável pela criação do tipo do resultado.</param>
		public QueryResult(IQueryResult result, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator = null)
		{
			if(result == null)
				throw new ArgumentNullException("result");
			else if(bindStrategy == null)
				throw new ArgumentNullException("bindStrategy");
			_result = result;
			_bindStrategy = bindStrategy;
			if(objectCreator == null)
				_objectCreator = new QueryResultObjectCreator(typeof(TModel));
			else
				_objectCreator = objectCreator;
		}

		/// <summary>
		/// Recupera o resulta simples.
		/// </summary>
		/// <returns></returns>
		public IQueryResult GetResult()
		{
			return _result;
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TModel> GetEnumerator()
		{
			return _bindStrategy.Bind<TModel>(_result, BindStrategyMode.All, _objectCreator).GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _bindStrategy.Bind<TModel>(_result, BindStrategyMode.All, _objectCreator).GetEnumerator();
		}
	}
}
