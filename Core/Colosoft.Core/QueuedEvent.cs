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
using System.Diagnostics;

namespace Colosoft.Threading
{
	/// <summary>
	/// Representa o evento enfilerado.
	/// </summary>
	internal class QueuedEvent
	{
		public Stopwatch stopwatch;

		public TimeScheduler.Task Task;

		/// <summary>
		/// Tempo restante.
		/// </summary>
		public long ElapsedTime
		{
			get
			{
				return this.stopwatch.ElapsedMilliseconds;
			}
		}

		/// <summary>
		/// Intervalo de execução.
		/// </summary>
		public long Interval
		{
			get
			{
				return this.Task.GetNextInterval();
			}
		}

		/// <summary>
		/// Horário programado.
		/// </summary>
		public DateTime SchedTime
		{
			get
			{
				return DateTime.Now.AddMilliseconds((double)(this.Interval - this.ElapsedTime));
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="task">Tarefa que associada.</param>
		public QueuedEvent(TimeScheduler.Task task)
		{
			this.Task = task;
			this.stopwatch = new Stopwatch();
			this.stopwatch.Start();
		}

		/// <summary>
		/// Reinfilera o evento.
		/// </summary>
		/// <returns></returns>
		public virtual bool ReQueue()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			return !this.Task.IsCancelled();
		}
	}
}
