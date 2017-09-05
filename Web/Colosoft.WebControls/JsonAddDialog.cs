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
using System.Web.Script.Serialization;

namespace Colosoft.WebControls.GridView
{
	internal class JsonAddDialog
	{
		private GridView _grid;

		private System.Collections.Hashtable _jsonValues = new System.Collections.Hashtable();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="grid"></param>
		public JsonAddDialog(GridView grid)
		{
			_grid = grid;
		}

		public string Process()
		{
			AddDialogSettings addDialogSettings = _grid.AddDialogSettings;
			if(addDialogSettings.TopOffset != 0)
				_jsonValues["top"] = addDialogSettings.TopOffset;
			if(addDialogSettings.LeftOffset != 0)
				_jsonValues["left"] = addDialogSettings.LeftOffset;
			if(addDialogSettings.Width != 300)
				_jsonValues["width"] = addDialogSettings.Width;
			if(addDialogSettings.Height != 300)
				_jsonValues["height"] = addDialogSettings.Height;
			if(addDialogSettings.Modal)
				_jsonValues["modal"] = true;
			if(!addDialogSettings.Draggable)
				_jsonValues["drag"] = false;
			if(!string.IsNullOrEmpty(addDialogSettings.Caption))
				_jsonValues["addCaption"] = addDialogSettings.Caption;
			if(!string.IsNullOrEmpty(addDialogSettings.SubmitText))
				_jsonValues["bSubmit"] = addDialogSettings.SubmitText;
			if(!string.IsNullOrEmpty(addDialogSettings.CancelText))
				_jsonValues["bCancel"] = addDialogSettings.CancelText;
			if(!string.IsNullOrEmpty(addDialogSettings.LoadingMessageText))
				_jsonValues["processData"] = addDialogSettings.LoadingMessageText;
			if(addDialogSettings.CloseAfterAdding)
				_jsonValues["closeAfterAdd"] = addDialogSettings.CloseAfterAdding;
			if(!addDialogSettings.ClearAfterAdding)
				_jsonValues["clearAfterAdding"] = false;
			if(!addDialogSettings.ReloadAfterSubmit)
				_jsonValues["reloadAfterSubmit"] = false;
			if(!addDialogSettings.Resizable)
				_jsonValues["resize"] = false;
			string str = new JavaScriptSerializer().Serialize(_jsonValues);
			bool flag = str.Length > 2;
			StringBuilder builder = new StringBuilder();
			if(!string.IsNullOrEmpty(_grid.ClientSideEvents.AfterAddDialogShown))
			{
				builder.AppendFormat("{0}afterShowForm: {1}", flag ? "," : "", _grid.ClientSideEvents.AfterEditDialogShown);
				flag = true;
			}
			if(!string.IsNullOrEmpty(_grid.ClientSideEvents.AfterAddDialogRowInserted))
			{
				builder.AppendFormat("{0}afterComplete: {1}", flag ? "," : "", _grid.ClientSideEvents.AfterAddDialogRowInserted);
				flag = true;
			}
			builder.AppendFormat("{0}errorTextFormat: {1}", flag ? "," : "", "function(data) { return 'Error: ' + data.responseText }");
			return str.Insert(str.Length - 1, builder.ToString());
		}
	}
}
