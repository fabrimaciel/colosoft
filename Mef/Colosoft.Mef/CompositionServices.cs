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
using System.Reflection;
using System.ComponentModel.Composition;

namespace Colosoft.Mef
{
	/// <summary>
	/// Prover os serviço associados com um provedor de composição.
	/// </summary>
	static class CompositionServices
	{
		/// <summary>
		/// Recupera o nome do contrato do <see cref="MemberInfo"/> e <see cref="ExportDescription"/>.
		/// </summary>
		/// <param name="memberGetter">Informações sobre o membro.</param>
		/// <param name="exportDescription">Informações sobre a definição de export.</param>
		/// <returns>Nome do contrato.</returns>
		public static string GetContractNameFromExportDescription(Func<MemberInfo> memberGetter, ExportDescription exportDescription)
		{
			memberGetter.Require("member").NotNull();
			exportDescription.Require("exportDescription").NotNull();
			if(!string.IsNullOrEmpty(exportDescription.ContractName))
				return exportDescription.ContractName;
			if(exportDescription.ContractTypeName != null)
				return exportDescription.ContractTypeName.FullName;
			if(exportDescription.ContractType != null)
				return AttributedModelServices.GetContractName(exportDescription.ContractType);
			var memberValue = memberGetter();
			if(memberValue == null)
				throw new InvalidOperationException("member is null");
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			return AttributedModelServices.GetContractName(memberValue.GetDefaultTypeFromMember());
		}

		/// <summary>
		/// Recupera o nome do contrato do <see cref="MemberInfo"/> e <see cref="ImportDescription"/>.
		/// </summary>
		/// <param name="member">Informações do membro.</param>
		/// <param name="importDescription">Informações sobre a definição do import.</param>
		/// <returns>Nome do contrato.</returns>
		public static string GetContractNameFromImportDescription(Lazy<MemberInfo> member, ImportDescription importDescription)
		{
			member.Require("member").NotNull();
			importDescription.Require("importDescription").NotNull();
			if(!string.IsNullOrEmpty(importDescription.ContractName))
				return importDescription.ContractName;
			if(importDescription.ContractType != null)
				return AttributedModelServices.GetContractName(importDescription.ContractType);
			var memberValue = member.Value;
			if(memberValue == null)
				throw new InvalidOperationException("member is null");
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			else if(memberValue.MemberType == MemberTypes.Constructor)
				return AttributedModelServices.GetContractName(memberValue.GetDefaultTypeFromMember());
			return AttributedModelServices.GetContractName(memberValue.GetDefaultTypeFromMember());
		}

