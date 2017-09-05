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
using System.Threading.Tasks;

namespace Colosoft.Web.SignalR
{
	/// <summary>
	/// Classe responsável por resolver da dependencia.
	/// </summary>
	class DependencyResolver : Microsoft.AspNet.SignalR.DefaultDependencyResolver
	{
		private HubDescriptorProvider _descriptorProvider;

		private HubActivator _activator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="agentService"></param>
		public DependencyResolver()
		{
			_descriptorProvider = new HubDescriptorProvider();
			_activator = new HubActivator();
		}

		/// <summary>
		/// Recupera o serviço pelo tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public override object GetService(Type serviceType)
		{
			if(serviceType == typeof(Microsoft.AspNet.SignalR.Hubs.IHubActivator))
				return _activator;
			return base.GetService(serviceType);
		}

		/// <summary>
		/// Recupera os serviços dos tipo informado.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public override IEnumerable<object> GetServices(Type serviceType)
		{
			if(serviceType == typeof(Microsoft.AspNet.SignalR.Hubs.IHubDescriptorProvider))
				return new[] {
					_descriptorProvider
				};
			else if(serviceType == typeof(Microsoft.AspNet.SignalR.Hubs.IHubActivator))
				return new[] {
					_activator
				};
			return base.GetServices(serviceType);
		}
	}
}
