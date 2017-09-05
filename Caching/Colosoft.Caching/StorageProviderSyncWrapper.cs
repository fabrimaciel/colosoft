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

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Implementação de uma classe de adaptação.
	/// </summary>
	internal class StorageProviderSyncWrapper : StorageProviderBase
	{
		protected StorageProviderBase _storage;

		/// <summary>
		/// Quantidade de itens na instancia.
		/// </summary>
		public override long Count
		{
			get
			{
				long count;
				base.Sync.AcquireReaderLock(-1);
				try
				{
					count = _storage.Count;
				}
				finally
				{
					base.Sync.ReleaseReaderLock();
				}
				return count;
			}
		}

		/// <summary>
		/// Chaves da instancia.
		/// </summary>
		public override Array Keys
		{
			get
			{
				Array keys;
				base.Sync.AcquireWriterLock(-1);
				try
				{
					keys = _storage.Keys;
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
				return keys;
			}
		}

		/// <summary>
		/// Tamanho máximo.
		/// </summary>
		public override long MaxSize
		{
			get
			{
				return _storage.MaxSize;
			}
			set
			{
				_storage.MaxSize = value;
			}
		}

		/// <summary>
		/// Tamanho da instancia.
		/// </summary>
		public override long Size
		{
			get
			{
				long size;
				base.Sync.AcquireWriterLock(-1);
				try
				{
					size = _storage.Size;
				}
				finally
				{
					base.Sync.ReleaseWriterLock();
				}
				return size;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="storageProvider">Instancia que será adaptada.</param>
		public StorageProviderSyncWrapper(StorageProviderBase storageProvider)
		{
			storageProvider.Require("storageProvider").NotNull();
			_storage = storageProvider;
			base._syncObj = _storage.Sync;
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public override StoreAddResult Add(object key, object item)
		{
			StoreAddResult result;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				result = _storage.Add(key, item);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return result;
		}

		/// <summary>
		/// Insere um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public override StoreInsResult Insert(object key, object item)
		{
			StoreInsResult result;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				result = _storage.Insert(key, item);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return result;
		}

		/// <summary>
		/// Recupera o item pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override object Get(object key)
		{
			object obj2;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				obj2 = _storage.Get(key);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return obj2;
		}

		/// <summary>
		/// Remove o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será removido.</param>
		/// <returns>Instancia do item removido.</returns>
		public override object Remove(object key)
		{
			object obj2;
			base.Sync.AcquireWriterLock(-1);
			try
			{
				obj2 = _storage.Remove(key);
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
			return obj2;
		}

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		public override void Clear()
		{
			base.Sync.AcquireWriterLock(-1);
			try
			{
				_storage.Clear();
			}
			finally
			{
				base.Sync.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Verifica existe algum item armazenado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <returns></returns>
		public override bool Contains(object key)
		{
			bool flag;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				flag = _storage.Contains(key);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return flag;
		}

		/// <summary>
		/// Recupera o enumerador para pecorrer os itens.
		/// </summary>
		/// <returns></returns>
		public override IDictionaryEnumerator GetEnumerator()
		{
			return _storage.GetEnumerator();
		}

		/// <summary>
		/// Recupera o tamanho do item associado com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override int GetItemSize(object key)
		{
			int itemSize;
			base.Sync.AcquireReaderLock(-1);
			try
			{
				itemSize = _storage.GetItemSize(key);
			}
			finally
			{
				base.Sync.ReleaseReaderLock();
			}
			return itemSize;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(_storage != null)
			{
				_storage.Dispose();
				_storage = null;
			}
			base.Dispose(disposing);
		}
	}
}
