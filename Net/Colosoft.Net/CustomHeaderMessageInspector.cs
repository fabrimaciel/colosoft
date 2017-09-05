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
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.ServiceModel.Configuration
{
	/// <summary>
	/// Implementação do inspetor de mensagens para adicionar cabeçalhos customizados.
	/// </summary>
	public class CustomHeaderMessageInspector : IDispatchMessageInspector
	{
		private Dictionary<string, string> _requiredHeaders;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="headers">Cabeçalhos customizados.</param>
		public CustomHeaderMessageInspector(Dictionary<string, string> headers)
		{
			_requiredHeaders = headers ?? new Dictionary<string, string>();
		}

		/// <summary>
		/// Método acionado depois de receber a requisição.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <param name="instanceContext"></param>
		/// <returns></returns>
		public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
		{
			return null;
		}

		/// <summary>
		/// Método acionado
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if(reply != null)
			{
				var httpHeader = reply.Properties["httpResponse"] as System.ServiceModel.Channels.HttpResponseMessageProperty;
				foreach (var item in _requiredHeaders)
				{
					httpHeader.Headers.Add(item.Key, item.Value);
				}
			}
		}
	}
}
