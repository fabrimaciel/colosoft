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
using Colosoft.Caching.Local;
using System.Collections;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação de um indice de consulta virtual.
	/// </summary>
	internal class VirtualQueryIndex : IQueryIndex
	{
		private LocalCacheBase _cache;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache">Instancia do cache que será utilizada pelo indice.</param>
		public VirtualQueryIndex(LocalCacheBase cache)
		{
			_cache = cache;
		}

		/// <summary>
		/// Adiciona uma chave e um valor para o indice.
		/// </summary>
		/// <param name="key">Chave que será adicionada.</param>
		/// <param name="value">Valor que será adicionado.</param>
		public void AddToIndex(object key, object value)
		{
		}

		/// <summary>
		/// Remove a chave e o valor do indice.
		/// </summary>
		/// <param name="key">Chave que será removida.</param>
		/// <param name="value">Valor que será removido.</param>
		public void RemoveFromIndex(object key, object value)
		{
		}

		/// <summary>
		/// Limpa os dados do indice.
		/// </summary>
		public void Clear()
		{
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		private IDictionaryEnumerator GetEnumerator()
		{
			if(_cache != null)
				return _cache.GetEnumerator();
			return null;
		}

		/// <summary>
		/// Recupera o enumerado dos dados do indice.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será usado para filtra os dados.</param>
		/// <param name="forTag">Identifica ser o enumerado será recuperado para tags.</param>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator(string typeName, bool forTag)
		{
			if(_cache == null)
				return null;
			if(typeName == "*")
				return this.GetEnumerator();
			IDictionaryEnumerator enumerator = _cache.GetEnumerator();
			Hashtable hashtable = new Hashtable();
			while (enumerator.MoveNext())
			{
				if(((CacheEntry)enumerator.Value).DeflattedValue(_cache.Context.CacheImpl.Name).GetType().FullName == typeName)
					hashtable[enumerator.Key] = enumerator.Value;
			}
			return hashtable.GetEnumerator();
		}
	}
}
