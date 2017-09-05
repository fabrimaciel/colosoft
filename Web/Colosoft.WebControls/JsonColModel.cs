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
using System.Web.UI.WebControls;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Classe usada para forma o model da coluna do Grid para JSON.
	/// </summary>
	internal class JsonColModel
	{
		private GridView _grid;

		private Hashtable _jsonValues;

		public JsonColModel(GridView grid)
		{
			_jsonValues = new Hashtable();
			_grid = grid;
		}

		public JsonColModel(Column column, GridView grid) : this(grid)
		{
			this.FromColumn(column);
		}

		private void ApplyFormatterOptions(Column column)
		{
			if((column.Formatter.Count > 0) && (column.Formatter[0] != null))
			{
				ColumnFormatter formatter = column.Formatter[0];
				Hashtable hashtable = new Hashtable();
				if(formatter is LinkFormatter)
				{
					LinkFormatter formatter2 = (LinkFormatter)formatter;
					_jsonValues["formatter"] = "link";
					if(!string.IsNullOrEmpty(formatter2.Target))
					{
						hashtable["target"] = formatter2.Target;
					}
				}
				if(formatter is EmailFormatter)
				{
					_jsonValues["formatter"] = "email";
				}
				if(formatter is IntegerFormatter)
				{
					IntegerFormatter formatter3 = (IntegerFormatter)formatter;
					_jsonValues["formatter"] = "integer";
					if(!string.IsNullOrEmpty(formatter3.ThousandsSeparator))
					{
						hashtable["thousandsSeparator"] = formatter3.ThousandsSeparator;
					}
					if(!string.IsNullOrEmpty(formatter3.DefaultValue))
					{
						hashtable["defaultValue"] = formatter3.DefaultValue;
					}
				}
				if(formatter is NumberFormatter)
				{
					NumberFormatter formatter4 = (NumberFormatter)formatter;
					_jsonValues["formatter"] = "integer";
					if(!string.IsNullOrEmpty(formatter4.ThousandsSeparator))
					{
						hashtable["thousandsSeparator"] = formatter4.ThousandsSeparator;
					}
					if(!string.IsNullOrEmpty(formatter4.DefaultValue))
					{
						hashtable["defaultValue"] = formatter4.DefaultValue;
					}
					if(!string.IsNullOrEmpty(formatter4.DecimalSeparator))
					{
						hashtable["decimalSeparator"] = formatter4.DecimalSeparator;
					}
					if(formatter4.DecimalPlaces != -1)
					{
						hashtable["decimalPlaces"] = formatter4.DecimalPlaces;
					}
				}
				if(formatter is CurrencyFormatter)
				{
					CurrencyFormatter formatter5 = (CurrencyFormatter)formatter;
					_jsonValues["formatter"] = "currency";
					if(!string.IsNullOrEmpty(formatter5.ThousandsSeparator))
					{
						hashtable["thousandsSeparator"] = formatter5.ThousandsSeparator;
					}
					if(!string.IsNullOrEmpty(formatter5.DefaultValue))
					{
						hashtable["defaultValue"] = formatter5.DefaultValue;
					}
					if(!string.IsNullOrEmpty(formatter5.DecimalSeparator))
					{
						hashtable["decimalSeparator"] = formatter5.DecimalSeparator;
					}
					if(formatter5.DecimalPlaces != -1)
					{
						hashtable["decimalPlaces"] = formatter5.DecimalPlaces;
					}
					if(!string.IsNullOrEmpty(formatter5.Prefix))
					{
						hashtable["prefix"] = formatter5.Prefix;
					}
					if(!string.IsNullOrEmpty(formatter5.Prefix))
					{
						hashtable["suffix"] = formatter5.Suffix;
					}
				}
				if(formatter is CheckBoxFormatter)
				{
					CheckBoxFormatter formatter6 = (CheckBoxFormatter)formatter;
					_jsonValues["formatter"] = "checkbox";
					if(formatter6.Enabled)
					{
						hashtable["disabled"] = false;
					}
				}
				if(formatter is CustomFormatter)
				{
					CustomFormatter formatter7 = (CustomFormatter)formatter;
					if(!string.IsNullOrEmpty(formatter7.FormatFunction))
					{
						_jsonValues["formatter"] = formatter7.FormatFunction;
					}
					if(!string.IsNullOrEmpty(formatter7.UnFormatFunction))
					{
						_jsonValues["unformat"] = formatter7.UnFormatFunction;
					}
				}
				if(hashtable.Count > 0)
				{
					_jsonValues["formatoptions"] = hashtable;
				}
			}
		}

		public void FromColumn(Column column)
		{
			string dataField = "";
			if(!string.IsNullOrEmpty(column.QualifiedName))
			{
				dataField = column.QualifiedName;
			}
			else if(!string.IsNullOrEmpty(column.HeaderText))
			{
				dataField = column.HeaderText;
			}
			else
			{
				dataField = _grid.Columns.IndexOf(column).ToString() + "_template";
			}
			_jsonValues["index"] = _jsonValues["name"] = dataField;
			if(column.Width != 150)
			{
				_jsonValues["width"] = column.Width;
			}
			if(!column.Sortable)
			{
				_jsonValues["sortable"] = false;
			}
			if(column.PrimaryKey)
			{
				_jsonValues["key"] = true;
			}
			if(!column.Visible)
			{
				_jsonValues["hidden"] = true;
			}
			if(!column.Searchable)
			{
				_jsonValues["search"] = false;
			}
			if(column.TextAlign != TextAlign.Left)
			{
				_jsonValues["align"] = column.TextAlign.ToString().ToLower();
			}
			if(!column.Resizable)
			{
				_jsonValues["resizable"] = false;
			}
			if(!string.IsNullOrEmpty(column.CssClass))
			{
				_jsonValues["classes"] = column.CssClass;
			}
			if(column.Formatter != null)
			{
				this.ApplyFormatterOptions(column);
			}
			if(column.EditActionIconsColumn)
			{
				this._jsonValues["formatter"] = "actions";
			}
			if(column.Searchable)
			{
				if(column.SearchType != SearchType.TextBox)
				{
					_jsonValues["stype"] = "select";
				}
				if(!string.IsNullOrEmpty(column.SearchValues))
				{
					Hashtable hashtable = new Hashtable();
					hashtable["value"] = column.SearchValues;
					_jsonValues["searchoptions"] = hashtable;
				}
				if((column.SearchType == SearchType.DropDown) && !string.IsNullOrEmpty(column.SearchControlID))
				{
					Hashtable hashtable2 = new Hashtable();
					DropDownList list = _grid.FindControlRecursive(_grid.Page, column.SearchControlID) as DropDownList;
					if(list == null)
					{
						throw new Exception("Cannot find a DropDownList control with ID = " + column.SearchControlID);
					}
					if(!string.IsNullOrEmpty(list.DataSourceID) || (list.DataSource != null))
					{
						list.DataBind();
					}
					StringBuilder builder = new StringBuilder();
					foreach (ListItem item in list.Items)
					{
						builder.AppendFormat("{0}:{1}", item.Value, item.Text);
						if(list.Items.IndexOf(item) < (list.Items.Count - 1))
						{
							builder.Append(";");
						}
					}
					list.Visible = false;
					hashtable2["value"] = builder.ToString();
					_jsonValues["searchoptions"] = hashtable2;
				}
			}
			if(column.Editable)
			{
				_jsonValues["editable"] = true;
				if(column.EditType != EditType.TextBox)
				{
					_jsonValues["edittype"] = this.GetEditType(column.EditType);
				}
				Hashtable hashtable3 = new Hashtable();
				foreach (EditFieldAttribute attribute in column.EditFieldAttributes)
				{
					hashtable3[attribute.Name] = attribute.Value;
				}
				if(!string.IsNullOrEmpty(column.EditValues))
				{
					hashtable3["value"] = column.EditValues;
				}
				if((column.EditType == EditType.DropDown) && !string.IsNullOrEmpty(column.EditorControlID))
				{
					DropDownList list2 = _grid.FindControlRecursive(_grid.Page, column.EditorControlID) as DropDownList;
					if(list2 == null)
					{
						throw new Exception("Cannot find a DropDownList control with ID = " + column.EditorControlID);
					}
					if(!string.IsNullOrEmpty(list2.DataSourceID) || (list2.DataSource != null))
					{
						list2.DataBind();
					}
					StringBuilder builder2 = new StringBuilder();
					foreach (ListItem item2 in list2.Items)
					{
						builder2.AppendFormat("{0}:{1}", item2.Value, item2.Text);
						if(list2.Items.IndexOf(item2) < (list2.Items.Count - 1))
						{
							builder2.Append(";");
						}
					}
					list2.Visible = false;
					hashtable3["value"] = builder2.ToString();
				}
				if(hashtable3.Count > 0)
				{
					_jsonValues["editoptions"] = hashtable3;
				}
				Hashtable hashtable4 = new Hashtable();
				if(column.EditDialogColumnPosition != 0)
				{
					hashtable4["colpos"] = column.EditDialogColumnPosition;
				}
				if(column.EditDialogRowPosition != 0)
				{
					hashtable4["rowpos"] = column.EditDialogRowPosition;
				}
				if(!string.IsNullOrEmpty(column.EditDialogLabel))
				{
					hashtable4["label"] = column.EditDialogLabel;
				}
				if(!string.IsNullOrEmpty(column.EditDialogFieldPrefix))
				{
					hashtable4["elmprefix"] = column.EditDialogFieldPrefix;
				}
				if(!string.IsNullOrEmpty(column.EditDialogFieldSuffix))
				{
					hashtable4["elmsuffix"] = column.EditDialogFieldSuffix;
				}
				if(hashtable4.Count > 0)
				{
					_jsonValues["formoptions"] = hashtable4;
				}
				Hashtable hashtable5 = new Hashtable();
				if(!column.Visible && column.Editable)
				{
					hashtable5["edithidden"] = true;
				}
				if(column.EditClientSideValidators != null)
				{
					foreach (EditClientSideValidator validator in column.EditClientSideValidators)
					{
						if(validator is DateValidator)
						{
							hashtable5["date"] = true;
						}
						if(validator is EmailValidator)
						{
							hashtable5["email"] = true;
						}
						if(validator is IntegerValidator)
						{
							hashtable5["integer"] = true;
						}
						if(validator is MaxValueValidator)
						{
							hashtable5["maxValue"] = ((MaxValueValidator)validator).MaxValue;
						}
						if(validator is MinValueValidator)
						{
							hashtable5["minValue"] = ((MinValueValidator)validator).MinValue;
						}
						if(validator is NumberValidator)
						{
							hashtable5["number"] = true;
						}
						if(validator is RequiredValidator)
						{
							hashtable5["required"] = true;
						}
						if(validator is TimeValidator)
						{
							hashtable5["time"] = true;
						}
						if(validator is UrlValidator)
						{
							hashtable5["url"] = true;
						}
						if(validator is CustomValidator)
						{
							hashtable5["custom"] = true;
							hashtable5["custom_func"] = ((CustomValidator)validator).ValidationFunction;
						}
					}
				}
				if(hashtable5.Count > 0)
				{
					_jsonValues["editrules"] = hashtable5;
				}
			}
		}

		private string GetEditType(EditType type)
		{
			switch(type)
			{
			case EditType.TextBox:
				return "text";
			case EditType.CheckBox:
				return "checkbox";
			case EditType.DropDown:
				return "select";
			case EditType.TextArea:
				return "textarea";
			case EditType.Password:
				return "password";
			}
			return "text";
		}

		public static string RemoveQuotesForJavaScriptMethods(string input, GridView grid)
		{
			string str = input;
			foreach (Column column in grid.Columns)
			{
				if(((column.Formatter != null) && (column.Formatter.Count > 0)) && (column.Formatter[0] != null))
				{
					ColumnFormatter formatter = column.Formatter[0];
					if(formatter is CustomFormatter)
					{
						CustomFormatter formatter2 = (CustomFormatter)formatter;
						string oldValue = string.Format("\"formatter\":\"{0}\"", formatter2.FormatFunction);
						string newValue = string.Format("\"formatter\":{0}", formatter2.FormatFunction);
						str = str.Replace(oldValue, newValue);
						oldValue = string.Format("\"unformat\":\"{0}\"", formatter2.UnFormatFunction);
						newValue = string.Format("\"unformat\":{0}", formatter2.UnFormatFunction);
						str = str.Replace(oldValue, newValue);
					}
				}
				foreach (EditClientSideValidator validator in column.EditClientSideValidators)
				{
					if(validator is CustomValidator)
					{
						var validator2 = (CustomValidator)validator;
						string str4 = string.Format("\"custom_func\":\"{0}\"", validator2.ValidationFunction);
						string str5 = string.Format("\"custom_func\":{0}", validator2.ValidationFunction);
						str = str.Replace(str4, str5);
					}
				}
			}
			return str;
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
