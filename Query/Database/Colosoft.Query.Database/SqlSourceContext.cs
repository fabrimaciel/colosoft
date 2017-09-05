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
using Microsoft.Practices.ServiceLocation;

namespace Colosoft.Query.Database
{
	/// <summary>
	/// Contexto da origem de um banco de dados SQL.
	/// </summary>
	public class SqlSourceContext : ISourceContext
	{
		private IQueryDataSource _dataSource;

		/// <summary>
		/// Fonte de dados associada com a instancia.
		/// </summary>
		public IQueryDataSource DataSource
		{
			get
			{
				return _dataSource;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataSource">Fonte de dados</param>
		public SqlSourceContext(IQueryDataSource dataSource)
		{
			dataSource.Require("dataSource").NotNull();
			_dataSource = dataSource;
		}

		/// <summary>
		/// Cria uma nova instancia de consulta.
		/// </summary>
		/// <returns>Retorna uma instância do objeto <see cref="Queryable"/></returns>
		public Queryable CreateQuery()
		{
			return new Queryable() {
				DataSource = _dataSource,
				SourceContext = this
			};
		}

		/// <summary>
		/// Cria uma nova instância de um container de consultas
		/// </summary>
		/// <returns>Retorna uma intância do objeto <see cref="MultiQueryable"/></returns>
		public MultiQueryable CreateMultiQuery()
		{
			return new MultiQueryable() {
				DataSource = _dataSource,
				SourceContext = this
			};
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing">Se está ou não sendo descartado</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
