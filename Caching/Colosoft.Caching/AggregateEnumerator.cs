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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa um enumerador de agregação.
	/// </summary>
	internal class AggregateEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private int _currId;

		private IDictionaryEnumerator[] _enums;

		DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				return _enums[_currId].Entry;
			}
		}

		object IDictionaryEnumerator.Key
		{
			get
			{
				return _enums[_currId].Key;
			}
		}

		object IDictionaryEnumerator.Value
		{
			get
			{
				return _enums[_currId].Value;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return _enums[_currId].Current;
			}
		}

		public AggregateEnumerator(params IDictionaryEnumerator[] enums)
		{
			_enums = enums;
			((IEnumerator)this).Reset();
		}

		bool IEnumerator.MoveNext()
		{
			bool flag = _enums[_currId].MoveNext();
			if(!flag && (_currId < (_enums.Length - 1)))
			{
				_enums[++_currId].Reset();
				flag = _enums[_currId].MoveNext();
			}
			return flag;
		}

		void IEnumerator.Reset()
		{
			_currId = 0;
			_enums[_currId].Reset();
		}
	}
}
