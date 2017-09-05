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

namespace Colosoft.Data.Caching.Properties
{
	using System;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources
	{
		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources()
		{
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if(object.ReferenceEquals(resourceMan, null))
				{
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Data.Caching.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao fazer o download dos dados do cache..
		/// </summary>
		internal static string CacheLoader_FailOnDownloadDataEntries
		{
			get
			{
				return ResourceManager.GetString("CacheLoader_FailOnDownloadDataEntries", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Carregando os dados &apos;{0}&apos;....
		/// </summary>
		internal static string CacheLoader_LoadingTypeName
		{
			get
			{
				return ResourceManager.GetString("CacheLoader_LoadingTypeName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The class specified does not implement ICacheLoader.
		/// </summary>
		internal static string ConfigurationException_ICacheLoaderNotImplemented
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_ICacheLoaderNotImplemented", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to A execução do build &apos;{0}&apos; do cache não existe....
		/// </summary>
		internal static string DataCacheBuildMonitor_BuildExecutionNotExits
		{
			get
			{
				return ResourceManager.GetString("DataCacheBuildMonitor_BuildExecutionNotExits", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Escuta expirada....
		/// </summary>
		internal static string DataCacheBuildMonitor_ListeningExpired
		{
			get
			{
				return ResourceManager.GetString("DataCacheBuildMonitor_ListeningExpired", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O serviço de construção do cache está offline..
		/// </summary>
		internal static string DataCacheBuildOfflineException_Message
		{
			get
			{
				return ResourceManager.GetString("DataCacheBuildOfflineException_Message", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um problema para criar a consulta dos dados de &apos;{0}&apos;..
		/// </summary>
		internal static string DataCacheLoader_CreateQueryError
		{
			get
			{
				return ResourceManager.GetString("DataCacheLoader_CreateQueryError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao carregados do dados da entidade &apos;{0}&apos;..
		/// </summary>
		internal static string DataCacheLoader_GetRecordError
		{
			get
			{
				return ResourceManager.GetString("DataCacheLoader_GetRecordError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível realizar a limpezas dos items de &apos;{0}&apos;..
		/// </summary>
		internal static string DataCacheManager_Reload_ClearItems
		{
			get
			{
				return ResourceManager.GetString("DataCacheManager_Reload_ClearItems", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível recuperar os itens de &apos;{0}&apos; para serem removidos..
		/// </summary>
		internal static string DataCacheManager_Reload_QueryItemsToDelete
		{
			get
			{
				return ResourceManager.GetString("DataCacheManager_Reload_QueryItemsToDelete", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O tipo &apos;{0}&apos; não foi encontrado..
		/// </summary>
		internal static string DataCacheManager_Reload_TypeNotFound
		{
			get
			{
				return ResourceManager.GetString("DataCacheManager_Reload_TypeNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when get property type &apos;{0}&apos; from property &apos;{1}&apos; of entity &apos;{2}&apos;..
		/// </summary>
		internal static string DynamicPersistenceExecuter_GetPropertyTypeFromPropertyMetadataError
		{
			get
			{
				return ResourceManager.GetString("DynamicPersistenceExecuter_GetPropertyTypeFromPropertyMetadataError", resourceCulture);
			}
		}
	}
}
