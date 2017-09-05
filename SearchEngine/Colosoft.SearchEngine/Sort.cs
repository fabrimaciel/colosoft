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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Armazena os dados do campo que será ordenado.
	/// </summary>
	public class SortField
	{
		/// <summary>
		/// Nome do campo que será ordenado.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é uma ordenação invertida.
		/// </summary>
		public bool Reverse
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SortField()
		{
		}

		/// <summary>
		/// Cria a instancia já definindo os parametros.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="reverse"></param>
		public SortField(string name, bool reverse)
		{
			Name = name;
			Reverse = reverse;
		}
	}
	/// <summary>
	/// Armazena os dados de ordenação.
	/// </summary>
	public class Sort
	{
		private SortField[] _fields;

		/// <summary>
		/// Campos da ordenação.
		/// </summary>
		public SortField[] Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				_fields = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Sort()
		{
			_fields = new SortField[0];
		}

		/// <summary>
		/// Cria uma instancia já definindo os campos.
		/// </summary>
		/// <param name="fields"></param>
		public Sort(params SortField[] fields)
		{
			_fields = fields ?? new SortField[0];
		}
	}
}
