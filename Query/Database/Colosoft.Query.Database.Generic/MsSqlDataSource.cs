﻿/* 
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

namespace Colosoft.Query.Database.Generic
{
	/// <summary>
	/// Implementação da origem de dados do MSSql server.
	/// </summary>
	public class MsSqlDataSource : GenericSqlQueryDataSource
	{
		/// <summary>
		/// Identifica se é para ignorar o último campo quando se trabalha com paginação.
		/// </summary>
		protected override bool IgnoreLastFieldWithPaging
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator"></param>
		/// <param name="typeSchema"></param>
		/// <param name="providerLocator"></param>
		public MsSqlDataSource(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Colosoft.Data.Schema.ITypeSchema typeSchema, IProviderLocator providerLocator) : base(serviceLocator, typeSchema, new SqlQueryTranslator(typeSchema), providerLocator)
		{
		}

		/// <summary>
		/// Cria o parser para ser utilizado.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected override SqlQueryParser CreateParser(QueryInfo query)
		{
			return new DefaultSqlQueryParser(Translator, TypeSchema, new MsSql.MsSqlTakeParametersParser()) {
				Query = query
			};
		}

		/// <summary>
		/// Cria uma sessão de persistencia para o grupo de execução.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		protected override GDA.GDASession CreateSession(QueryExecutionGroup group)
		{
			var providerConfiguration = GDA.GDASettings.GetProviderConfiguration(group.ProviderName);
			var session = new GDA.GDASession(providerConfiguration);
			var isolationLevel = group.IsolationLevel;
			if(isolationLevel == System.Transactions.IsolationLevel.Unspecified)
				isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
			string command = null;
			switch(isolationLevel)
			{
			case System.Transactions.IsolationLevel.ReadUncommitted:
				command = "READ UNCOMMITTED";
				break;
			case System.Transactions.IsolationLevel.ReadCommitted:
				command = "READ COMMITTED";
				break;
			case System.Transactions.IsolationLevel.RepeatableRead:
				command = "REPEATABLE READ";
				break;
			case System.Transactions.IsolationLevel.Serializable:
				command = "SERIALIZABLE";
				break;
			case System.Transactions.IsolationLevel.Snapshot:
				command = "SNAPSHOT";
				break;
			}
			if(!string.IsNullOrEmpty(command))
			{
				command = "SET TRANSACTION ISOLATION LEVEL " + command;
				new GDA.DataAccess().ExecuteCommand(session, System.Data.CommandType.Text, 30, command);
			}
			return session;
		}
	}
}
