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
	internal abstract class RequestChannelBase : ChannelBase, IRequestChannel
	{
		private ChannelFactoryBase channel_factory;

		private EndpointAddress address;

		private Uri via;

		public RequestChannelBase(ChannelFactoryBase factory, EndpointAddress address, Uri via) : base(factory)
		{
			this.channel_factory = factory;
			this.address = address;
			this.via = via;
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				return channel_factory.DefaultCloseTimeout2;
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				return channel_factory.DefaultOpenTimeout2;
			}
		}

		public EndpointAddress RemoteAddress
		{
			get
			{
				return address;
			}
		}

		public Uri Via
		{
			get
			{
				return via ?? RemoteAddress.Uri;
			}
		}

		public override T GetProperty<T>()
		{
			if(typeof(T) == typeof(MessageVersion) && channel_factory is IHasMessageEncoder)
				return (T)(object)((IHasMessageEncoder)channel_factory).MessageEncoder.MessageVersion;
			if(typeof(T) == typeof(IChannelFactory))
				return (T)(object)channel_factory;
			return base.GetProperty<T>();
		}

		public Message Request(Message message)
		{
			return Request(message, DefaultSendTimeout);
		}

		public abstract Message Request(Message message, TimeSpan timeout);

		public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
		{
			return BeginRequest(message, DefaultSendTimeout, callback, state);
		}

		Func<Message, TimeSpan, Message> request_delegate;

		public virtual IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(request_delegate == null)
				request_delegate = new Func<Message, TimeSpan, Message>(Request);
			return request_delegate.BeginInvoke(message, timeout, callback, state);
		}

		public virtual Message EndRequest(IAsyncResult result)
		{
			return request_delegate.EndInvoke(result);
		}

		Action<TimeSpan> open_delegate;

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(open_delegate == null)
				open_delegate = new Action<TimeSpan>(OnOpen);
			return open_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			open_delegate.EndInvoke(result);
		}

		Action<TimeSpan> close_delegate;

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(close_delegate == null)
				close_delegate = new Action<TimeSpan>(OnClose);
			return close_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			close_delegate.EndInvoke(result);
		}
	}
}
