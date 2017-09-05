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
using System.Collections;
using Colosoft.Caching.Expiration;

namespace Colosoft.Caching
{
	/// <summary>
	/// Possíveis resultados para o adiciona uma 
	/// nova entrada no cache.
	/// </summary>
	[Serializable]
	internal enum CacheAddResult
	{
		/// <summary>
		/// Adiciona com sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Será executado com sucesso na próxima liberação.
		/// </summary>
		SuccessNearEviction,
		/// <summary>
		/// Chave já existe.
		/// </summary>
		KeyExists,
		/// <summary>
		/// É necessário liberação.
		/// </summary>
		NeedsEviction,
		/// <summary>
		/// Ocorreu uma falha.
		/// </summary>
		Failure,
		/// <summary>
		/// Transferido.
		/// </summary>
		BucketTransfered,
		/// <summary>
		/// Timeout completo.
		/// </summary>
		FullTimeout,
		/// <summary>
		/// Timeout parcial.
		/// </summary>
		PartialTimeout
	}
	/// <summary>
	/// Possíveis resultados para uma inserção
	/// no cache.
	/// </summary>
	[Serializable]
	public enum CacheInsResult
	{
		/// <summary>
		/// Sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Sucesso e sobreescrito.
		/// </summary>
		SuccessOverwrite,
		/// <summary>
		/// Sucesso na próxima liberação.
		/// </summary>
		SuccessNearEvicition,
		/// <summary>
		/// Sucesso e sobreescrito na próxima liberação.
		/// </summary>
		SuccessOverwriteNearEviction,
		/// <summary>
		/// Precisa de liberação.
		/// </summary>
		NeedsEviction,
		/// <summary>
		/// Falha.
		/// </summary>
		Failure,
		/// <summary>
		/// 
		/// </summary>
		BucketTransfered,
		/// <summary>
		/// Grupo incompatível.
		/// </summary>
		IncompatibleGroup,
		/// <summary>
		/// Precisa de liberação.
		/// </summary>
		NeedsEvictionNotRemove,
		/// <summary>
		/// Item bloqueado.
		/// </summary>
		ItemLocked,
		/// <summary>
		/// Versão incompatível.
		/// </summary>
		VersionMismatch,
		/// <summary>
		/// Timeout completo.
		/// </summary>
		FullTimeout,
		/// <summary>
		/// Timeout parcial.
		/// </summary>
		PartialTimeout,
		/// <summary>
		/// Chave de dependencia não existe.
		/// </summary>
		DependencyKeyNotExist,
		/// <summary>
		/// Erro na chave de dependencia.
		/// </summary>
		DependencyKeyError
	}
	/// <summary>
	/// Possíveis razões para remover o item.
	/// </summary>
	[Serializable]
	public enum ItemRemoveReason
	{
		/// <summary>
		/// Dependencia alterada.
		/// </summary>
		DependencyChanged,
		/// <summary>
		/// Item expirou.
		/// </summary>
		Expired,
		/// <summary>
		/// Item removido.
		/// </summary>
		Removed,
		/// <summary>
		/// Item inutilizado.
		/// </summary>
		Underused,
		/// <summary>
		/// Dependencia inválida.
		/// </summary>
		DependencyInvalid
	}
	/// <summary>
	/// Opções de atualiza da fonte de dados.
	/// </summary>
	public enum DataSourceUpdateOptions
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		WriteThru,
		/// <summary>
		/// 
		/// </summary>
		WriteBehind
	}
	/// <summary>
	/// Assinatura de uma classe de cache.
	/// </summary>
	internal interface ICache
	{
		/// <summary>
		/// Quantidade de itens no cache.
		/// </summary>
		long Count
		{
			get;
		}

		/// <summary>
		/// Chave do cache.
		/// </summary>
		Array Keys
		{
			get;
		}

		/// <summary>
		/// Adiciona uma entrada no cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="cacheEntry">Entrada.</param>
		/// <param name="notify">True para abilitar notificação.</param>
		/// <param name="operationContext">Contexto de operação.</param>
		/// <returns>Resultado da operação.</returns>
		CacheAddResult Add(object key, CacheEntry cacheEntry, bool notify, OperationContext operationContext);

		/// <summary>
		/// Adiciona varias entradas no cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="cacheEntries">Entradas.</param>
		/// <param name="notify">Trua para abilitar a notificação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Hash com o resultado das operações.</returns>
		Hashtable Add(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext);

		/// <summary>
		/// Insere entradas no cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="cacheEntries">Entradas.</param>
		/// <param name="notify">Trua para abilitar a notificação.</param>
		/// <param name="operationContext">Contexto da operação</param>
		/// <returns></returns>
		Hashtable Insert(object[] keys, CacheEntry[] cacheEntries, bool notify, OperationContext operationContext);

		/// <summary>
		/// Insere uma entrada no cache.
		/// </summary>
		/// <param name="key">Chave a entrada.</param>
		/// <param name="cacheEntry">Entrada.</param>
		/// <param name="notify">Trua para abilitar a notificação.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		CacheInsResultWithEntry Insert(object key, CacheEntry cacheEntry, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext);

		/// <summary>
		/// Limpa todo o cache.
		/// </summary>
		/// <param name="cbEntry">Callback da entradas.</param>
		/// <param name="updateOptions">Opções de atualização.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		void Clear(CallbackEntry cbEntry, DataSourceUpdateOptions updateOptions, OperationContext operationContext);

		/// <summary>
		/// Verifica se no cache existe a chave informada.
		/// </summary>
		/// <param name="key">Chave usada na pesquisa.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		bool Contains(object key, OperationContext operationContext);

		/// <summary>
		/// Verifica se no cache existe as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves que serão pesquisadas.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		Hashtable Contains(object[] keys, OperationContext operationContext);

		/// <summary>
		/// Recupera as entradas associados com as chaves informadas.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		Hashtable Get(object[] keys, OperationContext operationContext);

		/// <summary>
		/// Recupera a entrada pela chave informada.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="version">Versão da entrada encontrada.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="lockDate">Data do lock.</param>
		/// <param name="lockExpiration">Expiração do lock.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da opera~çao.</param>
		/// <returns>Instancia da entrada.</returns>
		CacheEntry Get(object key, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext);

		/// <summary>
		/// Recupera o enumerador para as entradas.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator GetEnumerator();

		/// <summary>
		/// Remove várias entradas do cache.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="ir">Razão para a remoção.</param>
		/// <param name="notify">Trua para abilitar a notificação.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Hash com as entradas removidas.</returns>
		Hashtable Remove(object[] keys, ItemRemoveReason ir, bool notify, OperationContext operationContext);

		/// <summary>
		/// Remove uma entrada do cache.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="ir">Razão para a remoção da entrada.</param>
		/// <param name="notify">Trua para abilitar a notificação.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <param name="version">Versão.</param>
		/// <param name="accessType">Tipo de acesso.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		CacheEntry Remove(object key, ItemRemoveReason ir, bool notify, object lockId, ulong version, LockAccessType accessType, OperationContext operationContext);
	}
}
