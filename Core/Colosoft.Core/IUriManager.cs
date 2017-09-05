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
using System.IO;
using System.Reflection;

namespace Colosoft
{
	/// <summary>
	/// Assinatura da classe responsável por realizar o gerenciamento das Uris.
	/// </summary>
	public interface IUriManager
	{
		/// <summary>
		/// Obtém uma Stream do recurso a partir da sua URI.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <returns>Stream do recurso.</returns>
		/// <example>
		/// Absolute Pack - Resource File: "pack://application:,,,/Folder/File.bin"
		/// Absolute Pack - Content File: "pack://application:,,,/Folder/File.bin"
		/// Relative - (Resource -> Pack Content -> Pack Resource) Resource: "/Folder/Resource.bin"
		/// </example>
		Stream GetApplicationStream(Uri uri);

		/// <summary>
		/// Obtém uma Stream do recurso a partir da sua URI.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <returns>Stream do recurso.</returns>
		/// <example>
		/// Absolute Pack - Site of Origin File: "pack://siteoforigin:,,,/Folder/File.bin"
		/// Relative Pack - Site of Origin File: "/Folder/Resource.bin"
		/// </example>
		Stream GetRemoteStream(Uri uri);

		/// <summary>
		/// Obtém uma Stream do recurso a partir da sua URI.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <param name="assembly">Assembly padrão.</param>
		/// <returns>Stream do recurso.</returns>
		/// <example>
		/// Absolute Pack - Zip File: "pack://zip:,,,C:\\Folder\\Package.package/Folder/File.bin"
		/// Relative - (Resource -> Pack Content -> Pack Resource -> Pack Site of Origin) Zip File: "pack://zip:,,,Folder\\Resource.package/Folder/File.bin"
		/// </example>
		Stream GetPackZipStream(Uri uri, Assembly assembly);

		/// <summary>
		/// Obtém um Stream do recurso a partir da sua URI.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <returns>Stream do recurso.</returns>
		Stream GetStream(Uri uri);
	}
	/// <summary>
	/// Classe de acesso ao UriManager.
	/// </summary>
	public static class UriManager
	{
		private static IUriManager _uriManager;

		/// <summary>
		/// Instancia do gerenciador de Uri do sistema.
		/// </summary>
		public static IUriManager Current
		{
			get
			{
				if(_uriManager == null)
					_uriManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IUriManager>();
				return _uriManager;
			}
			set
			{
				_uriManager = value;
			}
		}
	}
}