		/// <summary>
		/// Recupera o nome do contrato do <see cref="ParameterInfo"/> e <see cref="ExportDescription"/>
		/// </summary>
		/// <param name="member">Informações sobre o membro.</param>
		/// <param name="importDescription">Informações sobre a definição de export.</param>
		/// <returns>Nome do contrato.</returns>
		public static string GetContractNameFromImportDescription(ParameterInfo member, ImportDescription importDescription)
		{
			member.Require("member").NotNull();
			importDescription.Require("importDescription").NotNull();
			if(!string.IsNullOrEmpty(importDescription.ContractName))
				return importDescription.ContractName;
			if(importDescription.ContractType != null)
				return AttributedModelServices.GetContractName(importDescription.ContractType);
			return AttributedModelServices.GetContractName(member.ParameterType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="memberGetter"></param>
		/// <param name="exportDescription"></param>
		/// <param name="creationPolicy"></param>
		/// <returns></returns>
		public static IDictionary<string, object> GetMetadataFromExportDescription(Func<MemberInfo> memberGetter, ExportDescription exportDescription, CreationPolicy creationPolicy)
		{
			memberGetter.Require("member").NotNull();
			exportDescription.Require("exportDescription").NotNull();
			var metadata = new Dictionary<string, object>();
			var contract = GetTypeIdentityFromExportDescription(memberGetter, exportDescription);
			metadata.Add(System.ComponentModel.Composition.Hosting.CompositionConstants.ExportTypeIdentityMetadataName, contract);
			if(creationPolicy != CreationPolicy.Any)
				metadata.Add("System.ComponentModel.Composition.CreationPolicy", creationPolicy);
			if(exportDescription.Metadata != null)
			{
				foreach (var pair in exportDescription.Metadata)
					metadata.Add(pair.Key, pair.Value);
			}
			return metadata;
		}

		/// <summary>
		/// Recupera os metadados de um descrição de import.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="importDescription"></param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<string, Type>> GetMetadataFromImportDescription(this Lazy<MemberInfo> member, ImportDescription importDescription)
		{
			member.Require("member").NotNull();
			importDescription.Require("importDescription").NotNull();
			var metadata = new Dictionary<string, Type>();
			return metadata;
		}

		/// <summary>
		/// Recupera os metadados de um descrição de import.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="importDescription"></param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<string, Type>> GetMetadataFromImportDescription(this ParameterInfo parameter, ImportDescription importDescription)
		{
			parameter.Require("parameter").NotNull();
			importDescription.Require("importDescription").NotNull();
			var metadata = new Dictionary<string, Type>();
			return metadata;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="memberGetter"></param>
		/// <param name="export"></param>
		/// <returns></returns>
		public static string GetTypeIdentityFromExportDescription(this Func<MemberInfo> memberGetter, ExportDescription export)
		{
			if(export.ContractTypeName != null)
				return export.ContractTypeName.FullName;
			if(export.ContractType != null)
				return AttributedModelServices.GetTypeIdentity(export.ContractType);
			var memberValue = memberGetter();
			if(memberValue == null)
				throw new InvalidOperationException("member is null");
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			return AttributedModelServices.GetTypeIdentity(memberValue.GetDefaultTypeFromMember());
		}

		/// <summary>
		/// Recupera a identidade do tipo com base na descrição do import.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="import"></param>
		/// <returns></returns>
		public static string GetTypeIdentityFromImportDescription(this Lazy<MemberInfo> member, ImportDescription import)
		{
			if(import.ContractTypeName != null)
				return import.ContractTypeName.FullName;
			if(import.ContractType != null)
				return AttributedModelServices.GetTypeIdentity(import.ContractType);
			var memberValue = member.Value;
			if(memberValue == null)
				throw new InvalidOperationException("member is null");
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			if(memberValue.MemberType == MemberTypes.Method)
				return AttributedModelServices.GetTypeIdentity((MethodInfo)memberValue);
			return AttributedModelServices.GetTypeIdentity(memberValue.GetDefaultTypeFromMember());
		}

		/// <summary>
		/// Recupera a identidade do para paremtro.
		/// </summary>
		/// <param name="parameterInfo"></param>
		/// <param name="import"></param>
		/// <returns></returns>
		public static string GetTypeIdentityFromImportDescription(this ParameterInfo parameterInfo, ImportDescription import)
		{
			if(import.ContractTypeName != null)
				return import.ContractTypeName.FullName;
			if(import.ContractType != null)
				return AttributedModelServices.GetTypeIdentity(import.ContractType);
			return AttributedModelServices.GetTypeIdentity(parameterInfo.ParameterType);
		}

		/// <summary>
		/// Recupera o tipo padrão para o membro.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		private static Type GetDefaultTypeFromMember(this MemberInfo member)
		{
			switch(member.MemberType)
			{
			case MemberTypes.Property:
				return ((PropertyInfo)member).PropertyType;
			case MemberTypes.NestedType:
			case MemberTypes.TypeInfo:
				return ((Type)member);
			case MemberTypes.Constructor:
				return ((ConstructorInfo)member).DeclaringType;
			case MemberTypes.Field:
			default:
				return ((FieldInfo)member).FieldType;
			}
		}

		/// <summary>
		/// Recupera o politica de criação requerida.
		/// </summary>
		/// <param name="definition"></param>
		/// <returns></returns>
		internal static CreationPolicy GetRequiredCreationPolicy(this System.ComponentModel.Composition.Primitives.ImportDefinition definition)
		{
			var definition2 = definition as System.ComponentModel.Composition.Primitives.ContractBasedImportDefinition;
			if(definition2 != null)
				return definition2.RequiredCreationPolicy;
			return CreationPolicy.Any;
		}
	}
}
