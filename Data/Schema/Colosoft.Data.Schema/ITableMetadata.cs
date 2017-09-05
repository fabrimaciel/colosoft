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
	/// Assinatura das classes que armazenam os metadados de uma tabela.
	/// </summary>
	public interface ITableMetadata
	{
		/// <summary>
		/// Nome da tabela.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Nome do catalogo onde a tabela está inserida.
		/// </summary>
		string Catalog
		{
			get;
		}

		/// <summary>
		/// Esquema onde a tabela está inserida.
		/// </summary>
		string Schema
		{
			get;
		}

		/// <summary>
		/// Recupera os metadados de uma coluna da tabela.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		IColumnMetadata GetColumnMetadata(string columnName);

		/// <summary>
		/// Recupera os metadados de uma chave estrangeira da tabela.
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		IForeignKeyMetadata GetForeignKeyMetadata(string keyName);

		/// <summary>
		/// Recupera os metadados de um indice da tabela.
		/// </summary>
		/// <param name="indexName"></param>
		/// <returns></returns>
		IIndexMetadata GetIndexMetadata(string indexName);
	}
}
