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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Colosoft.Data.Caching.Remote.Server
{
	/// <summary>
	/// Implementação do serviço <see cref="IDataEntryDownloaderService"/>.
	/// </summary>
	public class DataEntryDownloaderService : IDataEntryDownloaderService
	{
		private Lazy<Colosoft.Data.Caching.Server.Business.ICacheFlow> _cacheFlow;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DataEntryDownloaderService()
		{
			_cacheFlow = new Lazy<Caching.Server.Business.ICacheFlow>(() => Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Caching.Server.Business.ICacheFlow>());
		}

		/// <summary>
		/// Recupera a entradas de dados do cache.
		/// </summary>
		/// <param name="request">Versões que serão comparadas.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public DataEntryDownloaderContentInfo GetDataEntries(GetDataEntriesRequest request)
		{
			var versions2 = request.Versions.Select(f => new Caching.Server.Business.CacheEntryVersion {
				TypeName = f.TypeName.FullName + ", " + f.TypeName.AssemblyName.Name,
				Version = f.Version
			});
			CacheEntriesEnumerator enumerator = null;
			var tempFile = System.IO.Path.GetTempFileName();
			var outStream = System.IO.File.Open(tempFile, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
			try
			{
				using (enumerator = new CacheEntriesEnumerator(_cacheFlow.Value.GetCacheEntries(versions2).GetEnumerator()))
				{
					Caching.DataEntryPackage.BuildPackage(outStream, enumerator);
				}
			}
			catch
			{
				outStream.Dispose();
				System.IO.File.Delete(tempFile);
				throw;
			}
			outStream.Position = 0;
			var result = new DataEntryDownloaderContentInfo {
				Name = "Data",
				Length = outStream.Length,
				Data = outStream
			};
			result.Disposed += (sender, e) =>  {
				System.IO.File.Delete(tempFile);
			};
			return result;
		}

		/// <summary>
		/// Enumerator para pecorrer as entradas de dados.
		/// </summary>
		class CacheEntriesEnumerator : IEnumerator<Tuple<DataEntryVersion, System.IO.Stream>>
		{
			private IEnumerator<Caching.Server.Models.CacheEntry> _entries;

			private Tuple<DataEntryVersion, System.IO.Stream> _current;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entries"></param>
			public CacheEntriesEnumerator(IEnumerator<Caching.Server.Models.CacheEntry> entries)
			{
				_entries = entries;
			}

			/// <summary>
			/// Recupera a atual instancia.
			/// </summary>
			public Tuple<DataEntryVersion, System.IO.Stream> Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_current != null)
				{
					_current.Item2.Dispose();
					_current = null;
				}
			}

			/// <summary>
			/// Instancia da atual entrada.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Move para  próxima entrada.
			/// </summary>
			/// <returns></returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
			public bool MoveNext()
			{
				if(_current != null)
					_current.Item2.Dispose();
				if(_entries.MoveNext())
				{
					var entry = _entries.Current;
					var version = new DataEntryVersion {
						TypeName = entry.TypeName != null ? new Reflection.TypeName(entry.TypeName) : null,
						Version = entry.Version
					};
					_current = new Tuple<DataEntryVersion, System.IO.Stream>(version, new System.IO.MemoryStream(entry.Data));
					return true;
				}
				else
				{
					if(_current != null)
					{
						_current.Item2.Dispose();
						_current = null;
					}
				}
				return false;
			}

			public void Reset()
			{
				if(_current != null)
				{
					_current.Item2.Dispose();
					_current = null;
				}
				_entries.Reset();
			}
		}
	}
}
