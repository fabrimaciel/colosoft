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
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel.Channels
{
	[Obsolete("Use AllowCookies.")]
	public class HttpCookieContainerBindingElement : BindingElement
	{
		HttpCookieContainerManager manager;

		public HttpCookieContainerBindingElement()
		{
			manager = new HttpCookieContainerManager();
		}

		protected HttpCookieContainerBindingElement(HttpCookieContainerBindingElement elementToBeCloned)
		{
			if(elementToBeCloned == null)
				throw new ArgumentNullException("elementToBeCloned");
			manager = new HttpCookieContainerManager(elementToBeCloned.manager);
		}

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			return base.BuildChannelFactory<TChannel>(context);
		}

		public override BindingElement Clone()
		{
			return new HttpCookieContainerBindingElement(this);
		}

		public override T GetProperty<T>(BindingContext context)
		{
			if(manager is T)
				return (T)(object)manager;
			return context.GetInnerProperty<T>();
		}
	}
	class HttpCookieContainerManager : IHttpCookieContainerManager
	{
		public HttpCookieContainerManager()
		{
			CookieContainer = new CookieContainer();
		}

		public HttpCookieContainerManager(HttpCookieContainerManager original)
		{
			CookieContainer = original.CookieContainer;
		}

		public CookieContainer CookieContainer
		{
			get;
			set;
		}
	}
}
