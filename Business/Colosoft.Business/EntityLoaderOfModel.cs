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
	/// Implementação base da carga da entidade.
	/// </summary>
	/// <typeparam name="TEntity1"></typeparam>
	/// <typeparam name="Model"></typeparam>
	public abstract partial class EntityLoader<TEntity1, Model> : EntityLoader, IDisposable, IEntityLoader<Model> where Model : class, Colosoft.Data.IModel where TEntity1 : class, IEntity<Model>
	{
		private static readonly IEnumerable<EntityLoaderReference> EmptyReferences = new EntityLoaderReference[0];

		private bool _hasFindName = false;

		private Colosoft.Query.IQueryResultBindStrategy _bindStrategy;

		private Colosoft.Query.IQueryResultObjectCreator _objectCreator;

		private Colosoft.Query.IQueryResultBindStrategy _entityDescriptorBindStrategy;

		private IEntityDescriptorCreator _entityDescriptorCreator;

		private IFindNameConverter _findNameConverter;

		private Reflection.TypeName _dataModelTypeName;

		private bool _innerInstanceSupport = true;

		/// <summary>
		/// Armazena os filhos da entidade.
		/// </summary>
		private List<IEntityLoaderChildInfo> _children = new List<IEntityLoaderChildInfo>();

		/// <summary>
		/// Armazena a relação das propriedades onde está o filhos singulares.
		/// </summary>
		private string[] _singleChildProperties;

		/// <summary>
		/// Armazena os links da entidade.
		/// </summary>
		private List<EntityLoaderLinkInfo> _links = new List<EntityLoaderLinkInfo>();

		/// <summary>
		/// Armazena as referencias da entidade.
		/// </summary>
		private Dictionary<string, EntityLoaderReference> _references = new Dictionary<string, EntityLoaderReference>();

		/// <summary>
		/// Dicionário que armazena a relação das propriedades assistidas com a referencias.
		/// </summary>
		private Dictionary<string, List<EntityLoaderReference>> _referencesWatchedProperties = new Dictionary<string, List<EntityLoaderReference>>();

		/// <summary>
		/// Instancia responsável por criar uma nova instancia da entidade de dados.
		/// </summary>
		private Func<EntityLoaderCreatorArgs<Model>, IEntity<Model>> _entityCreator;

		/// <summary>
		/// Instancia responsável por recupera o identificador unico da instancia.
		/// </summary>
		private Func<Model, int> _uidGetter;

		/// <summary>
		/// Instancia responsável por recupera o nome unico da instancia.
		/// </summary>
		private Func<Model, object>[] _findNameGetters;

		private string[] _findNameProperties;

		/// <summary>
		/// Instância responsável por recuperar a descrição da instância.
		/// </summary>
		private Func<Model, object> _descriptionGetter;

		/// <summary>
		/// Nome da proriedade de descrição.
		/// </summary>
		private string _descriptionPropertyName;

		/// <summary>
		/// Instancia responsável por definir o valor do identificador da instancia.
		/// </summary>
		private Action<Model, int> _uidSetter;

		/// <summary>
		/// Instancia do dicionário com a relação das chaves da entidade.
		/// </summary>
		private Dictionary<string, Tuple<Func<Model, object>, Action<Model, object>>> _keysGettersAndSetters;

		/// <summary>
		/// Nome da propriedade do identificador unico da instancia.
		/// </summary>
		private string _uidPropertyName;

		private Query.Record.RecordDescriptor _keyRecordDescriptor;

		private Colosoft.Reflection.TypeName _modelTypeName;

		/// <summary>
		/// Método usado para recuperar uma instancia do descritor da entidade.
		/// </summary>
		private Func<CreateEntityDescriptorArgs, IEntityDescriptor> _entityDescriptorGetter;

		/// <summary>
		/// Instancia responsável por criar uma nova instancia da entidade de dados.
		/// </summary>
		internal Func<EntityLoaderCreatorArgs<Model>, IEntity<Model>> EntityCreator
		{
			get
			{
				if(_entityCreator == null)
					throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_CreatorUndefined, typeof(TEntity1).FullName).Format());
				return _entityCreator;
			}
		}

		/// <summary>
		/// Nome do tipo do Model.
		/// </summary>
		public Colosoft.Reflection.TypeName ModelTypeName
		{
			get
			{
				if(_modelTypeName == null)
					_modelTypeName = Colosoft.Reflection.TypeName.Get<Model>();
				return _modelTypeName;
			}
		}

		/// <summary>
		/// Recupera o descritor do registro que contém os valores chave do tipo da entidade.
		/// </summary>
		public override Query.Record.RecordDescriptor KeyRecordDescriptor
		{
			get
			{
				if(_keyRecordDescriptor == null)
				{
					var isVersioned = typeof(Data.IVersionedModel).IsAssignableFrom(typeof(Model));
					Query.Record.Field[] fields = null;
					if(this.HasUid)
					{
						var uidField = new Query.Record.Field(this.UidPropertyName, typeof(int));
						if(isVersioned)
							fields = new Query.Record.Field[] {
								uidField,
								new Query.Record.Field("RowVersion", typeof(long))
							};
						else
							fields = new Query.Record.Field[] {
								uidField
							};
					}
					else if(this.KeysPropertyNames != null)
					{
						var keys = this.KeysPropertyNames.ToArray();
						if(keys.Length > 0)
						{
							fields = new Query.Record.Field[keys.Length + (isVersioned ? 1 : 0)];
							var dataModelType = typeof(Model);
							for(var i = 0; i < keys.Length; i++)
							{
								var propertyInfo = dataModelType.GetProperty(keys[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
								if(propertyInfo != null)
									fields[i] = new Query.Record.Field(propertyInfo.Name, propertyInfo.PropertyType);
							}
							if(isVersioned)
								fields[fields.Length - 1] = new Query.Record.Field("RowVersion", typeof(long));
						}
					}
					_keyRecordDescriptor = new Query.Record.RecordDescriptor("default", fields ?? new Query.Record.Field[0]);
				}
				return _keyRecordDescriptor;
			}
		}

		/// <summary>
		/// Conversor que gera o nome unico da instância.
		/// </summary>
		public override IFindNameConverter FindNameConverter
		{
			get
			{
				return _findNameConverter;
			}
		}

		/// <summary>
		/// Propriedades que compôem o nome único.
		/// </summary>
		public override string[] FindNameProperties
		{
			get
			{
				return _findNameProperties ?? new string[0];
			}
		}

		/// <summary>
		/// Nome da propriedade de descrição.
		/// </summary>
		public override string DescriptionPropertyName
		{
			get
			{
				return _descriptionPropertyName ?? string.Empty;
			}
		}

		/// <summary>
		/// Recupera a instancia de configuração.
		/// </summary>
		/// <returns></returns>
		protected FluentEntityLoader Configure()
		{
			return new FluentEntityLoader(this);
		}

		/// <summary>
		/// Identifica se a entidade possui nome unico.
		/// </summary>
		public override bool HasFindName
		{
			get
			{
				return _hasFindName;
			}
		}

		/// <summary>
		/// Nome da propriedade do identificador unico da entidade.
		/// </summary>
		public override string UidPropertyName
		{
			get
			{
				return _uidPropertyName;
			}
		}

		/// <summary>
		/// Identifica se a entidade possui identificador unico.
		/// </summary>
		public override bool HasUid
		{
			get
			{
				return !string.IsNullOrEmpty(_uidPropertyName);
			}
		}

		/// <summary>
		/// Identifica se a entidade possui descrição.
		/// </summary>
		public override bool HasDescription
		{
			get
			{
				return !string.IsNullOrEmpty(_descriptionPropertyName);
			}
		}

		/// <summary>
		/// Tipo do modelo de dados associado com a entidade.
		/// </summary>
		public override Type DataModelType
		{
			get
			{
				return typeof(Model);
			}
		}

		/// <summary>
		/// Nome do tipo do modelo de dados associado com a entidade.
		/// </summary>
		public override Reflection.TypeName DataModelTypeName
		{
			get
			{
				if(_dataModelTypeName == null)
					_dataModelTypeName = Colosoft.Reflection.TypeName.Get(typeof(Model));
				return _dataModelTypeName;
			}
		}

		/// <summary>
		/// Nomes das propriedades chave.
		/// </summary>
		public override IEnumerable<string> KeysPropertyNames
		{
			get
			{
				return _keysGettersAndSetters != null ? (IEnumerable<string>)_keysGettersAndSetters.Keys : new string[0];
			}
		}

		/// <summary>
		/// Identifica se a entidade associada possui chaves.
		/// </summary>
		public override bool HasKeys
		{
			get
			{
				return _keysGettersAndSetters != null && _keysGettersAndSetters.Count > 0;
			}
		}

		/// <summary>
		/// Identifica se a entidade associada tem suporte para instancia interna.
		/// </summary>
		public override bool InnerInstanceSupport
		{
			get
			{
				return _innerInstanceSupport;
			}
		}

		/// <summary>
		/// Identifica se a entidade possui filhos.
		/// </summary>
		public override bool HasChildren
		{
			get
			{
				return _children.Any();
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoader()
		{
			Dispose(false);
		}

		/// <summary>
		/// Cria a instanca da lista de links.
		/// </summary>
		/// <param name="items">Items que serão adicionaods na lista.</param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="linkInfo"></param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="uiContext"></param>
		/// <param name="sourceContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="parentUidSetter"></param>
		/// <returns></returns>
		protected IEntityLinksList CreateLinksList(IEnumerable<IEntity> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Colosoft.Query.ISourceContext sourceContext, IEntityTypeManager entityTypeManager, Action<IEntity> parentUidSetter)
		{
			var createInstanceMethod = typeof(EntityLinksList<>).MakeGenericType(linkInfo.LinkEntityType).GetMethod("CreateInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (IEntityLinksList)createInstanceMethod.Invoke(null, new object[] {
				items,
				linkInfo,
				childFromModelCreator,
				child,
				uiContext,
				parentUidSetter,
				sourceContext,
				entityTypeManager
			});
		}

		/// <summary>
		/// Cria a lista de filhos.
		/// </summary>
		/// <param name="items">Items que serão adicionados na lista.</param>
		/// <param name="child">Instancia do filho associado com o link.</param>
		/// <param name="linkInfo"></param>
		/// <param name="childFromModelCreator"></param>
		/// <param name="uiContext"></param>
		/// <param name="sourceContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="parentUidSetter"></param>
		/// <returns></returns>
		protected IEntityLinksList CreateLinksList(Lazy<IEnumerable<IEntity>> items, EntityLoaderLinkInfo linkInfo, EntityFromModelCreatorHandler childFromModelCreator, IEntityList child, string uiContext, Colosoft.Query.ISourceContext sourceContext, IEntityTypeManager entityTypeManager, Action<IEntity> parentUidSetter)
		{
			var createInstanceMethod = typeof(EntityLinksList<>).MakeGenericType(linkInfo.LinkEntityType).GetMethod("CreateLazyInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (IEntityLinksList)createInstanceMethod.Invoke(null, new object[] {
				items,
				linkInfo,
				childFromModelCreator,
				child,
				uiContext,
				parentUidSetter,
				sourceContext,
				entityTypeManager
			});
		}

		/// <summary>
		/// Cria a lista de filhos.
		/// </summary>
		/// <param name="entityType">Tipo da entidade de dados.</param>
		/// <param name="items">Items que serão adicionados na lista.</param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="parentUidSetter"></param>
		/// <returns></returns>
		protected IEntityChildrenList CreateChildrenList(Type entityType, IEnumerable<IEntity> items, string uiContext, IEntityTypeManager entityTypeManager, Action<IEntity> parentUidSetter)
		{
			var createInstanceMethod = typeof(EntityChildrenList<>).MakeGenericType(entityType).GetMethod("CreateInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (IEntityChildrenList)createInstanceMethod.Invoke(null, new object[] {
				items,
				uiContext,
				parentUidSetter,
				entityTypeManager
			});
		}

		/// <summary>
		/// Cria a lista de filhos.
		/// </summary>
		/// <param name="entityType">Tipo da entidade de dados.</param>
		/// <param name="items">Items que serão adicionados na lista.</param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="parentUidSetter"></param>
		/// <returns></returns>
		protected IEntityChildrenList CreateChildrenList(Type entityType, Lazy<IEnumerable<IEntity>> items, string uiContext, IEntityTypeManager entityTypeManager, Action<IEntity> parentUidSetter)
		{
			var createInstanceMethod = typeof(EntityChildrenList<>).MakeGenericType(entityType).GetMethod("CreateLazyInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (IEntityChildrenList)createInstanceMethod.Invoke(null, new object[] {
				items,
				uiContext,
				parentUidSetter,
				entityTypeManager
			});
		}

		/// <summary>
		/// Método usado apenas como uma adaptação.
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="action"></param>
		/// <returns></returns>
		private static Action<TChild> ParentUidSetterWrapper<TChild>(Action<IEntity> action) where TChild : IEntity
		{
			return e => action(e);
		}

		/// <summary>
		/// Tenta recupera a instancia do single child com base no nome da propriedade.
		/// </summary>
		/// <param name="entity">Instancia onde o filho está.</param>
		/// <param name="propertyName"></param>
		/// <param name="childName">Nome do filho.</param>
		/// <param name="child">Instancia do filho.</param>
		/// <returns></returns>
		public override bool TryGetSingleChildFromProperty(IEntity entity, string propertyName, out string childName, out IEntity child)
		{
			if(_singleChildProperties == null)
				_singleChildProperties = _children.Select(f => f.PropertyName).OrderBy(f => f).ToArray();
			if(_singleChildProperties.BinarySearch(propertyName) >= 0)
			{
				var childInfo = _children.Where(f => f.PropertyName == propertyName).FirstOrDefault();
				child = childInfo.ParentValueGetter(entity);
				childName = childInfo.Name;
				return true;
			}
			childName = null;
			child = null;
			return false;
		}

		/// <summary>
		/// Cria uma descritor de entidade.
		/// </summary>
		/// <returns>Nova instancia do descritor de entidade.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override IEntityDescriptor CreateEntityDescriptor()
		{
			var args = new CreateEntityDescriptorArgs(this);
			if(_entityDescriptorGetter == null)
				return new BusinessEntityDescriptor(args);
			return _entityDescriptorGetter(args);
		}

		/// <summary>
		/// Cria um descritor com base na entidade informada.
		/// </summary>
		/// <param name="entity">Instancia da entidade com os valores que serão usados na criação do descritor.</param>
		/// <returns></returns>
		public override IEntityDescriptor CreateEntityDescriptor(IEntity entity)
		{
			entity.Require("entity").NotNull();
			var descriptor = CreateEntityDescriptor();
			if(descriptor is IEntityRecordObserver)
				((IEntityRecordObserver)descriptor).RegisterObserver(GetRecordKey(entity));
			var entity2 = (TEntity1)entity;
			var record = entity2.DataModel.CreateRecord();
			var bindStrategy = GetEntityDescriptorBindStrategy();
			object descriptor1 = (object)descriptor;
			bindStrategy.Bind(record, Query.BindStrategyMode.All, ref descriptor1);
			return descriptor;
		}

		/// <summary>
		/// Recupera a instancia da estratégia de vinculação.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override Colosoft.Query.IQueryResultBindStrategy GetBindStrategy()
		{
			if(_bindStrategy == null)
				_bindStrategy = new Colosoft.Query.TypeBindStrategy<Model>(GetObjectCreator());
			return _bindStrategy;
		}

		/// <summary>
		/// Recupera a instancia do responsável por criar os objetos da entidade.
		/// </summary>
		/// <returns></returns>
		public override Colosoft.Query.IQueryResultObjectCreator GetObjectCreator()
		{
			if(_objectCreator == null)
				_objectCreator = new Colosoft.Query.QueryResultObjectCreator(typeof(Model));
			return _objectCreator;
		}

		/// <summary>
		/// Recupera a estratégia de vinculação para o EntityDescriptor.
		/// </summary>
		/// <returns></returns>
		public override Colosoft.Query.IQueryResultBindStrategy GetEntityDescriptorBindStrategy()
		{
			if(_entityDescriptorBindStrategy == null)
				_entityDescriptorBindStrategy = new EntityDescriptorQueryResultBindStrategy(this);
			return _entityDescriptorBindStrategy;
		}

		/// <summary>
		/// Recupera a instancia do resposável por criar os descritores de entidade.
		/// </summary>
		/// <returns></returns>
		public override IEntityDescriptorCreator GetEntityDescriptorCreator()
		{
			if(_entityDescriptorCreator == null)
				_entityDescriptorCreator = new EntityDescriptorCreator(this);
			return _entityDescriptorCreator;
		}

		/// <summary>
		/// Recupera a instancia da factory da chave dos registros.
		/// </summary>
		/// <returns></returns>
		public override Query.IRecordKeyFactory GetRecordKeyFactory()
		{
			return Query.RecordKeyFactory.Instance;
		}

		/// <summary>
		/// Notifica que o Uid a entidade foi alterado.
		/// </summary>
		/// <param name="entity"></param>
		public override void NotifyUidChanged(IEntity entity)
		{
			if(entity == null)
				return;
			foreach (var child in _children)
			{
				var childInstance = child.ParentValueGetter(entity);
				if(childInstance != null)
				{
					if(childInstance is IEntityList)
					{
						foreach (IEntity item in ((IEntityList)childInstance))
							child.ParentUidSetter(entity, item);
					}
					else
					{
						child.ParentUidSetter(entity, childInstance);
					}
				}
			}
		}

		/// <summary>
		/// Reseta todos os identificadores associados com a entidade e seus filhos.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="typeManager"></param>
		public override void ResetAllUids(IEntity entity, IEntityTypeManager typeManager)
		{
			if(entity == null)
				return;
			if(HasUid)
			{
				var newUid = typeManager.GenerateInstanceUid(typeof(TEntity1));
				SetInstanceUid(entity, newUid);
			}
			entity.RemoveStorageControl();
			foreach (var child in _children)
			{
				var childInstance = child.ParentValueGetter(entity);
				if(childInstance != null)
				{
					if(childInstance is IEntityList)
					{
						foreach (IEntity item in ((IEntityList)childInstance))
							child.ParentUidSetter(entity, item);
					}
					else
					{
						child.ParentUidSetter(entity, childInstance);
					}
					childInstance.ResetAllUids();
				}
			}
		}

		/// <summary>
		/// Recupera o identificador unico associadom com a instancia do modelo informado.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public override int GetInstanceUid(Colosoft.Data.IModel model)
		{
			if(_uidGetter == null)
				return 0;
			return _uidGetter((Model)model);
		}

		/// <summary>
		/// Define o identificador único para o modelo informado.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="uid"></param>
		public override void SetInstanceUid(Data.IModel model, int uid)
		{
			if(_uidSetter != null)
				_uidSetter((Model)model, uid);
		}

		/// <summary>
		/// Recupera o valor do identificador unico da instancia.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override int GetInstanceUid(IEntity entity)
		{
			if(_uidGetter == null)
				return 0;
			var entity2 = (IEntity<Model>)entity;
			if(entity2.DataModel == null)
				return 0;
			return _uidGetter(entity2.DataModel);
		}

		/// <summary>
		/// Recupera o valor do nome unico da instancia.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override string GetInstanceFindName(IEntity entity)
		{
			if(_findNameGetters == null)
				return null;
			var values = new object[_findNameGetters.Length];
			for(int index = 0; index < _findNameGetters.Length; index++)
			{
				values[index] = _findNameGetters[index](((IEntity<Model>)entity).DataModel);
			}
			return GetInstanceFindName(values);
		}

		/// <summary>
		/// Recupera o nome unico da instancia da entidade a partir dos valores que compôem o nome.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public override string GetInstanceFindName(object[] values)
		{
			if(_findNameConverter != null)
			{
				return _findNameConverter.Convert(values);
			}
			else
			{
				return string.Join(" ", values.Select(f => (f ?? "").ToString()).ToArray());
			}
		}

		/// <summary>
		/// Define o identificador unico da instanciada entidade.
		/// </summary>
		/// <param name="entity">Instancia da entidade.</param>
		/// <param name="uid">Valor do novo identificador.</param>
		public override void SetInstanceUid(IEntity entity, int uid)
		{
			if(_uidSetter != null)
				_uidSetter(((IEntity<Model>)entity).DataModel, uid);
		}

		/// <summary>
		/// Recupera os valores das chaves da instancia.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override IEnumerable<Tuple<string, object>> GetInstanceKeysValues(IEntity entity)
		{
			if(_keysGettersAndSetters != null && _keysGettersAndSetters.Count > 0)
			{
				foreach (var i in _keysGettersAndSetters)
				{
					var value = i.Value.Item1(((IEntity<Model>)entity).DataModel);
					yield return new Tuple<string, object>(i.Key, value);
				}
			}
			else if(_uidGetter != null)
				yield return new Tuple<string, object>(UidPropertyName, GetInstanceUid(entity));
		}

		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave que representa o registro.</param>
		/// <param name="bindStrategySession">Estratégia de vinculação dos dados do resultado.</param>
		/// <param name="objectCreator">Instancia responsável por criar objetos.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Gerenciador de tipos.</param>
		/// <returns></returns>
		public override IEntity FullLoad(Colosoft.Query.IRecord record, Query.RecordKey recordKey, Colosoft.Query.IQueryResultBindStrategySession bindStrategySession, Colosoft.Query.IQueryResultObjectCreator objectCreator, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
		{
			var dataModel = objectCreator.Create();
			bindStrategySession.Bind(record, Query.BindStrategyMode.All, ref dataModel);
			return FullLoad(new DataModelRecordKey<Model>((Model)dataModel, recordKey), sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Cria as consultas aninhadas para recupera as referencia da entidade pai.
		/// </summary>
		/// <param name="queryable">Consulta usada para recueprar a entidade.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="parentResult">Resultados dos itens pai.</param>
		/// <param name="exceptions">Fila dos erros ocorridos.</param>
		public override void CreateNestedQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, IList<EntityLoaderCreatorArgs> parentResult, Queue<Exception> exceptions)
		{
			foreach (var i in _children)
			{
				if(!i.IsSingle && i.Options.HasFlag(LoadOptions.Lazy))
					continue;
				var child = i;
				var childCursorIndex = 0;
				child.CreateQueries(queryable, uiContext, entityTypeManager, exceptions, (sender, e) =>  {
					e.Result.Require("result").NotNull();
					var parent = parentResult[childCursorIndex++];
					if(parentResult.Count == childCursorIndex)
						childCursorIndex = 0;
					IEntity entity = null;
					if(child.IsSingle)
					{
						using (var enumerator = e.Result.GetEnumerator())
							if(enumerator.MoveNext())
								entity = enumerator.Current;
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(queryable.SourceContext);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
					}
					else
					{
						entity = CreateChildrenList(child.EntityType, e.Result, uiContext, entityTypeManager, new Action<IEntity>(f =>  {
							//// Verifica se existe o pai da entidade
							if(entity.Owner != null)
								child.ParentUidSetter(entity.Owner, f);
						}));
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(queryable.SourceContext);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
					}
					parent.Children.Add(child.Name, entity);
				}, (sender, e) =>  {
					childCursorIndex++;
					if(parentResult.Count == childCursorIndex)
						childCursorIndex = 0;
					var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, child.Name, typeof(Model).FullName, e.Result.Message.Format()).Format(), e.Result.Error);
					exceptions.Enqueue(ex2);
				});
			}
			foreach (var i in _links)
			{
				if(!i.IsSingle && i.Options.HasFlag(LoadOptions.Lazy))
					continue;
				var link = i;
				var linkCursorIndex = 0;
				var child = _children.Where(f => f.Name == link.ChildName).FirstOrDefault();
				if(child == null)
					throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_ChildOfLinkNotFound, link.ChildName, typeof(TEntity1).FullName, link.Name).Format());
				var loader = entityTypeManager.GetLoader(link.LinkEntityType);
				var childLoader = entityTypeManager.GetLoader(child.EntityType);
				link.CreateQueries(queryable, UidPropertyName, uiContext, entityTypeManager, exceptions, (sender, e) =>  {
					e.Result.Require("result").NotNull();
					var parent = parentResult[linkCursorIndex++];
					if(parentResult.Count == linkCursorIndex)
						linkCursorIndex = 0;
					try
					{
						IEntity entity = null;
						if(link.IsSingle)
						{
							using (var enumerator = e.Result.GetEnumerator())
							{
								if(enumerator.MoveNext())
									entity = enumerator.Current;
							}
							if(entity is IConnectedEntity)
								((IConnectedEntity)entity).Connect(queryable.SourceContext);
							if(entity is ILoadableEntity)
								((ILoadableEntity)entity).NotifyLoaded();
						}
						else
						{
							var childInstance = (IEntityList)parent.Children[link.ChildName];
							var entities = e.Result;
							entity = CreateLinksList(entities, link, new EntityFromModelCreatorHandler(childLoader.Create), childInstance, uiContext, queryable.SourceContext, entityTypeManager, new Action<IEntity>(f =>  {
								//// Verifica se o propriedade não foi definido
								if(entity.Owner != null)
									link.LinkParentForeignKeySetter(entity.Owner, f);
							}));
						}
						parent.Links.AddLink(link.Name, entity);
					}
					catch(Exception ex)
					{
						var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadLinkFromEntityError, child.Name, typeof(Model).FullName, ex.Message).Format(), ex);
						exceptions.Enqueue(ex2);
						return;
					}
				}, (sender, e) =>  {
					linkCursorIndex++;
					if(parentResult.Count == linkCursorIndex)
						linkCursorIndex = 0;
					var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadLinkFromEntityError, link.Name, typeof(Model).FullName, e.Result.Message.Format()).Format(), e.Result.Error);
					exceptions.Enqueue(ex2);
				});
			}
			foreach (var i in _references)
			{
				if(i.Value.IsLoadLazy)
					continue;
				var reference = i.Value;
				var referenceCursorIndex = 0;
				reference.CreateQueries(queryable, uiContext, entityTypeManager, exceptions, (sender, e) =>  {
					e.Result.Require("result").NotNull();
					var parent = parentResult[referenceCursorIndex++];
					if(parentResult.Count == referenceCursorIndex)
						referenceCursorIndex = 0;
					IEntity entity = null;
					using (var enumerator = e.Result.GetEnumerator())
						if(enumerator.MoveNext())
							entity = enumerator.Current;
					if(entity is IConnectedEntity)
						((IConnectedEntity)entity).Connect(queryable.SourceContext);
					if(entity is ILoadableEntity)
						((ILoadableEntity)entity).NotifyLoaded();
					parent.References.Add(reference.Name, entity);
				}, (sender, e) =>  {
					referenceCursorIndex++;
					if(parentResult.Count == referenceCursorIndex)
						referenceCursorIndex = 0;
					var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadReferenceFromEntityError, reference.Name, typeof(Model).FullName, e.Result.Message.Format()).Format(), e.Result.Error);
					exceptions.Enqueue(ex2);
				});
			}
		}

		/// <summary>
		/// Recupera os dados de carga tardia .
		/// </summary>
		/// <param name="creatorArgs">Argumentos para a criação da entidade.</param>
		/// <param name="state">Estado dos dados de carga tardia.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidades.</param>
		/// <param name="exceptions">Fila dos erros ocorridos.</param>
		public override void GetLazyData(EntityLoaderCreatorArgs creatorArgs, LazyDataState state, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions)
		{
			var dataModel = (Model)creatorArgs.DataModel;
			if(_children.Count > 0)
			{
				Colosoft.Query.MultiQueryable multiQuery = null;
				if(sourceContext != null)
					multiQuery = sourceContext.CreateMultiQuery();
				foreach (var i in _children)
				{
					var child = i;
					if(!child.IsSingle && child.Options.HasFlag(LoadOptions.Lazy))
					{
						Lazy<IEnumerable<IEntity>> entities = null;
						if(sourceContext != null)
							entities = new Lazy<IEnumerable<IEntity>>(() =>  {
								var resultEntities = new List<IEntity>();
								var queries = child.CreateQueries(state.Entity.Uid, dataModel, sourceContext);
								if(queries != null && queries.Length > 0)
								{
									var multiQuery2 = sourceContext.CreateMultiQuery();
									for(var j = 0; j < queries.Length; j++)
									{
										var query = queries[j];
										multiQuery2.Add(query.Query, (sender, queryInfo, queryResult) =>  {
											var loader = entityTypeManager.GetLoader(query.EntityType);
											resultEntities.AddRange(loader.GetLazyEntities(queryResult, sourceContext, uiContext, entityTypeManager));
										});
									}
									try
									{
										multiQuery2.Execute();
									}
									catch(Exception ex)
									{
										throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, child.Name, typeof(Model).FullName, ex.Message).Format(), ex);
									}
								}
								return resultEntities;
							});
						else
							entities = new Lazy<IEnumerable<IEntity>>(() => new IEntity[0]);
						var entity = CreateChildrenList(child.EntityType, entities, uiContext, entityTypeManager, new Action<IEntity>(e => child.ParentUidSetter(state.Entity, e)));
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(sourceContext);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
						creatorArgs.Children.Add(child.Name, entity);
					}
				}
			}
			foreach (var i in _links)
			{
				var link = i;
				var child = _children.Where(f => f.Name == link.ChildName).FirstOrDefault();
				if(child == null)
					throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_ChildOfLinkNotFound, link.ChildName, typeof(TEntity1).FullName, link.Name).Format());
				var loader = entityTypeManager.GetLoader(link.LinkEntityType);
				if(!link.IsSingle && link.Options.HasFlag(LoadOptions.Lazy))
				{
					Lazy<IEnumerable<IEntity>> entities = null;
					if(sourceContext != null)
						entities = new Lazy<IEnumerable<IEntity>>(() =>  {
							var query = link.CreateQuery(state.Entity.Uid, child.ForeignPropertyName, sourceContext);
							if(link.Options.HasFlag(LoadOptions.LinksContentLazy))
								return loader.GetLazyEntities(query.Execute(), sourceContext, uiContext, entityTypeManager);
							else
								return loader.GetFullEntities(query.Execute(), sourceContext, uiContext, entityTypeManager);
						});
					else
						entities = new Lazy<IEnumerable<IEntity>>(() => new IEntity[0]);
					var childInstance = (IEntityList)creatorArgs.Children.Where(f => f.Key == link.ChildName).Select(f => f.Value).FirstOrDefault();
					var childLoader = entityTypeManager.GetLoader(_children.Where(f => f.Name == link.ChildName).FirstOrDefault().EntityType);
					var entity = CreateLinksList(entities, link, new EntityFromModelCreatorHandler(childLoader.Create), childInstance, uiContext, sourceContext, entityTypeManager, new Action<IEntity>(e => link.LinkParentForeignKeySetter(state.Entity, e)));
					if(entity is IConnectedEntity)
						((IConnectedEntity)entity).Connect(sourceContext);
					if(entity is ILoadableEntity)
						((ILoadableEntity)entity).NotifyLoaded();
					creatorArgs.Links.AddLink(link.Name, entity);
				}
			}
		}

		/// <summary>
		/// Realiza a carga completa das entidades contidas nos registros informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override IEnumerable<IEntity> GetLazyEntities(IEnumerable<Colosoft.Query.IRecord> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null)
		{
			return new GetLazyEntitiesEnumerable(this, null, sourceContext, result, uiContext, entityTypeManager, args);
		}

		/// <summary>
		/// Recupera a entidade completas com base na consulta informada.
		/// </summary>
		/// <param name="queryable">Consulta que será realizado.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidas.</param>
		/// <returns></returns>
		public override IEnumerable<IEntity> GetFullEntities(Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
		{
			return new GetFullEntitiesEnumerable(this, queryable, sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Recupera as entidades completas com bas na preparação da consulta.
		/// </summary>
		/// <param name="prepareResult">Resultado da preparação da consulta.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		public override IEnumerable<IEntity> GetFullEntities(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext)
		{
			prepareResult.Require("prepareResult").NotNull();
			prepareResult.EntityLoader = this;
			return new GetFullEntitiesEnumerable(prepareResult, sourceContext);
		}

		/// <summary>
		/// Recupera as entidades completas com bas na preparação da consulta.
		/// </summary>
		/// <param name="prepareResult">Resultado da preparação da consulta.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <returns></returns>
		public override IEnumerable<IEntity> GetFullEntities(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext, Query.IQueryResult queryResult)
		{
			prepareResult.Require("prepareResult").NotNull();
			prepareResult.EntityLoader = this;
			return new GetFullEntitiesEnumerable(prepareResult, sourceContext, queryResult);
		}

		/// <summary>
		/// Recupera os descritores das entidades associadas com o consulta informada.
		/// </summary>
		/// <param name="queryable">Consulta que será realizada.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		public override IEnumerable<T> GetEntityDescriptors<T>(Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext)
		{
			return new GetEntityDescriptorsEnumerable<T>(this, queryable, sourceContext, uiContext);
		}

		/// <summary>
		/// Realiza a carga completa das entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		public virtual IEnumerable<IEntity<Model>> GetFullEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
		{
			result.Require("result").NotNull();
			foreach (var i in result)
				yield return FullLoad(i, sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Realiza a carga completa em modo lazy das entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		public virtual IEnumerable<IEntity<Model>> GetLazyFullEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
		{
			result.Require("result").NotNull();
			foreach (var i in result)
				yield return LazyLoad(i, sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="dataModelRecordKey">Instancia com o modelo de dados da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public virtual IEntity<Model> FullLoad(DataModelRecordKey<Model> dataModelRecordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager)
		{
			var dataModel = dataModelRecordKey.DataModel;
			IEntity<Model> result = null;
			var exceptions = new Queue<Exception>();
			var children = new List<Tuple<string, IEntity>>();
			var links = new List<Tuple<string, IEntity>>();
			var references = new List<Tuple<string, IEntity>>();
			if(_children.Count > 0)
			{
				Colosoft.Query.MultiQueryable multiQuery = null;
				if(sourceContext != null)
					multiQuery = sourceContext.CreateMultiQuery();
				var parentUid = _uidGetter(dataModel);
				foreach (var i in _children)
				{
					var child = i;
					var parentUid2 = parentUid;
					if(i.ParentUidGetter != null)
						parentUid2 = i.ParentUidGetter(dataModel);
					EntityInfoQuery[] queries = null;
					if(multiQuery != null)
						queries = child.CreateQueries(parentUid2, dataModel, sourceContext);
					if(queries != null && queries.Length > 0)
					{
						var entities = new List<IEntity>();
						for(var j = 0; j < queries.Length; j++)
						{
							var query = queries[j];
							var isLast = (j + 1) == queries.Length;
							multiQuery.Add(query.Query, new Colosoft.Query.QueryCallBack((a, b, queryResult) =>  {
								IEntityLoader loader = null;
								try
								{
									loader = entityTypeManager.GetLoader(query.EntityType);
									var recordTypeName = Colosoft.Reflection.TypeName.Get(loader.DataModelType);
									var recordKeyFactory = loader.GetRecordKeyFactory();
									if(child.IsSingle)
									{
										using (var enumerator = queryResult.GetEnumerator())
										{
											if(enumerator.MoveNext())
											{
												var recordKey = recordKeyFactory.Create(recordTypeName, enumerator.Current);
												if(child.Options.HasFlag(LoadOptions.ChildrenContentLazy))
													entities.Add(loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager));
												else
													entities.Add(loader.FullLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager));
											}
										}
									}
									else
									{
										if(child.Options.HasFlag(LoadOptions.ChildrenContentLazy))
											entities.AddRange(loader.GetLazyEntities(queryResult, sourceContext, uiContext, entityTypeManager));
										else
											entities.AddRange(loader.GetFullEntities(queryResult, sourceContext, uiContext, entityTypeManager));
									}
								}
								catch(Exception ex)
								{
									var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, child.Name, typeof(Model).FullName, ex.Message).Format(), ex);
									exceptions.Enqueue(ex2);
									return;
								}
								finally
								{
									if(isLast)
									{
										if(child.IsSingle)
											children.Add(new Tuple<string, IEntity>(child.Name, entities.FirstOrDefault()));
										else
										{
											var entity = CreateChildrenList(child.EntityType, entities, uiContext, entityTypeManager, new Action<IEntity>(e => child.ParentUidSetter(result, e)));
											children.Add(new Tuple<string, IEntity>(child.Name, entity));
										}
									}
								}
							}), new Colosoft.Query.QueryFailedCallBack((sender, info, errorResult) =>  {
								var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, child.Name, typeof(Model).FullName, errorResult.Message.Format()).Format(), errorResult.Error);
								exceptions.Enqueue(ex2);
							}));
						}
					}
					else
					{
						if(child.IsSingle)
							children.Add(new Tuple<string, IEntity>(child.Name, null));
						else
						{
							var childrenList = CreateChildrenList(child.EntityType, new IEntity[0], uiContext, entityTypeManager, new Action<IEntity>(e => child.ParentUidSetter(result, e)));
							if(childrenList is IConnectedEntity)
								((IConnectedEntity)childrenList).Connect(sourceContext);
							children.Add(new Tuple<string, IEntity>(child.Name, childrenList));
						}
					}
				}
				foreach (var i in _links)
				{
					var link = i;
					var child = _children.Where(f => f.Name == link.ChildName).FirstOrDefault();
					if(child == null)
						throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_ChildOfLinkNotFound, link.ChildName, typeof(TEntity1).FullName, link.Name).Format());
					var linkEntityLoader = entityTypeManager.GetLoader(link.LinkEntityType);
					var childLoader = entityTypeManager.GetLoader(child.EntityType);
					var parentUid2 = parentUid;
					if(child.ParentUidGetter != null)
						parentUid2 = child.ParentUidGetter(dataModel);
					if(multiQuery != null)
					{
						multiQuery.Add(link.CreateQuery(parentUid2, child.ForeignPropertyName, sourceContext), (a, b, c) =>  {
							IEntity entity = null;
							IEntityLoader loader = null;
							try
							{
								loader = entityTypeManager.GetLoader(link.LinkEntityType);
								var recordTypeName = Colosoft.Reflection.TypeName.Get(loader.DataModelType);
								var recordKeyFactory = loader.GetRecordKeyFactory();
								if(link.IsSingle)
								{
									using (var enumerator = c.GetEnumerator())
									{
										if(enumerator.MoveNext())
										{
											var recordKey = recordKeyFactory.Create(recordTypeName, enumerator.Current);
											if(link.Options.HasFlag(LoadOptions.LinksContentLazy))
												entity = loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
											else
												entity = loader.FullLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
										}
									}
								}
								else
								{
									var childInstance = (IEntityList)children.Where(f => f.Item1 == link.ChildName).Select(f => f.Item2).FirstOrDefault();
									IEnumerable<IEntity> entities = null;
									if(link.Options.HasFlag(LoadOptions.LinksContentLazy))
										entities = loader.GetLazyEntities(c, sourceContext, uiContext, entityTypeManager);
									else
										entities = loader.GetFullEntities(c, sourceContext, uiContext, entityTypeManager);
									entity = CreateLinksList(entities, link, new EntityFromModelCreatorHandler(childLoader.Create), childInstance, uiContext, sourceContext, entityTypeManager, new Action<IEntity>(e => link.LinkParentForeignKeySetter(result, e)));
								}
							}
							catch(Exception ex)
							{
								var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadLinkFromEntityError, link.Name, typeof(Model).FullName, ex.Message).Format(), ex);
								exceptions.Enqueue(ex2);
								return;
							}
							finally
							{
								if(entity is IConnectedEntity)
									((IConnectedEntity)entity).Connect(sourceContext);
								links.Add(new Tuple<string, IEntity>(link.Name, entity));
							}
						}, new Colosoft.Query.QueryFailedCallBack((sender, info, errorResult) =>  {
							var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadLinkFromEntityError, link.Name, typeof(Model).FullName, errorResult.Message.Format()).Format(), errorResult.Error);
							exceptions.Enqueue(ex2);
						}));
					}
					else
					{
						if(link.IsSingle)
							links.Add(new Tuple<string, IEntity>(link.Name, null));
						else
						{
							var childInstance = (IEntityList)children.Where(f => f.Item1 == link.ChildName).Select(f => f.Item2).FirstOrDefault();
							var linksList = CreateLinksList(new IEntity[0], link, new EntityFromModelCreatorHandler(childLoader.Create), childInstance, uiContext, sourceContext, entityTypeManager, new Action<IEntity>(e => link.LinkParentForeignKeySetter(result, e)));
							if(linksList is IConnectedEntity)
								((IConnectedEntity)linksList).Connect(sourceContext);
							links.Add(new Tuple<string, IEntity>(link.Name, linksList));
						}
					}
				}
				foreach (var i in _references)
				{
					if(i.Value.IsLoadLazy)
						continue;
					var reference = i.Value;
					EntityInfoQuery[] queries = null;
					if(multiQuery != null)
						queries = reference.CreateQueries(dataModel, sourceContext);
					if(queries != null && queries.Length > 0)
					{
						var entities = new List<IEntity>();
						for(var j = 0; j < queries.Length; j++)
						{
							var query = queries[j];
							var isLast = (j + 1) == queries.Length;
							multiQuery.Add(query.Query, new Colosoft.Query.QueryCallBack((a, b, queryResult) =>  {
								IEntityLoader loader = null;
								try
								{
									loader = entityTypeManager.GetLoader(query.EntityType);
									var recordTypeName = Colosoft.Reflection.TypeName.Get(loader.DataModelType);
									var recordKeyFactory = loader.GetRecordKeyFactory();
									using (var enumerator = queryResult.GetEnumerator())
									{
										if(enumerator.MoveNext())
										{
											var recordKey = recordKeyFactory.Create(recordTypeName, enumerator.Current);
											if(reference.Options.HasFlag(LoadOptions.ChildrenContentLazy))
												entities.Add(loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager));
											else
												entities.Add(loader.FullLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager));
										}
									}
								}
								catch(Exception ex)
								{
									var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadReferenceFromEntityError, reference.Name, typeof(Model).FullName, ex.Message).Format(), ex);
									exceptions.Enqueue(ex2);
									return;
								}
								finally
								{
									if(isLast)
										references.Add(new Tuple<string, IEntity>(reference.Name, entities.FirstOrDefault()));
								}
							}), new Colosoft.Query.QueryFailedCallBack((sender, info, errorResult) =>  {
								var ex2 = new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, reference.Name, typeof(Model).FullName, errorResult.Message.Format()).Format(), errorResult.Error);
								exceptions.Enqueue(ex2);
							}));
						}
					}
					else
					{
						references.Add(new Tuple<string, IEntity>(reference.Name, null));
					}
				}
				if(multiQuery != null && multiQuery.Count > 0)
				{
					try
					{
						multiQuery.Execute();
					}
					catch(Exception ex)
					{
						if(exceptions.Count == 0)
							exceptions.Enqueue(new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_FaillOnLoadChildrenFromEntity, typeof(Model).FullName, ex.Message).Format(), ex));
					}
				}
				if(exceptions.Count > 0)
					throw new AggregateException(exceptions.FirstOrDefault().Message, exceptions);
			}
			using (var args = new EntityLoaderCreatorArgs<Model>(dataModel, dataModelRecordKey.Key, new EntityLoaderChildContainer(children), new EntityLoaderLinksContainer(links), new EntityLoaderReferenceContainer(references), uiContext, entityTypeManager))
			{
				result = EntityCreator(args);
				if(result is IConnectedEntity)
					((IConnectedEntity)result).Connect(sourceContext);
				if(result is IEntityRecordObserver)
					((IEntityRecordObserver)result).RegisterObserver(dataModelRecordKey.Key);
				if(result is ILoadableEntity)
					((ILoadableEntity)result).NotifyLoaded();
				return result;
			}
		}

		/// <summary>
		/// Realiza a carda da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave do registro.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos dados do resultado.</param>
		/// <param name="objectCreator">Instancia responsável por criar objetos.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <returns></returns>
		public override IEntity LazyLoad(Colosoft.Query.IRecord record, Query.RecordKey recordKey, Colosoft.Query.IQueryResultBindStrategy bindStrategy, Colosoft.Query.IQueryResultObjectCreator objectCreator, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null)
		{
			var dataModel = objectCreator.Create();
			bindStrategy.Bind(record, Query.BindStrategyMode.All, ref dataModel);
			return LazyLoad(new DataModelRecordKey<Model>((Model)dataModel, recordKey), sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Realiza a carga tardia dos dados filhos da entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <returns></returns>
		public virtual IEnumerable<IEntity<Model>> GetLazyEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null)
		{
			foreach (var i in result)
				yield return LazyLoad(i, sourceContext, uiContext, entityTypeManager);
		}

		/// <summary>
		/// Realiza a carga da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="dataModelRecordKey">Instancia do modelo de dados da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public virtual IEntity<Model> LazyLoad(DataModelRecordKey<Model> dataModelRecordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null)
		{
			var dataModel = dataModelRecordKey.DataModel;
			IEntity<Model> result = null;
			var children = new List<Tuple<string, IEntity>>();
			var links = new List<Tuple<string, IEntity>>();
			var references = new List<Tuple<string, IEntity>>();
			if(_children.Count > 0)
			{
				Colosoft.Query.MultiQueryable multiQuery = null;
				if(sourceContext != null)
					multiQuery = sourceContext.CreateMultiQuery();
				var parentUid = _uidGetter != null ? _uidGetter(dataModel) : 0;
				foreach (var i in _children)
				{
					var child = i;
					if(child.IsSingle)
					{
						EntityInfoQuery[] queries = null;
						if(multiQuery != null)
							queries = child.CreateQueries(parentUid, dataModel, sourceContext);
						if(queries != null && queries.Length > 0)
						{
							for(var j = 0; j < queries.Length; j++)
							{
								var query = queries[j];
								var isLast = (j + 1) == queries.Length;
								multiQuery.Add(query.Query, (a, b, c) =>  {
									var loader = entityTypeManager.GetLoader(query.EntityType);
									var recordKeyFactory = loader.GetRecordKeyFactory();
									IEntity entity = null;
									using (var enumerator = c.GetEnumerator())
									{
										if(enumerator.MoveNext())
										{
											var recordKey = recordKeyFactory.Create(Colosoft.Reflection.TypeName.Get(loader.DataModelType), enumerator.Current);
											entity = loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
										}
									}
									var childIndex = children.FindIndex(f => f.Item1 == child.Name);
									if(childIndex >= 0 && entity != null)
									{
										children.RemoveAt(childIndex);
										childIndex = -1;
									}
									if(childIndex < 0)
										children.Add(new Tuple<string, IEntity>(child.Name, entity));
								});
							}
						}
						else
							children.Add(new Tuple<string, IEntity>(child.Name, null));
					}
					else
					{
						Lazy<IEnumerable<IEntity>> entities = null;
						if(sourceContext != null)
							entities = new Lazy<IEnumerable<IEntity>>(() =>  {
								var resultEntities = new List<IEntity>();
								var queries = child.CreateQueries(parentUid, dataModel, sourceContext);
								if(queries != null && queries.Length > 0)
								{
									var multiQuery2 = sourceContext.CreateMultiQuery();
									for(var j = 0; j < queries.Length; j++)
									{
										var query = queries[j];
										multiQuery2.Add(query.Query, (sender, queryInfo, queryResult) =>  {
											var loader = entityTypeManager.GetLoader(query.EntityType);
											resultEntities.AddRange(loader.GetLazyEntities(queryResult, sourceContext, uiContext, entityTypeManager));
										});
									}
									try
									{
										multiQuery2.Execute();
									}
									catch(Exception ex)
									{
										throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_LoadChildFromEntityError, child.Name, typeof(Model).FullName, ex.Message).Format(), ex);
									}
								}
								return resultEntities;
							});
						else
							entities = new Lazy<IEnumerable<IEntity>>(() => new IEntity[0]);
						var entity = CreateChildrenList(child.EntityType, entities, uiContext, entityTypeManager, new Action<IEntity>(e => child.ParentUidSetter(result, e)));
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(sourceContext);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
						children.Add(new Tuple<string, IEntity>(child.Name, entity));
					}
				}
				foreach (var i in _links)
				{
					var link = i;
					var child = _children.Where(f => f.Name == link.ChildName).FirstOrDefault();
					if(child == null)
						throw new EntityLoaderException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderOfModel_ChildOfLinkNotFound, link.ChildName, typeof(TEntity1).FullName, link.Name).Format());
					var loader = entityTypeManager.GetLoader(link.LinkEntityType);
					if(link.IsSingle)
					{
						if(multiQuery != null)
						{
							multiQuery.Add(link.CreateQuery(parentUid, child.ForeignPropertyName, sourceContext), (a, b, c) =>  {
								IEntity entity = null;
								using (var enumerator = c.GetEnumerator())
								{
									if(enumerator.MoveNext())
									{
										var recordKey = loader.GetRecordKeyFactory().Create(Colosoft.Reflection.TypeName.Get(loader.DataModelType), enumerator.Current);
										if(link.Options.HasFlag(LoadOptions.ChildrenContentLazy))
											entity = loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager, args);
										else
											entity = loader.FullLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
									}
								}
								var linkIndex = links.FindIndex(f => f.Item1 == link.Name);
								if(linkIndex >= 0 && entity != null)
								{
									links.RemoveAt(linkIndex);
									linkIndex = -1;
								}
								if(linkIndex < 0)
									links.Add(new Tuple<string, IEntity>(link.Name, entity));
							});
						}
						else
							links.Add(new Tuple<string, IEntity>(link.Name, null));
					}
					else
					{
						Lazy<IEnumerable<IEntity>> entities = null;
						if(sourceContext != null)
							entities = new Lazy<IEnumerable<IEntity>>(() =>  {
								var query = link.CreateQuery(parentUid, child.ForeignPropertyName, sourceContext);
								if(link.Options.HasFlag(LoadOptions.LinksContentLazy))
									return loader.GetLazyEntities(query.Execute(), sourceContext, uiContext, entityTypeManager, args);
								else
									return loader.GetFullEntities(query.Execute(), sourceContext, uiContext, entityTypeManager);
							});
						else
							entities = new Lazy<IEnumerable<IEntity>>(() => new IEntity[0]);
						var childInstance = (IEntityList)children.Where(f => f.Item1 == link.ChildName).Select(f => f.Item2).FirstOrDefault();
						var childLoader = entityTypeManager.GetLoader(_children.Where(f => f.Name == link.ChildName).FirstOrDefault().EntityType);
						var entity = CreateLinksList(entities, link, new EntityFromModelCreatorHandler(childLoader.Create), childInstance, uiContext, sourceContext, entityTypeManager, new Action<IEntity>(e => link.LinkParentForeignKeySetter(result, e)));
						if(entity is IConnectedEntity)
							((IConnectedEntity)entity).Connect(sourceContext);
						if(entity is ILoadableEntity)
							((ILoadableEntity)entity).NotifyLoaded();
						links.Add(new Tuple<string, IEntity>(link.Name, entity));
					}
				}
				foreach (var i in _references)
				{
					if(i.Value.IsLoadLazy)
						continue;
					var reference = i.Value;
					EntityInfoQuery[] queries = null;
					if(multiQuery != null)
						queries = reference.CreateQueries(dataModel, sourceContext);
					if(queries != null && queries.Length > 0)
					{
						for(var j = 0; j < queries.Length; j++)
						{
							var query = queries[j];
							var isLast = (j + 1) == queries.Length;
							multiQuery.Add(query.Query, (a, b, c) =>  {
								var loader = entityTypeManager.GetLoader(query.EntityType);
								var recordKeyFactory = loader.GetRecordKeyFactory();
								IEntity entity = null;
								using (var enumerator = c.GetEnumerator())
								{
									if(enumerator.MoveNext())
									{
										var recordKey = recordKeyFactory.Create(Colosoft.Reflection.TypeName.Get(loader.DataModelType), enumerator.Current);
										entity = loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
									}
								}
								var referenceIndex = references.FindIndex(f => f.Item1 == reference.Name);
								if(referenceIndex >= 0 && entity != null)
								{
									references.RemoveAt(referenceIndex);
									referenceIndex = -1;
								}
								if(referenceIndex < 0)
									references.Add(new Tuple<string, IEntity>(reference.Name, entity));
							});
						}
					}
					else
						references.Add(new Tuple<string, IEntity>(reference.Name, null));
				}
				if(multiQuery != null && multiQuery.Count > 0)
					multiQuery.Execute();
			}
			using (var creatorArgs = new EntityLoaderCreatorArgs<Model>(dataModel, dataModelRecordKey.Key, new EntityLoaderChildContainer(children), new EntityLoaderLinksContainer(links), new EntityLoaderReferenceContainer(references), uiContext, entityTypeManager))
			{
				result = EntityCreator(creatorArgs);
				if(result is IConnectedEntity)
					((IConnectedEntity)result).Connect(sourceContext);
				if(result is IEntityRecordObserver)
					((IEntityRecordObserver)result).RegisterObserver(dataModelRecordKey.Key);
				if(result is ILoadableEntity)
					((ILoadableEntity)result).NotifyLoaded();
				return result;
			}
		}

		/// <summary>
		/// Recupera a referencia da entidade.
		/// </summary>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="referenceName">Nome da referencia.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador dos tipos das entidade.s</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="isLazy">Identifica se é para fazer a carga em modo Lazy.</param>
		/// <returns></returns>
		public override IEntity GetEntityReference(IEntity parent, string referenceName, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext, bool isLazy)
		{
			parent.Require("parent").NotNull();
			entityTypeManager.Require("entityTypeManager").NotNull();
			sourceContext.Require("sourceContext").NotNull();
			EntityLoaderReference reference = null;
			if(!_references.TryGetValue(referenceName, out reference))
				throw new InvalidOperationException(string.Format("Reference '{0}' not found for entity.", referenceName));
			var dataModel = ((TEntity1)parent).DataModel;
			IEntity result = null;
			EntityInfoQuery[] queries = null;
			var multiQuery = sourceContext.CreateMultiQuery();
			queries = reference.CreateQueries(dataModel, sourceContext);
			if(queries != null && queries.Length > 0)
			{
				for(var j = 0; j < queries.Length; j++)
				{
					var query = queries[j];
					var isLast = (j + 1) == queries.Length;
					multiQuery.Add(query.Query, (a, b, c) =>  {
						var loader = entityTypeManager.GetLoader(query.EntityType);
						var recordKeyFactory = loader.GetRecordKeyFactory();
						IEntity entity = null;
						using (var enumerator = c.GetEnumerator())
						{
							if(enumerator.MoveNext())
							{
								var recordKey = recordKeyFactory.Create(Colosoft.Reflection.TypeName.Get(loader.DataModelType), enumerator.Current);
								if(isLazy)
									entity = loader.LazyLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
								else
									entity = loader.FullLoad(enumerator.Current, recordKey, sourceContext, uiContext, entityTypeManager);
							}
						}
						if(entity != null)
							result = entity;
					});
				}
			}
			multiQuery.Execute();
			return result;
		}

		/// <summary>
		/// Tenta recupera a referencia pelo nome da propriedade monitorada.
		/// </summary>
		/// <param name="propertyNames">Nome da propriedade monitorada.</param>
		/// <returns></returns>
		public override IEnumerable<EntityLoaderReference> GetReferenceByWatchedProperties(string[] propertyNames)
		{
			if(_referencesWatchedProperties.Count > 0 && propertyNames != null)
			{
				IEnumerable<EntityLoaderReference> result = null;
				List<EntityLoaderReference> references = null;
				foreach (var propertyName in propertyNames)
				{
					if(_referencesWatchedProperties.TryGetValue(propertyName, out references))
					{
						if(result == null)
							result = references;
						else
							result = result.Union(references);
					}
				}
				if(result != null)
					return result;
			}
			return EmptyReferences;
		}

		/// <summary>
		/// Tenta recupera a referencia pelo nome.
		/// </summary>
		/// <param name="referenceName"></param>
		/// <param name="reference"></param>
		/// <returns></returns>
		public override bool TryGetReference(string referenceName, out EntityLoaderReference reference)
		{
			return _references.TryGetValue(referenceName, out reference);
		}

		/// <summary>
		/// Recupera as referencias da entidade.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<EntityLoaderReference> GetReferences()
		{
			return _references.Values;
		}

		/// <summary>
		/// Clona os dados da entidade informada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override IEntity Clone(IEntity entity)
		{
			var entity1 = (IEntity<Model>)entity;
			var dataModel = (Model)entity1.DataModel.Clone();
			var children = new List<Tuple<string, IEntity>>();
			var links = new List<Tuple<string, IEntity>>();
			var references = new List<Tuple<string, IEntity>>();
			foreach (var i in _children)
			{
				var child = i.ParentValueGetter(entity);
				if(child != null)
				{
					child = (IEntity)child.Clone();
					child.Owner = null;
				}
				children.Add(new Tuple<string, IEntity>(i.Name, child));
			}
			foreach (var i in _links)
			{
				var link = i.ParentValueGetter(entity);
				if(link != null)
				{
					if(link is IEntityLink)
					{
						var child = children.Where(f => f.Item1 == i.ChildName).FirstOrDefault();
						if(child == null)
							throw new EntityLoaderException(string.Format("Not found child '{0}' for link '{1}'", i.ChildName, i.Name));
						link = (IEntity)((IEntityLink)link).Clone(child.Item2);
					}
					else
						link = (IEntity)link.Clone();
					link.Owner = null;
				}
				links.Add(new Tuple<string, IEntity>(i.Name, link));
			}
			foreach (var i in _references)
			{
				if(i.Value.IsLoadLazy)
					continue;
				var reference = i.Value.ParentValueGetter(entity);
				if(reference != null)
				{
					reference = (IEntity)reference.Clone();
					reference.Owner = null;
				}
				references.Add(new Tuple<string, IEntity>(i.Value.Name, reference));
			}
			var recordKey = dataModel.CreateRecordKey(GetRecordKeyFactory());
			using (var creatorArgs = new EntityLoaderCreatorArgs<Model>(dataModel, recordKey, new EntityLoaderChildContainer(children), new EntityLoaderLinksContainer(links), new EntityLoaderReferenceContainer(references), entity1.UIContext, entity1.TypeManager))
			{
				var result = EntityCreator(creatorArgs);
				if(result is IConnectedEntity)
					((IConnectedEntity)result).Connect(((IConnectedEntity)entity).SourceContext);
				if(result is IEntityRecordObserver)
					((IEntityRecordObserver)result).RegisterObserver(recordKey);
				if(result is ILoadableEntity)
					((ILoadableEntity)result).NotifyLoaded();
				result.Owner = entity1.Owner;
				return result;
			}
		}

		/// <summary>
		/// Copia os dados da entidade de origem para entidade de destino.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public override void Copy(IEntity source, IEntity destination)
		{
			source.Require("source").NotNull();
			destination.Require("destionation").NotNull();
			var source1 = (IEntity<Model>)source;
			var destination1 = (IEntity<Model>)destination;
			destination1.CopyFromDataModel(source1.DataModel);
			var copyFromStates = new Queue<IDisposable>();
			try
			{
				foreach (var i in _links)
				{
					var sourceValue = i.ParentValueGetter(source);
					var destinationValue = i.ParentValueGetter(destination);
					if(destinationValue is IEntityLink)
					{
						copyFromStates.Enqueue(((IEntityLink)destinationValue).GetCopyFromStateControl());
					}
					if(i.ParentValueSetter == null && destinationValue != null)
					{
						destinationValue.CopyFrom(sourceValue);
					}
					else if(i.ParentValueSetter != null)
						i.ParentValueSetter(destination, sourceValue);
				}
				foreach (var i in _children)
				{
					var sourceValue = i.ParentValueGetter(source);
					var destinationValue = i.ParentValueGetter(destination);
					if(i.ParentValueSetter == null && destinationValue != null)
						destinationValue.CopyFrom(sourceValue);
					else if(i.ParentValueSetter != null)
					{
						i.ParentValueSetter(destination, sourceValue);
						if(sourceValue != null && !sourceValue.IsChanged && (destinationValue != null && !destinationValue.IsChanged) && sourceValue.Equals(destinationValue))
							destination.IgnoreChanges(i.PropertyName);
					}
				}
			}
			finally
			{
				while (copyFromStates.Count > 0)
					copyFromStates.Dequeue().Dispose();
			}
		}

		/// <summary>
		/// Recupera o registro que representa a chave da entidade.
		/// </summary>
		/// <param name="propertyGetter">Getter para recuperar o valor da propriedade.</param>
		/// <returns></returns>
		public override Colosoft.Query.IRecord GetRecordOfKey(Func<string, object> propertyGetter)
		{
			var descriptor = KeyRecordDescriptor;
			if(descriptor.Count == 0)
				return descriptor.CreateRecord(new object[0]);
			object[] values = null;
			if(this.HasUid)
			{
				if(descriptor.Contains("RowVersion"))
					values = new object[] {
						propertyGetter("Uid"),
						propertyGetter("RowVersion")
					};
				else
					values = new object[] {
						propertyGetter("Uid")
					};
			}
			else
			{
				values = new object[descriptor.Count];
				for(var i = 0; i < descriptor.Count; i++)
					values[i] = propertyGetter(descriptor[i].Name);
			}
			var record = descriptor.CreateRecord(values);
			return record;
		}

		/// <summary>
		/// Recupera a chave de registro associada com a entidade.
		/// </summary>
		/// <param name="propertyGetter">Getter para recuperar o valor da propriedade</param>
		/// <returns></returns>
		public override Colosoft.Query.RecordKey GetRecordKey(Func<string, object> propertyGetter)
		{
			var record = GetRecordOfKey(propertyGetter);
			if(record.Descriptor.Count == 0)
				return new Colosoft.Query.RecordKey(string.Empty, 0);
			var keyFactory = GetRecordKeyFactory();
			return keyFactory.Create(ModelTypeName, GetRecordOfKey(propertyGetter));
		}

		/// <summary>
		/// Recupera a chave de registro associada com a entidade.
		/// </summary>
		/// <param name="entity">Instancia da entidade de onde os dados serão recuperados.</param>
		/// <returns></returns>
		public override Colosoft.Query.RecordKey GetRecordKey(IEntity entity)
		{
			var descriptor = KeyRecordDescriptor;
			if(descriptor.Count == 0)
				return new Colosoft.Query.RecordKey(string.Empty, 0);
			var keyFactory = GetRecordKeyFactory();
			object[] values = null;
			if(this.HasUid)
			{
				if(descriptor.Contains("RowVersion"))
					values = new object[] {
						entity.Uid,
						entity.RowVersion
					};
				else
					values = new object[] {
						entity.Uid
					};
			}
			else
			{
				Tuple<Func<Model, object>, Action<Model, object>> getterAndSetter = null;
				values = new object[descriptor.Count];
				for(var i = 0; i < descriptor.Count; i++)
					if(_keysGettersAndSetters.TryGetValue(descriptor[i].Name, out getterAndSetter))
					{
						values[i] = getterAndSetter.Item1(((IEntity<Model>)entity).DataModel);
					}
			}
			var record = descriptor.CreateRecord(values);
			return keyFactory.Create(ModelTypeName, record);
		}

		/// <summary>
		/// Recupera os getters dos filhos da entidade.
		/// </summary>
		/// <param name="savePriority">Prioridade dos filhos</param>
		/// <returns></returns>
		public override IEnumerable<IEntityAccessor> GetChildrenAccessors(EntityChildSavePriority? savePriority = null)
		{
			IEnumerable<IEntityLoaderChildInfo> result = _children;
			if(savePriority != null)
				result = _children.Where(f => f.SavePriority == savePriority.Value);
			return result;
		}

		/// <summary>
		/// Recupera os getters dos links da entidade.
		/// </summary>
		/// <param name="savePriority">Prioridade dos filhos que serão recuperados</param>
		/// <returns></returns>
		public override IEnumerable<IEntityAccessor> GetLinksAccessors(EntityChildSavePriority? savePriority = null)
		{
			foreach (var i in _links)
				if(savePriority == null || i.SavePriority == savePriority.GetValueOrDefault())
					yield return i;
		}

		/// <summary>
		/// Registra o filho da entidade.
		/// </summary>
		/// <param name="parent">Instancia do pai.</param>
		/// <param name="child">Instancia do filho.</param>
		/// <param name="childName">Nome do filho.</param>
		public override void RegisterChild(IEntity parent, IEntity child, string childName)
		{
			if(parent == null || child == null)
				return;
			var childInfo = _children.Where(f => f.Name == childName).FirstOrDefault();
			if(childInfo != null)
			{
				child.Owner = parent;
				if(HasUid || HasKeys)
					try
					{
						childInfo.ParentUidSetter(parent, child);
					}
					catch(NotSupportedException)
					{
					}
			}
		}

		/// <summary>
		/// Cria uma a instancia de uma filho
		/// </summary>
		/// <typeparam name="TChild">Tipo da entidade filho.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome do filho.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Contexto da origem dos dados.</param>
		/// <returns></returns>
		public override TChild CreateChild<TChild>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext)
		{
			parent.Require("parent").NotNull();
			entityTypeManager.Require("entityTypeManager").NotNull();
			var child = _children.Where(f => f.Name == name).FirstOrDefault();
			if(child == null)
				throw new InvalidOperationException(string.Format("Child '{0}' of type '{1}' not found for entity.", name, typeof(TChild).FullName));
			var type = child.EntityType;
			if(!child.IsSingle)
			{
				var listConstructor = typeof(EntityChildrenList<>).MakeGenericType(type).GetConstructor(new Type[] {
					typeof(string),
					typeof(Action<>).MakeGenericType(type),
					typeof(IEntityTypeManager)
				});
				var parentUidSetter = typeof(EntityLoader<TEntity1, Model>).GetMethod("ParentUidSetterWrapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(type).Invoke(null, new object[] {
					new Action<IEntity>(e => child.ParentUidSetter(parent, e))
				});
				var result = (TChild)listConstructor.Invoke(new object[] {
					uiContext,
					parentUidSetter,
					entityTypeManager
				});
				if(parent != null)
					result.Owner = parent;
				if(result is IConnectedEntity)
					((IConnectedEntity)result).Connect(sourceContext);
				return result;
			}
			if(!type.IsInterface)
			{
				var childLoader = entityTypeManager.GetLoader(type);
				TChild returnValue = (TChild)childLoader.Create(uiContext, entityTypeManager, sourceContext);
				if(parent != null)
					returnValue.Owner = parent;
				if(returnValue is IConnectedEntity)
					((IConnectedEntity)returnValue).Connect(sourceContext);
				if(returnValue is ILoadableEntity)
					((ILoadableEntity)returnValue).NotifyLoaded();
				return returnValue;
			}
			else
				return default(TChild);
		}

		/// <summary>
		/// Cria uma instancia de um link.
		/// </summary>
		/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome do link.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Contexto da origem dos dados</param>
		/// <returns></returns>
		public override TLink CreateLink<TLink>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext)
		{
			parent.Require("parent").NotNull();
			entityTypeManager.Require("entityTypeManager").NotNull();
			var link = _links.Where(f => f.Name == name).FirstOrDefault();
			if(link == null)
				throw new InvalidOperationException(string.Format("Link '{0}' of type '{1}' not found for entity.", name, typeof(TLink).FullName));
			var child = _children.Where(f => f.Name == link.ChildName).FirstOrDefault();
			if(child == null)
				throw new InvalidOperationException(string.Format("Child '{0}' not found for entity.", link.ChildName));
			var childLoader = entityTypeManager.GetLoader(child.EntityType);
			var type = link.LinkEntityType;
			if(!link.IsSingle)
			{
				var listConstructor = typeof(EntityLinksList<>).MakeGenericType(type).GetConstructor(new Type[] {
					typeof(IEntityList),
					typeof(EntityLoaderLinkInfo),
					typeof(EntityFromModelCreatorHandler),
					typeof(string),
					typeof(Action<>).MakeGenericType(type),
					typeof(Colosoft.Query.ISourceContext),
					typeof(IEntityTypeManager)
				});
				var parentUidSetter = typeof(EntityLoader<TEntity1, Model>).GetMethod("ParentUidSetterWrapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(type).Invoke(null, new object[] {
					new Action<IEntity>(e => link.LinkParentForeignKeySetter(parent, e))
				});
				var childInstance = child.ParentValueGetter(parent);
				if(childInstance == null)
					throw new EntityLoaderException(string.Format("Child instance '{0}' not found to link '{1}'", child.Name, name));
				TLink result = default(TLink);
				try
				{
					result = (TLink)listConstructor.Invoke(new object[] {
						childInstance,
						link,
						new EntityFromModelCreatorHandler(childLoader.Create),
						uiContext,
						parentUidSetter,
						null,
						entityTypeManager
					});
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				if(parent != null)
					result.Owner = parent;
				if(result is IConnectedEntity)
					((IConnectedEntity)result).Connect(sourceContext);
				if(result is ILoadableEntity)
					((ILoadableEntity)result).NotifyLoaded();
				return result;
			}
			TLink returnValue = (TLink)childLoader.Create(uiContext, entityTypeManager, sourceContext);
			if(parent != null)
				returnValue.Owner = parent;
			if(returnValue is IConnectedEntity)
				((IConnectedEntity)returnValue).Connect(sourceContext);
			if(returnValue is ILoadableEntity)
				((ILoadableEntity)returnValue).NotifyLoaded();
			return returnValue;
		}

		/// <summary>
		/// Cria uma instancia de uma referencia.
		/// </summary>
		/// <typeparam name="TReference">Tipo da referencia.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome da referencia.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Origem do contexto dos dados.</param>
		/// <returns></returns>
		public override TReference CreateReference<TReference>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext)
		{
			parent.Require("parent").NotNull();
			entityTypeManager.Require("entityTypeManager").NotNull();
			EntityLoaderReference reference = null;
			if(!_references.TryGetValue(name, out reference))
				throw new InvalidOperationException(string.Format("Reference '{0}' of type '{1}' not found for entity.", name, typeof(TReference).FullName));
			var referenceLoader = entityTypeManager.GetLoader(reference.EntityType);
			var type = reference.EntityType;
			TReference returnValue = (TReference)referenceLoader.Create(uiContext, entityTypeManager, sourceContext);
			if(parent != null)
				returnValue.Owner = parent;
			if(returnValue is IConnectedEntity)
				((IConnectedEntity)returnValue).Connect(sourceContext);
			if(returnValue is ILoadableEntity)
				((ILoadableEntity)returnValue).NotifyLoaded();
			return returnValue;
		}

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		public override IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext)
		{
			var dataModel = (Model)GetObjectCreator().Create();
			return Create(uiContext, entityTypeManager, dataModel, sourceContext);
		}

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="dataModel">Modelo de dados que será usado com base.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Data.IModel dataModel, Colosoft.Query.ISourceContext sourceContext)
		{
			entityTypeManager.Require("entityTypeManager").NotNull();
			dataModel.Require("dataModel").NotNull();
			IEntity parent = null;
			var children = new List<Tuple<string, IEntity>>();
			var links = new List<Tuple<string, IEntity>>();
			var references = new List<Tuple<string, IEntity>>();
			foreach (var i in _children)
			{
				IEntityLoaderChildInfo childInfo = i;
				IEntity child = null;
				var type = childInfo.EntityType;
				if(!childInfo.IsSingle)
				{
					var listConstructor = typeof(EntityChildrenList<>).MakeGenericType(type).GetConstructor(new Type[] {
						typeof(string),
						typeof(Action<>).MakeGenericType(type),
						typeof(IEntityTypeManager)
					});
					var parentUidSetter = typeof(EntityLoader<TEntity1, Model>).GetMethod("ParentUidSetterWrapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(type).Invoke(null, new object[] {
						new Action<IEntity>(e => childInfo.ParentUidSetter(parent, e))
					});
					child = (IEntity)listConstructor.Invoke(new object[] {
						uiContext,
						parentUidSetter,
						entityTypeManager
					});
				}
				else
				{
					var loader = entityTypeManager.GetLoader(childInfo.EntityType);
					child = loader.Create(uiContext, entityTypeManager, sourceContext);
				}
				if(child is IConnectedEntity)
					((IConnectedEntity)child).Connect(sourceContext);
				if(child is ILoadableEntity)
					((ILoadableEntity)child).NotifyLoaded();
				children.Add(new Tuple<string, IEntity>(childInfo.Name, child));
			}
			foreach (var i in _links)
			{
				EntityLoaderLinkInfo linkInfo = i;
				IEntity link = null;
				var type = linkInfo.LinkEntityType;
				if(!linkInfo.IsSingle)
				{
					var listConstructor = typeof(EntityLinksList<>).MakeGenericType(type).GetConstructor(new Type[] {
						typeof(IEntityList),
						typeof(EntityLoaderLinkInfo),
						typeof(EntityFromModelCreatorHandler),
						typeof(string),
						typeof(Action<>).MakeGenericType(type),
						typeof(Colosoft.Query.ISourceContext),
						typeof(IEntityTypeManager)
					});
					var parentUidSetter = typeof(EntityLoader<TEntity1, Model>).GetMethod("ParentUidSetterWrapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(type).Invoke(null, new object[] {
						new Action<IEntity>(e => linkInfo.LinkParentForeignKeySetter(parent, e))
					});
					var childInstance = _children.Where(f => f.Name == linkInfo.ChildName).FirstOrDefault();
					var childLoader = entityTypeManager.GetLoader(childInstance.EntityType);
					link = (IEntity)listConstructor.Invoke(new object[] {
						childInstance,
						linkInfo,
						new EntityFromModelCreatorHandler(childLoader.Create),
						uiContext,
						parentUidSetter,
						sourceContext,
						entityTypeManager
					});
				}
				else
				{
					var loader = entityTypeManager.GetLoader(linkInfo.LinkEntityType);
					link = loader.Create(uiContext, entityTypeManager, sourceContext);
				}
				if(link is IConnectedEntity)
					((IConnectedEntity)link).Connect(sourceContext);
				if(link is ILoadableEntity)
					((ILoadableEntity)link).NotifyLoaded();
				links.Add(new Tuple<string, IEntity>(linkInfo.Name, link));
			}
			foreach (var i in _references)
			{
				if(i.Value.IsLoadLazy)
					continue;
				references.Add(new Tuple<string, IEntity>(i.Value.Name, null));
			}
			var recordKey = dataModel.CreateRecordKey(GetRecordKeyFactory());
			using (var args = new EntityLoaderCreatorArgs<Model>((Model)dataModel, recordKey, new EntityLoaderChildContainer(children), new EntityLoaderLinksContainer(links), new EntityLoaderReferenceContainer(references), uiContext, entityTypeManager))
			{
				parent = EntityCreator(args);
				if(parent is IConnectedEntity)
					((IConnectedEntity)parent).Connect(sourceContext);
				if(parent is IEntityRecordObserver)
					((IEntityRecordObserver)parent).RegisterObserver(recordKey);
				if(parent is ILoadableEntity)
					((ILoadableEntity)parent).NotifyLoaded();
				return parent;
			}
		}

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="creatorArgs">Argumentos que serão usadaos na crição da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		public override IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderCreatorArgs creatorArgs, Colosoft.Query.ISourceContext sourceContext)
		{
			entityTypeManager.Require("entityTypeManager").NotNull();
			creatorArgs.Require("creatorArgs").NotNull();
			using (var args = new EntityLoaderCreatorArgs<Model>((Model)creatorArgs.DataModel, creatorArgs.RecordKey, creatorArgs.Children, creatorArgs.Links, creatorArgs.References, uiContext, entityTypeManager))
			{
				var entity = EntityCreator(args);
				if(entity is IConnectedEntity)
					((IConnectedEntity)entity).Connect(sourceContext);
				if(entity is IEntityRecordObserver)
					((IEntityRecordObserver)entity).RegisterObserver(args.RecordKey);
				if(entity is ILoadableEntity)
					((ILoadableEntity)entity).NotifyLoaded();
				return entity;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_bindStrategy != null && _bindStrategy is IDisposable)
				((IDisposable)_bindStrategy).Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	/// <summary>
	/// Classe que armazena os argumentos para a criação de uma entidade.
	/// </summary>
	public class EntityLoaderCreatorArgs<Model> : IDisposable where Model : class, Colosoft.Data.IModel
	{
		private Model _dataModel;

		private Query.RecordKey _recordKey;

		private EntityLoaderChildContainer _children;

		private IEntityLoaderLinksContainer _links;

		private EntityLoaderReferenceContainer _references;

		private string _uiContext;

		private IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Instancia do modelo de dados.
		/// </summary>
		public Model DataModel
		{
			get
			{
				return _dataModel;
			}
		}

		/// <summary>
		/// Chave que representa o registro associado.
		/// </summary>
		public Query.RecordKey RecordKey
		{
			get
			{
				return _recordKey;
			}
		}

		/// <summary>
		/// Container dos filhos.
		/// </summary>
		public EntityLoaderChildContainer Children
		{
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// Container dos links.
		/// </summary>
		public IEntityLoaderLinksContainer Links
		{
			get
			{
				return _links;
			}
		}

		/// <summary>
		/// Container das referencias.
		/// </summary>
		public EntityLoaderReferenceContainer References
		{
			get
			{
				return _references;
			}
		}

		/// <summary>
		/// Contexto visual.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
		}

		/// <summary>
		/// Gerenciador de tipos.
		/// </summary>
		public IEntityTypeManager TypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="recordKey">Chave do registro associado.</param>
		/// <param name="children"></param>
		/// <param name="linkContainer"></param>
		/// <param name="references"></param>
		/// <param name="uiContext"></param>
		/// <param name="typeManager"></param>
		public EntityLoaderCreatorArgs(Model dataModel, Query.RecordKey recordKey, EntityLoaderChildContainer children, IEntityLoaderLinksContainer linkContainer, EntityLoaderReferenceContainer references, string uiContext, IEntityTypeManager typeManager)
		{
			_dataModel = dataModel;
			_recordKey = recordKey;
			_children = children;
			_links = linkContainer;
			_references = references;
			_uiContext = uiContext;
			_entityTypeManager = typeManager;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoaderCreatorArgs()
		{
			Dispose(false);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_children.Dispose();
			_links.Dispose();
			_references.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
