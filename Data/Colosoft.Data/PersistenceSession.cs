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
using Colosoft.Query;

namespace Colosoft.Data
{
	/// <summary>
	/// Implementação básicas dos métodos da sessão de persistencia <see cref="IPersistenceSession"/>.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public abstract class PersistenceSession : IPersistenceSession, IPersistenceExecuterFactory, IPersistenceExecuterProvider, IPersistenceActionContainer
	{
		private List<PersistenceAction> _actions = new List<PersistenceAction>();

		private int _lastActionId;

		Dictionary<Type, IQueryResultBindStrategy> _bindStrategys = new Dictionary<Type, IQueryResultBindStrategy>();

		private Func<IPersistenceExecuter> _executerCreator;

		private PersistenceSessionParameters _parameters;

		private bool _ignoreEmptyActions;

		private static bool _ignoreAllEmptyActions;

		/// <summary>
		/// Evento acionado quando a sessão for executada.
		/// </summary>
		public virtual event PersistenceSessionExecutedHandler Executed;

		/// <summary>
		/// Identifica se é para ignorar as ações vazias para a sessão.
		/// </summary>
		public bool IgnoreEmptyActions
		{
			get
			{
				return _ignoreEmptyActions || IgnoreAllEmptyActions;
			}
			set
			{
				_ignoreEmptyActions = value;
			}
		}

		/// <summary>
		/// Identifica se é para ignorar todas as ações vazias do sistema.
		/// </summary>
		public static bool IgnoreAllEmptyActions
		{
			get
			{
				return _ignoreAllEmptyActions;
			}
			set
			{
				_ignoreAllEmptyActions = value;
			}
		}

		/// <summary>
		/// Parametros adicionais da sessão.
		/// </summary>
		public PersistenceSessionParameters Parameters
		{
			get
			{
				if(_parameters == null)
					_parameters = new PersistenceSessionParameters();
				return _parameters;
			}
		}

		/// <summary>
		/// Identifica a sessão raiz.
		/// </summary>
		public virtual IPersistenceSession Root
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Instacia do criador do executer.
		/// </summary>
		public Func<IPersistenceExecuter> ExecuterCreator
		{
			get
			{
				return _executerCreator ?? CreateExecuter;
			}
			set
			{
				_executerCreator = value;
			}
		}

		/// <summary>
		/// Recupera a ação na posição informada.
		/// </summary>
		/// <param name="index">Identificador da ação.</param>
		/// <returns></returns>
		public virtual PersistenceAction this[int index]
		{
			get
			{
				return Actions[index];
			}
		}

		/// <summary>
		/// Quantidade de ações na sessão.
		/// </summary>
		public virtual int Count
		{
			get
			{
				return Actions.Count;
			}
		}

		/// <summary>
		/// Ações da sessão.
		/// </summary>
		protected virtual List<PersistenceAction> Actions
		{
			get
			{
				return _actions;
			}
		}

		/// <summary>
		/// Método acionado quando a sessão for executada.
		/// </summary>
		/// <param name="result"></param>
		protected virtual void OnExecuted(ExecuteActionsResult result)
		{
			if(Executed != null)
				Executed(this, new PersistenceSessionExecutedEventArgs(result));
		}

		/// <summary>
		/// Cria um novo identificador de ação.
		/// </summary>
		/// <returns></returns>
		internal protected virtual int CreateActionId()
		{
			return ++_lastActionId;
		}

		/// <summary>
		/// Recupera a instancia responsável pela execução da sessão.
		/// </summary>
		/// <returns></returns>
		protected internal abstract IPersistenceExecuter CreateExecuter();

		/// <summary>
		/// Recupera a instancia do validador das sessões.
		/// </summary>
		/// <returns></returns>
		protected internal abstract IPersistenceSessionValidator GetValidator();

		/// <summary>
		/// Recupera as propriedades que podem ser persistidas do tipo informado.
		/// </summary>
		/// <param name="actionType">Tipo de ação que será realizada.</param>
		/// <param name="instanceType">Tipo da instancia onde as propriedades estão inseridas.</param>
		/// <param name="propertyNames">Nomes das propriedades que se deseja utilizar.</param>
		/// <param name="isConditional">True se operação de persistência for condicional.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns></returns>
		protected internal abstract IEnumerable<System.Reflection.PropertyInfo> GetPersistenceProperties(PersistenceActionType actionType, Type instanceType, string[] propertyNames, bool isConditional, DirectionPropertiesName direction = DirectionPropertiesName.Inclusion);

		/// <summary>
		/// Recupera as propriedades que compõem as chave do tipo da instancia informado.
		/// </summary>
		/// <param name="instanceType">Tipo da instancia.</param>
		/// <returns></returns>
		protected internal abstract IEnumerable<string> GetKeyProperties(Type instanceType);

		/// <summary>
		/// Recupera os parametros que serão persistidos.
		/// </summary>
		/// <param name="instanceType">Tipo da instancia de onde os parametros serão recuperados.</param>
		/// <param name="instance">Instancia.</param>
		/// <param name="properties"></param>
		/// <returns></returns>
		protected virtual IEnumerable<PersistenceParameter> GetPersistenceParameters(Type instanceType, object instance, IEnumerable<System.Reflection.PropertyInfo> properties)
		{
			instanceType.Require("instanceType").NotNull();
			if(properties != null)
			{
				foreach (var prop in properties)
				{
					var parameterName = prop.Name;
					var value = prop.GetValue(instance, null);
					yield return new PersistenceParameter(parameterName, value);
				}
			}
		}

		/// <summary>
		/// Registra uma ação para o sessão.
		/// </summary>
		/// <param name="actionType">Tipo da ação.</param>
		/// <param name="instanceType">Tipo da instancia da ação.</param>
		/// <param name="providerName"></param>
		/// <param name="instance">Instancia associada com a ação.</param>
		/// <param name="parameters">Parametros que serão processados pela ação.</param>
		/// <param name="callback">Callback que será registrado para a ação.</param>
		/// <param name="conditional">Condicional para a execução da ação.</param>
		/// <param name="query">Consulta associada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Retorna o identificador da ação.</returns>
		protected virtual int RegisterAction(PersistenceActionType actionType, Type instanceType, string providerName, object instance, IEnumerable<PersistenceParameter> parameters, PersistenceActionCallback callback, ConditionalContainer conditional = null, QueryInfo query = null, int commandTimeout = 30)
		{
			var actionId = CreateActionId();
			var versioned = instance as IVersionedModel;
			long? rowVersion = null;
			if(versioned != null)
				rowVersion = versioned.RowVersion;
			Actions.Add(new PersistenceAction(actionId, actionType, instanceType == null ? (query != null ? query.Entities.Select(f => f.FullName).FirstOrDefault() : null) : instanceType.FullName, providerName, parameters.ToArray(), instance, callback, rowVersion, conditional, query) {
				CommandTimeout = commandTimeout
			});
			return actionId;
		}

		/// <summary>
		/// Registra um StoredProcedure para ser executada.
		/// </summary>
		/// <param name="procedure">Procedure a ser executada.</param>
		/// <param name="callback">Callback que será registrado para a ação.</param>
		/// <returns>Retorna o identificador da ação.</returns>
		protected virtual int RegisterStoredProcedure(PersistenceStoredProcedure procedure, PersistenceActionCallback callback)
		{
			procedure.Require("procedure").NotNull();
			var actionId = CreateActionId();
			Actions.Add(new PersistenceAction(actionId, PersistenceActionType.ExecuteProcedure, null, procedure.ProviderName, procedure.ToArray(), null, callback, null) {
				StoredProcedureName = procedure.Name,
				CommandTimeout = procedure.CommandTimeout
			});
			return actionId;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			Actions.Clear();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Actions: {0}]", Count);
		}

