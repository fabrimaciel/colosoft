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
using System.Drawing;
using System.Xml;

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Alinhamento do texto
	/// </summary>
	public enum dhtmlxGridColumnAlign
	{
		Left,
		Center,
		Right
	}
	/// <summary>
	/// Tipo de ordenação suportados pelo grid.
	/// </summary>
	public enum dhtmlxGridSortType
	{
		Integer,
		String,
		Date,
		Custom
	}
	/// <summary>
	/// Tipos predefinidos de editores de cell.
	/// </summary>
	public enum dhtmlxeXcellType
	{
		Undefined,
		ReadOnly,
		SimpleEditor,
		TextEditor,
		Checkbox,
		Radiobutton,
		SelectBox,
		Combobox,
		Image,
		ColorPicker,
		PriceOriented,
		DynamicOfSales
	}
	/// <summary>
	/// Representa a coluna da grid.
	/// </summary>
	public class dhtmlxGridColumn
	{
		private int _width;

		private dhtmlxeXcellType _columnType = dhtmlxeXcellType.ReadOnly;

		private dhtmlxGridColumnAlign _align;

		private Color _backColor;

		private dhtmlxGridSortType _sortType = dhtmlxGridSortType.String;

		private string _ID;

		private IList<dhtmlxGridColumnOption> _options = new List<dhtmlxGridColumnOption>();

		private string _columnName;

		private string _caption;

		private bool _visible = true;

		/// <summary>
		/// Largura da coluna.
		/// </summary>
		public int Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		/// <summary>
		/// Tipo da coluna.
		/// </summary>
		[System.ComponentModel.DefaultValue(typeof(dhtmlxeXcellType), "ReadOnly")]
		public dhtmlxeXcellType ColumnType
		{
			get
			{
				return _columnType;
			}
			set
			{
				_columnType = value;
			}
		}

		/// <summary>
		/// Alinhamento do texto na coluna.
		/// </summary>
		public dhtmlxGridColumnAlign Align
		{
			get
			{
				return _align;
			}
			set
			{
				_align = value;
			}
		}

		/// <summary>
		/// Color de fundo para a coluna.
		/// </summary>
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
			}
		}

		/// <summary>
		/// Tipo de ordenação da coluna.
		/// </summary>
		public dhtmlxGridSortType SortType
		{
			get
			{
				return _sortType;
			}
			set
			{
				_sortType = value;
			}
		}

		/// <summary>
		/// Identificador da coluna.
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
		/// Opções suportadas pela coluna.
		/// </summary>
		public IList<dhtmlxGridColumnOption> Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
			}
		}

		/// <summary>
		/// Texto do cabeçalho da coluna.
		/// </summary>
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
			}
		}

		/// <summary>
		/// Identifica se a coluna está visivel ou não.
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public dhtmlxGridColumn()
		{
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome da coluna.</param>
		public dhtmlxGridColumn(string name) : this(name, 50, dhtmlxeXcellType.ReadOnly)
		{
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome da coluna.</param>
		/// <param name="columnType">Tipo da célula da coluna.</param>
		public dhtmlxGridColumn(string name, dhtmlxeXcellType columnType) : this(name, 50, columnType)
		{
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome da coluna.</param>
		/// <param name="width">Largura da coluna.</param>
		public dhtmlxGridColumn(string name, int width) : this(name, width, dhtmlxeXcellType.ReadOnly)
		{
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="name">Nome da coluna.</param>
		/// <param name="width">Largura da coluna.</param>
		/// <param name="columnType">Tipo da célula da coluna.</param>
		public dhtmlxGridColumn(string name, int width, dhtmlxeXcellType columnType)
		{
			_columnName = name;
			_width = width;
			_columnType = columnType;
		}

		/// <summary>
		/// Carrega os dados da coluna dentro do elemento pai informado.
		/// </summary>
		/// <param name="doc">Documento pai.</param>
		/// <param name="parent">Elemento pai.</param>
		/// <param name="type">Tipo da fonte do grid.</param>
		internal void LoadElement(XmlDocument doc, XmlElement parent, dhtmlxGridSourceType type)
		{
			XmlElement column = doc.CreateElement(type == dhtmlxGridSourceType.Xml ? "column" : "td");
			column.SetAttribute("width", _width.ToString());
			column.SetAttribute("type", dhtmlxUtils.ConverteXcellType(_columnType));
			column.SetAttribute("align", dhtmlxUtils.ConvertGridColumnAlign(_align));
			column.SetAttribute("sort", dhtmlxUtils.ConvertGridSortType(_sortType));
			if(!_backColor.IsEmpty)
				column.SetAttribute("color", System.Drawing.ColorTranslator.ToHtml(_backColor));
			if(_visible)
				column.SetAttribute("hidden", "1");
			column.InnerText = _caption;
			if(type == dhtmlxGridSourceType.Xml)
				foreach (dhtmlxGridColumnOption op in _options)
					op.LoadElement(doc, column);
			parent.AppendChild(column);
		}
	}
}
