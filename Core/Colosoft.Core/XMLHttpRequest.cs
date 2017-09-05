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

namespace Colosoft.Web
{
	public delegate void XHREventHandler ();
	/// <summary>
	/// Representa uma requisição HTTP Xml.
	/// </summary>
	public sealed class XMLHttpRequest
	{
		private WebClient _client;

		private RequestResult _lastResponse;

		private string _method;

		private string _url;

		private bool _isAsync;

		private string _user;

		private string _password;

		private string _mimeType;

		private System.Text.Encoding _encoding = System.Text.Encoding.Default;

		private System.Collections.Specialized.NameValueCollection _requestHeaders = new System.Collections.Specialized.NameValueCollection();

		private XMLHttpRequestReadyState _readyState = XMLHttpRequestReadyState.Unsent;

		private object _response;

		private XMLHttpRequestResponseType _responseType;

		private bool _withCredentials;

		private bool _isSending = false;

		/// <summary>
		/// Evento acionado quando o ReadyState for alterado.
		/// </summary>
		public event EventHandler OnReadyStateChange;

		/// <summary>
		/// Evento acionado quando a recuperação começar.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnLoadStart;

		/// <summary>
		/// Evento acionado para reportar o progresso.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnProgress;

		/// <summary>
		/// Evento acionado quando a operação for abortada.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnAbort;

		/// <summary>
		/// Evento acionado quando ocorrer um erro.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnError;

		/// <summary>
		/// Evento acionado quando for carregado.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnLoad;

		/// <summary>
		/// Evento acionado quando ocorrer o timeout.
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnTimeout;

		/// <summary>
		/// Evento acionado quando a carga for finalizada (com sucesso ou com falha).
		/// </summary>
		public event EventHandler<ProgressEventArgs> OnLoadEnd;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public XMLHttpRequest()
		{
			_client = new WebClient();
			_client.ResponseHeadersReceived += ClientResponseHeadersReceived;
			_client.ResponseProgressChanged += ClientResponseProgressChanged;
			_client.RequestProgressChanged += ClientRequestProgressChanged;
		}

		/// <summary>
		/// Método acionado quando os cabeçalhos da requisição forem recebidos.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientResponseHeadersReceived(object sender, WebClientResponseHeadersReceivedEventArgs e)
		{
			if(e.Response.StatusCode == System.Net.HttpStatusCode.OK)
				ReadyState = XMLHttpRequestReadyState.HeadersReceived;
			if(ReadyState == XMLHttpRequestReadyState.HeadersReceived)
				ReadyState = XMLHttpRequestReadyState.Loading;
		}

		/// <summary>
		/// Método acionado quando o progresso da resposta da requisição for alterado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientResponseProgressChanged(object sender, Progress.ProgressChangedEventArgs e)
		{
			if(OnProgress != null)
				OnProgress(this, new ProgressEventArgs(true, e.Processed, e.Total));
		}

		/// <summary>
		/// Método acionado quando o progresso da requisição for alterado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientRequestProgressChanged(object sender, Progress.ProgressChangedEventArgs e)
		{
			if(OnProgress != null)
				OnProgress(this, new ProgressEventArgs(true, e.Processed, e.Total));
		}

		/// <summary>
		/// Método usado praa tratar ao requisição assincrona.
		/// </summary>
		/// <param name="ar"></param>
		private void BeginRequestHandle(IAsyncResult ar)
		{
			try
			{
				_lastResponse = _client.EndRequest(ar);
			}
			catch(Exception ex)
			{
				_lastResponse = new RequestResult(ex);
			}
			if(!_lastResponse.IsValid && _lastResponse.Error != null)
			{
				_isSending = false;
				ReadyState = XMLHttpRequestReadyState.Done;
				InternalOnError(_lastResponse.Error);
			}
			_isSending = false;
			ReadyState = XMLHttpRequestReadyState.Done;
			InternalOnLoad();
			InternalOnLoadEnd();
		}

		/// <summary>
		/// Reporta uma alteração no progresso.
		/// </summary>
		/// <param name="processed"></param>
		/// <param name="total"></param>
		private void InternalOnProgressChanged(long processed, long total)
		{
			if(OnProgress != null)
				OnProgress(this, new ProgressEventArgs(true, processed, total));
		}

