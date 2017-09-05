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
	/// Representa os enumeradores usados pela <see cref="SRTree"/>.
	/// </summary>
	public class SRTreeEnumerator
	{
		private IEnumerator _ide;

		private ArrayList _list;

		/// <summary>
		/// Atual item do enumerador.
		/// </summary>
		public object Current
		{
			get
			{
				return _ide.Current;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="list"></param>
		public SRTreeEnumerator(ArrayList list)
		{
			_list = list;
			_ide = _list.GetEnumerator();
		}

		/// <summary>
		/// Move para o próximo item.
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			try
			{
				return _ide.MoveNext();
			}
			catch(Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Reseta o enumerado.
		/// </summary>
		public void Reset()
		{
			_ide.Reset();
		}
	}
}
