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
using System.Web.Services.Protocols;

namespace Colosoft.Diagnostics
{
	/// <summary>
	/// Classe que auxilia na formatação de mensagens de erro.
	/// </summary>
	public static class ExceptionFormatter
	{
		/// <summary>
		/// Forma o dicionário de dados.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		private static string FormatDataDictionary(Exception exception)
		{
			if(exception == null || exception.Data == null || exception.Data.Count <= 0)
				return string.Empty;
			var builder = new StringBuilder();
			foreach (System.Collections.DictionaryEntry entry in exception.Data)
				builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.ExceptionDataDictionaryReport, entry.Key, entry.Value).Format());
			return ResourceMessageFormatter.Create(() => Properties.Resources.LogExceptionDataDictionary, builder.ToString()).Format();
		}

		/// <summary>
		/// Formata um erro.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="useBriefFormat">Identifica se é para usar um pequena descrição.</param>
		/// <returns></returns>
		private static string FormatOneException(Exception exception, bool useBriefFormat)
		{
			StringBuilder builder = new StringBuilder(ResourceMessageFormatter.Create(() => Properties.Resources.LogExceptionHeader, exception.Message, exception.GetType().Name).Format());
			if(!useBriefFormat)
			{
				if(exception is WebException)
				{
					WebException exception3 = exception as WebException;
					HttpWebResponse response = exception3.Response as HttpWebResponse;
					if(response != null)
					{
						try
						{
							string str = response.Headers["X-ServiceError"];
							string str2 = string.IsNullOrEmpty(str) ? response.StatusDescription : Colosoft.Web.HttpUtility.UrlDecode(str);
							builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.WebExceptionReport, exception3.Status, response.StatusCode, str2).Format());
						}
						catch(ObjectDisposedException)
						{
							builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.WebExceptionReport_ResponseDisposed, exception3.Status).Format());
						}
					}
				}
				else if(exception is SoapException)
				{
					SoapException exception4 = exception as SoapException;
					if(exception4.Detail != null)
					{
						builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.SoapExceptionReport, exception4.Detail.OuterXml).Format());
					}
				}
				else
				{
					builder.AppendLine();
				}
				builder.AppendLine(FormatDataDictionary(exception));
				builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.ExceptionStackTrace, exception.StackTrace).Format());
			}
			else
			{
				var firstLineIndex = exception == null ? 0 : exception.StackTrace.IndexOf('\r');
				if(firstLineIndex >= 0)
					builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.ExceptionStackTrace, exception.StackTrace.Substring(0, firstLineIndex) + "...").Format());
			}
			return builder.ToString();
		}

		/// <summary>
		/// Recupera a descrição do método alvo onde o erro foi disparado
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetTargetSiteDescription(Exception ex)
		{
			var method = ex.TargetSite;
			return string.Format("{0}.{1}({2})", method.DeclaringType.FullName, method.Name, string.Join(", ", method.GetParameters().Select(f => string.Format("{0} {1}", f.ParameterType.FullName, f.Name)).ToArray()));
		}

		/// <summary>
		/// Formata erros.
		/// </summary>
		/// <param name="exceptions"></param>
		/// <returns></returns>
		public static string FormatExceptions(IEnumerable<Exception> exceptions)
		{
			if(exceptions == null || exceptions.Count() == 0)
				return string.Empty;
			var builder = new StringBuilder();
			foreach (var exception in exceptions)
			{
				builder.Append(FormatOneException(exception, false));
				for(Exception exception2 = exception.InnerException; exception2 != null; exception2 = exception2.InnerException)
				{
					if(exception2 != null)
					{
						builder.AppendLine();
						builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.InnerException).Format());
						builder.Append(FormatOneException(exception2, false));
					}
				}
				builder.AppendLine();
			}
			return builder.ToString();
		}

		/// <summary>
		/// Formata um erro.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="useBriefFormat">Identifica se é para usar um pequena descrição.</param>
		/// <returns></returns>
		public static string FormatException(Exception exception, bool useBriefFormat)
		{
			if(exception == null)
				return string.Empty;
			var builder = new StringBuilder(FormatOneException(exception, useBriefFormat));
			if(!useBriefFormat)
			{
				for(Exception exception2 = exception.InnerException; exception2 != null; exception2 = exception2.InnerException)
				{
					if(exception2 != null)
					{
						builder.AppendLine();
						builder.AppendLine(ResourceMessageFormatter.Create(() => Properties.Resources.InnerException).Format());
						builder.Append(FormatOneException(exception2, false));
					}
				}
			}
			return builder.ToString();
		}
	}
}
