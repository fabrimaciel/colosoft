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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Representa a configuração do cache.
	/// </summary>
	[Serializable, ConfigurationRoot("cache-config")]
	public class CacheConfig : ICloneable
	{
		private BackingSource _backingSource;

		private CacheLoader _cacheloader;

		private string _cacheType;

		private Compression _compression;

		private double _configID;

		private DataSharing _dataSharing;

		private CompactSerialization compactSerialization;

		private EvictionPolicy _evictionPolicy;

		private QueryIndex _indexes;

		private bool _inproc;

		private string _lastModified;

		private Log _log;

		private string _name;

		private PerfCounters _perfCounters;

		private Storage _storage;

		private Notifications _notifications;

		private Cleanup cleanup;

		/// <summary>
		/// Backsource.
		/// </summary>
		[ConfigurationSection("backing-source")]
		public BackingSource BackingSource
		{
			get
			{
				return _backingSource;
			}
			set
			{
				_backingSource = value;
			}
		}

		/// <summary>
		/// Configuração o CacheLoader.
		/// </summary>
		[ConfigurationSection("cache-loader")]
		public CacheLoader CacheLoader
		{
			get
			{
				return _cacheloader;
			}
			set
			{
				_cacheloader = value;
			}
		}

		/// <summary>
		/// Configuração de limpeza.
		/// </summary>
		[ConfigurationSection("cleanup")]
		public Cleanup Cleanup
		{
			get
			{
				return this.cleanup;
			}
			set
			{
				this.cleanup = value;
			}
		}

		/// <summary>
		/// Configuração da serialização compacta.
		/// </summary>
		[ConfigurationSection("compact-serialization")]
		public CompactSerialization CompactSerialization
		{
			get
			{
				return this.compactSerialization;
			}
			set
			{
				this.compactSerialization = value;
			}
		}

		/// <summary>
		/// Tipo do cache.
		/// </summary>
		[ConfigurationAttribute("type")]
		public string CacheType
		{
			get
			{
				string cacheType = this._cacheType;
				if(cacheType == null)
					cacheType = "local-cache";
				return cacheType;
			}
			set
			{
				this._cacheType = value;
			}
		}

		/// <summary>
		/// Configuração da compressão.
		/// </summary>
		[ConfigurationSection("compression")]
		public Compression Compression
		{
			get
			{
				return this._compression;
			}
			set
			{
				this._compression = value;
			}
		}

		/// <summary>
		/// Identificador da configuração.
		/// </summary>
		[ConfigurationAttribute("config-id")]
		public double ConfigID
		{
			get
			{
				return this._configID;
			}
			set
			{
				this._configID = value;
			}
		}

		/// <summary>
		/// Configuração do compartilhamento de dados.
		/// </summary>
		[ConfigurationSection("data-sharing")]
		public DataSharing DataSharing
		{
			get
			{
				return this._dataSharing;
			}
			set
			{
				this._dataSharing = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationSection("eviction-policy")]
		public EvictionPolicy EvictionPolicy
		{
			get
			{
				return this._evictionPolicy;
			}
			set
			{
				this._evictionPolicy = value;
			}
		}

		/// <summary>
		/// Identifica se o cache está executando como processo.
		/// </summary>
		[ConfigurationAttribute("inproc")]
		public bool InProc
		{
			get
			{
				return this._inproc;
			}
			set
			{
				this._inproc = value;
			}
		}

		/// <summary>
		/// Data da ultima modificação.
		/// </summary>
		[ConfigurationAttribute("last-modified")]
		public string LastModified
		{
			get
			{
				return this._lastModified;
			}
			set
			{
				this._lastModified = value;
			}
		}

		/// <summary>
		/// Configurações do log.
		/// </summary>
		[ConfigurationSection("log")]
		public Log Log
		{
			get
			{
				return this._log;
			}
			set
			{
				this._log = value;
			}
		}

		/// <summary>
		/// Nome do cache.
		/// </summary>
		[ConfigurationAttribute("name")]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		/// <summary>
		/// Configuração dos contadores.
		/// </summary>
		[ConfigurationSection("perf-counters")]
		public PerfCounters PerfCounters
		{
			get
			{
				return this._perfCounters;
			}
			set
			{
				this._perfCounters = value;
			}
		}

		/// <summary>
		/// Indices das consultas.
		/// </summary>
		[ConfigurationSection("indexes")]
		public QueryIndex QueryIndices
		{
			get
			{
				return this._indexes;
			}
			set
			{
				this._indexes = value;
			}
		}

		/// <summary>
		/// Configuração do armazenamento.
		/// </summary>
		[ConfigurationSection("storage")]
		public Storage Storage
		{
			get
			{
				return this._storage;
			}
			set
			{
				this._storage = value;
			}
		}

		/// <summary>
		/// Configuração das notificações.
		/// </summary>
		[ConfigurationSection("notifications")]
		public Notifications Notifications
		{
			get
			{
				return this._notifications;
			}
			set
			{
				this._notifications = value;
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			CacheConfig config = new CacheConfig();
			config.Name = (this.Name != null) ? ((string)this.Name.Clone()) : null;
			config._cacheType = this._cacheType;
			config.InProc = this.InProc;
			config.ConfigID = this.ConfigID;
			config.Cleanup = (this.Cleanup != null) ? ((Cleanup)this.Cleanup.Clone()) : null;
			config.LastModified = (this.LastModified != null) ? ((string)this.LastModified.Clone()) : null;
			config.Log = (this.Log != null) ? ((Log)this.Log.Clone()) : null;
			config.PerfCounters = (this.PerfCounters != null) ? ((PerfCounters)this.PerfCounters.Clone()) : null;
			config.Storage = (this.Storage != null) ? ((Storage)this.Storage.Clone()) : null;
			config.EvictionPolicy = (this.EvictionPolicy != null) ? ((EvictionPolicy)this.EvictionPolicy.Clone()) : null;
			config._backingSource = (this._backingSource != null) ? ((BackingSource)this._backingSource.Clone()) : null;
			config._cacheloader = (this._cacheloader != null) ? ((CacheLoader)this._cacheloader.Clone()) : null;
			config.Compression = (this.Compression != null) ? ((Compression)this.Compression.Clone()) : null;
			config.QueryIndices = (this.QueryIndices != null) ? ((QueryIndex)this.QueryIndices.Clone()) : null;
			config.DataSharing = (this.DataSharing != null) ? ((DataSharing)this.DataSharing.Clone()) : null;
			config.Notifications = (this.Notifications != null) ? ((Notifications)this.Notifications.Clone()) : null;
			config.CompactSerialization = (this.CompactSerialization != null) ? ((CompactSerialization)this.CompactSerialization.Clone()) : null;
			return config;
		}
	}
}
