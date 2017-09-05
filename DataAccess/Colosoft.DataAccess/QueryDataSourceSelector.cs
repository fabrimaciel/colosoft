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
	/// Implementação padrão do selector de origem de dados de consulta.
	/// </summary>
	public class QueryDataSourceSelector : IQueryDataSourceSelector
	{
		/// <summary>
		/// Hash que armazena a relação da entidade com a origem de dados.
		/// </summary>
		private Dictionary<string, Colosoft.Query.IQueryDataSource> _entityDataSource = new Dictionary<string, Query.IQueryDataSource>();

		private Lazy<Colosoft.Data.Caching.IDataCacheManager> _dataCacheManager;

		private Lazy<Query.IQueryDataSource> _cacheDataSource;

		private Lazy<Query.IQueryDataSource> _serverDataSource;

		private Dynamic.DynamicQueryDataSource _dynamicQueryDataSource;

		private object _objLock = new object();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataCacheManager"></param>
		/// <param name="cacheDataSource"></param>
		/// <param name="serverDataSource"></param>
		/// <param name="typeSchema"></param>
		/// <param name="keyFactory"></param>
		/// <param name="logger"></param>
		public QueryDataSourceSelector(Lazy<Colosoft.Data.Caching.IDataCacheManager> dataCacheManager, Lazy<Query.IQueryDataSource> cacheDataSource, Lazy<Query.IQueryDataSource> serverDataSource, Lazy<Colosoft.Data.Schema.ITypeSchema> typeSchema, Lazy<Colosoft.Query.IRecordKeyFactory> keyFactory, Colosoft.Logging.ILogger logger)
		{
			dataCacheManager.Require("dataCacheManager").NotNull();
			cacheDataSource.Require("cacheDataSource").NotNull();
			serverDataSource.Require("serverDataSource").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			_dataCacheManager = dataCacheManager;
			_cacheDataSource = cacheDataSource;
			_serverDataSource = serverDataSource;
			_dynamicQueryDataSource = new Dynamic.DynamicQueryDataSource(_serverDataSource, dataCacheManager, typeSchema, keyFactory, logger);
		}

		/// <summary>
		/// Recupera o origem de dados do cache.
		/// </summary>
		/// <returns></returns>
		public virtual Colosoft.Query.IQueryDataSource GetCacheDataSource()
		{
			return _cacheDataSource.Value;
		}

		/// <summary>
		/// Recupera a origem de dados do servidor.
		/// </summary>
		/// <returns></returns>
		public virtual Colosoft.Query.IQueryDataSource GetServerDataSource()
		{
			return _dynamicQueryDataSource;
		}

		/// <summary>
		/// Recupera o origem de dados associada com as informações da entidade informada.
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <returns></returns>
		protected virtual Colosoft.Query.IQueryDataSource GetDataSource(Query.EntityInfo entityInfo)
		{
			Colosoft.Query.IQueryDataSource dataSource = null;
			if(string.IsNullOrEmpty(entityInfo.FullName) && entityInfo.SubQuery != null)
				return GetDataSource(entityInfo.SubQuery);
			lock (_objLock)
				if(_entityDataSource.TryGetValue(entityInfo.FullName, out dataSource))
					return dataSource;
			var typeName = _dataCacheManager.Value.Where(f => f.FullName == entityInfo.FullName).FirstOrDefault();
			if(typeName != null)
			{
				if(_dataCacheManager.Value.Cache != null && _dataCacheManager.Value.Cache.IsLoaded)
					dataSource = GetCacheDataSource();
				else
					return GetServerDataSource();
			}
			if(dataSource == null || !dataSource.IsInitialized)
				dataSource = GetServerDataSource();
			lock (_objLock)
				if(!_entityDataSource.ContainsKey(entityInfo.FullName))
					_entityDataSource.Add(entityInfo.FullName, dataSource);
			return dataSource;
		}

		/// <summary>
		/// Verifica se nas informações da consulta existe algum inner join.
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		protected virtual bool ContainsJoin(Colosoft.Query.QueryInfo queryInfo)
		{
			if(queryInfo.Joins != null && queryInfo.Joins.Length > 0)
				return true;
			if(queryInfo.NestedQueries != null)
				foreach (var nestedQuery in queryInfo.NestedQueries)
					if(ContainsJoin(nestedQuery))
						return true;
			return false;
		}

		/// <summary>
		/// Apaga todos os registros de DataSource em cache.
		/// </summary>
		public void Reset()
		{
			lock (_objLock)
				_entityDataSource.Clear();
		}

		/// <summary>
		/// Recupera o origem de dados associada com a consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Query.IQueryDataSource GetDataSource(Query.QueryInfo query)
		{
			Query.IQueryDataSource lastDataSource = null;
			if(query.StoredProcedureName != null || !query.CanUseCache || !Colosoft.Caching.CacheDataSource.IsCompatible(query))
				return GetServerDataSource();
			foreach (var i in query.Entities)
			{
				var ds = GetDataSource(i);
				if(lastDataSource != null && lastDataSource != ds)
					return GetServerDataSource();
				lastDataSource = ds;
			}
			if(query.NestedQueries != null)
				foreach (var nestedQuery in query.NestedQueries)
				{
					var ds = GetDataSource(nestedQuery);
					if((lastDataSource != null && lastDataSource != ds) || !nestedQuery.CanUseCache)
						return GetServerDataSource();
					lastDataSource = ds;
				}
			return lastDataSource;
		}

		/// <summary>
		/// Recupera a origem de dados comum entre as consulta informadas.
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public Query.IQueryDataSource GetDataSource(Query.QueryInfo[] queries)
		{
			Query.IQueryDataSource lastDataSource = null;
			foreach (var i in queries)
			{
				var ds = GetDataSource(i);
				if(lastDataSource != null && lastDataSource != ds)
					return GetServerDataSource();
				lastDataSource = ds;
			}
			return lastDataSource;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_dynamicQueryDataSource")]
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
