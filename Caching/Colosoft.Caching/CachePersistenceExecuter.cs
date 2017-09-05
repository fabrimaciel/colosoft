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
using Colosoft.Data;
using Colosoft.Query;

namespace Colosoft.Caching
{
	/// <summary>
	/// Executor de persitência para o cache
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class CachePersistenceExecuter : IPersistenceExecuter
	{
		private ICacheProvider _cacheProvider;

		private CacheSourceContext _sourceContext = null;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Lazy<Query.IRecordKeyFactory> _keyFactory;

		/// <summary>
		/// Instancia do cache.
		/// </summary>
		protected Cache Cache
		{
			get
			{
				return _cacheProvider.Cache;
			}
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="cacheProvider">Objeto do cache</param>
		/// <param name="keyFactory"></param>
		/// <param name="typeSchema">Instancia do esquema dos tipos do sistema.</param>
		public CachePersistenceExecuter(ICacheProvider cacheProvider, Lazy<Query.IRecordKeyFactory> keyFactory, Colosoft.Data.Schema.ITypeSchema typeSchema)
		{
			cacheProvider.Require("cache").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_sourceContext = new CacheSourceContext(cacheProvider, typeSchema, keyFactory);
			_keyFactory = keyFactory;
		}

		/// <summary>
		/// Executa.
		/// </summary>
		/// <param name="actions">Ações a serem executadas</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns>Retorna um vetor resultados das ações</returns>
		public PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType)
		{
			bool existErrors = false;
			return new PersistenceExecuteResult(Execute(actions, out existErrors));
		}

		/// <summary>
		/// Salva o registro informado no cache. Caso já exista, o mesmo será substituído.
		/// </summary>
		/// <param name="cache">Instancia do cache onde o registro será salvo.</param>
		/// <param name="keyFactory">Factory usado para criar as chave do registro no cache.</param>
		/// <param name="typeName">Nome do tipo que o registro representa.</param>
		/// <param name="record">Registro com os dados que serão salvos.</param>
		/// <param name="isUpdated">Identifica se ocorreu uma atualização.</param>
		/// <returns></returns>
		public static ulong Save(Cache cache, IRecordKeyFactory keyFactory, Colosoft.Reflection.TypeName typeName, CacheItemRecord record, out bool isUpdated)
		{
			var keyGenerator = keyFactory.CreateGenerator(typeName);
			var key = keyGenerator.GetKey(record);
			var operationContext = new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation);
			TypeInfoMap typeInfoMap = cache.GetTypeInfoMap();
			var handleId = typeInfoMap.GetHandleId(typeName);
			isUpdated = false;
			if(typeInfoMap.GetAttribList(handleId).Count == 0)
				return 0;
			if(cache.Contains(key, operationContext))
			{
				cache.Remove(key, operationContext);
				isUpdated = true;
			}
			var queryInfo = GetCacheEntryQueryInfo(cache, typeInfoMap, record);
			var result = cache.Insert(key, record, null, null, new Colosoft.Caching.Policies.PriorityEvictionHint(CacheItemPriority.Normal), null, null, queryInfo, new BitSet(), null, 0, LockAccessType.IGNORE_LOCK, null, null, operationContext);
			return result;
		}

		/// <summary>
		/// Executa as ações informadas.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="existErrors">Identifica se existe error na execução das ações.</param>
		/// <returns></returns>
		private PersistenceActionResult[] Execute(PersistenceAction[] actions, out bool existErrors)
		{
			var results = new PersistenceActionResult[actions.Length];
			for(var i = 0; i < actions.Length; i++)
			{
				results[i] = Execute(actions[i], out existErrors);
				if(existErrors)
				{
					for(++i; i < actions.Length; i++)
						results[i] = new PersistenceActionResult {
							Success = false,
							FailureMessage = "Not executed"
						};
					return results;
				}
			}
			existErrors = false;
			return results;
		}

