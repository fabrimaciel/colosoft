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

namespace Colosoft.Data.Schema.GDA.Metadata
{
	/// <summary>
	/// Representa o esquema de tipos dos metadados do GDA.
	/// </summary>
	public class MetadataTypeSchema : Local.TypeSchema
	{
		private int _currentTypeCode = 1;

		private int _currentPropertyCode = 1;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		private MetadataTypeSchema()
		{
		}

		/// <summary>
		/// Recupera os metadados dos tipos do assembly informado.
		/// </summary>
		/// <param name="assembly">Assembly onde os metadados serão pesquisados.</param>
		/// <returns></returns>
		private IEnumerable<ITypeMetadata> GetTypeMetadataFromAssembly(System.Reflection.Assembly assembly)
		{
			var types = assembly.GetTypes();
			foreach (var type in types)
			{
				var customAttributes = type.GetCustomAttributes(typeof(global::GDA.PersistenceClassAttribute), false);
				if(customAttributes.Length > 0)
				{
					var persistenceClass = (global::GDA.PersistenceClassAttribute)customAttributes[0];
					var isCache = type.GetCustomAttributes(typeof(CacheAttribute), false).HasItems();
					var propertiesMetadata = new List<Local.PropertyMetadata>();
					foreach (var property in type.GetProperties())
					{
						var persistenceProperty = (global::GDA.PersistencePropertyAttribute)property.GetCustomAttributes(typeof(global::GDA.PersistencePropertyAttribute), false).FirstOrDefault();
						var propertyType = property.PropertyType;
						if(propertyType.IsNullable())
							propertyType = propertyType.GetRootType();
						if(propertyType == typeof(uint))
							propertyType = typeof(int);
						if(propertyType.IsEnum)
						{
							propertyType = Enum.GetUnderlyingType(propertyType);
						}
						if(persistenceProperty != null)
						{
							var parameterType = PersistenceParameterType.Field;
							switch(persistenceProperty.ParameterType)
							{
							case global::GDA.PersistenceParameterType.Field:
								parameterType = PersistenceParameterType.Field;
								break;
							case global::GDA.PersistenceParameterType.IdentityKey:
								parameterType = PersistenceParameterType.IdentityKey;
								break;
							case global::GDA.PersistenceParameterType.Key:
								parameterType = PersistenceParameterType.Key;
								break;
							}
							var directionParameter = (DirectionParameter)Enum.Parse(typeof(DirectionParameter), persistenceProperty.Direction.ToString());
							if(directionParameter == DirectionParameter.InputOptional || directionParameter == DirectionParameter.InputOptionalOutput)
								continue;
							if(directionParameter == DirectionParameter.InputOptionalOutputOnlyInsert || directionParameter == DirectionParameter.OutputOnlyInsert || directionParameter == DirectionParameter.OnlyInsert)
								directionParameter = DirectionParameter.InputOutput;
							var fkAttribute = property.GetCustomAttributes(typeof(global::GDA.PersistenceForeignKeyAttribute), false).FirstOrDefault() as global::GDA.PersistenceForeignKeyAttribute;
							var isCacheIndexed = parameterType == PersistenceParameterType.IdentityKey || parameterType == PersistenceParameterType.Key || fkAttribute != null || property.GetCustomAttributes(typeof(CacheIndexedAttribute), false).HasItems();
							var pMetadata = new Local.PropertyMetadata(_currentPropertyCode++, property.Name, persistenceProperty.Name, propertyType.FullName, null, isCacheIndexed, directionParameter, false, parameterType, false);
							if(fkAttribute != null)
							{
								pMetadata.ForeignKey = new Local.ForeignKeyInfo {
									Assembly = assembly.FullName,
									Namespace = fkAttribute.TypeOfClassRelated.Namespace,
									TypeName = fkAttribute.TypeOfClassRelated.Name,
									Property = fkAttribute.PropertyName
								};
							}
							propertiesMetadata.Add(pMetadata);
						}
					}
					var typeMetadata = new Colosoft.Data.Schema.Local.TypeMetadata(_currentTypeCode++, type.Name, type.Namespace, assembly.GetName().Name, isCache, false, new TableName(global::GDA.GDASettings.DefaultProviderName, "dbo", persistenceClass.Name), propertiesMetadata.Select(f => (IPropertyMetadata)f).ToArray());
					yield return typeMetadata;
				}
			}
		}

		/// <summary>
		/// Carrega os o esquema de tipos com base nos assemblies informados.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public static MetadataTypeSchema Load(params System.Reflection.Assembly[] assemblies)
		{
			var typeSchema = new MetadataTypeSchema();
			foreach (var assembly in assemblies)
				foreach (var typeMetadata in typeSchema.GetTypeMetadataFromAssembly(assembly))
					typeSchema.AddTypeMetadata(typeMetadata);
			typeSchema.FixForeignKeys();
			return typeSchema;
		}
	}
}
