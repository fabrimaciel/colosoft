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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Colosoft.ServiceModel.Channels
{
	class HttpChannelFactory<TChannel> : TransportChannelFactoryBase<TChannel>
	{
		private IHttpCookieContainerManager cookie_manager;

		public HttpChannelFactory(HttpTransportBindingElement source, BindingContext ctx) : base(source, ctx)
		{
			ClientCredentials = ctx.BindingParameters.Find<ClientCredentials>();
			foreach (BindingElement be in ctx.Binding.Elements)
			{
				MessageEncodingBindingElement mbe = be as MessageEncodingBindingElement;
				if(mbe != null)
				{
					MessageEncoder = CreateEncoder<TChannel>(mbe);
					continue;
				}
				var tbe = be as HttpTransportBindingElement;
				if(tbe != null)
					cookie_manager = tbe.GetProperty<IHttpCookieContainerManager>(ctx);
			}
			if(MessageEncoder == null)
				MessageEncoder = new TextMessageEncoder(MessageVersion.Default, Encoding.UTF8);
		}

		public ClientCredentials ClientCredentials
		{
			get;
			private set;
		}

		protected override TChannel OnCreateChannel(System.ServiceModel.EndpointAddress address, Uri via)
		{
			ThrowIfDisposedOrNotOpen();
			if(!address.Uri.Scheme.StartsWith(Transport.Scheme))
				throw new ArgumentException(String.Format("Argument EndpointAddress has unsupported URI scheme: {0}", address.Uri.Scheme));
			if(MessageEncoder.MessageVersion.Addressing.Equals(AddressingVersion.None) && via != null && !address.Uri.Equals(via))
				throw new ArgumentException(String.Format("The endpoint address '{0}' and via uri '{1}' must match when the corresponding binding has addressing version in the message version value as None.", address.Uri, via));
			Type t = typeof(TChannel);
			if(t == typeof(IRequestChannel))
				return (TChannel)(object)new HttpRequestChannel((HttpChannelFactory<IRequestChannel>)(object)this, address, via);
			else if(t == typeof(IOutputChannel))
				throw new NotImplementedException();
			throw new InvalidOperationException(String.Format("channel type {0} is not supported.", typeof(TChannel).Name));
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
			if(open_delegate == null)
				throw new InvalidOperationException("Async open operation has not started");
			open_delegate.EndInvoke(result);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
		}

		public override T GetProperty<T>()
		{
			if(cookie_manager is T)
				return (T)(object)cookie_manager;
			return base.GetProperty<T>();
		}
	}
}
