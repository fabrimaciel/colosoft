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
	/// Armazena as informações da referencia da entidade.
	/// </summary>
	public abstract class EntityLoaderReference
	{
		private string _name;

		private string _parentPropertyName;

		private string[] _watchedPropertyNames;

		private ConditionalLoader _conditional;

		private LoadOptions _options;

		private bool _isLoadLazy;

		private EntityChildSavePriority _savePriority;

		/// <summary>
		/// Instancia do loader da entidade.
		/// </summary>
		private IEntityLoader _entityLoader;

		/// <summary>
		/// Nome da referencia.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Nome da propriedade do pai na qual a referencia está vinculada.
		/// </summary>
		public string ParentPropertyName
		{
			get
			{
				return _parentPropertyName;
			}
		}

		/// <summary>
		/// Propriedades que serão monitoradas.
		/// </summary>
		public string[] WatchedPropertyNames
		{
			get
			{
				return _watchedPropertyNames;
			}
		}

		/// <summary>
		/// Condicional usada para a carga da referencia.
		/// </summary>
		public ConditionalLoader Conditional
		{
			get
			{
				return _conditional;
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
		/// Opções de carga da referência.
		/// </summary>
		public LoadOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Identifica se é para carregar a referncia de forma tardia.
		/// </summary>
		public bool IsLoadLazy
		{
			get
			{
				return _isLoadLazy;
			}
		}

		/// <summary>
		/// Instancia responsável por recupera o valor pai.
		/// </summary>
		public abstract Func<IEntity, IEntity> ParentValueGetter
		{
			get;
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
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parentPropertyName"></param>
		/// <param name="conditional"></param>
		/// <param name="isLoadLazy">Identifica se a referência terá carga tardia.</param>
		/// <param name="options">Opções de carga da referência.</param>
		/// <param name="savePriority">Prioridade para salvar a referência.</param>
		/// <param name="watchedProperties"></param>
		public EntityLoaderReference(string name, string parentPropertyName, ConditionalLoader conditional, bool isLoadLazy, LoadOptions options, EntityChildSavePriority savePriority, string[] watchedProperties)
		{
			_name = name;
			_parentPropertyName = parentPropertyName;
			_watchedPropertyNames = watchedProperties;
			_conditional = conditional;
			_isLoadLazy = isLoadLazy;
			_options = options;
			_savePriority = savePriority;
		}

		/// <summary>
		/// Recupera a condicional para a consulta.
		/// </summary>
		/// <returns></returns>
		protected EntityLoaderConditional GetConditional()
		{
			return this.Conditional();
		}

		/// <summary>
		/// Cria a consulta para recuperar os itens filhos.
		/// </summary>
		/// <param name="parentDataModel">Instancia com os dados do pai.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <returns></returns>
		internal EntityInfoQuery[] CreateQueries(Colosoft.Data.IModel parentDataModel, Colosoft.Query.ISourceContext sourceContext)
		{
			var query = sourceContext.CreateQuery().From(new Query.EntityInfo(DataModelType.FullName));
			string conditionalExpression = null;
			var conditional = GetConditional();
			if(conditional != null)
			{
				conditionalExpression = conditional.Expression;
				foreach (var i in conditional.Parameters)
				{
					object value = i.Value;
					if(value is Colosoft.Query.ReferenceParameter)
					{
						var referenceParameter = (Colosoft.Query.ReferenceParameter)i.Value;
						var prop = parentDataModel.GetType().GetProperty(referenceParameter.ColumnName);
						if(prop != null)
						{
							try
							{
								value = prop.GetValue(parentDataModel, null);
							}
							catch(System.Reflection.TargetInvocationException ex)
							{
								throw ex.InnerException;
							}
						}
						else
						{
							throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderReference_ParentPropertyNotFound, referenceParameter.ColumnName, parentDataModel.GetType().Name).Format());
						}
					}
					query.Add(i.Name, value);
				}
			}
			return new EntityInfoQuery[] {
				new EntityInfoQuery(DataModelType, EntityType, query.Where(conditionalExpression))
			};
		}

		/// <summary>
		/// Cria as consulta para recupera a referência.
		/// </summary>
		/// <param name="queryable">Consulta do item pai.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="exceptions">Fila do erros ocorridos.</param>
		/// <param name="entityTypeManager">Instancia do gereciador de tipos da entidade de negócio.</param>
		/// <param name="callBack"></param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void CreateQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack)
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
				callBack(this, new EntityLoaderExecuteQueryEventArgs(DataModelType, EntityType, e.ReferenceValues, entities));
				entities.Clear();
			}, failedCallBack).From(new Query.EntityInfo(DataModelType.FullName));
			var conditional = this.GetConditional();
			if(conditional == null)
				throw new InvalidOperationException("Conditional not found");
			query.Where(conditional.Expression);
			foreach (var i in conditional.Parameters)
				query.Add(i.Name, i.Value);
			_entityLoader.CreateNestedQueries(query, uiContext, entityTypeManager, result, exceptions);
			query.EndSubQuery();
		}
	}
}
