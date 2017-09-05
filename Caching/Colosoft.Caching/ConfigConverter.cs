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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Classe responsável pela conversão da configuração.
	/// </summary>
	public static class ConfigConverter
	{
		/// <summary>
		/// Converter os dados contidos no hashtable para instancia de configuração.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static CacheConfig[] ToDom(Hashtable config)
		{
			return HashtableToDom.GetConfig(config);
		}

		/// <summary>
		/// Converter as configurações em um Hashtable.
		/// </summary>
		/// <param name="configs"></param>
		/// <returns></returns>
		public static Hashtable ToHashtable(CacheConfig[] configs)
		{
			return DomToHashtable.GetConfig(configs);
		}

		/// <summary>
		/// Converter a configuração em um hashtable.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static Hashtable ToHashtable(CacheConfig config)
		{
			return DomToHashtable.GetConfig(config);
		}

		/// <summary>
		/// Classe que realiza a conversão dos dados Dom para Hashtable.
		/// </summary>
		private static class DomToHashtable
		{
			/// <summary>
			/// Recupera os dados da origem de fundo.
			/// </summary>
			/// <param name="backingSource"></param>
			/// <returns></returns>
			private static Hashtable GetBackingSource(BackingSource backingSource)
			{
				Hashtable hashtable = new Hashtable();
				if(backingSource.Readthru != null)
					hashtable.Add("read-thru", GetReadThru(backingSource.Readthru));
				if(backingSource.Writethru != null)
					hashtable.Add("write-thru", GetWriteThru(backingSource.Writethru));
				return hashtable;
			}

			/// <summary>
			/// Recuper as configurações do cache.
			/// </summary>
			/// <param name="cache"></param>
			/// <returns></returns>
			public static Hashtable GetCache(CacheConfig cache)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", cache.Name);
				if(cache.Log != null)
					hashtable.Add("log", GetLog(cache.Log));
				hashtable.Add("config-id", cache.ConfigID);
				if(cache.LastModified != null)
					hashtable.Add("last-modified", cache.LastModified);
				hashtable.Add("cache-classes", GetCacheClasses(cache));
				hashtable.Add("class", cache.Name);
				if(cache.PerfCounters != null)
					hashtable.Add("perf-counters", cache.PerfCounters.Enabled);
				if(cache.Compression != null)
					hashtable.Add("compression", GetCompression(cache.Compression));
				if(cache.BackingSource != null)
					hashtable.Add("backing-source", GetBackingSource(cache.BackingSource));
				if(cache.CacheLoader != null)
					hashtable.Add("cache-loader", GetCacheLoader(cache.CacheLoader));
				return hashtable;
			}

			/// <summary>
			/// Recupera as classes do cache.
			/// </summary>
			/// <param name="cache"></param>
			/// <returns></returns>
			private static Hashtable GetCacheClasses(CacheConfig cache)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add(cache.Name, GetClassifiedCache(cache));
				return hashtable;
			}

			/// <summary>
			/// Recupera a configuração do cache.
			/// </summary>
			/// <param name="cache"></param>
			/// <returns></returns>
			public static Hashtable GetCacheConfiguration(CacheConfig cache)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("cache", GetCache(cache));
				return hashtable;
			}

			/// <summary>
			/// Recupera as configurações do cache loader.
			/// </summary>
			/// <param name="CL"></param>
			/// <returns></returns>
			private static Hashtable GetCacheLoader(CacheLoader CL)
			{
				Hashtable hashtable = new Hashtable();
				if(CL.Provider != null)
				{
					hashtable.Add("assembly", CL.Provider.AssemblyName);
					hashtable.Add("classname", CL.Provider.ClassName);
					hashtable.Add("full-name", CL.Provider.FullProviderName);
					hashtable.Add("retries", CL.Retries);
					hashtable.Add("retry-interval", CL.RetryInterval);
					hashtable.Add("enabled", CL.Enabled);
					if(CL.Parameters != null)
						hashtable.Add("parameters", GetParameters(CL.Parameters));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera o cache classificado.
			/// </summary>
			/// <param name="cache"></param>
			/// <returns></returns>
			private static Hashtable GetClassifiedCache(CacheConfig cache)
			{
				Hashtable source = new Hashtable();
				source.Add("id", cache.Name);
				source.Add("type", "local-cache");
				GetInternalCache(source, cache, true);
				if(cache.Notifications != null)
				{
					source.Add("notifications", GetNotifications(cache.Notifications));
				}
				return source;
			}

			/// <summary>
			/// Recupera a lista dos atributos.
			/// </summary>
			/// <param name="attributeList"></param>
			/// <returns></returns>
			private static Hashtable GetCompactAttributeListUnion(AttributeListUnion attributeList)
			{
				Hashtable hashtable = new Hashtable();
				if((attributeList != null) && (attributeList.PortableAttributes != null))
				{
					hashtable.Add("attribute", GetCompactPortableAttributes(attributeList.PortableAttributes));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados de uma classe compacta.
			/// </summary>
			/// <param name="cls"></param>
			/// <returns></returns>
			private static Hashtable GetCompactClass(CompactClass cls)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("id", cls.ID);
				hashtable.Add("name", cls.Name);
				hashtable.Add("assembly", cls.Assembly);
				hashtable.Add("portable", cls.Portable);
				hashtable.Add("type", cls.Type);
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados de um atributo portável.
			/// </summary>
			/// <param name="attrib"></param>
			/// <returns></returns>
			private static Hashtable GetCompactPortableAttribute(PortableAttribute attrib)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", attrib.Name);
				hashtable.Add("type", attrib.Type);
				hashtable.Add("order", attrib.Order);
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados dos atributos portáveis.
			/// </summary>
			/// <param name="attributes"></param>
			/// <returns></returns>
			private static Hashtable GetCompactPortableAttributes(PortableAttribute[] attributes)
			{
				Hashtable hashtable = new Hashtable();
				foreach (PortableAttribute attribute in attributes)
					hashtable.Add(attribute.Name + ":" + attribute.Type, GetCompactPortableAttribute(attribute));
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados de uma classe portável.
			/// </summary>
			/// <param name="clas"></param>
			/// <returns></returns>
			private static Hashtable GetCompactPortableClass(PortableClass clas)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", clas.Name);
				if(clas.ID != null)
					hashtable.Add("handle-id", clas.ID);
				hashtable.Add("assembly", clas.Assembly);
				hashtable.Add("type", clas.Type);
				if(clas.PortableAttributes != null)
					hashtable.Add("attribute", GetCompactPortableAttributes(clas.PortableAttributes));
				return hashtable;
			}

			private static Hashtable GetCompactPortableClasses(PortableClass[] classes)
			{
				Hashtable hashtable = new Hashtable();
				foreach (PortableClass class2 in classes)
				{
					hashtable.Add(class2.Name, GetCompactPortableClass(class2));
				}
				return hashtable;
			}

			private static Hashtable GetCompactSerialization(CompactSerialization compactSerialization)
			{
				Hashtable hashtable = new Hashtable();
				foreach (CompactClass class2 in compactSerialization.CompactClasses)
				{
					hashtable.Add(class2.ID, GetCompactClass(class2));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados de um tipo compacto.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			private static Hashtable GetCompactType(Type type)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", type.Name);
				hashtable.Add("portable", type.Portable);
				hashtable.Add("id", type.ID);
				if(type.PortableClasses != null)
				{
					hashtable.Add("known-classes", GetCompactPortableClasses(type.PortableClasses));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados de compressão.
			/// </summary>
			/// <param name="compression"></param>
			/// <returns></returns>
			private static Hashtable GetCompression(Compression compression)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("threshold", compression.Threshold.ToString());
				hashtable.Add("enabled", compression.Enabled.ToString().ToLower());
				return hashtable;
			}

			/// <summary>
			/// Recupera as configurações dos caches informados.
			/// </summary>
			/// <param name="caches"></param>
			/// <returns></returns>
			public static Hashtable GetConfig(CacheConfig[] caches)
			{
				Hashtable hashtable = new Hashtable();
				foreach (var config in caches)
				{
					hashtable.Add(config.Name, GetCacheConfiguration(config));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera a configuração do cache.
			/// </summary>
			/// <param name="cache"></param>
			/// <returns></returns>
			public static Hashtable GetConfig(CacheConfig cache)
			{
				return GetCacheConfiguration(cache);
			}

			/// <summary>
			/// Recupera os dados do compartilhamento de dados.
			/// </summary>
			/// <param name="dataSharing"></param>
			/// <returns></returns>
			private static Hashtable GetDataSharing(DataSharing dataSharing)
			{
				Hashtable hashtable = new Hashtable();
				foreach (Type type in dataSharing.Types)
				{
					hashtable.Add(type.ID, GetDataSharingCompactType(type));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera um tipo compacto para o compartilhamento de dados.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			private static Hashtable GetDataSharingCompactType(Type type)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", type.Name);
				hashtable.Add("portable", type.Portable);
				hashtable.Add("id", type.ID);
				if(type.PortableClasses != null)
				{
					hashtable.Add("known-classes", GetDataSharingPortableClasses(type.PortableClasses));
					hashtable.Add("attribute-union-list", GetCompactAttributeListUnion(type.AttributeList));
				}
				return hashtable;
			}

			private static Hashtable GetDataSharingPortableClass(PortableClass clas)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", clas.Name);
				if(clas.ID != null)
					hashtable.Add("handle-id", clas.ID);
				hashtable.Add("assembly", clas.Assembly);
				hashtable.Add("type", clas.Type);
				if(clas.PortableAttributes != null)
					hashtable.Add("attribute", GetCompactPortableAttributes(clas.PortableAttributes));
				return hashtable;
			}

			private static Hashtable GetDataSharingPortableClasses(PortableClass[] classes)
			{
				Hashtable hashtable = new Hashtable();
				foreach (PortableClass class2 in classes)
				{
					hashtable.Add(class2.Name, GetDataSharingPortableClass(class2));
				}
				return hashtable;
			}

			private static Hashtable GetEvictionPolicy(EvictionPolicy evictionPolicy)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("class", evictionPolicy.Policy);
				hashtable.Add("eviction-enabled", evictionPolicy.Enabled);
				hashtable.Add("priority", GetEvictionPriority(evictionPolicy));
				hashtable.Add("evict-ratio", evictionPolicy.EvictionRatio.ToString());
				return hashtable;
			}

			private static Hashtable GetEvictionPriority(EvictionPolicy evictionPolicy)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("default-value", evictionPolicy.DefaultPriority);
				return hashtable;
			}

			private static Hashtable GetHeap(Storage storage)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("max-size", storage.Size.ToString());
				return hashtable;
			}

			private static Hashtable GetIndexAttribute(Attrib attrib)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", attrib.Name);
				hashtable.Add("data-type", attrib.Type);
				hashtable.Add("type", "attrib");
				hashtable.Add("id", attrib.ID);
				return hashtable;
			}

			private static Hashtable GetIndexAttributes(Attrib[] attributes)
			{
				Hashtable hashtable = new Hashtable();
				foreach (Attrib attrib in attributes)
					hashtable.Add(attrib.ID, GetIndexAttribute(attrib));
				return hashtable;
			}

			private static Hashtable GetIndexClass(Class cls)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("name", cls.Name);
				hashtable.Add("type", "class");
				hashtable.Add("id", cls.ID);
				if(cls.Attributes != null)
				{
					hashtable.Add("attributes", GetIndexAttributes(cls.Attributes));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados das classes dos indice.
			/// </summary>
			/// <param name="classes"></param>
			/// <returns></returns>
			private static Hashtable GetIndexClasses(Class[] classes)
			{
				Hashtable hashtable = new Hashtable();
				foreach (Class class2 in classes)
				{
					hashtable.Add(class2.ID, GetIndexClass(class2));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados dos indices.
			/// </summary>
			/// <param name="indexes"></param>
			/// <returns></returns>
			private static Hashtable GetIndexes(QueryIndex indexes)
			{
				Hashtable hashtable = new Hashtable();
				if(indexes.Classes != null)
				{
					hashtable.Add("index-classes", GetIndexClasses(indexes.Classes));
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados do cache interno.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="cache"></param>
			/// <param name="localCache"></param>
			private static void GetInternalCache(Hashtable source, CacheConfig cache, bool localCache)
			{
				if(cache.QueryIndices != null)
					source.Add("indexes", GetIndexes(cache.QueryIndices));
				if(cache.Storage != null)
					source.Add("storage", GetStorage(cache.Storage));
				if(!localCache)
				{
					source.Add("type", "local-cache");
					source.Add("id", "internal-cache");
				}
				if(cache.EvictionPolicy != null)
					source.Add("scavenging-policy", GetEvictionPolicy(cache.EvictionPolicy));
				if(cache.Cleanup != null)
					source.Add("clean-interval", cache.Cleanup.Interval.ToString());
			}

			/// <summary>
			/// Recupera os dados do log.
			/// </summary>
			/// <param name="log"></param>
			/// <returns></returns>
			private static Hashtable GetLog(Log log)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("enabled", log.Enabled.ToString().ToLower());
				hashtable.Add("trace-errors", log.TraceErrors.ToString().ToLower());
				hashtable.Add("trace-notices", log.TraceNotices.ToString().ToLower());
				hashtable.Add("trace-debug", log.TraceDebug.ToString().ToLower());
				hashtable.Add("trace-warnings", log.TraceWarnings.ToString().ToLower());
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados da notificações.
			/// </summary>
			/// <param name="notifications"></param>
			/// <returns></returns>
			private static Hashtable GetNotifications(Notifications notifications)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("item-add", notifications.ItemAdd.ToString().ToLower());
				hashtable.Add("item-remove", notifications.ItemRemove.ToString().ToLower());
				hashtable.Add("item-update", notifications.ItemUpdate.ToString().ToLower());
				hashtable.Add("cache-clear", notifications.CacheClear.ToString().ToLower());
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados dos parametros.
			/// </summary>
			/// <param name="parameters"></param>
			/// <returns></returns>
			private static Hashtable GetParameters(Parameter[] parameters)
			{
				if(parameters == null)
					return null;
				Hashtable hashtable = new Hashtable();
				for(int i = 0; i < parameters.Length; i++)
					hashtable[parameters[i].Name] = parameters[i].ParamValue;
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados do provedor.
			/// </summary>
			/// <param name="provider"></param>
			/// <returns></returns>
			private static Hashtable GetProvider(Provider provider)
			{
				Hashtable hashtable = new Hashtable();
				if(provider != null)
				{
					hashtable["provider-name"] = provider.ProviderName;
					hashtable["assembly-name"] = provider.AssemblyName;
					hashtable["class-name"] = provider.ClassName;
					hashtable["full-name"] = provider.FullProviderName;
					hashtable["default-provider"] = provider.IsDefaultProvider.ToString();
					hashtable["async-mode"] = provider.AsyncMode.ToString();
					Hashtable parameters = GetParameters(provider.Parameters);
					if(parameters != null)
					{
						hashtable["parameters"] = parameters;
					}
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados dos provedores.
			/// </summary>
			/// <param name="providers"></param>
			/// <returns></returns>
			private static Hashtable GetProviders(Provider[] providers)
			{
				Hashtable hashtable = new Hashtable();
				if((providers != null) && (providers.Length > 0))
				{
					for(int i = 0; i < providers.Length; i++)
						hashtable[providers[i].ProviderName] = GetProvider(providers[i]);
				}
				return hashtable;
			}

			/// <summary>
			/// Recupera os dados dos leitor.
			/// </summary>
			/// <param name="readthru"></param>
			/// <returns></returns>
			private static Hashtable GetReadThru(Readthru readthru)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["enabled"] = readthru.Enabled.ToString();
				if(readthru.Providers != null)
					hashtable.Add("read-thru-providers", GetProviders(readthru.Providers));
				return hashtable;
			}

			private static Hashtable GetRecipients(NotificationRecipient[] recipients)
			{
				Hashtable hashtable = new Hashtable();
				for(int i = 0; i < recipients.Length; i++)
					hashtable.Add(recipients[i].ID, recipients[i].ID);
				return hashtable;
			}

			private static Hashtable GetStorage(Storage storage)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("class", storage.Type);
				if(storage.Type == "heap")
				{
					hashtable.Add("heap", GetHeap(storage));
				}
				return hashtable;
			}

			public static Hashtable GetWebCache(CacheConfig cache)
			{
				Hashtable hashtable = new Hashtable();
				bool flag = !cache.InProc;
				hashtable.Add("shared", flag.ToString().ToLower());
				hashtable.Add("cache-id", cache.Name);
				return hashtable;
			}

			private static Hashtable GetWriteThru(Writethru writethru)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["enabled"] = writethru.Enabled.ToString();
				if(writethru.Providers != null)
					hashtable.Add("write-thru-providers", GetProviders(writethru.Providers));
				return hashtable;
			}
		}

		private static class HashtableToDom
		{
			private static BackingSource GetBackingSource(Hashtable settings)
			{
				BackingSource source = new BackingSource();
				if(settings.ContainsKey("read-thru"))
					source.Readthru = GetReadThru((Hashtable)settings["read-thru"]);
				if(settings.ContainsKey("write-thru"))
					source.Writethru = GetWriteThru((Hashtable)settings["write-thru"]);
				return source;
			}

			private static void GetCache(CacheConfig cache, Hashtable settings)
			{
				if(settings.ContainsKey("config-id"))
					cache.ConfigID = Convert.ToDouble(settings["config-id"]);
				if(settings.ContainsKey("last-modified"))
					cache.LastModified = settings["last-modified"].ToString();
				if(settings.ContainsKey("backing-source"))
					cache.BackingSource = GetBackingSource((Hashtable)settings["backing-source"]);
				if(settings.ContainsKey("cache-loader"))
					cache.CacheLoader = GetCacheLoader((Hashtable)settings["cache-loader"]);
				if(settings.ContainsKey("compression"))
					cache.Compression = GetCompression((Hashtable)settings["compression"]);
				if(settings.ContainsKey("log"))
					cache.Log = GetLog((Hashtable)settings["log"]);
				if(settings.ContainsKey("cache-classes"))
					GetCacheClasses(cache, (Hashtable)settings["cache-classes"]);
				if(settings.ContainsKey("perf-counters"))
					cache.PerfCounters = GetPerfCounters(settings);
			}

			private static void GetCacheClasses(CacheConfig cache, Hashtable settings)
			{
				if(settings.ContainsKey(cache.Name))
				{
					GetClassifiedCache(cache, (Hashtable)settings[cache.Name]);
				}
			}

			private static CacheConfig GetCacheConfiguration(Hashtable settings)
			{
				var cache = new CacheConfig();
				cache.Name = settings["id"].ToString();
				if(settings.ContainsKey("web-cache"))
					GetWebCache(cache, (Hashtable)settings["web-cache"]);
				if(settings.ContainsKey("cache"))
					GetCache(cache, (Hashtable)settings["cache"]);
				if(settings.ContainsKey("data-sharing"))
					cache.DataSharing = GetDataSharing((Hashtable)settings["data-sharing"]);
				if(settings.ContainsKey("compact-serialization"))
					cache.CompactSerialization = GetCompactSerialization((Hashtable)settings["compact-serialization"]);
				return cache;
			}

			private static CacheLoader GetCacheLoader(Hashtable settings)
			{
				CacheLoader loader = new CacheLoader();
				loader.Provider = new ProviderAssembly();
				if(settings.ContainsKey("cache-load"))
				{
					Hashtable hashtable = (Hashtable)settings["cache-load"];
					loader.Provider.AssemblyName = hashtable["assembly"].ToString();
					loader.Provider.ClassName = hashtable["classname"].ToString();
					loader.Provider.FullProviderName = settings["full-name"].ToString();
					loader.Retries = Convert.ToInt32(hashtable["retries"]);
					loader.RetryInterval = Convert.ToInt32(hashtable["retry-interval"]);
					if(hashtable.ContainsKey("parameters"))
						loader.Parameters = GetParameters(hashtable["parameters"] as Hashtable);
				}
				return loader;
			}

			private static void GetClassifiedCache(CacheConfig cache, Hashtable settings)
			{
				if(settings.ContainsKey("internal-cache"))
					GetInternalCache(cache, (Hashtable)settings["internal-cache"]);
				else
					GetInternalCache(cache, settings);
				if(settings.ContainsKey("notifications"))
					cache.Notifications = GetNotifications((Hashtable)settings["notifications"]);
			}

			/// <summary>
			/// Recupera os dados da configuração de limpeza.
			/// </summary>
			/// <param name="settings"></param>
			/// <returns></returns>
			private static Cleanup GetCleanup(Hashtable settings)
			{
				Cleanup cleanup = new Cleanup();
				cleanup.Interval = Convert.ToInt32(settings["clean-interval"]);
				return cleanup;
			}

			private static CompactSerialization GetCompactSerialization(Hashtable settings)
			{
				CompactSerialization serialization = new CompactSerialization();
				if(settings != null)
				{
					CompactClass[] classArray = new CompactClass[settings.Count];
					IDictionaryEnumerator enumerator = settings.GetEnumerator();
					int index = 0;
					while (enumerator.MoveNext())
					{
						Hashtable hashtable = enumerator.Value as Hashtable;
						if((hashtable != null) && (hashtable.Count > 0))
						{
							CompactClass class2 = new CompactClass();
							class2.ID = hashtable["id"] as string;
							class2.Name = hashtable["name"] as string;
							class2.Assembly = hashtable["assembly"] as string;
							class2.Type = hashtable["type"] as string;
							classArray[index] = class2;
						}
					}
					serialization.CompactClasses = classArray;
				}
				return serialization;
			}

			private static Compression GetCompression(Hashtable settings)
			{
				Compression compression = new Compression();
				if(settings.ContainsKey("enabled"))
				{
					compression.Enabled = Convert.ToBoolean(settings["enabled"]);
				}
				if(settings.ContainsKey("threshold"))
				{
					compression.Threshold = Convert.ToInt32(settings["threshold"]);
				}
				return compression;
			}

			public static CacheConfig[] GetConfig(Hashtable config)
			{
				var configArray = new CacheConfig[config.Count];
				int num = 0;
				foreach (Hashtable hashtable in config.Values)
					configArray[num++] = GetCacheConfiguration(System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable(hashtable));
				return configArray;
			}

			private static DataSharing GetDataSharing(Hashtable settings)
			{
				DataSharing sharing = new DataSharing();
				if(settings != null)
				{
					Type[] typeArray = new Type[settings.Count];
					IDictionaryEnumerator enumerator = settings.GetEnumerator();
					int index = 0;
					while (enumerator.MoveNext())
					{
						Hashtable hashtable = enumerator.Value as Hashtable;
						if((hashtable != null) && (hashtable.Count > 0))
						{
							Type type = new Type();
							type.ID = hashtable["id"] as string;
							type.Name = hashtable["name"] as string;
							type.Portable = Convert.ToBoolean(hashtable["portable"] as string);
							Hashtable hashtable2 = hashtable["class"] as Hashtable;
							if((hashtable2 != null) && (hashtable2.Count > 0))
							{
								PortableClass[] classArray = new PortableClass[hashtable2.Count];
								IDictionaryEnumerator enumerator2 = hashtable2.GetEnumerator();
								for(int i = 0; enumerator2.MoveNext(); i++)
								{
									Hashtable hashtable3 = enumerator2.Value as Hashtable;
									PortableClass class2 = new PortableClass();
									class2.Name = hashtable3["name"] as string;
									class2.ID = hashtable3["id"] as string;
									class2.Assembly = hashtable3["assembly"] as string;
									class2.Type = hashtable3["type"] as string;
									Hashtable hashtable4 = hashtable3["class"] as Hashtable;
									if((hashtable4 != null) && (hashtable4.Count > 0))
									{
										PortableAttribute[] attributeArray = new PortableAttribute[hashtable4.Count];
										IDictionaryEnumerator enumerator3 = hashtable4.GetEnumerator();
										for(int j = 0; enumerator3.MoveNext(); j++)
										{
											Hashtable hashtable5 = enumerator2.Value as Hashtable;
											PortableAttribute attribute = new PortableAttribute();
											attribute.Name = hashtable5["name"] as string;
											attribute.Type = hashtable5["type"] as string;
											attribute.Order = hashtable5["order"] as string;
											attributeArray[j] = attribute;
										}
										class2.PortableAttributes = attributeArray;
									}
									classArray[i] = class2;
								}
								type.PortableClasses = classArray;
							}
							Hashtable hashtable6 = hashtable["attribute-list"] as Hashtable;
							if((hashtable6 != null) && (hashtable6.Count > 0))
							{
								AttributeListUnion union = new AttributeListUnion();
								PortableAttribute[] attributeArray2 = new PortableAttribute[hashtable6.Count];
								IDictionaryEnumerator enumerator4 = hashtable6.GetEnumerator();
								for(int k = 0; enumerator4.MoveNext(); k++)
								{
									Hashtable hashtable7 = enumerator4.Value as Hashtable;
									PortableAttribute attribute2 = new PortableAttribute();
									attribute2.Name = hashtable7["name"] as string;
									attribute2.Type = hashtable7["type"] as string;
									attribute2.Order = hashtable7["order"] as string;
									attributeArray2[k] = attribute2;
								}
								union.PortableAttributes = attributeArray2;
								type.AttributeList = union;
							}
							typeArray[index] = type;
							index++;
						}
					}
					sharing.Types = typeArray;
				}
				return sharing;
			}

			private static EvictionPolicy GetEvictionPolicy(Hashtable settings)
			{
				EvictionPolicy policy = new EvictionPolicy();
				if(settings.ContainsKey("eviction-enabled"))
					policy.Enabled = Convert.ToBoolean(settings["eviction-enabled"]);
				if(settings.ContainsKey("priority"))
					policy.DefaultPriority = ((Hashtable)settings["priority"])["default-value"].ToString();
				if(settings.ContainsKey("class"))
					policy.Policy = settings["class"] as string;
				if(settings.ContainsKey("evict-ratio"))
					policy.EvictionRatio = Convert.ToDecimal(settings["evict-ratio"]);
				return policy;
			}

			private static Attrib GetIndexAttribute(Hashtable settings)
			{
				Attrib attrib = new Attrib();
				if(settings.ContainsKey("id"))
					attrib.ID = settings["id"].ToString();
				if(settings.ContainsKey("data-type"))
					attrib.Type = settings["data-type"].ToString();
				if(settings.ContainsKey("name"))
					attrib.Name = settings["name"].ToString();
				return attrib;
			}

			private static Attrib[] GetIndexAttributes(Hashtable settings)
			{
				Attrib[] attribArray = new Attrib[settings.Count];
				int num = 0;
				foreach (Hashtable hashtable in settings.Values)
					attribArray[num++] = GetIndexAttribute(hashtable);
				return attribArray;
			}

			private static Class GetIndexClass(Hashtable settings)
			{
				Class class2 = new Class();
				if(settings.ContainsKey("id"))
					class2.ID = settings["id"].ToString();
				if(settings.ContainsKey("name"))
					class2.Name = settings["name"].ToString();
				if(settings.ContainsKey("attributes"))
					class2.Attributes = GetIndexAttributes((Hashtable)settings["attributes"]);
				return class2;
			}

			private static Class[] GetIndexClasses(Hashtable settings)
			{
				Class[] classArray = new Class[settings.Count];
				int num = 0;
				foreach (Hashtable hashtable in settings.Values)
					classArray[num++] = GetIndexClass(hashtable);
				return classArray;
			}

			private static QueryIndex GetIndexes(Hashtable settings)
			{
				QueryIndex index = new QueryIndex();
				if(settings.ContainsKey("index-classes"))
				{
					index.Classes = GetIndexClasses((Hashtable)settings["index-classes"]);
				}
				return index;
			}

			private static void GetInternalCache(CacheConfig cache, Hashtable settings)
			{
				if(settings.ContainsKey("indexes"))
					cache.QueryIndices = GetIndexes((Hashtable)settings["indexes"]);
				if(settings.ContainsKey("storage"))
					cache.Storage = GetStorage((Hashtable)settings["storage"]);
				if(settings.ContainsKey("scavenging-policy"))
					cache.EvictionPolicy = GetEvictionPolicy((Hashtable)settings["scavenging-policy"]);
				if(settings.ContainsKey("clean-interval"))
				{
					cache.Cleanup = GetCleanup(settings);
				}
			}

			private static Log GetLog(Hashtable settings)
			{
				Log log = new Log();
				if(settings.ContainsKey("enabled"))
					log.Enabled = Convert.ToBoolean(settings["enabled"]);
				if(settings.ContainsKey("trace-errors"))
					log.TraceErrors = Convert.ToBoolean(settings["trace-errors"]);
				if(settings.ContainsKey("trace-notices"))
					log.TraceNotices = Convert.ToBoolean(settings["trace-notices"]);
				if(settings.ContainsKey("trace-debug"))
					log.TraceDebug = Convert.ToBoolean(settings["trace-debug"]);
				if(settings.ContainsKey("trace-warnings"))
					log.TraceWarnings = Convert.ToBoolean(settings["trace-warnings"]);
				return log;
			}

			private static Notifications GetNotifications(Hashtable settings)
			{
				Notifications notifications = new Notifications();
				if(settings.ContainsKey("item-remove"))
					notifications.ItemRemove = Convert.ToBoolean(settings["item-remove"]);
				if(settings.ContainsKey("item-add"))
					notifications.ItemAdd = Convert.ToBoolean(settings["item-add"]);
				if(settings.ContainsKey("item-update"))
					notifications.ItemUpdate = Convert.ToBoolean(settings["item-update"]);
				if(settings.ContainsKey("cache-clear"))
					notifications.CacheClear = Convert.ToBoolean(settings["cache-clear"]);
				return notifications;
			}

			/// <summary>
			/// Recupera os dados dos parametros.
			/// </summary>
			/// <param name="settings"></param>
			/// <returns></returns>
			private static Parameter[] GetParameters(Hashtable settings)
			{
				if(settings == null)
					return null;
				Parameter[] parameterArray = new Parameter[settings.Count];
				int index = 0;
				IDictionaryEnumerator enumerator = settings.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Parameter parameter = new Parameter();
					parameter.Name = enumerator.Key as string;
					parameter.ParamValue = enumerator.Value as string;
					parameterArray[index] = parameter;
					index++;
				}
				return parameterArray;
			}

			/// <summary>
			/// Recupera os dados do <see cref=" PerfCounters"/>.
			/// </summary>
			/// <param name="settings"></param>
			/// <returns></returns>
			private static PerfCounters GetPerfCounters(Hashtable settings)
			{
				PerfCounters counters = new PerfCounters();
				counters.Enabled = Convert.ToBoolean(settings["perf-counters"]);
				return counters;
			}

			private static Provider[] GetProviders(Hashtable settings)
			{
				if(settings == null)
				{
					return null;
				}
				Provider[] providerArray = new Provider[settings.Count];
				int index = 0;
				IDictionaryEnumerator enumerator = settings.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if(enumerator.Value is Hashtable)
					{
						Provider provider = new Provider();
						IDictionaryEnumerator enumerator2 = ((Hashtable)enumerator.Value).GetEnumerator();
						while (enumerator2.MoveNext())
						{
							if(enumerator2.Key.Equals("assembly-name"))
								provider.AssemblyName = (string)enumerator2.Value;
							if(enumerator2.Key.Equals("class-name"))
								provider.ClassName = (string)enumerator2.Value;
							if(enumerator2.Key.Equals("provider-name"))
								provider.ProviderName = (string)enumerator2.Value;
							if(enumerator2.Key.Equals("full-name"))
								provider.FullProviderName = (string)enumerator2.Value;
							if(enumerator2.Key.Equals("default-provider"))
								provider.IsDefaultProvider = Convert.ToBoolean(enumerator2.Value);
							if(enumerator2.Key.Equals("parameters"))
								provider.Parameters = GetParameters(enumerator2.Value as Hashtable);
						}
						providerArray[index] = provider;
						index++;
					}
				}
				return providerArray;
			}

			private static Readthru GetReadThru(Hashtable settings)
			{
				Readthru readthru = new Readthru();
				if(settings.ContainsKey("read-thru-providers"))
				{
					readthru.Providers = GetProviders(settings["read-thru-providers"] as Hashtable);
				}
				return readthru;
			}

			private static NotificationRecipient[] GetRecipients(Hashtable settings)
			{
				NotificationRecipient[] recipientArray = null;
				if(settings.Count != 0)
				{
					recipientArray = new NotificationRecipient[settings.Count];
					int index = 0;
					IDictionaryEnumerator enumerator = settings.GetEnumerator();
					while (enumerator.MoveNext())
					{
						recipientArray[index] = new NotificationRecipient();
						recipientArray[index].ID = enumerator.Key.ToString();
						index++;
					}
				}
				return recipientArray;
			}

			private static Storage GetStorage(Hashtable settings)
			{
				Storage storage = new Storage();
				if(settings.ContainsKey("class"))
					storage.Type = settings["class"].ToString();
				if(settings.ContainsKey("heap"))
					storage.Size = Convert.ToInt64(((Hashtable)settings["heap"])["max-size"]);
				return storage;
			}

			private static void GetWebCache(CacheConfig cache, Hashtable settings)
			{
				if(settings.ContainsKey("shared"))
					cache.InProc = !Convert.ToBoolean(settings["shared"]);
			}

			private static Writethru GetWriteThru(Hashtable settings)
			{
				Writethru writethru = new Writethru();
				if(settings.ContainsKey("write-thru-providers"))
					writethru.Providers = GetProviders(settings["write-thru-providers"] as Hashtable);
				return writethru;
			}
		}
	}
}
