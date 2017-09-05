﻿/* 
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
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace Colosoft.Mef
{
	/// <summary>
	/// Implementação da definição de import
	/// </summary>
	class ProviderImportDefinition : ContractBasedImportDefinition
	{
		private Lazy<System.Reflection.MemberInfo> _member;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="importDescription"></param>
		/// <param name="creationPolicy">Política de criação.</param>
		public ProviderImportDefinition(Lazy<MemberInfo> member, ImportDescription importDescription, System.ComponentModel.Composition.CreationPolicy creationPolicy) : base(CompositionServices.GetContractNameFromImportDescription(member, importDescription), CompositionServices.GetTypeIdentityFromImportDescription(member, importDescription), CompositionServices.GetMetadataFromImportDescription(member, importDescription), GetCardinality(member, importDescription.AllowDefault), importDescription.Recomposable, importDescription.Prerequisite, creationPolicy)
		{
			member.Require("member").NotNull();
			importDescription.Require("importDescription").NotNull();
			this.AllowDefault = importDescription.AllowDefault;
			_member = member;
		}

		/// <summary>
		/// Identifica se a propriedade ou campo pode ser definido com um valor padrão.
		/// </summary>
		public bool AllowDefault
		{
			get;
			protected set;
		}

		private static System.ComponentModel.Composition.CreationPolicy Convert(Colosoft.Reflection.Composition.CreationPolicy creationPolicy)
		{
			switch(creationPolicy)
			{
			case Reflection.Composition.CreationPolicy.NonShared:
				return System.ComponentModel.Composition.CreationPolicy.NonShared;
			case Reflection.Composition.CreationPolicy.Shared:
				return System.ComponentModel.Composition.CreationPolicy.Shared;
			default:
				return System.ComponentModel.Composition.CreationPolicy.Any;
			}
		}

		/// <summary>
		/// Recupera uma expressão que define as condições para compara a descrição do import
		/// </summary>
		/// <returns>
		/// </returns>
		private static System.Linq.Expressions.Expression<Func<ExportDefinition, bool>> GetConstraint(string contractName, IEnumerable<string> requiredMetadata)
		{
			string x = contractName;
			IEnumerable<string> y = requiredMetadata ?? Enumerable.Empty<string>();
			System.Linq.Expressions.Expression<Func<ExportDefinition, bool>> constraint = exportDefinition => exportDefinition.ContractName == x && y.All(m => exportDefinition.Metadata.ContainsKey(m));
			return constraint;
		}

		private static ImportCardinality GetCardinality(Lazy<MemberInfo> member, bool allowDefault)
		{
			return (allowDefault) ? System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrOne : System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne;
		}

		/// <summary>
		/// Membro que definido par ao import.
		/// </summary>
		public MemberInfo Member
		{
			get
			{
				return _member.Value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} (ContractName=\"{1}\")", _member.IsValueCreated ? _member.Value.Name : "", ContractName);
		}
	}
}