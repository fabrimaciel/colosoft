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
using System.IO;
using System.Linq;
using System.Text;

namespace Colosoft.Web
{
	/// <summary>
	///     Provides methods to interpret and read a stream as either character or binary
	///     data similar to a <see cref="BinaryReader" /> and provides the ability to push
	///     data onto the front of the stream.
	/// </summary>
	internal class RebufferableBinaryReader
	{
		/// <summary>
		///     The size of the buffer to use when reading new data.
		/// </summary>
		private readonly int bufferSize;

		/// <summary>
		///     The encoding to use for character based operations
		/// </summary>
		private readonly Encoding encoding;

		/// <summary>
		///     The stream to read raw data from.
		/// </summary>
		private readonly Stream stream;

		/// <summary>
		///     The stream stack to store buffered data.
		/// </summary>
		private readonly BinaryStreamStack streamStack;

		/// <summary>
		///     Initializes a new instance of the <see cref="RebufferableBinaryReader" /> class.
		///     Default encoding of UTF8 will be used.
		/// </summary>
		/// <param name="input">
		///     The input stream to read from.
		/// </param>
		public RebufferableBinaryReader(Stream input) : this(input, new UTF8Encoding(false))
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="RebufferableBinaryReader" /> class.
		/// </summary>
		/// <param name="input">
		///     The input stream to read from.
		/// </param>
		/// <param name="encoding">
		///     The encoding to use for character based operations.
		/// </param>
		public RebufferableBinaryReader(Stream input, Encoding encoding) : this(input, encoding, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="RebufferableBinaryReader" /> class.
		/// </summary>
		/// <param name="input">
		///     The input stream to read from.
		/// </param>
		/// <param name="encoding">
		///     The encoding to use for character based operations.
		/// </param>
		/// <param name="bufferSize">
		///     The buffer size to use for new buffers.
		/// </param>
		public RebufferableBinaryReader(Stream input, Encoding encoding, int bufferSize)
		{
			stream = input;
			streamStack = new BinaryStreamStack(encoding);
			this.encoding = encoding;
			this.bufferSize = bufferSize;
		}

		/// <summary>
		///     Adds data to the front of the stream. The most recently buffered data will
		///     be read first.
		/// </summary>
		/// <param name="data">
		///     The data to buffer.
		/// </param>
		public void Buffer(byte[] data)
		{
			streamStack.Push(data);
		}

		/// <summary>
		///     Adds the string to the front of the stream. The most recently buffered data will
		///     be read first.
		/// </summary>
		/// <param name="data">
		///     The data.
		/// </param>
		public void Buffer(string data)
		{
			streamStack.Push(encoding.GetBytes(data));
		}

		/// <summary>
		///     Reads a single byte as an integer from the stream. Returns -1 if no
		///     data is left to read.
		/// </summary>
		/// <returns>
		///     The <see cref="byte" /> that was read.
		/// </returns>
		public int Read()
		{
			int value = -1;
			while (value == -1)
			{
				if(!streamStack.HasData())
				{
					if(StreamData() == 0)
					{
						return -1;
					}
				}
				value = streamStack.Read();
			}
			return value;
		}

		/// <summary>
		///     Reads the specified number of bytes from the stream, starting from a
		///     specified point in the byte array.
		/// </summary>
		/// <param name="buffer">
		///     The buffer to read data into.
		/// </param>
		/// <param name="index">
		///     The index of buffer to start reading into.
		/// </param>
		/// <param name="count">
		///     The number of bytes to read into the buffer.
		/// </param>
		/// <returns>
		///     The number of bytes read into buffer. This might be less than the number of bytes requested if that many bytes are not available,
		///     or it might be zero if the end of the stream is reached.
		/// </returns>
		public int Read(byte[] buffer, int index, int count)
		{
			int amountRead = 0;
			while (amountRead < count)
			{
				if(!streamStack.HasData())
				{
					if(StreamData() == 0)
					{
						return amountRead;
					}
				}
				amountRead += streamStack.Read(buffer, index + amountRead, count - amountRead);
			}
			return amountRead;
		}

		/// <summary>
		///     Reads the specified number of characters from the stream, starting from a
		///     specified point in the byte array.
		/// </summary>
		/// <param name="buffer">
		///     The buffer to read data into.
		/// </param>
		/// <param name="index">
		///     The index of buffer to start reading into.
		/// </param>
		/// <param name="count">
		///     The number of characters to read into the buffer.
		/// </param>
		/// <returns>
		///     The number of characters read into buffer. This might be less than the number of
		///     characters requested if that many characters are not available,
		///     or it might be zero if the end of the stream is reached.
		/// </returns>
		public int Read(char[] buffer, int index, int count)
		{
			int amountRead = 0;
			while (amountRead < count)
			{
				if(!streamStack.HasData())
				{
					if(StreamData() == 0)
					{
						return amountRead;
					}
				}
				amountRead += streamStack.Read(buffer, index + amountRead, count - amountRead);
			}
			return amountRead;
		}

		/// <summary>
		///     Reads a series of bytes delimited by the byte encoding of newline for this platform.
		///     the newline bytes will not be included in the return data.
		/// </summary>
		/// <returns>
		///     A byte array containing all the data up to but not including the next newline in the stack.
		/// </returns>
		public byte[] ReadByteLine()
		{
			var builder = new MemoryStream();
			while (true)
			{
				if(!streamStack.HasData())
				{
					if(StreamData() == 0)
					{
						return builder.Length > 0 ? builder.ToArray() : null;
					}
				}
				bool hitStreamEnd;
				byte[] line = streamStack.ReadByteLine(out hitStreamEnd);
				builder.Write(line, 0, line.Length);
				if(!hitStreamEnd)
				{
					return builder.ToArray();
				}
			}
		}

		/// <summary>
		///     Reads a line from the stack delimited by the newline for this platform. The newline
		///     characters will not be included in the stream
		/// </summary>
		/// <returns>
		///     The <see cref="string" /> containing the line or null if end of stream.
		/// </returns>
		public string ReadLine()
		{
			byte[] data = ReadByteLine();
			return data == null ? null : encoding.GetString(data);
		}

		/// <summary>
		///     Determines the byte order marking offset (if any) from the
		///     given buffer.
		/// </summary>
		/// <param name="buffer">
		///     The buffer to examine.
		/// </param>
		/// <returns>
		///     The <see cref="int" /> representing the length of the byte order marking.
		/// </returns>
		private int GetBomOffset(byte[] buffer)
		{
			byte[] bom = encoding.GetPreamble();
			bool usesBom = true;
			for(int i = 0; i < bom.Length; ++i)
			{
				if(bom[i] != buffer[i])
				{
					usesBom = false;
				}
			}
			return usesBom ? bom.Length : 0;
		}

		/// <summary>
		///     Reads more data from the stream into the stream stack.
		/// </summary>
		/// <returns>
		///     The number of bytes read into the stream stack as an <see cref="int" />
		/// </returns>
		private int StreamData()
		{
			var buffer = new byte[bufferSize];
			int amountRead = stream.Read(buffer, 0, buffer.Length);
			int bomOffset = GetBomOffset(buffer);
			if(amountRead - bomOffset > 0)
			{
				if(amountRead != buffer.Length || bomOffset > 0)
				{
					var smallBuffer = new byte[amountRead - bomOffset];
					System.Buffer.BlockCopy(buffer, bomOffset, smallBuffer, 0, amountRead - bomOffset);
					streamStack.Push(smallBuffer);
				}
				else
				{
					streamStack.Push(buffer);
				}
			}
			return amountRead;
		}
	}
}
