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

namespace Colosoft.Web.SignalR
{
	/// <summary>
	/// Implementação do provedor dos hubs.
	/// </summary>
	class HubDescriptorProvider : Microsoft.AspNet.SignalR.Hubs.IHubDescriptorProvider
	{
		private List<Microsoft.AspNet.SignalR.Hubs.HubDescriptor> _descriptors;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public HubDescriptorProvider()
		{
			_descriptors = new List<Microsoft.AspNet.SignalR.Hubs.HubDescriptor>();
		}

		/// <summary>
		/// Recupera os descritores.
		/// </summary>
		/// <returns></returns>
		public IList<Microsoft.AspNet.SignalR.Hubs.HubDescriptor> GetHubs()
		{
			return _descriptors;
		}

		/// <summary>
		/// Tenta recuperar o descritor do hub.
		/// </summary>
		/// <param name="hubName"></param>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public bool TryGetHub(string hubName, out Microsoft.AspNet.SignalR.Hubs.HubDescriptor descriptor)
		{
			descriptor = _descriptors.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, hubName));
			return descriptor != null;
		}
	}
	/// <summary>
	/// Implementação do ativador de Hub.
	/// </summary>
	class HubActivator : Microsoft.AspNet.SignalR.Hubs.IHubActivator
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public HubActivator()
		{
		}

		/// <summary>
		/// Cria a instancia do hub.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public Microsoft.AspNet.SignalR.Hubs.IHub Create(Microsoft.AspNet.SignalR.Hubs.HubDescriptor descriptor)
		{
			return null;
		}
	}
}