		/// <summary>
		/// Executa a ação informada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="existErrors">Identifica se existem erros na execução da açõa.</param>
		/// <returns></returns>
		private PersistenceActionResult Execute(PersistenceAction action, out bool existErrors)
		{
			PersistenceActionResult[] beforeActions = null;
			if(action.BeforeActions.Count > 0)
			{
				beforeActions = Execute(action.BeforeActions.ToArray(), out existErrors);
				if(existErrors)
				{
					return new PersistenceActionResult {
						Success = false,
						FailureMessage = beforeActions.Where(f => !f.Success).FirstOrDefault().FailureMessage,
						BeforeActions = beforeActions
					};
				}
			}
			PersistenceActionResult result = null;
			try
			{
				switch(action.Type)
				{
				case PersistenceActionType.Insert:
					result = ExecuteInsertAction(action);
					break;
				case PersistenceActionType.Update:
					result = ExecuteUpdateAction(action);
					break;
				case PersistenceActionType.Delete:
					result = ExecuteDeleteAction(action);
					break;
				default:
					result = new PersistenceActionResult {
						Success = true
					};
					break;
				}
			}
			catch(Exception ex)
			{
				result = new PersistenceActionResult {
					Success = false,
					FailureMessage = ex.Message
				};
			}
			if(!result.Success)
				existErrors = true;
			result.BeforeActions = beforeActions;
			if(result.Success && action.AfterActions.Count > 0)
			{
				result.AfterActions = Execute(action.AfterActions.ToArray(), out existErrors);
				if(existErrors)
				{
					result.FailureMessage = result.AfterActions.Where(f => !f.Success).FirstOrDefault().FailureMessage;
					result.Success = false;
					return result;
				}
			}
			existErrors = false;
			return result;
		}

