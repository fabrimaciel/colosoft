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

namespace Colosoft.Logging
{
	/// <summary>
	/// Implementação de um logger agregado.
	/// </summary>
	public class AggregateLogger : ILogger
	{
		private List<ILogger> _loggers = new List<ILogger>();

		/// <summary>
		/// Loggers agregados.
		/// </summary>
		public List<ILogger> Loggers
		{
			get
			{
				return _loggers;
			}
		}

		/// <summary>
		/// Identifica se o debug está abilitado.
		/// </summary>
		public bool IsDebugEnabled
		{
			get
			{
				return _loggers.Any(f => f.IsDebugEnabled);
			}
		}

		/// <summary>
		/// Identifica se o error está abilitado.
		/// </summary>
		public bool IsErrorEnabled
		{
			get
			{
				return _loggers.Any(f => f.IsErrorEnabled);
			}
		}

		/// <summary>
		/// Identifica se o erro fatal está abilitado
		/// </summary>
		public bool IsFatalEnabled
		{
			get
			{
				return _loggers.Any(f => f.IsFatalEnabled);
			}
		}

		/// <summary>
		/// Identifica se informações estão abilitadas
		/// </summary>
		public bool IsInfoEnabled
		{
			get
			{
				return _loggers.Any(f => f.IsInfoEnabled);
			}
		}

		/// <summary>
		/// Identifica se o Warning está abilitado.
		/// </summary>
		public bool IsWarnEnabled
		{
			get
			{
				return _loggers.Any(f => f.IsWarnEnabled);
			}
		}

		/// <summary>
		/// Registra uma messagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Error(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Error(message);
		}

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Error(IMessageFormattable module, IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Error(module, message);
		}

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		public void Error(IMessageFormattable message, Exception exception)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Error(message, exception);
		}

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Fatal(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Fatal(message);
		}

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		public void Fatal(IMessageFormattable message, Exception exception)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Fatal(message, exception);
		}

		/// <summary>
		/// Registra uma mensagem de debug.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Debug(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Debug(message);
		}

		/// <summary>
		/// Registra uma mensagem de alerta.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void Warn(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Warn(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação.
		/// </summary>
		/// <param name="message">Mensagem que será regitrada.</param>
		public void Info(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.Info(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public void CriticalInfo(IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.CriticalInfo(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public void CriticalInfo(IMessageFormattable module, IMessageFormattable message)
		{
			foreach (var logger in _loggers.ToArray())
				logger.CriticalInfo(module, message);
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
			var result = false;
			foreach (var logger in _loggers.ToArray())
				result = logger.Write(message, category, priority) || result;
			return result;
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
			var result = false;
			foreach (var logger in _loggers.ToArray())
				result = logger.Write(message, exception, priority) || result;
			return result;
		}

		/// <summary>
		/// Define o nível do log.
		/// </summary>
		/// <param name="level">Descritivo do nível.</param>
		public void SetLevel(string level)
		{
			foreach (var logger in _loggers.ToArray())
				logger.SetLevel(level);
		}
	}
}
