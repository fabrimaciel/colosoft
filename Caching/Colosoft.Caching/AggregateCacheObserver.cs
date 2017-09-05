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

namespace Colosoft.Caching
{
	/// <summary>
	/// Agregador os observes do cache.
	/// </summary>
	public class AggregateCacheObserver : AggregateObserver<ICacheObserver>, ICacheObserver
	{
		/// <summary>
		/// Método acionado quando o cache for carregado.
		/// </summary>
		public void OnLoaded()
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnLoaded();
		}

		/// <summary>
		/// Método acionado quanod ocorre um erro na carga do cache.
		/// </summary>
		/// <param name="e"></param>
		public void OnLoadError(CacheErrorEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnLoadError(e);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		/// <param name="e"></param>
		public void OnLoadProcessingError(CacheErrorEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnLoadProcessingError(e);
		}

		/// <summary>
		/// Método acionado quando ocorreu um erro ao inserir uma entrada do cache.
		/// </summary>
		/// <param name="e"></param>
		public void OnInsertEntryError(CacheInsertEntryErrorEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnLoadProcessingError(e);
		}
	}
}
