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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Interface que se implementada retornará ítens que necessários à aplicação
	/// </summary>
	public interface IDataRepository
	{
		/// <summary>
		/// Recupera o elemento que está na posição informada.
		/// </summary>
		/// <param name="uid">Identificador do elemento.</param>
		/// <returns></returns>
		Element this[int uid]
		{
			get;
		}

		/// <summary>
		/// Quantidade de elementos no respositório.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Recupera o elemento pela posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		Element GetByPosition(int index);

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Recupera os elementos carregados.
		/// </summary>
		/// <returns>Lista com todos os elementos</returns>
		IEnumerable<Element> GetElements();

		/// <summary>
		/// Carregar o dicionario de palavras a substituir
		/// </summary>
		/// <returns>Dicionário</returns>
		IDictionary<string, string> GetReplaceItems();

		/// <summary>
		/// Limpa os elemento armazenados no repositório.
		/// </summary>
		void Clear();

		/// <summary>
		/// Remove o elemento do repositório com o identificador informado.
		/// </summary>
		/// <param name="uid"></param>
		/// <returns>True caso o elemento tenha sido removido.</returns>
		bool Remove(int uid);

		/// <summary>
		/// Adiciona um novo elemento no repositório.
		/// </summary>
		/// <param name="element"></param>
		void Add(Element element);
	}
}
