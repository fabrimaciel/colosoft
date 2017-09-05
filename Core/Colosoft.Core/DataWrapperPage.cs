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
	/// Pagina de dados.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataWrapperPage<T> : IDataPage<DataWrapper<T>>
	{
		private List<DataWrapper<T>> _items;

		private bool _hasError;

		/// <summary>
		/// Identifica se a página já foi populada.
		/// </summary>
		public bool IsPopulated
		{
			get
			{
				return _items != null;
			}
		}

		/// <summary>
		/// Quantidade de itens carregados na página.
		/// </summary>
		public int Count
		{
			get
			{
				return _items == null ? 0 : _items.Count;
			}
		}

		/// <summary>
		/// Identifica se a página sofreu algum erro na carga.
		/// </summary>
		public bool HasError
		{
			get
			{
				return _hasError;
			}
		}

		/// <summary>
		/// Recupera a define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataWrapper<T> this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				_items[index] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="firstIndex">Primeiro índice da página.</param>
		/// <param name="pageLength">Tamanho da página.</param>
		public DataWrapperPage(int firstIndex, int pageLength)
		{
			_items = new List<DataWrapper<T>>(pageLength);
			for(int i = 0; i < pageLength; i++)
				_items.Add(new DataWrapper<T>(firstIndex + i));
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DataWrapperPage()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método usado para popular a página de dados.
		/// </summary>
		/// <param name="items">Itens para carrega a página.</param>
		public void Populate(IEnumerable<DataWrapper<T>> items)
		{
			var newItems = items.ToArray();
			int i;
			int index = 0;
			for(i = 0; i < newItems.Length && i < _items.Count; i++)
			{
				_items[i].SetData(newItems[i].Data);
				index = _items[i].Index;
			}
			while (i < newItems.Length)
			{
				index++;
				newItems[i].Index = index;
				_items.Add(newItems[i]);
				i++;
			}
			while (i < _items.Count)
				_items.RemoveAt(_items.Count - 1);
		}

		/// <summary>
		/// Notifica que houve um erro ao carregar a página de dados.
		/// </summary>
		/// <param name="exception"></param>
		public void NotifyError(Exception exception)
		{
			_hasError = true;
			for(int i = 0; i < _items.Count; i++)
				_items[i].LoadError = true;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_items.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<DataWrapper<T>> GetEnumerator()
		{
			foreach (var i in _items)
				yield return i;
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}
