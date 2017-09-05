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
using Colosoft.Query;
using Colosoft.Data;

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação do <see cref="ISourceContext"/> para o cache.
	/// </summary>
	public class CacheSourceContext : ISourceContext, IPersistenceContext
	{
		private ICacheProvider _cacheProvider;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Lazy<Query.IRecordKeyFactory> _keyFactory;

		private CacheDataSource _dataSource;

		/// <summary>
		/// Instancia da origem dos dados.
		/// </summary>
		public IQueryDataSource DataSource
		{
			get
			{
				if(_dataSource == null)
					_dataSource = CreateDataSource();
				return _dataSource;
			}
		}

		/// <summary>
		/// Instancia do cache.
		/// </summary>
		private Cache Cache
		{
			get
			{
				if(_cacheProvider != null)
					return _cacheProvider.Cache;
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheProvider"></param>
		/// <param name="typeSchema"></param>
		/// <param name="keyFactory"></param>
		public CacheSourceContext(ICacheProvider cacheProvider, Colosoft.Data.Schema.ITypeSchema typeSchema, Lazy<Query.IRecordKeyFactory> keyFactory)
		{
			cacheProvider.Require("cacheProvider").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_keyFactory = keyFactory;
		}

		/// <summary>
		/// Cria o origem de dados que será usada pelo contexto.
		/// </summary>
		/// <returns></returns>
		protected virtual CacheDataSource CreateDataSource()
		{
			return new CacheDataSource(_cacheProvider, _typeSchema);
		}

		/// <summary>
		/// Cria uma nova consulta.
		/// </summary>
		/// <returns></returns>
		public Colosoft.Query.Queryable CreateQuery()
		{
			return new Query.Queryable() {
				DataSource = DataSource
			};
		}

		/// <summary>
		/// Cria uma nova instância de um container de consultas
		/// </summary>
		/// <returns></returns>
		public MultiQueryable CreateMultiQuery()
		{
			return new Query.MultiQueryable() {
				DataSource = DataSource
			};
		}

		/// <summary>
		/// Cria uma nova sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		public IPersistenceSession CreateSession()
		{
			return new CachePersistenceSession(_cacheProvider, _typeSchema, _keyFactory);
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
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
