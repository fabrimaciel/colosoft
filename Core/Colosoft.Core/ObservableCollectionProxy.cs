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
	/// Implementação da uma coleção observada com suporte a adaptação dos seus itens.
	/// </summary>
	/// <typeparam name="T">Tipo da coleção que será adaptada.</typeparam>
	/// <typeparam name="TProxy">Tipo adaptado da coleção.</typeparam>
	[Serializable, System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class ObservableCollectionProxy<T, TProxy> : IObservableCollection<TProxy>, IResetableCollection, System.Collections.IList, INotifyCollectionChangedDispatcher, IThreadSafeObservableCollection, INotifyCollectionChangedObserverContainer, IDisposable
	{
		/// <summary>
		/// Instancia da coleção que está sendo adaptada.
		/// </summary>
		private IObservableCollection<T> _internalCollection;

		/// <summary>
		/// Delegate usado para criar o item da coleção.
		/// </summary>
		private Func<T, TProxy> _proxyCreator;

		/// <summary>
		/// Delegate usado para criar o item a partir do proxy.
		/// </summary>
		private Func<TProxy, T> _itemCreator;

		/// <summary>
		/// Evento acioando quando uma propriedade for alterada.
		/// </summary>
		private PropertyChangedEventHandler _propertyChanged;

		private IObservableCollection<ProxyEntry> _proxyCollection;

		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> _collectionChangedHandlers;

		/// <summary>
		/// Lista das threads usadas para fazer o controle de reentrada.
		/// </summary>
		[NonSerialized]
		private List<System.Threading.Thread> _reentrancyThreads = new List<System.Threading.Thread>();

		private bool _disableThreadSafe = false;

		private NotifyCollectionChangedEventHandler _collectionChanged;

		[NonSerialized]
		private AggregateNotifyCollectionChangedObserver _observer = new AggregateNotifyCollectionChangedObserver();

		private bool _ignoreProxyCollectionChanged;

		private bool _ignoreProxyPropertyChanged;

		private bool _isDisposed;

		/// <summary>
		/// Instancia do acumulador de evento de alteração na coleção proxy.
		/// </summary>
		[NonSerialized]
		private Queue<NotifyCollectionChangedEventArgs> _proxyCollectionChangesAccumulator = new Queue<NotifyCollectionChangedEventArgs>();

		[NonSerialized]
		private int _waitCollectionChangesCount;

		[NonSerialized]
		private readonly object _syncCollectionChange = new object();

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
				_propertyChanged += value;
			}
			remove {
				_propertyChanged -= value;
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
		/// Verifica se as altearções na coleção estão ocupadas.
		/// </summary>
		private bool IsCollectionChangesBusy
		{
			get
			{
				return _waitCollectionChangesCount > 0;
			}
		}

		/// <summary>
		/// Instancia da coleção base no qual o proxy está aplicado.
		/// </summary>
		public IObservableCollection<T> SourceCollection
		{
			get
			{
				return _internalCollection;
			}
		}

		/// <summary>
		/// Delegate usado para criar um item.
		/// </summary>
		private Func<TProxy, T> ItemCreator
		{
			get
			{
				if(_itemCreator == null)
					throw new InvalidOperationException("ItemCreator undefined");
				return _itemCreator;
			}
		}

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _proxyCollection.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção possui um tamanho fixado.
		/// </summary>
		public bool IsFixedSize
		{
			get
			{
				return ((System.Collections.IList)_internalCollection).IsFixedSize;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return ((System.Collections.IList)_internalCollection).IsReadOnly;
			}
		}

		/// <summary>
		/// Identifica se a coleção é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return ((System.Collections.IList)_internalCollection).IsSynchronized;
			}
		}

		/// <summary>
		/// Instancia para sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return ((System.Collections.IList)_internalCollection).SyncRoot;
			}
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TProxy this[int index]
		{
			get
			{
				lock (_syncCollectionChange)
				{
					return _proxyCollection[index].Proxy;
				}
			}
			set
			{
				_internalCollection[index] = ItemCreator(value);
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
				this[index] = (TProxy)value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collection">Coleção que será adaptada.</param>
		/// <param name="proxyCreator"></param>
		public ObservableCollectionProxy(IObservableCollection<T> collection, Func<T, TProxy> proxyCreator) : this(collection, proxyCreator, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collection">Coleção que será adaptada.</param>
		/// <param name="proxyCreator"></param>
		/// <param name="itemCreator"></param>
		public ObservableCollectionProxy(IObservableCollection<T> collection, Func<T, TProxy> proxyCreator, Func<TProxy, T> itemCreator)
		{
			collection.Require("collection").NotNull();
			proxyCreator.Require("proxyCreator").NotNull();
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			Initialize(collection, proxyCreator, itemCreator);
		}

		/// <summary>
		/// Construtor interno.
		/// </summary>
		internal ObservableCollectionProxy()
		{
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~ObservableCollectionProxy()
		{
			Dispose(false);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="proxyCreator"></param>
		/// <param name="itemCreator"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void Initialize(IObservableCollection<T> collection, Func<T, TProxy> proxyCreator, Func<TProxy, T> itemCreator)
		{
			_internalCollection = collection;
			_proxyCreator = proxyCreator;
			_itemCreator = itemCreator;
			_proxyCollection = new BaseObservableCollection<ProxyEntry>();
			foreach (var i in collection)
				_proxyCollection.Add(new ProxyEntry(_proxyCreator(i), i != null ? i.GetHashCode() : 0));
			_proxyCollection.PropertyChanged += ProxyCollectionPropertyChanged;
			_proxyCollection.CollectionChanged += ProxyCollectionCollectionChanged;
			if(collection is INotifyCollectionChangedDispatcher)
				((INotifyCollectionChangedDispatcher)collection).AddCollectionChanged(InnerCollectionChanged, NotifyCollectionChangedDispatcherPriority.High);
			else
				collection.CollectionChanged += InnerCollectionChanged;
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
		/// Verifica a reentrada.
		/// </summary>
		protected void CheckReentrancy()
		{
			lock (_reentrancyThreads)
				if(_reentrancyThreads.Contains(System.Threading.Thread.CurrentThread))
					throw new InvalidOperationException("Reentrancy not allowed");
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if(_propertyChanged != null)
				_propertyChanged(this, e);
		}

		/// <summary>
		/// Método acionado para notificar que a coleção foi alterada.
		/// </summary>
		/// <param name="e"></param>
		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
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
					{
						_observer.OnCollectionChanged(this, e);
						_collectionChanged(this, e);
					}
			}
		}

		/// <summary>
		/// Método acionado quando a coleção proxy for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProxyCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(!_ignoreProxyCollectionChanged)
			{
				System.Collections.ArrayList newItems = null;
				if(e.NewItems != null)
				{
					newItems = new System.Collections.ArrayList();
					for(var i = 0; i < e.NewItems.Count; i++)
						newItems.Add(e.NewItems[i] is ProxyEntry ? ((ProxyEntry)e.NewItems[i]).Proxy : default(TProxy));
				}
				System.Collections.ArrayList oldItems = null;
				if(e.OldItems != null)
				{
					oldItems = new System.Collections.ArrayList();
					for(var i = 0; i < e.OldItems.Count; i++)
						oldItems.Add(e.OldItems[i] is ProxyEntry ? ((ProxyEntry)e.OldItems[i]).Proxy : default(TProxy));
				}
				switch(e.Action)
				{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldItems[0], e.NewStartingIndex, e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItems[0], newItems[0], e.OldStartingIndex);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
					break;
				default:
					return;
				}
				if(IsCollectionChangesBusy)
					_proxyCollectionChangesAccumulator.Enqueue(e);
				else
					OnCollectionChanged(e);
			}
		}

		/// <summary>
		/// Método acionado quando uma propriedade da coleção proxy for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProxyCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(!(_ignoreProxyPropertyChanged || (_propertyChanged == null)))
				_propertyChanged(this, e);
		}

		/// <summary>
		/// Verifica se a instancia já foi liberada.
		/// </summary>
		/// <returns></returns>
		private void CheckDisposed()
		{
			if(_isDisposed)
				throw new ObjectDisposedException(this.ToString());
		}

		/// <summary>
		/// Notifica a alteração de uma propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		private void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Método acioando quando a coleção adaptada sofrer alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InnerCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var collection = sender as Colosoft.Collections.IObservableCollection<T>;
			var newStartingIndex = e.NewStartingIndex < 0 ? 0 : e.NewStartingIndex;
			var oldStartingIndex = e.OldStartingIndex < 0 ? 0 : e.OldStartingIndex;
			ProxyMonitor monitor = null;
			switch(e.Action)
			{
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				monitor = new ProxyMonitor(this);
				try
				{
					for(int i = 0; i < e.NewItems.Count; i++)
					{
						_proxyCollection.Insert(newStartingIndex + i, new ProxyEntry(_proxyCreator((T)e.NewItems[i]), e.NewItems[i] != null ? e.NewItems[i].GetHashCode() : 0));
					}
				}
				finally
				{
					monitor.Free();
					monitor = null;
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
				monitor = new ProxyMonitor(this);
				try
				{
					if(e.OldItems.Count == 1)
					{
						_proxyCollection.Move(oldStartingIndex, newStartingIndex);
					}
					else
					{
						var items = _proxyCollection.Skip(oldStartingIndex).Take(e.OldItems.Count).ToList();
						for(int i = 0; i < e.OldItems.Count; i++)
							_proxyCollection.RemoveAt(oldStartingIndex + i);
						for(int i = 0; i < items.Count; i++)
							_proxyCollection.Insert(newStartingIndex + i, items[i]);
					}
				}
				finally
				{
					monitor.Free();
					monitor = null;
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				monitor = new ProxyMonitor(this);
				try
				{
					for(int i = 0; i < e.OldItems.Count; i++)
						_proxyCollection.RemoveAt(oldStartingIndex + i);
				}
				finally
				{
					monitor.Free();
					monitor = null;
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				NotifyCollectionChangedEventArgs e2 = null;
				try
				{
					_ignoreProxyCollectionChanged = true;
					_ignoreProxyPropertyChanged = true;
					var newItems = new System.Collections.ArrayList();
					var oldItems = new System.Collections.ArrayList();
					monitor = new ProxyMonitor(this);
					try
					{
						for(int i = 0; i < e.OldItems.Count; i++)
						{
							oldItems.Add(_proxyCollection[oldStartingIndex + i]);
							_proxyCollection.RemoveAt(oldStartingIndex + i);
						}
						for(int i = 0; i < e.NewItems.Count; i++)
						{
							var entry = new ProxyEntry(_proxyCreator((T)e.NewItems[i]), e.NewItems[i] != null ? e.NewItems[i].GetHashCode() : 0);
							newItems.Add(entry.Proxy);
							_proxyCollection.Insert(newStartingIndex + i, entry);
						}
					}
					finally
					{
						monitor.Free();
						monitor = null;
					}
					e2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItems[0], newItems[0], e.OldStartingIndex);
				}
				finally
				{
					_ignoreProxyCollectionChanged = false;
					_ignoreProxyPropertyChanged = false;
				}
				OnCollectionChanged(e2);
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				try
				{
					_ignoreProxyCollectionChanged = true;
					_ignoreProxyPropertyChanged = true;
					monitor = new ProxyMonitor(this);
					try
					{
						_proxyCollection.Clear();
						foreach (T local in collection)
							_proxyCollection.Add(new ProxyEntry(_proxyCreator(local), local != null ? local.GetHashCode() : 0));
					}
					finally
					{
						monitor.Free();
						monitor = null;
					}
				}
				finally
				{
					_ignoreProxyCollectionChanged = false;
					_ignoreProxyPropertyChanged = false;
				}
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				break;
			default:
				break;
			}
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex">Indice antigo</param>
		/// <param name="newIndex">Novo indice.</param>
		public void Move(int oldIndex, int newIndex)
		{
			CheckDisposed();
			_internalCollection.Move(oldIndex, newIndex);
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(TProxy item)
		{
			CheckDisposed();
			_internalCollection.Add(ItemCreator(item));
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(object value)
		{
			Add((TProxy)value);
			return IndexOf(value);
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			CheckDisposed();
			_internalCollection.Clear();
		}

		/// <summary>
		/// Verifica se a coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(TProxy item)
		{
			if(_isDisposed)
				return false;
			return _proxyCollection.Contains(new ProxyEntry(item, 0), ProxyEntryEqualityComparer.Instance);
		}

		/// <summary>
		/// Verifica se a coleção contém o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(object value)
		{
			if(_isDisposed)
				return false;
			return Contains((TProxy)value);
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(TProxy[] array, int arrayIndex)
		{
			CheckDisposed();
			for(int i = arrayIndex, j = 0; i < array.Length && j < this.Count; i++, j++)
				array[i] = this[j];
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			CheckDisposed();
			for(int i = index, j = 0; i < array.Length && j < this.Count; i++, j++)
				array.SetValue(this[j], i);
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(TProxy item)
		{
			if(_isDisposed)
				return -1;
			CheckDisposed();
			var item2 = new ProxyEntry(item, 0);
			for(var i = 0; i < _proxyCollection.Count; i++)
				if(ProxyEntryEqualityComparer.Instance.Equals(_proxyCollection[i], item2))
					return i;
			return -1;
		}

		/// <summary>
		/// Recupera o indice do valor na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(object value)
		{
			if(_isDisposed)
				return -1;
			CheckDisposed();
			if(value is TProxy)
				return IndexOf((TProxy)value);
			else
				return -1;
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index">Indice onde o item será inserido.</param>
		/// <param name="item">Instancia do item que será inserido.</param>
		public void Insert(int index, TProxy item)
		{
			CheckDisposed();
			_internalCollection.Insert(index, ItemCreator(item));
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index">Indice onde o item será inserido.</param>
		/// <param name="value">Instancia do valor que será inserido.</param>
		public void Insert(int index, object value)
		{
			Insert(index, (TProxy)value);
		}

		/// <summary>
		/// Remove a instancia do item da coleção.
		/// </summary>
		/// <param name="item">Instancia do item que será removido.</param>
		/// <returns></returns>
		public bool Remove(TProxy item)
		{
			CheckDisposed();
			var indexOf = this.IndexOf(item);
			if(indexOf >= 0)
			{
				_internalCollection.RemoveAt(indexOf);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove a instancia do valor da coleção.
		/// </summary>
		/// <param name="value">Instancia do valor que será removido.</param>
		/// <returns></returns>
		public void Remove(object value)
		{
			if(!(value is TProxy))
				return;
			CheckDisposed();
			var indexOf = this.IndexOf(value);
			if(indexOf >= 0)
				_internalCollection.RemoveAt(indexOf);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			CheckDisposed();
			_internalCollection.RemoveAt(index);
		}

		/// <summary>
		/// Reseta os dados da instancia.
		/// </summary>
		public void Reset()
		{
			if(_internalCollection is IResetableCollection)
			{
				((IResetableCollection)_internalCollection).Reset();
			}
		}

		/// <summary>
		/// Recupera a enumerador da coleção.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TProxy> GetEnumerator()
		{
			return new ProxyEntryEnumerator(_proxyCollection.GetEnumerator());
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new ProxyEntryEnumerator(_proxyCollection.GetEnumerator());
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
			_isDisposed = true;
			_observer.Dispose();
			if(_collectionChangedHandlers != null)
			{
				var handlers = _collectionChangedHandlers.ToArray();
				_collectionChangedHandlers.Clear();
				foreach (var i in handlers)
					i.Value.Dispose();
			}
			_collectionChanged = null;
			if(_internalCollection != null)
				_internalCollection.CollectionChanged -= InnerCollectionChanged;
			if(_proxyCollection != null)
				_proxyCollection.PropertyChanged -= ProxyCollectionPropertyChanged;
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
		/// Adiciona um observer para a instancia.
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="liveScope"></param>
		void INotifyCollectionChangedObserverContainer.AddObserver(INotifyCollectionChangedObserver observer, NotifyCollectionChangedObserverLiveScope liveScope)
		{
			observer.Require("observer").NotNull();
			_observer.Add(observer, liveScope);
		}

		/// <summary>
		/// Remove o observer da coleção.
		/// </summary>
		/// <param name="observer"></param>
		void INotifyCollectionChangedObserverContainer.RemoveObserver(INotifyCollectionChangedObserver observer)
		{
			observer.Require("observer").NotNull();
			_observer.Remove(observer);
		}

		/// <summary>
		/// Representa uma entrada do proxy.
		/// </summary>
		class ProxyEntry
		{
			/// <summary>
			/// Instancia do proxy associada.
			/// </summary>
			public TProxy Proxy;

			/// <summary>
			/// Hashcode da instancia original.
			/// </summary>
			public int OriginalHashCode;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="proxy"></param>
			/// <param name="originalHashCode"></param>
			public ProxyEntry(TProxy proxy, int originalHashCode)
			{
				Proxy = proxy;
				OriginalHashCode = originalHashCode;
			}
		}

		/// <summary>
		/// Comparador padrão.
		/// </summary>
		class ProxyEntryEqualityComparer : IEqualityComparer<ProxyEntry>
		{
			public static ProxyEntryEqualityComparer Instance = new ProxyEntryEqualityComparer();

			/// <summary>
			/// Compara as instancias informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(ProxyEntry x, ProxyEntry y)
			{
				return !object.ReferenceEquals(x.Proxy, null) && x.Proxy.Equals(y.Proxy);
			}

			/// <summary>
			/// Recupera o hashcode da instancia informada.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(ProxyEntry obj)
			{
				return obj != null && obj.Proxy != null ? obj.Proxy.GetHashCode() : 0;
			}
		}

		/// <summary>
		/// Implementação do enumerator para o Proxy.
		/// </summary>
		class ProxyEntryEnumerator : IEnumerator<TProxy>
		{
			private IEnumerator<ProxyEntry> _enumerator;

			public TProxy Current
			{
				get
				{
					return _enumerator.Current != null ? _enumerator.Current.Proxy : default(TProxy);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _enumerator.Current != null ? _enumerator.Current.Proxy : default(TProxy);
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerator"></param>
			public ProxyEntryEnumerator(IEnumerator<ProxyEntry> enumerator)
			{
				_enumerator = enumerator;
			}

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}
		}

		/// <summary>
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private ObservableCollectionProxy<T, TProxy> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(ObservableCollectionProxy<T, TProxy> owner)
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

		sealed class ProxyMonitor
		{
			private ObservableCollectionProxy<T, TProxy> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ProxyMonitor(ObservableCollectionProxy<T, TProxy> owner)
			{
				_owner = owner;
				if(_owner._waitCollectionChangesCount == 0)
					System.Threading.Monitor.Enter(_owner._syncCollectionChange);
				_owner._waitCollectionChangesCount++;
			}

			/// <summary>
			/// Libera ao monitor.
			/// </summary>
			public void Free()
			{
				_owner._waitCollectionChangesCount--;
				if(_owner._waitCollectionChangesCount == 0)
				{
					System.Threading.Monitor.Exit(_owner._syncCollectionChange);
					if(_owner._proxyCollectionChangesAccumulator.Count > 0)
					{
						var args = _owner._proxyCollectionChangesAccumulator.ToArray();
						_owner._proxyCollectionChangesAccumulator.Clear();
						foreach (var i in args)
							_owner.OnCollectionChanged(i);
					}
				}
			}
		}
	}
}
