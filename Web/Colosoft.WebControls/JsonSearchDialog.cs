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
using System.Collections;

namespace Colosoft.WebControls.GridView
{
	internal class JsonSearchDialog
	{
		private GridView _grid;

		private Hashtable _jsonValues = new Hashtable();

		public JsonSearchDialog(GridView grid)
		{
			_grid = grid;
		}

		public void Process()
		{
			SearchDialogSettings searchDialogSettings = _grid.SearchDialogSettings;
			if(searchDialogSettings.TopOffset != 0)
			{
				_jsonValues["top"] = searchDialogSettings.TopOffset;
			}
			if(searchDialogSettings.LeftOffset != 0)
			{
				_jsonValues["left"] = searchDialogSettings.LeftOffset;
			}
			if(searchDialogSettings.Width != 300)
			{
				_jsonValues["width"] = searchDialogSettings.Width;
			}
			if(searchDialogSettings.Height != 300)
			{
				_jsonValues["height"] = searchDialogSettings.Height;
			}
			if(searchDialogSettings.Modal)
			{
				_jsonValues["modal"] = true;
			}
			if(!searchDialogSettings.Draggable)
			{
				_jsonValues["drag"] = false;
			}
			if(!string.IsNullOrEmpty(searchDialogSettings.FindButtonText))
			{
				_jsonValues["Find"] = searchDialogSettings.FindButtonText;
			}
			if(!string.IsNullOrEmpty(searchDialogSettings.ResetButtonText))
			{
				_jsonValues["Clear"] = searchDialogSettings.ResetButtonText;
			}
			if(searchDialogSettings.MultipleSearch)
			{
				_jsonValues["multipleSearch"] = true;
			}
			if(searchDialogSettings.ValidateInput)
			{
				_jsonValues["checkInput"] = true;
			}
			if(!searchDialogSettings.Resizable)
			{
				_jsonValues["resize"] = false;
			}
		}

		public Hashtable JsonValues
		{
			get
			{
				return _jsonValues;
			}
			set
			{
				_jsonValues = value;
			}
		}
	}
}
