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
using Microsoft.Practices.ServiceLocation;
using Colosoft.Data.Schema;

namespace Colosoft.Query.Database
{
	/// <summary>
	/// Possíveis partes de uma consulta SQL.
	/// </summary>
	[Flags]
	public enum SqlQueryPart
	{
		/// <summary>
		/// Projeção.
		/// </summary>
		Select = 1,
		/// <summary>
		/// Junções.
		/// </summary>
		Joins = 2,
		/// <summary>
		/// Filtro.
		/// </summary>
		Where = 4,
		/// <summary>
		/// Agrupamento.
		/// </summary>
		GroupBy = 8,
		/// <summary>
		/// Ordenação.
		/// </summary>
		OrderBy = 16,
		/// <summary>
		/// Filtro para grupo ou agregação.
		/// </summary>
		Having = 32,
		/// <summary>
		/// Toda as partes
		/// </summary>
		All = 63
	}
	/// <summary>
	/// Monta a consulta sql a partir do objeto QueryInfo
	/// </summary>
	public abstract class SqlQueryParser
	{
		private Dictionary<string, EntityInfo> _entityAliasDictionary;

		private QueryInfo _query;

		private IQueryTranslator _translator;

		private ITypeSchema _typeSchema;

		private static object _lockObject = new object();

		private SqlQueryParser _owner;

		private bool _useTakeParameter;

		/// <summary>
		/// Relação das funções nativas do SQL Server.
		/// </summary>
		public static readonly string[] TransactSQLFunctions = new string[] {
			"ABS",
			"ASCII",
			"AVG",
			"CASE",
			"CAST",
			"CEILING",
			"CHAR",
			"CHARINDEX",
			"COALESCE",
			"CONCAT",
			"CONVERT",
			"COUNT",
			"CURRENT_TIMESTAMP",
			"CURRENT_USER",
			"DATALENGTH",
			"DATEADD",
			"DATEDIFF",
			"DATENAME",
			"DATEPART",
			"DAY",
			"DISTINCT",
			"EXISTS",
			"NOT EXISTS",
			"FLOOR",
			"GETDATE",
			"GETUTCDATE",
			"IIF",
			"ISDATE",
			"ISNULL",
			"ISNUMERIC",
			"LAG",
			"LEAD",
			"LEFT",
			"LEN",
			"LOWER",
			"LTRIM",
			"MAX",
			"MIN",
			"MONTH",
			"NCHAR",
			"NULLIF",
			"PATINDEX",
			"RAND",
			"REPLACE",
			"RIGHT",
			"ROUND",
			"RTRIM",
			"SESSION_USER",
			"SESSIONPROPERTY",
			"SIGN",
			"SPACE",
			"STR",
			"STUFF",
			"SUBSTRING",
			"SUM",
			"SYSTEM_USER",
			"UPPER",
			"USER_NAME",
			"YEAR"
		};

		/// <summary>
		/// Objeto QueryInfo que contém os dados da consulta
		/// </summary>
		public QueryInfo Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// Instancia do parser pai.
		/// </summary>
		public SqlQueryParser Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}

		/// <summary>
		/// Tradutor de nomes dos objetos CLR para tabelas SQL.
		/// </summary>
		protected IQueryTranslator Translator
		{
			get
			{
				return _translator;
			}
		}

		/// <summary>
		/// Classe de recuperação de metadados.
		/// </summary>
		protected ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Retorna uma entidade baseado no alias da mesma.
		/// </summary>
		protected Dictionary<string, EntityInfo> EntityAliasDictionary
		{
			get
			{
				return _entityAliasDictionary;
			}
			set
			{
				_entityAliasDictionary = value;
			}
		}

		/// <summary>
		/// Relação das entidades associadas com o Parser
		/// </summary>
		protected IEnumerable<EntityInfo> Entities
		{
			get
			{
				if(Query != null)
					foreach (var i in Query.Entities)
						yield return i;
				if(Owner != null)
					foreach (var i in Owner.Entities)
						yield return i;
			}
		}

		/// <summary>
		/// Identifica se é para usar os parametros de take and skip no parser.
		/// </summary>
		public bool UseTakeParameter
		{
			get
			{
				return _useTakeParameter;
			}
			set
			{
				_useTakeParameter = value;
			}
		}

