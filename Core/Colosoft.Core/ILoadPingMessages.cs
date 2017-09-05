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

namespace Colosoft.Security
{
	/// <summary>
	/// Contrato para carga de mensagens.
	/// </summary>
	public interface ILoadPingMessages
	{
		/// <summary>
		/// Carrega as mensagens do usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário.</param>
		/// <param name="tokenId">Identificador do token.</param>
		/// <returns></returns>
		IEnumerable<IPingMessage> GetMessages(int userId, int tokenId);

		/// <summary>
		/// Marca os despachos como lidos.
		/// </summary>
		/// <param name="dispatcherIds">Enumerador com os ids dos despachos.</param>
		void MarkAsRead(IEnumerable<int> dispatcherIds);
	}
}
