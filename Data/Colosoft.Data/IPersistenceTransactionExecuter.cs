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
	/// Interface que representa um objeto de transação
	/// </summary>
	public interface IPersistenceTransactionExecuter : IDisposable
	{
		/// <summary>
		/// Evento acionado antes de executar o commit da transação.
		/// </summary>
		event EventHandler Committing;

		/// <summary>
		/// Evento acionado depois do commit for realizado.
		/// </summary>
		event EventHandler Commited;

		/// <summary>
		/// Evento acionado antes do rollback.
		/// </summary>
		event EventHandler Rollbacking;

		/// <summary>
		/// Evento acionado depois de ocorrer o rollback da transação.
		/// </summary>
		event EventHandler Rollbacked;

		/// <summary>
		/// Nome do provedor conexão. 
		/// </summary>
		string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Da commit na transação
		/// </summary>
		void Commit();

		/// <summary>
		/// Da rollback na transação
		/// </summary>
		void Rollback();
	}
}