		/// <summary>
		/// Método acionado quando ocorre um erro.
		/// </summary>
		/// <param name="error"></param>
		private void InternalOnError(Exception error)
		{
			if(error is TimeoutException || (error is System.Net.WebException && ((System.Net.WebException)error).Status == System.Net.WebExceptionStatus.Timeout))
				OnTimeout(this, new ProgressEventArgs(false, 0, 0));
			if(OnError != null)
				OnError(this, new ProgressEventArgs(false, 0, 0));
		}

		/// <summary>
		/// Identifica o inicio da carga.
		/// </summary>
		private void InternalOnLoadStart()
		{
			if(OnLoadStart != null)
				OnLoadStart(this, new ProgressEventArgs(false, 0, 0));
		}

		/// <summary>
		/// Identifica que os dados da requisição foram carregados.
		/// </summary>
		private void InternalOnLoad()
		{
			if(OnLoad != null)
				OnLoad(this, new ProgressEventArgs(false, 0, 0));
		}

		/// <summary>
		/// Identifica que a carga foi finalizada.
		/// </summary>
		private void InternalOnLoadEnd()
		{
			if(OnLoadEnd != null)
				OnLoadEnd(this, new ProgressEventArgs(false, 0, 0));
		}

		/// <summary>
		/// Aborta a requisição.
		/// </summary>
		public void Abort()
		{
			_client.Abort();
			if(OnAbort != null)
				OnAbort(this, new ProgressEventArgs(false, 0, 0));
		}

		/// <summary>
		/// Recupera todos os cabeçalhos da resposta.
		/// </summary>
		/// <returns></returns>
		public string GetAllResponseHeaders()
		{
			if(_lastResponse != null)
			{
				var sb = new StringBuilder();
				foreach (string key in _lastResponse.Response.Headers.Keys)
					sb.AppendLine(string.Format("{0}:{1}", key, _lastResponse.Response.Headers[key]));
				return sb.ToString();
			}
			return null;
		}

		/// <summary>
		/// Recupera o cabeçalho da resposta.
		/// </summary>
		/// <param name="header">Nome do cabeçalho que precisa ser recuperado.</param>
		/// <returns></returns>
		public string GetResponseHeader(string header)
		{
			if(_lastResponse != null)
				return _lastResponse.Response.Headers[header];
			return null;
		}

		/// <summary>
		/// Abra a requisição para a url informada.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="url">Url para onde será feita a requisição.</param>
		public void Open(string method, string url)
		{
			Open(method, url, false);
		}

		/// <summary>
		/// Abra a requisição para a url informada.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="url">Url para onde será feita a requisição.</param>
		/// <param name="async">Identifica se será uma requisição assincrona.</param>
		public void Open(string method, string url, bool async)
		{
			Open(method, url, async, null);
		}

		/// <summary>
		/// Abra a requisição para a url informada.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="url">Url para onde será feita a requisição.</param>
		/// <param name="async">Identifica se será uma requisição assincrona.</param>
		/// <param name="user">Nome do usuário que será usado na requisição.</param>
		public void Open(string method, string url, bool async, string user)
		{
			Open(method, url, async, user, null);
		}

		/// <summary>
		/// Abra a requisição para a url informada.
		/// </summary>
		/// <param name="method">Método que será usado.</param>
		/// <param name="url">Url para onde será feita a requisição.</param>
		/// <param name="async">Identifica se será uma requisição assincrona.</param>
		/// <param name="user">Nome do usuário que será usado na requisição.</param>
		/// <param name="password">Senha do usuário.</param>
		public void Open(string method, string url, bool async, string user, string password)
		{
			_method = method;
			_url = url;
			_isAsync = async;
			_user = user;
			_password = password;
			ReadyState = XMLHttpRequestReadyState.Opened;
		}

		/// <summary>
		/// Sobrescreve o MIME Type.
		/// </summary>
		/// <param name="mimeType"></param>
		public void OverrideMimeType(string mimeType)
		{
			_mimeType = mimeType;
		}

