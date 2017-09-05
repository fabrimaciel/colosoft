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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Assinatura dos métodos do provedor do perfil.
	/// </summary>
	public interface IProfileProvider
	{
		/// <summary>
		/// Nome do provedor.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Apaga os perfis dos usuários informados.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários que terão seus perfis apagados.</param>
		/// <returns></returns>
		int DeleteProfiles(string[] usernames);

		/// <summary>
		/// Apaga os perfis informados.
		/// </summary>
		/// <param name="profiles">Informações dos perfis que serão apagados.</param>
		/// <returns></returns>
		int DeleteProfiles(ProfileInfo[] profiles);

		/// <summary>
		/// Pesquisa os perfis associados com o nome do usuário informado.
		/// </summary>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		IList<ProfileInfo> FindProfilesByUserName(string usernameToMatch);

		/// <summary>
		/// Recupera os perfis associados com o usuário informado.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		IList<ProfileInfo> GetUserProfiles(string userName);

		/// <summary>
		/// Recupera os dados do perfil.
		/// </summary>
		/// <param name="info">Informações usadas para recuperar o perfil.</param>
		/// <returns></returns>
		IProfile GetProfile(ProfileInfo info);

		/// <summary>
		/// Recupera a origem do perfil
		/// </summary>
		/// <param name="sourceId">identificador da origem</param>
		/// <returns>Origem</returns>
		IAuthenticationSource GetSource(int sourceId);

		/// <summary>
		/// Recupera as definições de propriedades do perfil.
		/// </summary>
		/// <returns></returns>
		ProfilePropertyDefinition[] GetProfilePropertyDefinitions();

		/// <summary>
		/// Salva as propriedades do perfil informado.
		/// </summary>
		/// <param name="profile">Instancia do perfil.</param>
		void SaveProfileProperties(IProfile profile);
	}
}
