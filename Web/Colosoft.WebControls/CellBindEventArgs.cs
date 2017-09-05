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
using System.ComponentModel;

namespace Colosoft.WebControls.GridView
{
	public class CellBindEventArgs : CancelEventArgs
	{
		private string _cellHtml;

		private int _columnIndex;

		private int _rowIndex;

		private string _rowKey;

		private object[] _rowValues;

		public CellBindEventArgs(string cellHtml, int columnIndex, int rowIndex, string rowKey, object[] rowValues)
		{
			this._cellHtml = cellHtml;
			this._columnIndex = columnIndex;
			this._rowIndex = rowIndex;
			this._rowKey = rowKey;
			this._rowValues = rowValues;
		}

		public string CellHtml
		{
			get
			{
				return this._cellHtml;
			}
			set
			{
				this._cellHtml = value;
			}
		}

		public int ColumnIndex
		{
			get
			{
				return this._columnIndex;
			}
			set
			{
				this._columnIndex = value;
			}
		}

		public int RowIndex
		{
			get
			{
				return this._rowIndex;
			}
			set
			{
				this._rowIndex = value;
			}
		}

		public string RowKey
		{
			get
			{
				return this._rowKey;
			}
			set
			{
				this._rowKey = value;
			}
		}

		public object[] RowValues
		{
			get
			{
				return this._rowValues;
			}
			set
			{
				this._rowValues = value;
			}
		}
	}
}
