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

namespace Colosoft.Caching.Storage.Util
{
	/// <summary>
	/// Representa um enumerador de retardo para o armazenamento.
	/// </summary>
	internal class LazyStoreEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private DictionaryEntry _de;

		private IDictionaryEnumerator _enumerator;

		private StorageProviderBase _store;

		/// <summary>
		/// Instancia da entrada do dicionário.
		/// </summary>
		DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if(_de.Value == null)
					_de.Value = _store.Get(_de.Key);
				return _de;
			}
		}

		object IDictionaryEnumerator.Key
		{
			get
			{
				return _de.Key;
			}
		}

		object IDictionaryEnumerator.Value
		{
			get
			{
				if(_de.Value == null)
					_de.Value = _store.Get(_de.Key);
				return _de.Value;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				if(_de.Value == null)
					_de.Value = _store.Get(_de.Key);
				return _de;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="store"></param>
		/// <param name="enumerator"></param>
		public LazyStoreEnumerator(StorageProviderBase store, IDictionaryEnumerator enumerator)
		{
			_store = store;
			_enumerator = enumerator;
		}

		bool IEnumerator.MoveNext()
		{
			if(_enumerator.MoveNext())
			{
				_de = new DictionaryEntry(_enumerator.Key, null);
				return true;
			}
			return false;
		}

		void IEnumerator.Reset()
		{
			_enumerator.Reset();
		}
	}
}
