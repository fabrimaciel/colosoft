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
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Colosoft.Net
{
	/// <summary>
	/// Classe com método que auxiliam as configurações de comunicação.
	/// </summary>
	public static class CommunicationHelpers
	{
		/// <summary>
		/// Cria um binding para comunicação.
		/// </summary>
		/// <param name="securityMode">Modo de segurança.</param>
		/// <param name="requireClientCertificates"></param>
		/// <returns></returns>
		public static Binding CreateBinding(SecurityMode securityMode, bool requireClientCertificates)
		{
			WSHttpBinding binding = new WSHttpBinding(securityMode);
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
			binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
			binding.MaxReceivedMessageSize = System.Xml.XmlDictionaryReaderQuotas.Max.MaxStringContentLength;
			Binding binding2 = binding;
			if((securityMode == SecurityMode.Transport) && requireClientCertificates)
			{
				binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
				BindingElementCollection bindingElementsInTopDownChannelStackOrder = binding.CreateBindingElements();
				TransportSecurityBindingElement item = new TransportSecurityBindingElement();
				System.ServiceModel.Security.Tokens.X509SecurityTokenParameters parameters = new System.ServiceModel.Security.Tokens.X509SecurityTokenParameters();
				parameters.InclusionMode = System.ServiceModel.Security.Tokens.SecurityTokenInclusionMode.AlwaysToRecipient;
				item.EndpointSupportingTokenParameters.Endorsing.Add(parameters);
				bindingElementsInTopDownChannelStackOrder.Insert(bindingElementsInTopDownChannelStackOrder.Count - 1, item);
				binding2 = new CustomBinding(bindingElementsInTopDownChannelStackOrder);
			}
			return binding2;
		}

		/// <summary>
		/// Desliga a comunicação.
		/// </summary>
		/// <param name="communicationObject"></param>
		public static void Shutdown(ICommunicationObject communicationObject)
		{
			if(communicationObject != null)
			{
				try
				{
					communicationObject.BeginClose(new AsyncCallback(CommunicationHelpers.ShutdownCallback), communicationObject);
				}
				catch(TimeoutException)
				{
					communicationObject.Abort();
				}
				catch(CommunicationException)
				{
					communicationObject.Abort();
				}
			}
		}

		private static void ShutdownCallback(IAsyncResult result)
		{
			ICommunicationObject asyncState = result.AsyncState as ICommunicationObject;
			try
			{
				asyncState.EndClose(result);
			}
			catch(TimeoutException)
			{
				asyncState.Abort();
			}
			catch(CommunicationException)
			{
				asyncState.Abort();
			}
		}
	}
}
