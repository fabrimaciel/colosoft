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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.ServiceModel.Cors
{
	/// <summary>
	/// PreflightOperationBehavior
	/// </summary>
	class PreflightOperationBehavior : IOperationBehavior
	{
		private OperationDescription _preflightOperation;

		private List<string> _allowedMethods;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="preflightOperation"></param>
		public PreflightOperationBehavior(OperationDescription preflightOperation)
		{
			_preflightOperation = preflightOperation;
			_allowedMethods = new List<string>();
		}

		/// <summary>
		/// Adiciona o método permitido.
		/// </summary>
		/// <param name="httpMethod"></param>
		public void AddAllowedMethod(string httpMethod)
		{
			_allowedMethods.Add(httpMethod);
		}

		/// <summary>
		/// Adiciona os parametros de vinculação.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento do cliente.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="clientOperation"></param>
		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		/// <summary>
		/// Aplica o comportamento do despachante.
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="dispatchOperation"></param>
		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.Invoker = new PreflightOperationInvoker(operationDescription.Messages[1].Action, _allowedMethods);
		}

		/// <summary>
		/// Valida a operação.
		/// </summary>
		/// <param name="operationDescription"></param>
		public void Validate(OperationDescription operationDescription)
		{
		}
	}
}
