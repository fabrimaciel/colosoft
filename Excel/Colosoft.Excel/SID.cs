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
using System.Text;

namespace Colosoft.Excel
{
	public class SID
	{
		/// <summary>
		/// Free SID,  Free sector
		/// (may exist in the file, but is not part of any stream)
		/// </summary>
		public const int Free = -1;

		/// <summary>
		/// End Of Chain SID
		/// </summary>
		public const int EOC = -2;

		/// <summary>
		/// SAT SID, Sector is used by the sector allocation table
		/// </summary>
		public const int SAT = -3;

		/// <summary>
		/// MSAT SID, Sector is used by the master sector allocation table
		/// </summary>
		public const int MSAT = -4;
	}
}