		/// <summary>
		/// Envia a requisição.
		/// </summary>
		public void Send()
		{
			Send((IRequestParameter)null);
		}

		/// <summary>
		/// Envia a requisição com os dados informados.
		/// </summary>
		/// <param name="data">Dados para a requisição.</param>
		public void Send(IRequestParameter data)
		{
			if(ReadyState != XMLHttpRequestReadyState.Opened)
				throw new XMLHttpRequestInvalidStateException("Invalid state, expected Opened");
			else if(_isSending)
				throw new XMLHttpRequestInvalidStateException("Another send operation has not been completed");
			if(StringComparer.InvariantCultureIgnoreCase.Equals(_method, "GET") || StringComparer.InvariantCultureIgnoreCase.Equals(_method, "HEAD"))
				data = null;
			_url.Require("url").NotNull().NotEmpty();
			if(_isAsync)
			{
				InternalOnLoadStart();
				_client.BeginRequest(_method, new Uri(_url), _requestHeaders, data, BeginRequestHandle, null);
			}
			else
			{
				try
				{
					_lastResponse = _client.Request(_method, new Uri(_url), _requestHeaders, data);
				}
				catch(Exception ex)
				{
					_lastResponse = new RequestResult(ex);
				}
				if(!_lastResponse.IsValid && _lastResponse.Error != null)
				{
					_isSending = false;
					ReadyState = XMLHttpRequestReadyState.Done;
					throw _lastResponse.Error;
				}
				_isSending = false;
				ReadyState = XMLHttpRequestReadyState.Done;
				InternalOnLoad();
				InternalOnLoadEnd();
			}
		}

		/// <summary>
		/// Envia a requisição com os dados informados.
		/// </summary>
		/// <param name="data"></param>
		public void Send(string data)
		{
			Send(new RequestData(data, _mimeType, _encoding));
		}

		/// <summary>
		/// Define o valor de um cabeçalho da requisição.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="value"></param>
		public void SetRequestHeader(string header, string value)
		{
			_requestHeaders[header] = value;
		}

