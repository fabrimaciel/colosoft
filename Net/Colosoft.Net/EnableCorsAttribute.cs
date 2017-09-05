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

namespace Colosoft.Net.ServiceModel
{
	/// <summary>
	/// Attributo usado par aidentificar que o CORS está habilitado para o método.
	/// </summary>
	public class EnableCorsAttribute : Attribute, IOperationBehavior
	{
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
