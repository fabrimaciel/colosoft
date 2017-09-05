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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Web.Configuration;
using System.Collections;
using System.Threading;
using System.Web.Security;
using System.Net;

namespace Colosoft.WebControls.Route
{
	internal class HttpAsyncResult : IAsyncResult
	{
		private object _asyncState;

		private AsyncCallback _callback;

		private bool _completed;

		private bool _completedSynchronously;

		private Exception _error;

		private object _result;

		private RequestNotificationStatus _status;

		public object AsyncState
		{
			get
			{
				return _asyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return null;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return _completedSynchronously;
			}
		}

		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return _completed;
			}
		}

		public RequestNotificationStatus Status
		{
			get
			{
				return _status;
			}
		}

		/// <summary>
		/// Construtor padrao.
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		public HttpAsyncResult(AsyncCallback cb, object state)
		{
			this._callback = cb;
			this._asyncState = state;
			this._status = RequestNotificationStatus.Continue;
		}

		public HttpAsyncResult(AsyncCallback cb, object state, bool completed, object result, Exception error)
		{
			this._callback = cb;
			this._asyncState = state;
			this._completed = completed;
			this._completedSynchronously = completed;
			this._result = result;
			this._error = error;
			this._status = RequestNotificationStatus.Continue;
			if(this._completed && (this._callback != null))
			{
				this._callback(this);
			}
		}

		public void Complete(bool synchronous, object result, Exception error)
		{
			this.Complete(synchronous, result, error, RequestNotificationStatus.Continue);
		}

		public void Complete(bool synchronous, object result, Exception error, RequestNotificationStatus status)
		{
			this._completed = true;
			this._completedSynchronously = synchronous;
			this._result = result;
			this._error = error;
			this._status = status;
			if(this._callback != null)
			{
				this._callback(this);
			}
		}

		public object End()
		{
			if(this._error != null)
			{
				throw new HttpException(null, this._error);
			}
			return this._result;
		}

		public void SetComplete()
		{
			this._completed = true;
		}
	}
	/// <summary>
	/// Representa os dados que estao sendo processados.
	/// </summary>
	internal class RequestData
	{
		public IHttpHandler HttpHandler
		{
			get;
			set;
		}

		public string OriginalPath
		{
			get;
			set;
		}

		public System.Collections.Specialized.NameValueCollection OriginalQueryStringParameters
		{
			get;
			set;
		}

		public System.Collections.Specialized.NameValueCollection RouteParameters
		{
			get;
			set;
		}

		/// <summary>
		/// Rota que está sendo processada.
		/// </summary>
		public RouteInfo Route
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Classe para retornar objetos não compilados.
	/// </summary>
	class NotCompiledRedirect : IHttpHandler
	{
		/// <summary>
		/// Nome do arquivo não compilado que será carregado.
		/// </summary>
		public string Filename
		{
			get;
			set;
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			if(System.IO.File.Exists(Filename))
			{
				var ext = System.IO.Path.GetExtension(Filename);
				context.Response.ContentType = ExtendedHtmlUtility.TranslateContentType(ext);
				context.Response.WriteFile(Filename);
			}
			else
				RouteModule.SendAccessIsDaniedResponse(context.Response);
		}
	}
	/// <summary>
	/// Modulo responsavel pelo gerenciamento das rotas.
	/// </summary>
	public class RouteModule : IHttpModule
	{
		private BeginEventHandler _beginEventHandler;

		private EndEventHandler _sessionEndEventHandler;

		private EventHandler _sessionOnReleaseState;

		private EventHandler _sessionOnEndRequest;

		private EventHandler _formsAuthOnEnter;

		private EventHandler _formsAuthOnLeave;

		private EventHandler _windowsAuthOnEnter;

		private static readonly object _requestDataKey;

		private static readonly object _requestCacheItemKey;

		static RouteModule()
		{
			_requestDataKey = new object();
			_requestCacheItemKey = new object();
		}

		/// <summary>
		/// Envia uma respota de acesso negado.
		/// </summary>
		/// <param name="response"></param>
		internal static void SendAccessUnauthorizedResponse(HttpResponse response)
		{
			StringBuilder message = new StringBuilder();
			message.Append("<HTML><BODY>");
			message.Append("<p> Error message 401: Unauthorized.</p>");
			message.Append("</BODY></HTML>");
			var message401 = message.ToString();
			response.StatusCode = (int)HttpStatusCode.Unauthorized;
			response.StatusDescription = "401 Unauthorized";
			System.Text.Encoding encoding = response.ContentEncoding;
			if(encoding == null)
			{
				encoding = System.Text.Encoding.UTF8;
				response.ContentEncoding = encoding;
			}
			byte[] buffer = encoding.GetBytes(message401);
			System.IO.Stream stream = response.OutputStream;
			stream.Write(buffer, 0, buffer.Length);
			response.End();
		}

		/// <summary>
		/// Envia uma respota de acesso negado.
		/// </summary>
		/// <param name="response"></param>
		internal static void SendAccessIsDaniedResponse(HttpResponse response)
		{
			StringBuilder message = new StringBuilder();
			message.Append("<HTML><BODY>");
			message.Append("<p> Error message 403: Access is denied.</p>");
			message.Append("</BODY></HTML>");
			var message403 = message.ToString();
			response.StatusCode = (int)HttpStatusCode.Forbidden;
			response.StatusDescription = "403 Forbidden";
			System.Text.Encoding encoding = response.ContentEncoding;
			if(encoding == null)
			{
				encoding = System.Text.Encoding.UTF8;
				response.ContentEncoding = encoding;
			}
			byte[] buffer = encoding.GetBytes(message403);
			System.IO.Stream stream = response.OutputStream;
			stream.Write(buffer, 0, buffer.Length);
			response.End();
		}

		/// <summary>
		/// Esse método recorre ao provider para recuperar a coleção de regras definidas e, em seguida, invoca o método IsUserAllowed da 
		/// classe DBAuthorizationRuleCollection informando os mesmos parâmetros.
		/// </summary>
		/// <param name="principal">usuário corrente</param>
		/// <param name="requestedPath">a página (path) requisitada</param>
		/// <param name="verb">verbo (GET ou POST)</param>
		/// <returns> retorna um valor booleano indicando se a autorização deve ou não ser avaliada.</returns>
		internal static bool IsUserAllowed(System.Security.Principal.IPrincipal principal, string requestedPath, string verb)
		{
			return Security.Authorization.GetAllRulesByPath(requestedPath).IsUserAllowed(principal, requestedPath, verb);
		}

		/// <summary>
		/// Libera a somente leitura da lista de QueryString.
		/// </summary>
		/// <param name="collection"></param>
		private static void MakeReadWriteCollection(object collection)
		{
			System.Reflection.MethodInfo mi = collection.GetType().GetMethod("MakeReadWrite", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			mi.Invoke(collection, null);
		}

		private void PostMapRequestHandler(HttpContext context)
		{
			RequestData data = (RequestData)context.Items[_requestDataKey];
			if(data != null)
			{
				context.RewritePath(data.OriginalPath);
				context.Handler = data.HttpHandler;
				RewriterUtils.MakeReadWriteCollection(context.Request.Form);
				RewriterUtils.MakeReadWriteCollection(context.Request.QueryString);
				foreach (var i in data.RouteParameters.AllKeys)
					context.Request.Form.Add(i, data.RouteParameters[i]);
				if(data.Route != null)
					data.Route.Load();
				RewriterUtils.MakeReadOnlyCollection(context.Request.QueryString);
				RewriterUtils.MakeReadOnlyCollection(context.Request.Form);
			}
		}

		private void PostResolveRequestCache(HttpContext context)
		{
			System.Diagnostics.Debug.Write("\r\n PostResolveRequestCache    -- " + context.Request.Path);
			var cacheItem = context.Items[_requestCacheItemKey] as LoadRouteResult;
			RouteInfoNavigate nav = cacheItem != null ? cacheItem.Navigate : null;
			if(nav != null)
			{
				string relative = context.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + context.Request.PathInfo;
				var destinationPage = nav.Info.DestinationPage;
				string[] partsRelative = relative.Split('/');
				string[] partsNav = nav.FullName.Split('/');
				string[] extensions = nav.Info.Path.Split(',');
				string pageName = null;
				string pageExtension = null;
				string part = partsRelative[partsRelative.Length - 1];
				if(string.IsNullOrEmpty(part))
				{
					if(Array.FindIndex(extensions, delegate(string s) {
						return s == "*";
					}) < 0)
						return;
					pageName = "";
				}
				else
				{
					System.IO.FileInfo fi = new System.IO.FileInfo(partsRelative[partsRelative.Length - 1]);
					pageExtension = fi.Extension;
					if(partsRelative[partsRelative.Length - 1].IndexOf('.') >= 0)
					{
						int i = 0;
						if(extensions.Length >= 1 && extensions[0] != "*")
						{
							for(i = 0; i < extensions.Length; i++)
								if(string.Compare(fi.Extension.Substring(1), extensions[i].Trim(), true) == 0)
									break;
							if(i == extensions.Length)
								return;
						}
						if(fi.Extension.Length > 0)
							pageName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
						else
							pageName = fi.Name;
					}
					else
					{
						int i = 0;
						for(i = 0; i < extensions.Length; i++)
							if(extensions[i].Trim() == "*")
								break;
						if(i == extensions.Length)
							return;
						pageName = fi.Name;
					}
				}
				var routeParameters = new System.Collections.Specialized.NameValueCollection();
				int count = (pageName == null ? partsRelative.Length : (partsRelative.Length - 1));
				for(int i = partsNav.Length; i < count; i++)
					routeParameters.Add("param" + (i - partsNav.Length), partsRelative[i]);
				if(pageName != null)
					routeParameters.Add("pageName", pageName);
				if(pageExtension != null)
					routeParameters.Add("pageExtension", pageExtension);
				if(nav.ComplexPartsCount > 0)
				{
					try
					{
						var match = System.Text.RegularExpressions.Regex.Match(relative, nav.ComplexRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
						if(match.Success)
							for(int i = 0; i < nav.ComplexPartsCount; i++)
								routeParameters.Add("complex" + i, match.Groups["complex" + i].Value);
					}
					catch(Exception ex)
					{
						throw new Exception(string.Format("Fail on process complex route for {0}.", relative), ex);
					}
				}
				var originalQueryStringParameters = new System.Collections.Specialized.NameValueCollection();
				foreach (var i in context.Request.QueryString.AllKeys)
					originalQueryStringParameters.Add(i, context.Request.QueryString[i]);
				var qsPos = destinationPage.IndexOf('?');
				if(qsPos >= 0)
				{
					var queryStringItems = destinationPage.Substring(qsPos + 1).Split('&');
					foreach (var i in queryStringItems)
					{
						var pos1 = i.IndexOf('=');
						if(pos1 > 0)
							routeParameters.Add(i.Substring(0, pos1), i.Substring(pos1 + 1));
					}
					destinationPage = destinationPage.Substring(0, qsPos);
				}
				var virtualPath = "";
				RouteActionResponse actionResponse = null;
				var isCompiled = nav.Info.Compiled;
				var startPos = 0;
				if(nav.Info.IsDirectory)
				{
					var relativeDir = relative.Length > cacheItem.FormattedLocation.Length ? relative.Substring(cacheItem.FormattedLocation.Length) : relative;
					startPos = 0;
					if(!string.IsNullOrEmpty(relativeDir))
						while (startPos < relativeDir.Length && relativeDir[startPos] == '/' || relativeDir[startPos] == '\\')
							startPos++;
					destinationPage += (startPos > 0 ? relativeDir.Substring(startPos) : relativeDir);
				}
				startPos = 0;
				if(!string.IsNullOrEmpty(destinationPage))
					while (startPos < destinationPage.Length && destinationPage[startPos] == '/' || destinationPage[startPos] == '\\')
						startPos++;
				destinationPage = (startPos > 0 ? destinationPage.Substring(startPos) : destinationPage);
				if(!isCompiled)
				{
					virtualPath = "~/" + destinationPage;
					actionResponse = nav.Info.PreLoad(new RouteActionRequest {
						DestinationVirtualPath = virtualPath,
						OriginalQueryString = originalQueryStringParameters,
						RouteParameters = routeParameters,
						Context = context
					});
					if(actionResponse != null && !string.IsNullOrEmpty(actionResponse.RedirectVirtualPath))
					{
						isCompiled = actionResponse.Compiled;
						virtualPath = actionResponse.RedirectVirtualPath;
					}
				}
				else
				{
					virtualPath = "~/" + destinationPage;
					actionResponse = nav.Info.PreLoad(new RouteActionRequest {
						DestinationVirtualPath = virtualPath,
						OriginalQueryString = originalQueryStringParameters,
						RouteParameters = routeParameters,
						Context = context
					});
					if(actionResponse != null && !string.IsNullOrEmpty(actionResponse.RedirectVirtualPath))
					{
						isCompiled = actionResponse.Compiled;
						virtualPath = actionResponse.RedirectVirtualPath;
					}
				}
				IHttpHandler ret = null;
				var inputFile = context.Server.MapPath(virtualPath);
				if(actionResponse != null && actionResponse.RouteHandler != null)
					ret = actionResponse.RouteHandler;
				else if(!isCompiled)
					ret = new NotCompiledRedirect {
						Filename = inputFile
					};
				else if(string.Compare(System.IO.Path.GetExtension(inputFile), ".asmx", true) == 0)
					ret = (new System.Web.Services.Protocols.WebServiceHandlerFactory()).GetHandler(context, "*", virtualPath, inputFile);
				else
					ret = PageParser.GetCompiledPageInstance(virtualPath, inputFile, context);
				RequestData data2 = new RequestData();
				data2.OriginalPath = context.Request.Path;
				data2.OriginalQueryStringParameters = originalQueryStringParameters;
				data2.HttpHandler = ret;
				data2.RouteParameters = routeParameters;
				data2.Route = nav.Info;
				context.Items[_requestDataKey] = data2;
				context.RewritePath("~/Route.axd");
			}
		}

		private void OnApplicationPostMapRequestHandler(object sender, EventArgs e)
		{
			var application = (HttpApplication)sender;
			this.PostMapRequestHandler(application.Context);
			if(_formsAuthOnEnter != null)
				_formsAuthOnEnter(application, e);
			if(_windowsAuthOnEnter != null)
				_windowsAuthOnEnter(application, e);
			if(Security.Authorization.Provider == null)
			{
				var authorizationSection = (AuthorizationSection)WebConfigurationManager.GetSection("system.web/authorization");
				var rules = authorizationSection.Rules;
				bool free = (rules.Count == 0);
				var danied = false;
				var found = false;
				foreach (AuthorizationRule au in rules)
				{
					if(au.Action == AuthorizationRuleAction.Allow)
					{
						foreach (string userName in au.Users)
						{
							if(userName == "*" || userName == "?" || (application.Context.User != null && application.Context.User.Identity != null && application.Context.User.Identity.Name == userName))
							{
								free = true;
								found = true;
								break;
							}
						}
						if(!free)
							foreach (var role in au.Roles)
							{
								if(role == "*" || role == "?" || application.Context.User != null && Roles.IsUserInRole(role))
								{
									free = true;
									found = true;
									break;
								}
							}
					}
					else if(!free && au.Action == AuthorizationRuleAction.Deny)
					{
						foreach (string userName in au.Users)
						{
							if(userName == "?")
							{
								danied = false;
								free = false;
								found = true;
								break;
							}
							else if(userName == "*" || (application.Context.User != null && application.Context.User.Identity != null && application.Context.User.Identity.Name == userName))
							{
								danied = true;
								free = false;
								found = true;
								break;
							}
						}
						if(free)
							foreach (var role in au.Roles)
							{
								if(role == "?")
								{
									danied = false;
									free = false;
									found = true;
									break;
								}
								if(role == "*" || application.Context.User != null && !Roles.IsUserInRole(role))
								{
									danied = true;
									free = false;
									found = true;
									break;
								}
							}
					}
					if(found)
						break;
				}
				if(!danied && !application.Context.SkipAuthorization && !free && (application.Context.User == null || application.Context.User.Identity == null || !application.Context.User.Identity.IsAuthenticated))
				{
					FormsAuthentication.RedirectToLoginPage();
				}
				else if(danied)
				{
					SendAccessIsDaniedResponse(application.Context.Response);
				}
			}
			else
			{
				if(!application.Context.SkipAuthorization)
				{
					if(!IsUserAllowed(application.Context.User, application.Context.Request.AppRelativeCurrentExecutionFilePath, application.Context.Request.RequestType))
					{
						var rule = application.Context.Items[Route.Security.AuthorizationRule.AUTH_RULE_CONTEXT_ID] as Route.Security.AuthorizationRule;
						if((rule == null || rule.LoginPageRedirect) && !application.Context.SkipAuthorization && (application.Context.User == null || application.Context.User.Identity == null || !application.Context.User.Identity.IsAuthenticated))
						{
							System.Web.Security.FormsAuthentication.RedirectToLoginPage();
							application.Context.Response.Flush();
							application.Context.Response.End();
						}
						else
						{
							SendAccessUnauthorizedResponse(application.Context.Response);
							application.CompleteRequest();
						}
					}
				}
			}
		}

		private void OnApplicationPostResolveRequestCache(object sender, EventArgs e)
		{
			var application = (HttpApplication)sender;
			this.PostResolveRequestCache(application.Context);
		}

		private IAsyncResult BeginAcquireState(object source, EventArgs e, AsyncCallback cb, object extraData)
		{
			var app = source as HttpApplication;
			if(app != null && app.Context.Items[_requestDataKey] != null && app.Context.Session == null && _beginEventHandler != null)
				return _beginEventHandler(source, e, cb, extraData);
			else
				return new HttpAsyncResult(cb, extraData, true, app, null);
		}

		private void EndAcquireState(IAsyncResult ar)
		{
			var httpResult = ar as HttpAsyncResult;
			if(httpResult == null && _sessionEndEventHandler != null)
				_sessionEndEventHandler(ar);
		}

		private void OnReleaseState(object source, EventArgs eventArgs)
		{
			var app = source as HttpApplication;
			if(app != null && app.Context.Items[_requestDataKey] == null)
				return;
			_sessionOnReleaseState(source, eventArgs);
		}

		private void OnEndRequest(object source, EventArgs eventArgs)
		{
			var app = source as HttpApplication;
			if(app != null && app.Context.Items[_requestDataKey] == null)
				return;
			if(_sessionOnEndRequest != null)
				_sessionOnEndRequest(source, eventArgs);
			if(_formsAuthOnLeave != null)
				_formsAuthOnLeave(source, eventArgs);
		}

		private void OnAuthenticateRequest(object source, EventArgs e)
		{
			var app = source as HttpApplication;
			if(app != null && app.Context.Items[_requestDataKey] == null)
				return;
			if(_formsAuthOnEnter != null)
				_formsAuthOnEnter(source, e);
		}

		public void Init(HttpApplication application)
		{
			application.BeginRequest += new EventHandler(application_BeginRequest);
			application.PostResolveRequestCache += new EventHandler(this.OnApplicationPostResolveRequestCache);
			application.PostMapRequestHandler += new EventHandler(this.OnApplicationPostMapRequestHandler);
			application.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
			application.AddOnAcquireRequestStateAsync(new BeginEventHandler(this.BeginAcquireState), new EndEventHandler(this.EndAcquireState));
			application.ReleaseRequestState += new EventHandler(this.OnReleaseState);
			application.EndRequest += new EventHandler(this.OnEndRequest);
			HttpModuleCollection modules = null;
			var moduleCollectionField = typeof(HttpApplication).GetField("_moduleCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if(moduleCollectionField != null)
				try
				{
					modules = (HttpModuleCollection)moduleCollectionField.GetValue(application);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					if(ex.InnerException != null)
						throw ex.InnerException;
					throw;
				}
			else
				modules = application.Modules;
			var sessionModule = modules["Session"] as SessionStateModule;
			if(sessionModule != null)
			{
				var mBeginAcquireState = typeof(SessionStateModule).GetMethod("BeginAcquireState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var mEndAcquireState = typeof(SessionStateModule).GetMethod("EndAcquireState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var mReleaseState = typeof(SessionStateModule).GetMethod("OnReleaseState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var mEndRequest = typeof(SessionStateModule).GetMethod("OnEndRequest", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				_beginEventHandler = (BeginEventHandler)Delegate.CreateDelegate(typeof(BeginEventHandler), sessionModule, mBeginAcquireState);
				_sessionEndEventHandler = (EndEventHandler)Delegate.CreateDelegate(typeof(EndEventHandler), sessionModule, mEndAcquireState);
				_sessionOnReleaseState = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), sessionModule, mReleaseState);
				_sessionOnEndRequest = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), sessionModule, mEndRequest);
			}
			var formsAuthModule = modules["FormsAuthentication"] as FormsAuthenticationModule;
			if(formsAuthModule != null)
			{
				var mOnEnter = typeof(FormsAuthenticationModule).GetMethod("OnEnter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var mOnLeave = typeof(FormsAuthenticationModule).GetMethod("OnLeave", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				_formsAuthOnEnter = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), formsAuthModule, mOnEnter);
				_formsAuthOnLeave = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), formsAuthModule, mOnLeave);
			}
			var windowsAuthModule = modules["WindowsAuthentication"] as WindowsAuthenticationModule;
			if(formsAuthModule != null)
			{
				var mOnEnter = typeof(WindowsAuthenticationModule).GetMethod("OnEnter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				_windowsAuthOnEnter = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), windowsAuthModule, mOnEnter);
			}
		}

		private void application_BeginRequest(object sender, EventArgs e)
		{
			var application = sender as HttpApplication;
			System.Diagnostics.Debug.Write("\r\n BeginRequest      -- " + application.Request.Path);
			var cacheItem = RouteCache.GetItem(application.Context.Request.AppRelativeCurrentExecutionFilePath);
			application.Context.Items[_requestCacheItemKey] = cacheItem;
			if(cacheItem != null && cacheItem.Navigate != null)
			{
				var response = cacheItem.Navigate.Info.BeginRequest(new RouteActionBeginRequest {
					Context = application.Context
				});
				if(response != null && response.ApplyChanges)
				{
					if(response.IsSecurePage)
						SSLHelper.RequestSecurePage(application.Context);
					else if(response.ForceNoSecurePage)
						SSLHelper.RequestUnsecurePage(application.Context);
				}
				else
				{
					if(cacheItem.Navigate.Info.IsSecurePage)
						SSLHelper.RequestSecurePage(application.Context);
					else if(cacheItem.Navigate.Info.ForceNoSecurePage)
						SSLHelper.RequestUnsecurePage(application.Context);
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
