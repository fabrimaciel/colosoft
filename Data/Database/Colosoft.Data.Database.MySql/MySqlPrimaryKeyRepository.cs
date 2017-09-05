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

namespace Colosoft.Data.Database.MySql
{
	/// <summary>
	/// Implementação do repositório das chaves primarias.
	/// </summary>
	public class MySqlPrimaryKeyRepository : IMySqlPrimaryKeyRepository
	{
		/// <summary>
		/// Recupera uma chave para a entidade informada.
		/// </summary>
		/// <param name="transaction">Transação.</param>
		/// <param name="entityName"></param>
		/// <returns></returns>
		public object GetPrimaryKey(IPersistenceTransactionExecuter transaction, string entityName)
		{
			var trans = (Colosoft.Data.Database.Generic.PersistenceTransactionExecuter)transaction;
			return new GDA.DataAccess().ExecuteScalar(trans.Transaction, "SELECT LAST_INSERT_ID()");
		}

		/// <summary>
		/// Define se a chave será recuperada por um pos comando.
		/// </summary>
		/// <param name="entityName"></param>
		/// <returns></returns>
		public bool IsPosCommand(string entityName)
		{
			return true;
		}

		/// <summary>
		/// Nome do provedor associado.
		/// </summary>
		public string ProviderName
		{
			get;
			set;
		}
	}
}
