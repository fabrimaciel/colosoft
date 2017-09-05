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

namespace Colosoft.ServiceModel.Channels
{
	public abstract class ChannelManagerBase : CommunicationObject, ICommunicationObject, IDefaultCommunicationTimeouts
	{
		protected ChannelManagerBase()
		{
		}

		public abstract TimeSpan DefaultReceiveTimeout
		{
			get;
		}

		public abstract TimeSpan DefaultSendTimeout
		{
			get;
		}

		TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
		{
			get
			{
				return DefaultOpenTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
		{
			get
			{
				return DefaultCloseTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
		{
			get
			{
				return DefaultReceiveTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.SendTimeout
		{
			get
			{
				return DefaultSendTimeout;
			}
		}

		internal MessageEncoder CreateEncoder<TChannel>(MessageEncodingBindingElement mbe)
		{
			var f = mbe.CreateMessageEncoderFactory();
			var t = typeof(TChannel);
			if(t == typeof(IRequestSessionChannel) || 
			#if !NET_2_1
			t == typeof(IReplySessionChannel) || 
			#endif
			t == typeof(IInputSessionChannel) || t == typeof(IOutputSessionChannel) || t == typeof(IDuplexSessionChannel))
				return f.CreateSessionEncoder();
			else
				return f.Encoder;
		}
	}
}
