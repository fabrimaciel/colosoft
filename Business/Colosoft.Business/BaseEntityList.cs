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
using Colosoft.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação base de uma lista de entidade.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public abstract class BaseEntityList<TEntity> : IEntityList<TEntity>, System.ComponentModel.IBindingList, IConnectedEntity, ILoadableEntity, IEntityPersistence, Collections.IThreadSafeObservableCollection, Collections.INotifyCollectionChangedObserverContainer, Collections.INotifyCollectionChangedDispatcher, Threading.IReentrancyController, Collections.IResetableCollection, ISaveOperationsContainer, IEntitySavePersistenceSessionObserver, IEntityInstanceRegister, System.Xml.Serialization.IXmlSerializable, Colosoft.Collections.IIndexedObservableCollection<TEntity>, IDisposableState where TEntity : IEntity
	{
		/// <summary>
		/// Comparador de items na lista.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class ItemComparer<T> : IEqualityComparer<T>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(T x, T y)
			{
				Colosoft.Collections.IUniqueItemInList<T> uniqueItem = x as Colosoft.Collections.IUniqueItemInList<T>;
				if(uniqueItem != null)
				{
					return uniqueItem.IsEqual(y);
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(T obj)
			{
				return obj.GetHashCode();
			}
		}

		private List<TEntity> _newItems;

		private List<TEntity> _removedItems;

		[NonSerialized]
		private object _syncRoot;

		/// <summary>
		/// Lista na qual o objeto está incluído.
		/// </summary>
		private Colosoft.Collections.IObservableCollection _myList;

		/// <summary>
		/// Instancia usada para carregar o itens de forma tardia.
		/// </summary>
		private Lazy<IEnumerable<TEntity>> _loadItems;

		private Colosoft.Collections.BaseObservableCollection<TEntity> _innerList;

		private IEntity _owner;

		private bool _isOwnerDefined;

		/// <summary>
		/// Identifica se owner foi definido.
		/// </summary>
		public bool IsOwnerDefined
		{
			get
			{
				return _isOwnerDefined;
			}
		}

		private Action<TEntity> _parentUidSetter;

		private bool _isLoaded;

		private bool _isDisposed;

		private bool _isEditing;

		[NonSerialized]
		private System.Collections.Specialized.NotifyCollectionChangedEventHandler _collectionChanged;

		[NonSerialized]
		private Dictionary<NotifyCollectionChangedEventHandler, Colosoft.Collections.CollectionChangedWrapperEventData> _collectionChangedHandlers;

		[NonSerialized]
		private System.ComponentModel.ListChangedEventHandler _listChanged;

		[NonSerialized]
		private Dictionary<System.ComponentModel.ListChangedEventHandler, Colosoft.Collections.ListChangedWrapperEventData> _listChangedHandlers;

		/// <summary>
		/// Instancia da transação de edição.
		/// </summary>
		private IEntityList _editingTransactionInstance;

		private string _uiContext;

		private EntityMonitor _lockMonitor;

		private Colosoft.Query.ISourceContext _sourceContext;

		private IEntityList _instance;

		[NonSerialized]
		private IEntityInstanceGetter _instanceGetter;

		private IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Armazena a relação dos indices da coleção.
		/// </summary>
		private Dictionary<string, Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>>> _indexes;

		private bool _disableThreadSafe = false;

		/// <summary>
		/// Identifica se a entidade possui Uid.
		/// </summary>
		private static bool? _hasUid;

		private System.ComponentModel.PropertyDescriptor _sortProperty;

		private System.ComponentModel.ListSortDirection _sortDirection;

		/// <summary>
		/// Evento chamado ao terminar atualizações.
		/// </summary>
		public event EventHandler AcceptedChanges;

		/// <summary>
		/// Evento acionado quando a lista está sendo alterada.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event NotifyCollectionChangingEventHandler<TEntity> CollectionChanging;

		/// <summary>
		/// Evento acionado quando a entidade sofre alguma alteração.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Evento acionado quando a entidade for carregada.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Evento acionado quando a coleção for alterada.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add
			{
				((Collections.INotifyCollectionChangedDispatcher)this).AddCollectionChanged(value, Collections.NotifyCollectionChangedDispatcherPriority.Normal);
			}
			remove {
				((Collections.INotifyCollectionChangedDispatcher)this).RemoveCollectionChanged(value);
			}
		}

		/// <summary>
		/// Evento acionado quando uma propriedade estiver sendo alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// Evento acionado quando uma propriedade for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Evento acionado quando a propriedade de um dos itens de coleção
		/// for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler ItemPropertyChanged;

		/// <summary>
		/// Evento acionado quando a entidade for salva.
		/// </summary>
		public event EventHandler<EntitySavedEventArgs> Saved;

		/// <summary>
		/// Evento acionado quando a entidade estiver sendo salva.
		/// </summary>
		public event EventHandler<EntitySavingEventArgs> Saving;

		/// <summary>
		/// Evento acionado quando a entidade estiver sendo apagada.
		/// </summary>
		public event EventHandler<EntityDeletingEventArgs> Deleting;

		/// <summary>
		/// Evento acionado quando a entidade for apagada.
		/// </summary>
		public event EventHandler<EntityDeletedEventArgs> Deleted;

		/// <summary>
		/// Nome que representa o tipo da entidade
		/// </summary>
		public IMessageFormattable EntityTypeName
		{
			get
			{
				return typeof(TEntity).Name.GetFormatter();
			}
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
		/// Identifica se a entidade foi carregada.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
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
		/// Identifica se a instancia original foi inicializada.
		/// </summary>
		bool IEntity.IsInstanceInicialized
		{
			get
			{
				return _instance != null;
			}
		}

		/// <summary>
		/// Instancia original da entidade.
		/// </summary>
		IEntity IEntity.Instance
		{
			get
			{
				return Instance;
			}
		}

		/// <summary>
		/// Instancia do recuperador do Instance para o item da coleção.
		/// </summary>
		protected IEntityInstanceGetter CreateItemInstanceGetter(IEntity item)
		{
			if(item != null)
				return new ItemEntityInstanceGetter(this, item);
			return null;
		}

		/// <summary>
		/// Instancia original da lista.
		/// </summary>
		internal protected IEntityList Instance
		{
			get
			{
				IEntityList instance = null;
				if(_instanceGetter != null)
					instance = _instanceGetter.GetInstance() as IEntityList;
				if(instance == null)
					instance = _instance;
				return instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Gerenciador dos tipos.
		/// </summary>
		public IEntityTypeManager TypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Verifica se a lista interna existe.
		/// </summary>
		protected virtual bool ExistsInnerList
		{
			get
			{
				return _innerList != null;
			}
		}

		/// <summary>
		/// Recupera a instancia da lista interna.
		/// </summary>
		protected Colosoft.Collections.BaseObservableCollection<TEntity> InnerList
		{
			get
			{
				if(_innerList == null)
				{
					_innerList = _loadItems != null ? new Colosoft.Collections.BaseObservableCollection<TEntity>(_loadItems.Value) : new Colosoft.Collections.BaseObservableCollection<TEntity>();
					((Colosoft.Collections.IThreadSafeObservableCollection)_innerList).DisableThreadSafe();
					foreach (var i in _innerList)
						RegisterItem(i);
					_loadItems = null;
					_innerList.CollectionChanged += OnInnerListCollectionChanged;
				}
				return _innerList;
			}
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TEntity this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		/// <summary>
		/// Contexto visual associado.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
			protected set
			{
				_uiContext = value;
			}
		}

		/// <summary>
		/// Identifica se a entidade está sendo inicializada.
		/// </summary>
		public bool IsInitializing
		{
			get
			{
				return _lockMonitor.Busy;
			}
		}

		/// <summary>
		/// Quantidade de itens na lista.
		/// </summary>
		public virtual int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

		/// <summary>
		/// Identificador unico da entidade.
		/// </summary>
		public int Uid
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Nome unico da entidade.
		/// </summary>
		public string FindName
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Identifica que a instancia não possui o identificador unico.
		/// </summary>
		public bool HasUid
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica que a instancia não possui o nome unico.
		/// </summary>
		public bool HasFindName
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Instancia da lista associada.
		/// </summary>
		public virtual Colosoft.Collections.IObservableCollection MyList
		{
			get
			{
				return _myList;
			}
		}

		/// <summary>
		/// Versão da instância.
		/// </summary>
		public virtual long RowVersion
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		public virtual Type ModelType
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Entidade dona da lista.
		/// </summary>
		public IEntity Owner
		{
			get
			{
				if(_owner == null && MyList is IEntity)
					return ((IEntity)MyList).Owner;
				return _owner;
			}
			set
			{
				if(_owner != value)
				{
					if(_owner != null)
						_owner.PropertyChanged -= OwnerPropertyChanged;
					_owner = value;
					_isOwnerDefined = true;
					if(_owner != null)
						_owner.PropertyChanged += OwnerPropertyChanged;
					if(_sourceContext == null && value is IConnectedEntity)
						_sourceContext = ((IConnectedEntity)value).SourceContext;
					OnOwnerChanged(value);
					RaisePropertyChanged("Owner");
				}
			}
		}

		/// <summary>
		/// Instância a partir da qual q instância atual foi clonada.
		/// </summary>
		public IEntity CloneFrom
		{
			get;
			protected set;
		}

		/// <summary>
		/// Identifica se o tipo da entidade da lista possui Uid.
		/// </summary>
		protected virtual bool EntityTypeOfListHasUid
		{
			get
			{
				if(_hasUid == null)
					_hasUid = this._entityTypeManager.HasUid(typeof(TEntity));
				return _hasUid.GetValueOrDefault();
			}
		}

		/// <summary>
		/// Instancia usada para carregar o itens de forma tardia.
		/// </summary>
		protected Lazy<IEnumerable<TEntity>> LoadItems
		{
			get
			{
				return _loadItems;
			}
			set
			{
				_loadItems = value;
			}
		}

		/// <summary>
		/// Action usada para define o identificador do pai para o item da lista.
		/// </summary>
		protected virtual Action<TEntity> ParentUidSetter
		{
			get
			{
				return _parentUidSetter;
			}
		}

		/// <summary>
		/// Identifica se a instancia já existe na fonte de armazenamento.
		/// </summary>
		public bool ExistsInStorage
		{
			get
			{
				return this.Owner != null ? this.Owner.ExistsInStorage : false;
			}
		}

		/// <summary>
		/// Identifica se a lista está em estado de carga tardia.
		/// </summary>
		public bool IsLazyLoadState
		{
			get
			{
				return LoadItems != null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter">Método usado para definir o identificador unico do pai para as entidades filas da lista.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		public BaseEntityList(string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null)
		{
			_lockMonitor = new EntityMonitor(this);
			_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, Colosoft.Collections.CollectionChangedWrapperEventData>();
			_listChangedHandlers = new Dictionary<System.ComponentModel.ListChangedEventHandler, Collections.ListChangedWrapperEventData>();
			_uiContext = uiContext;
			_parentUidSetter = parentUidSetter;
			if(entityTypeManager == null)
				entityTypeManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEntityTypeManager>();
			_entityTypeManager = entityTypeManager;
		}

		/// <summary>
		/// Cria uma instancia já definindos os itens iniciais.
		/// </summary>
		/// <param name="items">Itens que serão usados na inicialização.</param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		public BaseEntityList(IEnumerable<TEntity> items, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null) : this(uiContext, parentUidSetter, entityTypeManager)
		{
			if(items != null)
			{
				_innerList = new Colosoft.Collections.BaseObservableCollection<TEntity>();
				try
				{
					foreach (var i in items)
					{
						RegisterItem(i);
						_innerList.Add(i);
					}
				}
				catch(NullReferenceException)
				{
					throw;
				}
				if(items is Collections.INotifyCollectionChangedObserverRegister)
					((Collections.INotifyCollectionChangedObserverRegister)items).Register(this);
				_innerList.CollectionChanged += OnInnerListCollectionChanged;
			}
		}

		/// <summary>
		/// Cria uma instancia definindo o parametro de carga tardia dos itens.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		public BaseEntityList(Lazy<IEnumerable<TEntity>> items, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null) : this(uiContext, parentUidSetter, entityTypeManager)
		{
			_loadItems = items;
		}

		/// <summary>
		/// Cria uma instancia a partir dos dados clonados.
		/// </summary>
		/// <param name="itemsLoader"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		protected BaseEntityList(Func<BaseEntityList<TEntity>, IEnumerable<TEntity>> itemsLoader, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager) : this(uiContext, parentUidSetter, entityTypeManager)
		{
			var items = itemsLoader(this);
			if(items != null)
			{
				_innerList = new Colosoft.Collections.BaseObservableCollection<TEntity>();
				foreach (var i in items)
				{
					RegisterItem(i);
					_innerList.Add(i);
				}
				if(items is Collections.INotifyCollectionChangedObserverRegister)
					((Collections.INotifyCollectionChangedObserverRegister)items).Register(this);
				_innerList.CollectionChanged += OnInnerListCollectionChanged;
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~BaseEntityList()
		{
			Dispose(false);
		}

		/// <summary>
		/// Verifica se um item é unico na lista.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool CheckIfItemIsUnique(TEntity item)
		{
			if(item is Colosoft.Collections.IUniqueItemInList<TEntity>)
			{
				ItemComparer<TEntity> comparer = new ItemComparer<TEntity>();
				if(InnerList.Contains(item, comparer))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Verifica se o valor informado é compatível com a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal static bool IsCompatibleObject(object value)
		{
			if(!(value is TEntity) && ((value != null) || typeof(TEntity).IsValueType))
				return false;
			return true;
		}

		/// <summary>
		/// Registra o item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		private void RegisterItem(TEntity item)
		{
			item.PropertyChanged += OnItemPropertyChanged;
			item.AcceptedChanges += OnItemAcceptedChanges;
			if(item is IEntityPersistence)
				((IEntityPersistence)item).Deleted += OnItemDeleted;
			if(item is IEntityInstanceRegister)
				((IEntityInstanceRegister)item).Register(CreateItemInstanceGetter(item));
			item.InitList(this);
		}

		/// <summary>
		/// Remove o registro do item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="cleanMyList">Identifica se é para limpar o mylist.</param>
		private void UnregisterItem(TEntity item, bool cleanMyList)
		{
			item.PropertyChanged -= OnItemPropertyChanged;
			item.AcceptedChanges -= OnItemAcceptedChanges;
			if(_removedItems == null || !_removedItems.Contains(item))
				if(item is IEntityPersistence)
					((IEntityPersistence)item).Deleted -= OnItemDeleted;
			if(cleanMyList && item.MyList == this)
			{
				if(item is IEntityInstanceRegister)
				{
					var register = (IEntityInstanceRegister)item;
					if(register.InstanceGetter is ItemEntityInstanceGetter && ((ItemEntityInstanceGetter)register.InstanceGetter).Item.Equals(item))
						register.Register(null);
				}
				item.InitList(null);
			}
		}

		/// <summary>
		/// Registra o item removido.
		/// </summary>
		/// <param name="i">Instancia do item que deve ser registrado como removido.</param>
		private void RegisterRemovedItem(TEntity i)
		{
			if(_removedItems == null)
				_removedItems = new List<TEntity>();
			if(i is IEntityPersistence)
				((IEntityPersistence)i).Deleted -= OnItemDeleted;
			_removedItems.Add(i);
		}

		/// <summary>
		/// Remove o registro do item removido.
		/// </summary>
		/// <param name="i">Instancia do item registrado.</param>
		private void UnregisterRemovedItem(TEntity i)
		{
			if(i is IEntityPersistence)
				((IEntityPersistence)i).Deleted -= OnItemDeleted;
		}

		/// <summary>
		/// Remove o registro da lista pai.
		/// </summary>
		/// <param name="ownerList"></param>
		private void UnregisterList(Colosoft.Collections.IObservableCollection ownerList)
		{
			if(_myList != null)
			{
				_myList.CollectionChanged -= MyListCollectionChanged;
				_myList.PropertyChanged -= MyListPropertyChanged;
				_myList = null;
				RaisePropertyChanged("MyList", "Owner");
			}
		}

		/// <summary>
		/// Método acionado quanod o coleção associada for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
				if(e.OldItems != null)
					foreach (object item in e.OldItems)
						if(object.ReferenceEquals(item, this))
							UnregisterList((Collections.IObservableCollection)sender);
		}

		/// <summary>
		/// Método acionado quando uma propriedade da lista for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Owner")
				RaisePropertyChanged("Owner");
		}

		/// <summary>
		/// Método acionado quando a sessão de persistencia que está sendo usada para salvar a entidade
		/// tenha sido executada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal protected void SavePersistenceSessionExecutedCallback(object sender, Colosoft.Data.PersistenceSessionExecutedEventArgs e)
		{
			var session = (Colosoft.Data.IPersistenceSession)sender;
			session.Executed -= SavePersistenceSessionExecutedCallback;
			if(e.Result.Status == Data.ExecuteActionsResultStatus.Success)
			{
				this.AcceptChangesInternal();
				OnSaved(true, null);
			}
			else
				OnSaved(false, e.Result.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Método acionado quando a sessão de persistencia que está sendo usada para apagar a entidade
		/// tenha sido executada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeletePersistenceSessionExecutedCallback(object sender, Colosoft.Data.PersistenceSessionExecutedEventArgs e)
		{
			var session = (Colosoft.Data.IPersistenceSession)sender;
			session.Executed -= DeletePersistenceSessionExecutedCallback;
			if(e.Result.Status == Data.ExecuteActionsResultStatus.Success)
			{
				this.AcceptChangesInternal();
				OnDeleted(true, null);
			}
			else
				OnDeleted(false, e.Result.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Evento de coleção sendo alterada.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		protected virtual bool OnCollectionChanging(NotifyCollectionChangingEventArgs<TEntity> args)
		{
			if(CollectionChanging != null)
				return CollectionChanging(this, args);
			else
				return true;
		}

		/// <summary>
		/// Recupera o item pelo indice informado.
		/// </summary>
		/// <param name="index">Indice do item que será recuperado.</param>
		/// <returns></returns>
		protected virtual TEntity GetItem(int index)
		{
			return InnerList[index];
		}

		/// <summary>
		/// Define o item para o indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected virtual void SetItem(int index, TEntity item)
		{
			List<TEntity> newItems = new List<TEntity>();
			List<TEntity> oldItems = new List<TEntity>();
			NotifyCollectionChangingAction action;
			newItems.Add(item);
			var oldItem = InnerList[index];
			if(InnerList[index] != null)
			{
				oldItems.Add(InnerList[index]);
				action = NotifyCollectionChangingAction.Replace;
			}
			else
				action = NotifyCollectionChangingAction.Add;
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = action,
				NewItems = newItems,
				OldItems = oldItems
			};
			if(OnCollectionChanging(args))
			{
				FixItem(item);
				InnerList[index] = item;
				if(oldItem != null && !oldItem.Equals(item))
				{
					RegisterRemovedItem(oldItem);
				}
			}
		}

		/// <summary>
		/// Método acionado quando a entidade for carregada.
		/// </summary>
		protected virtual void OnLoaded()
		{
			if(!_isLoaded)
			{
				_isLoaded = true;
				RaisePropertyChanged("IsLoaded");
			}
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado quando o novo propriedade da coleção for alterado.
		/// </summary>
		/// <param name="newOwner"></param>
		protected virtual void OnOwnerChanged(IEntity newOwner)
		{
		}

		/// <summary>
		/// Método acioando quando alguma propriedade do pai for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OwnerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsLocked" || e.PropertyName == "IsLockedToMe" || e.PropertyName == "IsLockedToEdit" || e.PropertyName == "CanEdit" || e.PropertyName == "IsReadOnly")
				RaisePropertyChanged(e.PropertyName);
		}

		/// <summary>
		/// Método acionado quando a entidade estiver sendo salva.
		/// </summary>
		/// <returns>Retorna false é a operação for cancelada.</returns>
		protected virtual SaveResult OnSaving()
		{
			var args = new EntitySavingEventArgs(this);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntitySavingEvent>().Publish(args);
			if(Saving != null)
				Saving(this, args);
			return new SaveResult(!args.Cancel, args.Message);
		}

		/// <summary>
		/// Método acionado quando a entidade for salva.
		/// </summary>
		/// <param name="success">Identifica se a entidade foi salva com sucesso.</param>
		/// <param name="message">Mensagem associada.</param>
		protected virtual void OnSaved(bool success, IMessageFormattable message)
		{
			var args = new EntitySavedEventArgs(this, success, message);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntitySavedEvent>().Publish(args);
			if(Saved != null)
				Saved(this, args);
		}

		/// <summary>
		/// Método acionado quando a entidade estiver sendo apagada.
		/// </summary>
		/// <returns></returns>
		protected virtual DeleteResult OnDeleting()
		{
			var args = new EntityDeletingEventArgs(this);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityDeletingEvent>().Publish(args);
			if(Deleting != null)
				Deleting(this, args);
			return new DeleteResult(!args.Cancel, args.Message);
		}

		/// <summary>
		/// Método acionado quando a instancia for apagada.
		/// </summary>
		/// <param name="success">Identifica se a entidade foi apagada com sucesso.</param>
		/// <param name="message">Mensagem associada.</param>
		protected virtual void OnDeleted(bool success, IMessageFormattable message)
		{
			var args = new EntityDeletedEventArgs(this, success, message);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityDeletedEvent>().Publish(args);
			if(Deleted != null)
				Deleted(this, args);
		}

		/// <summary>
		/// Método usado para clonar um item
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual TEntity CloneItem(Collections.IObservableCollection parent, TEntity item)
		{
			TEntity result = (TEntity)item.Clone();
			if(result is IEntityInstanceRegister)
				((IEntityInstanceRegister)result).Register(CreateItemInstanceGetter(result));
			result.InitList(parent);
			result.Owner = ((IEntity)parent).Owner;
			return result;
		}

		/// <summary>
		/// Instancia do contexto da origem dos dados.
		/// </summary>
		protected virtual Query.ISourceContext SourceContext
		{
			get
			{
				return _sourceContext == null && (Owner is IConnectedEntity) ? ((IConnectedEntity)Owner).SourceContext : _sourceContext;
			}
		}

		/// <summary>
		/// Evento chamado ao aceitar atualizações.
		/// </summary>
		protected virtual void OnAcceptChanges()
		{
			if(AcceptedChanges != null)
				AcceptedChanges(this, EventArgs.Empty);
		}

		/// <summary>
		/// Evento acionado quando a entidade sofre alteração.
		/// </summary>
		protected virtual void OnChanged()
		{
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado toda vez que a propriedade de um item da lista for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsChanged" || e.PropertyName == "IsLockedToEdit" || e.PropertyName == "IsLockedToMe" || e.PropertyName == "IsLocked" || e.PropertyName == "CanEdit" || e.PropertyName == "IsReadOnly")
				RaisePropertyChanged(e.PropertyName);
			if(ItemPropertyChanged != null)
				ItemPropertyChanged(sender, e);
		}

		/// <summary>
		/// Método acionado quando as alterações de um item da coleção
		/// for aceita.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnItemAcceptedChanges(object sender, EventArgs e)
		{
			var entity = (TEntity)sender;
			if(_newItems != null && _newItems.Remove(entity))
				return;
			if(_removedItems != null && _removedItems.Remove(entity))
			{
				UnregisterRemovedItem(entity);
				return;
			}
		}

		/// <summary>
		/// Método acionado quando o item for apagado do banco de dados.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnItemDeleted(object sender, EntityDeletedEventArgs e)
		{
			var entity = (TEntity)sender;
			if(_newItems != null && _newItems.Remove(entity))
				return;
			if(_removedItems != null && _removedItems.Remove(entity))
			{
				UnregisterRemovedItem(entity);
				return;
			}
		}

		/// <summary>
		/// Dispara ou registra as alterações ocorridas para a propriedade
		/// </summary>
		/// <param name="propertyNames">Nome das propriedades</param>
		protected virtual void RaisePropertyChanged(params string[] propertyNames)
		{
			if(PropertyChanged != null && !IsDisposed)
				foreach (var i in propertyNames)
				{
					if(i == "IsChanged")
						OnChanged();
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(i));
				}
		}

		/// <summary>
		/// Dispara eventos ocorridas para o inicio de alteração da propriedade
		/// </summary>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="newValue">Valor da prorpiedade</param>
		protected virtual bool RaisePropertyChanging(string propertyName, object newValue)
		{
			if(PropertyChanging != null)
				PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			return true;
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
					foreach (KeyValuePair<NotifyCollectionChangedEventHandler, Colosoft.Collections.CollectionChangedWrapperEventData> kvp in handlers)
						kvp.Value.Invoke(this, e);
				}
			}
			else
			{
				if(_collectionChanged != null)
					_collectionChanged(this, e);
			}
		}

		/// <summary>
		/// Método acionado quando a lista for alterada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
		{
			if(this.IsThreadSafe)
			{
				if(_listChangedHandlers.Count > 0)
				{
					var handlers = _listChangedHandlers.ToArray();
					foreach (KeyValuePair<System.ComponentModel.ListChangedEventHandler, Colosoft.Collections.ListChangedWrapperEventData> kvp in handlers)
						kvp.Value.Invoke(this, e);
				}
			}
			else
			{
				if(_listChanged != null)
					_listChanged(this, e);
			}
		}

		/// <summary>
		/// Verifica se o item existe na lista.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual void CheckExists(TEntity item)
		{
			item.Require("item").NotNull();
			if(Contains(item, EntityEqualityComparer<TEntity>.Instance))
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.BaseEntityList_EntityInstanceExists, item.GetType().Name, item.HasUid ? item.Uid.ToString() : item.FindName, (Owner != null ? string.Format("{0} :: {1}", Owner.GetType().Name, (Owner.HasUid ? Owner.Uid.ToString() : Owner.FindName)) : ResourceMessageFormatter.Create(() => Properties.Resources.BaseEntityList_WithoutOwner).Format())).Format());
		}

		/// <summary>
		/// Processa o identificador unico do item.
		/// </summary>
		/// <param name="item"></param>
		protected virtual void ProcessNewItem(TEntity item)
		{
			CheckExists(item);
			FixItem(item);
		}

		/// <summary>
		/// Fixa o item na coleção.
		/// </summary>
		/// <param name="item"></param>
		protected virtual void FixItem(TEntity item)
		{
			if(EntityTypeOfListHasUid && item.Uid == 0)
				item.Uid = _entityTypeManager.GenerateInstanceUid(typeof(TEntity));
			if(_parentUidSetter != null)
				_parentUidSetter(item);
			if(item is IEntityInstanceRegister)
				((IEntityInstanceRegister)item).Register(CreateItemInstanceGetter(item));
			item.InitList(this);
		}

		/// <summary>
		/// Converte a ação de alteração de uma coleção para o tipo de alteração de lista.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected static System.ComponentModel.ListChangedType Convert(System.Collections.Specialized.NotifyCollectionChangedAction action)
		{
			switch(action)
			{
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				return System.ComponentModel.ListChangedType.ItemAdded;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
				return System.ComponentModel.ListChangedType.ItemMoved;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				return System.ComponentModel.ListChangedType.ItemDeleted;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				return System.ComponentModel.ListChangedType.ItemChanged;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				return System.ComponentModel.ListChangedType.Reset;
			default:
				return System.ComponentModel.ListChangedType.Reset;
			}
		}

		/// <summary>
		/// Converte os argumentos.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		protected static System.ComponentModel.ListChangedEventArgs Convert(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var changeType = Convert(e.Action);
			switch(changeType)
			{
			case System.ComponentModel.ListChangedType.ItemAdded:
				return new System.ComponentModel.ListChangedEventArgs(changeType, e.NewStartingIndex);
			case System.ComponentModel.ListChangedType.ItemChanged:
				return new System.ComponentModel.ListChangedEventArgs(changeType, e.NewStartingIndex);
			case System.ComponentModel.ListChangedType.ItemDeleted:
				return new System.ComponentModel.ListChangedEventArgs(changeType, e.OldStartingIndex);
			case System.ComponentModel.ListChangedType.ItemMoved:
				return new System.ComponentModel.ListChangedEventArgs(changeType, e.NewStartingIndex, e.OldStartingIndex);
			default:
				return new System.ComponentModel.ListChangedEventArgs(changeType, -1);
			}
		}

		/// <summary>
		/// Método acioando quando a coleção interna sofrer alguam alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnInnerListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (TEntity i in e.OldItems)
				{
					UnregisterItem(i, false);
					if(i != null)
					{
						if(i.ExistsInStorage)
							RegisterRemovedItem(i);
						if(_newItems != null)
						{
							for(var j = 0; j < _newItems.Count; j++)
							{
								if(Entity.Equals(_newItems[j], i))
								{
									_newItems.RemoveAt(j--);
									break;
								}
							}
						}
					}
				}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if(_newItems == null)
					_newItems = new List<TEntity>();
				foreach (TEntity i in e.NewItems)
				{
					RegisterItem(i);
					_newItems.Add(i);
					if(_removedItems != null)
					{
						for(var j = 0; j < _removedItems.Count; j++)
						{
							if(Entity.Equals(_removedItems[j], i))
							{
								UnregisterRemovedItem(_removedItems[j]);
								_removedItems.RemoveAt(j--);
								break;
							}
						}
					}
				}
			}
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				foreach (TEntity i in e.OldItems)
					UnregisterItem(i, true);
				foreach (TEntity i in e.NewItems)
					RegisterItem(i);
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
				foreach (TEntity i in e.OldItems)
				{
					UnregisterItem(i, true);
				}
			RaisePropertyChanged("Count", "IsChanged");
			OnCollectionChanged(e);
			OnListChanged(Convert(e));
		}

		/// <summary>
		/// Cria um <see cref="IEntityDescriptor"/> da entidade.
		/// </summary>
		/// <returns></returns>
		public IEntityDescriptor CreateDescriptor()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Compara a instancia com a chave do registro informada.
		/// </summary>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public bool Equals(Colosoft.Query.RecordKey recordKey)
		{
			return Equals(recordKey, Query.RecordKeyComparisonType.Key);
		}

		/// <summary>
		/// Verifica se a instancia possui dados iguais a chave
		/// de registro informada.
		/// </summary>
		/// <param name="recordKey">Instancia da chave que será comparada.</param>
		/// <param name="comparisonType">Tipo de comparação que será realizada.</param>
		/// <returns></returns>
		public bool Equals(Colosoft.Query.RecordKey recordKey, Colosoft.Query.RecordKeyComparisonType comparisonType)
		{
			return false;
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex"></param>
		/// <param name="newIndex"></param>
		public virtual void Move(int oldIndex, int newIndex)
		{
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Move,
				StartOldItems = oldIndex,
				StartNewItems = newIndex
			};
			if(OnCollectionChanging(args))
			{
				this.InnerList.Move(oldIndex, newIndex);
			}
		}

		/// <summary>
		/// Inicializa a lista do objeto.
		/// </summary>
		/// <param name="ownerList">Lista que contém a entidade.</param>
		public void InitList(Colosoft.Collections.IObservableCollection ownerList)
		{
			if(_myList != null)
			{
				_myList.CollectionChanged -= MyListCollectionChanged;
				_myList.PropertyChanged -= MyListPropertyChanged;
			}
			_myList = ownerList;
			if(ownerList != null && _owner != null)
			{
				_owner.PropertyChanged -= OwnerPropertyChanged;
				_owner = null;
				_isOwnerDefined = true;
			}
			if(_myList != null)
			{
				_myList.PropertyChanged += MyListPropertyChanged;
				_myList.CollectionChanged += MyListCollectionChanged;
				if(_sourceContext == null && ownerList is IConnectedEntity)
					_sourceContext = ((IConnectedEntity)_myList).SourceContext;
			}
			RaisePropertyChanged("MyList", "Owner");
		}

		/// <summary>
		/// Remove o controle de armazenamento da entidade.
		/// </summary>
		public virtual void RemoveStorageControl()
		{
		}

		/// <summary>
		/// Remove o control de armazenamento da entidade e de seus filhos.
		/// </summary>
		public virtual void RemoveAllStorageControl()
		{
			foreach (IEntity item in this)
				item.RemoveAllStorageControl();
		}

		/// <summary>
		/// Reseta a coleção.
		/// </summary>
		public void Reset()
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Desabilita o thread safe.
		/// </summary>
		void Colosoft.Collections.IThreadSafeObservableCollection.DisableThreadSafe()
		{
			_disableThreadSafe = true;
		}

		/// <summary>
		/// Habilita o thread safe.
		/// </summary>
		void Colosoft.Collections.IThreadSafeObservableCollection.EnableThreadSafe()
		{
			_disableThreadSafe = false;
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IEntity IEntityList.this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, (TEntity)value);
			}
		}

		/// <summary>
		/// Recupera o lock de notificações.
		/// </summary>
		/// <returns></returns>
		public IDisposable GetNotificationsLock()
		{
			return _lockMonitor;
		}

		/// <summary>
		/// Copia os dados da entidade informada para a instancia,
		/// inclusives os dados de alteração da nova instancia..
		/// </summary>
		/// <param name="from">Instancia com os dados que serão copiados.</param>
		public virtual void CopyFrom(IEntityList from)
		{
			var fromList = from as BaseEntityList<TEntity>;
			if(fromList == null)
				throw new InvalidOperationException(string.Format("from not is {0}.", this.GetType().FullName));
			UIContext = fromList.UIContext;
			if(ExistsInnerList)
			{
				if(fromList._removedItems != null)
				{
					foreach (var item in fromList._removedItems)
					{
						for(int i = 0; i < InnerList.Count; i++)
						{
							var destItem = InnerList[i];
							if(destItem != null && destItem.Equals(item))
								InnerList.RemoveAt(i--);
						}
					}
				}
			}
			LoadItems = null;
			if(fromList.ExistsInnerList)
			{
				var processedItems = new List<TEntity>();
				for(var i = 0; i < fromList.InnerList.Count; i++)
				{
					var fromItem = fromList.InnerList[i];
					if(InnerList.Count >= (i + 1))
					{
						var destItem = InnerList[i];
						if(destItem.Equals(fromItem))
						{
							destItem.CopyFrom(fromItem);
							processedItems.Add(destItem);
							continue;
						}
					}
					var found = false;
					int j = 0;
					for(j = 0; j < InnerList.Count; j++)
					{
						if(InnerList[j].Equals(fromItem))
						{
							found = true;
							break;
						}
					}
					if(found)
					{
						InnerList[j].CopyFrom(fromItem);
						processedItems.Add(InnerList[j]);
					}
					else
					{
						var newItem = (TEntity)fromItem.Clone();
						InnerList.Insert(i, newItem);
						fromItem.RegisterCloneToEdit(newItem);
						processedItems.Add(newItem);
					}
				}
				for(var i = 0; i < InnerList.Count; i++)
				{
					if(!processedItems.Contains(InnerList[i]))
						InnerList.RemoveAt(i--);
				}
			}
			else if(fromList.LoadItems != null)
				LoadItems = fromList.LoadItems;
		}

		/// <summary>
		/// Ignora o item removido da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool IEntityList.IgnoreRemovedItem(IEntity item)
		{
			if(_removedItems != null && item is TEntity)
				return _removedItems.Remove((TEntity)item);
			return false;
		}

		/// <summary>
		/// Ignora o item removido da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool IgnoreRemovedItem(TEntity item)
		{
			return (_removedItems != null) && _removedItems.Remove(item);
		}

		/// <summary>
		/// Identifica se a entidade está sendo editada.
		/// </summary>
		public bool IsEditing
		{
			get
			{
				return _isEditing;
			}
			protected set
			{
				if(_isEditing != value)
				{
					_isEditing = value;
					RaisePropertyChanged("IsEditing");
				}
			}
		}

		/// <summary>
		/// Loader da instancia.
		/// </summary>
		IEntityLoader IEntity.Loader
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Não da suporte.
		/// </summary>
		ObserverControl IEntity.Observer
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Libera.
		/// </summary>
		void IEntity.Flush()
		{
		}

		/// <summary>
		/// Reseta os identificadores da instancia.
		/// </summary>
		void IEntity.ResetAllUids()
		{
			this.CloneFrom = null;
			if(_newItems != null)
				_newItems.Clear();
			if(_removedItems != null)
			{
				while (_removedItems.Count > 0)
				{
					UnregisterRemovedItem(_removedItems[0]);
					_removedItems.RemoveAt(0);
				}
			}
			_newItems = new List<TEntity>(this);
			_removedItems = null;
			_instance = null;
			foreach (IEntity item in this)
				item.ResetAllUids();
		}

		/// <summary>
		/// Recupera a instancia original.
		/// </summary>
		/// <returns></returns>
		IEntity IEntity.GetOriginal()
		{
			return _instance;
		}

		/// <summary>
		/// Copia os dados da entidade informada para a instancia,
		/// inclusives os dados de alteração da nova instancia..
		/// </summary>
		/// <param name="fromEntity">Instancia com os dados que serão copiados.</param>
		void IEntity.CopyFrom(IEntity fromEntity)
		{
			var fromList = fromEntity as BaseEntityList<TEntity>;
			if(fromList == null)
				throw new InvalidOperationException(string.Format("from not is {0}.", this.GetType().FullName));
			CopyFrom(fromList);
		}

		/// <summary>
		/// Valida os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual Validation.ValidationResult Validate()
		{
			var result = new Validation.ValidationResult();
			Validate(ref result);
			return result;
		}

		/// <summary>
		/// Valida os dados da intancia.
		/// </summary>
		/// <param name="validationResult">Resultado da validação.</param>
		public virtual void Validate(ref Validation.ValidationResult validationResult)
		{
			if(validationResult == null)
				validationResult = new Validation.ValidationResult();
			foreach (var i in this)
				if(i.IsChanged)
					i.Validate(ref validationResult);
		}

		/// <summary>
		/// Valida as propriedades informadas.
		/// </summary>
		/// <param name="propertyNames">Nomes das propriedades que serão validadas.</param>
		/// <returns></returns>
		Validation.ValidationResult IEntity.Validate(params string[] propertyNames)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Bloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <param name="token">token que irá bloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult Lock(string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Bloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <returns></returns>
		public virtual Lock.LockProcessResult Lock()
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Realiza o lock da instancia.
		/// </summary>
		/// <param name="session">Instancia da sessão que deverá ser utilizada para o lock.</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult Lock(Lock.LockSession session)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Veifica se a instância está ou não locada.
		/// </summary>
		/// <returns></returns>
		public bool IsLocked
		{
			get
			{
				return Owner != null && Owner.IsLocked;
			}
		}

		/// <summary>
		/// Veifica se a instância está ou não locada.
		/// </summary>
		/// <returns></returns>
		public bool IsLockedToMe
		{
			get
			{
				return Owner != null && Owner.IsLockedToMe;
			}
		}

		/// <summary>
		/// Indica que a instância está locada para edição.
		/// </summary>
		public virtual bool IsLockedToEdit
		{
			get
			{
				return Owner != null && Owner.IsLockedToEdit;
			}
			set
			{
			}
		}

		/// <summary>
		/// Determina se a instancia da entidade pode ser editada.
		/// </summary>
		public bool CanEdit
		{
			get
			{
				return !IsReadOnly && IsLockedToEdit;
			}
		}

		/// <summary>
		/// Desbloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Desbloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock()
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Desbloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <param name="groupSession"></param>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(string groupSession, string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Desbloqueia a inatância(não aplicável a lista).
		/// </summary>
		/// <param name="groupSession"></param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(string groupSession)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		IMessageFormattable INamedType.TypeName
		{
			get
			{
				return EntityTypeName;
			}
		}

		/// <summary>
		/// Identificador da instância associada.
		/// </summary>
		IMessageFormattable INamedType.InstanceDescriptor
		{
			get
			{
				try
				{
					if(string.IsNullOrEmpty(FindName) || this.GetType().Name.Equals(FindName))
					{
						if(this.Owner != null)
							string.Format("{0} : {1} ", EntityTypeName.Format(), this.Owner.InstanceDescriptor);
						else
							return EntityTypeName;
					}
					return string.Format("{0} {1}", EntityTypeName.Format(), FindName).GetFormatter();
				}
				catch
				{
					return FindName.GetFormatter();
				}
			}
		}

		/// <summary>
		/// Identifica se a coleção possui alterações.
		/// </summary>
		public virtual bool IsChanged
		{
			get
			{
				if((_newItems != null && _newItems.Count > 0) || (_removedItems != null && _removedItems.Count > 0))
					return true;
				if(!IsLazyLoadState)
					foreach (var i in this)
						if(i.IsChanged)
							return true;
				return false;
			}
		}

		/// <summary>
		/// Confirma as alterações da instancia.
		/// </summary>
		void System.ComponentModel.IChangeTracking.AcceptChanges()
		{
			AcceptChangesInternal();
		}

		/// <summary>
		/// Confirma as alterações da instancia.
		/// </summary>
		protected virtual void AcceptChangesInternal()
		{
			if(_newItems != null)
				_newItems.Clear();
			if(_removedItems != null)
			{
				while (_removedItems.Count > 0)
				{
					UnregisterRemovedItem(_removedItems[0]);
					_removedItems.RemoveAt(0);
				}
			}
			_newItems = null;
			_removedItems = null;
			if(!IsLazyLoadState)
				foreach (var i in this)
					i.AcceptChanges();
			OnAcceptChanges();
		}

		/// <summary>
		/// Reseta o estado da instancia rejeitando as modificações.
		/// </summary>
		void System.ComponentModel.IRevertibleChangeTracking.RejectChanges()
		{
			this.CopyFrom(_instance);
		}

		/// <summary>
		/// Limpa as alterações da instancia.
		/// </summary>
		void IClearableChangedTracking.ClearChanges()
		{
		}

		/// <summary>
		/// Ignora as alterações das propriedades informadas.
		/// </summary>
		/// <param name="propertyNames"></param>
		void IClearableChangedTracking.IgnoreChanges(params string[] propertyNames)
		{
		}

		/// <summary>
		/// Inicia a edição da entidade.
		/// </summary>
		public void BeginEdit()
		{
			if(!IsEditing)
			{
				_editingTransactionInstance = this.Clone() as IEntityList;
				IsEditing = true;
			}
		}

		/// <summary>
		/// Discarta as alterações desde o último System.ComponentModel.IEditableObject.BeginEdit()
		/// </summary>
		public void CancelEdit()
		{
			if(IsEditing)
			{
				this.CopyFrom(_editingTransactionInstance);
				IsEditing = false;
			}
		}

		/// <summary>
		/// Empurra as modificações desde o último System.ComponentModel.IEditableObject.BeginEdit()
		/// </summary>
		public void EndEdit()
		{
			if(IsEditing)
			{
				_editingTransactionInstance = null;
				IsEditing = false;
			}
		}

		/// <summary>
		/// Sinaliza para o objeto que a initialização está començando.
		/// </summary>
		public virtual void BeginInit()
		{
			_lockMonitor.Enter();
		}

		/// <summary>
		/// Sinaliza que a inicialização foi completada.
		/// </summary>
		public virtual void EndInit()
		{
			_lockMonitor.Dispose();
		}

		/// <summary>
		/// Identifica se pode disparar oe ventos de notificação de alteração.
		/// </summary>
		public virtual bool RaisesItemChangedEvents
		{
			get
			{
				return !_lockMonitor.Busy;
			}
		}

		/// <summary>
		/// Cria o manipulador para carga.
		/// </summary>
		/// <returns></returns>
		IDisposable ILoadable.CreateLoadHandle()
		{
			_lockMonitor.Enter();
			return _lockMonitor;
		}

		Validation.IStateble Validation.IStateControl.InstanceState
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public abstract object Clone();

		/// <summary>
		/// Clona os dados de controle para a instancia informada.
		/// </summary>
		/// <param name="to"></param>
		protected void CloneControls(BaseEntityList<TEntity> to)
		{
			if(this._newItems != null)
			{
				if(to._newItems == null)
					to._newItems = new List<TEntity>();
				to._newItems.AddRange(this._newItems.Select(f => CloneItem(to, f)));
			}
			if(this._removedItems != null)
			{
				foreach (var i in this._removedItems.Select(f => CloneItem(to, f)))
				{
					to.RegisterRemovedItem(i);
				}
			}
		}

		bool IEquatable<IEntity>.Equals(IEntity other)
		{
			return ReferenceEquals(this, other);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		TEntity IList<TEntity>.this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		int System.Collections.IList.Add(object value)
		{
			Add((TEntity)value);
			return 1;
		}

		bool System.Collections.IList.Contains(object value)
		{
			if(value is TEntity)
				return Contains((TEntity)value, EntityEqualityComparer<TEntity>.Instance);
			return false;
		}

		int System.Collections.IList.IndexOf(object value)
		{
			if(IsCompatibleObject(value))
				return this.IndexOf((TEntity)value);
			return -1;
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			Insert(index, (TEntity)value);
		}

		void System.Collections.IList.Remove(object value)
		{
			Remove((TEntity)value);
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, (TEntity)value);
			}
		}

		/// <summary>
		/// Instancia do objeto de sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				if(_syncRoot == null)
					System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				return _syncRoot;
			}
		}

		/// <summary>
		/// Identifica se é uma coleção sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			((System.Collections.ICollection)InnerList).CopyTo(array, index);
		}

		/// <summary>
		/// Identifica se a instância está o como de entrada ativo.
		/// </summary>
		bool Threading.IReentrancyController.IsReentrancy
		{
			get
			{
				return _innerList != null && _innerList is Threading.IReentrancyController && ((Threading.IReentrancyController)_innerList).IsReentrancy;
			}
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
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_isDisposed = true;
			if(_owner != null)
				_owner.PropertyChanged -= OwnerPropertyChanged;
			if(_innerList != null)
			{
				for(var i = 0; i < _innerList.Count; i++)
				{
					var entity = _innerList[i];
					if(entity != null)
						entity.Dispose();
				}
				_innerList.Dispose();
			}
			if(_collectionChangedHandlers != null)
			{
				foreach (var i in _collectionChangedHandlers)
					if(i.Value != null)
						i.Value.Dispose();
				_collectionChangedHandlers.Clear();
			}
			if(_listChangedHandlers != null)
				_listChangedHandlers.Clear();
			_loadItems = null;
			_sourceContext = null;
			_owner = null;
			_myList = null;
			_parentUidSetter = null;
			_listChanged = null;
			_collectionChanged = null;
			if(_instance != null)
				_instance.Dispose();
			_instance = null;
			if(_editingTransactionInstance != null)
				_editingTransactionInstance.Dispose();
			_editingTransactionInstance = null;
			if(_removedItems != null)
				_removedItems.Clear();
			if(_newItems != null)
				_newItems.Clear();
		}

		/// <summary>
		/// Recupera os novos items registrados na lista.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IEntity> GetNewItems()
		{
			return (IEnumerable<IEntity>)_newItems ?? new IEntity[0];
		}

		/// <summary>
		/// Recupera os itens removidos da lista.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IEntity> GetRemovedItems()
		{
			return (IEnumerable<IEntity>)_removedItems ?? new IEntity[0];
		}

		/// <summary>
		/// Recupera os itens que sofreram alterações.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IEntity> GetChangedItems()
		{
			return ExistsInnerList ? (IEnumerable<IEntity>)InnerList.Where(f => f.IsChanged) : new IEntity[0];
		}

		/// <summary>
		/// Recupera o enumerador dos itens da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator<TEntity> GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		/// <summary>
		/// Remove o item no indice informado.
		/// </summary>
		/// <param name="index"></param>
		public virtual void RemoveAt(int index)
		{
			List<TEntity> oldItems = new List<TEntity>();
			oldItems.Add(InnerList[index]);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Remove,
				OldItems = oldItems,
				NewItems = null,
				StartOldItems = index
			};
			if(OnCollectionChanging(args))
			{
				InnerList.RemoveAt(index);
			}
		}

		/// <summary>
		/// Adiciona um novo item para a lista.
		/// </summary>
		/// <param name="item"></param>
		public virtual void Add(TEntity item)
		{
			List<TEntity> newItems = new List<TEntity>();
			newItems.Add(item);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Add,
				NewItems = newItems,
				OldItems = null
			};
			if(OnCollectionChanging(args))
			{
				if(!CheckIfItemIsUnique(item))
				{
					throw new Exception(Colosoft.Business.Properties.Resources.Exception_ItemMustBuUniqueInList);
				}
				ProcessNewItem(item);
				InnerList.Add(item);
			}
		}

		/// <summary>
		/// Adiciona um faixa de itens para a lista.
		/// </summary>
		/// <param name="items"></param>
		public virtual void AddRange(IEnumerable<TEntity> items)
		{
			items.Require("items").NotNull();
			foreach (var i in items)
				Add(i);
		}

		/// <summary>
		/// Recupera o indice do item na lista.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual int IndexOf(TEntity item)
		{
			return InnerList.IndexOf(item);
		}

		/// <summary>
		/// Inserte o item na indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public virtual void Insert(int index, TEntity item)
		{
			List<TEntity> newItems = new List<TEntity>();
			newItems.Add(item);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Add,
				NewItems = newItems,
				OldItems = null,
				StartNewItems = index
			};
			if(OnCollectionChanging(args))
			{
				if(!CheckIfItemIsUnique(item))
				{
					throw new Exception(Colosoft.Business.Properties.Resources.Exception_ItemMustBuUniqueInList);
				}
			}
			ProcessNewItem(item);
			InnerList.Insert(index, item);
		}

		/// <summary>
		/// Limpa os itens da lista.
		/// </summary>
		public virtual void Clear()
		{
			List<TEntity> oldItems = new List<TEntity>(InnerList);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Reset,
				OldItems = oldItems,
				NewItems = null
			};
			if(OnCollectionChanging(args))
			{
				InnerList.Clear();
			}
		}

		/// <summary>
		/// Verifica se na lista contem o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Contains(TEntity item)
		{
			return Contains(item, EntityEqualityComparer<TEntity>.Instance);
		}

		/// <summary>
		/// Verifica se na coleção possui o item informado.
		/// </summary>
		/// <param name="item">Instancia do item que será pesquisado.</param>
		/// <param name="comparer">Instancia do comparador que será utilizado.</param>
		/// <returns></returns>
		public virtual bool Contains(TEntity item, IEqualityComparer<TEntity> comparer)
		{
			if(comparer == null)
				comparer = EqualityComparer<TEntity>.Default;
			foreach (TEntity local in InnerList)
				if(comparer.Equals(local, item))
					return true;
			return false;
		}

		/// <summary>
		/// Copia os itens para a lista.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public virtual void CopyTo(TEntity[] array, int arrayIndex)
		{
			List<TEntity> newItems = new List<TEntity>(array);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Add,
				NewItems = newItems,
				OldItems = null,
				StartNewItems = arrayIndex
			};
			if(OnCollectionChanging(args))
			{
				InnerList.CopyTo(array, arrayIndex);
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public virtual bool IsReadOnly
		{
			get
			{
				return Owner != null && Owner.IsReadOnly;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Remove o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Remove(TEntity item)
		{
			List<TEntity> newItems = new List<TEntity>();
			newItems.Add(item);
			NotifyCollectionChangingEventArgs<TEntity> args = new NotifyCollectionChangingEventArgs<TEntity>() {
				Action = NotifyCollectionChangingAction.Remove,
				OldItems = newItems,
				NewItems = null
			};
			if(OnCollectionChanging(args))
			{
				return InnerList.Remove(item);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Recupera o manipulador que cancela a velidação de somente leitura
		/// na alteração de propriedades.
		/// </summary>
		/// <returns></returns>
		public IDisposable CreateReadOnlyPropertyChangingCancelHandler()
		{
			return null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(LoadItems != null)
				return "Count = Lazy load";
			else
				return "Count = " + Count;
		}

		/// <summary>
		/// Processa as operações de Delete.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual DeleteResult ProcessDeleteOperations(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new DeleteResult(true, null);
			var removedItems = GetRemovedItems().ToArray();
			foreach (var i in removedItems)
			{
				if(i.ExistsInStorage)
				{
					var result = i.Delete(session);
					if(!result.Success)
						return new DeleteResult(result.Success, result.Message);
				}
			}
			foreach (var i in this)
				if(i is IDeleteOperationsContainer)
					((IDeleteOperationsContainer)i).ProcessDeleteOperations(session);
			return new DeleteResult(true, null);
		}

		/// <summary>
		/// Processa as operações de Update.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual SaveResult ProcessUpdateOperations(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
			var newItems = GetNewItems().ToArray();
			foreach (var i in GetChangedItems())
			{
				if(!newItems.Contains(i, EntityEqualityComparer<IEntity>.Instance))
				{
					var result = i.Save(session);
					if(!result.Success)
						return result;
				}
			}
			return new SaveResult(true, null);
		}

		/// <summary>
		/// Processa as operações de Insert.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual SaveResult ProcessInsertOperations(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
			var newItems = GetNewItems().ToArray();
			foreach (var i in newItems)
			{
				var result = i.Save(session);
				if(!result.Success)
					return result;
			}
			return new SaveResult(true, null);
		}

		/// <summary>
		/// Salva um objeto
		/// </summary>
		/// <returns></returns>
		public virtual SaveResult Save()
		{
			try
			{
				return Save(null);
			}
			catch(Exception ex)
			{
				Colosoft.Log.Logger.Error(ex.Message.GetFormatter(), ex);
				return new SaveResult(false, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true).GetFormatter());
			}
		}

		/// <summary>
		/// Salva os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns></returns>
		public virtual SaveResult Save(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
			var deleteResult = ProcessDeleteOperations(session);
			if(!deleteResult.Success)
				return new SaveResult(deleteResult.Success, deleteResult.Message);
			var saveResult = ProcessInsertOperations(session);
			if(!saveResult.Success)
				return saveResult;
			saveResult = ProcessUpdateOperations(session);
			if(!saveResult.Success)
				return saveResult;
			session.Executed += SavePersistenceSessionExecutedCallback;
			return new SaveResult(true, null);
		}

		/// <summary>
		/// Apaga os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns></returns>
		public virtual DeleteResult Delete(Data.IPersistenceSession session)
		{
			var removedItems = GetRemovedItems().ToArray();
			foreach (var i in removedItems)
			{
				if(i.ExistsInStorage)
				{
					var result = i.Delete(session);
					if(!result.Success)
						return new DeleteResult(result.Success, result.Message);
				}
			}
			foreach (var i in InnerList)
			{
				var result = i.Delete(session);
				if(!result.Success)
					return result;
			}
			session.Executed += DeletePersistenceSessionExecutedCallback;
			return new DeleteResult(true, null);
		}

		/// <summary>
		/// Cria uma instância com a cópia da instância original para edição. 
		/// </summary>
		/// <returns></returns>
		public virtual IEntity CloneToEdit()
		{
			var result = (BaseEntityList<TEntity>)Clone();
			result.CloneFrom = this;
			result.Instance = this.Instance;
			result.Owner = this.Owner;
			return result;
		}

		/// <summary>
		/// Registra na entidade que ela foi clona para edição a partir da instancia informada.
		/// </summary>
		/// <param name="cloneFrom">Instancia de origem da entidade.</param>
		public virtual void RegisterCloneToEdit(IEntity cloneFrom)
		{
			CloneFrom = cloneFrom;
			Instance = cloneFrom.Instance as IEntityList;
			Owner = cloneFrom.Owner;
		}

		/// <summary>
		/// Evento acioando quando a lista sofre alterações.
		/// </summary>
		event System.ComponentModel.ListChangedEventHandler System.ComponentModel.IBindingList.ListChanged {
			add
			{
				if(this.IsThreadSafe)
				{
					if(value != null)
					{
						var dispatcher = Colosoft.Threading.DispatcherManager.Instance.FromThread(System.Threading.Thread.CurrentThread);
						if(!_listChangedHandlers.ContainsKey(value))
							_listChangedHandlers.Add(value, new Colosoft.Collections.ListChangedWrapperEventData(dispatcher, value));
					}
				}
				else
					_listChanged += value;
			}
			remove {
				if(this.IsThreadSafe)
				{
					_listChangedHandlers.Remove(value);
				}
				else
					_listChanged -= value;
			}
		}

		/// <summary>
		/// Identifica se pode editar os itens da lista.
		/// </summary>
		public virtual bool AllowEdit
		{
			get
			{
				return CanEdit;
			}
		}

		/// <summary>
		/// Identifica se pode adicionar novos itens na lista usando o método System.ComponentModel.IBindingList.AddNew()
		/// </summary>
		public virtual bool AllowNew
		{
			get
			{
				return CanEdit;
			}
		}

		/// <summary>
		/// Identifica se pode remover algum item da lista usando 
		/// System.Collections.IList.Remove(System.Object) ou System.Collections.IList.RemoveAt(System.Int32).
		/// </summary>
		public virtual bool AllowRemove
		{
			get
			{
				return CanEdit;
			}
		}

		/// <summary>
		/// Identifica se a lista tem suporte para ordenação.
		/// </summary>
		public virtual bool SupportsSorting
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Propriedade usada na ordenação da lista.
		/// </summary>
		public virtual System.ComponentModel.PropertyDescriptor SortProperty
		{
			get
			{
				return _sortProperty;
			}
		}

		/// <summary>
		/// Identifica se a instancia da suporte para o evento System.ComponentModel.IBindingList.ListChanged
		/// </summary>
		bool System.ComponentModel.IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se a lista está ordenada.
		/// </summary>
		public virtual bool IsSorted
		{
			get
			{
				return _sortProperty != null;
			}
		}

		/// <summary>
		/// Direção pela qual a lista está ordenada.
		/// </summary>
		public System.ComponentModel.ListSortDirection SortDirection
		{
			get
			{
				return _sortDirection;
			}
		}

		/// <summary>
		/// Identifica se a lista possui suporte para pesquisa.
		/// </summary>
		public virtual bool SupportsSearching
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Adiciona a propriedade para os indices usados na pesquisa.
		/// </summary>
		/// <param name="property"></param>
		public virtual void AddIndex(System.ComponentModel.PropertyDescriptor property)
		{
		}

		/// <summary>
		/// Adiciona um novo item para a lista.
		/// </summary>
		/// <returns>Instancia do novo item adicionado.</returns>
		/// <exception cref="System.NotSupportedException">
		///     System.ComponentModel.IBindingList.AllowNew is false.
		/// </exception>
		public object AddNew()
		{
			if(AllowNew)
			{
				var loader = TypeManager.GetLoader(typeof(TEntity));
				var item = (TEntity)loader.Create(UIContext, TypeManager, ((IConnectedEntity)this).SourceContext);
				Add(item);
				return item;
			}
			else
				throw new NotSupportedException("List not allow new");
		}

		/// <summary>
		/// Ordena a lista baseado em um System.ComponentModel.PropertyDescriptor e um System.ComponentModel.ListSortDirection
		/// </summary>
		/// <param name="property"></param>
		/// <param name="direction"></param>
		/// <exception cref="System.NotSupportedException">System.ComponentModel.IBindingList.SupportsSorting is false.</exception>
		public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
		{
			if(SupportsSorting)
			{
				_sortProperty = property;
				_sortDirection = direction;
			}
			else
				throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o indice da linha que possue a propriedade informada.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual int Find(System.ComponentModel.PropertyDescriptor property, object key)
		{
			var lambda = System.Linq.DynamicExpression.ParseLambda(typeof(TEntity), typeof(bool), string.Format("{0} == @0", property.Name), key);
			var comparer = (Func<TEntity, bool>)lambda.Compile();
			for(var i = 0; i < InnerList.Count; i++)
				if(comparer(InnerList[i]))
					return i;
			return -1;
		}

		/// <summary>
		/// Remove o indice usado para a pesquisa.
		/// </summary>
		/// <param name="property"></param>
		public void RemoveIndex(System.ComponentModel.PropertyDescriptor property)
		{
		}

		/// <summary>
		/// Remove a ordenação aplicada pelo método System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)
		/// </summary>
		/// <exception cref="System.NotSupportedException">System.ComponentModel.IBindingList.SupportsSorting is false.</exception>
		public void RemoveSort()
		{
			if(SupportsSorting)
			{
				_sortProperty = null;
			}
			else
				throw new NotSupportedException();
		}

		/// <summary>
		/// Instancia do contexto da origem dos dados.
		/// </summary>
		Query.ISourceContext IConnectedEntity.SourceContext
		{
			get
			{
				return this.SourceContext;
			}
		}

		/// <summary>
		/// Conecta a entidade com a origem de dados informada.
		/// </summary>
		/// <param name="sourceContext"></param>
		void IConnectedEntity.Connect(Query.ISourceContext sourceContext)
		{
			_sourceContext = sourceContext;
		}

		/// <summary>
		/// Desconecta a entidade do contexto da origem dos dados.
		/// </summary>
		void IConnectedEntity.Disconnect()
		{
			_sourceContext = null;
		}

		/// <summary>
		/// Notifica que a entidade foi carregada.
		/// </summary>
		void ILoadableEntity.NotifyLoaded()
		{
			OnLoaded();
		}

		/// <summary>
		/// Registra a instancia da entidade.
		/// </summary>
		/// <param name="instanceGetter">Instancia responsável por recuperar o Instance.</param>
		void IEntityInstanceRegister.Register(IEntityInstanceGetter instanceGetter)
		{
			_instanceGetter = instanceGetter;
		}

		/// <summary>
		/// Instancia do Getter.
		/// </summary>
		IEntityInstanceGetter IEntityInstanceRegister.InstanceGetter
		{
			get
			{
				return _instanceGetter;
			}
		}

		/// <summary>
		/// Adiciona um observer para a instancia.
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="liveScope"></param>
		void Collections.INotifyCollectionChangedObserverContainer.AddObserver(Collections.INotifyCollectionChangedObserver observer, Collections.NotifyCollectionChangedObserverLiveScope liveScope)
		{
			observer.Require("observer").NotNull();
			((Collections.INotifyCollectionChangedObserverContainer)InnerList).AddObserver(observer, liveScope);
		}

		/// <summary>
		/// Remove o observer da coleção.
		/// </summary>
		/// <param name="observer"></param>
		void Collections.INotifyCollectionChangedObserverContainer.RemoveObserver(Collections.INotifyCollectionChangedObserver observer)
		{
			observer.Require("observer").NotNull();
			((Collections.INotifyCollectionChangedObserverContainer)InnerList).RemoveObserver(observer);
		}

		/// <summary>
		/// Adiciona o evento que será acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="priority"></param>
		void Collections.INotifyCollectionChangedDispatcher.AddCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler, Collections.NotifyCollectionChangedDispatcherPriority priority)
		{
			if(this.IsThreadSafe)
			{
				if(eventHandler != null)
				{
					var dispatcher = Colosoft.Threading.DispatcherManager.Dispatcher;
					if(dispatcher != null && !dispatcher.CheckAccess())
						dispatcher = null;
					if(!_collectionChangedHandlers.ContainsKey(eventHandler))
						_collectionChangedHandlers.Add(eventHandler, new Collections.CollectionChangedWrapperEventData(dispatcher, eventHandler, priority));
				}
			}
			else
				_collectionChanged += eventHandler;
		}

		/// <summary>
		/// Remove o evento registrado para ser acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		void Collections.INotifyCollectionChangedDispatcher.RemoveCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler)
		{
			if(this.IsThreadSafe)
			{
				_collectionChangedHandlers.Remove(eventHandler);
			}
			else
				_collectionChanged -= eventHandler;
		}

		/// <summary>
		/// Processa as operações de Delete.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		DeleteResult IDeleteOperationsContainer.ProcessDeleteOperations(Data.IPersistenceSession session)
		{
			return ProcessDeleteOperations(session);
		}

		/// <summary>
		/// Processa as operações de Update.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		SaveResult ISaveOperationsContainer.ProcessUpdateOperations(Data.IPersistenceSession session)
		{
			return ProcessUpdateOperations(session);
		}

		/// <summary>
		/// Processa as operações de Insert.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		SaveResult ISaveOperationsContainer.ProcessInsertOperations(Data.IPersistenceSession session)
		{
			return ProcessInsertOperations(session);
		}

		/// <summary>
		/// Registra a sessão de persistencia para a instancia.
		/// </summary>
		/// <param name="session"></param>
		void IEntitySavePersistenceSessionObserver.Register(Data.IPersistenceSession session)
		{
			session.Executed += SavePersistenceSessionExecutedCallback;
		}

		/// <summary>
		/// Recupera o Schema de serialização.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Serializa os dados da instancia no "Writer" informado.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			var elementName = typeof(TEntity).Name;
			foreach (var i in this)
			{
				if(i is System.Xml.Serialization.IXmlSerializable)
				{
					writer.WriteStartElement(elementName);
					((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
					writer.WriteEndElement();
				}
			}
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
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
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
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
			if(_indexes == null)
				throw new IndexNotFoundException(propertyName);
			lock (_indexes)
				if(!_indexes.TryGetValue(propertyName, out values) || values.Count == 0 || (indexType != ObservableCollectionIndexType.Any && !values.ContainsKey(indexType)))
					throw new IndexNotFoundException(propertyName);
			IObservableCollectionIndex<TEntity> index = null;
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
		public void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<TEntity, PropertyType>> property, ObservableCollectionIndexType type)
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
		public void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<TEntity, PropertyType>> property, ObservableCollectionIndexType type, IComparer<PropertyType> comparer)
		{
			property.Require("property").NotNull();
			var propertyInfo = property.GetMember() as System.Reflection.PropertyInfo;
			if(propertyInfo == null)
				throw new InvalidOperationException("Invalid property");
			var indexName = propertyInfo.Name;
			if(_indexes != null)
				lock (_indexes)
				{
					Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
					if(_indexes.TryGetValue(indexName, out values) && values.ContainsKey(type))
					{
						return;
					}
				}
			IObservableCollectionIndex<TEntity> index = null;
			var propertyGetter = property.Compile();
			var getter = new Func<TEntity, object>(f => propertyGetter(f));
			if(type == ObservableCollectionIndexType.Sorted || type == ObservableCollectionIndexType.Hash)
				index = new ObservableCollectionSortedIndex<TEntity>(indexName, this, new string[] {
					property.Name
				}, getter, Comparer<PropertyType>.Default);
			if(_indexes == null)
				_indexes = new Dictionary<string, Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>>>();
			lock (_indexes)
			{
				Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
				if(!_indexes.TryGetValue(indexName, out values))
				{
					values = new Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>>();
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
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
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
		public IEnumerable<TEntity> Search(System.Linq.Expressions.Expression<Func<TEntity, object>> property, object key)
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
		public IEnumerable<TEntity> Search(System.Linq.Expressions.Expression<Func<TEntity, object>> property, ObservableCollectionIndexType indexType, object key)
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
		private IEnumerable<TEntity> Search2(string propertyName, ObservableCollectionIndexType indexType, object key)
		{
			Dictionary<ObservableCollectionIndexType, IObservableCollectionIndex<TEntity>> values = null;
			if(_indexes == null)
				throw new IndexNotFoundException(propertyName);
			lock (_indexes)
				if(!_indexes.TryGetValue(propertyName, out values) || values.Count == 0 || (indexType != ObservableCollectionIndexType.Any && !values.ContainsKey(indexType)))
					throw new IndexNotFoundException(propertyName);
			IObservableCollectionIndex<TEntity> index = null;
			if(indexType == ObservableCollectionIndexType.Any)
				index = values.Values.FirstOrDefault();
			else
				index = values[indexType];
			return index[key];
		}

		/// <summary>
		/// Representa um objeto qe controla a carga
		/// </summary>
		class EntityMonitor : Threading.SimpleMonitor
		{
			private List<IDisposable> _children;

			private BaseEntityList<TEntity> _innerList;

			/// <summary>
			/// Construtor com inicialialização da entidade
			/// </summary>
			/// <param name="list">entidade que será carregada</param>
			public EntityMonitor(BaseEntityList<TEntity> list)
			{
				_innerList = list;
			}

			protected override void InnerEnter()
			{
				if(BusyCount == 0)
				{
					var items = _innerList.Where(f => f is ILoadable).Select(f => (ILoadable)f);
					var aux = new List<ILoadable>();
					foreach (var i in items)
						if(i != _innerList && !aux.Contains(i))
							aux.Add(i);
					_children = aux.Select(f => f.CreateLoadHandle()).ToList();
				}
			}

			/// <summary>
			/// Controle de finalização na carga da instância
			/// </summary>
			/// <param name="disposing">Informa se deve ou não atribuir o objeto instance da instancia</param>
			protected override void Dispose(bool disposing)
			{
				if(BusyCount == 1 && _children != null)
				{
					while (_children.Count > 0)
					{
						_children[0].Dispose();
						_children.RemoveAt(0);
					}
				}
			}
		}

		/// <summary>
		/// Implementação do recupera do instancia para um item da lista.
		/// </summary>
		class ItemEntityInstanceGetter : IEntityInstanceGetter
		{
			private BaseEntityList<TEntity> _list;

			private IEntity _item;

			/// <summary>
			/// Identifica se a instância é valida.
			/// </summary>
			public bool IsValid
			{
				get
				{
					var disposableState = _list as IDisposableState;
					return disposableState == null || disposableState.IsDisposed;
				}
			}

			/// <summary>
			/// Instancia do item associado.
			/// </summary>
			public IEntity Item
			{
				get
				{
					return _item;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="list">Instancia da lista</param>
			/// <param name="item">Item associado.</param>
			public ItemEntityInstanceGetter(BaseEntityList<TEntity> list, IEntity item)
			{
				_list = list;
				_item = item;
			}

			/// <summary>
			/// Recupera o instancia da entidade.
			/// </summary>
			/// <returns></returns>
			public IEntity GetInstance()
			{
				IEntityList instance = _list.Instance;
				if(instance == null)
					return null;
				foreach (IEntity entity in instance)
					if(_item.Equals(entity))
						return entity;
				return null;
			}
		}
	}
}
