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
	/// Armazena as informações de um filho da entidade de dados.
	/// </summary>
	abstract class EntityLoaderChildInfo : IEntityLoaderChildInfo
	{
		private string _name;

		private string _propertyName;

		private string _foreignPropertyName;

		private Action<IEntity, IEntity> _parentUidSetter;

		private Action<Data.IModel, IEntity> _parentUidSetter2;

		private Func<IEntity, IEntity> _parentValueGetter;

		private Action<IEntity, IEntity> _parentValueSetter;

		private Func<Colosoft.Data.IModel, int> _parentUidGetter;

		private LoadOptions _options;

		/// <summary>
		/// Instancia do loader do pai.
		/// </summary>
		private IEntityLoader _parentLoader;

		/// <summary>
		/// Instancia do loader da entidade.
		/// </summary>
		private IEntityLoader _entityLoader;

		private ConditionalLoader _conditional;

		private bool _isSingle;

		private EntityChildSavePriority _savePriority;

		/// <summary>
		/// Nome da propriedade de associação.
		/// </summary>
		public string ForeignPropertyName
		{
			get
			{
				return _foreignPropertyName;
			}
		}

		/// <summary>
		/// Nome do filho.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Nome da propriedade do filho na entidade.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		public abstract Type DataModelType
		{
			get;
		}

		/// <summary>
		/// Tipo da entidade.
		/// </summary>
		public abstract Type EntityType
		{
			get;
		}

		/// <summary>
		/// Identifica se é apena um filho.
		/// </summary>
		public bool IsSingle
		{
			get
			{
				return _isSingle;
			}
		}

		/// <summary>
		/// Opções de carga.
		/// </summary>
		public LoadOptions Options
		{
			get
			{
				return _options;
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
		public Action<IEntity, IEntity> ParentUidSetter
		{
			get
			{
				return _parentUidSetter;
			}
		}

		/// <summary>
		/// Delegate responsáveç por definir para o filho o identificador do pai.
		/// </summary>
		public Action<Data.IModel, IEntity> ParentUidSetter2
		{
			get
			{
				return _parentUidSetter2;
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
		/// Delegate usado para recuperar o valor da instancia do pai.
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
		public Func<Colosoft.Data.IModel, int> ParentUidGetter
		{
			get
			{
				return _parentUidGetter;
			}
		}

		/// <summary>
		/// Func usado para carregar a condicional de carga.
		/// </summary>
		public ConditionalLoader Conditional
		{
			get
			{
				return _conditional;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="propertyName">Nome da propriedade do filho na entidade.</param>
		/// <param name="parentUidSetter">Delegate usado para define o identificador unico do pai.</param>
		/// <param name="parentUidGetter">Delegate usado para recupera o identificador unico do pai.</param>
		/// <param name="parentValueGetter">Delegate usado para recuperar o valor do pai.</param>
		/// <param name="parentValueSetter">Delegate usado para definir o valor do pai.</param>
		/// <param name="foreignProperty">Propriedade de associação.</param>
		/// <param name="parentLoader">Instancia do loader associado.</param>
		/// <param name="conditional">Condicional para ser usado na carga.</param>
		/// <param name="isSingle">Identifica que é um filho unico.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority">Prioridade para salvar o filho.</param>
		protected EntityLoaderChildInfo(string name, string propertyName, Action<IEntity, IEntity> parentUidSetter, Func<Colosoft.Data.IModel, int> parentUidGetter, Func<IEntity, IEntity> parentValueGetter, Action<IEntity, IEntity> parentValueSetter, System.Reflection.PropertyInfo foreignProperty, ConditionalLoader conditional, IEntityLoader parentLoader, bool isSingle, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity)
		{
			propertyName.Require("propertyName").NotNull().NotEmpty();
			_name = name;
			_propertyName = propertyName;
			_parentLoader = parentLoader;
			_foreignPropertyName = foreignProperty.Name;
			_parentUidSetter = parentUidSetter;
			_parentUidGetter = parentUidGetter ?? _parentLoader.GetInstanceUid;
			_parentValueGetter = parentValueGetter;
			_parentValueSetter = parentValueSetter;
			_options = options;
			_parentUidSetter2 = (parent, child) =>  {
				if(_parentUidGetter == null)
					throw new InvalidOperationException("ParentUidGetter undefined");
				foreignProperty.SetValue(child, _parentUidGetter(parent), null);
			};
			_conditional = conditional;
			_isSingle = isSingle;
			_savePriority = savePriority;
		}

		/// <summary>
		/// Cria a consulta para recuperar os itens filhos.
		/// </summary>
		/// <param name="parentUid">Identificador da entidade pai.</param>
		/// <param name="parentDataModel">Instancia com os dados do pai.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <returns></returns>
		public EntityInfoQuery[] CreateQueries(int parentUid, Colosoft.Data.IModel parentDataModel, Colosoft.Query.ISourceContext sourceContext)
		{
			var query = sourceContext.CreateQuery().From(new Query.EntityInfo(DataModelType.FullName));
			string conditionalExpression = null;
			if(_parentLoader.HasUid)
			{
				query.Add("?parentUid", parentUid);
				conditionalExpression = string.Format("{0} == ?parentUid", _foreignPropertyName);
			}
			if(Conditional != null)
			{
				var conditional = this.Conditional();
				if(conditional != null)
				{
					if(!string.IsNullOrEmpty(conditionalExpression))
						conditionalExpression = string.Format("{0} && ({1})", conditionalExpression, conditional.Expression);
					else
						conditionalExpression = conditional.Expression;
					foreach (var i in conditional.Parameters)
						query.Add(i.Name, i.Value);
				}
			}
			if(string.IsNullOrEmpty(conditionalExpression))
				throw new InvalidOperationException(string.Format("Not support child '{0}', because not found conditional expression.", Name));
			return new EntityInfoQuery[] {
				new EntityInfoQuery(DataModelType, EntityType, query.Where(conditionalExpression))
			};
		}

		/// <summary>
		/// Cria as consulta para recupera os itens filhos.
		/// </summary>
		/// <param name="queryable">Consulta do item pai.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="exceptions">Fila do erros ocorridos.</param>
		/// <param name="entityTypeManager">Instancia do gereciador de tipos da entidade de negócio.</param>
		/// <param name="callBack"></param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void CreateQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack)
		{
			if(_entityLoader == null)
			{
				_entityLoader = entityTypeManager.GetLoader(EntityType);
			}
			var result = new List<EntityLoaderCreatorArgs>();
			var query = queryable.BeginSubQuery((sender, e) =>  {
				List<IEntity> entities = null;
				try
				{
					var bindStrategy = _entityLoader.GetBindStrategy();
					var objectCreator = _entityLoader.GetObjectCreator();
					var recordKeyFactory = _entityLoader.GetRecordKeyFactory();
					Colosoft.Reflection.TypeName dataModelTypeName = null;
					try
					{
						dataModelTypeName = Colosoft.Reflection.TypeName.Get(_entityLoader.DataModelType);
					}
					catch(Exception ex)
					{
						throw new Exception("An error ocurred when get DataModelType", ex);
					}
					result.Clear();
					foreach (var record in e.Result)
					{
						var data = objectCreator.Create();
						var bindResult = bindStrategy.Bind(record, Query.BindStrategyMode.All, ref data);
						if(!bindResult.Any())
							throw new Exception(string.Format("Not found scheme for bind record data to type '{0}'", data.GetType().FullName));
						Query.RecordKey recordKey = null;
						try
						{
							recordKey = recordKeyFactory.Create(dataModelTypeName, record);
						}
						catch(Exception ex)
						{
							throw new Exception("An error ocurred when create record key", ex);
						}
						result.Add(new EntityLoaderCreatorArgs((Data.IModel)data, recordKey, new EntityLoaderChildContainer(), new EntityLoaderLinksContainer(), new EntityLoaderReferenceContainer(), uiContext, entityTypeManager));
					}
					entities = new List<IEntity>();
					foreach (var i in result)
						try
						{
							IEntity entity = null;
							LazyDataState lazyDataState = new LazyDataState();
							_entityLoader.GetLazyData(i, lazyDataState, queryable.SourceContext, uiContext, entityTypeManager, exceptions);
							entity = _entityLoader.Create(uiContext, entityTypeManager, i, queryable.SourceContext);
							lazyDataState.Entity = entity;
							entities.Add(entity);
							i.Dispose();
						}
						catch(Exception ex)
						{
							throw new AggregateException(ex.Message, exceptions);
						}
				}
				catch(Exception ex)
				{
					failedCallBack(sender, new Query.SubQueryCallBackFailedArgs(e.Info, new Query.QueryFailedInfo(Query.QueryFailedReason.Error, ex.Message.GetFormatter(), ex)));
					return;
				}
				using (var entitiesEnumerable = new EntityChildEnumerable(entities, this, _entityLoader, entityTypeManager, queryable.SourceContext, uiContext))
				{
					callBack(this, new EntityLoaderExecuteQueryEventArgs(DataModelType, EntityType, e.ReferenceValues, entitiesEnumerable));
				}
				entities.Clear();
			}, failedCallBack).From(new Query.EntityInfo(DataModelType.FullName));
			string conditionalExpression = null;
			if(_parentLoader.HasUid)
			{
				query.Add("?parentUid", new Query.ReferenceParameter(_parentLoader.UidPropertyName));
				conditionalExpression = string.Format("{0} == ?parentUid", _foreignPropertyName);
			}
			if(Conditional != null)
			{
				var conditional = this.Conditional();
				if(conditional != null)
				{
					if(!string.IsNullOrEmpty(conditionalExpression))
						conditionalExpression = string.Format("{0} && ({1})", conditionalExpression, conditional.Expression);
					else
						conditionalExpression = conditional.Expression;
					foreach (var i in conditional.Parameters)
						query.Add(i.Name, i.Value);
				}
			}
			if(string.IsNullOrEmpty(conditionalExpression))
				throw new InvalidOperationException(string.Format("Not support child '{0}', because not found conditional expression.", Name));
			query.Where(conditionalExpression);
			if(_entityLoader == _parentLoader)
				throw new InvalidOperationException(string.Format("Not support child '{0}', because be can ocurred recursive function.", Name));
			_entityLoader.CreateNestedQueries(query, uiContext, entityTypeManager, result, exceptions);
			query.EndSubQuery();
		}

		/// <summary>
		/// Avalia se o registro contém dados associados
		/// com a lista dos filhos..
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parent">Instancia do pai.</param>
		/// <returns></returns>
		public abstract bool Evaluate(Query.IRecord record, IEntity parent);

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("ChildInfo -> {0}; EntityType: {1}", Name, EntityType.FullName);
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
	}
}
