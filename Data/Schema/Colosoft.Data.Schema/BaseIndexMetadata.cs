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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação básica do <see cref="IIndexMetadata"/>.
	/// </summary>
	public abstract class BaseIndexMetadata : IIndexMetadata
	{
		private string _name;

		private readonly List<IColumnMetadata> _columns = new List<IColumnMetadata>();

		/// <summary>
		/// Construor padrão.
		/// </summary>
		/// <param name="rs"></param>
		public BaseIndexMetadata(System.Data.DataRow rs)
		{
		}

		/// <summary>
		/// Nome do indice.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Colunas associadas com o indice.
		/// </summary>
		public IColumnMetadata[] Columns
		{
			get
			{
				return _columns.ToArray();
			}
		}

		/// <summary>
		/// Adiciona uma nova coluna para o indice.
		/// </summary>
		/// <param name="column"></param>
		public void AddColumn(IColumnMetadata column)
		{
			if(column != null)
				_columns.Add(column);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "IndexMatadata(" + _name + ')';
		}
	}
}
