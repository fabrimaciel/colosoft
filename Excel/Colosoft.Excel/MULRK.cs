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
	public partial class MULRK : Record
	{
		public List<UInt32> RKList;

		public List<UInt16> XFList;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			RowIndex = reader.ReadUInt16();
			FirstColIndex = reader.ReadUInt16();
			int count = (Size - 6) / 6;
			RKList = new List<uint>(count);
			XFList = new List<ushort>(count);
			for(int i = 0; i < count; i++)
			{
				UInt16 XFIndex = reader.ReadUInt16();
				UInt32 RKValue = reader.ReadUInt32();
				XFList.Add(XFIndex);
				RKList.Add(RKValue);
			}
			LastColIndex = reader.ReadInt16();
		}
	}
}
