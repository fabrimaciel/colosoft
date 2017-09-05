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
	/// Armazena os argumentos do evento acionado quando
	/// uma página de dados for carregada.
	/// </summary>
	/// <typeparam name="T">Tipo tratado pela página de dados.</typeparam>
	public class DataPageLoadedEventArgs<T> : EventArgs
	{
		private IDataPage<T> _page;

		/// <summary>
		/// Página de dados associada.
		/// </summary>
		public IDataPage<T> Page
		{
			get
			{
				return _page;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="page"></param>
		public DataPageLoadedEventArgs(IDataPage<T> page)
		{
			_page = page;
		}
	}
	/// <summary>
	/// Assinatura do evento acionado quando um página de dados for carregada.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void DataPageLoadedEventHandler<T> (object sender, DataPageLoadedEventArgs<T> e);
	/// <summary>
	/// Assinatura de uma página de dados.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDataPage<T> : IEnumerable<T>, IDisposable
	{
		/// <summary>
		/// Identifica se a página já foi populada.
		/// </summary>
		bool IsPopulated
		{
			get;
		}

		/// <summary>
		/// Quantidade de itens na página de dados.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Identifica se a página possui algum erro de carga.
		/// </summary>
		bool HasError
		{
			get;
		}

		/// <summary>
		/// Recupera o item da página pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		T this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Método usado para popular a página de dados.
		/// </summary>
		/// <param name="items">Itens para carrega a página.</param>
		void Populate(IEnumerable<T> items);

		/// <summary>
		/// Notifica que houve um erro ao carregar a página de dados.
		/// </summary>
		/// <param name="exception"></param>
		void NotifyError(Exception exception);
	}
}
