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
	/// <summary>
	/// Classe que formata os dados para o Dialog de exclusão.
	/// </summary>
	internal class JsonDelDialog
	{
		private GridView _grid;

		private Hashtable _jsonValues = new Hashtable();

		public JsonDelDialog(GridView grid)
		{
			_grid = grid;
		}

		public void Process()
		{
			DeleteDialogSettings deleteDialogSettings = _grid.DeleteDialogSettings;
			if(deleteDialogSettings.TopOffset != 0)
			{
				_jsonValues["top"] = deleteDialogSettings.TopOffset;
			}
			if(deleteDialogSettings.LeftOffset != 0)
			{
				_jsonValues["left"] = deleteDialogSettings.LeftOffset;
			}
			if(deleteDialogSettings.Width != 300)
			{
				_jsonValues["width"] = deleteDialogSettings.Width;
			}
			if(deleteDialogSettings.Height != 300)
			{
				_jsonValues["height"] = deleteDialogSettings.Height;
			}
			if(deleteDialogSettings.Modal)
			{
				_jsonValues["modal"] = true;
			}
			if(!deleteDialogSettings.Draggable)
			{
				_jsonValues["drag"] = false;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.SubmitText))
			{
				_jsonValues["bSubmit"] = deleteDialogSettings.SubmitText;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.CancelText))
			{
				_jsonValues["bCancel"] = deleteDialogSettings.CancelText;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.LoadingMessageText))
			{
				_jsonValues["processData"] = deleteDialogSettings.LoadingMessageText;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.Caption))
			{
				_jsonValues["caption"] = deleteDialogSettings.Caption;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.DeleteMessage))
			{
				_jsonValues["msg"] = deleteDialogSettings.DeleteMessage;
			}
			if(!deleteDialogSettings.ReloadAfterSubmit)
			{
				_jsonValues["reloadAfterSubmit"] = false;
			}
			if(!deleteDialogSettings.Resizable)
			{
				_jsonValues["resize"] = false;
			}
			if(!string.IsNullOrEmpty(deleteDialogSettings.FunctionErrorTextFormat))
			{
				_jsonValues["errorTextFormat"] = deleteDialogSettings.FunctionErrorTextFormat;
			}
		}

		/// <summary>
		/// Remove as aspas dos método javascript.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string RemoveQuotesForJavaScriptMethods(string input)
		{
			return input.Replace("\"errorTextFormat\":\"" + _grid.DeleteDialogSettings.FunctionErrorTextFormat + "\"", "\"errorTextFormat\":" + _grid.DeleteDialogSettings.FunctionErrorTextFormat);
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
