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
	internal abstract class InternalChannelListenerBase<TChannel> : ChannelListenerBase<TChannel> where TChannel : class, IChannel
	{
		protected InternalChannelListenerBase(BindingContext context) : base(context.Binding)
		{
			listen_uri = context.ListenUriRelativeAddress != null ? new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress) : context.ListenUriBaseAddress;
		}

		Uri listen_uri;

		Func<TimeSpan, TChannel> accept_channel_delegate;

		Func<TimeSpan, bool> wait_delegate;

		Action<TimeSpan> open_delegate, close_delegate;

		public MessageEncoder MessageEncoder
		{
			get;
			internal set;
		}

		public override Uri Uri
		{
			get
			{
				return listen_uri;
			}
		}

		protected System.Threading.Thread CurrentAsyncThread
		{
			get;
			private set;
		}

		protected IAsyncResult CurrentAsyncResult
		{
			get;
			private set;
		}

		protected override void OnAbort()
		{
			if(CurrentAsyncThread != null)
				CurrentAsyncThread.Abort();
		}

		protected override void OnClose(TimeSpan timeout)
		{
			if(CurrentAsyncThread != null)
				if(!CancelAsync(timeout))
					if(CurrentAsyncThread != null)
						CurrentAsyncThread.Abort();
		}

		public virtual bool CancelAsync(TimeSpan timeout)
		{
			return CurrentAsyncResult == null || CurrentAsyncResult.AsyncWaitHandle.WaitOne(timeout);
		}

		protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object asyncState)
		{
			var wait = new System.Threading.ManualResetEvent(false);
			if(accept_channel_delegate == null)
				accept_channel_delegate = new Func<TimeSpan, TChannel>(delegate(TimeSpan tout) {
					wait.WaitOne();
					CurrentAsyncThread = System.Threading.Thread.CurrentThread;
					try
					{
						return OnAcceptChannel(tout);
					}
					finally
					{
						CurrentAsyncThread = null;
						CurrentAsyncResult = null;
					}
				});
			CurrentAsyncResult = accept_channel_delegate.BeginInvoke(timeout, callback, asyncState);
			wait.Set();
			return CurrentAsyncResult;
		}

		protected override TChannel OnEndAcceptChannel(IAsyncResult result)
		{
			if(accept_channel_delegate == null)
				throw new InvalidOperationException("Async AcceptChannel operation has not started");
			return accept_channel_delegate.EndInvoke(result);
		}

		protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(wait_delegate == null)
				wait_delegate = new Func<TimeSpan, bool>(OnWaitForChannel);
			return wait_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override bool OnEndWaitForChannel(IAsyncResult result)
		{
			if(wait_delegate == null)
				throw new InvalidOperationException("Async WaitForChannel operation has not started");
			return wait_delegate.EndInvoke(result);
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(open_delegate == null)
				open_delegate = new Action<TimeSpan>(OnOpen);
			return open_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			if(open_delegate == null)
				throw new InvalidOperationException("Async Open operation has not started");
			open_delegate.EndInvoke(result);
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(close_delegate == null)
				close_delegate = new Action<TimeSpan>(OnClose);
			return close_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			if(close_delegate == null)
				throw new InvalidOperationException("Async Close operation has not started");
			close_delegate.EndInvoke(result);
		}
	}
	public abstract class ChannelListenerBase<TChannel> : ChannelListenerBase, IChannelListener<TChannel>, IChannelListener, ICommunicationObject where TChannel : class, IChannel
	{
		IDefaultCommunicationTimeouts timeouts;

		protected ChannelListenerBase() : this(DefaultCommunicationTimeouts.Instance)
		{
		}

		protected ChannelListenerBase(IDefaultCommunicationTimeouts timeouts)
		{
			if(timeouts == null)
				throw new ArgumentNullException("timeouts");
			this.timeouts = timeouts;
		}

		public TChannel AcceptChannel()
		{
			return AcceptChannel(timeouts.ReceiveTimeout);
		}

		public TChannel AcceptChannel(TimeSpan timeout)
		{
			ThrowIfDisposedOrNotOpen();
			return OnAcceptChannel(timeout);
		}

		public IAsyncResult BeginAcceptChannel(AsyncCallback callback, object asyncState)
		{
			return BeginAcceptChannel(timeouts.ReceiveTimeout, callback, asyncState);
		}

		public IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object asyncState)
		{
			return OnBeginAcceptChannel(timeout, callback, asyncState);
		}

		public TChannel EndAcceptChannel(IAsyncResult result)
		{
			return OnEndAcceptChannel(result);
		}

		protected abstract TChannel OnAcceptChannel(TimeSpan timeout);

		protected abstract IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object asyncState);

		protected abstract TChannel OnEndAcceptChannel(IAsyncResult result);
	}
	public abstract class ChannelListenerBase : ChannelManagerBase, IChannelListener, ICommunicationObject, IDefaultCommunicationTimeouts
	{
		IDefaultCommunicationTimeouts timeouts;

		KeyedByTypeCollection<object> properties;

		protected ChannelListenerBase() : this(DefaultCommunicationTimeouts.Instance)
		{
		}

		protected ChannelListenerBase(IDefaultCommunicationTimeouts timeouts)
		{
			this.timeouts = timeouts;
		}

		public abstract Uri Uri
		{
			get;
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				return timeouts.CloseTimeout;
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				return timeouts.OpenTimeout;
			}
		}

		public override TimeSpan DefaultReceiveTimeout
		{
			get
			{
				return timeouts.ReceiveTimeout;
			}
		}

		public override TimeSpan DefaultSendTimeout
		{
			get
			{
				return timeouts.SendTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
		{
			get
			{
				return timeouts.CloseTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
		{
			get
			{
				return timeouts.OpenTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
		{
			get
			{
				return timeouts.ReceiveTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.SendTimeout
		{
			get
			{
				return timeouts.SendTimeout;
			}
		}

		internal virtual KeyedByTypeCollection<object> Properties
		{
			get
			{
				if(properties == null)
					properties = new KeyedByTypeCollection<object>();
				return properties;
			}
		}

		public virtual T GetProperty<T>() where T : class
		{
			return properties != null ? properties.Find<T>() : null;
		}

		public IAsyncResult BeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return OnBeginWaitForChannel(timeout, callback, state);
		}

		public bool EndWaitForChannel(IAsyncResult result)
		{
			return OnEndWaitForChannel(result);
		}

		public bool WaitForChannel(TimeSpan timeout)
		{
			return OnWaitForChannel(timeout);
		}

		protected abstract IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state);

		protected abstract bool OnEndWaitForChannel(IAsyncResult result);

		protected abstract bool OnWaitForChannel(TimeSpan timeout);
	}
}
