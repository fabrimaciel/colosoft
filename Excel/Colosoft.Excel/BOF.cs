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
	public partial class BOF : Record
	{
		public BOF(Record record) : base(record)
		{
		}

		public UInt16 BIFFversion;

		public UInt16 StreamType;

		public UInt16 BuildID;

		public UInt16 BuildYear;

		public UInt32 FileHistoryFlags;

		/// <summary>
		/// Lowest Excel version that can read all records in this file
		/// </summary>
		public UInt32 RequiredExcelVersion;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.BIFFversion = reader.ReadUInt16();
			this.StreamType = reader.ReadUInt16();
			this.BuildID = reader.ReadUInt16();
			this.BuildYear = reader.ReadUInt16();
			this.FileHistoryFlags = reader.ReadUInt32();
			this.RequiredExcelVersion = reader.ReadUInt32();
		}
	}
}
