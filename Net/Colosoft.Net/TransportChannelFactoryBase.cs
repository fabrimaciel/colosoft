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
	interface IHasMessageEncoder
	{
		MessageEncoder MessageEncoder
		{
			get;
		}
	}
	abstract class TransportChannelFactoryBase<TChannel> : ChannelFactoryBase<TChannel>, IHasMessageEncoder
	{
		protected TransportChannelFactoryBase(TransportBindingElement source, BindingContext ctx)
		{
			Transport = source;
		}

		public TransportBindingElement Transport
		{
			get;
			private set;
		}

		public MessageEncoder MessageEncoder
		{
			get;
			internal set;
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
	}
}
