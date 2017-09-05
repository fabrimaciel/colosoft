﻿/* 
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Colosoft.Net.Remote.Server
{
	/// <summary>
	/// Assinatura do serviço do provedor de serviços.
	/// </summary>
	[ServiceContract]
	public interface IServiceAddressProviderService
	{
		/// <summary>
		/// Recupera os endereços do serviço com base no nome informado.
		/// </summary>
		/// <param name="serviceName">Nome do serviço.</param>
		/// <param name="servicesContext">Contexto de serviços que será usado para filtrar.</param>
		/// <returns></returns>
		[OperationContract]
		Colosoft.Net.ServiceAddress[] GetServiceAddresses(string serviceName, string servicesContext);
	}
}
