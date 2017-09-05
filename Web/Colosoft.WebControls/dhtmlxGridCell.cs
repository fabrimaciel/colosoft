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
using System.Xml;

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Representa uma celula do grid.
	/// </summary>
	public class dhtmlxGridCell
	{
		private dhtmlxeXcellType _cellType;

		private string _value;

		private string _style;

		/// <summary>
		/// Tipo da célula.
		/// </summary>
		public dhtmlxeXcellType CellType
		{
			get
			{
				return _cellType;
			}
			set
			{
				_cellType = value;
			}
		}

		/// <summary>
		/// Valor da célula.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Definição de estilo CSS.
		/// </summary>
		public string Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public dhtmlxGridCell() : this(dhtmlxeXcellType.Undefined, null)
		{
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="cellType">Tipo da célula.</param>
		/// <param name="value">Valor da célula.</param>
		public dhtmlxGridCell(dhtmlxeXcellType cellType, string value)
		{
			_cellType = cellType;
			_value = value;
		}

		/// <summary>
		/// Carregas os dados da célula no elemento pai.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="type">Tipo da fonte do grid.</param>
		internal void LoadElement(XmlDocument doc, XmlElement parent, dhtmlxGridSourceType type)
		{
			XmlElement cell = doc.CreateElement(type == dhtmlxGridSourceType.Xml ? "cell" : "td");
			if(_cellType != dhtmlxeXcellType.Undefined)
				cell.SetAttribute("type", dhtmlxUtils.ConverteXcellType(_cellType));
			if(!string.IsNullOrEmpty(_style))
				cell.SetAttribute("style", _style);
			cell.InnerText = _value;
			parent.AppendChild(cell);
		}
	}
}
