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
using Colosoft.Data.Schema;

namespace Colosoft.Query.Database
{
	/// <summary>
	/// Nome da tabela
	/// </summary>
	public class TranslatedTableName : ITranslatedName
	{
		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema da tabela.
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="name"></param>
		public TranslatedTableName(string schema, string name)
		{
			name.Require("name").NotNull().NotEmpty();
			Schema = schema;
			Name = name;
		}

		/// <summary>
		/// Compara com outro nome.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ITranslatedName other)
		{
			var name = other as TranslatedTableName;
			if(name != null)
			{
				var result = string.Compare(this.Schema, name.Schema);
				if(result == 0)
					result = string.Compare(this.Name, name.Name);
				return result;
			}
			else
				return -1;
		}
	}
	/// <summary>
	/// Representa o nome de uma coluna.
	/// </summary>
	public class TranslatedColumnName : ITranslatedName
	{
		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Apelido da tabela associado com a coluna.
		/// </summary>
		public string TableAlias
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo da propriedade.
		/// </summary>
		public Type PropertyType
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="tableAlias">Apelido da tabela.</param>
		/// <param name="propertyType">Tipo da propriedade.</param>
		public TranslatedColumnName(string name, string tableAlias, Type propertyType)
		{
			name.Require("name").NotNull().NotEmpty();
			Name = name;
			TableAlias = tableAlias;
			PropertyType = propertyType;
		}

		/// <summary>
		/// Compara com outro nome.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ITranslatedName other)
		{
			var name = other as TranslatedColumnName;
			if(name != null)
				return string.Compare(this.Name, name.Name);
			return -1;
		}
	}
	/// <summary>
	/// Armazena o nome traduzido da stored procedure.
	/// </summary>
	public class TranslatedStoredProcedureName : ITranslatedName
	{
		/// <summary>
		/// Nome do stored procedure.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema da stored procedure.
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		public TranslatedStoredProcedureName(string name, string schema)
		{
			this.Name = name;
			this.Schema = schema;
		}

		/// <summary>
		/// Compara com outro nome.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ITranslatedName other)
		{
			var name = other as StoredProcedureName;
			if(name != null)
				return string.Compare(this.Schema + "." + this.Name, name.Schema + "." + name.Name);
			return -1;
		}
	}
	/// <summary>
	/// Armazena uma parte do Statement Select.
	/// </summary>
	public class TranslatedSelectPart : ITranslatedName
	{
		/// <summary>
		/// Texto da parte da seleção.
		/// </summary>
		public string Part
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="part"></param>
		public TranslatedSelectPart(string part)
		{
			this.Part = part;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Part;
		}

		/// <summary>
		/// Compara com outro nome.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ITranslatedName other)
		{
			var name = other as TranslatedSelectPart;
			if(name != null)
				return string.Compare(this.Part, name.Part);
			return -1;
		}
	}
	/// <summary>
	/// Implementação da interface que realiza a tradução do nome da classe para os dados de uma table do sistema.
	/// </summary>
	public class SqlQueryTranslator : IQueryTranslator
	{
		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Objeto de recuperação de metadados</param>
		public SqlQueryTranslator(Data.Schema.ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Recupera os metadados do tipo pelo nome informado.
		/// </summary>
		/// <param name="fullName">Nome informado</param>
		/// <returns>Metadados do tipo</returns>
		private Data.Schema.ITypeMetadata GetTypeMetadata(string fullName)
		{
			if(string.IsNullOrEmpty(fullName))
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.ArgumentException_EntityFullNameIsEmpty).Format());
			var typeMetadata = _typeSchema.GetTypeMetadata(fullName);
			if(typeMetadata == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_TypeMetadataNotFoundByFullName, fullName).Format());
			return typeMetadata;
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <param name="translatedName">Nome traduzido que foi encontrado.</param>
		/// <param name="error">Erro ocorrido.</param>
		/// <returns>Retorna o nome da entidade</returns>
		private bool TryGetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema, out ITranslatedName translatedName, out Exception error)
		{
			entity.Require("entity").NotNull();
			entity.Require("columnName").NotNull();
			string name = null;
			if(entity.SubQuery != null || ignoreTypeSchema)
			{
				name = propertyName;
			}
			else
			{
				var typeMetadata = GetTypeMetadata(entity.FullName);
				IPropertyMetadata propertyMetadata = null;
				if(!typeMetadata.TryGet(propertyName, out propertyMetadata))
				{
					if(propertyName == DataAccessConstants.RowVersionPropertyName)
					{
						name = DataAccessConstants.RowVersionColumnName;
					}
					else if(StringComparer.InvariantCultureIgnoreCase.Equals(propertyName, "TableId"))
					{
						var identityMetadatas = typeMetadata.GetKeyProperties();
						var identityEnumerator = identityMetadatas.GetEnumerator();
						identityEnumerator.MoveNext();
						var identityMetadata = identityEnumerator.Current;
						if(identityEnumerator.MoveNext())
						{
							translatedName = null;
							error = new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.NotSupportedException_TableIdOnlySupportedInNonCompositePrimaryKeyTable, typeMetadata.FullName).Format());
							return false;
						}
						else
							name = identityMetadata.ColumnName;
					}
					else
					{
						translatedName = null;
						error = new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_PropertyNotMappedForType, propertyName, typeMetadata.FullName).Format());
						return false;
					}
				}
				else
					name = propertyMetadata.ColumnName;
			}
			translatedName = new TranslatedColumnName(name, entity.Alias, null);
			error = null;
			return true;
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual ITranslatedName GetName(EntityInfo entity, string propertyName)
		{
			return GetName(entity, propertyName, false);
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual ITranslatedName GetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema)
		{
			Exception error = null;
			ITranslatedName result = null;
			if(!TryGetName(entity, propertyName, ignoreTypeSchema, out result, out error))
				throw error;
			return result;
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="name"></param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual bool TryGetName(EntityInfo entity, string propertyName, out ITranslatedName name)
		{
			return TryGetName(entity, propertyName, false, out name);
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <param name="name"></param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual bool TryGetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema, out ITranslatedName name)
		{
			Exception error = null;
			return TryGetName(entity, propertyName, ignoreTypeSchema, out name, out error);
		}

		/// <summary>
		/// Recupera o nome da tabela associado com a entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual ITranslatedName GetName(EntityInfo entity)
		{
			return GetName(entity, false);
		}

		/// <summary>
		/// Recupera o nome da tabela associado com a entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual ITranslatedName GetName(EntityInfo entity, bool ignoreTypeSchema)
		{
			entity.Require("entity").NotNull();
			if(ignoreTypeSchema)
			{
				var parts = entity.FullName.Split('.');
				if(parts.Length > 1)
					return new TranslatedTableName(parts[0], parts[1]);
				else
					return new TranslatedTableName("", entity.FullName);
			}
			else
			{
				var tableName = GetTypeMetadata(entity.FullName).TableName;
				return new TranslatedTableName(tableName.Schema, tableName.Name);
			}
		}

		/// <summary>
		/// Pega o nome da storedprocedure.
		/// </summary>
		/// <param name="storedProcedureName">Nome da stored procedure.</param>
		/// <returns></returns>
		public virtual ITranslatedName GetName(StoredProcedureName storedProcedureName)
		{
			return new TranslatedStoredProcedureName(storedProcedureName.Name, storedProcedureName.Schema);
		}
	}
}
