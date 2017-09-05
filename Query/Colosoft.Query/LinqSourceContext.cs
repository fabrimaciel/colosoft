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
	/// Implementação do contexto de origem do linq.
	/// </summary>
	class LinqSourceContext : ILinqSourceContext
	{
		private ISourceContext _sourceContext;

		private ISourceProvider _sourceProvider;

		/// <summary>
		/// Provedor associado.
		/// </summary>
		public ISourceProvider Provider
		{
			get
			{
				return _sourceProvider;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sourceContext"></param>
		/// <param name="sourceProvider"></param>
		public LinqSourceContext(ISourceContext sourceContext, ISourceProvider sourceProvider)
		{
			_sourceContext = sourceContext;
			_sourceProvider = sourceProvider;
		}

		/// <summary>
		/// Cria uma consulta do Linq.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <returns></returns>
		public IQueryable<TEntity> CreateQuery<TEntity>() where TEntity : new()
		{
			return new Queryable<TEntity>(this);
		}

		/// <summary>
		/// Cria uma consulta.
		/// </summary>
		/// <returns></returns>
		public Queryable CreateQuery()
		{
			return _sourceContext.CreateQuery();
		}

		/// <summary>
		/// Cria uma multiconsulta.
		/// </summary>
		/// <returns></returns>
		public MultiQueryable CreateMultiQuery()
		{
			return _sourceContext.CreateMultiQuery();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			_sourceProvider.Dispose();
		}
	}
}
