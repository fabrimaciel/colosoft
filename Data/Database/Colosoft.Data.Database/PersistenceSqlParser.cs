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
using Colosoft.Data.Schema;
using Colosoft.Query;

namespace Colosoft.Data.Database
{
	/// <summary>
	/// Classe abstrata que define um método para geração de comandos sql se persistência de dados
	/// </summary>
	public abstract class PersistenceSqlParser
	{
		private static object _lockObject = new object();

		private PersistenceAction _action;

		private IQueryTranslator _translator;

		private ITypeSchema _typeSchema;

		private IPrimaryKeyRepository _primaryKeyRepository;

		private Colosoft.Query.Database.SqlQueryParser _queryParser;

		/// <summary>
		/// Ação de persistência a ser executada
		/// </summary>
		public PersistenceAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		/// <summary>
		/// Instância da interface de mapeamento nome classe - nome tabela
		/// </summary>
		protected IQueryTranslator Translator
		{
			get
			{
				return _translator;
			}
		}

		/// <summary>
		/// Instância da interface de carga de méetadados
		/// </summary>
		protected ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Instância da interface de carga chaves primária
		/// </summary>
		public IPrimaryKeyRepository PrimaryKeyRepository
		{
			get
			{
				return _primaryKeyRepository;
			}
			set
			{
				_primaryKeyRepository = value;
			}
		}

		/// <summary>
		/// Parser de consulta associado a instancia.
		/// </summary>
		public Colosoft.Query.Database.SqlQueryParser QueryParser
		{
			get
			{
				return _queryParser;
			}
			set
			{
				_queryParser = value;
			}
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="translator">Interface de mapeamento de entidade-tabela no banco de dados</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		public PersistenceSqlParser(IQueryTranslator translator, ITypeSchema typeSchema)
		{
			translator.Require("translator");
			translator.Require("typeSchema");
			translator.Require("primaryKeyRepository");
			_translator = translator;
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Método que retorna o texto do comando sql a ser executado
		/// </summary>
		/// <returns>Retorna o texto do comando sql a ser executado</returns>
		public abstract string GetPersistenceCommandText();

		/// <summary>
		/// Retorna um objeto EntityInfo baseado no nome
		/// </summary>
		/// <param name="name">Nome da entidade</param>
		/// <returns>Retorna o nome da entidade</returns>
		protected EntityInfo GetEntityInfo(string name)
		{
			return new EntityInfo() {
				FullName = name,
				Alias = null
			};
		}

		/// <summary>
		/// Define o tipo do parâmetero.
		/// </summary>
		/// <param name="propertyMetadata">Metadados da propriedade.</param>
		/// <param name="parameter">Parâmetro de persistência.</param>
		protected virtual void SetParameterType(IPropertyMetadata propertyMetadata, PersistenceParameter parameter)
		{
			if(StringComparer.InvariantCultureIgnoreCase.Equals(propertyMetadata.PropertyType, "System.Byte[]"))
				parameter.DbType = DbType.Binary;
		}
	}
}
