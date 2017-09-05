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
	/// Interface das classes responsáveis por registrar o log.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Identifica se o debug está abilitado.
		/// </summary>
		bool IsDebugEnabled
		{
			get;
		}

		/// <summary>
		/// Identifica se o error está abilitado.
		/// </summary>
		bool IsErrorEnabled
		{
			get;
		}

		/// <summary>
		/// Identifica se o erro fatal está abilitado
		/// </summary>
		bool IsFatalEnabled
		{
			get;
		}

		/// <summary>
		/// Identifica se informações estão abilitadas
		/// </summary>
		bool IsInfoEnabled
		{
			get;
		}

		/// <summary>
		/// Identifica se o Warning está abilitado.
		/// </summary>
		bool IsWarnEnabled
		{
			get;
		}

		/// <summary>
		/// Registra uma messagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		void Error(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		void Error(IMessageFormattable module, IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de erro.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		void Error(IMessageFormattable message, Exception exception);

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		void Fatal(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem e erro fatal.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		/// <param name="exception">Erro corrido.</param>
		void Fatal(IMessageFormattable message, Exception exception);

		/// <summary>
		/// Registra uma mensagem de debug.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		void Debug(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de alerta.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		void Warn(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de informação.
		/// </summary>
		/// <param name="message">Mensagem que será regitrada.</param>
		void Info(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="message">Mensagem que será registrada.</param>
		void CriticalInfo(IMessageFormattable message);

		/// <summary>
		/// Registra uma mensagem de informação crítica.
		/// </summary>
		/// <param name="module">Modulo da mensagem que será registrada.</param>
		/// <param name="message">Mensagem que será registrada.</param>
		void CriticalInfo(IMessageFormattable module, IMessageFormattable message);

		/// <summary>
		/// Escreve uma nova entrada de log com uma categoria e prioridade especificada.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="category">Categoria da entrada.</param>
		/// <param name="priority">Prioridade da entrada.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		bool Write(IMessageFormattable message, Category category, Priority priority);

		/// <summary>
		/// Escreve uma nava entrada de log do tipo de Exception.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="exception">Instancia da exception ocorrida.</param>
		/// <param name="priority">Prioridade do log.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		bool Write(IMessageFormattable message, Exception exception, Priority priority);

		/// <summary>
		/// Define o nível do log.
		/// </summary>
		/// <param name="level">Descritivo do nível.</param>
		void SetLevel(string level);
	}
}
