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
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.ServiceModel.Channels.Http
{
	internal abstract class HttpListenerManager
	{
		protected HttpListenerManager()
		{
			Entries = new List<HttpChannelListenerEntry>();
		}

		protected List<HttpChannelListenerEntry> Entries
		{
			get;
			private set;
		}

		object entries_lock = new object();

		public abstract void RegisterListener(ChannelDispatcher channel, HttpTransportBindingElement element, TimeSpan timeout);

		public abstract void UnregisterListener(ChannelDispatcher channel, TimeSpan timeout);

		protected void RegisterListenerCommon(ChannelDispatcher channel, TimeSpan timeout)
		{
			lock (entries_lock)
			{
				Entries.Add(new HttpChannelListenerEntry(channel, new System.Threading.AutoResetEvent(false)));
				Entries.Sort(HttpChannelListenerEntry.CompareEntries);
			}
		}

		protected void UnregisterListenerCommon(ChannelDispatcher channel, TimeSpan timeout)
		{
			lock (entries_lock)
			{
				var entry = Entries.First(e => e.ChannelDispatcher == channel);
				Entries.Remove(entry);
				entry.WaitHandle.Set();
			}
		}

		public void ProcessNewContext(HttpContextInfo ctxi)
		{
			var ce = SelectChannel(ctxi);
			if(ce == null)
				throw new InvalidOperationException("HttpListenerContext does not match any of the registered channels");
			ce.ContextQueue.Enqueue(ctxi);
			ce.WaitHandle.Set();
		}

		HttpChannelListenerEntry SelectChannel(HttpContextInfo ctx)
		{
			lock (entries_lock)
			{
				foreach (var e in Entries)
					if(e.FilterHttpContext(ctx))
						return e;
			}
			return null;
		}

		public bool TryDequeueRequest(ChannelDispatcher channel, TimeSpan timeout, out HttpContextInfo context)
		{
			DateTime start = DateTime.Now;
			context = null;
			HttpChannelListenerEntry ce = null;
			lock (entries_lock)
			{
				ce = Entries.FirstOrDefault(e => e.ChannelDispatcher == channel);
			}
			if(ce == null)
				return false;
			lock (ce.RetrieverLock)
			{
				var q = ce.ContextQueue;
				if(q.Count == 0)
				{
					if(timeout.TotalMilliseconds < 0)
						return false;
					TimeSpan waitTimeout = timeout;
					if(timeout == TimeSpan.MaxValue)
						waitTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
					bool ret = ce.WaitHandle.WaitOne(waitTimeout);
					return ret && TryDequeueRequest(channel, waitTimeout - (DateTime.Now - start), out context);
				}
				context = q.Dequeue();
				return true;
			}
		}
	}
	internal class HttpStandaloneListenerManager : HttpListenerManager
	{
		public HttpStandaloneListenerManager(Uri uri, HttpTransportBindingElement element)
		{
			var l = new HttpListener();
			string uriString = element.HostNameComparisonMode == System.ServiceModel.HostNameComparisonMode.Exact ? uri.ToString() : uri.Scheme + "://*" + uri.GetComponents(UriComponents.Port | UriComponents.Path, UriFormat.SafeUnescaped);
			if(!uriString.EndsWith("/", StringComparison.Ordinal))
				uriString += "/";
			l.Prefixes.Add(uriString);
			this.listener = l;
		}

		HttpListener listener;

		System.Threading.Thread loop;

		public override void RegisterListener(ChannelDispatcher channel, HttpTransportBindingElement element, TimeSpan timeout)
		{
			RegisterListenerCommon(channel, timeout);
			if(Entries.Count != 1)
				return;
			if(element != null)
			{
				var l = listener;
				l.AuthenticationSchemeSelectorDelegate = delegate(HttpListenerRequest req) {
					return element.AuthenticationScheme;
				};
				l.Realm = element.Realm;
				l.UnsafeConnectionNtlmAuthentication = element.UnsafeConnectionNtlmAuthentication;
			}
			#if USE_SEPARATE_LOOP
						loop = new Thread (new ThreadStart (delegate {
				listener.Start ();
				try {
					while (true)
						ProcessNewContext (listener.GetContext ());
				} catch (ThreadAbortException) {
					Thread.ResetAbort ();
				}
				listener.Stop ();
			}));
			loop.Start ();
#else
			listener.Start();
			listener.BeginGetContext(GetContextCompleted, null);
			#endif
		}

		public override void UnregisterListener(ChannelDispatcher channel, TimeSpan timeout)
		{
			UnregisterListenerCommon(channel, timeout);
			if(Entries.Count > 0)
				return;
			#if USE_SEPARATE_LOOP
						loop.Abort ();
#else
			this.listener.Stop();
			#endif
		}

		void GetContextCompleted(IAsyncResult result)
		{
			var ctx = listener.EndGetContext(result);
			ProcessNewContext(ctx);
			listener.BeginGetContext(GetContextCompleted, null);
		}

		void ProcessNewContext(HttpListenerContext ctx)
		{
			if(ctx == null)
				return;
			ProcessNewContext(new HttpStandaloneContextInfo(ctx));
		}
	}
	internal class AspNetHttpListenerManager : HttpListenerManager
	{
		public AspNetHttpListenerManager(Uri uri)
		{
		}

		public override void RegisterListener(ChannelDispatcher channel, HttpTransportBindingElement element, TimeSpan timeout)
		{
			RegisterListenerCommon(channel, timeout);
		}

		public override void UnregisterListener(ChannelDispatcher channel, TimeSpan timeout)
		{
			UnregisterListenerCommon(channel, timeout);
		}
	}
}
