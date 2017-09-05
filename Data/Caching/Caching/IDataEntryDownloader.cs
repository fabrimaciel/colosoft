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
	/// Assinatura da classe responsável pelo download das entradas de dados do cache.
	/// </summary>
	public interface IDataEntryDownloader : Colosoft.Net.IDownloader
	{
		/// <summary>
		/// Adiciona a versão para realizar o download.
		/// </summary>
		/// <param name="version"></param>
		void Add(DataEntryVersion version);

		/// <summary>
		/// Adiciona uma faixa de versões para fazer o download.
		/// </summary>
		/// <param name="versions"></param>
		void AddRange(IEnumerable<DataEntryVersion> versions);

		/// <summary>
		/// Limpa as versões que foram registrada para ser feito o download.
		/// </summary>
		void Clear();
	}
}
