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

namespace Colosoft.Threading
{
	/// <summary>
	/// Representa uma operação do expedidor.
	/// </summary>
	public interface IDispatcherOperation
	{
		/// <summary>
		/// Evento acionado quando a operação for abortada.
		/// </summary>
		event EventHandler Aborted;

		/// <summary>
		/// Evento acionado quando a operação for completada.
		/// </summary>
		event EventHandler Completed;

		/// <summary>
		/// Aborta a execução.
		/// </summary>
		/// <returns></returns>
		bool Abort();

		/// <summary>
		/// Aguarda a operação completar.
		/// </summary>
		/// <returns></returns>
		DispatcherOperationStatus Wait();

		/// <summary>
		/// Aguarda a operação completar em um tempo determinado.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		DispatcherOperationStatus Wait(TimeSpan timeout);

		/// <summary>
		/// Instancia do expedidor onde a operação foi postada.
		/// </summary>
		IDispatcher Dispatcher
		{
			get;
		}

		/// <summary>
		/// Prioridade da operação.
		/// </summary>
		DispatcherPriority Priority
		{
			get;
			set;
		}

		/// <summary>
		/// Resultado da operação depois de completada.
		/// </summary>
		object Result
		{
			get;
		}

		/// <summary>
		/// Situação atual da operação.
		/// </summary>
		DispatcherOperationStatus Status
		{
			get;
		}
	}
}
