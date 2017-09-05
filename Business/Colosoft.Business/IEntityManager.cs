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
	/// <summary>
	/// Assinatura o gerenciador de entidade de negócio.
	/// </summary>
	public interface IEntityManager
	{
		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		IEnumerable<TEntity> ProcessResult<TEntity>(Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity;

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		IEnumerable<TEntity> ProcessLazyResult<TEntity>(Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext, string uiContext = null, EntityLoaderLazyArgs args = null) where TEntity : class, IEntity;

		/// <summary>
		/// Prepara a consulta para recupera todos os dados das entidades que serão recuperadas.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query">Consulta sobre a qual será aplicada as sub-consultas.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns>Resultado da preparação da consulta.</returns>
		PrepareNestedQueriesResult Prepare<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity;

		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="query">Consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		IEnumerable<TEntity> ProcessResult<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity;

		/// <summary>
		/// Processa o resultada da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="prepareResult"></param>
		/// <param name="sourceContext"></param>
		/// <param name="queryResult"></param>
		/// <returns></returns>
		IEnumerable<TEntity> ProcessResult<TEntity>(Colosoft.Business.PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext, Query.IQueryResult queryResult) where TEntity : class, IEntity;

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="query">Consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		IEnumerable<TEntity> ProcessLazyResult<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null, EntityLoaderLazyArgs args = null) where TEntity : class, IEntity;

		/// <summary>
		/// Recupera a instancia da entidade pelo identificador informado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="uid">Identificador unico da instancia.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		TEntity GetByUid<TEntity>(int uid, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity, new();

		/// <summary>
		/// Recupera a instancia do descritor da entidade pelo uid informador.
		/// </summary>
		/// <param name="uid">Identificador unico da instancia.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		IEntityDescriptor GetDescriptorByUid<TEntity>(int uid, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity;

		/// <summary>
		/// Processa a consulta e recupera o resultado como IEntityDescriptors.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query"></param>
		/// <param name="sourceContext"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		IEnumerable<IEntityDescriptor> ProcessResultDescriptor<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity;

		/// <summary>
		/// Processa a consulta e recupera o resultado como IEntityDescriptors.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TEntityDescriptor"></typeparam>
		/// <param name="query"></param>
		/// <param name="sourceContext"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		IEnumerable<TEntityDescriptor> ProcessResultDescriptor<TEntity, TEntityDescriptor>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity where TEntityDescriptor : IEntityDescriptor;

		/// <summary>
		/// Recuper o Loader da lista virtual para a entidade do tipo informado.
		/// </summary>
		/// <param name="queryable">Consulta.</param>
		/// <param name="countExpression">Expressão para a consulta Count.</param>
		/// <param name="isLazy">Identifica se é para carregar com lazy.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="castHandler">Instancia do manipulador que vai realizar o cast do item do resultado.</param>
		/// <returns></returns>
		Colosoft.Collections.VirtualListLoader<TEntity> GetEntityVirtualListLoader<TEntity>(Colosoft.Query.Queryable queryable, string countExpression, bool isLazy, string uiContext, Func<object, TEntity> castHandler) where TEntity : class;
	}
}
