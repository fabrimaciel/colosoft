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
	/// Implementação do do resultado de uma consulta que possui a
	/// estratégia de vinculação associada.
	/// </summary>
	public class BindableQueryResult : IQueryResultContainer, System.Collections.IEnumerable
	{
		private IQueryResult _result;

		private IQueryResultBindStrategy _bindStrategy;

		private IQueryResultObjectCreator _objectCreator;

		/// <summary>
		/// Identificador do resultado da consulta.
		/// </summary>
		public int Id
		{
			get
			{
				return _result.Id;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result">Instancia do resultado da consulta.</param>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator">Instancia do criador dos objetos.</param>
		public BindableQueryResult(IQueryResult result, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator)
		{
			_result = result;
			_bindStrategy = bindStrategy;
			_objectCreator = objectCreator;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~BindableQueryResult()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera uma instancia de TModel pelo registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private object Get(Record record)
		{
			object instance = _objectCreator.Create();
			_bindStrategy.Bind(record, BindStrategyMode.All, ref instance);
			return instance;
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
		/// Recupera o enumerador dos objetos do resultado.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var i in _result)
				yield return Get(i);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_result.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
