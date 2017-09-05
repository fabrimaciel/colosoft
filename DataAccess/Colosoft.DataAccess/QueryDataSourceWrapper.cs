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

namespace Colosoft.DataAccess
{
	/// <summary>
	/// Implementação adaptativa para o origem de dados de consultas.
	/// </summary>
	class QueryDataSourceWrapper : Colosoft.Query.IQueryDataSource, IDisposable
	{
		private IQueryDataSourceSelector _selector;

		/// <summary>
		/// Identifica se a instancia foi inicializada.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="selector"></param>
		public QueryDataSourceWrapper(IQueryDataSourceSelector selector)
		{
			selector.Require("selector").NotNull();
			_selector = selector;
		}

		/// <summary>
		/// Retorna o resultado de várias queries recebe os dados de uma query e enviando ao SQL Server
		/// </summary>
		/// <param name="queries">Informações das queries</param>
		/// <returns>Retorna o resultado da query</returns>
		public IEnumerable<Query.IQueryResult> Execute(Query.QueryInfo[] queries)
		{
			if(queries.Length == 0)
				return new Query.IQueryResult[0];
			var dataSource = _selector.GetDataSource(queries);
			if(dataSource == null)
				throw new InvalidOperationException("Not found data source for queries");
			return dataSource.Execute(queries);
		}

		/// <summary>
		/// Executa a consulta informada.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Query.IQueryResult Execute(Query.QueryInfo query)
		{
			var dataSource = _selector.GetDataSource(query);
			return dataSource.Execute(query);
		}

		/// <summary>
		/// Libera os dados da instancia.
		/// </summary>
		public void Dispose()
		{
			if(_selector != null)
			{
				_selector.Dispose();
				_selector = null;
			}
		}
	}
}
