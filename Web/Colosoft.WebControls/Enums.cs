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
	public enum AjaxCallBackMode
	{
		None,
		RequestData,
		EditRow,
		AddRow,
		DeleteRow,
		Search
	}
	public enum EditType
	{
		TextBox,
		CheckBox,
		DropDown,
		TextArea,
		Password
	}
	public enum SearchDataType
	{
		NotSet,
		String,
		Date,
		Numerical,
		Other
	}
	public enum SearchOperation
	{
		IsEqualTo,
		IsNotEqualTo,
		IsLessThan,
		IsLessOrEqualTo,
		IsGreaterThan,
		IsGreaterOrEqualTo,
		IsIn,
		IsNotIn,
		BeginsWith,
		DoesNotBeginWith,
		EndsWith,
		DoesNotEndWith,
		Contains,
		DoesNotContain
	}
	public enum SearchType
	{
		TextBox,
		DropDown
	}
	public enum SortAction
	{
		ClickOnHeader,
		ClickOnSortIcons
	}
	public enum SortDirection
	{
		Asc,
		Desc
	}
	public enum SortIconsPosition
	{
		Vertical,
		Horizontal
	}
	public enum ToolBarPosition
	{
		Top,
		Bottom,
		TopAndBottom,
		Hidden
	}
	public enum ToolBarAlign
	{
		Left,
		Center,
		Right
	}
	public enum HierarchyMode
	{
		None,
		Parent,
		Child,
		ParentAndChild
	}
	/// <summary>
	/// Possíveis teclas que podem ser usadas para a multiseleção.
	/// </summary>
	public enum MultiSelectKey
	{
		None,
		Shift,
		Ctrl,
		Alt
	}
	/// <summary>
	/// Possíveis modos de multiseleção.
	/// </summary>
	public enum MultiSelectMode
	{
		SelectOnCheckBoxClickOnly,
		SelectOnRowClick
	}
	/// <summary>
	/// Possíveis modos de renderizar o GridView.
	/// </summary>
	public enum RenderingMode
	{
		Default,
		/// <summary>
		/// Esse modo otimiza ao velocidade do Grid, mas reduz algumas funcionalidades.
		/// </summary>
		Optimized
	}
}
