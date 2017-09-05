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
using System.ServiceModel.Description;
using System.Text;
using Colosoft.Net.Json.Exceptions;
using Colosoft.Net.Json.Formatters;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class WebHttpJsonResolverBehavior : WebHttpJsonBehavior
	{
		private readonly Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter;

		private readonly Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dispatchFormatter"></param>
		/// <param name="clientFormatter"></param>
		public WebHttpJsonResolverBehavior(Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter, Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter) : this(new List<Type>(), dispatchFormatter, clientFormatter)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="knownTypes"></param>
		/// <param name="dispatchFormatter"></param>
		/// <param name="clientFormatter"></param>
		public WebHttpJsonResolverBehavior(IEnumerable<Type> knownTypes, Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter, Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter) : base(knownTypes)
		{
			if(dispatchFormatter == null)
				throw new ConfigServiceException("The dispatchFormatter function used by WebHttpJsonResolverBehavior instance cannot be null");
			if(clientFormatter == null)
				throw new ConfigServiceException("The clientFormatter function used by WebHttpJsonResolverBehavior instance cannot be null");
			this.dispatchFormatter = dispatchFormatter;
			this.clientFormatter = clientFormatter;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return this.dispatchFormatter.Invoke(operationDescription, endpoint);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return this.clientFormatter.Invoke(operationDescription, endpoint);
		}
	}
}
