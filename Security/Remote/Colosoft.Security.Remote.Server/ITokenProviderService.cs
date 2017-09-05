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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Assinatura do serviço de provedor de token
	/// </summary>
	[ServiceContract]
	public interface ITokenProviderService
	{
		/// <summary>
		/// Verifica se o token é válido.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <returns>True caso seja válido.</returns>
		[OperationContract]
		TokenConsultResult Check(string token);

		/// <summary>
		/// Define o perfil para o token
		/// </summary>
		/// <param name="token">Token</param>
		/// <param name="profileId">Informações do perfil.</param>
		/// <returns></returns>
		[OperationContract]
		TokenSetProfileResult SetProfile(string token, int profileId);

		/// <summary>
		/// Executa uma verificação do token no servidor.
		/// </summary>
		/// <param name="token">Token</param>
		/// <returns></returns>
		[OperationContract]
		TokenPingResult Ping(string token);

		/// <summary>
		/// Marca mensagens como lidas.
		/// </summary>
		/// <param name="dispatcherIds">Identificadores dos despachos.</param>
		[OperationContract]
		void MarkMessageAsRead(IEnumerable<int> dispatcherIds);

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		[OperationContract]
		void CloseUserTokens(int userId);

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário.</param>
		/// <param name="applicationName">Nome da aplicação associada com o token.</param>
		[OperationContract]
		void CloseUserTokens2(int userId, string applicationName);
	}
}
