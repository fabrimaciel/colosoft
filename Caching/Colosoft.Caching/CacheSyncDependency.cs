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

namespace Colosoft.Caching.Synchronization
{
	/// <summary>
	/// Possíveis situações de dependencia.
	/// </summary>
	public enum DependencyStatus
	{
		/// <summary>
		/// Sem mudança.
		/// </summary>
		Unchanged,
		/// <summary>
		/// Expirou.
		/// </summary>
		Expired,
		/// <summary>
		/// Houve modificação.
		/// </summary>
		HasChanged
	}
	/// <summary>
	/// Representa a dependencia de sincronização do cache.
	/// </summary>
	[Serializable]
	public class CacheSyncDependency : ICompactSerializable
	{
		private ISyncCache _cache;

		private string _cacheId;

		private string _key;

		/// <summary>
		/// Identificador do cache.
		/// </summary>
		public string CacheId
		{
			get
			{
				if(_cacheId == null)
					return null;
				return _cacheId.ToLower();
			}
			set
			{
				_cacheId = value;
			}
		}

		/// <summary>
		/// Chave da instancia.
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		/// <summary>
		/// Instancia de sincronização do cache.
		/// </summary>
		public ISyncCache SyncCache
		{
			get
			{
				return _cache;
			}
			set
			{
				_cache = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheId">Identificador do cache.</param>
		/// <param name="key">Chave associada.</param>
		/// <param name="cache">Instancia de sincronização do cache.</param>
		public CacheSyncDependency(string cacheId, string key, ISyncCache cache)
		{
			_key = key;
			_cacheId = cacheId;
			_cache = cache;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_cacheId);
			writer.WriteObject(_key);
			writer.WriteObject(_cache);
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_cacheId = reader.ReadObject() as string;
			_key = reader.ReadObject() as string;
			_cache = reader.ReadObject() as ISyncCache;
		}

		/// <summary>
		/// Recupera a situação.
		/// </summary>
		/// <param name="synchronizer"></param>
		/// <returns></returns>
		internal DependencyStatus GetStatus(CacheSyncManager synchronizer)
		{
			DependencyStatus expired = DependencyStatus.Expired;
			if(synchronizer != null)
				expired = synchronizer.GetDependencyStatus(new CacheSyncManager.SyncItem(_cacheId, this.Key));
			return expired;
		}
	}
}
