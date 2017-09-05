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
using System.Collections.Specialized;

namespace Colosoft.WebControls.GridView
{
	public class RowEditEventArgs : CancelEventArgs
	{
		private string _parentRowKey;

		private NameValueCollection _rowData;

		private string _rowKey;

		public string ParentRowKey
		{
			get
			{
				return this._parentRowKey;
			}
			set
			{
				this._parentRowKey = value;
			}
		}

		public NameValueCollection RowData
		{
			get
			{
				return this._rowData;
			}
			set
			{
				this._rowData = value;
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
	}
}
