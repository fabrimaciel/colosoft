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

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um dicionário de tags nomeadas.
	/// </summary>
	public class NamedTagsDictionary
	{
		private Hashtable _namedTags = new Hashtable();

		/// <summary>
		/// Quantidade de itens no dicionário.
		/// </summary>
		public int Count
		{
			get
			{
				return _namedTags.Count;
			}
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, bool value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, char value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, DateTime value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, decimal value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, double value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, int value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, long value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, float value)
		{
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave que representa o item.</param>
		/// <param name="value">Valor do item.</param>
		public void Add(string key, string value)
		{
			value.Require("value").NotNull();
			_namedTags.Add(key, value);
		}

		/// <summary>
		/// Verifica se no dicionário existe a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(string key)
		{
			return _namedTags.ContainsKey(key);
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _namedTags.GetEnumerator();
		}

		/// <summary>
		/// Remove a tag com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			_namedTags.Remove(key);
		}
	}
}
