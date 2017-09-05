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

namespace Colosoft.Web.Profile
{
	/// <summary>
	/// Implementação do provedor de perfis.
	/// </summary>
	public class DefaultProfileProvider : System.Web.Profile.ProfileProvider
	{
		/// <summary>
		/// Apaga os perfis inativos.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <returns></returns>
		public override int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Apaga os perfis associados com os nomes de usuários informados.
		/// </summary>
		/// <param name="usernames"></param>
		/// <returns></returns>
		public override int DeleteProfiles(string[] usernames)
		{
			return Colosoft.Security.Profile.ProfileManager.Provider.DeleteProfiles(usernames);
		}

		/// <summary>
		/// Apaga os perfis informados.
		/// </summary>
		/// <param name="profiles"></param>
		/// <returns></returns>
		public override int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Localiza perfis inativos pelo nome do usuário.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="usernameToMatch"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Localiza perfis pelo nome do usuário.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="usernameToMatch"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera todos os perfis inativos.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera todos os perfis.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override System.Web.Profile.ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o número de perfis inativos.
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <returns></returns>
		public override int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Nome da aplicação.
		/// </summary>
		public override string ApplicationName
		{
			get
			{
				return "Colosoft";
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Recupera os valores das propriedades do perfil.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override System.Configuration.SettingsPropertyValueCollection GetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection collection)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Define os valores das propriedades do perfil.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="collection"></param>
		public override void SetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyValueCollection collection)
		{
			throw new NotImplementedException();
		}
	}
}
