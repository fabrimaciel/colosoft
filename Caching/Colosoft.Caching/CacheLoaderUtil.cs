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

namespace Colosoft.Caching.Loaders
{
	/// <summary>
	/// Classe que auxilia na carga do cache.
	/// </summary>
	internal class CacheLoaderUtil
	{
		/// <summary>
		/// Verifica se existem itens com o nome publicados.
		/// </summary>
		/// <param name="value">Instancia.</param>
		/// <param name="namedTags">Dicionário com as tags nomeadas.</param>
		/// <param name="typeMap"></param>
		private static void CheckDuplicateIndexName(object value, NamedTagsDictionary namedTags, TypeInfoMap typeMap)
		{
			if(namedTags != null && value != null && typeMap != null)
			{
				int handleId = 0;
				if(value is CacheItemRecord)
					handleId = typeMap.GetHandleId(((CacheItemRecord)value).TypeName);
				else
					handleId = typeMap.GetHandleId(value.GetType());
				if(handleId != -1)
				{
					foreach (string str2 in typeMap.GetAttribList(handleId))
					{
						if(namedTags.Contains(str2))
							throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_DuplicateIndexName).Format());
					}
				}
			}
		}

		/// <summary>
		/// Realiza a validação da expiração pelos parametros informados.
		/// </summary>
		/// <param name="absoluteExpiration"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		internal static int EvaluateExpirationParameters(DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			if(DateTime.MaxValue.Equals(absoluteExpiration) && TimeSpan.Zero.Equals(slidingExpiration))
				return 2;
			if(DateTime.MaxValue.Equals(absoluteExpiration))
			{
				if(slidingExpiration.CompareTo(TimeSpan.Zero) < 0)
					throw new ArgumentOutOfRangeException("slidingExpiration");
				if(slidingExpiration.CompareTo((TimeSpan)(DateTime.Now.AddYears(1) - DateTime.Now)) >= 0)
					throw new ArgumentOutOfRangeException("slidingExpiration");
				return 0;
			}
			if(!TimeSpan.Zero.Equals(slidingExpiration))
				throw new ArgumentException("You cannot set both sliding and absolute expirations on the same cache item.");
			return 1;
		}

		/// <summary>
		/// Processa os parametros de tag.
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <param name="group"></param>
		internal static void EvaluateTagsParameters(Hashtable queryInfo, string group)
		{
			if(((queryInfo != null) && !string.IsNullOrEmpty(group)) && (queryInfo["tag-info"] != null))
				throw new ArgumentException("You cannot set both groups and tags on the same cache item.");
		}

		/// <summary>
		/// Recupera as informações das tags nomeadas.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="namedTags"></param>
		/// <param name="typeMap"></param>
		/// <returns></returns>
		internal static Hashtable GetNamedTagsInfo(object value, NamedTagsDictionary namedTags, TypeInfoMap typeMap)
		{
			CheckDuplicateIndexName(value, namedTags, typeMap);
			if((namedTags == null) || (namedTags.Count == 0))
				return null;
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			foreach (DictionaryEntry entry in namedTags)
			{
				if(entry.Value == null)
					throw new ArgumentNullException("Named Tag value cannot be null");
				hashtable2.Add(entry.Key, entry.Value);
			}
			hashtable["is-itemrecord"] = value is CacheItemRecord;
			string str = null;
			if(value is CacheItemRecord)
				str = ((CacheItemRecord)value).TypeName.FullName;
			else
				str = value.GetType().FullName.Replace("+", ".");
			hashtable["type"] = str;
			hashtable["named-tags-list"] = hashtable2;
			return hashtable;
		}

		/// <summary>
		/// Recupera as informações de consulta.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="typeMap"></param>
		/// <returns></returns>
		internal static Hashtable GetQueryInfo(object value, TypeInfoMap typeMap)
		{
			Hashtable hashtable = null;
			ArrayList list = null;
			if(typeMap == null)
				return null;
			try
			{
				int handleId = -1;
				if(value is ICacheItemRecord)
				{
					var record = (ICacheItemRecord)value;
					handleId = typeMap.GetHandleId(record.TypeName);
					if(handleId == -1)
						return hashtable;
					hashtable = new Hashtable();
					list = new ArrayList();
					var attribList = typeMap.GetAttribList(handleId);
					for(int i = 0; i < attribList.Count; i++)
					{
						var fieldIndex = -1;
						for(int j = 0; j < record.Descriptor.Count; j++)
							if(record.Descriptor[j].Name == attribList[i])
							{
								fieldIndex = j;
								break;
							}
						if(fieldIndex >= 0)
						{
							object obj2 = record.GetValue(fieldIndex);
							if(obj2 is string)
								obj2 = obj2.ToString().ToLower();
							else if(obj2 is DateTime)
							{
								DateTime time = (DateTime)obj2;
								obj2 = time.Ticks.ToString();
							}
							list.Add(obj2);
						}
						else
							list.Add(null);
					}
				}
				else
				{
					handleId = typeMap.GetHandleId(value.GetType());
					if(handleId == -1)
						return hashtable;
					hashtable = new Hashtable();
					list = new ArrayList();
					var attribList = typeMap.GetAttribList(handleId);
					for(int i = 0; i < attribList.Count; i++)
					{
						var property = value.GetType().GetProperty(attribList[i]);
						if(property != null)
						{
							object obj2 = property.GetValue(value, null);
							if(obj2 is string)
								obj2 = obj2.ToString().ToLower();
							if(obj2 is DateTime)
							{
								DateTime time = (DateTime)obj2;
								obj2 = time.Ticks.ToString();
							}
							list.Add(obj2);
						}
						else
						{
							var field = value.GetType().GetField((string)attribList[i]);
							if(field == null)
								throw new Exception("Unable extracting query information from user object.");
							object obj3 = field.GetValue(value);
							if(obj3 is string)
								obj3 = obj3.ToString().ToLower();
							if(obj3 is DateTime)
							{
								DateTime time2 = (DateTime)obj3;
								obj3 = time2.Ticks.ToString();
							}
							list.Add(obj3);
						}
					}
				}
				hashtable.Add(handleId, list);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.Fail("An error ocurred when get query info", Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, true));
				throw;
			}
			return hashtable;
		}

		/// <summary>
		/// Recupera as informações das tags assicoadas.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="tags"></param>
		/// <returns></returns>
		internal static Hashtable GetTagInfo(object value, Tag[] tags)
		{
			if(tags == null)
				return null;
			Hashtable hashtable = new Hashtable();
			ArrayList list = new ArrayList();
			foreach (Tag tag in tags)
			{
				if(tag == null)
					throw new ArgumentNullException("Tag");
				if(tag.TagName != null)
					list.Add(tag.TagName);
			}
			hashtable["is-itemrecord"] = value is CacheItemRecord;
			if(value is CacheItemRecord)
				hashtable["type"] = ((CacheItemRecord)value).TypeName.FullName;
			else
				hashtable["type"] = value.GetType().FullName;
			hashtable["tags-list"] = list;
			return hashtable;
		}
	}
}
