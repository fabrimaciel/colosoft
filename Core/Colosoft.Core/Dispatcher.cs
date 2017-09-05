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
	/// Dispatcher padrão.
	/// </summary>
	public class Dispatcher : IDispatcher, IDisposable
	{
		private System.Threading.Thread _thread;

		private Queue<DispatcherTask> _tasks = new Queue<DispatcherTask>();

		private System.Threading.ManualResetEvent _allDone;

		private string _name;

		/// <summary>
		/// Nome da thread do dispatcher.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Representa uma tarefa do dispatcher.
		/// </summary>
		class DispatcherTask
		{
			public Delegate Method;

			public object[] Arguments;

			public object Result;

			public Exception Error;

			public System.Threading.ManualResetEvent AllDone;

			/// <summary>
			/// Evento acionado quando a tarefa for executada.
			/// </summary>
			public event EventHandler Executed;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public DispatcherTask()
			{
				AllDone = new System.Threading.ManualResetEvent(false);
			}

			/// <summary>
			/// Executa a tarefa.
			/// </summary>
			public void Execute()
			{
				try
				{
					Result = Method.DynamicInvoke(Arguments);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					Error = ex.InnerException;
				}
				catch(Exception ex)
				{
					Error = ex;
				}
				finally
				{
					AllDone.Set();
					if(Executed != null)
						Executed(this, EventArgs.Empty);
				}
			}

			/// <summary>
			/// Recupera o resultado da tarefa  executada.
			/// </summary>
			/// <returns></returns>
			public object GetResult()
			{
				AllDone.WaitOne();
				if(Error != null)
					throw Error;
				return Result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Dispatcher() : this(null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		public Dispatcher(string name)
		{
			_name = name;
			_allDone = new System.Threading.ManualResetEvent(false);
			_thread = new System.Threading.Thread(Robot);
			_thread.Start();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~Dispatcher()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método do robo.
		/// </summary>
		private void Robot()
		{
			try
			{
				System.Threading.Thread.CurrentThread.Name = "DP " + Name;
				DispatcherTask task = null;
				while (true)
				{
					lock (_tasks)
						task = _tasks.Count > 0 ? _tasks.Dequeue() : null;
					if(task != null)
						task.Execute();
					var count = 0;
					lock (_tasks)
						count = _tasks.Count;
					if(count == 0)
						_allDone.WaitOne();
					lock (_tasks)
						if(_tasks.Count == 0)
							_allDone.Reset();
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
		}

		/// <summary>
		/// Verifica se a thread de chamada é a thread associada com o dispatcher.
		/// </summary>
		/// <returns></returns>
		public virtual bool CheckAccess()
		{
			return (System.Threading.Thread.CurrentThread == Thread || System.Threading.Thread.CurrentThread.ManagedThreadId == 1);
		}

		/// <summary>
		/// Verifica se a thraed de chamada é a thread associada com o dispatcher.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Caso a thread de chamada não seja a mesma que a Thread vinculada.</exception>
		public void VerifyAccess()
		{
			if(!CheckAccess())
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona com os argumentos especificados na linha que o Dispatcher foi criado.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Delegate method, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Action method)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada no segmento Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona com os argumentos especificados, na prioridade especificada, no segmento que o Dispatcher foi criado.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="priority"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada e com o argumento especificado no segmento do Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada e com a matriz especificada de argumentos na discussão do Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <param name="arg"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="method">Delegate do método que será executado.</param>
		/// <param name="args">Parametros do método que será executado.</param>
		/// <returns>
		/// O valor de retorno do delegate que está sendo 
		/// chamado ou nulo se o delegate não tem valor de retorno.
		/// </returns>
		public virtual object Invoke(Delegate method, params object[] args)
		{
			if(method == null)
				throw new ArgumentNullException("method");
			if(_thread == null)
				throw new ObjectDisposedException("this");
			var task = new DispatcherTask {
				Method = method,
				Arguments = args
			};
			if(_thread == System.Threading.Thread.CurrentThread)
				task.Execute();
			else
				lock (_tasks)
				{
					_tasks.Enqueue(task);
					_allDone.Set();
				}
			return task.GetResult();
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="method">Delegate do método que será executado.</param>
		/// <param name="priority"></param>
		/// <param name="args">Parametros do método que será executado.</param>
		/// <returns>
		/// O valor de retorno do delegate que está sendo 
		/// chamado ou nulo se o delegate não tem valor de retorno.
		/// </returns>
		public virtual object Invoke(Delegate method, DispatcherPriority priority, object[] args)
		{
			return Invoke(method, args);
		}

		/// <summary>
		/// Executa a sincronização com a thread do despachante.
		/// </summary>
		/// <param name="func">Func que será acionada.</param>
		/// <returns></returns>
		public T Invoke<T>(Func<T> func)
		{
			if(func == null)
				throw new ArgumentNullException("func");
			return (T)Invoke(func, DispatcherPriority.Normal, null);
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="action">Delegate do método que será executado.</param>
		public void Invoke(Action action)
		{
			if(action == null)
				throw new ArgumentNullException("action");
			Invoke(action, DispatcherPriority.Normal, null);
		}

		/// <summary>
		/// Processa os eventos pendentes no sistema.
		/// </summary>
		public void DoEvents()
		{
		}

		/// <summary>
		/// Thread relacionada com o dispatcher.
		/// </summary>
		public System.Threading.Thread Thread
		{
			get
			{
				return _thread;
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
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		public virtual void Dispose(bool disposing)
		{
			if(_thread != null)
				try
				{
					_thread.Abort();
				}
				catch
				{
				}
			_thread = null;
			_allDone.Set();
			while (_tasks.Count > 0)
			{
				var task = _tasks.Dequeue();
				task.Error = new ObjectDisposedException("dispatcher");
				task.AllDone.Set();
			}
		}
	}
}
