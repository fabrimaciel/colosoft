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
using Colosoft.Caching.Storage.Util;
using System.Threading;
using System.IO;

namespace Colosoft.Caching.Storage
{
	internal class FileSystemStorageProvider : StorageProviderBase, IPersistentCacheStorage
	{
		private FileSystemStorage _internalStore;

		protected Hashtable _itemDict;

		private Timer _persistenceTimer;

		private int _stateChangeId;

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
		/// Construtor para classes filhas.
		/// </summary>
		protected FileSystemStorageProvider()
		{
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="evictionEnabled"></param>
		public FileSystemStorageProvider(IDictionary properties, bool evictionEnabled) : base(properties, evictionEnabled)
		{
			_itemDict = new Hashtable(base.DEFAULT_CAPACITY);
			this.Initialize(properties);
		}

		/// <summary>
		/// Reseta o estado alterado.
		/// </summary>
		private void ResetStateChanged()
		{
			_stateChangeId = 0;
		}

		/// <summary>
		/// Define o estado como alterado.
		/// </summary>
		private void SetStateChanged()
		{
			_stateChangeId++;
		}

		/// <summary>
		/// Método acionado pelo timer.
		/// </summary>
		/// <param name="state"></param>
		private void OnPersistStateTimer(object state)
		{
			_persistenceTimer.Change(-1, 0);
			Colosoft.Logging.Trace.Info("FileSystemStorageProvider.OnPersistStateTimer()".GetFormatter());
			if(_internalStore.DataFolder != null)
			{
				((IPersistentCacheStorage)this).SaveStorageState();
				int dueTime = Convert.ToInt32(state);
				_persistenceTimer.Change(dueTime, dueTime);
			}
		}

		/// <summary>
		/// Carrega o estado.
		/// </summary>
		void IPersistentCacheStorage.LoadStorageState()
		{
			try
			{
				string path = Path.Combine(_internalStore.RootDir, "__ncfs__.state");
				lock (_itemDict)
				{
					using (Stream stream = new FileStream(path, FileMode.Open))
					{
						using (BinaryReader reader = new BinaryReader(stream))
						{
							int num = reader.ReadInt32();
							_itemDict.Clear();
							if(num >= 1)
							{
								for(int i = 0; i < num; i++)
								{
									int count = reader.ReadInt32();
									if(count >= 0)
									{
										try
										{
											StoreItem item = StoreItem.FromBinary(reader.ReadBytes(count), base.CacheContext);
											if(_internalStore.Contains(item.Value))
												_itemDict.Add(item.Key, item.Value);
										}
										catch(Exception)
										{
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.LoadStorageState()".GetFormatter(), exception.GetFormatter());
			}
			finally
			{
				this.ResetStateChanged();
			}
		}

		/// <summary>
		/// Salva o estado.
		/// </summary>
		void IPersistentCacheStorage.SaveStorageState()
		{
			try
			{
				if(_stateChangeId != 0)
				{
					string path = Path.Combine(_internalStore.RootDir, "__cfs__.state");
					lock (_itemDict)
					{
						using (Stream stream = new FileStream(path, FileMode.Create))
						{
							var writer = new BinaryWriter(stream);
							writer.Write(_itemDict.Count);
							IDictionaryEnumerator enumerator = _itemDict.GetEnumerator();
							while (enumerator.MoveNext())
							{
								try
								{
									byte[] buffer = StoreItem.ToBinary(enumerator.Key, enumerator.Value, base.CacheContext);
									writer.Write(buffer.Length);
									writer.Write(buffer);
									continue;
								}
								catch(Exception)
								{
									writer.Write(-1);
									continue;
								}
							}
							writer.Flush();
						}
					}
				}
			}
			catch(Exception exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.SaveStorageState()".GetFormatter(), exception.GetFormatter());
			}
			finally
			{
				this.ResetStateChanged();
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties"></param>
		public void Initialize(IDictionary properties)
		{
			try
			{
				string rootDir = null;
				if(properties.Contains("root-dir"))
					rootDir = Convert.ToString(properties["root-dir"]);
				string dataFolder = null;
				if(properties.Contains("persistence-key"))
					dataFolder = Convert.ToString(properties["persistence-key"]);
				int num = 60000;
				if(properties.Contains("persistence-interval"))
				{
					num = Convert.ToInt32(properties["persistence-interval"]);
					num = Math.Max(1000, num);
				}
				_internalStore = new FileSystemStorage(rootDir, dataFolder);
				if(_internalStore.DataFolder != null)
				{
					((IPersistentCacheStorage)this).LoadStorageState();
					TimerCallback callback = new TimerCallback(this.OnPersistStateTimer);
					_persistenceTimer = new Timer(new TimerCallback(callback.Invoke), num, num, num);
				}
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
				lock (_itemDict)
				{
					object obj2 = _internalStore.Add(key, item, base.CacheContext);
					if(obj2 != null)
					{
						_itemDict.Add(key, obj2);
						this.SetStateChanged();
						base.Added(item as ISizable);
					}
				}
				if(status == StorageProviderBase.StoreStatus.NearEviction)
					return StoreAddResult.SuccessNearEviction;
			}
			catch(OutOfMemoryException exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.Add()".GetFormatter(), exception.GetFormatter());
				return StoreAddResult.NotEnoughSpace;
			}
			catch(Exception exception2)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.Add()".GetFormatter(), exception2.GetFormatter());
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
			try
			{
				object obj2 = this.Get(key);
				StorageProviderBase.StoreStatus status = base.HasSpace((ISizable)item);
				if(status == StorageProviderBase.StoreStatus.HasNotEnoughSpace)
					return StoreInsResult.NotEnoughSpace;
				lock (_itemDict)
				{
					object obj3 = _internalStore.Insert(_itemDict[key], item, base.CacheContext);
					if(obj3 == null)
						return StoreInsResult.Failure;
					_itemDict[key] = obj3;
					this.SetStateChanged();
					base.Inserted(obj2 as ISizable, item as ISizable);
				}
				if(status == StorageProviderBase.StoreStatus.NearEviction)
					return ((obj2 != null) ? StoreInsResult.SuccessOverwriteNearEviction : StoreInsResult.SuccessNearEviction);
				return ((obj2 != null) ? StoreInsResult.SuccessOverwrite : StoreInsResult.Success);
			}
			catch(OutOfMemoryException exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.Insert()".GetFormatter(), exception.GetFormatter());
				return StoreInsResult.NotEnoughSpace;
			}
			catch(Exception exception2)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.Insert()".GetFormatter(), exception2.GetFormatter());
				return StoreInsResult.Failure;
			}
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
				return _internalStore.Get(_itemDict[key], base.CacheContext);
			}
			catch(Exception exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.Get()".GetFormatter(), exception.GetFormatter());
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
			lock (_itemDict)
			{
				object obj2 = this.Get(key);
				if(obj2 != null)
				{
					_internalStore.Remove(_itemDict[key]);
					base.Removed(obj2 as ISizable);
					_itemDict.Remove(key);
					this.SetStateChanged();
				}
				return obj2;
			}
		}

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		public override void Clear()
		{
			lock (_itemDict)
			{
				_itemDict.Clear();
				_internalStore.Clear();
				base.Cleared();
				this.SetStateChanged();
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
		/// Recupera o enumerador para pecorrer os itens.
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
				return ((ISizable)_internalStore.Get(_itemDict[key], base.CacheContext)).Size;
			}
			catch(Exception exception)
			{
				Colosoft.Logging.Trace.Error("FileSystemStorageProvider.GetItemSize()".GetFormatter(), exception.GetFormatter());
				return 0;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(_persistenceTimer != null)
			{
				_persistenceTimer.Dispose();
				_persistenceTimer = null;
			}
			if(_internalStore.DataFolder != null)
				((IPersistentCacheStorage)this).SaveStorageState();
			_internalStore.Dispose();
			base.Dispose(disposing);
		}
	}
}
