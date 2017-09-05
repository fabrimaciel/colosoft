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

namespace Colosoft.Owin.Server.Security
{
	internal sealed class CookielessHelperClass
	{
		private HttpContext _Context;

		private string _Headers;

		private string _OriginalHeaders;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="context"></param>
		internal CookielessHelperClass(HttpContext context)
		{
			_Context = context;
		}

		/// <summary>
		/// Verifica se o valor do cookie existe no original.
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		internal bool DoesCookieValueExistInOriginal(char identifier)
		{
			int startPos = 0;
			int endPos = 0;
			this.Init();
			return GetValueStartAndEnd(_OriginalHeaders, identifier, out startPos, out endPos);
		}

		/// <summary>
		/// Redireciona com detecção.
		/// </summary>
		/// <param name="redirectPath"></param>
		internal void RedirectWithDetection(string redirectPath)
		{
			Init();
			if(string.IsNullOrEmpty(redirectPath))
				redirectPath = _Context.Request.RawUrl;
			if(redirectPath.IndexOf("?", StringComparison.Ordinal) > 0)
				redirectPath = redirectPath + "&AspxAutoDetectCookieSupport=1";
			else
				redirectPath = redirectPath + "?AspxAutoDetectCookieSupport=1";
			_Context.Response.Cookies.Add(new System.Web.HttpCookie("AspxAutoDetectCookieSupport", "1"));
			_Context.Response.Redirect(redirectPath, true);
		}

		/// <summary>
		/// Redireciona com detecção se for requerido.
		/// </summary>
		/// <param name="redirectPath"></param>
		/// <param name="cookieMode"></param>
		internal void RedirectWithDetectionIfRequired(string redirectPath, System.Web.HttpCookieMode cookieMode)
		{
			Init();
			if((cookieMode == System.Web.HttpCookieMode.AutoDetect) && (_Context.Request.Browser.Cookies && _Context.Request.Browser.SupportsRedirectWithCookie))
			{
				string cookieValue = GetCookieValue('X');
				if((cookieValue == null) || (cookieValue != "1"))
				{
					string str2 = _Context.Request.Headers["Cookie"];
					if(string.IsNullOrEmpty(str2))
					{
						string str3 = _Context.Request.QueryString["AspxAutoDetectCookieSupport"];
						if((str3 != null) && (str3 == "1"))
							SetCookieValue('X', "1");
						else
							RedirectWithDetection(redirectPath);
					}
				}
			}
		}

		/// <summary>
		/// Recupera o valor do cookie.
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		internal string GetCookieValue(char identifier)
		{
			int startPos = 0;
			int endPos = 0;
			Init();
			if(!GetValueStartAndEnd(_Headers, identifier, out startPos, out endPos))
				return null;
			return _Headers.Substring(startPos, endPos - startPos);
		}

		/// <summary>
		/// Remove os valores sem cookie do caminho.
		/// </summary>
		internal void RemoveCookielessValuesFromPath()
		{
		}

		/// <summary>
		/// Define o valor do cookie.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="cookieValue"></param>
		internal void SetCookieValue(char identifier, string cookieValue)
		{
			int startPos = 0;
			int endPos = 0;
			Init();
			while (GetValueStartAndEnd(_Headers, identifier, out startPos, out endPos))
			{
				_Headers = _Headers.Substring(0, startPos - 2) + _Headers.Substring(endPos + 1);
			}
			if(!string.IsNullOrEmpty(cookieValue))
			{
				char[] chArray1 = new char[] {
					identifier,
					'('
				};
				_Headers = _Headers + new string(chArray1) + cookieValue + ")";
			}
			if(_Headers.Length > 0)
			{
			}
			else
			{
			}
		}

