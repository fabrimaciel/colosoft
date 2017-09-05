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

namespace Colosoft.Data
{
	/// <summary>
	/// Interface de gerenciamento de chaves primárias
	/// </summary>
	public interface IPrimaryKeyRepository
	{
		/// <summary>
		/// Nome do provedor.
		/// </summary>
		string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Retorna se o id é criado antes da execução do comando ou após a execução do comando     
		/// </summary>
		/// <param name="entityName">Nome da entidade</param>
		/// <returns>Retorna true se for após e false caso contrário</returns>
		bool IsPosCommand(string entityName);

		/// <summary>
		/// Retorna o valor do parâmetro correspondente a chave primária
		/// </summary>
		/// <param name="transaction">Transação.</param>
		/// <param name="entityName">Nome da entidade</param>
		/// <returns>Pâremetro correspondente a chave primária</returns>
		object GetPrimaryKey(IPersistenceTransactionExecuter transaction, string entityName);
	}
}
