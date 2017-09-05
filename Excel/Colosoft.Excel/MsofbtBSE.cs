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
	public partial class MsofbtBSE : EscherRecord
	{
		public MsofbtBSE(EscherRecord record) : base(record)
		{
		}

		public Byte BlipTypeWin32;

		public Byte BlipTypeMacOS;

		public Guid UID;

		public UInt16 Tag;

		public Int32 Size;

		public Int32 Ref;

		public Int32 Offset;

		public Byte Usage;

		public Byte NameLength;

		public Byte Unused2;

		public Byte Unused3;

		public Byte[] ExtraData;

		public void decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.BlipTypeWin32 = reader.ReadByte();
			this.BlipTypeMacOS = reader.ReadByte();
			this.UID = new Guid(reader.ReadBytes(16));
			this.Tag = reader.ReadUInt16();
			this.Size = reader.ReadInt32();
			this.Ref = reader.ReadInt32();
			this.Offset = reader.ReadInt32();
			this.Usage = reader.ReadByte();
			this.NameLength = reader.ReadByte();
			this.Unused2 = reader.ReadByte();
			this.Unused3 = reader.ReadByte();
			this.ExtraData = reader.ReadBytes((int)(stream.Length - stream.Position));
		}
	}
}
