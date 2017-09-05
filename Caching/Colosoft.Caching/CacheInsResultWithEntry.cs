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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe usada para armazenar o resultado da
	/// inserção no cache.
	/// </summary>
	[Serializable]
	internal class CacheInsResultWithEntry : ICompactSerializable
	{
		private CacheEntry _entry;

		private CacheInsResult _result;

		/// <summary>
		/// Entrada associada.
		/// </summary>
		public CacheEntry Entry
		{
			get
			{
				return _entry;
			}
			set
			{
				_entry = value;
			}
		}

		/// <summary>
		/// Resultado da inserção.
		/// </summary>
		public CacheInsResult Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CacheInsResultWithEntry()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry">Entrada inserida.</param>
		/// <param name="result">Resultado da inserção.</param>
		public CacheInsResultWithEntry(CacheEntry entry, CacheInsResult result)
		{
			_entry = entry;
			_result = result;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			_entry = (CacheEntry)reader.ReadObject();
			_result = (CacheInsResult)reader.ReadObject();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			writer.WriteObject(_entry);
			writer.WriteObject(_result);
		}
	}
}