		/// <summary>
		/// Possíveis estados de pronto.
		/// </summary>
		public XMLHttpRequestReadyState ReadyState
		{
			get
			{
				return _readyState;
			}
			private set
			{
				if(_readyState != value)
				{
					_readyState = value;
					if(OnReadyStateChange != null)
						OnReadyStateChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Objeto da resposta.
		/// </summary>
		public object Response
		{
			get
			{
				return _response ?? (_lastResponse != null ? _lastResponse.Data : null);
			}
		}

		/// <summary>
		/// Texto da resposta.
		/// </summary>
		public string ResponseText
		{
			get
			{
				return _lastResponse != null ? _lastResponse.Data : null;
			}
		}

		/// <summary>
		/// Tipo de resposta.
		/// </summary>
		public XMLHttpRequestResponseType ResponseType
		{
			get
			{
				if(_lastResponse != null && _lastResponse.IsValid)
				{
					var contentType = (_lastResponse.Response.ContentType ?? "").ToLower();
					if(contentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
						return XMLHttpRequestResponseType.Json;
					else if(contentType.StartsWith("text/html", StringComparison.InvariantCultureIgnoreCase) || contentType.StartsWith("application/xml", StringComparison.InvariantCultureIgnoreCase))
						return XMLHttpRequestResponseType.Document;
					else if(contentType.StartsWith("text/plain", StringComparison.InvariantCultureIgnoreCase))
						return XMLHttpRequestResponseType.Text;
					else if(contentType == "")
						return XMLHttpRequestResponseType.Blob;
					else
						return XMLHttpRequestResponseType.Default;
				}
				return _responseType;
			}
			set
			{
				_responseType = value;
			}
		}

		/// <summary>
		/// Situação.
		/// </summary>
		public ushort Status
		{
			get
			{
				if(_lastResponse != null)
					return (ushort)(int)_lastResponse.StatusCode;
				return (ushort)(int)System.Net.HttpStatusCode.Unused;
			}
		}

		/// <summary>
		/// Texto da situação.
		/// </summary>
		public string StatusText
		{
			get
			{
				if(_lastResponse != null)
					return _lastResponse.StatusDescription;
				return null;
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
				return _client.Timeout;
			}
			set
			{
				_client.Timeout = value;
			}
		}

		/// <summary>
		/// Identifica se é para enviar com credenciais.
		/// </summary>
		public bool WithCredentials
		{
			get
			{
				return _withCredentials;
			}
			set
			{
				_withCredentials = value;
			}
		}

		/// <summary>
		/// Representa os argumentos de um evento com progresso.
		/// </summary>
		public class ProgressEventArgs : EventArgs
		{
			private readonly bool _lengthComputable;

			private readonly long _loaded;

			private readonly long _total;

			/// <summary>
			/// Identifica se o tamanho  foi computado.
			/// </summary>
			public bool LengthComputable
			{
				get
				{
					return _lengthComputable;
				}
			}

			/// <summary>
			/// Tamanho carregado.
			/// </summary>
			public long Loaded
			{
				get
				{
					return _loaded;
				}
			}

			/// <summary>
			/// Total.
			/// </summary>
			public long Total
			{
				get
				{
					return _total;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="lengthComputable"></param>
			/// <param name="loaded"></param>
			/// <param name="total"></param>
			public ProgressEventArgs(bool lengthComputable, long loaded, long total)
			{
				_lengthComputable = lengthComputable;
				_loaded = loaded;
				_total = total;
			}
		}

		/// <summary>
		/// Possíveis estados quando a requisição estiver pronta.
		/// </summary>
		public enum XMLHttpRequestReadyState
		{
			/// <summary>
			/// Não foi enviado.
			/// </summary>
			Unsent = 0,
			/// <summary>
			/// Aberto.
			/// </summary>
			Opened = 1,
			/// <summary>
			/// Cabeçalhos recebidos.
			/// </summary>
			HeadersReceived = 2,
			/// <summary>
			/// Carregando.
			/// </summary>
			Loading = 3,
			/// <summary>
			/// Feito.
			/// </summary>
			Done = 4,
		}

		/// <summary>
		/// Possíveis tipos de resposta.
		/// </summary>
		public enum XMLHttpRequestResponseType
		{
			/// <summary>
			/// Padrão.
			/// </summary>
			Default,
			/// <summary>
			/// Buffer de vetores.
			/// </summary>
			ArrayBuffer,
			/// <summary>
			/// Blob.
			/// </summary>
			Blob,
			/// <summary>
			/// Documento.
			/// </summary>
			Document,
			/// <summary>
			/// Json.
			/// </summary>
			Json,
			/// <summary>
			/// Textoi.
			/// </summary>
			Text,
		}

		/// <summary>
		/// Representa os dados da requisição.
		/// </summary>
		class RequestData : IRequestParameter
		{
			private string _data;

			private string _contentType;

			private System.Text.Encoding _encoding;

			/// <summary>
			/// Evento acionado quando o progresso de escrita for alterado.
			/// </summary>
			public event EventHandler<Progress.ProgressChangedEventArgs> WriteProgressChanged {
				add
				{
				}
				remove {
				}
			}

			/// <summary>
			/// Tamanho do conteúdo.
			/// </summary>
			long IRequestParameter.ContentLength
			{
				get
				{
					var lenght = _encoding.GetByteCount(_data);
					return _encoding.GetPreamble().Length + lenght;
				}
			}

			/// <summary>
			/// Tipo de conteúdo.
			/// </summary>
			string IRequestParameter.ContentType
			{
				get
				{
					return _contentType;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="data"></param>
			/// <param name="contentType"></param>
			/// <param name="encoding"></param>
			public RequestData(string data, string contentType, System.Text.Encoding encoding)
			{
				_data = data ?? "";
				_contentType = contentType;
				_encoding = encoding;
			}

			/// <summary>
			/// Escreve a saída.
			/// </summary>
			/// <param name="outputStream"></param>
			public void WriteOutput(System.IO.Stream outputStream)
			{
				var writer = new System.IO.StreamWriter(outputStream, _encoding, 1024);
				writer.Write(_data);
				writer.Flush();
			}
		}
	}
}
