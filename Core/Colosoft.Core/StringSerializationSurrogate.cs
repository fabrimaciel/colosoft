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
	internal sealed class StringSerializationSurrogate : SerializationSurrogate
	{
		public StringSerializationSurrogate() : base(typeof(string))
		{
		}

		public override object Read(CompactBinaryReader reader)
		{
			int count = reader.ReadInt32();
			try
			{
				byte[] bytes = new byte[count];
				bytes = reader.ReadBytes(count);
				return Encoding.UTF8.GetString(bytes);
			}
			catch(OutOfMemoryException)
			{
				throw;
			}
		}

		public override void Skip(CompactBinaryReader reader)
		{
			int count = reader.ReadInt32();
			if(count > 0)
				reader.SkipBytes(count);
		}

		public override void Write(CompactBinaryWriter writer, object graph)
		{
			string text1 = (string)graph;
			if(text1 == null)
			{
				writer.Write(-1);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(graph as string);
			int length = bytes.Length;
			writer.Write(length);
			writer.Write(bytes);
		}
	}
}
