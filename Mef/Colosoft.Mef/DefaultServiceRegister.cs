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
using System.ComponentModel.Composition.Hosting;

namespace Colosoft.Mef
{
	/// <summary>
	/// Classe que auxilia no registro padrão do serviço.
	/// </summary>
	public static class DefaultServiceRegister
	{
		/// <summary>
		/// Recupera o catalogo padrão.
		/// </summary>
		/// <returns></returns>
		private static ComposablePartCatalog GetDefaultComposablePartCatalog()
		{
			return new AssemblyCatalog(System.Reflection.Assembly.GetAssembly(typeof(MefBootstrapper)));
		}

		/// <summary>
		/// Recupera as partes requeridas para o registro.
		/// </summary>
		/// <param name="aggregateCatalog"></param>
		/// <returns></returns>
		private static IEnumerable<ComposablePartDefinition> GetRequiredPartsToRegister(AggregateCatalog aggregateCatalog)
		{
			var list = new List<System.ComponentModel.Composition.Primitives.ComposablePartDefinition>();
			var defaultComposablePartCatalog = GetDefaultComposablePartCatalog();
			foreach (ComposablePartDefinition definition in defaultComposablePartCatalog.Parts)
			{
				foreach (ExportDefinition definition2 in definition.ExportDefinitions)
				{
					bool flag = false;
					foreach (ComposablePartDefinition definition3 in aggregateCatalog.Parts)
					{
						foreach (ExportDefinition definition4 in definition3.ExportDefinitions)
						{
							if(string.Compare(definition4.ContractName, definition2.ContractName, StringComparison.Ordinal) == 0)
							{
								flag = true;
								break;
							}
						}
					}
					if(!flag && !list.Contains(definition))
					{
						list.Add(definition);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Registra os requisitos.
		/// </summary>
		/// <param name="aggregateCatalog"></param>
		/// <returns></returns>
		public static System.ComponentModel.Composition.Hosting.AggregateCatalog RegisterRequiredServicesIfMissing(System.ComponentModel.Composition.Hosting.AggregateCatalog aggregateCatalog)
		{
			aggregateCatalog.Require("aggregateCatalog").NotNull();
			var item = new DefaultCatalog(GetRequiredPartsToRegister(aggregateCatalog));
			aggregateCatalog.Catalogs.Add(item);
			return aggregateCatalog;
		}
	}
}
