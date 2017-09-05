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
using System.Threading;

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação da chave da entrada do cache com tempo.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	class TimedCacheKey<TKey> : IComparable<TKey>
	{
		private DateTime _expirationDate;

		private bool _slidingExpiration;

		private TimeSpan _slidingExpirationWindowSize;

		private TKey _key;

		/// <summary>
		/// Data que o item ira expirar.
		/// </summary>
		public DateTime ExpirationDate
		{
			get
			{
				return _expirationDate;
			}
		}

		/// <summary>
		/// Chave o item.
		/// </summary>
		public TKey Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Identifica se á para deslizar a expiração.
		/// </summary>
		public bool SlidingExpiration
		{
			get
			{
				return _slidingExpiration;
			}
		}

		/// <summary>
		/// Tamanho da janela para deslizar a expiração.
		/// </summary>
		public TimeSpan SlidingExpirationWindowSize
		{
			get
			{
				return _slidingExpirationWindowSize;
			}
		}

		/// <summary>
		/// Cria uma nova instancia com a data de expiração.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="expirationDate"></param>
		public TimedCacheKey(TKey key, DateTime expirationDate)
		{
			_key = key;
			_slidingExpiration = false;
			_expirationDate = expirationDate;
		}

		/// <summary>
		/// Cria uma nova instancia com o tempo de corte.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="slidingExpirationWindowSize"></param>
		public TimedCacheKey(TKey key, TimeSpan slidingExpirationWindowSize)
		{
			_key = key;
			_slidingExpiration = true;
			_slidingExpirationWindowSize = slidingExpirationWindowSize;
			Accessed();
		}

		/// <summary>
		/// Registra um acesso a instancia.
		/// </summary>
		public void Accessed()
		{
			if(_slidingExpiration)
				_expirationDate = DateTime.UtcNow.Add(_slidingExpirationWindowSize);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(TKey other)
		{
			return _key.GetHashCode().CompareTo(other.GetHashCode());
		}
	}
	/// <summary>
	/// Argumentos do evento acionado quando o item um item for removido da coleção.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class ExpiringListRemovedArgs<TKey> : EventArgs
	{
		private TKey _item;

		/// <summary>
		/// Item removido.
		/// </summary>
		public TKey Item
		{
			get
			{
				return _item;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="item"></param>
		public ExpiringListRemovedArgs(TKey item)
		{
			_item = item;
		}
	}
	/// <summary>
	/// Representa o evento acionado quando um item for removido da lista.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void ExpiringListRemovedHandler<TKey> (object sender, ExpiringListRemovedArgs<TKey> e);
	/// <summary>
	/// List that has an expiring built in
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public sealed class ExpiringList<TKey> : IDisposable
	{
		/// <summary>
		/// Para thread safety
		/// </summary>
		private object _syncRoot = new object();

		/// <summary>
		/// Para thread safety
		/// </summary>
		private object _isPurging = new object();

		/// <summary>
		/// Lista onde são armazenados os tempos gerais.
		/// </summary>
		private List<TimedCacheKey<TKey>> _timedStorage = new List<TimedCacheKey<TKey>>();

		private Dictionary<TKey, TimedCacheKey<TKey>> _timedStorageIndex;

		private System.Timers.Timer _timer = new System.Timers.Timer(TimeSpan.FromSeconds(1.0).TotalMilliseconds);

		private double _defaultTime = 0;

		/// <summary>
		/// Evento acionado quando um item for removido.
		/// </summary>
		public event ExpiringListRemovedHandler<TKey> Removed;

		/// <summary>
		/// Recupera a instancia de um item pelo indice informado.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public TKey this[int i]
		{
			get
			{
				TKey o;
				if(!Monitor.TryEnter(_syncRoot, 5000))
					throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
				try
				{
					if(_timedStorage.Count > i)
					{
						TimedCacheKey<TKey> tkey = _timedStorage[i];
						o = tkey.Key;
						_timedStorage.Remove(tkey);
						tkey.Accessed();
						_timedStorage.Insert(i, tkey);
						return o;
					}
					else
					{
						throw new ArgumentException("Key not found in the cache");
					}
				}
				finally
				{
					Monitor.Exit(_syncRoot);
				}
			}
			set
			{
				AddOrUpdate(value, _defaultTime);
			}
		}

		/// <summary>
		/// Quantidade de itens na instancia.
		/// </summary>
		public int Count
		{
			get
			{
				return _timedStorage.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ExpiringList()
		{
			_timedStorageIndex = new Dictionary<TKey, TimedCacheKey<TKey>>();
			_timer.Elapsed += PurgeCache;
			_timer.Start();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="comparer">Comparador da chave.</param>
		public ExpiringList(IEqualityComparer<TKey> comparer)
		{
			_timedStorageIndex = new Dictionary<TKey, TimedCacheKey<TKey>>(comparer);
			_timer.Elapsed += PurgeCache;
			_timer.Start();
		}

		/// <summary>
		/// Define o tempo padrão.
		/// </summary>
		/// <param name="time"></param>
		public void SetDefaultTime(double time)
		{
			_defaultTime = time;
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Instancia do item.</param>
		/// <param name="expirationSeconds">Tempo de vida em segundos.</param>
		/// <returns></returns>
		public bool Add(TKey key, double expirationSeconds)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
					return false;
				else
				{
					var internalKey = new TimedCacheKey<TKey>(key, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds));
					_timedStorage.Add(internalKey);
					_timedStorageIndex.Add(key, internalKey);
					return true;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Instancia do item.</param>
		/// <param name="slidingExpiration">Tempo para corte.</param>
		/// <returns></returns>
		public bool Add(TKey key, TimeSpan slidingExpiration)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
					return false;
				else
				{
					var internalKey = new TimedCacheKey<TKey>(key, slidingExpiration);
					_timedStorage.Add(internalKey);
					_timedStorageIndex.Add(key, internalKey);
					return true;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Adiciona ou atualiza o item.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="expirationSeconds">Tempo de vida em segundos.</param>
		/// <returns></returns>
		public bool AddOrUpdate(TKey key, double expirationSeconds)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(Contains(key))
				{
					Update(key, expirationSeconds);
					return false;
				}
				else
				{
					Add(key, expirationSeconds);
					return true;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Adicioan ou atualiza um item.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		public bool AddOrUpdate(TKey key, TimeSpan slidingExpiration)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(Contains(key))
				{
					Update(key, slidingExpiration);
					return false;
				}
				else
				{
					Add(key, slidingExpiration);
					return true;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				_timedStorage.Clear();
				_timedStorageIndex.Clear();
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Verifica se na coleção existe o item informado.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(TKey key)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				return _timedStorageIndex.ContainsKey(key);
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Verifica se na coleção existe o item com o predicado informado e retorna o primeiro que encontrar
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryFind(Predicate<TKey> predicate, out TKey value)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				for(var i = 0; i < _timedStorage.Count; i++)
				{
					var tKey = _timedStorage[i];
					if(predicate(tKey.Key))
					{
						_timedStorage.Remove(tKey);
						tKey.Accessed();
						_timedStorage.Insert(i, tKey);
						value = tKey.Key;
						return true;
					}
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
			value = default(TKey);
			return false;
		}

		/// <summary>
		/// Remove a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove(TKey key)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
				{
					_timedStorage.Remove(_timedStorageIndex[key]);
					_timedStorageIndex.Remove(key);
					OnRemoved(key);
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Atualiza o item.
		/// </summary>
		/// <param name="key">Instancia do item que será atualizado.</param>
		/// <returns></returns>
		public bool Update(TKey key)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
				{
					_timedStorage.Remove(_timedStorageIndex[key]);
					_timedStorageIndex[key].Accessed();
					_timedStorage.Add(_timedStorageIndex[key]);
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Atualiza o item.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="expirationSeconds"></param>
		/// <returns></returns>
		public bool Update(TKey key, double expirationSeconds)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
				{
					_timedStorage.Remove(_timedStorageIndex[key]);
					_timedStorageIndex.Remove(key);
				}
				else
				{
					return false;
				}
				TimedCacheKey<TKey> internalKey = new TimedCacheKey<TKey>(key, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds));
				_timedStorage.Add(internalKey);
				_timedStorageIndex.Add(key, internalKey);
				return true;
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Atualiza o item.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		public bool Update(TKey key, TimeSpan slidingExpiration)
		{
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				if(_timedStorageIndex.ContainsKey(key))
				{
					_timedStorage.Remove(_timedStorageIndex[key]);
					_timedStorageIndex.Remove(key);
				}
				else
				{
					return false;
				}
				TimedCacheKey<TKey> internalKey = new TimedCacheKey<TKey>(key, slidingExpiration);
				_timedStorage.Add(internalKey);
				_timedStorageIndex.Add(key, internalKey);
				return true;
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Copia os itens da coleção para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="startIndex"></param>
		public void CopyTo(Array array, int startIndex)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			if(startIndex < 0)
				throw new ArgumentOutOfRangeException("startIndex", "startIndex must be >= 0.");
			if(array.Rank > 1)
				throw new ArgumentException("array must be of Rank 1 (one-dimensional)", "array");
			if(startIndex >= array.Length)
				throw new ArgumentException("startIndex must be less than the length of the array.", "startIndex");
			if(Count > array.Length - startIndex)
				throw new ArgumentException("There is not enough space from startIndex to the end of the array to accomodate all items in the cache.");
			if(!Monitor.TryEnter(_syncRoot, 5000))
				throw new ApplicationException("Lock could not be acquired after " + 5000 + "ms");
			try
			{
				foreach (object o in _timedStorage)
				{
					array.SetValue(o, startIndex);
					startIndex++;
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}

		/// <summary>
		/// Expurga os objetos expirados a partir do cache. Chamado automaticamente pelo temporizador.
		/// </summary>
		private void PurgeCache(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!Monitor.TryEnter(_isPurging))
				return;
			DateTime signalTime = DateTime.UtcNow;
			try
			{
				if(!Monitor.TryEnter(_syncRoot, 5000))
					return;
				try
				{
					Lazy<List<object>> expiredItems = new Lazy<List<object>>();
					foreach (TimedCacheKey<TKey> timedKey in _timedStorage)
					{
						if(timedKey.ExpirationDate < signalTime)
						{
							expiredItems.Value.Add(timedKey.Key);
						}
					}
					if(expiredItems.IsValueCreated)
					{
						foreach (TKey key in expiredItems.Value)
						{
							TimedCacheKey<TKey> timedKey = _timedStorageIndex[key];
							_timedStorageIndex.Remove(timedKey.Key);
							_timedStorage.Remove(timedKey);
							OnRemoved(key);
						}
					}
				}
				finally
				{
					Monitor.Exit(_syncRoot);
				}
			}
			finally
			{
				Monitor.Exit(_isPurging);
			}
		}

		/// <summary>
		/// Método acionado quando um item for removido.
		/// </summary>
		/// <param name="item"></param>
		private void OnRemoved(TKey item)
		{
			if(Removed != null)
				Removed(this, new ExpiringListRemovedArgs<TKey>(item));
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			_timer.Dispose();
			foreach (var i in _timedStorage)
				OnRemoved(i.Key);
			_timedStorage.Clear();
			_timedStorageIndex.Clear();
		}
	}
}
