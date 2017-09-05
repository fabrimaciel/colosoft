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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel.Channels
{
	class HttpRequestChannel : RequestChannelBase
	{
		HttpChannelFactory<IRequestChannel> source;

		List<WebRequest> web_requests = new List<WebRequest>();

		public HttpRequestChannel(HttpChannelFactory<IRequestChannel> factory, EndpointAddress address, Uri via) : base(factory, address, via)
		{
			this.source = factory;
		}

		public MessageEncoder Encoder
		{
			get
			{
				return source.MessageEncoder;
			}
		}

		#if NET_2_1
				public override T GetProperty<T> ()
		{
			if (typeof (T) == typeof (IHttpCookieContainerManager))
				return source.GetProperty<T> ();
			return base.GetProperty<T> ();
		}
#endif
		public override Message Request(Message message, TimeSpan timeout)
		{
			return EndRequest(BeginRequest(message, timeout, null, null));
		}

		/// <summary>
		/// Inicia o processamento da requisição.
		/// </summary>
		/// <param name="result">Resultado assincrono.</param>
		void BeginProcessRequest(HttpChannelRequestAsyncResult result)
		{
			Message message = result.Message;
			TimeSpan timeout = result.Timeout;
			Uri destination = message.Headers.To;
			if(destination == null)
			{
				if(source.Transport.ManualAddressing)
					throw new InvalidOperationException("When manual addressing is enabled on the transport, every request messages must be set its destination address.");
				else
					destination = Via ?? RemoteAddress.Uri;
			}
			var web_request = (HttpWebRequest)HttpWebRequest.Create(destination);
			web_requests.Add(web_request);
			result.WebRequest = web_request;
			web_request.Method = "POST";
			web_request.ContentType = Encoder.ContentType;
			HttpWebRequest hwr = (web_request as HttpWebRequest);
			var cmgr = source.GetProperty<IHttpCookieContainerManager>();
			if(cmgr != null)
				hwr.CookieContainer = cmgr.CookieContainer;
			var httpbe = (HttpTransportBindingElement)source.Transport;
			string authType = null;
			switch(httpbe.AuthenticationScheme)
			{
			case AuthenticationSchemes.Basic:
				authType = "Basic";
				break;
			case AuthenticationSchemes.Digest:
				authType = "Digest";
				break;
			case AuthenticationSchemes.Ntlm:
				authType = "Ntlm";
				break;
			case AuthenticationSchemes.Negotiate:
				authType = "Negotiate";
				break;
			}
			if(authType != null)
			{
				var cred = source.ClientCredentials;
				string user = cred != null ? cred.UserName.UserName : null;
				string pwd = cred != null ? cred.UserName.Password : null;
				if(String.IsNullOrEmpty(user))
					throw new InvalidOperationException(String.Format("Use ClientCredentials to specify a user name for required HTTP {0} authentication.", authType));
				var nc = new NetworkCredential(user, pwd);
				web_request.Credentials = nc;
			}
			web_request.Timeout = (int)timeout.TotalMilliseconds;
			if(message.Version.Envelope.Equals(EnvelopeVersion.Soap11) || message.Version.Addressing.Equals(AddressingVersion.None))
			{
				if(message.Headers.Action != null)
				{
					web_request.Headers["SOAPAction"] = String.Concat("\"", message.Headers.Action, "\"");
				}
			}
			bool suppressEntityBody = false;
			string pname = HttpRequestMessageProperty.Name;
			if(message.Properties.ContainsKey(pname))
			{
				HttpRequestMessageProperty hp = (HttpRequestMessageProperty)message.Properties[pname];
				foreach (var key in hp.Headers.AllKeys)
				{
					if(WebHeaderCollection.IsRestricted(key))
					{
						switch(key)
						{
						case "Accept":
							web_request.Accept = hp.Headers[key];
							break;
						case "Connection":
							web_request.Connection = hp.Headers[key];
							break;
						case "ContentType":
							web_request.ContentType = hp.Headers[key];
							break;
						case "Expect":
							web_request.Expect = hp.Headers[key];
							break;
						case "Host":
							web_request.Host = hp.Headers[key];
							break;
						case "Referer":
							web_request.Referer = hp.Headers[key];
							break;
						case "Transfer-Encoding":
							web_request.TransferEncoding = hp.Headers[key];
							break;
						case "User-Agent":
							web_request.UserAgent = hp.Headers[key];
							break;
						}
					}
					else
						web_request.Headers[key] = hp.Headers[key];
				}
				web_request.Method = hp.Method;
				if(hp.SuppressEntityBody)
					suppressEntityBody = true;
			}
			#if !NET_2_1
			if(source.ClientCredentials != null)
			{
				var cred = source.ClientCredentials;
				if((cred.ClientCertificate != null) && (cred.ClientCertificate.Certificate != null))
					((HttpWebRequest)web_request).ClientCertificates.Add(cred.ClientCertificate.Certificate);
			}
			#endif
			if(!suppressEntityBody && String.Compare(web_request.Method, "GET", StringComparison.OrdinalIgnoreCase) != 0)
			{
				var buffer = new System.IO.MemoryStream();
				Encoder.WriteMessage(message, buffer);
				if(buffer.Length > int.MaxValue)
					throw new InvalidOperationException("The argument message is too large.");
				web_request.ContentLength = (int)buffer.Length;
				var getRequestStreamCallback = new AsyncCallback(r =>  {
					try
					{
						result.CompletedSynchronously &= r.CompletedSynchronously;
						using (var s = web_request.EndGetRequestStream(r))
							s.Write(buffer.GetBuffer(), 0, (int)buffer.Length);
						web_request.BeginGetResponse(GotResponse, result);
					}
					catch(WebException ex)
					{
						switch(ex.Status)
						{
						case WebExceptionStatus.NameResolutionFailure:
						case WebExceptionStatus.ConnectFailure:
							result.Complete(new EndpointNotFoundException(new EndpointNotFoundException().Message, ex));
							break;
						default:
							result.Complete(ex);
							break;
						}
					}
					catch(Exception ex)
					{
						result.Complete(ex);
					}
				});
				if(Colosoft.Runtime.PlatformHelper.IsRunningOnMono)
				{
					Colosoft.Net.Mono.HttpWebRequestExtensions.BeginGetRequestStream(web_request, getRequestStreamCallback, null);
				}
				else
					web_request.BeginGetRequestStream(getRequestStreamCallback, null);
			}
			else
			{
				web_request.BeginGetResponse(GotResponse, result);
			}
		}

		/// <summary>
		/// Verifica se o OS é linux.
		/// </summary>
		static bool IsLinux
		{
			get
			{
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}

		void GotResponse(IAsyncResult result)
		{
			HttpChannelRequestAsyncResult channelResult = (HttpChannelRequestAsyncResult)result.AsyncState;
			channelResult.CompletedSynchronously &= result.CompletedSynchronously;
			WebResponse res;
			System.IO.Stream resstr;
			try
			{
				res = channelResult.WebRequest.EndGetResponse(result);
				resstr = res.GetResponseStream();
			}
			catch(WebException we)
			{
				res = we.Response;
				if(res == null)
				{
					channelResult.Complete(we);
					return;
				}
				var hrr2 = (HttpWebResponse)res;
				if((int)hrr2.StatusCode >= 400 && (int)hrr2.StatusCode < 500)
				{
					Exception exception = new WebException(String.Format("There was an error on processing web request: Status code {0}({1}): {2}", (int)hrr2.StatusCode, hrr2.StatusCode, hrr2.StatusDescription), null, WebExceptionStatus.ProtocolError, hrr2);
					if((int)hrr2.StatusCode == 404)
					{
						exception = new EndpointNotFoundException("There was no endpoint listening at {0} that could accept the message. This is often caused by an incorrect address " + "or SOAP action. See InnerException, if present, for more details.", exception);
					}
					channelResult.Complete(exception);
					return;
				}
				try
				{
					resstr = res.GetResponseStream();
				}
				catch(WebException we2)
				{
					channelResult.Complete(we2);
					return;
				}
			}
			var hrr = (HttpWebResponse)res;
			if((int)hrr.StatusCode >= 400 && (int)hrr.StatusCode < 500)
			{
				channelResult.Complete(new WebException(String.Format("There was an error on processing web request: Status code {0}({1}): {2}", (int)hrr.StatusCode, hrr.StatusCode, hrr.StatusDescription)));
			}
			try
			{
				Message ret;
				if(hrr.ContentLength == 0)
				{
					ret = Message.CreateMessage(Encoder.MessageVersion, String.Empty);
				}
				else
				{
					using (var responseStream = resstr)
					{
						var ms = new System.IO.MemoryStream();
						byte[] b = new byte[65536];
						int n = 0;
						while (true)
						{
							n = responseStream.Read(b, 0, 65536);
							if(n == 0)
								break;
							ms.Write(b, 0, n);
						}
						ms.Seek(0, System.IO.SeekOrigin.Begin);
						ret = Encoder.ReadMessage(ms, (int)source.Transport.MaxReceivedMessageSize, res.ContentType);
					}
				}
				var rp = new HttpResponseMessageProperty() {
					StatusCode = hrr.StatusCode,
					StatusDescription = hrr.StatusDescription
				};
				foreach (var key in hrr.Headers.AllKeys)
					rp.Headers[key] = hrr.Headers[key];
				ret.Properties.Add(HttpResponseMessageProperty.Name, rp);
				channelResult.Response = ret;
				channelResult.Complete();
			}
			catch(Exception ex)
			{
				channelResult.Complete(ex);
			}
			finally
			{
				res.Close();
			}
		}

		public override IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
		{
			ThrowIfDisposedOrNotOpen();
			HttpChannelRequestAsyncResult result = new HttpChannelRequestAsyncResult(message, timeout, this, callback, state);
			BeginProcessRequest(result);
			return result;
		}

		public override Message EndRequest(IAsyncResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");
			HttpChannelRequestAsyncResult r = result as HttpChannelRequestAsyncResult;
			if(r == null)
				throw new InvalidOperationException("Wrong IAsyncResult");
			r.WaitEnd();
			return r.Response;
		}

		protected override void OnAbort()
		{
			foreach (var web_request in web_requests.ToArray())
				web_request.Abort();
			web_requests.Clear();
		}

		protected override void OnClose(TimeSpan timeout)
		{
			OnAbort();
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			OnAbort();
			return base.OnBeginClose(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			base.OnEndClose(result);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return base.OnBeginOpen(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			base.OnEndOpen(result);
		}

		class HttpChannelRequestAsyncResult : IAsyncResult, IDisposable
		{
			public Message Message
			{
				get;
				private set;
			}

			public TimeSpan Timeout
			{
				get;
				private set;
			}

			AsyncCallback callback;

			System.Threading.ManualResetEvent wait;

			Exception error;

			object locker = new object();

			bool is_completed;

			HttpRequestChannel owner;

			public HttpChannelRequestAsyncResult(Message message, TimeSpan timeout, HttpRequestChannel owner, AsyncCallback callback, object state)
			{
				Message = message;
				Timeout = timeout;
				this.owner = owner;
				this.callback = callback;
				AsyncState = state;
			}

			public Message Response
			{
				get;
				set;
			}

			public WebRequest WebRequest
			{
				get;
				set;
			}

			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get
				{
					lock (locker)
					{
						if(wait == null)
							wait = new System.Threading.ManualResetEvent(is_completed);
					}
					return wait;
				}
			}

			public object AsyncState
			{
				get;
				private set;
			}

			public void Complete()
			{
				Complete(null);
			}

			public void Complete(Exception ex)
			{
				if(IsCompleted)
				{
					return;
				}
				error = error ?? ex;
				IsCompleted = true;
				if(callback != null)
					callback(this);
			}

			public bool CompletedSynchronously
			{
				get;
				set;
			}

			public bool IsCompleted
			{
				get
				{
					return is_completed;
				}
				set
				{
					is_completed = value;
					lock (locker)
					{
						if(is_completed && wait != null)
							wait.Set();
						Cleanup();
					}
				}
			}

			public void WaitEnd()
			{
				if(!IsCompleted)
				{
					#if NET_2_1
										// neither Moonlight nor MonoTouch supports contexts (WaitOne default to false)
					bool result = AsyncWaitHandle.WaitOne (Timeout);
#else
					bool result = AsyncWaitHandle.WaitOne(Timeout, true);
					#endif
					if(!result)
						throw new TimeoutException();
				}
				if(error != null)
					throw error;
			}

			public void Dispose()
			{
				Cleanup();
			}

			void Cleanup()
			{
				owner.web_requests.Remove(WebRequest);
			}
		}
	}
}
