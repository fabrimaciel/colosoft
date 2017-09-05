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
using System.Reflection;
using System.Collections;
using Colosoft.Lock;

namespace Colosoft.Business
{
	/// <summary>
	/// Representa uma entidade de negócio do sistema
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public abstract class Entity : IEntity, IRegisterPropertyChanged, IConnectedEntity, ILoadableEntity, IEntityPersistence, IEntityRecordObserver, IDisposableState, IEntityInstanceRegister, IEntityXmlSerializable
	{
		/// <summary>
		/// Lista na qual o objeto está incluído.
		/// </summary>
		private Colosoft.Collections.IObservableCollection _myList;

		/// <summary>
		/// Instancia original do objeto
		/// </summary>
		private IEntity _instance;

		private IEntityInstanceGetter _instanceGetter;

		/// <summary>
		/// Chave do registro que a instancia representa.
		/// </summary>
		private string _entityRecordKey;

		private IEntity _cloneFrom;

		private bool _isCloneFromChanged;

		private bool _isChangedAfterCloneToEdit;

		private bool _canSerialize = true;

		/// <summary>
		/// Variável usada para ignorar a notificação
		/// de que a instancia de origem do clone sofreu
		/// alguma alteração.
		/// </summary>
		private bool _ignoreIsCloneFromChanged;

		private bool _isLoaded;

		private bool _isFirstLoad;

		/// <summary>
		/// Lista das propriedades que foram alteradas
		/// </summary>
		private List<string> _changedProperties;

		/// <summary>
		/// Controlador de observadores
		/// </summary>
		private ObserverControl _observer;

		/// <summary>
		/// Mantém o estado do objeto
		/// </summary>
		private Lazy<InstanceState> _state;

		private EntityMonitor _monitor;

		private IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Loader da instancia.
		/// </summary>
		private IEntityLoader _loader;

		/// <summary>
		/// Instancia da transação de edição.
		/// </summary>
		private IEntity _editingTransactionInstance;

		private bool _isInstanceInitialized = false;

		private bool _isEditing;

		private bool _isReadOnly = false;

		private Colosoft.Threading.SimpleMonitor _readOnlyPropertyChangingCancelHandler = new Threading.SimpleMonitor();

		private bool _isDisposed;

		private bool _isInitInstance = true;

		private bool _initialized = false;

		private Colosoft.Query.ISourceContext _sourceContext;

		/// <summary>
		/// Instancia do observer do registro associado com a instancia.
		/// </summary>
		private Colosoft.Query.IRecordObserver _recordObserver;

		/// <summary>
		/// Armazena a relação da referencia da entidade.
		/// </summary>
		private Dictionary<string, IEntity> _references = new Dictionary<string, IEntity>();

		private IEntity _owner;

		private List<Tuple<string, Action>> _propertyUpdates;

		/// <summary>
		/// Indica se a instância está ou não locada.
		/// </summary>
		private bool _isLockedToEdit = false;

		/// <summary>
		/// Cache de estado de componente.
		/// </summary>
		private static object _lockObject = 1;

		private EventHandler<EntityDeletedEventArgs> _deleted;

		private EventHandler<EntityDeletingEventArgs> _deleting;

		/// <summary>
		/// Armazena a relação dos filhos singulares que foram removidos.
		/// </summary>
		private Dictionary<string, IEntity> _singleChildRemoved = new Dictionary<string, IEntity>();

		private string _transaction;

		private Predicate<System.ComponentModel.PropertyDescriptor> _typeDescriptorPropertiesFilter;

		/// <summary>
		/// Evento chamado ao terminar atualizações.
		/// </summary>
		public event EventHandler AcceptedChanges;

		/// <summary>
		/// Evento acionado quando a entidade sofre alguma alteração.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Evento acionado quando a entidade for carregada.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Evento acionado ao iniciar a alteração de uma propriedade
		/// </summary>
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

		/// <summary>
		/// Evento acionado após a alteração de uma propriedade
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
		event EventHandler<EntityDeletingEventArgs> IEntityPersistence.Deleting {
			add
			{
				_deleting += value;
			}
			remove {
				_deleting -= value;
			}
		}

		/// <summary>
		/// Evento acionado quando a entidade for apagada.
		/// </summary>
		event EventHandler<EntityDeletedEventArgs> IEntityPersistence.Deleted {
			add
			{
				_deleted += value;
			}
			remove {
				_deleted -= value;
			}
		}

		/// <summary>
		/// Filtro usado para determinar as propriedades que serão recuperadas
		/// pelo TypeDescriptor.
		/// </summary>
		public Predicate<System.ComponentModel.PropertyDescriptor> TypeDescriptorPropertiesFilter
		{
			get
			{
				return _typeDescriptorPropertiesFilter;
			}
			set
			{
				_typeDescriptorPropertiesFilter = value;
			}
		}

		/// <summary>
		/// Recupera o nome do tipo da entidade.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual IMessageFormattable EntityTypeName
		{
			get
			{
				return this.GetType().Name.GetFormatter();
			}
		}

		/// <summary>
		/// Identifica se a entidade suporta inicialização da instancia.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		protected bool IsInitInstance
		{
			get
			{
				return _isInitInstance;
			}
			set
			{
				_isInitInstance = value;
			}
		}

		/// <summary>
		/// Identifica se a instancia foi liberada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
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
		/// Identifica se houve alguma alteração na origem do clone em relação 
		/// aos dados da entidade, ou seja, se alterei os dados originais
		/// depois que o clone foi realizado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsCloneFromChanged
		{
			get
			{
				return _isCloneFromChanged;
			}
			private set
			{
				if(_isCloneFromChanged != value)
				{
					_isCloneFromChanged = value;
					if(!_ignoreIsCloneFromChanged)
						RaisePropertyChanged("IsCloneFromChanged");
				}
			}
		}

		/// <summary>
		/// Identifica se a instancia foi alterada após o clone para edição.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsChangedAfterCloneToEdit
		{
			get
			{
				return _isChangedAfterCloneToEdit;
			}
			private set
			{
				if(_isChangedAfterCloneToEdit != value)
				{
					_isChangedAfterCloneToEdit = value;
					RaisePropertyChanged("IsChangedAfterCloneToEdit");
				}
			}
		}

		/// <summary>
		/// Identifica se a entidade está sendo inicializada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsInitializing
		{
			get
			{
				return _monitor.Busy;
			}
		}

		/// <summary>
		/// Instância a partir da qual q instância atual foi clonada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEntity CloneFrom
		{
			get
			{
				return _cloneFrom;
			}
			protected set
			{
				if(_cloneFrom != value)
				{
					if(_cloneFrom != null)
					{
						_cloneFrom.Changed -= CloneFromChanged;
						_cloneFrom.PropertyChanged -= CloneFromPropertyChanged;
						_cloneFrom.AcceptedChanges -= CloneFromAcceptedChanges;
					}
					_cloneFrom = value;
					if(_cloneFrom != null)
					{
						_cloneFrom.Changed += CloneFromChanged;
						_cloneFrom.PropertyChanged += CloneFromPropertyChanged;
						_cloneFrom.AcceptedChanges += CloneFromAcceptedChanges;
					}
					IsCloneFromChanged = false;
					RaisePropertyChanged("CloneFrom");
				}
			}
		}

		/// <summary>
		/// Gerenciador dos tipos.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEntityTypeManager TypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Lista de propriedades alteradas
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEnumerable<string> ChangedProperties
		{
			get
			{
				return (IEnumerable<string>)_changedProperties ?? new string[0];
			}
		}

		/// <summary>
		/// Objeto que controla os objetos que observam as alterações
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public ObserverControl Observer
		{
			get
			{
				return _observer;
			}
			set
			{
				_observer = value;
			}
		}

		/// <summary>
		/// Informa se houve ou não alteração na instância
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool IsChanged
		{
			get
			{
				if((_changedProperties != null) && (_changedProperties.Count > 0))
					return true;
				foreach (var i in _loader.GetChildrenAccessors())
				{
					var child = i.Get(this);
					if(child != null && child.IsChanged)
						return true;
				}
				foreach (var i in _loader.GetLinksAccessors())
				{
					var link = i.Get(this);
					if(link != null && link.IsChanged)
						return true;
				}
				foreach (var i in _loader.GetReferences())
				{
					if(this.IsReferenceInitialized(i.Name))
					{
						var reference = i.ParentValueGetter(this);
						if(reference != null && reference.IsChanged)
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Identificador unico da entidade.
		/// </summary>
		public virtual int Uid
		{
			get
			{
				if(_loader != null)
					return _loader.GetInstanceUid(this);
				else
					return 0;
			}
			set
			{
				if(HasUid)
				{
					var oldValue = _loader.GetInstanceUid(this);
					if(oldValue != value && RaisePropertyChanging(_loader.UidPropertyName, value))
					{
						_loader.SetInstanceUid(this, value);
						RaisePropertyChanged(_loader.UidPropertyName, "IsLockedToEdit", "IsLockedToMe", "CanEdit");
					}
				}
			}
		}

		/// <summary>
		/// Nome que identifica a instância
		/// </summary>
		public virtual string FindName
		{
			get
			{
				if(_loader != null)
					return _loader.GetInstanceFindName(this);
				return null;
			}
		}

		/// <summary>
		/// Identifica se a entidade possui um identificador único.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool HasUid
		{
			get
			{
				if(_loader != null)
					return _loader.HasUid;
				return false;
			}
		}

		/// <summary>
		/// Identifica se a entidade possui um nome único.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool HasFindName
		{
			get
			{
				if(_loader != null)
					return _loader.HasFindName;
				return false;
			}
		}

		/// <summary>
		/// Identifica se a entidade está sendo editada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsEditing
		{
			get
			{
				return _isEditing;
			}
			private set
			{
				if(_isEditing != value)
				{
					_isEditing = value;
					RaisePropertyChanged("IsEditing");
				}
			}
		}

		/// <summary>
		/// Identifica que a instancia é somente leitura.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool IsReadOnly
		{
			get
			{
				return (Owner != null && Owner.IsReadOnly) || _isReadOnly;
			}
			set
			{
				if(_isReadOnly != value)
				{
					_isReadOnly = value;
					RaisePropertyChanged("IsReadOnly", "CanEdit");
				}
			}
		}

		/// <summary>
		/// Determina se a instancia da entidade pode ser editada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool CanEdit
		{
			get
			{
				return !IsReadOnly && IsLockedToEdit;
			}
		}

		/// <summary>
		/// Instancia da lista associada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual Colosoft.Collections.IObservableCollection MyList
		{
			get
			{
				return _myList;
			}
		}

		/// <summary>
		/// Veifica se a instância está ou não locada.
		/// </summary>
		/// <returns></returns>
		[System.Xml.Serialization.XmlIgnore]
		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerNonUserCode()]
		public bool IsLocked
		{
			get
			{
				if(Owner != null && Owner.IsLocked || !ExistsInStorage)
					return true;
				var lockProcess = LockProcessManager.LockProcess;
				return lockProcess != null && lockProcess.IsLocked(new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid));
			}
		}

		/// <summary>
		/// Veifica se a instância está ou não locada.
		/// </summary>
		/// <returns></returns>
		[System.Xml.Serialization.XmlIgnore]
		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerNonUserCode()]
		public bool IsLockedToMe
		{
			get
			{
				return IsLockedToEdit;
			}
		}

		/// <summary>
		/// Instancia do recuperador do Instance.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		protected IEntityInstanceGetter InstanceGetter
		{
			get
			{
				return _instanceGetter;
			}
		}

		/// <summary>
		/// Identificador se a instancia original do objeto foi inicializada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsInstanceInicialized
		{
			get
			{
				return _isInstanceInitialized;
			}
		}

		/// <summary>
		/// Instancia original do objeto.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEntity Instance
		{
			get
			{
				IEntity instance = null;
				if(InstanceGetter != null)
				{
					instance = InstanceGetter.GetInstance();
					_isInstanceInitialized = true;
				}
				if(instance == null)
					instance = _instance;
				if(instance == null)
				{
					InitInstance();
					instance = _instance;
				}
				return instance;
			}
		}

		/// <summary>
		/// Ações para atualização de campos.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public List<Tuple<string, Action>> PropertyUpdates
		{
			get
			{
				return _propertyUpdates;
			}
		}

		/// <summary>
		/// Entidade dona da entidade atual.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual IEntity Owner
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
					if(_owner != null)
						_owner.PropertyChanged += OwnerPropertyChanged;
					if(_sourceContext == null && value is IConnectedEntity)
						_sourceContext = ((IConnectedEntity)value).SourceContext;
					RaisePropertyChanged("Owner");
				}
			}
		}

		/// <summary>
		/// Identifica se a instancia já existe na fonte de armazenamento.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual bool ExistsInStorage
		{
			get
			{
				return HasUid && Uid > 0;
			}
		}

		/// <summary>
		/// Indica que a instância está locada para edição.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsLockedToEdit
		{
			get
			{
				return (MyList is IEntityLink ? ((IEntityLink)MyList).IsLockedToEdit : (Owner != null && Owner.IsLockedToEdit)) || (CloneFrom != null && CloneFrom.IsLockedToEdit) || Uid <= 0 || _isLockedToEdit;
			}
			set
			{
				if(value != _isLockedToEdit)
				{
					InitInstance();
					_isLockedToEdit = value;
					RaisePropertyChanged("IsLockedToEdit", "IsLockedToMe", "CanEdit");
				}
			}
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual Type ModelType
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Estado da instância
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public Validation.IStateble InstanceState
		{
			get
			{
				if(!_isDisposed)
					return _state.Value;
				return InvalidStateble.Instance;
			}
		}

		/// <summary>
		/// Contexto da interface com o usuário
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public string UIContext
		{
			get;
			protected set;
		}

		/// <summary>
		/// Loader da instancia.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEntityLoader Loader
		{
			get
			{
				return _loader;
			}
		}

		/// <summary>
		/// Identifica se a instancia recebeu a notificação de que ela foi iniciada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}

		/// <summary>
		/// Construtor e inicializador de objetos
		/// </summary>
		protected Entity(string uiContext, IEntityTypeManager entityTypeManager = null)
		{
			_isFirstLoad = true;
			_monitor = new EntityMonitor(this);
			this.UIContext = uiContext ?? "WPF";
			_entityTypeManager = entityTypeManager;
			if(_entityTypeManager == null)
				_entityTypeManager = EntityTypeManager.Instance;
			var entityType = this.GetType();
			_loader = _entityTypeManager.GetLoader(entityType);
			_observer = new ObserverControl();
			_state = new Lazy<InstanceState>(CreateState);
			_propertyUpdates = new List<Tuple<string, Action>>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~Entity()
		{
			Dispose(false);
		}

		/// <summary>
		/// Define o Instance da instancia.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="instanceGetter"></param>
		private void SetInstance(IEntity instance, IEntityInstanceGetter instanceGetter)
		{
			_instance = instance;
			_instanceGetter = instanceGetter;
		}

		/// <summary>
		/// Notifica lock.
		/// </summary>
		/// <param name="stateInfo"></param>
		private void NotifyLock(object stateInfo)
		{
			if(!string.IsNullOrEmpty(_transaction) && LockProcessManager.RegisterLockProcess != null)
				LockProcessManager.RegisterLockProcess.RegisterLock(_transaction);
			_transaction = string.Empty;
		}

		/// <summary>
		/// Notifica remoção do lock.
		/// </summary>
		private void NotifyUnLock()
		{
			if(!string.IsNullOrEmpty(_transaction) && LockProcessManager.RegisterLockProcess != null)
				LockProcessManager.RegisterLockProcess.UnRegisterLock(_transaction);
			_transaction = string.Empty;
		}

		private void InstanceStateRaiseStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			RaiseStateChanged(e.PropertyName);
		}

		/// <summary>
		/// Cria o estado para a instancia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private InstanceState CreateState()
		{
			var entityType = this.GetType();
			var validationManager = Validation.ValidationManager.Instance;
			if(validationManager == null)
				throw new InvalidOperationException("ValidationManager undefined");
			InstanceState instanceState = new InstanceState(this, _entityTypeManager, validationManager, InstanceStateRaiseStateChanged, Colosoft.Globalization.Culture.SystemCulture);
			if(_isFirstLoad)
				_isFirstLoad = false;
			return instanceState;
		}

		/// <summary>
		/// Inicializa a instância base do objeto
		/// </summary>
		private void InitInstance()
		{
			if(!_isDisposed && _instance == null && IsInitInstance && Loader.InnerInstanceSupport)
			{
				_instance = this.Clone() as IEntity;
				_isInstanceInitialized = true;
				((Entity)_instance).IsInitInstance = false;
				foreach (var accessor in Loader.GetChildrenAccessors())
				{
					var child = accessor.Get(this);
					if(child is IEntityInstanceRegister)
						((IEntityInstanceRegister)child).Register(new EntityInstanceGetter(this, accessor));
				}
				foreach (var accessor in Loader.GetLinksAccessors())
				{
					var link = accessor.Get(this);
					if(link is IEntityInstanceRegister)
						((IEntityInstanceRegister)link).Register(new EntityInstanceGetter(this, accessor));
				}
			}
		}

		/// <summary>
		/// Dispara os eventos padrão do sistema
		/// </summary>
		/// <param name="propertyName">Nome da propriedade</param>
		internal void PropertyChangedEventDispatch(string propertyName)
		{
			if(propertyName == "IsChanged")
				OnChanged();
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityPropertyChangedEvent>().Publish(new EntityPropertyChangedEventArgs(this, propertyName));
			if(PropertyChanged != null)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			if((Observer != null) && (Observer.HasObserver))
				Observer.Raise(this, propertyName);
			if(propertyName == "IsChanged")
				IsChangedAfterCloneToEdit = true;
		}

		/// <summary>
		/// Método acionado toda vez que algum propriedade do filho da entidade for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChildChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("IsChanged");
		}

		/// <summary>
		/// Método acionado quando a instancia de um link sofrer alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LinkChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("IsChanged");
		}

		/// <summary>
		/// Reseta o valor da reference.
		/// </summary>
		/// <param name="reference"></param>
		private void ResetReference(EntityLoaderReference reference)
		{
			lock (_references)
				_references.Remove(reference.Name);
		}

		/// <summary>
		/// Remove o registro da lista pai.
		/// </summary>
		/// <param name="ownerList"></param>
		private void UnregisterList(Colosoft.Collections.IObservableCollection ownerList)
		{
			var list = _myList;
			if(list != null)
			{
				list.CollectionChanged -= MyListCollectionChanged;
				list.PropertyChanged -= MyListPropertyChanged;
				if(_myList != null)
				{
					_myList = null;
					RaisePropertyChanged("MyList", "Owner");
				}
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
			OnMyListCollectionChanged(e);
		}

		/// <summary>
		/// Método acionado quando a lista onde a entidade está inserida for alterad.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnMyListCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
		}

		/// <summary>
		/// Método acionado quando uma propriedade da lista for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Owner")
			{
				if(_myList is IEntity)
				{
					if(_owner != null)
						_owner.PropertyChanged -= OwnerPropertyChanged;
					_owner = ((IEntity)_myList).Owner;
					if(_owner != null)
						_owner.PropertyChanged += OwnerPropertyChanged;
				}
				RaisePropertyChanged("Owner");
			}
		}

		/// <summary>
		/// Remove o registro do observer do registro associado com a entidade.
		/// </summary>
		private void UnregisterRecordObserver()
		{
			var modelType = ModelType;
			var manager = Colosoft.Query.RecordObserverManager.Instance;
			if(modelType != null && _recordObserver != null && manager != null && manager.IsEnabled)
			{
				manager.Unregister(Colosoft.Reflection.TypeName.Get(modelType), _recordObserver);
				if(_recordObserver is IDisposable)
					((IDisposable)_recordObserver).Dispose();
				_recordObserver = null;
			}
		}

		/// <summary>
		/// Recupera a instancia removida para o filho associado com o nome informado.
		/// </summary>
		/// <param name="childName"></param>
		/// <returns></returns>
		protected IEntity GetSingleChildRemoved(string childName)
		{
			IEntity result = null;
			_singleChildRemoved.TryGetValue(childName, out result);
			return result;
		}

		/// <summary>
		/// Conecta a entidade de destino a conexão usada na entidade de origem.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		protected static void Connect(IEntity from, IEntity to)
		{
			if(to is IConnectedEntity && from is IConnectedEntity)
				((Colosoft.Business.IConnectedEntity)to).Connect(((Colosoft.Business.IConnectedEntity)from).SourceContext);
		}

		/// <summary>
		/// Verifica se a instancia já foi liberada.
		/// </summary>
		protected void CheckDispose()
		{
			if(_isDisposed)
				throw new ObjectDisposedException("this");
		}

		/// <summary>
		/// Atualiza os dados da chave associada com a entidade.
		/// </summary>
		protected virtual void UpdateRecordKey()
		{
			var resourceKey = _loader.GetRecordKey(this);
			if(resourceKey != null)
				_entityRecordKey = resourceKey.Key;
		}

		/// <summary>
		/// Método acionado quando o identificador único da entidade for alterado.
		/// </summary>
		protected virtual void OnUidChanged()
		{
			_loader.NotifyUidChanged(this);
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
		/// Inicializa as referências as instancia.
		/// </summary>
		/// <param name="references"></param>
		protected void InitializeReferences(EntityLoaderReferenceContainer references)
		{
			if(references == null)
				return;
			foreach (var i in references)
			{
				InitializeReference(i.Key, i.Value);
				references.NotifyLoaded(i.Key);
			}
		}

		/// <summary>
		/// Inicializa a instancia a referência.
		/// </summary>
		/// <param name="referenceName">Nome da referencia.</param>
		/// <param name="entity"></param>
		protected void InitializeReference(string referenceName, IEntity entity)
		{
			referenceName.Require("referenceName").NotNull().NotEmpty();
			EntityLoaderReference reference = null;
			if(!_loader.TryGetReference(referenceName, out reference))
				throw new Exception(string.Format("Reference '{0}' not found", referenceName));
			lock (_references)
			{
				_references.Remove(referenceName);
				_references.Add(referenceName, entity);
			}
		}

		/// <summary>
		/// Recuepra o valor de uma referencia da entidade.
		/// </summary>
		/// <typeparam name="TReference"></typeparam>
		/// <param name="referenceName"></param>
		/// <param name="isLazy">Identifica se é para carregar em modo lazy.</param>
		/// <returns></returns>
		protected TReference GetReference<TReference>(string referenceName, bool isLazy = false) where TReference : IEntity
		{
			referenceName.Require("referenceName").NotNull().NotEmpty();
			if(this.IsDisposed)
				return default(TReference);
			IEntity result = null;
			bool found = false;
			lock (_references)
				found = _references.TryGetValue(referenceName, out result);
			if(!found)
			{
				EntityLoaderReference reference = null;
				if(!_loader.TryGetReference(referenceName, out reference))
					throw new Exception(string.Format("Reference '{0}' not found", referenceName));
				if(_sourceContext == null)
					throw new Exception(string.Format("SourceContext undefined in entity for load reference '{0}'", referenceName));
				result = _loader.GetEntityReference(this, referenceName, this.UIContext, _entityTypeManager, _sourceContext, isLazy);
				lock (_references)
				{
					_references.Remove(referenceName);
					_references.Add(referenceName, result);
				}
			}
			return (TReference)result;
		}

		/// <summary>
		/// Reseta o valor de uma referencia carregada na entidade.
		/// </summary>
		/// <param name="referenceName"></param>
		protected void ResetReference(string referenceName)
		{
			referenceName.Require("referenceName").NotNull().NotEmpty();
			EntityLoaderReference reference = null;
			if(!_loader.TryGetReference(referenceName, out reference))
				throw new Exception(string.Format("Reference '{0}' not found", referenceName));
			lock (_references)
				_references.Remove(reference.Name);
			RaisePropertyChanged(reference.ParentPropertyName);
		}

		/// <summary>
		/// Verifica se a referencia com o nome informado já foi inicializada.
		/// </summary>
		/// <param name="referenceName"></param>
		/// <returns></returns>
		protected bool IsReferenceInitialized(string referenceName)
		{
			referenceName.Require("referenceName").NotNull().NotEmpty();
			EntityLoaderReference reference = null;
			if(!_loader.TryGetReference(referenceName, out reference))
				throw new Exception(string.Format("Reference '{0}' not found", referenceName));
			lock (_references)
				return _references.ContainsKey(reference.Name);
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
		/// Evento acionado quando a entidade sofre alteração.
		/// </summary>
		protected virtual void OnChanged()
		{
			if(Changed != null)
				Changed(this, EventArgs.Empty);
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
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="notify">Identifica se é para notificar a inicialização.</param>
		protected void Initialize(bool notify)
		{
			if(!_initialized)
			{
				_initialized = true;
				var entityType = this.GetType();
				var entityEventManager = EntityEventManager.Instance;
				if(entityEventManager == null)
					throw new InvalidOperationException("EntityEventManager undefined");
				entityEventManager.Register(entityType);
				if(notify)
					OnInitialized();
			}
		}

		/// <summary>
		/// Verifica se é para ignorar 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected virtual bool IgnoreProperty(string propertyName)
		{
			return IsBaseProperty(propertyName);
		}

		/// <summary>
		/// Verifica se a propriedade informada é uma propriedade base da classe.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <returns></returns>
		protected virtual bool IsBaseProperty(string propertyName)
		{
			return propertyName == "IsEditing" || propertyName == "IsLoaded" || propertyName == "IsChanged" || propertyName == "RowVersion" || propertyName == "Owner" || propertyName == "MyList" || propertyName == "IsLockedToEdit" || propertyName == "IsLockedToMe" || propertyName == "IsReadOnly" || propertyName == "CanEdit" || propertyName == "CloneFrom" || propertyName == "IsCloneFromChanged" || propertyName == "IsChangedAfterCloneToEdit" || propertyName == "ExistsInStorage" || propertyName.StartsWith("State.");
		}

		/// <summary>
		/// Dispara eventos ocorridas para o inicio de alteração da propriedade
		/// </summary>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="newValue">Valor da prorpiedade</param>
		protected virtual bool RaisePropertyChanging(string propertyName, object newValue)
		{
			if(CanValidateReadOnlyOnChanging() && IsReadOnly)
				return false;
			var args = new EntityPropertyChangingEventArgs(this, propertyName);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityPropertyChangingEvent>().Publish(args);
			if(args.Cancel)
				return false;
			if(PropertyChanging != null)
			{
				PropertyChanging(this, args);
				if(args.Cancel)
					return false;
			}
			IEntity oldValue = null;
			string childName = null;
			if(Loader != null && Loader.TryGetSingleChildFromProperty(this, propertyName, out childName, out oldValue))
			{
				lock (_singleChildRemoved)
				{
					IEntity current = null;
					if(!_singleChildRemoved.TryGetValue(childName, out current))
					{
						if(oldValue != null && oldValue.ExistsInStorage)
							_singleChildRemoved.Add(childName, oldValue);
					}
					else
					{
						if(current != null && current.Equals(newValue))
							_singleChildRemoved.Remove(childName);
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Dispara ou registra as alterações ocorridas para a propriedade de estado
		/// </summary>
		/// <param name="propertyName">Nome das propriedades</param>
		protected virtual void RaiseStateChanged(string propertyName)
		{
			RaisePropertyChanged(string.Format("State.{0}", propertyName));
		}

		/// <summary>
		/// Registra propriedades alteradas.
		/// </summary>
		/// <param name="propertyNames"></param>
		protected void AddChangedProperties(params string[] propertyNames)
		{
			if(_changedProperties == null)
				_changedProperties = new List<string>();
			foreach (var propertyName in propertyNames)
				if(!_changedProperties.Contains(propertyName))
					_changedProperties.Add(propertyName);
		}

		/// <summary>
		/// Dispara ou registra as alterações ocorridas para as propriedades.
		/// </summary>
		/// <param name="addChangedProperties">Identifica se é para adicionar na coleção das propriedades alteradas.</param>
		/// <param name="propertyNames">Nome das propriedades que foram alteradas.</param>
		protected virtual void RaisePropertyChanged2(bool addChangedProperties, params string[] propertyNames)
		{
			if(!_monitor.Busy && propertyNames != null && !IsDisposed)
			{
				var isChanged = false;
				IEnumerable<string> propertyNames2 = propertyNames;
				List<string> referencesProperties = null;
				foreach (var reference in _loader.GetReferenceByWatchedProperties(propertyNames))
				{
					if(referencesProperties == null)
					{
						referencesProperties = new List<string>();
						propertyNames2 = propertyNames2.Union(referencesProperties);
					}
					referencesProperties.Add(reference.ParentPropertyName);
					ResetReference(reference);
				}
				foreach (string propertyName in propertyNames2)
				{
					if(addChangedProperties && !IgnoreProperty(propertyName))
					{
						AddChangedProperties(propertyName);
						isChanged = true;
					}
					if(Loader.KeysPropertyNames != null && Loader.KeysPropertyNames.Contains(propertyName))
					{
						UpdateRecordKey();
						if(!HasUid && Loader.HasChildren)
							OnUidChanged();
					}
					if(_observer != null && _observer.HasObserver)
						_observer.RegisterNotify(propertyName);
					if(_state.IsValueCreated)
					{
						var property = this.InstanceState[propertyName];
						if(property != null && property.ReloadSettings)
						{
							this.InstanceState.ClearStateCache();
							_isFirstLoad = true;
						}
					}
					PropertyChangedEventDispatch(propertyName);
					if(HasUid && propertyName == _loader.UidPropertyName)
					{
						OnUidChanged();
					}
				}
				if(isChanged)
					PropertyChangedEventDispatch("IsChanged");
			}
		}

		/// <summary>
		/// Dispara ou registra as alterações ocorridas para a propriedade
		/// </summary>
		/// <param name="propertyNames">Nome das propriedades</param>
		protected virtual void RaisePropertyChanged(params string[] propertyNames)
		{
			RaisePropertyChanged2(true, propertyNames);
		}

		/// <summary>
		/// Cria a instancia do filho da entidade.
		/// </summary>
		/// <typeparam name="TChild">Tipo do filho.</typeparam>
		/// <param name="name">Nome do fiho.</param>
		/// <returns></returns>
		protected TChild CreateChild<TChild>(string name) where TChild : IEntity
		{
			var child = Loader.CreateChild<TChild>(this, name, UIContext, TypeManager, ((IConnectedEntity)this).SourceContext);
			RegisterChild(child);
			return child;
		}

		/// <summary>
		/// Cria a instancia do link da entidade.
		/// </summary>
		/// <typeparam name="TLink"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		protected TLink CreateLink<TLink>(string name) where TLink : IEntity
		{
			var link = Loader.CreateLink<TLink>(this, name, UIContext, TypeManager, ((IConnectedEntity)this).SourceContext);
			RegisterLink(link);
			return link;
		}

		/// <summary>
		/// Registra a instancia do link.
		/// </summary>
		/// <param name="link"></param>
		protected virtual void RegisterLink(IEntity link)
		{
			if(link != null)
			{
				link.Owner = this;
				link.Changed += LinkChanged;
			}
		}

		/// <summary>
		/// Remove o registro do link da entidade.
		/// </summary>
		/// <param name="link"></param>
		protected virtual void UnregisterLink(IEntity link)
		{
			if(link != null)
			{
				link.Changed -= LinkChanged;
				if(link.Owner == this)
					link.Owner = null;
			}
		}

		/// <summary>
		/// Registra a instancia de um filho da entidade.
		/// </summary>
		/// <param name="child"></param>
		protected void RegisterChild(IEntity child)
		{
			if(child != null)
			{
				child.Owner = this;
				child.Changed += ChildChanged;
			}
		}

		/// <summary>
		/// Registra a instancia de um filho da entidade.
		/// </summary>
		/// <param name="child">Instancia do filho</param>
		/// <param name="childName">Nome do filho</param>
		protected void RegisterChild(IEntity child, string childName)
		{
			if(child != null)
			{
				_loader.RegisterChild(this, child, childName);
				child.Changed += ChildChanged;
			}
		}

		/// <summary>
		/// Remove o registro de uma instancia de um filho da entidade.
		/// </summary>
		/// <param name="child"></param>
		protected void UnregisterChild(IEntity child)
		{
			if(child != null)
			{
				child.Changed -= ChildChanged;
				if(child.Owner == this)
					child.Owner = null;
			}
		}

		/// <summary>
		/// Método acionado quando a entidade for inicializada.
		/// </summary>
		protected virtual void OnInitialized()
		{
			var args = new EntityInitializedEventArgs(this);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityInitializedEvent>().Publish(args);
		}

		/// <summary>
		/// Método acionado quando a entidade estiver sendo validada.
		/// </summary>
		/// <param name="validationResult"></param>
		/// <returns>Retorna false é a operação for cancelada.</returns>
		protected virtual bool OnValidating(ref Validation.ValidationResult validationResult)
		{
			var args = new EntityValidatingEventArgs(this, validationResult);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityValidatingEvent>().Publish(args);
			validationResult = args.ValidationResult;
			return !args.Cancel;
		}

		/// <summary>
		/// Método acioando quando a entidade for validada.
		/// </summary>
		/// <param name="validationResult"></param>
		protected virtual void OnValidated(ref Validation.ValidationResult validationResult)
		{
			var args = new EntityValidatedEventArgs(this, validationResult);
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityValidatedEvent>().Publish(args);
			validationResult = args.ValidationResult;
		}

		/// <summary>
		/// Método acionado quando a entidade estiver sendo salva.
		/// </summary>
		/// <returns>Retorna false é a operação for cancelada.</returns>
		protected virtual SaveResult OnSaving()
		{
			var result = CheckProperties();
			if(result.Success)
			{
				var args = new EntitySavingEventArgs(this);
				Colosoft.Domain.DomainEvents.Instance.GetEvent<EntitySavingEvent>().Publish(args);
				if(Saving != null)
					Saving(this, args);
				return new SaveResult(!args.Cancel, args.Message);
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Verifica as propriedades requeridas ou necssárias.
		/// </summary>
		/// <returns></returns>
		private SaveResult CheckProperties()
		{
			var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var state = this.InstanceState[property.Name];
				if(state != null)
				{
					if((state.IsRequired) || (state.IsNecessary))
					{
						if(!CheckValue(property.GetValue(this, null)))
						{
							return new SaveResult() {
								Success = false,
								Message = String.Format(Colosoft.Business.Properties.Resources.Exception_ValueIsRquiredOrNecessaryToProperty, property.Name).GetFormatter()
							};
						}
					}
				}
			}
			return new SaveResult() {
				Success = true
			};
		}

		/// <summary>
		/// Verifica se o valor está preenchido.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool CheckValue(object value)
		{
			if(value == null)
			{
				return false;
			}
			if(value is String)
			{
				return !String.IsNullOrEmpty(value.ToString());
			}
			else if((value is int) || (value is byte) || (value is long))
			{
				long longValue = Convert.ToInt64(value);
				return longValue != 0;
			}
			else if(value is double)
			{
				return ((double)value).IsNotClose(0.0);
			}
			else if(value is Sex)
			{
				return ((Sex)value) != Sex.None;
			}
			return true;
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
			if(_deleting != null)
				_deleting(this, args);
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
			if(_deleted != null)
				_deleted(this, args);
		}

		/// <summary>
		/// Atualiza a lista vinculada com a instancia.
		/// </summary>
		/// <param name="isAcceptChanges">
		/// Identifica se a atualização está partindo de uma chamada
		/// que está aceitando as modificações da entidade.
		/// </param>
		protected void UpdateMyList(bool isAcceptChanges)
		{
			if(_myList != null)
			{
				IEntity myListEntity = null;
				foreach (object item in _myList)
				{
					var currentEntity = item as IEntity;
					if(currentEntity != null && currentEntity.Equals(this))
					{
						myListEntity = currentEntity;
						break;
					}
				}
				if(myListEntity != null)
				{
					if(myListEntity != this && IsChangedAfterCloneToEdit)
					{
						_ignoreIsCloneFromChanged = true;
						try
						{
							myListEntity.CopyFrom(this);
							if((Entity)this.CloneFrom != null && Loader.InnerInstanceSupport)
							{
								var entity = (Entity)this.CloneFrom;
								SetInstance(entity.Instance, entity.InstanceGetter);
							}
							if(isAcceptChanges)
								myListEntity.AcceptChanges();
						}
						finally
						{
							_ignoreIsCloneFromChanged = false;
						}
						IsChangedAfterCloneToEdit = false;
						IsCloneFromChanged = false;
					}
				}
				else
				{
					var list = _myList as IList;
					if(list == null)
						throw new InvalidOperationException(string.Format(Colosoft.Business.Properties.Resources.Exception_InvalidList, typeof(IList).FullName));
					var found = false;
					foreach (var i in list)
						if(this.Equals(i))
						{
							found = true;
							break;
						}
					if(!found)
						list.Add(this);
				}
			}
		}

		/// <summary>
		/// Método usado para criar o observer do registro.
		/// </summary>
		/// <returns></returns>
		protected virtual Colosoft.Query.IRecordObserver CreateRecordObserver()
		{
			return null;
		}

		/// <summary>
		/// Verifica se pode validar somente leitura na alteração de propriedades.
		/// </summary>
		/// <returns></returns>
		protected bool CanValidateReadOnlyOnChanging()
		{
			return !_readOnlyPropertyChangingCancelHandler.Busy;
		}

		/// <summary>
		/// Adiciona um observador ao objeto
		/// </summary>
		/// <param name="entity">objeto</param>
		/// <param name="newObserver">Novo observador</param>
		/// <returns>objeto com o novo observador</returns>
		public static Entity operator +(Entity entity, IEntityObserver newObserver)
		{
			if(entity._observer == null)
				entity._observer = new ObserverControl();
			entity._observer += newObserver;
			return entity;
		}

		/// <summary>
		/// Remove um observador do objeto
		/// </summary>
		/// <param name="entity">objeto</param>
		/// <param name="removeObserver">observador a ser removido</param>
		/// <returns>objeto sem o observador</returns>
		public static Entity operator -(Entity entity, IEntityObserver removeObserver)
		{
			if(entity._observer != null)
			{
				entity._observer -= removeObserver;
				if(!entity._observer.HasObserver)
					entity._observer = null;
			}
			return entity;
		}

		/// <summary>
		/// Sinaliza para o objeto que a initialização está començando.
		/// </summary>
		public void BeginInit()
		{
			_monitor.Enter();
		}

		/// <summary>
		/// Sinaliza que a inicialização foi completada.
		/// </summary>
		public void EndInit()
		{
			_monitor.Dispose();
		}

		/// <summary>
		/// Identifica se pode disparar oe ventos de notificação de alteração.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public bool RaisesItemChangedEvents
		{
			get
			{
				return !_monitor.Busy;
			}
		}

		/// <summary>
		/// Inicia a edição da entidade.
		/// </summary>
		public void BeginEdit()
		{
			if(!IsEditing)
			{
				InitInstance();
				_editingTransactionInstance = this.Clone() as IEntity;
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
				if(_editingTransactionInstance != null)
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
		/// Nome do tipo associado.
		/// </summary>
		IMessageFormattable INamedType.TypeName
		{
			get
			{
				return EntityTypeName;
			}
		}

		/// <summary>
		/// Nome do tipo associado.
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_monitor")]
		protected virtual void Dispose(bool disposing)
		{
			_isDisposed = true;
			UnregisterRecordObserver();
			if(_state != null && _state.IsValueCreated)
			{
				_state.Value.Dispose();
				_state = null;
			}
			if(_owner != null)
				_owner.PropertyChanged -= OwnerPropertyChanged;
			if(_cloneFrom != null)
			{
				_cloneFrom.Changed -= CloneFromChanged;
				_cloneFrom.PropertyChanged -= CloneFromPropertyChanged;
				_cloneFrom.AcceptedChanges -= CloneFromAcceptedChanges;
			}
			if(_references != null)
			{
				foreach (var i in _references.ToArray())
				{
					if(i.Value != null)
						i.Value.Dispose();
				}
				_references.Clear();
			}
			if(_instance != null)
				_instance.Dispose();
			_instance = null;
			UnregisterList(MyList);
			if(_loader != null)
			{
				foreach (var i in _loader.GetChildrenAccessors())
				{
					var child = i.Get(this);
					if(child != null && child.Owner == this)
					{
						if(child != null)
							child.Dispose();
						UnregisterChild(child);
					}
				}
				foreach (var i in _loader.GetLinksAccessors())
				{
					var link = i.Get(this);
					if(link != null)
						link.Dispose();
					if(link is IList)
						((IList)link).Clear();
				}
			}
			if(_propertyUpdates != null)
				_propertyUpdates.Clear();
			if(_singleChildRemoved != null)
			{
				foreach (var i in _singleChildRemoved)
					i.Value.Dispose();
				_singleChildRemoved.Clear();
			}
			if(PropertyChanged != null)
				foreach (var call in PropertyChanged.GetInvocationList())
					PropertyChanged -= (System.ComponentModel.PropertyChangedEventHandler)call;
			if(_editingTransactionInstance != null)
				_editingTransactionInstance.Dispose();
			_owner = null;
			_cloneFrom = null;
			_myList = null;
			_observer = null;
			_editingTransactionInstance = null;
			_sourceContext = null;
			if(_deleting != null)
				foreach (var i in _deleting.GetInvocationList())
					_deleting -= (EventHandler<EntityDeletingEventArgs>)i;
			if(_deleted != null)
				foreach (var i in _deleted.GetInvocationList())
					_deleted -= (EventHandler<EntityDeletedEventArgs>)i;
		}

		/// <summary>
		/// Cria um <see cref="IEntityDescriptor"/> da entidade.
		/// </summary>
		/// <returns></returns>
		public IEntityDescriptor CreateDescriptor()
		{
			return Loader.CreateEntityDescriptor(this);
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
			recordKey.Require("recordKey").NotNull();
			bool result = false;
			if((comparisonType & Query.RecordKeyComparisonType.Key) == Query.RecordKeyComparisonType.Key)
				result = recordKey.Key == _entityRecordKey;
			if((comparisonType & Query.RecordKeyComparisonType.RowVersion) == Query.RecordKeyComparisonType.RowVersion)
				result = result || RowVersion == recordKey.RowVersion;
			return result;
		}

		/// <summary>
		/// Compara as duas entidades informadas.
		/// </summary>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		/// <returns></returns>
		public static bool Equals(IEntity objA, IEntity objB)
		{
			return ((object.ReferenceEquals(objA, objB)) || (((objA != null) && (objB != null)) && objA.Equals(objB)));
		}

		/// <summary>
		/// Verifica se um objeto é igual ao outro
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(obj is IEntity)
				return this.Equals((IEntity)obj);
			return base.Equals(obj);
		}

		/// <summary>
		/// Recupera o hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Reseta o estado da instancia rejeitando as modificações.
		/// </summary>
		public void RejectChanges()
		{
			if(CloneFrom != null)
				CopyFrom(CloneFrom);
			else if(_instance != null)
			{
				CopyFrom(_instance);
				((IClearableChangedTracking)this).ClearChanges();
			}
		}

		/// <summary>
		/// Limpa as alterações da instancia.
		/// </summary>
		void IClearableChangedTracking.ClearChanges()
		{
			if(_changedProperties != null)
				_changedProperties.Clear();
			foreach (var i in _loader.GetChildrenAccessors())
			{
				var child = i.Get(this);
				if(child != null)
					child.ClearChanges();
			}
			foreach (var i in _loader.GetLinksAccessors())
			{
				var link = i.Get(this);
				if(link != null)
					link.ClearChanges();
			}
		}

		/// <summary>
		/// Ignora as alterações das propriedades informadas.
		/// </summary>
		/// <param name="propertyNames"></param>
		void IClearableChangedTracking.IgnoreChanges(params string[] propertyNames)
		{
			propertyNames.Require("propertyNames").NotNull();
			if(_changedProperties != null)
				foreach (var i in propertyNames)
					_changedProperties.Remove(i);
		}

		/// <summary>
		/// Confirma as alterações da instancia.
		/// </summary>
		public void AcceptChanges()
		{
			if(Loader.InnerInstanceSupport && _instance != null)
			{
				if(InstanceGetter == null)
					SetInstance((IEntity)this.Clone(), null);
			}
			_changedProperties = null;
			_isChangedAfterCloneToEdit = false;
			foreach (var i in _loader.GetChildrenAccessors())
			{
				var child = i.Get(this);
				if(child != null && child.IsChanged)
					child.AcceptChanges();
			}
			foreach (var i in _loader.GetLinksAccessors())
			{
				var link = i.Get(this);
				if(link != null && link.IsChanged)
					link.AcceptChanges();
			}
			var childRemoved = _singleChildRemoved.Select(f => f.Value).ToArray();
			_singleChildRemoved.Clear();
			foreach (var i in childRemoved)
				i.Dispose();
			UpdateMyList(true);
			OnAcceptChanges();
			RaisePropertyChanged("IsChangedAfterCloneToEdit");
		}

		/// <summary>
		/// Notifica os clientes sobre as alterações
		/// </summary>
		public void Flush()
		{
		}

		/// <summary>
		/// Recupera a instancia com os dados originais da entidade.
		/// </summary>
		/// <returns></returns>
		public IEntity GetOriginal()
		{
			return Instance;
		}

		/// <summary>
		/// Recupera o manipulador que cancela a velidação de somente leitura
		/// na alteração de propriedades.
		/// </summary>
		/// <returns></returns>
		public IDisposable CreateReadOnlyPropertyChangingCancelHandler()
		{
			_readOnlyPropertyChangingCancelHandler.Enter();
			return _readOnlyPropertyChangingCancelHandler;
		}

		/// <summary>
		/// Cria o objeto que controla a carga do objeto
		/// </summary>
		/// <returns></returns>
		public IDisposable CreateLoadHandle()
		{
			_monitor.Enter();
			return _monitor;
		}

		/// <summary>
		/// Copia os dados da entidade informada para a instancia,
		/// inclusives os dados de alteração da nova instancia..
		/// </summary>
		/// <param name="fromEntity">Instancia com os dados que serão copiados.</param>
		public virtual void CopyFrom(IEntity fromEntity)
		{
			_loader.Copy(fromEntity, this);
		}

		/// <summary>
		/// Cria um clone do objeto
		/// </summary>
		/// <returns>Objec clonado</returns>
		public virtual object Clone()
		{
			CheckDispose();
			var result = _loader.Clone(this);
			((Entity)result)._isReadOnly = this._isReadOnly;
			((Entity)result)._isFirstLoad = this._isFirstLoad;
			return result;
		}

		/// <summary>
		/// Salva a instancia.
		/// </summary>
		/// <returns></returns>
		public SaveResult Save()
		{
			var validateResult = this.Validate();
			if(validateResult.IsValid)
			{
				var result = OnSaving();
				if(!result.Success)
				{
					OnSaved(false, result.Message);
					return result;
				}
				UpdateMyList(false);
				OnSaved(true, null);
			}
			return new SaveResult(validateResult.IsValid, validateResult.Items.Select(f => f.Message).FirstOrDefault());
		}

		/// <summary>
		/// Salva os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns></returns>
		public abstract SaveResult Save(Data.IPersistenceSession session);

		/// <summary>
		/// Apaga os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns></returns>
		public abstract DeleteResult Delete(Data.IPersistenceSession session);

		/// <summary>
		/// Valida os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual Validation.ValidationResult Validate()
		{
			var result = new Validation.ValidationResult();
			Validate(ref result);
			var propertiesValidation = new Validation.ValidationResult();
			var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var state = this.InstanceState[property.Name];
				if(state != null)
				{
					var value = new Lazy<object>(() =>  {
						try
						{
							return property.GetValue(this, null);
						}
						catch(Exception ex)
						{
							if(ex is TargetInvocationException)
								throw ex.InnerException;
							throw;
						}
					});
					var input = new Lazy<string>(() =>  {
						return (value.Value != null) ? value.Value.ToString() : string.Empty;
					});
					if((state.IsRequired) || (state.IsNecessary))
					{
						if(!CheckValue(value.Value))
						{
							propertiesValidation.IsValid = false;
							propertiesValidation.Items.Add(new Validation.ValidationResultItem("RequiredAndNecessary" + property.Name, ResourceMessageFormatter.Create(() => Properties.Resources.Exception_ValueIsRquiredOrNecessaryToProperty, state.GetPropertyTitle()), Validation.ValidationResultType.Error));
						}
					}
					if(property.PropertyType.FullName.Equals("System.String") && (state.Mask != null) && (!String.IsNullOrEmpty(state.Mask.Mask)) && state.Mask.Mask.GetMask().IsIncompleteValue(input.Value))
					{
						propertiesValidation.IsValid = false;
						propertiesValidation.Items.Add(new Validation.ValidationResultItem("MaskFormat" + property.Name, ResourceMessageFormatter.Create(() => Properties.Resources.Exception_ValueNotProperlyFormatted, state.GetPropertyTitle()), Validation.ValidationResultType.Error));
					}
					if((state.Customization != null) && (state.Customization.CheckMethod != null))
					{
						var check = state.Customization.CheckMethod.Check(input.Value);
						if(!check.Success)
						{
							propertiesValidation.IsValid = false;
							propertiesValidation.Items.Add(new Validation.ValidationResultItem("InvalidByCustomization" + property.Name, (string.IsNullOrEmpty(check.Message) ? ResourceMessageFormatter.Create(() => Properties.Resources.Exception_InvalidCustomizationValue, state.GetPropertyTitle()) : ResourceMessageFormatter.Create(() => check.Message, state.GetPropertyTitle())), Validation.ValidationResultType.Error));
						}
					}
				}
			}
			result = result.Merge(propertiesValidation);
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
			if(!OnValidating(ref validationResult))
				return;
			foreach (var i in Loader.GetLinksAccessors())
			{
				var link = i.Get(this);
				if(link != null && link.IsChanged)
					validationResult.Merge(link.Validate());
			}
			foreach (var i in Loader.GetChildrenAccessors())
			{
				var child = i.Get(this);
				if(child != null)
					validationResult.Merge(child.Validate());
			}
			foreach (var i in Loader.GetReferences())
			{
				if(this.IsReferenceInitialized(i.Name))
				{
					var reference = i.ParentValueGetter(this);
					if(reference != null && reference.IsChanged)
						validationResult.Merge(reference.Validate());
				}
			}
			var validationManager = Validation.ValidationManager.Instance;
			validationManager.Validate(this, ref validationResult);
			OnValidated(ref validationResult);
		}

		/// <summary>
		/// Valida as propriedades informadas.
		/// </summary>
		/// <param name="propertyNames">Nomes das propriedades que serão validadas.</param>
		/// <returns></returns>
		public Validation.ValidationResult Validate(params string[] propertyNames)
		{
			propertyNames.Require("propertyNames").NotNull().NotEmptyCollection();
			var validationManager = Validation.ValidationManager.Instance;
			var result = new Validation.ValidationResult();
			var propNames = new List<string>(propertyNames);
			foreach (var i in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				var index = propNames.IndexOf(i.Name);
				if(index >= 0)
				{
					object value = i.GetValue(this, null);
					validationManager.ValidateProperty(this, i.Name, value, ref result);
					propNames.RemoveAt(index);
				}
				if(propNames.Count == 0)
					break;
			}
			foreach (var i in propNames)
			{
				result.IsValid = false;
				result.Items.Add(new Validation.ValidationResultItem {
					ResultType = Validation.ValidationResultType.Error,
					Message = ResourceMessageFormatter.Create((() => Properties.Resources.Entity_PropertyNotFound))
				});
			}
			return result;
		}

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <param name="token">token que irá bloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult Lock(string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			return Lock(new Colosoft.Lock.LockSession(), token, hostName, lockType, mainInLock, String.Empty);
		}

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <param name="session">Sessão do lock</param>
		/// <param name="token">token que irá bloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <param name="lockGroup">Grupo do lock</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult Lock(Colosoft.Lock.LockSession session, string token, string hostName, Lock.LockType lockType, bool mainInLock, string lockGroup)
		{
			if(LockProcessManager.LockProcess == null)
				return new LockProcessResult {
					Message = "LockProcess undefined",
					ProcessResult = LockProcessResultType.Error
				};
			var lockableObject = new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid);
			var result = LockProcessManager.LockProcess.Lock(session, token, hostName, lockableObject, (int)lockType, lockGroup);
			if(result.ProcessResult == LockProcessResultType.Success)
			{
				_transaction = session.SessionUid;
				if(!System.Threading.ThreadPool.QueueUserWorkItem(NotifyLock))
					NotifyLock(null);
			}
			this.IsLockedToEdit = lockableObject.IsLockedToEdit;
			return result;
		}

		/// <summary>
		/// Cria uma transação de lock.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string CreateLockTransaction(Colosoft.Enums.LockTransactionType type, string token)
		{
			CheckLockProcess();
			return LockProcessManager.LockProcess.CreateTransaction(type, token);
		}

		/// <summary>
		/// Registra um bloqueio.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="session"></param>
		/// <param name="hostName"></param>
		/// <param name="lockType"></param>
		/// <param name="mainInLock"></param>
		/// <param name="lockGroup"></param>
		/// <returns></returns>
		public virtual bool RegisterLock(string transaction, Colosoft.Lock.LockSession session, string hostName, Lock.LockType lockType, bool mainInLock, string lockGroup)
		{
			CheckLockProcess();
			var lockableObject = new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid);
			lockableObject.RealObject = this;
			return LockProcessManager.LockProcess.RegisterLock(transaction, session, hostName, lockableObject, (int)lockType, lockGroup);
		}

		/// <summary>
		/// Registra um desbloqueio.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="hostName"></param>
		/// <param name="lockType"></param>
		/// <param name="mainInLock"></param>
		/// <returns></returns>
		public virtual bool RegisterUnlock(string transaction, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			CheckLockProcess();
			var lockableObject = new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid);
			lockableObject.RealObject = this;
			return LockProcessManager.LockProcess.RegisterUnlock(transaction, hostName, lockableObject, (int)lockType);
		}

		/// <summary>
		/// Processa a transação de lock.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public virtual LockProcessResult ProcessLockTransaction(string transaction)
		{
			if(LockProcessManager.LockProcess == null)
				return new LockProcessResult {
					Message = "LockProcess undefined",
					ProcessResult = LockProcessResultType.Error
				};
			var result = LockProcessManager.LockProcess.ProcessTransaction(transaction);
			if(result.ProcessResult == LockProcessResultType.Success)
			{
				_transaction = transaction;
				if(!System.Threading.ThreadPool.QueueUserWorkItem(NotifyLock))
					NotifyLock(null);
			}
			return result;
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult UnLock(string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			CheckLockProcess();
			if(IsLockedToEdit)
			{
				var lockableObject = new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid);
				var result = LockProcessManager.LockProcess.UnLock(token, hostName, lockableObject, (int)lockType);
				if(!(result.ProcessResult == Colosoft.Lock.LockProcessResultType.Error))
				{
					this.IsLockedToEdit = false;
				}
				return result;
			}
			else
				return new LockProcessResult {
					ProcessResult = LockProcessResultType.Success
				};
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="groupSession">sessão do grupo.</param>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public virtual Lock.LockProcessResult UnLock(string groupSession, string token, string hostName, Lock.LockType lockType, bool mainInLock)
		{
			CheckLockProcess();
			var lockableObject = new Lock.Lockable(this.IsLockedToEdit, this.ModelType.ToString(), this.RowVersion, this.Uid);
			var result = LockProcessManager.LockProcess.UnLock(token, hostName, lockableObject, (int)lockType);
			if(!(result.ProcessResult == Colosoft.Lock.LockProcessResultType.Error))
			{
				_transaction = groupSession;
				new System.Threading.Thread(new System.Threading.ThreadStart(NotifyUnLock)).Start();
				this.IsLockedToEdit = false;
			}
			return result;
		}

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult Lock(Lock.LockType lockType = Colosoft.Lock.LockType.ToEdit, bool mainInLock = true)
		{
			return Lock(Colosoft.Security.UserContext.Current.Token, Environment.MachineName, lockType, mainInLock);
		}

		/// <summary>
		/// Mata um grupo de lock.
		/// </summary>
		/// <param name="lockGroup"></param>
		public virtual void KillLockGroup(string lockGroup)
		{
			CheckLockProcess();
			LockProcessManager.LockProcess.KillLockGroup(lockGroup);
			_transaction = lockGroup;
			new System.Threading.Thread(new System.Threading.ThreadStart(NotifyUnLock)).Start();
		}

		/// <summary>
		/// Valida o processo de lock.
		/// </summary>
		private static void CheckLockProcess()
		{
			if(LockProcessManager.LockProcess == null)
				throw new InvalidOperationException("LockProcess undefined");
		}

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <returns></returns>
		public Lock.LockProcessResult Lock()
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return Lock(userContext.Token, Environment.MachineName, Colosoft.Lock.LockType.ToEdit, true);
		}

		/// <summary>
		/// Realiza o lock da instancia.
		/// </summary>
		/// <param name="session">Instancia da sessão que deverá ser utilizada para o lock.</param>
		/// <returns></returns>
		public Lock.LockProcessResult Lock(Lock.LockSession session)
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return Lock(session, userContext.Token, Environment.MachineName, Colosoft.Lock.LockType.ToEdit, true, string.Empty);
		}

		/// <summary>
		/// Realiza o lock da instancia.
		/// </summary>
		/// <param name="session">Instancia da sessão que deverá ser utilizada para o lock.</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult Lock(Lock.LockSession session, Lock.LockType lockType, bool mainInLock)
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return Lock(session, userContext.Token, Environment.MachineName, lockType, mainInLock, string.Empty);
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="groupSession">Sessão do grupo.</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(string groupSession, Lock.LockType lockType = Colosoft.Lock.LockType.ToEdit, bool mainInLock = true)
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return UnLock(groupSession, userContext.Token, Environment.MachineName, lockType, mainInLock);
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(Lock.LockType lockType = Colosoft.Lock.LockType.ToEdit, bool mainInLock = true)
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return UnLock(userContext.Token, Environment.MachineName, lockType, mainInLock);
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock()
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return UnLock(userContext.Token, Environment.MachineName, Colosoft.Lock.LockType.ToEdit, true);
		}

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="groupSession">Sessão do grupo.</param>
		/// <returns></returns>
		public Lock.LockProcessResult UnLock(string groupSession)
		{
			var userContext = Colosoft.Security.UserContext.Current;
			if(userContext == null)
				throw new InvalidOperationException("UserContext undefined");
			return UnLock(groupSession, userContext.Token, Environment.MachineName, Colosoft.Lock.LockType.ToEdit, true);
		}

		/// <summary>
		/// Cria uma instância com a cópia da instância original para edição. 
		/// </summary>
		/// <returns></returns>
		public IEntity CloneToEdit()
		{
			CheckDispose();
			Entity result = (Entity)Clone();
			result.Owner = Owner;
			result.InitList(_myList);
			result.CloneFrom = this;
			if(Loader.InnerInstanceSupport)
				result.SetInstance(_instance, InstanceGetter);
			IsChangedAfterCloneToEdit = false;
			return result;
		}

		/// <summary>
		/// Registra na entidade que ela foi clonada para edição a partir da instancia informada.
		/// </summary>
		/// <param name="cloneFrom">Instancia de origem da entidade.</param>
		public void RegisterCloneToEdit(IEntity cloneFrom)
		{
			cloneFrom.Require("cloneFrom").NotNull();
			this.InitList(cloneFrom.MyList);
			this.CloneFrom = cloneFrom;
			if(Loader.InnerInstanceSupport)
			{
				if(cloneFrom is Entity)
				{
					var entity = (Entity)cloneFrom;
					this.SetInstance(entity._instance, entity.InstanceGetter);
				}
				else
				{
					IEntityInstanceGetter getter = cloneFrom is IEntityInstanceRegister ? ((IEntityInstanceRegister)cloneFrom).InstanceGetter : null;
					this.SetInstance(cloneFrom.Instance, getter);
				}
			}
			IsChangedAfterCloneToEdit = false;
		}

		/// <summary>
		/// Método acionado quando a instancia da origem do clone
		/// sofrer alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloneFromChanged(object sender, EventArgs e)
		{
			IsCloneFromChanged = true;
		}

		/// <summary>
		/// Método acionado quando alguma propriedade da instancia
		/// de origem do clone sofrer alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloneFromPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsLockedToEdit" || e.PropertyName == "CanEdit" || e.PropertyName == "IsLockedToMe")
				RaisePropertyChanged(e.PropertyName);
		}

		/// <summary>
		/// Método acionado quando a instancia da origem do clone receber uma aceito de alterações.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloneFromAcceptedChanges(object sender, EventArgs e)
		{
			if(!IsChangedAfterCloneToEdit)
			{
				_ignoreIsCloneFromChanged = true;
				try
				{
					this.CopyFrom(CloneFrom);
				}
				finally
				{
					_ignoreIsCloneFromChanged = false;
				}
				IsCloneFromChanged = false;
				IsChangedAfterCloneToEdit = false;
				this.AcceptChanges();
			}
		}

		/// <summary>
		/// Inicializa a lista.
		/// </summary>
		/// <param name="ownerList">Lista mãe da entidade</param>
		public void InitList(Colosoft.Collections.IObservableCollection ownerList)
		{
			if(_myList != null)
			{
				_myList.CollectionChanged -= MyListCollectionChanged;
				_myList.PropertyChanged -= MyListPropertyChanged;
			}
			var count = 20;
			while (ownerList is Collections.IFilteredObservableCollection && (count--) > 0)
				ownerList = ((Collections.IFilteredObservableCollection)ownerList).Source;
			_myList = ownerList;
			if((ownerList != null && ownerList is IEntity) && _owner != null)
			{
				_owner.PropertyChanged -= OwnerPropertyChanged;
				_owner = null;
			}
			if(_myList is IEntity)
			{
				_owner = ((IEntity)_myList).Owner;
				if(_owner != null)
					_owner.PropertyChanged += OwnerPropertyChanged;
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
		/// Define o <see cref="Validation.IStatebleItem.IsReadOnly"/> para o valor do parâmetro
		/// para a propriedade.
		/// </summary>
		/// <param name="propertyName">A propriedade a ser alterada.</param>
		/// <param name="value">O valor a atribuir ao campo IsReadOnly do estado.</param>
		public void SetReadOnlyState(string propertyName, bool value)
		{
			if(String.IsNullOrEmpty(propertyName))
			{
				return;
			}
			var state = this.InstanceState[propertyName];
			if(state == null)
			{
				return;
			}
			state.IsReadOnly = !value;
			state.IsReadOnly = value;
		}

		/// <summary>
		/// Registra o observer para o registro.
		/// </summary>
		/// <param name="recordKey">Chave do registro que será observado.</param>
		public virtual void RegisterObserver(Colosoft.Query.RecordKey recordKey)
		{
			recordKey.Require("record").NotNull();
			var manager = Colosoft.Query.RecordObserverManager.Instance;
			var modelType = ModelType;
			if(manager != null && manager.IsEnabled && modelType != null)
			{
				if(_recordObserver != null)
					UnregisterRecordObserver();
				var observer = CreateRecordObserver();
				manager.Register(Colosoft.Reflection.TypeName.Get(modelType), recordKey, observer);
				_recordObserver = observer;
			}
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
			this.RemoveStorageControl();
			foreach (var accessor in Loader.GetChildrenAccessors())
			{
				IEntity child = accessor.Get(this);
				if(child != null)
					child.RemoveAllStorageControl();
			}
		}

		/// <summary>
		/// Reseta todos os Uids da instancia e das instancias filhas.
		/// </summary>
		public virtual void ResetAllUids()
		{
			this.CloneFrom = null;
			Loader.ResetAllUids(this, TypeManager);
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
		[System.Xml.Serialization.XmlIgnore]
		IEntityInstanceGetter IEntityInstanceRegister.InstanceGetter
		{
			get
			{
				return _instanceGetter;
			}
		}

		/// <summary>
		/// Verifica se a instancia é igual a instancia informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool Equals(IEntity other)
		{
			if(other == null)
				return false;
			var type = this.GetType();
			if(other.GetType() == type)
			{
				if(_loader.HasUid)
					return this.Uid == other.Uid;
				else if(_loader.HasKeys)
				{
					var keys1 = _loader.GetInstanceKeysValues(this).GetEnumerator();
					var keys2 = _loader.GetInstanceKeysValues(other).GetEnumerator();
					while (keys1.MoveNext() && keys2.MoveNext())
					{
						var i1IsNull = object.ReferenceEquals(keys1.Current.Item2, null);
						var i2IsNull = object.ReferenceEquals(keys2.Current.Item2, null);
						if(!(keys1.Current.Item1 == keys2.Current.Item1 && ((i1IsNull && i2IsNull) || (!i1IsNull && keys1.Current.Item2.Equals(keys2.Current.Item2)))))
							return false;
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(HasUid)
				return string.Format("[{0}, Uid: {1}, RowVersion: {2}, IsLockedToEdit: {3}]", this.GetType().Name, Uid, RowVersion, IsLockedToEdit);
			return string.Format("[{0}, RowVersion: {1}, IsLockedToEdit: {2}]", this.GetType().Name, RowVersion, IsLockedToEdit);
		}

		/// <summary>
		/// Registra que a propriedade foi alterada.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		void IRegisterPropertyChanged.RegistrerPropertyChanged(string propertyName)
		{
			RaisePropertyChanged(propertyName);
		}

		/// <summary>
		/// Conecta a entidade com a origem de dados informada.
		/// </summary>
		/// <param name="sourceContext"></param>
		protected virtual void Connect(Query.ISourceContext sourceContext)
		{
			_sourceContext = sourceContext;
			foreach (var accessor in this.Loader.GetChildrenAccessors())
			{
				var value = accessor.Get(this) as IConnectedEntity;
				if(value != null && value.SourceContext == null)
					value.Connect(sourceContext);
			}
			foreach (var link in this.Loader.GetLinksAccessors())
			{
				var value = link.Get(this) as IConnectedEntity;
				if(value != null && value.SourceContext == null)
					value.Connect(sourceContext);
			}
		}

		/// <summary>
		/// Instancia do contexto da origem dos dados.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		Query.ISourceContext IConnectedEntity.SourceContext
		{
			get
			{
				return _sourceContext;
			}
		}

		/// <summary>
		/// Conecta a entidade com a origem de dados informada.
		/// </summary>
		/// <param name="sourceContext"></param>
		void IConnectedEntity.Connect(Query.ISourceContext sourceContext)
		{
			Connect(sourceContext);
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
		/// Identifica se pode serializa a instancia.
		/// </summary>
		protected virtual bool CanSerialize
		{
			get
			{
				return _canSerialize;
			}
			set
			{
				_canSerialize = value;
			}
		}

		/// <summary>
		/// Identifica se pode serializa a instancia.
		/// </summary>
		bool IEntityXmlSerializable.CanSerialize
		{
			get
			{
				return CanSerialize;
			}
		}

		/// <summary>
		/// Recupera o esquema a entidade.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera os dados serializados da entidade.
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
			EntityXmlSerializer.Serialize(writer, _entityTypeManager, this);
		}

		/// <summary>
		/// Representa um objeto qe controla a carga
		/// </summary>
		class EntityMonitor : Threading.SimpleMonitor
		{
			private List<IDisposable> _children;

			private IEntity _entity;

			/// <summary>
			/// Construtor com inicialialização da entidade
			/// </summary>
			/// <param name="entity">entidade que será carregada</param>
			public EntityMonitor(IEntity entity)
			{
				_entity = entity;
			}

			protected override void InnerEnter()
			{
				if(BusyCount == 0)
				{
					var items = _entity.Loader.GetChildrenAccessors().Select(f => f.Get(_entity)).Where(f => f is ILoadable).Select(f => (ILoadable)f);
					var aux = new List<ILoadable>();
					foreach (var i in items)
						if(i != _entity && !aux.Contains(i))
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
		/// Implementação da classe responsável por observar o registro associado com a entidade.
		/// </summary>
		internal abstract class EntityRecordObserver : Colosoft.Query.IRecordObserver, IDisposable, IDisposableState
		{
			private WeakReference _entityReference;

			private ulong _uid;

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
			/// Identifica se a instancia ainda está viva.
			/// </summary>
			public bool IsAlive
			{
				get
				{
					if(_entityReference != null && _entityReference.IsAlive)
					{
						var entity = _entityReference.Target;
						if(entity != null)
						{
							if(entity is IDisposableState && ((IDisposableState)entity).IsDisposed)
							{
								_entityReference = null;
								return false;
							}
							return _entityReference.IsAlive;
						}
					}
					return false;
				}
			}

			/// <summary>
			/// Instancia da entidade associada.
			/// </summary>
			protected Entity Owner
			{
				get
				{
					return _entityReference.Target as Entity;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entity">Instancia da entidade associada.</param>
			public EntityRecordObserver(Entity entity)
			{
				entity.Require("entity").NotNull();
				_uid = Query.RecordObserverUidGenerator.CreateUid();
				_entityReference = new WeakReference(entity);
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~EntityRecordObserver()
			{
				Dispose(false);
			}

			/// <summary>
			/// Instancia do modelo de dados associado.
			/// </summary>
			protected abstract object DataModel
			{
				get;
			}

			/// <summary>
			/// Notifica que os dados do registro que está sendo observado
			/// foram alterados.
			/// </summary>
			/// <param name="record"></param>
			public void OnChanged(Query.IRecord record)
			{
				if(!IsAlive)
					return;
				var entity = _entityReference.Target as Entity;
				if(entity == null)
					return;
				var descriptor = record.Descriptor;
				long rowVersion = 0;
				if(descriptor.Any(f => f.Name == "RowVersion"))
					rowVersion = record["RowVersion"].ToInt64();
				var bindingStrategy = entity.Loader.GetBindStrategy();
				var dataModel = DataModel;
				List<string> changeProperties = null;
				if(dataModel != null)
					changeProperties = bindingStrategy.Bind(record, Query.BindStrategyMode.Differences, ref dataModel).ToList();
				else
					changeProperties = new List<string>();
				if(rowVersion != 0 && !changeProperties.Contains("RowVersion"))
					changeProperties.Add("RowVersion");
				entity.RaisePropertyChanged2(false, changeProperties.ToArray());
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			/// <param name="disposing"></param>
			protected virtual void Dispose(bool disposing)
			{
				_entityReference = null;
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
			/// Identifica se a instancia está liberada.
			/// </summary>
			public bool IsDisposed
			{
				get
				{
					return !IsAlive;
				}
			}
		}

		/// <summary>
		/// Classe usada pare recupera o Instance para uma entidade.
		/// </summary>
		class EntityInstanceGetter : IEntityInstanceGetter
		{
			private IEntity _owner;

			private IEntityAccessor _accessor;

			/// <summary>
			/// Identifica se a instância é valida.
			/// </summary>
			public bool IsValid
			{
				get
				{
					var disposableState = _owner as IDisposableState;
					return disposableState == null || disposableState.IsDisposed;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner">Instancia do pai.</param>
			/// <param name="accessor">Instancia do método usado para recupera o Instance.</param>
			public EntityInstanceGetter(IEntity owner, IEntityAccessor accessor)
			{
				_owner = owner;
				_accessor = accessor;
			}

			/// <summary>
			/// Recupera o instancia da entidade.
			/// </summary>
			/// <returns></returns>
			public IEntity GetInstance()
			{
				return _accessor.Get(_owner.Instance);
			}
		}
	}
}
