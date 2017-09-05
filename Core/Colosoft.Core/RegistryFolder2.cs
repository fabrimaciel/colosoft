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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class RegistryFolder2 : IRegistry2
	{
		private string _name;

		private Collections.IObservableCollection<RegistryFolder2> _children = new Collections.BaseObservableCollection<RegistryFolder2>();

		private Collections.IObservableCollection<IRegistryEntry2> _entries = new Collections.BaseObservableCollection<IRegistryEntry2>();

		private Collections.AggregateObservableCollection<IRegistry2> _registries = null;

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
				_name = value;
			}
		}

		/// <summary>
		/// Entradas
		/// </summary>
		public Collections.IObservableCollection<IRegistry2> Entries
		{
			get
			{
				return _entries.CastObservableCollection<IRegistry2>();
			}
		}

		/// <summary>
		/// Filhos
		/// </summary>
		public Collections.IObservableCollection<IRegistry2> Children
		{
			get
			{
				return _children.CastObservableCollection<IRegistry2>();
			}
		}

		/// <summary>
		/// Registros.
		/// </summary>
		public Collections.AggregateObservableCollection<IRegistry2> Registries
		{
			get
			{
				if(_registries == null)
					_registries = new Collections.AggregateObservableCollection<IRegistry2>(Children, Entries);
				return _registries;
			}
		}
	}
}
