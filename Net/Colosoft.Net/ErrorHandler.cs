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

namespace Colosoft.Net.Diagnostics
{
	/// <summary>
	/// Classe responsável captura e tratar os erros ocorridos nos serviços.
	/// </summary>
	class ErrorHandler : System.ServiceModel.Dispatcher.IErrorHandler
	{
		/// <summary>
		/// Tenta tratar o erro ocorrido.
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public bool HandleError(Exception error)
		{
			System.Diagnostics.Debug.WriteLine("ERROR : " + error.ToString());
			return false;
		}

		/// <summary>
		/// Processa e fornece uma mensagem sobre o erro ocorrido.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="version"></param>
		/// <param name="fault"></param>
		public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
		{
			if(error is System.ServiceModel.FaultException)
			{
			}
			else
			{
				System.ServiceModel.Channels.MessageFault messageFault = System.ServiceModel.Channels.MessageFault.CreateFault(new System.ServiceModel.FaultCode("Sender"), new System.ServiceModel.FaultReason(error.Message), error, new System.Runtime.Serialization.NetDataContractSerializer());
				fault = System.ServiceModel.Channels.Message.CreateMessage(version, messageFault, null);
			}
		}
	}
}
