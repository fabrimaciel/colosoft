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
using Colosoft.Web;

namespace Colosoft.Net.Remote.Server
{
	/// <summary>
	/// Manipuladro do arquivos enviados pelo Uploader.
	/// </summary>
	public class UploaderHandler : System.Web.IHttpHandler
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
		/// Processa os dados recebidos pelo upload.
		/// </summary>
		/// <param name="item"></param>
		protected virtual void ProcessUpload(Colosoft.Net.IUploaderItem item)
		{
		}

		/// <summary>
		/// Processa a requisição realizada.
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Request.RemoveValidateRequest();
			try
			{
				var result = Colosoft.Net.SecurityToken.SecurityTokenHttpRequest.Validate(context.Request.Headers);
				if(!result.Success)
					HandleException(result.Exception ?? new Exception(result.Message.Format()), 401, false);
				else
				{
					var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Colosoft.Net.UploaderItemInfo));
					Colosoft.Net.UploaderItemInfo info = null;
					using (var stream = new System.IO.StringReader(context.Request.Form["info"]))
						info = (Colosoft.Net.UploaderItemInfo)serializer.Deserialize(stream);
					var dataFile = context.Request.Files["data"];
					if(dataFile != null)
					{
						var item = new Colosoft.Net.UploaderStreamItem(dataFile.InputStream);
						if(info != null)
							foreach (var attr in info.Attributes)
								item.Attributes.Add(attr.Name, attr.Value);
						ProcessUpload(item);
					}
				}
			}
			catch(Exception ex)
			{
				HandleException(ex, 200, false);
			}
		}
	}
}
