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
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="item"></param>
	/// <param name="item2"></param>
	/// <returns></returns>
	public delegate bool SelectionEntryEqualityComparer<T1, T2> (T1 item, T2 item2);
	/// <summary>
	/// Implementação de uma coleção de seleção.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProxy"></typeparam>
	public class SelectionCollection<T, TProxy> : ObservableCollectionProxy<T, SelectionCollection<T, TProxy>.Entry>
	{
		/// <summary>
		/// Representa uma entrada.
		/// </summary>
		public class Entry : NotificationObject
		{
			private SelectionCollection<T, TProxy> _parent;

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
							var proxyItem = _parent._selectedItemCreator(Item);
							selectedItems.Add(proxyItem);
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
			internal Entry(SelectionCollection<T, TProxy> parent, T item)
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
		private IObservableCollection<TProxy> _selectedItems;

		private SelectionEntryEqualityComparer<T, TProxy> _selectionComparer;

		/// <summary>
		/// Delegate do método usado para criar um instancia do proxy para o item selecionado.
		/// </summary>
		private Func<T, TProxy> _selectedItemCreator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="source">Coleção de origem com todos os dados da lista.</param>
		/// <param name="selectedItems">Relação dos itens selecionados.</param>
		/// <param name="selectionComparer">Comparador que será utilizado na solução.</param>
		/// <param name="selectedItemCreator">Delegate usado para cria o item selecionado.</param>
		public SelectionCollection(IObservableCollection<T> source, IObservableCollection<TProxy> selectedItems, SelectionEntryEqualityComparer<T, TProxy> selectionComparer, Func<T, TProxy> selectedItemCreator)
		{
			source.Require("source").NotNull();
			selectedItems.Require("selectedItems").NotNull();
			selectionComparer.Require("selectionComparer").NotNull();
			selectedItemCreator.Require("selectedItemCreator").NotNull();
			_selectionComparer = selectionComparer;
			_selectedItemCreator = selectedItemCreator;
			_selectedItems = selectedItems;
			Initialize(source, CreateEntryProxy, null);
			_selectedItems.CollectionChanged += SelectedItemsCollectionChanged;
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
				foreach (TProxy i in e.NewItems)
					foreach (var entry in this)
						if(_selectionComparer(entry.Item, i))
						{
							entry.SetIsSelected(true);
							break;
						}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (TProxy i in e.OldItems)
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
