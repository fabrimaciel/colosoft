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
using Colosoft.Caching.Queries;
using Colosoft.Caching.Statistics;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Data;
using Colosoft.Caching.Configuration;

namespace Colosoft.Caching.Local
{
	internal class OverflowCache : LocalCacheBase
	{
		protected LocalCacheBase _primary;

		protected LocalCacheBase _secondary;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="parentCache"></param>
		/// <param name="properties"></param>
		/// <param name="listener"></param>
		/// <param name="context"></param>
		/// <param name="activeQueryAnalyzer"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public OverflowCache(IDictionary cacheClasses, CacheBase parentCache, IDictionary properties, ICacheEventsListener listener, CacheRuntimeContext context, ActiveQueryAnalyzer activeQueryAnalyzer) : base(properties, parentCache, listener, context, activeQueryAnalyzer)
		{
			this.Initialize(cacheClasses, properties);
		}

		internal override bool AddInternal(object key, ExpirationHint eh, OperationContext operationContext)
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			if(_primary.ContainsInternal(key))
			{
				return _primary.AddInternal(key, eh, operationContext);
			}
			return (_secondary.Contains(key, operationContext) && _secondary.AddInternal(key, eh, operationContext));
		}

		internal override CacheAddResult AddInternal(object key, CacheEntry cacheEntry, bool isUserOperation)
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			if(_secondary.ContainsInternal(key))
			{
				return CacheAddResult.KeyExists;
			}
			return _primary.AddInternal(key, cacheEntry, false);
		}

		internal override void ClearInternal()
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			_secondary.ClearInternal();
			_primary.ClearInternal();
		}

		internal override bool ContainsInternal(object key)
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			if(!_primary.ContainsInternal(key))
			{
				return _secondary.ContainsInternal(key);
			}
			return true;
		}

		protected virtual LocalCacheBase CreateLocalCache(CacheBase parentCache, IDictionary cacheClasses, IDictionary schemeProps)
		{
			return new LocalCache(cacheClasses, parentCache, schemeProps, null, base._context, base._activeQueryAnalyzer);
		}

		protected virtual LocalCacheBase CreateOverflowCache(IDictionary cacheClasses, IDictionary schemeProps)
		{
			return new OverflowCache(cacheClasses, this, schemeProps, null, base._context, base._activeQueryAnalyzer);
		}

		public override void Dispose()
		{
			if(_primary != null)
			{
				_primary.Dispose();
				_primary = null;
			}
			if(_secondary != null)
			{
				_secondary.Dispose();
				_secondary = null;
			}
			base.Dispose();
		}

		public override void Evict()
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			_primary.Evict();
		}

		public override IDictionaryEnumerator GetEnumerator()
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			return new AggregateEnumerator(new IDictionaryEnumerator[] {
				_primary.GetEnumerator(),
				_secondary.GetEnumerator()
			});
		}

		internal override CacheEntry GetInternal(object key, bool isUserOperation, OperationContext operationContext)
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			CacheEntry cacheEntry = _primary.GetInternal(key, isUserOperation, operationContext);
			if(cacheEntry == null)
			{
				cacheEntry = _secondary.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
				if(cacheEntry != null)
				{
					_primary.Add(key, cacheEntry, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
				}
			}
			return cacheEntry;
		}

		protected override void Initialize(IDictionary cacheClasses, IDictionary properties)
		{
			if(properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			try
			{
				base.Initialize(cacheClasses, properties);
				IDictionary schemeProps = ConfigHelper.GetCacheScheme(cacheClasses, properties, "primary-cache");
				string str = Convert.ToString(schemeProps["type"]).ToLower();
				if(str.CompareTo("local-cache") == 0)
				{
					_primary = this.CreateLocalCache(this, cacheClasses, schemeProps);
					_primary._allowAsyncEviction = false;
				}
				else
				{
					if(str.CompareTo("overflow-cache") != 0)
					{
						throw new Colosoft.Caching.Exceptions.ConfigurationException("invalid or non-local cache class specified in composite cache");
					}
					_primary = this.CreateOverflowCache(cacheClasses, schemeProps);
				}
				IDictionary dictionary2 = ConfigHelper.GetCacheScheme(cacheClasses, properties, "secondary-cache");
				string str2 = Convert.ToString(dictionary2["type"]).ToLower();
				if(str2.CompareTo("local-cache") == 0)
				{
					_secondary = this.CreateLocalCache(base._parentCache, cacheClasses, dictionary2);
					_secondary._allowAsyncEviction = true;
				}
				else
				{
					if(str2.CompareTo("overflow-cache") != 0)
					{
						throw new Colosoft.Caching.Exceptions.ConfigurationException("invalid or non-local cache class specified in composite cache");
					}
					_secondary = this.CreateOverflowCache(cacheClasses, dictionary2);
				}
				_primary.Listener = new PrimaryCacheListener(this);
				_secondary.Listener = new SecondaryCacheListener(this);
			}
			catch(Colosoft.Caching.Exceptions.ConfigurationException exception)
			{
				if(base._context != null)
				{
					base._context.Logger.Error("OverflowCache.Initialize()".GetFormatter(), exception.GetFormatter());
				}
				this.Dispose();
				throw;
			}
			catch(Exception exception2)
			{
				if(base._context != null)
				{
					base._context.Logger.Error("OverflowCache.Initialize()".GetFormatter(), exception2.GetFormatter());
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
			if((_primary == null) || (_secondary == null))
				throw new InvalidOperationException();
			if(_primary.ContainsInternal(key))
				return _primary.InsertInternal(key, cacheEntry, false, oldEntry, operationContext);
			if(_secondary.Contains(key, operationContext))
				return _secondary.InsertInternal(key, cacheEntry, false, oldEntry, operationContext);
			CacheAddResult result2 = this.AddInternal(key, cacheEntry, false);
			if(result2 != CacheAddResult.Success)
			{
				if(result2 == CacheAddResult.NeedsEviction)
					return CacheInsResult.NeedsEviction;
				return CacheInsResult.Failure;
			}
			return CacheInsResult.Success;
		}

		internal override CacheEntry RemoveInternal(object key, ItemRemoveReason removalReason, bool isUserOperation, OperationContext operationContext)
		{
			if((_primary == null) || (_secondary == null))
			{
				throw new InvalidOperationException();
			}
			CacheEntry entry = _primary.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
			if(entry == null)
			{
				entry = _secondary.RemoveInternal(key, ItemRemoveReason.Removed, false, operationContext);
			}
			return entry;
		}

		public override object RemoveSync(object[] keys, ItemRemoveReason reason, bool notify, OperationContext operationContext)
		{
			if(reason == ItemRemoveReason.Expired)
			{
				return base._context.CacheImpl.RemoveSync(keys, reason, notify, operationContext);
			}
			if(_primary != null)
			{
				for(int i = 0; i < keys.Length; i++)
				{
					CacheEntry cacheEntry = _primary.Remove(keys[i], reason, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
					if(cacheEntry != null)
					{
						if((reason == ItemRemoveReason.Underused) && (cacheEntry != null))
						{
							_secondary.Add(keys[i], cacheEntry, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
						}
						else
						{
							((IDisposable)cacheEntry).Dispose();
						}
					}
				}
			}
			return null;
		}

		public override long Count
		{
			get
			{
				if((_primary == null) || (_secondary == null))
				{
					throw new InvalidOperationException();
				}
				return (_primary.Count + _secondary.Count);
			}
		}

		public CacheBase Primary
		{
			get
			{
				return _primary;
			}
		}

		public CacheBase Secondary
		{
			get
			{
				return _secondary;
			}
		}

		internal override long Size
		{
			get
			{
				if((_primary == null) || (_secondary == null))
				{
					throw new InvalidOperationException();
				}
				long num = 0;
				num += _primary.Size;
				return (num + _secondary.Size);
			}
		}

		private class PrimaryCacheListener : ICacheEventsListener
		{
			private OverflowCache _parent;

			public PrimaryCacheListener(OverflowCache parent)
			{
				_parent = parent;
			}

			void ICacheEventsListener.OnCacheCleared()
			{
			}

			void ICacheEventsListener.OnCustomEvent(object notifId, object data)
			{
			}

			void ICacheEventsListener.OnItemAdded(object key)
			{
			}

			void ICacheEventsListener.OnItemRemoved(object key, object val, ItemRemoveReason reason, OperationContext operationContext)
			{
				if(reason == ItemRemoveReason.Underused)
				{
					_parent.Secondary.Add(key, (CacheEntry)val, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
				}
				else
				{
					((IDisposable)val).Dispose();
				}
			}

			void ICacheEventsListener.OnItemUpdated(object key, OperationContext operationContext)
			{
			}

			void ICacheEventsListener.OnWriteBehindOperationCompletedCallback(OpCode operationCode, object result, CallbackEntry cbEntry)
			{
			}

			public void OnCustomRemoveCallback(object key, object value, ItemRemoveReason reason)
			{
			}

			public void OnCustomUpdateCallback(object key, object value)
			{
			}

			public void OnItemsRemoved(object[] key, object[] val, ItemRemoveReason reason, OperationContext operationContext)
			{
				if(reason == ItemRemoveReason.Underused)
				{
					for(int i = 0; i < key.Length; i++)
					{
						_parent.Secondary.Add(key[i], (CacheEntry)val[i], false, operationContext);
					}
				}
				else
				{
					for(int j = 0; j < key.Length; j++)
					{
						((IDisposable)val[j]).Dispose();
					}
				}
			}
		}

		private class SecondaryCacheListener : ICacheEventsListener
		{
			private OverflowCache _parent;

			public SecondaryCacheListener(OverflowCache parent)
			{
				_parent = parent;
			}

			void ICacheEventsListener.OnCacheCleared()
			{
			}

			void ICacheEventsListener.OnCustomEvent(object notifId, object data)
			{
			}

			void ICacheEventsListener.OnItemAdded(object key)
			{
			}

			void ICacheEventsListener.OnItemRemoved(object key, object val, ItemRemoveReason reason, OperationContext operationContext)
			{
				if((reason == ItemRemoveReason.Underused) && (_parent.Listener != null))
				{
					_parent.Listener.OnItemRemoved(key, val, reason, operationContext);
				}
				((IDisposable)val).Dispose();
			}

			void ICacheEventsListener.OnItemUpdated(object key, OperationContext operationContext)
			{
			}

			void ICacheEventsListener.OnWriteBehindOperationCompletedCallback(OpCode operationCode, object result, CallbackEntry cbEntry)
			{
			}

			public void OnCustomRemoveCallback(object key, object value, ItemRemoveReason reason)
			{
			}

			public void OnCustomUpdateCallback(object key, object value)
			{
			}

			public void OnItemsRemoved(object[] key, object[] val, ItemRemoveReason reason, OperationContext operationContext)
			{
				if((reason == ItemRemoveReason.Underused) && (_parent.Listener != null))
				{
					for(int i = 0; i < key.Length; i++)
					{
						_parent.Listener.OnItemRemoved(key[i], val[i], reason, operationContext);
						((IDisposable)val[i]).Dispose();
					}
				}
			}
		}
	}
}
