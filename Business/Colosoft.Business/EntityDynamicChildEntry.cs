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
	/// Armazena os dados da entrada de um filho dinamico.
	/// </summary>
	public abstract class EntityDynamicChildEntry
	{
		private string _foreignPropertyName;

		private Action<IEntity, IEntity> _parentUidSetter;

		private Func<Colosoft.Data.IModel, int> _parentUidGetter;

		private EntityLoaderConditional _conditional;

		private Colosoft.Query.QueryExecutePredicate _executePredicate;

		private IEntityLoader _entityLoader;

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
		public EntityLoaderConditional Conditional
		{
			get
			{
				return _conditional;
			}
		}

		/// <summary>
		/// Predicado que será utilizado para filtrar a execução da consulta do filho.
		/// </summary>
		public Colosoft.Query.QueryExecutePredicate ExecutePredicate
		{
			get
			{
				return _executePredicate;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parentUidSetter">Delegate usado para define o identificador unico do pai.</param>
		/// <param name="parentUidGetter">Delegate usado para recupera o identificador unico do pai.</param>
		/// <param name="foreignProperty">Propriedade de associação.</param>
		/// <param name="conditional">Condicional para ser usado na carga.</param>
		/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
		protected EntityDynamicChildEntry(Action<IEntity, IEntity> parentUidSetter, Func<Colosoft.Data.IModel, int> parentUidGetter, System.Reflection.PropertyInfo foreignProperty, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate)
		{
			_foreignPropertyName = foreignProperty.Name;
			_parentUidSetter = parentUidSetter;
			_parentUidGetter = parentUidGetter;
			_conditional = conditional;
			_executePredicate = executePredicate;
		}

		/// <summary>
		/// Cria a consulta para recuperar os itens filhos.
		/// </summary>
		/// <param name="parentUid">Identificador da entidade pai.</param>
		/// <param name="parentLoader">Loader do pai.</param>
		/// <param name="parentDataModel">Instancia com os dados do pai.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <returns></returns>
		internal EntityInfoQuery CreateQuery(int parentUid, IEntityLoader parentLoader, Colosoft.Data.IModel parentDataModel, Colosoft.Query.ISourceContext sourceContext)
		{
			var query = sourceContext.CreateQuery().From(new Query.EntityInfo(DataModelType.FullName));
			if(ExecutePredicate != null)
				query.ExecutePredicate = (Colosoft.Query.QueryExecutePredicate)ExecutePredicate.Clone();
			string conditionalExpression = null;
			if(parentLoader.HasUid)
			{
				query.Add("?parentUid", parentUid);
				conditionalExpression = string.Format("{0} == ?parentUid", _foreignPropertyName);
			}
			if(Conditional != null)
			{
				if(parentLoader.HasUid)
					conditionalExpression = string.Format("{0} && ({1})", conditionalExpression, this.Conditional.Expression);
				else
					conditionalExpression = this.Conditional.Expression;
				foreach (var i in this.Conditional.Parameters)
				{
					if(i.Value is Query.ReferenceParameter)
					{
						var reference = (Query.ReferenceParameter)i.Value;
						var property = parentDataModel.GetType().GetProperty(reference.ColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
						if(property == null)
							throw new Exception(string.Format("Property {0} not found in type {1}", reference.ColumnName, parentDataModel.GetType().FullName));
						try
						{
							var value = property.GetValue(parentDataModel, null);
							query.Add(i.Name, value);
						}
						catch(System.Reflection.TargetInvocationException ex)
						{
							throw ex.InnerException;
						}
					}
					else
						query.Add(i.Name, i.Value);
				}
			}
			query.Where(conditionalExpression);
			return new EntityInfoQuery(DataModelType, EntityType, query);
		}

		/// <summary>
		/// Cria as consulta para recupera os itens filhos.
		/// </summary>
		/// <param name="queryable">Consulta do item pai.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="parentLoader">Instancia do loader do pai.</param>
		/// <param name="entityTypeManager">Instancia do gereciador de tipos da entidade de negócio.</param>
		/// <param name="exceptions">Fila do erros ocorridos.</param>
		/// <param name="callBack"></param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void CreateQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityLoader parentLoader, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack)
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
					var dataModelTypeName = Colosoft.Reflection.TypeName.Get(_entityLoader.DataModelType);
					result.Clear();
					foreach (var record in e.Result)
					{
						var data = objectCreator.Create();
						if(!bindStrategy.Bind(record, Query.BindStrategyMode.All, ref data).Any())
							throw new Exception(string.Format("Not found scheme for bind record data to type '{0}'", data.GetType().FullName));
						var recordKey = recordKeyFactory.Create(dataModelTypeName, record);
						result.Add(new EntityLoaderCreatorArgs((Data.IModel)data, recordKey, new EntityLoaderChildContainer(), new EntityLoaderLinksContainer(), new EntityLoaderReferenceContainer(), uiContext, entityTypeManager));
					}
					entities = new List<IEntity>();
					foreach (var i in result)
					{
						IEntity entity = null;
						LazyDataState lazyDataState = new LazyDataState();
						_entityLoader.GetLazyData(i, lazyDataState, queryable.SourceContext, uiContext, entityTypeManager, exceptions);
						entity = _entityLoader.Create(uiContext, entityTypeManager, i, queryable.SourceContext);
						lazyDataState.Entity = entity;
						entities.Add(entity);
						i.Dispose();
					}
				}
				catch(Exception ex)
				{
					failedCallBack(sender, new Query.SubQueryCallBackFailedArgs(e.Info, new Query.QueryFailedInfo(Query.QueryFailedReason.Error, ex.Message.GetFormatter(), ex)));
					return;
				}
				callBack(this, new EntityLoaderExecuteQueryEventArgs(DataModelType, EntityType, e.ReferenceValues, entities));
			}, failedCallBack).From(new Query.EntityInfo(DataModelType.FullName));
			if(ExecutePredicate != null)
				query.ExecutePredicate = (Colosoft.Query.QueryExecutePredicate)ExecutePredicate.Clone();
			string conditionalExpression = null;
			if(parentLoader.HasUid)
			{
				query.Add("?parentUid", new Query.ReferenceParameter(parentLoader.UidPropertyName));
				conditionalExpression = string.Format("{0} == ?parentUid", _foreignPropertyName);
			}
			if(Conditional != null)
			{
				if(!string.IsNullOrEmpty(conditionalExpression))
					conditionalExpression = string.Format("{0} && ({1})", conditionalExpression, this.Conditional.Expression);
				else
					conditionalExpression = this.Conditional.Expression;
				foreach (var i in this.Conditional.Parameters)
					query.Add(i.Name, i.Value);
			}
			if(string.IsNullOrEmpty(conditionalExpression))
				throw new InvalidOperationException("Not support child, because not found conditional expression.");
			query.Where(conditionalExpression);
			if(_entityLoader != parentLoader)
				_entityLoader.CreateNestedQueries(query, uiContext, entityTypeManager, result, exceptions);
			query.EndSubQuery();
		}
	}
}
