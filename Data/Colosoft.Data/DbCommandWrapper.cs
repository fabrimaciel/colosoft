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
	/// Adapatador de um comando do banco de dados.
	/// </summary>
	class DbCommandWrapper : System.Data.IDbCommand
	{
		private System.Data.IDbCommand _command;

		private System.Data.IDbConnection _connection;

		/// <summary>
		/// Texto do comando.
		/// </summary>
		public string CommandText
		{
			get
			{
				return _command.CommandText;
			}
			set
			{
				_command.CommandText = value;
			}
		}

		/// <summary>
		/// Timeout do comando.
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return _command.CommandTimeout;
			}
			set
			{
				_command.CommandTimeout = value;
			}
		}

		/// <summary>
		/// Tipo do comando.
		/// </summary>
		public System.Data.CommandType CommandType
		{
			get
			{
				return _command.CommandType;
			}
			set
			{
				_command.CommandType = value;
			}
		}

		/// <summary>
		/// Conexão associada.
		/// </summary>
		public System.Data.IDbConnection Connection
		{
			get
			{
				return _connection ?? _command.Connection;
			}
			set
			{
				_connection = value;
				if(value is DbConnectionWrapper)
					_command.Connection = ((DbConnectionWrapper)value).Connection;
				_command.Connection = value;
			}
		}

		/// <summary>
		/// Parâmetros do comando.
		/// </summary>
		public System.Data.IDataParameterCollection Parameters
		{
			get
			{
				return _command.Parameters;
			}
		}

		/// <summary>
		/// Transação do comando.
		/// </summary>
		public System.Data.IDbTransaction Transaction
		{
			get
			{
				return _command.Transaction;
			}
			set
			{
				_command.Transaction = value;
			}
		}

		/// <summary>
		/// Linha de origem atualizada.
		/// </summary>
		public System.Data.UpdateRowSource UpdatedRowSource
		{
			get
			{
				return _command.UpdatedRowSource;
			}
			set
			{
				_command.UpdatedRowSource = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connection">Instancia da conexão.</param>
		/// <param name="command"></param>
		public DbCommandWrapper(System.Data.IDbConnection connection, System.Data.IDbCommand command)
		{
			_connection = connection;
			_command = command;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DbCommandWrapper()
		{
			Dispose(false);
		}

		/// <summary>
		/// Cancela o comando.
		/// </summary>
		public void Cancel()
		{
			_command.Cancel();
		}

		/// <summary>
		/// Cria o parametro para o comando.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDbDataParameter CreateParameter()
		{
			return _command.CreateParameter();
		}

		/// <summary>
		/// Executa o comando sem resultado de consulta.
		/// </summary>
		/// <returns></returns>
		public int ExecuteNonQuery()
		{
			return _command.ExecuteNonQuery();
		}

		/// <summary>
		/// Executa o comando para leitura dos dados.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader()
		{
			return _command.ExecuteReader();
		}

		/// <summary>
		/// Executa o comando para leitura dos dados.
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
		{
			return _command.ExecuteReader(behavior);
		}

		/// <summary>
		/// Realiza uma execução escalar do comando.
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar()
		{
			return _command.ExecuteScalar();
		}

		/// <summary>
		/// Prepara o comando.
		/// </summary>
		public void Prepare()
		{
			_command.Prepare();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_command.Dispose();
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
