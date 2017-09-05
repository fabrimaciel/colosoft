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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Assinatura dos provedores de leitura através.
	/// </summary>
	public interface IReadThruProvider
	{
		/// <summary>
		/// Carrega os itens com base nas chaves.
		/// </summary>
		/// <param name="keys">Chaves do itens que serão carregados.</param>
		/// <returns>Dicionário dos itens encontrados.</returns>
		Dictionary<string, ProviderCacheItem> LoadFromSource(string[] keys);

		/// <summary>
		/// Carrega o item com base na chave informada.
		/// </summary>
		/// <param name="key">Chave associada com o item.</param>
		/// <param name="cacheItem">Instancia do item encontrado.</param>
		void LoadFromSource(string key, out ProviderCacheItem cacheItem);

		/// <summary>
		/// Inicia o provedor.
		/// </summary>
		/// <param name="parameters">Parametros de configuração do provedor.</param>
		void Start(IDictionary parameters);

		/// <summary>
		/// Para a execução do provedor.
		/// </summary>
		void Stop();
	}
}
