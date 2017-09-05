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
	internal class CacheEnumerator : MarshalByRefObject, IDictionaryEnumerator, IEnumerator
	{
		private string _cacheContext;

		private DictionaryEntry _de;

		private IDictionaryEnumerator _enumerator;

		public CacheEnumerator(string cacheContext, IDictionaryEnumerator enumerator)
		{
			_enumerator = enumerator;
			_cacheContext = cacheContext;
		}

		protected object FetchObject()
		{
			return (_enumerator.Value as CacheEntry);
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

		DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				if(_de.Value == null)
				{
					_de.Value = this.FetchObject();
				}
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
				{
					_de.Value = this.FetchObject();
				}
				return _de.Value;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				if(_de.Value == null)
				{
					_de.Value = this.FetchObject();
				}
				return _de;
			}
		}
	}
}
