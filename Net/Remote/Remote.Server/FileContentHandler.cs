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

namespace Colosoft.Net.Remote.Server
{
	/// <summary>
	/// Implementação do manipulador para acessar o conteúdo de arquivos
	/// conditos no repositório associado.
	/// </summary>
	public abstract class FileContentHandler : System.Web.IHttpHandler
	{
		/// <summary>
		/// Identifica se o conteúdo do Handler é reutilizado.
		/// </summary>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Repositório dos arquivos.
		/// </summary>
		public abstract Colosoft.IO.FileRepository.IFileRepository Repository
		{
			get;
		}

		/// <summary>
		/// Identifica se é necessário está autenticado para acessar o recurso.
		/// </summary>
		public virtual bool NeedAuthenticated
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Trata a exception.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="statusCode"></param>
		/// <param name="responseStarted"></param>
		/// <returns></returns>
		private Exception HandleException(Exception exception, int statusCode, bool responseStarted)
		{
			try
			{
				var current = System.Web.HttpContext.Current;
				if(current != null)
				{
					try
					{
						current.Response.StatusCode = statusCode;
						if(!string.IsNullOrEmpty("X-Exception"))
							current.Response.AddHeader("X-Exception", exception.GetType().Name);
						current.Response.ContentType = "text/plain";
						var builder = new StringBuilder();
						builder.AppendFormat("--> ({0}) ", exception.GetType().FullName).AppendLine(exception.Message);
						current.Response.Write(builder.ToString());
						current.Response.Flush();
					}
					catch(Exception)
					{
						responseStarted = false;
					}
				}
			}
			catch(Exception)
			{
			}
			finally
			{
				if(responseStarted && (System.Web.HttpContext.Current != null))
					System.Web.HttpContext.Current.Response.Flush();
			}
			return exception;
		}

		/// <summary>
		/// Processa a requisição realizada.
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(System.Web.HttpContext context)
		{
			try
			{
				Colosoft.Net.SecurityToken.SecurityTokenHttpRequest.ValidateResult result = null;
				if(NeedAuthenticated)
					result = Colosoft.Net.SecurityToken.SecurityTokenHttpRequest.Validate(context.Request.Headers);
				if(NeedAuthenticated && !result.Success)
				{
					HandleException(result.Exception ?? new Exception(result.Message.Format()), 401, false);
					return;
				}
				else
				{
					var path = context.Request["path"];
					if(string.IsNullOrEmpty(path))
					{
						context.Response.StatusCode = 404;
						return;
					}
					var item = Repository.GetItem(path, IO.FileRepository.ItemType.File);
					if(item != null && item.CanRead)
					{
						var buffer = new byte[1024];
						var read = 0;
						context.Response.ContentType = Colosoft.Web.ExtendedHtmlUtility.TranslateContentType(System.IO.Path.GetExtension(item.FullName));
						var outputStream = context.Response.OutputStream;
						using (var stream = item.OpenRead())
							while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
								outputStream.Write(buffer, 0, read);
						outputStream.Flush();
					}
					else
					{
						context.Response.StatusCode = 404;
					}
				}
			}
			catch(Exception ex)
			{
				HandleException(ex, 500, false);
			}
		}
	}
}
