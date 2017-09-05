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
	/// Implementação da <see cref="ILogger"/> que salva o log dentro de um <see cref="System.IO.TextWriter"/>.
	/// </summary>
	public class TextLogger : ILogger, IDisposable
	{
		private readonly System.IO.TextWriter writer;

		/// <summary>
		/// Identifica se o debug está abilitado.
		/// </summary>
		public bool IsDebugEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se o error está abilitado.
		/// </summary>
		public bool IsErrorEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se o erro fatal está abilitado
		/// </summary>
		public bool IsFatalEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se informações estão abilitadas
		/// </summary>
		public bool IsInfoEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se o Warning está abilitado.
		/// </summary>
		public bool IsWarnEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Inicializa uma nova instancia do <see cref="TextLogger"/> que escreve o log para a saída do console.
		/// </summary>
		public TextLogger() : this(Console.Out)
		{
		}

		/// <summary>
		/// Inicializa uma nova instancia do <see cref="TextLogger"/>.
		/// </summary>
		/// <param name="writer">Escritor onde o log será salvo.</param>
		public TextLogger(System.IO.TextWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
		}

		/// <summary>
		/// Registra uma message de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Error(IMessageFormattable message)
		{
			Write(message, Category.Exception, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Error(IMessageFormattable module, IMessageFormattable message)
		{
			Write(message, Category.Exception, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		public void Error(IMessageFormattable message, Exception exception)
		{
			Write(message, exception, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Fatal(IMessageFormattable message)
		{
			Write(message, Category.Exception, Priority.High);
		}

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		public void Fatal(IMessageFormattable message, Exception exception)
		{
			Write(message, exception, Priority.High);
		}

		/// <summary>
		/// Registra uma mensagem de debug.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Debug(IMessageFormattable message)
		{
			Write(message, Category.Debug, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de alerta.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Warn(IMessageFormattable message)
		{
			Write(message, Category.Warn, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de informação.
		/// </summary>
		/// <param name="message"></param>
		public void Info(IMessageFormattable message)
		{
			Write(message, Category.Info, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void CriticalInfo(IMessageFormattable message)
		{
			Write(message, Category.Info, Priority.None);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public void CriticalInfo(IMessageFormattable module, IMessageFormattable message)
		{
			Write(message, Category.Info, Priority.None);
		}

		/// <summary>
		/// Define o nível do log.
		/// </summary>
		/// <param name="level">Descritivo do nível.</param>
		public void SetLevel(string level)
		{
		}

		/// <summary>
		/// Escreve uma nova entrada de log com uma categoria e prioridade especificada.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="category">Categoria da entrada.</param>
		/// <param name="priority">Prioridade da entrada.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public bool Write(IMessageFormattable message, Category category, Priority priority)
		{
			string messageToLog = String.Format(CultureInfo.InvariantCulture, Colosoft.Properties.Resources.DefaultTextLoggerPattern, DateTime.Now, category.ToString().ToUpper(CultureInfo.InvariantCulture), message.Format(CultureInfo.CurrentCulture), priority.ToString());
			try
			{
				writer.WriteLine(messageToLog);
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
		public bool Write(IMessageFormattable message, Exception exception, Priority priority)
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

		/// <summary>
		/// Libera o <see cref="System.IO.TextWriter"/> associado.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(writer != null)
				{
					writer.Dispose();
				}
			}
		}

		///<summary>
		/// Libera a instancia.
		///</summary>
		/// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
