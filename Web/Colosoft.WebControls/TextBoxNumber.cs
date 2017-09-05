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
using System.Web.UI.WebControls;

namespace Colosoft.WebControls
{
	public class TextBoxNumber : TextBox
	{
		private string _culture = "en-US";

		public string Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		/// <summary>
		/// Returns script used to validate numbers
		/// </summary>
		/// <returns></returns>
		protected string GetNumberValidatorScript()
		{
			string _str;
			_str = "<script> ";
			_str += @" function FilterNumeric(){";
			_str += @" var re; ";
			_str += @"var ch=String.fromCharCode(event.keyCode);";
			_str += @"if (event.keyCode<32)";
			_str += @"{";
			_str += @"return;";
			_str += @"};";
			_str += @"if( (event.keyCode<=57)&&(event.keyCode>=48))";
			_str += @"{";
			_str += @"if (!event.shiftKey)";
			_str += @"{";
			_str += @"return;";
			_str += @"}";
			_str += @"}";
			_str += @"if ((ch=='-') ||(ch=='.') || (ch==','))";
			_str += @"{";
			_str += @"return;";
			_str += @"}";
			_str += @"event.returnValue=false;";
			_str += "}</script>";
			return _str;
		}

		protected RegularExpressionValidator BuildNumberValidator(string iIDArg, string strErrArg)
		{
			string _strSeperator;
			_strSeperator = System.Globalization.CultureInfo.GetCultureInfo(_culture).NumberFormat.NumberDecimalSeparator;
			RegularExpressionValidator rVal = new RegularExpressionValidator();
			rVal.ControlToValidate = iIDArg;
			rVal.ValidationExpression = @"(^[-]?[1-9]\d+$)|(^[-]?[1-9]$)|(^0$)|(^[-]?[1-9]\d+\" + _strSeperator + @"\d$)|(^[-]?[0-9]\" + _strSeperator + @"\d$)";
			rVal.Font.Bold = true;
			rVal.Font.Name = "Arial";
			rVal.ErrorMessage = strErrArg;
			rVal.ToolTip = "NNNNN.N";
			rVal.Display = ValidatorDisplay.None;
			rVal.EnableViewState = false;
			rVal.Visible = true;
			return rVal;
		}

		protected override void OnInit(EventArgs e)
		{
			string _strSeperator;
			_strSeperator = System.Globalization.CultureInfo.GetCultureInfo(_culture).NumberFormat.NumberDecimalSeparator;
			if(!Page.ClientScript.IsClientScriptBlockRegistered("FilterNumeric"))
				Page.ClientScript.RegisterClientScriptBlock(typeof(string), "FilterNumeric", GetNumberValidatorScript());
			this.Attributes.Add("onkeypress", "FilterNumeric()");
			base.OnInit(e);
		}
	}
}
