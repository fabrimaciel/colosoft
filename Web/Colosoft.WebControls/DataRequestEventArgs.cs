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

namespace Colosoft.WebControls.GridView
{
	public class DataRequestEventArgs : EventArgs
	{
		private int _newPageIndex;

		private int _totalRows;

		public DataRequestEventArgs(string sortExpression, string sortDirection, int newPageIndex, string searchExpression, string parentRowKey)
		{
			this.SortExpression = sortExpression;
			this._newPageIndex = newPageIndex;
			this.SearchExpression = searchExpression;
			this.ParentRowKey = parentRowKey;
			if(sortDirection == null)
			{
				sortDirection = string.Empty;
			}
			this.SortDirection = (sortDirection.ToLower() == "asc") ? SortDirection.Asc : SortDirection.Desc;
		}

		public int NewPageIndex
		{
			get
			{
				return this._newPageIndex;
			}
			set
			{
				if(value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._newPageIndex = value;
			}
		}

		public string ParentRowKey
		{
			get;
			set;
		}

		public string SearchExpression
		{
			get;
			set;
		}

		public SortDirection SortDirection
		{
			get;
			set;
		}

		public string SortExpression
		{
			get;
			set;
		}

		public int TotalRows
		{
			get
			{
				return this._totalRows;
			}
			set
			{
				if(value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._totalRows = value;
			}
		}
	}
}
