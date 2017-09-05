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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação do indice de atributos para o calculo da consulta ativa.
	/// </summary>
	public class ActiveQueryEvaluationIndex : AttributeIndex
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="attribList">Lista dos atributso que serão associados.</param>
		/// <param name="cacheName">Nome do cache.</param>
		public ActiveQueryEvaluationIndex(IEnumerable<string> attribList, string cacheName) : base(attribList, cacheName)
		{
		}

		/// <summary>
		/// Adiciona uma chave e um valor para o indice.
		/// </summary>
		/// <param name="key">Chave que será adicionada.</param>
		/// <param name="value">Valor que será adicionado.</param>
		public override void AddToIndex(object key, object value)
		{
			IDictionaryEnumerator enumerator = (value as Hashtable).GetEnumerator();
			while (enumerator.MoveNext())
			{
				string indexKey = (string)enumerator.Key;
				IIndexStore store = base._indexTable[indexKey] as IIndexStore;
				if(store == null)
				{
					if(indexKey == "$Tag$")
					{
						store = new HashStore();
						base._indexTable[indexKey] = store;
					}
					else
					{
						string str2 = base.ConvertToNamedTagKey(indexKey);
						store = base._indexTable[str2] as IIndexStore;
						if(store == null)
						{
							store = new HashStore();
							base._indexTable[str2] = store;
						}
					}
				}
				if(store != null)
				{
					object obj2 = enumerator.Value;
					if(obj2 != null)
					{
						store.Add(obj2, key);
					}
					else
					{
						store.Add(CacheNull.Value, key);
					}
				}
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="attribList"></param>
		protected override void Initialize(IEnumerable<string> attribList)
		{
			if(attribList != null)
			{
				IIndexStore store = null;
				IEnumerator enumerator = attribList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					store = new HashStore();
					base._indexTable[enumerator.Current.ToString()] = store;
				}
				store = new HashStore();
				base._indexTable["$Tag$"] = store;
			}
		}

		/// <summary>
		/// Remove a chave e o valor do indice.
		/// </summary>
		/// <param name="key">Chave que será removida.</param>
		/// <param name="value">Valor que será removido.</param>
		public override void RemoveFromIndex(object key, object value)
		{
			IDictionaryEnumerator enumerator = (value as Hashtable).GetEnumerator();
			while (enumerator.MoveNext())
			{
				string str = (string)enumerator.Key;
				if(base._indexTable.Contains(str) || base._indexTable.Contains(str = base.ConvertToNamedTagKey(str)))
				{
					IIndexStore store = base._indexTable[str] as IIndexStore;
					object obj2 = enumerator.Value;
					if(obj2 != null)
					{
						store.Remove(obj2, key);
					}
					else
					{
						store.Remove(CacheNull.Value, key);
					}
					if(store.Size == 0)
					{
						if((str == "$Tag$") || base.IsNamedTagKey(str))
						{
							base._indexTable.Remove(str);
						}
						else
						{
							base._indexTable[str] = new HashStore();
						}
					}
				}
			}
		}
	}
}
