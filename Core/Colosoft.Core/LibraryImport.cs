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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Armazena os dados da importação de uma biblioteca.
	/// </summary>
	[Serializable]
	public class LibraryImport
	{
		private bool _exists;

		private string _fileName;

		private string _fullPath;

		private string _version;

		/// <summary>
		/// Identifica se o arquivo existe.
		/// </summary>
		public bool Exists
		{
			get
			{
				return _exists;
			}
			set
			{
				_exists = value;
			}
		}

		/// <summary>
		/// Nome do arquivo.
		/// </summary>
		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				_fileName = value;
			}
		}

		/// <summary>
		/// Caminho completo para a biblioteca.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2141:TransparentMethodsMustNotSatisfyLinkDemandsFxCopRule")]
		public string FullPath
		{
			get
			{
				return _fullPath;
			}
			[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand)]
			set
			{
				_fullPath = value;
				if(System.IO.File.Exists(_fullPath))
				{
					var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(_fullPath);
					_version = info.FileVersion;
				}
			}
		}

		/// <summary>
		/// Versão da biblioteca.
		/// </summary>
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Cria uma instancia com o caminho do biblioteca.
		/// </summary>
		/// <param name="fullPath"></param>
		/// <param name="exists">Identifica se existe</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2141:TransparentMethodsMustNotSatisfyLinkDemandsFxCopRule"), System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand)]
		public LibraryImport(string fullPath, bool exists) : this(System.IO.Path.GetFileName(fullPath), fullPath, exists)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="fullPath"></param>
		/// <param name="exists"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2141:TransparentMethodsMustNotSatisfyLinkDemandsFxCopRule"), System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand)]
		public LibraryImport(string fileName, string fullPath, bool exists)
		{
			_fileName = fileName;
			_fullPath = fullPath;
			_exists = exists;
			if(System.IO.File.Exists(fullPath))
			{
				var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullPath);
				_version = info.FileVersion;
			}
		}

		/// <summary>
		/// Recupera a descrição da biblioteca.
		/// </summary>
		/// <returns></returns>
		public string GetLongDescription()
		{
			if(!_exists)
				return (_fileName + " [missing]");
			return (_fullPath + " (version " + _version + ")");
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return FileName;
		}
	}
}
