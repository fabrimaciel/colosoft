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

namespace Colosoft.Net
{
	/// <summary>
	/// Armazena os dados de evento acionado pela class <see cref="ServicesConfiguration"/>.
	/// </summary>
	public class ServicesConfigurationActionEventArgs : EventArgs
	{
		private string _serviceName;

		private ServiceAddress _serviceAddress;

		private string _servicesContext;

		/// <summary>
		/// Nome do serviço associado.
		/// </summary>
		public string ServiceName
		{
			get
			{
				return _serviceName;
			}
		}

		/// <summary>
		/// Instancia do endereço associada com o evento.
		/// </summary>
		public ServiceAddress ServiceAddress
		{
			get
			{
				return _serviceAddress;
			}
		}

		/// <summary>
		/// Contexto do serviços adicionados.
		/// </summary>
		public string ServicesContext
		{
			get
			{
				return _servicesContext;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceName">Nome do serviço.</param>
		/// <param name="serviceAddress">Instancia do endereço associada com o evento.</param>
		/// <param name="servicesContext">Contexto de serviços.</param>
		public ServicesConfigurationActionEventArgs(string serviceName, ServiceAddress serviceAddress, string servicesContext)
		{
			_serviceName = serviceName;
			_serviceAddress = serviceAddress;
			_servicesContext = servicesContext;
		}
	}
	/// <summary>
	/// Representa os evento acionados por ações da classe de configuração.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate void ServicesConfigurationActionEventHandler (object sender, ServicesConfigurationActionEventArgs e);
}
