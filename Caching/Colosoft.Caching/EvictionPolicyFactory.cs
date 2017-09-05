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
using System.Threading;
using System.Globalization;

namespace Colosoft.Caching.Policies
{
	internal class EvictionPolicyFactory
	{
		/// <summary>
		/// Cria a politica de liberação padrão.
		/// </summary>
		/// <returns></returns>
		public static IEvictionPolicy CreateDefaultEvictionPolicy()
		{
			return null;
		}

		/// <summary>
		/// Cria uma política de liberação com base nas propriedades informadas.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static IEvictionPolicy CreateEvictionPolicy(IDictionary properties)
		{
			properties.Require("properties").NotNull();
			IEvictionPolicy policy2;
			try
			{
				float ratio = 0f;
				if(properties.Contains("evict-ratio"))
				{
					CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
					ratio = Convert.ToSingle(properties["evict-ratio"]);
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
				IEvictionPolicy policy = null;
				string str = Convert.ToString(properties["class"]).ToLower();
				IDictionary dictionary = (IDictionary)properties[str];
				string str2 = str;
				if(str2 != null)
				{
					if(!(str2 == "lru"))
					{
						if(str2 == "lfu")
							policy = new LFUEvictionPolicy(dictionary, ratio);
						if(str2 == "priority")
							policy = new PriorityEvictionPolicy(dictionary, ratio);
					}
					else
						policy = new LRUEvictionPolicy(dictionary, ratio);
				}
				if(policy == null)
					throw new Colosoft.Caching.Exceptions.ConfigurationException("Invalid Eviction Policy: " + str);
				policy2 = policy;
			}
			catch(Colosoft.Caching.Exceptions.ConfigurationException)
			{
				throw;
			}
			catch(Exception exception)
			{
				throw new Colosoft.Caching.Exceptions.ConfigurationException("EvictionPolicyFactory.CreateEvictionPolicy(): " + exception.ToString());
			}
			return policy2;
		}
	}
}
