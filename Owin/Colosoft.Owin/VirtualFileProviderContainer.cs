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
using System.Threading.Tasks;

namespace Colosoft.Web.Hosting
{
	/// <summary>
	/// Implementação do container de provedores de arquivos virtuais.
	/// </summary>
	class VirtualFileProviderContainer : System.Web.Hosting.VirtualPathProvider
	{
		private List<System.Web.Hosting.VirtualPathProvider> _providers;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="providers"></param>
		public VirtualFileProviderContainer(IEnumerable<System.Web.Hosting.VirtualPathProvider> providers)
		{
			if(providers == null)
				throw new ArgumentNullException("providers");
			_providers = new List<System.Web.Hosting.VirtualPathProvider>(providers);
		}

		/// <summary>
		/// Verifica se o diretório existe
		/// </summary>
		/// <param name="virtualDir"></param>
		/// <returns></returns>
		public override bool DirectoryExists(string virtualDir)
		{
			return _providers.Any(f => f.DirectoryExists(virtualDir));
		}

		/// <summary>
		/// Verifica se existe arquivo para o caminho informado.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override bool FileExists(string virtualPath)
		{
			return _providers.Any(f => f.FileExists(virtualPath));
		}

		/// <summary>
		/// Recupera o diretório virtual com base no caminho informado.
		/// </summary>
		/// <param name="virtualDir"></param>
		/// <returns></returns>
		public override System.Web.Hosting.VirtualDirectory GetDirectory(string virtualDir)
		{
			foreach (var provider in _providers)
			{
				var directory = provider.GetDirectory(virtualDir);
				if(directory != null)
					return directory;
			}
			return new CustomVirtualDirectory(virtualDir);
		}

		/// <summary>
		/// Recupera o arquivo virtual pelo caminho informado.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
		{
			foreach (var provider in _providers)
			{
				var file = provider.GetFile(virtualPath);
				if(file != null)
					return file;
			}
			return null;
		}
	}
}
