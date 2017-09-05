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
using System.Collections;
using Colosoft.Caching.Configuration;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe responsável pela criação de uma instancia do cache.
	/// </summary>
	public class CacheFactory : MarshalByRefObject
	{
		/// <summary>
		/// Cria uma instancia do cache com base nas propriedade informadas.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private static Cache CreateFromProperties(IDictionary properties)
		{
			Cache cache = new Cache();
			cache.Initialize(properties, true);
			return cache;
		}

		/// <summary>
		/// Cria o cache com base em um texto com as as configurações das propriedades.
		/// </summary>
		/// <param name="propertyString">Texto contendo as propriedades de configuração.</param>
		/// <returns></returns>
		public static Cache CreateFromPropertyString(string propertyString)
		{
			ConfigReader reader = new PropsConfigReader(propertyString);
			return CreateFromProperties(reader.Properties);
		}

		/// <summary>
		/// Cria o cache com base em um texto com as as configurações das propriedades.
		/// </summary>
		/// <param name="propertyString"></param>
		/// <param name="config"></param>
		/// <param name="twoPhaseInitialization"></param>
		/// <returns></returns>
		public static Cache CreateFromPropertyString(string propertyString, Configuration.Dom.CacheConfig config, bool twoPhaseInitialization)
		{
			ConfigReader reader = new PropsConfigReader(propertyString);
			return CreateFromProperties(reader.Properties, config, twoPhaseInitialization);
		}

		/// <summary>
		/// Cria o cache a partir das propriedades informadas.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="config"></param>
		/// <param name="twoPhaseInitialization"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Cache CreateFromProperties(IDictionary properties, Configuration.Dom.CacheConfig config, bool twoPhaseInitialization)
		{
			Cache cache = new Cache();
			cache.Configuration = config;
			cache.Initialize(properties, true, twoPhaseInitialization);
			return cache;
		}
	}
}
