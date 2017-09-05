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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação do manipulador para get.
	/// </summary>
	class RequestHandler : RequestHandlerBase
	{
		private const string HTTP_DATEFORMAT = "{0:ddd,' 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'}";

		private Web.IHttpApplication _application;

		private static readonly System.Reflection.FieldInfo _responseHttpWriterField;

		private static readonly System.Reflection.MethodInfo _responseInitResponseWriterMethod;

		private static readonly System.Reflection.MethodInfo _responseReportRuntimeErrorMethod;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="env"></param>
		/// <param name="routes"></param>
		public RequestHandler(IDictionary<string, object> env, Web.IHttpApplication application) : base(env)
		{
			_application = application;
		}

		/// <summary>
		/// Construtor geral
		/// </summary>
		static RequestHandler()
		{
			_responseHttpWriterField = typeof(System.Web.HttpResponse).GetField("_httpWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			_responseInitResponseWriterMethod = typeof(System.Web.HttpResponse).GetMethod("InitResponseWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			_responseReportRuntimeErrorMethod = typeof(System.Web.HttpResponse).GetMethod("ReportRuntimeError", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		}

		/// <summary>
		/// Inicializa o escritor da resposta.
		/// </summary>
		/// <param name="response"></param>
		private void InitResponseWriter(System.Web.HttpResponse response)
		{
			if(_responseHttpWriterField != null && _responseInitResponseWriterMethod != null)
			{
				_responseHttpWriterField.SetValue(response, null);
				_responseInitResponseWriterMethod.Invoke(response, null);
			}
		}

		/// <summary>
		/// Finaliza a requisição.
		/// </summary>
		/// <param name="wr"></param>
		/// <param name="context"></param>
		/// <param name="e"></param>
		private bool FinishRequest(System.Web.HttpWorkerRequest wr, System.Web.HttpContext context, Exception e)
		{
			if(_responseReportRuntimeErrorMethod != null)
			{
				try
				{
					_responseReportRuntimeErrorMethod.Invoke(context.Response, new object[] {
						e,
						true,
						false
					});
				}
				catch(Exception)
				{
					try
					{
						_responseReportRuntimeErrorMethod.Invoke(context.Response, new object[] {
							e,
							false,
							false
						});
					}
					catch
					{
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Processa a requisição.
		/// </summary>
		/// <returns></returns>
		public async override Task<object> Handle()
		{
			var context = new Microsoft.Owin.OwinContext(Environment);
			var wr = new HttpWorkerRequestWrapper(context);
			var httpContext = new System.Web.HttpContext(wr);
			httpContext.User = System.Threading.Thread.CurrentPrincipal;
			httpContext.Request.Browser = new System.Web.HttpBrowserCapabilities() {
				Capabilities = new System.Collections.Hashtable()
			};
			System.Web.HttpContext.Current = httpContext;
			System.Web.HttpContextBase httpContextBase = null;
			httpContextBase = new System.Web.HttpContextWrapper(httpContext);
			httpContext.Request.Form.HasKeys();
			HttpContext.Current = new HttpContext(context, httpContextBase.Request, httpContextBase.Response);
			var route = base.GetRoute(httpContextBase);
			context.Response.Headers.Add("Server", new[] {
				"Colosoft 1.0"
			});
			if(_application != null)
				_application.OnBeginRequest(context, EventArgs.Empty);
			if(_application != null)
				_application.OnAuthenticateRequest(context, EventArgs.Empty);
			httpContext.User = System.Threading.Thread.CurrentPrincipal;
			System.Web.Routing.IRouteHandler routeHandler = null;
			if(route != null)
			{
				routeHandler = route.RouteHandler;
				if(routeHandler == null)
					throw new InvalidOperationException("NoRouteHandler");
			}
			if(route != null && !(routeHandler is System.Web.Routing.StopRoutingHandler))
			{
				var requestContext = new System.Web.Routing.RequestContext(httpContextBase, route);
				var httpHandler = route.RouteHandler.GetHttpHandler(requestContext);
				if(httpHandler == null)
					throw new InvalidOperationException(string.Format("NoHttpHandler {0}", routeHandler.GetType()));
				try
				{
					if(httpHandler is System.Web.IHttpAsyncHandler)
					{
						var asyncHandler = (System.Web.IHttpAsyncHandler)httpHandler;
						await Task.Factory.FromAsync<System.Web.HttpContext>(asyncHandler.BeginProcessRequest, asyncHandler.EndProcessRequest, httpContext, null);
					}
					else
						httpHandler.ProcessRequest(httpContext);
				}
				catch(Exception ex)
				{
					if(!FinishRequest(wr, httpContext, ex))
					{
						wr.SendStatus(400, "Bad Request");
						wr.SendKnownResponseHeader(12, "text/html; charset=utf-8");
						byte[] data = Encoding.ASCII.GetBytes("<html><body>Bad Request </ br>" + ex.ToString() + "</body></html>");
						wr.SendResponseFromMemory(data, data.Length);
						wr.FlushResponse(true);
					}
				}
				if(httpContextBase.Response.StatusCode == 401)
					httpContextBase.Response.Redirect(string.Format("/login?returnurl={0}", context.Request.Uri.AbsolutePath));
			}
			else
			{
				var path = this.RequestPath;
				if(System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(path))
				{
					var file = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(path);
					var index = path.LastIndexOf('.');
					if(index >= 0)
					{
						var extension = path.Substring(index);
						var contentType = Colosoft.Web.ExtendedHtmlUtility.TranslateContentType(extension);
						if(!string.IsNullOrEmpty(contentType))
						{
							httpContextBase.Response.ContentType = contentType;
						}
					}
					var cacheControl = httpContextBase.Request.Headers["Cache-Control"];
					var ifModifiedSince = httpContextBase.Request.Headers["If-Modified-Since"];
					DateTime ifModifiedSinceDate = DateTime.Now;
					if(!string.IsNullOrEmpty(ifModifiedSince) && DateTime.TryParseExact(ifModifiedSince, "ddd,' 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out ifModifiedSinceDate))
					{
						if(file is Web.Hosting.IPhysicalFileInfo && ((Web.Hosting.IPhysicalFileInfo)file).LastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss") == ifModifiedSinceDate.ToString("yyyy/MM/dd HH:mm:ss"))
						{
							httpContextBase.Response.StatusCode = 304;
							httpContextBase.Response.Flush();
							if(_application != null)
								_application.OnEndRequest(context, EventArgs.Empty);
							return await Task.FromResult<object>(null);
						}
					}
					httpContextBase.Response.AddHeader("Age", "25000");
					httpContextBase.Response.AddHeader("Cache-Control", "max-age=10000, public");
					httpContextBase.Response.AddHeader("Date", string.Format(System.Globalization.CultureInfo.InvariantCulture, HTTP_DATEFORMAT, DateTimeOffset.UtcNow));
					httpContextBase.Response.AddHeader("Expires", string.Format(System.Globalization.CultureInfo.InvariantCulture, HTTP_DATEFORMAT, DateTimeOffset.UtcNow.AddYears(1)));
					httpContextBase.Response.AddHeader("Vary", "*");
					if(file is Web.Hosting.IPhysicalFileInfo)
					{
						var physicalFile = (Web.Hosting.IPhysicalFileInfo)file;
						httpContextBase.Response.AddHeader("Content-Length", physicalFile.ContentLength.ToString());
						httpContextBase.Response.AddHeader("Last-Modified", string.Format(System.Globalization.CultureInfo.InvariantCulture, HTTP_DATEFORMAT, physicalFile.LastWriteTimeUtc));
					}
					httpContextBase.Response.Flush();
					if(file != null)
					{
						System.IO.Stream outstream = httpContextBase.Response.OutputStream;
						var read = 0;
						byte[] buffer = new byte[81920];
						using (var inputStream = file.Open())
						{
							while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
								outstream.Write(buffer, 0, read);
						}
						outstream.Flush();
					}
				}
				else
				{
					httpContextBase.Response.StatusCode = 404;
					var virtualPath = "~/Views/404.html";
					if(System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
					{
						var virtualFile = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);
						using (var stream = virtualFile.Open())
						{
							var reader = new System.IO.StreamReader(stream, httpContextBase.Response.ContentEncoding);
							var writer = new System.IO.StreamWriter(httpContextBase.Response.OutputStream, httpContextBase.Response.ContentEncoding);
							writer.Write(reader.ReadToEnd());
							writer.Flush();
						}
					}
				}
			}
			httpContextBase.Response.Flush();
			if(_application != null)
				_application.OnEndRequest(context, EventArgs.Empty);
			return Task.FromResult<object>(null);
		}
	}
}
