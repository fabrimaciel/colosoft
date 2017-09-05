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
using Colosoft.Caching.Util;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação do armazena do tipo Hash para indices.
	/// </summary>
	public class HashStore : IIndexStore
	{
		private Hashtable _store = new Hashtable();

		/// <summary>
		/// Tamanho do armazenamento.
		/// </summary>
		public int Size
		{
			get
			{
				if(_store == null)
					return 0;
				return _store.Count;
			}
		}

		/// <summary>
		/// Adiciona um novo indice.
		/// </summary>
		/// <param name="key">Chave do indice.</param>
		/// <param name="value">Valor do indice.</param>
		public void Add(object key, object value)
		{
			if(_store != null)
				_store[key] = value;
		}

		/// <summary>
		/// Remove o indice informado.
		/// </summary>
		/// <param name="key">Chave do indice.</param>
		/// <param name="value">Valor do indice.</param>
		public void Remove(object key, object value)
		{
			if((_store != null) && _store.Contains(key))
				_store.Remove(key);
		}

		/// <summary>
		/// Limpa todos os dados.
		/// </summary>
		public void Clear()
		{
			if(_store != null)
				_store.Clear();
		}

		/// <summary>
		/// Recupera o indice com base na chave informada.
		/// </summary>
		/// <param name="key">Chave que será pesquisada.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <returns></returns>
		public ArrayList GetData(object key, ComparisonType comparisonType)
		{
			ArrayList list = new ArrayList();
			IComparable comparable = key as IComparable;
			if(_store != null)
			{
				switch(comparisonType)
				{
				case ComparisonType.EQUALS:
					if(_store.Contains(key))
						list.Add(_store[key]);
					return list;
				case ComparisonType.NOT_EQUALS:
					foreach (object obj2 in _store.Keys)
					{
						if(((IComparable)obj2).CompareTo(comparable) != 0)
							list.Add(_store[obj2]);
					}
					return list;
				case ComparisonType.LESS_THAN:
					foreach (object obj3 in _store.Keys)
					{
						if(((IComparable)obj3).CompareTo(comparable) < 0)
							list.Add(_store[obj3]);
					}
					return list;
				case ComparisonType.GREATER_THAN:
					foreach (object obj4 in _store.Keys)
					{
						if(((IComparable)obj4).CompareTo(comparable) > 0)
							list.Add(_store[obj4]);
					}
					return list;
				case ComparisonType.LESS_THAN_EQUALS:
					foreach (object obj5 in _store.Keys)
					{
						if(((IComparable)obj5).CompareTo(comparable) <= 0)
							list.Add(_store[obj5]);
					}
					return list;
				case ComparisonType.GREATER_THAN_EQUALS:
					foreach (object obj6 in _store.Keys)
					{
						if(((IComparable)obj6).CompareTo(comparable) >= 0)
							list.Add(_store[obj6]);
					}
					return list;
				case ComparisonType.LIKE:
					foreach (object obj7 in _store.Keys)
					{
						string pattern = key as string;
						var regex = new WildcardEnabledRegex(pattern);
						if((obj7 is string) && regex.IsMatch((string)obj7))
						{
							list.Add(_store[key]);
						}
					}
					return list;
				case ComparisonType.NOT_LIKE:
					foreach (object obj8 in _store.Keys)
					{
						string str2 = key as string;
						WildcardEnabledRegex regex2 = new WildcardEnabledRegex(str2);
						if((obj8 is string) && !regex2.IsMatch((string)obj8))
							list.Add(_store[key]);
					}
					return list;
				}
			}
			return list;
		}

		/// <summary>
		/// Recupera o enumerador do itens armazenados.
		/// </summary>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			if(_store != null)
				return _store.GetEnumerator();
			return new Hashtable().GetEnumerator();
		}
	}
}
