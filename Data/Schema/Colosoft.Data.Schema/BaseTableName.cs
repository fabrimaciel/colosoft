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
	/// Implementação basica de <see cref="ITableMetadata"/>.
	/// </summary>
	public abstract class BaseTableMetadata : ITableMetadata
	{
		private string _catalog;

		private string _schema;

		private string _name;

		private Dictionary<string, IColumnMetadata> _columns = new Dictionary<string, IColumnMetadata>();

		private Dictionary<string, IForeignKeyMetadata> _foreignKeys = new Dictionary<string, IForeignKeyMetadata>();

		private Dictionary<string, IIndexMetadata> _indexes = new Dictionary<string, IIndexMetadata>();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="rs">Linha contendo os dados dos metadados da tabela.</param>
		/// <param name="meta"></param>
		/// <param name="extras"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public BaseTableMetadata(System.Data.DataRow rs, IDataBaseSchema meta, bool extras)
		{
			this.ParseTableInfo(rs);
			this.InitColumns(meta);
			if(extras)
			{
				this.InitForeignKeys(meta);
				this.InitIndexes(meta);
			}
		}

		/// <summary>
		/// Recupera as informações da tabela que estão na linha informada.
		/// </summary>
		/// <param name="rs"></param>
		protected abstract void ParseTableInfo(System.Data.DataRow rs);

		/// <summary>
		/// Recupera o nome da constraint.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract string GetConstraintName(System.Data.DataRow rs);

		/// <summary>
		/// Recupera o nome da coluna.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract string GetColumnName(System.Data.DataRow rs);

		/// <summary>
		/// Recupera o nome do indice.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract string GetIndexName(System.Data.DataRow rs);

		/// <summary>
		/// Recupera os metadados da coluna.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract IColumnMetadata GetColumnMetadata(System.Data.DataRow rs);

		/// <summary>
		/// Recupera os metadados da chave entrageira.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract IForeignKeyMetadata GetForeignKeyMetadata(System.Data.DataRow rs);

		/// <summary>
		/// Recupera os metadados do indice.
		/// </summary>
		/// <param name="rs"></param>
		/// <returns></returns>
		protected abstract IIndexMetadata GetIndexMetadata(System.Data.DataRow rs);

		/// <summary>
		/// Nome da tabela.
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
		/// Catalogo onde a tabela está inserida.
		/// </summary>
		public string Catalog
		{
			get
			{
				return _catalog;
			}
			protected set
			{
				_catalog = value;
			}
		}

		/// <summary>
		/// Esquema onde a tabela está inserida.
		/// </summary>
		public string Schema
		{
			get
			{
				return _schema;
			}
			protected set
			{
				_schema = value;
			}
		}

		/// <summary>
		/// Adiciona uma chave estrangeira para a instancia.
		/// </summary>
		/// <param name="rs">Linha que contem</param>
		/// <param name="meta"></param>
		private void AddForeignKey(System.Data.DataRow rs, IDataBaseSchema meta)
		{
			string fk = GetConstraintName(rs);
			if(string.IsNullOrEmpty(fk))
				return;
			IForeignKeyMetadata info = GetForeignKeyMetadata(fk);
			if(info == null)
			{
				info = GetForeignKeyMetadata(rs);
				_foreignKeys[info.Name.ToLowerInvariant()] = info;
			}
			foreach (System.Data.DataRow row in meta.GetIndexColumns(_catalog, _schema, _name, fk).Rows)
			{
				info.AddColumn(GetColumnMetadata(GetColumnName(row)));
			}
		}

		/// <summary>
		/// Adiciona um indice para a instancia.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="meta"></param>
		private void AddIndex(System.Data.DataRow rs, IDataBaseSchema meta)
		{
			string index = GetIndexName(rs);
			if(string.IsNullOrEmpty(index))
				return;
			IIndexMetadata info = GetIndexMetadata(index);
			if(info == null)
			{
				info = GetIndexMetadata(rs);
				_indexes[info.Name.ToLowerInvariant()] = info;
			}
			foreach (System.Data.DataRow row in meta.GetIndexColumns(_catalog, _schema, _name, index).Rows)
			{
				info.AddColumn(GetColumnMetadata(GetColumnName(row)));
			}
		}

		/// <summary>
		/// Adiciona uma coluna para a instancia.
		/// </summary>
		/// <param name="rs"></param>
		private void AddColumn(System.Data.DataRow rs)
		{
			string column = GetColumnName(rs);
			if(string.IsNullOrEmpty(column))
				return;
			if(GetColumnMetadata(column) == null)
			{
				IColumnMetadata info = GetColumnMetadata(rs);
				_columns[info.Name.ToLowerInvariant()] = info;
			}
		}

		/// <summary>
		/// Inicializa as chaves estrangeiras da instancia.
		/// </summary>
		/// <param name="meta"></param>
		private void InitForeignKeys(IDataBaseSchema meta)
		{
			foreach (System.Data.DataRow row in meta.GetForeignKeys(_catalog, _schema, _name).Rows)
			{
				AddForeignKey(row, meta);
			}
		}

		/// <summary>
		/// Inicializa os indices da instancia.
		/// </summary>
		/// <param name="meta"></param>
		private void InitIndexes(IDataBaseSchema meta)
		{
			foreach (System.Data.DataRow row in meta.GetIndexInfo(_catalog, _schema, _name).Rows)
			{
				AddIndex(row, meta);
			}
		}

		/// <summary>
		/// Inicializa as colunas da instancia.
		/// </summary>
		/// <param name="meta"></param>
		private void InitColumns(IDataBaseSchema meta)
		{
			foreach (System.Data.DataRow row in meta.GetColumns(_catalog, _schema, _name, null).Rows)
			{
				AddColumn(row);
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "TableMetadata(" + _name + ')';
		}

		/// <summary>
		/// Recupera os metadados da coluna.
		/// </summary>
		/// <param name="columnName">Nome da coluna.</param>
		/// <returns></returns>
		public IColumnMetadata GetColumnMetadata(string columnName)
		{
			IColumnMetadata result;
			_columns.TryGetValue(columnName.ToLowerInvariant(), out result);
			return result;
		}

		/// <summary>
		/// Recupera os metadados do chave estrangeira.
		/// </summary>
		/// <param name="keyName">Nome da chave.</param>
		/// <returns></returns>
		public IForeignKeyMetadata GetForeignKeyMetadata(string keyName)
		{
			IForeignKeyMetadata result;
			_foreignKeys.TryGetValue(keyName.ToLowerInvariant(), out result);
			return result;
		}

		/// <summary>
		/// Recupera os metadados do indice pelo nome informado.
		/// </summary>
		/// <param name="indexName"></param>
		/// <returns></returns>
		public IIndexMetadata GetIndexMetadata(string indexName)
		{
			IIndexMetadata result;
			_indexes.TryGetValue(indexName.ToLowerInvariant(), out result);
			return result;
		}
	}
}
