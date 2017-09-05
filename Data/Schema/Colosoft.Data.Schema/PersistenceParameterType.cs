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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Identifica o tipo de paramentro que a propriedade representa.
	/// </summary>
	public enum PersistenceParameterType : int
	{
		/// <summary>
		/// Idetifica um campo normal.
		/// </summary>
		Field = 1,
		/// <summary>
		/// Identifica um campo do tipo chave primária.
		/// </summary>
		Key = 2,
		/// <summary>
		/// Identifica um campo do tipo chave primária identidade.
		/// </summary>
		IdentityKey = 3
	}
}