		/// <summary>
		/// Texto do parse.
		/// </summary>
		public abstract string Text
		{
			get;
		}

		/// <summary>
		/// Construtor da classe
		/// </summary>
		/// <param name="translator">Tradutor de nomes dos objetos CLR para tabelas SQL.</param>
		/// <param name="typeSchema">Classe de recuperação de metadados.</param>
		public SqlQueryParser(IQueryTranslator translator, ITypeSchema typeSchema)
		{
			translator.Require("translator").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
			_translator = translator;
		}

		/// <summary>
		/// Construtor da classe
		/// </summary>
		/// <param name="translator">Tradutor de nomes dos objetos CLR para tabelas SQL.</param>
		/// <param name="typeSchema">Classe de recuperação de metadados.</param>
		/// <param name="queryInfo"></param>
		protected SqlQueryParser(IQueryTranslator translator, ITypeSchema typeSchema, QueryInfo queryInfo) : this(translator, typeSchema)
		{
			queryInfo.Require("queryInfo").NotNull();
			_query = queryInfo;
		}

		/// <summary>
		/// Monta a consulta sql a partir do objeto <see cref="QueryInfo"/>.
		/// </summary>
		/// <returns>String  que contém a consulta SQL.</returns>
		public abstract string GetText();

		/// <summary>
		/// Monta a consulta sql a partir do objeto <see cref="QueryInfo"/>.
		/// </summary>
		/// <param name="parts">Partes que devem ser recuperadas no texto.</param>
		/// <returns>String  que contém a consulta SQL.</returns>
		public abstract string GetText(SqlQueryPart parts);

		/// <summary>
		/// Recupera o nome da coluna.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public abstract string GetColumnName(Column column);

		/// <summary>
		/// Formata o alias da projeção.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		protected virtual string FormatProjectionAlias(string alias)
		{
			return alias;
		}

		/// <summary>
		/// Verifica se uma string é alias de alguma projeção
		/// </summary>
		/// <param name="expression">Expressão que contém o texto do alias</param>
		/// <returns>Retorna true se for e false se não</returns>
		protected bool IsProjectionAlias(string expression)
		{
			foreach (var proj in Query.Projection)
			{
				if(!String.IsNullOrEmpty(proj.Alias) && proj.Alias == expression)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Encontra uma entidade baseado no seu alias
		/// </summary>
		/// <param name="alias">Alias da entidade</param>
		/// <returns>Entidade a ser retornada ou nulo caso não seja encontratada</returns>
		protected EntityInfo GetEntity(string alias)
		{
			if(EntityAliasDictionary == null)
			{
				EntityAliasDictionary = new Dictionary<string, EntityInfo>();
				return FindAndAddEntity(alias);
			}
			EntityInfo entityInfo;
			if(EntityAliasDictionary.TryGetValue(alias, out entityInfo))
				return entityInfo;
			else
				return FindAndAddEntity(alias);
		}

		/// <summary>
		/// Verifica se uma string é o nome de algum parâmetro da query
		/// </summary>
		/// <param name="name">Nome do parâmetro a ser verificado</param>
		/// <returns>True se for e false caso contrário</returns>
		protected bool IsParameterOrConstant(string name)
		{
			if(String.IsNullOrEmpty(name))
				return false;
			if(name.StartsWith("?"))
				return true;
			if(name.StartsWith("'") && name.EndsWith("'"))
				return true;
			int aux;
			if(int.TryParse(name, out aux))
				return true;
			double aux1;
			if(double.TryParse(name, out aux1))
				return true;
			if(string.Compare(name, "NULL", true) == 0)
				return true;
			return false;
		}

		/// <summary>
		/// Encontra uma entidade interando pelo vetor de entidades do QueryInfo baseano no alias e adiciona ela no dicionário de Entidades interno
		/// </summary>
		/// <param name="alias">alias da entidade</param>
		/// <returns>Entidade a ser retornada ou nulo caso não seja encontratada</returns>
		private EntityInfo FindAndAddEntity(string alias)
		{
			foreach (var entity in Entities)
			{
				if(alias == entity.Alias)
				{
					EntityAliasDictionary.Add(alias, entity);
					return entity;
				}
			}
			throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_NotFoundEntityFromAlias, alias).Format());
		}
	}
}
