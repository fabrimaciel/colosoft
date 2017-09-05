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
	/// Representa uma linha do grid.
	/// </summary>
	public class dhtmlxGridRow : IEnumerable<dhtmlxGridCell>
	{
		private dhtmlxGridSource _gridSource;

		private dhtmlxGridCell[] _cells;

		private string _ID;

		private bool _selected;

		private string _style;

		private string _cssClass;

		private dhtmlxAttributeCollection _attributes;

		/// <summary>
		/// Fonte da grid relacionada com a linha.
		/// </summary>
		public dhtmlxGridSource GridSource
		{
			get
			{
				return _gridSource;
			}
		}

		/// <summary>
		/// Identificador da linha.
		/// </summary>
		public string ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		/// <summary>
		/// Identificador se a linha está selecionada.
		/// </summary>
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
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
		/// Nome do CSS class relacionada com a célula.
		/// </summary>
		public string CssClass
		{
			get
			{
				return _cssClass;
			}
			set
			{
				_cssClass = value;
			}
		}

		/// <summary>
		/// Atributos customizados da linha.
		/// </summary>
		public dhtmlxAttributeCollection Attributes
		{
			get
			{
				return _attributes;
			}
		}

		internal dhtmlxGridRow(dhtmlxGridSource gridSource)
		{
			_gridSource = gridSource;
			_cells = new dhtmlxGridCell[gridSource.Columns.Count];
			_attributes = new dhtmlxAttributeCollection();
		}

		/// <summary>
		/// Carrega os dados da linha.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="type">Tipo da fonte do grid.</param>
		internal void LoadElement(XmlDocument doc, XmlElement parent, dhtmlxGridSourceType type)
		{
			XmlElement row = doc.CreateElement(type == dhtmlxGridSourceType.Xml ? "row" : "tr");
			if(!string.IsNullOrEmpty(_ID))
				row.SetAttribute("id", _ID);
			if(_selected)
				row.SetAttribute("selected", "1");
			if(!string.IsNullOrEmpty(_style))
				row.SetAttribute("style", _style);
			if(!string.IsNullOrEmpty(_cssClass))
				row.SetAttribute("class", _cssClass);
			_attributes.LoadAttributes(row);
			foreach (dhtmlxGridCell cell in _cells)
				cell.LoadElement(doc, row, type);
			parent.AppendChild(row);
		}

		/// <summary>
		/// Define e recupera o valor da célula.
		/// </summary>
		/// <param name="index">Index da coluna.</param>
		/// <returns>Valor da célula.</returns>
		public string this[int index]
		{
			get
			{
				if(_cells[index] == null)
					return null;
				else
					return _cells[index].Value;
			}
			set
			{
				if(value != null && _cells[index] == null)
					_cells[index] = new dhtmlxGridCell();
				else if(value == null && _cells[index] == null)
					return;
				_cells[index].Value = value;
			}
		}

		/// <summary>
		/// Define e recupera o valor da célula.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns>Valor da célula.</returns>
		public string this[string columnName]
		{
			get
			{
				for(int i = 0; i < GridSource.Columns.Count; i++)
					if(GridSource.Columns[i].ColumnName == columnName)
						return this[i];
				throw new KeyNotFoundException(String.Format("Column name {0} not found.", columnName));
			}
			set
			{
				for(int i = 0; i < GridSource.Columns.Count; i++)
					if(GridSource.Columns[i].ColumnName == columnName)
					{
						this[i] = value;
						return;
					}
				throw new KeyNotFoundException(String.Format("Column name {0} not found.", columnName));
			}
		}

		public IEnumerator<dhtmlxGridCell> GetEnumerator()
		{
			return (IEnumerator<dhtmlxGridCell>)_cells.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _cells.GetEnumerator();
		}
	}
}
