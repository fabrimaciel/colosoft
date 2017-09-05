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
	/// Representa a implementação de uma entidade para um modelo de dados.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public abstract class Entity<Model> : Entity, IEntity<Model>, IPersistence, IDeleteOperationsContainer where Model : class, Colosoft.Data.IModel, new()
	{
		private Model _dataModel;

		[NonSerialized]
		private int _lastActionId = 0;

		/// <summary>
		/// Identificador da última ação da sessão de persistencia vinculada a entidade.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public int LastActionId
		{
			get
			{
				return _lastActionId;
			}
		}

		/// <summary>
		/// Instancia do modelo de dados associado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual Model DataModel
		{
			get
			{
				return _dataModel;
			}
		}

		/// <summary>
		/// Versão da instância.
		/// </summary>
		public override long RowVersion
		{
			get
			{
				if(DataModel is Colosoft.Data.IVersionedModel)
				{
					return ((Colosoft.Data.IVersionedModel)DataModel).RowVersion;
				}
				return base.RowVersion;
			}
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public override Type ModelType
		{
			get
			{
				return typeof(Model);
			}
		}

		/// <summary>
		/// Verifica se a entidade existe na fonte de armazenamento.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public override bool ExistsInStorage
		{
			get
			{
				if(DataModel is Colosoft.Data.IStorageControl)
					return ((Colosoft.Data.IStorageControl)DataModel).ExistsInStorage;
				return base.ExistsInStorage;
			}
		}

		/// <summary>
		/// Cria uma instancia com base no contexto visual.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Entity(Model dataModel, string uiContext, IEntityTypeManager entityTypeManager = null) : this(dataModel, uiContext, true, entityTypeManager)
		{
		}

		/// <summary>
		/// Construtor interno
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="uiContext"></param>
		/// <param name="initialize">Identifica se é para inicializar.</param>
		/// <param name="entityTypeManager"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected Entity(Model dataModel, string uiContext, bool initialize, IEntityTypeManager entityTypeManager) : base(uiContext, entityTypeManager)
		{
			Initialize(initialize);
			if(dataModel == null)
			{
				SetDataModel(new Model());
				InitializeModel(DataModel);
			}
			else
				SetDataModel(dataModel);
			var entityType = this.GetType();
			if(TypeManager.HasUid(entityType) && Uid == 0)
				Uid = TypeManager.GenerateInstanceUid(entityType);
			UpdateRecordKey();
		}

		/// <summary>
		/// Inicializa o modelo para a entidade.
		/// </summary>
		/// <returns></returns>
		protected Model InitializeModel(Model instance)
		{
			var state = InstanceState;
			if(state == null)
				return instance;
			foreach (var outData in state.ToArray())
			{
				var data = state[outData.PropertyName];
				if((data != null) && (data.DefaultValue != null) && (!String.IsNullOrEmpty(data.DefaultValue.DefaultValue)))
				{
					var info = typeof(Model).GetProperty(data.PropertyName);
					if(info == null)
					{
						continue;
					}
					if(info.PropertyType.IsEnum)
						info.SetValue(instance, Enum.Parse(info.PropertyType, data.DefaultValue.DefaultValue), null);
					else if(typeof(IConvertible).IsAssignableFrom(info.PropertyType))
						info.SetValue(instance, Convert.ChangeType(data.DefaultValue.DefaultValue, info.PropertyType), null);
					else if(typeof(DateTimeOffset).IsAssignableFrom(info.PropertyType))
						info.SetValue(instance, DateTimeOffset.Parse(data.DefaultValue.DefaultValue), null);
				}
			}
			return instance;
		}

		/// <summary>
		/// Recupera a instancia do link da entidade.
		/// </summary>
		/// <typeparam name="TLink"></typeparam>
		/// <param name="linkContainer"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected IEntityLinksList<TLink> GetLink<TLink>(IEntityLoaderLinksContainer linkContainer, string name) where TLink : IEntity
		{
			var link = linkContainer.Get<TLink>(name);
			RegisterLink(link);
			return link;
		}

		/// <summary>
		/// Recupera a instancia do filho da entidade.
		/// </summary>
		/// <typeparam name="TLink"></typeparam>
		/// <param name="linkContainer"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected TLink GetSingleLink<TLink>(IEntityLoaderLinksContainer linkContainer, string name) where TLink : IEntity
		{
			var link = linkContainer.GetSingle<TLink>(name);
			RegisterLink(link);
			return link;
		}

		/// <summary>
		/// Recupera a instancia do filho da entidade.
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="childContainer"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected IEntityChildrenList<TChild> GetChild<TChild>(EntityLoaderChildContainer childContainer, string name) where TChild : IEntity
		{
			var child = childContainer.Get<TChild>(name);
			RegisterChild(child);
			return child;
		}

		/// <summary>
		/// Recupera a instancia do filho da entidade.
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="childContainer"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected TChild GetSingleChild<TChild>(EntityLoaderChildContainer childContainer, string name) where TChild : IEntity
		{
			var child = childContainer.GetSingle<TChild>(name);
			RegisterChild(child);
			return child;
		}

		/// <summary>
		/// Cria o observer para a entidade.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		protected override Query.IRecordObserver CreateRecordObserver()
		{
			return new EntityOfModelRecordObserver(this);
		}

		/// <summary>
		/// Método acionado quando a persistencia for executada.
		/// </summary>
		/// <param name="result"></param>
		protected virtual void OnPersisted(Colosoft.Data.PersistenceActionResult result)
		{
		}

		/// <summary>
		/// Define o modelo de dados da entidade.
		/// </summary>
		/// <param name="dataModel"></param>
		private void SetDataModel(Model dataModel)
		{
			dataModel.Require("dataModel").NotNull();
			_dataModel = dataModel;
		}

		/// <summary>
		/// Realiza uma cópia dos dados básicos da entidade.
		/// </summary>
		/// <param name="to"></param>
		public virtual void CopyBasicData(IEntity<Model> to)
		{
			to.Require("to").NotNull();
			Connect(this, to);
			var dataModel = (Model)this.DataModel.Clone();
			to.CopyFromDataModel(dataModel);
			to.ResetAllUids();
		}

		/// <summary>
		/// Remove o controle de armazenamento da entidade.
		/// </summary>
		public override void RemoveStorageControl()
		{
			if(DataModel is Colosoft.Data.IStorageControl)
				((Colosoft.Data.IStorageControl)DataModel).ExistsInStorage = false;
		}

		/// <summary>
		/// Verifica se a instancia é igual a instancia informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(IEntity other)
		{
			var other2 = other as IEntity<Model>;
			return (other2 != null && this.Equals(other2));
		}

		/// <summary>
		/// Altera o valor da instancia do modelo dados.
		/// </summary>
		/// <param name="dataModel"></param>
		public virtual void CopyFromDataModel(Model dataModel)
		{
			CopyFromDataModel(dataModel, true);
		}

		/// <summary>
		/// Altera o valor da instancia do modelo dados.
		/// </summary>
		/// <param name="dataModel">Instancia com os novos dados.</param>
		/// <param name="notifyPropertyChanged">Identifica se é para notificar que as propriedades foram alteradas.</param>
		public virtual void CopyFromDataModel(Model dataModel, bool notifyPropertyChanged)
		{
			dataModel.Require("dataModel").NotNull();
			var clone = (Model)dataModel.Clone();
			if(notifyPropertyChanged)
			{
				var propertyInfos = typeof(Model).GetProperties().Where(f => f.CanRead).Intersect(this.GetType().GetProperties().Where(f => f.CanRead && f.CanWrite), PropertyInfoNameTypeIEqualityComparer.Instance);
				var properties = new List<string>();
				foreach (var info in propertyInfos)
				{
					var value1 = info.GetValue(DataModel, null);
					var value2 = info.GetValue(clone, null);
					if(info.PropertyType.IsNullable())
					{
						var propertyType = Nullable.GetUnderlyingType(info.PropertyType);
						if(typeof(IComparable).IsAssignableFrom(propertyType))
						{
							var comparer = typeof(Comparer<>).MakeGenericType(propertyType).GetProperty("Default", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null) as System.Collections.IComparer;
							if(comparer.Compare(value1, value2) != 0)
								properties.Add(info.Name);
						}
					}
					else if(typeof(IComparable).IsAssignableFrom(info.PropertyType))
					{
						if(System.Collections.Comparer.Default.Compare(value1, value2) != 0)
							properties.Add(info.Name);
					}
					else if(value1 is byte[] && value2 is byte[])
					{
						var buffer1 = (byte[])value1;
						var buffer2 = (byte[])value2;
						for(var i = 0; i < buffer1.Length; i++)
						{
							if(buffer1[i] != buffer2[i])
							{
								properties.Add(info.Name);
								break;
							}
						}
					}
					else if(!object.ReferenceEquals(value1, value2))
						properties.Add(info.Name);
				}
				SetDataModel(clone);
				if(properties.Count > 0)
					RaisePropertyChanged(properties.ToArray());
			}
			else
				SetDataModel(clone);
			UpdateRecordKey();
		}

		/// <summary>
		/// Método acionado quando o modelo de dados for salvo.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="result"></param>
		private void SaveActionCallback(Colosoft.Data.PersistenceAction action, Colosoft.Data.PersistenceActionResult result)
		{
			if(result.Success)
			{
				this.AcceptChanges();
				if(DataModel is Colosoft.Data.IStorageControl)
					((Colosoft.Data.IStorageControl)DataModel).ExistsInStorage = true;
				RaisePropertyChanged("RowVersion");
			}
			OnPersisted(result);
			OnSaved(result.Success, result.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Salva os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		public override SaveResult Save(Data.IPersistenceSession session)
		{
			var validateResult = Validate();
			if(!validateResult.IsValid)
				return new SaveResult(false, validateResult.Items.FirstOrDefault().Message);
			return Persist(session);
		}

		/// <summary>
		///  Recupera as entidade pelos getters informados.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="accessors"></param>
		/// <returns></returns>
		private static IEnumerable<IEntity> GetEntities(IEntity parent, IEnumerable<IEntityAccessor> accessors)
		{
			return accessors.Select(f => f.Get(parent)).Where(f => f != null);
		}

		/// <summary>
		/// Processa as operações de delete.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public DeleteResult ProcessDeleteOperations(Colosoft.Data.IPersistenceSession session)
		{
			var beforeLinksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.BeforeEntity);
			var beforeChildrenAccessors = Loader.GetChildrenAccessors(EntityChildSavePriority.BeforeEntity);
			var linksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.AfterEntity);
			var childrenAccessors = Loader.GetChildrenAccessors(EntityChildSavePriority.AfterEntity);
			var saveOperationsContainers = GetEntities(this, linksAccessors.Reverse().Concat(beforeLinksAccessors.Reverse())).Where(f => f is IDeleteOperationsContainer).Select(f => (IDeleteOperationsContainer)f);
			DeleteResult deleteResult = null;
			foreach (var i in childrenAccessors.Reverse())
			{
				var entityRemoved = GetSingleChildRemoved(i.Name);
				if(entityRemoved != null)
				{
					deleteResult = entityRemoved.Delete(session);
					if(!deleteResult.Success)
						return deleteResult;
					continue;
				}
				var entity = i.Get(this);
				if(entity is IDeleteOperationsContainer)
				{
					deleteResult = ((IDeleteOperationsContainer)entity).ProcessDeleteOperations(session);
					if(!deleteResult)
						return deleteResult;
				}
			}
			foreach (var i in beforeChildrenAccessors.Reverse())
			{
				var entityRemoved = GetSingleChildRemoved(i.Name);
				if(entityRemoved != null)
				{
					deleteResult = entityRemoved.Delete(session);
					if(!deleteResult.Success)
						return deleteResult;
					continue;
				}
				var entity = i.Get(this);
				if(entity is IDeleteOperationsContainer)
				{
					deleteResult = ((IDeleteOperationsContainer)entity).ProcessDeleteOperations(session);
					if(!deleteResult)
						return deleteResult;
				}
			}
			foreach (var entity in saveOperationsContainers)
			{
				deleteResult = entity.ProcessDeleteOperations(session);
				if(!deleteResult.Success)
					return deleteResult;
			}
			return new DeleteResult(true, null);
		}

		/// <summary>
		/// Persiste a entidade no banco de dados.
		/// </summary>
		/// <param name="session"></param>
		public virtual SaveResult Persist(Data.IPersistenceSession session)
		{
			if(!IsChanged && ExistsInStorage)
				return new SaveResult(true, null);
			var result = OnSaving();
			if(!result.Success)
				return result;
			var actionId = 0;
			DeleteResult deleteResult = null;
			Data.IPersistenceSession beforeSession = null;
			Data.IPersistenceSession afterSession = null;
			if(!ExistsInStorage)
				actionId = session.Insert(DataModel, SaveActionCallback);
			else
			{
				var changeProperties = ChangedProperties.ToArray();
				if(changeProperties.Length > 0)
					actionId = session.Update(DataModel, SaveActionCallback, changeProperties);
			}
			_lastActionId = actionId;
			if(actionId > 0)
			{
				beforeSession = session.CreateBeforeSessionForAction(actionId);
				afterSession = session.CreateAfterSessionForAction(actionId);
			}
			else
			{
				beforeSession = session;
				afterSession = session;
			}
			Colosoft.Domain.DomainEvents.Instance.GetEvent<EntityPersistingEvent>().Publish(new EntityPersistingEventArgs(this, session, beforeSession, afterSession));
			var references = Loader.GetReferences().Where(f => this.IsReferenceInitialized(f.Name));
			var beforeLinksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.BeforeEntity);
			var beforeChildrenAccessors = Loader.GetChildrenAccessors(EntityChildSavePriority.BeforeEntity);
			var linksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.AfterEntity);
			var childrenAccessors = Loader.GetChildrenAccessors(EntityChildSavePriority.AfterEntity);
			var deleteProcessing = false;
			if(!session.Root.Parameters.Contains("DeleteOperationsProcessing"))
			{
				session.Root.Parameters["DeleteOperationsProcessing"] = true;
				deleteProcessing = true;
				deleteResult = ProcessDeleteOperations(beforeSession);
				if(!deleteResult.Success)
					return new SaveResult(deleteResult.Success, deleteResult.Message);
			}
			foreach (var i in beforeLinksAccessors)
			{
				var entity = i.Get(this);
				if(entity != null)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(beforeSession);
						if(result.Success)
							result = container.ProcessInsertOperations(beforeSession);
					}
					else
						result = entity.Save(beforeSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(beforeSession);
				}
			}
			foreach (var i in beforeChildrenAccessors)
			{
				var entity = i.Get(this);
				var entityRemoved = GetSingleChildRemoved(i.Name);
				if(entityRemoved != null)
				{
					deleteResult = entityRemoved.Delete(beforeSession);
					if(!deleteResult.Success)
						return new SaveResult(deleteResult.Success, deleteResult.Message);
				}
				if(entity != null)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(beforeSession);
						if(result.Success)
							result = container.ProcessInsertOperations(beforeSession);
					}
					else
						result = entity.Save(beforeSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(beforeSession);
				}
			}
			foreach (var i in references.Where(f => f.SavePriority == EntityChildSavePriority.BeforeEntity))
			{
				var entity = i.ParentValueGetter(this);
				if(entity != null && entity.IsChanged)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(beforeSession);
						if(result.Success)
							result = container.ProcessInsertOperations(beforeSession);
					}
					else
						result = entity.Save(beforeSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(beforeSession);
				}
			}
			foreach (var i in linksAccessors)
			{
				var entity = i.Get(this);
				if(entity != null)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(afterSession);
						if(result.Success)
							result = container.ProcessInsertOperations(afterSession);
					}
					else
						result = entity.Save(afterSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(afterSession);
				}
			}
			foreach (var i in childrenAccessors)
			{
				var entity = i.Get(this);
				var entityRemoved = GetSingleChildRemoved(i.Name);
				if(entityRemoved != null)
				{
					deleteResult = entityRemoved.Delete(afterSession);
					if(!deleteResult.Success)
						return new SaveResult(deleteResult.Success, deleteResult.Message);
				}
				if(entity != null)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(afterSession);
						if(result.Success)
							result = container.ProcessInsertOperations(afterSession);
					}
					else
						result = entity.Save(afterSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(afterSession);
				}
			}
			foreach (var i in references.Where(f => f.SavePriority == EntityChildSavePriority.AfterEntity))
			{
				var entity = i.ParentValueGetter(this);
				if(entity != null && entity.IsChanged)
				{
					if(entity is ISaveOperationsContainer)
					{
						var container = (ISaveOperationsContainer)entity;
						result = container.ProcessUpdateOperations(afterSession);
						if(result.Success)
							result = container.ProcessInsertOperations(afterSession);
					}
					else
						result = entity.Save(afterSession);
					if(!result.Success)
						return result;
					if(entity is IEntitySavePersistenceSessionObserver)
						((IEntitySavePersistenceSessionObserver)entity).Register(afterSession);
				}
			}
			if(deleteProcessing)
				session.Root.Parameters.Remove("DeleteOperationsProcessing");
			return result;
		}

		/// <summary>
		/// Registra a entidade para ser apagada.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		public override DeleteResult Delete(Data.IPersistenceSession session)
		{
			var result = OnDeleting();
			if(!result.Success)
				return result;
			if(ExistsInStorage)
			{
				var actionId = session.Delete(DataModel, DeleteActionCallback);
				var beforeSession = session.CreateBeforeSessionForAction(actionId);
				var afterSession = session.CreateAfterSessionForAction(actionId);
				Domain.DomainEvents.Instance.GetEvent<EntityDeletingWithPersistenceSessionEvent>().Publish(new EntityDeletingWithPersistenceSessionEventArgs(this, session, beforeSession, afterSession));
				var beforeLinksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.BeforeEntity);
				var linksAccessors = Loader.GetLinksAccessors(EntityChildSavePriority.AfterEntity);
				foreach (var i in beforeLinksAccessors)
				{
					var entity = i.Get(this);
					if(entity != null)
					{
						SaveResult saveResult = null;
						if(entity is ISaveOperationsContainer)
						{
							var container = (ISaveOperationsContainer)entity;
							saveResult = container.ProcessUpdateOperations(beforeSession);
							if(saveResult.Success)
								saveResult = container.ProcessInsertOperations(beforeSession);
						}
						else
							saveResult = entity.Save(beforeSession);
						if(!saveResult.Success)
							return new DeleteResult(false, saveResult.Message);
						if(entity is IEntitySavePersistenceSessionObserver)
							((IEntitySavePersistenceSessionObserver)entity).Register(beforeSession);
					}
				}
				foreach (var i in Loader.GetChildrenAccessors().Reverse())
				{
					var entityRemoved = GetSingleChildRemoved(i.Name);
					if(entityRemoved != null)
					{
						var deleteResult = entityRemoved.Delete(beforeSession);
						if(!deleteResult.Success)
							return deleteResult;
					}
					var child = i.Get(this);
					if(child != null)
					{
						result = child.Delete(beforeSession);
						if(!result.Success)
							return result;
					}
				}
				var references = Loader.GetReferences().Where(f => this.IsReferenceInitialized(f.Name));
				foreach (var item in new Tuple<Data.IPersistenceSession, EntityChildSavePriority>[] {
					new Tuple<Data.IPersistenceSession, EntityChildSavePriority>(beforeSession, EntityChildSavePriority.BeforeEntity)
				})
				{
					foreach (var i in references.Where(f => f.SavePriority == item.Item2))
					{
						var entity = i.ParentValueGetter(this);
						Colosoft.Business.SaveResult saveResult = null;
						if(entity != null && entity.IsChanged)
						{
							if(entity is ISaveOperationsContainer)
							{
								var container = (ISaveOperationsContainer)entity;
								saveResult = container.ProcessUpdateOperations(item.Item1);
								if(result.Success)
									saveResult = container.ProcessInsertOperations(item.Item1);
							}
							else
								saveResult = entity.Save(item.Item1);
							if(!result.Success)
								return new DeleteResult(false, saveResult.Message);
							if(entity is IEntitySavePersistenceSessionObserver)
								((IEntitySavePersistenceSessionObserver)entity).Register(afterSession);
						}
					}
				}
				foreach (var i in linksAccessors)
				{
					var entity = i.Get(this);
					if(entity != null)
					{
						SaveResult saveResult = null;
						if(entity is ISaveOperationsContainer)
						{
							var container = (ISaveOperationsContainer)entity;
							saveResult = container.ProcessUpdateOperations(afterSession);
							if(result.Success)
								saveResult = container.ProcessInsertOperations(afterSession);
						}
						else
							saveResult = entity.Save(afterSession);
						if(!saveResult.Success)
							return new DeleteResult(false, saveResult.Message);
						if(entity is IEntitySavePersistenceSessionObserver)
							((IEntitySavePersistenceSessionObserver)entity).Register(afterSession);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Método acionado quando retorna da operação de exclusão dos dados da entidade.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="result"></param>
		private void DeleteActionCallback(Colosoft.Data.PersistenceAction action, Colosoft.Data.PersistenceActionResult result)
		{
			if(result.Success)
			{
				OnDeleted(true, null);
			}
			else
				OnDeleted(result.Success, result.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		/// <summary>
		/// Verifica se a instancia é igual a instancia informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool Equals(IEntity<Model> other)
		{
			return base.Equals(other);
		}

		/// <summary>
		/// Instancia do modelo da dados associado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		Data.IModel IEntityOfModel.DataModel
		{
			get
			{
				return this.DataModel;
			}
		}

		/// <summary>
		/// Classe usada para compara <see cref="System.Reflection.PropertyInfo"/> pelo nome e tipo.
		/// </summary>
		class PropertyInfoNameTypeIEqualityComparer : IEqualityComparer<System.Reflection.PropertyInfo>
		{
			public static readonly PropertyInfoNameTypeIEqualityComparer Instance = new PropertyInfoNameTypeIEqualityComparer();

			public bool Equals(System.Reflection.PropertyInfo x, System.Reflection.PropertyInfo y)
			{
				return x != null && y != null && x.Name == y.Name && x.PropertyType == y.PropertyType;
			}

			public int GetHashCode(System.Reflection.PropertyInfo obj)
			{
				return obj.Name.GetHashCode() ^ obj.PropertyType.GetHashCode();
			}
		}

		/// <summary>
		/// Implementação do observer da entidade de um modelo de dados.
		/// </summary>
		class EntityOfModelRecordObserver : Entity.EntityRecordObserver
		{
			/// <summary>
			/// Instancia do modelo de dados associado com a entidade.
			/// </summary>
			protected override object DataModel
			{
				get
				{
					var entity = this.Owner as Entity<Model>;
					return entity == null ? null : entity.DataModel;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entity"></param>
			public EntityOfModelRecordObserver(Entity<Model> entity) : base(entity)
			{
			}
		}
	}
}
