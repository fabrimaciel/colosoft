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
	/// Armazena as informações de um link da entidade.
	/// </summary>
	public abstract class EntityLoaderLinkInfo : IEntityAccessor
	{
		private string _name;

		private string _childName;

		private LoadOptions _options;

		/// <summary>
		/// Nome da propriedade estrangeira que se liga com o filho associado.
		/// </summary>
		private string _foreignPropertyName;

		/// <summary>
		/// Nome da propriedade do filho associado com o link.
		/// </summary>
		private string _childPropertyName;

		private Action<IEntity, IEntity> _parentValueSetter;

		private Func<IEntity, IEntity> _parentValueGetter;

		private Action<IEntity, IEntity> _linkParentForeignKeySetter;

		private Func<Colosoft.Data.IModel, int> _linkParentForeignKeyGetter;

		private Func<Colosoft.Data.IModel, int> _childKeyGetter;

		private bool _isSingle;

		private EntityChildSavePriority _savePriority;

		private EntityFromLinkCreatorHandler _entityFromLinkCreator;

		private LinkCreatorHandler _linkCreator;

		/// <summary>
		/// Nome do link.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Nome do filho no qual o link está associado.
		/// </summary>
		public string ChildName
		{
			get
			{
				return _childName;
			}
		}

		/// <summary>
		/// Opções de carga para o link.
		/// </summary>
		public LoadOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Nome da propriedade estrangeira que se liga com o filho associado.
		/// </summary>
		public string ForeignPropertyName
		{
			get
			{
				return _foreignPropertyName;
			}
		}

		/// <summary>
		/// Nome da propriedade do filho associado.
		/// </summary>
		public string ChildPropertyName
		{
			get
			{
				return _childPropertyName;
			}
		}

		/// <summary>
		/// Tipo do modelo de dados do link.
		/// </summary>
		public abstract Type LinkDataModelType
		{
			get;
		}

		/// <summary>
		/// Tipo da entidade do link.
		/// </summary>
		public abstract Type LinkEntityType
		{
			get;
		}

		/// <summary>
		/// Tipo do modelo da dados do filho associado.
		/// </summary>
		public abstract Type ChildDataModelType
		{
			get;
		}

		/// <summary>
		/// Identifica se é o link para uma entidade simples.
		/// </summary>
		public bool IsSingle
		{
			get
			{
				return _isSingle;
			}
		}

		/// <summary>
		/// Prioridade para salvar o filho.
		/// </summary>
		public EntityChildSavePriority SavePriority
		{
			get
			{
				return _savePriority;
			}
		}

		/// <summary>
		/// Instancia responsável por define para o filho o identificador do pai.
		/// </summary>
		public Action<IEntity, IEntity> LinkParentForeignKeySetter
		{
			get
			{
				return _linkParentForeignKeySetter;
			}
		}

		/// <summary>
		/// Instancia responsável por recupera o valor pai.
		/// </summary>
		public Func<IEntity, IEntity> ParentValueGetter
		{
			get
			{
				return _parentValueGetter;
			}
		}

		/// <summary>
		/// Delegate usado para definir o valor da instancia do pai.
		/// </summary>
		public Action<IEntity, IEntity> ParentValueSetter
		{
			get
			{
				return _parentValueSetter;
			}
		}

		/// <summary>
		/// Delegate usado para recuperar a chave unica do pai.
		/// </summary>
		public Func<Colosoft.Data.IModel, int> LinkParentForeignKeyGetter
		{
			get
			{
				return _linkParentForeignKeyGetter;
			}
		}

		/// <summary>
		/// Instancia da func usada para criar o link com base no modelo de dados 
		/// usando uma consulta sobre o SourceContext.
		/// </summary>
		public LinkCreatorHandler LinkCreator
		{
			get
			{
				return _linkCreator;
			}
		}

		/// <summary>
		/// Instancia da func usada para criar o entity com base no link.
		/// </summary>
		public EntityFromLinkCreatorHandler EntityFromLinkCreator
		{
			get
			{
				return _entityFromLinkCreator;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected EntityLoaderLinkInfo()
		{
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <param name="childName">Nome do filho associado com o link.</param>
		/// <param name="linkParentForeignKeySetter"></param>
		/// <param name="linkParentForeignKeyGetter">Instancia usada para recuperar o valor da chave do pai do link.</param>
		/// <param name="childKeyGetter">Instancia usada para recuperar o valor da chave do filho que está no link.</param>
		/// <param name="parentValueGetter"></param>
		/// <param name="parentValueSetter"></param>
		/// <param name="foreignProperty"></param>
		/// <param name="childProperty">Dados da propriedade do link usada para associa-lo com filho.</param>
		/// <param name="linkCreator">Func usada para criar uma instancia do link associado com entidade do filho associado.</param>
		/// <param name="entityFromLinkCreator">Func usada para criar uma instancia do filho baseado na entidade do link.</param>
		/// <param name="isSingle"></param>
		/// <param name="options">Opções de carga do link.</param>
		/// <param name="savePriority"></param>
		protected void Initialize(string name, string childName, Action<IEntity, IEntity> linkParentForeignKeySetter, Func<Colosoft.Data.IModel, int> linkParentForeignKeyGetter, Func<Colosoft.Data.IModel, int> childKeyGetter, Func<IEntity, IEntity> parentValueGetter, Action<IEntity, IEntity> parentValueSetter, System.Reflection.PropertyInfo foreignProperty, System.Reflection.PropertyInfo childProperty, LinkCreatorHandler linkCreator, EntityFromLinkCreatorHandler entityFromLinkCreator, bool isSingle, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity)
		{
			_name = name;
			_childName = childName;
			_foreignPropertyName = foreignProperty.Name;
			_childPropertyName = childProperty.Name;
			_options = options;
			_linkParentForeignKeySetter = linkParentForeignKeySetter;
			_linkParentForeignKeyGetter = linkParentForeignKeyGetter;
			_childKeyGetter = childKeyGetter;
			_parentValueGetter = parentValueGetter;
			_parentValueSetter = parentValueSetter;
			_entityFromLinkCreator = entityFromLinkCreator;
			_linkCreator = linkCreator;
			_isSingle = isSingle;
			_savePriority = savePriority;
		}

		/// <summary>
		/// Compara a instancia de dados do link com a instancia de dados do filho.
		/// </summary>
		/// <param name="link">Instancia dos dados do link.</param>
		/// <param name="child">Instancia dos dados do filho.</param>
		/// <returns></returns>
		public bool Equals(Colosoft.Data.IModel link, Colosoft.Data.IModel child)
		{
			var childId = this._childKeyGetter(link);
			var linkId = this._linkParentForeignKeyGetter(child);
			return childId == linkId;
		}

		/// <summary>
		/// Cria a consulta para recuperar os itens filhos.
		/// </summary>
		/// <param name="parentUid">Identificador da entidade pai.</param>
		/// <param name="parentUidProperty"></param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <returns></returns>
		public Colosoft.Query.Queryable CreateQuery(int parentUid, string parentUidProperty, Colosoft.Query.ISourceContext sourceContext)
		{
			return sourceContext.CreateQuery().From(new Query.EntityInfo(LinkDataModelType.FullName, "t1")).Join(ChildDataModelType.FullName, Query.JoinType.Inner, Query.ConditionalContainer.Parse(string.Format("t1.[{0}] == t2.[{1}]", ChildPropertyName, ForeignPropertyName)), "t2").Where(string.Format("t2.[{0}] == ?parentUid", parentUidProperty)).Add("?parentUid", parentUid);
		}

		/// <summary>
		/// Cria as consulta para recupera os itens.
		/// </summary>
		/// <param name="queryable">Consulta do item pai.</param>
		/// <param name="parentUidProperty">Nome da propriedade Uid do pai.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="exceptions">Fila do erros ocorridos.</param>
		/// <param name="entityTypeManager">Instancia do gereciador de tipos da entidade de negócio.</param>
		/// <param name="callBack"></param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void CreateQueries(Colosoft.Query.Queryable queryable, string parentUidProperty, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack)
		{
			var loader = entityTypeManager.GetLoader(LinkEntityType);
			var result = new List<EntityLoaderCreatorArgs>();
			var query = queryable.BeginSubQuery((sender, e) =>  {
				var bindStrategy = loader.GetBindStrategy();
				var objectCreator = loader.GetObjectCreator();
				var recordKeyFactory = loader.GetRecordKeyFactory();
				var dataModelTypeName = Colosoft.Reflection.TypeName.Get(loader.DataModelType);
				result.Clear();
				foreach (var record in e.Result)
				{
					var data = objectCreator.Create();
					if(!bindStrategy.Bind(record, Query.BindStrategyMode.All, ref data).Any())
						throw new Exception(string.Format("Not found scheme for bind record data to type '{0}'", data.GetType().FullName));
					var recordKey = recordKeyFactory.Create(dataModelTypeName, record);
					result.Add(new EntityLoaderCreatorArgs((Data.IModel)data, recordKey, new EntityLoaderChildContainer(), new EntityLoaderLinksContainer(), new EntityLoaderReferenceContainer(), uiContext, entityTypeManager));
				}
				var entities = new List<IEntity>();
				foreach (var i in result)
				{
					IEntity entity = null;
					LazyDataState lazyDataState = new LazyDataState();
					loader.GetLazyData(i, lazyDataState, queryable.SourceContext, uiContext, entityTypeManager, exceptions);
					entity = loader.Create(uiContext, entityTypeManager, i, queryable.SourceContext);
					lazyDataState.Entity = entity;
					entities.Add(entity);
					i.Dispose();
				}
				var entitiesEnumerable = entities;
				callBack(this, new EntityLoaderExecuteQueryEventArgs(LinkDataModelType, LinkEntityType, e.ReferenceValues, entitiesEnumerable));
			}, failedCallBack).From(new Query.EntityInfo(LinkDataModelType.FullName, "t1"));
			query.Join(ChildDataModelType.FullName, Query.JoinType.Inner, Query.ConditionalContainer.Parse(string.Format("t1.{0} == t2.{1}", ChildPropertyName, ForeignPropertyName)), "t2");
			query.Where(string.Format("t2.{0} == ?parentUid", parentUidProperty));
			query.Add("?parentUid", new Query.ReferenceParameter(parentUidProperty));
			loader.CreateNestedQueries(query, uiContext, entityTypeManager, result, exceptions);
			query.EndSubQuery();
		}

		/// <summary>
		/// Recupera a instancia do filho associado com a entidade pai informada.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		IEntity IEntityAccessor.Get(IEntity parent)
		{
			return ParentValueGetter(parent);
		}

		/// <summary>
		/// TODO: Implementar
		/// </summary>
		class EntityLinkEnumerable : QueryResultEnumerable<IEntity>
		{
			private EntityLoaderLinkInfo _linkInfo;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerable">Instancia do Enumerable que será adaptada.</param>
			/// <param name="linkInfo"></param>
			public EntityLinkEnumerable(IEnumerable<IEntity> enumerable, EntityLoaderLinkInfo linkInfo) : base(enumerable, null, null)
			{
				_linkInfo = linkInfo;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="typeName"></param>
			/// <param name="collection"></param>
			/// <returns></returns>
			protected override Collections.INotifyCollectionChangedObserver CreateQueryResultChangedObserver(Reflection.TypeName typeName, System.Collections.IList collection)
			{
				if(Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
					throw new NotImplementedException();
				return null;
			}

			/// <summary>
			/// Recupera o nome do tipo.
			/// </summary>
			/// <returns></returns>
			protected override Reflection.TypeName GetTypeName()
			{
				return Colosoft.Reflection.TypeName.Get(_linkInfo.LinkDataModelType);
			}
		}
	}
}
