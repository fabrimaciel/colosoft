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

namespace Colosoft.Query.Linq
{
	/// <summary>
	/// Implementação do provedor de origem associado a uma consulta do Colosoft.
	/// </summary>
	class QueryableSourceProvider : ISourceProvider
	{
		private Query.Queryable _queryable;

		private Func<Query.Queryable, System.Collections.IEnumerable> _executeSelect;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="executeSelect">Referencia do método que será usado para executar a seleção dos dados.</param>
		public QueryableSourceProvider(Query.Queryable queryable, Func<Query.Queryable, System.Collections.IEnumerable> executeSelect)
		{
			_queryable = queryable;
			_executeSelect = executeSelect;
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <param name="query">Expressão da consulta.</param>
		/// <returns></returns>
		public object Execute(System.Linq.Expressions.Expression query)
		{
			var translator = new Colosoft.Query.Linq.Translators.QueryTranslator();
			var queryable = (Query.Queryable)_queryable.Clone();
			translator.Apply(queryable, query);
			if(translator.UseMethod == QueryMethod.Select)
				return ExecuteSelect(queryable);
			else if(translator.UseMethod == QueryMethod.Count)
				return queryable.Execute().Select(f => translator.LongCount ? (object)f.GetInt64(0) : f.GetInt32(0)).FirstOrDefault();
			return queryable.Execute().Select(f => f.GetValue(0));
		}

		/// <summary>
		/// Executa a consulta que recupera uma seleção de valores.
		/// </summary>
		/// <param name="queryable"></param>
		/// <returns></returns>
		protected virtual System.Collections.IEnumerable ExecuteSelect(Query.Queryable queryable)
		{
			if(_executeSelect != null)
				return _executeSelect(queryable);
			return queryable.Execute();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
