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
using Colosoft.Threading;

namespace Colosoft.Caching.Loaders
{
	/// <summary>
	/// Representa um tarefa de carga do cache.
	/// </summary>
	public class LoadCacheTask : ThreadClass, IDisposable
	{
		private CacheStartupLoader _cacheLoader;

		/// <summary>
		/// Evento acionado quando ocorre um erro no carga.
		/// </summary>
		public event CacheErrorEventHandler LoadError;

		/// <summary>
		/// Evento acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		public event CacheErrorEventHandler LoadProcessingError {
			add
			{
				_cacheLoader.LoadProcessingError += value;
			}
			remove {
				_cacheLoader.LoadProcessingError -= value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheLoader"></param>
		public LoadCacheTask(CacheStartupLoader cacheLoader)
		{
			cacheLoader.Require("cacheLoader").NotNull();
			_cacheLoader = cacheLoader;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			((IDisposable)_cacheLoader).Dispose();
		}

		/// <summary>
		/// Executa a tarefa.
		/// </summary>
		public override void Run()
		{
			try
			{
				try
				{
					_cacheLoader.Initialize();
				}
				catch(Exception)
				{
					throw;
				}
				_cacheLoader.LoadCache();
			}
			catch(ThreadAbortException)
			{
			}
			catch(ThreadInterruptedException)
			{
			}
			catch(Exception ex)
			{
				if(LoadError != null)
					LoadError(this, new CacheErrorEventArgs(ex));
			}
		}
	}
}
