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
	public partial class XF : Record
	{
		public XF(Record record) : base(record)
		{
		}

		public UInt16 FontIndex;

		public UInt16 FormatIndex;

		public UInt16 CellProtection;

		public Byte Alignment;

		public Byte Rotation;

		public Byte Indent;

		public Byte Attributes;

		public UInt32 LineStyle;

		public UInt32 LineColor;

		public UInt16 Background;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.FontIndex = reader.ReadUInt16();
			this.FormatIndex = reader.ReadUInt16();
			this.CellProtection = reader.ReadUInt16();
			this.Alignment = reader.ReadByte();
			this.Rotation = reader.ReadByte();
			this.Indent = reader.ReadByte();
			this.Attributes = reader.ReadByte();
			this.LineStyle = reader.ReadUInt32();
			this.LineColor = reader.ReadUInt32();
			this.Background = reader.ReadUInt16();
		}
	}
}
