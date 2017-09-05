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

namespace Colosoft.Caching
{
	/// <summary>
	/// Possíveis tipos de lock de acesso.
	/// </summary>
	public enum LockAccessType : byte
	{
		/// <summary>
		/// 
		/// </summary>
		ACQUIRE = 1,
		/// <summary>
		/// Lock para comparação de versão.
		/// </summary>
		COMPARE_VERSION = 6,
		/// <summary>
		/// Padrão.
		/// </summary>
		DEFAULT = 10,
		/// <summary>
		/// 
		/// </summary>
		DONT_ACQUIRE = 2,
		/// <summary>
		/// 
		/// </summary>
		DONT_RELEASE = 4,
		/// <summary>
		/// Lock para recuperar a versão.
		/// </summary>
		GET_VERSION = 7,
		/// <summary>
		/// 
		/// </summary>
		IGNORE_LOCK = 5,
		/// <summary>
		/// Comparação de versão.
		/// </summary>
		MATCH_VERSION = 8,
		/// <summary>
		/// 
		/// </summary>
		PRESERVE_VERSION = 9,
		/// <summary>
		/// 
		/// </summary>
		RELEASE = 3
	}
}
