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
using StandardAnalyzer = Lucene.Net.Analysis.Standard.StandardAnalyzer;
using IndexWriter = Lucene.Net.Index.IndexWriter;
using FSDirectory = Lucene.Net.Store.FSDirectory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Documents;

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Implementa a interface IRepository
	/// </summary>
	[Export(typeof(IDataRepository))]
	public class DataRepository : IDataRepository
	{
		private bool _initialized;

		private List<Element> _elements;

		private IDictionary<int, Element> _elementsDic;

		private readonly IRepositoryLoader _loader;

		private object _objLock = new object();

		/// <summary>
		/// Retorna um elemento da lista
		/// </summary>
		/// <param name="uid">Identificador unico do elemento.</param>
		/// <returns>Elemento desejado</returns>
		public Element this[int uid]
		{
			get
			{
				lock (_objLock)
					return _elementsDic[uid];
			}
		}

		/// <summary>
		/// Quantidade de elementos no respositório.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_objLock)
					return _elementsDic != null ? _elementsDic.Count : 0;
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
				lock (_objLock)
				{
					_elements = new List<Element>();
					_elementsDic = new Dictionary<int, Element>();
					foreach (var i in _loader.GetElements())
					{
						_elements.Add(i);
						_elementsDic.Add(i.Uid, i);
					}
				}
			}
		}

		/// <summary>
		/// Recupera o elemento pela posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Element GetByPosition(int index)
		{
			lock (_objLock)
				return _elements[index];
		}

		/// <summary>
		/// Retorna a lista de elementos
		/// </summary>
		/// <returns>Lista com todos os elementos</returns>
		public IEnumerable<Element> GetElements()
		{
			if(_elementsDic == null)
				return new Element[0];
			return _elements.ToArray();
		}

		/// <summary>
		/// Carregar o dicionario de palavras a substituir
		/// </summary>
		/// <returns>Dicionário</returns>
		public IDictionary<string, string> GetReplaceItems()
		{
			var result = new Dictionary<string, string>();
			foreach (var i in _loader.GetReplaceItems())
				result.Add(i.Key, i.Value);
			return result;
		}

		/// <summary>
		/// Adiciona o elemento para o repositório.
		/// </summary>
		/// <param name="element">Instancia do elemento que será adicionado.</param>
		public void Add(Element element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			lock (_objLock)
			{
				Element aux = null;
				if(_elementsDic.TryGetValue(element.Uid, out aux))
				{
					_elementsDic.Remove(element.Uid);
					_elements.Remove(aux);
				}
				_elements.Add(element);
				_elementsDic.Add(element.Uid, element);
			}
		}

		/// <summary>
		/// Remove o elemento com o identificador informado.
		/// </summary>
		/// <param name="uid">Identificador do elemento que será removido.</param>
		/// <returns></returns>
		public bool Remove(int uid)
		{
			lock (_objLock)
			{
				Element element = null;
				if(_elementsDic.TryGetValue(uid, out element))
				{
					_elements.Remove(element);
					return _elementsDic.Remove(uid);
				}
				return false;
			}
		}

		/// <summary>
		/// Limpa os elemento armazenados no repositório.
		/// </summary>
		public void Clear()
		{
			lock (_objLock)
			{
				_elements.Clear();
				_elementsDic.Clear();
			}
		}
	}
}
