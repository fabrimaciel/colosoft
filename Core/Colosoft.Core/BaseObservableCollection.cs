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
	/// Implementação base de uma coleção observada.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), Serializable, System.Diagnostics.DebuggerTypeProxy(typeof(ObservableCollectionDebugView<>)), System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class BaseObservableCollection<T> : ItemCollection<T>, IObservableCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IThreadSafeObservableCollection, INotifyCollectionChangedObserverContainer, INotifyCollectionChangedDispatcher, Colosoft.Threading.IReentrancyController, IResetableCollection, IDisposable, IDisposableState, IIndexedObservableCollection<T>, ISearchParameterDescriptionContainer
	{
		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> _collectionChangedHandlers;

		/// <summary>
		/// Instancia usada para armazena os eventos registrados das alterações na coleção.
		/// </summary>
		[NonSerialized]
		private NotifyCollectionChangedEventHandler _collectionChanged;

		[NonSerialized]
		private AggregateNotifyCollectionChangedObserver _observer = new AggregateNotifyCollectionChangedObserver();

		/// <summary>
		/// Lista das threads usadas para fazer o controle de reentrada.
		/// </summary>
		[NonSerialized]
		private List<System.Threading.Thread> _reentrancyThreads = new List<System.Threading.Thread>();

		private bool _disableThreadSafe = false;

		/// <summary>
		/// Armazena a relação dos indices da coleção.
		/// </summary>
		private Dictionary<string, Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>>> _indexes;

		[NonSerialized]
		private SearchParameterDescriptionCollection _searchParameterDescriptions;

		[NonSerialized]
		private bool _isDisposed;

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
		/// Evento acioando quando uma propriedade for alterada.
		/// </summary>
		protected event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Evento acioando quando uma propriedade for alterada.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add
			{
				PropertyChanged += value;
			}
			remove {
				PropertyChanged -= value;
			}
		}

		/// <summary>
		/// Identifica se a instancia já foi liberada.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
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
		/// Construtor padrão.
		/// </summary>
		public BaseObservableCollection()
		{
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
		}

		/// <summary>
		/// Cria uma instancia com base no enumerador informado.
		/// </summary>
		/// <param name="collection"></param>
		public BaseObservableCollection(IEnumerable<T> collection)
		{
			if(collection == null)
				throw new ArgumentNullException("collection");
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			this.CopyFrom(collection);
		}

		/// <summary>
		/// Cria um nova instancia com os dados da lista informada.
		/// </summary>
		/// <param name="list"></param>
		public BaseObservableCollection(List<T> list) : base((list != null) ? new List<T>(list.Count) : list)
		{
			this.CopyFrom(list);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		~BaseObservableCollection()
		{
			Dispose(false);
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
		/// Limpa os itens da coleção.
		/// </summary>
		protected override void ClearItems()
		{
			CheckReentrancy();
			while (Count > 0)
				this.RemoveAt(0);
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
		}

		/// <summary>
		/// Insere um novo item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, T item)
		{
			CheckReentrancy();
			base.InsertItem(index, item);
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex"></param>
		/// <param name="newIndex"></param>
		protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			this.CheckReentrancy();
			T item = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, item);
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
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
					{
						_observer.OnCollectionChanged(this, e);
						_collectionChanged(this, e);
					}
			}
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			CheckReentrancy();
			T item = base[index];
			base.RemoveItem(index);
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}

		/// <summary>
		/// Define o item para a posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, T item)
		{
			CheckReentrancy();
			T oldItem = base[index];
			base.SetItem(index, item);
			OnPropertyChanged("Item[]");
			OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
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
		/// Copia os dados para a coleção.
		/// </summary>
		/// <param name="collection"></param>
		private void CopyFrom(IEnumerable<T> collection)
		{
			IList<T> items = base.Items;
			if((collection != null) && (items != null))
			{
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
						items.Add(enumerator.Current);
				}
				if(collection is INotifyCollectionChangedObserverRegister)
					((INotifyCollectionChangedObserverRegister)collection).Register(this);
			}
		}

		/// <summary>
		/// Método acionado quando a coleção foi alterada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="item"></param>
		/// <param name="index"></param>
		private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
		}

		/// <summary>
		/// Reseta a coleção.
		/// </summary>
		internal protected void OnCollectionReset()
		{
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Reseta os dados da coleção.
		/// </summary>
		public void Reset()
		{
			OnCollectionReset();
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
		/// Verifica se a lista contem o item informado
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Contains(T item)
		{
			foreach (var i in Items)
			{
				if(item == null)
					continue;
				if(item.Equals(i))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex">Indice antigo</param>
		/// <param name="newIndex">Novo indice.</param>
		public void Move(int oldIndex, int newIndex)
		{
			MoveItem(oldIndex, newIndex);
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
		/// Identifica se a instância está o como de entrada ativo.
		/// </summary>
		bool Threading.IReentrancyController.IsReentrancy
		{
			get
			{
				lock (_reentrancyThreads)
					return _reentrancyThreads.Contains(System.Threading.Thread.CurrentThread);
			}
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
					if(i.Value != null)
						i.Value.Dispose();
			}
			_collectionChanged = null;
			base.ClearItems();
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
		/// Remove todos os indices.
		/// </summary>
		public void RemoveAllIndexes()
		{
			if(_indexes != null)
				lock (_indexes)
					_indexes.Clear();
		}

		/// <summary>
		/// Remove o indice.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="indexType"></param>
		public bool RemoveIndex(string propertyName, ObservableCollectionIndexType indexType)
		{
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
			if(_indexes != null)
				lock (_indexes)
					if(!_indexes.TryGetValue(propertyName, out values) && values.Remove(indexType))
					{
						if(values.Count == 0)
							_indexes.Remove(propertyName);
						return true;
					}
			return false;
		}

		/// <summary>
		/// Reseta o indice.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade do indice.</param>
		/// <param name="indexType">Tipo do indice.</param>
		public void ResetIndex(string propertyName, ObservableCollectionIndexType indexType)
		{
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
			if(_indexes == null)
				throw new IndexNotFoundException(propertyName);
			lock (_indexes)
				if(!_indexes.TryGetValue(propertyName, out values) || values.Count == 0 || (indexType != ObservableCollectionIndexType.Any && !values.ContainsKey(indexType)))
					throw new IndexNotFoundException(propertyName);
			IObservableCollectionIndex<T> index = null;
			if(indexType == ObservableCollectionIndexType.Any)
				index = values.Values.FirstOrDefault();
			else
				index = values[indexType];
			index.Reset();
		}

		/// <summary>
		/// Cria o indice para a propriedade informada.
		/// </summary>
		/// <typeparam name="PropertyType"></typeparam>
		/// <param name="type"></param>
		/// <param name="property"></param>
		public void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<T, PropertyType>> property, ObservableCollectionIndexType type)
		{
			CreateIndex<PropertyType>(property, type, Comparer<PropertyType>.Default);
		}

		/// <summary>
		/// Cria o indice para a propriedade informada.
		/// </summary>
		/// <typeparam name="PropertyType"></typeparam>
		/// <param name="type"></param>
		/// <param name="property"></param>
		/// <param name="comparer"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<T, PropertyType>> property, ObservableCollectionIndexType type, IComparer<PropertyType> comparer)
		{
			property.Require("property").NotNull();
			var propertyInfo = property.GetMember() as System.Reflection.PropertyInfo;
			if(propertyInfo == null)
				throw new InvalidOperationException("Invalid property");
			var indexName = propertyInfo.Name;
			if(_indexes != null)
				lock (_indexes)
				{
					Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
					if(_indexes.TryGetValue(indexName, out values) && values.ContainsKey(type))
					{
						return;
					}
				}
			IObservableCollectionIndex<T> index = null;
			var propertyGetter = property.Compile();
			var getter = new Func<T, object>(f => propertyGetter(f));
			if(type == ObservableCollectionIndexType.Sorted || type == ObservableCollectionIndexType.Hash)
				index = new ObservableCollectionSortedIndex<T>(indexName, this, new string[] {
					property.Name
				}, getter, Comparer<PropertyType>.Default);
			if(_indexes == null)
				_indexes = new Dictionary<string, Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>>>();
			lock (_indexes)
			{
				Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
				if(!_indexes.TryGetValue(indexName, out values))
				{
					values = new Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>>();
					_indexes.Add(indexName, values);
				}
				if(!values.ContainsKey(type))
					values.Add(type, index);
			}
		}

		/// <summary>
		/// Verifica se contém um indice para a propriedade informada.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade do indice.</param>
		/// <param name="indexType">Tipo de indice.</param>
		/// <returns></returns>
		public bool ContainsIndex(string propertyName, ObservableCollectionIndexType indexType)
		{
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
			if(_indexes == null)
				return false;
			lock (_indexes)
				if(!_indexes.TryGetValue(propertyName, out values) || values.Count == 0 || (indexType != ObservableCollectionIndexType.Any && !values.ContainsKey(indexType)))
					return false;
			return true;
		}

		/// <summary>
		/// Realiza a pesquisa usando o indice com a chave informada.
		/// </summary>
		/// <param name="property">Propriedade indexada.</param>
		/// <param name="key">Chave que será pesquisa.</param>
		/// <returns></returns>
		public IEnumerable<T> Search(System.Linq.Expressions.Expression<Func<T, object>> property, object key)
		{
			property.Require("property").NotNull();
			var propertyInfo = property.GetMember();
			return Search2(propertyInfo.Name, ObservableCollectionIndexType.Any, key);
		}

		/// <summary>
		/// Realiza a pesquisa usando o indice com a chave informada.
		/// </summary>
		/// <param name="property">Propriedade indexada.</param>
		/// <param name="indexType"></param>
		/// <param name="key">Chave que será pesquisa.</param>
		/// <returns></returns>
		public IEnumerable<T> Search(System.Linq.Expressions.Expression<Func<T, object>> property, ObservableCollectionIndexType indexType, object key)
		{
			property.Require("property").NotNull();
			var propertyInfo = property.GetMember();
			return Search2(propertyInfo.Name, indexType, key);
		}

		/// <summary>
		/// Realiza uma pesquisa no
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="indexType"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		System.Collections.IEnumerable IIndexedObservableCollection.Search(string propertyName, ObservableCollectionIndexType indexType, object key)
		{
			return Search2(propertyName, indexType, key);
		}

		/// <summary>
		/// Realiza a pesquisa no indice associado com o nome da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="indexType"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private IEnumerable<T> Search2(string propertyName, ObservableCollectionIndexType indexType, object key)
		{
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<T>> values = null;
			if(_indexes == null)
				throw new IndexNotFoundException(propertyName);
			lock (_indexes)
				if(!_indexes.TryGetValue(propertyName, out values) || values.Count == 0 || (indexType != ObservableCollectionIndexType.Any && !values.ContainsKey(indexType)))
					throw new IndexNotFoundException(propertyName);
			IObservableCollectionIndex<T> index = null;
			if(indexType == ObservableCollectionIndexType.Any)
				index = values.Values.FirstOrDefault();
			else
				index = values[indexType];
			return index[key];
		}

		/// <summary>
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private BaseObservableCollection<T> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(BaseObservableCollection<T> owner)
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

		SearchParameterDescriptionCollection ISearchParameterDescriptionContainer.SearchParameterDescriptions
		{
			get
			{
				if(_searchParameterDescriptions == null)
					_searchParameterDescriptions = new SearchParameterDescriptionCollection();
				return _searchParameterDescriptions;
			}
		}
	}
}
