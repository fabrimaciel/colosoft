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
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web
{
	/// <summary>
	/// Representa os argumentos do evento acionado quando os cabeçalhos da resposta
	/// da requisição forem recebidos.
	/// </summary>
	public class WebClientResponseHeadersReceivedEventArgs : EventArgs
	{
		private HttpWebResponse _response;

		private bool _cancel;

		/// <summary>
		/// Identifica se é para cancelar a operação.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}

		/// <summary>
		/// Resposta associada.
		/// </summary>
		public HttpWebResponse Response
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="response"></param>
		internal WebClientResponseHeadersReceivedEventArgs(HttpWebResponse response)
		{
			_response = response;
		}
	}
	/// <summary>
	/// Representa o cliente de acesso a web.
	/// </summary>
	public class WebClient
	{
		private CookieContainer _cookieContainer = new CookieContainer();

		private Uri _lastUri;

		private int _timeout = 100000;

		private HttpWebRequest _currentRequest;

		/// <summary>
		/// Evento acionado quando as cabeçalhos da resposta forem recebidos.
		/// </summary>
		public event EventHandler<WebClientResponseHeadersReceivedEventArgs> ResponseHeadersReceived;

		/// <summary>
		/// Evento acionado quando o progresso da requisição for alterado.
		/// </summary>
		public event EventHandler<Progress.ProgressChangedEventArgs> RequestProgressChanged;

		/// <summary>
		/// Evento acionado quando o progresso para receber os dados da resposta for alterado.
		/// </summary>
		public event EventHandler<Progress.ProgressChangedEventArgs> ResponseProgressChanged;

		/// <summary>
		/// Container dos cookies.
		/// </summary>
		public virtual CookieContainer CookieContainer
		{
			get
			{
				return _cookieContainer;
			}
			set
			{
				_cookieContainer = value;
			}
		}

		/// <summary>
		/// Ultima url acessada.
		/// </summary>
		public Uri LastUri
		{
			get
			{
				return _lastUri;
			}
			set
			{
				_lastUri = value;
			}
		}

		/// <summary>
		/// Valor de timeout em milisegundos para a recuperação de resposta
		/// e o envio dos dados da requisição.
		/// O padrão é 100.000 milisegundos (100 segundos).
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		/// <summary>
		/// Método acionado quando os cabeçalhos da resposta forem recebidos.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnResponseHeadersReceived(WebClientResponseHeadersReceivedEventArgs e)
		{
			if(ResponseHeadersReceived != null)
				ResponseHeadersReceived(this, e);
		}

		/// <summary>
		/// Recupera a requisição.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="uri"></param>
		/// <param name="headers"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private HttpWebRequest GetWebRequest(string method, Uri uri, System.Collections.Specialized.NameValueCollection headers, IRequestParameter parameter)
		{
			if(uri == null)
				throw new ArgumentNullException("uri");
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
			var acceptCharsetFound = false;
			var acceptEncodingFound = false;
			var acceptLanguageFound = false;
			if(headers != null && headers.HasKeys())
				foreach (string key in headers.Keys)
				{
					if(!acceptCharsetFound && StringComparer.InvariantCultureIgnoreCase.Equals(key, "Accept-Charset"))
						acceptCharsetFound = true;
					else if(!acceptEncodingFound && StringComparer.InvariantCultureIgnoreCase.Equals(key, "Accept-Encoding"))
						acceptEncodingFound = true;
					else if(!acceptLanguageFound && StringComparer.InvariantCultureIgnoreCase.Equals(key, "Accept-Language"))
						acceptLanguageFound = true;
					if(StringComparer.InvariantCultureIgnoreCase.Equals(key, "Content-type"))
						req.ContentType = headers[key];
					else if(StringComparer.InvariantCultureIgnoreCase.Equals(key, "Content-Length"))
						req.ContentLength = long.Parse(headers[key]);
					else
						req.Headers.Add(key, headers[key]);
				}
			if(!acceptCharsetFound)
				req.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
			if(!acceptEncodingFound)
				req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
			if(!acceptLanguageFound)
				req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
			req.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
			req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.127 Safari/534.16";
			req.KeepAlive = true;
			req.CookieContainer = _cookieContainer;
			req.Timeout = _timeout;
			_lastUri = uri;
			if(!string.IsNullOrEmpty(method))
				req.Method = method;
			else if(parameter != null)
				req.Method = "POST";
			else
				req.Method = "GET";
			return req;
		}

		/// <summary>
		/// Recupera o resultado da requisição.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private RequestResult GetRequestResult(WebResponse response)
		{
			if(response != null)
			{
				var webResponse = response as HttpWebResponse;
				var args = new WebClientResponseHeadersReceivedEventArgs(webResponse);
				OnResponseHeadersReceived(args);
				var responseStream = new System.IO.MemoryStream();
				if(!args.Cancel)
				{
					var buffer = new byte[1024];
					var read = 0;
					var contentLength = webResponse.ContentLength;
					if(string.Compare(webResponse.ContentEncoding, "gzip", true) == 0)
					{
						using (var stream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress))
							while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
							{
								responseStream.Write(buffer, 0, read);
								OnResponseProgressChanged(contentLength, read);
							}
					}
					else if(string.Compare(webResponse.ContentEncoding, "deflate", true) == 0)
					{
						using (var stream = new System.IO.Compression.DeflateStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress))
							while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
							{
								responseStream.Write(buffer, 0, read);
								OnResponseProgressChanged(contentLength, read);
							}
					}
					else
					{
						using (var stream = response.GetResponseStream())
							while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
							{
								responseStream.Write(buffer, 0, read);
								OnResponseProgressChanged(contentLength, read);
							}
					}
					responseStream.Seek(0, System.IO.SeekOrigin.Begin);
				}
				System.Text.Encoding encoding = null;
				try
				{
					if(webResponse.CharacterSet != null && webResponse.CharacterSet != "")
						encoding = System.Text.Encoding.GetEncoding(webResponse.CharacterSet);
				}
				catch
				{
				}
				return new RequestResult(webResponse, webResponse != null ? webResponse.StatusCode : HttpStatusCode.BadRequest, webResponse != null ? webResponse.StatusDescription : "Bad Request", encoding ?? System.Text.Encoding.Default, responseStream);
			}
			else
				return new RequestResult(null, HttpStatusCode.BadRequest, "Bad Request", System.Text.Encoding.Default, null);
		}

		/// <summary>
		/// Método acionado quando o progresso de download da resposta for alterado.
		/// </summary>
		/// <param name="total"></param>
		/// <param name="processed"></param>
		private void OnResponseProgressChanged(long total, long processed)
		{
			if(ResponseProgressChanged != null)
				ResponseProgressChanged(this, new Progress.ProgressChangedEventArgs(total, processed));
		}

		/// <summary>
		/// Aborta a requisição.
		/// </summary>
		public void Abort()
		{
			if(_currentRequest != null)
				_currentRequest.Abort();
		}

		/// <summary>
		/// Realiza a requisicao de uma Uri.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public RequestResult Request(string uri)
		{
			uri.Require("uri").NotNull().NotEmpty();
			return Request(new Uri(uri), null);
		}

		/// <summary>
		/// Realiza a requisicao de uma Uri.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public RequestResult Request(string uri, IRequestParameter parameters)
		{
			uri.Require("uri").NotNull().NotEmpty();
			return Request(new Uri(uri), parameters);
		}

		/// <summary>
		/// Faz uma requisicao no servidor.
		/// </summary>
		/// <param name="uri">Uri onde a requisição será realizada.</param>
		/// <param name="parameter">Parametros que serão usados na requisição.</param>
		/// <returns></returns>
		public RequestResult Request(Uri uri, IRequestParameter parameter)
		{
			return Request(null, uri, null, parameter);
		}

		/// <summary>
		/// Realiza a requisição dos dados no servidor.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="uri">Uri onde a requisição será realizada.</param>
		/// <param name="headers">Cabeçalhos da requisição.</param>
		/// <param name="parameter">Parametro que será enviado.</param>
		/// <returns></returns>
		public RequestResult Request(string method, Uri uri, System.Collections.Specialized.NameValueCollection headers, IRequestParameter parameter)
		{
			var req = GetWebRequest(method, uri, headers, parameter);
			_currentRequest = req;
			try
			{
				if(parameter != null)
				{
					req.ContentType = parameter.ContentType;
					req.ContentLength = parameter.ContentLength;
					var stream = req.GetRequestStream();
					parameter.WriteOutput(stream);
					stream.Flush();
					stream.Close();
				}
				else if(req.ContentLength < 0)
					req.ContentLength = 0;
				WebResponse response = null;
				try
				{
					response = req.GetResponse();
				}
				catch(Exception ex)
				{
					return new RequestResult(ex);
				}
				return GetRequestResult(response);
			}
			finally
			{
				_currentRequest = null;
			}
		}

		/// <summary>
		/// Método usaddo para tratar a recuperação a stream para enviar os dados de requisição.
		/// </summary>
		/// <param name="ar"></param>
		private void GetRequestStreamHandle(IAsyncResult ar)
		{
			var items = (object[])ar.AsyncState;
			var req = (HttpWebRequest)items[0];
			var parameter = (IRequestParameter)items[1];
			var ar2 = (Threading.AsyncResult<RequestResult>)items[2];
			System.IO.Stream requestStream = null;
			parameter.WriteProgressChanged += ParameterWriteProgressChanged;
			try
			{
				requestStream = req.EndGetRequestStream(ar);
				parameter.WriteOutput(requestStream);
				requestStream.Flush();
				requestStream.Close();
				req.BeginGetResponse(GetResponseHandle, new object[] {
					req,
					ar2
				});
			}
			catch(Exception ex)
			{
				ar2.HandleException(ex, false);
				_currentRequest = null;
				return;
			}
			finally
			{
				parameter.WriteProgressChanged -= ParameterWriteProgressChanged;
			}
		}

		/// <summary>
		/// Método acionado quando o progresso da escrita dos dados do parametro for alterado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ParameterWriteProgressChanged(object sender, Progress.ProgressChangedEventArgs e)
		{
			if(RequestProgressChanged != null)
				RequestProgressChanged(this, e);
		}

		/// <summary>
		/// Método usado para tratar a recuperação das resposta.
		/// </summary>
		/// <param name="ar"></param>
		private void GetResponseHandle(IAsyncResult ar)
		{
			var items = (object[])ar.AsyncState;
			var req = (HttpWebRequest)items[0];
			var ar2 = (Threading.AsyncResult<RequestResult>)items[1];
			WebResponse response = null;
			RequestResult result = null;
			try
			{
				response = req.EndGetResponse(ar);
			}
			catch(Exception ex)
			{
				result = new RequestResult(ex);
				ar2.Complete(result, false);
				_currentRequest = null;
				return;
			}
			try
			{
				result = GetRequestResult(response);
			}
			catch(Exception ex)
			{
				result = new RequestResult(ex);
				ar2.Complete(result, false);
				_currentRequest = null;
				return;
			}
			_currentRequest = null;
			ar2.Complete(result, false);
		}

		/// <summary>
		/// Inicia o processo da requisição de forma assincrona.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="uri">Uri onde a requisição será realizada.</param>
		/// <param name="headers">Cabeçalhos da requisição.</param>
		/// <param name="parameter">Parametro que será enviado.</param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginRequest(string method, Uri uri, System.Collections.Specialized.NameValueCollection headers, IRequestParameter parameter, AsyncCallback callback, object state)
		{
			var asyncResult = new Threading.AsyncResult<RequestResult>(callback, state);
			var req = GetWebRequest(method, uri, headers, parameter);
			_currentRequest = req;
			if(parameter != null)
			{
				req.ContentType = parameter.ContentType;
				req.ContentLength = parameter.ContentLength;
				req.BeginGetRequestStream(GetRequestStreamHandle, new object[] {
					req,
					parameter,
					asyncResult
				});
				return asyncResult;
			}
			else
			{
				req.BeginGetResponse(GetResponseHandle, new object[] {
					req,
					asyncResult
				});
			}
			return asyncResult;
		}

		/// <summary>
		/// Finaliza a requisição.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns></returns>
		public RequestResult EndRequest(IAsyncResult ar)
		{
			var ar2 = (Threading.AsyncResult<RequestResult>)ar;
			if(ar2.Exception != null)
				throw ar2.Exception;
			return ar2.Result;
		}
	}
}
