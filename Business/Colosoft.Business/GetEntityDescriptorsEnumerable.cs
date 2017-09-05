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
	/// Implementação do enumerable dos descritores de entidades.
	/// </summary>
	class GetEntityDescriptorsEnumerable<T> : IEnumerable<T>, Collections.INotifyCollectionChangedObserverRegister, IDisposable, IDisposableState where T : IEntityDescriptor
	{
		private IEntityLoader _entityLoader;

		private Query.Queryable _queryable;

		private Colosoft.Query.ISourceContext _sourceContext;

		private string _uiContext;

		private bool _isDisposed;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityLoader">Loader.</param>
		/// <param name="queryable">Consulta que será realizado.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		public GetEntityDescriptorsEnumerable(IEntityLoader entityLoader, Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext)
		{
			_entityLoader = entityLoader;
			_queryable = queryable;
			_sourceContext = sourceContext;
			_uiContext = uiContext;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~GetEntityDescriptorsEnumerable()
		{
			Dispose(false);
		}

		/// <summary>
		/// Registra os observers no container.
		/// </summary>
		/// <param name="container"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public virtual void Register(Collections.INotifyCollectionChangedObserverContainer container)
		{
			var collection = container as System.Collections.IList;
			if(collection != null && Colosoft.Query.RecordObserverManager.Instance.IsEnabled && (_queryable.Entity != null && _queryable.WhereClause != null && _queryable.WhereClause.ConditionalsCount == 0))
			{
				var observer = new EntityDescriptorQueryResultChangedObserver(_entityLoader, _sourceContext, collection);
				container.AddObserver(observer, Collections.NotifyCollectionChangedObserverLiveScope.Instance);
			}
		}

		/// <summary>
		/// Recupera o enumerador de resultados.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			var bindStrategy = _entityLoader.GetEntityDescriptorBindStrategy();
			var objectCreator = _entityLoader.GetEntityDescriptorCreator();
			using (var queryResult = _queryable.Execute(_queryable.DataSource))
			{
				foreach (var item in bindStrategy.Bind(queryResult, Query.BindStrategyMode.All, objectCreator))
				{
					if(item is IConnectedEntity)
						((IConnectedEntity)item).Connect(_sourceContext);
					if(item is IEntityRecordObserver && item is BusinessEntityDescriptor)
						((IEntityRecordObserver)item).RegisterObserver(((BusinessEntityDescriptor)item).RecordKey);
					if(item is ILoadableEntity)
						((ILoadableEntity)item).NotifyLoaded();
					yield return (T)item;
				}
			}
		}

		/// <summary>
		/// Recupera o enumerador associado.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
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
	}
}