		/// <summary>
		/// Cria uma sessão de persistencia para ações anteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateBeforeSessionForAction(int actionId)
		{
			var action = GetAction(actionId);
			if(action == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.ActionNotFound, actionId).Format());
			return new AggregatePersistenceSession(this, action.BeforeActions);
		}

		/// <summary>
		/// Cria uma sessão de persistencia para ações posteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateAfterSessionForAction(int actionId)
		{
			var action = GetAction(actionId);
			if(action == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.ActionNotFound, actionId).Format());
			return new AggregatePersistenceSession(this, action.AfterActions);
		}

		/// <summary>
		/// Cria uma seção de persistencia para ações alternativas da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateAlternativeSessionForAction(int actionId)
		{
			var action = GetAction(actionId);
			if(action == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.ActionNotFound, actionId).Format());
			return new AggregatePersistenceSession(this, action.AlternativeActions);
		}

		/// <summary>
		/// Recupera a ação pelo identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação.</param>
		/// <returns></returns>
		public virtual PersistenceAction GetAction(int actionId)
		{
			return Actions.FirstOrDefault(f => f.ActionId == actionId);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class
		{
			instance.Require("instance").NotNull();
			Type instanceType = typeof(T);
			if(bindStrategy != null)
				_bindStrategys.Add(typeof(T), bindStrategy);
			var properties = GetPersistenceProperties(PersistenceActionType.Insert, instanceType, propertyNames, false);
			var parameters = GetPersistenceParameters(instanceType, instance, properties).ToArray();
			return RegisterAction(PersistenceActionType.Insert, instanceType, null, instance, parameters, callback, commandTimeout: commandTimeout);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			instance.Require("instance").NotNull();
			Type instanceType = typeof(T);
			if(bindStrategy != null)
				_bindStrategys.Add(typeof(T), bindStrategy);
			var properties = GetPersistenceProperties(PersistenceActionType.Insert, instanceType, propertyNames, false);
			var parameters = GetPersistenceParameters(instanceType, instance, properties).ToArray();
			return RegisterAction(PersistenceActionType.Insert, instanceType, null, instance, parameters, callback);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, params string[] propertyNames) where T : class
		{
			return Insert(instance, callback, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return Insert(instance, null, bindStrategy, propertyNames);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance) where T : class
		{
			return Insert(instance, null, null, (string[])null);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, params string[] propertyNames) where T : class
		{
			return Insert(instance, null, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Insert(instance, callback, bindStrategy, commandTimeout, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Insert(instance, callback, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Insert(instance, callback, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Insert(instance, null, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Insert(instance, null, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class
		{
			instance.Require("instance").NotNull();
			Type instanceType = typeof(T);
			var properties = GetPersistenceProperties(PersistenceActionType.Update, instanceType, propertyNames, conditional != null).ToArray();
			if(properties.Length == 0)
				return 0;
			var keyPropeties = GetKeyProperties(instanceType).ToArray();
			if(properties.Length == keyPropeties.Length)
			{
				var equalsProperties = true;
				for(var i = 0; equalsProperties && i < keyPropeties.Length; i++)
					equalsProperties = properties.Any(f => f.Name == keyPropeties[i]);
				if(equalsProperties)
					return 0;
			}
			var parameters = GetPersistenceParameters(instanceType, instance, properties).ToArray();
			return RegisterAction(PersistenceActionType.Update, instanceType, null, instance, parameters, callback, conditional, commandTimeout: commandTimeout);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return Update(instance, conditional, callback, bindStrategy, 30, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update dos dados do tipo T no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="parameters">Coleção dos parametros que serão utilizados para realizar a atualização.</param>
		/// <param name="conditional">Condicional da atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(PersistenceParameterCollection parameters, Query.ConditionalContainer conditional, PersistenceActionCallback callback, int commandTimeout) where T : class
		{
			parameters.Require("parameters").NotNull().NotEmptyCollection();
			Type instanceType = typeof(T);
			return RegisterAction(PersistenceActionType.Update, instanceType, null, null, parameters, callback, conditional, commandTimeout: commandTimeout);
		}

		/// <summary>
		/// Registra a operação de update dos dados do tipo T no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="parameters">Coleção dos parametros que serão utilizados para realizar a atualização.</param>
		/// <param name="conditional">Condicional da atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(PersistenceParameterCollection parameters, Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class
		{
			parameters.Require("parameters").NotNull().NotEmptyCollection();
			Type instanceType = typeof(T);
			return RegisterAction(PersistenceActionType.Update, instanceType, null, null, parameters, callback, conditional);
		}

		/// <summary>
		/// Registra a operação de update para os dados baseados na consulta informada.
		/// </summary>
		/// <param name="parameters">Coleção de parametros quer serão utilizados para realizar a atualização.</param>
		/// <param name="query">Consulta que será usada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update(PersistenceParameterCollection parameters, Query.QueryInfo query, PersistenceActionCallback callback, int commandTimeout)
		{
			parameters.Require("parameters").NotNull().NotEmptyCollection();
			parameters.Require("query").NotNull();
			return RegisterAction(PersistenceActionType.Update, null, null, null, parameters, callback, query.WhereClause, query, commandTimeout: commandTimeout);
		}

		/// <summary>
		/// Registra a operação de update para os dados baseados na consulta informada.
		/// </summary>
		/// <param name="parameters">Coleção de parametros quer serão utilizados para realizar a atualização.</param>
		/// <param name="query">Consulta que será usada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update(PersistenceParameterCollection parameters, Query.QueryInfo query, PersistenceActionCallback callback)
		{
			parameters.Require("parameters").NotNull().NotEmptyCollection();
			parameters.Require("query").NotNull();
			return RegisterAction(PersistenceActionType.Update, null, null, null, parameters, callback, null, query);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, PersistenceActionCallback callback, params string[] propertyNames) where T : class
		{
			return Update(instance, conditional, callback, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return Update(instance, conditional, null, bindStrategy, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, params string[] propertyNames) where T : class
		{
			return Update(instance, conditional, null, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return Update(instance, null, callback, bindStrategy, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando ação for executada.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, PersistenceActionCallback callback, params string[] propertyNames) where T : class
		{
			return Update(instance, null, callback, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return Update(instance, null, null, bindStrategy, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance) where T : class
		{
			return Update(instance, null, null, null, (string[])null);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, params string[] propertyNames) where T : class
		{
			return Update(instance, null, null, null, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, conditional, callback, bindStrategy, commandTimeout, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, conditional, callback, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, conditional, callback, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, conditional, null, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, ConditionalContainer conditional, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, conditional, null, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor de propriedades que serão atualizadas.</param>
		/// <returns>Identificador de ação registrada.</returns>
		public int Update<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, null, callback, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor de propriedades que serão atualizadas.</param>
		/// <returns>Identificador de ação registrada.</returns>
		public int Update<T>(T instance, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, null, callback, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor de propriedades que serão atualizadas.</param>
		/// <returns>Identificador de ação registrada.</returns>
		public int Update<T>(T instance, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, null, null, bindStrategy, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="propertiesSelector">Seletor de propriedades que serão atualizadas.</param>
		/// <returns>Identificador de ação registrada.</returns>
		public int Update<T>(T instance, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return Update(instance, null, null, null, PropertySelector<T>.GetPropertyNames(propertiesSelector).ToArray());
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, int commandTimeout) where T : class
		{
			Type instanceType = typeof(T);
			var properties = GetPersistenceProperties(PersistenceActionType.Delete, instanceType, null, conditional != null);
			var parameters = GetPersistenceParameters(instanceType, instance, properties).ToArray();
			return RegisterAction(PersistenceActionType.Delete, instanceType, null, instance, parameters, callback, conditional, commandTimeout: commandTimeout);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, ConditionalContainer conditional, PersistenceActionCallback callback) where T : class
		{
			return Delete(instance, conditional, callback, 30);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, ConditionalContainer conditional) where T : class
		{
			return Delete(instance, conditional, null);
		}

		/// <summary>
		/// Registra a operação de remoção.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será apagada.</typeparam>
		/// <param name="instance">Instância que será apagada.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, PersistenceActionCallback callback) where T : class
		{
			return Delete(instance, null, callback);
		}

		/// <summary>
		/// Registra a operação de remoção.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será apagada.</typeparam>
		/// <param name="instance">Instância que será apagada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance) where T : class
		{
			return Delete(instance, null, null);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(ConditionalContainer conditional, PersistenceActionCallback callback) where T : class
		{
			return Delete<T>(null, conditional, callback);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(ConditionalContainer conditional) where T : class
		{
			return Delete<T>(null, conditional, null);
		}

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int ExecuteStoredProcedure(PersistenceStoredProcedure procedure)
		{
			return ExecuteStoredProcedure(procedure, null);
		}

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int ExecuteStoredProcedure(PersistenceStoredProcedure procedure, PersistenceActionCallback callback)
		{
			return RegisterStoredProcedure(procedure, callback);
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
		public ExecuteActionsResult Execute()
		{
			return Execute(false, ExecutionType.Default);
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
		public ExecuteActionsResult Execute(bool throwOnError)
		{
			return Execute(throwOnError, ExecutionType.Default);
		}

		/// <summary>
		/// Realiza o merge dos dados.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="validateResult"></param>
		/// <param name="results"></param>
		private bool Merge(IEnumerable<PersistenceAction> actions, PersistenceSessionValidateResult validateResult, out PersistenceActionResult[] results)
		{
			var actionResults = new List<PersistenceActionResult>();
			var success = true;
			foreach (var action in actions)
			{
				var actionResult = new PersistenceActionResult {
					ActionId = action.ActionId
				};
				var actionError = validateResult.Errors.Where(f => f.Action.ActionId == action.ActionId).FirstOrDefault();
				PersistenceActionResult[] beforeResults = null;
				PersistenceActionResult[] afterResults = null;
				var beforeSuccess = Merge(action.BeforeActions, validateResult, out beforeResults);
				var afterSuccess = Merge(action.AfterActions, validateResult, out afterResults);
				actionResult.BeforeActions = beforeResults;
				actionResult.AfterActions = afterResults;
				actionResult.AlternativeActions = action.AlternativeActions.Select(f => new PersistenceActionResult {
					ActionId = f.ActionId,
					Success = true
				}).ToArray();
				if(actionError != null || !beforeSuccess || !afterSuccess)
				{
					actionResult.FailureMessage = actionError.Message.Format();
					actionResult.Success = false;
				}
				if(success && (!beforeSuccess || !afterSuccess))
					success = false;
				actionResults.Add(actionResult);
			}
			results = actionResults.ToArray();
			return success;
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
		public virtual ExecuteActionsResult Execute(bool throwOnError, ExecutionType executionType)
		{
			ExecuteActionsResultStatus resultStatus = ExecuteActionsResultStatus.Success;
			var actions = Actions.ToArray();
			PersistenceActionResult[] results = null;
			PersistenceExecuteResult executeResult = null;
			Exception lastException = null;
			IPersistenceExecuter executer = null;
			if(Count == 0)
			{
				if(!IgnoreEmptyActions)
				{
					lastException = new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_NotFoundActionsForExecuteSession).Format());
					resultStatus = ExecuteActionsResultStatus.Fail;
					if(throwOnError)
						throw lastException;
					resultStatus = ExecuteActionsResultStatus.Success;
				}
				else
				{
					results = new PersistenceActionResult[0];
					resultStatus = ExecuteActionsResultStatus.Success;
				}
			}
			if(lastException == null && Count > 0)
			{
				var validator = GetValidator();
				PersistenceSessionValidateResult validateResult = null;
				if(validator != null)
				{
					validateResult = validator.Validate(this);
					if(!validateResult.Success)
					{
						Merge(actions, validateResult, out results);
					}
				}
				if(validateResult == null || validateResult.Success)
				{
					try
					{
						executer = CreateExecuter();
						executeResult = executer.Execute(actions, executionType);
						results = executeResult.ActionsResult;
					}
					catch(Exception ex)
					{
						if(executer != null)
						{
							executer.Dispose();
							executer = null;
						}
						resultStatus = ExecuteActionsResultStatus.ErrorOnComunication;
						lastException = ex;
					}
				}
			}
			if(lastException != null)
			{
				results = actions.Select(f => new PersistenceActionResult {
					ActionId = f.ActionId,
					Success = false
				}).ToArray();
				if(results.Length > 0)
					results[0].FailureMessage = Colosoft.Diagnostics.ExceptionFormatter.FormatException(lastException, false);
			}
			var executeActions = new ExecuteActionsResult.ExecuteAction[actions.Length];
			var failAction = results.FirstOrDefault(f => f != null && !f.Success);
			if(failAction != null && string.IsNullOrEmpty(failAction.FailureMessage))
				failAction = results.FirstOrDefault(f => f != null && !f.Success && !string.IsNullOrEmpty(f.FailureMessage)) ?? failAction;
			try
			{
				if(failAction == null)
					executeActions = ProcessExecuteResult(actions, results);
				else
					executeActions = ProcessExecuteFailResult(actions, results);
			}
			catch(Exception ex)
			{
				if(executer != null)
				{
					executer.Dispose();
					executer = null;
				}
				lastException = ex;
				resultStatus = ExecuteActionsResultStatus.Fail;
			}
			try
			{
				if(executer is IPersistenceExecuteResultProcessor)
				{
					var processor = (IPersistenceExecuteResultProcessor)executer;
					var processResult = processor.Process(executeResult);
					if(!processResult.Success)
					{
						lastException = new PersistenceExecuterResultException(processResult.Message != null ? processResult.Message : ResourceMessageFormatter.Create(() => Properties.Resources.PersistenceExecuteResultProcessorError));
						resultStatus = ExecuteActionsResultStatus.Fail;
					}
				}
			}
			catch(Exception ex)
			{
				lastException = ex;
				resultStatus = ExecuteActionsResultStatus.Fail;
			}
			finally
			{
				if(executer != null)
					executer.Dispose();
			}
			ExecuteActionsResult result = null;
			if(failAction != null && resultStatus == ExecuteActionsResultStatus.Success)
			{
				resultStatus = ExecuteActionsResultStatus.Fail;
				var failureMessage = failAction.GetRecursiveFailureMessage();
				if(throwOnError)
					lastException = new Exception(failureMessage);
				result = new ExecuteActionsResult(executeActions, resultStatus, failureMessage);
			}
			else
				result = new ExecuteActionsResult(executeActions, resultStatus, failAction != null ? failAction.FailureMessage : (lastException != null ? Colosoft.Diagnostics.ExceptionFormatter.FormatException(lastException, false) : null));
			OnExecuted(result);
			if(throwOnError && lastException != null)
				throw lastException;
			return result;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Processa o resultado da execução.
		/// </summary>
		/// <param name="actions">Ações.</param>
		/// <param name="results">Resultados das ações.</param>
		/// <returns></returns>
		private ExecuteActionsResult.ExecuteAction[] ProcessExecuteResult(PersistenceAction[] actions, PersistenceActionResult[] results)
		{
			if(actions.Length != results.Length)
				throw new InvalidOperationException("Action length not equal Result length");
			var result = new ExecuteActionsResult.ExecuteAction[actions.Length];
			for(int i = 0; i < actions.Length; i++)
			{
				ExecuteActionsResult.ExecuteAction[] beforeActions = null;
				if(actions[i].BeforeActions.Count > 0)
				{
					beforeActions = ProcessExecuteResult(actions[i].BeforeActions.ToArray(), results[i].BeforeActions);
				}
				if(results[i].RowVersion > 0 || actions[i].Type == PersistenceActionType.Insert)
				{
					object aux = actions[i].Instance;
					IQueryResultBindStrategy bs;
					if(!_bindStrategys.TryGetValue(aux.GetType(), out bs))
						bs = TypeBindStrategyCache.GetItem(aux.GetType(), t => new QueryResultObjectCreator(t));
					var changedProperties = bs.Bind(results[i].ToRecord(), BindStrategyMode.All, ref aux);
					actions[i].ChangedProperties = changedProperties.ToArray();
					var versioned = aux as BaseVersionedModel;
					if(versioned != null)
						versioned.SetRowVersion(results[i].RowVersion);
				}
				ExecuteActionsResult.ExecuteAction[] afterActions = null;
				if(actions[i].AfterActions.Count > 0)
					afterActions = ProcessExecuteResult(actions[i].AfterActions.ToArray(), results[i].AfterActions);
				ExecuteActionsResult.ExecuteAction[] alternativeActions = null;
				if(actions[i].AlternativeActions.Count > 0)
				{
					if(results[i].AlternativeActions == null || results[i].AlternativeActions.Length == 0)
					{
						alternativeActions = new ExecuteActionsResult.ExecuteAction[0];
					}
					else
						alternativeActions = ProcessExecuteResult(actions[i].AlternativeActions.ToArray(), results[i].AlternativeActions);
				}
				var ea = new ExecuteActionsResult.ExecuteAction(actions[i], results[i], beforeActions, afterActions, alternativeActions);
				if(ea.Action.Callback != null)
					ea.Action.Callback(ea.Action, ea.Result);
				result[i] = ea;
			}
			return result;
		}

		/// <summary>
		/// Processa o resultado com falha da execução.
		/// </summary>
		/// <param name="actions">Ações.</param>
		/// <param name="results">Resultados das ações.</param>
		/// <returns></returns>
		private ExecuteActionsResult.ExecuteAction[] ProcessExecuteFailResult(PersistenceAction[] actions, PersistenceActionResult[] results)
		{
			var result = new ExecuteActionsResult.ExecuteAction[actions.Length];
			var emptyActionResult = new PersistenceActionResult[0];
			for(int i = 0; i < actions.Length; i++)
			{
				ExecuteActionsResult.ExecuteAction[] beforeActions = null;
				PersistenceActionResult resultInstance = results.Length > i ? results[i] : null;
				if(actions[i].BeforeActions.Count > 0 && resultInstance != null && resultInstance.BeforeActions != null)
				{
					beforeActions = ProcessExecuteFailResult(actions, resultInstance.BeforeActions);
				}
				ExecuteActionsResult.ExecuteAction[] afterActions = null;
				if(actions[i].AfterActions.Count > 0 && resultInstance != null && resultInstance.AfterActions != null)
					afterActions = ProcessExecuteFailResult(actions, resultInstance.AfterActions ?? emptyActionResult);
				ExecuteActionsResult.ExecuteAction[] alternativeActions = null;
				if(actions[i].AlternativeActions.Count > 0 && resultInstance != null && resultInstance.AlternativeActions != null)
					alternativeActions = ProcessExecuteFailResult(actions, resultInstance.AlternativeActions ?? emptyActionResult);
				var ea = new ExecuteActionsResult.ExecuteAction(actions[i], resultInstance, beforeActions, afterActions, alternativeActions);
				result[i] = ea;
			}
			return result;
		}

		/// <summary>
		/// Recupera o enumerador das ações da sessão.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<PersistenceAction> GetEnumerator()
		{
			return Actions.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das ações da sessão.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Actions.GetEnumerator();
		}

		/// <summary>
		/// Cria uma instancia do executor de persistencia.
		/// </summary>
		/// <returns></returns>
		IPersistenceExecuter IPersistenceExecuterFactory.CreateExecuter()
		{
			return CreateExecuter();
		}

		/// <summary>
		/// Cria o executor.
		/// </summary>
		/// <returns></returns>
		IPersistenceExecuter IPersistenceExecuterProvider.CreateExecuter()
		{
			return CreateExecuter();
		}

		/// <summary>
		/// Ações associadas.
		/// </summary>
		IEnumerable<PersistenceAction> IPersistenceActionContainer.Actions
		{
			get
			{
				return Actions;
			}
		}
	}
}
