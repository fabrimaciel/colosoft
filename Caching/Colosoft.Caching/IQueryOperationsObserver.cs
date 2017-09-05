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
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Assinatura das classes de observação das operaçãoes de consulta.
	/// </summary>
	internal interface IQueryOperationsObserver
	{
		/// <summary>
		/// Método acionado quando um noto item é adicionado.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void OnItemAdded(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify);

		/// <summary>
		/// Método acionado quando o item for removido.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void OnItemRemoved(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify);

		/// <summary>
		/// Método acionado quando um item é atualizado.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void OnItemUpdated(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify);
	}
}
