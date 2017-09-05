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

using Colosoft.ServiceModel.Channels.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Colosoft.ServiceModel.Channels
{
	public class HttpTransportBindingElement : TransportBindingElement, IPolicyExportExtension, IWsdlExportExtension
	{
		bool allow_cookies, bypass_proxy_on_local, unsafe_ntlm_auth;

		bool use_default_proxy = true, keep_alive_enabled = true;

		int max_buffer_size = 0x10000;

		HostNameComparisonMode host_cmp_mode;

		Uri proxy_address;

		string realm = String.Empty;

		TransferMode transfer_mode;

		IDefaultCommunicationTimeouts timeouts;

		AuthenticationSchemes auth_scheme = AuthenticationSchemes.Anonymous;

		AuthenticationSchemes proxy_auth_scheme = AuthenticationSchemes.Anonymous;

		HttpCookieContainerManager cookie_manager;

		public HttpTransportBindingElement()
		{
		}

		protected HttpTransportBindingElement(HttpTransportBindingElement other) : base(other)
		{
			allow_cookies = other.allow_cookies;
			bypass_proxy_on_local = other.bypass_proxy_on_local;
			unsafe_ntlm_auth = other.unsafe_ntlm_auth;
			use_default_proxy = other.use_default_proxy;
			keep_alive_enabled = other.keep_alive_enabled;
			max_buffer_size = other.max_buffer_size;
			host_cmp_mode = other.host_cmp_mode;
			proxy_address = other.proxy_address;
			realm = other.realm;
			transfer_mode = other.transfer_mode;
			timeouts = other.timeouts;
			auth_scheme = other.auth_scheme;
			proxy_auth_scheme = other.proxy_auth_scheme;
			DecompressionEnabled = other.DecompressionEnabled;
			LegacyExtendedProtectionPolicy = other.LegacyExtendedProtectionPolicy;
			ExtendedProtectionPolicy = other.ExtendedProtectionPolicy;
			cookie_manager = other.cookie_manager;
		}

		[DefaultValue(AuthenticationSchemes.Anonymous)]
		public AuthenticationSchemes AuthenticationScheme
		{
			get
			{
				return auth_scheme;
			}
			set
			{
				auth_scheme = value;
			}
		}

		[DefaultValue(AuthenticationSchemes.Anonymous)]
		public AuthenticationSchemes ProxyAuthenticationScheme
		{
			get
			{
				return proxy_auth_scheme;
			}
			set
			{
				proxy_auth_scheme = value;
			}
		}

		[DefaultValue(false)]
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

		[DefaultValue(false)]
		public bool BypassProxyOnLocal
		{
			get
			{
				return bypass_proxy_on_local;
			}
			set
			{
				bypass_proxy_on_local = value;
			}
		}

		[DefaultValue(false)]
		public bool DecompressionEnabled
		{
			get;
			set;
		}

		[DefaultValue(HostNameComparisonMode.StrongWildcard)]
		public HostNameComparisonMode HostNameComparisonMode
		{
			get
			{
				return host_cmp_mode;
			}
			set
			{
				host_cmp_mode = value;
			}
		}

		[DefaultValue(true)]
		public bool KeepAliveEnabled
		{
			get
			{
				return keep_alive_enabled;
			}
			set
			{
				keep_alive_enabled = value;
			}
		}

		[DefaultValue(0x10000)]
		public int MaxBufferSize
		{
			get
			{
				return max_buffer_size;
			}
			set
			{
				max_buffer_size = value;
			}
		}

		[DefaultValue(null)]
		[TypeConverter(typeof(UriTypeConverter))]
		public Uri ProxyAddress
		{
			get
			{
				return proxy_address;
			}
			set
			{
				proxy_address = value;
			}
		}

		[DefaultValue("")]
		public string Realm
		{
			get
			{
				return realm;
			}
			set
			{
				realm = value;
			}
		}

		public override string Scheme
		{
			get
			{
				return Uri.UriSchemeHttp;
			}
		}

		[DefaultValue(TransferMode.Buffered)]
		public TransferMode TransferMode
		{
			get
			{
				return transfer_mode;
			}
			set
			{
				transfer_mode = value;
			}
		}

		[DefaultValue(false)]
		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				return unsafe_ntlm_auth;
			}
			set
			{
				unsafe_ntlm_auth = value;
			}
		}

		[DefaultValue(true)]
		public bool UseDefaultWebProxy
		{
			get
			{
				return use_default_proxy;
			}
			set
			{
				use_default_proxy = value;
			}
		}

		[Obsolete("Use ExtendedProtectionPolicy")]
		public object LegacyExtendedProtectionPolicy
		{
			get;
			set;
		}

		public ExtendedProtectionPolicy ExtendedProtectionPolicy
		{
			get;
			set;
		}

		public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
		{
			return typeof(TChannel) == typeof(IRequestChannel);
		}

		#if !NET_2_1 && !XAMMAC_4_5
		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			return typeof(TChannel) == typeof(IReplyChannel);
		}

		#endif
		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			return new HttpChannelFactory<TChannel>(this, context);
		}

		#if !NET_2_1 && !XAMMAC_4_5
		internal static object ListenerBuildLock = new object();

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			return new HttpChannelListener<TChannel>(this, context);
		}

		#endif
		public override BindingElement Clone()
		{
			return new HttpTransportBindingElement(this);
		}

		public override T GetProperty<T>(BindingContext context)
		{
			if(typeof(T) == typeof(ISecurityCapabilities))
				return (T)(object)new HttpBindingProperties(this);
			if(typeof(T) == typeof(IBindingDeliveryCapabilities))
				return (T)(object)new HttpBindingProperties(this);
			if(typeof(T) == typeof(TransferMode))
				return (T)(object)TransferMode;
			if(typeof(T) == typeof(IHttpCookieContainerManager))
			{
				if(!AllowCookies)
					return null;
				if(cookie_manager == null)
					cookie_manager = new HttpCookieContainerManager();
				return (T)(object)cookie_manager;
			}
			return base.GetProperty<T>(context);
		}

		public WebSocketTransportSettings WebSocketSettings
		{
			get
			{
				throw new NotImplementedException("WebSocketSettings");
			}
			set
			{
				throw new NotImplementedException("WebSocketSettings");
			}
		}

		#if !NET_2_1 && !XAMMAC_4_5
		private static System.Reflection.MethodInfo _exportAddressingPolicy;

		private static MessageEncodingBindingElement ExportAddressingPolicy(PolicyConversionContext context)
		{
			return (MessageEncodingBindingElement)(_exportAddressingPolicy ?? (_exportAddressingPolicy = typeof(TransportBindingElement).GetMethod("ExportAddressingPolicy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static))).Invoke(null, new object[] {
				context
			});
		}

		void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
		{
			if(exporter == null)
				throw new ArgumentNullException("exporter");
			if(context == null)
				throw new ArgumentNullException("context");
			PolicyAssertionCollection assertions = context.GetBindingAssertions();
			var doc = new System.Xml.XmlDocument();
			ExportAddressingPolicy(context);
			switch(auth_scheme)
			{
			case AuthenticationSchemes.Basic:
			case AuthenticationSchemes.Digest:
			case AuthenticationSchemes.Negotiate:
			case AuthenticationSchemes.Ntlm:
				assertions.Add(doc.CreateElement("http", auth_scheme.ToString() + "Authentication", "http://schemas.microsoft.com/ws/06/2004/policy/http"));
				break;
			}
			var transportProvider = this as ITransportTokenAssertionProvider;
			if(transportProvider != null)
			{
				var token = transportProvider.GetTransportTokenAssertion();
				assertions.Add(CreateTransportBinding(token));
			}
		}

		XmlElement CreateTransportBinding(XmlElement transportToken)
		{
			var doc = new XmlDocument();
			var transportBinding = doc.CreateElement("sp", "TransportBinding", PolicyImportHelper.SecurityPolicyNS);
			var token = doc.CreateElement("sp", "TransportToken", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement(token, transportToken);
			var algorithmSuite = doc.CreateElement("sp", "AlgorithmSuite", PolicyImportHelper.SecurityPolicyNS);
			var basic256 = doc.CreateElement("sp", "Basic256", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement(algorithmSuite, basic256);
			var layout = doc.CreateElement("sp", "Layout", PolicyImportHelper.SecurityPolicyNS);
			var strict = doc.CreateElement("sp", "Strict", PolicyImportHelper.SecurityPolicyNS);
			PolicyImportHelper.AddWrappedPolicyElement(layout, strict);
			PolicyImportHelper.AddWrappedPolicyElements(transportBinding, token, algorithmSuite, layout);
			return transportBinding;
		}

		void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
		{
			throw new NotImplementedException();
		}

		void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
		{
			var extension = new System.Web.Services.Description.SoapBinding {
				Transport = "http://schemas.xmlsoap.org/soap/http",
				Style = System.Web.Services.Description.SoapBindingStyle.Document
			};
			context.WsdlBinding.Extensions.Add(extension);
			var binding2 = new System.Web.Services.Description.SoapAddressBinding {
				Location = context.Endpoint.Address.Uri.AbsoluteUri
			};
			context.WsdlPort.Extensions.Add(binding2);
		}
	#endif
	}
	class HttpBindingProperties : ISecurityCapabilities, IBindingDeliveryCapabilities
	{
		HttpTransportBindingElement source;

		public HttpBindingProperties(HttpTransportBindingElement source)
		{
			this.source = source;
		}

		public bool AssuresOrderedDelivery
		{
			get
			{
				return false;
			}
		}

		public bool QueuedDelivery
		{
			get
			{
				return false;
			}
		}

		public virtual ProtectionLevel SupportedRequestProtectionLevel
		{
			get
			{
				return ProtectionLevel.None;
			}
		}

		public virtual ProtectionLevel SupportedResponseProtectionLevel
		{
			get
			{
				return ProtectionLevel.None;
			}
		}

		public virtual bool SupportsClientAuthentication
		{
			get
			{
				return source.AuthenticationScheme != AuthenticationSchemes.Anonymous;
			}
		}

		public virtual bool SupportsServerAuthentication
		{
			get
			{
				switch(source.AuthenticationScheme)
				{
				case AuthenticationSchemes.Negotiate:
					return true;
				default:
					return false;
				}
			}
		}

		public virtual bool SupportsClientWindowsIdentity
		{
			get
			{
				switch(source.AuthenticationScheme)
				{
				case AuthenticationSchemes.Basic:
				case AuthenticationSchemes.Digest:
				case AuthenticationSchemes.Negotiate:
				case AuthenticationSchemes.Ntlm:
					return true;
				default:
					return false;
				}
			}
		}
	}
}
