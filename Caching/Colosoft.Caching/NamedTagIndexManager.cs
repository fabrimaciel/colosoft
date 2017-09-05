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
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação de um gerenciador de indice de tags nomeadas.
	/// </summary>
	internal class NamedTagIndexManager : TagIndexManager
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="props"></param>
		/// <param name="cache"></param>
		/// <param name="cacheName"></param>
		internal NamedTagIndexManager(IDictionary props, IndexedLocalCache cache, string cacheName) : base(props, cache, cacheName)
		{
		}

		private void AddNamedTag(object key, Hashtable value, CacheEntry entry)
		{
			if(value != null)
			{
				string str = value["type"] as string;
				Hashtable hashtable = (Hashtable)value["named-tags-list"];
				Hashtable attributeValues = (Hashtable)hashtable.Clone();
				List<string> list = new List<string>();
				foreach (string str2 in hashtable.Keys)
				{
					if(hashtable[str2] is string)
					{
						list.Add(str2);
					}
				}
				foreach (string str3 in list)
				{
					hashtable[str3] = (hashtable[str3] as string).ToLower();
				}
				if(!base.IndexMapInternal.ContainsKey(str))
				{
					base.IndexMapInternal[str] = new AttributeIndex(null, base._cacheName);
				}
				(base.IndexMapInternal[str] as IQueryIndex).AddToIndex(key, hashtable);
				if(entry.MetaInformation != null)
				{
					entry.MetaInformation.Add(attributeValues);
				}
				else
				{
					entry.MetaInformation = new MetaInformation(attributeValues);
					entry.MetaInformation.CacheKey = key as string;
					entry.MetaInformation.Type = value["type"] as string;
				}
			}
		}

		public override void AddToIndex(object key, CacheEntry entry)
		{
			base.AddToIndex(key, entry);
			Hashtable queryInfo = entry.QueryInfo;
			if(queryInfo.Contains("named-tag-info"))
				this.AddNamedTag(key, queryInfo["named-tag-info"] as Hashtable, entry);
		}

		internal override bool Initialize()
		{
			if(!base.Initialize())
			{
				base.IndexMapInternal = new Dictionary<string, IQueryIndex>();
			}
			return true;
		}

		public override void RemoveFromIndex(object key, Hashtable value)
		{
			base.RemoveFromIndex(key, value);
			if(value.Contains("named-tag-info"))
				this.RemoveNamedTag(key, (Hashtable)value["named-tag-info"]);
		}

		protected void RemoveNamedTag(object key, Hashtable value)
		{
			if(value != null)
			{
				string str = value["type"] as string;
				Hashtable hashtable = value["named-tags-list"] as Hashtable;
				List<string> list = new List<string>();
				foreach (string str2 in hashtable.Keys)
				{
					if(hashtable[str2] is string)
						list.Add(str2);
				}
				foreach (string str3 in list)
				{
					hashtable[str3] = (hashtable[str3] as string).ToLower();
				}
				if(base.IndexMapInternal.ContainsKey(str))
				{
					IQueryIndex index = base.IndexMapInternal[str] as IQueryIndex;
					index.RemoveFromIndex(key, hashtable);
					if(((AttributeIndex)index).Size == 0)
					{
						base.IndexMapInternal.Remove(str);
					}
				}
			}
		}
	}
}
