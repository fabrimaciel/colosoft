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
	/// Implementação da coleção observada que da suporte para filtro.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable, System.Diagnostics.DebuggerTypeProxy(typeof(ObservableCollectionDebugView<>)), System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class FilteredObservableCollection<T> : IObservableCollection<T>, System.Collections.IList, IDisposable, IThreadSafeObservableCollection, INotifyCollectionChangedDispatcher, Colosoft.Collections.IFilteredObservableCollection
	{
		private Dictionary<int, int> _indexReference = new Dictionary<int, int>();

		private IObservableCollection<T> _collection;

		private Predicate<T> _filter;

		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> _collectionChangedHandlers;

		private event NotifyCollectionChangedEventHandler _collectionChanged;

		private event PropertyChangedEventHandler _propertychanged;

		private bool _disableThreadSafe = false;

		/// <summary>
		/// Lista das threads usadas para fazer o controle de reentrada.
		/// </summary>
		[NonSerialized]
		private List<System.Threading.Thread> _reentrancyThreads = new List<System.Threading.Thread>();

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
				_propertychanged += value;
			}
			remove {
				_propertychanged -= value;
			}
		}

		/// <summary>
		/// Origem dos dados filtrados.
		/// </summary>
		public IObservableCollection Source
		{
			get
			{
				return _collection;
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
				if(_filter == null)
					return _collection.Count;
				else
				{
					int count = 0;
					for(int i = 0; i < _collection.Count; i++)
					{
						T item = _collection[i];
						if(_filter(item))
							count++;
					}
					return count;
				}
			}
		}

		/// <summary>
		/// Filtro que será aplicado a coleção.
		/// </summary>
		public Predicate<T> Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				if(_filter != value)
				{
					_filter = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("Filter"));
				}
			}
		}

		/// <summary>
		/// Filtra que será aplicado a coleção.
		/// </summary>
		Predicate<object> IFilteredObservableCollection.Filter
		{
			get
			{
				if(Filter != null)
					return new Predicate<object>(f => Filter((T)f));
				return null;
			}
			set
			{
				if(value != null)
					Filter = new Predicate<T>(f => value(f));
				else
					Filter = null;
			}
		}

		/// <summary>
		/// Identifica se a coleção possui um tamanho fixado.
		/// </summary>
		public bool IsFixedSize
		{
			get
			{
				return ((System.Collections.IList)_collection).IsFixedSize;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return ((System.Collections.IList)_collection).IsReadOnly;
			}
		}

		/// <summary>
		/// Identifica se a coleção é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return ((System.Collections.IList)_collection).IsSynchronized;
			}
		}

		/// <summary>
		/// Instancia para sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return ((System.Collections.IList)_collection).SyncRoot;
			}
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
				if(_filter == null)
					return _collection[index];
				else
				{
					for(int i = 0; i < _collection.Count; i++)
					{
						T indexitem = _collection[i];
						if(_filter(indexitem))
						{
							if(index == 0)
								return indexitem;
							else
								index--;
						}
					}
					throw new ArgumentOutOfRangeException("index");
				}
			}
			set
			{
				if(_filter == null)
					_collection[index] = value;
				else if(_filter(value) == false)
					throw new InvalidOperationException();
				else
				{
					for(int i = 0; i < _collection.Count; i++)
					{
						T indexitem = _collection[i];
						if(_filter(indexitem))
						{
							if(index == 0)
								_collection[i] = value;
							else
								index--;
						}
					}
					throw new ArgumentOutOfRangeException();
				}
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
		/// <param name="collection">Coleção que será filtrada.</param>
		/// <param name="filter">Predicado para o filtro.</param>
		public FilteredObservableCollection(IObservableCollection<T> collection, Predicate<T> filter)
		{
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			_filter = filter;
			_collection = collection;
			ResetIndexReference();
			if(collection is INotifyCollectionChangedDispatcher)
				((INotifyCollectionChangedDispatcher)collection).AddCollectionChanged(OnInnerCollectionChanged, NotifyCollectionChangedDispatcherPriority.High);
			else
				_collection.CollectionChanged += OnInnerCollectionChanged;
			((INotifyPropertyChanged)_collection).PropertyChanged += OnPropertyChanged;
		}

		/// <summary>
		/// Destrutor
		/// </summary>
		~FilteredObservableCollection()
		{
			Dispose(false);
		}

		/// <summary>
		/// Realiza um reset das referencias de indice.
		/// </summary>
		private void ResetIndexReference()
		{
			lock (_indexReference)
			{
				if(_indexReference != null)
				{
					_indexReference.Clear();
					int index1 = 0, index2 = 0;
					if(_collection != null)
					{
						var aux = new Dictionary<int, int>();
						var restartAttempts = 0;
						restart1:
						aux.Clear();
						using (var enumerator = _collection.GetEnumerator())
						{
							while (true)
							{
								try
								{
									if(!enumerator.MoveNext())
										break;
								}
								catch(InvalidOperationException)
								{
									restartAttempts++;
									if(restartAttempts > 5)
										throw;
									goto restart1;
								}
								var item = enumerator.Current;
								aux.Add(index1++, (_filter == null || _filter(item) ? index2++ : -1));
							}
						}
						foreach (var i in aux)
							_indexReference.Add(i.Key, i.Value);
					}
				}
			}
		}

		/// <summary>
		/// Método acionado quando a coleção que está filtrar sofrer alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			List<T> newlist = new List<T>();
			if(e.NewItems != null)
				foreach (T item in e.NewItems)
					if(_filter(item))
						newlist.Add(item);
			List<Tuple<int, T>> oldlist = new List<Tuple<int, T>>();
			var index = 0;
			if(e.OldItems != null)
				foreach (T item in e.OldItems)
				{
					if(_filter(item))
						oldlist.Add(new Tuple<int, T>(e.OldStartingIndex + index, item));
					index++;
				}
			List<T> addlist = new List<T>();
			var removelist = new List<Tuple<int, T>>();
			List<T> replacelist = new List<T>();
			foreach (T item in newlist)
				if(oldlist.Any(f => object.Equals(f.Item2, item)))
					replacelist.Add(item);
				else
					addlist.Add(item);
			foreach (var item in oldlist)
				if(newlist.Any(f => object.Equals(f, item.Item2)))
					continue;
				else
					removelist.Add(item);
			switch(e.Action)
			{
			case NotifyCollectionChangedAction.Move:
				int oldIndex = -1;
				int newIndex = -1;
				lock (_indexReference)
				{
					if(!_indexReference.TryGetValue(e.OldStartingIndex, out oldIndex))
						oldIndex = -1;
					if(!_indexReference.TryGetValue(e.NewStartingIndex, out newIndex))
						newIndex = -1;
				}
				ResetIndexReference();
				if(newIndex >= 0 && oldIndex >= 0)
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems[0], newIndex, oldIndex));
				else if(newIndex >= 0)
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.OldItems[0], e.NewItems[0], newIndex));
				break;
			case NotifyCollectionChangedAction.Add:
				lock (_indexReference)
				{
					var indexRef1 = _indexReference.Select(f => f.Value).ToList();
					_indexReference.Clear();
					for(int i = e.NewStartingIndex, x = 0; i < (e.NewStartingIndex + e.NewItems.Count); i++, x++)
					{
						index = IndexOf(e.NewItems[x]);
						if(index >= 0)
							for(var j = 0; j < indexRef1.Count; j++)
								if(indexRef1[j] >= index)
									indexRef1[j]++;
						indexRef1.Insert(i, index);
					}
					for(int i = 0; i < indexRef1.Count; i++)
						_indexReference.Add(i, indexRef1[i]);
				}
				foreach (var addItem in addlist)
				{
					T originalItem = default(T);
					index = -1;
					int count = 0;
					for(int i = 0; i < _collection.Count; i++)
					{
						T indexItem = _collection[i];
						if(_filter(indexItem))
						{
							if(indexItem.Equals(addItem))
							{
								index = count;
								originalItem = indexItem;
							}
							else
								count++;
						}
					}
					if(index >= 0)
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T> {
							originalItem
						}, index));
				}
				break;
			case NotifyCollectionChangedAction.Replace:
				foreach (var i in replacelist)
				{
					index = IndexOf(i);
					if(index >= 0)
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<T> {
							i
						}, index));
				}
				break;
			case NotifyCollectionChangedAction.Remove:
				if(removelist.Count > 0)
				{
					foreach (var i in removelist)
					{
						lock (_indexReference)
							if(!_indexReference.TryGetValue(i.Item1, out index))
							{
								index = -1;
							}
						if(index >= 0)
							OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T> {
								i.Item2
							}, index));
					}
				}
				lock (_indexReference)
				{
					for(var i = e.OldStartingIndex; i < (e.OldStartingIndex + e.OldItems.Count); i++)
					{
						_indexReference.Remove(i);
					}
					var indexRef3 = _indexReference.Select(f => f.Value).ToList();
					_indexReference.Clear();
					var lastIndex = -1;
					for(int i = 0, j = 0; i < indexRef3.Count; i++)
					{
						if(indexRef3[i] > lastIndex)
						{
							lastIndex = indexRef3[i];
							indexRef3[i] = j++;
						}
					}
					for(int i = 0; i < indexRef3.Count; i++)
						_indexReference.Add(i, indexRef3[i]);
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				ResetIndexReference();
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				break;
			}
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(_propertychanged != null)
				_propertychanged(this, e);
		}

		/// <summary>
		/// Reseta a coleção.
		/// </summary>
		public void Reset()
		{
			ResetIndexReference();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex">Indice antigo</param>
		/// <param name="newIndex">Novo indice.</param>
		public void Move(int oldIndex, int newIndex)
		{
			var oldIndex2 = -1;
			var newIndex2 = -1;
			if(_filter != null)
			{
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						if(oldIndex == 0)
							oldIndex2 = i;
						else
							oldIndex--;
					}
				}
				if(oldIndex2 < 0)
					throw new ArgumentOutOfRangeException("oldIndex");
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						if(newIndex == 0)
							newIndex2 = i;
						else
							newIndex--;
					}
				}
				if(newIndex2 < 0)
					throw new ArgumentOutOfRangeException("newIndex");
			}
			_collection.Move(oldIndex2, newIndex2);
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			if(_filter != null && _filter(item) == false)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperation_ItemIsNotMatchingWithFilter).Format());
			_collection.Add(item);
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(object value)
		{
			Add((T)value);
			return IndexOf(value);
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			if(_filter == null)
				_collection.Clear();
			else
				for(int i = 0; i < _collection.Count;)
				{
					T item = _collection[i];
					if(_filter(item))
						_collection.RemoveAt(i);
					else
						i++;
				}
		}

		/// <summary>
		/// Verifica se a coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			if(_filter != null && _filter(item) == false)
				return false;
			return _collection.Contains(item);
		}

		/// <summary>
		/// Verifica se a coleção contém o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(object value)
		{
			if(value == null)
				return false;
			if(_filter != null && _filter((T)value) == false)
				return false;
			return ((System.Collections.IList)_collection).Contains(value);
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			if(_filter == null)
				_collection.CopyTo(array, arrayIndex);
			else
			{
				for(int i = 0; i < _collection.Count; i++)
				{
					T item = _collection[i];
					if(_filter(item))
						array[arrayIndex++] = item;
				}
			}
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			if(_filter == null)
				((System.Collections.IList)_collection).CopyTo(array, index);
			else
			{
				for(int i = 0; i < _collection.Count; i++)
				{
					T item = _collection[i];
					if(_filter(item))
						array.SetValue(item, index++);
				}
			}
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item)
		{
			if(_filter == null)
				return _collection.IndexOf(item);
			else
			{
				int count = 0;
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						if(indexitem.Equals(item))
							return count;
						else
							count++;
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// Recupera o indice do valor na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(object value)
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
			if(_filter != null && _filter(item) == false)
				throw new InvalidOperationException();
			if(_filter == null || index == 0)
				_collection.Insert(index, item);
			else
			{
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						index--;
						if(index == 0)
						{
							_collection.Insert(i + 1, item);
							return;
						}
					}
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index">Indice onde o item será inserido.</param>
		/// <param name="value">Instancia do valor que será inserido.</param>
		public void Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		/// <summary>
		/// Remove a instancia do item da coleção.
		/// </summary>
		/// <param name="item">Instancia do item que será removido.</param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			if(_filter == null)
				return _collection.Remove(item);
			else
			{
				int count = 0;
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						if(indexitem.Equals(item))
						{
							_collection.RemoveAt(i);
							return true;
						}
						else
							count++;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Remove a instancia do valor da coleção.
		/// </summary>
		/// <param name="value">Instancia do valor que será removido.</param>
		/// <returns></returns>
		public void Remove(object value)
		{
			if(!(value is T) || (_filter != null && _filter((T)value) == false))
				return;
			((System.Collections.IList)_collection).Remove(value);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			if(_filter == null)
				_collection.RemoveAt(index);
			else
			{
				for(int i = 0; i < _collection.Count; i++)
				{
					T indexitem = _collection[i];
					if(_filter(indexitem))
					{
						if(index == 0)
						{
							_collection.RemoveAt(i);
							return;
						}
						else
							index--;
					}
				}
				throw new ArgumentOutOfRangeException();
			}
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
					var handlers = _collectionChangedHandlers.OrderByDescending(f => (int)f.Value.Priority).ToArray();
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
		/// Recupera a enumerador da coleção.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return new FilteredEnumerator(this, _collection.GetEnumerator());
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new FilteredEnumerator(this, _collection.GetEnumerator());
		}

		/// <summary>
		/// Implementação do enumerador dos itens da coleção filtrada.
		/// </summary>
		private class FilteredEnumerator : IEnumerator<T>, System.Collections.IEnumerator
		{
			private FilteredObservableCollection<T> _filteredcollection;

			private IEnumerator<T> _enumerator;

			public FilteredEnumerator(FilteredObservableCollection<T> filteredcollection, IEnumerator<T> enumerator)
			{
				_filteredcollection = filteredcollection;
				_enumerator = enumerator;
			}

			public T Current
			{
				get
				{
					if(_filteredcollection.Filter == null)
						return _enumerator.Current;
					else if(_filteredcollection.Filter(_enumerator.Current) == false)
						throw new InvalidOperationException();
					else
						return _enumerator.Current;
				}
			}

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				while (true)
				{
					if(_enumerator.MoveNext() == false)
						return false;
					if(_filteredcollection.Filter == null || _filteredcollection.Filter(_enumerator.Current) == true)
						return true;
				}
			}

			public void Reset()
			{
				_enumerator.Reset();
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_collection != null)
			{
				_collection.CollectionChanged -= OnInnerCollectionChanged;
				((INotifyPropertyChanged)_collection).PropertyChanged -= OnPropertyChanged;
			}
			if(_indexReference != null)
				_indexReference.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
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
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private FilteredObservableCollection<T> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(FilteredObservableCollection<T> owner)
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
