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

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Possíveis resultados da operação de adicionar um novo item
	/// no armazenamento.
	/// </summary>
	public enum StoreAddResult
	{
		/// <summary>
		/// Sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// A chave já existe.
		/// </summary>
		KeyExists,
		/// <summary>
		/// Sem espaço suficiente.
		/// </summary>
		NotEnoughSpace,
		/// <summary>
		/// Sucesso na próxima liberação.
		/// </summary>
		SuccessNearEviction,
		/// <summary>
		/// Falha.
		/// </summary>
		Failure
	}
	/// <summary>
	/// Possíveis resultados para a operação de inserir
	/// um novo item no armazenamento.
	/// </summary>
	public enum StoreInsResult
	{
		/// <summary>
		/// Sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Sucesso e o item foi sobreescrito.
		/// </summary>
		SuccessOverwrite,
		/// <summary>
		/// Sucesso na próxima liberação.
		/// </summary>
		SuccessNearEviction,
		/// <summary>
		/// Sucesso na próxima liberação e o item foi sobreescrito.
		/// </summary>
		SuccessOverwriteNearEviction,
		/// <summary>
		/// Sem espaço suficiente.
		/// </summary>
		NotEnoughSpace,
		/// <summary>
		/// Falha.
		/// </summary>
		Failure
	}
	/// <summary>
	/// Assinatura das classes de armazenamento dos dados do cache.
	/// </summary>
	public interface ICacheStorage : IDisposable
	{
		/// <summary>
		/// Quantidade de itens armazenados.
		/// </summary>
		long Count
		{
			get;
		}

		/// <summary>
		/// Vetor da chaves armazenadas.
		/// </summary>
		Array Keys
		{
			get;
		}

		/// <summary>
		/// Quantidade máxima suportada.
		/// </summary>
		long MaxCount
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho máximo suportado.
		/// </summary>
		long MaxSize
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho dos dados armazenados.
		/// </summary>
		long Size
		{
			get;
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		StoreAddResult Add(object key, object item);

		/// <summary>
		/// Insere um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="item">Instancia do item.</param>
		/// <returns>Resultado da operação.</returns>
		StoreInsResult Insert(object key, object item);

		/// <summary>
		/// Recupera o item pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Get(object key);

		/// <summary>
		/// Remove o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave do item que será removido.</param>
		/// <returns>Instancia do item removido.</returns>
		object Remove(object key);

		/// <summary>
		/// Verifica existe algum item armazenado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <returns></returns>
		bool Contains(object key);

		/// <summary>
		/// Recupera o tamanho do item associado com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		int GetItemSize(object key);

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		void Clear();

		/// <summary>
		/// Recupera o enumerador para pecorrer os itens.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator GetEnumerator();
	}
}
