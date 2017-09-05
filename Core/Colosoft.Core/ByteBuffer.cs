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

namespace Colosoft.IO.Compression
{
	/// <summary>
	/// byte[] array plus current offset.
	/// useful for reading/writing headers, ensuring the offset is updated correctly
	/// </summary>
	internal struct ByteBuffer
	{
		private byte[] _buffer;

		private int _offset;

		public int Length
		{
			get
			{
				return _buffer.Length;
			}
		}

		public ByteBuffer(int size)
		{
			_buffer = new byte[size];
			_offset = 0;
		}

		public void SkipBytes(int count)
		{
			_offset += count;
		}

		public uint ReadUInt32()
		{
			return (uint)(_buffer[_offset++] | ((_buffer[_offset++] | ((_buffer[_offset++] | (_buffer[_offset++] << 8)) << 8)) << 8));
		}

		public ushort ReadUInt16()
		{
			return (ushort)(_buffer[_offset++] | ((_buffer[_offset++]) << 8));
		}

		public void WriteUInt32(uint value)
		{
			_buffer[_offset++] = (byte)value;
			_buffer[_offset++] = (byte)(value >> 8);
			_buffer[_offset++] = (byte)(value >> 16);
			_buffer[_offset++] = (byte)(value >> 24);
		}

		public void WriteUInt16(ushort value)
		{
			_buffer[_offset++] = (byte)value;
			_buffer[_offset++] = (byte)(value >> 8);
		}

		public void WriteContentsTo(System.IO.Stream writer)
		{
			System.Diagnostics.Debug.Assert(_offset == _buffer.Length);
			writer.Write(_buffer, 0, _buffer.Length);
		}

		public int ReadContentsFrom(System.IO.Stream reader)
		{
			System.Diagnostics.Debug.Assert(_offset == 0);
			return reader.Read(_buffer, 0, _buffer.Length);
		}
	}
}
