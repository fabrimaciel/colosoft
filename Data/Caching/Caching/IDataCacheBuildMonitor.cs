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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Assinatura do observer do monitor de build do cache.
	/// </summary>
	public interface IDataCacheBuildMonitorObserver : Colosoft.Progress.IProgressWithMessageObserver
	{
		/// <summary>
		/// Evento acioando quando a escuta da execução do build for terminada.
		/// </summary>
		/// <param name="buildExecutionUid">Identifica dor da execução do build.</param>
		void OnListenTerminate(Guid buildExecutionUid);

		/// <summary>
		/// Evento acionado quando ocorre um erro de comunicação.
		/// </summary>
		/// <param name="buildExecutionUid"></param>
		/// <param name="exception"></param>
		void OnCheckError(Guid buildExecutionUid, Exception exception);
	}
	/// <summary>
	/// Assinatura da classe que é utilizada como monitor
	/// para o build do cache.
	/// </summary>
	public interface IDataCacheBuildMonitor : IDisposable
	{
		/// <summary>
		/// Identifica se o monitor está executando.
		/// </summary>
		bool IsRunning
		{
			get;
		}

		/// <summary>
		/// Registra para escutar a execução do build.
		/// </summary>
		/// <param name="buildExecutionUid">Identificador da execução.</param>
		/// <param name="timeSpan">Tempo de vida da escuta.</param>
		void Listen(Guid buildExecutionUid, TimeSpan timeSpan);

		/// <summary>
		/// Aborta a escuta da execução do build.
		/// </summary>
		/// <param name="buildExecutionUid"></param>
		void AbortListen(Guid buildExecutionUid);

		/// <summary>
		/// Inicia o monitor.
		/// </summary>
		void Start();

		/// <summary>
		/// Para o monitor.
		/// </summary>
		void Stop();

		/// <summary>
		/// Adiciona o observer para o monitor.
		/// </summary>
		/// <param name="observer">Instancia que será adicionada.</param>
		void Add(IDataCacheBuildMonitorObserver observer);

		/// <summary>
		/// Remove o observer do monitor.
		/// </summary>
		/// <param name="observer">Instancia que será removida.</param>
		void Remove(IDataCacheBuildMonitorObserver observer);
	}
}
