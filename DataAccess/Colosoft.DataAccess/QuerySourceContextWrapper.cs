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
	/// Implementação do contexto de origem.
	/// </summary>
	class QuerySourceContextWrapper : Colosoft.Query.ISourceContext
	{
		private QueryDataSourceWrapper _dataSource;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataSource"></param>
		public QuerySourceContextWrapper(QueryDataSourceWrapper dataSource)
		{
			dataSource.Require("dataSource").NotNull();
			_dataSource = dataSource;
		}

		/// <summary>
		/// Cria uma nova consulta.
		/// </summary>
		/// <returns></returns>
		public Query.Queryable CreateQuery()
		{
			var query = new Query.Queryable();
			query.DataSource = _dataSource;
			query.SourceContext = this;
			return query;
		}

		/// <summary>
		/// Cria uma nova instância de um container de consultas
		/// </summary>
		/// <returns></returns>
		public Query.MultiQueryable CreateMultiQuery()
		{
			var multiQuery = new Colosoft.Query.MultiQueryable();
			multiQuery.DataSource = _dataSource;
			multiQuery.SourceContext = this;
			return multiQuery;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_dataSource != null)
			{
				_dataSource.Dispose();
				_dataSource = null;
			}
		}
	}
}
