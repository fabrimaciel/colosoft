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
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;

namespace Colosoft.ServiceModel.Channels.Http
{
	internal class HttpChannelListener<TChannel> : InternalChannelListenerBase<TChannel> where TChannel : class, IChannel
	{
		HttpListenerManager listener_manager;

		public HttpChannelListener(HttpTransportBindingElement source, BindingContext context) : base(context)
		{
			this.Source = source;
			if(Uri != null && source.Scheme != Uri.Scheme)
				throw new ArgumentException(String.Format("Requested listen uri scheme must be {0}, but was {1}.", source.Scheme, Uri.Scheme));
			foreach (BindingElement be in context.Binding.Elements)
			{
				MessageEncodingBindingElement mbe = be as MessageEncodingBindingElement;
				if(mbe != null)
				{
					MessageEncoder = CreateEncoder<TChannel>(mbe);
					break;
				}
			}
			if(MessageEncoder == null)
				MessageEncoder = new TextMessageEncoder(MessageVersion.Default, Encoding.UTF8);
			if(context.BindingParameters.Contains(typeof(ServiceCredentials)))
				SecurityTokenManager = new ServiceCredentialsSecurityTokenManager((ServiceCredentials)context.BindingParameters[typeof(ServiceCredentials)]);
		}

		public HttpTransportBindingElement Source
		{
			get;
			private set;
		}

		public HttpListenerManager ListenerManager
		{
			get
			{
				return listener_manager;
			}
		}

		public ServiceCredentialsSecurityTokenManager SecurityTokenManager
		{
			get;
			private set;
		}

		private System.Threading.ManualResetEvent accept_channel_handle = new System.Threading.ManualResetEvent(true);

		protected override TChannel OnAcceptChannel(TimeSpan timeout)
		{
			DateTime start = DateTime.Now;
			TimeSpan waitTimeout;
			if(timeout == TimeSpan.MaxValue)
				waitTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
			else
				waitTimeout = timeout - (DateTime.Now - start);
			accept_channel_handle.WaitOne(waitTimeout);
			accept_channel_handle.Reset();
			TChannel ch = CreateChannel(timeout - (DateTime.Now - start));
			ch.Closed += delegate {
				accept_channel_handle.Set();
			};
			return ch;
		}

		protected TChannel CreateChannel(TimeSpan timeout)
		{
			lock (ThisLock)
			{
				return CreateChannelCore(timeout);
			}
		}

		TChannel CreateChannelCore(TimeSpan timeout)
		{
			if(typeof(TChannel) == typeof(IReplyChannel))
				return (TChannel)(object)new HttpReplyChannel((HttpChannelListener<IReplyChannel>)(object)this);
			throw new NotSupportedException(String.Format("Channel type {0} is not supported", typeof(TChannel)));
		}

		protected override bool OnWaitForChannel(TimeSpan timeout)
		{
			throw new NotImplementedException("HttpChannelListener OnWaitForChannel");
		}

		protected HttpListenerManager GetOrCreateListenerManager()
		{
			var table = HttpListenerManagerTable.GetOrCreate(null);
			return table.GetOrCreateManager(Uri, Source);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			listener_manager = GetOrCreateListenerManager();
			Properties.Add(listener_manager);
			listener_manager.RegisterListener(null, Source, timeout);
		}

		protected override void OnAbort()
		{
			listener_manager.UnregisterListener(null, TimeSpan.Zero);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			if(State == CommunicationState.Closed)
				return;
			base.OnClose(timeout);
			listener_manager.UnregisterListener(null, timeout);
		}

		public override bool CancelAsync(TimeSpan timeout)
		{
			try
			{
				CurrentAsyncResult.AsyncWaitHandle.WaitOne(TimeSpan.Zero);
			}
			catch(TimeoutException)
			{
			}
			return true;
		}
	}
}
