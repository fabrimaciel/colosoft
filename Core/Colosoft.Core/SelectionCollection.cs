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
	/// Compara dois itens para a entrada se uma seleção.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="item"></param>
	/// <param name="item2"></param>
	/// <returns></returns>
	public delegate bool SelectionEntryEqualityComparer<T> (T item, T item2);
	/// <summary>
	/// Implementação de uma coleção de seleção.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SelectionCollection<T> : ObservableCollectionProxy<T, SelectionCollection<T>.Entry>
	{
		/// <summary>
		/// Representa uma entrada.
		/// </summary>
		public class Entry : NotificationObject
		{
			private SelectionCollection<T> _parent;

			private T _item;

			private bool _isSelected;

			/// <summary>
			/// Item associado com a instancia.
			/// </summary>
			public T Item
			{
				get
				{
					return _item;
				}
			}

			/// <summary>
			/// Identifica se a entrada está selecionada
			/// </summary>
			public bool IsSelected
			{
				get
				{
					return _isSelected;
				}
				set
				{
					if(_isSelected != value)
					{
						var selectedItems = _parent._selectedItems;
						if(value)
						{
							selectedItems.Add(Item);
						}
						else
						{
							for(var i = 0; i < selectedItems.Count; i++)
							{
								if(_parent._selectionComparer(Item, selectedItems[i]))
								{
									selectedItems.RemoveAt(i);
									break;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="item"></param>
			internal Entry(SelectionCollection<T> parent, T item)
			{
				_parent = parent;
				_item = item;
				_isSelected = _parent._selectedItems.Any(f => _parent._selectionComparer(_item, f));
			}

			/// <summary>
			/// Define se e entrada está selecionada.
			/// </summary>
			internal void SetIsSelected(bool isSelected)
			{
				_isSelected = isSelected;
				RaisePropertyChanged("IsSelected");
			}
		}

		/// <summary>
		/// Lista com a relação dos itens selecionados.
		/// </summary>
		private IObservableCollection<T> _selectedItems;

		private SelectionEntryEqualityComparer<T> _selectionComparer;

		/// <summary>
		/// Cria a instancia com a coleção de origem.
		/// </summary>
		/// <param name="source">Coleção de origem com todos os dados da lista.</param>
		/// <param name="selectedItems">Relação dos itens selecionados.</param>
		public SelectionCollection(IObservableCollection<T> source, IObservableCollection<T> selectedItems) : this(source, selectedItems, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="source">Coleção de origem com todos os dados da lista.</param>
		/// <param name="selectedItems">Relação dos itens selecionados.</param>
		/// <param name="selectionComparer">Comparador que será utilizado na solução.</param>
		public SelectionCollection(IObservableCollection<T> source, IObservableCollection<T> selectedItems, SelectionEntryEqualityComparer<T> selectionComparer)
		{
			source.Require("source").NotNull();
			selectedItems.Require("selectedItems").NotNull();
			_selectionComparer = selectionComparer == null ? (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)) ? new SelectionEntryEqualityComparer<T>(Compare2) : new SelectionEntryEqualityComparer<T>(Compare1)) : selectionComparer;
			_selectedItems = selectedItems;
			Initialize(source, CreateEntryProxy, null);
			_selectedItems.CollectionChanged += SelectedItemsCollectionChanged;
		}

		/// <summary>
		/// Comparação explicita dos itens.
		/// </summary>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <returns></returns>
		private static bool Compare1(T item1, T item2)
		{
			return (!object.ReferenceEquals(item1, null) && item2.Equals(item1)) || object.ReferenceEquals(item1, item2);
		}

		/// <summary>
		/// Compara quando o item é um IEquatable.
		/// </summary>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <returns></returns>
		private static bool Compare2(T item1, T item2)
		{
			return (!object.ReferenceEquals(item1, null) && ((IEquatable<T>)item2).Equals(item2));
		}

		/// <summary>
		/// Método acionado toda vez que a lista de itens selecionados for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (T i in e.NewItems)
					foreach (var entry in this)
						if(_selectionComparer(entry.Item, i))
						{
							entry.SetIsSelected(true);
							break;
						}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (T i in e.OldItems)
					foreach (var entry in this)
						if(_selectionComparer(entry.Item, i))
						{
							entry.SetIsSelected(false);
							break;
						}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				foreach (var entry in this)
				{
					bool found = false;
					foreach (var item in _selectedItems)
						if(_selectionComparer(entry.Item, item))
						{
							entry.SetIsSelected(true);
							found = true;
							break;
						}
					if(!found)
						entry.SetIsSelected(false);
				}
			}
		}

		/// <summary>
		/// Cria o proxy para a entrada.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private Entry CreateEntryProxy(T item)
		{
			return new Entry(this, item);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_selectedItems.CollectionChanged -= SelectedItemsCollectionChanged;
		}
	}
}
