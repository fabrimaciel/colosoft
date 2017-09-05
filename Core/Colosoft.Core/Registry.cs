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

namespace Colosoft.Configuration
{
	/// <summary>
	/// Classe que armazena os dados dos registros de configuração.
	/// </summary>
	public class Registry
	{
		private RegistryFolder _root;

		private IRegistryLoader _registryLoader;

		/// <summary>
		/// Lista dos nodos raiz.
		/// </summary>
		public RegistryFolder Root
		{
			get
			{
				return _root;
			}
		}

		/// <summary>
		/// Recupera o valor de um item, passando o caminho.
		/// </summary>
		/// <param name="Path"></param>
		/// <returns></returns>
		public string GetValue(string Path)
		{
			return "";
		}

		/// <summary>
		/// Popula a lista
		/// </summary>
		private void PopulateRoot()
		{
			foreach (var entry in _registryLoader.GetRoot())
			{
				RegistryFolder currentFolder = _root;
				var entries = entry.Path.Split('\\').ToArray();
				for(int i = 0; i < entries.Count() - 1; i++)
				{
					var child = currentFolder.Children.Where(f => StringComparer.OrdinalIgnoreCase.Equals(entries[i], f.Name)).FirstOrDefault();
					if(child == null)
					{
						child = new RegistryFolder() {
							Name = entries[i]
						};
						currentFolder.Children.Add(child);
					}
					currentFolder = child;
				}
				currentFolder.Entries.Add(entry);
			}
		}

		/// <summary>
		/// Construtor Padrão
		/// </summary>
		/// <param name="registryLoader"></param>
		public Registry(IRegistryLoader registryLoader)
		{
			registryLoader.Require("registryLoader").NotNull();
			_registryLoader = registryLoader;
			_root = new RegistryFolder();
			PopulateRoot();
		}
	}
}
