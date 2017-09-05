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

namespace Colosoft.Data.Database.Generic
{
	/// <summary>
	/// Implementação do executor da persistencia no MSSQL Server.
	/// </summary>
	public class MsSqlPersistenceExecuter : GenericPersistenceExecuter
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator"></param>
		/// <param name="typeSchema"></param>
		public MsSqlPersistenceExecuter(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Data.Schema.ITypeSchema typeSchema) : base(serviceLocator, typeSchema, new Query.Database.SqlQueryTranslator(typeSchema))
		{
		}

		/// <summary>
		/// Cria o parser para a açõa informada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="providerName">Nome do provider associado.</param>
		/// <returns></returns>
		protected override PersistenceSqlParser CreateParser(PersistenceAction action, string providerName)
		{
			return new DefaultPersistenceSqlParser(Translator, TypeSchema, new Query.Database.Generic.MsSql.MsSqlTakeParametersParser()) {
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
			return this.ServiceLocator.GetInstance<IMsSqlPrimaryKeyRepository>(providerName + "MsSqlPrimaryKeyRepository");
		}
	}
}
