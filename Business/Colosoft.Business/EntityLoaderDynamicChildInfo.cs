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
	/// Implementação base as informações dos filhos dinâmicos.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	abstract class EntityLoaderDynamicChildInfo<TEntity> : IEntityLoaderChildInfo where TEntity : IEntity
	{
		private string _name;

		private string _propertyName;

		private EntityDynamicChild _dynamicChild;

		private Action<IEntity, IEntity> _parentUidSetter;

		private Func<IEntity, IEntity> _parentValueGetter;

		private Action<IEntity, IEntity> _parentValueSetter;

		private bool _isSingle;

		private IEntityLoader _parentLoader;

		private LoadOptions _options;

		/// <summary>
		/// Nome da propriedade estrangueira da entidade na qual o filho está associado.
		/// </summary>
		public string ForeignPropertyName
		{
			get
			{
				return null;
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
		/// Nome da propriedade onde a instancia do filho
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
		public Type DataModelType
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Tipo da entidade.
		/// </summary>
		public Type EntityType
		{
			get
			{
				return typeof(TEntity);
			}
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
				return EntityChildSavePriority.AfterEntity;
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
				return null;
			}
		}

		/// <summary>
		/// Func usado para carregar a condicional de carga.
		/// </summary>
		public ConditionalLoader Conditional
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="propertyName">Nome da propriedade do filho na entidade.</param>
		/// <param name="dynamicChild">Dados do filho dinamico.</param>
		/// <param name="parentUidSetter"></param>
		/// <param name="parentValueGetter"></param>
		/// <param name="parentValueSetter"></param>
		/// <param name="isSingle"></param>
		/// <param name="parentLoader">Instancia do loader do pai.</param>
		/// <param name="options">Opções de carga.</param>
		public EntityLoaderDynamicChildInfo(string name, string propertyName, EntityDynamicChild dynamicChild, Func<IEntity, IEntity> parentValueGetter, Action<IEntity, IEntity> parentValueSetter, Action<IEntity, IEntity> parentUidSetter, bool isSingle, IEntityLoader parentLoader, LoadOptions options)
		{
			_name = name;
			_propertyName = propertyName;
			_dynamicChild = dynamicChild;
			_isSingle = isSingle;
			_parentUidSetter = parentUidSetter ?? SetParentUid;
			_parentValueGetter = parentValueGetter;
			_parentValueSetter = parentValueSetter;
			_parentLoader = parentLoader;
			_options = options;
		}

		/// <summary>
		/// Define o identificado pai para o filho.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		private void SetParentUid(IEntity parent, IEntity child)
		{
			var childType = child.GetType();
			foreach (var entry in _dynamicChild.Entries)
			{
				if(entry.EntityType.IsAssignableFrom(childType))
				{
					entry.ParentUidSetter(parent, child);
					break;
				}
			}
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
			var result = new EntityInfoQuery[_dynamicChild.Entries.Count];
			var startDate = ServerData.GetDateTimeOffSet();
			if(parentDataModel is Colosoft.Data.ITraceableModel)
				startDate = ((Colosoft.Data.ITraceableModel)parentDataModel).CreatedDate;
			for(var i = 0; i < result.Length; i++)
			{
				var entry = _dynamicChild.Entries[i];
				result[i] = entry.CreateQuery(parentUid, _parentLoader, parentDataModel, sourceContext);
			}
			return result;
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
		public void CreateQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack)
		{
			IEnumerable<IEntity> entities = null;
			var entriesCount = _dynamicChild.Entries.Count;
			var dynamicChildPosition = 0;
			for(var i = 0; i < _dynamicChild.Entries.Count; i++)
			{
				var entry = _dynamicChild.Entries[i];
				entry.CreateQueries(queryable, uiContext, _parentLoader, entityTypeManager, exceptions, (sender, e) =>  {
					if(dynamicChildPosition == 0)
						entities = e.Result;
					else
						entities = entities.Concat(e.Result);
					if(++dynamicChildPosition == entriesCount)
					{
						dynamicChildPosition = 0;
						IEnumerable<IEntity> entitiesEnumerable = entities ?? new IEntity[0];
						callBack(this, new EntityLoaderExecuteQueryEventArgs(DataModelType, EntityType, new Query.ReferenceParameterValueCollection(), entitiesEnumerable));
						entities = null;
					}
				}, (sender, e) =>  {
					failedCallBack(this, e);
				});
			}
			if(entriesCount == 0)
			{
				queryable.NestedQueriesProcessed += (sender, e) =>  {
					for(var i = 0; i < e.RecordsCount; i++)
					{
						IEnumerable<IEntity> entitiesEnumerable = new IEntity[0];
						callBack(this, new EntityLoaderExecuteQueryEventArgs(DataModelType, EntityType, new Query.ReferenceParameterValueCollection(), entitiesEnumerable));
					}
				};
			}
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
