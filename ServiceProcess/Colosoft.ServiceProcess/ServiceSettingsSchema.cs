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

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Representa o esquema da configuração do serviço.
	/// </summary>
	public class ServiceSettingsSchema : IEnumerable<ServiceSettingsSchema.Entry>
	{
		private string _name;

		private List<Entry> _entries;

		/// <summary>
		/// Nome do esquema.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Quantidade de entradas no esquema.
		/// </summary>
		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		/// <summary>
		/// Recupera a entrada pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Entry this[int index]
		{
			get
			{
				return _entries[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do esquema.</param>
		/// <param name="entries">Entradas.</param>
		public ServiceSettingsSchema(string name, IEnumerable<Entry> entries)
		{
			name.Require("name").NotNull().NotEmpty();
			entries.Require("entries").NotNull();
			_name = name;
			_entries = new List<Entry>(entries);
		}

		/// <summary>
		/// Recupera o enumerador das entradas.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ServiceSettingsSchema.Entry> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enemerador das entradas.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Representa uma entrada do esquema.
		/// </summary>
		public class Entry
		{
			/// <summary>
			/// Nome da entrada.
			/// </summary>
			public string Name
			{
				get;
				private set;
			}

			/// <summary>
			/// Tipo da entrada.
			/// </summary>
			public Type Type
			{
				get;
				private set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="type"></param>
			public Entry(string name, Type type)
			{
				this.Name = name;
				this.Type = type;
			}
		}
	}
}
