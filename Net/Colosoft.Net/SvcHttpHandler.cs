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
using System.Text;
using System.Web;

namespace Colosoft.ServiceModel.Channels
{
	class SvcHttpHandler : IHttpHandler
	{
		internal static SvcHttpHandler Current;

		static object type_lock = new object();

		Type type;

		Type factory_type;

		string path;

		ServiceHostBase host;

		Dictionary<HttpContext, System.Threading.ManualResetEvent> wcf_wait_handles = new Dictionary<HttpContext, System.Threading.ManualResetEvent>();

		int close_state;

		public SvcHttpHandler(Type type, Type factoryType, string path)
		{
			this.type = type;
			this.factory_type = factoryType;
			this.path = path;
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public ServiceHostBase Host
		{
			get
			{
				return host;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			EnsureServiceHost();
			var table = Colosoft.ServiceModel.Channels.Http.HttpListenerManagerTable.GetOrCreate(host);
			var manager = table.GetOrCreateManager(context.Request.Url, null);
			if(manager == null)
				manager = table.GetOrCreateManager(host.BaseAddresses[0], null);
			var wait = new System.Threading.ManualResetEvent(false);
			wcf_wait_handles[context] = wait;
			manager.ProcessNewContext(new Colosoft.ServiceModel.Channels.Http.AspNetHttpContextInfo(this, context));
			wait.WaitOne();
		}

		public void EndHttpRequest(HttpContext context)
		{
			System.Threading.ManualResetEvent wait;
			if(!wcf_wait_handles.TryGetValue(context, out wait))
				return;
			wcf_wait_handles.Remove(context);
			if(wait != null)
				wait.Set();
		}

		public void Close()
		{
			host.Close();
			host = null;
		}

		void EnsureServiceHost()
		{
			lock (type_lock)
			{
				Current = this;
				try
				{
					EnsureServiceHostCore();
				}
				finally
				{
					Current = null;
				}
			}
		}

		void EnsureServiceHostCore()
		{
			if(host != null)
				return;
			var baseUri = new Uri(new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)), path);
			host = new ServiceHost(type, baseUri);
			host.Extensions.Add(new Activation.VirtualPathExtension(baseUri.AbsolutePath));
			host.Open();
		}
	}
}
