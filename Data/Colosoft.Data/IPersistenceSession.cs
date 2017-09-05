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
using Colosoft.Query;

namespace Colosoft.Data
{
	/// <summary>
	/// Armazena os parametros adicionais de uma sessão de persistencia.
	/// </summary>
	public class PersistenceSessionParameters
	{
		private System.Collections.Hashtable _values;

		/// <summary>
		/// Recupera  define o nome do parametro.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object this[string name]
		{
			get
			{
				if(_values == null)
					return null;
				return _values;
			}
			set
			{
				if(_values == null)
					_values = new System.Collections.Hashtable();
				_values[name] = value;
			}
		}

		/// <summary>
		/// Quantidade de parametro registrados.
		/// </summary>
		public int Count
		{
			get
			{
				return _values != null ? _values.Count : 0;
			}
		}

		/// <summary>
		/// Verifica se existe o parametro com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			return _values != null && _values.ContainsKey(name);
		}

		/// <summary>
		/// Remove o parametro.
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			if(_values != null)
				_values.Remove(name);
		}
	}
	/// <summary>
	/// Assinatura da classe que irá representar uma seção de persistencia.
	/// </summary>
	public interface IPersistenceSession : IDisposable, IEnumerable<PersistenceAction>
	{
		/// <summary>
		/// Evento acionado quando a sessão for executada.
		/// </summary>
		event PersistenceSessionExecutedHandler Executed;

		/// <summary>
		/// Parametros adicionais para a sessão.
		/// </summary>
		PersistenceSessionParameters Parameters
		{
			get;
		}

		/// <summary>
		/// Recupera a ação na posição informada.
		/// </summary>
		/// <param name="index">Identificador da ação.</param>
		/// <returns></returns>
		PersistenceAction this[int index]
		{
			get;
		}

		/// <summary>
		/// Quantidade de ações na sessão.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Sessão raiz.
		/// </summary>
		IPersistenceSession Root
		{
			get;
		}

		/// <summary>
		/// Cria uma sessão de persistencia para ações anteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		IPersistenceSession CreateBeforeSessionForAction(int actionId);

		/// <summary>
		/// Cria uma sessão de persistencia para ações posteriores da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		IPersistenceSession CreateAfterSessionForAction(int actionId);

		/// <summary>
		/// Cria uma seção de persistencia para ações alternativas da ação associada com o identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação pai.</param>
		/// <returns></returns>
		IPersistenceSession CreateAlternativeSessionForAction(int actionId);

		/// <summary>
		/// Recupera a ação pelo identificador informado.
		/// </summary>
		/// <param name="actionId">Identificador da ação.</param>
		/// <returns></returns>
		PersistenceAction GetAction(int actionId);

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

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
		int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, PersistenceActionCallback callback, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="propertyNames">Propriedades da instancia que serão persistidas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

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
		int Insert<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de insert da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será inserida.</typeparam>
		/// <param name="instance">Instancia que com os dados que serão inseridos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão inseridas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Insert<T>(T instance, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

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
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

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
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, PersistenceActionCallback callback, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, IQueryResultBindStrategy bindStrategy, params string[] propertyNames) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="propertyNames">Nomes das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, params string[] propertyNames) where T : class;

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
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, int commandTimeout, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// /// <param name="conditional">Condicional que será utilizada na atualização.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, Query.ConditionalContainer conditional, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos objetos.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, PersistenceActionCallback callback, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, PersistenceActionCallback callback, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, IQueryResultBindStrategy bindStrategy, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update da instancia no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="instance">Instancia com os dados que serão atualizados.</param>
		/// <param name="propertiesSelector">Seletor das propriedades que serão atualizadas..</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(T instance, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : class;

		/// <summary>
		/// Registra a operação de update dos dados do tipo T no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="parameters">Coleção dos parametros que serão utilizados para realizar a atualização.</param>
		/// <param name="conditional">Condicional da atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(PersistenceParameterCollection parameters, Query.ConditionalContainer conditional, PersistenceActionCallback callback, int commandTimeout) where T : class;

		/// <summary>
		/// Registra a operação de update dos dados do tipo T no sistema.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será atualizada.</typeparam>
		/// <param name="parameters">Coleção dos parametros que serão utilizados para realizar a atualização.</param>
		/// <param name="conditional">Condicional da atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update<T>(PersistenceParameterCollection parameters, Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class;

		/// <summary>
		/// Registra a operação de update para os dados baseados na consulta informada.
		/// </summary>
		/// <param name="parameters">Coleção de parametros quer serão utilizados para realizar a atualização.</param>
		/// <param name="query">Consulta que será usada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update(PersistenceParameterCollection parameters, Query.QueryInfo query, PersistenceActionCallback callback);

		/// <summary>
		/// Registra a operação de update para os dados baseados na consulta informada.
		/// </summary>
		/// <param name="parameters">Coleção de parametros quer serão utilizados para realizar a atualização.</param>
		/// <param name="query">Consulta que será usada na atualização.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Update(PersistenceParameterCollection parameters, Query.QueryInfo query, PersistenceActionCallback callback, int commandTimeout);

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <param name="commandTimeout">Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(T instance, Query.ConditionalContainer conditional, PersistenceActionCallback callback, int commandTimeout) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(T instance, Query.ConditionalContainer conditional) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(T instance, PersistenceActionCallback callback) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="instance">Instancia que será apagada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(T instance) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(Query.ConditionalContainer conditional, PersistenceActionCallback callback) where T : class;

		/// <summary>
		/// Registra a operação de remoção
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será apagada.</typeparam>
		/// <param name="conditional">Condicional que será utilizada na exclusão.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int Delete<T>(Query.ConditionalContainer conditional) where T : class;

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int ExecuteStoredProcedure(PersistenceStoredProcedure procedure);

		/// <summary>
		/// Registra stored procedure a ser executada.
		/// </summary>
		/// <param name="procedure">StoredProcedure a ser executada.</param>
		/// <param name="callback">Callback que será acionado quando a ação for executada.</param>
		/// <returns>Identificador da ação registrada.</returns>
		int ExecuteStoredProcedure(PersistenceStoredProcedure procedure, PersistenceActionCallback callback);

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <returns>Identificador da ação registrada.</returns>
		ExecuteActionsResult Execute();

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <returns>Identificador da ação registrada.</returns>
		ExecuteActionsResult Execute(bool throwOnError);

		/// <summary>
		/// Executa as ações da sessão de persistencia.
		/// </summary>
		/// <param name="throwOnError">Identifica se é para dispara algum excessão caso um erro ocorra na execução.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns>Identificador da ação registrada.</returns>
		ExecuteActionsResult Execute(bool throwOnError, ExecutionType executionType);
	}
}
