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
	/// Armazena o argumento da alteração.
	/// </summary>
	public class TreeViewSourceNodeSelectionChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Instancia do nó selecionado.
		/// </summary>
		public ITreeViewSourceNode Node
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Implementação de uma estrutura de árvore.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class TreeViewSource : NotificationObject, IEnumerable<ITreeViewSourceNode>, System.Collections.Specialized.INotifyCollectionChanged, INotifyCollectionChangedDispatcher
	{
		private System.Collections.IEnumerable _items;

		private Func<object, IMessageFormattable> _nameGetter;

		private Func<object, object> _keyGetter;

		private Func<object, object> _ownerKeyGetter;

		private string _namePropertyName;

		private string _keyPropertyName;

		private string _ownerPropertyName;

		private bool _isMultiSelect;

		private object _objLock = new object();

		private IObservableCollection<ITreeViewSourceNode> _selectedNodes = new BaseObservableCollection<ITreeViewSourceNode>();

		private TreeViewSourceNode _root;

		private System.Collections.IEqualityComparer _keyEqualityComparer;

		private bool _isInitializeExpanded;

		private bool _isLazyLoad;

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
		/// Evento acionado quando a seleção de um nó for alterado.
		/// Esse evento só funciona para o nó raiz.
		/// </summary>
		public event EventHandler<TreeViewSourceNodeSelectionChangedEventArgs> NodeSelectionChanged;

		/// <summary>
		/// Método acionado quando o nó selecionado for alterado.
		/// </summary>
		public event EventHandler SelectedNodeChanged;

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
		/// Identifica se a instancia é multiseleção.
		/// </summary>
		public bool IsMultiSelect
		{
			get
			{
				return _isMultiSelect;
			}
			set
			{
				if(_isMultiSelect != value)
				{
					_isMultiSelect = value;
					RaisePropertyChanged("IsMultiSelect");
				}
			}
		}

		/// <summary>
		/// Nome da propriedade chave.
		/// </summary>
		public string KeyPropertyName
		{
			get
			{
				return _keyPropertyName;
			}
		}

		/// <summary>
		/// Nome da propriedade nome.
		/// </summary>
		public string NamePropertyName
		{
			get
			{
				return _namePropertyName;
			}
		}

		/// <summary>
		/// Nome da propriedade Owner.
		/// </summary>
		public string OwnerPropertyName
		{
			get
			{
				return _ownerPropertyName;
			}
		}

		/// <summary>
		/// Identifica se a árvore está configurada para carregar em modo Lazy.
		/// </summary>
		public bool IsLazyLoad
		{
			get
			{
				return _isLazyLoad;
			}
		}

		/// <summary>
		/// Seleciona o item informado.
		/// </summary>
		public object SelectedItem
		{
			get
			{
				return _selectedNodes.FirstOrDefault();
			}
			set
			{
				if(value != _selectedNodes.FirstOrDefault())
				{
					if(value == null)
						UnselectedAll();
					else
					{
						var node = FindNodeFromItem(value);
						node.IsSelected = true;
					}
				}
			}
		}

		/// <summary>
		/// Instancia do nó selecionado.
		/// </summary>
		public ITreeViewSourceNode SelectedNode
		{
			get
			{
				return _selectedNodes.FirstOrDefault();
			}
		}

		/// <summary>
		/// Relação dos nós selecionados.
		/// </summary>
		protected IObservableCollection<ITreeViewSourceNode> SelectedNodes
		{
			get
			{
				return _selectedNodes;
			}
		}

		/// <summary>
		/// Itens associados.
		/// </summary>
		public System.Collections.IEnumerable Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Instancia do comparador das chaves dos nós
		/// </summary>
		public System.Collections.IEqualityComparer KeyEqualityComparer
		{
			get
			{
				return _keyEqualityComparer;
			}
		}

		/// <summary>
		/// Identifica se é para inicializar com os itens expandidos.
		/// </summary>
		public bool IsInitializeExpanded
		{
			get
			{
				return _isInitializeExpanded;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="items">Items que serão tratados pela árvore.</param>
		/// <param name="keyGetter"></param>
		/// <param name="keyPropertyName"></param>
		/// <param name="ownerKeyGetter"></param>
		/// <param name="ownerKeyPropertyName"></param>
		/// <param name="nameGetter"></param>
		/// <param name="namePropertyName"></param>
		/// <param name="rootItem">Item da raiz da árvore.</param>
		/// <param name="isInitializeExpanded"></param>
		/// <param name="isLazyLoad">Identifica se a árvore possui cargar tardia.</param>
		/// <param name="keyEqualityComparer"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private TreeViewSource(System.Collections.IEnumerable items, Func<object, object> keyGetter, string keyPropertyName, Func<object, object> ownerKeyGetter, string ownerKeyPropertyName, Func<object, IMessageFormattable> nameGetter, string namePropertyName, object rootItem, bool isInitializeExpanded, bool isLazyLoad, System.Collections.IEqualityComparer keyEqualityComparer)
		{
			items.Require("items");
			_items = items;
			_keyGetter = keyGetter;
			_keyPropertyName = keyPropertyName;
			_ownerKeyGetter = ownerKeyGetter;
			_ownerPropertyName = ownerKeyPropertyName;
			_nameGetter = nameGetter;
			_namePropertyName = namePropertyName;
			_isInitializeExpanded = isInitializeExpanded;
			_keyEqualityComparer = keyEqualityComparer;
			_isLazyLoad = isLazyLoad;
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();
			_root = new TreeViewSourceNode(this, rootItem);
			_root.CollectionChanged += RootCollectionChanged;
		}

		/// <summary>
		/// Pesquisa o nó para o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private ITreeViewSourceNode FindNodeFromItem(object item)
		{
			return FindNodeFromItem(_root, item);
		}

		/// <summary>
		/// Pesquisa o nó para o item informado.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		private ITreeViewSourceNode FindNodeFromItem(ITreeViewSourceNode owner, object item)
		{
			if(owner == null)
				return null;
			foreach (var i in owner)
				if(i.Value == item)
					return i;
				else
				{
					var node = FindNodeFromItem(i, item);
					if(node != null)
						return node;
				}
			return null;
		}

		/// <summary>
		/// Cria o comparador para a chave.
		/// </summary>
		/// <param name="keyPropertyInfo"></param>
		/// <returns></returns>
		private static System.Collections.IEqualityComparer CreateKeyComparer(System.Reflection.PropertyInfo keyPropertyInfo)
		{
			if(keyPropertyInfo.PropertyType.IsNullable())
				switch(keyPropertyInfo.PropertyType.GetGenericParameterConstraints()[0].Name)
				{
				case "Int32":
					return EqualityComparer<int?>.Default;
				case "Int16":
					return EqualityComparer<short?>.Default;
				case "Int64":
					return EqualityComparer<long?>.Default;
				case "Double":
					return EqualityComparer<double?>.Default;
				case "Single":
					return EqualityComparer<float?>.Default;
				case "DateTime":
					return EqualityComparer<DateTime?>.Default;
				case "DateTimeOffset":
					return EqualityComparer<DateTimeOffset?>.Default;
				}
			else
				switch(keyPropertyInfo.PropertyType.Name)
				{
				case "Int32":
					return EqualityComparer<int>.Default;
				case "Int16":
					return EqualityComparer<short>.Default;
				case "Int64":
					return EqualityComparer<long>.Default;
				case "String":
					return EqualityComparer<string>.Default;
				case "Double":
					return EqualityComparer<double>.Default;
				case "Single":
					return EqualityComparer<float>.Default;
				case "DateTime":
					return EqualityComparer<DateTime>.Default;
				case "DateTimeOffset":
					return EqualityComparer<DateTimeOffset>.Default;
				}
			return null;
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
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RootCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
		}

		/// <summary>
		/// Remove a seleção de todos os nós.
		/// </summary>
		public void UnselectedAll()
		{
			ITreeViewSourceNode[] nodes = null;
			lock (_selectedNodes)
				nodes = _selectedNodes.ToArray();
			foreach (var i in nodes)
				i.IsSelected = false;
		}

		/// <summary>
		/// Cria uma instancia do TreeViewSource.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="keyProperty"></param>
		/// <param name="ownerKeyProperty"></param>
		/// <param name="nameProperty"></param>
		/// <param name="root">Instancia do item da raiz.</param>
		/// <param name="isInitializeExpanded"></param>
		/// <param name="isLazyLoad"></param>
		/// <param name="keyEqualityComparer"></param>
		/// <returns></returns>
		public static TreeViewSource Create<T>(IEnumerable<T> items, System.Linq.Expressions.Expression<Func<T, object>> keyProperty, System.Linq.Expressions.Expression<Func<T, object>> ownerKeyProperty, System.Linq.Expressions.Expression<Func<T, string>> nameProperty, T root, bool isInitializeExpanded = false, bool isLazyLoad = false, System.Collections.IEqualityComparer keyEqualityComparer = null)
		{
			items.Require("items").NotNull();
			var keyPropertyInfo = keyProperty.GetMember() as System.Reflection.PropertyInfo;
			var keyGetterCompiled = keyProperty.Compile();
			var keyGetter = new Func<object, object>(f =>  {
				var item = (T)f;
				return keyGetterCompiled(item);
			});
			var ownerKeyPropertyInfo = ownerKeyProperty.GetMember() as System.Reflection.PropertyInfo;
			var ownerKeyGetterCompiled = ownerKeyProperty.Compile();
			var ownerKeyGetter = new Func<object, object>(f => ownerKeyGetterCompiled((T)f));
			var namePropertyInfo = nameProperty.GetMember() as System.Reflection.PropertyInfo;
			var namePropertyCompiled = nameProperty.Compile();
			var nameGetter = new Func<object, IMessageFormattable>(f => namePropertyCompiled((T)f).GetFormatter());
			return new TreeViewSource(items, keyGetter, keyPropertyInfo.Name, ownerKeyGetter, ownerKeyPropertyInfo.Name, nameGetter, namePropertyInfo.Name, root, isInitializeExpanded, isLazyLoad, keyEqualityComparer ?? CreateKeyComparer(keyPropertyInfo));
		}

		/// <summary>
		/// Cria uma instancia do TreeViewSource.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="keyProperty"></param>
		/// <param name="ownerKeyProperty"></param>
		/// <param name="nameProperty"></param>
		/// <param name="root">Instancia do item da raiz.</param>
		/// <param name="isInitializeExpanded"></param>
		/// <param name="isLazyLoad"></param>
		/// <param name="keyEqualityComparer"></param>
		/// <returns></returns>
		public static TreeViewSource Create<T>(IEnumerable<T> items, System.Linq.Expressions.Expression<Func<T, object>> keyProperty, System.Linq.Expressions.Expression<Func<T, object>> ownerKeyProperty, System.Linq.Expressions.Expression<Func<T, IMessageFormattable>> nameProperty, T root, bool isInitializeExpanded = false, bool isLazyLoad = false, System.Collections.IEqualityComparer keyEqualityComparer = null)
		{
			items.Require("items").NotNull();
			var keyPropertyInfo = keyProperty.GetMember() as System.Reflection.PropertyInfo;
			var keyGetterCompiled = keyProperty.Compile();
			var keyGetter = new Func<object, object>(f => keyGetterCompiled((T)f));
			var ownerKeyPropertyInfo = ownerKeyProperty.GetMember() as System.Reflection.PropertyInfo;
			var ownerKeyGetterCompiled = ownerKeyProperty.Compile();
			var ownerKeyGetter = new Func<object, object>(f => ownerKeyGetterCompiled((T)f));
			var namePropertyInfo = nameProperty.GetMember() as System.Reflection.PropertyInfo;
			var namePropertyCompiled = nameProperty.Compile();
			var nameGetter = new Func<object, IMessageFormattable>(f => namePropertyCompiled((T)f));
			return new TreeViewSource(items, keyGetter, keyPropertyInfo.Name, ownerKeyGetter, ownerKeyPropertyInfo.Name, nameGetter, namePropertyInfo.Name, root, isInitializeExpanded, isLazyLoad, keyEqualityComparer ?? CreateKeyComparer(keyPropertyInfo));
		}

		/// <summary>
		/// Método acionado quando a seleção do nó for alterada.
		/// </summary>
		/// <param name="node"></param>
		internal void OnNodeSelectionChanged(ITreeViewSourceNode node)
		{
			lock (_objLock)
				if(IsMultiSelect)
				{
					if(node.IsSelected)
					{
						foreach (var i in _selectedNodes)
							if(i != node)
								i.IsSelected = false;
						_selectedNodes.Clear();
						_selectedNodes.Add(node);
					}
					else
						_selectedNodes.Remove(node);
				}
				else
				{
					if(node.IsSelected && !_selectedNodes.Contains(node))
						_selectedNodes.Add(node);
					else if(!node.IsSelected)
						_selectedNodes.Remove(node);
				}
			if(NodeSelectionChanged != null)
				NodeSelectionChanged(this, new TreeViewSourceNodeSelectionChangedEventArgs {
					Node = node
				});
			if(node.IsSelected && SelectedNodeChanged != null)
				SelectedNodeChanged(this, EventArgs.Empty);
			RaisePropertyChanged("SelectedItem", "SelectedNode");
		}

		/// <summary>
		/// Recupera o nome do item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal IMessageFormattable GetName(object item)
		{
			return _nameGetter(item);
		}

		/// <summary>
		/// Recupera o valor da chave para o item informado.
		/// </summary>
		internal object GetKey(object item)
		{
			return _keyGetter(item);
		}

		/// <summary>
		/// Recupera a chave do proprietário do item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal object GetOwnerKey(object item)
		{
			return _ownerKeyGetter(item);
		}

		/// <summary>
		/// Recupera o enumerador dos nós.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ITreeViewSourceNode> GetEnumerator()
		{
			return _root.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos nós.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _root.GetEnumerator();
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
				_root.CollectionChanged += eventHandler;
		}

		/// <summary>
		/// Remove o evento registrado para ser acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		void INotifyCollectionChangedDispatcher.RemoveCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler)
		{
			if(this.IsThreadSafe)
				_collectionChangedHandlers.Remove(eventHandler);
			else
				_root.CollectionChanged -= eventHandler;
		}

		/// <summary>
		/// Implementação usada para monitorar reentradas de chamada.
		/// </summary>
		sealed class ReentracyMonitor : IDisposable
		{
			private TreeViewSource _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public ReentracyMonitor(TreeViewSource owner)
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
	/// <summary>
	/// Assinatura de um nó da arvore.
	/// </summary>
	public interface ITreeViewSourceNode : System.Collections.IList, IList<ITreeViewSourceNode>, System.Collections.Specialized.INotifyCollectionChanged
	{
		/// <summary>
		/// Chave do nó.
		/// </summary>
		object Key
		{
			get;
		}

		/// <summary>
		/// Nome do nó.
		/// </summary>
		IMessageFormattable Name
		{
			get;
		}

		/// <summary>
		/// Valor do nó.
		/// </summary>
		object Value
		{
			get;
		}

		/// <summary>
		/// Identifica se o nó está expandido.
		/// </summary>
		bool IsExpanded
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se o nó está selecionado.
		/// </summary>
		bool IsSelected
		{
			get;
			set;
		}
	}
}
