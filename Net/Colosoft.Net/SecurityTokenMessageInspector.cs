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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Colosoft.Net
{
	/// <summary>
	/// Classe responsável por inspencionar o envia das mensagens com token de segurança.
	/// </summary>
	public class SecurityTokenMessageInspector : IDispatchMessageInspector, IClientMessageInspector
	{
		/// <summary>
		/// Nome do cabeçalho do token.
		/// </summary>
		public const string TokenHeaderName = "x-user-token";

		/// <summary>
		/// Namespace do usado nas mensagens da instancia..
		/// </summary>
		public const string Namespace = "http://www.Colosoft.com.br/2011/security";

		/// <summary>
		/// Código que identifica a falha devido ao token inválido.
		/// </summary>
		public const string InvalidTokenFaultReasonCode = "InvalidToken";

		/// <summary>
		/// Código que identifica que o token recebido pelo servidor estava vazio.
		/// </summary>
		public const string EmptyTokenFaultReasonCode = "EmptyToken";

		/// <summary>
		/// Configura o token valido.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="checkResult">Resultado da verificação do </param>
		protected virtual void ConfigureValidToken(string token, Colosoft.Security.TokenConsultResult checkResult)
		{
			Colosoft.Security.UserContext.Current.SetAuth(checkResult.UserName, token);
			if(checkResult.ProfileId > 0)
			{
				Colosoft.Security.Profile.ProfileManager.SetCurrentProfile(new Lazy<Colosoft.Security.Profile.ProfileInfo>(() => checkResult.GetProfileInfo()), true);
			}
		}

		/// <summary>
		/// Método usado para notifica que o cabeçalho do token não foi encontrado.
		/// </summary>
		protected virtual void NotifyTokenHeaderNotFound()
		{
		}

		/// <summary>
		/// Método acionado depois de receber a mensagem do serviço.
		/// </summary>
		/// <param name="request">Dados da requisição da mensage.</param>
		/// <param name="channel">Canal usado na comunicação.</param>
		/// <param name="instanceContext">Contexto da instancia.</param>
		/// <returns></returns>
		public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
		{
			string token = null;
			if(request.Properties.ContainsKey("httpRequest"))
			{
				var prop = (HttpRequestMessageProperty)request.Properties["httpRequest"];
				token = prop.Headers[TokenHeaderName];
			}
			if(string.IsNullOrEmpty(token))
			{
				var tokenHeaderIndex = request.Headers.FindHeader(TokenHeaderName, Namespace);
				if(tokenHeaderIndex >= 0)
				{
					var headerReader = request.Headers.GetReaderAtHeader(tokenHeaderIndex);
					token = headerReader.ReadElementContentAsString();
				}
			}
			if(string.IsNullOrEmpty(token))
			{
				var reason = new System.ServiceModel.FaultReason(ResourceMessageFormatter.Create(() => Properties.Resources.FaultException_EmptyToken).Format(System.Globalization.CultureInfo.CurrentCulture));
				var code = new System.ServiceModel.FaultCode(EmptyTokenFaultReasonCode, Namespace);
				throw new System.ServiceModel.FaultException(reason, code);
			}
			Colosoft.Security.TokenConsultResult checkResult = null;
			try
			{
				checkResult = Colosoft.Security.Tokens.Check(token);
			}
			catch(Exception ex)
			{
				var message = ResourceMessageFormatter.Create(() => Properties.Resources.Exception_TokenCheckingError, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true)).Format();
				throw new Exception(message, ex);
			}
			if(checkResult == null || !checkResult.Success)
			{
				var reason = new System.ServiceModel.FaultReason(ResourceMessageFormatter.Create(() => Properties.Resources.FaultException_InvalidToken).Format(System.Globalization.CultureInfo.CurrentCulture) + " " + checkResult.Message);
				var code = new System.ServiceModel.FaultCode(InvalidTokenFaultReasonCode, Namespace);
				throw new System.ServiceModel.FaultException(reason, code);
			}
			ConfigureValidToken(token, checkResult);
			return null;
		}

		/// <summary>
		/// Método acionado antes de enviar uma resposta.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
		}

		/// <summary>
		/// Processa a respota recebida.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if(reply.IsFault)
			{
				var buffer = reply.CreateBufferedCopy(Int32.MaxValue);
				var copy = buffer.CreateMessage();
				reply = buffer.CreateMessage();
				var messageFault = MessageFault.CreateFault(copy, 0x10000);
				if((messageFault.Code.Name == InvalidTokenFaultReasonCode || messageFault.Code.Name == EmptyTokenFaultReasonCode) && messageFault.Code.Namespace == Namespace)
					throw new InvalidSecurityTokenException(messageFault.Reason.ToString(), FaultException.CreateFault(messageFault));
				throw FaultException.CreateFault(messageFault);
			}
		}

		/// <summary>
		/// Processa a requisição antes de enviar.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
		{
			var identity = Colosoft.Security.UserContext.Current.Principal.Identity as Colosoft.Security.Principal.DefaultIdentity;
			if(identity != null && identity.IsAuthenticated)
			{
				if(identity.IsAuthenticated && string.IsNullOrEmpty(identity.Token))
				{
					return null;
				}
				object httpRequestMessageObject;
				if(request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
				{
					var httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
					if(string.IsNullOrWhiteSpace(httpRequestMessage.Headers[TokenHeaderName]))
						httpRequestMessage.Headers[TokenHeaderName] = identity.Token;
				}
				else
				{
					var httpRequestMessage = new HttpRequestMessageProperty();
					httpRequestMessage.Headers.Add(TokenHeaderName, identity.Token);
					request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
				}
				var tokenHeader = System.ServiceModel.Channels.MessageHeader.CreateHeader(TokenHeaderName, Namespace, identity.Token);
				request.Headers.Add(tokenHeader);
			}
			return null;
		}
	}
}
