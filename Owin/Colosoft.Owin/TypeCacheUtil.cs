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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Utilitário para o cache de tipos.
	/// </summary>
	static class TypeCacheUtil
	{
		/// <summary>
		/// Aplica o filtro sobre os assemblies do BuildManager.
		/// </summary>
		/// <param name="buildManager"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		private static IEnumerable<Type> FilterTypesInAssemblies(IBuildManager buildManager, Predicate<Type> predicate)
		{
			IEnumerable<Type> typesSoFar = Type.EmptyTypes;
			var assemblies = buildManager.GetReferencedAssemblies();
			foreach (System.Reflection.Assembly assembly in assemblies)
			{
				Type[] typesInAsm;
				try
				{
					typesInAsm = assembly.GetTypes();
				}
				catch(System.Reflection.ReflectionTypeLoadException ex)
				{
					typesInAsm = ex.Types;
				}
				typesSoFar = typesSoFar.Concat(typesInAsm);
			}
			return typesSoFar.Where(type => TypeIsPublicClass(type) && predicate(type));
		}

		/// <summary>
		/// Recupera os tipos filtrados dos assemblies.
		/// </summary>
		/// <param name="cacheName"></param>
		/// <param name="predicate"></param>
		/// <param name="buildManager"></param>
		/// <returns></returns>
		public static List<Type> GetFilteredTypesFromAssemblies(string cacheName, Predicate<Type> predicate, IBuildManager buildManager)
		{
			var serializer = new TypeCacheSerializer();
			List<Type> matchingTypes = ReadTypesFromCache(cacheName, predicate, buildManager, serializer);
			if(matchingTypes != null)
			{
				return matchingTypes;
			}
			matchingTypes = FilterTypesInAssemblies(buildManager, predicate).ToList();
			SaveTypesToCache(cacheName, matchingTypes, buildManager, serializer);
			return matchingTypes;
		}

		/// <summary>
		/// Lê os tipos do cache.
		/// </summary>
		/// <param name="cacheName"></param>
		/// <param name="predicate"></param>
		/// <param name="buildManager"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Cache failures are not fatal, and the code should continue executing normally.")]
		internal static List<Type> ReadTypesFromCache(string cacheName, Predicate<Type> predicate, IBuildManager buildManager, TypeCacheSerializer serializer)
		{
			try
			{
				var stream = buildManager.ReadCachedFile(cacheName);
				if(stream != null)
				{
					using (var reader = new System.IO.StreamReader(stream))
					{
						List<Type> deserializedTypes = serializer.DeserializeTypes(reader);
						if(deserializedTypes != null && deserializedTypes.All(type => TypeIsPublicClass(type) && predicate(type)))
						{
							return deserializedTypes;
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Salva os tipos para o cache.
		/// </summary>
		/// <param name="cacheName"></param>
		/// <param name="matchingTypes"></param>
		/// <param name="buildManager"></param>
		/// <param name="serializer"></param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Cache failures are not fatal, and the code should continue executing normally.")]
		internal static void SaveTypesToCache(string cacheName, IList<Type> matchingTypes, IBuildManager buildManager, TypeCacheSerializer serializer)
		{
			try
			{
				var stream = buildManager.CreateCachedFile(cacheName);
				if(stream != null)
				{
					using (var writer = new System.IO.StreamWriter(stream))
					{
						serializer.SerializeTypes(matchingTypes, writer);
					}
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Verifica se o tipo é uma classe pública.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool TypeIsPublicClass(Type type)
		{
			return (type != null && type.IsPublic && type.IsClass && !type.IsAbstract);
		}
	}
}
