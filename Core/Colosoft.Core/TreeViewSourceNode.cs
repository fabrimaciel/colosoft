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

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação genérico do nó de uma arvore.
	/// </summary>
	class TreeViewSourceNode : NotificationObject, ITreeViewSourceNode, IDisposable, IThreadSafeObservableCollection, INotifyCollectionChangedDispatcher
	{
		private static readonly TreeViewSourceNode Empty = new TreeViewSourceNode(null, null);

		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> _collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();

		/// <summary>
		/// Instancia usada para armazena os eventos registrados das alterações na coleção.
		/// </summary>
		[NonSerialized]
		private NotifyCollectionChangedEventHandler _collectionChanged;

		/// <summary>
		/// Lista das threads usadas para fazer o controle de reentrada.
		/// </summary>
		[NonSerialized]
		private List<System.Threading.Thread> _reentrancyThreads = new List<System.Threading.Thread>();

		private bool _disableThreadSafe = false;

		[NonSerialized]
		private TreeViewSource _tree;

		private object _item;

		private TreeViewSourceNode _owner;

		private TreeViewSourceNode _left;

		private TreeViewSourceNode _right;

		private TreeViewSourceNode _firstChild;

		private bool _isExpanded;

		private bool _isSelected;

		/// <summary>
		/// Identifica se o nó está vazio.
		/// </summary>
		private bool _isEmpty = false;

		private int _count;

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
		/// Identifica se a instancia está em modo de ThreadSafe.
		/// </summary>
		public virtual bool IsThreadSafe
		{
			get
			{
				return !_disableThreadSafe && Threading.DispatcherManager.Instance != null;
			}
		}

		/// <summary>
		/// Nó proprietário.
		/// </summary>
		private TreeViewSourceNode Owner
		{
			get
			{
				if(_owner == null && _left != null)
					return _left.Owner;
				return _owner;
			}
		}

		/// <summary>
		/// Nó da esquerda.
		/// </summary>
		internal TreeViewSourceNode Left
		{
			get
			{
				return _left;
			}
		}

		/// <summary>
		/// Nó da direita.
		/// </summary>
		internal TreeViewSourceNode Right
		{
			get
			{
				return _right;
			}
			private set
			{
				_right = value;
			}
		}

		/// <summary>
		/// Primeiro filho
		/// </summary>
		internal TreeViewSourceNode FirstChild
		{
			get
			{
				return _firstChild;
			}
			private set
			{
				_firstChild = value;
			}
		}

		/// <summary>
		/// Quantidade de itens no nó.
		/// </summary>
		public int Count
		{
			get
			{
				if(_firstChild != null || _count > 0 || _isEmpty)
					return _count;
				var tree = GetTree();
				if(tree != null && tree.IsLazyLoad && !IsExpanded)
					return 1;
				if(_firstChild == null && !_isEmpty)
					using (var enumerator = GetEnumerator())
						while (enumerator.MoveNext())
							;
				return _count;
			}
		}

		/// <summary>
		/// Instancia da árvore onde o nó está inserido.
		/// </summary>
		public TreeViewSource GetTree()
		{
			if(_tree != null)
				return _tree;
			var owner = Owner;
			if(owner != null)
				return owner.GetTree();
			return null;
		}

		/// <summary>
		/// Nome do item associado com o nó.
		/// </summary>
		public IMessageFormattable Name
		{
			get
			{
				if(_item == null)
					return MessageFormattable.Empty;
				var tree = GetTree();
				return tree == null ? MessageFormattable.Empty : tree.GetName(_item);
			}
		}

		/// <summary>
		/// Chave do nó.
		/// </summary>
		public object Key
		{
			get
			{
				if(_item == null)
					return null;
				return GetTree().GetKey(_item);
			}
		}

		/// <summary>
		/// Valor do nó.
		/// </summary>
		public object Value
		{
			get
			{
				return _item;
			}
		}

		/// <summary>
		/// Identifica que o nó está ou não expandido.
		/// </summary>
		public bool IsExpanded
		{
			get
			{
				return _item == null || _isExpanded;
			}
			set
			{
				if(_isExpanded != value)
				{
					_isExpanded = value;
					RaisePropertyChanged("IsExpanded");
				}
			}
		}

		/// <summary>
		/// Identifica que o nó está selecionado.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if(_isSelected != value)
				{
					_isSelected = value;
					GetTree().OnNodeSelectionChanged(this);
					RaisePropertyChanged("IsSelected");
				}
			}
		}

		/// <summary>
		/// Cria um nó para a raiz da árvore.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="item"></param>
		internal TreeViewSourceNode(TreeViewSource tree, object item)
		{
			_tree = tree;
			_item = item;
			_isExpanded = true;
			Initialize();
			_isExpanded = true;
		}

		/// <summary>
		/// Cria um nó filho.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="left">Nó da esquerda.</param>
		/// <param name="item"></param>
		private TreeViewSourceNode(TreeViewSourceNode owner, TreeViewSourceNode left, object item)
		{
			_owner = owner;
			_left = left;
			_item = item;
			if(item is System.ComponentModel.INotifyPropertyChanged)
				((System.ComponentModel.INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
			Initialize();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~TreeViewSourceNode()
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
		/// Limpa os itens do cache.
		/// </summary>
		protected void ClearCache()
		{
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
						_collectionChanged(this, e);
					}
			}
		}

		/// <summary>
		/// Método acionado quando uma propriedade do item for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var tree = GetTree();
			if(e.PropertyName == tree.NamePropertyName)
				RaisePropertyChanged("Name");
			else if(e.PropertyName == tree.KeyPropertyName)
				RaisePropertyChanged("Key");
			else if(e.PropertyName == tree.OwnerPropertyName)
			{
				var owner = Owner;
				if(owner != null)
					owner.ResetCollection();
			}
		}

		/// <summary>
		/// Inicializa o nó.
		/// </summary>
		private void Initialize()
		{
			var tree = GetTree();
			if(tree != null)
			{
				_isExpanded = tree.IsInitializeExpanded;
				var items = tree.Items;
				if(items is System.Collections.Specialized.INotifyCollectionChanged)
					((System.Collections.Specialized.INotifyCollectionChanged)items).CollectionChanged += ItemsCollectionChanged;
			}
		}

		/// <summary>
		/// Método acionado quando a coleção dos itens for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			System.Collections.IList items = null;
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
				items = e.OldItems;
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
				items = e.NewItems;
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				ResetCollection();
				return;
			}
			var tree = GetTree();
			foreach (var item in items)
			{
				var ownerKey = tree.GetOwnerKey(item);
				if((_tree == null && ownerKey == null) || KeysEquals(tree, ownerKey, Key))
				{
					ResetCollection();
					return;
				}
			}
		}

		/// <summary>
		/// Verifica se as chaves informadas são iguais.
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <returns></returns>
		private static bool KeysEquals(TreeViewSource tree, object key1, object key2)
		{
			if(tree.KeyEqualityComparer != null)
				return tree.KeyEqualityComparer.Equals(key1, key2);
			return (key1 == null && key2 == null) || (key1 != null && key1.Equals(key2)) || (key2 != null && key2.Equals(key1));
		}

		/// <summary>
		/// Reseta a coleção.
		/// </summary>
		private void ResetCollection()
		{
			FirstChild = null;
			ClearCache();
			OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("K: {0}; N: {1}", this.Key, this.Name);
		}

		/// <summary>
		/// Recupera a enumereação a partir do primeiro filho.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ITreeViewSourceNode> GetEnumeratorByFirstChild()
		{
			var node = FirstChild;
			while (node != null)
			{
				yield return node;
				node = node.Right;
			}
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ITreeViewSourceNode> GetEnumerator()
		{
			return FirstChild != null ? GetEnumeratorByFirstChild().GetEnumerator() : new Enumerator(this);
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<ITreeViewSourceNode>)this).GetEnumerator();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_firstChild = null;
			_owner = null;
			_left = null;
			_right = null;
			if(_item is System.ComponentModel.INotifyPropertyChanged)
				((System.ComponentModel.INotifyPropertyChanged)_item).PropertyChanged -= ItemPropertyChanged;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Adiciona um item para a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.Add(object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		void System.Collections.IList.Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Verifica se no nó existe algum filho com o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool System.Collections.IList.Contains(object value)
		{
			if(FirstChild != null)
			{
				var node = FirstChild;
				while (node != null)
				{
					if(node.Value == value)
						return true;
					node = node.Right;
				}
			}
			return false;
		}

		/// <summary>
		/// Localiza o indice do valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.IndexOf(object value)
		{
			var num = 0;
			if(FirstChild != null)
			{
				var node = FirstChild;
				while (node != null)
				{
					if(node.Value == value)
						return num;
					node = node.Right;
					num++;
				}
			}
			return -1;
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Identifica se possui um tamanho fixado.
		/// </summary>
		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se é uma coleçao somente leitura.
		/// </summary>
		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Remove o valor informado.
		/// </summary>
		/// <param name="value"></param>
		void System.Collections.IList.Remove(object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		void System.Collections.IList.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object System.Collections.IList.this[int index]
		{
			get
			{
				if(GetTree().IsLazyLoad && !IsExpanded)
					return Empty;
				int num = 0;
				if(FirstChild != null)
				{
					var node = FirstChild;
					while (node != null)
					{
						if(num == index)
							return node;
						node = node.Right;
						num++;
					}
				}
				return Empty;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Localiza o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		int IList<ITreeViewSourceNode>.IndexOf(ITreeViewSourceNode item)
		{
			return ((System.Collections.IList)this).IndexOf(item);
		}

		/// <summary>
		/// Insere o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		void IList<ITreeViewSourceNode>.Insert(int index, ITreeViewSourceNode item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		void IList<ITreeViewSourceNode>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		ITreeViewSourceNode IList<ITreeViewSourceNode>.this[int index]
		{
			get
			{
				return (ITreeViewSourceNode)((System.Collections.IList)this)[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Adiciona o item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		void ICollection<ITreeViewSourceNode>.Add(ITreeViewSourceNode item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		void ICollection<ITreeViewSourceNode>.Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Verifica se a coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool ICollection<ITreeViewSourceNode>.Contains(ITreeViewSourceNode item)
		{
			return ((System.Collections.IList)this).Contains(item);
		}

		/// <summary>
		/// Copia os dados da lista.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		void ICollection<ITreeViewSourceNode>.CopyTo(ITreeViewSourceNode[] array, int arrayIndex)
		{
			var count = array.Length;
			foreach (var i in this.Skip(arrayIndex))
				if(count == 0)
					break;
				else
					array[array.Length - (count--)] = i;
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		bool ICollection<ITreeViewSourceNode>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool ICollection<ITreeViewSourceNode>.Remove(ITreeViewSourceNode item)
		{
			throw new NotImplementedException();
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
					var dispatcher = Threading.DispatcherManager.Dispatcher;
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
		/// Implementação do enumerador do nó.
		/// </summary>
		class Enumerator : IEnumerator<ITreeViewSourceNode>
		{
			public const ObservableCollectionIndexType DefaultIndexType = ObservableCollectionIndexType.Any;

			private TreeViewSource _tree;

			private TreeViewSourceNode _node;

			private System.Collections.IEnumerator _itemEnumerator;

			private TreeViewSourceNode _current;

			/// <summary>
			/// Identifica que a propriedade que armazena o ponteiro para o pai é indexada.
			/// </summary>
			private bool _isOwnerPropertyIndexed;

			/// <summary>
			/// Instancia do atual nó.
			/// </summary>
			public ITreeViewSourceNode Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Instancia do atual nó.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="node"></param>
			public Enumerator(TreeViewSourceNode node)
			{
				_tree = node.GetTree();
				_node = node;
			}

			/// <summary>
			/// Move para o próximo nó.
			/// </summary>
			public bool MoveNext()
			{
				var oldCount = _node._count;
				if(_tree == null)
					return false;
				if(_tree.IsLazyLoad && !_node.IsExpanded)
				{
					if(_current == null)
					{
						_current = TreeViewSourceNode.Empty;
						return true;
					}
					return false;
				}
				if(_itemEnumerator == null)
				{
					_node.FirstChild = null;
					if(_tree.Items is IIndexedObservableCollection)
					{
						var indexedCollection = ((IIndexedObservableCollection)_tree.Items);
						if(indexedCollection.ContainsIndex(_tree.OwnerPropertyName, DefaultIndexType))
						{
							_itemEnumerator = indexedCollection.Search(_tree.OwnerPropertyName, DefaultIndexType, _node.Key).GetEnumerator();
							_isOwnerPropertyIndexed = true;
						}
						else
							_isOwnerPropertyIndexed = false;
					}
					else
						_isOwnerPropertyIndexed = false;
					if(!_isOwnerPropertyIndexed)
						_itemEnumerator = _tree.Items.GetEnumerator();
				}
				object ownerKey = null;
				while (_itemEnumerator.MoveNext())
				{
					if(!_isOwnerPropertyIndexed)
						ownerKey = _tree.GetOwnerKey(_itemEnumerator.Current);
					if(_isOwnerPropertyIndexed || KeysEquals(_tree, _node.Key, ownerKey))
					{
						var old = _current;
						_current = new TreeViewSourceNode(_current == null ? _node : null, _current == null ? null : _current, _itemEnumerator.Current);
						if(_node.FirstChild == null)
							_node.FirstChild = _current;
						if(old != null)
							old.Right = _current;
						_node._count++;
						return true;
					}
				}
				_node._isEmpty = (_node._count == 0);
				if(_node._count != oldCount)
					_node.RaisePropertyChanged("Count");
				return false;
			}

			/// <summary>
			/// Reseta o enumerador.
			/// </summary>
			public void Reset()
			{
				_current = null;
				if(_itemEnumerator != null)
					_itemEnumerator.Reset();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_current")]
			public void Dispose()
			{
				if(_itemEnumerator is IDisposable)
					((IDisposable)_itemEnumerator).Dispose();
				_current = null;
				_itemEnumerator = null;
			}
		}

		/// <summary>
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private TreeViewSourceNode _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(TreeViewSourceNode owner)
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
