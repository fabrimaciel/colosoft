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
using Colosoft.Caching.Loaders;
using Colosoft.Caching.Synchronization;
using Colosoft.Threading;
using Colosoft.Caching.Data;
using Colosoft.Caching.Expiration;
using Colosoft.Serialization;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa o contexto de tempo de execução do cache.
	/// </summary>
	internal class CacheRuntimeContext : IDisposable
	{
		public AsyncProcessor AsyncProc;

		public ExpirationManager ExpiryMgr;

		public TimeScheduler TimeSched;

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public virtual void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			lock (this)
			{
				if(this.SerializationContext != null)
					CompactFormatterServices.UnregisterAllCustomCompactTypes(this.SerializationContext);
				if(this.ExpiryMgr != null)
				{
					this.ExpiryMgr.Dispose();
					this.ExpiryMgr = null;
				}
				if(this.CacheImpl != null)
				{
					this.CacheImpl.Dispose();
					this.CacheImpl = null;
				}
				if(this.TimeSched != null)
				{
					this.TimeSched.Dispose();
					this.TimeSched = null;
				}
				if(this.AsyncProc != null)
				{
					this.AsyncProc.Stop();
					this.AsyncProc = null;
				}
				if(disposing)
					GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Implementação do cache.
		/// </summary>
		public CacheBase CacheImpl
		{
			get;
			set;
		}

		/// <summary>
		/// Implementação interna do cache.
		/// </summary>
		public CacheBase CacheInternal
		{
			get
			{
				return this.CacheImpl.InternalCache;
			}
		}

		/// <summary>
		/// Instancia principal do cache.
		/// </summary>
		public Cache CacheRoot
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a compressão está abilitada.
		/// </summary>
		public bool CompressionEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Limite da compressão.
		/// </summary>
		public long CompressionThreshold
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do loader do cache.
		/// </summary>
		public CacheStartupLoader CSLManager
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do gerenciado das fontes de dados.
		/// </summary>
		public DatasourceMgr DatasourceMgr
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsDbSyncCoordinator
		{
			get
			{
				if((((this.CacheRoot.CacheType != "partitioned-server") && (this.CacheRoot.CacheType != "local-cache")) && (this.CacheRoot.CacheType != "overflow-cache")) && ((((this.CacheRoot.CacheType != "replicated-server") && (this.CacheRoot.CacheType != "mirror-server"))) && (!(this.CacheRoot.CacheType == "partitioned-replicas-server"))))
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Instancia do logger que será usada no sistema.
		/// </summary>
		public ILogger Logger
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do contexto de serialização.
		/// </summary>
		public string SerializationContext
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do gerenciador de sincronização.
		/// </summary>
		internal CacheSyncManager SyncManager
		{
			get;
			set;
		}
	}
}
