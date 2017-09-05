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

namespace QiHe.CodeLib
{
	public class StreamHelper
	{
		/// <summary>
		/// Write Byte Order Mark(BOM) of streamWriter's Encoding
		/// to streamWriter's underlying stream.
		/// </summary>
		/// <param name="writer"></param>
		public static void WriteByteOrderMark(StreamWriter streamWriter)
		{
			byte[] BOM = streamWriter.Encoding.GetPreamble();
			streamWriter.BaseStream.Write(BOM, 0, BOM.Length);
		}

		public static void WriteBytes(Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		public static byte[] ReadBytes(Stream stream, int count)
		{
			int length = Math.Min((int)stream.Length, count);
			byte[] bytes = new byte[length];
			stream.Read(bytes, 0, bytes.Length);
			return bytes;
		}

		public static void ReadBytes(Stream stream, byte[] buffer)
		{
			stream.Read(buffer, 0, buffer.Length);
		}

		public static byte[] ReadToEnd(Stream stream)
		{
			byte[] bytes = new byte[stream.Length - stream.Position];
			stream.Read(bytes, 0, bytes.Length);
			return bytes;
		}

		/// <summary>
		/// Search the first occurance of values from current position in stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="values"></param>
		/// <returns>the stream position after values or -1 if not fount</returns>
		public static long SearchStream(Stream stream, byte[] values)
		{
			int index = 0;
			int code = -1;
			do
			{
				code = stream.ReadByte();
				if(code == values[index])
				{
					index++;
				}
				else if(index > 0)
				{
					stream.Position -= index;
					index = 0;
				}
			}
			while (code != -1 && index < values.Length);
			if(index == values.Length)
			{
				return stream.Position;
			}
			else
			{
				return -1;
			}
		}

		public static long SearchStream(Stream stream, byte[] values, int maxlength)
		{
			int index = 0;
			int code = -1;
			long maxpos = stream.Position + maxlength;
			do
			{
				code = stream.ReadByte();
				if(code == values[index])
				{
					index++;
				}
				else
				{
					stream.Position -= index;
					index = 0;
				}
			}
			while (code != -1 && index < values.Length && stream.Position < maxpos);
			if(index == values.Length)
			{
				return stream.Position;
			}
			else
			{
				return -1;
			}
		}
	}
}
