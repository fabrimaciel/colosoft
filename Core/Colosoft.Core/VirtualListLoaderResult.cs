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
	/// Resultado da carga de dados.
	/// </summary>
	public class VirtualListLoaderResult<T>
	{
		private readonly IEnumerable<T> _items;

		private readonly bool _updateCount;

		private readonly int _newCount;

		private readonly Exception _error;

		/// <summary>
		/// Relação dos itens do resultado.
		/// </summary>
		public IEnumerable<T> Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Identifica se o Count foi atualizado.
		/// </summary>
		public bool UpdateCount
		{
			get
			{
				return _updateCount;
			}
		}

		/// <summary>
		/// Nova quantidade de elementos.
		/// </summary>
		public int NewCount
		{
			get
			{
				return _newCount;
			}
		}

		/// <summary>
		/// Instancia do erro ocorrido na carga.
		/// </summary>
		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Cria um instancia do resultado informando o enumerable dos itens que serão carregados.
		/// </summary>
		/// <param name="items"></param>
		public VirtualListLoaderResult(IEnumerable<T> items)
		{
			_items = items;
			_updateCount = false;
		}

		/// <summary>
		/// Cria uma instancia do resultado informandos o itens que serão carregados
		/// e o novo tamanho da coleção.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="newCount"></param>
		public VirtualListLoaderResult(IEnumerable<T> items, int newCount)
		{
			_items = items;
			_updateCount = true;
			_newCount = newCount;
		}

		/// <summary>
		/// Construtor usado para informar que houve um erro na carga.
		/// </summary>
		/// <param name="exception"></param>
		public VirtualListLoaderResult(Exception exception)
		{
			_error = exception;
		}
	}
}
