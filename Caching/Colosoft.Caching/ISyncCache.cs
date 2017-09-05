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

namespace Colosoft.Caching.Synchronization
{
	/// <summary>
	/// Assinatura das classe que necessitam da sincronização.
	/// </summary>
	public interface ISyncCache : IDisposable
	{
		/// <summary>
		/// Identificador do cache.
		/// </summary>
		string CacheId
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera a instancia pela chave informada.
		/// </summary>
		/// <param name="key">Chave dos dados.</param>
		/// <param name="version">Versão da chave recuperada.</param>
		/// <param name="flag">Conjunto de bits associado.</param>
		/// <returns></returns>
		object Get(string key, ref ulong version, ref BitSet flag);

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Registra a instancia para escutar os eventos para a chave informada.
		/// </summary>
		/// <param name="key">Chave os dados.</param>
		/// <param name="eventListener">Listener</param>
		void RegisterSyncKeyNotifications(string key, ISyncCacheEventsListener eventListener);

		/// <summary>
		/// Remove o registra da instancia para escutar os eventos para a chave informada.
		/// </summary>
		/// <param name="key">Chave os dados.</param>
		/// <param name="eventListener">Listener</param>
		void UnRegisterSyncKeyNotifications(string key, ISyncCacheEventsListener eventListener);
	}
}
