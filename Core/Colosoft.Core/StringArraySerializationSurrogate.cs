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
	internal sealed class StringArraySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		public StringArraySerializationSurrogate() : base(typeof(string[]))
		{
		}

		public override object Instantiate(CompactBinaryReader reader)
		{
			return new string[reader.ReadInt32()];
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			string[] strArray = (string[])graph;
			for(int i = 0; i < strArray.Length; i++)
			{
				if(reader.ReadInt16() != 0)
				{
					int count = reader.ReadInt32();
					if(count == 0)
						strArray[i] = null;
					else
					{
						byte[] bytes = new byte[count];
						bytes = reader.ReadBytes(count);
						strArray[i] = Encoding.UTF8.GetString(bytes);
					}
				}
			}
			return strArray;
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			string[] strArray = (string[])graph;
			for(int i = 0; i < strArray.Length; i++)
			{
				if(reader.ReadInt16() == 0)
					strArray[i] = null;
				else
				{
					int count = reader.ReadInt32();
					reader.ReadBytes(count);
				}
			}
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			string[] strArray = (string[])graph;
			writer.Write(strArray.Length);
			for(int i = 0; i < strArray.Length; i++)
			{
				if(strArray[i] != null)
				{
					writer.Write((short)1);
					int length = strArray[i].Length;
					writer.Write(length);
					byte[] bytes = Encoding.UTF8.GetBytes(strArray[i]);
					writer.Write(bytes);
				}
				else
					writer.Write((short)0);
			}
		}
	}
}
