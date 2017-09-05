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
using Colosoft.Data.Database.Generic;

namespace Colosoft.Data.Database.MySql
{
	/// <summary>
	/// Implementação do executor da persistencia no MSSQL Server.
	/// </summary>
	public class MySqlPersistenceExecuter : GenericPersistenceExecuter
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator"></param>
		/// <param name="typeSchema"></param>
		public MySqlPersistenceExecuter(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Data.Schema.ITypeSchema typeSchema) : base(serviceLocator, typeSchema, new Query.Database.SqlQueryTranslator(typeSchema))
		{
		}

		/// <summary>
		/// Configura a transação.
		/// </summary>
		/// <param name="transaction"></param>
		protected override void ConfigureTransaction(IPersistenceTransactionExecuter transaction)
		{
			base.ConfigureTransaction(transaction);
		}

		/// <summary>
		/// Cria o parser para a açõa informada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="providerName">Nome do provider associado.</param>
		/// <returns></returns>
		protected override PersistenceSqlParser CreateParser(PersistenceAction action, string providerName)
		{
			return new MySqlPersistenceSqlParser(Translator, TypeSchema) {
				Action = action,
				PrimaryKeyRepository = GetKeyRepository(providerName)
			};
		}

		/// <summary>
		/// Cria o repositório de chave para o contrato informado.
		/// </summary>
		/// <param name="providerName"></param>
		/// <returns></returns>
		protected override IPrimaryKeyRepository CreatePrimaryKeyRepository(string providerName)
		{
			return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IMySqlPrimaryKeyRepository>();
		}

		/// <summary>
		/// Cria a procedure.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected override GDA.GDAStoredProcedure CreateProcedure(PersistenceAction action)
		{
			var procedure = new GDA.GDAStoredProcedure((!string.IsNullOrEmpty(action.StoredProcedureName.Schema) ? string.Format("`{0}`.`{1}`", action.StoredProcedureName.Schema, action.StoredProcedureName.Name) : string.Format("`{0}`", action.StoredProcedureName.Name)));
			return procedure;
		}

		/// <summary>
		/// Cria uma parametro.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override GDA.GDAParameter CreateParameter(string name, object value)
		{
			if(value is DateTimeOffset)
				value = ((DateTimeOffset)value).DateTime;
			return base.CreateParameter(name, value);
		}
	}
}
