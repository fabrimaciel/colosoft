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

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação do enumerador do resultado de uma consulta.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	abstract class QueryResultEnumerable<T> : IEnumerable<T>, Collections.INotifyCollectionChangedObserverRegister, IDisposable, IDisposableState where T : IEntity
	{
		private IEnumerable<T> _enumerable;

		private QueryInfo _info;

		private IQueryResult _result;

		private bool _isDisposed;

		/// <summary>
		/// Instancia do resultado associado.
		/// </summary>
		protected IQueryResult Result
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Recupera as informações da consulta associada.
		/// </summary>
		protected QueryInfo QueryInfo
		{
			get
			{
				return _info == null ? (_result is IQueryResultExt ? ((IQueryResultExt)_result).QueryInfo : null) : _info;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerable">Instancia do Enumerable que será adaptada.</param>
		/// <param name="result">Instancia do resulta associado.</param>
		/// <param name="info">Informações da consulta.</param>
		public QueryResultEnumerable(IEnumerable<T> enumerable, IQueryResult result, QueryInfo info = null)
		{
			_enumerable = enumerable;
			_result = result;
			_info = info;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~QueryResultEnumerable()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o nome do tipo principal associado com o resultado da consulta.
		/// </summary>
		/// <returns></returns>
		protected virtual Colosoft.Reflection.TypeName GetTypeName()
		{
			var info = QueryInfo;
			if(info != null && info.Entities.Length > 0)
				return new Colosoft.Reflection.TypeName(QueryInfo.Entities[0].FullName);
			return null;
		}

		/// <summary>
		/// Cria o observer para a coleção do resultado da consulta.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="collection"></param>
		protected abstract Colosoft.Collections.INotifyCollectionChangedObserver CreateQueryResultChangedObserver(Colosoft.Reflection.TypeName typeName, System.Collections.IList collection);

		/// <summary>
		/// Registra os observers no container.
		/// </summary>
		/// <param name="container"></param>
		public virtual void Register(Collections.INotifyCollectionChangedObserverContainer container)
		{
			var collection = container as System.Collections.IList;
			var typeName = GetTypeName();
			if(collection != null && typeName != null)
			{
				var observer = CreateQueryResultChangedObserver(typeName, collection);
				if(observer != null)
					container.AddObserver(observer, Collections.NotifyCollectionChangedObserverLiveScope.Instance);
			}
		}

		/// <summary>
		/// Recupera o enumerador associado.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(_enumerable.GetEnumerator());
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
			if(_enumerable is IDisposable)
				((IDisposable)_enumerable).Dispose();
			if(_result != null)
				_result.Dispose();
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
		/// Implementação do Enumerator para o resultado.
		/// </summary>
		sealed class Enumerator : IEnumerator<T>
		{
			private IEnumerator<T> _innerEnumerator;

			/// <summary>
			/// Recupera a atual instancia do enumerador.
			/// </summary>
			public T Current
			{
				get
				{
					return (T)_innerEnumerator.Current;
				}
			}

			/// <summary>
			/// Recupera a atual instancia do enumerador.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _innerEnumerator.Current;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerator">Instancia do enumerador adaptado.</param>
			public Enumerator(IEnumerator<T> enumerator)
			{
				_innerEnumerator = enumerator;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_innerEnumerator.Dispose();
			}

			/// <summary>
			/// Move para o próximo item.
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
