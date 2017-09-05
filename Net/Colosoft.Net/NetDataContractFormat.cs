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
using System.ServiceModel.Description;

namespace Colosoft.Net.Serialization
{
	/// <summary>
	/// Classe responsável pelo comportamento das operações do WebService.
	/// </summary>
	public class NetDataContractFormat : Attribute, IOperationBehavior
	{
		/// <summary>
		/// Registra o comportamento para o endpoint informado.
		/// </summary>
		/// <param name="endpoint"></param>
		public static void Register(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
			endpoint.Require("endpoint").NotNull();
			foreach (OperationDescription od in endpoint.Contract.Operations)
				od.Behaviors.Add(new NetDataContractFormat());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameters"></param>
		public void AddBindingParameters(OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters)
		{
		}

		/// <summary>
		/// Aplica o comportamento para o cliente.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="proxy"></param>
		public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
		{
			ReplaceDataContractSerializerOperationBehavior(description);
		}

		/// <summary>
		/// Aplica o comportamento para o despachante.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="dispatch"></param>
		public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
		{
			ReplaceDataContractSerializerOperationBehavior(description);
		}

		/// <summary>
		/// Valida a operação.
		/// </summary>
		/// <param name="description"></param>
		public void Validate(OperationDescription description)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="description"></param>
		private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
		{
			DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
			if(dcsOperationBehavior != null)
			{
				description.Behaviors.Remove(dcsOperationBehavior);
				description.Behaviors.Add(new NetDataContractSerializerOperationBehavior(description));
			}
		}
	}
}
