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

using Colosoft.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação de uma lista virtual que possui um queryable associado.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class QueryableVirtualList<T> : VirtualList<T>, ISortableCollection, Colosoft.Collections.ISearchParameterDescriptionContainer, Colosoft.Query.IQueryableContainer, System.Linq.IQueryable<T>
	{
		private Colosoft.Query.Queryable _queryable;

		private IQueryProvider _queryProvider;

		private Colosoft.Query.QueryableExecuterHandler<T> _executeSelect;

		private Colosoft.Query.IQueryResultBindStrategy _bindStrategy;

		private Colosoft.Query.IQueryResultObjectCreator _objectCreator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable">Consulta associada.</param>
		/// <param name="pageSize"></param>
		/// <param name="loader"></param>
		/// <param name="executeSelect">Referencia do método que será usado para executar a seleção dos dados.</param>
		/// <param name="referenceObject"></param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		public QueryableVirtualList(Colosoft.Query.Queryable queryable, int pageSize, VirtualListLoader<T> loader, Colosoft.Query.QueryableExecuterHandler<T> executeSelect, object referenceObject, Colosoft.Query.IQueryResultBindStrategy bindStrategy, Colosoft.Query.IQueryResultObjectCreator objectCreator) : base(pageSize, loader, referenceObject)
		{
			_queryable = queryable;
			_executeSelect = executeSelect;
			_bindStrategy = bindStrategy;
			_objectCreator = objectCreator;
			if(_bindStrategy == null && _objectCreator == null)
			{
				var ts = Colosoft.Query.TypeBindStrategyCache.GetItem(typeof(T), t => new Colosoft.Query.QueryResultObjectCreator(t));
				_objectCreator = ts;
				_bindStrategy = ts;
			}
			if(_bindStrategy == null)
				_bindStrategy = new Colosoft.Query.TypeBindStrategy(typeof(T), objectCreator);
		}

		/// <summary>
		/// Aplica a ordenação.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="direction"></param>
		public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
		{
			var entries = new Query.SortEntry[] {
				new Query.SortEntry(new Query.Column(property.Name), direction == System.ComponentModel.ListSortDirection.Descending)
			};
			_queryable.OrderBy(new Query.Sort(entries));
			this.Refresh();
		}

		/// <summary>
		/// Aplica a ordenação.
		/// </summary>
		/// <param name="sortExpression"></param>
		public void ApplySort(string sortExpression)
		{
			_queryable.OrderBy(sortExpression);
			this.Refresh();
		}

		/// <summary>
		/// Remove a ordenação.
		/// </summary>
		public void RemoveSort()
		{
			string sortExpression = null;
			_queryable.OrderBy(sortExpression);
			this.Refresh();
		}

		/// <summary>
		/// Método acionado quando a consulta for executada.
		/// </summary>
		/// <param name="queryable"></param>
		/// <returns></returns>
		private IEnumerable<T> ExecuteSelect(Query.Queryable queryable)
		{
			IEnumerable<T> result = null;
			if(_executeSelect != null)
				result = _executeSelect(queryable, queryable.DataSource, _bindStrategy, _objectCreator);
			else
				result = queryable.Execute<T>(queryable.DataSource, _bindStrategy, _objectCreator);
			if(((IVirtualListResultProcessor<T>)this).HasDataPageLoadRegisters)
			{
				var dataPage = new DataPage();
				result = result.ToArray();
				dataPage.Populate(result);
				this.OnDataPageLoaded(new DataPageLoadedEventArgs<T>(dataPage));
			}
			return result;
		}

		/// <summary>
		/// Descrições dos parametros de pesquisa.
		/// </summary>
		SearchParameterDescriptionCollection ISearchParameterDescriptionContainer.SearchParameterDescriptions
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

		/// <summary>
		/// Tipo de elemento associado.
		/// </summary>
		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		/// <summary>
		/// Expressão associada com a instancia;
		/// </summary>
		System.Linq.Expressions.Expression IQueryable.Expression
		{
			get
			{
				return System.Linq.Expressions.Expression.Constant(this);
			}
		}

		/// <summary>
		/// Provedor de consulta.
		/// </summary>
		IQueryProvider IQueryable.Provider
		{
			get
			{
				if(_queryProvider == null)
					_queryProvider = Colosoft.Query.Linq.QueryProvider.Create<T>(_queryable, ExecuteSelect);
				return _queryProvider;
			}
		}

		/// <summary>
		/// Representa uma página de dados.
		/// </summary>
		class DataPage : Collections.IDataPage<T>
		{
			private IEnumerable<T> _items;

			/// <summary>
			/// Quantidade de itens.
			/// </summary>
			public int Count
			{
				get
				{
					return _items.Count();
				}
			}

			/// <summary>
			/// Identifica se possui erros.
			/// </summary>
			public bool HasError
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identifica se está populada.
			/// </summary>
			public bool IsPopulated
			{
				get
				{
					return _items != null;
				}
			}

			/// <summary>
			/// Notifica um erro.
			/// </summary>
			/// <param name="exception"></param>
			public void NotifyError(Exception exception)
			{
			}

			/// <summary>
			/// Popula a página.
			/// </summary>
			/// <param name="items"></param>
			public void Populate(IEnumerable<T> items)
			{
				_items = items;
			}

			/// <summary>
			/// Recupera e define o item na posição informada.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public T this[int index]
			{
				get
				{
					return _items.ElementAt(index);
				}
				set
				{
					if(_items is System.Collections.IList)
						((System.Collections.IList)_items)[index] = value;
				}
			}

			/// <summary>
			/// Recupera o enumerador dos itens.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<T> GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			/// <summary>
			/// Recupera o enumerador dos itens.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
			}
		}
	}
}
