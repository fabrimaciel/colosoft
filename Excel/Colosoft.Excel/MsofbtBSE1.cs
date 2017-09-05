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
using QiHe.CodeLib;

namespace Colosoft.Excel
{
	/// <summary>
	/// File BLIP Store Entry 
	/// </summary>
	public partial class MsofbtBSE : EscherRecord
	{
		public MsofbtBlip BlipRecord;

		public byte[] ImageData;

		public byte[] RemainedData;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			BlipTypeWin32 = reader.ReadByte();
			BlipTypeMacOS = reader.ReadByte();
			UID = new Guid(reader.ReadBytes(16));
			Tag = reader.ReadUInt16();
			Size = reader.ReadInt32();
			Ref = reader.ReadInt32();
			Offset = reader.ReadInt32();
			Usage = reader.ReadByte();
			NameLength = reader.ReadByte();
			Unused2 = reader.ReadByte();
			Unused3 = reader.ReadByte();
			if(stream.Position < stream.Length)
			{
				BlipRecord = EscherRecord.Read(stream) as MsofbtBlip;
				if(BlipRecord != null)
				{
					int HeaderSize = 17;
					ImageData = new byte[BlipRecord.Data.Length - HeaderSize];
					Array.Copy(BlipRecord.Data, HeaderSize, ImageData, 0, ImageData.Length);
				}
				else
				{
					throw new Exception("Image Type Not supported.");
				}
			}
			if(stream.Position < stream.Length)
			{
				RemainedData = StreamHelper.ReadToEnd(stream);
			}
		}
	}
}
