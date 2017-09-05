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

namespace Colosoft.Mef
{
	/// <summary>
	/// Metodos de extensão para o provedor de exportação.
	/// </summary>
	public static class ExportProviderExtensions
	{
		private static readonly Type DefaultMetadataViewType;

		private static System.Reflection.FieldInfo _compositionContainerRootProviderField;

		private static System.Reflection.FieldInfo _composablePartExportProviderPartsField;

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static ExportProviderExtensions()
		{
			DefaultMetadataViewType = typeof(IDictionary<string, object>);
		}

		/// <summary>
		/// Recupera o provedor de export.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private static System.ComponentModel.Composition.Hosting.ExportProvider GetRootExportProvider(System.ComponentModel.Composition.Hosting.CompositionContainer container)
		{
			if(_compositionContainerRootProviderField == null)
				_compositionContainerRootProviderField = typeof(System.ComponentModel.Composition.Hosting.CompositionContainer).GetField("_rootProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return (System.ComponentModel.Composition.Hosting.ExportProvider)_compositionContainerRootProviderField.GetValue(container);
		}

		/// <summary>
		/// Recupera as parts do export provider informado.
		/// </summary>
		/// <param name="exportProvider"></param>
		/// <returns></returns>
		private static IEnumerable<System.ComponentModel.Composition.Primitives.ComposablePart> GetComposeParts(System.ComponentModel.Composition.Hosting.ComposablePartExportProvider exportProvider)
		{
			if(_composablePartExportProviderPartsField == null)
				_composablePartExportProviderPartsField = typeof(System.ComponentModel.Composition.Hosting.ComposablePartExportProvider).GetField("_parts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return (List<System.ComponentModel.Composition.Primitives.ComposablePart>)_composablePartExportProviderPartsField.GetValue(exportProvider);
		}

		/// <summary>
		/// Constrói da definição de import.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="metadataViewType"></param>
		/// <param name="contractName"></param>
		/// <param name="cardinality"></param>
		/// <returns></returns>
		private static System.ComponentModel.Composition.Primitives.ImportDefinition BuildImportDefinition(Type type, Type metadataViewType, string contractName, System.ComponentModel.Composition.Primitives.ImportCardinality cardinality)
		{
			IEnumerable<KeyValuePair<string, Type>> requiredMetadata = GetRequiredMetadata(metadataViewType);
			string requiredTypeIdentity = null;
			if(type != typeof(object))
				requiredTypeIdentity = System.ComponentModel.Composition.AttributedModelServices.GetTypeIdentity(type);
			return new System.ComponentModel.Composition.Primitives.ContractBasedImportDefinition(contractName, requiredTypeIdentity, requiredMetadata, cardinality, false, true, System.ComponentModel.Composition.CreationPolicy.Any);
		}

		/// <summary>
		/// Verifica se o tipo de view de metadados padrão.
		/// </summary>
		/// <param name="metadataViewType"></param>
		/// <returns></returns>
		private static bool IsDefaultMetadataViewType(Type metadataViewType)
		{
			metadataViewType.Require("metadataViewType").NotNull();
			return metadataViewType.IsAssignableFrom(DefaultMetadataViewType);
		}

		internal static bool IsDictionaryConstructorViewType(Type metadataViewType)
		{
			metadataViewType.Require("metadataViewType").NotNull();
			return (metadataViewType.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, Type.DefaultBinder, new Type[] {
				typeof(IDictionary<string, object>)
			}, new System.Reflection.ParameterModifier[0]) != null);
		}

		/// <summary>
		/// Recupera osm atributos do tipo especificado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributeProvider"></param>
		/// <returns></returns>
		public static T[] GetAttributes<T>(this System.Reflection.ICustomAttributeProvider attributeProvider) where T : class
		{
			return (T[])attributeProvider.GetCustomAttributes(typeof(T), false);
		}

		/// <summary>
		/// Recupera o primeiro atributo do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributeProvider"></param>
		/// <returns></returns>
		public static T GetFirstAttribute<T>(this System.Reflection.ICustomAttributeProvider attributeProvider) where T : class
		{
			return attributeProvider.GetAttributes<T>().FirstOrDefault<T>();
		}

		/// <summary>
		/// Recupera todas as propriedades do tipo informado
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<System.Reflection.PropertyInfo> GetAllProperties(this Type type)
		{
			return type.GetInterfaces().Concat<Type>(new Type[] {
				type
			}).SelectMany(f => f.GetProperties());
		}

		/// <summary>
		/// Recupera os metadados requeridos
		/// </summary>
		/// <param name="metadataViewType"></param>
		/// <returns></returns>
		private static IEnumerable<KeyValuePair<string, Type>> GetRequiredMetadata(Type metadataViewType)
		{
			if(((metadataViewType == null) || IsDefaultMetadataViewType(metadataViewType)) || (IsDictionaryConstructorViewType(metadataViewType) || !metadataViewType.IsInterface))
			{
				return Enumerable.Empty<KeyValuePair<string, Type>>();
			}
			return metadataViewType.GetAllProperties().Where(f => f.GetFirstAttribute<System.ComponentModel.DefaultValueAttribute>() == null).Select(f => new KeyValuePair<string, Type>(f.Name, f.PropertyType));
		}

		/// <summary>
		/// Verifica se é um tipo de view válida.
		/// </summary>
		/// <param name="metadataViewType"></param>
		/// <returns></returns>
		public static bool IsViewTypeValid(Type metadataViewType)
		{
			metadataViewType.Require("metadataViewType");
			if((!IsDefaultMetadataViewType(metadataViewType) && !metadataViewType.IsInterface) && !IsDictionaryConstructorViewType(metadataViewType))
				return false;
			return true;
		}

		private static IEnumerable<System.ComponentModel.Composition.Primitives.Export> GetExportsCore(System.ComponentModel.Composition.Hosting.ExportProvider exportProvider, Type type, Type metadataViewType, string contractName, System.ComponentModel.Composition.Primitives.ImportCardinality cardinality)
		{
			type.Require("type").NotNull();
			if(string.IsNullOrEmpty(contractName))
				contractName = System.ComponentModel.Composition.AttributedModelServices.GetContractName(type);
			if(metadataViewType == null)
				metadataViewType = DefaultMetadataViewType;
			if(!IsViewTypeValid(metadataViewType))
				throw new InvalidOperationException(string.Format("Invalid MetadataView '{0}'", metadataViewType.Name));
			var definition = BuildImportDefinition(type, metadataViewType, contractName, cardinality);
			return exportProvider.GetExports(definition, null);
		}

		/// <summary>
		/// Tenta recupera as definições de exportação.
		/// </summary>
		/// <param name="exportProvider"></param>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		/// <param name="exportDefintions"></param>
		/// <returns></returns>
		public static bool TryGetExportDefintions(System.ComponentModel.Composition.Hosting.ExportProvider exportProvider, Type type, string contractName, out IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> exportDefintions)
		{
			if(string.IsNullOrEmpty(contractName))
				contractName = System.ComponentModel.Composition.AttributedModelServices.GetContractName(type);
			var result = new List<System.ComponentModel.Composition.Primitives.ExportDefinition>();
			if(exportProvider is System.ComponentModel.Composition.Hosting.CompositionContainer)
			{
				var container = (System.ComponentModel.Composition.Hosting.CompositionContainer)exportProvider;
				var rootExportProvider = GetRootExportProvider(container) as System.ComponentModel.Composition.Hosting.AggregateExportProvider;
				if(rootExportProvider != null)
				{
					foreach (var provider in rootExportProvider.Providers)
					{
						if(provider is System.ComponentModel.Composition.Hosting.ComposablePartExportProvider)
						{
							foreach (var part in GetComposeParts((System.ComponentModel.Composition.Hosting.ComposablePartExportProvider)provider))
								result.AddRange(GetExportDefinitions(part.ExportDefinitions, contractName, type.FullName));
						}
						else if(provider is System.ComponentModel.Composition.Hosting.CatalogExportProvider)
						{
							var catalog = ((System.ComponentModel.Composition.Hosting.CatalogExportProvider)provider).Catalog;
							result.AddRange(GetExportDefinitions(container.Catalog, contractName, type.FullName));
						}
					}
				}
				else
				{
					result.AddRange(GetExportDefinitions(container.Catalog, contractName, type.FullName));
				}
			}
			exportDefintions = result;
			return (result.Count > 0);
		}

		/// <summary>
		/// Recupera a definição de export 
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="contractName"></param>
		/// <param name="typeFullName"></param>
		private static IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> GetExportDefinitions(System.ComponentModel.Composition.Primitives.ComposablePartCatalog catalog, string contractName, string typeFullName)
		{
			foreach (var partDefinition in catalog.Parts)
				foreach (var i in GetExportDefinitions(partDefinition.ExportDefinitions, contractName, typeFullName))
					yield return i;
		}

		/// <summary>
		/// Localiza a definição de export com os parametros informados.
		/// </summary>
		/// <param name="exportDefintions"></param>
		/// <param name="contractName"></param>
		/// <param name="typeFullName"></param>
		/// <returns></returns>
		private static IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> GetExportDefinitions(IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> exportDefintions, string contractName, string typeFullName)
		{
			object metadataValue = null;
			foreach (var exportDefinition in exportDefintions)
				if(exportDefinition.ContractName == contractName && exportDefinition.Metadata.TryGetValue("ExportTypeIdentity", out metadataValue) && (metadataValue ?? "").ToString() == typeFullName)
				{
					yield return exportDefinition;
				}
		}

		/// <summary>
		/// Tenta recuperar os exposrt do tipo informado.
		/// </summary>
		/// <param name="exportProvider"></param>
		/// <param name="type"></param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="exports"></param>
		/// <returns></returns>
		public static bool TryGetExports(System.ComponentModel.Composition.Hosting.ExportProvider exportProvider, Type type, string contractName, out IEnumerable<System.ComponentModel.Composition.Primitives.Export> exports)
		{
			if(string.IsNullOrEmpty(contractName))
				contractName = System.ComponentModel.Composition.AttributedModelServices.GetContractName(type);
			exports = GetExportsCore(exportProvider, type, null, contractName, System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrMore);
			return true;
		}
	}
}
