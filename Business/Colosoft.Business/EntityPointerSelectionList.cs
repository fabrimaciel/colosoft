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
	/// A seleção de um item adiciona uma entidade relacionando o item selecionado com o item pai.
	/// </summary>
	/// <typeparam name="TBusiness">Tipo dos itens filhos.</typeparam>
	/// <typeparam name="TPointer">Tipo da entidade de relacionamento.</typeparam>
	/// <typeparam name="TItem">Tipo do item da coleção.</typeparam>
	public class EntityPointerSelectionList<TBusiness, TPointer, TItem> : Colosoft.Collections.ObservableCollectionProxy<TBusiness, TItem> where TBusiness : class where TPointer : Colosoft.Business.IEntity where TItem : IProxyListItem<TBusiness, TPointer>
	{
		private Colosoft.Collections.IObservableCollection<TPointer> _selected;

		/// <summary>
		/// Lista de relacionamentos entre pai e filhos.
		/// </summary>
		public Colosoft.Collections.IObservableCollection<TPointer> Selected
		{
			get
			{
				return _selected;
			}
		}

		/// <summary>
		/// Contrutor parameterizado.
		/// </summary>
		/// <param name="items">Os itens filhos.</param>
		/// <param name="selected">A lista dos relacionamentos entre pais e filhos.</param>
		/// <param name="createCall">Chamada de criação de relacionamento entre pai e um filho.</param>
		/// <param name="compareCall">Comparação que indica se um relacionamento se refere ao filho passado como parâmetro.</param>
		/// <param name="itemCreate">Chamada de criação de wrapper da coleção.</param>
		public EntityPointerSelectionList(Colosoft.Collections.IObservableCollection<TBusiness> items, Colosoft.Collections.IObservableCollection<TPointer> selected, Func<TBusiness, TPointer> createCall, Func<TBusiness, TPointer, bool> compareCall, Func<TBusiness, TItem> itemCreate) : base(items, e => itemCreate(e))
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
			foreach (TItem data in this)
			{
				data.Dispose();
			}
		}
	}
}
