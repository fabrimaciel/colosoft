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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura de uma lista de entidades.
	/// </summary>
	public interface IEntityList : Colosoft.Collections.IObservableCollection, IEntity
	{
		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IEntity this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Objeto para sincronização.
		/// </summary>
		object SyncRoot
		{
			get;
		}

		/// <summary>
		/// Identifica se a instancia é sincronizada.
		/// </summary>
		bool IsSynchronized
		{
			get;
		}

		/// <summary>
		/// Identifica se a lista está em estado de carga tardia.
		/// </summary>
		bool IsLazyLoadState
		{
			get;
		}

		/// <summary>
		/// Adiciona um novo item para a lista.
		/// </summary>
		/// <returns>Instancia do novo item adicionado.</returns>
		/// <exception cref="System.NotSupportedException">
		///     System.ComponentModel.IBindingList.AllowNew is false.
		/// </exception>
		object AddNew();

		/// <summary>
		/// Cria um manipulador para bloquear as notificações da lista.
		/// </summary>
		/// <returns></returns>
		IDisposable GetNotificationsLock();

		/// <summary>
		/// Copia os dados da entidade informada para a instancia,
		/// inclusives os dados de alteração da nova instancia..
		/// </summary>
		/// <param name="from">Instancia com os dados que serão copiados.</param>
		void CopyFrom(IEntityList from);

		/// <summary>
		/// Ignora o item removido da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool IgnoreRemovedItem(IEntity item);
	}
	/// <summary>
	/// Assinatura da uma lista genérica de entidades.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IEntityList<TEntity> : IEntityList, IList<TEntity>, Colosoft.Collections.IObservableCollection<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		new TEntity this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		new void RemoveAt(int index);

		/// <summary>
		/// Ignora o item removido da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool IgnoreRemovedItem(TEntity item);

		/// <summary>
		/// Quantidade de registro na lista.
		/// </summary>
		new int Count
		{
			get;
		}

		/// <summary>
		/// Adiciona um faixa de itens para a lista.
		/// </summary>
		/// <param name="items"></param>
		void AddRange(IEnumerable<TEntity> items);
	}
}
