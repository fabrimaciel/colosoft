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
using Colosoft.Caching.Queries;

namespace Colosoft.Caching
{
	/// <summary>
	/// Assinatura da classe responsável por executar os eventos do cache.
	/// </summary>
	public interface ICacheEventsListener
	{
		/// <summary>
		/// Método acionado quando o cache for limpo.
		/// </summary>
		void OnCacheCleared();

		/// <summary>
		/// Método acionado quano um evento customizado for disparado.
		/// </summary>
		/// <param name="notifId"></param>
		/// <param name="data"></param>
		void OnCustomEvent(object notifId, object data);

		/// <summary>
		/// Método acionado quando ocorrer alguma remoção customizada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="reason"></param>
		void OnCustomRemoveCallback(object key, object value, ItemRemoveReason reason);

		/// <summary>
		/// Método acionado quando ocorrer alguma atualização customizada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="value">Valor do item.</param>
		void OnCustomUpdateCallback(object key, object value);

		/// <summary>
		/// Método acionado quando um item for adicionado.
		/// </summary>
		/// <param name="key">Chave do item adicionado.</param>
		void OnItemAdded(object key);

		/// <summary>
		/// Método acionado quando um item for removido.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="val">Valor do item.</param>
		/// <param name="reason">Razão para remoção.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		void OnItemRemoved(object key, object val, ItemRemoveReason reason, OperationContext operationContext);

		/// <summary>
		/// Método acionado quando vários itens forem removidos.
		/// </summary>
		/// <param name="keys">Chaves dos itens.</param>
		/// <param name="vals">Valores dos itens.</param>
		/// <param name="reason">Razõa para a remoção.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		void OnItemsRemoved(object[] keys, object[] vals, ItemRemoveReason reason, OperationContext operationContext);

		/// <summary>
		/// Método acionado quando um item for atualizado.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		void OnItemUpdated(object key, OperationContext operationContext);

		/// <summary>
		/// Método acionado quando a operação de escrita em background for completada.
		/// </summary>
		/// <param name="operationCode"></param>
		/// <param name="result"></param>
		/// <param name="cbEntry"></param>
		void OnWriteBehindOperationCompletedCallback(OpCode operationCode, object result, CallbackEntry cbEntry);
	}
}
