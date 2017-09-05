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
using Colosoft.Caching.Policies;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Data;
using System.Collections;
using Colosoft.Caching.Locking;
using Colosoft.Serialization.IO;
using Colosoft.IO.Compression;
using Colosoft.Serialization.Formatters;
using System.Threading;
using System.IO;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa uma entrada do cache.
	/// </summary>
	[Serializable]
	public class CacheEntry : CacheItemBase, IDisposable, ICloneable, ICompactSerializable, ICustomSerializable, IStreamItem, ISizable
	{
		private LockAccessType _accessType;

		private DateTime _creationTime;

		private EvictionHint _evictionHint;

		private ExpirationHint _expirationHint;

		private BitSet _flags;

		private GroupInfo _grpInfo;

		private Hashtable _keysDependingOnMe;

		private DateTime _lastModifiedTime;

		private TimeSpan _lockAge;

		private DateTime _lockDate;

		private LockExpiration _lockExpiration;

		private object _lockId;

		private LockManager _lockManager;

		private MetaInformation _metaInformation;

		private CacheItemPriority _priorityValue;

		private string _providerName;

		private Hashtable _queryInfo;

		private string _resyncProviderName;

		private int _size;

		private Colosoft.Caching.Synchronization.CacheSyncDependency _syncDependency;

		private ulong _version;

		/// <summary>
		/// Data de criação da entrada.
		/// </summary>
		public DateTime CreationTime
		{
			get
			{
				return _creationTime;
			}
			set
			{
				lock (this)
					_creationTime = value;
			}
		}

		/// <summary>
		/// Tamanho dos dados.
		/// </summary>
		public long DataSize
		{
			get
			{
				if(_size > -1)
					return (long)_size;
				int size = 0;
				if(this.Value != null)
				{
					if(this.Value is UserBinaryObject)
						size = ((UserBinaryObject)this.Value).Size;
					else if((this.Value is CallbackEntry) && (((CallbackEntry)this.Value).Value != null))
						size = ((UserBinaryObject)((CallbackEntry)this.Value).Value).Size;
				}
				return (long)size;
			}
		}

		/// <summary>
		/// Hint de liberação.
		/// </summary>
		public EvictionHint EvictionHint
		{
			get
			{
				return _evictionHint;
			}
			set
			{
				lock (this)
					_evictionHint = value;
			}
		}

		/// <summary>
		/// <see cref="ExpirationHint"/> associado.
		/// </summary>
		public ExpirationHint ExpirationHint
		{
			get
			{
				return _expirationHint;
			}
			set
			{
				lock (this)
					_expirationHint = value;
			}
		}

		/// <summary>
		/// Conjunto com os flags da instancia.
		/// </summary>
		public BitSet Flag
		{
			get
			{
				return _flags;
			}
			set
			{
				lock (this)
				{
					_flags = value;
				}
			}
		}

		/// <summary>
		/// Informações do grupo da entrada.
		/// </summary>
		public GroupInfo GroupInfo
		{
			get
			{
				return _grpInfo;
			}
			set
			{
				lock (this)
				{
					_grpInfo = value;
				}
			}
		}

		/// <summary>
		/// Identifica se os dados da entrada estão comprimidos.
		/// </summary>
		internal bool IsCompressed
		{
			get
			{
				return _flags.IsBitSet(2);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal bool IsFlattened
		{
			get
			{
				return _flags.IsBitSet(1);
			}
		}

		/// <summary>
		/// Chave que são depdendente da entrada.
		/// </summary>
		public Hashtable KeysDependingOnMe
		{
			get
			{
				return _keysDependingOnMe;
			}
			set
			{
				lock (this)
					_keysDependingOnMe = value;
			}
		}

		/// <summary>
		/// Chave que a entrada é dependente.
		/// </summary>
		public object[] KeysIAmDependingOn
		{
			get
			{
				ArrayList list = null;
				if(this.ExpirationHint != null)
				{
					if(this.ExpirationHint.HintType == ExpirationHintType.AggregateExpirationHint)
					{
						ExpirationHint[] hints = ((AggregateExpirationHint)this.ExpirationHint).Hints;
						for(int i = 0; i < hints.Length; i++)
						{
							if(hints[i].HintType == ExpirationHintType.KeyDependency)
							{
								if(list == null)
									list = new ArrayList();
								string[] cacheKeys = ((KeyDependency)hints[i]).CacheKeys;
								if((cacheKeys != null) && (cacheKeys.Length > 0))
								{
									for(int j = 0; j < cacheKeys.Length; j++)
									{
										if(!list.Contains(cacheKeys[j]))
											list.Add(cacheKeys[j]);
									}
								}
							}
						}
						if((list != null) && (list.Count > 0))
						{
							object[] array = new object[list.Count];
							list.CopyTo(array, 0);
							return array;
						}
					}
					else if(this.ExpirationHint.HintType == ExpirationHintType.KeyDependency)
						return ((KeyDependency)this.ExpirationHint).CacheKeys;
				}
				return null;
			}
		}

		/// <summary>
		/// Data da ultima modificação da entrada.
		/// </summary>
		public DateTime LastModifiedTime
		{
			get
			{
				return _lastModifiedTime;
			}
			set
			{
				lock (this)
				{
					_lastModifiedTime = value;
				}
			}
		}

		/// <summary>
		/// Comprimento da entrada.
		/// </summary>
		public int Length
		{
			get
			{
				int size = 0;
				if(this.Value != null)
				{
					if(this.Value is UserBinaryObject)
						return ((UserBinaryObject)this.Value).Size;
					if((this.Value is CallbackEntry) && (((CallbackEntry)this.Value).Value != null))
						size = ((UserBinaryObject)((CallbackEntry)this.Value).Value).Size;
				}
				return size;
			}
			set
			{
				throw new NotSupportedException("Set length is not supported.");
			}
		}

		/// <summary>
		/// Tipo de acesso de lock.
		/// </summary>
		public LockAccessType LockAccessType
		{
			get
			{
				return _accessType;
			}
			set
			{
				lock (this)
					_accessType = value;
			}
		}

		/// <summary>
		/// Idade do lock.
		/// </summary>
		public TimeSpan LockAge
		{
			get
			{
				return _lockAge;
			}
			set
			{
				lock (this)
					_lockAge = value;
			}
		}

		/// <summary>
		/// Data do lock.
		/// </summary>
		public DateTime LockDate
		{
			get
			{
				return _lockDate;
			}
			set
			{
				lock (this)
					_lockDate = value;
			}
		}

		/// <summary>
		/// Expiração do lock
		/// </summary>
		public LockExpiration LockExpiration
		{
			get
			{
				return _lockExpiration;
			}
			set
			{
				_lockExpiration = value;
			}
		}

		/// <summary>
		/// Identificador do lock da instancia.
		/// </summary>
		public object LockId
		{
			get
			{
				return _lockId;
			}
			set
			{
				lock (this)
					_lockId = value;
			}
		}

		/// <summary>
		/// Informações de metadado.
		/// </summary>
		internal MetaInformation MetaInformation
		{
			get
			{
				return _metaInformation;
			}
			set
			{
				_metaInformation = value;
			}
		}

		/// <summary>
		/// Prioridade do item.
		/// </summary>
		public CacheItemPriority Priority
		{
			get
			{
				return _priorityValue;
			}
			set
			{
				_priorityValue = value;
			}
		}

		/// <summary>
		/// Nome do provedor associado.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
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
			set
			{
				lock (this)
				{
					_queryInfo = value;
				}
			}
		}

		/// <summary>
		/// Nome do provedor de ressincronização.
		/// </summary>
		public string ResyncProviderName
		{
			get
			{
				return _resyncProviderName;
			}
			set
			{
				_resyncProviderName = value;
			}
		}

		/// <summary>
		/// Gerenciador do lcok de escrita e leitura.
		/// </summary>
		public LockManager RWLockManager
		{
			get
			{
				if(_lockManager == null)
					lock (this)
						if(_lockManager == null)
							_lockManager = new LockManager();
				return _lockManager;
			}
		}

		/// <summary>
		/// Tamanho da instancia.
		/// </summary>
		public int Size
		{
			get
			{
				return (200 + ((int)this.DataSize));
			}
		}

		/// <summary>
		/// Dependencia de sincronização.
		/// </summary>
		public Colosoft.Caching.Synchronization.CacheSyncDependency SyncDependency
		{
			get
			{
				return _syncDependency;
			}
			set
			{
				lock (this)
					_syncDependency = value;
			}
		}

		/// <summary>
		/// Dados do usuário.
		/// </summary>
		public Array UserData
		{
			get
			{
				Array array = null;
				if(this.Value == null)
					return array;
				UserBinaryObject obj2 = null;
				if(this.Value is CallbackEntry)
				{
					if(((CallbackEntry)this.Value).Value != null)
						obj2 = ((CallbackEntry)this.Value).Value as UserBinaryObject;
				}
				else
					obj2 = this.Value as UserBinaryObject;
				return obj2.Data;
			}
		}

		/// <summary>
		/// Valor associado.
		/// </summary>
		public override object Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				lock (this)
				{
					if(_flags != null)
					{
						if((value is byte[]) || (value is UserBinaryObject))
							_flags.SetBit(1);
						else
							_flags.UnsetBit(1);
					}
					object obj2 = value;
					if((value is Array) && !(obj2 is byte[]))
						obj2 = new UserBinaryObject((Array)value);
					if((base.Value is CallbackEntry) && (obj2 is UserBinaryObject))
					{
						CallbackEntry entry = base.Value as CallbackEntry;
						entry.Value = obj2;
					}
					else
						base.Value = obj2;
				}
			}
		}

		/// <summary>
		/// Versão da instancia.
		/// </summary>
		public ulong Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CacheEntry()
		{
			_flags = new BitSet();
			_creationTime = new DateTime();
			_lastModifiedTime = new DateTime();
			_size = -1;
			_version = 1;
		}

		/// <summary>
		/// Construtor interno.
		/// </summary>
		/// <param name="val">Instancia associada com a entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="evictionHint">Hint de liberação.</param>
		internal CacheEntry(object val, ExpirationHint expiryHint, EvictionHint evictionHint) : base(val)
		{
			_flags = new BitSet();
			_creationTime = new DateTime();
			_lastModifiedTime = new DateTime();
			_size = -1;
			_version = 1;
			_expirationHint = expiryHint;
			_evictionHint = evictionHint;
			_creationTime = DateTime.Now;
			_lastModifiedTime = DateTime.Now;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			lock (this)
			{
				this.Value = reader.ReadObject();
				_flags = new BitSet(reader.ReadByte());
				_evictionHint = EvictionHint.ReadEvcHint(reader);
				_expirationHint = ExpirationHint.ReadExpHint(reader);
				_grpInfo = GroupInfo.ReadGrpInfo(reader);
				_syncDependency = reader.ReadObject() as Colosoft.Caching.Synchronization.CacheSyncDependency;
				_queryInfo = (Hashtable)reader.ReadObject();
				_keysDependingOnMe = (Hashtable)reader.ReadObject();
				_size = reader.ReadInt32();
				_lockId = reader.ReadObject();
				_lockDate = reader.ReadDateTime();
				_version = reader.ReadUInt64();
				_lockExpiration = reader.ReadObject() as LockExpiration;
				_creationTime = reader.ReadDateTime();
				_lastModifiedTime = reader.ReadDateTime();
				_resyncProviderName = reader.ReadObject() as string;
				_priorityValue = (CacheItemPriority)reader.ReadInt32();
				_lockManager = reader.ReadObject() as LockManager;
				_providerName = reader.ReadObject() as string;
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			lock (this)
			{
				writer.WriteObject(this.Value);
				writer.Write(_flags.Data);
				EvictionHint.WriteEvcHint(writer, _evictionHint);
				ExpirationHint.WriteExpHint(writer, _expirationHint);
				GroupInfo.WriteGrpInfo(writer, _grpInfo);
				writer.WriteObject(_syncDependency);
				writer.WriteObject(_queryInfo);
				writer.WriteObject(_keysDependingOnMe);
				writer.Write(_size);
				writer.WriteObject(_lockId);
				writer.Write(_lockDate);
				writer.Write(_version);
				writer.WriteObject(_lockExpiration);
				writer.Write(_creationTime);
				writer.Write(_lastModifiedTime);
				writer.WriteObject(_resyncProviderName);
				writer.Write((int)_priorityValue);
				writer.WriteObject(_lockManager);
				writer.WriteObject(_providerName);
			}
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="reader">Leitor contendo os dados para serem deserializados.</param>
		public void DeserializeLocal(System.IO.BinaryReader reader)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		public void SerializeLocal(BinaryWriter writer)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Copia a versão.
		/// </summary>
		/// <param name="entry"></param>
		private void CopyVersion(CacheEntry entry)
		{
			lock (this)
				_version = entry.Version;
		}

		/// <summary>
		/// Cria uma instancia clone.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public object Clone()
		{
			CacheEntry entry = new CacheEntry(this.Value, _expirationHint, _evictionHint);
			lock (this)
			{
				if(_grpInfo != null)
					entry._grpInfo = (GroupInfo)_grpInfo.Clone();
				entry._flags = (BitSet)_flags.Clone();
				entry.Priority = this.Priority;
				entry._syncDependency = _syncDependency;
				entry._queryInfo = _queryInfo;
				if(_keysDependingOnMe != null)
					entry._keysDependingOnMe = _keysDependingOnMe.Clone() as Hashtable;
				entry._lockId = _lockId;
				entry._lockDate = _lockDate;
				entry._size = _size;
				entry._lockAge = _lockAge;
				entry._version = _version;
				entry._creationTime = _creationTime;
				entry._lastModifiedTime = _lastModifiedTime;
				entry._lockExpiration = _lockExpiration;
				entry._resyncProviderName = _resyncProviderName;
				entry._providerName = _providerName;
				entry._lockManager = _lockManager;
			}
			return entry;
		}

		/// <summary>
		/// Cria uma instancia clone sem o valor.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public CacheEntry CloneWithoutValue()
		{
			CacheEntry entry = new CacheEntry();
			lock (this)
			{
				entry._expirationHint = _expirationHint;
				entry._evictionHint = _evictionHint;
				if(_grpInfo != null)
				{
					entry._grpInfo = (GroupInfo)_grpInfo.Clone();
				}
				entry._flags = (BitSet)_flags.Clone();
				entry._syncDependency = _syncDependency;
				entry._queryInfo = _queryInfo;
				if(_keysDependingOnMe != null)
				{
					entry._keysDependingOnMe = _keysDependingOnMe.Clone() as Hashtable;
				}
				entry._lockId = _lockId;
				entry._lockDate = _lockDate;
				entry._size = _size;
				entry._lockAge = _lockAge;
				entry._version = _version;
				entry._creationTime = _creationTime;
				entry._lastModifiedTime = _lastModifiedTime;
				entry._lockExpiration = _lockExpiration;
				entry._resyncProviderName = _resyncProviderName;
				entry._providerName = _providerName;
				if(this.Value is CallbackEntry)
				{
					var entry2 = (CallbackEntry)this.Value;
					entry2 = entry2.Clone() as CallbackEntry;
					entry2.Value = null;
					entry.Value = entry2;
				}
				entry._lockManager = _lockManager;
			}
			return entry;
		}

		/// <summary>
		/// Compara o identificador de lock com o lock da instancia.
		/// </summary>
		/// <param name="lockId">Identificador de lock que será comparado.</param>
		/// <returns></returns>
		public bool CompareLock(object lockId)
		{
			lock (this)
			{
				if(this.Flag.IsAnyBitSet(0x40))
				{
					if(lockId == null)
						return false;
					if(object.Equals(this.LockId, lockId))
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Compara com a versão informada.
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public bool CompareVersion(ulong version)
		{
			lock (this)
				return _version == version;
		}

		/// <summary>
		/// Copia os dados do lock para a instancia.
		/// </summary>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="lockExpiration">Data que o lock vai expirar.</param>
		public void CopyLock(object lockId, DateTime lockDate, LockExpiration lockExpiration)
		{
			lock (this)
			{
				if(lockId != null)
					Flag.SetBit(0x40);
				else
					Flag.UnsetBit(0x40);
				LockId = lockId;
				LockDate = lockDate;
				LockExpiration = lockExpiration;
			}
		}

		/// <summary>
		/// Adiciona informações da callback para a instancia.
		/// </summary>
		/// <param name="updateCallback"></param>
		/// <param name="removeCallback"></param>
		internal void AddCallbackInfo(CallbackInfo updateCallback, CallbackInfo removeCallback)
		{
			lock (this)
			{
				CallbackEntry entry;
				if(this.Value is CallbackEntry)
					entry = this.Value as CallbackEntry;
				else
				{
					entry = new CallbackEntry();
					entry.Value = this.Value;
					entry.Flag = this.Flag;
					this.Value = entry;
				}
				if(updateCallback != null)
					entry.AddItemUpdateCallback(updateCallback);
				if(removeCallback != null)
					entry.AddItemRemoveCallback(removeCallback);
			}
		}

		/// <summary>
		/// Recupera o valor da entrada.
		/// </summary>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal object DeflattedValue(string cacheContext)
		{
			object obj2 = this.Value;
			lock (this)
			{
				if(!IsFlattened)
					return obj2;
				UserBinaryObject obj3 = null;
				CallbackEntry entry = obj2 as CallbackEntry;
				if(entry != null)
					obj3 = entry.Value as UserBinaryObject;
				else
					obj3 = obj2 as UserBinaryObject;
				byte[] fullObject = obj3.GetFullObject();
				if(IsCompressed)
				{
					fullObject = CompressionUtil.Decompress(fullObject);
					_flags.UnsetBit(2);
				}
				_size = fullObject.Length;
				obj2 = CompactBinaryFormatter.FromByteBuffer(fullObject, cacheContext);
				if(entry != null)
				{
					entry.Value = obj2;
					obj2 = entry;
				}
			}
			return obj2;
		}

		/// <summary>
		/// Reencontra o valor do objeto
		/// </summary>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal object DeflattenObject(string cacheContext)
		{
			lock (this)
				if(IsFlattened)
					Value = CompactBinaryFormatter.FromByteBuffer(this.UserData as byte[], cacheContext);
			return this.Value;
		}

		/// <summary>
		/// Cria um clone da entrada do os dados reduzidos.
		/// </summary>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal CacheEntry FlattenedClone(string cacheContext)
		{
			CacheEntry entry = (CacheEntry)this.Clone();
			entry.FlattenObject(cacheContext);
			return entry;
		}

		/// <summary>
		/// Recupera o objeto resumido.
		/// </summary>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal object FlattenObject(string cacheContext)
		{
			return this.Value;
		}

		/// <summary>
		/// Verifica se a entrada está bloqueada.
		/// </summary>
		/// <returns></returns>
		public bool IsItemLocked()
		{
			lock (this)
			{
				return (((LockExpiration == null) || !LockExpiration.HasExpired()) && Flag.IsAnyBitSet(0x40));
			}
		}

		/// <summary>
		/// Verifica se a instancia está com lock.
		/// </summary>
		/// <param name="lockId"></param>
		/// <param name="lockDate"></param>
		/// <returns></returns>
		public bool IsLocked(ref object lockId, ref DateTime lockDate)
		{
			lock (this)
			{
				if(this.Flag.IsAnyBitSet(0x40))
				{
					if((LockExpiration == null) || !LockExpiration.HasExpired())
					{
						lockId = this.LockId;
						lockDate = this.LockDate;
						return true;
					}
					this.ReleaseLock();
					return false;
				}
				return false;
			}
		}

		/// <summary>
		/// Verifica se a versão é mais nova qua a informada.
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public bool IsNewer(ulong version)
		{
			lock (this)
			{
				return (this.Version > version);
			}
		}

		/// <summary>
		/// Mantem o valor compactado.
		/// </summary>
		/// <param name="cacheContext"></param>
		internal void KeepDeflattedValue(string cacheContext)
		{
			CacheEntry entry;
			Monitor.Enter(entry = this);
			try
			{
				if(this.IsFlattened)
				{
					this.Value = this.DeflattedValue(cacheContext);
					_flags.UnsetBit(1);
				}
			}
			catch(Exception)
			{
			}
			finally
			{
				Monitor.Exit(entry);
			}
		}

		/// <summary>
		/// Realiza um lock na instancia.
		/// </summary>
		/// <param name="lockExpiration">Dados sobre a expiração do lock.</param>
		/// <param name="lockId">Identificador do lock gerado.</param>
		/// <param name="lockDate">Data do lock gerado.</param>
		/// <returns>True caso o lock tenha sido criado com sucesso.</returns>
		public bool Lock(LockExpiration lockExpiration, ref object lockId, ref DateTime lockDate)
		{
			lock (this)
			{
				if(!IsLocked(ref lockId, ref lockDate))
				{
					Flag.SetBit(0x40);
					LockId = lockId;
					LockDate = lockDate;
					LockExpiration = lockExpiration;
					if(LockExpiration != null)
						LockExpiration.Set();
					return true;
				}
				lockId = LockId;
				lockDate = LockDate;
				return false;
			}
		}

		/// <summary>
		/// Libera o lock da instancia.
		/// </summary>
		public void ReleaseLock()
		{
			lock (this)
			{
				this.LockId = null;
				this.LockDate = new DateTime();
				this.Flag.UnsetBit(0x40);
			}
		}

		/// <summary>
		/// Remove as informações de callback.
		/// </summary>
		/// <param name="updateCallback"></param>
		/// <param name="removeCallback"></param>
		internal void RemoveCallbackInfo(CallbackInfo updateCallback, CallbackInfo removeCallback)
		{
			lock (this)
			{
				if((updateCallback != null) || (removeCallback != null))
				{
					CallbackEntry entry = null;
					if(this.Value is CallbackEntry)
					{
						entry = this.Value as CallbackEntry;
						if(updateCallback != null)
							entry.RemoveItemUpdateCallback(updateCallback);
						if(removeCallback != null)
							entry.RemoveItemRemoveCallback(removeCallback);
					}
				}
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("CacheEntry[" + this.Value.ToString() + "]");
		}

		/// <summary>
		/// Atualiza a ultima data da modificação com base na entrada informada.
		/// </summary>
		/// <param name="entry"></param>
		public void UpdateLastModifiedTime(CacheEntry entry)
		{
			lock (this)
				_creationTime = entry.CreationTime;
		}

		/// <summary>
		/// Atualiza a versão com base na entrada informada.
		/// </summary>
		/// <param name="entry"></param>
		public void UpdateVersion(CacheEntry entry)
		{
			lock (this)
			{
				this.CopyVersion(entry);
				_version++;
			}
		}

		/// <summary>
		/// Lê dados da instancia.
		/// </summary>
		/// <param name="offset">Offset para leitura.</param>
		/// <param name="length">Quantidade que serão lidos.</param>
		/// <returns></returns>
		public VirtualArray Read(int offset, int length)
		{
			VirtualArray array = null;
			UserBinaryObject obj2 = (this.Value is CallbackEntry) ? ((UserBinaryObject)((CallbackEntry)this.Value).Value) : ((UserBinaryObject)this.Value);
			if(obj2 != null)
				array = obj2.Read(offset, length);
			return array;
		}

		/// <summary>
		/// Escreve dados para a instancia.
		/// </summary>
		/// <param name="vBuffer">Buffer contendo os dados que serão escritos.</param>
		/// <param name="srcOffset">Offset da origem.</param>
		/// <param name="dstOffset">Offset de destino.</param>
		/// <param name="length">Quantidade de dados que serão escritos.</param>
		public void Write(VirtualArray vBuffer, int srcOffset, int dstOffset, int length)
		{
			UserBinaryObject obj2 = (this.Value is CallbackEntry) ? ((UserBinaryObject)((CallbackEntry)this.Value).Value) : ((UserBinaryObject)this.Value);
			if(obj2 != null)
				obj2.Write(vBuffer, srcOffset, dstOffset, length);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if(_expirationHint != null)
				{
					((IDisposable)_expirationHint).Dispose();
					if(((this.KeysIAmDependingOn == null) || (this.KeysIAmDependingOn.Length == 0)) && ((this.KeysDependingOnMe == null) || (this.KeysDependingOnMe.Count == 0)))
						_expirationHint = null;
				}
				_evictionHint = null;
			}
		}
	}
}
