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

namespace Colosoft.Query.Database
{
	/// <summary>
	/// Implementação de um enumerador para DataReader.
	/// </summary>
	public sealed class DataReaderEnumerator : IEnumerator<System.Data.IDataRecord>
	{
		private System.Data.IDataReader _dataReader;

		private System.Data.IDbCommand _command;

		/// <summary>
		/// Instancia atual.
		/// </summary>
		public System.Data.IDataRecord Current
		{
			get
			{
				return _dataReader;
			}
		}

		/// <summary>
		/// Instancia atual.
		/// </summary>
		object System.Collections.IEnumerator.Current
		{
			get
			{
				return _dataReader;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="dataReader"></param>
		public DataReaderEnumerator(System.Data.IDbCommand command, System.Data.IDataReader dataReader)
		{
			command.Require("command").NotNull();
			dataReader.Require("dataReader").NotNull();
			_command = command;
			_dataReader = dataReader;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DataReaderEnumerator()
		{
			Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_dataReader != null)
			{
				_dataReader.Close();
				_dataReader.Dispose();
				_dataReader = null;
			}
			if(_command != null)
			{
				_command.Dispose();
				_command = null;
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Tenta mover para o próximo registro
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			if(!(_dataReader != null && _dataReader.Read()))
			{
				Dispose();
				return false;
			}
			return true;
		}

		/// <summary>
		/// Reseta.
		/// </summary>
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}
