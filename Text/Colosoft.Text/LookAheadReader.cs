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
using System.IO;

namespace Colosoft.Text.Parser
{
	/// <summary>
	/// Classe de auxilio de leitura.
	/// </summary>
	internal class LookAheadReader : IDisposable
	{
		private TextReader _reader;

		private System.Text.StringBuilder _buffer;

		private int _curpos;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="reader"></param>
		public LookAheadReader(TextReader reader)
		{
			_reader = reader;
			_curpos = 0;
			_buffer = new System.Text.StringBuilder();
		}

		/// Checks if there are enough characters in the buffer.
		private bool LookAheadBufferHasCharsForLength(int length)
		{
			return ((_curpos + length) <= _buffer.Length);
		}

		/// Makes sure there are enough characters in the buffer.
		private bool FillBuffer(int length)
		{
			if(LookAheadBufferHasCharsForLength(length))
			{
				return true;
			}
			int attemptRead = (_curpos + length) - _buffer.Length;
			attemptRead += 1024 - (attemptRead % 1024);
			if(_curpos != 0)
			{
				_buffer.Remove(0, _curpos);
				_curpos = 0;
			}
			char[] temp = new char[attemptRead];
			int actualRead = _reader.Read(temp, 0, attemptRead);
			_buffer.Append(temp, 0, actualRead);
			return LookAheadBufferHasCharsForLength(length);
		}

		/// Returns the next char in the buffer but doesn't advance the current position.
		public char LookAhead()
		{
			if(!FillBuffer(1))
			{
				throw new EndOfStreamException();
			}
			return _buffer[_curpos];
		}

		/// <summary>Returns the char at current position + the specified number of characters.
		/// Does not change the current position.</summary>
		/// <param name="pos">The position after the current one where the character to return is</param>
		public int LookAhead(int pos)
		{
			if(!FillBuffer(pos + 1))
			{
				return -1;
			}
			return _buffer[_curpos + pos];
		}

		/// Discards the next character from the buffer.
		public void Discard()
		{
			_curpos += 1;
		}

		/// Discards the next n characters from the buffer.
		public void Discard(int length)
		{
			_curpos += length;
		}

		/// Returns the next char in the buffer and advances the current position by one.
		public char Read()
		{
			char c = LookAhead();
			Discard();
			return c;
		}

		/// Returns the next n characters in the buffer and advances the current position by n.
		public String Read(int length)
		{
			if(!FillBuffer(length))
			{
				throw new EndOfStreamException();
			}
			String result = _buffer.ToString(_curpos, length);
			Discard(length);
			return result;
		}

		/// Advances the current position in the buffer until a newline is encountered.
		public void DiscardLine()
		{
			while (FillBuffer(1) && (_buffer[_curpos] != '\n'))
			{
				Discard();
			}
		}

		/// Closes the underlying StreamReader.
		public void Close()
		{
			_reader.Close();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_reader != null)
			{
				_reader.Dispose();
				_reader = null;
			}
		}
	}
}
