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
using System.Data;
using System.Globalization;

namespace Colosoft.Excel.ExcelXml
{
	partial class ExcelXmlWorkbook
	{
		/// <summary>
		/// Converts a dataset to a work book
		/// </summary>
		/// <param name="source">The source dataset to convert to a work book</param>
		/// <returns>Returns the <see cref="Colosoft.Excel.ExcelXml.ExcelXmlWorkbook"/>
		/// for the dataset.</returns>
		/// <remarks>All the tables are converted into sheets with sheet names as table + table number, 
		/// eg. "Table0" "Table1" etc. Supported types which can be successfully
		/// converted to cells are the same as described in <see cref="Colosoft.Excel.ExcelXml.Cell"/>
		/// except <see cref="Colosoft.Excel.ExcelXml.Cell"/> and 
		/// <see cref="Colosoft.Excel.ExcelXml.Formula"/></remarks>
		public static ExcelXmlWorkbook DataSetToWorkbook(DataSet source)
		{
			ExcelXmlWorkbook book = new ExcelXmlWorkbook();
			for(int tableNumber = 0; tableNumber < source.Tables.Count; tableNumber++)
			{
				Worksheet sheet = book[tableNumber];
				if(!string.IsNullOrEmpty(source.Tables[tableNumber].TableName))
					sheet.Name = source.Tables[tableNumber].TableName;
				else
					sheet.Name = "Table" + tableNumber.ToString(CultureInfo.InvariantCulture);
				int columnCount = source.Tables[tableNumber].Columns.Count;
				for(int columnNumber = 0; columnNumber < columnCount; columnNumber++)
				{
					sheet[columnNumber, 0].Value = source.Tables[tableNumber].Columns[columnNumber].ColumnName;
					sheet[columnNumber, 0].Font.Bold = true;
				}
				int rowNumber = 0;
				foreach (DataRow row in source.Tables[tableNumber].Rows)
				{
					rowNumber++;
					for(int columnNumber = 0; columnNumber < columnCount; columnNumber++)
					{
						string rowType = row[columnNumber].GetType().FullName;
						switch(rowType)
						{
						case "System.DateTime":
						{
							sheet[columnNumber, rowNumber].Value = (DateTime)row[columnNumber];
							break;
						}
						case "System.Boolean":
						{
							sheet[columnNumber, rowNumber].Value = (bool)row[columnNumber];
							break;
						}
						case "System.SByte":
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.Byte":
						case "System.UInt16":
						case "System.UInt32":
						case "System.UInt64":
						case "System.Single":
						case "System.Double":
						case "System.Decimal":
						{
							sheet[columnNumber, rowNumber].Value = Convert.ToDecimal(row[columnNumber], CultureInfo.InvariantCulture);
							break;
						}
						case "System.DBNull":
							break;
						default:
						{
							sheet[columnNumber, rowNumber].Value = row[columnNumber].ToString();
							break;
						}
						}
					}
				}
			}
			return book;
		}
	}
}
