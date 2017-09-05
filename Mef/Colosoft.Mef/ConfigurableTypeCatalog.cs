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
using System.Configuration;
using System.ComponentModel.Composition.Hosting;

namespace Colosoft.Mef
{
	/// <summary>
	/// Essa classe representa a configuração de typos de dados anexados ao catalogo
	/// </summary>
	public class ConfigurableTypeCatalog : TypeCatalog
	{
		/// <summary>
		/// Contsrutor padrão
		/// </summary>
		public ConfigurableTypeCatalog() : base(GetTypes())
		{
		}

		/// <summary>
		/// Contrutor lendo de outra configuração
		/// </summary>
		/// <param name="sectionName"></param>
		public ConfigurableTypeCatalog(string sectionName) : base(GetTypes(sectionName))
		{
		}

		/// <summary>
		/// Recupera os tipos configurados.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<Type> GetTypes()
		{
			return GetTypes("mef.configurableTypes");
		}

		/// <summary>
		/// Recupera os tipos da seção de configuração.
		/// </summary>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		private static IEnumerable<Type> GetTypes(string sectionName)
		{
			var config = GetSection(sectionName);
			IList<Type> types = new List<Type>();
			foreach (ConfigurableTypeElement p in config.Parts)
			{
				types.Add(Type.GetType(p.TypeExport));
			}
			return types;
		}

		/// <summary>
		/// Recupera a seção de configuração.
		/// </summary>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		private static ConfigurableTypeSection GetSection(string sectionName)
		{
			var config = ConfigurationManager.GetSection(sectionName) as ConfigurableTypeSection;
			if(config == null)
				throw new ConfigurationErrorsException(string.Format("The configuration section {0} could not be found.", sectionName));
			return config;
		}
	}
}
