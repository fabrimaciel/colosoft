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
using System.Xml;

namespace Colosoft.ServiceModel
{
	public abstract class WSHttpBindingBase : System.ServiceModel.Channels.Binding, System.ServiceModel.Channels.IBindingRuntimePreferences
	{
		bool bypass_proxy_on_local, reliable_session_enabled;

		System.ServiceModel.HostNameComparisonMode host_name_comparison_mode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;

		long max_buffer_pool_size = 0x80000;

		long max_recv_msg_size = 0x10000;

		System.ServiceModel.WSMessageEncoding message_encoding = System.ServiceModel.WSMessageEncoding.Text;

		Uri proxy_address;

		XmlDictionaryReaderQuotas reader_quotas = new XmlDictionaryReaderQuotas();

		System.ServiceModel.OptionalReliableSession reliable_session;

		System.ServiceModel.EnvelopeVersion env_version = System.ServiceModel.EnvelopeVersion.Soap12;

		Encoding text_encoding = new UTF8Encoding();

		bool transaction_flow;

		bool use_default_web_proxy = true;

		System.ServiceModel.Channels.ReliableSessionBindingElement rel_element = new System.ServiceModel.Channels.ReliableSessionBindingElement();

		protected WSHttpBindingBase() : this(false)
		{
		}

		protected WSHttpBindingBase(bool reliableSessionEnabled)
		{
			reliable_session = new System.ServiceModel.OptionalReliableSession(rel_element);
			reliable_session.Enabled = reliableSessionEnabled;
		}

		internal WSHttpBindingBase(System.ServiceModel.Configuration.WSHttpBindingBaseElement config) : this(config.ReliableSession.Enabled)
		{
			BypassProxyOnLocal = config.BypassProxyOnLocal;
			HostNameComparisonMode = config.HostNameComparisonMode;
			MaxBufferPoolSize = config.MaxBufferPoolSize;
			MaxReceivedMessageSize = config.MaxReceivedMessageSize;
			MessageEncoding = config.MessageEncoding;
			ProxyAddress = config.ProxyAddress;
			TextEncoding = config.TextEncoding;
			TransactionFlow = config.TransactionFlow;
			UseDefaultWebProxy = config.UseDefaultWebProxy;
			throw new NotImplementedException();
		}

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

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
		{
			get
			{
				return host_name_comparison_mode;
			}
			set
			{
				host_name_comparison_mode = value;
			}
		}

		public long MaxBufferPoolSize
		{
			get
			{
				return max_buffer_pool_size;
			}
			set
			{
				max_buffer_pool_size = value;
			}
		}

		public long MaxReceivedMessageSize
		{
			get
			{
				return max_recv_msg_size;
			}
			set
			{
				max_recv_msg_size = value;
			}
		}

		public System.ServiceModel.WSMessageEncoding MessageEncoding
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

		public XmlDictionaryReaderQuotas ReaderQuotas
		{
			get
			{
				return reader_quotas;
			}
			set
			{
				reader_quotas = value;
			}
		}

		public override string Scheme
		{
			get
			{
				return GetTransport().Scheme;
			}
		}

		public System.ServiceModel.OptionalReliableSession ReliableSession
		{
			get
			{
				return reliable_session;
			}
		}

		public System.ServiceModel.EnvelopeVersion EnvelopeVersion
		{
			get
			{
				return env_version;
			}
		}

		public Encoding TextEncoding
		{
			get
			{
				return text_encoding;
			}
			set
			{
				text_encoding = value;
			}
		}

		public bool TransactionFlow
		{
			get
			{
				return transaction_flow;
			}
			set
			{
				transaction_flow = value;
			}
		}

		public bool UseDefaultWebProxy
		{
			get
			{
				return use_default_web_proxy;
			}
			set
			{
				use_default_web_proxy = value;
			}
		}

		public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements()
		{
			System.ServiceModel.Channels.BindingElement tx = new System.ServiceModel.Channels.TransactionFlowBindingElement(System.ServiceModel.TransactionProtocol.WSAtomicTransactionOctober2004);
			var sec = CreateMessageSecurity();
			System.ServiceModel.Channels.BindingElement msg = null;
			var msgver = System.ServiceModel.Channels.MessageVersion.CreateVersion(EnvelopeVersion, System.ServiceModel.Channels.AddressingVersion.WSAddressing10);
			switch(MessageEncoding)
			{
			case System.ServiceModel.WSMessageEncoding.Mtom:
				msg = new System.ServiceModel.Channels.MtomMessageEncodingBindingElement(msgver, TextEncoding);
				break;
			case System.ServiceModel.WSMessageEncoding.Text:
				msg = new System.ServiceModel.Channels.TextMessageEncodingBindingElement(msgver, TextEncoding);
				break;
			default:
				throw new NotImplementedException("mhm, another WSMessageEncoding?");
			}
			var tr = GetTransport();
			var list = new List<System.ServiceModel.Channels.BindingElement>();
			list.Add(tx);
			if(sec != null)
				list.Add(sec);
			list.Add(msg);
			if(tr != null)
				list.Add(tr);
			return new System.ServiceModel.Channels.BindingElementCollection(list.ToArray());
		}

		protected abstract System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity();

		protected abstract System.ServiceModel.Channels.TransportBindingElement GetTransport();

		bool System.ServiceModel.Channels.IBindingRuntimePreferences.ReceiveSynchronously
		{
			get
			{
				return false;
			}
		}
	}
}
