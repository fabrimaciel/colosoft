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
using System.Text;

namespace Colosoft.ServiceModel.Channels.Http
{
	internal class HttpListenerManagerTable
	{
		static readonly List<HttpListenerManagerTable> instances = new List<HttpListenerManagerTable>();

		public static HttpListenerManagerTable GetOrCreate(object serviceHostKey)
		{
			var m = instances.FirstOrDefault(p => p.ServiceHostKey == serviceHostKey);
			if(m == null)
			{
				m = new HttpListenerManagerTable(serviceHostKey);
				instances.Add(m);
			}
			return m;
		}

		HttpListenerManagerTable(object serviceHostKey)
		{
			ServiceHostKey = serviceHostKey ?? new object();
			listeners = new Dictionary<Uri, HttpListenerManager>();
		}

		Dictionary<Uri, HttpListenerManager> listeners;

		public object ServiceHostKey
		{
			get;
			private set;
		}

		public HttpListenerManager GetOrCreateManager(Uri uri, HttpTransportBindingElement element)
		{
			var m = listeners.FirstOrDefault(p => p.Key.Equals(uri)).Value;
			if(m == null)
			{
				string absolutePath = uri.AbsolutePath;
				if(absolutePath.EndsWith("/js", StringComparison.Ordinal) || absolutePath.EndsWith("/jsdebug", StringComparison.Ordinal))
					return CreateListenerManager(uri, element);
				UriBuilder ub = null;
				if(!String.IsNullOrEmpty(uri.Query))
				{
					ub = new UriBuilder(uri);
					ub.Query = null;
					m = listeners.FirstOrDefault(p => p.Key.Equals(ub.Uri)).Value;
					if(m != null)
						return m;
				}
				if(ub == null)
				{
					ub = new UriBuilder(uri);
					ub.Query = null;
				}
				int lastSlash = absolutePath.LastIndexOf('/');
				if(lastSlash != -1)
				{
					ub.Path = absolutePath.Substring(0, lastSlash);
					m = listeners.FirstOrDefault(p => p.Key.Equals(ub.Uri)).Value;
					if(m != null)
						return m;
				}
			}
			if(m == null)
				return CreateListenerManager(uri, element);
			return m;
		}

		HttpListenerManager CreateListenerManager(Uri uri, HttpTransportBindingElement element)
		{
			HttpListenerManager m;
			if(ServiceHostingEnvironment.InAspNet)
				m = new AspNetHttpListenerManager(uri);
			else
				m = new HttpStandaloneListenerManager(uri, element);
			listeners[uri] = m;
			return m;
		}
	}
}
