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
using Colosoft.Logging;
using System.Threading;
using System.Collections;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Implementação base para um provedor de armazenamento.
	/// </summary>
	internal class StorageProviderBase : ICacheStorage, IDisposable
	{
		public const uint GB = 0x40000000;

		public const uint KB = 0x400;

		public const uint MB = 0x100000;

		private string _cacheContext;

		/// <summary>
		/// Tamanho dos dados.
		/// </summary>
		protected long _dataSize;

		/// <summary>
		/// Data do ultimo relatório.
		/// </summary>
		protected DateTime _lastReportedTime;

		/// <summary>
		/// Valor do intervalor dos relatórios.
		/// </summary>
		protected int _reportInterval;

		/// <summary>
		/// Instancia responsável pelo lock.
		/// </summary>
		protected ReaderWriterLock _syncObj;

		private long _extraDataSize;

		private long _maxCount;

		private long _maxSize;

		private ILogger _logger;

		/// <summary>
		/// Capacidade padrão.
		/// </summary>
		protected readonly int DEFAULT_CAPACITY;

		protected readonly double DEFAULT_EXTRA_ACCOMDATION_PERCENTAGE;

		/// <summary>
		/// Nome do contexto do cache.
		/// </summary>
		public string CacheContext
		{
			get
			{
				return _cacheContext;
			}
			set
			{
				_cacheContext = value;
			}
		}

		/// <summary>
		/// Quantidade de itens na instancia.
		/// </summary>
		public virtual long Count
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Chaves adicionadas na instancia.
		/// </summary>
		public virtual Array Keys
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Quantidade máxima de itens.
		/// </summary>
		public virtual long MaxCount
		{
			get
			{
				return _maxCount;
			}
			set
			{
				_maxCount = value;
			}
		}

		/// <summary>
		/// Tamanho máximo.
		/// </summary>
		public virtual long MaxSize
		{
			get
			{
				return _maxSize;
			}
			set
			{
				_maxSize = value;
			}
		}

		/// <summary>
		/// Instancia do <see cref="ILogger"/> associada.
		/// </summary>
		public ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Tamanho dos dados armazenados.
		/// </summary>
		public virtual long Size
		{
			get
			{
				return _dataSize;
			}
		}

		/// <summary>
		/// Objeto de sincronização.
		/// </summary>
		public ReaderWriterLock Sync
		{
			get
			{
				return _syncObj;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public StorageProviderBase() : this(0)
		{
		}

		/// <summary>
		/// Cria uma instancia já definindo o tamanho máximo do armazenamento.
		/// </summary>
		/// <param name="maxSize">Tamanho máximo.</param>
		public StorageProviderBase(long maxSize)
		{
			DEFAULT_CAPACITY = 0x61a8;
			DEFAULT_EXTRA_ACCOMDATION_PERCENTAGE = 0.20000000298023224;
			_reportInterval = 5;
			_lastReportedTime = DateTime.MinValue;
			_syncObj = new ReaderWriterLock();
			_maxSize = maxSize;
		}

		/// <summary>
		/// Cria a instancia já definindo as propriedades do armazenamento e se a liberação está abilitada.
		/// </summary>
		/// <param name="properties">Propriedades que serão atribuídas a instancia.</param>
		/// <param name="evictionEnabled"></param>
		public StorageProviderBase(IDictionary properties, bool evictionEnabled) : this(properties, evictionEnabled, null)
		{
		}

		/// <summary>
		/// Cria a instancia já definindo as propriedades do armazenamento e se a liberação está abilitada.
		/// </summary>
		/// <param name="properties">Propriedades que serão atribuídas a instancia.</param>
		/// <param name="evictionEnabled"></param>
		/// <param name="logger">Instacia do logger associado.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public StorageProviderBase(IDictionary properties, bool evictionEnabled, ILogger logger)
		{
			this.DEFAULT_CAPACITY = 25000;
			this.DEFAULT_EXTRA_ACCOMDATION_PERCENTAGE = 0.20000000298023224;
			_reportInterval = 5;
			_lastReportedTime = DateTime.MinValue;
			Initialize(properties, evictionEnabled);
			_logger = logger;
		}

		/// <summary>
		/// Verifica se existe espaço para armazenar o item informado.
		/// </summary>
		/// <param name="item">Item que será armazenado.</param>
		/// <returns></returns>
		protected StoreStatus HasSpace(ISizable item)
		{
			long num = _dataSize + item.Size;
			StoreStatus hasSpace = StoreStatus.HasSpace;
			if(SystemMemoryTask.PercentMemoryUsed > 90)
			{
				if(_extraDataSize > 0)
					hasSpace = StoreStatus.NearEviction;
				else
					hasSpace = StoreStatus.HasNotEnoughSpace;
			}
			if(num <= _maxSize)
				return hasSpace;
			if(num > (_maxSize + _extraDataSize))
				return StoreStatus.HasNotEnoughSpace;
			return StoreStatus.NearEviction;
		}

		/// <summary>
		/// Verifica se existe espaço para aramzenar o item informado substituindo um
		/// item já existente.
		/// </summary>
		/// <param name="oldItem">Item já existente.</param>
		/// <param name="newItem">Novo item que será adicionado.</param>
		/// <returns></returns>
		protected StoreStatus HasSpace(ISizable oldItem, ISizable newItem)
		{
			long num = (_dataSize + newItem.Size) - ((oldItem == null) ? ((long)0) : ((long)oldItem.Size));
			StoreStatus hasSpace = StoreStatus.HasSpace;
			try
			{
				if(SystemMemoryTask.PercentMemoryUsed <= 90)
				{
					if(num <= _maxSize)
						return hasSpace;
					if(num > (_maxSize + _extraDataSize))
						return StoreStatus.HasNotEnoughSpace;
					return StoreStatus.NearEviction;
				}
				if(_extraDataSize > 0)
				{
					return StoreStatus.NearEviction;
				}
				return StoreStatus.HasNotEnoughSpace;
			}
			catch(Exception)
			{
			}
			return StoreStatus.HasNotEnoughSpace;
		}

		/// <summary>
		/// Método acionado quando um item for inserido.
		/// </summary>
		/// <param name="oldItem">Item anterior.</param>
		/// <param name="newItem">Novo item.</param>
		protected void Inserted(ISizable oldItem, ISizable newItem)
		{
			_dataSize += newItem.Size - ((oldItem == null) ? 0 : oldItem.Size);
		}

		/// <summary>
		/// Método acionado quando um novo item for adicionado.
		/// </summary>
		/// <param name="item"></param>
		protected void Added(ISizable item)
		{
			_dataSize += item.Size;
		}

		/// <summary>
		/// Método acionado quando um item for removido.
		/// </summary>
		/// <param name="item"></param>
		protected void Removed(ISizable item)
		{
			_dataSize -= item.Size;
		}

		/// <summary>
		/// Método acionado quando a instancia for limpa.
		/// </summary>
		protected void Cleared()
		{
			_dataSize = 0;
		}

		/// <summary>
		/// Recupera o total de bytes para o total de megabytes informados.
		/// </summary>
		/// <param name="mbytes"></param>
		/// <returns></returns>
		private long ToBytes(long mbytes)
		{
			return ((mbytes * 0x400) * 0x400);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties">Propriedades associadas.</param>
		/// <param name="evictionEnabled"></param>
		public virtual void Initialize(IDictionary properties, bool evictionEnabled)
		{
			_syncObj = new ReaderWriterLock();
			if((properties != null) && properties.Contains("max-size"))
			{
				try
				{
					_maxSize = this.ToBytes(Convert.ToInt64(properties["max-size"]));
					_maxCount = Convert.ToInt64(properties["max-objects"]);
					if(evictionEnabled)
						_extraDataSize = (long)(_maxSize * this.DEFAULT_EXTRA_ACCOMDATION_PERCENTAGE);
				}
				catch(Exception)
				{
				}
			}
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public virtual StoreAddResult Add(object key, object item)
		{
			return StoreAddResult.Failure;
		}

		/// <summary>
		/// Insere um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		public virtual StoreInsResult Insert(object key, object item)
		{
			return StoreInsResult.Failure;
		}

		/// <summary>
		/// Recupera o item pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual object Get(object key)
		{
			return null;
		}

		/// <summary>
		/// Remove o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será removido.</param>
		/// <returns>Instancia do item removido.</returns>
		public virtual object Remove(object key)
		{
			return null;
		}

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		public virtual void Clear()
		{
		}

		/// <summary>
		/// Verifica existe algum item armazenado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <returns></returns>
		public virtual bool Contains(object key)
		{
			return false;
		}

		/// <summary>
		/// Recupera o enumerador para pecorrer os itens.
		/// </summary>
		/// <returns></returns>
		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return null;
		}

		/// <summary>
		/// Recupera o tamanho do item associado com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual int GetItemSize(object key)
		{
			return 0;
		}

		/// <summary>
		/// Recupera um provedor sincronizado.
		/// </summary>
		/// <param name="cacheStorage"></param>
		/// <returns></returns>
		public static StorageProviderBase Synchronized(StorageProviderBase cacheStorage)
		{
			return new StorageProviderSyncWrapper(cacheStorage);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.Collect();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_syncObj = null;
			Cleared();
		}

		/// <summary>
		/// Possíveis situações para armazenar.
		/// </summary>
		protected enum StoreStatus
		{
			/// <summary>
			/// Identifica que possue espaço.
			/// </summary>
			HasSpace,
			/// <summary>
			/// Será feito na próxima liberação.
			/// </summary>
			NearEviction,
			/// <summary>
			/// Não possue mais espaço.
			/// </summary>
			HasNotEnoughSpace
		}
	}
}
