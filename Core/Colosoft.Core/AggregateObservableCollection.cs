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
using System.Collections.Specialized;
using System.ComponentModel;

namespace Colosoft.Collections
{
	/// <summary>
	/// Representa um container de coleções observadas
	/// </summary>
	[Serializable, System.Diagnostics.DebuggerTypeProxy(typeof(ObservableCollectionDebugView<>)), System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class AggregateObservableCollection<T> : IObservableCollection<T>, IThreadSafeObservableCollection, INotifyCollectionChangedDispatcher, System.Collections.IList, IDisposable
	{
		private List<IObservableCollection<T>> _collections;

		private List<ChildMonitor> _monitors;

		private NotifyCollectionChangedEventHandler _collectionChanged;

		private PropertyChangedEventHandler _propertyChangedHandler;

		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> _collectionChangedHandlers;

		/// <summary>
		/// Lista das threads usadas para fazer o controle de reentrada.
		/// </summary>
		[NonSerialized]
		private List<System.Threading.Thread> _reentrancyThreads = new List<System.Threading.Thread>();

		private bool _disableThreadSafe = false;

		/// <summary>
		/// Evento acionado quando a coleção for alterada.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add
			{
				((INotifyCollectionChangedDispatcher)this).AddCollectionChanged(value, NotifyCollectionChangedDispatcherPriority.Normal);
			}
			remove {
				((INotifyCollectionChangedDispatcher)this).RemoveCollectionChanged(value);
			}
		}

		/// <summary>
		/// Evento acionado quando um propriedade for alterada.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged {
			add
			{
				_propertyChangedHandler += value;
			}
			remove {
				_propertyChangedHandler -= value;
			}
		}

