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
using System.Threading.Tasks;

namespace Colosoft
{
	/// <summary>
	/// Classe para geração de Log
	/// </summary>
	public static class Log
	{
		private static Logging.ILogger _logger;

		private static object _lock = new object();

		/// <summary>
		/// Instancia da classe responsável pelo log.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
		public static Logging.ILogger Logger
		{
			get
			{
				if(_logger == null)
					lock (_lock)
					{
						if(_logger != null)
							return _logger;
						try
						{
							_logger = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Logging.ILogger>();
						}
						catch(NullReferenceException)
						{
						}
						catch(Microsoft.Practices.ServiceLocation.ActivationException)
						{
						}
						if(_logger == null)
							_logger = new Logging.TextLogger();
					}
				return _logger;
			}
		}

		/// <summary>
		/// Define o logger padrão.
		/// </summary>
		/// <param name="logger"></param>
		public static void SetLogger(Logging.ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Método para a geração de Log
		/// </summary>
		/// <param name="message">String que será logada</param>
		/// <returns>verdadeiro se conseguiu salvar corretamente o log</returns>
		public static bool Write(IMessageFormattable message)
		{
			return Logger.Write(message, Logging.Category.Info, Logging.Priority.None);
		}

		/// <summary>
		/// Escreve uma nova entrada de log com uma categoria e prioridade especificada.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="category">Categoria da entrada.</param>
		/// <param name="priority">Prioridade da entrada.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public static bool Write(IMessageFormattable message, Logging.Category category, Logging.Priority priority)
		{
			return Logger.Write(message, category, priority);
		}

		/// <summary>
		/// Escreve uma nava entrada de log do tipo de Exception.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="exception">Instancia da exception ocorrida.</param>
		/// <param name="priority">Prioridade do log.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public static bool Write(IMessageFormattable message, Exception exception, Logging.Priority priority)
		{
			return Logger.Write(message, exception, priority);
		}
	}
}
