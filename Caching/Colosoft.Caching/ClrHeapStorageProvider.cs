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
using Colosoft.Logging;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Implementação do provedor de armazenamento utilizando a
	/// memória gerenciada pelo próprio CLR.NET.
	/// </summary>
	internal class ClrHeapStorageProvider : StorageProviderBase
	{
		/// <summary>
		/// Dicionário onde os itens serão armazenados.
		/// </summary>
		protected Hashtable _itemDict;

		/// <summary>
		/// Quantidade de itens na instancia.
		/// </summary>
		public override long Count
		{
			get
			{
				return (long)_itemDict.Count;
			}
		}

		/// <summary>
		/// Chaves da instancia.
		/// </summary>
		public override Array Keys
		{
			get
			{
				lock (_itemDict.SyncRoot)
				{
					Array array = Array.CreateInstance(typeof(object), _itemDict.Keys.Count);
					_itemDict.Keys.CopyTo(array, 0);
					return array;
				}
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ClrHeapStorageProvider()
		{
			_itemDict = Hashtable.Synchronized(new Hashtable(base.DEFAULT_CAPACITY, 0.7f));
		}

		/// <summary>
		/// Cria uma instanci já definindo o tamanho máximo dos dados.
		/// </summary>
		/// <param name="maxDataSize"></param>
		public ClrHeapStorageProvider(long maxDataSize) : base(maxDataSize)
		{
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY, 0.7f);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="evictionEnabled"></param>
		/// <param name="logger"></param>
		public ClrHeapStorageProvider(IDictionary properties, bool evictionEnabled, ILogger logger) : base(properties, evictionEnabled, logger)
		{
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY, 0.7f);
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
				lock (_itemDict.SyncRoot)
				{
					_itemDict.Add(key, item);
					base.Added(item as ISizable);
				}
				if(status == StorageProviderBase.StoreStatus.NearEviction)
					return StoreAddResult.SuccessNearEviction;
			}
			catch(OutOfMemoryException)
			{
				return StoreAddResult.NotEnoughSpace;
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
				object obj2 = _itemDict[key];
				StorageProviderBase.StoreStatus status = base.HasSpace(obj2 as ISizable, (ISizable)item);
				if(status == StorageProviderBase.StoreStatus.HasNotEnoughSpace)
					return StoreInsResult.NotEnoughSpace;
				lock (_itemDict.SyncRoot)
				{
					_itemDict[key] = item;
					base.Inserted(obj2 as ISizable, item as ISizable);
				}
				if(status == StorageProviderBase.StoreStatus.NearEviction)
					return ((obj2 != null) ? StoreInsResult.SuccessOverwriteNearEviction : StoreInsResult.SuccessNearEviction);
				notEnoughSpace = (obj2 != null) ? StoreInsResult.SuccessOverwrite : StoreInsResult.Success;
			}
			catch(OutOfMemoryException)
			{
				notEnoughSpace = StoreInsResult.NotEnoughSpace;
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
				return _itemDict[key];
			}
			catch(Exception exception)
			{
				Trace.Error("ClrHeapStorageProvider.Get()".GetFormatter(), exception.GetFormatter());
				return null;
			}
		}

		/// <summary>
		/// Remove o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será removido.</param>
		/// <returns>Instancia do item removido.</returns>
		public override object Remove(object key)
		{
			object obj2 = this.Get(key);
			if(obj2 != null)
			{
				lock (_itemDict.SyncRoot)
				{
					_itemDict.Remove(key);
					base.Removed(obj2 as ISizable);
				}
			}
			return obj2;
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
		/// Limpa os dados armazenados.
		/// </summary>
		public override void Clear()
		{
			lock (_itemDict.SyncRoot)
			{
				_itemDict.Clear();
				base.Cleared();
			}
		}

		/// <summary>
		/// Recupera o enumerador para pecorrer os itens.
		/// </summary>
		/// <returns></returns>
		public override IDictionaryEnumerator GetEnumerator()
		{
			return _itemDict.GetEnumerator();
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
				ISizable sizable = _itemDict[key] as ISizable;
				return ((sizable != null) ? sizable.Size : 0);
			}
			catch(Exception exception)
			{
				Trace.Error("ClrHeapStorageProvider.GetItemSize()".GetFormatter(), exception.GetFormatter());
				return 0;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			_itemDict.Clear();
			_itemDict = null;
			base.Dispose(disposing);
		}
	}
}
