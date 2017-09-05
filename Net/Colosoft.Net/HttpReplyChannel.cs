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
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel.Channels.Http
{
	internal class HttpReplyChannel : InternalReplyChannelBase
	{
		HttpChannelListener<IReplyChannel> source;

		RequestContext reqctx;

		SecurityTokenAuthenticator security_token_authenticator;

		SecurityTokenResolver security_token_resolver;

		public HttpReplyChannel(HttpChannelListener<IReplyChannel> listener) : base(listener)
		{
			this.source = listener;
			if(listener.SecurityTokenManager != null)
			{
				var str = new SecurityTokenRequirement() {
					TokenType = SecurityTokenTypes.UserName
				};
				security_token_authenticator = listener.SecurityTokenManager.CreateSecurityTokenAuthenticator(str, out security_token_resolver);
			}
		}

		internal HttpChannelListener<IReplyChannel> Source
		{
			get
			{
				return source;
			}
		}

		public MessageEncoder Encoder
		{
			get
			{
				return source.MessageEncoder;
			}
		}

		internal MessageVersion MessageVersion
		{
			get
			{
				return source.MessageEncoder.MessageVersion;
			}
		}

		public override RequestContext ReceiveRequest(TimeSpan timeout)
		{
			RequestContext ctx;
			if(!TryReceiveRequest(timeout, out ctx))
				throw new TimeoutException();
			return ctx;
		}

		protected override void OnOpen(TimeSpan timeout)
		{
		}

		protected override void OnAbort()
		{
			AbortConnections(TimeSpan.Zero);
			base.OnAbort();
		}

		public override bool CancelAsync(TimeSpan timeout)
		{
			AbortConnections(timeout);
			return base.CancelAsync(timeout);
		}

		void AbortConnections(TimeSpan timeout)
		{
			if(reqctx != null)
				reqctx.Close(timeout);
		}

		bool close_started;

		object close_lock = new object();

		protected override void OnClose(TimeSpan timeout)
		{
			lock (close_lock)
			{
				if(close_started)
					return;
				close_started = true;
			}
			DateTime start = DateTime.Now;
			AbortConnections(timeout - (DateTime.Now - start));
			base.OnClose(timeout - (DateTime.Now - start));
		}

		protected string GetHeaderItem(string raw)
		{
			if(raw == null || raw.Length == 0)
				return raw;
			switch(raw[0])
			{
			case '\'':
			case '"':
				if(raw[raw.Length - 1] == raw[0])
					return raw.Substring(1, raw.Length - 2);
				break;
			}
			return raw;
		}

		protected HttpRequestMessageProperty CreateRequestProperty(HttpContextInfo ctxi)
		{
			var query = ctxi.Request.Url.Query;
			var prop = new HttpRequestMessageProperty();
			prop.Method = ctxi.Request.HttpMethod;
			prop.QueryString = query.StartsWith("?") ? query.Substring(1) : query;
			prop.Headers.Add(ctxi.Request.Headers);
			return prop;
		}

		public override bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
		{
			context = null;
			HttpContextInfo ctxi;
			if(!source.ListenerManager.TryDequeueRequest(null, timeout, out ctxi))
				return false;
			if(ctxi == null)
				return true;
			if(source.Source.AuthenticationScheme != AuthenticationSchemes.Anonymous)
			{
				if(security_token_authenticator != null)
					try
					{
						security_token_authenticator.ValidateToken(new UserNameSecurityToken(ctxi.User, ctxi.Password));
					}
					catch(Exception)
					{
						ctxi.ReturnUnauthorized();
					}
				else
				{
					ctxi.ReturnUnauthorized();
				}
			}
			Message msg = null;
			if(ctxi.Request.HttpMethod == "POST")
				msg = CreatePostMessage(ctxi);
			else if(ctxi.Request.HttpMethod == "GET")
				msg = Message.CreateMessage(MessageVersion.None, null);
			if(msg == null)
				return false;
			if(msg.Headers.To == null)
				msg.Headers.To = ctxi.Request.Url;
			msg.Properties.Add("Via", LocalAddress.Uri);
			msg.Properties.Add(HttpRequestMessageProperty.Name, CreateRequestProperty(ctxi));
			context = new HttpRequestContext(this, ctxi, msg);
			reqctx = context;
			return true;
		}

		protected Message CreatePostMessage(HttpContextInfo ctxi)
		{
			if(ctxi.Response.StatusCode != 200)
			{
				ctxi.Close();
				return null;
			}
			if(!Encoder.IsContentTypeSupported(ctxi.Request.ContentType))
			{
				ctxi.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
				ctxi.Response.StatusDescription = String.Format("Expected content-type '{0}' but got '{1}'", Encoder.ContentType, ctxi.Request.ContentType);
				ctxi.Close();
				return null;
			}
			int maxSizeOfHeaders = 0x10000;
			#if false
						Stream stream = ctxi.Request.InputStream;
			if (source.Source.TransferMode == TransferMode.Buffered) {
				if (ctxi.Request.ContentLength64 <= 0)
					throw new ArgumentException ("This HTTP channel is configured to use buffered mode, and thus expects Content-Length sent to the listener");
				long size = 0;
				var ms = new MemoryStream ();
				var buf = new byte [0x1000];
				while (size < ctxi.Request.ContentLength64) {
					if ((size += stream.Read (buf, 0, 0x1000)) > source.Source.MaxBufferSize)
						throw new QuotaExceededException ("Message quota exceeded");
					ms.Write (buf, 0, (int) (size - ms.Length));
				}
				ms.Position = 0;
				stream = ms;
			}

			var msg = Encoder.ReadMessage (
				stream, maxSizeOfHeaders, ctxi.Request.ContentType);
#else
			var msg = Encoder.ReadMessage(ctxi.Request.InputStream, maxSizeOfHeaders, ctxi.Request.ContentType);
			#endif
			if(MessageVersion.Envelope.Equals(EnvelopeVersion.Soap11) || MessageVersion.Addressing.Equals(AddressingVersion.None))
			{
				string action = GetHeaderItem(ctxi.Request.Headers["SOAPAction"]);
				if(action != null)
				{
					if(action.Length > 2 && action[0] == '"' && action[action.Length] == '"')
						action = action.Substring(1, action.Length - 2);
					msg.Headers.Action = action;
				}
			}
			msg.Properties.Add(RemoteEndpointMessageProperty.Name, new RemoteEndpointMessageProperty(ctxi.Request.ClientIPAddress, ctxi.Request.ClientPort));
			return msg;
		}

		public override bool WaitForRequest(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}
	}
}
