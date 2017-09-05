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

namespace Colosoft.Text.Jep.DataStructures
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	internal class ExtendedEnumerator<T> : IExtendedEnumerator<T>, IEnumerator<T>, IDisposable, IEnumerator
	{
		private List<T> _list;

		private int _position;

		public ExtendedEnumerator(List<T> input)
		{
			this._list = new List<T>();
			this._position = -1;
			this._list = input;
			this._position = -1;
		}

		public void Dispose()
		{
			this._list.Clear();
		}

		public bool HasNext()
		{
			return (this._position < (this._list.Count - 1));
		}

		public bool MoveNext()
		{
			this._position++;
			return (this._position < this._list.Count);
		}

		public T Next()
		{
			return this._list[++this._position];
		}

		public void Reset()
		{
			this._position = -1;
		}

		public T Current
		{
			get
			{
				try
				{
					return this._list[this._position];
				}
				catch(ArgumentOutOfRangeException)
				{
					return default(T);
				}
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._list[this._position];
			}
		}
	}
}
