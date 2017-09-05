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

namespace Colosoft.Caching.Policies
{
	/// <summary>
	/// Assinatura das classe que representa um política para liberação do cache.
	/// </summary>
	internal interface IEvictionPolicy
	{
		float EvictRatio
		{
			get;
			set;
		}

		/// <summary>
		/// Limpa todos os itens associados.
		/// </summary>
		void Clear();

		/// <summary>
		/// Recupera uma dica compatível com a instancia informada.
		/// </summary>
		/// <param name="eh">Instancia da dica que será comparada.</param>
		/// <returns>Instancia da dica compatível.</returns>
		EvictionHint CompatibleHint(EvictionHint eh);

		/// <summary>
		/// Executa a politica de liberação.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="context"></param>
		/// <param name="size"></param>
		void Execute(CacheBase cache, CacheRuntimeContext context, long size);

		/// <summary>
		/// Notifica algum alteração.
		/// </summary>
		/// <param name="key">Chave dos dados alterados.</param>
		/// <param name="oldhint">Antigo hint.</param>
		/// <param name="newHint">Novo hint.</param>
		void Notify(object key, EvictionHint oldhint, EvictionHint newHint);

		void Remove(object key, EvictionHint hint);
	}
}
