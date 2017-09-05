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
using Colosoft.Caching.Storage;
using Colosoft.Caching.Policies;
using System.Threading;
using System.Collections;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Queries;
using Colosoft.Caching.Synchronization;

namespace Colosoft.Caching.Local
{
	/// <summary>
	/// Implementação de um cache local.
	/// </summary>
	internal class LocalCache : LocalCacheBase
	{
		private bool _allowExplicitGCCollection;

		private ICacheStorage _cacheStore;

		private object _evictionSyncMutex;

		private IEvictionPolicy _evictionPolicy;

		private Thread _evictionThread;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheClasses">Dicionário com as classes do cache.</param>
		/// <param name="parentCache">Instancia do cache pai.</param>
		/// <param name="properties"></param>
		/// <param name="listener"></param>
		/// <param name="context"></param>
		/// <param name="activeQueryAnalyzer"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public LocalCache(IDictionary cacheClasses, CacheBase parentCache, IDictionary properties, ICacheEventsListener listener, CacheRuntimeContext context, ActiveQueryAnalyzer activeQueryAnalyzer) : base(properties, parentCache, listener, context, activeQueryAnalyzer)
		{
			_evictionSyncMutex = new object();
			_allowExplicitGCCollection = true;
			Initialize(cacheClasses, properties);
		}

		/// <summary>
		/// Adiciona uma dependencia de sincronização para o item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="syncDependency">Instancia da dependencia de sincronização.</param>
		/// <returns>True caso a operação tenha sido executada com sucesso.</returns>
		internal override bool AddInternal(object key, CacheSyncDependency syncDependency)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry item = (CacheEntry)_cacheStore.Get(key);
			if(item == null)
				return false;
			item.SyncDependency = syncDependency;
			_cacheStore.Insert(key, item);
			item.LastModifiedTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Adiciona um <see cref="ExpirationHint"/> para o item associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item do cache.</param>
		/// <param name="expirationHint"><see cref="ExpirationHint"/> que será adicionado.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>True caso a operação tenha sido executada com sucesso.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal override bool AddInternal(object key, ExpirationHint expirationHint, OperationContext operationContext)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry item = (CacheEntry)_cacheStore.Get(key);
			if(item == null)
				return false;
			if(item.ExpirationHint == null)
				item.ExpirationHint = expirationHint;
			else if(item.ExpirationHint is AggregateExpirationHint)
				((AggregateExpirationHint)item.ExpirationHint).Add(expirationHint);
			else
			{
				AggregateExpirationHint hint = new AggregateExpirationHint();
				hint.Add(item.ExpirationHint);
				hint.Add(expirationHint);
				item.ExpirationHint = hint;
			}
			_cacheStore.Insert(key, item);
			item.LastModifiedTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Método interno acionado para adicionar um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada que está sendo adicionada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <returns>Resultado da operação.</returns>
		internal override CacheAddResult AddInternal(object key, CacheEntry cacheEntry, bool isUserOperation)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			if(_evictionPolicy != null)
			{
				if(cacheEntry.EvictionHint is PriorityEvictionHint)
					cacheEntry.Priority = ((PriorityEvictionHint)cacheEntry.EvictionHint).Priority;
				cacheEntry.EvictionHint = _evictionPolicy.CompatibleHint(cacheEntry.EvictionHint);
			}
			StoreAddResult result = _cacheStore.Add(key, cacheEntry);
			if((result == StoreAddResult.Success || result == StoreAddResult.SuccessNearEviction) && (_evictionPolicy != null))
				_evictionPolicy.Notify(key, null, cacheEntry.EvictionHint);
			switch(result)
			{
			case StoreAddResult.Success:
				return CacheAddResult.Success;
			case StoreAddResult.KeyExists:
				return CacheAddResult.KeyExists;
			case StoreAddResult.NotEnoughSpace:
				return CacheAddResult.NeedsEviction;
			case StoreAddResult.SuccessNearEviction:
				return CacheAddResult.SuccessNearEviction;
			}
			return CacheAddResult.Failure;
		}

		/// <summary>
		/// Verifica se o tamanho do cache pode ser alterado.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		internal override bool CanChangeCacheSize(long size)
		{
			return (_cacheStore.Size <= size);
		}

