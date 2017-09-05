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

namespace Colosoft.Excel.Csv
{
	/// <summary>
	/// Specifies the action to take when a parsing error has occured.
	/// </summary>
	public enum ParseErrorAction
	{
		/// <summary>
		/// Raises the <see cref="M:CsvReader.ParseError"/> event.
		/// </summary>
		RaiseEvent = 0,
		/// <summary>
		/// Tries to advance to next line.
		/// </summary>
		AdvanceToNextLine = 1,
		/// <summary>
		/// Throws an exception.
		/// </summary>
		ThrowException = 2,
	}
}
