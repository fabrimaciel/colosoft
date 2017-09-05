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

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação de um conjunto de itens.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private Dictionary<T, object> _dictionary;

		/// <summary>
		/// Quantidade de itens no conjunto.
		/// </summary>
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Cria uma nova instancia.
		/// </summary>
		public Set()
		{
			this._dictionary = new Dictionary<T, object>();
		}

		/// <summary>
		/// Cria uma nova instancia definindo o comparador.
		/// </summary>
		/// <param name="equalityComparer"></param>
		public Set(IEqualityComparer<T> equalityComparer)
		{
			this._dictionary = new Dictionary<T, object>(equalityComparer);
		}

		/// <summary>
		/// Cria uma nova instancia com a lista do conteúdo inicial.
		/// </summary>
		/// <param name="initialContents"></param>
		public Set(IList<T> initialContents)
		{
			this._dictionary = new Dictionary<T, object>(initialContents.Count);
			this.AddRange(initialContents);
		}

		/// <summary>
		/// Cria uma nova instancia definindo o tamanho incial do conjunto.
		/// </summary>
		/// <param name="numItems"></param>
		public Set(int numItems)
		{
			this._dictionary = new Dictionary<T, object>(numItems);
		}

		/// <summary>
		/// Cria uma nova instancia definindo os itens inciais e o comparador.
		/// </summary>
		/// <param name="initialContents"></param>
		/// <param name="equalityComparer"></param>
		public Set(IList<T> initialContents, IEqualityComparer<T> equalityComparer)
		{
			this._dictionary = new Dictionary<T, object>(initialContents.Count, equalityComparer);
			this.AddRange(initialContents);
		}

		/// <summary>
		/// Cria uma nova instancia definindo o tamanho e o comparador.
		/// </summary>
		/// <param name="numItems"></param>
		/// <param name="equalityComparer"></param>
		public Set(int numItems, IEqualityComparer<T> equalityComparer)
		{
			this._dictionary = new Dictionary<T, object>(numItems, equalityComparer);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			_dictionary[item] = null;
		}

		/// <summary>
		/// Adiciona uma faixa de itens.
		/// </summary>
		/// <param name="list"></param>
		public void AddRange(IEnumerable<T> list)
		{
			foreach (T local in list)
			{
				Add(local);
			}
		}

		/// <summary>
		/// Limpa os itens do conjunto.
		/// </summary>
		public void Clear()
		{
			_dictionary.Clear();
		}

		/// <summary>
		/// Verifica se no conjunto existe o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			return _dictionary.ContainsKey(item);
		}

		/// <summary>
		/// Copia o itens para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(T[] array, int index)
		{
			_dictionary.Keys.CopyTo(array, index);
		}

		/// <summary>
		/// Recupera o enumerador para os itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _dictionary.Keys.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador para os itens.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Remove o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			return _dictionary.Remove(item);
		}

		/// <summary>
		/// Transforma o conjunto em um vetor.
		/// </summary>
		/// <returns></returns>
		public T[] ToArray()
		{
			T[] array = new T[this.Count];
			this.CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Transforma o conjunto em uma lista.
		/// </summary>
		/// <returns></returns>
		public List<T> ToList()
		{
			return new List<T>(this._dictionary.Keys);
		}
	}
}
