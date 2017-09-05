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

namespace Colosoft.Net
{
	/// <summary>
	/// Possíveis tipos de logon.
	/// </summary>
	public enum LogonType
	{
		/// <summary>
		/// Logon interativo.
		/// </summary>
		LOGON32_LOGON_INTERACTIVE = 2,
		/// <summary>
		/// Logon de rede.
		/// </summary>
		LOGON32_LOGON_NETWORK = 3,
		/// <summary>
		/// Logon de lote.
		/// </summary>
		LOGON32_LOGON_BATCH = 4,
		/// <summary>
		/// Logon de serviço.
		/// </summary>
		LOGON32_LOGON_SERVICE = 5,
		/// <summary>
		/// Logon de desbloqueio.
		/// </summary>
		LOGON32_LOGON_UNLOCK = 7,
		/// <summary>
		/// Logon de rede com texto limpo.
		/// </summary>
		LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
		/// <summary>
		/// Logon com novas credenciais.
		/// </summary>
		LOGON32_LOGON_NEW_CREDENTIALS = 9,
	}
}
