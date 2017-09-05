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
	/// Implementação básica do <see cref="IForeignKeyMetadata"/>.
	/// </summary>
	public class BaseForeignKeyMetadata : IForeignKeyMetadata
	{
		private string _name;

		private readonly List<IColumnMetadata> _columns = new List<IColumnMetadata>();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="rs"></param>
		public BaseForeignKeyMetadata(System.Data.DataRow rs)
		{
		}

		/// <summary>
		/// Nome da chave estrageira.
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
		/// Adiciona uma nova coluna para a instancia.
		/// </summary>
		/// <param name="column"></param>
		public void AddColumn(IColumnMetadata column)
		{
			if(column != null)
				_columns.Add(column);
		}

		/// <summary>
		/// Colunas associadas com a instancia.
		/// </summary>
		public IColumnMetadata[] Columns
		{
			get
			{
				return _columns.ToArray();
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "ForeignKeyMetadata(" + _name + ')';
		}
	}
}
