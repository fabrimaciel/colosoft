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
using Colosoft.Logging;
using Colosoft.Caching.Exceptions;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Classe responsável pela criação de armazenamentos para o cache.
	/// </summary>
	public class CacheStorageFactory
	{
		/// <summary>
		/// Cria um provedor de armazenamenteo.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="cacheContext"></param>
		/// <param name="evictionEnabled"></param>
		/// <param name="logger"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static ICacheStorage CreateStorageProvider(IDictionary properties, string cacheContext, bool evictionEnabled, ILogger logger)
		{
			properties.Require("properties").NotNull();
			StorageProviderBase base2 = null;
			try
			{
				if(!properties.Contains("class"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingCacheStorageClass).Format());
				string str = Convert.ToString(properties["class"]).ToLower();
				IDictionary dictionary = (IDictionary)properties[str];
				if(str.CompareTo("heap") == 0)
					base2 = new ClrHeapStorageProvider(dictionary, evictionEnabled, logger);
				else if(str.CompareTo("memory") == 0)
					base2 = new InMemoryStorageProvider(dictionary, evictionEnabled);
				else if(str.CompareTo("memory-mapped") == 0)
					base2 = new MmfStorageProvider(dictionary, evictionEnabled);
				else
				{
					if(str.CompareTo("file") != 0)
						throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_InvalidCacheStorageClass, str).Format());
					base2 = new FileSystemStorageProvider(dictionary, evictionEnabled);
				}
				if(base2 != null)
					base2.CacheContext = cacheContext;
			}
			catch(ConfigurationException exception)
			{
				Trace.Error("CacheStorageFactory.CreateCacheStore()".GetFormatter(), exception.GetFormatter());
				throw;
			}
			catch(Exception exception2)
			{
				Trace.Error("CacheStorageFactory.CreateCacheStore()".GetFormatter(), exception2.GetFormatter());
				throw new ConfigurationException("Configuration Error: " + exception2.ToString(), exception2);
			}
			return base2;
		}
	}
}
