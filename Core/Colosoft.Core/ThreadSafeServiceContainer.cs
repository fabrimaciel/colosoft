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
using System.ComponentModel.Design;

namespace Colosoft.Threading
{
	/// <summary>
	/// Implementação de um container de serviço ThreadSage
	/// </summary>
	public class ThreadSafeServiceContainer : IServiceContainer, IServiceProvider, IDisposable
	{
		private object _lock = new object();

		private ServiceContainer _serviceContainer = new ServiceContainer();

		/// <summary>
		/// Adiciona um serviço.
		/// </summary>
		/// <param name="serviceType">Tipo do serviço.</param>
		/// <param name="callback">Callback para a criação do serviço.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			lock (_lock)
				_serviceContainer.AddService(serviceType, callback);
		}

		/// <summary>
		/// Adiciona um serviço.
		/// </summary>
		/// <param name="serviceType">Tipo do serviço.</param>
		/// <param name="serviceInstance">Instancia do serviço.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
			lock (_lock)
				_serviceContainer.AddService(serviceType, serviceInstance);
		}

		/// <summary>
		/// Adiciona o serviço.
		/// </summary>
		/// <param name="serviceType">Tipo do serviço.</param>
		/// <param name="callback">Callback para criação.</param>
		/// <param name="promote"></param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			lock (_lock)
				_serviceContainer.AddService(serviceType, callback, promote);
		}

		/// <summary>
		/// Adiciona o serviço.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="serviceInstance"></param>
		/// <param name="promote"></param>
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			lock (_lock)
				_serviceContainer.AddService(serviceType, serviceInstance, promote);
		}

		/// <summary>
		/// Libera os serviços.
		/// </summary>
		public void FlushServices()
		{
			lock (_lock)
			{
				_serviceContainer.Dispose();
				_serviceContainer = new ServiceContainer();
			}
		}

		/// <summary>
		/// Recupera o serviço pelo tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public object GetService(Type serviceType)
		{
			lock (_lock)
				return _serviceContainer.GetService(serviceType);
		}

		/// <summary>
		/// Remove o serviço para o tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		public void RemoveService(Type serviceType)
		{
			lock (_lock)
				_serviceContainer.RemoveService(serviceType);
		}

		/// <summary>
		/// Remove o serviço para o tipo informado.
		/// </summary>
		/// <param name="serviceType">Tipo do serviço.</param>
		/// <param name="promote"></param>
		public void RemoveService(Type serviceType, bool promote)
		{
			lock (_lock)
				_serviceContainer.RemoveService(serviceType);
		}

		/// <summary>
		/// Libera a instancia. 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			lock (_lock)
				_serviceContainer.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
