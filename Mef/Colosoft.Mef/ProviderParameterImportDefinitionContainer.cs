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
	/// Representa um container das definições de exportação.
	/// </summary>
	class ProviderParameterImportDefinitionContainer
	{
		private PartDescription _partDescription;

		private IAssemblyContainer _assemblyContainer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="partDescription"></param>
		/// <param name="assemblyContainer"></param>
		public ProviderParameterImportDefinitionContainer(PartDescription partDescription, IAssemblyContainer assemblyContainer)
		{
			partDescription.Require("partDescription");
			assemblyContainer.Require("assemblyContainer");
			_partDescription = partDescription;
			_assemblyContainer = assemblyContainer;
		}

		/// <summary>
		/// Recupera as exportações de definição.
		/// </summary>
		/// <returns></returns>
		public ProviderParameterImportDefinition[] GetImportDefinitions()
		{
			var imports = new List<ProviderParameterImportDefinition>();
			if(_partDescription.ImportingConstructor != null)
				foreach (var i in _partDescription.ImportingConstructor.GetParameterImportDefinitions(_assemblyContainer))
					imports.Add(new ProviderParameterImportDefinition(i.Item2, i.Item1, System.ComponentModel.Composition.CreationPolicy.Any));
			return imports.ToArray();
		}
	}
}