		/// <summary>
		/// Método interno usado para limpar os dados da instancia.
		/// </summary>
		internal override void ClearInternal()
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			_cacheStore.Clear();
			if(_evictionThread != null)
			{
				_evictionThread.Abort();
			}
			if(_evictionPolicy != null)
				_evictionPolicy.Clear();
		}

		/// <summary>
		/// Método interno que verifica se existe alguma entrada com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal override bool ContainsInternal(object key)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			return _cacheStore.Contains(key);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public override void Dispose()
		{
			if(_cacheStore != null)
			{
				_cacheStore.Dispose();
				_cacheStore = null;
			}
			base.Dispose();
		}

		/// <summary>
		/// Faz a liberação.
		/// </summary>
		/// <param name="cache"></param>
		private void DoEvict(CacheBase cache)
		{
			if(_evictionPolicy != null)
			{
				_evictionPolicy.Execute(cache, base._context, this.Size);
				if(_allowExplicitGCCollection)
				{
					GC.Collect();
				}
			}
		}

		/// <summary>
		/// Executa a liberação.
		/// </summary>
		public override void Evict()
		{
			if(_evictionPolicy != null)
			{
				lock (_evictionSyncMutex)
				{
					if(base._parentCache.IsEvictionAllowed)
					{
						if(base._allowAsyncEviction)
						{
							if(_evictionThread == null)
							{
								_evictionThread = new Thread(new ThreadStart(this.EvictAysnc));
								_evictionThread.IsBackground = true;
								_evictionThread.Start();
							}
						}
						else
						{
							this.DoEvict(this);
						}
					}
				}
			}
		}

		/// <summary>
		/// Método da execução da liberação assincrona
		/// </summary>
		private void EvictAysnc()
		{
			try
			{
				if(!base.IsSelfInternal)
					this.DoEvict(base._context.CacheImpl);
				else
					this.DoEvict(base._context.CacheInternal);
			}
			catch(ThreadAbortException)
			{
			}
			catch(ThreadInterruptedException)
			{
			}
			catch(Exception exception)
			{
				if(base._context != null)
					base._context.Logger.Error(exception.ToString().GetFormatter());
			}
			finally
			{
				lock (_evictionSyncMutex)
				{
					_evictionThread = null;
				}
			}
		}

		internal override CacheEntry GetEntryInternal(object key, bool isUserOperation)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry entry = (CacheEntry)_cacheStore.Get(key);
			if(entry != null)
			{
				EvictionHint evictionHint = entry.EvictionHint;
				if((isUserOperation && (_evictionPolicy != null)) && ((evictionHint != null) && evictionHint.IsVariant))
					_evictionPolicy.Notify(key, evictionHint, null);
			}
			return entry;
		}

		public override IDictionaryEnumerator GetEnumerator()
		{
			if(_cacheStore == null)
			{
				throw new InvalidOperationException();
			}
			return _cacheStore.GetEnumerator();
		}

		internal override CacheEntry GetInternal(object key, bool isUserOperation, OperationContext operationContext)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry entry = (CacheEntry)_cacheStore.Get(key);
			if(entry != null)
			{
				EvictionHint evictionHint = entry.EvictionHint;
				if((isUserOperation && (_evictionPolicy != null)) && ((evictionHint != null) && evictionHint.IsVariant))
					_evictionPolicy.Notify(key, evictionHint, null);
			}
			return entry;
		}

		/// <summary>
		/// Recupera o tamanho do item associado com a chave.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override int GetItemSize(object key)
		{
			if(_cacheStore == null)
				return 0;
			return _cacheStore.GetItemSize(key);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="properties"></param>
		protected override void Initialize(IDictionary cacheClasses, IDictionary properties)
		{
			properties.Require("properties").NotNull();
			try
			{
				base.Initialize(cacheClasses, properties);
				if(!properties.Contains("storage"))
					throw new Colosoft.Caching.Exceptions.ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingOption, "storage").Format());
				if(properties.Contains("scavenging-policy"))
				{
					IDictionary dictionary = properties["scavenging-policy"] as IDictionary;
					if((dictionary.Contains("eviction-enabled") && Convert.ToBoolean(dictionary["eviction-enabled"])) && (Convert.ToDouble(dictionary["evict-ratio"]) > 0.0))
						_evictionPolicy = EvictionPolicyFactory.CreateEvictionPolicy(dictionary);
				}
				else
					_evictionPolicy = EvictionPolicyFactory.CreateDefaultEvictionPolicy();
				IDictionary dictionary2 = properties["storage"] as IDictionary;
				_cacheStore = CacheStorageFactory.CreateStorageProvider(dictionary2, base._context.SerializationContext, _evictionPolicy != null, base._context.Logger);
			}
			catch(Colosoft.Caching.Exceptions.ConfigurationException exception)
			{
				if(base._context != null)
					base._context.Logger.Error("LocalCache.Initialize()".GetFormatter(), exception.GetFormatter());
				this.Dispose();
				throw;
			}
			catch(Exception exception2)
			{
				if(base._context != null)
				{
					base._context.Logger.Error("LocalCache.Initialize()".GetFormatter(), exception2.GetFormatter());
				}
				this.Dispose();
				throw new Colosoft.Caching.Exceptions.ConfigurationException("Configuration Error: " + exception2.ToString(), exception2);
			}
		}

		/// <summary>
		/// Método interno usado para inserir uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <param name="oldEntry">Valor da antiga entrada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Resulta da operação.</returns>
		internal override CacheInsResult InsertInternal(object key, CacheEntry cacheEntry, bool isUserOperation, CacheEntry oldEntry, OperationContext operationContext)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			if(cacheEntry.EvictionHint is PriorityEvictionHint)
				cacheEntry.Priority = ((PriorityEvictionHint)cacheEntry.EvictionHint).Priority;
			if(_evictionPolicy != null)
				cacheEntry.EvictionHint = _evictionPolicy.CompatibleHint(cacheEntry.EvictionHint);
			EvictionHint oldhint = (oldEntry == null) ? null : oldEntry.EvictionHint;
			StoreInsResult result = _cacheStore.Insert(key, cacheEntry);
			switch(result)
			{
			case StoreInsResult.Success:
			case StoreInsResult.SuccessNearEviction:
				if(_evictionPolicy != null)
					_evictionPolicy.Notify(key, null, cacheEntry.EvictionHint);
				break;
			case StoreInsResult.SuccessOverwrite:
			case StoreInsResult.SuccessOverwriteNearEviction:
				if(isUserOperation)
					cacheEntry.UpdateVersion(oldEntry);
				cacheEntry.UpdateLastModifiedTime(oldEntry);
				if(_evictionPolicy != null)
					_evictionPolicy.Notify(key, oldhint, cacheEntry.EvictionHint);
				break;
			}
			switch(result)
			{
			case StoreInsResult.Success:
				return CacheInsResult.Success;
			case StoreInsResult.SuccessOverwrite:
				return CacheInsResult.SuccessOverwrite;
			case StoreInsResult.SuccessNearEviction:
				return CacheInsResult.SuccessNearEvicition;
			case StoreInsResult.SuccessOverwriteNearEviction:
				return CacheInsResult.SuccessOverwriteNearEviction;
			case StoreInsResult.NotEnoughSpace:
				return CacheInsResult.NeedsEviction;
			}
			return CacheInsResult.Failure;
		}

		/// <summary>
		/// Remove o Hint de expiração associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="eh"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal override bool RemoveInternal(object key, ExpirationHint eh)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry item = (CacheEntry)_cacheStore.Get(key);
			if((item == null) || (item.ExpirationHint == null))
				return false;
			if(item.ExpirationHint is AggregateExpirationHint)
			{
				AggregateExpirationHint hint = new AggregateExpirationHint();
				AggregateExpirationHint expirationHint = (AggregateExpirationHint)item.ExpirationHint;
				foreach (ExpirationHint hint3 in expirationHint)
				{
					if(!hint3.Equals(eh))
						hint.Add(hint3);
				}
				item.ExpirationHint = hint;
			}
			else if(item.ExpirationHint.Equals(eh))
			{
				item.ExpirationHint = null;
			}
			_cacheStore.Insert(key, item);
			item.LastModifiedTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Remove a entrada associada com a chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada que será removida.</param>
		/// <param name="removalReason"></param>
		/// <param name="isUserOperation"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal override CacheEntry RemoveInternal(object key, ItemRemoveReason removalReason, bool isUserOperation, OperationContext operationContext)
		{
			if(_cacheStore == null)
				throw new InvalidOperationException();
			CacheEntry entry = (CacheEntry)_cacheStore.Remove(key);
			if((entry != null) && (_evictionPolicy != null))
				_evictionPolicy.Remove(key, entry.EvictionHint);
			return entry;
		}

		public override long Count
		{
			get
			{
				if(_cacheStore == null)
					throw new InvalidOperationException();
				return _cacheStore.Count;
			}
		}

		internal override float EvictRatio
		{
			get
			{
				if(_evictionPolicy != null)
				{
					return _evictionPolicy.EvictRatio;
				}
				return 0f;
			}
			set
			{
				if(_evictionPolicy != null)
				{
					_evictionPolicy.EvictRatio = value;
				}
			}
		}

		public override Array Keys
		{
			get
			{
				if(_cacheStore == null)
				{
					throw new InvalidOperationException();
				}
				return _cacheStore.Keys;
			}
		}

		internal override long MaxSize
		{
			get
			{
				if(_cacheStore != null)
				{
					return _cacheStore.MaxSize;
				}
				return 0;
			}
			set
			{
				if(_cacheStore != null)
				{
					if(_cacheStore.Size > value)
						throw new Exception("You need to remove some data from cache before applying the new size");
					_cacheStore.MaxSize = value;
				}
			}
		}

		internal override long Size
		{
			get
			{
				if(_cacheStore != null)
				{
					return _cacheStore.Size;
				}
				return 0;
			}
		}
	}
}
