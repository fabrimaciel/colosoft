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
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o indice de uma enumeração.
	/// </summary>
	internal class EnumerationIndex
	{
		private IndexedLocalCache _cache;

		private Dictionary<EnumerationPointer, IEnumerationProvider> _index;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache"></param>
		internal EnumerationIndex(IndexedLocalCache cache)
		{
			_cache = cache;
		}

		/// <summary>
		/// Verifica se na instancia contem o ponteiro informado.
		/// </summary>
		/// <param name="pointer">Instancia do ponteiro que será verificado.</param>
		/// <returns></returns>
		internal bool Contains(EnumerationPointer pointer)
		{
			return ((_index != null) && _index.ContainsKey(pointer));
		}

		/// <summary>
		/// Recupera o próximo pedaço.
		/// </summary>
		/// <param name="pointer">Ponteiro para o pedaço.</param>
		/// <returns></returns>
		internal EnumerationDataChunk GetNextChunk(EnumerationPointer pointer)
		{
			EnumerationDataChunk nextChunk = null;
			IEnumerationProvider provider = this.GetProvider(pointer);
			if(pointer.IsDisposable && (provider != null))
			{
				provider.Dispose();
				if(_index.ContainsKey(pointer))
					_index.Remove(pointer);
				nextChunk = new EnumerationDataChunk();
				nextChunk.Pointer = pointer;
				return nextChunk;
			}
			if(provider != null)
			{
				nextChunk = provider.GetNextChunk(pointer);
				if(nextChunk.IsLastChunk)
				{
					provider.Dispose();
					if(_index.ContainsKey(pointer))
						_index.Remove(pointer);
				}
			}
			return nextChunk;
		}

		/// <summary>
		/// Recupera o provedor de enumeração.
		/// </summary>
		/// <param name="pointer"></param>
		/// <returns></returns>
		private IEnumerationProvider GetProvider(EnumerationPointer pointer)
		{
			if(_index == null)
				_index = new Dictionary<EnumerationPointer, IEnumerationProvider>();
			IEnumerationProvider provider = null;
			if(_index.ContainsKey(pointer))
				return _index[pointer];
			return provider;
		}
	}
}
