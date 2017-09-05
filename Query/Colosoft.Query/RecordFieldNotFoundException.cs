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
	/// Representa o erro acionado quando um campo
	/// </summary>
	[Serializable]
	public class RecordFieldNotFoundException : QueryException
	{
		/// <summary>
		/// Cria a instancia com as informações do campo não encontrado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="descriptor"></param>
		/// <param name="queryResult">Resultado da consulta associada.</param>
		public RecordFieldNotFoundException(string fieldName, Record.RecordDescriptor descriptor, IQueryResult queryResult) : base(FormatMessage(fieldName, descriptor, queryResult))
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message"></param>
		public RecordFieldNotFoundException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public RecordFieldNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected RecordFieldNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Formata a mensagem do erro.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="descriptor"></param>
		/// <param name="queryResult">Resulta da consulta associada.</param>
		/// <returns></returns>
		private static string FormatMessage(string fieldName, Record.RecordDescriptor descriptor, IQueryResult queryResult)
		{
			QueryCommand command = null;
			if(queryResult is IQueryCommandContainer)
				command = ((IQueryCommandContainer)queryResult).Command;
			var fieldsAvailables = string.Join("; ", descriptor.Select(f => f.Name).ToArray());
			if(command != null)
				return ResourceMessageFormatter.Create(() => Properties.Resources.RecordFieldNotFoundExceptionWithQueryCommand_Message, fieldName, fieldsAvailables, command.ToString()).Format();
			else
				return ResourceMessageFormatter.Create(() => Properties.Resources.RecordFieldNotFoundException_Message, fieldName, fieldsAvailables).Format();
		}
	}
}
