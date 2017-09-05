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
using System.Data;
using System.IO;
using System.Xml;

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Tipos de fonte do grid.
	/// </summary>
	public enum dhtmlxGridSourceType
	{
		Xml,
		HtmlTable
	}
	/// <summary>
	/// Unidade de medida.
	/// </summary>
	public enum dhtmlxGridUom
	{
		Percent,
		Pixel
	}
	/// <summary>
	/// Representa a fonte de dados para o grid.
	/// </summary>
	public class dhtmlxGridSource
	{
		private IList<dhtmlxGridColumn> _columns = new List<dhtmlxGridColumn>();

		private dhtmlxCommandCollection _beforeInit = new dhtmlxCommandCollection();

		private dhtmlxCommandCollection _afterInit = new dhtmlxCommandCollection();

		private IList<dhtmlxGridRow> _rows = new List<dhtmlxGridRow>();

		private dhtmlxGridUom _columnWidthUom = dhtmlxGridUom.Pixel;

		/// <summary>
		/// Colunas do cabeçalho do grid.
		/// </summary>
		public IList<dhtmlxGridColumn> Columns
		{
			get
			{
				return _columns;
			}
		}

		/// <summary>
		/// Lista dos comandos a serem executados antes de iniciar o grid.
		/// </summary>
		public dhtmlxCommandCollection BeforeInit
		{
			get
			{
				return _beforeInit;
			}
		}

		/// <summary>
		/// Lista dos comandos a serem executados após iniciar o grid.
		/// </summary>
		public dhtmlxCommandCollection AfterInit
		{
			get
			{
				return _afterInit;
			}
		}

		/// <summary>
		/// Linha do grid.
		/// </summary>
		public IList<dhtmlxGridRow> Rows
		{
			get
			{
				return _rows;
			}
		}

		/// <summary>
		/// Unidade de medida usada na largura da coluna.
		/// </summary>
		public dhtmlxGridUom ColumnWidthUom
		{
			get
			{
				return _columnWidthUom;
			}
			set
			{
				_columnWidthUom = value;
			}
		}

		/// <summary>
		/// Cria uma nova linha do grid.
		/// </summary>
		/// <returns></returns>
		public dhtmlxGridRow NewRow()
		{
			return new dhtmlxGridRow(this);
		}

		/// <summary>
		/// Preenche do grid com as informações do <see cref="DataTable"/>.
		/// </summary>
		/// <param name="table"></param>
		public void Databind(DataTable table)
		{
			_columns.Clear();
			_rows.Clear();
			DataColumnCollection cols = table.Columns;
			foreach (DataColumn c in cols)
			{
				dhtmlxGridColumn col = new dhtmlxGridColumn(c.ColumnName);
				col.Caption = c.Caption;
				if(c.DataType == typeof(int) || c.DataType == typeof(short) || c.DataType == typeof(long) || c.DataType == typeof(double) || c.DataType == typeof(float))
				{
					col.SortType = dhtmlxGridSortType.Integer;
				}
				else if(c.DataType == typeof(DateTime))
					col.SortType = dhtmlxGridSortType.Date;
				else
					col.SortType = dhtmlxGridSortType.String;
				_columns.Add(col);
			}
			DataRowCollection rows = table.Rows;
			foreach (DataRow r in rows)
			{
				dhtmlxGridRow row = NewRow();
				for(int i = 0; i < cols.Count; i++)
					row[i] = r[i].ToString();
				_rows.Add(row);
			}
		}

		/// <summary>
		/// Salva a dados no stream.
		/// </summary>
		/// <param name="outStream"></param>
		/// <param name="type">Tipo da fonte de dados a ser gerada.</param>
		public void SaveSource(Stream outStream)
		{
			SaveSource(outStream, dhtmlxGridSourceType.Xml);
		}

		/// <summary>
		/// Salva a dados no stream.
		/// </summary>
		/// <param name="outStream"></param>
		/// <param name="type">Tipo da fonte de dados a ser gerada.</param>
		public void SaveSource(Stream outStream, dhtmlxGridSourceType type)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement grid = doc.CreateElement(type == dhtmlxGridSourceType.Xml ? "rows" : "table");
			XmlElement head = doc.CreateElement(type == dhtmlxGridSourceType.Xml ? "head" : "tr");
			if(type == dhtmlxGridSourceType.Xml && _beforeInit.Count > 0)
			{
				XmlElement bInitElements = doc.CreateElement("beforeInit");
				foreach (dhtmlxCommand cmd in _beforeInit)
					cmd.LoadElement(doc, bInitElements);
				head.AppendChild(bInitElements);
			}
			if(type == dhtmlxGridSourceType.Xml && _afterInit.Count > 0)
			{
				XmlElement aInitElements = doc.CreateElement("afterInit");
				foreach (dhtmlxCommand cmd in _afterInit)
					cmd.LoadElement(doc, aInitElements);
				head.AppendChild(aInitElements);
			}
			foreach (dhtmlxGridColumn col in _columns)
				col.LoadElement(doc, head, type);
			if(type == dhtmlxGridSourceType.Xml)
			{
				XmlElement settingsElement = doc.CreateElement("settings");
				XmlElement colWidth = doc.CreateElement("colwidth");
				switch(ColumnWidthUom)
				{
				case dhtmlxGridUom.Percent:
					colWidth.InnerText = "%";
					break;
				case dhtmlxGridUom.Pixel:
					colWidth.InnerText = "px";
					break;
				}
				settingsElement.AppendChild(colWidth);
				head.AppendChild(settingsElement);
			}
			grid.AppendChild(head);
			int idnum = 1;
			foreach (dhtmlxGridRow row in _rows)
			{
				row.ID = (idnum++).ToString();
				row.LoadElement(doc, grid, type);
			}
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Reset();
			settings.Encoding = Encoding.GetEncoding("iso-8859-1");
			if(type == dhtmlxGridSourceType.HtmlTable)
				settings.OmitXmlDeclaration = true;
			XmlWriter writer = XmlWriter.Create(outStream, settings);
			doc.AppendChild(grid);
			doc.Save(writer);
		}
	}
}
