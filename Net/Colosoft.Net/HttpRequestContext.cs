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
using System.Text;

namespace Colosoft.ServiceModel.Channels.Http
{
	internal class HttpRequestContext : RequestContext
	{
		public HttpRequestContext(HttpReplyChannel channel, HttpContextInfo context, Message request)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");
			if(context == null)
				throw new ArgumentNullException("context");
			if(request == null)
				throw new ArgumentNullException("request");
			this.channel = channel;
			this.context = context;
			this.request = request;
		}

		Message request;

		HttpReplyChannel channel;

		HttpContextInfo context;

		public override Message RequestMessage
		{
			get
			{
				return request;
			}
		}

		public HttpReplyChannel Channel
		{
			get
			{
				return channel;
			}
		}

		public HttpContextInfo Context
		{
			get
			{
				return context;
			}
		}

		public override IAsyncResult BeginReply(Message msg, AsyncCallback callback, object state)
		{
			return BeginReply(msg, Channel.DefaultSendTimeout, callback, state);
		}

		Action<Message, TimeSpan> reply_delegate;

		public override IAsyncResult BeginReply(Message msg, TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(reply_delegate == null)
				reply_delegate = new Action<Message, TimeSpan>(Reply);
			return reply_delegate.BeginInvoke(msg, timeout, callback, state);
		}

		public override void EndReply(IAsyncResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");
			if(reply_delegate == null)
				throw new InvalidOperationException("reply operation has not started");
			reply_delegate.EndInvoke(result);
		}

		public override void Reply(Message msg)
		{
			Reply(msg, Channel.DefaultSendTimeout);
		}

		public override void Reply(Message msg, TimeSpan timeout)
		{
			InternalReply(msg, timeout);
		}

		public override void Abort()
		{
			InternalAbort();
		}

		public override void Close()
		{
			Close(Channel.DefaultSendTimeout);
		}

		public override void Close(TimeSpan timeout)
		{
			InternalClose(timeout);
		}

		protected virtual void InternalAbort()
		{
			Context.Abort();
		}

		protected virtual void InternalClose(TimeSpan timeout)
		{
			Context.Close();
		}

		protected virtual void InternalReply(Message msg, TimeSpan timeout)
		{
			if(msg == null)
				throw new ArgumentNullException("msg");
			var ms = new System.IO.MemoryStream();
			Channel.Encoder.WriteMessage(msg, ms);
			Context.Response.ContentType = Channel.Encoder.ContentType;
			string pname = HttpResponseMessageProperty.Name;
			bool suppressEntityBody = false;
			if(msg.Properties.ContainsKey(pname))
			{
				HttpResponseMessageProperty hp = (HttpResponseMessageProperty)msg.Properties[pname];
				string contentType = hp.Headers["Content-Type"];
				if(contentType != null)
					Context.Response.ContentType = contentType;
				Context.Response.Headers.Add(hp.Headers);
				if(hp.StatusCode != default(System.Net.HttpStatusCode))
					Context.Response.StatusCode = (int)hp.StatusCode;
				Context.Response.StatusDescription = hp.StatusDescription;
				if(hp.SuppressEntityBody)
					suppressEntityBody = true;
			}
			if(msg.IsFault)
				Context.Response.StatusCode = 500;
			if(!suppressEntityBody)
			{
				Context.Response.SetLength(ms.Length);
				Context.Response.OutputStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
				Context.Response.OutputStream.Flush();
			}
			else
				Context.Response.SuppressContent = true;
		}
	}
}
