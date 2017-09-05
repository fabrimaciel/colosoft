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

namespace Colosoft.WebControls.dhtmlx
{
	class dhtmlxUtils
	{
		/// <summary>
		/// Recupera para o valor textual do tipo de célula.
		/// </summary>
		/// <param name="eXcellType"></param>
		/// <returns>Valor textual.</returns>
		public static string ConverteXcellType(dhtmlxeXcellType eXcellType)
		{
			switch(eXcellType)
			{
			case dhtmlxeXcellType.ReadOnly:
				return "ro";
			case dhtmlxeXcellType.SimpleEditor:
				return "ed";
			case dhtmlxeXcellType.TextEditor:
				return "txt";
			case dhtmlxeXcellType.Checkbox:
				return "ch";
			case dhtmlxeXcellType.Radiobutton:
				return "ra";
			case dhtmlxeXcellType.SelectBox:
				return "coro";
			case dhtmlxeXcellType.Combobox:
				return "co";
			case dhtmlxeXcellType.Image:
				return "img";
			case dhtmlxeXcellType.ColorPicker:
				return "cp";
			case dhtmlxeXcellType.PriceOriented:
				return "price";
			case dhtmlxeXcellType.DynamicOfSales:
				return "dyn";
			default:
				return "";
			}
		}

		/// <summary>
		/// Recupera para o valor textual do tipo da alinhamento.
		/// </summary>
		/// <param name="align"></param>
		/// <returns></returns>
		public static string ConvertGridColumnAlign(dhtmlxGridColumnAlign align)
		{
			return align.ToString().ToLower();
		}

		/// <summary>
		/// Converto para o valor textual do tipo de ordenação.
		/// </summary>
		/// <param name="sortType"></param>
		/// <returns></returns>
		public static string ConvertGridSortType(dhtmlxGridSortType sortType)
		{
			switch(sortType)
			{
			case dhtmlxGridSortType.Custom:
				return "cus";
			case dhtmlxGridSortType.String:
				return "str";
			case dhtmlxGridSortType.Integer:
				return "int";
			case dhtmlxGridSortType.Date:
				return "date";
			default:
				return "";
			}
		}
	}
}
