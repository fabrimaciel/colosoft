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
	/// Assinatura das classes responsáveis pela carga do repositório.
	/// </summary>
	public interface IRepositoryLoader
	{
		/// <summary>
		/// Recupera todos os campos do esquema.
		/// </summary>
		/// <returns></returns>
		IEnumerable<SchemeField> GetSchemeFields();

		/// <summary>
		/// Recupera todos os campos do esquema index.
		/// </summary>
		/// <returns></returns>
		IEnumerable<SchemeIndex> GetSchemeIndex();

		/// <summary>
		/// Recupera todos os canais.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Channel> GetChannels();

		/// <summary>
		/// Recupera os elemento que serão armazenados pelo repositório.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Element> GetElements();

		/// <summary>
		/// Recupera os elementos com base na data de corte.
		/// </summary>
		/// <param name="cutoffDate"></param>
		/// <returns></returns>
		IEnumerable<Element> GetElements(DateTime cutoffDate);

		/// <summary>
		/// Recupera os elementos que foram removidos com base na date de corte.
		/// </summary>
		/// <param name="cutoffDate"></param>
		/// <returns></returns>
		IEnumerable<int> GetRemoved(DateTime cutoffDate);

		/// <summary>
		/// Verifica se é para limpar todos anúncios publicados no portal.
		/// </summary>
		/// <returns></returns>
		bool CheckClearAllElements();

		/// <summary>
		/// Confirma a limpeza de todos os elementos
		/// </summary>
		void ConfirmClearAllElements();

		/// <summary>
		/// Recupera os itens do dicionário.
		/// </summary>
		/// <returns></returns>
		IEnumerable<KeyValuePair<string, string>> GetReplaceItems();
	}
}