		/// <summary>
		/// Identifica se a instancia está em modo de ThreadSafe.
		/// </summary>
		public virtual bool IsThreadSafe
		{
			get
			{
				return !_disableThreadSafe && Colosoft.Threading.DispatcherManager.Instance != null;
			}
		}

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				var count = 0;
				foreach (var i in _collections)
					count += i.Count;
				return count;
			}
		}

		/// <summary>
		/// Identifica se a coleção possui um tamanho fixado.
		/// </summary>
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se a coleção é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Instancia para sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return ((System.Collections.IList)_collections).SyncRoot;
			}
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public T this[int index]
		{
			get
			{
				var maxIndex = 0;
				foreach (var i in _collections)
				{
					if(index <= (maxIndex + (i.Count - 1)))
					{
						return i[index - maxIndex];
					}
					maxIndex += i.Count;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				var maxIndex = 0;
				foreach (var i in _collections)
				{
					if(index <= (maxIndex + (i.Count - 1)))
					{
						i[index - maxIndex] = value;
						break;
					}
					maxIndex = i.Count - 1;
				}
				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object System.Collections.IList.this[int index]
		{
			get
			{
				return (object)this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collections">Coleções que serão agregadas.</param>
		public AggregateObservableCollection(IEnumerable<IObservableCollection<T>> collections)
		{
			_collections = new List<IObservableCollection<T>>();
			_monitors = new List<ChildMonitor>();
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			var index = 0;
			foreach (var i in collections)
			{
				_monitors.Add(new ChildMonitor(this, i, index++));
				_collections.Add(i);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collections">Coleções que serão agregadas.</param>
		public AggregateObservableCollection(params IObservableCollection<T>[] collections)
		{
			_collections = new List<IObservableCollection<T>>();
			_monitors = new List<ChildMonitor>();
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			var index = 0;
			foreach (var i in collections)
			{
				_monitors.Add(new ChildMonitor(this, i, index++));
				_collections.Add(i);
			}
		}

		/// <summary>
		/// Desabilita o thread safe.
		/// </summary>
		void IThreadSafeObservableCollection.DisableThreadSafe()
		{
			_disableThreadSafe = true;
		}

		/// <summary>
		/// Habilita o thread safe.
		/// </summary>
		void IThreadSafeObservableCollection.EnableThreadSafe()
		{
			_disableThreadSafe = false;
		}

		/// <summary>
		/// Recupera o bloco de reentrada.
		/// </summary>
		/// <returns></returns>
		protected IDisposable BlockReentrancy()
		{
			return new ReentracyMonitor(this);
		}

		/// <summary>
		/// Método acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(this.IsThreadSafe)
			{
				if(_collectionChangedHandlers.Count > 0)
				{
					var handlers = _collectionChangedHandlers.ToArray();
					using (this.BlockReentrancy())
					{
						foreach (KeyValuePair<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> kvp in handlers)
							kvp.Value.Invoke(this, e);
					}
				}
			}
			else
			{
				if(_collectionChanged != null)
					using (this.BlockReentrancy())
						_collectionChanged(this, e);
			}
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex">Indice antigo</param>
		/// <param name="newIndex">Novo indice.</param>
		public void Move(int oldIndex, int newIndex)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Verifica se a coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			foreach (var i in _collections)
				if(i.Contains(item))
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se a coleção contém o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains((T)value);
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			var index = arrayIndex;
			foreach (var i in _collections)
			{
				if(index + 1 >= array.Length)
					return;
				i.CopyTo(array, index);
				index += i.Count;
			}
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			var index1 = index;
			foreach (var i in _collections)
			{
				if(index1 + 1 >= array.Length)
					return;
				((System.Collections.IList)i).CopyTo(array, index1);
				index1 += i.Count;
			}
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item)
		{
			var index = 0;
			foreach (var i in _collections)
			{
				var index2 = i.IndexOf(item);
				if(index2 >= 0)
					return index + index2;
				index += i.Count;
			}
			return -1;
		}

		/// <summary>
		/// Recupera o indice do valor na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.IndexOf(object value)
		{
			if(value is T)
				return IndexOf((T)value);
			else
				return -1;
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index">Indice onde o item será inserido.</param>
		/// <param name="item">Instancia do item que será inserido.</param>
		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index">Indice onde o item será inserido.</param>
		/// <param name="value">Instancia do valor que será inserido.</param>
		void System.Collections.IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Remove a instancia do item da coleção.
		/// </summary>
		/// <param name="item">Instancia do item que será removido.</param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			foreach (var i in _collections)
				if(i.Remove(item))
					return true;
			return false;
		}

		/// <summary>
		/// Remove a instancia do valor da coleção.
		/// </summary>
		/// <param name="value">Instancia do valor que será removido.</param>
		/// <returns></returns>
		void System.Collections.IList.Remove(object value)
		{
			if(!(value is T))
				return;
			var indexOf = ((System.Collections.IList)this).IndexOf(value);
			if(indexOf >= 0)
				RemoveAt(indexOf);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			var maxIndex = 0;
			foreach (var i in _collections)
			{
				if(index <= (maxIndex + (i.Count - 1)))
					i.RemoveAt(index);
				maxIndex = i.Count - 1;
			}
		}

		/// <summary>
		/// Recupera a enumerador da coleção.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach (var i in _collections)
				foreach (var j in i)
					yield return j;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
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
			foreach (var i in _monitors)
				i.Dispose();
			_monitors.Clear();
			_collections.Clear();
		}

		/// <summary>
		/// Adiciona o evento que será acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="priority"></param>
		void INotifyCollectionChangedDispatcher.AddCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler, NotifyCollectionChangedDispatcherPriority priority)
		{
			if(this.IsThreadSafe)
			{
				if(eventHandler != null)
				{
					var dispatcher = Colosoft.Threading.DispatcherManager.Dispatcher;
					if(dispatcher != null && !dispatcher.CheckAccess())
						dispatcher = null;
					if(!_collectionChangedHandlers.ContainsKey(eventHandler))
						_collectionChangedHandlers.Add(eventHandler, new CollectionChangedWrapperEventData(dispatcher, eventHandler, priority));
				}
			}
			else
				_collectionChanged += eventHandler;
		}

		/// <summary>
		/// Remove o evento registrado para ser acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		void INotifyCollectionChangedDispatcher.RemoveCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler)
		{
			if(this.IsThreadSafe)
			{
				_collectionChangedHandlers.Remove(eventHandler);
			}
			else
				_collectionChanged -= eventHandler;
		}

		/// <summary>
		/// Classe responsável por monitorar a coleção filha.
		/// </summary>
		sealed class ChildMonitor : IDisposable
		{
			private IObservableCollection<T> _collection;

			private AggregateObservableCollection<T> _owner;

			private int _collectionIndex = 0;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner">Instancia do proprietário.</param>
			/// <param name="collection">Coleção que será monitorada.</param>
			/// <param name="collectionIndex">Indice da coleção no pai.</param>
			public ChildMonitor(AggregateObservableCollection<T> owner, IObservableCollection<T> collection, int collectionIndex)
			{
				_owner = owner;
				_collection = collection;
				_collectionIndex = collectionIndex;
				collection.CollectionChanged += CollectionCollectionChanged;
				collection.PropertyChanged += CollectionPropertyChanged;
			}

			/// <summary>
			/// Método acioando quando uma propriedade da coleção for alterada.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void CollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if(_owner._propertyChangedHandler != null)
					_owner._propertyChangedHandler(_owner, e);
			}

			/// <summary>
			/// Método acionado quando a coleção associada é alterada.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void CollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				NotifyCollectionChangedEventArgs args = null;
				var startIndex = 0;
				for(var i = 0; i < _owner._collections.Count; i++)
				{
					if(i == _collectionIndex)
						break;
					startIndex += _owner._collections[i].Count;
				}
				System.Collections.ArrayList newItems = null;
				if(e.NewItems != null)
				{
					newItems = new System.Collections.ArrayList();
					for(var j = 0; j < e.NewItems.Count; j++)
						newItems.Add(_owner[startIndex + e.NewStartingIndex + j]);
				}
				System.Collections.ArrayList oldItems = null;
				if(e.OldItems != null)
				{
					oldItems = new System.Collections.ArrayList();
					for(var j = 0; j < e.OldItems.Count; j++)
						oldItems.Add(e.OldItems[j]);
				}
				switch(e.Action)
				{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, startIndex + e.NewStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldItems[0], startIndex + e.NewStartingIndex, startIndex + e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, startIndex + e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldItems[0], newItems[0], startIndex + e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
					break;
				default:
					return;
				}
				_owner.OnCollectionChanged(args);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_collection.PropertyChanged -= CollectionPropertyChanged;
				_collection.CollectionChanged -= CollectionCollectionChanged;
			}
		}

		/// <summary>
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private AggregateObservableCollection<T> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(AggregateObservableCollection<T> owner)
			{
				_owner = owner;
				lock (_owner._reentrancyThreads)
					_owner._reentrancyThreads.Add(System.Threading.Thread.CurrentThread);
			}

			~ReentracyMonitor()
			{
				Dispose();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				lock (_owner._reentrancyThreads)
					_owner._reentrancyThreads.Remove(System.Threading.Thread.CurrentThread);
			}
		}
	}
}
