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

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Armazena os dados do lock de uma expiração.
	/// </summary>
	[Serializable]
	public class LockExpiration : ICompactSerializable
	{
		private long _lastTimeStamp;

		private long _lockTTL;

		private TimeSpan _ttl;

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		private long SortKey
		{
			get
			{
				return (_lastTimeStamp + _lockTTL);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public LockExpiration()
		{
		}

		/// <summary>
		/// Cria uma instancia já informando o timeout do lock.
		/// </summary>
		/// <param name="lockTimeout"></param>
		public LockExpiration(TimeSpan lockTimeout)
		{
			_ttl = lockTimeout;
		}

		/// <summary>
		/// Verifica se já expirou.
		/// </summary>
		/// <returns></returns>
		public bool HasExpired()
		{
			return (SortKey.CompareTo(CachingUtils.DiffTicks(DateTime.Now)) < 0);
		}

		/// <summary>
		/// Set no lock.
		/// </summary>
		public void Set()
		{
			_lockTTL = _ttl.Ticks;
			_lastTimeStamp = CachingUtils.DiffTicks(DateTime.Now);
		}

		/// <summary>
		/// Serialializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write(_lockTTL);
			writer.Write(_lastTimeStamp);
			writer.WriteObject(_ttl);
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_lockTTL = reader.ReadInt64();
			_lastTimeStamp = reader.ReadInt64();
			_ttl = (TimeSpan)reader.ReadObject();
		}
	}
}