		/// <summary>
		/// Idenrifica se trabalha sem cookie.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="doRedirect"></param>
		/// <param name="cookieMode"></param>
		/// <returns></returns>
		internal static bool UseCookieless(HttpContext context, bool doRedirect, System.Web.HttpCookieMode cookieMode)
		{
			switch(cookieMode)
			{
			case System.Web.HttpCookieMode.UseUri:
				return true;
			case System.Web.HttpCookieMode.UseCookies:
				return false;
			case System.Web.HttpCookieMode.AutoDetect:
				if(context == null)
				{
					context = HttpContext.Current;
				}
				if(context == null)
				{
					return false;
				}
				if(context.Request.Browser.Cookies && context.Request.Browser.SupportsRedirectWithCookie)
				{
					string cookieValue = context.CookielessHelper.GetCookieValue('X');
					if((cookieValue != null) && (cookieValue == "1"))
					{
						return true;
					}
					string str2 = context.Request.Headers["Cookie"];
					if(string.IsNullOrEmpty(str2))
					{
						string str3 = context.Request.QueryString["AspxAutoDetectCookieSupport"];
						if((str3 != null) && (str3 == "1"))
						{
							context.CookielessHelper.SetCookieValue('X', "1");
							return true;
						}
						if(doRedirect)
						{
							context.CookielessHelper.RedirectWithDetection(null);
						}
					}
					return false;
				}
				return true;
			case System.Web.HttpCookieMode.UseDeviceProfile:
				if(context == null)
				{
					context = HttpContext.Current;
				}
				if(context == null)
				{
					return false;
				}
				return (!context.Request.Browser.Cookies || !context.Request.Browser.SupportsRedirectWithCookie);
			}
			return false;
		}

		/// <summary>
		/// Recupera os valores sem cookie do cabeçalho.
		/// </summary>
		private void GetCookielessValuesFromHeader()
		{
			_Headers = _Context.Request.Headers["AspFilterSessionId"];
			_OriginalHeaders = _Headers;
			if(!string.IsNullOrEmpty(_Headers))
			{
				if((_Headers.Length == 0x18) && !_Headers.Contains("("))
				{
					_Headers = null;
				}
				else
				{
				}
			}
		}

		/// <summary>
		/// Recupera o valor de início de fim.
		/// </summary>
		/// <param name="headers"></param>
		/// <param name="identifier"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		/// <returns></returns>
		private static bool GetValueStartAndEnd(string headers, char identifier, out int startPos, out int endPos)
		{
			if(string.IsNullOrEmpty(headers))
			{
				startPos = endPos = -1;
				return false;
			}
			char[] chArray1 = new char[] {
				identifier,
				'('
			};
			string str = new string(chArray1);
			startPos = headers.IndexOf(str, StringComparison.Ordinal);
			if(startPos < 0)
			{
				startPos = endPos = -1;
				return false;
			}
			startPos += 2;
			endPos = headers.IndexOf(')', startPos);
			if(endPos < 0)
			{
				startPos = endPos = -1;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Inicializa.
		/// </summary>
		private void Init()
		{
			if(_Headers == null)
			{
				if(_Headers == null)
					GetCookielessValuesFromHeader();
				if(_Headers == null)
					RemoveCookielessValuesFromPath();
				if(_Headers == null)
					_Headers = string.Empty;
				_OriginalHeaders = _Headers;
			}
		}

		/// <summary>
		/// Verifica se o cabeçalho.
		/// </summary>
		/// <param name="path">Caminho.</param>
		/// <param name="startPos">Posição inicial.</param>
		/// <param name="endPos">Posição final.</param>
		/// <returns></returns>
		private static bool IsValidHeader(string path, int startPos, int endPos)
		{
			if((endPos - startPos) >= 3)
			{
				while (startPos <= (endPos - 3))
				{
					if((path[startPos] < 'A') || (path[startPos] > 'Z'))
					{
						return false;
					}
					if(path[startPos + 1] != '(')
					{
						return false;
					}
					startPos += 2;
					bool flag = false;
					while (startPos < endPos)
					{
						if(path[startPos] == ')')
						{
							startPos++;
							flag = true;
							break;
						}
						if(path[startPos] == '/')
						{
							return false;
						}
						startPos++;
					}
					if(!flag)
					{
						return false;
					}
				}
				if(startPos < endPos)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
