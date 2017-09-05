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

namespace Colosoft.Business
{
	partial class EntityLoader<TEntity1, Model>
	{
		/// <summary>
		/// Classe usa para pecorre o resultado de uma consulta
		/// e recuperar as entidades equivalentes.
		/// </summary>
		sealed class GetFullEntitiesEnumerable : GetEntitiesEnumerable
		{
			private PrepareNestedQueriesResult _prepareResult;

			private Query.IQueryResult _queryResult;

			/// <summary>
			/// Preparação do resultado.
			/// </summary>
			private PrepareNestedQueriesResult PrepareResult
			{
				get
				{
					return _prepareResult;
				}
			}

			/// <summary>
			/// Resultado da consulta.
			/// </summary>
			private Query.IQueryResult QueryResult
			{
				get
				{
					return _queryResult;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entityLoader">Loader.</param>
			/// <param name="queryable">Consulta que será realizado.</param>
			/// <param name="sourceContext">Contexto de origem dos dados.</param>
			/// <param name="uiContext">Contexto de interface com o usuário.</param>
			/// <param name="entityTypeManager">Gerenciador dos tipos de entidas.</param>
			/// <returns></returns>
			public GetFullEntitiesEnumerable(EntityLoader<TEntity1, Model> entityLoader, Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager) : base(entityLoader, queryable, sourceContext, uiContext, entityTypeManager)
			{
			}

			/// <summary>
			/// Construtor usado para carregar as entidade com base no resultado preparado.
			/// </summary>
			/// <param name="prepareResult"></param>
			/// <param name="sourceContext"></param>
			public GetFullEntitiesEnumerable(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext) : base((EntityLoader<TEntity1, Model>)prepareResult.EntityLoader, prepareResult.Queryable, sourceContext, prepareResult.UiContext, prepareResult.EntityTypeManager)
			{
				_prepareResult = prepareResult;
			}

			/// <summary>
			/// Construtor usado para carregar as entidades com base no resulta da consulta.
			/// </summary>
			/// <param name="prepareResult"></param>
			/// <param name="sourceContext"></param>
			/// <param name="queryResult"></param>
			public GetFullEntitiesEnumerable(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext, Query.IQueryResult queryResult) : this(prepareResult, sourceContext)
			{
				_queryResult = queryResult;
			}

			/// <summary>
			/// Recupera o nome do tipo principal associado com o resultado da consulta.
			/// </summary>
			/// <returns></returns>
			protected override Colosoft.Reflection.TypeName GetTypeName()
			{
				var queryable = Queryable;
				if(queryable == null && _prepareResult != null && _prepareResult.Queryable != null)
					queryable = _prepareResult.Queryable;
				if(queryable != null && queryable.Entity != null)
					return new Colosoft.Reflection.TypeName(queryable.Entity.FullName);
				return null;
			}

			/// <summary>
			/// Executa a consulta.
			/// </summary>
			/// <param name="prepareResult"></param>
			private void ExecuteQuery(PrepareNestedQueriesResult prepareResult)
			{
				using (var queryableResult = prepareResult.Queryable.Execute(prepareResult.Queryable.DataSource))
					ProcessQueryResult(prepareResult, queryableResult);
			}

			/// <summary>
			/// Processo o resultado da consulta.
			/// </summary>
			/// <param name="prepareResult"></param>
			/// <param name="queryableResult"></param>
			private static void ProcessQueryResult(PrepareNestedQueriesResult prepareResult, Query.IQueryResult queryableResult)
			{
				var bindStrategy = prepareResult.EntityLoader.GetBindStrategy();
				var objectCreator = prepareResult.EntityLoader.GetObjectCreator();
				var recordKeyFactory = prepareResult.EntityLoader.GetRecordKeyFactory();
				var dataModelTypeName = Colosoft.Reflection.TypeName.Get(prepareResult.EntityLoader.DataModelType);
				Colosoft.Query.IQueryResultBindStrategySession bindStrategySession = null;
				foreach (var record in queryableResult)
				{
					var data = objectCreator.Create();
					if(bindStrategySession == null)
						bindStrategySession = bindStrategy.CreateSession(record.Descriptor);
					var bindResult = bindStrategySession.Bind(record, Query.BindStrategyMode.All, ref data);
					if(!bindResult.Any())
						throw new Exception(string.Format("Not found scheme for bind record data to type '{0}'", data.GetType().FullName));
					var recordKey = recordKeyFactory.Create(dataModelTypeName, record);
					EntityLoaderCreatorArgs creatorArgs = null;
					creatorArgs = new EntityLoaderCreatorArgs((Model)data, recordKey, new EntityLoaderChildContainer(), new EntityLoaderLinksContainer(), new EntityLoaderReferenceContainer(), prepareResult.UiContext, prepareResult.EntityTypeManager);
					prepareResult.ParentResult.Add(creatorArgs);
				}
			}

			/// <summary>
			/// Recupera o enumerador das entidade com base no resultado preparado.
			/// </summary>
			/// <param name="prepareResult"></param>
			/// <param name="sourceContext"></param>
			/// <returns></returns>
			private IEnumerator<IEntity> GetEnumerator(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext)
			{
				if(prepareResult.Exceptions.Count > 0)
					throw new AggregateException(prepareResult.Exceptions.FirstOrDefault().Message, prepareResult.Exceptions);
				foreach (var i in prepareResult.ParentResult)
				{
					IEntity entity = null;
					LazyDataState lazyDataState = new LazyDataState();
					prepareResult.EntityLoader.GetLazyData(i, lazyDataState, sourceContext, prepareResult.UiContext, prepareResult.EntityTypeManager, prepareResult.Exceptions);
					using (var args = new EntityLoaderCreatorArgs<Model>((Model)i.DataModel, i.RecordKey, i.Children, i.Links, i.References, prepareResult.UiContext, prepareResult.EntityTypeManager))
					{
						entity = ((EntityLoader<TEntity1, Model>)prepareResult.EntityLoader).EntityCreator(args);
						lazyDataState.Entity = entity;
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(sourceContext);
						if(entity is IEntityRecordObserver)
							((IEntityRecordObserver)entity).RegisterObserver(i.RecordKey);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
					}
					i.Dispose();
					yield return entity;
				}
			}

			/// <summary>
			/// Recupera o enumerador do resultad.
			/// </summary>
			/// <returns></returns>
			public override IEnumerator<IEntity> GetEnumerator()
			{
				if(PrepareResult == null)
				{
					var result = new List<EntityLoaderCreatorArgs>();
					var exceptions = new Queue<Exception>();
					EntityLoader.CreateNestedQueries(Queryable, UiContext, EntityTypeManager, result, exceptions);
					var prepareResult = new PrepareNestedQueriesResult(Queryable, EntityLoader, UiContext, EntityTypeManager, result, exceptions);
					ExecuteQuery(prepareResult);
					return GetEnumerator(prepareResult, SourceContext);
				}
				else
				{
					if(QueryResult != null)
						ProcessQueryResult(PrepareResult, QueryResult);
					else
						ExecuteQuery(PrepareResult);
					return GetEnumerator(PrepareResult, SourceContext);
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				if(_queryResult != null)
					_queryResult.Dispose();
				_queryResult = null;
				base.Dispose(disposing);
			}
		}
	}
}