		/// <summary>
		/// Executa a ação para inserir um novo registro no cache.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		private PersistenceActionResult ExecuteInsertAction(PersistenceAction action)
		{
			var typeMetadata = _typeSchema.GetTypeMetadata(action.EntityFullName);
			if(typeMetadata == null)
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_TypeMetadataNotFound, action.EntityFullName).Format(),
				};
			IEnumerable<Colosoft.Query.Record.Field> recordFields = null;
			try
			{
				recordFields = GetRecordFields(action.EntityFullName, typeMetadata);
			}
			catch(Exception ex)
			{
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ex.Message
				};
			}
			var recordDescriptor = new Colosoft.Query.Record.RecordDescriptor(action.EntityFullName, recordFields);
			var recordValues = new object[recordDescriptor.Count];
			foreach (var actionParameter in action.Parameters)
			{
				var indexOf = 0;
				for(; indexOf < recordDescriptor.Count; indexOf++)
					if(StringComparer.InvariantCultureIgnoreCase.Equals(recordDescriptor[indexOf].Name, actionParameter.Name))
						break;
				if(indexOf < recordDescriptor.Count)
					recordValues[indexOf] = actionParameter.Value;
				else
					return new PersistenceActionResult {
						Success = false,
						FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_PropertyMetadataNotFound, actionParameter.Name, action.EntityFullName).Format()
					};
			}
			var record = new CacheItemRecord(new Reflection.TypeName(action.EntityFullName), recordValues, recordDescriptor);
			var typeName = new Reflection.TypeName(action.EntityFullName);
			var keyGenerator = _keyFactory.Value.CreateGenerator(typeName);
			var key = keyGenerator.GetKey(record);
			var queryInfo = GetCacheEntryQueryInfo(Cache, Cache.GetTypeInfoMap(), record);
			var insertResult = Cache.Insert(key, record, null, null, new Colosoft.Caching.Policies.PriorityEvictionHint(CacheItemPriority.Normal), null, null, queryInfo, new BitSet(), null, 0, LockAccessType.IGNORE_LOCK, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
			return new PersistenceActionResult {
				Success = true
			};
		}

		/// <summary>
		/// Recupera as informações de consulta para a entrada do cache
		/// </summary>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="typeInfoMap"></param>
		/// <param name="record"></param>
		/// <returns></returns>
		private static System.Collections.Hashtable GetCacheEntryQueryInfo(Cache cache, TypeInfoMap typeInfoMap, CacheItemRecord record)
		{
			var queryInfo = new System.Collections.Hashtable();
			if(typeInfoMap != null)
				queryInfo["query-info"] = Colosoft.Caching.Loaders.CacheLoaderUtil.GetQueryInfo(record, typeInfoMap);
			queryInfo["tag-info"] = Colosoft.Caching.Loaders.CacheLoaderUtil.GetTagInfo(record, new Tag[0]);
			var hashtable2 = Colosoft.Caching.Loaders.CacheLoaderUtil.GetNamedTagsInfo(record, new NamedTagsDictionary(), typeInfoMap);
			if(hashtable2 != null)
				queryInfo["named-tag-info"] = hashtable2;
			return queryInfo;
		}

		/// <summary>
		/// Recupera os campos do registro.
		/// </summary>
		/// <param name="entityFullName"></param>
		/// <param name="typeMetadata"></param>
		/// <returns></returns>
		private static List<Record.Field> GetRecordFields(string entityFullName, Colosoft.Data.Schema.ITypeMetadata typeMetadata)
		{
			var recordFields = new List<Colosoft.Query.Record.Field>();
			foreach (var propertyMetadata in typeMetadata)
			{
				Type propertyType = null;
				try
				{
					propertyType = Type.GetType(propertyMetadata.PropertyType, true);
				}
				catch(Exception ex)
				{
					throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_GetPropertyTypeFromPropertyMetadataError, propertyMetadata.PropertyType, propertyMetadata.Name, entityFullName).Format(), ex);
				}
				recordFields.Add(new Colosoft.Query.Record.Field(propertyMetadata.Name, propertyType));
			}
			if(typeMetadata.IsVersioned && !recordFields.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, "RowVersion")))
				recordFields.Add(new Colosoft.Query.Record.Field("RowVersion", typeof(long)));
			return recordFields;
		}

		/// <summary>
		/// Executa a ação para atualizar os dados do registro no cache.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		private PersistenceActionResult ExecuteUpdateAction(PersistenceAction action)
		{
			var typeName = new Reflection.TypeName(action.EntityFullName);
			var typeMetadata = _typeSchema.GetTypeMetadata(action.EntityFullName);
			if(typeMetadata == null)
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_TypeMetadataNotFound, action.EntityFullName).Format(),
				};
			var query = GetActionQuery(action, typeMetadata).CreateQueryInfo();
			Queries.QueryResultSet queryResult = null;
			try
			{
				queryResult = ((CacheDataSource)_sourceContext.DataSource).ExecuteInCache(query);
			}
			catch(Exception ex)
			{
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_ExecuteQueryInCacheError, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true)).Format()
				};
			}
			Colosoft.Query.Record.RecordDescriptor recordDescriptor = null;
			try
			{
				recordDescriptor = new Colosoft.Query.Record.RecordDescriptor(action.EntityFullName, GetRecordFields(action.EntityFullName, typeMetadata));
			}
			catch(Exception ex)
			{
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ex.Message
				};
			}
			var keyGenerator = _keyFactory.Value.CreateGenerator(typeName);
			var typeInfoMap = Cache.GetTypeInfoMap();
			var keysResult = queryResult.SearchKeysResult.ToArray();
			if(keysResult.Length > 0)
			{
				foreach (var key in keysResult)
				{
					var record = CacheDataSource.CreateRecord(Cache, key, ref recordDescriptor, query);
					var recordValues = new object[recordDescriptor.Count];
					record.GetValues(recordValues);
					foreach (var actionParameter in action.Parameters)
					{
						var indexOf = 0;
						for(; indexOf < recordDescriptor.Count; indexOf++)
							if(StringComparer.InvariantCultureIgnoreCase.Equals(recordDescriptor[indexOf].Name, actionParameter.Name))
								break;
						if(indexOf < recordDescriptor.Count)
						{
							recordValues[indexOf] = actionParameter.Value;
						}
					}
					var newRecord = new CacheItemRecord(new Reflection.TypeName(action.EntityFullName), recordValues, recordDescriptor);
					var newKey = keyGenerator.GetKey(newRecord);
					try
					{
						var queryInfo = GetCacheEntryQueryInfo(Cache, typeInfoMap, newRecord);
						Cache.Remove(key, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						var insertResult = Cache.Insert(newKey, newRecord, null, null, new Colosoft.Caching.Policies.PriorityEvictionHint(CacheItemPriority.Normal), null, null, queryInfo, new BitSet(), null, 0, LockAccessType.IGNORE_LOCK, null, null, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					}
					catch(Exception ex)
					{
						return new PersistenceActionResult {
							Success = false,
							FailureMessage = ex.Message
						};
					}
				}
			}
			else
			{
			}
			return new PersistenceActionResult {
				Success = true
			};
		}

		/// <summary>
		/// Recupera usado para recupera a consulta associada com a ação.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeMetadata"></param>
		/// <returns></returns>
		private Query.Queryable GetActionQuery(PersistenceAction action, Colosoft.Data.Schema.ITypeMetadata typeMetadata)
		{
			var query = _sourceContext.CreateQuery();
			StringBuilder wherePart = new StringBuilder();
			List<QueryParameter> wherePartParameters = new List<QueryParameter>();
			bool isFirstWhere = true;
			for(int i = 0; i < action.Parameters.Count; i++)
			{
				var propertyMetadata = typeMetadata[action.Parameters[i].Name];
				if(propertyMetadata != null && (propertyMetadata.ParameterType == Colosoft.Data.Schema.PersistenceParameterType.IdentityKey || propertyMetadata.ParameterType == Colosoft.Data.Schema.PersistenceParameterType.Key))
				{
					if(action.Conditional != null)
						continue;
					wherePartParameters.Add(new QueryParameter("?" + action.Parameters[i].Name, action.Parameters[i].Value));
					if(!isFirstWhere)
						wherePart.Append(" && ");
					else
						isFirstWhere = false;
					wherePart.Append(action.Parameters[i].Name).Append("=?").Append(action.Parameters[i].Name);
				}
			}
			if(action.Conditional == null && wherePart.Length > 0)
				query.WhereClause = ConditionalContainer.Parse(wherePart.ToString(), wherePartParameters.ToArray());
			else if(action.Conditional != null)
				query.WhereClause = action.Conditional;
			query.From(new EntityInfo() {
				Alias = null,
				FullName = typeMetadata.FullName
			});
			return query;
		}

		/// <summary>
		/// Executa a ação para apagar os ados do registro no cache.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		private PersistenceActionResult ExecuteDeleteAction(PersistenceAction action)
		{
			var typeMetadata = _typeSchema.GetTypeMetadata(action.EntityFullName);
			if(typeMetadata == null)
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_TypeMetadataNotFound, action.EntityFullName).Format(),
				};
			var query = GetActionQuery(action, typeMetadata).CreateQueryInfo();
			Queries.QueryResultSet queryResult = null;
			try
			{
				queryResult = ((CacheDataSource)_sourceContext.DataSource).ExecuteInCache(query);
			}
			catch(Exception ex)
			{
				return new PersistenceActionResult {
					Success = false,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.CachePersistenceExecuter_ExecuteQueryInCacheError, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true)).Format()
				};
			}
			var typeName = new Colosoft.Reflection.TypeName(action.EntityFullName);
			var keyGenerator = _keyFactory.Value.CreateGenerator(typeName);
			var recordKeys = new List<RecordKey>();
			using (var recordEnumerator = new Colosoft.Caching.Queries.QueryResultSetRecordEnumerator(_typeSchema, Cache, queryResult, query))
			{
				while (recordEnumerator.MoveNext())
				{
					recordKeys.Add(RecordKeyFactory.Instance.Create(typeName, recordEnumerator.Current));
					try
					{
						Cache.Remove(recordEnumerator.CurrentKey, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					}
					catch(Exception ex)
					{
						return new PersistenceActionResult {
							Success = false,
							FailureMessage = ex.Message
						};
					}
				}
			}
			return new PersistenceActionResult {
				Success = true,
				Result = new DeleteActionResult(recordKeys)
			};
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
		protected virtual void Dispose(bool disposing)
		{
			if(_sourceContext != null)
				_sourceContext.Dispose();
			_sourceContext = null;
		}

		/// <summary>
		/// Armazena o resultado da ação de exclusão.
		/// </summary>
		public class DeleteActionResult
		{
			private IList<RecordKey> _recordKeys;

			/// <summary>
			/// Chaves dos registros apagados.
			/// </summary>
			public IList<RecordKey> RecordKeys
			{
				get
				{
					return _recordKeys;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="recordKeys">Relação das chaves dos registros apagados.</param>
			public DeleteActionResult(IList<RecordKey> recordKeys)
			{
				_recordKeys = recordKeys;
			}
		}
	}
}
