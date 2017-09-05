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
	public abstract class ChannelFactoryBase<TChannel> : ChannelFactoryBase, IChannelFactory<TChannel>
	{
		List<TChannel> channels = new List<TChannel>();

		protected ChannelFactoryBase() : this(DefaultCommunicationTimeouts.Instance)
		{
		}

		protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts) : base(timeouts)
		{
		}

		public TChannel CreateChannel(EndpointAddress remoteAddress)
		{
			if(remoteAddress == null)
				throw new ArgumentNullException("remoteAddress");
			return CreateChannel(remoteAddress, remoteAddress.Uri);
		}

		public TChannel CreateChannel(EndpointAddress remoteAddress, Uri via)
		{
			if(remoteAddress == null)
				throw new ArgumentNullException("remoteAddress");
			if(via == null)
				throw new ArgumentNullException("via");
			ValidateCreateChannel();
			var ch = OnCreateChannel(remoteAddress, via);
			channels.Add(ch);
			return ch;
		}

		protected abstract TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via);

		protected override void OnAbort()
		{
			foreach (IChannel ch in channels)
				ch.Abort();
			base.OnAbort();
		}

		protected override void OnClose(TimeSpan timeout)
		{
			DateTime start = DateTime.Now;
			foreach (IChannel ch in channels)
				ch.Close(timeout - (DateTime.Now - start));
			base.OnClose(timeout - (DateTime.Now - start));
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return base.OnBeginClose(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			base.OnEndClose(result);
		}

		protected void ValidateCreateChannel()
		{
			ThrowIfDisposedOrNotOpen();
			if(State == CommunicationState.Faulted)
				throw new CommunicationObjectFaultedException();
		}
	}
	public abstract class ChannelFactoryBase : ChannelManagerBase, IChannelFactory, ICommunicationObject
	{
		TimeSpan open_timeout, close_timeout, receive_timeout, send_timeout;

		protected ChannelFactoryBase() : this(DefaultCommunicationTimeouts.Instance)
		{
		}

		protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts)
		{
			open_timeout = timeouts.OpenTimeout;
			close_timeout = timeouts.CloseTimeout;
			send_timeout = timeouts.SendTimeout;
			receive_timeout = timeouts.ReceiveTimeout;
		}

		internal TimeSpan DefaultCloseTimeout2
		{
			get
			{
				return DefaultCloseTimeout;
			}
		}

		internal TimeSpan DefaultOpenTimeout2
		{
			get
			{
				return DefaultOpenTimeout;
			}
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				return close_timeout;
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				return open_timeout;
			}
		}

		public override TimeSpan DefaultReceiveTimeout
		{
			get
			{
				return receive_timeout;
			}
		}

		public override TimeSpan DefaultSendTimeout
		{
			get
			{
				return send_timeout;
			}
		}

		public virtual T GetProperty<T>() where T : class
		{
			return null;
		}

		protected override void OnAbort()
		{
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
			if(close_delegate == null)
				throw new InvalidOperationException("Async close operation has not started");
			close_delegate.EndInvoke(result);
		}

		protected override void OnClose(TimeSpan timeout)
		{
		}
	}
}
