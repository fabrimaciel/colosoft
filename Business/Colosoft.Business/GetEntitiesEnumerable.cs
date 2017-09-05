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
		abstract class GetEntitiesEnumerable : IEnumerable<IEntity>, Collections.INotifyCollectionChangedObserverRegister, IDisposable, IDisposableState, Collections.ISearchParameterDescriptionContainer, Colosoft.Query.IQueryableContainer
		{
			private EntityLoader<TEntity1, Model> _entityLoader;

			private Query.Queryable _queryable;

			private Colosoft.Query.ISourceContext _sourceContext;

			private string _uiContext;

			private IEntityTypeManager _entityTypeManager;

			private bool _isDisposed;

			/// <summary>
			/// Loader associado.
			/// </summary>
			protected EntityLoader<TEntity1, Model> EntityLoader
			{
				get
				{
					return _entityLoader;
				}
			}

			/// <summary>
			/// Consulta associada.
			/// </summary>
			protected Query.Queryable Queryable
			{
				get
				{
					return _queryable;
				}
			}

			/// <summary>
			/// Contexto de origem associado.
			/// </summary>
			protected Colosoft.Query.ISourceContext SourceContext
			{
				get
				{
					return _sourceContext;
				}
			}

			/// <summary>
			/// Contexto de interface com o usuário.
			/// </summary>
			protected string UiContext
			{
				get
				{
					return _uiContext;
				}
			}

			/// <summary>
			/// Gerenciador dos tipos de entidade.
			/// </summary>
			protected IEntityTypeManager EntityTypeManager
			{
				get
				{
					return _entityTypeManager;
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
			public GetEntitiesEnumerable(EntityLoader<TEntity1, Model> entityLoader, Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
			{
				_entityLoader = entityLoader;
				_queryable = queryable;
				_sourceContext = sourceContext;
				_uiContext = uiContext;
				_entityTypeManager = entityTypeManager;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~GetEntitiesEnumerable()
			{
				Dispose(false);
			}

			/// <summary>
			/// Recupera o nome do tipo principal associado com o resultado da consulta.
			/// </summary>
			/// <returns></returns>
			protected virtual Colosoft.Reflection.TypeName GetTypeName()
			{
				if(Queryable != null && Queryable.Entity != null)
					return new Colosoft.Reflection.TypeName(Queryable.Entity.FullName);
				return null;
			}

			/// <summary>
			/// Registra os observers no container.
			/// </summary>
			/// <param name="container"></param>
			public void Register(Collections.INotifyCollectionChangedObserverContainer container)
			{
				var collection = container as System.Collections.IList;
				var typeName = GetTypeName();
				if(collection != null && typeName != null && Colosoft.Query.RecordObserverManager.Instance.IsEnabled && (_queryable == null || (_queryable.Entity != null && _queryable.WhereClause != null && _queryable.WhereClause.ConditionalsCount == 0)))
				{
					var dataModelTypeName = Colosoft.Reflection.TypeName.Get<Model>();
					var observer = new SingleEntityQueryResultChangedObserver<TEntity1>(_entityLoader, _entityTypeManager, _sourceContext, _uiContext, dataModelTypeName, collection);
					container.AddObserver(observer, Collections.NotifyCollectionChangedObserverLiveScope.Instance);
				}
			}

			/// <summary>
			/// Recupera o enumerador do resultad.
			/// </summary>
			/// <returns></returns>
			public abstract IEnumerator<IEntity> GetEnumerator();

			/// <summary>
			/// Recupera o enumerador associado.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<IEntity>)this).GetEnumerator();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			protected virtual void Dispose(bool disposing)
			{
				_isDisposed = true;
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
			/// Identifica se a instancia foi liberada.
			/// </summary>
			public bool IsDisposed
			{
				get
				{
					return _isDisposed;
				}
			}

			/// <summary>
			/// Recupera as descrições dos parametros de pesquisa.
			/// </summary>
			Collections.SearchParameterDescriptionCollection Collections.ISearchParameterDescriptionContainer.SearchParameterDescriptions
			{
				get
				{
					return _queryable.SearchParameterDescriptions;
				}
			}

			/// <summary>
			/// Consulta associada.
			/// </summary>
			Query.Queryable Query.IQueryableContainer.Queryable
			{
				get
				{
					return _queryable;
				}
			}
		}
	}
}
