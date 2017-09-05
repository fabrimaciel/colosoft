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

namespace Colosoft.Query
{
	/// <summary>
	/// Implementação do observer de alteração do resultado 
	/// de uma consulta.
	/// </summary>
	public abstract class QueryResultChangedObserver<T> : IQueryResultChangedObserver, Collections.INotifyCollectionChangedObserver, IDisposableState
	{
		private WeakReference _collectionReference;

		private ulong _uid;

		private bool _isDisposed;

		/// <summary>
		/// Instancia da coleção associada.
		/// </summary>
		protected System.Collections.IList Collection
		{
			get
			{
				return _collectionReference.Target as System.Collections.IList;
			}
		}

		/// <summary>
		/// Identificador unico do observer.
		/// </summary>
		public ulong Uid
		{
			get
			{
				return _uid;
			}
		}

		/// <summary>
		/// Identifica se o observer está vivo.
		/// </summary>
		public virtual bool IsAlive
		{
			get
			{
				if(!_collectionReference.IsAlive)
					return false;
				var collection = _collectionReference.Target;
				if(collection is IDisposableState)
					return !((IDisposableState)collection).IsDisposed;
				return collection != null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collection"></param>
		public QueryResultChangedObserver(System.Collections.IList collection)
		{
			collection.Require("collection").NotNull();
			_uid = QueryResultChangedObserverUidGenerator.CreateUid();
			_collectionReference = new WeakReference(collection);
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~QueryResultChangedObserver()
		{
			Dispose(false);
		}

		/// <summary>
		/// Cria um registro para a coleção com base no registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected abstract T CreateCollectionItem(IRecord record);

		/// <summary>
		/// Avalia se o registro informado é compatível.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public abstract bool Evaluate(IRecord record);

		/// <summary>
		/// Avalia se a chave do registro é compatível.
		/// </summary>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public abstract bool Evaluate(RecordKey recordKey);

		/// <summary>
		/// Método acionado quando um registro é criado.
		/// </summary>
		/// <param name="record"></param>
		public virtual void OnRecordInserted(IRecord record)
		{
			if(!IsAlive)
				return;
			var item = CreateCollectionItem(record);
			var collection = Collection;
			if(collection != null && !collection.Contains(item))
				collection.Add(item);
		}

		/// <summary>
		/// Método acionado quando um registro for removido.
		/// </summary>
		/// <param name="recordKey"></param>
		public abstract void OnRecordDeleted(RecordKey recordKey);

		/// <summary>
		/// Método acionado quando a coleção sore alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
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
