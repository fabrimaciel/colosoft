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
	/// Implementação base do monitor do build de cache.
	/// </summary>
	public class DataCacheBuildMonitor : IDataCacheBuildMonitor
	{
		private System.Threading.Thread _monitorThread;

		private IDataCacheBuildManager _cacheBuildManager;

		private object _objLock = new object();

		private bool _isRunning = false;

		private AggregateDataCacheBuilderMonitorObserver _observer;

		private Guid _buildExecutionUid;

		private DateTime _expires = DateTime.Now;

		/// <summary>
		/// Identifica se há uma nova escuta registrada no monitor.
		/// </summary>
		private bool _isNewListen = false;

		/// <summary>
		/// Identifica se o monitor está executando.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return _isRunning;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheBuildManager"></param>
		public DataCacheBuildMonitor(IDataCacheBuildManager cacheBuildManager)
		{
			cacheBuildManager.Require("cacheBuildManager").NotNull();
			_cacheBuildManager = cacheBuildManager;
			_observer = new AggregateDataCacheBuilderMonitorObserver();
		}

		/// <summary>
		/// Método do robo do monitor.
		/// </summary>
		private void Robot()
		{
			_isRunning = true;
			try
			{
				while (true)
				{
					var entryUid = _buildExecutionUid;
					if(_expires <= DateTime.Now)
					{
						if(entryUid != Guid.Empty)
							_observer.OnProgressCompleted(new Progress.ProgressCompletedEventArgs(new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheBuildMonitor_ListeningExpired).Format()), true, null));
						System.Threading.Thread.Sleep(1000);
						continue;
					}
					BuildExecutionResult executionResult = null;
					try
					{
						executionResult = _cacheBuildManager.GetExecution(entryUid);
					}
					catch(Exception ex)
					{
						_observer.OnCheckError(entryUid, ex);
						continue;
					}
					if(_isNewListen)
						_observer.OnStart(this);
					_isNewListen = true;
					if(!string.IsNullOrEmpty(executionResult.Message))
						_observer.OnProgressMessageChanged(new Progress.ProgressMessageChangedEventArgs(executionResult.Message.GetFormatter(), null));
					switch(executionResult.State)
					{
					case BuildExecutionState.Finalized:
						_observer.OnProgressCompleted(new Progress.ProgressCompletedEventArgs((executionResult.ErrorMessage != null ? new Exception(executionResult.ErrorMessage) : null), false, null));
						_expires = DateTime.Now;
						_buildExecutionUid = Guid.Empty;
						break;
					case BuildExecutionState.Aborted:
						_observer.OnProgressCompleted(new Progress.ProgressCompletedEventArgs(new Exception(executionResult.Message ?? executionResult.ErrorMessage), false, null));
						_expires = DateTime.Now;
						_buildExecutionUid = Guid.Empty;
						break;
					case BuildExecutionState.NoExists:
						_observer.OnProgressCompleted(new Progress.ProgressCompletedEventArgs(new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheBuildMonitor_BuildExecutionNotExits, entryUid).Format()), false, null));
						_expires = DateTime.Now;
						_buildExecutionUid = Guid.Empty;
						break;
					default:
						_observer.OnProgressChanged(new Progress.ProgressChangedEventArgs(executionResult.TotalProgress, executionResult.CurrentProgress, this));
						break;
					}
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
			finally
			{
				_isRunning = false;
			}
		}

		/// <summary>
		/// Registra para escutar a execução do build.
		/// </summary>
		/// <param name="buildExecutionUid">Identificador da execução.</param>
		/// <param name="lifeTime">Tempo de vida da escuta.</param>
		public void Listen(Guid buildExecutionUid, TimeSpan lifeTime)
		{
			_buildExecutionUid = buildExecutionUid;
			_expires = DateTime.Now.Add(lifeTime);
			_isNewListen = true;
		}

		/// <summary>
		/// Aborta a escuta da execução do build.
		/// </summary>
		/// <param name="buildExecutionUid"></param>
		public void AbortListen(Guid buildExecutionUid)
		{
			if(_expires > DateTime.Now)
				_observer.OnProgressCompleted(new Progress.ProgressCompletedEventArgs(new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheBuildMonitor_ListeningExpired).Format()), true, null));
			_buildExecutionUid = Guid.Empty;
			_expires = DateTime.Now;
		}

		/// <summary>
		/// Inicia o monitor.
		/// </summary>
		public void Start()
		{
			if(!_isRunning)
				lock (_objLock)
				{
					if(_isRunning)
						return;
					_monitorThread = new System.Threading.Thread(Robot);
					_monitorThread.Start();
				}
		}

		/// <summary>
		/// Para o monitor.
		/// </summary>
		public void Stop()
		{
			if(_isRunning)
				lock (_objLock)
				{
					if(!_isRunning)
						return;
					_monitorThread.Abort();
					_monitorThread = null;
				}
		}

		/// <summary>
		/// Adiciona o observer para o monitor.
		/// </summary>
		/// <param name="observer">Instancia que será adicionada.</param>
		public void Add(IDataCacheBuildMonitorObserver observer)
		{
			_observer.Add(observer);
		}

		/// <summary>
		/// Remove o observer do monitor.
		/// </summary>
		/// <param name="observer">Instancia que será removida.</param>
		/// <returns></returns>
		public void Remove(IDataCacheBuildMonitorObserver observer)
		{
			_observer.Remove(observer);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			Stop();
		}

		/// <summary>
		/// Libera a instancia
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
