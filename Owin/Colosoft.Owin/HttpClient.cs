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
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Representa o cliente HTTP.
	/// </summary>
	class HttpClient : IDisposable
	{
		private static readonly System.Text.RegularExpressions.Regex PrologRegex = new System.Text.RegularExpressions.Regex("^([A-Z]+) ([^ ]+) (HTTP/[^ ]+)$", System.Text.RegularExpressions.RegexOptions.Compiled);

		private bool _disposed;

		private readonly byte[] _writeBuffer;

		private Stream _stream;

		private ClientState _state;

		private HttpRequestParser _parser;

		private bool _writeOutput = false;

		/// <summary>
		/// Evento acionado quando a requisição estiver sendo executada.
		/// </summary>
		public event EventHandler RequestExecuting;

		/// <summary>
		/// Evento acionado quando a instancia estiver sendo liberada.
		/// </summary>
		public event EventHandler Disposing;

		/// <summary>
		/// Estado do cliente.
		/// </summary>
		public ClientState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		/// <summary>
		/// Método.
		/// </summary>
		public string Method
		{
			get;
			private set;
		}

		/// <summary>
		/// Protocolo.
		/// </summary>
		public string Protocol
		{
			get;
			private set;
		}

		/// <summary>
		/// Requisição.
		/// </summary>
		public string Request
		{
			get;
			private set;
		}

		/// <summary>
		/// Cabeçalhos.
		/// </summary>
		public Dictionary<string, string> Headers
		{
			get;
			private set;
		}

		/// <summary>
		/// Parametros postados.
		/// </summary>
		public NameValueCollection PostParameters
		{
			get;
			set;
		}

		/// <summary>
		/// Items da multpart.
		/// </summary>
		public List<HttpMultiPartItem> MultiPartItems
		{
			get;
			set;
		}

		/// <summary>
		/// Buffer de leitura.
		/// </summary>
		public HttpReadBuffer ReadBuffer
		{
			get;
			private set;
		}

		/// <summary>
		/// Stream de entrada.
		/// </summary>
		public Stream InputStream
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="request"></param>
		public HttpClient(Microsoft.Owin.IOwinRequest request)
		{
			ReadBuffer = new HttpReadBuffer(2048);
			_writeBuffer = new byte[2048];
			_stream = request.Body;
		}

		/// <summary>
		/// Reseta.
		/// </summary>
		public void Reset()
		{
			if(_parser != null)
			{
				_parser.Dispose();
				_parser = null;
			}
			if(InputStream != null)
			{
				InputStream.Dispose();
				InputStream = null;
			}
			ReadBuffer.Reset();
			Method = null;
			Protocol = null;
			Request = null;
			Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			PostParameters = new NameValueCollection();
			if(MultiPartItems != null)
			{
				foreach (var item in MultiPartItems)
				{
					if(item.Stream != null)
						item.Stream.Dispose();
				}
				MultiPartItems = null;
			}
		}

		/// <summary>
		/// Inicia a requisição.
		/// </summary>
		public void BeginRequest()
		{
			BeginRead();
		}

		/// <summary>
		/// Executa a requisição.
		/// </summary>
		public void ExecuteRequest()
		{
			_writeOutput = true;
			if(RequestExecuting != null)
				RequestExecuting(this, EventArgs.Empty);
		}

		/// <summary>
		/// Remove o parse da instancia.
		/// </summary>
		public void UnsetParser()
		{
			_parser = null;
		}

		/// <summary>
		/// Inicia a leitura.
		/// </summary>
		private void BeginRead()
		{
			if(_disposed)
				return;
			try
			{
				ReadBuffer.BeginRead(_stream, ReadCallback, null);
			}
			catch
			{
				Dispose();
			}
		}

		/// <summary>
		/// Processa a chamada de retorno da leitura.
		/// </summary>
		/// <param name="asyncResult"></param>
		private void ReadCallback(IAsyncResult asyncResult)
		{
			if(_disposed)
				return;
			if(_state == ClientState.ReadingProlog)
			{
				Dispose();
				return;
			}
			try
			{
				ReadBuffer.EndRead(_stream, asyncResult);
				if(ReadBuffer.DataAvailable)
					ProcessReadBuffer();
				else
				{
					if(InputStream != null)
						InputStream.Seek(0, SeekOrigin.Begin);
					if(!_writeOutput)
						ExecuteRequest();
					else
						Dispose();
				}
			}
			catch(ObjectDisposedException)
			{
				Dispose();
			}
			catch(Exception ex)
			{
				ProcessException(ex);
			}
		}

		/// <summary>
		/// Método usado para processar a leitura do buffer.
		/// </summary>
		private void ProcessReadBuffer()
		{
			while (ReadBuffer.DataAvailable)
			{
				switch(_state)
				{
				case ClientState.ReadingProlog:
					ProcessProlog();
					break;
				case ClientState.ReadingHeaders:
					ProcessHeaders();
					break;
				case ClientState.ReadingContent:
					ProcessContent();
					break;
				default:
					throw new InvalidOperationException("Invalid state");
				}
			}
			if(!_writeOutput)
				BeginRead();
		}

		/// <summary>
		/// Processa o prolog.
		/// </summary>
		private void ProcessProlog()
		{
			string line = ReadBuffer.ReadLine();
			if(line == null)
				return;
			var match = PrologRegex.Match(line);
			if(!match.Success)
				throw new System.ServiceModel.ProtocolException(String.Format("Could not parse prolog '{0}'", line));
			Method = match.Groups[1].Value;
			Request = match.Groups[2].Value;
			Protocol = match.Groups[3].Value;
			_state = ClientState.ReadingHeaders;
			ProcessHeaders();
		}

		/// <summary>
		/// Lê os cabeçalhos.
		/// </summary>
		private void ProcessHeaders()
		{
			string line;
			while ((line = ReadBuffer.ReadLine()) != null)
			{
				if(line.Length == 0)
				{
					ReadBuffer.Reset();
					_state = ClientState.ReadingContent;
					ProcessContent();
					return;
				}
				string[] parts = line.Split(new[] {
					':'
				}, 2);
				if(parts.Length != 2)
					throw new ProtocolException("Received header without colon");
				Headers[parts[0].Trim()] = parts[1].Trim();
			}
		}

		/// <summary>
		/// Lê o conteúdo.
		/// </summary>
		private void ProcessContent()
		{
			if(_parser != null)
			{
				_parser.Parse();
				return;
			}
			if(ProcessExpectHeader())
				return;
			if(ProcessContentLengthHeader())
				return;
			ExecuteRequest();
		}

		/// <summary>
		/// Processa o cabeçalho esperado.
		/// </summary>
		/// <returns></returns>
		private bool ProcessExpectHeader()
		{
			string expectHeader;
			if(Headers.TryGetValue("Expect", out expectHeader))
			{
				Headers.Remove("Expect");
				int pos = expectHeader.IndexOf(';');
				if(pos != -1)
					expectHeader = expectHeader.Substring(0, pos).Trim();
				if(!String.Equals("100-continue", expectHeader, StringComparison.OrdinalIgnoreCase))
					throw new ProtocolException(String.Format("Could not process Expect header '{0}'", expectHeader));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Processa o cabeçalho com o tamanho do conteúdo.
		/// </summary>
		/// <returns></returns>
		private bool ProcessContentLengthHeader()
		{
			string contentLengthHeader;
			if(Headers.TryGetValue("Content-Length", out contentLengthHeader))
			{
				int contentLength;
				if(!int.TryParse(contentLengthHeader, out contentLength))
					throw new ProtocolException(String.Format("Could not parse Content-Length header '{0}'", contentLengthHeader));
				string contentTypeHeader;
				string contentType = null;
				string contentTypeExtra = null;
				if(Headers.TryGetValue("Content-Type", out contentTypeHeader))
				{
					string[] parts = contentTypeHeader.Split(new[] {
						';'
					}, 2);
					contentType = parts[0].Trim().ToLowerInvariant();
					contentTypeExtra = parts.Length == 2 ? parts[1].Trim() : null;
				}
				if(_parser != null)
				{
					_parser.Dispose();
					_parser = null;
				}
				switch(contentType)
				{
				case "application/x-www-form-urlencoded":
					_parser = new HttpUrlEncodedRequestParser(this, contentLength);
					break;
				case "multipart/form-data":
					string boundary = null;
					if(contentTypeExtra != null)
					{
						string[] parts = contentTypeExtra.Split(new[] {
							'='
						}, 2);
						if(parts.Length == 2 && String.Equals(parts[0], "boundary", StringComparison.OrdinalIgnoreCase))
							boundary = parts[1];
					}
					if(boundary == null)
						throw new ProtocolException("Expected boundary with multipart content type");
					_parser = new HttpMultiPartRequestParser(this, contentLength, boundary);
					break;
				default:
					_parser = new HttpUnknownRequestParser(this, contentLength);
					break;
				}
				ProcessContent();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Processa o erro ocorrido.
		/// </summary>
		/// <param name="exception"></param>
		private void ProcessException(Exception exception)
		{
			if(_disposed)
				return;
			_writeOutput = true;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(!_disposed)
			{
				if(Disposing != null)
					Disposing(this, EventArgs.Empty);
				_state = ClientState.Closed;
				if(_stream != null)
				{
					_stream.Dispose();
					_stream = null;
				}
				Reset();
				_disposed = true;
			}
		}

		/// <summary>
		/// Possíveis estados do cliente.
		/// </summary>
		public enum ClientState
		{
			ReadingProlog,
			ReadingHeaders,
			ReadingContent,
			WritingHeaders,
			WritingContent,
			Closed
		}
	}
}
