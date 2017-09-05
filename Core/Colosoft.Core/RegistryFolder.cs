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
	/// Classe que armazena os dados dos registros da configuração
	/// </summary>
	public class RegistryFolder
	{
		private string _name;

		private IList<RegistryFolder> _children = new List<RegistryFolder>();

		private IList<IRegistryEntry> _entries = new List<IRegistryEntry>();

		/// <summary>
		/// Nome do registro
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
					_name = value;
			}
		}

		/// <summary>
		/// Entradas
		/// </summary>
		public IList<IRegistryEntry> Entries
		{
			get
			{
				return _entries;
			}
		}

		/// <summary>
		/// Filhos
		/// </summary>
		public IList<RegistryFolder> Children
		{
			get
			{
				return _children;
			}
		}
	}
}
