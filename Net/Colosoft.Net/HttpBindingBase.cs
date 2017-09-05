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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Colosoft.ServiceModel
{
	public abstract class HttpBindingBase : Binding, IBindingRuntimePreferences
	{
		bool allow_cookies, bypass_proxy_on_local;

		HostNameComparisonMode host_name_comparison_mode = HostNameComparisonMode.StrongWildcard;

		long max_buffer_pool_size = 0x80000;

		int max_buffer_size = 0x10000;

		long max_recv_message_size = 0x10000;

		Uri proxy_address;

		XmlDictionaryReaderQuotas reader_quotas = new XmlDictionaryReaderQuotas();

		EnvelopeVersion env_version = EnvelopeVersion.Soap11;

		Encoding text_encoding = default_text_encoding;

		static readonly Encoding default_text_encoding = new UTF8Encoding();

		TransferMode transfer_mode = TransferMode.Buffered;

		bool use_default_web_proxy = true;

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

		public HostNameComparisonMode HostNameComparisonMode
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
				if(value <= 0)
					throw new ArgumentOutOfRangeException();
				max_buffer_pool_size = value;
			}
		}

		public int MaxBufferSize
		{
			get
			{
				return max_buffer_size;
			}
			set
			{
				if(value <= 0)
					throw new ArgumentOutOfRangeException();
				max_buffer_size = value;
			}
		}

		public long MaxReceivedMessageSize
		{
			get
			{
				return max_recv_message_size;
			}
			set
			{
				if(value <= 0)
					throw new ArgumentOutOfRangeException();
				max_recv_message_size = value;
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

		public override abstract string Scheme
		{
			get;
		}

		public EnvelopeVersion EnvelopeVersion
		{
			get
			{
				return env_version;
			}
		}

		internal static Encoding DefaultTextEncoding
		{
			get
			{
				return default_text_encoding;
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

		public override abstract BindingElementCollection CreateBindingElements();

		bool IBindingRuntimePreferences.ReceiveSynchronously
		{
			get
			{
				return false;
			}
		}
	}
}
