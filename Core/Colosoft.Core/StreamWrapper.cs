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

namespace Colosoft.IO
{
	/// <summary>
	/// A <see cref="Stream"/> that wraps another stream. The major feature of <see cref="StreamWrapper"/> is that it does not dispose the
	/// underlying stream when it is disposed; this is useful when using classes such as <see cref="BinaryReader"/> and
	/// <see cref="System.Security.Cryptography.CryptoStream"/> that take ownership of the stream passed to their constructors.
	/// </summary>
	public class StreamWrapper : Stream
	{
		private Stream _streamBase;

		/// <summary>
		/// Evento acionado quando a instancia for liberada.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="StreamWrapper"/> class.
		/// </summary>
		/// <param name="streamBase">The wrapped stream.</param>
		public StreamWrapper(Stream streamBase)
		{
			if(streamBase == null)
				throw new ArgumentNullException("streamBase");
			_streamBase = streamBase;
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns><c>true</c> if the stream supports reading; otherwise, <c>false</c>.</returns>
		public override bool CanRead
		{
			get
			{
				return _streamBase == null ? false : _streamBase.CanRead;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns><c>true</c> if the stream supports seeking; otherwise, <c>false</c>.</returns>
		public override bool CanSeek
		{
			get
			{
				return _streamBase == null ? false : _streamBase.CanSeek;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns><c>true</c> if the stream supports writing; otherwise, <c>false</c>.</returns>
		public override bool CanWrite
		{
			get
			{
				return _streamBase == null ? false : _streamBase.CanWrite;
			}
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get
			{
				ThrowIfDisposed();
				return _streamBase.Length;
			}
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get
			{
				ThrowIfDisposed();
				return _streamBase.Position;
			}
			set
			{
				ThrowIfDisposed();
				_streamBase.Position = value;
			}
		}

		/// <summary>
		/// Begins an asynchronous read operation.
		/// </summary>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ThrowIfDisposed();
			return _streamBase.BeginRead(buffer, offset, count, callback, state);
		}

		/// <summary>
		/// Begins an asynchronous write operation.
		/// </summary>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ThrowIfDisposed();
			return _streamBase.BeginWrite(buffer, offset, count, callback, state);
		}

		/// <summary>
		/// Waits for the pending asynchronous read to complete.
		/// </summary>
		public override int EndRead(IAsyncResult asyncResult)
		{
			ThrowIfDisposed();
			return _streamBase.EndRead(asyncResult);
		}

		/// <summary>
		/// Ends an asynchronous write operation.
		/// </summary>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			ThrowIfDisposed();
			_streamBase.EndWrite(asyncResult);
		}

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			ThrowIfDisposed();
			_streamBase.Flush();
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			return _streamBase.Read(buffer, offset, count);
		}

		/// <summary>
		/// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
		/// </summary>
		public override int ReadByte()
		{
			ThrowIfDisposed();
			return _streamBase.ReadByte();
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
		/// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			ThrowIfDisposed();
			return _streamBase.Seek(offset, origin);
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value)
		{
			ThrowIfDisposed();
			_streamBase.SetLength(value);
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			_streamBase.Write(buffer, offset, count);
		}

		/// <summary>
		/// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
		/// </summary>
		public override void WriteByte(byte value)
		{
			ThrowIfDisposed();
			_streamBase.WriteByte(value);
		}

		/// <summary>
		/// Gets the wrapped stream.
		/// </summary>
		/// <value>The wrapped stream.</value>
		protected Stream WrappedStream
		{
			get
			{
				return _streamBase;
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="StreamWrapper"/> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
				_streamBase = null;
			base.Dispose(disposing);
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}

		private void ThrowIfDisposed()
		{
			if(_streamBase == null)
				throw new ObjectDisposedException(GetType().Name);
		}
	}
}
