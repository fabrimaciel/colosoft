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
	/// Implementação do provedor de serviço.
	/// </summary>
	public class ServiceProvider
	{
		private Dictionary<Type, object> _loadingServices = new Dictionary<Type, object>();

		private Dictionary<Type, IServerService> _managedServices = new Dictionary<Type, IServerService>();

		private object _serviceAccessLock = new object();

		private static List<Type> _coreServices = new List<Type>();

		private Logging.ILogger _logger;

		/// <summary>
		/// Relação dos serviços carregados
		/// </summary>
		public IEnumerable<IServerService> Services
		{
			get
			{
				return _managedServices.Values;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceProvider(Logging.ILogger logger)
		{
			logger.Require("logger").NotNull();
			_logger = logger;
		}

		/// <summary>
		/// Libera a instancia do serviço.
		/// </summary>
		/// <param name="service"></param>
		private static void DisposeService(ref IServerService service)
		{
			if((service != null) && (service is IDisposable))
				((IDisposable)service).Dispose();
			service = null;
		}

		/// <summary>
		/// Recupera o serviço do tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="creator"></param>
		/// <returns></returns>
		public IServerService GetService(Type serviceType, Func<IServerService> creator)
		{
			IServerService service = null;
			lock (_serviceAccessLock)
				_managedServices.TryGetValue(serviceType, out service);
			if(service == null)
			{
				object obj2 = null;
				lock (_serviceAccessLock)
				{
					if(!_loadingServices.TryGetValue(serviceType, out obj2))
					{
						obj2 = new object();
						_loadingServices[serviceType] = obj2;
					}
				}
				lock (obj2)
				{
					lock (_serviceAccessLock)
					{
						_managedServices.TryGetValue(serviceType, out service);
					}
					if(service == null)
					{
						try
						{
							try
							{
								service = creator();
							}
							catch(MissingMethodException exception)
							{
								throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.GetServiceArgumentError, serviceType).Format(), exception);
							}
							service.Initialize(_logger);
							service.ServiceStart();
							lock (_serviceAccessLock)
							{
								_managedServices.Add(serviceType, service);
								_loadingServices.Remove(serviceType);
							}
						}
						catch(Exception)
						{
							DisposeService(ref service);
							throw;
						}
					}
				}
			}
			return service;
		}

		/// <summary>
		/// Recupera o serviço do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>(Func<T> creator) where T : class, IServerService
		{
			return (T)GetService(typeof(T), () => creator());
		}

		/// <summary>
		/// Recupera o serviço do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>() where T : class, IServerService
		{
			return GetService<T>(() => (T)Activator.CreateInstance(typeof(T), true));
		}

		/// <summary>
		/// Para os serviços.
		/// </summary>
		/// <param name="stopCoreServices"></param>
		public void StopServices(bool stopCoreServices)
		{
			int num2 = 0;
			int num = 0x7f;
			do
			{
				List<KeyValuePair<Type, IServerService>> list = null;
				num2 = 0;
				lock (_serviceAccessLock)
					list = new List<KeyValuePair<Type, IServerService>>(_managedServices);
				foreach (var pair in list)
				{
					if(!_coreServices.Contains(pair.Key))
					{
						num2++;
						lock (_serviceAccessLock)
							_managedServices.Remove(pair.Key);
						try
						{
							pair.Value.ServiceEnd();
						}
						catch(Exception)
						{
						}
						finally
						{
							IDisposable disposable = pair.Value as IDisposable;
							if(disposable != null)
								disposable.Dispose();
						}
						continue;
					}
				}
			}
			while ((num2 != 0) && (--num != 0));
			if(stopCoreServices)
			{
				lock (_serviceAccessLock)
				{
					for(int i = 0; i < _coreServices.Count; i++)
					{
						IServerService service3;
						if(_managedServices.TryGetValue(_coreServices[i], out service3))
						{
							try
							{
								service3.ServiceEnd();
							}
							catch(Exception)
							{
							}
							finally
							{
								IDisposable disposable2 = service3 as IDisposable;
								if(disposable2 != null)
								{
									disposable2.Dispose();
								}
							}
						}
					}
					_managedServices.Clear();
				}
			}
		}

		/// <summary>
		/// Suspende os serviços.
		/// </summary>
		public void SuspendServices()
		{
			lock (_serviceAccessLock)
				foreach (var service in _managedServices.Values)
				{
				}
		}
	}
}
