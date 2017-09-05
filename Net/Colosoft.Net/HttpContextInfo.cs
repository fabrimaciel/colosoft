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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Colosoft.ServiceModel.Channels.Http
{
	abstract class HttpContextInfo
	{
		public abstract HttpRequestInfo Request
		{
			get;
		}

		public abstract HttpResponseInfo Response
		{
			get;
		}

		public abstract string User
		{
			get;
		}

		public abstract string Password
		{
			get;
		}

		public abstract void ReturnUnauthorized();

		public void Abort()
		{
			Response.Abort();
			OnContextClosed();
		}

		public void Close()
		{
			Response.Close();
			OnContextClosed();
		}

		protected virtual void OnContextClosed()
		{
		}
	}
	class HttpStandaloneContextInfo : HttpContextInfo
	{
		public HttpStandaloneContextInfo(HttpListenerContext ctx)
		{
			this.ctx = ctx;
			request = new HttpStandaloneRequestInfo(ctx.Request);
			response = new HttpStandaloneResponseInfo(ctx.Response);
		}

		HttpListenerContext ctx;

		HttpStandaloneRequestInfo request;

		HttpStandaloneResponseInfo response;

		public HttpListenerContext Source
		{
			get
			{
				return ctx;
			}
		}

		public override HttpRequestInfo Request
		{
			get
			{
				return request;
			}
		}

		public override HttpResponseInfo Response
		{
			get
			{
				return response;
			}
		}

		public override string User
		{
			get
			{
				return ctx.User != null ? ((HttpListenerBasicIdentity)ctx.User.Identity).Name : null;
			}
		}

		public override string Password
		{
			get
			{
				return ctx.User != null ? ((HttpListenerBasicIdentity)ctx.User.Identity).Password : null;
			}
		}

		public override void ReturnUnauthorized()
		{
			ctx.Response.StatusCode = 401;
		}
	}
	class AspNetHttpContextInfo : HttpContextInfo
	{
		public AspNetHttpContextInfo(SvcHttpHandler handler, HttpContext ctx)
		{
			this.ctx = ctx;
			this.handler = handler;
			this.request = new AspNetHttpRequestInfo(ctx.Request);
			this.response = new AspNetHttpResponseInfo(ctx.Response);
		}

		HttpContext ctx;

		SvcHttpHandler handler;

		AspNetHttpRequestInfo request;

		AspNetHttpResponseInfo response;

		public HttpContext Source
		{
			get
			{
				return ctx;
			}
		}

		public override HttpRequestInfo Request
		{
			get
			{
				return request;
			}
		}

		public override HttpResponseInfo Response
		{
			get
			{
				return response;
			}
		}

		public override string User
		{
			get
			{
				return ctx.User != null ? ((GenericIdentity)ctx.User.Identity).Name : null;
			}
		}

		public override string Password
		{
			get
			{
				return null;
			}
		}

		public override void ReturnUnauthorized()
		{
			ctx.Response.StatusCode = 401;
		}

		protected override void OnContextClosed()
		{
			handler.EndHttpRequest(ctx);
		}
	}
	abstract class HttpRequestInfo
	{
		public abstract long ContentLength64
		{
			get;
		}

		public abstract NameValueCollection QueryString
		{
			get;
		}

		public abstract NameValueCollection Headers
		{
			get;
		}

		public abstract Uri Url
		{
			get;
		}

		public abstract string ContentType
		{
			get;
		}

		public abstract string HttpMethod
		{
			get;
		}

		public abstract System.IO.Stream InputStream
		{
			get;
		}

		public abstract string ClientIPAddress
		{
			get;
		}

		public abstract int ClientPort
		{
			get;
		}
	}
	class HttpStandaloneRequestInfo : HttpRequestInfo
	{
		public HttpStandaloneRequestInfo(HttpListenerRequest request)
		{
			this.req = request;
		}

		HttpListenerRequest req;

		public override long ContentLength64
		{
			get
			{
				return req.ContentLength64;
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return req.QueryString;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return req.Headers;
			}
		}

		public override Uri Url
		{
			get
			{
				return req.Url;
			}
		}

		public override string ContentType
		{
			get
			{
				return req.ContentType;
			}
		}

		public override string HttpMethod
		{
			get
			{
				return req.HttpMethod;
			}
		}

		public override System.IO.Stream InputStream
		{
			get
			{
				return req.InputStream;
			}
		}

		public override string ClientIPAddress
		{
			get
			{
				return req.RemoteEndPoint.Address.ToString();
			}
		}

		public override int ClientPort
		{
			get
			{
				return req.RemoteEndPoint.Port;
			}
		}
	}
	class AspNetHttpRequestInfo : HttpRequestInfo
	{
		public AspNetHttpRequestInfo(HttpRequest request)
		{
			this.req = request;
		}

		HttpRequest req;

		public override long ContentLength64
		{
			get
			{
				return req.ContentLength;
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return req.QueryString;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return req.Headers;
			}
		}

		public override Uri Url
		{
			get
			{
				return req.Url;
			}
		}

		public override string ContentType
		{
			get
			{
				return req.ContentType;
			}
		}

		public override string HttpMethod
		{
			get
			{
				return req.HttpMethod;
			}
		}

		public override System.IO.Stream InputStream
		{
			get
			{
				return req.InputStream;
			}
		}

		public override string ClientIPAddress
		{
			get
			{
				return req.UserHostAddress;
			}
		}

		public override int ClientPort
		{
			get
			{
				return -1;
			}
		}
	}
	abstract class HttpResponseInfo
	{
		public abstract string ContentType
		{
			get;
			set;
		}

		public abstract NameValueCollection Headers
		{
			get;
		}

		public abstract System.IO.Stream OutputStream
		{
			get;
		}

		public abstract int StatusCode
		{
			get;
			set;
		}

		public abstract string StatusDescription
		{
			get;
			set;
		}

		public abstract void Abort();

		public abstract void Close();

		public abstract void SetLength(long value);

		public virtual bool SuppressContent
		{
			get;
			set;
		}
	}
	class HttpStandaloneResponseInfo : HttpResponseInfo
	{
		public HttpStandaloneResponseInfo(HttpListenerResponse response)
		{
			this.res = response;
		}

		HttpListenerResponse res;

		public override string ContentType
		{
			get
			{
				return res.ContentType;
			}
			set
			{
				res.ContentType = value;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return res.Headers;
			}
		}

		public override int StatusCode
		{
			get
			{
				return res.StatusCode;
			}
			set
			{
				res.StatusCode = value;
			}
		}

		public override string StatusDescription
		{
			get
			{
				return res.StatusDescription;
			}
			set
			{
				res.StatusDescription = value;
			}
		}

		public override System.IO.Stream OutputStream
		{
			get
			{
				return res.OutputStream;
			}
		}

		public override void Abort()
		{
			res.Abort();
		}

		public override void Close()
		{
			res.Close();
		}

		public override void SetLength(long value)
		{
			res.ContentLength64 = value;
		}
	}
	class AspNetHttpResponseInfo : HttpResponseInfo
	{
		public AspNetHttpResponseInfo(HttpResponse response)
		{
			this.res = response;
		}

		HttpResponse res;

		public override bool SuppressContent
		{
			get
			{
				return res.SuppressContent;
			}
			set
			{
				res.SuppressContent = value;
			}
		}

		public override string ContentType
		{
			get
			{
				return res.ContentType;
			}
			set
			{
				res.ContentType = value;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return res.Headers;
			}
		}

		public override int StatusCode
		{
			get
			{
				return res.StatusCode;
			}
			set
			{
				res.StatusCode = value;
			}
		}

		public override string StatusDescription
		{
			get
			{
				return res.StatusDescription;
			}
			set
			{
				res.StatusDescription = value;
			}
		}

		public override System.IO.Stream OutputStream
		{
			get
			{
				return res.OutputStream;
			}
		}

		public override void Abort()
		{
			res.End();
		}

		public override void Close()
		{
		}

		public override void SetLength(long value)
		{
			res.AddHeader("Content-Length", value.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
	}
}
