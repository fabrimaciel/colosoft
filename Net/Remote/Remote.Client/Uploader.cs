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
using System.Net;

namespace Colosoft.Net.Remote.Client
{
	/// <summary>
	/// Implementação da classe de uploader.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class Uploader : Colosoft.Net.IUploader
	{
		private Colosoft.Net.AggregateUploaderObserver _observers = new Colosoft.Net.AggregateUploaderObserver();

		private List<Colosoft.Net.IUploaderItem> _items = new List<Colosoft.Net.IUploaderItem>();

		private Colosoft.Threading.SimpleMonitor _monitor = new Threading.SimpleMonitor();

		private int _timeout = 300000;

		private Lazy<string> _url;

		private object _objLock = new object();

		private System.Diagnostics.Stopwatch _progressTimer = new System.Diagnostics.Stopwatch();

		private long _elapsedMilliseconds = 0;

		/// <summary>
		/// Identifica se o cancelamento foi requisitado.
		/// </summary>
		private bool _cancelRequested;

		/// <summary>
		/// Instancia da atual requisição do servidor.
		/// </summary>
		private HttpWebRequest _currentRequest;

		/// <summary>
		/// Evento disparado quando o progresso do upload se altera.
		/// </summary>
		public event Colosoft.Net.UploadProgressEventHandler ProgressChanged;

		/// <summary>
		/// Evento disparado quando o upload for finalizado.
		/// </summary>
		public event Colosoft.Net.UploadCompletedEventHandler Completed;

		/// <summary>
		/// Observadores da instancia.
		/// </summary>
		public Colosoft.Net.AggregateUploaderObserver Observers
		{
			get
			{
				return _observers;
			}
		}

		/// <summary>
		/// Timeout do proxy.
		/// </summary>
		protected int Timeout
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
		/// Url de acesso ao serviço no servidor.
		/// </summary>
		protected virtual string Url
		{
			get
			{
				return _url.Value;
			}
		}

		/// <summary>
		/// Relação dos itens que serão enviados pelo uploader.
		/// </summary>
		public List<Colosoft.Net.IUploaderItem> Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Identifica se a instancia está ocupada.
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return _monitor.Busy;
			}
		}

		/// <summary>
		/// Quantidade de bytes para serem enviados.
		/// </summary>
		public long TotalBytesToSend
		{
			get
			{
				return Items.Sum(f => f.Length);
			}
		}

		/// <summary>
		/// Total de bytes enviados.
		/// </summary>
		public long TotalBytesSent
		{
			get
			{
				return Items.Sum(f => f.NumberBytesSent);
			}
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected Uploader()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="url"></param>
		public Uploader(Lazy<string> url)
		{
			url.Require("url").NotNull();
			_url = url;
		}

		/// <summary>
		/// Crir a uri.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		private UriBuilder CreateUri(string url, string queryString)
		{
			UriBuilder builder = new UriBuilder(url);
			builder.Query = queryString;
			return builder;
		}

		/// <summary>
		/// Prepara a requisição web.
		/// </summary>
		/// <param name="request"></param>
		private void PrepareWebRequest(HttpWebRequest request)
		{
			PrepareWebRequest(request, Timeout, null);
		}

		/// <summary>
		/// Prepara a requisição web.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="timeout"></param>
		/// <param name="cultureName"></param>
		private static void PrepareWebRequest(HttpWebRequest request, int timeout, string cultureName)
		{
			if(((request.Proxy != null)) && !request.Proxy.IsBypassed(request.RequestUri))
				request.Proxy = new WebProxy(request.Proxy.GetProxy(request.RequestUri), true, new string[0], request.Proxy.Credentials);
			request.UserAgent = "UploaderClient";
			Colosoft.Net.SecurityToken.SecurityTokenHttpRequest.RegisterToken(request.Headers);
			request.Pipelined = false;
			request.KeepAlive = true;
			request.UnsafeAuthenticatedConnectionSharing = true;
			request.PreAuthenticate = false;
			request.Timeout = timeout;
			request.ReadWriteTimeout = timeout;
			if(string.IsNullOrEmpty(cultureName))
				cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
			request.Headers.Set("accept-language", cultureName);
		}

		/// <summary>
		/// Cria uma requisição web.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private HttpWebRequest CreateRequest(string url)
		{
			UriBuilder builder = CreateUri(url, null);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(builder.Uri);
			this.PrepareWebRequest(request);
			request.Method = "POST";
			_currentRequest = request;
			return request;
		}

		/// <summary>
		/// Recupera a stream para enviar os dados da requisição.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		private System.IO.Stream GetRequestStream(Colosoft.Net.IUploaderItem item, System.Net.HttpWebRequest request)
		{
			return new TracingUploadStream(request.GetRequestStream(), this, item);
		}

		/// <summary>
		/// Cria a stream com os parametros para a requisição.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private System.Net.HttpWebRequest CreateStreamRequest(Colosoft.Net.IUploaderItem item)
		{
			byte[] buffer = new byte[1024];
			int read = 0;
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
			System.Net.HttpWebRequest request = CreateRequest(Url);
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.Method = "POST";
			request.KeepAlive = true;
			request.Credentials = System.Net.CredentialCache.DefaultCredentials;
			var info = new Colosoft.Net.UploaderItemInfo(item);
			using (var outputStream = GetRequestStream(item, request))
			{
				outputStream.Write(boundarybytes, 0, boundarybytes.Length);
				string header = "Content-Disposition: form-data; name=\"info\";\r\nContent-Type:application/octet-stream\r\n\r\n";
				byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
				outputStream.Write(headerbytes, 0, headerbytes.Length);
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Colosoft.Net.UploaderItemInfo));
				serializer.Serialize(outputStream, info);
				outputStream.Write(boundarybytes, 0, boundarybytes.Length);
				header = "Content-Disposition: form-data; name=\"data\"; filename=\"item.data\"\r\nContent-Type:application/octet-stream\r\n\r\n";
				headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
				outputStream.Write(headerbytes, 0, headerbytes.Length);
				using (var itemStream = item.GetContent())
					while ((read = itemStream.Read(buffer, 0, buffer.Length)) > 0)
						outputStream.Write(buffer, 0, read);
				byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
				outputStream.Write(trailer, 0, trailer.Length);
			}
			return request;
		}

		/// <summary>
		/// Formata a situação http.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private static string FormatHttpStatus(HttpWebResponse response)
		{
			return ResourceMessageFormatter.Create(() => Properties.Resources.HttpStatusInfo, response.StatusCode, response.StatusDescription).Format();
		}

		/// <summary>
		/// Trata o erro da resposta.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		private static Exception HandleErrorResponse(WebResponse response, WebExceptionStatus status)
		{
			Exception exception = null;
			var httpResponse = (HttpWebResponse)response;
			try
			{
				if(httpResponse != null && httpResponse.StatusCode == HttpStatusCode.Unauthorized)
					return new Exception("Unauthorized : " + httpResponse.Server);
				string message = ResourceMessageFormatter.Create(() => Properties.Resources.InvalidServerResponse, httpResponse != null ? FormatHttpStatus(httpResponse) : status.ToString()).Format();
				string name = null;
				for(int i = 0; i < response.Headers.Count; i++)
				{
					if(response.Headers.Keys[i] == "X-Exception")
					{
						name = response.Headers.GetValues(i)[0];
						using (var reader = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
						{
							message = reader.ReadToEnd();
							break;
						}
					}
				}
				exception = new Exception(message);
			}
			catch(Exception exception2)
			{
				return exception2;
			}
			return exception;
		}

		/// <summary>
		/// Executa o upload.
		/// </summary>
		/// <param name="state"></param>
		private void DoUpload(object state)
		{
			_monitor.Enter();
			_elapsedMilliseconds = 0;
			var asyncResult = (Colosoft.Threading.AsyncResult<Colosoft.Net.UploaderOperationResult>)state;
			Colosoft.Net.UploaderOperationResult result = null;
			try
			{
				var items = new Queue<Colosoft.Net.IUploaderItem>(Items);
				while (items.Count > 0 && !_cancelRequested)
				{
					var item = items.Dequeue();
					HttpWebResponse response = null;
					try
					{
						var request = CreateStreamRequest(item);
						response = (HttpWebResponse)request.GetResponse();
					}
					catch(System.Net.WebException ex)
					{
						if(ex.Response != null)
							throw HandleErrorResponse(ex.Response, ex.Status);
						throw;
					}
					if(response != null)
					{
						if(response.StatusCode != HttpStatusCode.OK || response.Headers.AllKeys.Contains("X-Exception", StringComparer.InvariantCultureIgnoreCase))
						{
							throw HandleErrorResponse(response, WebExceptionStatus.Success);
						}
					}
				}
			}
			catch(Exception ex)
			{
				asyncResult.HandleException(ex, false);
				OnCompleted(ex, _cancelRequested);
				return;
			}
			finally
			{
				_monitor.Dispose();
			}
			asyncResult.Complete(result, false);
			OnCompleted(null, _cancelRequested);
		}

		/// <summary>
		/// Método acionado quando o número de bytes escritos for atualizado.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="numBytesWritten"></param>
		private void UpdateNumBytesWritten(Colosoft.Net.IUploaderItem item, long numBytesWritten)
		{
			item.NumberBytesSent += numBytesWritten;
			if(_progressTimer.ElapsedMilliseconds > (_elapsedMilliseconds + 1000))
			{
				OnProgressChanged();
				_elapsedMilliseconds = _progressTimer.ElapsedMilliseconds;
			}
		}

		/// <summary>
		/// Método acionado quando o progresso do upload for atualizador.
		/// </summary>
		protected void OnProgressChanged()
		{
			var args = new Colosoft.Net.UploadProgressChangedEventArgs(TotalBytesSent, TotalBytesToSend);
			if(ProgressChanged != null)
				ProgressChanged(this, args);
			Observers.OnProgressChanged(args);
		}

		/// <summary>
		/// Método acionado quando o download for finalizado.
		/// </summary>
		/// <param name="error">Instancia do erro caso tenha ocorrido.</param>
		/// <param name="canceled">Identifica que o upload foi cancelado.</param>
		protected void OnCompleted(Exception error, bool canceled)
		{
			var args = new Colosoft.Net.UploadCompletedEventArgs(error, canceled, null);
			if(Completed != null)
				Completed(this, args);
			Observers.OnCompleted(args);
		}

		/// <summary>
		/// Cancela a operação de upload.
		/// </summary>
		public void Cancel()
		{
			_cancelRequested = true;
			if(_currentRequest != null)
			{
				_currentRequest.Abort();
				_currentRequest = null;
			}
		}

		/// <summary>
		/// Inicia o processo de upload.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IAsyncResult BeginUpload(AsyncCallback callback, object state)
		{
			foreach (var i in Items)
				i.NumberBytesSent = 0;
			_progressTimer.Reset();
			_progressTimer.Start();
			_cancelRequested = false;
			var asyncResult = new Colosoft.Threading.AsyncResult<Colosoft.Net.UploaderOperationResult>(callback, state);
			if(!System.Threading.ThreadPool.QueueUserWorkItem(DoUpload, asyncResult))
				DoUpload(asyncResult);
			return asyncResult;
		}

		/// <summary>
		/// Finaliza a operação assincrona de Upload.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns></returns>
		public Colosoft.Net.UploaderOperationResult EndUpload(IAsyncResult ar)
		{
			var asyncResult = (Colosoft.Threading.AsyncResult<Colosoft.Net.UploaderOperationResult>)ar;
			if(asyncResult.Exception != null)
				throw asyncResult.Exception;
			return asyncResult.Result;
		}

		class TracingUploadStream : Colosoft.IO.TracingStream
		{
			private Colosoft.Net.IUploaderItem _item;

			private Uploader _uploader;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="stream"></param>
			/// <param name="uploader"></param>
			/// <param name="item"></param>
			public TracingUploadStream(System.IO.Stream stream, Uploader uploader, Colosoft.Net.IUploaderItem item) : base(stream)
			{
				_uploader = uploader;
				_item = item;
			}

			/// <summary>
			/// Método acionado para notificar que o número de bytes escritor foi atualizado.
			/// </summary>
			protected override void OnNumBytesWrittenUpdate()
			{
				_uploader.UpdateNumBytesWritten(_item, this.NumBytesWritten);
			}
		}
	}
}
