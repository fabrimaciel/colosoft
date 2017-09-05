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
using System.IO;

namespace Colosoft.Excel
{
	public partial class SST : Record
	{
		public SST(Record record) : base(record)
		{
		}

		/// <summary>
		/// Total number of strings in the workbook
		/// </summary>
		public Int32 TotalOccurance;

		/// <summary>
		/// Number of following strings (nm)
		/// </summary>
		public Int32 NumStrings;

		/// <summary>
		/// List of nm Unicode strings, 16-bit string length
		/// </summary>
		public List<String> StringList;

		public void decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.TotalOccurance = reader.ReadInt32();
			this.NumStrings = reader.ReadInt32();
			reader.ReadString();
		}
	}
}
