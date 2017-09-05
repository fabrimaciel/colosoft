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

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Status do service host.
	/// </summary>
	public enum ServiceHostStatus
	{
		/// <summary>
		/// Iniciando.
		/// </summary>
		Starting = 0,
		/// <summary>
		/// Iniciado.
		/// </summary>
		Started = 1,
		/// <summary>
		/// Parando
		/// </summary>
		Stopping = 2,
		/// <summary>
		/// Parado.
		/// </summary>
		Stopped = 3,
		/// <summary>
		/// Pausando.
		/// </summary>
		Pausing = 4,
		/// <summary>
		/// Pausado.
		/// </summary>
		Paused = 5,
	}
	/// <summary>
	/// Host de serviço.
	/// </summary>
	public abstract class ServiceHost : IDisposable
	{
		private object _hostPropertiesLock = new object();

		private ServiceProvider _serviceProvider;

		private Logging.ILogger _logger;

		private string _name;

		private System.Collections.Hashtable _items = new System.Collections.Hashtable();

		/// <summary>
		/// Itens do contexto do host.
		/// </summary>
		public System.Collections.Hashtable Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Horário de inicio.
		/// </summary>
		public DateTime StartTime
		{
			get;
			set;
		}

		/// <summary>
		/// Situação do serviço.
		/// </summary>
		public ServiceHostStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do provedor de serviço.
		/// </summary>
		public ServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
		}

		/// <summary>
		/// Instancia do logger.
		/// </summary>
		protected Logging.ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Nome do Host.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Relação dos serviços carregados.
		/// </summary>
		public IEnumerable<IServerService> Services
		{
			get
			{
				return _serviceProvider.Services;
			}
		}

		/// <summary>
		/// Configurações.
		/// </summary>
		public virtual IServiceSettings Settings
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger"></param>
		public ServiceHost(Logging.ILogger logger)
		{
			logger.Require("logger").NotNull();
			_logger = logger;
			_serviceProvider = new ServiceProvider(logger);
		}

		/// <summary>
		/// Inicia o host
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// Para o host
		/// </summary>
		/// <param name="timeout"></param>
		public abstract void Stop(TimeSpan timeout);

		/// <summary>
		/// Recupera um serviço pelo tipo.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>() where T : class, IServerService
		{
			var service = _serviceProvider.GetService<T>();
			if(service is IServiceHostAccessor)
				((IServiceHostAccessor)service).ServiceHost = this;
			return service;
		}

		/// <summary>
		/// Recupera o serviço do tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="creator"></param>
		/// <returns></returns>
		public IServerService GetService(Type serviceType, Func<IServerService> creator)
		{
			var service = _serviceProvider.GetService(serviceType, creator);
			if(service is IServiceHostAccessor)
				((IServiceHostAccessor)service).ServiceHost = this;
			return service;
		}

		/// <summary>
		/// Define o status do host.
		/// </summary>
		/// <param name="newStatus"></param>
		/// <returns></returns>
		protected abstract bool SetHostStatus(ServiceHostStatus newStatus);

		/// <summary>
		/// Inicia os serviços.
		/// </summary>
		protected virtual void StartServices()
		{
		}

		/// <summary>
		/// Para o serviços.
		/// </summary>
		/// <param name="stopCoreServices"></param>
		protected internal void StopServices(bool stopCoreServices)
		{
			_serviceProvider.StopServices(stopCoreServices);
		}

		/// <summary>
		/// Supende os serviços.
		/// </summary>
		protected internal void SuspendServices()
		{
			_serviceProvider.SuspendServices();
		}

		/// <summary>
		/// Método acionado quando o status estiver sendo alterado.
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		protected bool BeginStatusChange(ServiceHostStatus status)
		{
			bool flag = false;
			lock (_hostPropertiesLock)
			{
				try
				{
					flag = this.SetHostStatus(status);
				}
				catch(ServiceException)
				{
					if(status != ServiceHostStatus.Stopping)
						throw;
					flag = true;
				}
				if(flag)
				{
					Status = status;
				}
				return flag;
			}
		}

		/// <summary>
		/// Método acionado quando o status estiver for alterado.
		/// </summary>
		protected void EndStatusChange()
		{
			lock (_hostPropertiesLock)
			{
				var newStatus = Status;
				if(Status == ServiceHostStatus.Starting)
					newStatus = ServiceHostStatus.Started;
				else if(Status == ServiceHostStatus.Stopping)
					newStatus = ServiceHostStatus.Stopped;
				else if(Status == ServiceHostStatus.Pausing)
					newStatus = ServiceHostStatus.Paused;
				if(newStatus != Status)
				{
					bool flag = false;
					try
					{
						flag = this.SetHostStatus(newStatus);
					}
					catch(ServiceException)
					{
						if(newStatus != ServiceHostStatus.Stopped)
							throw;
						flag = true;
					}
					if(flag)
					{
						Status = newStatus;
					}
				}
			}
		}

		/// <summary>
		/// Cancela todas as requisições.
		/// </summary>
		/// <param name="timeout"></param>
		protected void CancelAllRequests(TimeSpan timeout)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
