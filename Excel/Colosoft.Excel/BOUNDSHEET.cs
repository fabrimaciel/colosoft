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
	public partial class BOUNDSHEET : Record
	{
		public BOUNDSHEET(Record record) : base(record)
		{
		}

		/// <summary>
		/// Absolute stream position of the BOF record of the sheet represented by this record.
		/// </summary>
		public UInt32 StreamPosition;

		/// <summary>
		/// 00H = Visible, 01H = Hidden, 02H = Strong hidden
		/// </summary>
		public Byte Visibility;

		/// <summary>
		/// 00H = Worksheet, 02H = Chart, 06H = Visual Basic module
		/// </summary>
		public Byte SheetType;

		/// <summary>
		/// BIFF8: Unicode string, 8-bit string length
		/// </summary>
		public String SheetName;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.StreamPosition = reader.ReadUInt32();
			this.Visibility = reader.ReadByte();
			this.SheetType = reader.ReadByte();
			this.SheetName = this.ReadString(reader, 8);
		}
	}
}
