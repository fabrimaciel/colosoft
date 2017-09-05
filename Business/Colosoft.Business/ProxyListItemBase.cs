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
	/// Implementação base de item de lista de seleção.
	/// </summary>
	/// <typeparam name="TBusiness"></typeparam>
	/// <typeparam name="TPointer"></typeparam>
	public class ProxyListItemBase<TBusiness, TPointer> : NotificationObject, IProxyListItem<TBusiness, TPointer> where TBusiness : class where TPointer : Colosoft.Business.IEntity
	{
		private IEntity _controller;

		private TBusiness _data;

		private Func<TBusiness, TPointer> _create;

		private Func<TBusiness, TPointer, bool> _compare;

		private Colosoft.Collections.IObservableCollection<TPointer> _selected;

		/// <summary>
		/// Entidade que determina o estado do item
		/// </summary>
		protected IEntity Controller
		{
			get
			{
				return _controller;
			}
		}

		/// <summary>
		/// Chamada de criação de ponteiro.
		/// </summary>
		protected Func<TBusiness, TPointer> CreateCall
		{
			get
			{
				return _create;
			}
		}

		/// <summary>
		/// Chamada de comparação de item com ponteiro.
		/// </summary>
		protected Func<TBusiness, TPointer, bool> CompareCall
		{
			get
			{
				return _compare;
			}
		}

		/// <summary>
		/// Lista dos itens selecionados (ponteiros).
		/// </summary>
		protected Colosoft.Collections.IObservableCollection<TPointer> Selected
		{
			get
			{
				return _selected;
			}
		}

		/// <summary>
		/// Método para detecção de alterações na lista de selecionados, evita que alterações efetuadas
		/// fora do Wrapper não sejam observadas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectedChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				if(e.NewItems.Cast<TPointer>().Any(p => _compare(_data, p)))
				{
					RaisePropertyChanged("IsSelected");
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if(e.OldItems.Cast<TPointer>().Any(p => _compare(_data, p)))
				{
					RaisePropertyChanged("IsSelected");
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				if(e.NewItems.Cast<TPointer>().Any(p => _compare(_data, p)) || e.OldItems.Cast<TPointer>().Any(p => _compare(_data, p)))
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
		/// Construtor parametrizado.
		/// </summary>
		/// <param name="controller">Controlador da coleção</param>
		/// <param name="data">Item filho.</param>
		/// <param name="selected">Lista de itens filhos associados ao pai.</param>
		/// <param name="createCall">Método de criação de entidades de relaciomento entre pai e um filho.</param>
		/// <param name="compareCall">Método de comparação que indica se um relacionamento se refere ao filho passado como parâmetro.</param>
		public ProxyListItemBase(IEntity controller, TBusiness data, Colosoft.Collections.IObservableCollection<TPointer> selected, Func<TBusiness, TPointer> createCall, Func<TBusiness, TPointer, bool> compareCall)
		{
			_controller = controller;
			_data = data;
			_selected = selected;
			_create = createCall;
			_compare = compareCall;
			_selected.CollectionChanged += SelectedChanged;
		}

		/// <summary>
		/// Objeto encapsulado.
		/// </summary>
		public TBusiness Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Indicador do estado de seleção do item.
		/// </summary>
		public virtual bool IsSelected
		{
			get
			{
				return _selected.Any(e => _compare(_data, e));
			}
			set
			{
				var item = _selected.Where(e => _compare(_data, e));
				var state = item.Any();
				if(state && (!value))
				{
					_selected.Remove(item.First());
				}
				else if((!state) && value)
				{
					_selected.Add(_create(_data));
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
		/// Liberação de recursos alocados pela instância.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}
	}
}
