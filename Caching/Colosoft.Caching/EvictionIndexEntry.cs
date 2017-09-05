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

namespace Colosoft.Caching.Policies
{
	/// <summary>
	/// Representa um entrada do indice de liberação.
	/// </summary>
	internal class EvictionIndexEntry
	{
		private Hashtable _hintIndex = new Hashtable();

		private long _next = -1;

		private long _previous = -1;

		/// <summary>
		/// Identificador da próxima entrada.
		/// </summary>
		internal long Next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}

		/// <summary>
		/// Identificador da entrada anterior.
		/// </summary>
		internal long Previous
		{
			get
			{
				return _previous;
			}
			set
			{
				_previous = value;
			}
		}

		/// <summary>
		/// Verifica se na entrada existe algum item com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal bool Contains(object key)
		{
			return _hintIndex.Contains(key);
		}

		/// <summary>
		/// Recupera todas as chaves da instancia.
		/// </summary>
		/// <returns></returns>
		internal ICollection GetAllKeys()
		{
			return _hintIndex.Keys;
		}

		/// <summary>
		/// Insere um novo chave.
		/// </summary>
		/// <param name="key"></param>
		internal void Insert(object key)
		{
			_hintIndex[key] = null;
		}

		/// <summary>
		/// Remove a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal bool Remove(object key)
		{
			if(_hintIndex != null)
				_hintIndex.Remove(key);
			return (_hintIndex.Count == 0);
		}
	}
}
