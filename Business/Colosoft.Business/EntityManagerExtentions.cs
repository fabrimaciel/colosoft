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
using Colosoft.Collections;

namespace Colosoft
{
	/// <summary>
	/// Armazena os método extensíveis usados pelo gerenciador de entidades.
	/// </summary>
	public static class EntityManagerExtentions
	{
		/// <summary>
		/// Processa os resultado da consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="uiContext"></param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		private static IEnumerable<T> ExecuteSelectProcessResult<T>(this Query.Queryable queryable, string uiContext = null, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null)
		{
			var method = typeof(Business.IEntityManager).GetMethod("ProcessResult", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, System.Reflection.CallingConventions.Any, new[] {
				typeof(Query.Queryable),
				typeof(Query.ISourceContext),
				typeof(string)
			}, null);
			method = method.MakeGenericMethod(typeof(T));
			try
			{
				return (IEnumerable<T>)method.Invoke(Colosoft.Business.EntityManager.Instance, new object[] {
					queryable,
					queryable.SourceContext,
					uiContext
				});
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Processa os resultado da consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="uiContext"></param>
		/// <param name="args"></param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		private static IEnumerable<T> ExecuteSelectProcessLazyResult<T>(this Query.Queryable queryable, string uiContext = null, Business.EntityLoaderLazyArgs args = null, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null)
		{
			var method = typeof(Business.IEntityManager).GetMethod("ProcessLazyResult", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, System.Reflection.CallingConventions.Any, new[] {
				typeof(Query.Queryable),
				typeof(Query.ISourceContext),
				typeof(string),
				typeof(Business.EntityLoaderLazyArgs)
			}, null);
			method = method.MakeGenericMethod(typeof(T));
			try
			{
				return (IEnumerable<T>)method.Invoke(Colosoft.Business.EntityManager.Instance, new object[] {
					queryable,
					queryable.SourceContext,
					uiContext,
					args
				});
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessResult<TEntity>(this Colosoft.Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext = null, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryResult.Require("queryResult").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessResult<TEntity>(queryResult, sourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResultContainer">Container do resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessResult<TEntity>(this Colosoft.Query.IQueryResultContainer queryResultContainer, Colosoft.Query.ISourceContext sourceContext = null, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryResultContainer.Require("queryResultContainer").NotNull();
			return ProcessResult<TEntity>(queryResultContainer.GetResult(), sourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryable">Consulta.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessResult<TEntity>(this Colosoft.Query.Queryable queryable, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryable.Require("queryable").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessResult<TEntity>(queryable, queryable.SourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado para a preparação da consulta.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="prepareResult"></param>
		/// <param name="queryResult"></param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessResult<TEntity>(this Colosoft.Business.PrepareNestedQueriesResult prepareResult, Colosoft.Query.IQueryResult queryResult = null) where TEntity : class, Colosoft.Business.IEntity
		{
			prepareResult.Require("prepareResult").NotNull();
			return Colosoft.Business.EntityManager.Instance.ProcessResult<TEntity>(prepareResult, prepareResult.Queryable.SourceContext, queryResult);
		}

		/// <summary>
		/// Prepara a consulta para carregar os dados completos das entidades do resultado.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryable">Consulta.</param>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <returns></returns>
		public static Colosoft.Business.PrepareNestedQueriesResult Prepare<TEntity>(this Colosoft.Query.Queryable queryable, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryable.Require("queryable").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.Prepare<TEntity>(queryable, queryable.SourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessLazyResult<TEntity>(this Colosoft.Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext = null, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryResult.Require("queryResult").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessLazyResult<TEntity>(queryResult, sourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResultContainer">Container d resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessLazyResult<TEntity>(this Colosoft.Query.IQueryResultContainer queryResultContainer, Colosoft.Query.ISourceContext sourceContext = null, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryResultContainer.Require("queryResultContainer").NotNull();
			return ProcessLazyResult<TEntity>(queryResultContainer.GetResult(), sourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryable">Consulta.</param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public static IEnumerable<TEntity> ProcessLazyResult<TEntity>(this Colosoft.Query.Queryable queryable, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			queryable.Require("queryable").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessLazyResult<TEntity>(queryable, queryable.SourceContext, uiContext);
		}

		/// <summary>
		/// Processa a consulta e recupera o resultado como IEntityDescriptors.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query">Consulta que será utilizada no processamento.</param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public static IEnumerable<IEntityDescriptor> ProcessResultDescriptor<TEntity>(this Colosoft.Query.Queryable query, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			query.Require("query").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessResultDescriptor<TEntity>(query, query.SourceContext, uiContext);
		}

		/// <summary>
		/// Processa a consulta e recupera o resultado como IEntityDescriptors.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TEntityDescriptor"></typeparam>
		/// <param name="query">Consulta que será utilizada no processamento.</param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public static IEnumerable<TEntityDescriptor> ProcessResultDescriptor<TEntity, TEntityDescriptor>(this Colosoft.Query.Queryable query, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity where TEntityDescriptor : IEntityDescriptor
		{
			query.Require("query").NotNull();
			if(Colosoft.Business.EntityManager.Instance == null)
				throw new InvalidOperationException("EntityManager instance is null");
			return Colosoft.Business.EntityManager.Instance.ProcessResultDescriptor<TEntity, TEntityDescriptor>(query, query.SourceContext, uiContext);
		}

		/// <summary>
		/// Recupera o descritor da instancia pelo identificador informado.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade</typeparam>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uid">Identificador unico da instancia.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		public static IEntityDescriptor GetDescriptor<TEntity>(this Colosoft.Query.ISourceContext sourceContext, int uid, string uiContext = null) where TEntity : class, Colosoft.Business.IEntity
		{
			return Colosoft.Business.EntityManager.Instance.GetDescriptorByUid<TEntity>(uid, sourceContext, uiContext);
		}

		/// <summary>
		/// Retorna um resultado assíncrono.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator">Criador do objeto.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TEntity> ToVirtualResult<TEntity>(this Colosoft.Query.Queryable queryable, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null) where TEntity : class
		{
			return ToVirtualResult<TEntity>(queryable, 0, null, null, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Retorna um resultado assíncrono.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="pageSize"></param>
		/// <param name="countExpression"></param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator">Criador do objeto.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TEntity> ToVirtualResult<TEntity>(this Colosoft.Query.Queryable queryable, int pageSize, string countExpression = null, string uiContext = null, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null) where TEntity : class
		{
			queryable.Require("queryable").NotNull();
			Collections.VirtualListLoader<TEntity> virtualListLoader = null;
			Colosoft.Query.QueryableExecuterHandler<TEntity> executeSelect = null;
			Collections.VirtualList<TEntity> result = null;
			if(bindStrategy == null && objectCreator == null)
			{
				var ts = Colosoft.Query.TypeBindStrategyCache.GetItem(typeof(TEntity), t => new Colosoft.Query.QueryResultObjectCreator(t));
				objectCreator = ts;
				bindStrategy = ts;
			}
			if(bindStrategy == null)
				bindStrategy = new Colosoft.Query.TypeBindStrategy(typeof(TEntity), objectCreator);
			var queryable2 = (Colosoft.Query.Queryable)queryable.Clone();
			if(typeof(Colosoft.Business.IEntity).IsAssignableFrom(typeof(TEntity)))
			{
				virtualListLoader = Colosoft.Business.EntityManager.Instance.GetEntityVirtualListLoader<TEntity>(queryable2, countExpression, false, uiContext, null);
				executeSelect = (queryable3, dataSource, bindStrategy1, objectCreator1) => queryable3.ExecuteSelectProcessResult<TEntity>(uiContext, bindStrategy1, objectCreator1);
			}
			else
				virtualListLoader = new Colosoft.Business.QueryableVirtualListLoader<TEntity>(queryable2, countExpression, bindStrategy, objectCreator, null);
			result = new Business.QueryableVirtualList<TEntity>(queryable2, pageSize, virtualListLoader, executeSelect, null, bindStrategy, objectCreator);
			return result;
		}

		/// <summary>
		/// Retorna um resultado assíncrono.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable"></param>
		/// <param name="pageSize"></param>
		/// <param name="listLoader">Carregador da lista.</param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TEntity> ToVirtualResult<TEntity>(this Colosoft.Query.Queryable queryable, int pageSize, Collections.VirtualListLoader<TEntity> listLoader, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null) where TEntity : class
		{
			listLoader.Require("listLoader").NotNull();
			queryable.Require("queryable").NotNull();
			var queryable2 = (Colosoft.Query.Queryable)queryable.Clone();
			Colosoft.Query.QueryableExecuterHandler<TEntity> executeSelect = null;
			if(typeof(Colosoft.Business.IEntity).IsAssignableFrom(typeof(TEntity)))
				executeSelect = (queryable3, dataSource, bindStrategy1, objectCreator1) => queryable3.ExecuteSelectProcessResult<TEntity>(null, bindStrategy1, objectCreator1);
			return new Business.QueryableVirtualList<TEntity>(queryable2, pageSize, listLoader, executeSelect, null, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Cria um resultado virtual para a consulta informada.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade do resultado</typeparam>
		/// <typeparam name="TProxy">Tipo do proxy do resultado.</typeparam>
		/// <param name="queryable"></param>
		/// <param name="pageSize"></param>
		/// <param name="countExpression"></param>
		/// <param name="proxyCreator">Referencia do método usado para cria o proxy do itens da coleção.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TProxy> ToProxyVirtualResult<TEntity, TProxy>(this Colosoft.Query.Queryable queryable, int pageSize = 0, string countExpression = null, Func<TEntity, TProxy> proxyCreator = null) where TEntity : new()
		{
			queryable.Require("queryable").NotNull();
			var queryable2 = (Colosoft.Query.Queryable)queryable.Clone();
			var bindStrategy = Colosoft.Query.TypeBindStrategyCache.GetItem(typeof(TEntity), f => new Colosoft.Query.QueryResultObjectCreator(() => new TEntity()));
			var objectCreator = bindStrategy;
			return new Business.QueryableVirtualList<TProxy>(queryable2, pageSize, new Colosoft.Business.QueryableVirtualListLoader<TProxy>(queryable2, countExpression, bindStrategy, bindStrategy, proxyCreator != null ? new Func<object, TProxy>(f => proxyCreator((TEntity)f)) : null), null, null, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Retorna um resultado assíncrono.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable">Consulta que será utilizada pela lista virtual.</param>
		/// <param name="pageSize">Tamanho da páginda de dados da lista virtual.</param>
		/// <param name="countExpression">Expressão que será repassada para o método Count.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TEntity> ToVirtualResultLazy<TEntity>(this Colosoft.Query.Queryable queryable, int pageSize = 0, string countExpression = null, string uiContext = null, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null) where TEntity : class
		{
			queryable.Require("queryable").NotNull();
			Collections.VirtualListLoader<TEntity> virtualListLoader = null;
			Colosoft.Query.QueryableExecuterHandler<TEntity> executeSelect = null;
			if(bindStrategy == null && objectCreator == null)
			{
				var ts = Colosoft.Query.TypeBindStrategyCache.GetItem(typeof(TEntity), t => new Colosoft.Query.QueryResultObjectCreator(t));
				objectCreator = ts;
				bindStrategy = ts;
			}
			if(bindStrategy == null)
				bindStrategy = new Colosoft.Query.TypeBindStrategy(typeof(TEntity), objectCreator);
			var queryable2 = (Colosoft.Query.Queryable)queryable.Clone();
			if(typeof(Colosoft.Business.IEntity).IsAssignableFrom(typeof(TEntity)))
			{
				virtualListLoader = Colosoft.Business.EntityManager.Instance.GetEntityVirtualListLoader<TEntity>(queryable2, countExpression, true, uiContext, null);
				executeSelect = (queryable3, dataSource, bindStrategy1, objectCreator1) => queryable3.ExecuteSelectProcessLazyResult<TEntity>(uiContext, null, bindStrategy1, objectCreator1);
			}
			else
				virtualListLoader = new Colosoft.Business.QueryableVirtualListLoader<TEntity>(queryable2, countExpression, bindStrategy, objectCreator, null);
			return new Business.QueryableVirtualList<TEntity>(queryable2, pageSize, virtualListLoader, executeSelect, null, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Retorna um resultado assíncrono.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="queryable">Consulta que será utilizada pela lista virtual.</param>
		/// <param name="pageSize">Tamanho da páginda de dados da lista virtual.</param>
		/// <param name="listLoader">Loader da lista.</param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.VirtualList<TEntity> ToVirtualResultLazy<TEntity>(this Colosoft.Query.Queryable queryable, int pageSize, Collections.VirtualListLoader<TEntity> listLoader, Colosoft.Query.IQueryResultBindStrategy bindStrategy = null, Colosoft.Query.IQueryResultObjectCreator objectCreator = null) where TEntity : class
		{
			queryable.Require("queryable").NotNull();
			var queryable2 = (Colosoft.Query.Queryable)queryable.Clone();
			Colosoft.Query.QueryableExecuterHandler<TEntity> executeSelect = null;
			if(typeof(Colosoft.Business.IEntity).IsAssignableFrom(typeof(TEntity)))
				executeSelect = (queryable3, dataSource, bindStragety1, objectCreator1) => queryable3.ExecuteSelectProcessLazyResult<TEntity>(null, null, bindStragety1, objectCreator1);
			return new Business.QueryableVirtualList<TEntity>(queryable2, pageSize, listLoader, executeSelect, null, bindStrategy, objectCreator);
		}
	///// <summary>
	///// Transforma o enumerable passado para um enumerable com
	///// o vinculo o resultado da consulta.
	///// </summary>
	///// <typeparam name="TEntity"></typeparam>
	///// <param name="enumerable"></param>
	///// <param name="result"></param>
	///// <param name="info"></param>
	///// <returns></returns>
	}
}
