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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Assinatura de um repositório de entradas de dados do cache.
	/// </summary>
	public interface IDataEntriesRepository
	{
		/// <summary>
		/// Recupera as versões das entradas carregadas no cache.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DataEntryVersion> GetEntryVersions();

		/// <summary>
		/// Recupera as entradas carregadas para o cache.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DataEntry> GetEntries();

		/// <summary>
		/// Recupera as entradas carregadas para o cache.
		/// </summary>
		/// <param name="typeNames">Nomes do tipos da entradas que serão carregadas.</param>
		/// <returns></returns>
		IEnumerable<DataEntry> GetEntries(Colosoft.Reflection.TypeName[] typeNames);

		/// <summary>
		/// Insere uma nova entrada no repositório.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns>True caso a entrada tenha sido inserida com sucesso.</returns>
		bool Insert(DataEntry entry);

		/// <summary>
		/// Insere uma nova entrada no respositório
		/// </summary>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="stream">Stream com os dados.</param>
		/// <returns></returns>
		bool Insert(DataEntryVersion version, System.IO.Stream stream);
	}
}
