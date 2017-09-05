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
using System.Threading.Tasks;

namespace Colosoft.Threading
{
	/// <summary>
	/// Define a interface de uma Task.
	/// </summary>
	/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
	/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
	public interface ITask<TResult, TState> : ITask
	{
		/// <summary>
		/// Obtém o estado fornecido a Task quando foi criada ou null se não foi fornecida.
		/// </summary>
		new TState AsyncState
		{
			get;
		}

		/// <summary>
		/// Obtém a configuração da Task.
		/// </summary>
		Task.Config<TResult, TState> Config
		{
			get;
		}

		/// <summary>
		/// Obtém o valor do resultado desta Task.
		/// </summary>
		TResult Result
		{
			get;
		}
	}
	/// <summary>
	/// Define a interface de uma Task.
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Obtém o estado fornecido a Task quando foi criada ou null se não foi fornecida.
		/// </summary>
		object AsyncState
		{
			get;
		}

		/// <summary>
		/// Cancela ou aborta esta Task.
		/// </summary>
		void CancelOrAbort();

		/// <summary>
		/// Lança uma <see cref="System.OperationCanceledException" /> se esta Task teve o cancelamento solicitado.
		/// </summary>
		void ThrowIfCancellationRequested();

		/// <summary>
		/// Aguarda a Task concluir a execução.
		/// </summary>
		/// <param name="millisecondsTimeout">O número de milissegundos de espera, ou Infinite (-1) para esperar indefinidamente.</param>
		/// <returns>Verdadeiro se a Task é completada no tempo alocado; Caso contrário, falso.</returns>
		bool Wait(int millisecondsTimeout = -1);

		/// <summary>
		/// Aguarda a Task concluir a execução.
		/// </summary>
		/// <param name="timeout">O número de milissegundos de espera, ou Infinite (-1) para esperar indefinidamente.</param>
		/// <returns>Verdadeiro se a Task é completada no tempo alocado; Caso contrário, falso.</returns>
		bool Wait(TimeSpan timeout);

		/// <summary>
		/// Obtém a <see cref="System.AggregateException" /> que causou a Task o encerramento prematuro.
		/// Se a Task foi concluída com êxito ou ainda não foi lançada quaisquer exceção, retornará null.
		/// </summary>
		AggregateException Exception
		{
			get;
		}

		/// <summary>
		/// Obtém uma identificação única para essa instância de Task.
		/// </summary>
		int Id
		{
			get;
		}

		/// <summary>
		/// Obtém se esta Task foi abortada.
		/// </summary>
		bool IsAborted
		{
			get;
		}

		/// <summary>
		/// Obtém se esta Task foi cancelada.
		/// </summary>
		bool IsCanceled
		{
			get;
		}

		/// <summary>
		/// Obtém se esta Task foi cancelada ou abortada.
		/// </summary>
		bool IsCanceledOrAborted
		{
			get;
		}

		/// <summary>
		/// Obtém se esta Task foi concluída.
		/// </summary>
		bool IsCompleted
		{
			get;
		}

		/// <summary>
		/// Obtém se a Task concluída devido a uma exceção não tratada.
		/// </summary>
		bool IsFaulted
		{
			get;
		}

		/// <summary>
		/// Obtém o status desta Task.
		/// </summary>
		TaskStatus Status
		{
			get;
		}
	}
}
