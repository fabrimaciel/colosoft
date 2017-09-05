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
	public partial class CommonObjectData : SubRecord
	{
		public CommonObjectData(SubRecord record) : base(record)
		{
		}

		public UInt16 ObjectType;

		public UInt16 ObjectID;

		public UInt16 OptionFlags;

		public UInt32 Reserved1;

		public UInt32 Reserved2;

		public UInt32 Reserved3;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.ObjectType = reader.ReadUInt16();
			this.ObjectID = reader.ReadUInt16();
			this.OptionFlags = reader.ReadUInt16();
			this.Reserved1 = reader.ReadUInt32();
			this.Reserved2 = reader.ReadUInt32();
			this.Reserved3 = reader.ReadUInt32();
		}
	}
}
