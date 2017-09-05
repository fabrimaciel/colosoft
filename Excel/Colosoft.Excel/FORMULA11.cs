﻿/* 
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
	public partial class FORMULA : Record
	{
		public FORMULA(Record record) : base(record)
		{
		}

		public UInt16 RowIndex;

		public UInt16 ColIndex;

		public UInt16 XFIndex;

		/// <summary>
		/// Result of the formula.
		/// </summary>
		public UInt64 Result;

		public UInt16 OptionFlags;

		public UInt32 Unused;

		/// <summary>
		/// Formula data (RPN token array)
		/// </summary>
		public Byte[] FormulaData;

		public override void Decode()
		{
			MemoryStream stream = new MemoryStream(Data);
			BinaryReader reader = new BinaryReader(stream);
			this.RowIndex = reader.ReadUInt16();
			this.ColIndex = reader.ReadUInt16();
			this.XFIndex = reader.ReadUInt16();
			this.Result = reader.ReadUInt64();
			this.OptionFlags = reader.ReadUInt16();
			this.Unused = reader.ReadUInt32();
			this.FormulaData = reader.ReadBytes((int)(stream.Length - stream.Position));
		}
	}
}
