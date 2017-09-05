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
	/// Adaptação do worker de requisição.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
	class HttpWorkerRequestWrapper : System.Web.HttpWorkerRequest
	{
		private Microsoft.Owin.IOwinContext _owinContext;

		private string _rawUrl;

		private string _appPhysPath;

		private string _appVirtPath;

		private string _page;

		private string _pathInfo;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		public HttpWorkerRequestWrapper(Microsoft.Owin.IOwinContext context)
		{
			_owinContext = context;
			_appPhysPath = System.Threading.Thread.GetDomain().GetData(".appPath").ToString();
			_appVirtPath = System.Threading.Thread.GetDomain().GetData(".appVPath").ToString();
			_page = context.Request.Uri.LocalPath;
			ExtractPagePathInfo();
		}

		/// <summary>
		/// Extrai as informações do caminho da página.
		/// </summary>
		private void ExtractPagePathInfo()
		{
			int index = _page.IndexOf('/');
			if(index >= 0)
			{
				_pathInfo = _page.Substring(index);
				_page = _page.Substring(0, index);
			}
		}

		/// <summary>
		/// Recupera o caminho da requisição.
		/// </summary>
		/// <param name="includePathInfo"></param>
		/// <returns></returns>
		private string GetPathInternal(bool includePathInfo)
		{
			string str = _appVirtPath.Equals("/") ? ("/" + _page) : (_appVirtPath + "/" + _page);
			var result = str;
			if(includePathInfo && (_pathInfo != null))
				result = (str + _pathInfo);
			while (result.Length > 1 && result[0] == '/' && result[1] == '/')
				result = result.Substring(1);
			return result;
		}

		/// <summary>
		/// Recupera o caminho invalidado.
		/// </summary>
		/// <returns></returns>
		private string GetUnvalidatedPath()
		{
			return _owinContext.Request.Path.Value;
		}

		/// <summary>
		/// Assegura a RawUrl.
		/// </summary>
		/// <returns></returns>
		private string EnsureRawUrl()
		{
			if(_rawUrl == null)
			{
				string rawUrl;
				string unvalidatedPath = GetUnvalidatedPath();
				string queryStringText = _owinContext.Request.QueryString.Value;
				if(!string.IsNullOrEmpty(queryStringText))
					rawUrl = unvalidatedPath + "?" + queryStringText;
				else
					rawUrl = unvalidatedPath;
				_rawUrl = rawUrl;
			}
			return _rawUrl;
		}

		/// <summary>
		/// Envia o tamanho calculado do conteúdo.
		/// </summary>
		/// <param name="contentLength"></param>
		public override void SendCalculatedContentLength(long contentLength)
		{
			base.SendCalculatedContentLength(contentLength);
		}

		/// <summary>
		/// Recupera o caminho do URI.
		/// </summary>
		/// <example>"/foo/page.aspx/tail"</example>
		/// <returns></returns>
		public override string GetUriPath()
		{
			return GetPathInternal(true);
		}

		/// <summary>
		/// Recupera a querystring.
		/// </summary>
		/// <returns></returns>
		public override string GetQueryString()
		{
			return _owinContext.Request.QueryString.Value;
		}

		/// <summary>
		/// Recupera a url crua.
		/// "/foo/page.aspx/tail?param=bar"
		/// </summary>
		/// <returns></returns>
		public override string GetRawUrl()
		{
			EnsureRawUrl();
			return _rawUrl;
		}

		/// <summary>
		/// Recupera o nome do verbo http.
		/// </summary>
		/// <returns></returns>
		public override String GetHttpVerbName()
		{
			return _owinContext.Request.Method;
		}

		/// <summary>
		/// Recupera a versão do HTTP.
		/// </summary>
		/// <returns></returns>
		public override String GetHttpVersion()
		{
			return "HTTP/1.0";
		}

		/// <summary>
		/// Recupera o endereço remoto.
		/// </summary>
		/// <returns></returns>
		public override String GetRemoteAddress()
		{
			return _owinContext.Request.RemoteIpAddress;
		}

		/// <summary>
		/// Recupera a porta remota.
		/// </summary>
		/// <returns></returns>
		public override int GetRemotePort()
		{
			return _owinContext.Request.RemotePort.GetValueOrDefault();
		}

		/// <summary>
		/// Recupera o endereço local.
		/// </summary>
		/// <returns></returns>
		public override String GetLocalAddress()
		{
			return _owinContext.Request.LocalIpAddress;
		}

		/// <summary>
		/// Recupera a porta local.
		/// </summary>
		/// <returns></returns>
		public override int GetLocalPort()
		{
			return _owinContext.Request.LocalPort.GetValueOrDefault();
		}

		/// <summary>
		/// Recupera o token do usuário.
		/// </summary>
		/// <returns></returns>
		public override IntPtr GetUserToken()
		{
			return IntPtr.Zero;
		}

		/// <summary>
		/// Recupera o caminho do arquivo.
		/// </summary>
		/// <returns></returns>
		public override string GetFilePath()
		{
			return GetPathInternal(true);
		}

		/// <summary>
		/// Recupera o caminho do arquivo traduzido.
		/// </summary>
		/// <returns></returns>
		public override string GetFilePathTranslated()
		{
			string path = _appPhysPath + _page.Replace('/', '\\');
			return path;
		}

		/// <summary>
		/// Recupera as informações do caminho.
		/// </summary>
		/// <returns></returns>
		public override string GetPathInfo()
		{
			return null;
		}

		/// <summary>
		/// Recupera o caminho do aplicação.
		/// </summary>
		/// <returns></returns>
		public override string GetAppPath()
		{
			return _appVirtPath;
		}

		/// <summary>
		/// Recupera o caminho traduzido da aplicação.
		/// </summary>
		/// <returns></returns>
		public override String GetAppPathTranslated()
		{
			return this._appPhysPath;
		}

		/// <summary>
		/// Recupera uma variável do sistema.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetServerVariable(string name)
		{
			object value = null;
			if(_owinContext.Environment.TryGetValue(name, out value) && value is string)
				return (string)value;
			return null;
		}

		/// <summary>
		/// Mapeai o caminho informado.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public override string MapPath(string path)
		{
			string mappedPath = null;
			string appPath = _appPhysPath.Substring(0, _appPhysPath.Length - 1);
			if(string.IsNullOrEmpty(path) || path.Equals("/"))
				mappedPath = appPath;
			if(path.StartsWith(_appVirtPath))
				mappedPath = appPath + path.Substring(_appVirtPath.Length).Replace('/', '\\');
			return mappedPath;
		}

		/// <summary>
		/// Recupera o caminho do machine.config.
		/// </summary>
		public override string MachineConfigPath
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Recupera o caminho do web.config do root.
		/// </summary>
		public override string RootWebConfigPath
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Diretório de instalação da máquina.
		/// </summary>
		public override String MachineInstallDirectory
		{
			get
			{
				return null;
			}
		}

		public override int GetTotalEntityBodyLength()
		{
			return base.GetTotalEntityBodyLength();
		}

		public override int GetPreloadedEntityBody(byte[] buffer, int offset)
		{
			return base.GetPreloadedEntityBody(buffer, offset);
		}

		/// <summary>
		/// Recupera o protocolo.
		/// </summary>
		/// <returns></returns>
		public override string GetProtocol()
		{
			return _owinContext.Request.IsSecure ? "https" : "http";
		}

		/// <summary>
		/// Verifica se é uma entidade de corpo e se foi pré-carregada.
		/// </summary>
		/// <returns></returns>
		public override bool IsEntireEntityBodyIsPreloaded()
		{
			return base.IsEntireEntityBodyIsPreloaded();
		}

		/// <summary>
		/// Recupera o valor conhecido do cabeçalho da requisição.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override string GetKnownRequestHeader(int index)
		{
			var name = System.Web.HttpWorkerRequest.GetKnownRequestHeaderName(index);
			if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "content-type"))
				return _owinContext.Request.ContentType;
			return _owinContext.Request.Headers.Get(name);
		}

		public override byte[] GetQueryStringRawBytes()
		{
			return base.GetQueryStringRawBytes();
		}

		/// <summary>
		/// Recupera o valor desconhecido do cabeçalho da requisição.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetUnknownRequestHeader(string name)
		{
			return _owinContext.Request.Headers.Get(name);
		}

		/// <summary>
		/// Recupera o nome do servidor.
		/// </summary>
		/// <returns></returns>
		public override string GetServerName()
		{
			var address = _owinContext.Request.LocalIpAddress;
			if(address == "::1" || address == "127.0.0.1")
				return "localhost";
			return address;
		}

		/// <summary>
		/// Recupera o tamanho do corpo pré-carregado da entidade.
		/// </summary>
		/// <returns></returns>
		public override int GetPreloadedEntityBodyLength()
		{
			return 0;
		}

		/// <summary>
		/// Recupera o corpo pré-carregado da entidade.
		/// </summary>
		/// <returns></returns>
		public override byte[] GetPreloadedEntityBody()
		{
			return null;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return _owinContext.Request.Body.BeginRead(buffer, offset, count, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return _owinContext.Request.Body.EndRead(asyncResult);
		}

		/// <summary>
		/// Lê o corpo da entidade.
		/// </summary>
		/// <param name="buffer">Buffer onde os dados serão gravados.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="size">Tamanho.</param>
		/// <returns></returns>
		public override int ReadEntityBody(byte[] buffer, int offset, int size)
		{
			var read = _owinContext.Request.Body.Read(buffer, offset, size);
			return read;
		}

		/// <summary>
		/// Lê o corpo da entidade.
		/// </summary>
		/// <param name="buffer">Buffer onde os dados serão gravados.</param>
		/// <param name="size">Tamanho do buffer que deve ser lido.</param>
		/// <returns></returns>
		public override int ReadEntityBody(byte[] buffer, int size)
		{
			var read = _owinContext.Request.Body.Read(buffer, 0, size);
			return read;
		}

		/// <summary>
		/// Envia a situação.
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="statusDescription"></param>
		public override void SendStatus(int statusCode, string statusDescription)
		{
			_owinContext.Response.StatusCode = statusCode;
		}

		/// <summary>
		/// Envia um cabeçalho conhecido.
		/// </summary>
		/// <param name="index">Indice do cabeçalho.</param>
		/// <param name="value">Valor que será atribuido ao cabeçalho.</param>
		public override void SendKnownResponseHeader(int index, string value)
		{
			var name = System.Web.HttpWorkerRequest.GetKnownResponseHeaderName(index);
			if(StringComparer.InvariantCultureIgnoreCase.Equals(name, "content-type"))
				_owinContext.Response.ContentType = value;
			_owinContext.Response.Headers.Set(name, value);
		}

		/// <summary>
		/// Envia um cabeçalho desconhecido.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SendUnknownResponseHeader(string name, string value)
		{
			_owinContext.Response.Headers.Set(name, value);
		}

		/// <summary>
		/// Envia a resposta a partir do arquivo.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			using (var stream = System.IO.File.OpenRead(filename))
			{
				stream.Seek(offset, System.IO.SeekOrigin.Begin);
				var read = 0;
				var buffer = new byte[4096];
				var body = _owinContext.Response.Body;
				while (length > 0 && (read = stream.Read(buffer, 0, buffer.Length <= length ? buffer.Length : (int)length)) > 0)
					body.Write(buffer, 0, read);
				body.Flush();
			}
		}

		/// <summary>
		/// Envia a resposta a partir da memória.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public override void SendResponseFromMemory(byte[] data, int length)
		{
			_owinContext.Response.Body.Write(data, 0, length);
		}

		/// <summary>
		/// Envia a respostas a partir da memória.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public override void SendResponseFromMemory(IntPtr data, int length)
		{
			var byteArray = new byte[length];
			System.Runtime.InteropServices.Marshal.Copy(data, byteArray, 0, length);
			SendResponseFromMemory(byteArray, length);
		}

		/// <summary>
		/// Envia a mensagem a partir do arquivo.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Flush da resposta.
		/// </summary>
		/// <param name="finalFlush"></param>
		public override void FlushResponse(bool finalFlush)
		{
			_owinContext.Response.Body.Flush();
		}

		/// <summary>
		/// Finaliza a requisição.
		/// </summary>
		public override void EndOfRequest()
		{
		}
	}
}
