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

namespace Colosoft.Web
{
	/// <summary>
	/// Decoder de URL.
	/// </summary>
	class UrlDecoder
	{
		private int _bufferSize;

		private byte[] _byteBuffer;

		private char[] _charBuffer;

		private Encoding _encoding;

		private int _numBytes;

		private int _numChars;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="bufferSize"></param>
		/// <param name="encoding"></param>
		internal UrlDecoder(int bufferSize, Encoding encoding)
		{
			_bufferSize = bufferSize;
			_encoding = encoding;
			_charBuffer = new char[bufferSize];
		}

		/// <summary>
		/// Adiciona um byte.
		/// </summary>
		/// <param name="b"></param>
		internal void AddByte(byte b)
		{
			if(_byteBuffer == null)
				_byteBuffer = new byte[_bufferSize];
			_byteBuffer[_numBytes++] = b;
		}

		/// <summary>
		/// Adiciona um char.
		/// </summary>
		/// <param name="ch"></param>
		internal void AddChar(char ch)
		{
			if(_numBytes > 0)
				this.FlushBytes();
			_charBuffer[_numChars++] = ch;
		}

		/// <summary>
		/// Flush nos bytes.
		/// </summary>
		private void FlushBytes()
		{
			if(_numBytes > 0)
			{
				_numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
				_numBytes = 0;
			}
		}

		/// <summary>
		/// Recupera a string associado.
		/// </summary>
		/// <returns></returns>
		internal string GetString()
		{
			if(_numBytes > 0)
				this.FlushBytes();
			if(_numChars > 0)
				return new string(_charBuffer, 0, _numChars);
			return string.Empty;
		}
	}
}
