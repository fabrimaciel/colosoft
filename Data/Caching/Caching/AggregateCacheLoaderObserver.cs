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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Implementação de uma aggregador de observers.
	/// </summary>
	public class AggregateCacheLoaderObserver : Colosoft.Net.AggregateDownloaderObserver, ICacheLoaderObserver, IDataCacheLoaderObserver
	{
		private Queue<ICacheLoaderObserver> _observers = new Queue<ICacheLoaderObserver>();

		/// <summary>
		/// Método acioando quando o processo for finalizado.
		/// </summary>
		/// <param name="e"></param>
		public void OnLoadComplete(ApplicationLoaderCompletedEventArgs e)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.OnLoadComplete(e);
		}

		/// <summary>
		/// Define o progresso total.
		/// </summary>
		/// <param name="e"></param>
		public void OnTotalProgressChanged(System.ComponentModel.ProgressChangedEventArgs e)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.OnTotalProgressChanged(e);
		}

		/// <summary>
		/// Define o progresso do atual estágio.
		/// </summary>
		/// <param name="e"></param>
		public void OnCurrentProgressChanged(System.ComponentModel.ProgressChangedEventArgs e)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.OnCurrentProgressChanged(e);
		}

		/// <summary>
		/// Acionado quando o estágio for alterado.
		/// </summary>
		/// <param name="state"></param>
		public void OnStageChanged(CacheLoaderStage state)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.OnStageChanged(state);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro na carga do cache.
		/// </summary>
		/// <param name="e"></param>
		public void OnLoadError(CacheLoaderErrorEventArgs e)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.OnLoadError(e);
		}

		/// <summary>
		/// Identifica o inicio da carga do <see cref="Colosoft.Data.Schema.ITypeMetadata"/>
		/// informado.
		/// </summary>
		/// <param name="metadata"></param>
		public void OnBeginLoadTypeMetadata(Colosoft.Data.Schema.ITypeMetadata metadata)
		{
			lock (_observers)
				foreach (var i in _observers)
					if(i is IDataCacheLoaderObserver)
						((IDataCacheLoaderObserver)i).OnBeginLoadTypeMetadata(metadata);
		}

		/// <summary>
		/// Identifica o fim da carga dos dados
		/// </summary>
		/// <param name="metadata"></param>
		/// <param name="exception"></param>
		public void OnEndLoadTypeMetadata(Colosoft.Data.Schema.ITypeMetadata metadata, Exception exception)
		{
			lock (_observers)
				foreach (var i in _observers)
					if(i is IDataCacheLoaderObserver)
						((IDataCacheLoaderObserver)i).OnEndLoadTypeMetadata(metadata, exception);
		}

		/// <summary>
		/// Adiciona mais um novo observer para o agregador.
		/// </summary>
		/// <param name="aggregate"></param>
		/// <param name="observer"></param>
		public static AggregateCacheLoaderObserver operator +(AggregateCacheLoaderObserver aggregate, ICacheLoaderObserver observer)
		{
			if(aggregate != null && observer != null)
				lock (aggregate._observers)
					aggregate._observers.Enqueue(observer);
			return aggregate;
		}

		/// <summary>
		/// Remove o observer que já foi anexado.
		/// </summary>
		/// <param name="aggregate"></param>
		/// <param name="observer"></param>
		public static AggregateCacheLoaderObserver operator -(AggregateCacheLoaderObserver aggregate, ICacheLoaderObserver observer)
		{
			if(aggregate != null && observer != null)
			{
				lock (aggregate._observers)
				{
					var aux = new Queue<ICacheLoaderObserver>();
					while (aggregate._observers.Count > 0)
					{
						var i = aggregate._observers.Dequeue();
						if(i != observer)
							aux.Enqueue(i);
					}
					while (aux.Count > 0)
						aggregate._observers.Enqueue(aux.Dequeue());
				}
			}
			return aggregate;
		}
	}
}
