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
using Colosoft.Threading;
using System.Collections;
using Colosoft.Serialization;
using System.Diagnostics;
using Colosoft.Caching.Policies;
using Colosoft.Caching.Expiration;
using System.Reflection;
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Loaders;
using Colosoft.Caching.Util;
using Colosoft.IO.Compression;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o gerenciador da fonte de dados.
	/// </summary>
	internal class DatasourceMgr : IDisposable
	{
		public AsyncProcessor _asyncProc;

		private string _cacheName;

		private CacheRuntimeContext _context;

		private string _defaultReadThruProvider;

		private string _defaultWriteThruProvider;

		private Dictionary<object, IAsyncTask> _queue;

		private IDictionary<string, ReadThruProviderManager> _readerProivder = new Dictionary<string, ReadThruProviderManager>();

		private Type _type = typeof(ICompactSerializable);

		public WriteBehindAsyncProcessor _writeBehindAsyncProcess;

		private IDictionary<string, WriteThruProviderManager> _writerProivder = new Dictionary<string, WriteThruProviderManager>();

		private bool anyWriteBehindEnabled;

		private bool anyWriteThruEnabled;

		/// <summary>
		/// Nome da classe.
		/// </summary>
		public string CacheName
		{
			get
			{
				return _cacheName;
			}
		}

		/// <summary>
		/// Nome do provedor padrão de leitura.
		/// </summary>
		public string DefaultReadThruProvider
		{
			get
			{
				if(_defaultReadThruProvider != null)
					return _defaultReadThruProvider.ToLower();
				return null;
			}
			set
			{
				_defaultReadThruProvider = value;
			}
		}

		/// <summary>
		/// Nome do provedor padrão de escrita.
		/// </summary>
		public string DefaultWriteThruProvider
		{
			get
			{
				if(_defaultWriteThruProvider != null)
					return _defaultWriteThruProvider.ToLower();
				return null;
			}
			set
			{
				_defaultWriteThruProvider = value;
			}
		}

		/// <summary>
		/// Identifica se a leitura através está ativa.
		/// </summary>
		public bool IsReadThruEnabled
		{
			get
			{
				return (_readerProivder.Count > 0);
			}
		}

		/// <summary>
		/// Identifica se  escrita através está ativa.
		/// </summary>
		public bool IsWriteThruEnabled
		{
			get
			{
				return this.anyWriteThruEnabled;
			}
		}

		/// <summary>
		/// Identifica se a escrita oculta está ativa.
		/// </summary>
		public bool IsWriteBehindEnabled
		{
			get
			{
				return this.anyWriteBehindEnabled;
			}
		}

		/// <summary>
		/// Identifica se é para carrega os tipos compactos.
		/// </summary>
		public bool LoadCompactTypes
		{
			get
			{
				if(!this.IsReadThruEnabled)
					return this.IsWriteThruEnabled;
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheName">Nome do cache.</param>
		/// <param name="properties">Hash com as propriedades de configuração.</param>
		/// <param name="context">Contexto de execução.</param>
		/// <param name="timeout"></param>
		public DatasourceMgr(string cacheName, IDictionary properties, CacheRuntimeContext context, long timeout)
		{
			_cacheName = cacheName;
			_context = context;
			_queue = new Dictionary<object, IAsyncTask>();
			this.Initialize(properties, timeout);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties">Propriedades de configuração.</param>
		/// <param name="timeout"></param>
		private void Initialize(IDictionary properties, long timeout)
		{
			properties.Require("properties").NotNull();
			try
			{
				if(properties.Contains("read-thru"))
				{
					IDictionary dictionary = (IDictionary)properties["read-thru"];
					string str = (string)dictionary["enabled"];
					if(str.ToLower() == "true")
					{
						IDictionary dictionary2 = (IDictionary)dictionary["read-thru-providers"];
						if(dictionary2 != null)
						{
							IDictionaryEnumerator enumerator = dictionary2.GetEnumerator();
							while (enumerator.MoveNext())
							{
								if(!_readerProivder.ContainsKey(enumerator.Key.ToString().ToLower()))
									_readerProivder.Add(enumerator.Key.ToString().ToLower(), new ReadThruProviderManager(_cacheName, dictionary2[enumerator.Key] as Hashtable, _context));
							}
						}
					}
				}
				if(properties.Contains("write-thru"))
				{
					IDictionary dictionary3 = (IDictionary)properties["write-thru"];
					string str2 = (string)dictionary3["enabled"];
					if(str2.ToLower() == "true")
					{
						IDictionary dictionary4 = (IDictionary)dictionary3["write-thru-providers"];
						if(dictionary4 != null)
						{
							IDictionaryEnumerator enumerator2 = dictionary4.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								if(!_writerProivder.ContainsKey(enumerator2.Key.ToString().ToLower()))
								{
									_writerProivder.Add(enumerator2.Key.ToString().ToLower(), new WriteThruProviderManager(_cacheName, dictionary4[enumerator2.Key] as Hashtable, _context, (int)timeout, enumerator2.Key.ToString()));
								}
							}
						}
					}
				}
				foreach (KeyValuePair<string, WriteThruProviderManager> pair in _writerProivder)
				{
					if(!pair.Value.AsyncWriteEnabled)
					{
						this.anyWriteThruEnabled = true;
						break;
					}
				}
				foreach (KeyValuePair<string, WriteThruProviderManager> pair2 in _writerProivder)
				{
					if(pair2.Value.AsyncWriteEnabled)
					{
						this.anyWriteBehindEnabled = true;
						break;
					}
				}
				if((_writerProivder != null) && this.anyWriteBehindEnabled)
				{
					_writeBehindAsyncProcess = new WriteBehindAsyncProcessor(timeout, _context.Logger);
				}
				if(_readerProivder != null)
				{
					_asyncProc = new AsyncProcessor("Cache.DataSourceMgr", _context.Logger);
					_asyncProc.Start();
				}
			}
			catch(ConfigurationException)
			{
				throw;
			}
			catch(Exception exception)
			{
				throw new ConfigurationException("Configuration Error: " + exception.ToString(), exception);
			}
		}

		/// <summary>
		/// Recupera a requisição de leitura enfilerada.
		/// </summary>
		/// <param name="key">Chave da tarefa.</param>
		/// <returns></returns>
		private CacheResyncTask GetQueuedReadRequest(object key)
		{
			lock (_queue)
				return (CacheResyncTask)_queue[key];
		}

		/// <summary>
		/// Recupera os gerenciadores de escrita através.
		/// </summary>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="providers">Hash com os provedores.</param>
		/// <param name="operationCode">Código da operação.</param>
		/// <returns></returns>
		private IEnumerable<WriteThruProviderManager> GetWriteThruMgr(string providerName, IDictionary<string, WriteThruProviderManager> providers, OpCode operationCode)
		{
			if(string.IsNullOrEmpty(providerName))
				providerName = _defaultWriteThruProvider;
			if(providerName != null)
			{
				if(operationCode != OpCode.Clear)
				{
					WriteThruProviderManager item = null;
					providers.TryGetValue(providerName.ToLower(), out item);
					yield return item;
				}
				else
					foreach (var i in providers)
						yield return i.Value;
			}
		}

		/// <summary>
		/// Retira uma tarefa de escrita oculta.
		/// </summary>
		/// <param name="taskId">Identificador da tarefa.</param>
		/// <param name="providerName">Nome do provedor.</param>
		public void DequeueWriteBehindTask(string taskId, string providerName)
		{
			WriteThruProviderManager mgr = null;
			_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
			if(mgr != null)
				mgr.DequeueWriteBehindTask(taskId);
		}

		/// <summary>
		/// Recupera uma entrada do cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="item"></param>
		/// <param name="flag"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="cacheEntry"></param>
		/// <returns></returns>
		public UserBinaryObject GetCacheEntry(string key, ProviderCacheItem item, ref BitSet flag, string group, string subGroup, out CacheEntry cacheEntry)
		{
			UserBinaryObject val = null;
			cacheEntry = null;
			object serializableObject = null;
			if((item != null) && (item.Value != null))
			{
				if((item.Group == null) && (item.SubGroup != null))
					throw new OperationFailedException("Error occurred while synchronization with data source; group must be specified for sub group");
				if((group != null) && !CacheHelper.CheckDataGroupsCompatibility(new GroupInfo(item.Group, item.SubGroup), new GroupInfo(group, subGroup)))
					throw new OperationFailedException("Error occurred while synchronization with data source; groups are incompatible");
				if(flag == null)
					flag = new BitSet();
				serializableObject = item.Value;
				Hashtable hashtable = new Hashtable();
				TypeInfoMap typeInfoMap = _context.CacheRoot.GetTypeInfoMap();
				hashtable["query-info"] = CacheLoaderUtil.GetQueryInfo(item.Value, typeInfoMap);
				if(item.Tags != null)
				{
					Hashtable tagInfo = CacheLoaderUtil.GetTagInfo(item.Value, item.Tags);
					if(tagInfo != null)
						hashtable.Add("tag-info", tagInfo);
				}
				if(item.NamedTags != null)
				{
					try
					{
						Hashtable hashtable3 = CacheLoaderUtil.GetNamedTagsInfo(item.Value, item.NamedTags, typeInfoMap);
						if(hashtable3 != null)
							hashtable.Add("named-tag-info", hashtable3);
					}
					catch(Exception exception)
					{
						throw new OperationFailedException("Error occurred while synchronization with data source; " + exception.Message);
					}
				}
				if(!item.Value.GetType().IsSerializable && !_type.IsAssignableFrom(item.Value.GetType()))
					throw new OperationFailedException("Read through provider returned an object that is not serializable.");
				serializableObject = SerializationUtil.SafeSerialize(serializableObject, _context.SerializationContext, ref flag);
				if(_context.CompressionEnabled)
					item.Value = CompressionUtil.Compress(item.Value as byte[], ref flag, _context.CompressionThreshold);
				val = UserBinaryObject.CreateUserBinaryObject(serializableObject as byte[]);
				EvictionHint evictionHint = new PriorityEvictionHint(item.ItemPriority);
				ExpirationHint expiryHint = DependencyHelper.GetExpirationHint(item.Dependency, item.AbsoluteExpiration, item.SlidingExpiration);
				if(expiryHint != null)
				{
					expiryHint.CacheKey = key;
					if(item.ResyncItemOnExpiration)
					{
						expiryHint.SetBit(2);
					}
				}
				cacheEntry = new CacheEntry(val, expiryHint, evictionHint);
				cacheEntry.Flag = flag;
				cacheEntry.GroupInfo = new GroupInfo(item.Group, item.SubGroup);
				cacheEntry.QueryInfo = hashtable;
				cacheEntry.ResyncProviderName = (item.ResyncProviderName == null) ? null : item.ResyncProviderName.ToLower();
			}
			return val;
		}

		/// <summary>
		/// Recupera o provedor pelo nome informado.
		/// </summary>
		/// <param name="providerName">Nome do provedor.</param>
		/// <returns></returns>
		internal WriteThruProviderManager GetProvider(string providerName)
		{
			WriteThruProviderManager mgr = null;
			if((_writerProivder != null) && _writerProivder.ContainsKey(providerName.ToLower()))
				_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
			return mgr;
		}

		/// <summary>
		/// Lê os itens associados com as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves que serão lidas.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <returns></returns>
		public Dictionary<string, ProviderCacheItem> ReadThru(string[] keys, string providerName)
		{
			if(_readerProivder != null)
			{
				ReadThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultReadThruProvider;
				}
				if(_readerProivder.ContainsKey(providerName.ToLower()))
				{
					_readerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
						return mgr.ReadThru(keys);
				}
			}
			return null;
		}

		/// <summary>
		/// Lê o item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item que será recuperado.</param>
		/// <param name="providerName">Nome do provedor de leitura que será utilizado.</param>
		public void ReadThru(string key, out ProviderCacheItem item, string providerName)
		{
			item = null;
			if(_readerProivder != null)
			{
				ReadThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
					providerName = this.DefaultReadThruProvider;
				if(_readerProivder.ContainsKey(providerName.ToLower()))
				{
					_readerProivder.TryGetValue(providerName.ToLower(), out mgr);
					try
					{
						if(mgr != null)
							mgr.ReadThru(key, out item);
					}
					catch(Exception)
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Resincroniza
		/// </summary>
		/// <param name="orginalTable"></param>
		/// <param name="keys"></param>
		/// <param name="e"></param>
		/// <param name="flag"></param>
		/// <param name="providerName"></param>
		/// <param name="operationContext"></param>
		public void ResyncCacheItem(Hashtable orginalTable, string[] keys, CacheEntry[] e, BitSet[] flag, string providerName, OperationContext operationContext)
		{
			Dictionary<string, ProviderCacheItem> dictionary = this.ReadThru(keys, providerName);
			if((dictionary != null) && ((dictionary == null) || (dictionary.Count != 0)))
			{
				object[] array = new object[dictionary.Count];
				CacheEntry[] entryArray = new CacheEntry[dictionary.Count];
				int index = 0;
				for(int i = 0; i < keys.Length; i++)
				{
					ProviderCacheItem item;
					if(dictionary.TryGetValue(keys[i], out item) && (item != null))
					{
						try
						{
							CacheEntry entry;
							if(this.GetCacheEntry(keys[i], item, ref flag[i], null, null, out entry) != null)
							{
								array[index] = keys[i];
								entryArray[index++] = entry;
							}
						}
						catch(Exception exception)
						{
							_context.Logger.Error(("Error occurred while synchronization with data source; " + exception.Message).GetFormatter());
						}
					}
				}
				if(index != 0)
				{
					Cache.Resize(ref array, index);
					Cache.Resize(ref entryArray, index);
					Hashtable hashtable = null;
					try
					{
						hashtable = _context.CacheImpl.Insert(array, entryArray, false, operationContext);
					}
					catch(Exception exception2)
					{
						throw new OperationFailedException("error while trying to synchronize the cache with data source. Error: " + exception2.Message, exception2);
					}
					for(int j = 0; j < array.Length; j++)
					{
						if(hashtable.ContainsKey(array[j]))
						{
							CacheInsResultWithEntry entry2 = hashtable[array[j]] as CacheInsResultWithEntry;
							if((entry2 != null) && ((entry2.Result == CacheInsResult.Success) || (entry2.Result == CacheInsResult.SuccessOverwrite)))
							{
								object obj3 = entryArray[j].Value;
								if(obj3 is CallbackEntry)
								{
									obj3 = ((CallbackEntry)obj3).Value;
								}
								orginalTable.Add(array[j], new CompressedValueEntry(obj3, entryArray[j].Flag));
							}
						}
						else
						{
							object obj4 = entryArray[j].Value;
							if(obj4 is CallbackEntry)
							{
								obj4 = ((CallbackEntry)obj4).Value;
							}
							orginalTable.Add(array[j], new CompressedValueEntry(obj4, entryArray[j].Flag));
						}
					}
				}
			}
		}

		/// <summary>
		/// Ressincroniza a entrada do cache.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="entry">Instancia da entrada.</param>
		/// <param name="flag">Conjunto associado.</param>
		/// <param name="group">Grupo onde a entrada está inserida.</param>
		/// <param name="subGroup">Subgrupo da entrada.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public object ResyncCacheItem(string key, out CacheEntry entry, ref BitSet flag, string group, string subGroup, string providerName, OperationContext operationContext)
		{
			ProviderCacheItem item = null;
			this.ReadThru(key, out item, providerName);
			UserBinaryObject obj2 = null;
			try
			{
				obj2 = this.GetCacheEntry(key, item, ref flag, group, subGroup, out entry);
				if(obj2 == null)
					return obj2;
				CacheInsResultWithEntry entry2 = _context.CacheImpl.Insert(key, entry, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
				if(entry2.Result == CacheInsResult.Failure)
					throw new OperationFailedException("Operation failed to synchronize with data source");
				if(entry2.Result == CacheInsResult.NeedsEviction)
					throw new OperationFailedException("The cache is full and not enough items could be evicted.");
			}
			catch(Exception exception)
			{
				throw new OperationFailedException("Error occurred while synchronization with data source. Error: " + exception.Message, exception);
			}
			return obj2;
		}

		/// <summary>
		/// Ressincroniza o item de forma assincrona.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="exh">Hint de expiração.</param>
		/// <param name="evh">Hint de liberação.</param>
		/// <param name="groupInfo">Informações do grupo.</param>
		/// <param name="queryInfo">Informações da consulta.</param>
		/// <param name="resyncProviderName">Nome do provedor de ressincronização.</param>
		/// <returns></returns>
		public object ResyncCacheItemAsync(object key, ExpirationHint exh, EvictionHint evh, GroupInfo groupInfo, Hashtable queryInfo, string resyncProviderName)
		{
			lock (_queue)
			{
				if((_asyncProc != null) && (this.GetQueuedReadRequest(key) == null))
				{
					IAsyncTask evnt = new CacheResyncTask(this, key as string, exh, evh, _context.CompressionThreshold, groupInfo, queryInfo, resyncProviderName);
					_queue[key] = evnt;
					_asyncProc.Enqueue(evnt);
				}
				return null;
			}
		}

		/// <summary>
		/// Define o estado da tarefa.
		/// </summary>
		/// <param name="taskId">Identificador da tarefa.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="opCode">Código da operação.</param>
		/// <param name="state">Estadoda tarefa.</param>
		public void SetState(string taskId, string providerName, OpCode opCode, WriteBehindAsyncProcessor.TaskState state)
		{
			foreach (WriteThruProviderManager mgr in this.GetWriteThruMgr(providerName, _writerProivder, opCode))
			{
				if(mgr != null)
					mgr.SetState(taskId, state);
			}
		}

		/// <summary>
		/// Define o estado da tarefa.
		/// </summary>
		/// <param name="taskId">Identificador da tarefa.</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="opCode">Código da operação.</param>
		/// <param name="state">Estado da tarefa.</param>
		/// <param name="table"></param>
		public void SetState(string taskId, string providerName, OpCode opCode, WriteBehindAsyncProcessor.TaskState state, Hashtable table)
		{
			foreach (WriteThruProviderManager mgr in this.GetWriteThruMgr(providerName, _writerProivder, opCode))
				if(mgr != null)
					mgr.SetState(taskId, state, table);
		}

		public void StartWriteBehindProcessor()
		{
			if(_writeBehindAsyncProcess != null)
				_writeBehindAsyncProcess.Start();
		}

		public void WriteBehind(CacheBase internalCache, object key, CacheEntry entry, string source, string taskId, string providerName, OpCode operationCode)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
					providerName = this.DefaultWriteThruProvider;
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
						mgr.WriteBehind(internalCache, key, entry, source, taskId, operationCode);
				}
			}
		}

		public void WriteBehind(CacheBase internalCache, object key, CacheEntry entry, string source, string taskId, string providerName, OpCode operationCode, WriteBehindAsyncProcessor.TaskState state)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultWriteThruProvider;
				}
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
					{
						mgr.WriteBehind(internalCache, key, entry, source, taskId, operationCode, state);
					}
				}
			}
		}

		public void WriteBehind(CacheBase internalCache, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, string providerName, OpCode operationCode)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultWriteThruProvider;
				}
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
					{
						mgr.WriteBehind(internalCache, keys, values, entries, source, taskId, operationCode);
					}
				}
			}
		}

		public void WriteBehind(CacheBase internalCache, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, string providerName, OpCode operationCode, WriteBehindAsyncProcessor.TaskState state)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultWriteThruProvider;
				}
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
					{
						mgr.WriteBehind(internalCache, keys, values, entries, source, taskId, operationCode, state);
					}
				}
			}
		}

		public void WriteThru(string key, CacheEntry val, OpCode opCode, string providerName, OperationContext operationContext)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultWriteThruProvider;
				}
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
					{
						mgr.WriteThru(_context.CacheImpl, key, val, opCode, operationContext);
					}
				}
			}
		}

		public void WriteThru(string[] keys, object[] values, CacheEntry[] entries, Hashtable returnSet, OpCode opCode, string providerName, OperationContext operationContext)
		{
			if(_writerProivder != null)
			{
				WriteThruProviderManager mgr = null;
				if(string.IsNullOrEmpty(providerName))
				{
					providerName = this.DefaultWriteThruProvider;
				}
				if(_writerProivder.ContainsKey(providerName.ToLower()))
				{
					_writerProivder.TryGetValue(providerName.ToLower(), out mgr);
					if(mgr != null)
					{
						mgr.WriteThru(_context.CacheImpl, keys, values, entries, returnSet, opCode, operationContext);
					}
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_readerProivder != null)
			{
				IEnumerator enumerator = _readerProivder.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if(enumerator.Current != null)
						((IDisposable)enumerator.Current).Dispose();
				}
				_readerProivder = null;
			}
			if(_writeBehindAsyncProcess != null)
			{
				_writeBehindAsyncProcess.Stop();
				_writeBehindAsyncProcess = null;
			}
			if(_writerProivder != null)
			{
				IEnumerator enumerator2 = _writerProivder.Values.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if(enumerator2.Current != null)
						((IDisposable)enumerator2.Current).Dispose();
				}
				_writerProivder = null;
			}
			if(_asyncProc != null)
			{
				_asyncProc.Stop();
				_asyncProc = null;
			}
		}

		/// <summary>
		/// Implementação da tarefa de resincronização do cache.
		/// </summary>
		private class CacheResyncTask : IAsyncTask
		{
			private EvictionHint _evh;

			private ExpirationHint _exh;

			private BitSet _flag;

			private GroupInfo _groupInfo;

			private string _key;

			private DatasourceMgr _parent;

			private Hashtable _queryInfo;

			private string _resyncProviderName;

			private object _val;

			public EvictionHint EvictionHint
			{
				get
				{
					return _evh;
				}
			}

			public ExpirationHint ExpirationHint
			{
				get
				{
					return _exh;
				}
			}

			public BitSet Flag
			{
				get
				{
					return _flag;
				}
			}

			public GroupInfo GroupInfo
			{
				get
				{
					return _groupInfo;
				}
			}

			public Hashtable QueryInfo
			{
				get
				{
					return _queryInfo;
				}
			}

			public object Value
			{
				get
				{
					return _val;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="key"></param>
			/// <param name="exh"></param>
			/// <param name="evh"></param>
			/// <param name="compressionThreshold"></param>
			/// <param name="groupInfo"></param>
			/// <param name="queryInfo"></param>
			/// <param name="resyncProviderName"></param>
			public CacheResyncTask(DatasourceMgr parent, string key, ExpirationHint exh, EvictionHint evh, long compressionThreshold, GroupInfo groupInfo, Hashtable queryInfo, string resyncProviderName)
			{
				_parent = parent;
				_key = key;
				_exh = exh;
				_evh = evh;
				_groupInfo = groupInfo;
				_queryInfo = queryInfo;
				_resyncProviderName = resyncProviderName;
			}

			/// <summary>
			/// Recupera as informações da consulta com base na instancia informada.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			private Hashtable GetQueryInfo(object value)
			{
				Hashtable hashtable = null;
				if(_parent._context.CacheImpl.TypeInfoMap == null)
					return null;
				int handleId = 0;
				if(value is CacheItemRecord)
					handleId = _parent._context.CacheImpl.TypeInfoMap.GetHandleId(((CacheItemRecord)value).TypeName);
				else
					handleId = _parent._context.CacheImpl.TypeInfoMap.GetHandleId(value.GetType());
				if(handleId == -1)
					return hashtable;
				hashtable = new Hashtable();
				ArrayList list = new ArrayList();
				var typeMap = _parent._context.CacheImpl.TypeInfoMap;
				if(value is ICacheItemRecord)
				{
					var record = (ICacheItemRecord)value;
					var attribList = typeMap.GetAttribList(handleId);
					for(int i = 0; i < attribList.Count; i++)
					{
						var fieldIndex = -1;
						for(int j = 0; j < record.Descriptor.Count; j++)
							if(record.Descriptor[j].Name == attribList[i])
							{
								fieldIndex = j;
								break;
							}
						if(fieldIndex >= 0)
						{
							object obj2 = record.GetValue(fieldIndex);
							if(obj2 is string)
								obj2 = obj2.ToString().ToLower();
							else if(obj2 is DateTime)
							{
								DateTime time = (DateTime)obj2;
								obj2 = time.Ticks.ToString();
							}
							list.Add(obj2);
						}
						else
							list.Add(null);
					}
				}
				else
				{
					var attribList = typeMap.GetAttribList(handleId);
					for(int i = 0; i < attribList.Count; i++)
					{
						PropertyInfo property = value.GetType().GetProperty(attribList[i]);
						if(property != null)
						{
							object obj2 = property.GetValue(value, null);
							if(obj2 is string)
								obj2 = obj2.ToString().ToLower();
							list.Add(obj2);
						}
						else
						{
							FieldInfo field = value.GetType().GetField(attribList[i]);
							if(field == null)
								throw new Exception("Unable extracting query information from user object.");
							object obj3 = field.GetValue(value);
							if(obj3 is string)
								obj3 = obj3.ToString().ToLower();
							list.Add(obj3);
						}
					}
				}
				hashtable.Add(handleId, list);
				return hashtable;
			}

			/// <summary>
			/// Método do processo da tarefa.
			/// </summary>
			public void Process()
			{
				lock (this)
				{
					try
					{
						if(_val == null)
						{
							ProviderCacheItem item = null;
							CacheEntry cacheEntry = null;
							UserBinaryObject obj2 = null;
							try
							{
								_parent.ReadThru(_key, out item, _resyncProviderName);
								obj2 = _parent.GetCacheEntry(_key, item, ref _flag, (_groupInfo != null) ? _groupInfo.Group : null, (_groupInfo != null) ? _groupInfo.SubGroup : null, out cacheEntry);
							}
							catch(Exception exception)
							{
								_val = exception;
								_parent._context.Logger.Error(("DatasourceMgr.ResyncCacheItem: " + exception.StackTrace).GetFormatter());
							}
							if(!(_val is Exception) && (obj2 != null))
								_parent._context.CacheImpl.Insert(_key, cacheEntry, true, null, 0, LockAccessType.IGNORE_LOCK, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
							else
								_parent._context.CacheImpl.Remove(_key, ItemRemoveReason.Expired, true, null, 0, LockAccessType.IGNORE_LOCK, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						}
					}
					catch(Exception exception2)
					{
						_val = exception2;
						_parent._context.Logger.Error((exception2.Message + exception2.StackTrace).GetFormatter());
					}
					finally
					{
						_parent._queue.Remove(_key);
					}
				}
			}
		}
	}
}
