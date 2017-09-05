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

namespace Colosoft.Net.SecurityToken
{
	/// <summary>
	/// Classe responsável por gerenciar o token de seguraça
	/// em um requisição http.
	/// </summary>
	public class SecurityTokenHttpRequest
	{
		/// <summary>
		/// Armazena os dados do resultado da validação.
		/// </summary>
		public class ValidateResult
		{
			/// <summary>
			/// Identifica se a validação foi realizada com sucesso.
			/// </summary>
			public bool Success
			{
				get;
				set;
			}

			/// <summary>
			/// Mensagem do resultado.
			/// </summary>
			public IMessageFormattable Message
			{
				get;
				set;
			}

			/// <summary>
			/// Error ocorrido.
			/// </summary>
			public Exception Exception
			{
				get;
				set;
			}

			/// <summary>
			/// Resultado da consulta do token.
			/// </summary>
			public Colosoft.Security.TokenConsultResult TokenConsultResult
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Nome do cabeçalho do token.
		/// </summary>
		public const string TokenHeaderName = "X-Token";

		/// <summary>
		/// Valida o token informado.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static ValidateResult Validate(string token)
		{
			if(string.IsNullOrEmpty(token))
			{
				return new ValidateResult {
					Success = false,
					Message = ResourceMessageFormatter.Create(() => Properties.Resources.FaultException_EmptyToken)
				};
			}
			Colosoft.Security.TokenConsultResult checkResult = null;
			try
			{
				checkResult = Colosoft.Security.Tokens.Check(token);
			}
			catch(Exception ex)
			{
				var message = ResourceMessageFormatter.Create(() => Properties.Resources.Exception_TokenCheckingError, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true));
				return new ValidateResult {
					Success = false,
					Exception = ex,
					Message = message
				};
			}
			if(checkResult == null || !checkResult.Success)
			{
				return new ValidateResult {
					Success = false,
					Message = ResourceMessageFormatter.Create(() => Properties.Resources.FaultException_InvalidToken, checkResult.Message)
				};
			}
			try
			{
				Colosoft.Security.UserContext.Current.SetAuth(checkResult.UserName, token);
				if(checkResult.ProfileId > 0)
				{
					Colosoft.Security.Profile.ProfileManager.SetCurrentProfile(new Lazy<Colosoft.Security.Profile.ProfileInfo>(() => checkResult.GetProfileInfo()), true);
				}
			}
			catch(Exception ex)
			{
				return new ValidateResult {
					Success = false,
					Exception = ex,
					Message = ResourceMessageFormatter.Create(() => Properties.Resources.FaultException_SetAuthError)
				};
			}
			return new ValidateResult {
				Success = true,
				TokenConsultResult = checkResult
			};
		}

		/// <summary>
		/// Valida os dados do token que estão na requisição.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static ValidateResult Validate(System.ServiceModel.Web.IncomingWebRequestContext request)
		{
			request.Require("request").NotNull();
			var httpHeader = request.Headers;
			var tokenHash = httpHeader[TokenHeaderName];
			string token = null;
			if(!string.IsNullOrEmpty(tokenHash))
				try
				{
					token = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(tokenHash));
				}
				catch
				{
				}
			else
				token = httpHeader[SecurityTokenMessageInspector.TokenHeaderName];
			return Validate(token);
		}

		/// <summary>
		/// Valida o token contido no cabeçalho da requisição http.
		/// </summary>
		/// <param name="httpHeader"></param>
		/// <returns></returns>
		public static ValidateResult Validate(System.Collections.Specialized.NameValueCollection httpHeader)
		{
			httpHeader.Require("httpHeader").NotNull();
			var tokenHash = httpHeader[TokenHeaderName];
			string token = null;
			if(!string.IsNullOrEmpty(tokenHash))
				try
				{
					token = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(tokenHash));
				}
				catch
				{
				}
			else
				token = httpHeader[SecurityTokenMessageInspector.TokenHeaderName];
			return Validate(token);
		}

		/// <summary>
		/// Registra o token da resposta informada.
		/// </summary>
		/// <param name="response"></param>
		public static void RegisterToken(System.ServiceModel.Web.OutgoingWebResponseContext response)
		{
			if(response == null)
				return;
			var identity = Colosoft.Security.UserContext.Current.Principal.Identity as Colosoft.Security.Principal.DefaultIdentity;
			if(identity != null)
			{
				if(identity.IsAuthenticated && string.IsNullOrEmpty(identity.Token))
					throw new InvalidOperationException("Identity token is empty.");
				RegisterToken(response, identity.Token);
			}
		}

		/// <summary>
		/// Registra o token da resposta informada.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="token">Token que será registrado.</param>
		public static void RegisterToken(System.ServiceModel.Web.OutgoingWebResponseContext response, string token)
		{
			if(response == null)
				return;
			response.Headers.Add(SecurityTokenMessageInspector.TokenHeaderName, token);
			response.Headers.Add(System.Net.HttpResponseHeader.SetCookie, string.Format("{0}={1}", SecurityTokenMessageInspector.TokenHeaderName, token));
		}

		/// <summary>
		/// Registra o token no cabeçalho http.
		/// </summary>
		/// <param name="httpHeader"></param>
		public static void RegisterToken(System.Collections.Specialized.NameValueCollection httpHeader)
		{
			var identity = Colosoft.Security.UserContext.Current.Principal.Identity as Colosoft.Security.Principal.DefaultIdentity;
			if(identity != null)
			{
				if(identity.IsAuthenticated && string.IsNullOrEmpty(identity.Token))
					throw new InvalidOperationException("Identity token is empty.");
				RegisterToken(httpHeader, identity.Token);
			}
		}

		/// <summary>
		/// Registra o token no cabeçalho http.
		/// </summary>
		/// <param name="httpHeader"></param>
		/// <param name="token">Token que será registrado.</param>
		public static void RegisterToken(System.Collections.Specialized.NameValueCollection httpHeader, string token)
		{
			httpHeader.Set(TokenHeaderName, token);
			httpHeader.Add("Set-Cookie", string.Format("{0}={1}", SecurityTokenMessageInspector.TokenHeaderName, token));
		}
	}
}
