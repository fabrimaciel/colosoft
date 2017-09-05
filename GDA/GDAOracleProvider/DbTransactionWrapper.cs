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

namespace GDA.Provider.Oracle
{
	/// <summary>
	/// Adaptador da transação do banco de dados.
	/// </summary>
	class DbTransactionWrapper : System.Data.IDbTransaction
	{
		private System.Data.IDbConnection _connection;

		private System.Data.IDbTransaction _transaction;

		private static System.Reflection.FieldInfo _completedField;

		/// <summary>
		/// Conexão associada.
		/// </summary>
		public System.Data.IDbConnection Connection
		{
			get
			{
				return _connection ?? _transaction.Connection;
			}
		}

		/// <summary>
		/// Nível de isolamento.
		/// </summary>
		public System.Data.IsolationLevel IsolationLevel
		{
			get
			{
				return _transaction.IsolationLevel;
			}
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static DbTransactionWrapper()
		{
			_completedField = typeof(global::Oracle.ManagedDataAccess.Client.OracleTransaction).GetField("m_completed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		public DbTransactionWrapper(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
		{
			_connection = connection;
			_transaction = transaction;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DbTransactionWrapper()
		{
			Dispose(false);
		}

		/// <summary>
		/// Realiza o commit.
		/// </summary>
		public void Commit()
		{
			_transaction.Commit();
		}

		/// <summary>
		/// Desfaz.
		/// </summary>
		public void Rollback()
		{
			_transaction.Rollback();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_completedField != null)
				_completedField.SetValue(_transaction, true);
			_transaction.Dispose();
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
