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

namespace Colosoft.IO.FileRepository.Local
{
	/// <summary>
	/// Implementação de um repositório local.
	/// </summary>
	public class LocalRepository : IFileRepository
	{
		private string _root;

		/// <summary>
		/// Evento acionado quando o repositório é atualizado.
		/// </summary>
		public event EventHandler Updated;

		/// <summary>
		/// Diretório raiz do repositório.
		/// </summary>
		public virtual string Root
		{
			get
			{
				return _root;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="root">Diretório principal do repositório.</param>
		public LocalRepository(string root)
		{
			root.Require("root").NotNull().NotEmpty();
			_root = root;
		}

		/// <summary>
		/// Construtor para ser usado para classe filhas.
		/// </summary>
		protected LocalRepository()
		{
		}

		/// <summary>
		/// Método acionado quando o repositório for atualizado.
		/// </summary>
		public void OnUpdated()
		{
			if(Updated != null)
				Updated(this, EventArgs.Empty);
		}

		/// <summary>
		/// Recupera o caminho local.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string GetLocalPath(string path)
		{
			path.Require("path").NotNull().NotEmpty();
			path = path.Replace('/', '\\');
			if(path == "\\")
				return Root;
			if(path.StartsWith("\\"))
				path = path.Substring(1);
			return System.IO.Path.Combine(Root, path);
		}

		/// <summary>
		/// Consulta os itens no repositório.
		/// </summary>
		/// <param name="path">Caminho que será pesquisa.</param>
		/// <param name="searchPattern">Padrão que será usada na comparação da pesquisa.</param>
		/// <param name="itemType">Tipo do item.</param>
		/// <param name="searchOptions"></param>
		/// <returns></returns>
		public IEnumerable<IItem> QueryItems(string path, string searchPattern, ItemType itemType, SearchOption searchOptions)
		{
			path.Require("path").NotNull().NotEmpty();
			path = GetLocalPath(path);
			IEnumerable<IItem> directories = null;
			IEnumerable<IItem> files = null;
			if(string.IsNullOrEmpty(searchPattern))
				searchPattern = "*.*";
			if((itemType == ItemType.Folder || itemType == ItemType.Any) && System.IO.Directory.Exists(path))
				directories = System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOptions == SearchOption.AllDirectories ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly).Select(f => ((IItem)new LocalItem(new System.IO.DirectoryInfo(f))));
			else
				directories = new IItem[0];
			if((itemType == ItemType.File || itemType == ItemType.Any) && System.IO.Directory.Exists(path))
				files = System.IO.Directory.EnumerateFiles(path, searchPattern, searchOptions == SearchOption.AllDirectories ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly).Select(f => ((IItem)new LocalItem(new System.IO.FileInfo(f))));
			else
				files = new IItem[0];
			return directories.Concat(files);
		}

		/// <summary>
		/// Recupera o item associado com o caminho informado.
		/// </summary>
		/// <param name="path">Caminho do item.</param>
		/// <param name="itemType">Tipo do item.</param>
		/// <returns></returns>
		public IItem GetItem(string path, ItemType itemType)
		{
			path.Require("path").NotNull().NotEmpty();
			path = GetLocalPath(path);
			if(itemType == ItemType.Folder && System.IO.Directory.Exists(path))
				return new LocalItem(new System.IO.DirectoryInfo(path));
			else if(System.IO.File.Exists(path))
				return new LocalItem(new System.IO.FileInfo(path));
			return null;
		}

		/// <summary>
		/// Cria um diretório.
		/// </summary>
		/// <param name="path"></param>
		public IItem CreateFolder(string path)
		{
			path.Require("path").NotNull().NotEmpty();
			path = GetLocalPath(path);
			OnUpdated();
			return new LocalItem(System.IO.Directory.CreateDirectory(path));
		}

		/// <summary>
		/// Verifica se o caminho informado existe no repositório.
		/// </summary>
		/// <param name="path">Caminho do item que será verificado.</param>
		/// <param name="itemType">Tipo do item que será verificado.</param>
		/// <returns></returns>
		public bool Exists(string path, ItemType itemType)
		{
			path.Require("path").NotNull().NotEmpty();
			path = GetLocalPath(path);
			if(itemType == ItemType.Folder)
				return System.IO.Directory.Exists(path);
			else
				return System.IO.File.Exists(path);
		}

		/// <summary>
		/// Remove o item do repositório.
		/// </summary>
		/// <param name="item">Item que será removido.</param>
		/// <param name="recursive">Identifiac se para remove os itens filhos recursivamente.</param>
		public void Delete(IItem item, bool recursive)
		{
			item.Require("item").NotNull();
			var path = GetLocalPath(item.FullName);
			if(item.Type == ItemType.Folder)
				System.IO.Directory.Delete(path, recursive);
			else
				System.IO.File.Delete(path);
			OnUpdated();
		}

		/// <summary>
		/// Move o item para o caminho de destino informado.
		/// </summary>
		/// <param name="sourceItem"></param>
		/// <param name="destPath"></param>
		public void Move(IItem sourceItem, string destPath)
		{
			sourceItem.Require("sourceItem").NotNull();
			destPath.Require("destPath").NotNull().NotEmpty();
			var sourcePath = GetLocalPath(sourceItem.FullName);
			destPath = GetLocalPath(destPath);
			if(sourceItem.Type == ItemType.Folder)
				System.IO.Directory.Move(sourcePath, destPath);
			else
				System.IO.File.Move(sourcePath, destPath);
			OnUpdated();
		}

		/// <summary>
		/// Atualiza os dados do repositório.
		/// </summary>
		public void Refresh()
		{
			OnUpdated();
		}
	}
}
