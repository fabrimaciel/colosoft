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
	internal class JsonToolBar
	{
		public bool add
		{
			get;
			set;
		}

		public bool cloneToTop
		{
			get;
			set;
		}

		public bool del
		{
			get;
			set;
		}

		public bool edit
		{
			get;
			set;
		}

		public string position
		{
			get;
			set;
		}

		public bool refresh
		{
			get;
			set;
		}

		public bool search
		{
			get;
			set;
		}

		public bool view
		{
			get;
			set;
		}

		public JsonToolBar(ToolBarSettings settings)
		{
			this.edit = settings.ShowEditButton;
			this.add = settings.ShowAddButton;
			this.del = settings.ShowDeleteButton;
			this.search = settings.ShowSearchButton;
			this.refresh = settings.ShowRefreshButton;
			this.view = settings.ShowViewRowDetailsButton;
			this.position = settings.ToolBarAlign.ToString().ToLower();
			this.cloneToTop = true;
		}
	}
}
