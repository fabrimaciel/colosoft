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
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;

namespace Colosoft.ServiceModel
{
	public class BasicHttpBinding : HttpBindingBase, IBindingRuntimePreferences
	{
		WSMessageEncoding message_encoding = WSMessageEncoding.Text;

		BasicHttpSecurity security;

		public BasicHttpBinding() : this(BasicHttpSecurityMode.None)
		{
		}

		#if !NET_2_1 && !XAMMAC_4_5
		public BasicHttpBinding(string configurationName) : this()
		{
			BindingsSection bindingsSection = Configuration.ConfigUtil.BindingsSection;
			BasicHttpBindingElement el = bindingsSection.BasicHttpBinding.Bindings[configurationName];
			el.ApplyConfiguration(this);
		}

		#endif
		public BasicHttpBinding(BasicHttpSecurityMode securityMode)
		{
			security = new BasicHttpSecurity();
		}

		public WSMessageEncoding MessageEncoding
		{
			get
			{
				return message_encoding;
			}
			set
			{
				message_encoding = value;
			}
		}

		public override string Scheme
		{
			get
			{
				switch(Security.Mode)
				{
				case BasicHttpSecurityMode.Transport:
				case BasicHttpSecurityMode.TransportWithMessageCredential:
					return Uri.UriSchemeHttps;
				default:
					return Uri.UriSchemeHttp;
				}
			}
		}

		public BasicHttpSecurity Security
		{
			get
			{
				return security;
			}
			set
			{
				security = value;
			}
		}

		public override BindingElementCollection CreateBindingElements()
		{
			var list = new List<BindingElement>();
			var security = CreateSecurityBindingElement();
			if(security != null)
				list.Add(security);
			list.Add(BuildMessageEncodingBindingElement());
			list.Add(GetTransport());
			return new BindingElementCollection(list.ToArray());
		}

		SecurityBindingElement CreateSecurityBindingElement()
		{
			SecurityBindingElement element;
			switch(Security.Mode)
			{
			case BasicHttpSecurityMode.Message:
				#if NET_2_1 || XAMMAC_4_5
								throw new NotImplementedException ();
#else
				if(Security.Message.ClientCredentialType != BasicHttpMessageCredentialType.Certificate)
					throw new InvalidOperationException("When Message security is enabled in a BasicHttpBinding, the message security credential type must be BasicHttpMessageCredentialType.Certificate.");
				element = SecurityBindingElement.CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10);
				break;
			#endif
			case BasicHttpSecurityMode.TransportWithMessageCredential:
				#if NET_2_1 || XAMMAC_4_5
								throw new NotImplementedException ();
#else
				if(Security.Message.ClientCredentialType != BasicHttpMessageCredentialType.Certificate)
					element = SecurityBindingElement.CreateCertificateOverTransportBindingElement();
				else
					element = new AsymmetricSecurityBindingElement();
				break;
			#endif
			default:
				return null;
			}
			#if !NET_2_1 && !XAMMAC_4_5
			element.SetKeyDerivation(false);
			element.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
			#endif
			return element;
		}

		MessageEncodingBindingElement BuildMessageEncodingBindingElement()
		{
			if(MessageEncoding == WSMessageEncoding.Text)
			{
				TextMessageEncodingBindingElement tm = new TextMessageEncodingBindingElement(MessageVersion.CreateVersion(EnvelopeVersion, AddressingVersion.None), TextEncoding);
				ReaderQuotas.CopyTo(tm.ReaderQuotas);
				return tm;
			}
			else
			{
				#if NET_2_1 || XAMMAC_4_5
								throw new NotImplementedException ();
#else
				return new MtomMessageEncodingBindingElement(MessageVersion.CreateVersion(EnvelopeVersion, AddressingVersion.None), TextEncoding);
				#endif
			}
		}

		TransportBindingElement GetTransport()
		{
			Channels.HttpTransportBindingElement h;
			switch(Security.Mode)
			{
			default:
				h = new Channels.HttpTransportBindingElement();
				break;
			}
			h.AllowCookies = AllowCookies;
			h.BypassProxyOnLocal = BypassProxyOnLocal;
			h.HostNameComparisonMode = HostNameComparisonMode;
			h.MaxBufferPoolSize = MaxBufferPoolSize;
			h.MaxBufferSize = MaxBufferSize;
			h.MaxReceivedMessageSize = MaxReceivedMessageSize;
			h.ProxyAddress = ProxyAddress;
			h.UseDefaultWebProxy = UseDefaultWebProxy;
			h.TransferMode = TransferMode;
			h.ExtendedProtectionPolicy = Security.Transport.ExtendedProtectionPolicy;
			switch(Security.Transport.ClientCredentialType)
			{
			case HttpClientCredentialType.Basic:
				h.AuthenticationScheme = AuthenticationSchemes.Basic;
				break;
			case HttpClientCredentialType.Ntlm:
				h.AuthenticationScheme = AuthenticationSchemes.Ntlm;
				break;
			case HttpClientCredentialType.Windows:
				h.AuthenticationScheme = AuthenticationSchemes.Negotiate;
				break;
			case HttpClientCredentialType.Digest:
				h.AuthenticationScheme = AuthenticationSchemes.Digest;
				break;
			case HttpClientCredentialType.Certificate:
				switch(Security.Mode)
				{
				case BasicHttpSecurityMode.Transport:
					break;
				case BasicHttpSecurityMode.TransportCredentialOnly:
					throw new InvalidOperationException("Certificate-based client authentication is not supported by 'TransportCredentialOnly' mode.");
				}
				break;
			}
			return h;
		}
	}
}
