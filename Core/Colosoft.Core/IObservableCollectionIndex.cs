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

namespace Colosoft.Collections
{
	/// <summary>
	/// Possíveis tipos de indice.
	/// </summary>
	public enum ObservableCollectionIndexType
	{
		/// <summary>
		/// Qualquer tipo de indice.
		/// </summary>
		Any = 0,
		/// <summary>
		/// Indice ordenado.
		/// </summary>
		Sorted,
		/// <summary>
		/// Indice por hash.
		/// </summary>
		Hash
	}
	/// <summary>
	/// Assinatura das classes de indices da coleções observadas.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IObservableCollectionIndex<T>
	{
		/// <summary>
		/// Nome do indice.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Propriedade que são monitoradas pelo indice.
		/// </summary>
		string[] WatchProperties
		{
			get;
		}

		/// <summary>
		/// Recupera o itens associado com o a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		System.Collections.Generic.IEnumerable<T> this[object key]
		{
			get;
		}

		/// <summary>
		/// Reseta o indice.
		/// </summary>
		void Reset();
	}
}
