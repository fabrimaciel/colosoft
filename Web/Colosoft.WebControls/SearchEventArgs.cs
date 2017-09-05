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
	public class SearchEventArgs : CancelEventArgs
	{
		private string _searchColumn;

		private SearchOperation _searchOperation;

		private string _searchString;

		public SearchEventArgs()
		{
		}

		public SearchEventArgs(string searchColumn, string searchString, SearchOperation searchOperation) : this()
		{
			this._searchColumn = searchColumn;
			this._searchString = searchString;
			this._searchOperation = searchOperation;
		}

		public string SearchColumn
		{
			get
			{
				return this._searchColumn;
			}
			set
			{
				this._searchColumn = value;
			}
		}

		public SearchOperation SearchOperation
		{
			get
			{
				return this._searchOperation;
			}
			set
			{
				this._searchOperation = value;
			}
		}

		public string SearchString
		{
			get
			{
				return this._searchString;
			}
			set
			{
				this._searchString = value;
			}
		}
	}
}
