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

namespace Colosoft.Security
{
	/// <summary>
	/// Objeto com o resultado da consulta do token
	/// </summary>
	public class TokenConsultResult
	{
		/// <summary>
		/// Se existe o token
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem associada com o resultado.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do usuário
		/// </summary>
		public int UserId
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do usuário associado com o token.
		/// </summary>
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do perfil associado com o token.
		/// </summary>
		public string ProfileName
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do perfil associado.
		/// </summary>
		public int ProfileId
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do origem de autenticação do perfil.
		/// </summary>
		public string ProfileSourceName
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador único da origem do perfil.
		/// </summary>
		public int ProfileSourceUid
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a origem do perfil está ativa.
		/// </summary>
		public bool ProfileSourceIsActive
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera as informações do perfil associado.
		/// </summary>
		/// <returns></returns>
		public Security.Profile.ProfileInfo GetProfileInfo()
		{
			if(ProfileId == 0)
				return null;
			return new Profile.ProfileInfo {
				FullName = ProfileName,
				IsAnonymous = false,
				ProfileId = ProfileId,
				Source = new AuthenticationSource {
					Uid = ProfileSourceUid,
					FullName = ProfileSourceName,
					IsActive = ProfileSourceIsActive
				},
				UserName = UserName
			};
		}
	}
}
