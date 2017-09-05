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

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação da interface que realiza a tradução do nome da classe para os dados de uma table do sistema.
	/// </summary>
	public class CacheQueryTranslator : Colosoft.Query.IQueryTranslator
	{
		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Objeto de recuperação de metadados</param>
		public CacheQueryTranslator(Colosoft.Data.Schema.ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Recupera os metadados do tipo pelo nome informado.
		/// </summary>
		/// <param name="fullName">Nome informado</param>
		/// <returns>Metadados do tipo</returns>
		private Colosoft.Data.Schema.ITypeMetadata GetTypeMetadata(string fullName)
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
		/// <param name="translatedName">Nome traduzido que foi encontrado.</param>
		/// <param name="error">Erro ocorrido.</param>
		/// <returns>Retorna o nome da entidade</returns>
		private bool TryGetName(Colosoft.Query.EntityInfo entity, string propertyName, out Colosoft.Query.ITranslatedName translatedName, out Exception error)
		{
			entity.Require("entity").NotNull();
			entity.Require("columnName").NotNull();
			Colosoft.Data.Schema.IPropertyMetadata propertyMetadata = null;
			Colosoft.Data.Schema.ITypeMetadata typeMetadata = null;
			try
			{
				typeMetadata = GetTypeMetadata(entity.FullName);
				propertyMetadata = typeMetadata[propertyName];
			}
			catch
			{
			}
			string name;
			if(propertyMetadata == null)
			{
				if(StringComparer.InvariantCultureIgnoreCase.Equals(propertyName, "TableId"))
				{
					var identityMetadatas = typeMetadata.GetKeyProperties();
					using (var identityEnumerator = identityMetadatas.GetEnumerator())
					{
						identityEnumerator.MoveNext();
						var identityMetadata = identityEnumerator.Current;
						if(identityEnumerator.MoveNext())
						{
							translatedName = null;
							error = new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.NotSupportedException_TableIdOnlySupportedInNonCompositePrimaryKeyTable, typeMetadata.FullName).Format());
							return false;
						}
						else
							name = identityMetadata.Name;
					}
				}
				else
					name = propertyName;
			}
			else
				name = propertyMetadata.Name;
			translatedName = new TranslatedPropertyName(name, entity.Alias, null);
			error = null;
			return true;
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual Colosoft.Query.ITranslatedName GetName(Colosoft.Query.EntityInfo entity, string propertyName)
		{
			Exception error = null;
			Colosoft.Query.ITranslatedName result = null;
			if(!TryGetName(entity, propertyName, out result, out error))
				throw error;
			return result;
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <param name="ignoreTypeSchema"></param>
		/// <returns></returns>
		public virtual Colosoft.Query.ITranslatedName GetName(Colosoft.Query.EntityInfo entity, string propertyName, bool ignoreTypeSchema)
		{
			return GetName(entity, propertyName);
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual bool TryGetName(Colosoft.Query.EntityInfo entity, string propertyName, out Colosoft.Query.ITranslatedName name)
		{
			Exception error = null;
			return TryGetName(entity, propertyName, out name, out error);
		}

		/// <summary>
		/// Recupera o nome da coluna associado com as informações da entidade.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <param name="ignoreTypeSchema"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual bool TryGetName(Colosoft.Query.EntityInfo entity, string propertyName, bool ignoreTypeSchema, out Colosoft.Query.ITranslatedName name)
		{
			return TryGetName(entity, propertyName, out name);
		}

		/// <summary>
		/// Recupera o nome da tabela associado com a entidade.
		/// </summary>
		/// <param name="entity">Entidade</param>
		/// <returns>Retorna o nome da entidade</returns>
		public virtual Colosoft.Query.ITranslatedName GetName(Colosoft.Query.EntityInfo entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o nome da tabela associado com a entidade.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="ignoreTypeSchema"></param>
		/// <returns></returns>
		public virtual Colosoft.Query.ITranslatedName GetName(Colosoft.Query.EntityInfo entity, bool ignoreTypeSchema)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Pega o nome da storedprocedure.
		/// </summary>
		/// <param name="storedProcedureName">Nome da stored procedure.</param>
		/// <returns></returns>
		public virtual Colosoft.Query.ITranslatedName GetName(Colosoft.Query.StoredProcedureName storedProcedureName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Representa o nome de uma coluna.
		/// </summary>
		public class TranslatedPropertyName : Colosoft.Query.ITranslatedName
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
			public TranslatedPropertyName(string name, string tableAlias, Type propertyType)
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
			public int CompareTo(Colosoft.Query.ITranslatedName other)
			{
				var name = other as TranslatedPropertyName;
				if(name != null)
					return string.Compare(this.Name, name.Name);
				return -1;
			}
		}
	}
}
