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

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação da lista de entidades de link.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class EntityLinksList<TEntity> : BaseEntityList<TEntity>, IEntityLinksList<TEntity>, IEntityLink where TEntity : IEntity
	{
		private IEntityList _child;

		private EntityLoaderLinkInfo _linkInfo;

		private EntityFromModelCreatorHandler _childFromModelCreator;

		private bool _isLazy;

		/// <summary>
		/// Lista onde são adicionados os itens dos filho para serem ignorados no evento 
		/// de alteração da coleção de filhos.
		/// </summary>
		private List<IEntity> _ignoreChildItems = new List<IEntity>();

		/// <summary>
		/// Identifica se a entidade possui Uid.
		/// </summary>
		private static bool? _hasUid;

		/// <summary>
		/// Indica que a instância está locada para edição.
		/// </summary>
		public override bool IsLockedToEdit
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="linkInfo">Informações do link.</param>
		/// <param name="childFromModelCreator">Instancia do método usado para criar a entidade do filho associado com o link.</param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter">Método usado para definir o identificador unico do pai para as entidades filas da lista.</param>
		/// <param name="sourceContext">Contexto de origem de consultas.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		public EntityLinksList(IEntityList child, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, string uiContext, Action<TEntity> parentUidSetter, Colosoft.Query.ISourceContext sourceContext = null, IEntityTypeManager entityTypeManager = null) : base(uiContext, parentUidSetter, entityTypeManager)
		{
			Initialize(child, linkInfo, childFromModelCreator, sourceContext);
		}

		/// <summary>
		/// Cria uma instancia já definindos os itens iniciais.
		/// </summary>
		/// <param name="items">Itens que serão usados na inicialização.</param>
		/// <param name="linkInfo">Informações do link.</param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="sourceContext">Contexto de origem de consultas.</param>
		/// <param name="entityTypeManager"></param>
		public EntityLinksList(IEnumerable<TEntity> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Action<TEntity> parentUidSetter, Colosoft.Query.ISourceContext sourceContext = null, IEntityTypeManager entityTypeManager = null) : base(items, uiContext, parentUidSetter, entityTypeManager)
		{
			Initialize(child, linkInfo, childFromModelCreator, sourceContext);
		}

		/// <summary>
		/// Cria uma instancia definindo o parametro de carga tardia dos itens.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="linkInfo">Informações do link.</param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="sourceContext">Contexto de origem de consultas.</param>
		/// <param name="entityTypeManager"></param>
		public EntityLinksList(Lazy<IEnumerable<TEntity>> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Action<TEntity> parentUidSetter, Colosoft.Query.ISourceContext sourceContext = null, IEntityTypeManager entityTypeManager = null) : base(items, uiContext, parentUidSetter, entityTypeManager)
		{
			Initialize(child, linkInfo, childFromModelCreator, sourceContext);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="child">Instancia do filho associado.</param>
		/// <param name="linkInfo">Informações do link.</param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="sourceContext">Contexto de origem de consultas.</param>
		private void Initialize(IEntityList child, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, Colosoft.Query.ISourceContext sourceContext)
		{
			child.Require("child").NotNull();
			linkInfo.Require("linkInfo").NotNull();
			_child = child;
			_linkInfo = linkInfo;
			_childFromModelCreator = childFromModelCreator;
			((IConnectedEntity)this).Connect(sourceContext);
			if(_hasUid == null)
				_hasUid = TypeManager.HasUid(typeof(TEntity));
			if(child is Collections.INotifyCollectionChangedDispatcher)
				((Collections.INotifyCollectionChangedDispatcher)child).AddCollectionChanged(ChildCollectionChanged, Collections.NotifyCollectionChangedDispatcherPriority.High);
			else
				child.CollectionChanged += ChildCollectionChanged;
		}

		/// <summary>
		/// Método acioando quando a coleção adaptada sofrer alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChildCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var collection = sender as Colosoft.Collections.IObservableCollection<IEntity>;
			if(collection is IDisposableState && ((IDisposableState)collection).IsDisposed)
			{
				_child.CollectionChanged -= ChildCollectionChanged;
				return;
			}
			switch(e.Action)
			{
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				for(int i = 0; i < e.NewItems.Count; i++)
				{
					var entity = e.NewItems[i] as IEntity;
					if(entity == null)
						continue;
					lock (_ignoreChildItems)
						if(_ignoreChildItems.Where(f => f.Equals(entity)).Any())
							continue;
					if(SourceContext == null)
						throw new InvalidOperationException(string.Format("SourceContext undefined for create link for entity '{0}'", typeof(TEntity).FullName));
					var entityOfModel = entity as IEntityOfModel;
					if(entityOfModel != null && this.Count > (e.NewStartingIndex + i))
					{
						IEntityOfModel linkInstance = this[e.NewStartingIndex + i] as IEntityOfModel;
						if(_linkInfo.Equals(linkInstance.DataModel, entityOfModel.DataModel))
							continue;
					}
					var item = (TEntity)_linkInfo.LinkCreator(new LinkCreatorArgs(SourceContext, ((IEntityOfModel)entity).DataModel, _isLazy, UIContext));
					if(item == null)
						throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLinksList_CouldNotCreateTheEntityLink, _linkInfo.Name, entity.GetType().FullName, entity.HasFindName ? entity.FindName : entity.Uid.ToString()).Format());
					base.Insert(e.NewStartingIndex + i, item);
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
				if(e.OldItems.Count == 1)
					this.Move(e.OldStartingIndex, e.NewStartingIndex);
				else
				{
					var items = this.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
					for(int i = 0; i < e.OldItems.Count; i++)
						this.RemoveAt(e.OldStartingIndex);
					for(int i = 0; i < items.Count; i++)
						this.Insert(e.NewStartingIndex + i, items[i]);
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				for(int i = 0; i < e.OldItems.Count; i++)
					if(e.OldStartingIndex < this.Count)
						this.RemoveAt(e.OldStartingIndex);
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				for(int i = 0; i < e.OldItems.Count; i++)
					if(e.OldStartingIndex < this.Count)
						this.RemoveAt(e.OldStartingIndex);
				goto case System.Collections.Specialized.NotifyCollectionChangedAction.Add;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				Clear();
				if(e.NewItems != null)
					for(int i = 0; i < e.NewItems.Count; i++)
					{
						var entity = e.NewItems[i] as IEntity;
						if(entity == null)
							continue;
						lock (_ignoreChildItems)
							if(_ignoreChildItems.Where(f => f.Equals(entity)).Any())
								continue;
						this.Add((TEntity)_linkInfo.LinkCreator(new LinkCreatorArgs(SourceContext, ((IEntityOfModel)entity).DataModel, _isLazy, UIContext)));
					}
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Adiciona um item do filho com base na entidade do link.
		/// </summary>
		/// <param name="item"></param>
		private void AddChildItemFromLinkEntity(TEntity item)
		{
			var childItem = _linkInfo.EntityFromLinkCreator(new EntityFromLinkCreatorArgs(item, _childFromModelCreator, UIContext, TypeManager, SourceContext));
			lock (_ignoreChildItems)
				_ignoreChildItems.Add(childItem);
			((System.Collections.IList)_child).Add(childItem);
			lock (_ignoreChildItems)
				_ignoreChildItems.Remove(childItem);
		}

		/// <summary>
		/// Remove o item do filho associado com a entidade do link.
		/// </summary>
		/// <param name="linkEntity">Instancia da entidade do link</param>
		private void RemoveChildItem(TEntity linkEntity)
		{
			if(!(linkEntity is IEntityOfModel))
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLinkList_ExpectedEntityOfModel).Format(), "linkEntity");
			var linkDataModel = ((IEntityOfModel)linkEntity).DataModel;
			IEntity childItem = null;
			foreach (IEntityOfModel i in _child)
			{
				if(_linkInfo.Equals(linkDataModel, i.DataModel))
				{
					childItem = i;
					break;
				}
			}
			if(childItem != null)
			{
				var childList = (System.Collections.IList)_child;
				if(!(_child is Colosoft.Threading.IReentrancyController) || !((Colosoft.Threading.IReentrancyController)_child).IsReentrancy)
					childList.Remove(childItem);
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override object Clone()
		{
			return Clone(_child);
		}

		/// <summary>
		/// Clona a instancia da entidade link.
		/// </summary>
		/// <param name="child">Instancia do filho associado.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public object Clone(IEntity child)
		{
			return Clone(child, true);
		}

		/// <summary>
		/// Clona a instancia da entidade link.
		/// </summary>
		/// <param name="child">Instancia do filho associado.</param>
		/// <param name="forceCloneLazyChildren">Identifica se é para força os clone dos filhos com carga tardia.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public object Clone(IEntity child, bool forceCloneLazyChildren)
		{
			if(base.IsDisposed)
				throw new ObjectDisposedException("EntityLiksList");
			child.Require("child").NotNull();
			var child2 = child as IEntityList;
			if(child2 == null)
				throw new InvalidOperationException(string.Format("Child '{0}' is not IEntityList", child.GetType().FullName));
			EntityLinksList<TEntity> result = null;
			result = new EntityLinksList<TEntity>(new Collections.NotifyCollectionChangedObserverRegisterEnumerable<TEntity>(this, f => (f is IDisposableState && ((IDisposableState)f).IsDisposed) ? (TEntity)f : (TEntity)((ICloneable)f).Clone()), _linkInfo, _childFromModelCreator, child2, UIContext, ParentUidSetter, SourceContext, TypeManager);
			CloneControls(result);
			((IConnectedEntity)result).Connect(((IConnectedEntity)this).SourceContext);
			return result;
		}

		/// <summary>
		/// Recupera um controle de estado para gerenciar a operação de CopyFrom
		/// que a instancia está passando.
		/// </summary>
		/// <returns></returns>
		IDisposable IEntityLink.GetCopyFromStateControl()
		{
			_child.CollectionChanged -= ChildCollectionChanged;
			return new CopyFromControl(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_linkInfo = null;
			_child = null;
			_childFromModelCreator = null;
			_ignoreChildItems.Clear();
		}

		/// <summary>
		/// Verifica se o item já existe na coleção.
		/// </summary>
		/// <param name="item"></param>
		protected override void CheckExists(TEntity item)
		{
		}

		/// <summary>
		/// Fixa o item na coleção.
		/// </summary>
		/// <param name="item"></param>
		protected override void FixItem(TEntity item)
		{
			if(EntityTypeOfListHasUid && item.Uid == 0)
				item.Uid = TypeManager.GenerateInstanceUid(typeof(TEntity));
			item.InitList(this);
		}

		/// <summary>
		/// Adiciona um novo item na coleção.
		/// </summary>
		/// <param name="item"></param>
		public override void Add(TEntity item)
		{
			AddChildItemFromLinkEntity(item);
			base.Add(item);
		}

		/// <summary>
		/// Insere um novo item na coleção.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public override void Insert(int index, TEntity item)
		{
			AddChildItemFromLinkEntity(item);
			base.Insert(index, item);
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Remove(TEntity item)
		{
			if(Contains(item))
				RemoveChildItem(item);
			return base.Remove(item);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public override void RemoveAt(int index)
		{
			var item = this[index];
			RemoveChildItem(item);
			base.RemoveAt(index);
		}

		/// <summary>
		/// Processa as operações de Update.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override SaveResult ProcessUpdateOperations(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
			var newItems = GetNewItems().ToArray();
			foreach (var i in GetChangedItems())
			{
				if(i.IsChanged && !newItems.Contains(i, EntityEqualityComparer<IEntity>.Instance))
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
		protected override SaveResult ProcessInsertOperations(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
			var newItems = GetNewItems().ToArray();
			foreach (var i in newItems)
			{
				if(i.IsChanged)
				{
					var result = i.Save(session);
					if(!result.Success)
						return result;
				}
			}
			return new SaveResult(true, null);
		}

		/// <summary>
		/// Processa as operações de delete.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override DeleteResult ProcessDeleteOperations(Data.IPersistenceSession session)
		{
			return new DeleteResult(true, null);
		}

		/// <summary>
		/// Salva os dados da entidade na sessão informada.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override SaveResult Save(Data.IPersistenceSession session)
		{
			if(LoadItems != null)
				return new SaveResult(true, null);
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
		/// Cria um nova instancia da lista.
		/// Esse método é usado por reflexão.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="linkInfo"></param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="sourceContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <returns></returns>
		internal static EntityLinksList<TEntity> CreateInstance(IEnumerable<IEntity> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Action<IEntity> parentUidSetter, Colosoft.Query.ISourceContext sourceContext, IEntityTypeManager entityTypeManager)
		{
			return new EntityLinksList<TEntity>(new Collections.NotifyCollectionChangedObserverRegisterEnumerable<TEntity>(items), linkInfo, childFromModelCreator, child, uiContext, new Action<TEntity>(e => parentUidSetter(e)), sourceContext, entityTypeManager);
		}

		/// <summary>
		/// Cria um nova instancia da lista.
		/// Esse método é usado por reflexão.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="linkInfo"></param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="sourceContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <returns></returns>
		internal static EntityLinksList<TEntity> CreateLazyInstance(Lazy<IEnumerable<IEntity>> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Action<IEntity> parentUidSetter, Colosoft.Query.ISourceContext sourceContext, IEntityTypeManager entityTypeManager)
		{
			return new EntityLinksList<TEntity>(new Lazy<IEnumerable<TEntity>>(() => new Collections.NotifyCollectionChangedObserverRegisterEnumerable<TEntity>(items.Value)), linkInfo, childFromModelCreator, child, uiContext, new Action<TEntity>(e => parentUidSetter(e)), sourceContext, entityTypeManager) {
				_isLazy = true
			};
		}

		/// <summary>
		/// Classe usada para confirmar a liberação da operação de CopyFrom da lista de links.
		/// </summary>
		sealed class CopyFromControl : IDisposable
		{
			private EntityLinksList<TEntity> _linkList;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="linkList"></param>
			public CopyFromControl(EntityLinksList<TEntity> linkList)
			{
				_linkList = linkList;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_linkList._child.CollectionChanged += _linkList.ChildCollectionChanged;
			}
		}
	}
}
