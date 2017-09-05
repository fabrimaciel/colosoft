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

namespace Colosoft.Query
{
	/// <summary>
	/// Possíveis tipos de comando de uma consulta;
	/// </summary>
	public enum QueryCommandType
	{
		/// <summary>
		/// Comando de texto.
		/// </summary>
		Text,
		/// <summary>
		/// Chamada de uma StoredProcedure
		/// </summary>
		StoredProcedure,
	}
	/// <summary>
	/// Armazena os dados od comando da consulta.
	/// </summary>
	public class QueryCommand
	{
		private string _commandText;

		private QueryCommandType _commandType;

		private int _timeout;

		/// <summary>
		/// Texto do comando.
		/// </summary>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
		}

		/// <summary>
		/// Tipo de comando.
		/// </summary>
		public QueryCommandType CommandType
		{
			get
			{
				return _commandType;
			}
		}

		/// <summary>
		/// Tempo limite para a execução do comando.
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandType"></param>
		/// <param name="timeout"></param>
		public QueryCommand(string commandText, QueryCommandType commandType, int timeout)
		{
			_commandText = commandText;
			_commandType = commandType;
			_timeout = timeout;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("CommandType: {0}; Timeout: {1}; CommandText: {2}", CommandType, Timeout, CommandText);
		}
	}
	/// <summary>
	/// Assinatura das classe que contém um comando de consulta.
	/// </summary>
	public interface IQueryCommandContainer
	{
		/// <summary>
		/// Instancia do comando associado.
		/// </summary>
		QueryCommand Command
		{
			get;
			set;
		}
	}
}
