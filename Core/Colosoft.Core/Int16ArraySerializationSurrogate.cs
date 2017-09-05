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
using System.Linq;
using System.Text;
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do substituto de serialização para o vetor de <see cref="Int16"/>.
	/// </summary>
	internal sealed class Int16ArraySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		public Int16ArraySerializationSurrogate() : base(typeof(short[]))
		{
		}

		public override object Instantiate(CompactBinaryReader reader)
		{
			return new short[reader.ReadInt32()];
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			short[] numArray = (short[])graph;
			for(int i = 0; i < numArray.Length; i++)
				numArray[i] = reader.ReadInt16();
			return numArray;
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			short[] numArray = (short[])graph;
			for(int i = 0; i < numArray.Length; i++)
				reader.SkipInt16();
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			short[] numArray = (short[])graph;
			writer.Write(numArray.Length);
			for(int i = 0; i < numArray.Length; i++)
				writer.Write(numArray[i]);
		}
	}
}
