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
	/// Implementação do provedor de arquivo para diretório.
	/// </summary>
	public class DirectoryVirtualFileProvider : System.Web.Hosting.VirtualPathProvider
	{
		private string _baseDirectory;

		private Dictionary<string, RazorEngine.Templating.ITemplateKey> _razorTemplateKeys = new Dictionary<string, RazorEngine.Templating.ITemplateKey>();

		/// <summary>
		/// Caminho do diretório base.
		/// </summary>
		public virtual string BaseDirectory
		{
			get
			{
				return _baseDirectory;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="baseDirectory"></param>
		public DirectoryVirtualFileProvider(string baseDirectory) : base()
		{
			_baseDirectory = baseDirectory;
		}

		/// <summary>
		/// Recupera o diretório virtual.
		/// </summary>
		/// <param name="virtualDir"></param>
		/// <returns></returns>
		public override System.Web.Hosting.VirtualDirectory GetDirectory(string virtualDir)
		{
			return new Colosoft.Web.Hosting.CustomVirtualDirectory(virtualDir);
		}

		/// <summary>
		/// Verifica se o diretório existem
		/// </summary>
		/// <param name="virtualDir"></param>
		/// <returns></returns>
		public override bool DirectoryExists(string virtualDir)
		{
			if(!string.IsNullOrEmpty(BaseDirectory))
			{
				if(virtualDir.StartsWith("/") || virtualDir.StartsWith("\\"))
					virtualDir = virtualDir.Substring(1);
				return System.IO.Directory.Exists(System.IO.Path.Combine(BaseDirectory, virtualDir));
			}
			return false;
		}

		/// <summary>
		/// Verifica se o arquivo existe.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override bool FileExists(string virtualPath)
		{
			if(virtualPath.StartsWith("~"))
				virtualPath = virtualPath.Substring(1);
			if(!string.IsNullOrEmpty(BaseDirectory))
			{
				if(virtualPath.StartsWith("/") || virtualPath.StartsWith("\\"))
					virtualPath = virtualPath.Substring(1);
				virtualPath = virtualPath.Replace('/', '\\');
				return System.IO.File.Exists(System.IO.Path.Combine(BaseDirectory, virtualPath));
			}
			return false;
		}

		/// <summary>
		/// Recupera o arquivo virtual.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
		{
			if(virtualPath.StartsWith("~"))
				virtualPath = virtualPath.Substring(1);
			if(!string.IsNullOrEmpty(BaseDirectory))
			{
				if(virtualPath.StartsWith("/") || virtualPath.StartsWith("\\"))
					virtualPath = virtualPath.Substring(1);
				var physicalPath = System.IO.Path.Combine(BaseDirectory, virtualPath);
				if(System.IO.File.Exists(physicalPath))
					return new PhysicVirtualFile(this, virtualPath.StartsWith("/") ? "" : "/" + virtualPath, physicalPath);
			}
			return null;
		}

		/// <summary>
		/// Remove o modelo.
		/// </summary>
		/// <param name="physicalPath"></param>
		/// <returns></returns>
		public RazorEngine.Templating.ITemplateKey GetTemplateKey(string physicalPath)
		{
			RazorEngine.Templating.ITemplateKey result = null;
			lock (_razorTemplateKeys)
				if(_razorTemplateKeys.TryGetValue(physicalPath, out result))
					return result;
			return result;
		}

		/// <summary>
		/// Notifica que o template foi compilado para o arquivo.
		/// </summary>
		/// <param name="physicalPath"></param>
		/// <param name="templateKey"></param>
		public void NotifyTemplateCompiled(string physicalPath, RazorEngine.Templating.ITemplateKey templateKey)
		{
			lock (_razorTemplateKeys)
				if(!_razorTemplateKeys.ContainsKey(physicalPath))
					_razorTemplateKeys.Add(physicalPath, templateKey);
		}

		/// <summary>
		/// Representa um arquivo virtual físico.
		/// </summary>
		class PhysicVirtualFile : System.Web.Hosting.VirtualFile, IPhysicalFileInfo, Owin.Razor.ITemplateCacheSupport
		{
			private string _physicalPath;

			private DirectoryVirtualFileProvider _directory;

			/// <summary>
			/// Nome do arquivo.
			/// </summary>
			public override string Name
			{
				get
				{
					return System.IO.Path.GetFileName(_physicalPath);
				}
			}

			/// <summary>
			/// Tamanho do conteúdo do arquivo.
			/// </summary>
			public long ContentLength
			{
				get
				{
					return new System.IO.FileInfo(_physicalPath).Length;
				}
			}

			/// <summary>
			/// Data da última escrita do arquivo.
			/// </summary>
			public DateTime LastWriteTimeUtc
			{
				get
				{
					return System.IO.File.GetLastWriteTimeUtc(_physicalPath);
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="virtualPath"></param>
			/// <param name="physicalPath"></param>
			public PhysicVirtualFile(DirectoryVirtualFileProvider directory, string virtualPath, string physicalPath) : base(virtualPath)
			{
				_directory = directory;
				_physicalPath = physicalPath;
			}

			/// <summary>
			/// Abre o arquivo.
			/// </summary>
			/// <returns></returns>
			public override System.IO.Stream Open()
			{
				if(!string.IsNullOrEmpty(_physicalPath))
					return System.IO.File.OpenRead(_physicalPath);
				return null;
			}

			RazorEngine.Templating.ITemplateKey Owin.Razor.ITemplateCacheSupport.TemplateKey
			{
				get
				{
					return _directory.GetTemplateKey(_physicalPath);
				}
			}

			void Owin.Razor.ITemplateCacheSupport.Register(RazorEngine.Templating.ITemplateKey templateKey)
			{
				_directory.NotifyTemplateCompiled(_physicalPath, templateKey);
			}
		}
	}
}
