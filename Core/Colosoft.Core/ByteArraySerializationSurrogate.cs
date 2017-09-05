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
using System.Linq;
using System.Text;
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do substituto para a serialização do vetor de bytes.
	/// </summary>
	internal sealed class ByteArraySerializationSurrogate : SerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ByteArraySerializationSurrogate() : base(typeof(byte[]))
		{
		}

		public override object Read(CompactBinaryReader reader)
		{
			int count = reader.ReadInt32();
			return reader.ReadBytes(count);
		}

		public override void Skip(CompactBinaryReader reader)
		{
			int count = reader.ReadInt32();
			if(count > 0)
				reader.SkipBytes(count);
		}

		public override void Write(CompactBinaryWriter writer, object graph)
		{
			byte[] buffer = (byte[])graph;
			writer.Write(buffer.Length);
			writer.Write(buffer, 0, buffer.Length);
		}
	}
}
