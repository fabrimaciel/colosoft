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

namespace Colosoft.Serialization
{
	/// <summary>
	/// Representa um item de stream.
	/// </summary>
	public interface IStreamItem
	{
		/// <summary>
		/// Tamanho dos dados no stream.
		/// </summary>
		int Length
		{
			get;
			set;
		}

		/// <summary>
		/// Lê os dados do item.
		/// </summary>
		/// <param name="offset">Offset para recupera os dados.</param>
		/// <param name="length">Quantidade de dados que serão recuperados.</param>
		/// <returns>Dados lidos.</returns>
		VirtualArray Read(int offset, int length);

		/// <summary>
		/// Escreve o buffer no item.
		/// </summary>
		/// <param name="buffer">buffer com os dados que serão inseridos.</param>
		/// <param name="srcOffset">Offset da origem dom buffer.</param>
		/// <param name="dstOffset">Offset de destino.</param>
		/// <param name="length">Quantidade de dados que serão escritors</param>
		void Write(VirtualArray buffer, int srcOffset, int dstOffset, int length);
	}
}
