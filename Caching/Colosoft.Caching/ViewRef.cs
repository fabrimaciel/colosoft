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

namespace Colosoft.Caching.Storage.Mmf
{
	/// <summary>
	/// Representa a referencia da visão da memória.
	/// </summary>
	internal class ViewRef : View
	{
		private int _refCount;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="mmf">MmfFile.</param>
		/// <param name="id">Identificador da instancia.</param>
		/// <param name="size"></param>
		public ViewRef(MmfFile mmf, uint id, uint size) : base(mmf, id, size)
		{
		}

		/// <summary>
		/// Abrea a instancia.
		/// </summary>
		/// <returns></returns>
		public override int Open()
		{
			lock (this)
			{
				if(!base.IsOpen)
					base.Open();
				return ++_refCount;
			}
		}

		/// <summary>
		/// Fecha a instancia.
		/// </summary>
		/// <returns></returns>
		public override int Close()
		{
			lock (this)
			{
				if(base.IsOpen)
				{
					_refCount--;
					if(_refCount <= 0)
						base.Close();
				}
				return _refCount;
			}
		}

		/// <summary>
		/// Força fechar a instancia.
		/// </summary>
		public void ForceClose()
		{
			lock (this)
			{
				if(base.IsOpen)
				{
					base.Close();
					_refCount = 0;
				}
			}
		}
	}
}
