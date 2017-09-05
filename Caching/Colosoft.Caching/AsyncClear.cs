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
	internal class AsyncClear : IAsyncTask
	{
		private Cache _cache;

		private CallbackEntry _cbEntry;

		private BitSet _flagMap;

		private OperationContext _operationContext;

		public AsyncClear(Cache cache, CallbackEntry cbEntry, BitSet flagMap, OperationContext operationContext)
		{
			_cache = cache;
			_cbEntry = cbEntry;
			_flagMap = flagMap;
			_operationContext = operationContext;
		}

		void IAsyncTask.Process()
		{
			object success = null;
			try
			{
				_cache.Clear(_flagMap, _cbEntry, _operationContext);
				success = AsyncOpResult.Success;
			}
			catch(Exception exception)
			{
				if(this.Logger != null)
				{
					this.Logger.Error("AsyncClear.Process()".GetFormatter(), exception.GetFormatter());
				}
				success = exception;
			}
			finally
			{
				if((_cbEntry != null) && (_cbEntry.AsyncOperationCompleteCallback != null))
				{
					object[] result = new object[3];
					result[1] = _cbEntry.AsyncOperationCompleteCallback;
					result[2] = success;
					_cache.OnAsyncOperationCompleted(AsyncOpCode.Clear, result);
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
