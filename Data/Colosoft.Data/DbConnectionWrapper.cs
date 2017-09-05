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

namespace Colosoft.Data.Wrappers
{
	/// <summary>
	/// Adaptador de uma conexão com o banco de dados.
	/// </summary>
	class DbConnectionWrapper : System.Data.IDbConnection
	{
		private System.Data.IDbConnection _connection;

		/// <summary>
		/// Instancia da conexãod adaptada.
		/// </summary>
		internal System.Data.IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// String da conexão.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return _connection.ConnectionString;
			}
			set
			{
				_connection.ConnectionString = value;
			}
		}

		/// <summary>
		/// Timeout da conexão.
		/// </summary>
		public int ConnectionTimeout
		{
			get
			{
				return _connection.ConnectionTimeout;
			}
		}

		/// <summary>
		/// Nome do banco de dados.
		/// </summary>
		public string Database
		{
			get
			{
				return _connection.Database;
			}
		}

		/// <summary>
		/// Estado da conexão.
		/// </summary>
		public System.Data.ConnectionState State
		{
			get
			{
				return _connection.State;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connection"></param>
		public DbConnectionWrapper(System.Data.IDbConnection connection)
		{
			_connection = connection;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DbConnectionWrapper()
		{
			Dispose(false);
		}

		/// <summary>
		/// Inicia a transação.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDbTransaction BeginTransaction()
		{
			return new DbTransactionWrapper(this, _connection.BeginTransaction());
		}

		/// <summary>
		/// Inicia a transação.
		/// </summary>
		/// <param name="il"></param>
		/// <returns></returns>
		public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
		{
			return new DbTransactionWrapper(this, _connection.BeginTransaction(il));
		}

		/// <summary>
		/// Altera o banco de dados da conexão.
		/// </summary>
		/// <param name="databaseName"></param>
		public void ChangeDatabase(string databaseName)
		{
			_connection.ChangeDatabase(databaseName);
		}

		/// <summary>
		/// Fecha a conexão.
		/// </summary>
		public void Close()
		{
			_connection.Close();
		}

		/// <summary>
		/// Cria ao comando da conexão.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDbCommand CreateCommand()
		{
			return new DbCommandWrapper(this, _connection.CreateCommand());
		}

		/// <summary>
		/// Abre a conexão.
		/// </summary>
		public void Open()
		{
			_connection.Open();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_connection.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
