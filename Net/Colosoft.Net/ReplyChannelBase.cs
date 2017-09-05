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
using System.Threading;

namespace Colosoft.ServiceModel.Channels
{
	internal abstract class InternalReplyChannelBase : ReplyChannelBase
	{
		public InternalReplyChannelBase(ChannelListenerBase listener) : base(listener)
		{
			local_address = new EndpointAddress(listener.Uri);
		}

		EndpointAddress local_address;

		public override EndpointAddress LocalAddress
		{
			get
			{
				return local_address;
			}
		}
	}
	internal abstract class ReplyChannelBase : ChannelBase, IReplyChannel
	{
		public ReplyChannelBase(ChannelListenerBase listener) : base(listener)
		{
			this.listener = listener;
		}

		ChannelListenerBase listener;

		public ChannelListenerBase Listener
		{
			get
			{
				return listener;
			}
		}

		public abstract EndpointAddress LocalAddress
		{
			get;
		}

		public override T GetProperty<T>()
		{
			if(typeof(T) == typeof(MessageVersion) && listener is IHasMessageEncoder)
				return (T)(object)((IHasMessageEncoder)listener).MessageEncoder.MessageVersion;
			if(typeof(T) == typeof(IChannelListener))
				return (T)(object)listener;
			return base.GetProperty<T>();
		}

		protected override void OnAbort()
		{
			OnClose(TimeSpan.Zero);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			if(currentAsyncThreads.Count > 0)
				if(!CancelAsync(timeout))
					foreach (System.Threading.Thread asyncThread in currentAsyncThreads)
						asyncThread.Abort();
		}

		public virtual bool CancelAsync(TimeSpan timeout)
		{
			return currentAsyncResults.Count > 0;
		}

		public virtual bool TryReceiveRequest()
		{
			RequestContext dummy;
			return TryReceiveRequest(DefaultReceiveTimeout, out dummy);
		}

		public abstract bool TryReceiveRequest(TimeSpan timeout, out RequestContext context);

		delegate bool TryReceiveDelegate (TimeSpan timeout, out RequestContext context);

		TryReceiveDelegate try_recv_delegate;

		object async_result_lock = new object();

		HashSet<Thread> currentAsyncThreads = new HashSet<Thread>();

		HashSet<IAsyncResult> currentAsyncResults = new HashSet<IAsyncResult>();

		public virtual IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			IAsyncResult result = null;
			if(try_recv_delegate == null)
				try_recv_delegate = new TryReceiveDelegate(delegate(TimeSpan tout, out RequestContext ctx) {
					lock (async_result_lock)
					{
						if(currentAsyncResults.Contains(result))
							currentAsyncThreads.Add(Thread.CurrentThread);
					}
					try
					{
						return TryReceiveRequest(tout, out ctx);
					}
					catch(System.Xml.XmlException ex)
					{
						Console.WriteLine("Xml Exception (Dropped Connection?):" + ex.Message);
					}
					catch(System.Net.Sockets.SocketException ex)
					{
						Console.WriteLine("Socket Exception (Dropped Connection?):" + ex.Message);
					}
					catch(System.IO.IOException ex)
					{
						Console.WriteLine("I/O Exception (Dropped Connection?):" + ex.Message);
					}
					finally
					{
						lock (async_result_lock)
						{
							currentAsyncResults.Remove(result);
							currentAsyncThreads.Remove(Thread.CurrentThread);
						}
					}
					ctx = null;
					return false;
				});
			RequestContext dummy;
			lock (async_result_lock)
			{
				result = try_recv_delegate.BeginInvoke(timeout, out dummy, callback, state);
				currentAsyncResults.Add(result);
			}
			return result;
		}

		public virtual bool EndTryReceiveRequest(IAsyncResult result)
		{
			RequestContext dummy;
			return EndTryReceiveRequest(result, out dummy);
		}

		public virtual bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
		{
			if(try_recv_delegate == null)
				throw new InvalidOperationException("BeginTryReceiveRequest operation has not started");
			return try_recv_delegate.EndInvoke(out context, result);
		}

		public virtual bool WaitForRequest()
		{
			return WaitForRequest(DefaultReceiveTimeout);
		}

		public abstract bool WaitForRequest(TimeSpan timeout);

		Func<TimeSpan, bool> wait_delegate;

		public virtual IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(wait_delegate == null)
				wait_delegate = new Func<TimeSpan, bool>(WaitForRequest);
			return wait_delegate.BeginInvoke(timeout, callback, state);
		}

		public virtual bool EndWaitForRequest(IAsyncResult result)
		{
			if(wait_delegate == null)
				throw new InvalidOperationException("BeginWaitForRequest operation has not started");
			return wait_delegate.EndInvoke(result);
		}

		public virtual RequestContext ReceiveRequest()
		{
			return ReceiveRequest(DefaultReceiveTimeout);
		}

		public abstract RequestContext ReceiveRequest(TimeSpan timeout);

		public virtual IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
		{
			return BeginReceiveRequest(DefaultReceiveTimeout, callback, state);
		}

		Func<TimeSpan, RequestContext> recv_delegate;

		public virtual IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(recv_delegate == null)
				recv_delegate = new Func<TimeSpan, RequestContext>(ReceiveRequest);
			return recv_delegate.BeginInvoke(timeout, callback, state);
		}

		public virtual RequestContext EndReceiveRequest(IAsyncResult result)
		{
			if(recv_delegate == null)
				throw new InvalidOperationException("BeginReceiveRequest operation has not started");
			return recv_delegate.EndInvoke(result);
		}

		Action<TimeSpan> open_delegate, close_delegate;

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(open_delegate == null)
				open_delegate = new Action<TimeSpan>(OnOpen);
			return open_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			if(open_delegate == null)
				throw new InvalidOperationException("async open operation has not started");
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
				throw new InvalidOperationException("async close operation has not started");
			close_delegate.EndInvoke(result);
		}
	}
}
