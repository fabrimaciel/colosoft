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

namespace Colosoft.Owin.Server.Configuration
{
	/// <summary>
	/// Configuração da autenticação.
	/// </summary>
	static class AuthenticationConfig
	{
		private static System.Web.Configuration.AuthenticationMode? s_explicitMode;

		/// <summary>
		/// Modo de autenticação.
		/// </summary>
		internal static System.Web.Configuration.AuthenticationMode Mode
		{
			get
			{
				if(s_explicitMode.HasValue)
					return s_explicitMode.Value;
				return System.Web.Configuration.AuthenticationMode.Forms;
			}
			set
			{
				s_explicitMode = new System.Web.Configuration.AuthenticationMode?(value);
			}
		}

		/// <summary>
		/// Identifica se está acessando a página de login.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="loginUrl"></param>
		/// <returns></returns>
		internal static bool AccessingLoginPage(HttpContext context, string loginUrl)
		{
			if(!string.IsNullOrEmpty(loginUrl))
			{
				loginUrl = GetCompleteLoginUrl(context, loginUrl);
				if(string.IsNullOrEmpty(loginUrl))
				{
					return false;
				}
				int index = loginUrl.IndexOf('?');
				if(index >= 0)
				{
					loginUrl = loginUrl.Substring(0, index);
				}
				string path = context.Request.Path;
				if(StringUtil.EqualsIgnoreCase(path, loginUrl))
				{
					return true;
				}
				if(loginUrl.IndexOf('%') >= 0)
				{
					string str2 = System.Web.HttpUtility.UrlDecode(loginUrl);
					if(StringUtil.EqualsIgnoreCase(path, str2))
					{
						return true;
					}
					str2 = System.Web.HttpUtility.UrlDecode(loginUrl, context.Request.ContentEncoding);
					if(StringUtil.EqualsIgnoreCase(path, str2))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera a url completa do login.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="loginUrl"></param>
		/// <returns></returns>
		internal static string GetCompleteLoginUrl(HttpContext context, string loginUrl)
		{
			if(string.IsNullOrEmpty(loginUrl))
			{
				return string.Empty;
			}
			return loginUrl;
		}
	}
}
