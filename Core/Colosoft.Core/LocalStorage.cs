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
	/// Gerencia os arquivos do armazenamento virtual de forma local.
	/// </summary>
	public static class LocalStorage
	{
		/// <summary>
		/// Diretório base do armazenamento local.
		/// </summary>
		public static string Directory
		{
			get
			{
				return System.IO.Path.Combine(IsolatedStorage.IsolatedStorage.AuthenticationContextDirectory, "VirtualStorage");
			}
		}

		/// <summary>
		/// Navega pelos itens dos diretório pai informado.
		/// </summary>
		/// <param name="parentDirectory"></param>
		/// <returns></returns>
		private static IEnumerable<Item> NavigateItems(string parentDirectory)
		{
			if(System.IO.Directory.Exists(parentDirectory))
			{
				foreach (var file in System.IO.Directory.GetFiles(parentDirectory))
					yield return new Item(parentDirectory, System.IO.Path.GetFileName(file), true);
				foreach (var directory in System.IO.Directory.GetDirectories(parentDirectory))
				{
					yield return new Item(parentDirectory, System.IO.Path.GetFileName(directory), false);
					foreach (var i in NavigateItems(directory))
						yield return i;
				}
			}
		}

		/// <summary>
		/// Recupera o caminho do item local.
		/// </summary>
		/// <param name="itemPath">Caminho do item.</param>
		/// <returns></returns>
		public static string GetLocalItemPath(string itemPath)
		{
			return System.IO.Path.Combine(Directory, itemPath);
		}

		/// <summary>
		/// Recupera o cam
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Item> GetItems()
		{
			foreach (var item in NavigateItems(Directory))
				yield return item;
		}

		/// <summary>
		/// Representa um item do armazenamento local.
		/// </summary>
		public class Item
		{
			private string _name;

			private string _parent;

			private bool _isFile;

			/// <summary>
			/// Nome do item.
			/// </summary>
			public string Name
			{
				get
				{
					return _name;
				}
			}

			/// <summary>
			/// Caminho do pai do item.
			/// </summary>
			public string Parent
			{
				get
				{
					return _parent;
				}
			}

			/// <summary>
			/// Identifica se o item é um arquivo.
			/// </summary>
			public bool IsFile
			{
				get
				{
					return _isFile;
				}
			}

			/// <summary>
			/// Caminho completo do item.
			/// </summary>
			public string FullName
			{
				get
				{
					return System.IO.Path.Combine(_parent, _name);
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="name"></param>
			/// <param name="isFile"></param>
			public Item(string parent, string name, bool isFile)
			{
				_parent = parent;
				_name = name;
				_isFile = isFile;
			}

			/// <summary>
			/// Recupera as informações do arquivo do sistema.
			/// </summary>
			/// <returns></returns>
			public System.IO.FileSystemInfo GetFileSystemInfo()
			{
				if(IsFile)
					return new System.IO.FileInfo(FullName);
				else
					return new System.IO.DirectoryInfo(FullName);
			}

			/// <summary>
			/// Calcula o hash do item.
			/// </summary>
			/// <returns></returns>
			public byte[] CalculateHash()
			{
				if(IsFile)
					return Colosoft.Util.Checksum.CalculateMD5(FullName);
				return new byte[0];
			}
		}
	}
}
