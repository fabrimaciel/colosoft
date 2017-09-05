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

namespace Colosoft.Excel
{
	public class Cell
	{
		public object Value;

		internal int XFIndex;

		internal Worksheet Sheet;

		public Cell(object value, int xfindex)
		{
			Value = value;
			XFIndex = xfindex;
		}

		public string StringValue
		{
			get
			{
				return Value.ToString();
			}
		}

		public DateTime DateTimeValue
		{
			get
			{
				if(Value is double)
				{
					double days = (double)Value;
					if(days > 366)
						days--;
					return Sheet.Book.BaseDate.AddDays(days);
				}
				else if(Value is string)
				{
					return DateTime.Parse((string)Value);
				}
				else
				{
					throw new Exception("Invalid DateTime Cell.");
				}
			}
		}

		public int BackColorIndex
		{
			get
			{
				return Sheet.Book.ExtendedFormats[XFIndex].PatternColorIndex;
			}
		}

		public Color BackColor
		{
			get
			{
				return Sheet.Book.ColorPalette[BackColorIndex];
			}
		}
	}
}
