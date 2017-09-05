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
	/// Implementa um horário programa de execução.
	/// </summary>
	public class TimeScheduler : IDisposable
	{
		/// <summary>
		/// Possíveis estados.
		/// </summary>
		private enum State
		{
			RUN,
			SUSPEND,
			STOPPING,
			STOP,
			DISPOSED
		}

		private EventQueue _queue;

		private long _suspendInterval;

		private Thread _thread;

		private State _threadState;

		/// <summary>
		/// Construtor padrão. Inicializa com intervalo de 2000 miliseguntos.
		/// </summary>
		public TimeScheduler() : this(2000)
		{
		}

		/// <summary>
		/// Cria a instancia definindo o intervalo para supender.
		/// </summary>
		/// <param name="suspendInterval">Intervalor de suspensão.</param>
		public TimeScheduler(long suspendInterval)
		{
			_threadState = State.SUSPEND;
			_queue = new EventQueue();
			_suspendInterval = suspendInterval;
		}

		/// <summary>
		/// Método de execução.
		/// </summary>
		private void Run()
		{
			try
			{
				Task task;
				TimeScheduler scheduler;
				while (true)
				{
					Monitor.Enter(scheduler = this);
					try
					{
						if(_thread == null)
							return;
					}
					finally
					{
						Monitor.Exit(scheduler);
					}
					lock (_queue)
					{
						long elapsedTime;
						if(_queue.IsEmpty)
							Monitor.Wait(_queue, (int)_suspendInterval);
						if(_queue.IsEmpty)
						{
							InternalSuspend();
							return;
						}
						QueuedEvent o = _queue.Peek();
						lock (o)
						{
							task = o.Task;
							if(task.IsCancelled())
							{
								_queue.Pop();
								continue;
							}
							elapsedTime = o.ElapsedTime;
							if(elapsedTime >= o.Interval)
							{
								_queue.Pop();
								if(o.ReQueue())
								{
									_queue.Push(o);
								}
							}
						}
						if(elapsedTime < o.Interval)
						{
							Monitor.Wait(_queue, (int)(o.Interval - elapsedTime));
							continue;
						}
					}
					try
					{
						task.Run();
					}
					catch(Exception exception)
					{
						Colosoft.Logging.Trace.Error("TimeScheduler._run()".GetFormatter(), exception.GetFormatter());
					}
				}
			}
			catch(ThreadInterruptedException exception2)
			{
				Colosoft.Logging.Trace.Error("TimeScheduler._run()".GetFormatter(), exception2.GetFormatter());
			}
		}

		/// <summary>
		/// Método usado para dar inicio ao processamento.
		/// </summary>
		private void InternalStart()
		{
			lock (this)
			{
				if(_threadState != State.DISPOSED)
				{
					_threadState = State.RUN;
					_thread = new Thread(new ThreadStart(this.Run));
					_thread.Name = "TimeScheduler.Thread";
					_thread.IsBackground = true;
					_thread.Start();
				}
			}
		}

		/// <summary>
		/// Método usado para finalizar o processamente.
		/// </summary>
		private void InternalStop()
		{
			lock (this)
			{
				if(_threadState != State.DISPOSED)
				{
					_threadState = State.STOP;
					_thread = null;
				}
			}
		}

		/// <summary>
		/// Método acionado quando o processa está sendo finalizado.
		/// </summary>
		private void InternalStopping()
		{
			lock (this)
			{
				if(_threadState != State.DISPOSED)
					_threadState = State.STOPPING;
			}
		}

		/// <summary>
		/// Suspende o processamento.
		/// </summary>
		private void InternalSuspend()
		{
			lock (this)
			{
				if(_threadState != State.DISPOSED)
				{
					_threadState = State.SUSPEND;
					_thread = null;
				}
			}
		}

		/// <summary>
		/// Desfaz a tarefa de suspender.
		/// </summary>
		private void InternalUnsuspend()
		{
			this.InternalStart();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			Thread thread = null;
			lock (this)
			{
				if(_threadState == State.DISPOSED)
					return;
				thread = _thread;
				_threadState = State.DISPOSED;
				_thread = null;
				if(thread != null)
				{
					thread.Interrupt();
				}
			}
			if(thread != null)
			{
				thread.Join();
				_queue.Clear();
			}
		}

		/// <summary>
		/// Adiciona uma nova tarefa.
		/// </summary>
		/// <param name="task"></param>
		public void AddTask(Task task)
		{
			this.AddTask(task, true);
		}

		/// <summary>
		/// Adiciona uma nova tarefa.
		/// </summary>
		/// <param name="task">Instancia da tarefa.</param>
		/// <param name="relative">True identifica se é uma tarefa relativa.</param>
		public void AddTask(Task task, bool relative)
		{
			lock (this)
			{
				if((_threadState != State.DISPOSED) && (task.GetNextInterval() >= 0))
				{
					_queue.Push(new QueuedEvent(task));
					switch(_threadState)
					{
					case State.SUSPEND:
						this.InternalUnsuspend();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Inicia o processamento.
		/// </summary>
		public void Start()
		{
			lock (this)
			{
				switch(_threadState)
				{
				case State.SUSPEND:
					this.InternalUnsuspend();
					break;
				case State.STOP:
					this.InternalStart();
					break;
				}
			}
		}

		/// <summary>
		/// Para o processamento.
		/// </summary>
		public void Stop()
		{
			lock (this)
			{
				switch(_threadState)
				{
				case State.RUN:
					this.InternalStopping();
					break;
				case State.SUSPEND:
					this.InternalStop();
					return;
				case State.STOPPING:
				case State.STOP:
				case State.DISPOSED:
					return;
				}
				_thread.Interrupt();
			}
			_thread.Join();
			lock (this)
			{
				_queue.Clear();
				this.InternalStop();
			}
		}

		/// <summary>
		/// Assinatura da classe que represente uma tarfa.
		/// </summary>
		public interface Task
		{
			/// <summary>
			/// Recupera o próximo intervalo.
			/// </summary>
			/// <returns></returns>
			long GetNextInterval();

			/// <summary>
			/// Verifica se foi cancelada.
			/// </summary>
			/// <returns></returns>
			bool IsCancelled();

			/// <summary>
			/// Método de execução.
			/// </summary>
			void Run();
		}
	}
}
