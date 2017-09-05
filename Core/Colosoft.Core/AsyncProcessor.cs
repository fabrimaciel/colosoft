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
using System.Threading;

namespace Colosoft.Threading
{
	/// <summary>
	/// Representa um processador de tarefas assincronas.
	/// </summary>
	public class AsyncProcessor : IAsyncProcessor
	{
		private Queue<IAsyncTask> _eventsHi;

		private Queue<IAsyncTask> _eventsLow;

		private Thread _worker;

		private Logging.ILogger _logger;

		private string _name;

		/// <summary>
		/// Logger associado.
		/// </summary>
		public Logging.ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do processador.</param>
		/// <param name="logger">Logger associado com a instancia.</param>
		public AsyncProcessor(string name, Logging.ILogger logger)
		{
			name.Require("name").NotNull().NotEmpty();
			_name = name;
			_worker = null;
			_eventsHi = new Queue<IAsyncTask>(256);
			_eventsLow = new Queue<IAsyncTask>(256);
			_logger = logger;
		}

		/// <summary>
		/// Adiciona uma nova tarefa para ser realizada.
		/// </summary>
		/// <param name="evnt">Instancia da tarefa.</param>
		public void Enqueue(IAsyncTask evnt)
		{
			lock (this)
			{
				_eventsHi.Enqueue(evnt);
				Monitor.Pulse(this);
			}
		}

		/// <summary>
		/// Adiciona uma nova tarefa com baixa prioridade.
		/// </summary>
		/// <param name="evnt">Instancia da tarefa.</param>
		public void EnqueueLowPriority(IAsyncTask evnt)
		{
			lock (this)
			{
				_eventsLow.Enqueue(evnt);
				Monitor.Pulse(this);
			}
		}

		/// <summary>
		/// Método acionado pelo robo do processador.
		/// </summary>
		protected void Run()
		{
			while (_worker != null)
			{
				IAsyncTask task = null;
				try
				{
					lock (this)
					{
						if((_eventsHi.Count < 1) && (_eventsLow.Count < 1))
							Monitor.Wait(this);
						if(_eventsHi.Count > 0)
							task = _eventsHi.Dequeue();
						else if(_eventsLow.Count > 0)
							task = _eventsLow.Dequeue();
					}
					if(task != null)
						task.Process();
					continue;
				}
				catch(ThreadAbortException)
				{
					break;
				}
				catch(NullReferenceException)
				{
					continue;
				}
				catch(Exception)
				{
					continue;
				}
			}
		}

		/// <summary>
		/// Inicializa o processador.
		/// </summary>
		public void Start()
		{
			lock (this)
			{
				if(_worker == null)
				{
					_worker = new Thread(new ThreadStart(this.Run));
					_worker.IsBackground = true;
					_worker.Name = "AsyncProcessor";
					_worker.Start();
				}
			}
		}

		/// <summary>
		/// Para o processador.
		/// </summary>
		public void Stop()
		{
			lock (this)
			{
				if(_worker != null && _worker.IsAlive)
				{
					_worker.Abort();
					_worker = null;
				}
			}
		}
	}
}
