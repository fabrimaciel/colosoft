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
using Colosoft.Caching.Data;
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação do gerenciar do indice de tags.
	/// </summary>
	internal class TagIndexManager : QueryIndexManager
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="props">Properties do gerenciador.</param>
		/// <param name="cache">Instancia do cache indexado.</param>
		/// <param name="cacheName">Nome do cache.</param>
		internal TagIndexManager(IDictionary props, IndexedLocalCache cache, string cacheName) : base(props, cache, cacheName)
		{
		}

		/// <summary>
		/// Adiciona uma tag para a instancia.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void AddTag(object key, Hashtable value)
		{
			if(value != null)
			{
				string str = value["type"] as string;
				ArrayList list = value["tags-list"] as ArrayList;
				if(!base.IndexMapInternal.ContainsKey(str))
					base.IndexMapInternal[str] = new AttributeIndex(null, base._cacheName);
				IQueryIndex index = base.IndexMapInternal[str] as IQueryIndex;
				foreach (string str2 in list)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["$Tag$"] = str2.ToLower();
					index.AddToIndex(key, hashtable);
				}
			}
		}

		/// <summary>
		/// Remove a tag do gerenciador.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void RemoveTag(object key, Hashtable value)
		{
			if(value != null)
			{
				string str = value["type"] as string;
				var list = value["tags-list"] as ArrayList;
				if(base.IndexMapInternal.ContainsKey(str))
				{
					IQueryIndex index = base.IndexMapInternal[str] as IQueryIndex;
					foreach (string str2 in list)
					{
						Hashtable hashtable = new Hashtable();
						hashtable["$Tag$"] = str2.ToLower();
						index.RemoveFromIndex(key, hashtable);
					}
					if(((AttributeIndex)index).Size == 0)
					{
						base.IndexMapInternal.Remove(str);
					}
				}
			}
		}

		/// <summary>
		/// Recupera a combinação das chaves de qualquer tipo.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		private ArrayList GetCombinedKeysFromEveryType(string tag)
		{
			if(base.IndexMapInternal == null)
				return null;
			ArrayList data = null;
			Hashtable hashtable = new Hashtable();
			IDictionaryEnumerator enumerator = base.IndexMapInternal.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IIndexStore store = (enumerator.Value as AttributeIndex).GetStore("$Tag$");
				if(store != null)
				{
					data = store.GetData(tag.ToLower(), ComparisonType.EQUALS);
					if((data != null) && (data.Count > 0))
					{
						for(int i = 0; i < data.Count; i++)
							hashtable[data[i]] = null;
					}
				}
			}
			return new ArrayList(hashtable.Keys);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <returns></returns>
		internal override bool Initialize()
		{
			if(!base.Initialize())
				base.IndexMapInternal = new Dictionary<string, IQueryIndex>();
			return true;
		}

		/// <summary>
		/// Adiciona a entrada para o indice.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="entry"></param>
		public override void AddToIndex(object key, CacheEntry entry)
		{
			Hashtable queryInfo = entry.QueryInfo;
			if(queryInfo.Contains("query-info"))
				base.AddToIndex(key, entry);
			if(queryInfo.Contains("tag-info"))
				this.AddTag(key, queryInfo["tag-info"] as Hashtable);
		}

		/// <summary>
		/// Remove a entrada do gerenciador.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public override void RemoveFromIndex(object key, Hashtable value)
		{
			if(((Hashtable)value).Contains("query-info"))
				base.RemoveFromIndex(key, (Hashtable)value["query-info"]);
			if(((Hashtable)value).Contains("tag-info"))
				this.RemoveTag(key, (Hashtable)value["tag-info"]);
		}

		/// <summary>
		/// Recupera a lista com todas as tags compatíveis.
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
		public ArrayList GetAllMatchingTags(string[] tags)
		{
			Hashtable hashtable = new Hashtable();
			ArrayList combinedKeysFromEveryType = this.GetCombinedKeysFromEveryType(tags[0]);
			for(int i = 0; i < combinedKeysFromEveryType.Count; i++)
				hashtable[combinedKeysFromEveryType[i]] = null;
			for(int j = 1; j < tags.Length; j++)
			{
				Hashtable hashtable2 = new Hashtable();
				combinedKeysFromEveryType = this.GetCombinedKeysFromEveryType(tags[j]);
				if(combinedKeysFromEveryType != null)
				{
					for(int k = 0; k < combinedKeysFromEveryType.Count; k++)
					{
						object key = combinedKeysFromEveryType[k];
						if(hashtable.ContainsKey(key))
						{
							hashtable2[key] = null;
						}
					}
				}
				hashtable = hashtable2;
			}
			return new ArrayList(hashtable.Keys);
		}

		/// <summary>
		/// Recupera qualquer tag compatível.
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
		public ArrayList GetAnyMatchingTag(string[] tags)
		{
			ArrayList combinedKeysFromEveryType = this.GetCombinedKeysFromEveryType(tags[0]);
			Hashtable hashtable = new Hashtable();
			for(int i = 0; i < combinedKeysFromEveryType.Count; i++)
				hashtable[combinedKeysFromEveryType[i]] = null;
			for(int j = 1; j < tags.Length; j++)
			{
				ArrayList list2 = this.GetCombinedKeysFromEveryType(tags[j]);
				if((list2 != null) && (list2.Count > 0))
				{
					for(int k = 0; k < list2.Count; k++)
						hashtable[list2[k]] = null;
				}
			}
			return new ArrayList(hashtable.Keys);
		}

		/// <summary>
		/// Recupera os dados pela tag informada.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public ArrayList GetByTag(string tag)
		{
			return this.GetCombinedKeysFromEveryType(tag);
		}
	}
}
