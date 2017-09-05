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
using Colosoft.Threading;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Gerenciador de expiração.
	/// </summary>
	internal class ExpirationManager : IDisposable
	{
		private bool _allowExplicitGCCollection = true;

		private bool _cacheCleared;

		private bool _cacheLastAccessCountEnabled;

		private bool _cacheLastAccessCountLoggingEnabled;

		private int _cacheLastAccessInterval;

		private int _cacheLastAccessLoggingInterval = 20;

		private int _cacheLastAccessLoggingIntervalPassed;

		private int _cleanInterval = 30000;

		private float _cleanRatio = 1f;

		private CacheRuntimeContext _context;

		private static object _DATA = new object();

		private bool _indexCleared;

		private bool _inProgress;

		private Hashtable _mainIndex = new Hashtable(25000, 0.7f);

		private ILogger _logger;

		private int _removeThreshhold = 10;

		private ulong _runCount;

		private int _sleepInterval;

		private object _status_mutex = new object();

		private AutoExpirationTask _taskExpiry;

		private Cache _topLevelCache;

		private Hashtable _transitoryIndex = new Hashtable(25000, 0.7f);

		/// <summary>
		/// Intervalo de log do último acesso.
		/// </summary>
		private int CacheLastAccessCountInterval
		{
			get
			{
				return _cacheLastAccessInterval;
			}
		}

		/// <summary>
		/// Intervalo de log do ultimo acesso.
		/// </summary>
		private int CacheLastAccessLoggingInterval
		{
			get
			{
				return _cacheLastAccessLoggingInterval;
			}
		}

		/// <summary>
		/// Intervalo de limpeza do gerenciador.
		/// </summary>
		public long CleanInterval
		{
			get
			{
				return _taskExpiry.Interval;
			}
			set
			{
				_taskExpiry.Interval = value;
			}
		}

		/// <summary>
		/// Identifica se a quantidade de ultimos acessos está ativo.
		/// </summary>
		private bool IsCacheLastAccessCountEnabled
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se o log de ultimo acesso está ativo.
		/// </summary>
		private bool IsCacheLastAccessLoggingEnabled
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se instancia já foi liberada.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				if(_taskExpiry != null)
					return _taskExpiry.IsCancelled;
				return true;
			}
		}

		/// <summary>
		/// Identifica se o gerenciador está em progresso.
		/// </summary>
		public bool IsInProgress
		{
			get
			{
				lock (_status_mutex)
					return _inProgress;
			}
			set
			{
				lock (_status_mutex)
					_inProgress = value;
			}
		}

		private ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		internal Cache TopLevelCache
		{
			get
			{
				return _topLevelCache;
			}
			set
			{
				_topLevelCache = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties">Propriedades de configuração.</param>
		/// <param name="context">Contexto de execução.</param>
		public ExpirationManager(IDictionary properties, CacheRuntimeContext context)
		{
			_context = context;
			_logger = context.Logger;
			this.Initialize(properties);
			_sleepInterval = ServiceConfiguration.ExpirationBulkRemoveDelay;
			_removeThreshhold = ServiceConfiguration.ExpirationBulkRemoveSize;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties"></param>
		private void Initialize(IDictionary properties)
		{
			if(properties == null)
				throw new ArgumentNullException("properties");
			if(properties.Contains("clean-interval"))
				_cleanInterval = Convert.ToInt32(properties["clean-interval"]) * 1000;
			_cacheLastAccessCountEnabled = this.IsCacheLastAccessCountEnabled;
			_cacheLastAccessCountLoggingEnabled = this.IsCacheLastAccessLoggingEnabled;
			_cacheLastAccessInterval = this.CacheLastAccessCountInterval;
			_cacheLastAccessLoggingInterval = this.CacheLastAccessLoggingInterval;
			if(properties.Contains("allow-explicit-GCcollection"))
				_allowExplicitGCCollection = (bool)properties["allow-explicit-GCcollection"];
		}

		/// <summary>
		/// Aplica o logs do gerenciador.
		/// </summary>
		private void ApplyLoggs()
		{
			lock (_status_mutex)
			{
				this.IsInProgress = false;
				if(_indexCleared)
				{
					_mainIndex.Clear();
					_indexCleared = false;
				}
				var enumerator = _transitoryIndex.GetEnumerator();
				while (enumerator.MoveNext())
				{
					object key = enumerator.Key;
					ExpiryIndexEntry entry = enumerator.Value as ExpiryIndexEntry;
					if(entry != null)
						_mainIndex[key] = entry;
					else
						_mainIndex.Remove(key);
				}
			}
		}

		/// <summary>
		/// Inicia o login
		/// </summary>
		private void StartLogging()
		{
			this.IsInProgress = true;
		}

		/// <summary>
		/// Limpa os dados do gerenciador.
		/// </summary>
		public void Clear()
		{
			lock (this)
			{
				_cacheCleared = true;
			}
			lock (_status_mutex)
			{
				if(!this.IsInProgress)
					_mainIndex.Clear();
				else
				{
					_transitoryIndex.Clear();
					_indexCleared = true;
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public virtual void Dispose()
		{
			if(_taskExpiry != null)
			{
				_taskExpiry.Cancel();
				_taskExpiry = null;
			}
			lock (_status_mutex)
			{
				_mainIndex.Clear();
				_mainIndex = null;
				_transitoryIndex.Clear();
				_transitoryIndex = null;
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Valida os dados que devem expirar.
		/// </summary>
		/// <returns></returns>
		public bool Expire()
		{
			bool flag = false;
			lock (this)
				_runCount++;
			_sleepInterval = Convert.ToInt32(ServiceConfiguration.ExpirationBulkRemoveDelay);
			_removeThreshhold = Convert.ToInt32(ServiceConfiguration.ExpirationBulkRemoveSize);
			CacheBase cacheImpl = _context.CacheImpl;
			CacheBase cacheInternal = _context.CacheInternal;
			Cache cacheRoot = _context.CacheRoot;
			if(cacheInternal == null)
				throw new InvalidOperationException("No cache instance defined");
			ArrayList list = new ArrayList();
			new ArrayList();
			int num = 0;
			Hashtable hashtable = null;
			try
			{
				this.StartLogging();
				DateTime now = DateTime.Now;
				int diffSeconds = CachingUtils.DiffSeconds(now);
				int num3 = (int)Math.Ceiling((double)(cacheInternal.Count * _cleanRatio));
				if(_cacheLastAccessLoggingIntervalPassed >= _cacheLastAccessLoggingInterval)
				{
					_cacheLastAccessLoggingInterval = this.CacheLastAccessLoggingInterval;
					_cacheLastAccessCountEnabled = this.IsCacheLastAccessCountEnabled;
					_cacheLastAccessCountLoggingEnabled = this.IsCacheLastAccessLoggingEnabled;
					_cacheLastAccessInterval = this.CacheLastAccessCountInterval;
				}
				else
					_cacheLastAccessLoggingIntervalPassed++;
				if((_cacheLastAccessCountEnabled && _cacheLastAccessCountLoggingEnabled) && (_cacheLastAccessLoggingIntervalPassed >= _cacheLastAccessLoggingInterval))
				{
					_cacheLastAccessLoggingIntervalPassed = 0;
					hashtable = new Hashtable();
				}
				lock (_mainIndex.SyncRoot)
				{
					IDictionaryEnumerator enumerator2 = _mainIndex.GetEnumerator();
					if(enumerator2 != null)
					{
						while (enumerator2.MoveNext())
						{
							var entry2 = enumerator2.Value as ExpiryIndexEntry;
							var exh = entry2.Hint;
							if(((exh != null) && _cacheLastAccessCountEnabled) && (exh is IdleExpiration))
							{
								IdleExpiration expiration = exh as IdleExpiration;
								TimeSpan span = (TimeSpan)(CachingUtils.GetDateTime(CachingUtils.DiffSeconds(DateTime.Now)) - CachingUtils.GetDateTime(expiration.LastAccessTime));
								if(span.TotalMinutes >= _cacheLastAccessInterval)
								{
									num++;
									if(hashtable != null)
										hashtable.Add(enumerator2.Key, null);
								}
							}
							if((((exh != null) && (exh.SortKey.CompareTo(diffSeconds) < 0)) && !exh.IsRoutable) && exh.DetermineExpiration(_context))
							{
								if(exh.NeedsReSync && (_context.DatasourceMgr != null))
								{
									CacheEntry entry3 = cacheInternal.Get(enumerator2.Key, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
									if(entry3 != null)
										_context.DatasourceMgr.ResyncCacheItemAsync(enumerator2.Key, exh, null, entry3.GroupInfo, entry3.QueryInfo, entry3.ResyncProviderName);
									else
										_context.DatasourceMgr.ResyncCacheItemAsync(enumerator2.Key, exh, null, null, null, entry3.ResyncProviderName);
								}
								else
									list.Add(enumerator2.Key);
								if((num3 > 0) && (list.Count == num3))
									break;
							}
						}
					}
				}
			}
			catch(Exception exception)
			{
				this.Logger.Error(("LocalCache(Expire): " + exception.ToString()).GetFormatter());
			}
			finally
			{
				this.ApplyLoggs();
				ArrayList list4 = new ArrayList();
				ArrayList c = null;
				DateTime time1 = DateTime.Now;
				try
				{
					if(list.Count > 0)
					{
						var list6 = new ArrayList();
						for(int i = 0; (i < list.Count) && !_cacheCleared; i++)
						{
							list6.Add(list[i]);
							if((list6.Count % _removeThreshhold) == 0)
							{
								try
								{
									if(this.IsDisposed)
										break;
									c = cacheInternal.RemoveSync(list6.ToArray(), ItemRemoveReason.Expired, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
									flag = true;
								}
								catch(Exception exception2)
								{
									this.Logger.Error(("an error occured while removing expired items. Error " + exception2.ToString()).GetFormatter());
								}
								list6.Clear();
								if((c != null) && (c.Count > 0))
									list4.AddRange(c);
								System.Threading.Thread.Sleep((int)(_sleepInterval * 1000));
							}
						}
						if(!this.IsDisposed && (list6.Count > 0))
						{
							try
							{
								c = cacheInternal.RemoveSync(list6.ToArray(), ItemRemoveReason.Expired, false, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation)) as ArrayList;
								flag = true;
								if((c != null) && (c.Count > 0))
									list4.AddRange(c);
							}
							catch(Exception exception3)
							{
								this.Logger.Error(("an error occured while removing expired items. Error " + exception3.ToString()).GetFormatter());
							}
						}
					}
					if(!this.IsDisposed && (list4.Count > 0))
					{
						ArrayList list7 = new ArrayList();
						if(cacheRoot != null)
						{
							foreach (object obj2 in list4)
							{
								if(obj2 != null)
								{
									list7.Add(obj2);
									if((list7.Count % 100) == 0)
									{
										try
										{
											if(this.IsDisposed)
												break;
											cacheRoot.CascadedRemove(list7.ToArray(), ItemRemoveReason.Expired, true, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
										}
										catch(Exception exception4)
										{
											this.Logger.Error(("an error occured while removing dependent items. Error " + exception4.ToString()).GetFormatter());
										}
										list7.Clear();
									}
								}
							}
							if(!this.IsDisposed && (list7.Count > 0))
							{
								try
								{
									cacheRoot.CascadedRemove(list7.ToArray(), ItemRemoveReason.Expired, true, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
								}
								catch(Exception exception5)
								{
									this.Logger.Error(("an error occured while removing dependent items. Error " + exception5.ToString()).GetFormatter());
								}
								list7.Clear();
							}
						}
					}
				}
				finally
				{
					_transitoryIndex.Clear();
					lock (this)
					{
						_cacheCleared = false;
					}
					if(hashtable != null)
					{
						StringBuilder builder = new StringBuilder();
						IDictionaryEnumerator enumerator3 = hashtable.GetEnumerator();
						int num5 = 1;
						while (enumerator3.MoveNext())
						{
							builder.Append(enumerator3.Key + ", ");
							if((num5 % 10) == 0)
							{
								builder.Append("\r\n");
								num5 = 1;
							}
							else
								num5++;
						}
						this.Logger.Info(builder.ToString().Trim().GetFormatter());
					}
				}
			}
			return flag;
		}

		/// <summary>
		/// Remove o item com a chave informada do indice.
		/// </summary>
		/// <param name="key"></param>
		public void RemoveFromIndex(object key)
		{
			lock (_status_mutex)
			{
				if(!this.IsInProgress)
					_mainIndex.Remove(key);
				else
					_transitoryIndex[key] = null;
			}
		}

		/// <summary>
		/// Reseta os dados do Hint.
		/// </summary>
		/// <param name="oldHint"></param>
		/// <param name="newHint"></param>
		public void ResetHint(ExpirationHint oldHint, ExpirationHint newHint)
		{
			lock (this)
			{
				if(newHint != null)
				{
					if(oldHint != null)
						((IDisposable)oldHint).Dispose();
					newHint.Reset(_context);
				}
			}
		}

		/// <summary>
		/// Reseta o hint.
		/// </summary>
		/// <param name="hint"></param>
		public void ResetVariant(ExpirationHint hint)
		{
			lock (this)
			{
				hint.ResetVariant(_context);
			}
		}

		/// <summary>
		/// Inicia o gerenciador.
		/// </summary>
		public void Start()
		{
			if(_taskExpiry == null)
			{
				_taskExpiry = new AutoExpirationTask(this, (long)_cleanInterval);
				_context.TimeSched.AddTask(_taskExpiry);
			}
		}

		/// <summary>
		/// Para o gerenciador.
		/// </summary>
		public void Stop()
		{
			if(_taskExpiry != null)
				_taskExpiry.Cancel();
		}

		/// <summary>
		/// Atualiza a entrada no gerenciador.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="entry">Instancia da entrada.</param>
		public void UpdateIndex(object key, CacheEntry entry)
		{
			if((entry != null) && (entry.ExpirationHint != null))
			{
				bool hasDependentKeys = (entry.KeysDependingOnMe != null) && (entry.KeysDependingOnMe.Count > 0);
				this.UpdateIndex(key, entry.ExpirationHint, hasDependentKeys);
			}
		}

		public void UpdateIndex(object key, ExpirationHint hint, bool hasDependentKeys)
		{
			if((key != null) && (hint != null))
			{
				lock (_status_mutex)
				{
					if(!this.IsInProgress)
					{
						if(!_mainIndex.Contains(key))
							_mainIndex[key] = new ExpiryIndexEntry(hint, hasDependentKeys);
						else
						{
							ExpiryIndexEntry entry = _mainIndex[key] as ExpiryIndexEntry;
							if(entry != null)
							{
								entry.Hint = hint;
								entry.HasDependentKeys = hasDependentKeys;
							}
						}
					}
					else if(_transitoryIndex[key] == null)
						_transitoryIndex[key] = new ExpiryIndexEntry(hint, hasDependentKeys);
					else
					{
						ExpiryIndexEntry entry2 = _transitoryIndex[key] as ExpiryIndexEntry;
						if(entry2 != null)
						{
							entry2.Hint = hint;
							entry2.HasDependentKeys = hasDependentKeys;
						}
					}
				}
			}
		}

		/// <summary>
		/// Implementação da tarefa de auto expiração.
		/// </summary>
		private class AutoExpirationTask : TimeScheduler.Task
		{
			private long _interval = 1000;

			private ExpirationManager _parent;

			/// <summary>
			/// Intervalo.
			/// </summary>
			public long Interval
			{
				get
				{
					lock (this)
						return _interval;
				}
				set
				{
					lock (this)
						_interval = value;
				}
			}

			/// <summary>
			/// Identifica se a tarefa foi cancelada.
			/// </summary>
			public bool IsCancelled
			{
				get
				{
					return (_parent == null);
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="interval"></param>
			internal AutoExpirationTask(ExpirationManager parent, long interval)
			{
				_parent = parent;
				_interval = interval;
			}

			long TimeScheduler.Task.GetNextInterval()
			{
				lock (this)
				{
					return _interval;
				}
			}

			bool TimeScheduler.Task.IsCancelled()
			{
				lock (this)
				{
					return (_parent == null);
				}
			}

			void TimeScheduler.Task.Run()
			{
				if(_parent != null)
				{
					try
					{
						if(_parent != null && _parent.Expire() && _parent._allowExplicitGCCollection)
							GC.Collect();
					}
					catch(Exception)
					{
					}
				}
			}

			public void Cancel()
			{
				lock (this)
					_parent = null;
			}
		}

		/// <summary>
		/// Implementação da entrada do indice.
		/// </summary>
		private class ExpiryIndexEntry
		{
			private bool _hasDependentKeys;

			private ExpirationHint _hint;

			/// <summary>
			/// Identifica se possui chaves de dependencia.
			/// </summary>
			public bool HasDependentKeys
			{
				get
				{
					return _hasDependentKeys;
				}
				set
				{
					_hasDependentKeys = value;
				}
			}

			/// <summary>
			/// <see cref="ExpirationHint"/> associado.
			/// </summary>
			public ExpirationHint Hint
			{
				get
				{
					return _hint;
				}
				set
				{
					_hint = value;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="hint"></param>
			public ExpiryIndexEntry(ExpirationHint hint) : this(hint, false)
			{
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="hint"></param>
			/// <param name="hasDependentKeys"></param>
			public ExpiryIndexEntry(ExpirationHint hint, bool hasDependentKeys)
			{
				_hint = hint;
				_hasDependentKeys = hasDependentKeys;
			}

			/// <summary>
			/// Verifica se a instancia já expirou.
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public bool IsExpired(CacheRuntimeContext context)
			{
				return ((_hint != null) && _hint.DetermineExpiration(context));
			}
		}
	}
}
