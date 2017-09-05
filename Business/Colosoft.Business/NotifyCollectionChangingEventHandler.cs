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
using System.Collections.Specialized;
using System.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Evento para alterção em listas.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate bool NotifyCollectionChangingEventHandler<TEntity> (object sender, NotifyCollectionChangingEventArgs<TEntity> e) where TEntity : IEntity;
	/// <summary>
	/// Ações de alteações em listas.
	/// </summary>
	public enum NotifyCollectionChangingAction
	{
		/// <summary>
		/// One or more items were added to the collection.
		/// </summary>
		Add = 0,
		/// <summary>
		/// One or more items were removed from the collection.
		/// </summary>
		Remove = 1,
		/// <summary>
		/// One or more items were replaced in the collection.
		/// </summary>
		Replace = 2,
		/// <summary>
		/// One or more items were moved within the collection.
		/// </summary>
		Move = 3,
		/// <summary>
		/// The content of the collection changed dramatically.
		/// </summary>
		Reset = 4,
	}
	/// <summary>
	/// Parametros para o evento de alteração em lista.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class NotifyCollectionChangingEventArgs<TEntity> : EventArgs where TEntity : IEntity
	{
		/// <summary>
		/// Ação.
		/// </summary>
		public NotifyCollectionChangingAction Action
		{
			get;
			set;
		}

		/// <summary>
		/// Itens inseridos.
		/// </summary>
		public IList<TEntity> NewItems
		{
			get;
			set;
		}

		/// <summary>
		/// Itens removidos.
		/// </summary>
		public IList<TEntity> OldItems
		{
			get;
			set;
		}

		/// <summary>
		/// Indice de inserção de novos items.
		/// </summary>
		public int StartNewItems
		{
			get;
			set;
		}

		/// <summary>
		/// Indice de inserção de items antigos.
		/// </summary>
		public int StartOldItems
		{
			get;
			set;
		}
	}
}
