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
	/// Coleção genérica para a seleção de itens de um grupo. Exibe os elementos
	/// através de Wrappers que possuem a propriedade IsSelected, indicando o estado de seleção do item.
	/// A seleção de um item adiciona o mesmo em uma lista de seleção.
	/// </summary>
	/// <typeparam name="TBusiness"></typeparam>
	public class EntitySelectionList<TBusiness> : Colosoft.Collections.ObservableCollectionProxy<TBusiness, EntitySelectionList<TBusiness>.Item> where TBusiness : class
	{
		/// <summary>
		/// Wrapper de manutenção do estado de seleção do item.
		/// </summary>
		public class Item : NotificationObject, IDisposable
		{
			private Colosoft.Business.IEntity _controller;

			private TBusiness _data;

			private Colosoft.Collections.IObservableCollection<TBusiness> _selected;

			/// <summary>
			/// O item encapsulado.
			/// </summary>
			public TBusiness Data
			{
				get
				{
					return _data;
				}
			}

			/// <summary>
			/// O estado de seleção do item
			/// </summary>
			public virtual bool IsSelected
			{
				get
				{
					return _selected.Contains(_data);
				}
				set
				{
					var state = _selected.Contains(_data);
					if(state && (!value))
					{
						_selected.Remove(_data);
					}
					else if((!state) && value)
					{
						_selected.Add(_data);
					}
				}
			}

			/// <summary>
			/// Indica se o estado dos itens da coleção pode ser alterado.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return (_controller != null) && (!_controller.IsLockedToEdit);
				}
			}

			/// <summary>
			/// Consrutor parametrizado.
			/// </summary>
			/// <param name="controller"></param>
			/// <param name="data"></param>
			/// <param name="selected"></param>
			public Item(Colosoft.Business.IEntity controller, TBusiness data, Colosoft.Collections.IObservableCollection<TBusiness> selected)
			{
				_controller = controller;
				_data = data;
				_selected = selected;
				_selected.CollectionChanged += SelectedChanged;
			}

			/// <summary>
			/// Tratamento de alterações na lista de seleção.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void SelectedChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				switch(e.Action)
				{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					if(e.NewItems.Cast<TBusiness>().Contains(_data))
					{
						RaisePropertyChanged("IsSelected");
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					if(e.OldItems.Cast<TBusiness>().Contains(_data))
					{
						RaisePropertyChanged("IsSelected");
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					if(e.NewItems.Cast<TBusiness>().Contains(_data) || e.OldItems.Cast<TBusiness>().Contains(_data))
					{
						RaisePropertyChanged("IsSelected");
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					RaisePropertyChanged("IsSelected");
					break;
				}
			}

			/// <summary>
			/// Método para liberar recursos alocados pelo item.
			/// </summary>
			/// <param name="disposing"></param>
			protected virtual void Dispose(bool disposing)
			{
				_selected.CollectionChanged -= SelectedChanged;
			}

			/// <summary>
			/// Método para liberar recursos alocados pelo item.
			/// </summary>
			public void Dispose()
			{
				this.Dispose(true);
			}
		}

		private Colosoft.Collections.IObservableCollection<TBusiness> _selected;

		/// <summary>
		/// Lista de relacionamentos entre pai e filhos.
		/// </summary>
		public Colosoft.Collections.IObservableCollection<TBusiness> Selected
		{
			get
			{
				return _selected;
			}
		}

		/// <summary>
		/// Contrutor parameterizado.
		/// </summary>
		/// <param name="controller">A entidade que controla o estado dos items da lista.</param>
		/// <param name="items">A lista de itens.</param>
		/// <param name="selected">A lista dos selecionados.</param>
		public EntitySelectionList(Colosoft.Business.IEntity controller, Colosoft.Collections.IObservableCollection<TBusiness> items, Colosoft.Collections.IObservableCollection<TBusiness> selected) : base(items, e => new Item(controller, e, selected))
		{
			_selected = selected;
		}

		/// <summary>
		/// Construtor para lista com items personalizados.
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="items"></param>
		/// <param name="selected"></param>
		/// <param name="itemCreator"></param>
		public EntitySelectionList(Colosoft.Business.IEntity controller, Collections.IObservableCollection<TBusiness> items, Collections.IObservableCollection<TBusiness> selected, Func<Colosoft.Business.IEntity, TBusiness, Collections.IObservableCollection<TBusiness>, Item> itemCreator) : base(items, e => itemCreator(controller, e, selected))
		{
			_selected = selected;
		}

		/// <summary>
		/// Liberação de recursos alocados pela coleção.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			foreach (Item data in this)
			{
				data.Dispose();
			}
		}
	}
}
