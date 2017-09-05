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
using System.ComponentModel.Composition.Primitives;

namespace Colosoft.Mef
{
	class ProviderParameterImportDefinition : ContractBasedImportDefinition
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="importDescription"></param>
		/// <param name="creationPolicy"></param>
		public ProviderParameterImportDefinition(System.Reflection.ParameterInfo parameter, ImportDescription importDescription, System.ComponentModel.Composition.CreationPolicy creationPolicy) : base(CompositionServices.GetContractNameFromImportDescription(parameter, importDescription), CompositionServices.GetTypeIdentityFromImportDescription(parameter, importDescription), CompositionServices.GetMetadataFromImportDescription(parameter, importDescription), System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne, importDescription.Recomposable, importDescription.Prerequisite, creationPolicy)
		{
			parameter.Require("parameter").NotNull();
			importDescription.Require("importDescription").NotNull();
			this.AllowDefault = importDescription.AllowDefault;
			this.Parameter = parameter;
		}

		/// <summary>
		/// Identifica se a propriedade ou campo pode ser definido com um valor padrão.
		/// </summary>
		public bool AllowDefault
		{
			get;
			protected set;
		}

		/// <summary>
		/// Recupera a expressão que define as condições para encontrar a import.
		/// </summary>
		/// <returns></returns>
		private static System.Linq.Expressions.Expression<Func<ExportDefinition, bool>> GetConstraint(string contractName, IEnumerable<string> requiredMetadata)
		{
			string x = contractName;
			IEnumerable<string> y = requiredMetadata ?? Enumerable.Empty<string>();
			System.Linq.Expressions.Expression<Func<ExportDefinition, bool>> constraint = exportDefinition => exportDefinition.ContractName == x && y.All(m => exportDefinition.Metadata.ContainsKey(m));
			return constraint;
		}

		/// <summary>
		/// 
		/// </summary>
		public System.Reflection.ParameterInfo Parameter
		{
			get;
			protected set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} (ContractName=\"{1}\")", Parameter.Name, ContractName);
		}
	}
}
