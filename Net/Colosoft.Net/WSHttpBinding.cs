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
using System.ServiceModel;
using System.Text;

namespace Colosoft.ServiceModel
{
	public class WSHttpBinding : WSHttpBindingBase
	{
		private WSHttpSecurity security;

		private bool allow_cookies;

		public WSHttpBinding() : this(SecurityMode.Message)
		{
		}

		public WSHttpBinding(SecurityMode mode) : this(mode, false)
		{
		}

		public WSHttpBinding(SecurityMode mode, bool reliableSessionEnabled) : base(reliableSessionEnabled)
		{
			security = new WSHttpSecurity(mode);
		}

		public WSHttpBinding(string configurationName)
		{
			throw new NotImplementedException();
		}

		public bool AllowCookies
		{
			get
			{
				return allow_cookies;
			}
			set
			{
				allow_cookies = value;
			}
		}

		public WSHttpSecurity Security
		{
			get
			{
				return security;
			}
		}

		public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements()
		{
			var bc = base.CreateBindingElements();
			switch(Security.Mode)
			{
			case SecurityMode.None:
			case SecurityMode.Transport:
				bc.RemoveAll<System.ServiceModel.Channels.SecurityBindingElement>();
				break;
			}
			return bc;
		}

		protected override System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity()
		{
			if(Security.Mode == SecurityMode.Transport || Security.Mode == SecurityMode.None)
				return null;
			var element = new System.ServiceModel.Channels.SymmetricSecurityBindingElement();
			element.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
			element.RequireSignatureConfirmation = true;
			switch(Security.Message.ClientCredentialType)
			{
			case MessageCredentialType.Certificate:
				var p = new System.ServiceModel.Security.Tokens.X509SecurityTokenParameters(System.ServiceModel.Security.Tokens.X509KeyIdentifierClauseType.Thumbprint);
				p.RequireDerivedKeys = false;
				element.EndpointSupportingTokenParameters.Endorsing.Add(p);
				goto default;
			case MessageCredentialType.IssuedToken:
				var istp = new System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters();
				istp.IssuerBinding = new System.ServiceModel.Channels.CustomBinding(new System.ServiceModel.Channels.TextMessageEncodingBindingElement(), GetTransport());
				element.EndpointSupportingTokenParameters.Endorsing.Add(istp);
				goto default;
			case MessageCredentialType.UserName:
				element.EndpointSupportingTokenParameters.SignedEncrypted.Add(new System.ServiceModel.Security.Tokens.UserNameSecurityTokenParameters());
				element.RequireSignatureConfirmation = false;
				goto default;
			case MessageCredentialType.Windows:
				if(Security.Message.NegotiateServiceCredential)
				{
					element.ProtectionTokenParameters = new System.ServiceModel.Security.Tokens.SspiSecurityTokenParameters();
				}
				else
				{
					element.ProtectionTokenParameters = new System.ServiceModel.Security.Tokens.KerberosSecurityTokenParameters();
				}
				break;
			default:
				if(Security.Message.NegotiateServiceCredential)
				{
					element.ProtectionTokenParameters = new System.ServiceModel.Security.Tokens.SslSecurityTokenParameters(false, true);
				}
				else
				{
					element.ProtectionTokenParameters = new System.ServiceModel.Security.Tokens.X509SecurityTokenParameters(System.ServiceModel.Security.Tokens.X509KeyIdentifierClauseType.Thumbprint, System.ServiceModel.Security.Tokens.SecurityTokenInclusionMode.Never);
					element.ProtectionTokenParameters.RequireDerivedKeys = true;
				}
				break;
			}
			if(!Security.Message.EstablishSecurityContext)
				return element;
			var reqs = new System.ServiceModel.Security.ChannelProtectionRequirements();
			return System.ServiceModel.Channels.SecurityBindingElement.CreateSecureConversationBindingElement(element, true, reqs);
		}

		protected override System.ServiceModel.Channels.TransportBindingElement GetTransport()
		{
			switch(Security.Mode)
			{
			case SecurityMode.Transport:
			case SecurityMode.TransportWithMessageCredential:
				return new System.ServiceModel.Channels.HttpsTransportBindingElement();
			default:
				return new ServiceModel.Channels.HttpTransportBindingElement();
			}
		}
	}
}
