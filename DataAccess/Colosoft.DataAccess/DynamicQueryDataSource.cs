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

namespace Colosoft.DataAccess.Dynamic
{
	/// <summary>
	/// Implementação da origem de dados para consulta dinamica.
	/// </summary>
	public class DynamicQueryDataSource : IQueryDataSource, IDisposable
	{
		private Lazy<IQueryDataSource> _serverQueryDataSource;

		private Lazy<Colosoft.Data.Caching.IDataCacheManager> _dataCacheManager;

		private Lazy<Colosoft.Data.Schema.ITypeSchema> _typeSchema;

		private Lazy<Colosoft.Query.IRecordKeyFactory> _keyFactory;

		/// <summary>
		/// Instancia do origem de dados da consulta do servidor.
		/// </summary>
		public IQueryDataSource ServerQueryDataSource
		{
			get
			{
				return _serverQueryDataSource.Value;
			}
		}

		/// <summary>
		/// Instancia do gerenciador de dados do cache.
		/// </summary>
		public Colosoft.Data.Caching.IDataCacheManager DataCacheManager
		{
			get
			{
				return _dataCacheManager.Value;
			}
		}

		/// <summary>
		/// Instancia da fabrica dos chaves que são inseridas no cache.
		/// </summary>
		private Colosoft.Query.IRecordKeyFactory KeyFactory
		{
			get
			{
				return _keyFactory.Value;
			}
		}

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
		/// <param name="serverQueryDataSource">Instancia da origem de ados do query do servidor.</param>
		/// <param name="dataCacheManager">Instancia do gerenciadoir de dados do cache.</param>
		/// <param name="typeSchema">Instancia com os esquemas dos tipos do sistema.</param>
		/// <param name="keyFactory">Instacia da factory das chaves.</param>
		/// <param name="logger">Instancia resposável pelo registro dos logs.</param>
		public DynamicQueryDataSource(Lazy<IQueryDataSource> serverQueryDataSource, Lazy<Colosoft.Data.Caching.IDataCacheManager> dataCacheManager, Lazy<Colosoft.Data.Schema.ITypeSchema> typeSchema, Lazy<Colosoft.Query.IRecordKeyFactory> keyFactory, Colosoft.Logging.ILogger logger)
		{
			serverQueryDataSource.Require("serverQueryDataSource").NotNull();
			dataCacheManager.Require("dataCacheManager").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			_serverQueryDataSource = serverQueryDataSource;
			_dataCacheManager = dataCacheManager;
			_typeSchema = typeSchema;
			_keyFactory = keyFactory;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DynamicQueryDataSource()
		{
			Dispose(false);
		}

		/// <summary>
		/// Executa as consulta informadas no banco de dados.
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public IEnumerable<IQueryResult> Execute(QueryInfo[] queries)
		{
			queries.Require("queries").NotNull();
			foreach (var query in queries)
				if(query.StoredProcedureName == null && query.Entities != null && query.Entities.Length > 0 && query.CanUseCache)
					new QueryInfoListener(this, query);
			return ServerQueryDataSource.Execute(queries);
		}

		/// <summary>
		/// Executa a consulta informada.
		/// </summary>
		/// <param name="query">Instância com os dados da consulta que será executada.</param>
		/// <returns></returns>
		public IQueryResult Execute(QueryInfo query)
		{
			query.Require("query").NotNull();
			if(query.StoredProcedureName == null && query.Entities != null && query.Entities.Length > 0 && query.CanUseCache)
				new QueryInfoListener(this, query);
			return ServerQueryDataSource.Execute(query);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		sealed class QueryInfoListener : IQueryResultObserver
		{
			private DynamicQueryDataSource _dataSource;

			private QueryInfo _root;

			private Colosoft.Reflection.TypeName _entityTypeName;

			/// <summary>
			/// Nome da entidade associada com a consulta.
			/// </summary>
			public Colosoft.Reflection.TypeName EntityTypeName
			{
				get
				{
					if(_entityTypeName == null)
					{
						_entityTypeName = new Colosoft.Reflection.TypeName(_root.Entities[0].FullName);
					}
					return _entityTypeName;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="dataSource"></param>
			/// <param name="queryInfo"></param>
			public QueryInfoListener(DynamicQueryDataSource dataSource, QueryInfo queryInfo)
			{
				_dataSource = dataSource;
				_root = queryInfo;
				AnalysisQueryInfo(_root, true);
			}

			/// <summary>
			/// Analisa as informações da consulta.
			/// </summary>
			/// <param name="queryInfo"></param>
			/// <param name="includeObserver"></param>
			private void AnalysisQueryInfo(QueryInfo queryInfo, bool includeObserver)
			{
				if(queryInfo == null)
					return;
				if(queryInfo.NestedQueries != null)
				{
					foreach (var i in queryInfo.NestedQueries)
						AnalysisQueryInfo(i, includeObserver);
				}
				if(queryInfo.Joins == null || queryInfo.Joins.Length == 0)
				{
					if(includeObserver)
						queryInfo.ResultObserver = this;
					else
						queryInfo.ResultObserver = null;
				}
			}

			/// <summary>
			/// Identifica que inicioa o processamento do
			/// resultado de uma das consultas.
			/// </summary>
			/// <param name="result"></param>
			public void BeginProcessing(IQueryResult result)
			{
			}

			/// <summary>
			/// Identifica que finalizou o processamento do
			/// resultado de uma das consultas.
			/// </summary>
			/// <param name="result"></param>
			public void EndProcessing(IQueryResult result)
			{
				var result2 = result as IQueryResultExt;
				if(result2 != null && result2.QueryInfo == _root)
					AnalysisQueryInfo(_root, false);
			}

			/// <summary>
			/// Método acionado quando o registro de um resultado for carregado.
			/// </summary>
			/// <param name="result"></param>
			/// <param name="record"></param>
			public void LoadRecord(IQueryResult result, Record record)
			{
			}

			/// <summary>
			/// Método acionado quando ocorrer algum erro no processamento do resultado.
			/// </summary>
			/// <param name="result"></param>
			/// <param name="exception">Instancia do erro ocorrido.</param>
			public void Error(IQueryResult result, Exception exception)
			{
				AnalysisQueryInfo(_root, false);
			}
		}

		/// <summary>
		/// Implementação da tarefa assincrona para registrar o cache
		/// o registro carregado.
		/// </summary>
		class RegisterRecordAsyncTask : Colosoft.Threading.IAsyncTask
		{
			private object[] _recordValues;

			private Record.RecordDescriptor _recordDescriptor;

			private DynamicQueryDataSource _dataSource;

			private Colosoft.Reflection.TypeName _entityTypeName;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="dataSource"></param>
			/// <param name="recordValues">Instancia do registro que será processado.</param>
			/// <param name="recordDescriptor"></param>
			/// <param name="entityTypeName">Nome do tipo da entidade associada.</param>
			public RegisterRecordAsyncTask(DynamicQueryDataSource dataSource, object[] recordValues, Record.RecordDescriptor recordDescriptor, Colosoft.Reflection.TypeName entityTypeName)
			{
				_dataSource = dataSource;
				_recordValues = recordValues;
				_recordDescriptor = recordDescriptor;
				_entityTypeName = entityTypeName;
			}

			/// <summary>
			/// Procesas a tarefa
			/// </summary>
			public void Process()
			{
				try
				{
					var cacheItemRecord = new Colosoft.Caching.CacheItemRecord(_entityTypeName, _recordValues, _recordDescriptor);
					var cache = _dataSource.DataCacheManager.Cache;
				}
				catch(Exception)
				{
				}
			}
		}
	}
}
