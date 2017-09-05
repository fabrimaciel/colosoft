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

namespace Colosoft.Excel.ExcelXml
{
	/// <summary>
	/// Represents a strongly typed list of cells that can be accessed by index.
	/// </summary>
	public class CellCollection : List<Cell>
	{
		/// <summary>
		/// Adds a range to the collection
		/// </summary>
		/// <param name="range">Range to add</param>
		public void Add(Range range)
		{
			foreach (Cell cell in range)
				Add(cell);
		}

		/// <summary>
		/// Adds a worksheet to the collection
		/// </summary>
		/// <param name="ws">Worksheet to add</param>
		public void Add(Worksheet ws)
		{
			foreach (Cell cell in ws)
				Add(cell);
		}

		/// <summary>
		/// Adds a row to the collection
		/// </summary>
		/// <param name="row">Row to add</param>
		public void Add(Row row)
		{
			foreach (Cell cell in row)
				Add(cell);
		}

		/// <summary>
		/// Adds a single cell to the collection if it matches the filter condition
		/// </summary>
		/// <param name="cell">Cell to add</param>
		/// <param name="filterCondition">Filter predicate</param>
		public void Add(Cell cell, Predicate<Cell> filterCondition)
		{
			if(filterCondition(cell))
				Add(cell);
		}

		/// <summary>
		/// Adds a range to the collection if it matches the filter condition
		/// </summary>
		/// <param name="range">Range to add</param>
		/// <param name="filterCondition">Filter predicate</param>
		public void Add(Range range, Predicate<Cell> filterCondition)
		{
			foreach (Cell cell in range)
			{
				if(filterCondition(cell))
					Add(cell);
			}
		}

		/// <summary>
		/// Adds a worksheet to the collection if it matches the filter condition
		/// </summary>
		/// <param name="ws">Worksheet to add</param>
		/// <param name="filterCondition">Filter predicate</param>
		public void Add(Worksheet ws, Predicate<Cell> filterCondition)
		{
			foreach (Cell cell in ws)
			{
				if(filterCondition(cell))
					Add(cell);
			}
		}

		/// <summary>
		/// Adds a row to the collection if it matches the filter condition
		/// </summary>
		/// <param name="row">Row to add</param>
		/// <param name="filterCondition">Filter predicate</param>
		public void Add(Row row, Predicate<Cell> filterCondition)
		{
			foreach (Cell cell in row)
			{
				if(filterCondition(cell))
					Add(cell);
			}
		}
	}
}
