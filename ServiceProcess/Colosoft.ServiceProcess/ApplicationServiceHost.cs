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
using System.Web;

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Host de serviços da aplicação.
	/// </summary>
	public class ApplicationServiceHost : ServiceHost
	{
		/// <summary>
		/// Lista dos erros fatais.
		/// </summary>
		private List<Type> _fatalExceptionTypes = new List<Type>(new Type[] {
			typeof(OutOfMemoryException)
		});

		private List<Type> _exceptionTypesNotReported = new List<Type>(new Type[] {
			typeof(HttpException),
			typeof(CannotUnloadAppDomainException),
			typeof(ThreadAbortException),
			typeof(HttpRequestValidationException),
			typeof(HttpCompileException)
		});

		private ReaderWriterLock _exceptionTypesNotReportedLock = new ReaderWriterLock();

		private bool _disposed;

		private Thread _hostManagementThread;

		private Exception _hostManagementException;

		private ManualResetEvent _startedEvent;

		private ManualResetEvent _disposedEvent;

		private readonly TimeSpan _pollingTimeout = new TimeSpan(0, 0, 1);

		private TimeSpan _shutdownTimeLimit;

		private bool _isThreadSafe;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instanceId">Identificador da instancia.</param>
		/// <param name="logger"></param>
		public ApplicationServiceHost(Guid instanceId, Logging.ILogger logger) : this(instanceId, logger, false)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instanceId">Identificador da instancia.</param>
		/// <param name="logger"></param>
		/// <param name="threadSafe"></param>
		public ApplicationServiceHost(Guid instanceId, Logging.ILogger logger, bool threadSafe) : base(logger)
		{
			Initialize(instanceId, threadSafe);
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected ApplicationServiceHost(Logging.ILogger logger) : base(logger)
		{
		}

		/// <summary>
		/// Destrói.
		/// </summary>
		private void TearDown()
		{
			AssertManagementThread();
			if((base.Status == ServiceHostStatus.Started) || (base.Status == ServiceHostStatus.Paused))
			{
				Stop(this._shutdownTimeLimit);
			}
			base.StopServices(true);
			UnregisterServiceHost(this);
		}

		/// <summary>
		/// Thread do gereciamneto do host
		/// </summary>
		private void HostManagementThread()
		{
			bool initialStartup = true;
			DateTime utcNow = DateTime.UtcNow;
			Thread.CurrentThread.Name = "Host Management";
			while (!_disposedEvent.WaitOne(0, false))
			{
				if((_hostManagementException != null) || initialStartup)
				{
					Startup(initialStartup);
					if(_hostManagementException != null)
					{
						System.Diagnostics.EventLog.WriteEntry("ApplicationServiceHost", "Startup threw Exception - waiting 15 seconds for retry " + _hostManagementException, System.Diagnostics.EventLogEntryType.Error);
						_disposedEvent.WaitOne(new TimeSpan(0, 0, 15), false);
						continue;
					}
					initialStartup = false;
				}
				_disposedEvent.WaitOne(_pollingTimeout, false);
			}
			TearDown();
		}

		/// <summary>
		/// Assegura da thread que está chamando
		/// </summary>
		private void AssertManagementThread()
		{
			if(!_isThreadSafe && Thread.CurrentThread.ManagedThreadId != _hostManagementThread.ManagedThreadId)
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Carrega as configurações.
		/// </summary>
		private void LoadSettings()
		{
			_shutdownTimeLimit = TimeSpan.FromSeconds(90.0);
		}

		/// <summary>
		/// Inicializa o host.
		/// </summary>
		/// <param name="initialStartup"></param>
		private void Startup(bool initialStartup)
		{
			AssertManagementThread();
			try
			{
				base.Status = RegisterServiceHost(this);
				LoadSettings();
				Start();
				_hostManagementException = null;
			}
			catch(Exception exception)
			{
				if(initialStartup)
				{
				}
				_hostManagementException = exception;
			}
			if(initialStartup)
			{
				_startedEvent.Set();
			}
			else if(_hostManagementException == null)
			{
			}
		}

		/// <summary>
		/// Verifica se o erro foi filtrado.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		private bool IsExceptionFiltered(Exception exception)
		{
			try
			{
				_exceptionTypesNotReportedLock.AcquireReaderLock(-1);
				if(_exceptionTypesNotReported.Contains(exception.GetType()))
					return true;
			}
			finally
			{
				if(_exceptionTypesNotReportedLock.IsReaderLockHeld || _exceptionTypesNotReportedLock.IsWriterLockHeld)
					_exceptionTypesNotReportedLock.ReleaseReaderLock();
			}
			return ((exception is InvalidOperationException) && StringComparer.OrdinalIgnoreCase.Equals("System.Web.Services", exception.Source));
		}

		/// <summary>
		/// Inicializa o desligamento ser necessário.
		/// </summary>
		/// <param name="ex"></param>
		private void InitiateShutdownIfNecessary(Exception ex)
		{
			if(_fatalExceptionTypes.Contains(ex.GetType()))
			{
				if(System.Web.Hosting.HostingEnvironment.IsHosted)
					System.Web.Hosting.HostingEnvironment.InitiateShutdown();
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="instanceId"></param>
		/// <param name="threadSafe"></param>
		protected void Initialize(Guid instanceId, bool threadSafe)
		{
			_isThreadSafe = threadSafe;
			bool flag = false;
			try
			{
				_disposedEvent = new ManualResetEvent(false);
				_startedEvent = new ManualResetEvent(false);
				if(_isThreadSafe)
					Startup(true);
				else
				{
					_hostManagementThread = new Thread(HostManagementThread);
					_hostManagementThread.Start();
					_startedEvent.WaitOne();
				}
				if(_hostManagementException != null)
				{
					throw new ApplicationException(ResourceMessageFormatter.Create(() => Properties.Resources.UnhandledExceptionError).Format(), _hostManagementException);
				}
				flag = true;
			}
			finally
			{
				if(!flag)
					this.Dispose();
			}
		}

		/// <summary>
		/// Reporta o erro.
		/// </summary>
		/// <param name="watsonReportingName"></param>
		/// <param name="eventCategory"></param>
		/// <param name="exception"></param>
		/// <param name="additionalInfo"></param>
		protected virtual void ReportException(string watsonReportingName, string eventCategory, Exception exception, string[] additionalInfo)
		{
		}

		/// <summary>
		/// Registra uma informação.
		/// </summary>
		/// <param name="message"></param>
		protected void Info(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		/// <summary>
		/// Inicia o serviço.
		/// </summary>
		public override void Start()
		{
			Info("ApplicationServiceHost::Start");
			bool flag = false;
			try
			{
				flag = base.BeginStatusChange(ServiceHostStatus.Starting);
				if(flag)
				{
					StartServices();
				}
				else
				{
					Info("ApplicationServiceHost::Start - Status Change ignored");
				}
			}
			finally
			{
				if(flag)
					base.EndStatusChange();
			}
		}

		/// <summary>
		/// Para o serviço.
		/// </summary>
		/// <param name="timeout"></param>
		public override void Stop(TimeSpan timeout)
		{
			if(base.BeginStatusChange(ServiceHostStatus.Stopping))
			{
				base.CancelAllRequests(timeout);
				base.StopServices(false);
				base.EndStatusChange();
			}
			else
			{
			}
		}

		/// <summary>
		/// Registra o servido host.
		/// </summary>
		/// <param name="serviceHost"></param>
		/// <returns></returns>
		public ServiceHostStatus RegisterServiceHost(ServiceHost serviceHost)
		{
			serviceHost.Require("serviceHost").NotNull();
			serviceHost.StartTime = DateTime.UtcNow;
			return ServiceHostStatus.Starting;
		}

		/// <summary>
		/// Remove o registro do service host.
		/// </summary>
		/// <param name="serviceHost"></param>
		public virtual void UnregisterServiceHost(ServiceHost serviceHost)
		{
		}

		/// <summary>
		/// Define a situação do host.
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		protected override bool SetHostStatus(ServiceHostStatus status)
		{
			return true;
		}

		/// <summary>
		/// Manipula o erro.
		/// </summary>
		/// <param name="exception"></param>
		public void ExceptionHandler(Exception exception)
		{
			try
			{
				bool reportException;
				if(exception is ServiceException)
					reportException = ((ServiceException)exception).ReportException;
				else
					reportException = !IsExceptionFiltered(exception);
				InitiateShutdownIfNecessary(exception);
				if(reportException)
					ReportException("", "General", exception, null);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_disposed)
					return;
				_disposed = true;
				if(_disposedEvent != null)
				{
					_disposedEvent.Set();
					if(_hostManagementThread != null)
					{
						if(_hostManagementThread.ThreadState != System.Threading.ThreadState.Unstarted)
						{
							_hostManagementThread.Join();
						}
						_hostManagementThread = null;
					}
					_disposedEvent.Close();
					_disposedEvent = null;
				}
				if(_startedEvent != null)
				{
					_startedEvent.Close();
					_startedEvent = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
