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
	/// Implementação do gerenciador de entidades.
	/// </summary>
	public class EntityManager : IEntityManager
	{
		private static Lazy<IEntityManager> _loader;

		private static IEntityManager _instance;

		private IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Instancia do gerenciador do sistema.
		/// </summary>
		public static IEntityManager Instance
		{
			get
			{
				if(_instance == null && _loader != null)
					_instance = _loader.Value;
				return _instance;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityTypeManager"></param>
		public EntityManager(IEntityTypeManager entityTypeManager)
		{
			entityTypeManager.Require("entityTypeManager").NotNull();
			_entityTypeManager = entityTypeManager;
		}

		/// <summary>
		/// Define a instancia que será usada no sistema.
		/// </summary>
		/// <param name="entityManager"></param>
		public static void SetEntityManager(Lazy<IEntityManager> entityManager)
		{
			_loader = entityManager;
		}

		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<TEntity> ProcessResult<TEntity>(Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity
		{
			queryResult.Require("queryResult").NotNull();
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return new EntitiesEnumerable<TEntity>(loader.GetFullEntities(queryResult, sourceContext, uiContext, _entityTypeManager));
		}

		/// <summary>
		/// Processa o resultado da consulta e carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="prepareResult">Resultado da prepraração da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<TEntity> ProcessResult<TEntity>(Colosoft.Business.PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext, Query.IQueryResult queryResult) where TEntity : class, IEntity
		{
			prepareResult.Require("prepareResult").NotNull();
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return new EntitiesEnumerable<TEntity>(loader.GetFullEntities(prepareResult, sourceContext, queryResult));
		}

		/// <summary>
		/// Prepara a consulta para recupera todos os dados das entidades que serão recuperadas.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query">Consulta sobre a qual será aplicada as sub-consultas.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns>Resultado da preparação da consulta.</returns>
		public PrepareNestedQueriesResult Prepare<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity
		{
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return loader.PrepareNestedQueries(query, uiContext, _entityTypeManager);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="query">Consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<TEntity> ProcessResult<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity
		{
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return new EntitiesEnumerable<TEntity>(loader.GetFullEntities(query, sourceContext, uiContext, _entityTypeManager));
		}

		/// <summary>
		/// Processa os descritores para o resultado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TEntityDescritor"></typeparam>
		/// <param name="query"></param>
		/// <param name="sourceContext"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public IEnumerable<TEntityDescritor> ProcessResultDescriptor<TEntity, TEntityDescritor>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity where TEntityDescritor : IEntityDescriptor
		{
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return loader.GetEntityDescriptors<TEntityDescritor>(query, sourceContext, uiContext);
		}

		/// <summary>
		/// Processa os descritores para o resultado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query"></param>
		/// <param name="sourceContext"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public IEnumerable<IEntityDescriptor> ProcessResultDescriptor<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity
		{
			return ProcessResultDescriptor<TEntity, IEntityDescriptor>(query, sourceContext, uiContext);
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<TEntity> ProcessLazyResult<TEntity>(Query.IQueryResult queryResult, Colosoft.Query.ISourceContext sourceContext, string uiContext = null, EntityLoaderLazyArgs args = null) where TEntity : class, IEntity
		{
			queryResult.Require("queryResult").NotNull();
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return new EntitiesEnumerable<TEntity>(loader.GetLazyEntities(queryResult, sourceContext, uiContext, _entityTypeManager, args));
		}

		/// <summary>
		/// Processa o resultado da consulta a carrega as entidades de negócio.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade de negócio.</typeparam>
		/// <param name="query">Consulta.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<TEntity> ProcessLazyResult<TEntity>(Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext = null, EntityLoaderLazyArgs args = null) where TEntity : class, IEntity
		{
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			return new EntitiesEnumerable<TEntity>(loader.GetLazyEntities(query.Execute(), sourceContext, uiContext, _entityTypeManager, args));
		}

		/// <summary>
		/// Recupera a instancia da entidade pelo identificador informado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="uid">Identificador unico da instancia.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		public TEntity GetByUid<TEntity>(int uid, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity, new()
		{
			sourceContext.Require("sourceContext").NotNull();
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			if(!loader.HasUid)
				throw new InvalidOperationException("Entity has not uid.");
			var query = sourceContext.CreateQuery().From(new Query.EntityInfo(loader.DataModelType.FullName)).Where(string.Format("{0} = ?entityuid", loader.UidPropertyName)).Add("?entityuid", uid);
			return (TEntity)loader.FullLoad(query, sourceContext, uiContext, _entityTypeManager);
		}

		/// <summary>
		/// Recupera a instancia do descritor da entidade pelo uid informador.
		/// </summary>
		/// <param name="uid">Identificador unico da instancia.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <returns></returns>
		public IEntityDescriptor GetDescriptorByUid<TEntity>(int uid, Colosoft.Query.ISourceContext sourceContext, string uiContext = null) where TEntity : class, IEntity
		{
			sourceContext.Require("sourceContext").NotNull();
			var loader = _entityTypeManager.GetLoader(typeof(TEntity));
			if(!loader.HasUid)
				throw new InvalidOperationException("Entity has not uid.");
			var query = sourceContext.CreateQuery().From(new Query.EntityInfo(loader.DataModelType.FullName)).Where(string.Format("{0} = ?entityuid", loader.UidPropertyName)).Add("?entityuid", uid);
			return ProcessResultDescriptor<TEntity>(query, sourceContext, uiContext).FirstOrDefault();
		}

		/// <summary>
		/// Recuper o Loader da lista virtual para a entidade do tipo informado.
		/// </summary>
		/// <param name="queryable">Consulta.</param>
		/// <param name="countExpression">Expressão para a consulta Count.</param>
		/// <param name="isLazy">Identifica se é para carregar com lazy.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="castHandler">Instancia do manipulador que vai realizar o cast do item do resultado.</param>
		/// <returns></returns>
		public Colosoft.Collections.VirtualListLoader<TEntity> GetEntityVirtualListLoader<TEntity>(Colosoft.Query.Queryable queryable, string countExpression, bool isLazy, string uiContext, Func<object, TEntity> castHandler) where TEntity : class
		{
			var loaderType = typeof(Business.QueryableEntityVirtualListLoader<>).MakeGenericType(typeof(TEntity));
			var parameters = new object[] {
				_entityTypeManager,
				_entityTypeManager.GetLoader(typeof(TEntity)),
				queryable,
				countExpression,
				isLazy,
				castHandler,
				uiContext,
			};
			return (Colosoft.Collections.VirtualListLoader<TEntity>)Activator.CreateInstance(loaderType, parameters);
		}

		/// <summary>
		/// Implementação do enumerable dos descritores de entidade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class EntitiesDescriptorEnumerable<T> : IEnumerable<T>, Collections.INotifyCollectionChangedObserverRegister, IDisposable
		{
			private IEnumerable<IEntityDescriptor> _innerEnumerable;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerable"></param>
			public EntitiesDescriptorEnumerable(IEnumerable<IEntityDescriptor> enumerable)
			{
				_innerEnumerable = enumerable;
			}

			/// <summary>
			/// Método usado para registrar os observers.
			/// </summary>
			/// <param name="container"></param>
			public void Register(Collections.INotifyCollectionChangedObserverContainer container)
			{
				var register = _innerEnumerable as Collections.INotifyCollectionChangedObserverRegister;
				if(register != null)
					register.Register(container);
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~EntitiesDescriptorEnumerable()
			{
				Dispose();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_innerEnumerable is IDisposable)
					((IDisposable)_innerEnumerable).Dispose();
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<T> GetEnumerator()
			{
				return new EntitiesDescriptorEnumerator<T>(this, _innerEnumerable.GetEnumerator());
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return new EntitiesDescriptorEnumerator<T>(this, _innerEnumerable.GetEnumerator());
			}
		}

		/// <summary>
		/// Implementação do enumerador das entidades.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		sealed class EntitiesEnumerable<T> : IEnumerable<T>, Collections.INotifyCollectionChangedObserverRegister, IDisposable, Collections.ISearchParameterDescriptionContainer
		{
			private IEnumerable<IEntity> _innerEnumerable;

			private Collections.SearchParameterDescriptionCollection _searchParameterDescriptionCollection;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerable"></param>
			public EntitiesEnumerable(IEnumerable<IEntity> enumerable)
			{
				_innerEnumerable = enumerable;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~EntitiesEnumerable()
			{
				Dispose();
			}

			/// <summary>
			/// Método usado para registrar os observers.
			/// </summary>
			/// <param name="container"></param>
			public void Register(Collections.INotifyCollectionChangedObserverContainer container)
			{
				var register = _innerEnumerable as Collections.INotifyCollectionChangedObserverRegister;
				if(register != null)
					register.Register(container);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_innerEnumerable is IDisposable)
					((IDisposable)_innerEnumerable).Dispose();
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<T> GetEnumerator()
			{
				return new EntitiesEnumerator<T>(this, _innerEnumerable.GetEnumerator());
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return new EntitiesEnumerator<T>(this, _innerEnumerable.GetEnumerator());
			}

			Collections.SearchParameterDescriptionCollection Collections.ISearchParameterDescriptionContainer.SearchParameterDescriptions
			{
				get
				{
					if(_innerEnumerable is Collections.ISearchParameterDescriptionContainer)
						return ((Collections.ISearchParameterDescriptionContainer)_innerEnumerable).SearchParameterDescriptions;
					if(_searchParameterDescriptionCollection == null)
						_searchParameterDescriptionCollection = new Collections.SearchParameterDescriptionCollection();
					return _searchParameterDescriptionCollection;
				}
			}
		}

		/// <summary>
		/// Implementação do enumerador dos descritores de entidades.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		sealed class EntitiesDescriptorEnumerator<T> : IEnumerator<T>
		{
			private IEnumerator<IEntityDescriptor> _innerEnumerator;

			private IDisposable _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="enumerator"></param>
			public EntitiesDescriptorEnumerator(IDisposable owner, IEnumerator<IEntityDescriptor> enumerator)
			{
				_owner = owner;
				_innerEnumerator = enumerator;
			}

			public T Current
			{
				get
				{
					return (T)_innerEnumerator.Current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _innerEnumerator.Current;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_innerEnumerator.Dispose();
				if(_owner != null)
					_owner.Dispose();
			}

			public bool MoveNext()
			{
				return _innerEnumerator.MoveNext();
			}

			public void Reset()
			{
				_innerEnumerator.Reset();
			}
		}

		/// <summary>
		/// Implementação do enumerador das entidades.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		sealed class EntitiesEnumerator<T> : IEnumerator<T>
		{
			private IEnumerator<Colosoft.Business.IEntity> _innerEnumerator;

			private IDisposable _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="enumerator"></param>
			public EntitiesEnumerator(IDisposable owner, IEnumerator<IEntity> enumerator)
			{
				_owner = owner;
				_innerEnumerator = enumerator;
			}

			/// <summary>
			/// Instancia da atual entidade.
			/// </summary>
			public T Current
			{
				get
				{
					return (T)_innerEnumerator.Current;
				}
			}

			/// <summary>
			/// Instancia da atual entidade.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _innerEnumerator.Current;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_innerEnumerator.Dispose();
				if(_owner != null)
				{
					_owner.Dispose();
					_owner = null;
				}
			}

			/// <summary>
			/// Move para a próxima entidade.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				return _innerEnumerator.MoveNext();
			}

			/// <summary>
			/// Reseta o enumerador.
			/// </summary>
			public void Reset()
			{
				_innerEnumerator.Reset();
			}
		}
	}
}
