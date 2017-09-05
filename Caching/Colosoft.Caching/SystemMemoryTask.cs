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
using Colosoft.Threading;
using System.Management;
using Colosoft.Logging;

namespace Colosoft.Caching.Statistics
{
	/// <summary>
	/// Implementação de uma tarefa que monitora a memória do sistema.
	/// </summary>
	public sealed class SystemMemoryTask : TimeScheduler.Task, IDisposable
	{
		/// <summary>
		/// Porcentagem da memória em uso.
		/// </summary>
		public static int PercentMemoryUsed
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="Logger"></param>
		public SystemMemoryTask(ILogger Logger)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Recupera o próximo intervalo de execução.
		/// </summary>
		/// <returns></returns>
		public long GetNextInterval()
		{
			return 0x7d0L;
		}

		/// <summary>
		/// Identifica se a tarefa foi cancelada.
		/// </summary>
		/// <returns></returns>
		public bool IsCancelled()
		{
			return false;
		}

		/// <summary>
		/// Executa a tarefa.
		/// </summary>
		public void Run()
		{
			try
			{
			}
			catch(Exception)
			{
			}
		}
	}
}
