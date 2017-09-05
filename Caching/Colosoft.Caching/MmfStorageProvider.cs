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
using System.Collections;
using Colosoft.Caching.Storage.Mmf;
using Colosoft.Caching.Storage.Util;
using Colosoft.Logging;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Implementação do provedor de armazenamento usando MMF.
	/// </summary>
	internal class MmfStorageProvider : StorageProviderBase, IPersistentCacheStorage
	{
		private string _fileName;

		private uint _initialSizeMB;

		private MmfStorage _internalStore;

		protected Hashtable _itemDict;

		private uint _viewCount;

		private uint _viewSize;

		/// <summary>
		/// Quantidade de itens armazenados.
		/// </summary>
		public override long Count
		{
			get
			{
				return (long)_itemDict.Count;
			}
		}

		static MmfStorageProvider()
		{
			CompactFormatterServices.RegisterCompactType(typeof(StoreItem), 80);
		}

		/// <summary>
		/// Construtor usado por classes filhas.
		/// </summary>
		protected MmfStorageProvider()
		{
			_viewCount = 8;
			_viewSize = 0x400000;
			_initialSizeMB = 0x20;
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY);
			_internalStore = new MmfStorage();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties">Propriedades que serão usadas pela instancia.</param>
		/// <param name="evictinEnabled">Identifica se a liberação está ativa.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public MmfStorageProvider(IDictionary properties, bool evictinEnabled)
		{
			_viewCount = 8;
			_viewSize = 0x400000;
			_initialSizeMB = 0x20;
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY);
			_internalStore = new MmfStorage();
			this.Initialize(properties, evictinEnabled);
		}

		/// <summary>
		/// Carrega o estado do armazenamento.
		/// </summary>
		void IPersistentCacheStorage.LoadStorageState()
		{
			try
			{
				lock (_itemDict.SyncRoot)
				{
					_itemDict.Clear();
					IEnumerator enumerator = _internalStore.GetEnumerator();
					while (enumerator.MoveNext())
					{
						MemArea current = (MemArea)enumerator.Current;
						if(!current.IsFree)
						{
							StoreItem item = StoreItem.FromBinary(current.GetMemContents(), base.CacheContext);
							_itemDict.Add(item.Key, new MmfObjectPtr(current.View, current));
						}
					}
				}
			}
			catch(Exception exception)
			{
				Trace.Error("MmfStorageProvider.IPersistentCacheStorage()".GetFormatter(), exception.GetFormatter());
			}
		}

		/// <summary>
		/// Salva o estado.
		/// </summary>
		void IPersistentCacheStorage.SaveStorageState()
		{
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="evictinEnabled"></param>
		public override void Initialize(IDictionary properties, bool evictinEnabled)
		{
			if(properties == null)
				throw new ArgumentNullException("properties");
			try
			{
				if(properties.Contains("file-name"))
					_fileName = Convert.ToString(properties["file-name"]);
				if(properties.Contains("num-views"))
					_viewCount = Convert.ToUInt32(properties["num-views"]);
				if(properties.Contains("view-size"))
					_viewSize = Convert.ToUInt32(properties["view-size"]);
				if(properties.Contains("initial-size-mb"))
					_initialSizeMB = Convert.ToUInt32(properties["initial-size-mb"]);
				_internalStore.OpenMemoryMappedStore(_fileName, _viewCount, _viewSize, _initialSizeMB);
				if(!_internalStore.IsPageFileStore)
					((IPersistentCacheStorage)this).LoadStorageState();
				base.Initialize(properties, evictinEnabled);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public override StoreAddResult Add(object key, object item)
		{
			try
			{
				if(_itemDict.ContainsKey(key))
					return StoreAddResult.KeyExists;
				StorageProviderBase.StoreStatus status = base.HasSpace((ISizable)item);
				if(status == StorageProviderBase.StoreStatus.HasNotEnoughSpace)
					return StoreAddResult.NotEnoughSpace;
				byte[] buffer = StoreItem.ToBinary(key, item, base.CacheContext);
				lock (_itemDict.SyncRoot)
				{
					MmfObjectPtr ptr = _internalStore.Add(buffer);
					if(ptr == null)
						return StoreAddResult.NotEnoughSpace;
					ptr.View.ParentStorageProvider = this;
					_itemDict.Add(key, ptr);
					base.Added(item as ISizable);
				}
				if(status == StorageProviderBase.StoreStatus.NearEviction)
					return StoreAddResult.SuccessNearEviction;
			}
			catch(OutOfMemoryException exception)
			{
				Trace.Error("OutofMemoryException::MmfStorageProvider.Add()".GetFormatter(), exception.GetFormatter());
				return StoreAddResult.NotEnoughSpace;
			}
			catch(Exception exception2)
			{
				Trace.Error("General Exception::MmfStorageProvider.Add()".GetFormatter(), exception2.GetFormatter());
				return StoreAddResult.Failure;
			}
			return StoreAddResult.Success;
		}

		/// <summary>
		/// Insere um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public override StoreInsResult Insert(object key, object item)
		{
			StoreInsResult notEnoughSpace;
			try
			{
				MmfObjectPtr info = (MmfObjectPtr)_itemDict[key];
				object obj2 = null;
				if(info == null)
				{
					switch(this.Add(key, item))
					{
					case StoreAddResult.NotEnoughSpace:
						return StoreInsResult.NotEnoughSpace;
					case StoreAddResult.Failure:
						return StoreInsResult.Failure;
					}
					return StoreInsResult.Success;
				}
				obj2 = this.Get(key);
				StorageProviderBase.StoreStatus status = base.HasSpace(obj2 as ISizable, (ISizable)item);
				if(status == StorageProviderBase.StoreStatus.HasNotEnoughSpace)
					notEnoughSpace = StoreInsResult.NotEnoughSpace;
				else
				{
					byte[] buffer = StoreItem.ToBinary(key, item, base.CacheContext);
					lock (_itemDict.SyncRoot)
					{
						MmfObjectPtr ptr2 = _internalStore.Insert(info, buffer);
						if(ptr2 == null)
							return StoreInsResult.NotEnoughSpace;
						if(ptr2.Area != info.Area)
						{
							_itemDict[key] = ptr2;
							_internalStore.Remove(info);
						}
						base.Inserted(obj2 as ISizable, item as ISizable);
						if(status == StorageProviderBase.StoreStatus.NearEviction)
							return ((obj2 != null) ? StoreInsResult.SuccessOverwriteNearEviction : StoreInsResult.SuccessNearEviction);
						notEnoughSpace = (ptr2 != null) ? StoreInsResult.SuccessOverwrite : StoreInsResult.Success;
					}
				}
			}
			catch(OutOfMemoryException exception)
			{
				Trace.Error("MmfStorageProvider.Insert()".GetFormatter(), exception.GetFormatter());
				notEnoughSpace = StoreInsResult.NotEnoughSpace;
			}
			catch(Exception exception2)
			{
				Trace.Error("MmfStorageProvider.Insert()".GetFormatter(), exception2.GetFormatter());
				notEnoughSpace = StoreInsResult.Failure;
			}
			return notEnoughSpace;
		}

		/// <summary>
		/// Recupera o item pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override object Get(object key)
		{
			try
			{
				MmfObjectPtr info = (MmfObjectPtr)_itemDict[key];
				if(info != null)
				{
					return StoreItem.FromBinary(_internalStore.Get(info), base.CacheContext).Value;
				}
			}
			catch(Exception exception)
			{
				Trace.Error("MmfStorageProvider.Get()".GetFormatter(), exception.GetFormatter());
			}
			return null;
		}

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		public override void Clear()
		{
			lock (_itemDict.SyncRoot)
			{
				_itemDict.Clear();
				_internalStore.Clear();
				base.Cleared();
			}
		}

		/// <summary>
		/// Verifica existe algum item armazenado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <returns></returns>
		public override bool Contains(object key)
		{
			return _itemDict.ContainsKey(key);
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		public override IDictionaryEnumerator GetEnumerator()
		{
			return new LazyStoreEnumerator(this, _itemDict.GetEnumerator());
		}

		/// <summary>
		/// Recupera o tamanho do item associado com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override int GetItemSize(object key)
		{
			try
			{
				MmfObjectPtr info = (MmfObjectPtr)_itemDict[key];
				if(info != null)
					return ((ISizable)StoreItem.FromBinary(_internalStore.Get(info), base.CacheContext).Value).Size;
			}
			catch(Exception exception)
			{
				Trace.Error("MmfStorageProvider.GetItemSize()".GetFormatter(), exception.GetFormatter());
			}
			return 0;
		}

		/// <summary>
		/// Recupera a <see cref="MemArea"/> associada com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public MemArea GetMemArea(object key)
		{
			MmfObjectPtr ptr = (MmfObjectPtr)_itemDict[key];
			return ptr.Area;
		}

		/// <summary>
		/// Remove o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será removido.</param>
		/// <returns>Instancia do item removido.</returns>
		public override object Remove(object key)
		{
			try
			{
				lock (_itemDict.SyncRoot)
				{
					MmfObjectPtr info = (MmfObjectPtr)_itemDict[key];
					if(info != null)
					{
						StoreItem item = StoreItem.FromBinary(_internalStore.Remove(info), base.CacheContext);
						_itemDict.Remove(key);
						base.Removed(item.Value as ISizable);
						return item.Value;
					}
				}
			}
			catch(Exception exception)
			{
				Trace.Error("MmfStorageProvider.Remove()".GetFormatter(), exception.GetFormatter());
			}
			return null;
		}

		/// <summary>
		/// Define uma <see cref="MemArea"/> para a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="areaTmp"></param>
		public void SetMemArea(object key, MemArea areaTmp)
		{
			_itemDict[key] = areaTmp;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(!_internalStore.IsPageFileStore)
				((IPersistentCacheStorage)this).SaveStorageState();
			_internalStore.Dispose();
			base.Dispose(disposing);
		}
	}
}
