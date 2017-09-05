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

namespace Colosoft.Data.Caching.Dymanic
{
	/// <summary>
	/// Implementação da sessão de persistencia dinâmica
	/// </summary>
	class DynamicPersistenceSession : IPersistenceSession
	{
		private IPersistenceSession _databaseSession;

		private Func<IPersistenceExecuter> _defaultExecuterCreator;

		private IPersistenceSession _cacheSession;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Query.IRecordKeyFactory _recordKeyFactory;

		private PersistenceSessionParameters _parameters;

		/// <summary>
		/// Evento acionado quanod a sessão for executada.
		/// </summary>
		public event PersistenceSessionExecutedHandler Executed {
			add
			{
				_databaseSession.Executed += value;
			}
			remove {
				_databaseSession.Executed -= value;
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
		/// Quantidade de ações na sessão.
		/// </summary>
		public int Count
		{
			get
			{
				return _databaseSession.Count;
			}
		}

		/// <summary>
		/// Recupera a ação na posição informada.
		/// </summary>
		/// <param name="index">Identificador da ação.</param>
		/// <returns></returns>
		public PersistenceAction this[int index]
		{
			get
			{
				return _databaseSession[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="databaseSession">Instancia da sessão de persistencia do banco de dados.</param>
		/// <param name="cacheSession">Instancia da sessão de persistencia do cache.</param>
		/// <param name="typeSchema">Instancia dos esquemas dos tipos do sistema.</param>
		/// <param name="recordKeyFactory">Instancia da factory responsável pela criação das chaves de registro.</param>
		public DynamicPersistenceSession(IPersistenceSession databaseSession, IPersistenceSession cacheSession, Colosoft.Data.Schema.ITypeSchema typeSchema, Query.IRecordKeyFactory recordKeyFactory)
		{
			databaseSession.Require("databaseSession").NotNull();
			cacheSession.Require("cacheSession").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			recordKeyFactory.Require("recordKeyFactory").NotNull();
			_databaseSession = databaseSession;
			_cacheSession = cacheSession;
			_typeSchema = typeSchema;
			_recordKeyFactory = recordKeyFactory;
			_defaultExecuterCreator = ((IPersistenceExecuterFactory)_databaseSession).ExecuterCreator;
			((IPersistenceExecuterFactory)_databaseSession).ExecuterCreator = CreateExecuter;
		}

		/// <summary>
		/// Cria o executar da sessão.
		/// </summary>
		/// <returns></returns>
		public IPersistenceExecuter CreateExecuter()
		{
			Colosoft.Caching.ICacheProvider cacheProvider = null;
			var session2 = _cacheSession as Colosoft.Caching.CachePersistenceSession;
			if(session2 != null)
				cacheProvider = session2.CacheProvider;
			return new DynamicPersistenceExecuter(_defaultExecuterCreator(), ((IPersistenceExecuterFactory)_cacheSession).CreateExecuter(), cacheProvider, _typeSchema, _recordKeyFactory);
		}

		/// <summary>
		/// Cria uma sessão de persistencia para ações posteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateAfterSessionForAction(int actionId)
		{
			return _databaseSession.CreateAfterSessionForAction(actionId);
		}

		/// <summary>
		/// Cria uma seção de persistencia para ações alternativas da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateAlternativeSessionForAction(int actionId)
		{
			return _databaseSession.CreateAlternativeSessionForAction(actionId);
		}

		/// <summary>
		/// Cria uma sessão de persistencia para ações anteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		public IPersistenceSession CreateBeforeSessionForAction(int actionId)
		{
			return _databaseSession.CreateBeforeSessionForAction(actionId);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(Query.ConditionalContainer conditional) where T : class
		{
			return _databaseSession.Delete<T>(conditional);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class
		{
			return _databaseSession.Delete<T>(conditional, callback);
		}

		/// <summary>
		/// Registra a operação de remoção.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será apagada.</typeparam>
		/// <param name="instance">Instância que será apagada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance) where T : class
		{
			return _databaseSession.Delete<T>(instance);
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
			return _databaseSession.Delete<T>(instance, callback);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, Query.ConditionalContainer conditional) where T : class
		{
			return _databaseSession.Delete<T>(instance, conditional);
		}

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Delete<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class
		{
			return _databaseSession.Delete(instance, conditional, callback);
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
			return _databaseSession.Delete(instance, conditional, callback, commandTimeout);
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns></returns>
		public ExecuteActionsResult Execute(bool throwOnError, ExecutionType executionType)
		{
			return _databaseSession.Execute(throwOnError, executionType);
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <returns></returns>
		public ExecuteActionsResult Execute(bool throwOnError)
		{
			return _databaseSession.Execute(throwOnError);
		}

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		public ExecuteActionsResult Execute()
		{
			return _databaseSession.Execute();
		}

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int ExecuteStoredProcedure(PersistenceStoredProcedure procedure, PersistenceActionCallback callback)
		{
			return _databaseSession.ExecuteStoredProcedure(procedure, callback);
		}

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int ExecuteStoredProcedure(PersistenceStoredProcedure procedure)
		{
			return _databaseSession.ExecuteStoredProcedure(procedure);
		}

		/// <summary>
		/// Recupera a ação pelo identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação.</param>
		/// <returns></returns>
		public PersistenceAction GetAction(int actionId)
		{
			return _databaseSession.GetAction(actionId);
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
			return _databaseSession.Insert(instance, propertiesSelector);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Insert(instance, bindStrategy, propertiesSelector);
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
			return _databaseSession.Insert(instance, callback, propertiesSelector);
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
		public int Insert<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Insert(instance, callback, bindStrategy, propertiesSelector);
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
		public int Insert<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Insert(instance, callback, bindStrategy, commandTimeout, propertiesSelector);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance) where T : class
		{
			return _databaseSession.Insert(instance);
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
			return _databaseSession.Insert(instance, propertyNames);
		}

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Insert<T>(T instance, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Insert(instance, bindStrategy, propertyNames);
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
			return _databaseSession.Insert(instance, callback, propertyNames);
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
		public int Insert<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Insert(instance, callback, bindStrategy, propertyNames);
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
		public int Insert<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class
		{
			return _databaseSession.Insert(instance, callback, bindStrategy, commandTimeout, propertyNames);
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
			return _databaseSession.Update(instance, propertiesSelector);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor de propriedades que serão atualizadas.</param>
		/// <returns>Identificador de ação registrada.</returns>
		public int Update<T>(T instance, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, bindStrategy, propertiesSelector);
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
			return _databaseSession.Update(instance, callback, propertiesSelector);
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
		public int Update<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, callback, bindStrategy, propertiesSelector);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, Query.ConditionalContainer conditional, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, conditional, propertiesSelector);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, conditional, bindStrategy, propertiesSelector);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, propertiesSelector);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, bindStrategy, propertiesSelector);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, bindStrategy, commandTimeout, propertiesSelector);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance) where T : class
		{
			return _databaseSession.Update(instance);
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
			return _databaseSession.Update(instance, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instância no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instância que será atualizada.</typeparam>
		/// <param name="instance">Instância com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, bindStrategy, propertyNames);
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
			return _databaseSession.Update(instance, callback, propertyNames);
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
		public int Update<T>(T instance, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, callback, bindStrategy, propertyNames);
		}

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		public int Update<T>(T instance, Query.ConditionalContainer conditional, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, conditional, propertyNames);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, conditional, bindStrategy, propertyNames);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, bindStrategy, commandTimeout, propertyNames);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, propertyNames);
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
		public int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, Query.IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class
		{
			return _databaseSession.Update(instance, conditional, callback, bindStrategy, propertyNames);
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
			return _databaseSession.Update(parameters, conditional, callback, (string[])null);
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
			return _databaseSession.Update<T>(parameters, conditional, callback, commandTimeout);
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
			return _databaseSession.Update(parameters, query, callback);
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
			return _databaseSession.Update(parameters, query, callback, commandTimeout);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_databaseSession.Dispose();
			_cacheSession.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Recupera o enumerador das ações.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<PersistenceAction> GetEnumerator()
		{
			return _databaseSession.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das ações.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _databaseSession.GetEnumerator();
		}
	}
}
