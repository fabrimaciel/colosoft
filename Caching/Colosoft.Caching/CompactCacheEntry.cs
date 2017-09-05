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
using Colosoft.Caching.Expiration;
using Colosoft.Serialization.IO;
using Colosoft.Caching.Synchronization;
using Colosoft.Runtime;
using Colosoft.Serialization;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa uma entrada do cache compacta.
	/// </summary>
	public class CompactCacheEntry : ICompactSerializable, IRentableObject
	{
		private LockAccessType _accessType;

		private ExpirationHint _dependency;

		private long _expiration;

		private BitSet _flag;

		private string _group;

		private object _itemRemovedCallback;

		private object _key;

		private ArrayList _keysDependingOnMe;

		private object _lockId;

		private byte _options;

		private string _providerName;

		private Hashtable _queryInfo;

		private int _rentId;

		private string _resyncProviderName;

		private string _subgroup;

		private CacheSyncDependency _syncDependency;

		private object _value;

		private ulong _version;

		/// <summary>
		/// Objeto o callback.
		/// </summary>
		public object Callback
		{
			get
			{
				return _itemRemovedCallback;
			}
		}

		/// <summary>
		/// Dependencia associada.
		/// </summary>
		public ExpirationHint Dependency
		{
			get
			{
				return _dependency;
			}
		}

		/// <summary>
		/// Tempo para a expiração.
		/// </summary>
		public long Expiration
		{
			get
			{
				return _expiration;
			}
		}

		/// <summary>
		/// Flags.
		/// </summary>
		public BitSet Flag
		{
			get
			{
				return _flag;
			}
		}

		/// <summary>
		/// Nome do grupo.
		/// </summary>
		public string Group
		{
			get
			{
				return _group;
			}
		}

		/// <summary>
		/// Chave da entrada.
		/// </summary>
		public object Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Lista das chaves de dependencia ligas a entrada.
		/// </summary>
		public ArrayList KeysDependingOnMe
		{
			get
			{
				return _keysDependingOnMe;
			}
			set
			{
				_keysDependingOnMe = value;
			}
		}

		/// <summary>
		/// Tipo o lock de acesso.
		/// </summary>
		public LockAccessType LockAccessType
		{
			get
			{
				return _accessType;
			}
		}

		/// <summary>
		/// Identificador do lock;
		/// </summary>
		public object LockId
		{
			get
			{
				return _lockId;
			}
		}

		/// <summary>
		/// Flags com as opções.
		/// </summary>
		public byte Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
		}

		/// <summary>
		/// Informações da consulta.
		/// </summary>
		public Hashtable QueryInfo
		{
			get
			{
				return _queryInfo;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int RentId
		{
			get
			{
				return _rentId;
			}
			set
			{
				_rentId = value;
			}
		}

		/// <summary>
		/// Nome do provedor de sincronização.
		/// </summary>
		public string ResyncProviderName
		{
			get
			{
				return _resyncProviderName;
			}
		}

		/// <summary>
		/// Nome do subgrupo.
		/// </summary>
		public string SubGroup
		{
			get
			{
				return _subgroup;
			}
		}

		/// <summary>
		/// Dependencia de sincronização.
		/// </summary>
		public CacheSyncDependency SyncDependency
		{
			get
			{
				return _syncDependency;
			}
		}

		/// <summary>
		/// Valor associado.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Versão;
		/// </summary>
		public ulong Version
		{
			get
			{
				return _version;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CompactCacheEntry()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="dependency"></param>
		/// <param name="syncDependency"></param>
		/// <param name="expiration"></param>
		/// <param name="options"></param>
		/// <param name="itemRemovedCallback"></param>
		/// <param name="group"></param>
		/// <param name="subgroup"></param>
		/// <param name="queryInfo"></param>
		/// <param name="Flag"></param>
		/// <param name="lockId"></param>
		/// <param name="version"></param>
		/// <param name="accessType"></param>
		/// <param name="providername"></param>
		/// <param name="resyncProviderName"></param>
		public CompactCacheEntry(object key, object value, ExpirationHint dependency, CacheSyncDependency syncDependency, long expiration, byte options, object itemRemovedCallback, string group, string subgroup, Hashtable queryInfo, BitSet Flag, object lockId, ulong version, LockAccessType accessType, string providername, string resyncProviderName)
		{
			_key = key;
			_flag = Flag;
			_value = value;
			_dependency = dependency;
			_syncDependency = syncDependency;
			_expiration = expiration;
			_options = options;
			_itemRemovedCallback = itemRemovedCallback;
			if(group != null)
			{
				_group = group;
				if(subgroup != null)
					_subgroup = subgroup;
			}
			_queryInfo = queryInfo;
			_lockId = lockId;
			_accessType = accessType;
			_version = version;
			_providerName = providername;
			_resyncProviderName = resyncProviderName;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			try
			{
				writer.WriteObject(_key);
				writer.WriteObject(_value);
				writer.Write(_expiration);
				ExpirationHint.WriteExpHint(writer, _dependency);
				writer.Write(_options);
				writer.WriteObject(_itemRemovedCallback);
				writer.WriteObject(_group);
				writer.WriteObject(_subgroup);
				writer.WriteObject(_queryInfo);
				writer.WriteObject(_keysDependingOnMe);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_key = reader.ReadObject();
			_value = reader.ReadObject();
			_expiration = reader.ReadInt64();
			_dependency = ExpirationHint.ReadExpHint(reader);
			_options = reader.ReadByte();
			_itemRemovedCallback = reader.ReadObject();
			_group = (string)reader.ReadObject();
			_subgroup = (string)reader.ReadObject();
			_queryInfo = (Hashtable)reader.ReadObject();
			_keysDependingOnMe = (ArrayList)reader.ReadObject();
		}

		/// <summary>
		/// Reseta os dados da entrada.
		/// </summary>
		public void Reset()
		{
			_key = null;
			_value = null;
			_dependency = null;
			_expiration = 0;
			_options = 0;
			_itemRemovedCallback = null;
			_group = null;
			_subgroup = null;
			_queryInfo = null;
		}
	}
}
