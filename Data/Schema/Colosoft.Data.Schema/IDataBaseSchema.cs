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
using System.Data;

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Assinatura das classes que fazem acesso ao esquema do banco de dados.
	/// </summary>
	public interface IDataBaseSchema
	{
		/// <summary>
		/// Nome da coluna que representa o TABLE_NAME no <see cref="DataTable"/> retornado pelo <see cref="GetTables"/>.
		/// </summary>
		string ColumnNameForTableName
		{
			get;
		}

		/// <summary>
		/// Recupera a descrição da tabela disponíveis no catalogo.
		/// </summary>
		/// <param name="catalog">Nome do catalogo que contem as tabelas.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableNamePattern">Padrão que será usado para filtrar as tabelas.</param>
		/// <returns></returns>
		DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern);

		/// <summary>
		/// Recupera a descrição das colunas disponíveis para a tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableNamePattern">Padrão que será usado para filtrar as tabelas.</param>
		/// <param name="columnNamePattern">Padrão usado para filtrar os nomes das colunas.</param>
		/// <returns>Descrição das colunas.</returns>
		DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern);

		/// <summary>
		/// Recupera a descrição dos indices da tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableName">Nome da tabela que os indices que estão associados.</param>
		/// <returns>Descrição dos indices da tabela.</returns>
		DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName);

		/// <summary>
		/// Recupera a descrição das colunas dos indices da tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableName">Nome da tabela que os indices que estão associados.</param>
		/// <param name="indexName">Nome do indice.</param>
		/// <returns>Descrição das colunas dos indices da tabela</returns>
		DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName);

		/// <summary>
		/// Recupera a descrição da chaves estrangeiras disponíveis.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schema">Nome do esquema onde a tabela está inserida.</param>
		/// <param name="table">Nome da tabela.</param>
		/// <returns>Descrição da chaves estrangeiras disponíveis</returns>
		DataTable GetForeignKeys(string catalog, string schema, string table);
	}
}
