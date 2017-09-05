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
using Colosoft.Data.Schema;
using Microsoft.Practices.ServiceLocation;

namespace Colosoft.Query.Database
{
	/// <summary>
	/// Implementação de <see cref="IQueryDataSource"/>
	/// </summary>
	public abstract class SqlQueryDataSource : IQueryDataSource
	{
		private IServiceLocator _serviceLocator;

		private ITypeSchema _typeSchema;

		private IProviderLocator _providerLocator;

		private List<int> hashCode = new List<int>();

		private object _lockObject = new object();

		/// <summary>
		/// Instancia do <see cref="IServiceLocator"/> associada.
		/// </summary>
		protected IServiceLocator ServiceLocator
		{
			get
			{
				return _serviceLocator;
			}
		}

		/// <summary>
		/// Instancia do <see cref="ITypeSchema"/> associado.
		/// </summary>
		protected ITypeSchema TypeSchema
		{
			get
			{
				return TypeSchemaUpdate.TypeSchema;
			}
		}

		/// <summary>
		/// Localizador de provedor.
		/// </summary>
		protected IProviderLocator ProviderLocator
		{
			get
			{
				return _providerLocator;
			}
		}

		/// <summary>
		/// Identifica se a instancia está inicializada.
		/// 
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão   
		/// </summary>
		/// <param name="serviceLocator"><see cref="IServiceLocator"/> que será usado pela instancia.</param>
		/// <param name="typeSchema">Instancia do esquema do tipo associado.</param>
		/// <param name="providerLocator">Localizador de provedor.</param>
		public SqlQueryDataSource(IServiceLocator serviceLocator, ITypeSchema typeSchema, IProviderLocator providerLocator)
		{
			serviceLocator.Require("serviceLocator").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			providerLocator.Require("providerLocator").NotNull();
			_providerLocator = providerLocator;
			_serviceLocator = serviceLocator;
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Converte o nível de isolação do System.Transactions para o System.Data.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		public static System.Data.IsolationLevel Convert(System.Transactions.IsolationLevel isolationLevel)
		{
			switch(isolationLevel)
			{
			case System.Transactions.IsolationLevel.Chaos:
				return System.Data.IsolationLevel.Chaos;
			case System.Transactions.IsolationLevel.ReadCommitted:
				return System.Data.IsolationLevel.ReadCommitted;
			case System.Transactions.IsolationLevel.ReadUncommitted:
				return System.Data.IsolationLevel.ReadUncommitted;
			case System.Transactions.IsolationLevel.RepeatableRead:
				return System.Data.IsolationLevel.RepeatableRead;
			case System.Transactions.IsolationLevel.Serializable:
				return System.Data.IsolationLevel.Serializable;
			case System.Transactions.IsolationLevel.Snapshot:
				return System.Data.IsolationLevel.Snapshot;
			default:
				return System.Data.IsolationLevel.Unspecified;
			}
		}

		/// <summary>
		/// Retorna o resultado de uma query receve os dados de uma query e enviando ao SQL Server
		/// </summary>
		/// <param name="query">Informações da query</param>
		/// <returns>Retorna o resultado da query</returns>
		public IQueryResult Execute(QueryInfo query)
		{
			query.Require("query").NotNull();
			if(query.StoredProcedureName == null)
			{
				var parser = CreateParser(query);
				string queryString = parser.GetText();
				return ExecuteQuery(queryString, query);
			}
			else
			{
				return ExecuteStoredProcedure(query);
			}
		}

		/// <summary>
		/// Retorna o resultado de várias queries recebe os dados de uma query e enviando ao SQL Server
		/// </summary>
		/// <param name="queries">Informações das queries</param>
		/// <returns>Retorna os resultados da queries</returns>
		public virtual IEnumerable<IQueryResult> Execute(QueryInfo[] queries)
		{
			queries.Require("queries").NotNull();
			IQueryResult[] result = new IQueryResult[queries.Length];
			for(int i = 0; i < queries.Length; i++)
				result[i] = Execute(queries[i]);
			return result;
		}

		/// <summary>
		/// Cria um parser com base nos dados da consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected abstract SqlQueryParser CreateParser(QueryInfo query);

		/// <summary>
		/// Executa a consulta no sistema.
		/// </summary>
		/// <param name="queryString">Texto do comando</param>
		/// <param name="queryInfo">Informações da consulta</param>
		/// <returns>Resultado da consulta</returns>
		protected abstract IQueryResult ExecuteQuery(string queryString, QueryInfo queryInfo);

		/// <summary>
		/// Executa uma procedure no sistema.
		/// </summary>
		/// <param name="queryInfo">Informações da procedure.</param>
		/// <returns>Resultado da consulta.</returns>
		protected abstract IQueryResult ExecuteStoredProcedure(QueryInfo queryInfo);

		/// <summary>
		/// Registra as informações do usuário no banco de dados.
		/// </summary>
		/// <param name="transaction">Transação que será utilizada na operação.</param>
		protected abstract void RegisterUserInfo(IStoredProcedureTransaction transaction);
	}
}
