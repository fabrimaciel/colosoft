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
using System.Collections.Specialized;

namespace Colosoft.Util
{
	/// <summary>
	/// Argumento dos eventos da lista
	/// </summary>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public class ListChangedEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Indice do elemento
		/// </summary>
		public int index;

		/// <summary>
		/// Ítem
		/// </summary>
		public T item;

		/// <summary>
		/// Construtor do argumento
		/// </summary>
		/// <param name="index">Indice</param>
		/// <param name="item">Ítem</param>
		public ListChangedEventArgs(int index, T item)
		{
			this.index = index;
			this.item = item;
		}
	}
	/// <summary>
	/// Argumentos dos eventos que ocorrem antes da inserção de itens na lista
	/// </summary>
	/// <typeparam name="T">Tipo do ítem a ser inserido</typeparam>
	public class BeforeInsertItemEventArge<T> : EventArgs
	{
		/// <summary>
		/// Item a ser inserido
		/// </summary>
		public T Item;

		/// <summary>
		/// Se pode ou não fazer a inserção do ítem
		/// </summary>
		public bool CanInsert;

		/// <summary>
		/// Construtor do argumento
		/// </summary>
		/// <param name="item">item que será inserido</param>
		public BeforeInsertItemEventArge(T item)
		{
			Item = item;
			CanInsert = true;
		}
	}
	/// <summary>
	/// Evento que ocorre antes de inserir um ítem na lista
	/// </summary>
	/// <typeparam name="T">Tipo do item a ser inserido</typeparam>
	/// <param name="sender">Lista de origem</param>
	/// <param name="e">Argumentos do evento</param>
	public delegate void BeforeInsertItemEventHandler<T> (object sender, BeforeInsertItemEventArge<T> e);
	/// <summary>
	/// Evento que ocorre quando um elemento é adicionado à lista
	/// </summary>
	/// <param name="sender">Lista de origem</param>
	/// <param name="e">Argumentos do evento</param>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public delegate void ItemAddedEventHandler<T> (object sender, ListChangedEventArgs<T> e);
	/// <summary>
	/// Evento que ocorre quando um elemento é removido da lista
	/// </summary>
	/// <param name="sender">Lista de origem</param>
	/// <param name="e">Argumentos do evento</param>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public delegate void ItemRemovedEventHandler<T> (object sender, ListChangedEventArgs<T> e);
	/// <summary>
	/// Evento que ocorre quando um elemento é adicionado ou removido da lista
	/// </summary>
	/// <param name="sender">Lista de origem</param>
	/// <param name="e">Argumentos do evento</param>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public delegate void ListChangedEventHandler<T> (object sender, ListChangedEventArgs<T> e);
	/// <summary>
	/// Evento que ocorre quando a lista é limpad
	/// </summary>
	/// <param name="sender">Lista de origem</param>
	/// <param name="e">Argumentos do evento</param>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public delegate void ListClearedEventHandler<T> (object sender, EventArgs e);
	/// <summary>
	/// Lista que aciona evento quando os ítens são alterados
	/// </summary>
	/// <typeparam name="T">Tipo de elemento da lista</typeparam>
	public class ObservableList<T> : IList<T>, INotifyCollectionChanged
	{
		private IList<T> internalList;

		/// <summary>
		/// Ocorre sempre que a lista é alterada
		/// </summary>
		public event ListChangedEventHandler<T> ListChanged;

		/// <summary>
		/// Ocorre quando um ítem é removido
		/// </summary>
		public event ItemRemovedEventHandler<T> ItemRemoved;

		/// <summary>
		/// Ocorre quando um ítem é adicionado à lista
		/// </summary>
		public event ItemAddedEventHandler<T> ItemAdded;

		/// <summary>
		/// Ocorre quando a lista é limpa
		/// </summary>
		public event ListClearedEventHandler<T> ListCleared;

		/// <summary>
		/// Ocorre antes de adicionar um elemanto na lista
		/// </summary>
		public event BeforeInsertItemEventHandler<T> BeforeInsert;

		/// <summary>
		/// Ocorre sempre que a coleção de dados é alterada
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Construto da classe
		/// </summary>
		public ObservableList()
		{
			internalList = new List<T>();
		}

		/// <summary>
		/// Cosntrutor da classe com inicialização de um IList que será observado
		/// </summary>
		/// <param name="list">IList a observar</param>
		public ObservableList(IList<T> list)
		{
			internalList = list;
		}

		/// <summary>
		/// Cosntrutor da classe com inicialização de um IEnumerable que será observado
		/// </summary>
		/// <param name="collection">Enumerador</param>
		public ObservableList(IEnumerable<T> collection)
		{
			internalList = new List<T>(collection);
		}

		/// <summary>
		/// Evento chamado antes de inserir um ítem na lista
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnBeforeInsert(BeforeInsertItemEventArge<T> e)
		{
			if(BeforeInsert != null)
				BeforeInsert(this, e);
		}

		/// <summary>
		/// Evento de ítem adicionado
		/// </summary>
		/// <param name="e">Argumentos</param>
		protected virtual void OnItemAdded(ListChangedEventArgs<T> e)
		{
			if(ItemAdded != null)
				ItemAdded(this, e);
		}

		/// <summary>
		/// Evento de ítem removido
		/// </summary>
		/// <param name="e">Argumento</param>
		protected virtual void OnItemRemoved(ListChangedEventArgs<T> e)
		{
			if(ItemRemoved != null)
				ItemRemoved(this, e);
		}

		/// <summary>
		/// Evento de lista alterada
		/// </summary>
		/// <param name="e">Argumento</param>
		protected virtual void OnListChanged(ListChangedEventArgs<T> e)
		{
			if(ListChanged != null)
				ListChanged(this, e);
		}

		/// <summary>
		/// Evento de lista limpa
		/// </summary>
		/// <param name="e">Argumentos</param>
		protected virtual void OnListCleared(EventArgs e)
		{
			if(ListCleared != null)
				ListCleared(this, e);
		}

		/// <summary>
		/// Chama evento de quando uma coleção é alterada
		/// </summary>
		/// <param name="e">Argumentos</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}

		/// <summary>
		/// Retorna o índice de um item
		/// </summary>
		/// <param name="item">Ítem</param>
		/// <returns>Índice do item</returns>
		public virtual int IndexOf(T item)
		{
			return internalList.IndexOf(item);
		}

		/// <summary>
		/// Insere um ítem no índice especificado
		/// </summary>
		/// <param name="index">Índice</param>
		/// <param name="item">Ítem</param>
		public virtual void Insert(int index, T item)
		{
			BeforeInsertItemEventArge<T> ea = new BeforeInsertItemEventArge<T>(item);
			OnBeforeInsert(ea);
			if(ea.CanInsert)
			{
				internalList.Insert(index, item);
				OnListChanged(new ListChangedEventArgs<T>(index, item));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		/// <summary>
		/// Remove o ítem da posição especificada
		/// </summary>
		/// <param name="index">Índice</param>
		public virtual void RemoveAt(int index)
		{
			T item = internalList[index];
			internalList.Remove(item);
			OnListChanged(new ListChangedEventArgs<T>(index, item));
			OnItemRemoved(new ListChangedEventArgs<T>(index, item));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
		}

		/// <summary>
		/// Retorna o elemento da posição específicada
		/// </summary>
		/// <param name="index">Índice</param>
		/// <returns>Ítem</returns>
		public virtual T this[int index]
		{
			get
			{
				return internalList[index];
			}
			set
			{
				BeforeInsertItemEventArge<T> ea = new BeforeInsertItemEventArge<T>(value);
				OnBeforeInsert(ea);
				if(ea.CanInsert)
				{
					internalList[index] = value;
					OnItemAdded(new ListChangedEventArgs<T>(index, value));
					OnListChanged(new ListChangedEventArgs<T>(index, value));
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
				}
			}
		}

		/// <summary>
		/// Adiciona um ítem à lista
		/// </summary>
		/// <param name="item">ítem</param>
		public virtual void Add(T item)
		{
			BeforeInsertItemEventArge<T> ea = new BeforeInsertItemEventArge<T>(item);
			OnBeforeInsert(ea);
			if(ea.CanInsert)
			{
				internalList.Add(item);
				OnItemAdded(new ListChangedEventArgs<T>(internalList.IndexOf(item), item));
				OnListChanged(new ListChangedEventArgs<T>(internalList.IndexOf(item), item));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		/// <summary>
		/// Limpa a lista
		/// </summary>
		public void Clear()
		{
			internalList.Clear();
			OnListCleared(new EventArgs());
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new T[0]));
		}

		/// <summary>
		/// Verifica se a lista contém determinado ítem
		/// </summary>
		/// <param name="item">ítem</param>
		/// <returns>Verdadeiro-Tem o ítem Falso-Não tem o ítem</returns>
		public bool Contains(T item)
		{
			return internalList.Contains(item);
		}

		/// <summary>
		/// Copia a lista para um array
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="arrayIndex">Índice inicial do array</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Retorna a quantidade de ítens da lista
		/// </summary>
		public int Count
		{
			get
			{
				return internalList.Count;
			}
		}

		/// <summary>
		/// Indica se a lista é ou não apenas para leitura
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return IsReadOnly;
			}
		}

		/// <summary>
		/// Remove um ítem da lista
		/// </summary>
		/// <param name="item">Ítem a ser removido</param>
		/// <returns>Sucesso da operação</returns>
		public virtual bool Remove(T item)
		{
			lock (this)
			{
				int index = internalList.IndexOf(item);
				if(internalList.Remove(item))
				{
					OnListChanged(new ListChangedEventArgs<T>(index, item));
					OnItemRemoved(new ListChangedEventArgs<T>(index, item));
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
					return true;
				}
				else
					return false;
			}
		}

		/// <summary>
		/// Retorna o enumerador
		/// </summary>
		/// <returns>enumerador</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return internalList.GetEnumerator();
		}

		/// <summary>
		/// Retorna o enumerador
		/// </summary>
		/// <returns>enumerador</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)internalList).GetEnumerator();
		}
	}
}
