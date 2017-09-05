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
using Microsoft.Practices;

namespace Colosoft.Query
{
	/// <summary>
	/// Source context padrão
	/// </summary>
	public class SourceContext : Colosoft.Query.ISourceContext
	{
		private Colosoft.Query.IQueryDataSource _dataSource;

		/// <summary>
		/// Construtor privado.
		/// </summary>
		protected SourceContext()
		{
			try
			{
				_dataSource = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Colosoft.Query.IQueryDataSource>();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(Properties.Resources.Exception_FailOnLoadDataSourceToSourceContext, ex);
			}
		}

		/// <summary>
		/// Cria um nova consulta.
		/// </summary>
		/// <returns></returns>
		public Colosoft.Query.Queryable CreateQuery()
		{
			var query = new Colosoft.Query.Queryable();
			query.DataSource = _dataSource;
			return query;
		}

		/// <summary>
		/// Cria uma nova instância de um container de consultas
		/// </summary>
		/// <returns></returns>
		public Colosoft.Query.MultiQueryable CreateMultiQuery()
		{
			var multiQuery = new Colosoft.Query.MultiQueryable();
			multiQuery.DataSource = _dataSource;
			return multiQuery;
		}

		/// <summary>
		/// Libera a memória dos componentes
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a memória dos componentes
		/// </summary>
		/// <param name="disposing">indica chamada fora do destrutor</param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(this._dataSource != null)
				{
					this._dataSource = null;
				}
			}
		}
	}
}
