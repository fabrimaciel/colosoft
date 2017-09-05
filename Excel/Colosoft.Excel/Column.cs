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
using System.Globalization;
using System.Xml;
using Colosoft.Excel.ExcelXml.Extensions;

namespace Colosoft.Excel.ExcelXml
{
	/// <summary>
	/// Column class represents a column properties of a single column in a worksheet
	/// </summary>
	/// <remarks>
	/// Column class represents a column properties of a single column in a worksheet.
	/// <para>You cannot directly declare a instance of a column class from your code by using
	/// <c>new</c> keyword. The only way to access a column is to retrieve it from
	/// a worksheet by using the <see cref="Colosoft.Excel.ExcelXml.Worksheet.Columns"/>
	/// method of the <see cref="Colosoft.Excel.ExcelXml.Worksheet"/> class.</para>
	/// </remarks>
	public class Column
	{
		private ExcelXmlWorkbook ParentBook;

		private string styleID;

		/// <summary>
		/// Gets or sets the default width of the column
		/// </summary>
		public double Width
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the hidden status of the column
		/// </summary>
		public bool Hidden
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Colosoft.Excel.ExcelXml.XmlStyle"/> reference of the column.
		/// <para>Setting this option only affects cells which are added after this value is set. The 
		/// cells which are added in the same column retain their original style settings.</para>
		/// </summary>
		public XmlStyle Style
		{
			get
			{
				return ParentBook.GetStyleByID(styleID);
			}
			set
			{
				styleID = ParentBook.AddStyle(value);
			}
		}

		internal Column(Worksheet parent)
		{
			if(parent == null)
				throw new ArgumentNullException("parent");
			ParentBook = parent.ParentBook;
		}

		internal void Export(XmlWriter writer)
		{
			writer.WriteStartElement("Column");
			if(Width > 0)
				writer.WriteAttributeString("ss", "Width", null, Width.ToString(CultureInfo.InvariantCulture));
			if(Hidden)
			{
				writer.WriteAttributeString("ss", "Hidden", null, "1");
				writer.WriteAttributeString("ss", "AutoFitWidth", null, "0");
			}
			if(!Style.ID.IsNullOrEmpty() && Style.ID != "Default")
				writer.WriteAttributeString("ss", "StyleID", null, Style.ID);
			writer.WriteEndElement();
		}
	}
}
