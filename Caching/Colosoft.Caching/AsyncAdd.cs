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
using Colosoft.Logging;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Synchronization;
using Colosoft.Caching.Policies;
using System.Collections;

namespace Colosoft.Caching.Threading
{
	/// <summary>
	/// Representa a tarefa assincrono para adiciona uma nova entrada no cache.
	/// </summary>
	internal class AsyncAdd : IAsyncTask
	{
		private Cache _cache;

		private EvictionHint _evictionHint;

		private ExpirationHint _expiryHint;

		private BitSet _flag;

		private string _group;

		private object _key;

		private OperationContext _operationContext;

		private Hashtable _queryInfo;

		private string _subGroup;

		private CacheSyncDependency _syncDependency;

		private object _value;

		/// <summary>
		/// Logger que será usado.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _cache.Logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache">Instancia do cache onde a entrada será adicionada.</param>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <param name="expiryHint">Hint de expiração.</param>
		/// <param name="syncDependency">Dependencia de sincronização.</param>
		/// <param name="evictionHint">Hint de liberação</param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="Flag"></param>
		/// <param name="queryInfo"></param>
		/// <param name="operationContext"></param>
		public AsyncAdd(Cache cache, object key, object value, ExpirationHint expiryHint, CacheSyncDependency syncDependency, EvictionHint evictionHint, string group, string subGroup, BitSet Flag, Hashtable queryInfo, OperationContext operationContext)
		{
			_cache = cache;
			_key = key;
			_value = value;
			_expiryHint = expiryHint;
			_syncDependency = syncDependency;
			_evictionHint = evictionHint;
			_group = group;
			_subGroup = subGroup;
			_flag = Flag;
			_queryInfo = queryInfo;
			_operationContext = operationContext;
		}

		/// <summary>
		/// Método de execução da tarefa.
		/// </summary>
		void IAsyncTask.Process()
		{
			object success = null;
			try
			{
				_cache.Add(_key, _value, _expiryHint, _syncDependency, _evictionHint, _group, _subGroup, _queryInfo, _flag, null, null, _operationContext);
				success = AsyncOpResult.Success;
			}
			catch(Exception exception)
			{
				if(this.Logger != null)
					this.Logger.Error("AsyncAdd.Process()".GetFormatter(), exception.GetFormatter());
				success = exception;
			}
			finally
			{
				CallbackEntry entry = _value as CallbackEntry;
				if((entry != null) && (entry.AsyncOperationCompleteCallback != null))
					_cache.OnAsyncOperationCompleted(AsyncOpCode.Add, new object[] {
						_key,
						entry.AsyncOperationCompleteCallback,
						success
					});
			}
		}
	}
}
