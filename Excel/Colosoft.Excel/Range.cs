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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Colosoft.Excel.ExcelXml.Extensions;

namespace Colosoft.Excel.ExcelXml
{
	/// <summary>
	/// Defines a range of cells
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class Range : Styles, IEnumerable<Cell>
	{
		internal Cell CellFrom;

		internal Cell CellTo;

		internal string UnresolvedRangeReference;

		/// <summary>
		/// Gets or sets the range's flag to return a absolute reference or otherwise
		/// </summary>
		public bool Absolute
		{
			get;
			set;
		}

		private string name;

		/// <summary>
		/// Gets or sets the name of the range
		/// </summary>
		/// <remarks>This property always adds global (i.e. Workbook level)
		/// named ranges. To add sheet limited ranges, use Worksheet's
		/// AddNamedRange method of <see cref="Colosoft.Excel.ExcelXml.Worksheet"/>
		/// class.</remarks>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if(name != value)
				{
					if(value.IsNullOrEmpty() || IsSystemRangeName(value))
						throw new ArgumentException("name");
					name = value;
					CellFrom.GetParentBook().AddNamedRange(this, name);
				}
			}
		}

		/// <summary>
		/// Gets the number of rows in a range
		/// </summary>
		/// <returns>Number of rows in a range</returns>
		public int RowCount
		{
			get
			{
				if(CellFrom == null)
					return 0;
				if(CellTo == null)
					return 1;
				int rowFrom = CellFrom.ParentRow.RowIndex;
				int rowTo = CellTo.ParentRow.RowIndex;
				return (rowTo - rowFrom) + 1;
			}
		}

		/// <summary>
		/// Gets the number of columns in a range
		/// </summary>
		/// <returns>Number of columns in a range</returns>
		public int ColumnCount
		{
			get
			{
				if(CellFrom == null)
					return 0;
				if(CellTo == null)
					return 1;
				int cellIndexFrom = CellFrom.CellIndex;
				int cellIndexTo = CellTo.CellIndex;
				return (cellIndexTo - cellIndexFrom) + 1;
			}
		}

		/// <summary>
		/// Defines a unresolved range
		/// </summary>
		/// <param name="range">Unresolved range address</param>
		internal Range(string range)
		{
			if(range[0] == '=')
				range = range.Substring(1);
			UnresolvedRangeReference = range;
		}

		/// <summary>
		/// Defines a range
		/// </summary>
		/// <param name="cell">A single cell as a range</param>
		public Range(Cell cell)
		{
			CellFrom = cell;
			UnresolvedRangeReference = "";
		}

		/// <summary>
		/// Defines a range
		/// </summary>
		/// <param name="cellFrom">Starting cell</param>
		/// <param name="cellTo">Ending cell</param>
		/// <remarks>Defines a rectangular area of a sheet with a starting cell and a ending cell</remarks>
		public Range(Cell cellFrom, Cell cellTo)
		{
			UnresolvedRangeReference = "";
			if(cellTo == null)
			{
				CellFrom = cellFrom;
				return;
			}
			if(cellFrom.ParentRow.ParentSheet != cellTo.ParentRow.ParentSheet)
				throw new ArgumentException("cellFrom and cellTo's parent worksheets should be same");
			if(cellFrom == cellTo)
			{
				CellFrom = cellFrom;
				return;
			}
			int rowFrom = cellFrom.ParentRow.RowIndex;
			int rowTo = cellTo.ParentRow.RowIndex;
			int cellIndexFrom = cellFrom.CellIndex;
			int cellIndexTo = cellTo.CellIndex;
			if(rowFrom > rowTo || cellIndexFrom > cellIndexTo)
			{
				CellFrom = cellTo;
				CellTo = cellFrom;
			}
			else
			{
				CellFrom = cellFrom;
				CellTo = cellTo;
			}
		}

		private string AbsoluteReference()
		{
			string range = String.Format(CultureInfo.InvariantCulture, "R{0}C{1}", CellFrom.ParentRow.RowIndex + 1, CellFrom.CellIndex + 1);
			if(CellFrom != null)
				range += String.Format(CultureInfo.InvariantCulture, ":R{0}C{1}", CellTo.ParentRow.RowIndex + 1, CellTo.CellIndex + 1);
			return range;
		}

		internal bool Match(Range range)
		{
			if(range.CellFrom == CellFrom && range.CellTo == CellTo)
				return true;
			return false;
		}

		internal override ExcelXmlWorkbook GetParentBook()
		{
			return null;
		}

		internal override Cell FirstCell()
		{
			return CellFrom;
		}

		internal override void IterateAndApply(IterateFunction applyStyleFunction)
		{
			if(CellFrom == null)
				return;
			if(CellTo == null)
			{
				applyStyleFunction(CellFrom);
				return;
			}
			int rowFrom = CellFrom.ParentRow.RowIndex;
			int rowTo = CellTo.ParentRow.RowIndex;
			int cellIndexFrom = CellFrom.CellIndex;
			int cellIndexTo = CellTo.CellIndex;
			Worksheet ws = CellFrom.ParentRow.ParentSheet;
			for(int i = rowFrom; i <= rowTo; i++)
			{
				for(int j = cellIndexFrom; j <= cellIndexTo; j++)
				{
					applyStyleFunction(ws[j, i]);
				}
			}
		}

		internal string NamedRangeReference(bool sheetReference)
		{
			if(CellFrom == null)
				return UnresolvedRangeReference;
			string range = "";
			if(sheetReference)
				range = "'" + CellFrom.ParentRow.ParentSheet.Name + "'!";
			range += AbsoluteReference();
			return range;
		}

		internal string RangeReference(Cell cell)
		{
			if(CellFrom == null)
				return UnresolvedRangeReference;
			if(CellFrom.ParentRow == null)
				return "#N/A";
			if(CellTo != null && CellTo.ParentRow == null)
				return "#N/A";
			if(cell == null)
				throw new ArgumentNullException("cell");
			string range;
			if(Absolute)
			{
				range = AbsoluteReference();
			}
			else
			{
				if(CellTo != null)
				{
					range = String.Format(CultureInfo.InvariantCulture, "R[{0}]C[{1}]:R[{2}]C[{3}]", CellFrom.ParentRow.RowIndex - cell.ParentRow.RowIndex, CellFrom.CellIndex - cell.CellIndex, CellTo.ParentRow.RowIndex - cell.ParentRow.RowIndex, CellTo.CellIndex - cell.CellIndex);
				}
				else
				{
					range = String.Format(CultureInfo.InvariantCulture, "R[{0}]C[{1}]", CellFrom.ParentRow.RowIndex - cell.ParentRow.RowIndex, CellFrom.CellIndex - cell.CellIndex);
				}
			}
			string sheetReference = "";
			if(CellFrom.ParentRow.ParentSheet != cell.ParentRow.ParentSheet)
			{
				sheetReference = CellFrom.ParentRow.ParentSheet.Name;
				ExcelXmlWorkbook workBook = CellFrom.GetParentBook();
				if(workBook != cell.GetParentBook())
					throw new ArgumentException("External workbook references are not supported");
			}
			if(!sheetReference.IsNullOrEmpty())
			{
				range = "'" + sheetReference + "'!" + range;
			}
			return range;
		}

		internal void ParseUnresolvedReference(Cell cell)
		{
			if(UnresolvedRangeReference.IsNullOrEmpty())
				return;
			Match match;
			ParseArgumentType pat = FormulaParser.GetArgumentType(UnresolvedRangeReference, out match);
			Range range;
			if(cell == null)
				throw new ArgumentNullException("cell");
			bool parsed = FormulaParser.ParseRange(cell, match, out range, pat == ParseArgumentType.AbsoluteRange);
			if(parsed)
			{
				UnresolvedRangeReference = "";
				CellFrom = range.CellFrom;
				CellTo = range.CellTo;
			}
		}

		internal static bool IsSystemRangeName(string name)
		{
			if(name == "Print_Titles" || name == "_FilterDatabase" || name == "Print_Area")
				return true;
			return false;
		}

		/// <summary>
		/// Get a cell enumerator
		/// </summary>
		/// <returns>returns IEnumerator&gt;Cell&lt;</returns>
		public IEnumerator<Cell> GetEnumerator()
		{
			if(CellFrom != null)
			{
				if(CellTo == null)
				{
					yield return CellFrom;
				}
				else
				{
					int rowFrom = CellFrom.ParentRow.RowIndex;
					int rowTo = CellTo.ParentRow.RowIndex;
					int cellIndexFrom = CellFrom.CellIndex;
					int cellIndexTo = CellTo.CellIndex;
					Worksheet ws = CellFrom.ParentRow.ParentSheet;
					for(int i = rowFrom; i <= rowTo; i++)
					{
						for(int j = cellIndexFrom; j <= cellIndexTo; j++)
						{
							yield return ws[j, i];
						}
					}
				}
			}
		}

		/// <summary>
		/// Get a object enumerator
		/// </summary>
		/// <returns>returns IEnumerator&gt;Cell&lt;</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks if a particular cell is present in a range or not
		/// </summary>
		/// <param name="cell">Cell to check</param>
		/// <returns>true if cell is present, false otherwise</returns>
		public bool Contains(Cell cell)
		{
			if(CellFrom == null)
				return false;
			if(CellFrom.ParentRow.ParentSheet != cell.ParentRow.ParentSheet)
				return false;
			if(CellTo == null)
				return CellFrom == cell;
			int rowFrom = CellFrom.ParentRow.RowIndex;
			int rowTo = CellTo.ParentRow.RowIndex;
			int cellIndexFrom = CellFrom.CellIndex;
			int cellIndexTo = CellTo.CellIndex;
			return (cell.ParentRow.RowIndex >= rowFrom && cell.ParentRow.RowIndex <= rowTo && cell.CellIndex >= cellIndexFrom && cell.CellIndex <= cellIndexTo);
		}

		/// <summary>
		/// Sets this range as a auto-filter range in the sheet
		/// </summary>
		public void AutoFilter()
		{
			CellFrom.ParentRow.ParentSheet.AutoFilter = true;
			CellFrom.GetParentBook().AddNamedRange(this, "_FilterDatabase", CellFrom.ParentRow.ParentSheet);
		}

		/// <summary>
		/// Sets this range as the current print area in the sheet
		/// </summary>
		public void SetAsPrintArea()
		{
			CellFrom.ParentRow.ParentSheet.PrintArea = true;
			CellFrom.GetParentBook().AddNamedRange(this, "Print_Area", CellFrom.ParentRow.ParentSheet);
		}

		/// <summary>
		/// Merges a range into one cell
		/// </summary>
		/// <returns>true if merge was successful, false otherwise</returns>
		public bool Merge()
		{
			if(CellFrom.MergeStart)
				return true;
			bool rangeHasMergedCells = false;
			IterateAndApply(cell => rangeHasMergedCells = cell.MergeStart);
			if(rangeHasMergedCells)
				return false;
			Worksheet ws = CellFrom.ParentRow.ParentSheet;
			ws._MergedCells.Add(this);
			CellFrom.MergeStart = true;
			CellFrom.ColumnSpan = ColumnCount;
			CellFrom.RowSpan = RowCount;
			return true;
		}

		/// <summary>
		/// Unmerges a merged range
		/// </summary>
		public void Unmerge()
		{
			if(!CellFrom.MergeStart)
				return;
			Worksheet ws = CellFrom.ParentRow.ParentSheet;
			ws._MergedCells.Remove(this);
			CellFrom.MergeStart = false;
			CellFrom.ColumnSpan = 1;
			CellFrom.RowSpan = 1;
		}
	}
}
