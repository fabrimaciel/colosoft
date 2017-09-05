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

namespace Colosoft.Query.Database.Generic.MsSql
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
		public MsSqlDataSource(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Data.Schema.ITypeSchema typeSchema, IProviderLocator providerLocator) : base(serviceLocator, typeSchema, new SqlQueryTranslator(typeSchema), providerLocator)
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
	}
}
