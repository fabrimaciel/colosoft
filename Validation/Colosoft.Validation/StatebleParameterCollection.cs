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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Validation
{
	/// <summary>
	/// Representa a coleção de parametros do controle de estado.
	/// </summary>
	public class StatebleParameterCollection : IDictionary
	{
		private Hashtable _table = new Hashtable();

		/// <summary>
		/// Determina se a coleção possui um tamanho fixado.
		/// </summary>
		public bool IsFixedSize
		{
			get
			{
				return false;
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
		/// Chaves dos parametros.
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return _table.Keys;
			}
		}

		/// <summary>
		/// Coleção dos valores dos parametros.
		/// </summary>
		public ICollection Values
		{
			get
			{
				return _table.Values;
			}
		}

		/// <summary>
		/// Recupera e define o parametro pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object this[object key]
		{
			get
			{
				return _table[key];
			}
			set
			{
				_table[key] = value;
			}
		}

		/// <summary>
		/// Quantidade de parametros na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _table.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return _table.IsSynchronized;
			}
		}

		/// <summary>
		/// Objeto de sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return _table.SyncRoot;
			}
		}

		/// <summary>
		/// Adiciona um parametro para a coleção.
		/// </summary>
		/// <param name="key">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public void Add(object key, object value)
		{
			_table[key] = value;
		}

		/// <summary>
		/// Limpa os parametros da coleção.
		/// </summary>
		public void Clear()
		{
			_table.Clear();
		}

		/// <summary>
		/// Verifica se na coleção possui o parametro com o nome informado.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(object key)
		{
			return _table.Contains(key);
		}

		/// <summary>
		/// Recupera o enumerador dos parametros na coleção.
		/// </summary>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			return _table.GetEnumerator();
		}

		/// <summary>
		/// Remove um parametro pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(object key)
		{
			_table.Remove(key);
		}

		/// <summary>
		/// Copia os daos para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			_table.CopyTo(array, index);
		}

		/// <summary>
		/// Recuper ao enumerador dos parametros.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _table.GetEnumerator();
		}
	}
}
