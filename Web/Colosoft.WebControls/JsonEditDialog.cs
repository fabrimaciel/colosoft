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
using System.Collections;

namespace Colosoft.WebControls.GridView
{
	internal class JsonEditDialog
	{
		private GridView _grid;

		private Hashtable _jsonValues = new Hashtable();

		public JsonEditDialog(GridView grid)
		{
			_grid = grid;
		}

		public string Process()
		{
			EditDialogSettings editDialogSettings = this._grid.EditDialogSettings;
			if(editDialogSettings.TopOffset != 0)
			{
				this._jsonValues["top"] = editDialogSettings.TopOffset;
			}
			if(editDialogSettings.LeftOffset != 0)
			{
				this._jsonValues["left"] = editDialogSettings.LeftOffset;
			}
			if(editDialogSettings.Width != 300)
			{
				this._jsonValues["width"] = editDialogSettings.Width;
			}
			if(editDialogSettings.Height != 300)
			{
				this._jsonValues["height"] = editDialogSettings.Height;
			}
			if(editDialogSettings.Modal)
			{
				this._jsonValues["modal"] = true;
			}
			if(!editDialogSettings.Draggable)
			{
				this._jsonValues["drag"] = false;
			}
			if(!string.IsNullOrEmpty(editDialogSettings.Caption))
			{
				this._jsonValues["editCaption"] = editDialogSettings.Caption;
			}
			if(!string.IsNullOrEmpty(editDialogSettings.SubmitText))
			{
				this._jsonValues["bSubmit"] = editDialogSettings.SubmitText;
			}
			if(!string.IsNullOrEmpty(editDialogSettings.CancelText))
			{
				this._jsonValues["bCancel"] = editDialogSettings.CancelText;
			}
			if(!string.IsNullOrEmpty(editDialogSettings.LoadingMessageText))
			{
				this._jsonValues["processData"] = editDialogSettings.LoadingMessageText;
			}
			if(editDialogSettings.CloseAfterEditing)
			{
				this._jsonValues["closeAfterEdit"] = true;
			}
			if(!editDialogSettings.ReloadAfterSubmit)
			{
				this._jsonValues["reloadAfterSubmit"] = false;
			}
			if(!editDialogSettings.Resizable)
			{
				this._jsonValues["resize"] = false;
			}
			this._jsonValues["recreateForm"] = true;
			string str = new JavaScriptSerializer().Serialize(this._jsonValues);
			bool flag = str.Length > 2;
			StringBuilder builder = new StringBuilder();
			if(!string.IsNullOrEmpty(this._grid.ClientSideEvents.AfterEditDialogShown))
			{
				builder.AppendFormat("{0}afterShowForm: {1}", flag ? "," : "", this._grid.ClientSideEvents.AfterEditDialogShown);
				flag = true;
			}
			if(!string.IsNullOrEmpty(this._grid.ClientSideEvents.AfterEditDialogRowInserted))
			{
				builder.AppendFormat("{0}afterComplete: {1}", flag ? "," : "", this._grid.ClientSideEvents.AfterEditDialogRowInserted);
				flag = true;
			}
			builder.AppendFormat("{0}errorTextFormat: {1}", flag ? "," : "", "function(data) { return 'Error: ' + data.responseText }");
			return str.Insert(str.Length - 1, builder.ToString());
		}
	}
}
