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
using Colosoft.Logging;
using System.Threading;
using System.Collections;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o processador assincrono de escrita oculta.
	/// </summary>
	public class WriteBehindAsyncProcessor
	{
		private bool _isDisposing;

		private ILogger _logger;

		private object _processMutex;

		private WriteBehindQueue _queue;

		private object _statusMutex;

		private int _timeout;

		private Thread _worker;

		/// <summary>
		/// Identifica se o processador está em execução.
		/// </summary>
		internal bool IsRunning
		{
			get
			{
				return ((_worker != null) && _worker.IsAlive);
			}
		}

		/// <summary>
		/// <see cref="ILogger"/> que será usado pela instancia.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="taskWaiteTimeout"></param>
		/// <param name="logger"></param>
		internal WriteBehindAsyncProcessor(long taskWaiteTimeout, ILogger logger)
		{
			_logger = logger;
			_worker = null;
			_timeout = (int)taskWaiteTimeout;
			_statusMutex = new object();
			_processMutex = new object();
			_isDisposing = false;
			_queue = new WriteBehindQueue(0x100);
		}

		/// <summary>
		/// Método de execução do processador.
		/// </summary>
		protected void Run()
		{
			while ((_worker != null) && !_isDisposing)
			{
				IWriteBehindTask task = null;
				try
				{
					lock (this)
					{
						if(_queue.Count < 1)
							Monitor.Wait(this);
						if(_queue.Count > 0)
							task = _queue.Peek();
					}
					if(task != null)
					{
						switch(task.State)
						{
						case TaskState.Wait:
						{
							lock (_statusMutex)
							{
								if((task.State == TaskState.Wait) && !Monitor.Wait(_statusMutex, _timeout))
									task.State = TaskState.Execute;
							}
							continue;
						}
						case TaskState.Execute:
							_queue.Dequeue();
							lock (_processMutex)
							{
								if(!_isDisposing)
									task.Process();
								continue;
							}
						case TaskState.Remove:
							_queue.Dequeue();
							continue;
						}
					}
				}
				catch(ThreadAbortException)
				{
					break;
				}
				catch(Exception)
				{
					continue;
				}
			}
		}

		/// <summary>
		/// Executa todas as tarefas da origem.
		/// </summary>
		/// <param name="args"></param>
		protected void ExecuteAllTaskForSource(object args)
		{
			IWriteBehindTask task = null;
			object[] objArray = args as object[];
			string str = objArray[0] as string;
			bool flag = (bool)objArray[1];
			ArrayList list = new ArrayList();
			for(int i = 0; i < _queue.Count; i++)
			{
				try
				{
					task = _queue[i];
					if((task != null) && (task.Source == str))
					{
						list.Add(i);
						if(flag)
						{
							switch(task.State)
							{
							case TaskState.Wait:
							{
								lock (_statusMutex)
									if((task.State == TaskState.Wait) && !Monitor.Wait(_statusMutex, _timeout))
										task.State = TaskState.Execute;
								continue;
							}
							case TaskState.Execute:
								lock (_processMutex)
								{
									if(!_isDisposing)
										task.Process();
									continue;
								}
							case TaskState.Remove:
								_queue.Dequeue();
								continue;
							}
						}
					}
				}
				catch(ThreadAbortException)
				{
					break;
				}
				catch(Exception)
				{
				}
			}
			for(int j = list.Count - 1; j >= 0; j--)
			{
				_queue.RemoveAt((int)list[j]);
			}
		}

		/// <summary>
		/// Remove a tarefa da fila.
		/// </summary>
		/// <param name="taskId">Identificador da tarefa.</param>
		internal void Dequeue(string taskId)
		{
			lock (this)
			{
				IWriteBehindTask task = _queue.Peek();
				if((task != null) && (task.TaskId == taskId))
					_queue.Dequeue();
				else
				{
					var queue = new WriteBehindQueue(_queue);
					for(int i = 0; i < _queue.Count; i++)
					{
						task = queue.Dequeue();
						if((task != null) && (task.TaskId == taskId))
						{
							_queue = queue;
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Adiciona na fila uma nova tarefa de escrita.
		/// </summary>
		/// <param name="task"></param>
		internal void Enqueue(IWriteBehindTask task)
		{
			lock (this)
			{
				_queue.Enqueue(task);
				Monitor.PulseAll(this);
			}
		}

		/// <summary>
		/// Define o estado para a tarefa.
		/// </summary>
		/// <param name="taskId">Identificador da tarefa.</param>
		/// <param name="state">Estado da tarefa.</param>
		internal void SetState(string taskId, TaskState state)
		{
			lock (this)
			{
				_queue.UpdateState(taskId, state);
				IWriteBehindTask task = _queue.Peek();
				if((task != null) && (task.TaskId == taskId))
				{
					lock (_statusMutex)
						Monitor.PulseAll(_statusMutex);
				}
			}
		}

		/// <summary>
		/// Define um novo estado para a tarefa.
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="state"></param>
		/// <param name="newTable"></param>
		internal void SetState(string taskId, TaskState state, Hashtable newTable)
		{
			lock (this)
			{
				_queue.UpdateState(taskId, state, newTable);
				IWriteBehindTask task = _queue.Peek();
				if((task != null) && (task.TaskId == taskId))
				{
					lock (_statusMutex)
					{
						Monitor.PulseAll(_statusMutex);
					}
				}
			}
		}

		/// <summary>
		/// Inicia o processador.
		/// </summary>
		internal void Start()
		{
			lock (this)
			{
				if(_worker == null)
				{
					_worker = new Thread(new ThreadStart(this.Run));
					_worker.IsBackground = true;
					_worker.Name = "WriteBehindAsyncProcessor";
					_worker.Start();
				}
			}
		}

		/// <summary>
		/// Para o processador.
		/// </summary>
		internal void Stop()
		{
			lock (this)
			{
				Monitor.PulseAll(this);
				lock (_processMutex)
				{
					_isDisposing = true;
					if((_worker != null) && _worker.IsAlive)
					{
						_worker.Abort();
						_worker = null;
					}
				}
			}
		}

		/// <summary>
		/// Inicia a execução das tarefas para um origem.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="execute"></param>
		public void StartExecutionOfTasksForSource(string source, bool execute)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteAllTaskForSource), new object[] {
				source,
				execute
			});
		}

		/// <summary>
		/// Assinatura da tarefa de escrita oculta.
		/// </summary>
		internal interface IWriteBehindTask
		{
			/// <summary>
			/// Processa a tarefa.
			/// </summary>
			void Process();

			/// <summary>
			/// Atualiza os dados.
			/// </summary>
			/// <param name="updates">Hash com os dados para atualizar.</param>
			void Update(Hashtable updates);

			/// <summary>
			/// Identificador da tarefa.
			/// </summary>
			string TaskId
			{
				get;
			}

			/// <summary>
			/// Implementação do cache associada.
			/// </summary>
			CacheBase CacheImpl
			{
				get;
				set;
			}

			WriteThruProviderManager Manager
			{
				get;
				set;
			}

			/// <summary>
			/// Nome do provedor associado.
			/// </summary>
			string ProviderName
			{
				get;
			}

			/// <summary>
			/// Tamanho dos dados da tarefa.
			/// </summary>
			long Size
			{
				get;
			}

			/// <summary>
			/// Nome da origem da tarefa.
			/// </summary>
			string Source
			{
				get;
			}

			/// <summary>
			/// Estado da tarefa.
			/// </summary>
			WriteBehindAsyncProcessor.TaskState State
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Possíveis estados das tarefas.
		/// </summary>
		public enum TaskState : byte
		{
			/// <summary>
			/// Em execução.
			/// </summary>
			Execute = 1,
			/// <summary>
			/// Removendo.
			/// </summary>
			Remove = 2,
			/// <summary>
			/// Aguardando.
			/// </summary>
			Wait = 0
		}

		/// <summary>
		/// Implementação de uma fila para escritores.
		/// </summary>
		internal class WriteBehindQueue : ICollection, IEnumerable, ICloneable
		{
			private ArrayList _queue;

			public int Count
			{
				get
				{
					lock (_queue)
					{
						return _queue.Count;
					}
				}
			}

			public bool IsSynchronized
			{
				get
				{
					lock (_queue)
					{
						return _queue.IsSynchronized;
					}
				}
			}

			public WriteBehindAsyncProcessor.IWriteBehindTask this[int index]
			{
				get
				{
					lock (_queue)
					{
						if((index >= _queue.Count) || (index < 0))
						{
							throw new IndexOutOfRangeException();
						}
						return (_queue[index] as WriteBehindAsyncProcessor.IWriteBehindTask);
					}
				}
				set
				{
				}
			}

			public object SyncRoot
			{
				get
				{
					lock (_queue)
					{
						return _queue.SyncRoot;
					}
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public WriteBehindQueue()
			{
				_queue = new ArrayList();
			}

			public WriteBehindQueue(ICollection c)
			{
				_queue = new ArrayList(c);
			}

			public WriteBehindQueue(int capacity)
			{
				_queue = new ArrayList(capacity);
			}

			public void Clear()
			{
				lock (_queue)
				{
					_queue.Clear();
				}
			}

			public object Clone()
			{
				lock (_queue)
				{
					return _queue.Clone();
				}
			}

			public void CopyFrom(Array queue)
			{
				lock (_queue)
				{
					if((queue.Length > 0) && ((queue.Length + _queue.Count) > 0))
					{
						Array array = new WriteBehindAsyncProcessor.IWriteBehindTask[_queue.Count + queue.Length];
						queue.CopyTo(array, 0);
						_queue.CopyTo(array, queue.Length);
						_queue = new ArrayList(array);
					}
				}
			}

			public void CopyTo(Array array, int index)
			{
				lock (_queue)
				{
					_queue.CopyTo(array, index);
				}
			}

			public WriteBehindAsyncProcessor.IWriteBehindTask Dequeue()
			{
				lock (_queue)
				{
					if(_queue.Count == 0)
						return null;
					var task = _queue[0] as WriteBehindAsyncProcessor.IWriteBehindTask;
					_queue.RemoveAt(0);
					return task;
				}
			}

			public void Enqueue(WriteBehindAsyncProcessor.IWriteBehindTask task)
			{
				lock (_queue)
					_queue.Add(task);
			}

			public IEnumerator GetEnumerator()
			{
				lock (_queue)
					return _queue.GetEnumerator();
			}

			public WriteBehindAsyncProcessor.IWriteBehindTask Peek()
			{
				lock (_queue)
				{
					if(_queue.Count == 0)
					{
						return null;
					}
					return (_queue[0] as WriteBehindAsyncProcessor.IWriteBehindTask);
				}
			}

			public void Remove(string taskId)
			{
				lock (_queue)
				{
					for(int i = _queue.Count - 1; i >= 0; i--)
					{
						WriteBehindAsyncProcessor.IWriteBehindTask task = _queue[i] as WriteBehindAsyncProcessor.IWriteBehindTask;
						if(task.TaskId == taskId)
						{
							_queue.RemoveAt(i);
							break;
						}
					}
				}
			}

			public void RemoveAt(int index)
			{
				lock (_queue)
				{
					if((index >= _queue.Count) || (index < 0))
					{
						throw new IndexOutOfRangeException();
					}
					_queue.RemoveAt(index);
				}
			}

			public void UpdateState(Hashtable states)
			{
				lock (_queue)
				{
					int count = states.Count;
					int num = 0;
					int num2 = 0;
					while (num < _queue.Count)
					{
						WriteBehindAsyncProcessor.IWriteBehindTask task = _queue[num] as WriteBehindAsyncProcessor.IWriteBehindTask;
						if(states.ContainsKey(task.TaskId))
						{
							task.State = (WriteBehindAsyncProcessor.TaskState)states[task.TaskId];
							_queue[num] = task;
							if(states.Count == ++num2)
							{
								break;
							}
						}
						num++;
					}
				}
			}

			public void UpdateState(string source)
			{
				lock (_queue)
				{
					for(int i = 0; i < _queue.Count; i++)
					{
						WriteBehindAsyncProcessor.IWriteBehindTask task = _queue[i] as WriteBehindAsyncProcessor.IWriteBehindTask;
						if((task.Source == source) && (task.State == WriteBehindAsyncProcessor.TaskState.Wait))
						{
							task.State = WriteBehindAsyncProcessor.TaskState.Execute;
							_queue[i] = task;
						}
					}
				}
			}

			public void UpdateState(string taskId, WriteBehindAsyncProcessor.TaskState state)
			{
				lock (_queue)
				{
					for(int i = _queue.Count - 1; i >= 0; i--)
					{
						WriteBehindAsyncProcessor.IWriteBehindTask task = _queue[i] as WriteBehindAsyncProcessor.IWriteBehindTask;
						if(task.TaskId == taskId)
						{
							task.State = state;
							_queue[i] = task;
							break;
						}
					}
				}
			}

			public void UpdateState(string taskId, WriteBehindAsyncProcessor.TaskState state, Hashtable newBulkTable)
			{
				lock (_queue)
				{
					for(int i = _queue.Count - 1; i >= 0; i--)
					{
						WriteBehindAsyncProcessor.IWriteBehindTask task = _queue[i] as WriteBehindAsyncProcessor.IWriteBehindTask;
						if(task.TaskId == taskId)
						{
							task.Update(newBulkTable);
							task.State = state;
							_queue[i] = task;
							break;
						}
					}
				}
			}
		}
	}
}
