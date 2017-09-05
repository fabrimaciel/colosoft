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

namespace Colosoft.Collections
{
	/// <summary>
	/// Assinatura de uma coleção observada indexada.
	/// </summary>
	public interface IIndexedObservableCollection : IObservableCollection
	{
		/// <summary>
		/// Remove todos os indices.
		/// </summary>
		void RemoveAllIndexes();

		/// <summary>
		/// Remove o indice.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="indexType"></param>
		bool RemoveIndex(string propertyName, ObservableCollectionIndexType indexType);

		/// <summary>
		/// Reseta o indice.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade do indice.</param>
		/// <param name="indexType">Tipo do indice.</param>
		void ResetIndex(string propertyName, ObservableCollectionIndexType indexType);

		/// <summary>
		/// Verifica se contém um indice para a propriedade informada.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade do indice.</param>
		/// <param name="indexType">Tipo de indice.</param>
		/// <returns></returns>
		bool ContainsIndex(string propertyName, ObservableCollectionIndexType indexType);

		/// <summary>
		/// Realiza uma pesquisa no
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="indexType"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		System.Collections.IEnumerable Search(string propertyName, ObservableCollectionIndexType indexType, object key);
	}
	/// <summary>
	/// Assinatura da coleção observada indexada.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IIndexedObservableCollection<T> : IObservableCollection<T>, IIndexedObservableCollection
	{
		/// <summary>
		/// Cria o indice.
		/// </summary>
		/// <typeparam name="PropertyType">Tipo da propriedade que será indexada.</typeparam>
		/// <param name="property">Propriedade que será indexada.</param>
		/// <param name="type">Tipo de indice.</param>
		/// <param name="comparer">Comparador</param>
		void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<T, PropertyType>> property, ObservableCollectionIndexType type, IComparer<PropertyType> comparer);

		/// <summary>
		/// Cria o indice.
		/// </summary>
		/// <typeparam name="PropertyType">Tipo da propriedade que será indexada.</typeparam>
		/// <param name="property">Propriedade que será indexada.</param>
		/// <param name="type">Tipo de indice.</param>
		void CreateIndex<PropertyType>(System.Linq.Expressions.Expression<Func<T, PropertyType>> property, ObservableCollectionIndexType type);

		/// <summary>
		/// Realiza a pesquisa usando o indice com a chave informada.
		/// </summary>
		/// <param name="property">Propriedade indexada.</param>
		/// <param name="key">Chave que será pesquisa.</param>
		/// <returns></returns>
		IEnumerable<T> Search(System.Linq.Expressions.Expression<Func<T, object>> property, object key);

		/// <summary>
		/// Realiza a pesquisa usando o indice com a chave informada.
		/// </summary>
		/// <param name="property">Propriedade indexada.</param>
		/// <param name="indexType">Tipo de indice que será usado pela pesquisa.</param>
		/// <param name="key">Chave que será pesquisa.</param>
		/// <returns></returns>
		IEnumerable<T> Search(System.Linq.Expressions.Expression<Func<T, object>> property, ObservableCollectionIndexType indexType, object key);
	}
}
