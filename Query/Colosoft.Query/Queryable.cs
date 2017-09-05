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
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa um estrutura de consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class Queryable : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, IQueryExecuteObserver, Collections.ISearchParameterDescriptionContainer
	{
		private StoredProcedureName _storedProcedureName;

		private string _storedProcedureProvider;

		private bool _isSelectDistinct = false;

		private QueryParameterCollection _parameters = new QueryParameterCollection();

		private Projection _projection;

		private QueryExecutePredicate _executePredicate;

		private ConditionalContainer _whereClause = new ConditionalContainer();

		private EntityInfo _entity;

		private List<JoinInfo> _joins = new List<JoinInfo>();

		private Queue<EntityInfo> _joinEntities = new Queue<EntityInfo>();

		private IQueryDataSource _dataSource;

		private ISourceContext _sourceContext;

		private Sort _sort;

		private GroupBy _groupByClause;

		private ConditionalContainer _havingClause;

		private TakeParameters _takeParameters;

		private List<Queryable> _nestedQueries = new List<Queryable>();

		private UnionCollection _unions = new UnionCollection();

		private System.Transactions.IsolationLevel _isolationLevel = System.Transactions.IsolationLevel.Unspecified;

		private int _commandTimeout = 30;

		private string _providerName;

		private bool _canUseCache = true;

		private bool _ignoreTypeSchema;

		private bool _hasRowVersion = false;

		/// <summary>
		/// Armazena a relação da consulta pai no caso de subconsulta.
		/// </summary>
		private Queryable _parent;

		private SubQueryCallBack _subQueryCallBack;

		private SubQueryFailedCallBack _subQueryFailedCallBack;

		private Collections.SearchParameterDescriptionCollection _searchParameterDescriptions;

		/// <summary>
		/// Evento acionado quando as consulta aninhadas forem processadas.
		/// </summary>
		public event NestedQueriesProcessedHandler NestedQueriesProcessed;

		/// <summary>
		/// Evento acioando quando uma consulta aninhada for processada.
		/// </summary>
		public event NestedQueryProcessedHandler NestedQueryProcessed;

		/// <summary>
		/// Evento acionado quando um registro das consultas aninhadas for processado.
		/// </summary>
		public event NestedQueriesRecordProcessedHandler NestedQueriesRecordProcessed;

		/// <summary>
		/// Identifica se para ignorar o esquema de tipo.
		/// </summary>
		public bool IgnoreTypeSchemaInternal
		{
			get
			{
				return _ignoreTypeSchema;
			}
			set
			{
				_ignoreTypeSchema = value;
			}
		}

		/// <summary>
		/// Nível de isolamento da consulta.
		/// </summary>
		public System.Transactions.IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
			set
			{
				_isolationLevel = value;
			}
		}

		/// <summary>
		/// Tempo de espera antes de terminar a tentativa de executar um comando e de gerar um erro.
		/// </summary>
		/// <value>O tempo em segundos de esperar o comando executar. O padrão é 30 segundos.</value>
		public int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				_commandTimeout = value;
			}
		}

		/// <summary>
		/// Nome do provedor que será usado na consulta.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		/// <summary>
		/// Identifica se pode usar o cache para obter o resultado da consulta.
		/// </summary>
		public bool CanUseCache
		{
			get
			{
				return _canUseCache;
			}
			set
			{
				_canUseCache = value;
			}
		}

		/// <summary>
		/// Predicado que define a execução da consulta.
		/// </summary>
		public QueryExecutePredicate ExecutePredicate
		{
			get
			{
				return _executePredicate;
			}
			set
			{
				_executePredicate = value;
			}
		}

		/// <summary>
		/// Callback da subquery representada.
		/// </summary>
		public SubQueryCallBack SubQueryCallBack
		{
			get
			{
				return _subQueryCallBack;
			}
			set
			{
				_subQueryCallBack = value;
			}
		}

		/// <summary>
		/// Callback da falha da subquery representada.
		/// </summary>
		public SubQueryFailedCallBack SubQueryFailedCallBack
		{
			get
			{
				return _subQueryFailedCallBack;
			}
			set
			{
				_subQueryFailedCallBack = value;
			}
		}

		/// <summary>
		/// Informação dos joins da query.
		/// </summary>
		internal List<JoinInfo> Joins
		{
			get
			{
				return _joins;
			}
		}

		/// <summary>
		/// Relação das entidades do join.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IEnumerable<EntityInfo> JoinEntities
		{
			get
			{
				return _joinEntities;
			}
		}

		/// <summary>
		/// Clausula WHERE.
		/// </summary>
		public ConditionalContainer WhereClause
		{
			get
			{
				return _whereClause;
			}
			set
			{
				value.Require("value").NotNull();
				_whereClause = value;
			}
		}

		/// <summary>
		/// Informações da entidade que será processada pela pesquisa.
		/// </summary>
		public EntityInfo Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}

		/// <summary>
		/// Instancia da fonte de dados associada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IQueryDataSource DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}

		/// <summary>
		/// Instancia do contexto de origem associado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public ISourceContext SourceContext
		{
			get
			{
				return _sourceContext;
			}
			set
			{
				_sourceContext = value;
			}
		}

		/// <summary>
		/// Dados da ordenação da consulta.
		/// </summary>
		public Sort Sort
		{
			get
			{
				return _sort;
			}
			set
			{
				_sort = value;
			}
		}

		/// <summary>
		/// Projeção de seleção.
		/// </summary>
		public Projection Projection
		{
			get
			{
				return _projection;
			}
			set
			{
				_projection = value;
			}
		}

		/// <summary>
		/// Clausula da agrupamento da consulta.
		/// </summary>
		public GroupBy GroupByClause
		{
			get
			{
				return _groupByClause;
			}
			set
			{
				_groupByClause = value;
			}
		}

		/// <summary>
		/// Clausula having.
		/// </summary>
		public ConditionalContainer HavingClause
		{
			get
			{
				return _havingClause;
			}
			set
			{
				_havingClause = value;
			}
		}

		/// <summary>
		/// Parameter de recuperação.
		/// </summary>
		public TakeParameters TakeParameters
		{
			get
			{
				return _takeParameters;
			}
		}

		/// <summary>
		/// Parametros da consulta.
		/// </summary>
		public QueryParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
			internal set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Nome da StoredProcedure a ser executada.
		/// </summary>
		public StoredProcedureName StoredProcedureName
		{
			get
			{
				return _storedProcedureName;
			}
		}

		/// <summary>
		/// Retorna se a query representa uma StoredProcedure.
		/// </summary>
		public bool IsStoredProcedure
		{
			get
			{
				return _storedProcedureName != null;
			}
		}

		/// <summary>
		/// Consultas aninhadas.
		/// </summary>
		public List<Queryable> NestedQueries
		{
			get
			{
				return _nestedQueries;
			}
		}

		/// <summary>
		/// Uniões da consulta.
		/// </summary>
		public UnionCollection Unions
		{
			get
			{
				return _unions;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Queryable()
		{
			_takeParameters = new TakeParameters();
			_whereClause = new ConditionalContainer(this);
		}

		/// <summary>
		/// Cria uma consulta iniciando com a expressão condicional.
		/// </summary>
		/// <param name="whereExpression"></param>
		public Queryable(string whereExpression)
		{
			_takeParameters = new TakeParameters();
			_whereClause = ConditionalContainer.Parse(whereExpression);
			_whereClause.ConfigureSearchParameterDescriptionContainer(this);
		}

		/// <summary>
		/// Cria uma consulta já com os parametros condicionais.
		/// </summary>
		/// <param name="conditional"></param>
		public Queryable(ConditionalContainer conditional)
		{
			_takeParameters = new TakeParameters();
			conditional.Require("conditional").NotNull();
			_whereClause = conditional;
			_whereClause.ConfigureSearchParameterDescriptionContainer(this);
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private Queryable(SerializationInfo info, StreamingContext context)
		{
			_isolationLevel = (System.Transactions.IsolationLevel)info.GetInt16("IsolationLevel");
			_commandTimeout = info.GetInt32("CommandTimeout");
			_providerName = info.GetString("ProviderName");
			_ignoreTypeSchema = info.GetBoolean("IgnoreTypeSchema");
			_takeParameters = new TakeParameters();
			_entity = (EntityInfo)info.GetValue("Entity", typeof(EntityInfo));
			_groupByClause = (GroupBy)info.GetValue("GroupByClause", typeof(GroupBy));
			_havingClause = (ConditionalContainer)info.GetValue("HavingClause", typeof(ConditionalContainer));
			_parameters = (QueryParameterCollection)info.GetValue("Parameters", typeof(QueryParameterCollection));
			_projection = (Projection)info.GetValue("Projection", typeof(Projection));
			_executePredicate = (QueryExecutePredicate)info.GetValue("ExecutePredicate", typeof(QueryExecutePredicate));
			_sort = (Sort)info.GetValue("Sort", typeof(Sort));
			_whereClause = (ConditionalContainer)info.GetValue("WhereClause", typeof(ConditionalContainer));
			var joinsCount = info.GetInt32("JC");
			var _joins = new List<JoinInfo>();
			for(var i = 0; i < joinsCount; i++)
				_joins.Add((JoinInfo)info.GetValue("j" + i, typeof(JoinInfo)));
			var joinsEntitiesCount = info.GetInt32("JEC");
			var joinsEntities = new List<EntityInfo>();
			for(var i = 0; i < joinsEntitiesCount; i++)
				joinsEntities.Add((EntityInfo)info.GetValue("je" + i, typeof(EntityInfo)));
			_joinEntities = new Queue<EntityInfo>(joinsEntities);
			var nestedQueriesCount = info.GetInt32("NQ");
			var nestedQueries = new List<Queryable>();
			for(var i = 0; i < nestedQueriesCount; i++)
				nestedQueries.Add((Queryable)info.GetValue("nq" + i, typeof(Queryable)));
			_nestedQueries = nestedQueries;
			_unions = (UnionCollection)info.GetValue("Unions", typeof(UnionCollection));
		}

		/// <summary>
		/// Identifica que é para ignora o esquema de tipo.
		/// </summary>
		/// <returns></returns>
		public Queryable IgnoreTypeSchema()
		{
			_ignoreTypeSchema = true;
			return this;
		}

		/// <summary>
		/// Converte para um queryble do linq.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public System.Linq.IQueryable<T> AsQueryable<T>() where T : new()
		{
			return new Linq.Queryable<T>(this);
		}

		/// <summary>
		/// Converte para um queryble do linq.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator">Criador dos objetos.</param>
		/// <returns></returns>
		public System.Linq.IQueryable<T> AsQueryable<T>(IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator = null) where T : new()
		{
			return new Linq.Queryable<T>(this, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Converte para um queryble do linq.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="processor">Método usado para processar os itens do resultado.</param>
		/// <returns></returns>
		public System.Linq.IQueryable<T> AsQueryable<T>(Func<IEnumerable<IRecord>, IEnumerable<T>> processor) where T : new()
		{
			return new Linq.Queryable<T>(this, new DynamicBindStrategy<T>((records, mode) => processor(records)), null);
		}

		/// <summary>
		/// Adiciona a descrição da condicinal aplicada a consulta.
		/// </summary>
		/// <param name="description">Descrição que será adicionada.</param>
		/// <param name="parameterName">Nome do parametro associado a condicionais.</param>
		/// <returns></returns>
		public Queryable AddConditionalDescription(IMessageFormattable description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description);
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicinal aplicada a consulta.
		/// </summary>
		/// <param name="description">Descrição que será adicionada.</param>
		/// <param name="parameterName">Nome do parametro associado a condicionais.</param>
		/// <returns></returns>
		public Queryable AddConditionalDescription(string description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description.GetFormatter());
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public Queryable AddDescription(Lazy<IMessageFormattable> description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description);
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public Queryable AddDescription(Lazy<string> description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description.Value != null ? description.Value.GetFormatter() : null));
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public Queryable AddDescription(Func<string> description, string parameterName = null)
		{
			description.Require("description").NotNull();
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description().GetFormatter()));
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public Queryable AddDescription(Func<IMessageFormattable> description, string parameterName = null)
		{
			description.Require("description").NotNull();
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description()));
			return this;
		}

		/// <summary>
		/// Configura o provider da consulta.
		/// </summary>
		/// <param name="providerName"></param>
		/// <returns></returns>
		public Queryable Provider(string providerName)
		{
			ProviderName = providerName;
			return this;
		}

		/// <summary>
		/// Where da consulta.
		/// </summary>
		public IWhereClause CreateWhere()
		{
			if(_whereClause == null)
			{
				_whereClause = new ConditionalContainer();
				_whereClause.ConfigureSearchParameterDescriptionContainer(this);
			}
			return new QueryableWhereClause(this);
		}

		/// <summary>
		/// Where de um inner join.
		/// </summary>
		/// <param name="alias">Alias do Join.</param>
		/// <returns></returns>
		public IWhereClause CreateWhereInnerJoin<T>(string alias)
		{
			return CreateWhereJoin(typeof(T).FullName, JoinType.Inner, alias);
		}

		/// <summary>
		/// Where de um left join.
		/// </summary>
		/// <param name="alias">Alias do Join.</param>
		/// <returns></returns>
		public IWhereClause CreateWhereLeftJoin<T>(string alias)
		{
			return CreateWhereJoin(typeof(T).FullName, JoinType.Left, alias);
		}

		/// <summary>
		/// Where de um right join.
		/// </summary>
		/// <param name="alias">Alias do Join.</param>
		/// <returns></returns>
		public IWhereClause CreateWhereRightJoin<T>(string alias)
		{
			return CreateWhereJoin(typeof(T).FullName, JoinType.Right, alias);
		}

		/// <summary>
		/// Where de um join.
		/// </summary>
		/// <param name="entityFullName">Nome da entidade de join.</param>
		/// <param name="type">Tipo do join.</param>
		/// <param name="alias">Alias do Join.</param>
		/// <returns></returns>
		public IWhereClause CreateWhereJoin(string entityFullName, JoinType type, string alias)
		{
			var conditional = new ConditionalContainer();
			int index = _joins.Count;
			Join(entityFullName, type, conditional, alias);
			return new JoinWhereClause(this, index);
		}

		/// <summary>
		///Recupera uma whereClause de join.
		/// </summary>
		/// <returns></returns>
		public IWhereClause GetWhereJoin<T>()
		{
			return GetWhereJoin(typeof(T).FullName);
		}

		/// <summary>
		/// Recupera uma whereClause de join.
		/// </summary>
		/// <param name="entityFullName">Nome da entidade.</param>
		/// <returns></returns>
		public IWhereClause GetWhereJoin(string entityFullName)
		{
			int index = 0;
			foreach (var joinEntity in _joinEntities)
			{
				index++;
				if(joinEntity.FullName == entityFullName)
					break;
			}
			if(index < _joinEntities.Count)
				return new JoinWhereClause(this, index);
			else
				return null;
		}

		/// <summary>
		/// Faz a query representar uma StoredProcedure.
		/// </summary>
		/// <param name="name">Nome da StoredProcedure a ser representada.</param>
		/// <param name="providerName">Nome do provedor de conexão de banco de dados da procedure.</param>
		/// <returns>Referência a própria instância.</returns>
		public Queryable SetStoredProcedure(string name, string providerName = null)
		{
			_storedProcedureName = StoredProcedureName.Parse(name);
			_storedProcedureProvider = providerName;
			return this;
		}

		/// <summary>
		/// Remove da query a StoredProcedure fazendo com que ela volte a ser uma query simples.
		/// </summary>
		/// <returns></returns>
		public Queryable RemoveStoredProcedure()
		{
			_storedProcedureName = null;
			return this;
		}

		/// <summary>
		/// Define o container condicional do having.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public Queryable Having(ConditionalContainer conditional)
		{
			conditional.Require("conditional").NotNull();
			_havingClause = conditional;
			_havingClause.ConfigureSearchParameterDescriptionContainer(this);
			return this;
		}

		/// <summary>
		/// Define as condicionais do having.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Queryable Having(string expression)
		{
			expression.Require("expression").NotNull().NotEmpty();
			_havingClause = ConditionalContainer.Parse(expression);
			_havingClause.ConfigureSearchParameterDescriptionContainer(this);
			return this;
		}

		/// <summary>
		/// Define o container condicional da consulta.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public Queryable Where(ConditionalContainer conditional)
		{
			conditional.Require("conditional").NotNull();
			_whereClause = conditional;
			_whereClause.ConfigureSearchParameterDescriptionContainer(this);
			return this;
		}

		/// <summary>
		/// Define as condicionais da consulta.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Queryable Where(string expression)
		{
			expression.Require("expression").NotNull().NotEmpty();
			_whereClause = ConditionalContainer.Parse(expression);
			_whereClause.ConfigureSearchParameterDescriptionContainer(this);
			return this;
		}

		/// <summary>
		/// Adiciona um novo parametro na consulta.
		/// </summary>
		/// <param name="parameter">Paramentro a ser adicionado.</param>
		/// <returns>Retorna a referencia da consulta aonde o parametro foi adicionado.</returns>
		public Queryable Add(QueryParameter parameter)
		{
			parameter.Require("parameter").NotNull();
			var index = _parameters.FindIndex(f => f.Name == parameter.Name);
			if(index >= 0)
				_parameters.RemoveAt(index);
			_parameters.Add(parameter);
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public Queryable Add(params QueryParameter[] parameters)
		{
			parameters.Require("parameters").NotNull();
			foreach (var i in parameters)
			{
				var index = _parameters.FindIndex(f => f.Name == i.Name);
				if(index >= 0)
					_parameters.RemoveAt(index);
				_parameters.Add(i);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public Queryable Add(IEnumerable<QueryParameter> parameters)
		{
			parameters.Require("parameters").NotNull();
			foreach (var i in parameters)
			{
				var index = _parameters.FindIndex(f => f.Name == i.Name);
				if(index >= 0)
					_parameters.RemoveAt(index);
				_parameters.Add(i);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um parametro para a consulta.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Queryable Add(string name, object value)
		{
			return Add(new QueryParameter(name, value));
		}

		/// <summary>
		/// Adiciona um parametro para consulta.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public Queryable Add(string name, object value, ParameterDirection direction)
		{
			return Add(new QueryParameter(name, value, direction));
		}

		/// <summary>
		/// Adiciona uma consulta
		/// </summary>
		/// <param name="entityFullName"></param>
		/// <param name="type"></param>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable Join(string entityFullName, JoinType type, ConditionalContainer conditional, string alias)
		{
			var entity = new EntityInfo(entityFullName, alias ?? entityFullName);
			if(_joinEntities.Any(f => f.Alias == entity.Alias))
				throw new InvalidOperationException(string.Format("Join '{0}' exist.", entity.Alias));
			var join = new JoinInfo {
				Right = entity.Alias ?? entity.FullName,
				Type = type,
				Conditional = conditional
			};
			_joinEntities.Enqueue(entity);
			_joins.Add(join);
			return this;
		}

		/// <summary>
		/// Adiciona uma consulta
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="type"></param>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable Join(Queryable queryable, JoinType type, ConditionalContainer conditional, string alias)
		{
			queryable.Require("query").NotNull();
			var entity = new EntityInfo(queryable.CreateQueryInfo(), alias);
			if(_joinEntities.Any(f => f.Alias == entity.Alias))
				throw new InvalidOperationException(string.Format("Join '{0}' exist.", entity.Alias));
			var join = new JoinInfo {
				Right = entity.Alias,
				Type = type,
				Conditional = conditional
			};
			_joinEntities.Enqueue(entity);
			_joins.Add(join);
			return this;
		}

		/// <summary>
		/// Adiciona uma junção para a consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public Queryable Join<T>(JoinType type, ConditionalContainer conditional, string alias)
		{
			return Join(typeof(T).FullName, type, conditional, alias);
		}

		/// <summary>
		/// Adiciona uma junção cruzada para a consulta.
		/// </summary>
		/// <typeparam name="T">Tipo da classe que será feita a junção.</typeparam>
		/// <param name="alias">Apelido da junção.</param>
		/// <returns></returns>
		public Queryable CrossJoin<T>(string alias)
		{
			return Join<T>(JoinType.Cross, null, alias);
		}

		/// <summary>
		/// Adiciona uma junção cruzada com a consulta informada.
		/// </summary>
		/// <param name="query">Consulta que irá representar a junção.</param>
		/// <param name="alias">Apelido da junçã.</param>
		/// <returns></returns>
		public Queryable CrossJoin(Queryable query, string alias)
		{
			return Join(query, JoinType.Cross, null, alias);
		}

		/// <summary>
		/// Adiciona uma junção para a consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de classe que será feita a junção</typeparam>
		/// <param name="alias">Apelido da junção.</param>
		/// <param name="conditional">Condicional da junção.</param>
		/// <returns></returns>
		public Queryable InnerJoin<T>(ConditionalContainer conditional, string alias)
		{
			return Join<T>(JoinType.Inner, conditional, alias);
		}

		/// <summary>
		/// Adiciona uma junção para a consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de classe que será feita a junção</typeparam>
		/// <param name="expression">Expressão condicional da junção.</param>
		/// <param name="alias">Apelido da junção.</param>
		/// <returns></returns>
		public Queryable InnerJoin<T>(string expression, string alias)
		{
			return Join<T>(JoinType.Inner, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona uma junção para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="alias">Apelido da junção.</param>
		/// <param name="conditional">Condicional da junção.</param>
		/// <returns></returns>
		public Queryable InnerJoin(Queryable query, ConditionalContainer conditional, string alias)
		{
			return Join(query, JoinType.Inner, conditional, alias);
		}

		/// <summary>
		/// Adiciona uma junção para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="expression">Expressão condicional da junção.</param>
		/// <param name="alias">Apelido da junção.</param>
		/// <returns></returns>
		public Queryable InnerJoin(Queryable query, string expression, string alias)
		{
			return Join(query, JoinType.Inner, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona um LEFT JOIN para a consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable LeftJoin<T>(ConditionalContainer conditional, string alias)
		{
			return Join<T>(JoinType.Left, conditional, alias);
		}

		/// <summary>
		/// Adiciona um LEFT JOIN para a consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable LeftJoin<T>(string expression, string alias)
		{
			return Join<T>(JoinType.Left, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona um LEFT JOIN para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable LeftJoin(Queryable query, ConditionalContainer conditional, string alias)
		{
			return Join(query, JoinType.Left, conditional, alias);
		}

		/// <summary>
		/// Adiciona um LEFT JOIN para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable LeftJoin(Queryable query, string expression, string alias)
		{
			return Join(query, JoinType.Left, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona um RIGHT JOIN para a consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable RightJoin<T>(ConditionalContainer conditional, string alias)
		{
			return Join<T>(JoinType.Right, conditional, alias);
		}

		/// <summary>
		/// Adiciona um RIGHT JOIN para a consulta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable RightJoin<T>(string expression, string alias)
		{
			return Join<T>(JoinType.Right, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona um RIGHT JOIN para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="expression"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable RightJoin(Queryable query, string expression, string alias)
		{
			return Join(query, JoinType.Right, ConditionalContainer.Parse(expression), alias);
		}

		/// <summary>
		/// Adiciona um RIGHT JOIN para a consulta.
		/// </summary>
		/// <param name="query">Query aninhada no join.</param>
		/// <param name="conditional"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable RightJoin(Queryable query, ConditionalContainer conditional, string alias)
		{
			return Join(query, JoinType.Right, conditional, alias);
		}

		/// <summary>
		/// Define a projeção de seleção.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public Queryable Select(Projection projection)
		{
			_projection = projection;
			return this;
		}

		/// <summary>
		/// Define a projeção da seleção.
		/// </summary>
		/// <param name="expression">Expressão a projeção.</param>
		/// <returns></returns>
		public Queryable Select(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				_projection = null;
			_projection = Query.Projection.Parse(expression);
			return this;
		}

		/// <summary>
		/// Define a projeção da seleção.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertiesSelector">Propriedades.</param>
		/// <returns></returns>
		public Queryable Select<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
		{
			return this.Select(string.Join(",", PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray()));
		}

		/// <summary>
		/// Faz seleção distinta.
		/// </summary>
		/// <returns></returns>
		public Queryable SelectDistinct()
		{
			_isSelectDistinct = true;
			return this;
		}

		/// <summary>
		/// Define a projeção de seleção distinta.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public Queryable SelectDistinct(Projection projection)
		{
			_isSelectDistinct = true;
			return Select(projection);
		}

		/// <summary>
		/// Define a projeção da seleção.
		/// </summary>
		/// <param name="expression">Expressão a projeção.</param>
		/// <returns></returns>
		public Queryable SelectDistinct(string expression)
		{
			_isSelectDistinct = true;
			return Select(expression);
		}

		/// <summary>
		/// Define a projeção da seleção.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertiesSelector">Propriedades.</param>
		/// <returns></returns>
		public Queryable SelectDistinct<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
		{
			_isSelectDistinct = true;
			return Select(propertiesSelector);
		}

		/// <summary>
		/// Define a projeção para recupera a função COUNT.
		/// </summary>
		/// <returns></returns>
		public Queryable Count()
		{
			if(Unions.Count > 0)
			{
				return (SourceContext != null ? SourceContext.CreateQuery() : new Queryable()).From(this, "tbl").Select("COUNT(*) AS count");
			}
			_isSelectDistinct = false;
			_projection = new Projection(new ProjectionEntry[] {
				ProjectionEntry.Parse("COUNT(*) AS count")
			});
			return this;
		}

		/// <summary>
		/// Conta segundo uma expressão.
		/// </summary>
		/// <param name="countExpression"></param>
		/// <returns></returns>
		public Queryable Count(string countExpression)
		{
			if(Unions.Count > 0)
			{
				return (SourceContext != null ? SourceContext.CreateQuery() : new Queryable()).From(this, "tbl").Select(string.Format("COUNT({0}) AS count", countExpression));
			}
			_isSelectDistinct = false;
			_projection = new Projection(new ProjectionEntry[] {
				ProjectionEntry.Parse(string.Format("COUNT({0}) AS count", countExpression))
			});
			return this;
		}

		/// <summary>
		/// Define a entidade sobre qual a pesquisa será realizada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable From<T>(string alias = null)
		{
			_entity = new EntityInfo(typeof(T).FullName, alias);
			return this;
		}

		/// <summary>
		/// Define a entidade sobre qual a pesquisa será realizada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public Queryable From(EntityInfo entity)
		{
			_entity = entity;
			return this;
		}

		/// <summary>
		/// Define a entidade sobre qual a pesquisa será realizada.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Queryable From(Queryable query, string alias = null)
		{
			query.Require<Queryable>("query");
			_entity = new EntityInfo(query.CreateQueryInfo(), alias);
			return this;
		}

		/// <summary>
		/// Define a ordenação do resultado.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Queryable OrderBy(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				_sort = null;
			else
				_sort = Query.Sort.Parse(expression);
			return this;
		}

		/// <summary>
		/// Define a ordenação do resultado.
		/// </summary>
		/// <param name="sort">Instancia com os dados da ordenação.</param>
		/// <returns></returns>
		public Queryable OrderBy(Sort sort)
		{
			_sort = sort;
			return this;
		}

		/// <summary>
		/// Define o agrupamento do resultado.
		/// </summary>
		/// <param name="groupBy"></param>
		/// <returns></returns>
		public Queryable GroupBy(GroupBy groupBy)
		{
			_groupByClause = groupBy;
			return this;
		}

		/// <summary>
		/// Define o agrupamento do resultado.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Queryable GroupBy(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				_groupByClause = null;
			else
				_groupByClause = Query.GroupBy.Parse(expression);
			return this;
		}

		/// <summary>
		/// Define a projeção da seleção.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertiesSelector">Propriedades.</param>
		/// <returns></returns>
		public Queryable GroupBy<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
		{
			return this.GroupBy(string.Join(",", PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray()));
		}

		/// <summary>
		/// Salta os skip primeiros registros do resultado
		/// </summary>
		/// <param name="skip">Número de registros a serem saltados</param>
		/// <returns>Instância atual do Queryable</returns>
		public Queryable Skip(int skip)
		{
			_takeParameters.Skip = skip;
			return this;
		}

		/// <summary>
		/// Traz os count primeiros registros do resultado
		/// </summary>
		/// <param name="count">Número de registros a serem trazidos</param>
		/// <returns>Instância atual do Queryable</returns>
		public Queryable Take(int count)
		{
			_takeParameters.Take = count;
			return this;
		}

		/// <summary>
		/// Define o tempo de espera do comando.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public Queryable SetCommandTimeout(int timeout)
		{
			_commandTimeout = timeout;
			return this;
		}

		/// <summary>
		/// Define o nível de isolamento da consulta.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		public Queryable SetIsolationLevel(System.Transactions.IsolationLevel isolationLevel)
		{
			_isolationLevel = isolationLevel;
			return this;
		}

		/// <summary>
		/// Identifica que a consulta possui RowVersion.
		/// </summary>
		/// <returns></returns>
		public Queryable HasRowVersion(bool hasRowVersion = true)
		{
			_hasRowVersion = true;
			return this;
		}

		/// <summary>
		/// Inicia uma subconsulta.
		/// </summary>
		/// <returns></returns>
		public Queryable BeginSubQuery()
		{
			return BeginSubQuery(null, null);
		}

		/// <summary>
		/// Inicia uma subconsulta.
		/// </summary>
		/// <param name="callBack">Callback acionado quando a subquery for executada.</param>
		/// <param name="failedCallBack">Callback acioando quando ocorrer alguma falha na execução da subquery.</param>
		/// <returns></returns>
		public Queryable BeginSubQuery(SubQueryCallBack callBack, SubQueryFailedCallBack failedCallBack)
		{
			return new Queryable() {
				SourceContext = this.SourceContext,
				_parent = this,
				_subQueryCallBack = callBack,
				_subQueryFailedCallBack = failedCallBack
			};
		}

		/// <summary>
		/// Finaliza a subconsulta.
		/// </summary>
		/// <returns></returns>
		public Queryable EndSubQuery()
		{
			this._parent.NestedQueries.Add(this);
			return this._parent;
		}

		/// <summary>
		/// Cria uma união com a consulta informada.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Queryable Union(Queryable query)
		{
			Unions.Add(new Union(query, false));
			return this;
		}

		/// <summary>
		/// Cria uma união do tipo ALL com a consulta informada.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Queryable UnionAll(Queryable query)
		{
			Unions.Add(new Union(query, true));
			return this;
		}

		/// <summary>
		/// Configura o predicado que será usado na execuçao.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Queryable ConfigureExecutePredicate(string expression, params QueryParameter[] parameters)
		{
			expression.Require("expression").NotNull().NotEmpty();
			_executePredicate = QueryExecutePredicate.Create(expression, parameters);
			return this;
		}

		/// <summary>
		/// Identifica que não é para usar o cache 
		/// no resultado da consulta.
		/// </summary>
		/// <returns></returns>
		public Queryable NoUseCache()
		{
			CanUseCache = false;
			return this;
		}

		/// <summary>
		/// Executa a consulta na fonte de dados informada.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <returns></returns>
		public IQueryResult Execute(IQueryDataSource dataSource)
		{
			dataSource.Require("dataSource").NotNull();
			if(_entity == null && _storedProcedureName == null)
				throw new QueryException("From undefined for query");
			ProjectionFix();
			var queryInfo = CreateQueryInfo();
			var result = dataSource.Execute(queryInfo);
			_parameters = queryInfo.Parameters;
			return result;
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns></returns>
		public IQueryResult Execute()
		{
			if(_entity == null && _storedProcedureName == null)
				throw new QueryException("From undefined for query");
			else if(_dataSource == null)
				throw new QueryException("DataSource undefined");
			ProjectionFix();
			var queryInfo = CreateQueryInfo();
			var result = _dataSource.Execute(queryInfo);
			_parameters = queryInfo.Parameters;
			return result;
		}

		/// <summary>
		/// 
		/// Executa a consulta.
		/// </summary>
		/// <param name="dataSource">Intancia da origem de dados que será usada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação que será utilizada.</param>
		/// <param name="objectCreator">Instancia responsável pela criação do objeto.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IQueryResult<TModel> Execute<TModel>(IQueryDataSource dataSource, IQueryResultBindStrategy bindStrategy = null, IQueryResultObjectCreator objectCreator = null)
		{
			if(bindStrategy == null && objectCreator == null)
			{
				var ts = TypeBindStrategyCache.GetItem(typeof(TModel), t => new QueryResultObjectCreator(t));
				objectCreator = ts;
				bindStrategy = ts;
			}
			if(bindStrategy == null)
				bindStrategy = TypeBindStrategyCache.GetItem(typeof(TModel), t => objectCreator);
			return new QueryResult<TModel>(Execute(dataSource), bindStrategy, objectCreator);
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns></returns>
		public IQueryResult<TModel> Execute<TModel>()
		{
			if(_entity == null && StoredProcedureName == null)
				throw new InvalidOperationException("From undefined for query");
			else if(_dataSource == null)
				throw new InvalidOperationException("DataSource undefined");
			var ts = TypeBindStrategyCache.GetItem(typeof(TModel), t => new QueryResultObjectCreator(t));
			var objectCreator = ts;
			var bindStrategy = ts;
			return new QueryResult<TModel>(Execute(), bindStrategy, objectCreator);
		}

		/// <summary>
		/// Verifica se a consulta possui algum resultado.
		/// </summary>
		/// <returns></returns>
		public bool ExistsResult()
		{
			return Count().Execute().Select(f => f.GetInt32(0)).FirstOrDefault() > 0;
		}

		/// <summary>
		/// Clona o queryable.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var query = new Queryable {
				_entity = (EntityInfo)this._entity.Clone(),
				_joinEntities = new Queue<EntityInfo>(this._joinEntities.Select(f => (EntityInfo)f.Clone())),
				_groupByClause = (this._groupByClause != null) ? (GroupBy)this._groupByClause.Clone() : null,
				_havingClause = (this._havingClause != null) ? (ConditionalContainer)this._havingClause.Clone() : null,
				_parameters = (QueryParameterCollection)this._parameters.Clone(),
				_projection = (this.Projection != null) ? (Projection)this.Projection.Clone() : null,
				_executePredicate = (this.ExecutePredicate != null) ? (QueryExecutePredicate)this.ExecutePredicate.Clone() : null,
				_storedProcedureName = this._storedProcedureName,
				_storedProcedureProvider = this._storedProcedureProvider,
				_isolationLevel = this._isolationLevel,
				_commandTimeout = this._commandTimeout,
				_sort = (this._sort != null) ? (Sort)this._sort.Clone() : null,
				_whereClause = (ConditionalContainer)this.WhereClause.Clone(),
				_takeParameters = (TakeParameters)this._takeParameters.Clone(),
				_dataSource = this._dataSource,
				_sourceContext = this._sourceContext,
				_isSelectDistinct = this._isSelectDistinct,
				_canUseCache = this._canUseCache,
				_searchParameterDescriptions = _searchParameterDescriptions != null ? (Collections.SearchParameterDescriptionCollection)_searchParameterDescriptions.Clone() : null,
				_nestedQueries = new List<Queryable>(_nestedQueries.Select(f => (Queryable)f.Clone())),
				_unions = (UnionCollection)this._unions.Clone(),
				_subQueryCallBack = _subQueryCallBack,
				_subQueryFailedCallBack = _subQueryFailedCallBack,
				_providerName = _providerName,
				_ignoreTypeSchema = _ignoreTypeSchema,
				_hasRowVersion = _hasRowVersion
			};
			for(int i = 0; i < _joins.Count; i++)
			{
				query.Joins.Add((JoinInfo)_joins[i].Clone());
			}
			return query;
		}

		/// <summary>
		/// Fixa a projeção da consulta.
		/// </summary>
		private void ProjectionFix()
		{
			if((_projection == null || _projection.Count == 0) && _entity != null)
			{
				var projection = ProjectionProvider.Instance.Search(_entity);
				if(projection != null && projection.Count > 0)
				{
					_projection = new Query.Projection(projection.Select(f => new ProjectionEntry(string.Format("{0}.{1}", _entity.Alias, f.ColumnName), f.Alias)));
				}
			}
		}

		/// <summary>
		/// Cria um QueryInfo da instancia.
		/// </summary>
		/// <returns></returns>
		public QueryInfo CreateQueryInfo(QueryMethod method = QueryMethod.Select)
		{
			var queryParameterCollection = new QueryParameterCollection(QueryParameter.GetParameters(this, false));
			var whereClause = (ConditionalContainer)_whereClause.Clone();
			((IQueryParameterContainer)whereClause).RemoveAllParameters();
			var queryInfo = new QueryInfo {
				Method = method,
				Entities = (_entity != null ? new EntityInfo[] {
					_entity
				} : new EntityInfo[0]).Union(_joinEntities).ToArray(),
				Joins = _joins.ToArray(),
				WhereClause = whereClause,
				Projection = _projection,
				ExecutePredicate = _executePredicate,
				Sort = _sort,
				GroupBy = _groupByClause,
				Having = _havingClause,
				Parameters = queryParameterCollection,
				TakeParameters = _takeParameters,
				StoredProcedureName = _storedProcedureName,
				StoredProcedureProvider = _storedProcedureProvider,
				IsolationLevel = _isolationLevel,
				CommandTimeout = _commandTimeout,
				NestedQueries = _nestedQueries != null ? _nestedQueries.Select(f => f.CreateQueryInfo()).ToArray() : null,
				Unions = new UnionInfoCollection(_unions.Select(f => f.CreateUnionInfo())),
				ExecuteObserver = this,
				IsSelectDistinct = _isSelectDistinct,
				CanUseCache = CanUseCache,
				ProviderName = ProviderName,
				IgnoreTypeSchema = IgnoreTypeSchemaInternal,
				HasRowVersion = _hasRowVersion
			};
			queryInfo.NestedQueriesProcessed += QueryInfoNestedQueriesProcessed;
			queryInfo.NestedQueryProcessed += QueryInfoNestedQueryProcessed;
			queryInfo.NestedQueriesRecordProcessed += QueryInfoNestedQueriesRecordProcessed;
			return queryInfo;
		}

		/// <summary>
		/// Método acionado quando o registro de uma consulta aninhada for processada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QueryInfoNestedQueriesRecordProcessed(object sender, NestedQueriesRecordProcessedEventArgs e)
		{
			if(NestedQueriesRecordProcessed != null)
				NestedQueriesRecordProcessed(this, e);
		}

		/// <summary>
		/// Método acionado quando uma consulta aninhada for processada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QueryInfoNestedQueryProcessed(object sender, NestedQueryProcessedEventArgs e)
		{
			if(NestedQueryProcessed != null)
				NestedQueryProcessed(this, e);
		}

		/// <summary>
		/// Método acionado quando as consultas aninhadas forem processadas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QueryInfoNestedQueriesProcessed(object sender, NestedQueriesProcessedEventArgs e)
		{
			if(NestedQueriesProcessed != null)
				NestedQueriesProcessed(this, e);
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("IsolationLevel", (short)IsolationLevel);
			info.AddValue("CommandTimeout", CommandTimeout);
			info.AddValue("ProviderName", ProviderName);
			info.AddValue("Entity", _entity);
			info.AddValue("GroupByClause", _groupByClause);
			info.AddValue("HavingClause", _havingClause);
			info.AddValue("Parameters", _parameters);
			info.AddValue("Projection", _projection);
			info.AddValue("ExecutePredicate", _executePredicate);
			info.AddValue("Sort", _sort);
			info.AddValue("WhereClause", _whereClause);
			info.AddValue("IgnoreTypeSchema", _ignoreTypeSchema);
			var index = 0;
			info.AddValue("JC", _joins.Count);
			foreach (var i in _joins)
				info.AddValue("j" + (index++).ToString(), i);
			index = 0;
			info.AddValue("JEC", _joinEntities.Count);
			foreach (var i in _joinEntities)
				info.AddValue("je" + (index++).ToString(), i);
			index = 0;
			info.AddValue("NQ", _nestedQueries.Count);
			foreach (var i in _nestedQueries)
				info.AddValue("nq" + (index++).ToString(), i);
			info.AddValue("Unions", _unions);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Queryable", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			var attribute = reader.GetAttribute("IsolationLevel");
			if(!Enum.TryParse<System.Transactions.IsolationLevel>(attribute, out _isolationLevel))
				_isolationLevel = System.Transactions.IsolationLevel.Unspecified;
			attribute = reader.GetAttribute("CommandTimeout");
			if(!int.TryParse(attribute, out _commandTimeout))
				_commandTimeout = 30;
			_providerName = reader.GetAttribute("ProviderName");
			attribute = reader.GetAttribute("IgnoreTypeSchema");
			bool.TryParse(attribute, out _ignoreTypeSchema);
			_entity = ReadItem<EntityInfo>(reader, "Entity");
			_projection = ReadItem<Projection>(reader, "Projection");
			_executePredicate = ReadItem<QueryExecutePredicate>(reader, "ExecutePredicate");
			_whereClause = ReadItem<ConditionalContainer>(reader, "Where");
			_sort = ReadItem<Sort>(reader, "Sort");
			_groupByClause = ReadItem<GroupBy>(reader, "GroupBy");
			_havingClause = ReadItem<ConditionalContainer>(reader, "Having");
			_takeParameters = ReadItem<TakeParameters>(reader, "TakeParameters");
			if(!reader.IsEmptyElement && reader.LocalName == "Joins")
			{
				reader.ReadStartElement("Joins", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var join = new JoinInfo();
					((System.Xml.Serialization.IXmlSerializable)join).ReadXml(reader);
					_joins.Add(join);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "JoinsEntities")
			{
				reader.ReadStartElement("JoinsEntities", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var join = new EntityInfo();
					((System.Xml.Serialization.IXmlSerializable)join).ReadXml(reader);
					_joinEntities.Enqueue(join);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "NestedQueries")
			{
				reader.ReadStartElement("NestedQueries", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var queryable = new Queryable();
					((System.Xml.Serialization.IXmlSerializable)queryable).ReadXml(reader);
					_nestedQueries.Add(queryable);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "Parameters")
			{
				reader.ReadStartElement("Parameters", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var parameter = new QueryParameter();
					((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
					_parameters.Add(parameter);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "Unions")
			{
				reader.ReadStartElement("Unions", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var union = new Union();
					((System.Xml.Serialization.IXmlSerializable)union).ReadXml(reader);
					_unions.Add(union);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteAttributeString("xmlns", "q", null, Namespaces.Query);
			writer.WriteAttributeString("IsolationLevel", IsolationLevel.ToString());
			writer.WriteAttributeString("CommandTimeout", CommandTimeout.ToString());
			writer.WriteAttributeString("ProviderName", ProviderName);
			writer.WriteAttributeString("IgnoreTypeSchema", _ignoreTypeSchema.ToString());
			WriteItem<EntityInfo>(writer, "Entity", _entity);
			WriteItem<Projection>(writer, "Projection", _projection);
			WriteItem<QueryExecutePredicate>(writer, "ExecutePredicate", _executePredicate);
			WriteItem<ConditionalContainer>(writer, "Where", _whereClause);
			WriteItem<Sort>(writer, "Sort", _sort);
			WriteItem<GroupBy>(writer, "GroupBy", _groupByClause);
			WriteItem<ConditionalContainer>(writer, "Having", _havingClause);
			WriteItem<TakeParameters>(writer, "TakeParameters", _takeParameters);
			writer.WriteStartElement("Joins", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in _joins)
			{
				writer.WriteStartElement("JoinInfo", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("JoinsEntities", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in _joinEntities)
			{
				writer.WriteStartElement("EntityInfo", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("NestedQueries", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in _nestedQueries)
			{
				writer.WriteStartElement("Queryable", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in _parameters)
			{
				writer.WriteStartElement("QueryParameter", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Unions", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in _unions)
			{
				writer.WriteStartElement("Union", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera o item do Reader.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será preechida.</typeparam>
		/// <param name="reader"></param>
		/// <param name="itemName">Nome do item.</param>
		private static T ReadItem<T>(System.Xml.XmlReader reader, string itemName) where T : System.Xml.Serialization.IXmlSerializable, new()
		{
			if(reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == itemName && (!reader.IsEmptyElement || reader.HasAttributes))
			{
				T item = new T();
				item.ReadXml(reader);
				return item;
			}
			reader.Skip();
			return default(T);
		}

		/// <summary>
		/// Escreve os dados do item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="itemName">Nome do item.</param>
		/// <param name="item">Instancia do item.</param>
		private static void WriteItem<T>(System.Xml.XmlWriter writer, string itemName, T item) where T : System.Xml.Serialization.IXmlSerializable
		{
			writer.WriteStartElement(itemName, Namespaces.Query);
			if(item != null)
				((System.Xml.Serialization.IXmlSerializable)item).WriteXml(writer);
			writer.WriteEndElement();
		}

		bool IQueryExecuteObserver.ErrorHandlerEnabled
		{
			get
			{
				return _subQueryFailedCallBack != null;
			}
		}

		void IQueryExecuteObserver.Executed(QueryInfo info, ReferenceParameterValueCollection referenceValues, IQueryResult result)
		{
			if(_subQueryCallBack != null)
				_subQueryCallBack(this, new SubQueryCallBackArgs(info, referenceValues, result));
		}

		void IQueryExecuteObserver.Error(QueryInfo info, QueryFailedInfo fail)
		{
			if(_subQueryFailedCallBack != null)
				_subQueryFailedCallBack(this, new SubQueryCallBackFailedArgs(info, fail));
		}

		/// <summary>
		/// Descrições dos parametros de pesquisa.
		/// </summary>
		public Collections.SearchParameterDescriptionCollection SearchParameterDescriptions
		{
			get
			{
				if(_searchParameterDescriptions == null)
					_searchParameterDescriptions = new Collections.SearchParameterDescriptionCollection();
				return _searchParameterDescriptions;
			}
		}
	}
}
