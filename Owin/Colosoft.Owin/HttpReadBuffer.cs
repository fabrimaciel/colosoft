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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	class HttpReadBuffer
	{
		private readonly int _bufferSize;

		private StringBuilder _lineBuffer;

		private int _totalRead;

		private int _offset;

		private byte[] _buffer;

		private int _available;

		private bool _forceNewRead;

		public bool DataAvailable
		{
			get
			{
				return !_forceNewRead && _offset < _available;
			}
		}

		public HttpReadBuffer(int size)
		{
			_bufferSize = size;
			_buffer = new byte[size];
		}

		public string ReadLine()
		{
			if(_lineBuffer == null)
				_lineBuffer = new StringBuilder();
			while (_offset < _available)
			{
				int c = _buffer[_offset++];
				_totalRead++;
				if(c == '\n')
				{
					string line = _lineBuffer.ToString();
					_lineBuffer = new StringBuilder();
					return line;
				}
				else if(c != '\r')
				{
					_lineBuffer.Append((char)c);
				}
			}
			return null;
		}

		public void Reset()
		{
			_lineBuffer = null;
			_totalRead = 0;
		}

		public void CopyToStream(System.IO.Stream stream, int maximum)
		{
			CopyToStream(stream, maximum, null);
		}

		public bool CopyToStream(System.IO.Stream stream, int maximum, byte[] boundary)
		{
			int toRead = Math.Min(_available - _offset, maximum - _totalRead);
			bool atBoundary = false;
			bool partialBoundaryMatch = false;
			if(boundary != null)
			{
				int boundaryOffset = -1;
				for(int i = 0; i < toRead; i++)
				{
					bool boundaryMatched = true;
					for(int j = 0; j < boundary.Length; j++)
					{
						if(i + j >= toRead)
						{
							partialBoundaryMatch = true;
							break;
						}
						if(_buffer[i + _offset + j] != boundary[j])
						{
							boundaryMatched = false;
							break;
						}
					}
					if(boundaryMatched)
					{
						boundaryOffset = i;
						break;
					}
				}
				if(boundaryOffset != -1)
				{
					toRead = boundaryOffset;
					atBoundary = !partialBoundaryMatch;
					if(partialBoundaryMatch)
						_forceNewRead = true;
				}
			}
			stream.Write(_buffer, _offset, toRead);
			_offset += toRead;
			_totalRead += toRead;
			if(atBoundary)
			{
				_offset += boundary.Length;
				_totalRead += boundary.Length;
			}
			else if(boundary != null && maximum - _totalRead < boundary.Length)
			{
				throw new System.ServiceModel.ProtocolException("Not enough data available for multipart boundary to match");
			}
			return atBoundary;
		}

		public bool? AtBoundary(byte[] boundary, int maximum)
		{
			if(boundary == null)
				throw new ArgumentNullException("boundary");
			if(maximum - _totalRead < boundary.Length)
				throw new System.ServiceModel.ProtocolException("Not enough data available for multipart boundary to match");
			if(_available - _offset < boundary.Length)
				return null;
			for(int i = 0; i < boundary.Length; i++)
			{
				if(boundary[i] != _buffer[i + _offset])
					return false;
			}
			_offset += boundary.Length;
			_totalRead += boundary.Length;
			return true;
		}

		public IAsyncResult BeginRead(System.IO.Stream stream, AsyncCallback callback, object state)
		{
			_forceNewRead = false;
			if(_offset == _available)
			{
				_offset = 0;
				_available = 0;
			}
			else if(_buffer.Length - _available < _bufferSize)
			{
				if(_buffer.Length - (_available - _offset) < _bufferSize)
				{
					var buffer = new byte[_buffer.Length * 2];
					Array.Copy(_buffer, _offset, buffer, 0, _available - _offset);
					_buffer = buffer;
				}
				else
				{
					Array.Copy(_buffer, _offset, _buffer, 0, _available - _offset);
				}
				_available -= _offset;
				_offset = 0;
			}
			int bufferAvailable = Math.Min(_buffer.Length - _available, _bufferSize);
			return stream.BeginRead(_buffer, _available, bufferAvailable, callback, state);
		}

		public void EndRead(System.IO.Stream stream, IAsyncResult asyncResult)
		{
			int read = stream.EndRead(asyncResult);
			_available += read;
		}
	}
}
