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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Assinatura das classes que armazenam os indices.
	/// </summary>
	public interface IIndexStore
	{
		/// <summary>
		/// Tamanho da fonte de armazenamento.
		/// </summary>
		int Size
		{
			get;
		}

		/// <summary>
		/// Adiciona um novo indice.
		/// </summary>
		/// <param name="key">Chave do indice.</param>
		/// <param name="value">Valor do indice.</param>
		void Add(object key, object value);

		/// <summary>
		/// Remove o indice informado.
		/// </summary>
		/// <param name="key">Chave do indice.</param>
		/// <param name="value">Valor do indice.</param>
		void Remove(object key, object value);

		/// <summary>
		/// Limpa todos os dados.
		/// </summary>
		void Clear();

		/// <summary>
		/// Recupera o indice com base na chave informada.
		/// </summary>
		/// <param name="key">Chave que será pesquisada.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <returns></returns>
		ArrayList GetData(object key, ComparisonType comparisonType);

		/// <summary>
		/// Recupera o enumerador do itens armazenados.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator GetEnumerator();
	}
}
