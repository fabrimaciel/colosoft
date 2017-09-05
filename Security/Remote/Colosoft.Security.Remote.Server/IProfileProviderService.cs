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
using Colosoft.Security.Profile;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Representa o serviço do provedor de perfis.
	/// </summary>
	[ServiceContract]
	public interface IProfileProviderService
	{
		/// <summary>
		/// Apaga os perfis dos usuários informados.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários que terão seus perfis apagados.</param>
		/// <returns></returns>
		[OperationContract]
		int DeleteProfiles(string[] usernames);

		/// <summary>
		/// Apaga os perfis informados.
		/// </summary>
		/// <param name="profiles">Informações dos perfis que serão apagados.</param>
		/// <returns></returns>
		[OperationContract]
		int DeleteProfilesByProfileInfo(ProfileInfoWrapper[] profiles);

		/// <summary>
		/// Pesquisa os perfis associados com o nome do usuário informado.
		/// </summary>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		[OperationContract]
		ProfileInfoWrapper[] FindProfilesByUserName(string usernameToMatch);

		/// <summary>
		/// Recupera os perfis associados com o usuário informado.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		[OperationContract]
		ProfileInfoWrapper[] GetUserProfiles(string userName);

		/// <summary>
		/// Recupera os dados do perfil.
		/// </summary>
		/// <param name="info">Informações usadas para recuperar o perfil.</param>
		/// <returns></returns>
		[OperationContract]
		Profile GetProfile(ProfileInfoWrapper info);

		/// <summary>
		/// Recupera a origem do perfil
		/// </summary>
		/// <param name="sourceId">Identificador da origem.</param>
		/// <returns></returns>
		[OperationContract]
		AuthenticationSource GetSource(int sourceId);

		/// <summary>
		/// Define o valor da propriedade do perfil.
		/// </summary>
		/// <param name="info">Informações do perfil.</param>
		/// <param name="property">Dados da propriedade.</param>
		/// <param name="propertyValue">Valor da propriedade.</param>
		[OperationContract]
		void SetProfilePropertyValue(ProfileInfoWrapper info, ProfilePropertyDefinition property, string propertyValue);

		/// <summary>
		/// Recupera as definições das propriedades dos perfis do sistema.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		ProfilePropertyDefinition[] GetProfilePropertyDefinitions();
	}
}
