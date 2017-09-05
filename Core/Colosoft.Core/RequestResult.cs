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

namespace Colosoft.Web
{
	///<summary>
	/// Armazena os dados de uma requisicao.
	/// </summary>
	public class RequestResult : IDisposable
	{
		private System.IO.Stream _responseStream;

		private HttpStatusCode _statusCode;

		private string _statusDescription;

		private HttpWebResponse _response;

		private string _data;

		private System.Text.Encoding _encoding;

		private Exception _error;

		/// <summary>
		/// Resposta do servidor.
		/// </summary>
		public HttpWebResponse Response
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Codigo da situacao da requisicao.
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get
			{
				return _statusCode;
			}
		}

		/// <summary>
		/// Descricao da situacao da requisicao.
		/// </summary>
		public string StatusDescription
		{
			get
			{
				return _statusDescription;
			}
		}

		/// <summary>
		/// Dados da resposta.
		/// </summary>
		public string Data
		{
			[System.Diagnostics.DebuggerStepThrough]
			get
			{
				if(_data == null && _responseStream != null)
				{
					var pos = _responseStream.Position;
					_responseStream.Seek(0, System.IO.SeekOrigin.Begin);
					try
					{
						_data = new System.IO.StreamReader(_responseStream, _encoding).ReadToEnd();
					}
					finally
					{
						_responseStream.Seek(pos, System.IO.SeekOrigin.Begin);
					}
				}
				return _data;
			}
		}

		/// <summary>
		/// Identifica se é um resultado valido.
		/// </summary>
		public bool IsValid
		{
			get
			{
				return _error == null && StatusCode == System.Net.HttpStatusCode.OK;
			}
		}

		/// <summary>
		/// Erro associado.
		/// </summary>
		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Construtor padrao.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="statusCode"></param>
		/// <param name="statusDescription"></param>
		/// <param name="encoding"></param>
		/// <param name="responseStream"></param>
		public RequestResult(HttpWebResponse response, HttpStatusCode statusCode, string statusDescription, System.Text.Encoding encoding, System.IO.Stream responseStream)
		{
			_response = response;
			_statusCode = statusCode;
			_statusDescription = statusDescription;
			_encoding = encoding;
			_responseStream = responseStream;
		}

		/// <summary>
		/// Cria o resultado com base no erro informado.
		/// </summary>
		/// <param name="error"></param>
		public RequestResult(Exception error)
		{
			_error = error;
			if(error is WebException)
			{
				var error2 = (WebException)error;
				_response = error2.Response as HttpWebResponse;
				_statusDescription = error2.Status.ToString();
				_statusCode = HttpStatusCode.BadRequest;
			}
		}

		/// <summary>
		/// Recupera a stream da resposta.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream GetResponseStream()
		{
			return _responseStream;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_responseStream != null)
			{
				_responseStream.Dispose();
				_responseStream = null;
			}
		}
	}
}
