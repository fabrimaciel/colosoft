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
using Colosoft.Caching.Configuration;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa o callback acionado quando uma operação assincrona for completada.
	/// </summary>
	/// <param name="opCode"></param>
	/// <param name="result"></param>
	public delegate void AsyncOperationCompletedCallback (object opCode, object result);
	/// <summary>
	/// Representa o callback acionado quando o cache começar a ser ativado.
	/// </summary>
	/// <param name="cacheId"></param>
	public delegate void CacheBecomeActiveCallback (string cacheId);
	/// <summary>
	/// Representa o callback acionado quando o cache for limpo.
	/// </summary>
	public delegate void CacheClearedCallback ();
	/// <summary>
	/// Representa o callback acionaodo quando o cache for parada.
	/// </summary>
	/// <param name="cacheid"></param>
	public delegate void CacheStoppedCallback (string cacheid);
	/// <summary>
	/// Representa o callback acionado para uma notificação customizada.
	/// </summary>
	/// <param name="notifId"></param>
	/// <param name="data"></param>
	public delegate void CustomNotificationCallback (object notifId, object data);
	/// <summary>
	/// Representa o callback acionado para notificar uma remoção customizada.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <param name="reason"></param>
	/// <param name="Flag"></param>
	public delegate void CustomRemoveCallback (object key, object value, ItemRemoveReason reason, BitSet Flag);
	/// <summary>
	/// Representa o callback acionado para notificar uma atualização customizada.
	/// </summary>
	/// <param name="key">Chave do item.</param>
	/// <param name="value">Valor do item.</param>
	public delegate void CustomUpdateCallback (object key, object value);
	/// <summary>
	/// Representa o callback acionado quando a origem de dados for atualizada.
	/// </summary>
	/// <param name="result"></param>
	/// <param name="cbEntry"></param>
	/// <param name="operationCode"></param>
	public delegate void DataSourceUpdatedCallback (object result, CallbackEntry cbEntry, OpCode operationCode);
	/// <summary>
	/// Representa o callback que será acionado quando um item for adicionado.
	/// </summary>
	/// <param name="key">Chave do item.</param>
	public delegate void ItemAddedCallback (object key);
	/// <summary>
	/// Representa o callback que será acionado quando um item for removido.
	/// </summary>
	/// <param name="key">Chave do item que está sendo removido.</param>
	/// <param name="value">Valor do item.</param>
	/// <param name="reason">Razão da remoção.</param>
	/// <param name="flag"></param>
	public delegate void ItemRemovedCallback (object key, object value, ItemRemoveReason reason, BitSet flag);
	/// <summary>
	/// Representa o callback que será acionado quando um item for atualizado.
	/// </summary>
	/// <param name="key">Chave do item atualizado.</param>
	public delegate void ItemUpdatedCallback (object key);
}
