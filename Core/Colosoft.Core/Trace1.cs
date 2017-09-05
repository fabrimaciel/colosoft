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
	/// Implementação da classe de trace.
	/// </summary>
	public class Trace
	{
		private static ILogger _logger;

		/// <summary>
		/// Logger que será usado pelo trace.
		/// </summary>
		public static ILogger Logger
		{
			get
			{
				if(_logger == null)
					_logger = new TraceLogger();
				return _logger;
			}
			set
			{
				_logger = value;
			}
		}

		/// <summary>
		/// Registra uma messagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void Error(IMessageFormattable message)
		{
			Logger.Error(message);
		}

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void Error(IMessageFormattable module, IMessageFormattable message)
		{
			Logger.Error(module, message);
		}

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void Fatal(IMessageFormattable message)
		{
			Logger.Fatal(message);
		}

		/// <summary>
		/// Registra uma mensagem de debug.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void Debug(IMessageFormattable message)
		{
			Logger.Debug(message);
		}

		/// <summary>
		/// Registra uma mensagem de alerta.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void Warn(IMessageFormattable message)
		{
			Logger.Warn(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação.
		/// </summary>
		/// <param name="message">Mensagem que será regitrada.</param>
		public static void Info(IMessageFormattable message)
		{
			Logger.Info(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void CriticalInfo(IMessageFormattable message)
		{
			Logger.CriticalInfo(message);
		}

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		public static void CriticalInfo(IMessageFormattable module, IMessageFormattable message)
		{
			Logger.CriticalInfo(module, message);
		}
	}
}
