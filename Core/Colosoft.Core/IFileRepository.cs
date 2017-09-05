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

namespace Colosoft.IO.FileRepository
{
	/// <summary>
	/// Assinatura da classe que representa um repositório de arquivos.
	/// </summary>
	public interface IFileRepository
	{
		/// <summary>
		/// Evento acionado quando o repositório é atualizado.
		/// </summary>
		event EventHandler Updated;

		/// <summary>
		/// Consulta os itens no repositório.
		/// </summary>
		/// <param name="path">Caminho que será pesquisa.</param>
		/// <param name="searchPattern">Padrão que será usada na comparação da pesquisa.</param>
		/// <param name="itemType">Tipo do item.</param>
		/// <param name="searchOptions"></param>
		/// <returns></returns>
		IEnumerable<IItem> QueryItems(string path, string searchPattern, ItemType itemType, SearchOption searchOptions);

		/// <summary>
		/// Recupera o item associado com o caminho informado.
		/// </summary>
		/// <param name="path">Caminho do item.</param>
		/// <param name="itemType">Tipo do item.</param>
		/// <returns></returns>
		IItem GetItem(string path, ItemType itemType);

		/// <summary>
		/// Cria um diretório.
		/// </summary>
		/// <param name="path"></param>
		IItem CreateFolder(string path);

		/// <summary>
		/// Verifica se o caminho informado existe no repositório.
		/// </summary>
		/// <param name="path">Caminho do item que será verificado.</param>
		/// <param name="itemType">Tipo do item que será verificado.</param>
		/// <returns></returns>
		bool Exists(string path, ItemType itemType);

		/// <summary>
		/// Remove o item do repositório.
		/// </summary>
		/// <param name="item">Item que será removido.</param>
		/// <param name="recursive">Identifiac se para remove os itens filhos recursivamente.</param>
		void Delete(IItem item, bool recursive);

		/// <summary>
		/// Move o item para o caminho de destino informado.
		/// </summary>
		/// <param name="sourceItem"></param>
		/// <param name="destPath"></param>
		void Move(IItem sourceItem, string destPath);

		/// <summary>
		/// Atualiza os dados do repositório.
		/// </summary>
		void Refresh();
	}
}
