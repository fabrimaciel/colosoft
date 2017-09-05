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

namespace Colosoft.IO.IsolatedStorage
{
	/// <summary>
	/// Armazena as informações do armazenamento isolado.
	/// </summary>
	public class IsolatedStorage
	{
		private static string _applicationName = "Colosoft";

		/// <summary>
		/// Nome da aplicação representada pelo IsolatedStorage.
		/// </summary>
		public static string ApplicationName
		{
			get
			{
				return _applicationName;
			}
			set
			{
				_applicationName = value;
			}
		}

		/// <summary>
		/// Diretório isolado do o contexto da autentidação.
		/// </summary>
		public static string AuthenticationContextDirectory
		{
			get
			{
				return System.IO.Path.Combine(SystemDirectory, "AuthData", string.IsNullOrEmpty(Colosoft.Security.Authentication.AuthenticationService.Name) ? "Default" : Colosoft.Text.StringExtensions.NormalizeStringForUrl(Colosoft.Security.Authentication.AuthenticationService.Name));
			}
		}

		/// <summary>
		/// Diretório do sistema.
		/// </summary>
		public static string SystemDirectory
		{
			get
			{
				return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _applicationName);
			}
		}

		/// <summary>
		/// Diretório do perfil do usuário.
		/// </summary>
		public static string UserProfileDirectory
		{
			get
			{
				return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _applicationName);
			}
		}
	}
}
