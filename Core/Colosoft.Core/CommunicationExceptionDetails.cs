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

namespace Colosoft.Net
{
	/// <summary>
	/// Armazena os detalhes da exception ocorrida.
	/// </summary>
	public class CommunicationExceptionDetails
	{
		/// <summary>
		/// Mensagem.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Pilha da rastreamento.
		/// </summary>
		public string StackTrace
		{
			get;
			set;
		}

		/// <summary>
		/// InnerException
		/// </summary>
		public CommunicationExceptionDetails InnerException
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CommunicationExceptionDetails()
		{
		}

		/// <summary>
		/// Cria uma instancia com base nos dados da Exception informada.
		/// </summary>
		/// <param name="exception"></param>
		public CommunicationExceptionDetails(Exception exception) : this(exception.Message, exception.StackTrace, exception.InnerException)
		{
		}

		/// <summary>
		/// Cria uma instancia com os dados iniciais.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="stackTrace"></param>
		/// <param name="innerException"></param>
		public CommunicationExceptionDetails(string message, string stackTrace, Exception innerException)
		{
			this.Message = message;
			this.StackTrace = stackTrace;
			if(innerException != null)
				this.InnerException = new CommunicationExceptionDetails(message, innerException.StackTrace, innerException.InnerException);
		}
	}
}
