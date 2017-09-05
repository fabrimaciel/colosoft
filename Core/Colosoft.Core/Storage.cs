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

namespace Colosoft.IO.VirtualStorage
{
	/// <summary>
	/// Implementação do armazenamento virtual.
	/// </summary>
	public static class Storage
	{
		/// <summary>
		/// Recupera o caminho do arquivo com base na Uri informada.
		/// </summary>
		/// <param name="uri">URI do caminho do arquivo.</param>
		/// <returns></returns>
		public static string GetFilePath(Uri uri)
		{
			if(uri.IsPackVirtualStorage())
			{
				var str = uri.PathAndQuery;
				var startIndex = 0;
				if(str[0] == '/')
					startIndex = 1;
				var path = str.Substring(startIndex).Replace('/', '\\');
				return System.IO.Path.Combine(LocalStorage.Directory, path);
			}
			return null;
		}

		/// <summary>
		/// Abre o arquivo com o nome informado.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static System.IO.Stream OpenFile(string path)
		{
			var path2 = System.IO.Path.Combine(LocalStorage.Directory, path);
			return System.IO.File.Open(path2, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
		}

		/// <summary>
		/// Recupera os caminhos dos arquivos contidos no armazenamento.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string[] GetFiles(string path)
		{
			return GetFiles(path, null);
		}

		/// <summary>
		/// Recupera os caminhos dos arquivos contidos no armazenamento.
		/// </summary>
		/// <param name="path">Caminho a partir do qual serão recuperados do arquivos.</param>
		/// <param name="searchPattern"></param>
		/// <returns></returns>
		public static string[] GetFiles(string path, string searchPattern)
		{
			return System.IO.Directory.GetFiles(System.IO.Path.Combine(LocalStorage.Directory, path), searchPattern).Select(f => f.Substring(LocalStorage.Directory.Length)).ToArray();
		}

		/// <summary>
		/// Recupera os caminhos dos diretórios contidos no armazenamento.
		/// </summary>
		/// <param name="path">Caminho a partir do qual serão recuperados os direttórios.</param>
		/// <returns></returns>
		public static string[] GetDirectories(string path)
		{
			return GetDirectories(path, null);
		}

		/// <summary>
		/// Recupera os caminhos dos diretórios contidos no armazenamento.
		/// </summary>
		/// <param name="path">Caminho a partir do qual serão recuperados os direttórios.</param>
		/// <param name="searchPattern"></param>
		/// <returns></returns>
		public static string[] GetDirectories(string path, string searchPattern)
		{
			return System.IO.Directory.GetDirectories(System.IO.Path.Combine(LocalStorage.Directory, path), searchPattern).Select(f => f.Substring(LocalStorage.Directory.Length)).ToArray();
		}
	}
}
