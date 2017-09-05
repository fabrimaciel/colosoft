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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Cache dos tpos de controllers.
	/// </summary>
	sealed class ControllerTypeCache
	{
		private volatile Dictionary<string, ILookup<string, Type>> _cache;

		private object _lockObj = new object();

		/// <summary>
		/// Quantidade de entradas no cache.
		/// </summary>
		internal int Count
		{
			get
			{
				int count = 0;
				foreach (var lookup in _cache.Values)
				{
					foreach (var grouping in lookup)
					{
						count += grouping.Count();
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Assegura a inicialização.
		/// </summary>
		/// <param name="buildManager"></param>
		public void EnsureInitialized(IBuildManager buildManager)
		{
			if(_cache == null)
			{
				lock (_lockObj)
				{
					if(_cache == null)
					{
						List<Type> controllerTypes = TypeCacheUtil.GetFilteredTypesFromAssemblies("MVC-ControllerTypeCache.xml", IsControllerType, buildManager);
						var groupedByName = controllerTypes.GroupBy(t => t.Name.Substring(0, t.Name.Length - "Controller".Length), StringComparer.OrdinalIgnoreCase);
						_cache = groupedByName.ToDictionary(g => g.Key, g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
					}
				}
			}
		}

		/// <summary>
		/// Recupera os tipos de controllers pelo nome e namespace informados.
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="namespaces"></param>
		/// <returns></returns>
		public ICollection<Type> GetControllerTypes(string controllerName, HashSet<string> namespaces)
		{
			HashSet<Type> matchingTypes = new HashSet<Type>();
			ILookup<string, Type> namespaceLookup;
			if(_cache.TryGetValue(controllerName, out namespaceLookup))
			{
				if(namespaces != null)
				{
					foreach (string requestedNamespace in namespaces)
					{
						foreach (var targetNamespaceGrouping in namespaceLookup)
						{
							if(IsNamespaceMatch(requestedNamespace, targetNamespaceGrouping.Key))
							{
								matchingTypes.UnionWith(targetNamespaceGrouping);
							}
						}
					}
				}
				else
				{
					foreach (var namespaceGroup in namespaceLookup)
					{
						matchingTypes.UnionWith(namespaceGroup);
					}
				}
			}
			return matchingTypes;
		}

		/// <summary>
		/// Verifica se o tipo informado é um controller.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		internal static bool IsControllerType(Type t)
		{
			return t != null && t.IsPublic && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) && !t.IsAbstract && typeof(System.Web.Mvc.IController).IsAssignableFrom(t);
		}

		/// <summary>
		/// Verifica se o namespace é compatível,
		/// </summary>
		/// <param name="requestedNamespace"></param>
		/// <param name="targetNamespace"></param>
		/// <returns></returns>
		internal static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
		{
			if(requestedNamespace == null)
			{
				return false;
			}
			else if(requestedNamespace.Length == 0)
			{
				return true;
			}
			if(!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
			{
				return String.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
				if(!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				if(requestedNamespace.Length == targetNamespace.Length)
				{
					return true;
				}
				else if(targetNamespace[requestedNamespace.Length] == '.')
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
