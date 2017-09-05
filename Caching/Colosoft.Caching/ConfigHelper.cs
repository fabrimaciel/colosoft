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
using Colosoft.Caching.Exceptions;

namespace Colosoft.Caching.Configuration
{
	/// <summary>
	/// Classe com método auxiliares para a configuração.
	/// </summary>
	public class ConfigHelper
	{
		/// <summary>
		/// Recupera as informações do cache com base no dicionário das propriedades.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private static CacheInfo GetCacheInfo(IDictionary properties)
		{
			if(!properties.Contains("cache"))
				throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_MissingAttribute, "cache").Format());
			CacheInfo info = new CacheInfo();
			var dictionary = (IDictionary)properties["cache"];
			string str = "";
			if(dictionary.Contains("name"))
				info.Name = Convert.ToString(dictionary["name"]).Trim();
			if(!dictionary.Contains("class"))
				throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_MissingAttribute, "class").Format());
			str = Convert.ToString(dictionary["class"]);
			if(info.Name.Length < 1)
				info.Name = str;
			if(!dictionary.Contains("cache-classes"))
				throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_MissingSection, "cache-classes").Format());
			var dictionary2 = (IDictionary)dictionary["cache-classes"];
			if(!dictionary2.Contains(str.ToLower()))
				throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_NotFoundClassCache, str).Format());
			IDictionary dictionary3 = (IDictionary)dictionary2[str.ToLower()];
			if(!dictionary3.Contains("type"))
				throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_CannotFindTypeOfCache, str).Format());
			info.ClassName = Convert.ToString(dictionary3["type"]);
			return info;
		}

		/// <summary>
		/// Constrói o atributos para um string com base na lista de subpropriedades informadas.
		/// </summary>
		/// <param name="subProps"></param>
		/// <returns></returns>
		private static string BuildAttributes(Hashtable subProps)
		{
			var builder = new StringBuilder();
			var enumerator = (subProps.Clone() as Hashtable).GetEnumerator();
			while (enumerator.MoveNext())
			{
				string key = enumerator.Key as string;
				if(!(enumerator.Value is Hashtable))
				{
					builder.Append(" ").Append(key).Append("=").Append("\"").Append(enumerator.Value).Append("\"");
					subProps.Remove(key);
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o valor do dicionario de forma segura.
		/// </summary>
		/// <param name="dic">Dicionario onde estão os itens.</param>
		/// <param name="key">Chave do item que será recuperado.</param>
		/// <returns></returns>
		public static string SafeGet(IDictionary dic, string key)
		{
			return SafeGet(dic, key, null);
		}

		/// <summary>
		/// Recupera o valor do dicionário de forma segura.
		/// </summary>
		/// <param name="dic">Dicionario onde estão os itens.</param>
		/// <param name="key">Chave do item que será recuperado.</param>
		/// <param name="defaultValue">Valor padrão.</param>
		/// <returns></returns>
		public static string SafeGet(IDictionary dic, string key, object defaultValue)
		{
			object obj2 = null;
			if(dic != null)
				obj2 = dic[key];
			if(obj2 == null)
				obj2 = defaultValue;
			if(obj2 == null)
				return string.Empty;
			return obj2.ToString();
		}

		/// <summary>
		/// Recupera o par do valor do dicionário de forma segura.
		/// </summary>
		/// <param name="dic">Dicionario onde estão os itens.</param>
		/// <param name="key">Chave do item que será recuperado.</param>
		/// <param name="defaultValue">Valor padrão.</param>
		/// <returns></returns>
		public static string SafeGetPair(IDictionary dic, string key, object defaultValue)
		{
			string str = SafeGet(dic, key, defaultValue);
			if(str == "")
				return str;
			StringBuilder builder = new StringBuilder(64);
			builder.Append(key).Append("=").Append(str.ToString()).Append(";");
			return builder.ToString();
		}

		/// <summary>
		/// Converte a lista de propriedades para um xml.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <returns></returns>
		public static string CreatePropertiesXml(IDictionary properties)
		{
			return CreatePropertiesXml(properties, 0, false);
		}

		/// <summary>
		/// Converte a lista das propriedades para xml.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <param name="configId">Identificador da configuração.</param>
		/// <returns></returns>
		public static string CreatePropertiesXml(IDictionary properties, string configId)
		{
			StringBuilder builder = new StringBuilder("<cache-configuration id='");
			builder.Append(configId).Append("'>");
			builder.Append(CreatePropertiesXml(properties, 0, false)).Append("</cache-configuration>");
			return builder.ToString();
		}

		/// <summary>
		/// Converte a lista das propriedades para xml.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <param name="indent"></param>
		/// <param name="format">True identifica se é para formatar.</param>
		/// <returns></returns>
		public static string CreatePropertiesXml(IDictionary properties, int indent, bool format)
		{
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			var builder = new StringBuilder(0x1fa0);
			var builder2 = new StringBuilder(0x1fa0);
			string str = format ? "".PadRight(indent * 2) : "";
			string str2 = format ? "\n" : "";
			while (enumerator.MoveNext())
			{
				DictionaryEntry current = (DictionaryEntry)enumerator.Current;
				string key = current.Key as string;
				string str4 = "";
				if(current.Value is Hashtable)
				{
					Hashtable hashtable = (Hashtable)current.Value;
					if(hashtable.Contains("type") && hashtable.Contains("id"))
					{
						key = (string)hashtable["type"];
						if(hashtable.Contains("partitionId"))
							str4 = string.Concat(new object[] {
								" id='",
								hashtable["id"],
								"' partitionId='",
								hashtable["partitionId"],
								"'"
							});
						else
							str4 = " id='" + hashtable["id"] + "'";
						hashtable = (Hashtable)hashtable.Clone();
						hashtable.Remove("id");
						hashtable.Remove("type");
					}
					builder2.Append(str).Append("<" + key + str4 + ">").Append(str2);
					builder2.Append(CreatePropertiesXml(hashtable, indent + 1, format)).Append(str).Append("</" + key + ">").Append(str2);
				}
				else
				{
					builder.Append(str).Append("<" + key + ">").Append(current.Value).Append("</" + key + ">").Append(str2);
				}
			}
			builder.Append(builder2.ToString());
			return builder.ToString();
		}

		/// <summary>
		/// Converte a lista das propriedades para xml.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <param name="indent"></param>
		/// <param name="format">True identifica se é para formatar.</param>
		/// <returns></returns>
		public static string CreatePropertiesXml2(IDictionary properties, int indent, bool format)
		{
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			var builder = new StringBuilder(0x1fa0);
			var builder2 = new StringBuilder(0x1fa0);
			string str = format ? "".PadRight(indent * 2) : "";
			string str2 = format ? "\n" : "";
			while (enumerator.MoveNext())
			{
				DictionaryEntry current = (DictionaryEntry)enumerator.Current;
				string key = current.Key as string;
				string strB = "";
				if(current.Value is Hashtable)
				{
					Hashtable subProps = (Hashtable)current.Value;
					if(subProps.Contains("type") && subProps.Contains("name"))
					{
						strB = subProps["name"] as string;
						key = (string)subProps["type"];
						subProps = (Hashtable)subProps.Clone();
						subProps.Remove("type");
					}
					builder2.Append(str).Append("<" + key + BuildAttributes(subProps));
					if(subProps.Count == 0)
					{
						builder2.Append("/>").Append(str2);
					}
					else
					{
						if(subProps.Count == 1)
						{
							IDictionaryEnumerator enumerator2 = subProps.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								if((((string)enumerator2.Key).ToLower().CompareTo(strB) == 0) && (enumerator2.Value is IDictionary))
									subProps = enumerator2.Value as Hashtable;
							}
						}
						builder2.Append(">").Append(str2);
						builder2.Append(CreatePropertiesXml2(subProps, indent + 1, format)).Append(str).Append("</" + key + ">").Append(str2);
					}
				}
			}
			builder.Append(builder2.ToString());
			return builder.ToString();
		}

		/// <summary>
		/// Cria uma string com as propriedades de configuração.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static string CreatePropertyString(IDictionary properties)
		{
			return CreatePropertyString(properties, 0, false);
		}

		/// <summary>
		/// Cria uma string com as propriedades de configuração.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="indent"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string CreatePropertyString(IDictionary properties, int indent, bool format)
		{
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			StringBuilder builder = new StringBuilder(0x1fa0);
			StringBuilder builder2 = new StringBuilder(0x1fa0);
			string str = format ? "".PadRight(indent * 2) : "";
			string str2 = format ? "\n" : "";
			while (enumerator.MoveNext())
			{
				DictionaryEntry current = (DictionaryEntry)enumerator.Current;
				if(current.Value is Hashtable)
				{
					Hashtable hashtable = (Hashtable)current.Value;
					if(hashtable.Contains("type") && hashtable.Contains("id"))
					{
						builder2.Append(str).Append(hashtable["id"]);
						builder2.Append("=").Append(hashtable["type"]).Append(str2);
						hashtable = (Hashtable)hashtable.Clone();
						hashtable.Remove("id");
						hashtable.Remove("type");
					}
					else
						builder2.Append(str).Append(current.Key.ToString()).Append(str2);
					builder2.Append(str).Append("(").Append(str2).Append(CreatePropertyString(hashtable, indent + 1, format)).Append(str).Append(")").Append(str2);
				}
				else
				{
					builder.Append(str).Append(current.Key.ToString());
					if(current.Value is string)
						builder.Append("='").Append(current.Value).Append("';").Append(str2);
					else
						builder.Append("=").Append(current.Value).Append(";").Append(str2);
				}
			}
			builder.Append(builder2.ToString());
			return builder.ToString();
		}

		/// <summary>
		/// Cria uma string com as propriedades de configuração.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="indent"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string CreatePropertyString2(IDictionary properties, int indent, bool format)
		{
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			StringBuilder builder = new StringBuilder(0x1fa0);
			StringBuilder builder2 = new StringBuilder(0x1fa0);
			string str = format ? "".PadRight(indent * 2) : "";
			string str2 = format ? "\n" : "";
			while (enumerator.MoveNext())
			{
				DictionaryEntry current = (DictionaryEntry)enumerator.Current;
				if(current.Value is Hashtable)
				{
					Hashtable hashtable = (Hashtable)current.Value;
					if(hashtable.Contains("type") && hashtable.Contains("id"))
					{
						builder2.Append(str).Append(hashtable["id"]);
						builder2.Append("=").Append(hashtable["type"]).Append(str2);
						hashtable = (Hashtable)hashtable.Clone();
						hashtable.Remove("id");
						hashtable.Remove("type");
					}
					else
					{
						builder2.Append(str).Append(current.Key.ToString()).Append(str2);
					}
					builder2.Append(str).Append("(").Append(str2).Append(CreatePropertyString(hashtable, indent + 1, format)).Append(str).Append(")").Append(str2);
				}
				else
				{
					builder.Append(str).Append(current.Key.ToString());
					if(current.Value is string)
					{
						builder.Append("='").Append(current.Value).Append("';").Append(str2);
					}
					else
					{
						builder.Append("=").Append(current.Value).Append(";").Append(str2);
					}
				}
			}
			builder.Append(builder2.ToString());
			return builder.ToString();
		}

		/// <summary>
		/// Recupera as informações do cache do texto da propriedades.
		/// </summary>
		/// <param name="propString">Texto contendo os dados das propriedades.</param>
		/// <returns></returns>
		internal static CacheInfo GetCacheInfo(string propString)
		{
			PropsConfigReader reader = new PropsConfigReader(propString);
			CacheInfo cacheInfo = GetCacheInfo(reader.Properties);
			cacheInfo.ConfigString = propString;
			return cacheInfo;
		}

		/// <summary>
		/// Recupera o esquema do cache.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="properties"></param>
		/// <param name="cacheName"></param>
		/// <returns></returns>
		public static IDictionary GetCacheScheme(IDictionary cacheClasses, IDictionary properties, string cacheName)
		{
			IDictionary dictionary = null;
			if(properties.Contains(cacheName + "-ref"))
			{
				string key = Convert.ToString(properties[cacheName + "-ref"]).ToLower();
				if(!cacheClasses.Contains(key))
					throw new ConfigurationException("Cannot find cache class '" + key + "'");
				dictionary = (IDictionary)cacheClasses[key];
			}
			else if(properties.Contains(cacheName))
				dictionary = (IDictionary)properties[cacheName];
			if((dictionary == null) || !dictionary.Contains("type"))
				throw new ConfigurationException("Cannot find the type of cache, invalid configuration for cache class");
			return dictionary;
		}
	}
}
