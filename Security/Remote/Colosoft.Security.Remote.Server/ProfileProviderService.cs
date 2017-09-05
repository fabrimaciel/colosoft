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
	/// Implmentação do provedor dos perfis do sistema.
	/// </summary>
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	public class ProfileProviderService : IProfileProviderService
	{
		/// <summary>
		/// Instancia do provedor de perfis.
		/// </summary>
		private Colosoft.Security.Profile.IProfileProvider ProfileProvider
		{
			get
			{
				try
				{
					return Colosoft.Security.Profile.ProfileManager.Provider;
				}
				catch(Exception ex)
				{
					throw new DetailsException(ResourceMessageFormatter.Create(() => global::Colosoft.Security.Remote.Server.Properties.Resources.ProfileProviderService_GetProfileProviderError, ex.Message), ex);
				}
			}
		}

		/// <summary>
		/// Apaga os perfis dos usuários informados.
		/// </summary>
		/// <param name="usernames">Nomes dos usuários que terão seus perfis apagados.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public int DeleteProfiles(string[] usernames)
		{
			return ProfileProvider.DeleteProfiles(usernames);
		}

		/// <summary>
		/// Apaga os perfis informados.
		/// </summary>
		/// <param name="profiles">Informações dos perfis que serão apagados.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public int DeleteProfilesByProfileInfo(ProfileInfoWrapper[] profiles)
		{
			if(profiles == null)
				return 0;
			return ProfileProvider.DeleteProfiles(profiles.Select(f => f.GetProfileInfo()).ToArray());
		}

		/// <summary>
		/// Pesquisa os perfis associados com o nome do usuário informado.
		/// </summary>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public ProfileInfoWrapper[] FindProfilesByUserName(string usernameToMatch)
		{
			var result = ProfileProvider.FindProfilesByUserName(usernameToMatch);
			if(result == null)
				return new ProfileInfoWrapper[0];
			return result.Select(f => new ProfileInfoWrapper(f)).ToArray();
		}

		/// <summary>
		/// Recupera os perfis associados com o usuário informado.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public ProfileInfoWrapper[] GetUserProfiles(string userName)
		{
			var result = ProfileProvider.GetUserProfiles(userName);
			if(result == null)
				return new ProfileInfoWrapper[0];
			return result.Select(f => new ProfileInfoWrapper(f)).ToArray();
		}

		/// <summary>
		/// Recupera os dados do perfil.
		/// </summary>
		/// <param name="info">Informações usadas para recuperar o perfil.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public Profile GetProfile(ProfileInfoWrapper info)
		{
			if(info == null)
				return null;
			var profile = ProfileProvider.GetProfile(info.GetProfileInfo());
			return profile == null ? null : new Profile(profile);
		}

		/// <summary>
		/// Recupera a origem do perfil
		/// </summary>
		/// <param name="sourceId">Identificador da origem.</param>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public AuthenticationSource GetSource(int sourceId)
		{
			var source = ProfileProvider.GetSource(sourceId);
			if(source == null)
				return null;
			return new AuthenticationSource {
				FullName = source.FullName
			};
		}

		/// <summary>
		/// Define o valor da propriedade do perfil.
		/// </summary>
		/// <param name="info">Informações do perfil.</param>
		/// <param name="property">Dados da propriedade.</param>
		/// <param name="propertyValue">Valor da propriedade.</param>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public void SetProfilePropertyValue(ProfileInfoWrapper info, ProfilePropertyDefinition property, string propertyValue)
		{
			if(info == null)
				return;
			var profile = ProfileProvider.GetProfile(info.GetProfileInfo());
			object value = null;
			if(property.TypeDefinition != null)
			{
				if(property.TypeDefinition == typeof(string))
					value = propertyValue;
				else
				{
					var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(property.TypeDefinition);
					if(typeConverter != null)
						value = typeConverter.ConvertFromString(propertyValue);
					else
						value = propertyValue;
				}
			}
			else
				value = propertyValue;
			profile.SetPropertyValue(property.Name, value);
			ProfileProvider.SaveProfileProperties(profile);
		}

		/// <summary>
		/// Recupera as definições das propriedades dos perfis do sistema.
		/// </summary>
		/// <returns></returns>
		[System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Authenticated = true)]
		public ProfilePropertyDefinition[] GetProfilePropertyDefinitions()
		{
			return ProfileProvider.GetProfilePropertyDefinitions();
		}
	}
}
