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
	/// Argumentos usados no evento do loader
	/// </summary>
	public class VirtualListLoaderEventArgs : EventArgs
	{
		private readonly object _referenceObject;

		private readonly int _startRow;

		private readonly int _pageSize;

		private readonly bool _needItemsCount;

		/// <summary>
		/// Instancia do objeto de referencia usado na lista.
		/// </summary>
		public object ReferenceObject
		{
			get
			{
				return _referenceObject;
			}
		}

		/// <summary>
		/// Linha inicial para recuperar os itens.
		/// </summary>
		public int StartRow
		{
			get
			{
				return _startRow;
			}
		}

		/// <summary>
		/// Tamanho da página de itens que será carregada.
		/// </summary>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
		}

		/// <summary>
		/// Identifica que é necessário fornecer a quantidade de itens da lista.
		/// </summary>
		public bool NeedItemsCount
		{
			get
			{
				return _needItemsCount;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="startRow">Linha inicial para recuperar os itens.</param>
		/// <param name="pageSize">Tamanho da página de itens que será carregada.</param>
		/// <param name="needItemsCount">Identifica que é necessárion fornecer a quantidade de itens da lista.</param>
		/// <param name="referenceObject">Instancia do objeto de referencia usado na lista.</param>
		public VirtualListLoaderEventArgs(int startRow, int pageSize, bool needItemsCount, object referenceObject = null)
		{
			_referenceObject = referenceObject;
			_startRow = startRow;
			_pageSize = pageSize;
			_needItemsCount = needItemsCount;
		}
	}
	/// <summary>
	/// Delegate que representa os métodos utilizados para carregar os dados da lista virtual.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate VirtualListLoaderResult<T> VirtualListLoaderHandler<T> (object sender, VirtualListLoaderEventArgs e);
	/// <summary>
	/// Classe responsável pelo fornecimento de dados para a lista virtual.
	/// </summary>
	public abstract class VirtualListLoader<T> : IDisposable
	{
		/// <summary>
		/// Destrutor.
		/// </summary>
		~VirtualListLoader()
		{
			Dispose(false);
		}

		/// <summary>
		/// Carrega os dados para a lista virtual.
		/// </summary>
		/// <param name="virtualList">Instancia da lista virtual associada.</param>
		/// <param name="startRow">Linha inicial para recuperar os itens.</param>
		/// <param name="pageSize">Tamanho da página de itens que será carregada.</param>
		/// <param name="needItemsCount">Identifica que é necessárion fornecer a quantidade de itens da lista.</param>
		/// <param name="referenceObject">Instancia do objeto de referencia usado na lista.</param>
		/// <returns></returns>
		public abstract VirtualListLoaderResult<T> Load(IObservableCollection virtualList, int startRow, int pageSize, bool needItemsCount, object referenceObject = null);

		/// <summary>
		/// Cria uma instancia do loader com base no metodo informado.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static VirtualListLoader<T> Create(VirtualListLoaderHandler<T> method)
		{
			return new Default(method);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
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
		/// Implementação do loader padrão.
		/// </summary>
		class Default : VirtualListLoader<T>
		{
			private VirtualListLoaderHandler<T> _handler;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="handler"></param>
			public Default(VirtualListLoaderHandler<T> handler)
			{
				handler.Require("handler").NotNull();
				_handler = handler;
			}

			/// <summary>
			/// Carrega os dados para a lista virtual.
			/// </summary>
			/// <param name="virtualList">Instancia da lista virtual associada.</param>
			/// <param name="startRow">Linha inicial para recuperar os itens.</param>
			/// <param name="pageSize">Tamanho da página de itens que será carregada.</param>
			/// <param name="needItemsCount">Identifica que é necessárion fornecer a quantidade de itens da lista.</param>
			/// <param name="referenceObject">Instancia do objeto de referencia usado na lista.</param>
			/// <returns></returns>
			public override VirtualListLoaderResult<T> Load(IObservableCollection virtualList, int startRow, int pageSize, bool needItemsCount, object referenceObject = null)
			{
				return _handler(virtualList, new VirtualListLoaderEventArgs(startRow, pageSize, needItemsCount, referenceObject));
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				_handler = null;
			}
		}
	}
}
