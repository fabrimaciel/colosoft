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
using System.Globalization;

namespace Colosoft.Logging
{
	/// <summary>
	/// Implementação do logger para trace.
	/// </summary>
	public class TraceLogger : LoggerBase
	{
		/// <summary>
		/// Escreve uma nova entrada de log com uma categoria e prioridade especificada.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="category">Categoria da entrada.</param>
		/// <param name="priority">Prioridade da entrada.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public override bool Write(IMessageFormattable message, Category category, Priority priority)
		{
			string messageToLog = String.Format(CultureInfo.InvariantCulture, Colosoft.Properties.Resources.DefaultTextLoggerPattern, DateTime.Now, category.ToString().ToUpper(CultureInfo.InvariantCulture), message.Format(CultureInfo.CurrentCulture), priority.ToString());
			try
			{
				System.Diagnostics.Trace.WriteLine(messageToLog);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Escreve uma nava entrada de log do tipo de Exception.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="exception">Instancia da exception ocorrida.</param>
		/// <param name="priority">Prioridade do log.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public override bool Write(IMessageFormattable message, Exception exception, Priority priority)
		{
			if(exception != null)
			{
				var exceptionMessage = exception.Message;
				var stackTrace = exception.StackTrace;
				var exceptionText = new StringBuilder();
				while (exception != null)
				{
					exceptionText.AppendFormat("{0} : {1}\r\n", exception.GetType().Name, exception.Message);
					exception = exception.InnerException;
				}
				message = string.Format("Message: {0}\r\nException: {1} : {2}", message != null ? message.Format(CultureInfo.InvariantCulture) : exceptionMessage, exceptionText.ToString(), stackTrace).GetFormatter();
			}
			return Write(message, Category.Exception, priority);
		}
	}
}
