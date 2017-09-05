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
	/// Armazena os dados do nome da tabela.
	/// </summary>
	[Serializable]
	public class TableName
	{
		/// <summary>
		/// Catalago onde a tabela está inserida.
		/// </summary>
		public string Catalog
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema onde a tabela está inserida.,
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TableName()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo os valores padrão.
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="schema"></param>
		/// <param name="name"></param>
		public TableName(string catalog, string schema, string name)
		{
			this.Catalog = catalog;
			this.Schema = schema;
			this.Name = name;
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new TableName(Catalog, Schema, Name);
		}
	}
}
