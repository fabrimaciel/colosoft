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
	/// Implementação básica do <see cref="IDataBaseSchema"/>.
	/// </summary>
	public abstract class BaseDataBaseSchema : IDataBaseSchema
	{
		private System.Data.Common.DbConnection _connection;

		/// <summary>
		/// Nome da coluna que representa o TABLE_NAME no <see cref="System.Data.DataTable"/> retornado pelo <see cref="GetTables"/>.
		/// </summary>
		public virtual string ColumnNameForTableName
		{
			get
			{
				return "TABLE_NAME";
			}
		}

		/// <summary>
		/// Nome do esquema que armazena as chaves estrangeiras.
		/// </summary>
		protected virtual string ForeignKeysSchemaName
		{
			get
			{
				return "ForeignKeys";
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connection"></param>
		public BaseDataBaseSchema(System.Data.Common.DbConnection connection)
		{
			connection.Require("connection").NotNull();
			_connection = connection;
		}

		/// <summary>
		/// Recupera os metadados da linha de dados informada.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="extras"></param>
		/// <returns></returns>
		public abstract ITableMetadata GetTableMetadata(System.Data.DataRow rs, bool extras);

		/// <summary>
		/// Recupera a descrição da tabela disponíveis no catalogo.
		/// </summary>
		/// <param name="catalog">Nome do catalogo que contem as tabelas.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableNamePattern">Padrão que será usado para filtrar as tabelas.</param>
		/// <returns></returns>
		public virtual System.Data.DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern)
		{
			var restrictions = new[] {
				catalog,
				schemaPattern,
				tableNamePattern,
				null
			};
			return _connection.GetSchema("Tables", restrictions);
		}

		/// <summary>
		/// Recupera a descrição das colunas disponíveis para a tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableNamePattern">Padrão que será usado para filtrar as tabelas.</param>
		/// <param name="columnNamePattern">Padrão usado para filtrar os nomes das colunas.</param>
		/// <returns>Descrição das colunas.</returns>
		public virtual System.Data.DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{
			var restrictions = new[] {
				catalog,
				schemaPattern,
				tableNamePattern,
				columnNamePattern
			};
			return _connection.GetSchema("Columns", restrictions);
		}

		/// <summary>
		/// Recupera a descrição dos indices da tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableName">Nome da tabela que os indices que estão associados.</param>
		/// <returns>Descrição dos indices da tabela.</returns>
		public virtual System.Data.DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			var restrictions = new[] {
				catalog,
				schemaPattern,
				tableName,
				null
			};
			return _connection.GetSchema("Indexes", restrictions);
		}

		/// <summary>
		/// Recupera a descrição das colunas dos indices da tabela.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schemaPattern">Padrão que será usado para filtrar os esquemas.</param>
		/// <param name="tableName">Nome da tabela que os indices que estão associados.</param>
		/// <param name="indexName">Nome do indice.</param>
		/// <returns>Descrição das colunas dos indices da tabela</returns>
		public virtual System.Data.DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			var restrictions = new[] {
				catalog,
				schemaPattern,
				tableName,
				indexName,
				null
			};
			return _connection.GetSchema("IndexColumns", restrictions);
		}

		/// <summary>
		/// Recupera a descrição da chaves estrangeiras disponíveis.
		/// </summary>
		/// <param name="catalog">Nome do catalogo.</param>
		/// <param name="schema">Nome do esquema onde a tabela está inserida.</param>
		/// <param name="table">Nome da tabela.</param>
		/// <returns>Descrição da chaves estrangeiras disponíveis</returns>
		public virtual System.Data.DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			var restrictions = new[] {
				catalog,
				schema,
				table,
				null
			};
			return _connection.GetSchema(ForeignKeysSchemaName, restrictions);
		}
	}
}
