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
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Threading
{
	/// <summary>
	/// Implementa metodos para uma Task.
	/// </summary>
	public static class Task
	{
		/// <summary>
		/// Inicializa variáveis estáticas.
		/// </summary>
		static Task()
		{
			_globalTokenSource = new CancellationTokenSource();
			_globalToken = _globalTokenSource.Token;
		}

		/// <summary>
		/// Cancela ou aborta todas as Tasks.
		/// </summary>
		public static void CancelOrAbortAll()
		{
			_globalTokenSource.Cancel();
		}

		private static readonly CancellationToken _globalToken;

		private static readonly CancellationTokenSource _globalTokenSource;

		/// <summary>
		/// Fornece suporte para a criação e programação de Task objetos.
		/// </summary>
		public static class Factory
		{
			/// <summary>
			/// Cria uma nova Task.
			/// </summary>
			/// <param name="config">Configuração da Task.</param>
			/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
			/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
			public static ITask<TResult, TState> New<TResult, TState>(Config<TResult, TState> config)
			{
				if(config == null)
					throw new ArgumentNullException("config");
				return new _Task<TResult, TState>(config, new CancellationTokenSource());
			}

			/// <summary>
			/// Cria e executa uma nova Task.
			/// </summary>
			/// <param name="config">Configuração da Task.</param>
			/// <param name="state">Os dados a serem utilizados pela Task.</param>
			/// <param name="runSynchronously">Executa a Task de forma síncrona.</param>
			/// <param name="scheduler">O <see cref="System.Threading.Tasks.TaskScheduler"/> ao qual é associada e executada essa Task.</param>
			/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
			/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
			public static ITask<TResult, TState> StartNew<TResult, TState>(Config<TResult, TState> config, TState state, bool runSynchronously = false, TaskScheduler scheduler = null)
			{
				if(config == null)
					throw new ArgumentNullException("config");
				Data<TResult, TState> data = new Data<TResult, TState>(state);
				return new _Task<TResult, TState>(config, data, null, new CancellationTokenSource(), scheduler, runSynchronously);
			}

			/// <summary>
			/// Cancela a Task e cria e executa uma nova Task.
			/// </summary>
			/// <param name="task">Task a ser cancelada e criada novamente.</param>
			/// <param name="state">Os dados a serem utilizados pela Task.</param>
			/// <param name="runSynchronously">Executa a Task de forma síncrona.</param>
			/// <param name="scheduler">O <see cref="System.Threading.Tasks.TaskScheduler"/> ao qual é associada e executada essa Task.</param>
			/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
			/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
			public static ITask<TResult, TState> Restart<TResult, TState>(ITask<TResult, TState> task, TState state, bool runSynchronously = false, TaskScheduler scheduler = null)
			{
				if(task == null)
					throw new ArgumentNullException("task");
				if(!task.IsCompleted)
					task.CancelOrAbort();
				Data<TResult, TState> data = new Data<TResult, TState>(state);
				return new _Task<TResult, TState>(task.Config, data, task, new CancellationTokenSource(), scheduler, runSynchronously);
			}
		}

		/// <summary>
		/// Configuração da Task.
		/// </summary>
		/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
		/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
		public sealed class Config<TResult, TState>
		{
			/// <summary>
			/// Cria um novo objeto <c>Config</c>.
			/// </summary>
			/// <param name="runFunction">A função a ser executada na Task. A propriedade <c>Result</c> da Task será definido para o resultado desta função.</param>
			/// <param name="continueAction">Uma ação a ser executada de acordo com a condição especificada em continuationOptions, após a Task ser concluída.</param>
			/// <param name="creationOptions">Opções para personalizar o comportamento da Task.</param>
			/// <param name="continuationOptions">Opções para quando a continuação está agendada e como ele se comporta.</param>
			public Config(Func<ITask<TResult, TState>, TResult> runFunction, Action<ITask<TResult, TState>> continueAction = null, TaskCreationOptions creationOptions = TaskCreationOptions.None, TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
			{
				if(runFunction == null)
					throw new ArgumentNullException("runFunction");
				RunFunction = runFunction;
				ContinueAction = continueAction;
				CreationOptions = creationOptions;
				ContinuationOptions = continuationOptions;
			}

			/// <summary>
			/// A função a ser executada na Task. A propriedade <c>Result</c> da Task será definido para o resultado desta função.
			/// </summary>
			public readonly Func<ITask<TResult, TState>, TResult> RunFunction;

			/// <summary>
			/// Uma ação a ser executada de acordo com a condição especificada em continuationOptions, após a Task ser concluída.
			/// </summary>
			public readonly Action<ITask<TResult, TState>> ContinueAction;

			/// <summary>
			/// Opções para personalizar o comportamento da Task.
			/// </summary>
			public readonly TaskCreationOptions CreationOptions;

			/// <summary>
			/// Opções para quando a continuação está agendada e como ele se comporta.
			/// </summary>
			public readonly TaskContinuationOptions ContinuationOptions;
		}

		/// <summary>
		/// Os dados a serem utilizados pela Task.
		/// </summary>
		/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
		/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
		private sealed class Data<TResult, TState>
		{
			public Data(TState state)
			{
				State = state;
			}

			public readonly TState State;

			public _Task<TResult, TState> Task;
		}

		/// <summary>
		/// Representa uma operação assíncrona que pode retornar um valor.
		/// </summary>
		/// <typeparam name="TResult">Tipo do resultado da Task.</typeparam>
		/// <typeparam name="TState">Tipo do estado da Task.</typeparam>
		private sealed class _Task<TResult, TState> : Task<TResult>, ITask<TResult, TState>
		{
			/// <summary>
			/// Cria um novo objeto <c>Task</c>.
			/// </summary>
			/// <param name="config">Configuração da Task.</param>
			/// <param name="tokenSource">O <see cref="System.Threading.CancellationTokenSource"/> a ser atribuída à nova Task.</param>
			public _Task(Config<TResult, TState> config, CancellationTokenSource tokenSource) : base(DoRun, null, tokenSource.Token)
			{
				_config = config;
				_token = tokenSource.Token;
				_tokenSource = tokenSource;
				_globalToken.ThrowIfCancellationRequested();
				_globalToken.Register(CancelOrAbort);
			}

			/// <summary>
			/// Cria um novo objeto <c>Task</c>.
			/// </summary>
			/// <param name="config">Configuração da Task.</param>
			/// <param name="data">Os dados a serem utilizados pela Task.</param>
			/// <param name="prev">A Task anterior.</param>
			/// <param name="tokenSource">O <see cref="System.Threading.CancellationTokenSource"/> a ser atribuída à nova Task.</param>
			/// <param name="scheduler">O <see cref="System.Threading.Tasks.TaskScheduler"/> ao qual é associada e executada essa Task.</param>
			/// <param name="runSynchronously">Executa a Task de forma síncrona.</param>
			public _Task(Config<TResult, TState> config, Data<TResult, TState> data, ITask<TResult, TState> prev, CancellationTokenSource tokenSource, TaskScheduler scheduler, bool runSynchronously) : base(DoRun, data, tokenSource.Token, config.CreationOptions)
			{
				data.Task = this;
				_prev = prev;
				_config = config;
				_token = tokenSource.Token;
				_tokenSource = tokenSource;
				_globalToken.ThrowIfCancellationRequested();
				_globalToken.Register(CancelOrAbort);
				scheduler = scheduler ?? TaskScheduler.Current;
				if(runSynchronously)
				{
					if(config.ContinueAction != null)
						ContinueWith(DoContinue, CancellationToken.None, config.ContinuationOptions | TaskContinuationOptions.ExecuteSynchronously, scheduler);
					RunSynchronously(scheduler);
				}
				else
				{
					if(config.ContinueAction != null)
						ContinueWith(DoContinue, CancellationToken.None, config.ContinuationOptions, scheduler);
					Start(scheduler);
				}
			}

			/// <summary>
			/// Cancela ou aborta esta Task.
			/// </summary>
			public void CancelOrAbort()
			{
				try
				{
					_tokenSource.Cancel(true);
				}
				catch(AggregateException ex)
				{
					if(!(ex.InnerException is TaskCanceledException))
						throw ex.InnerException;
				}
			}

			/// <summary>
			/// Lança uma <see cref="System.OperationCanceledException" /> se esta Task teve o cancelamento solicitado.
			/// </summary>
			public void ThrowIfCancellationRequested()
			{
				_token.ThrowIfCancellationRequested();
			}

			/// <summary>
			/// Obtém o estado fornecido a Task quando foi criada ou null se não foi fornecida.
			/// </summary>
			public new TState AsyncState
			{
				get
				{
					return ((Data<TResult, TState>)base.AsyncState).State;
				}
			}

			/// <summary>
			/// Obtém se esta Task foi abortada.
			/// </summary>
			public bool IsAborted
			{
				get
				{
					return _token.IsCancellationRequested && !IsCanceled;
				}
			}

			/// <summary>
			/// Obtém se esta Task foi cancelada ou abortada.
			/// </summary>
			public bool IsCanceledOrAborted
			{
				get
				{
					return _token.IsCancellationRequested || IsCanceled;
				}
			}

			object ITask.AsyncState
			{
				get
				{
					return ((Data<TResult, TState>)base.AsyncState).State;
				}
			}

			Config<TResult, TState> ITask<TResult, TState>.Config
			{
				get
				{
					return _config;
				}
			}

			private static TResult DoRun(object taskData)
			{
				_Task<TResult, TState> task = ((Data<TResult, TState>)taskData).Task;
				if(task._prev != null)
					try
					{
						task._prev.Wait();
					}
					catch(AggregateException ex)
					{
						if(!(ex.InnerException is TaskCanceledException))
							throw ex.InnerException;
					}
				return task._config.RunFunction(task);
			}

			private static void DoContinue(Task<TResult> baseTask)
			{
				_Task<TResult, TState> task = (_Task<TResult, TState>)baseTask;
				task._config.ContinueAction(task);
			}

			private readonly CancellationToken _token;

			private readonly ITask<TResult, TState> _prev;

			private readonly Config<TResult, TState> _config;

			private readonly CancellationTokenSource _tokenSource;
		}
	}
}
