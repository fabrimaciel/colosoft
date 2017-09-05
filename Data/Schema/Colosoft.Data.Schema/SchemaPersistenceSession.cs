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
using Colosoft.Query;

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação da sessão de persistencia utilizando os dados do Schema.
	/// </summary>
	public class SchemaPersistenceSession : PersistenceSession
	{
		private ITypeSchema _typeSchema;

		private SchemaPersistenceSessionValidator _validator;

		/// <summary>
		/// Instancia do esquema dos tipos.
		/// </summary>
		protected virtual ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Instancia do esquema de tipos que será utilizado pela instancia.</param>
		/// <param name="executerFactory">Factory responsável pela criação de um executor.</param>
		public SchemaPersistenceSession(ITypeSchema typeSchema, Func<IPersistenceExecuter> executerFactory)
		{
			typeSchema.Require("typeSchema").NotNull();
			executerFactory.Require("executerFactory").NotNull();
			_typeSchema = typeSchema;
			_validator = new SchemaPersistenceSessionValidator(typeSchema);
			((IPersistenceExecuterFactory)this).ExecuterCreator = executerFactory;
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected SchemaPersistenceSession()
		{
		}

		/// <summary>
		/// Recupera as propriedades de persistencia que são usadas pela ação de inserção.
		/// </summary>
		/// <param name="instanceType"></param>
		/// <param name="propertyNames"></param>
		/// <param name="direction"></param>
		/// <param name="typeMetadata"></param>
		/// <returns></returns>
		private IEnumerable<System.Reflection.PropertyInfo> GetInsertPersistenceProperties(Type instanceType, string[] propertyNames, DirectionPropertiesName direction, ITypeMetadata typeMetadata)
		{
			var parameterTypes = new PersistenceParameterType[] {
				PersistenceParameterType.Field
			};
			var directions = new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert,
				DirectionParameter.OnlyInsert,
				DirectionParameter.InputOptionalOutputOnlyInsert
			};
			var mapping = typeMetadata.Where(f => directions.Contains(f.Direction) && parameterTypes.Contains(f.ParameterType)).ToList();
			FilterMapping(propertyNames, direction, mapping, typeMetadata, PersistenceActionType.Insert);
			var keyMapping = typeMetadata.GetKeyProperties();
			return mapping.Union(keyMapping, PropertyMetadataEqualityComparer.Instance).Select(f => instanceType.GetProperty(f.Name)).Where(f => f != null);
		}

		/// <summary>
		/// Recupera as propriedades de persistencia que são usadas pela ação de atualização.
		/// </summary>
		/// <param name="instanceType"></param>
		/// <param name="propertyNames"></param>
		/// <param name="direction"></param>
		/// <param name="typeMetadata"></param>
		/// <param name="isConditional"></param>
		/// <returns></returns>
		private IEnumerable<System.Reflection.PropertyInfo> GetUpdatePersistenceProperties(Type instanceType, string[] propertyNames, DirectionPropertiesName direction, ITypeMetadata typeMetadata, bool isConditional)
		{
			var parameterTypes = new PersistenceParameterType[] {
				PersistenceParameterType.Field
			};
			var directions = new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			};
			var mapping = typeMetadata.Where(f => directions.Contains(f.Direction) && parameterTypes.Contains(f.ParameterType)).ToList();
			FilterMapping(propertyNames, direction, mapping, typeMetadata, PersistenceActionType.Update);
			if(isConditional)
				return mapping.Select(f => instanceType.GetProperty(f.Name)).Where(f => f != null);
			else
				return mapping.Union(typeMetadata.GetKeyProperties(), PropertyMetadataEqualityComparer.Instance).Select(f => instanceType.GetProperty(f.Name)).Where(f => f != null);
		}

		/// <summary>
		/// Recupera as propriedades de persistencia que são usadas pela ação de atualização.
		/// </summary>
		/// <param name="instanceType"></param>
		/// <param name="propertyNames"></param>
		/// <param name="direction"></param>
		/// <param name="typeMetadata"></param>
		/// <param name="isConditional"></param>
		/// <returns></returns>
		private IEnumerable<System.Reflection.PropertyInfo> GetDeletePersistenceProperties(Type instanceType, string[] propertyNames, DirectionPropertiesName direction, ITypeMetadata typeMetadata, bool isConditional)
		{
			if(isConditional)
				return Enumerable.Empty<System.Reflection.PropertyInfo>();
			else
				return typeMetadata.GetKeyProperties().Select(f => instanceType.GetProperty(f.Name)).Where(f => f != null);
		}

		/// <summary>
		/// Filtra as propriedades mapeadas.
		/// </summary>
		/// <param name="propertyNames">Nome das propriedades que serão filtradas.</param>
		/// <param name="direction">Direção das propriedades.</param>
		/// <param name="mapping">Mapeamento onde o filtro será aplicado.</param>
		/// <param name="metadata">Metadados da entidade de persistência.</param>
		/// <param name="persistenceActionType">Tipo de ação de persistencia.</param>
		private void FilterMapping(string[] propertyNames, DirectionPropertiesName direction, List<IPropertyMetadata> mapping, ITypeMetadata metadata, PersistenceActionType persistenceActionType)
		{
			if(propertyNames != null && propertyNames.Length > 0)
			{
				List<int> indexs = new List<int>(propertyNames.Length);
				int i = 0;
				for(i = 0; i < propertyNames.Length; i++)
				{
					string p = propertyNames[i].Trim();
					int index = mapping.FindIndex(m => m.Name == p);
					if(index >= 0)
						indexs.Add(index);
				}
				if(direction == DirectionPropertiesName.Inclusion)
				{
					var mapping2 = new List<IPropertyMetadata>();
					for(i = 0; i < indexs.Count; i++)
						mapping2.Add(mapping[indexs[i]]);
					mapping.Clear();
					mapping.AddRange(mapping2);
				}
				else
				{
					for(i = 0; i < indexs.Count; i++)
						mapping.RemoveAt(indexs[i]);
				}
			}
		}

		/// <summary>
		/// Recupera o executar da sessão.
		/// </summary>
		/// <returns></returns>
		protected override IPersistenceExecuter CreateExecuter()
		{
			if(this.ExecuterCreator != null)
				return this.ExecuterCreator();
			return null;
		}

		/// <summary>
		/// Recupera as propriedades que compõem as chave do tipo da instancia informado.
		/// </summary>
		/// <param name="instanceType">Tipo da instancia.</param>
		/// <returns></returns>
		protected override IEnumerable<string> GetKeyProperties(Type instanceType)
		{
			var typeMetadata = TypeSchema.GetTypeMetadata(instanceType.FullName);
			if(typeMetadata == null)
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_TypeMetadataNotFound, instanceType.FullName).Format());
			return typeMetadata.GetKeyProperties().Select(f => f.Name);
		}

		/// <summary>
		/// Recupera as propriedades de persistencia.
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="instanceType"></param>
		/// <param name="propertyNames"></param>
		/// <param name="isConditional"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		protected override IEnumerable<System.Reflection.PropertyInfo> GetPersistenceProperties(PersistenceActionType actionType, Type instanceType, string[] propertyNames, bool isConditional, DirectionPropertiesName direction = DirectionPropertiesName.Inclusion)
		{
			var typeMetadata = TypeSchema.GetTypeMetadata(instanceType.FullName);
			if(typeMetadata == null)
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_TypeMetadataNotFound, instanceType.FullName).Format());
			if(actionType == PersistenceActionType.Insert)
				return GetInsertPersistenceProperties(instanceType, propertyNames, direction, typeMetadata);
			else if(actionType == PersistenceActionType.Update)
				return GetUpdatePersistenceProperties(instanceType, propertyNames, direction, typeMetadata, isConditional);
			else if(actionType == PersistenceActionType.Delete)
				return GetDeletePersistenceProperties(instanceType, propertyNames, direction, typeMetadata, isConditional);
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera a instancia do validador das sessões.
		/// </summary>
		/// <returns></returns>
		protected override IPersistenceSessionValidator GetValidator()
		{
			return _validator;
		}
	}
}
