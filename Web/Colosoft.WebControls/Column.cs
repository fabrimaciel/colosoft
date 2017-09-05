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
using System.Web;
using System.Security.Permissions;
using System.Web.UI;
using System.Globalization;

namespace Colosoft.WebControls.GridView
{
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class Column : BaseItem
	{
		private EditClientSideValidatorCollection _editClientSideValidatorCollection;

		private EditFieldAttributeCollection _editFieldAttributeCollection;

		private ColumnFormatterCollection _formatterCollection;

		private IColumnValuesFormatter _valueFormatterInstance;

		[Category("Functionality"), DefaultValue(true), Description("Determines if empty string should be converted to null.")]
		public bool ConvertEmptyStringToNull
		{
			get
			{
				if(base.ViewState["ConvertEmptyStringToNull"] != null)
					return (bool)base.ViewState["ConvertEmptyStringToNull"];
				return true;
			}
			set
			{
				base.ViewState["ConvertEmptyStringToNull"] = value;
			}
		}

		[Category("Appearance"), Description("Custom CssClass to apply to column cells."), DefaultValue("")]
		public string CssClass
		{
			get
			{
				if(base.ViewState["CssClass"] != null)
					return (string)base.ViewState["CssClass"];
				return "";
			}
			set
			{
				base.ViewState["CssClass"] = value;
			}
		}

		[Category("Behavior"), Description("The name mapped to this column."), DefaultValue("")]
		public string QualifiedName
		{
			get
			{
				if(string.IsNullOrEmpty(FieldName))
					return DataField;
				return FieldName;
			}
		}

		[Category("Behavior"), Description("The field name mapped to this column."), DefaultValue("")]
		public string FieldName
		{
			get
			{
				if(base.ViewState["FieldName"] != null)
					return (string)base.ViewState["FieldName"];
				return "";
			}
			set
			{
				base.ViewState["FieldName"] = value;
			}
		}

		[Category("Behavior"), Description("The field from the datasource mapped to this column."), DefaultValue("")]
		public string DataField
		{
			get
			{
				if(base.ViewState["DataField"] != null)
					return (string)base.ViewState["DataField"];
				return "";
			}
			set
			{
				base.ViewState["DataField"] = value;
			}
		}

		[DefaultValue(""), Description("The format applied to the column value for displaying. Uses the standard String.Format syntax."), Category("Behavior")]
		public string DataFormatString
		{
			get
			{
				if(base.ViewState["DataFormatString"] != null)
					return (string)base.ViewState["DataFormatString"];
				return "";
			}
			set
			{
				base.ViewState["DataFormatString"] = value;
			}
		}

		[Category("Editing"), Description("Determines if the column is editable. Default is false."), DefaultValue(false)]
		public bool Editable
		{
			get
			{
				return ((base.ViewState["Editable"] != null) && ((bool)base.ViewState["Editable"]));
			}
			set
			{
				base.ViewState["Editable"] = value;
			}
		}

		[DefaultValue(false), Category("Editing"), MergableProperty(false), Description("Shows edit/save/cancel icons in the columns and forces the row in the respective edit actions based on end-user selection")]
		public bool EditActionIconsColumn
		{
			get
			{
				return ((base.ViewState["EditActionIconsColumn"] != null) && ((bool)base.ViewState["EditActionIconsColumn"]));
			}
			set
			{
				base.ViewState["EditActionIconsColumn"] = value;
			}
		}

		[MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Description("The client-side validators that will be applied to the cell value upon editing."), Category("Editing"), DefaultValue((string)null)]
		public EditClientSideValidatorCollection EditClientSideValidators
		{
			get
			{
				if(_editClientSideValidatorCollection == null)
					_editClientSideValidatorCollection = new EditClientSideValidatorCollection();
				return _editClientSideValidatorCollection;
			}
		}

		[DefaultValue(0), Category("Editing"), Description("The column in which the edit field for this column will be shown in the Edit Dialog")]
		public int EditDialogColumnPosition
		{
			get
			{
				if(base.ViewState["EditDialogColumnPosition"] != null)
					return (int)base.ViewState["EditDialogColumnPosition"];
				return 0;
			}
			set
			{
				base.ViewState["EditDialogColumnPosition"] = value;
			}
		}

		[Category("Editing"), DefaultValue(""), Description("Text shown before the editing field in the Edit Dialog. Default is none ('')")]
		public string EditDialogFieldPrefix
		{
			get
			{
				if(base.ViewState["EditDialogFieldPrefix"] != null)
					return (string)base.ViewState["EditDialogFieldPrefix"];
				return "";
			}
			set
			{
				base.ViewState["EditDialogFieldPrefix"] = value;
			}
		}

		[Category("Editing"), Description("Text shown after the editing field in the Edit Dialog. Default is none ('')."), DefaultValue("")]
		public string EditDialogFieldSuffix
		{
			get
			{
				if(base.ViewState["EditDialogFieldSuffix"] != null)
					return (string)base.ViewState["EditDialogFieldSuffix"];
				return "";
			}
			set
			{
				base.ViewState["EditDialogFieldSuffix"] = value;
			}
		}

		[DefaultValue(""), Description("The label of the editing field in edit dialog. Default to the column DataField if not set"), Category("Editing")]
		public string EditDialogLabel
		{
			get
			{
				if(base.ViewState["EditDialogLabel"] != null)
					return (string)base.ViewState["EditDialogLabel"];
				return "";
			}
			set
			{
				base.ViewState["EditDialogLabel"] = value;
			}
		}

		[Category("Editing"), DefaultValue(0), Description("The row in which the edit field for this column will be shown in the Edit Dialog")]
		public int EditDialogRowPosition
		{
			get
			{
				if(base.ViewState["EditDialogRowPosition"] != null)
				{
					return (int)base.ViewState["EditDialogRowPosition"];
				}
				return 0;
			}
			set
			{
				base.ViewState["EditDialogRowPosition"] = value;
			}
		}

		[DefaultValue((string)null), Category("Editing"), PersistenceMode(PersistenceMode.InnerProperty), Description("The HTML attributes (name/value pairs) that will be applied to the edit field of the respective column."), MergableProperty(false)]
		public EditFieldAttributeCollection EditFieldAttributes
		{
			get
			{
				if(this._editFieldAttributeCollection == null)
				{
					this._editFieldAttributeCollection = new EditFieldAttributeCollection();
				}
				return this._editFieldAttributeCollection;
			}
		}

		[DefaultValue(""), IDReferenceProperty, Description("The ID of the server-side asp:dropdownlist to be used for editing"), Category("Editing")]
		public string EditorControlID
		{
			get
			{
				if(base.ViewState["EditDropDownListControlID"] != null)
					return (string)base.ViewState["EditDropDownListControlID"];
				return "";
			}
			set
			{
				base.ViewState["EditDropDownListControlID"] = value;
			}
		}

		[DefaultValue(0), Category("Editing"), Description("The type of the editing field for this column - TextBox, TextArea, DropDown, CheckBox, etc")]
		public EditType EditType
		{
			get
			{
				if(base.ViewState["EditType"] != null)
					return (EditType)base.ViewState["EditType"];
				return EditType.TextBox;
			}
			set
			{
				base.ViewState["EditType"] = value;
			}
		}

		[Description("The list of values when EditType = DropDown. Name/value pairs separated with ':', e.g. 'TNT:TNT;DHL:DHL;UPS:UPS'"), Category("Editing"), DefaultValue("")]
		public string EditValues
		{
			get
			{
				if(base.ViewState["EditValues"] != null)
					return (string)base.ViewState["EditValues"];
				return "";
			}
			set
			{
				base.ViewState["EditValues"] = value;
			}
		}

		[Description("What to display in the column footer. AppearanceSettings.ShowFooter must be set to True for this to have effect."), Category("Appearance"), DefaultValue("")]
		public string FooterValue
		{
			get
			{
				if(base.ViewState["FooterValue"] != null)
					return (string)base.ViewState["FooterValue"];
				return "";
			}
			set
			{
				base.ViewState["FooterValue"] = value;
			}
		}

		[MergableProperty(false), Category("Functionality"), PersistenceMode(PersistenceMode.InnerProperty), Description("The formatter (client-side template) that will be applied to cells of this column."), DefaultValue((string)null)]
		public ColumnFormatterCollection Formatter
		{
			get
			{
				if(this._formatterCollection == null)
					this._formatterCollection = new ColumnFormatterCollection();
				return this._formatterCollection;
			}
		}

		[DefaultValue(""), Description("The text of the column header. If empty (default) the DataField value is used"), Category("Appearance")]
		public string HeaderText
		{
			get
			{
				if(base.ViewState["HeaderText"] != null)
					return (string)base.ViewState["HeaderText"];
				return "";
			}
			set
			{
				base.ViewState["HeaderText"] = value;
			}
		}

		[DefaultValue(""), Category("Appearance")]
		public string Tooltip
		{
			get
			{
				if(base.ViewState["Tooltip"] != null)
					return (string)base.ViewState["Tooltip"];
				return HeaderText;
			}
			set
			{
				base.ViewState["Tooltip"] = value;
			}
		}

		[DefaultValue(true), Description("Determines if the text in the column cell should be HtmlEncdoed. Default is true."), Category("Functionality")]
		public bool HtmlEncode
		{
			get
			{
				if(base.ViewState["HtmlEncode"] != null)
					return (bool)base.ViewState["HtmlEncode"];
				return true;
			}
			set
			{
				base.ViewState["HtmlEncode"] = value;
			}
		}

		[DefaultValue(true), Description("Determines if the DataFormatString should be HtmlEncoded. Default is true."), Category("Functionality")]
		public bool HtmlEncodeFormatString
		{
			get
			{
				if(base.ViewState["HtmlEncodeFormatString"] != null)
					return (bool)base.ViewState["HtmlEncodeFormatString"];
				return true;
			}
			set
			{
				base.ViewState["HtmlEncodeFormatString"] = value;
			}
		}

		[Category("Functionality"), Description("What to display if the cell value is null. Default is nothing ('')"), DefaultValue("")]
		public string NullDisplayText
		{
			get
			{
				if(base.ViewState["NullDisplayText"] != null)
					return (string)base.ViewState["NullDisplayText"];
				return "";
			}
			set
			{
				base.ViewState["NullDisplayText"] = value;
			}
		}

		[DefaultValue(false), Category("Functionality"), Description("Sets the column as a primary key")]
		public bool PrimaryKey
		{
			get
			{
				return ((base.ViewState["PrimaryKey"] != null) && ((bool)base.ViewState["PrimaryKey"]));
			}
			set
			{
				base.ViewState["PrimaryKey"] = value;
			}
		}

		[Category("Functionality"), DefaultValue(true), Description("Controls if the columns can be resized. Default is true.")]
		public bool Resizable
		{
			get
			{
				if(base.ViewState["Resizable"] != null)
					return (bool)base.ViewState["Resizable"];
				return true;
			}
			set
			{
				base.ViewState["Resizable"] = value;
			}
		}

		[Category("Searching"), DefaultValue(true), Description("Determines if the column should be searchable. Default is true.")]
		public bool Searchable
		{
			get
			{
				if(base.ViewState["Searchable"] != null)
					return (bool)base.ViewState["Searchable"];
				return true;
			}
			set
			{
				base.ViewState["Searchable"] = value;
			}
		}

		[Description("The ID of the server-side asp:dropdownlist to be used for searching"), Category("Searching"), DefaultValue(""), IDReferenceProperty]
		public string SearchControlID
		{
			get
			{
				if(base.ViewState["SearchControlID"] != null)
					return (string)base.ViewState["SearchControlID"];
				return "";
			}
			set
			{
				base.ViewState["SearchControlID"] = value;
			}
		}

		[Category("Searching"), DefaultValue(0), Description("The type of the column - used by the auto-search functionality of the grid.")]
		public SearchDataType SearchDataType
		{
			get
			{
				if(base.ViewState["SearchDataType"] != null)
					return (SearchDataType)base.ViewState["SearchDataType"];
				return SearchDataType.NotSet;
			}
			set
			{
				base.ViewState["SearchDataType"] = value;
			}
		}

		[Category("Searching"), DefaultValue(12), Description("The default search function for this column when toolbar searching is enabled. Default is Contains.")]
		public SearchOperation SearchToolBarOperation
		{
			get
			{
				if(base.ViewState["SearchToolBarOperation"] != null)
				{
					return (SearchOperation)base.ViewState["SearchToolBarOperation"];
				}
				return SearchOperation.Contains;
			}
			set
			{
				base.ViewState["SearchToolBarOperation"] = value;
			}
		}

		[DefaultValue(0), Description("The type of the control to be used for searching - TextBox or DropDown"), Category("Searching")]
		public SearchType SearchType
		{
			get
			{
				if(base.ViewState["SearchType"] != null)
				{
					return (SearchType)base.ViewState["SearchType"];
				}
				return SearchType.TextBox;
			}
			set
			{
				base.ViewState["SearchType"] = value;
			}
		}

		[Description("The list of values when SearchType = DropDown. Name/value pairs separated with ':', e.g. 'TNT:TNT;DHL:DHL;UPS:UPS'"), Category("Searching"), DefaultValue("")]
		public string SearchValues
		{
			get
			{
				if(base.ViewState["SearchValues"] != null)
				{
					return (string)base.ViewState["SearchValues"];
				}
				return "";
			}
			set
			{
				base.ViewState["SearchValues"] = value;
			}
		}

		[Description("Controls if the columns can be sorted by clicking on the header. Default is true."), Category("Functionality"), DefaultValue(true)]
		public bool Sortable
		{
			get
			{
				if(base.ViewState["Sortable"] != null)
				{
					return (bool)base.ViewState["Sortable"];
				}
				return true;
			}
			set
			{
				base.ViewState["Sortable"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(0), Description("The alignment of the cell text in the column. Default is left")]
		public System.Web.UI.WebControls.TextAlign TextAlign
		{
			get
			{
				if(base.ViewState["TextAlign"] != null)
				{
					return (System.Web.UI.WebControls.TextAlign)base.ViewState["TextAlign"];
				}
				return System.Web.UI.WebControls.TextAlign.Left;
			}
			set
			{
				base.ViewState["TextAlign"] = value;
			}
		}

		[DefaultValue(true), Description("If the column should be visible. You may want to have column non-visible, but editable for example"), Category("Appearance")]
		public bool Visible
		{
			get
			{
				if(base.ViewState["Visible"] != null)
				{
					return (bool)base.ViewState["Visible"];
				}
				return true;
			}
			set
			{
				base.ViewState["Visible"] = value;
			}
		}

		[Description("The width of the column. Supports integers only. Default is 150."), Category("Appearance"), DefaultValue(150)]
		public int Width
		{
			get
			{
				if(base.ViewState["Width"] != null)
				{
					return (int)base.ViewState["Width"];
				}
				return 150;
			}
			set
			{
				base.ViewState["Width"] = value;
			}
		}

		/// <summary>
		/// Name of class used to format the value of column. 
		/// </summary>
		[Category("Appearance"), DefaultValue(0), Description("Name of class used to format the value of column.")]
		public string ValuesFormatter
		{
			get
			{
				if(base.ViewState["ValuesFormatter"] != null)
				{
					return (string)base.ViewState["ValuesFormatter"];
				}
				return "";
			}
			set
			{
				base.ViewState["ValuesFormatter"] = value;
			}
		}

		/// <summary>
		/// Recupera a instancia do formatador dos valores da coluna.
		/// </summary>
		/// <returns></returns>
		public IColumnValuesFormatter GetValuesFormatterInstance()
		{
			if(_valueFormatterInstance == null && !string.IsNullOrEmpty(ValuesFormatter))
			{
				Type type = System.Web.Compilation.BuildManager.GetType(ValuesFormatter, false, true);
				if(type == null)
					throw new Exception(string.Format("Type \"{0}\" not found for ValueFormatter", ValuesFormatter));
				if(!Array.Exists(type.GetInterfaces(), f => f.FullName == typeof(IColumnValuesFormatter).FullName))
					throw new Exception(string.Format("Type \"{0}\" not implement \"{1}\" for ValueFormatter", ValuesFormatter, typeof(IColumnValuesFormatter).FullName));
				try
				{
					_valueFormatterInstance = (IColumnValuesFormatter)Activator.CreateInstance(type);
				}
				catch(Exception ex)
				{
					throw new Exception(string.Format("Fail on create instance to ValueFormatter \"{0}\"", ValuesFormatter), ex);
				}
			}
			return _valueFormatterInstance;
		}

		/// <summary>
		/// Forma o valor dos dados.
		/// </summary>
		/// <param name="dataValue"></param>
		/// <param name="encode"></param>
		/// <returns></returns>
		internal virtual string FormatDataValue(object dataValue, bool encode)
		{
			if(IsNull(dataValue))
				return NullDisplayText;
			var s = dataValue.ToString();
			var dataFormatString = DataFormatString;
			int length = s.Length;
			if(!this.HtmlEncodeFormatString)
			{
				if((length > 0) && encode)
					s = HttpUtility.HtmlEncode(s);
				if((length == 0) && this.ConvertEmptyStringToNull)
					return this.NullDisplayText;
				if(dataFormatString.Length == 0)
					return s;
				if(encode)
					return string.Format(CultureInfo.CurrentCulture, dataFormatString, new object[] {
						s
					});
				return string.Format(CultureInfo.CurrentCulture, dataFormatString, new object[] {
					dataValue
				});
			}
			if((length == 0) && this.ConvertEmptyStringToNull)
				return this.NullDisplayText;
			if(!string.IsNullOrEmpty(dataFormatString))
				s = string.Format(CultureInfo.CurrentCulture, dataFormatString, new object[] {
					dataValue
				});
			if(!string.IsNullOrEmpty(s) && encode)
				s = HttpUtility.HtmlEncode(s);
			return s;
		}

		/// <summary>
		/// Verifica se o valor é nulo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal bool IsNull(object value)
		{
			if((value != null) && !Convert.IsDBNull(value))
				return false;
			return true;
		}
	}
}
