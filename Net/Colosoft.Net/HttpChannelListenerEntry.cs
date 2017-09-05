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
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;

namespace Colosoft.ServiceModel.Channels.Http
{
	class HttpChannelListenerEntry
	{
		public HttpChannelListenerEntry(ChannelDispatcher channel, EventWaitHandle waitHandle)
		{
			ChannelDispatcher = channel;
			WaitHandle = waitHandle;
			ContextQueue = new Queue<HttpContextInfo>();
			RetrieverLock = new object();
		}

		public object RetrieverLock
		{
			get;
			private set;
		}

		public ChannelDispatcher ChannelDispatcher
		{
			get;
			private set;
		}

		public EventWaitHandle WaitHandle
		{
			get;
			private set;
		}

		public Queue<HttpContextInfo> ContextQueue
		{
			get;
			private set;
		}

		internal static int CompareEntries(HttpChannelListenerEntry e1, HttpChannelListenerEntry e2)
		{
			if(e1.ChannelDispatcher.Endpoints.Count == 0)
				return 1;
			if(e2.ChannelDispatcher.Endpoints.Count == 0)
				return -1;
			int p1 = e1.ChannelDispatcher.Endpoints.OrderByDescending(e => e.FilterPriority).First().FilterPriority;
			int p2 = e2.ChannelDispatcher.Endpoints.OrderByDescending(e => e.FilterPriority).First().FilterPriority;
			return p2 - p1;
		}

		internal bool FilterHttpContext(HttpContextInfo ctx)
		{
			if(ChannelDispatcher == null)
				return true;
			if(ctx.Request.HttpMethod.ToUpper() != "GET")
				return true;
			var sme = ChannelDispatcher.Host.Extensions.Find<ServiceMetadataExtension>();
			if(sme == null)
				return true;
			return true;
		}
	}
}
