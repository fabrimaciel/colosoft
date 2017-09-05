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
	/// Armazena os dados do contexto do perfil de um usuário do sistema.
	/// </summary>
	[Serializable]
	class ProfileContext
	{
		[NonSerialized]
		private IProfile _profile;

		private static ProfileManager.SetCurrentProfileData _currentProfileData;

		private ProfileInfo _profileInfo;

		public object CurrentProfileDataLock = new object();

		/// <summary>
		/// Instancia do perfil associado.
		/// </summary>
		public IProfile Profile
		{
			get
			{
				return _profile;
			}
			set
			{
				_profile = value;
			}
		}

		/// <summary>
		/// Informações do perfil.
		/// </summary>
		public ProfileInfo ProfileInfo
		{
			get
			{
				if(ProfileManager.CurrentProfileInfoProvider == null)
					return _profileInfo;
				else
					return ProfileManager.CurrentProfileInfoProvider.GetProfileInfo();
			}
			set
			{
				if(ProfileManager.CurrentProfileInfoProvider == null)
					_profileInfo = value;
				else
					_profileInfo = null;
			}
		}

		/// <summary>
		/// Dados do perfil.
		/// </summary>
		public ProfileManager.SetCurrentProfileData CurrentProfileData
		{
			get
			{
				return _currentProfileData;
			}
			set
			{
				_currentProfileData = value;
			}
		}
	}
}
