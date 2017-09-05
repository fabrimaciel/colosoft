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
using System.ComponentModel.Composition;

namespace Colosoft.SearchEngine.Default
{
	/// <summary>
	/// Implementa a interface IRepository
	/// </summary>
	[Export(typeof(IDataRepository))]
	public class DataRepository : IDataRepository
	{
		private bool _initialized;

		private List<Element> _elements;

		private readonly IRepositoryLoader _loader;

		/// <summary>
		/// Retorna um elemento da lista
		/// </summary>
		/// <param name="index">indice do elemento na lista</param>
		/// <returns>Elemento desejado</returns>
		public Element this[int index]
		{
			get
			{
				return _elements[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader">Loader dos dados.</param>
		[ImportingConstructor]
		public DataRepository(IRepositoryLoader loader)
		{
			if(loader == null)
				throw new ArgumentNullException("loader");
			_loader = loader;
		}

		/// <summary>
		/// Inicializa o repositório.
		/// </summary>
		public void Initialize()
		{
			if(!_initialized)
			{
				_initialized = true;
				_elements = new List<Element>(_loader.GetElements());
			}
		}

		/// <summary>
		/// Retorna a lista de elementos
		/// </summary>
		/// <returns>Lista com todos os elementos</returns>
		public IEnumerable<Element> GetElements()
		{
			return _elements;
		}

		/// <summary>
		/// Carregar o dicionario de palavras a substituir
		/// </summary>
		/// <returns>Dicionário</returns>
		public IDictionary<string, string> LoadDictionary()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (var i in _loader.GetDictionaryItems())
				result.Add(i.Key, i.Value);
			return result;
		}
	}
}
