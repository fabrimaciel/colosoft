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
	internal class AsyncRemove : IAsyncTask
	{
		private Cache _cache;

		private object _key;

		private OperationContext _operationContext;

		public AsyncRemove(Cache cache, object key, OperationContext operationContext)
		{
			_cache = cache;
			_key = key;
			_operationContext = operationContext;
		}

		void IAsyncTask.Process()
		{
			object success = null;
			BitSet flag = new BitSet();
			CallbackEntry cbEntry = null;
			string providerName = null;
			try
			{
				if(_key is object[])
				{
					object[] objArray = (object[])_key;
					_key = objArray[0];
					flag = objArray[1] as BitSet;
					if(objArray.Length > 2)
					{
						cbEntry = objArray[2] as CallbackEntry;
					}
					if(objArray.Length == 4)
					{
						providerName = objArray[3] as string;
					}
				}
				_cache.Remove(_key as string, flag, cbEntry, null, 0, LockAccessType.IGNORE_LOCK, providerName, _operationContext);
				success = AsyncOpResult.Success;
			}
			catch(Exception exception)
			{
				if(this.Logger != null)
				{
					this.Logger.Error("AsyncRemove.Process()".GetFormatter(), exception.GetFormatter());
				}
				success = exception;
			}
			finally
			{
				if((cbEntry != null) && (cbEntry.AsyncOperationCompleteCallback != null))
				{
					_cache.OnAsyncOperationCompleted(AsyncOpCode.Remove, new object[] {
						_key,
						cbEntry.AsyncOperationCompleteCallback,
						success
					});
				}
			}
		}

		private ILogger Logger
		{
			get
			{
				return _cache.Logger;
			}
		}
	}
}
