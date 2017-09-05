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
	/// Representa a resposta HTTP.
	/// </summary>
	class HttpResponse : System.Web.HttpResponseBase
	{
		private HttpContext _context;

		private Microsoft.Owin.IOwinResponse _response;

		private System.Web.HttpCookieCollection _cookies;

		private System.Collections.Specialized.NameValueCollection _headers;

		private HttpResponseWriter _writer;

		private System.Text.Encoding _encoding;

		private HttpCachePolicy _cache;

		private string _chartset = "utf-8";

		private System.Web.HttpRequestBase _request;

		private string _statusDescription = "OK";

		/// <summary>
		/// Writer associado.
		/// </summary>
		public System.IO.TextWriter Writer
		{
			get
			{
				return _writer;
			}
		}

		/// <summary>
		/// OwinResponse
		/// </summary>
		internal Microsoft.Owin.IOwinResponse OwinResponse
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Recupera a parte de saída da resposta.
		/// </summary>
		public override System.IO.TextWriter Output
		{
			get
			{
				return _writer;
			}
		}

		/// <summary>
		/// Cookies da resposta.
		/// </summary>
		public override System.Web.HttpCookieCollection Cookies
		{
			get
			{
				return _cookies;
			}
		}

		/// <summary>
		/// Cabeçalhos
		/// </summary>
		public override System.Collections.Specialized.NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		/// <summary>
		/// Código do Status.
		/// </summary>
		public override int StatusCode
		{
			get
			{
				return _response.StatusCode;
			}
			set
			{
				_response.StatusCode = value;
			}
		}

		/// <summary>
		/// Descrição da situação.
		/// </summary>
		public override string StatusDescription
		{
			get
			{
				return _statusDescription;
			}
			set
			{
				_statusDescription = value;
			}
		}

		/// <summary>
		/// Encoding do conteúdo.
		/// </summary>
		public override Encoding ContentEncoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				_encoding = value;
			}
		}

		/// <summary>
		/// Charset.
		/// </summary>
		public override string Charset
		{
			get
			{
				return _chartset;
			}
			set
			{
				_chartset = value;
				Headers["Content-Type"] = FormatContentType();
			}
		}

		/// <summary>
		/// Tipo do conteúdo.
		/// </summary>
		public override string ContentType
		{
			get
			{
				return OwinResponse.ContentType;
			}
			set
			{
				OwinResponse.ContentType = value;
				Headers["Content-Type"] = FormatContentType();
			}
		}

		/// <summary>
		/// Controle de cache.
		/// </summary>
		public override string CacheControl
		{
			get
			{
				return _response.Headers["Cache-Control"];
			}
			set
			{
				_response.Headers["Cache-Control"] = value;
			}
		}

		/// <summary>
		/// Cache.
		/// </summary>
		public override System.Web.HttpCachePolicyBase Cache
		{
			get
			{
				return _cache;
			}
		}

		/// <summary>
		/// Expires.
		/// </summary>
		public override int Expires
		{
			get
			{
				return _response.Expires.HasValue ? (int)(_response.Expires.Value - DateTimeOffset.Now).TotalMinutes : 0;
			}
			set
			{
				_response.Expires = DateTimeOffset.Now.AddMinutes(value);
			}
		}

		/// <summary>
		/// Stream de saída.
		/// </summary>
		public override System.IO.Stream OutputStream
		{
			get
			{
				return _response.Body;
			}
		}

		/// <summary>
		/// Requisição associada.
		/// </summary>
		protected System.Web.HttpRequestBase Request
		{
			get
			{
				return _request;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="response"></param>
		/// <param name="request"></param>
		/// <param name="encoding">Encoding que será usado na reposta.</param>
		public HttpResponse(HttpContext context, Microsoft.Owin.IOwinResponse response, System.Web.HttpRequestBase request, System.Text.Encoding encoding)
		{
			_context = context;
			_response = response;
			_request = request;
			_encoding = encoding;
			_cookies = new System.Web.HttpCookieCollection();
			_headers = new System.Collections.Specialized.NameValueCollection();
			_response.OnSendingHeaders(SendHeaders, null);
			_writer = new HttpResponseWriter(response, encoding);
			_cache = new HttpCachePolicy(this);
			ContentType = response.ContentType;
		}

		/// <summary>
		/// Formata o tipo do conteúdo.
		/// </summary>
		/// <returns></returns>
		private string FormatContentType()
		{
			if(!string.IsNullOrEmpty(Charset))
				return string.Format("{0}; charset={1}", ContentType, Charset);
			return ContentType;
		}

		/// <summary>
		/// Envia os cabeçalhos.
		/// </summary>
		/// <param name="state"></param>
		private void SendHeaders(object state)
		{
			foreach (string key in _headers.Keys)
				_response.Headers[key] = _headers.GetValues(key).FirstOrDefault();
			foreach (var i in Cookies.AllKeys)
				_response.Cookies.Append(i, Cookies.Get(i).Value ?? "");
		}

		/// <summary>
		/// Adiciona um cabeçalho.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void AddHeader(string name, string value)
		{
			_headers.Add(name, value);
		}

		/// <summary>
		/// Aplica o modificador do caminho da aplicação.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override string ApplyAppPathModifier(string virtualPath)
		{
			return virtualPath;
		}

		/// <summary>
		/// Redireciona a requisição.
		/// </summary>
		/// <param name="url"></param>
		public override void Redirect(string url)
		{
			OwinResponse.Redirect(url);
		}

		/// <summary>
		/// Redidireona a requisição.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="endResponse"></param>
		public override void Redirect(string url, bool endResponse)
		{
			OwinResponse.Redirect(url);
			if(endResponse)
				End();
		}

		/// <summary>
		/// Flush.
		/// </summary>
		public override void Flush()
		{
			OwinResponse.Body.Flush();
		}

		/// <summary>
		/// Finaliza.
		/// </summary>
		public override void End()
		{
			OwinResponse.Body.Close();
		}

		/// <summary>
		/// Anexa uma cabeçalho.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void AppendHeader(string name, string value)
		{
			OwinResponse.Headers.Add(new KeyValuePair<string, string[]>(name, new[] {
				value
			}));
		}

		/// <summary>
		/// Escreve o buffer para a resposta.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public override void Write(char[] buffer, int index, int count)
		{
			_response.Write(new string(buffer.Skip(index).Take(count).ToArray()));
		}

		/// <summary>
		/// Escreve o char para a resposta.
		/// </summary>
		/// <param name="ch"></param>
		public override void Write(char ch)
		{
			_response.Write(ch.ToString());
		}

		/// <summary>
		/// Escreve o texto para a resposta.
		/// </summary>
		/// <param name="s"></param>
		public override void Write(string s)
		{
			_response.Write(s);
		}

		/// <summary>
		/// Implementação da política do cache.
		/// </summary>
		class HttpCachePolicy : System.Web.HttpCachePolicyBase
		{
			private HttpResponse _response;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="response"></param>
			public HttpCachePolicy(HttpResponse response)
			{
				_response = response;
			}

			/// <summary>
			/// Adiciona a chamada de retorno da validação.
			/// </summary>
			/// <param name="handler"></param>
			/// <param name="data"></param>
			public override void AddValidationCallback(System.Web.HttpCacheValidateHandler handler, object data)
			{
			}

			/// <summary>
			/// Define a idade máxima.
			/// </summary>
			/// <param name="delta"></param>
			public override void SetProxyMaxAge(TimeSpan delta)
			{
			}

			/// <summary>
			/// Define a data de expiração.
			/// </summary>
			/// <param name="date"></param>
			public override void SetExpires(DateTime date)
			{
				_response.OwinResponse.Expires = new DateTimeOffset(date);
			}
		}
	}
}
