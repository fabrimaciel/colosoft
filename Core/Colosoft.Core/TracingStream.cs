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

namespace Colosoft.IO
{
	/// <summary>
	/// Armazena os argumentos do evento acionado quando bytes são escritos.
	/// </summary>
	public class BytesWrittenEventArgs : EventArgs
	{
		private byte[] _buffer;

		private int _offset;

		private int _count;

		/// <summary>
		/// Buffer com os dados.
		/// </summary>
		public byte[] Buffer
		{
			get
			{
				return _buffer;
			}
		}

		/// <summary>
		/// Offset dos dados no buffer.
		/// </summary>
		public int Offset
		{
			get
			{
				return _offset;
			}
		}

		/// <summary>
		/// Quantidade dos dados que foram escritos do buffer.
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public BytesWrittenEventArgs(byte[] buffer, int offset, int count)
		{
			_buffer = buffer;
			_offset = offset;
			_count = count;
		}
	}
	/// <summary>
	/// Armazena os argumentos do evento acionado quando bytes são lidos.
	/// </summary>
	public class BytesReadEventArgs : EventArgs
	{
		private byte[] _buffer;

		private int _offset;

		private int _read;

		/// <summary>
		/// Buffer com os dados.
		/// </summary>
		public byte[] Buffer
		{
			get
			{
				return _buffer;
			}
		}

		/// <summary>
		/// Offset dos dados no buffer.
		/// </summary>
		public int Offset
		{
			get
			{
				return _offset;
			}
		}

		/// <summary>
		/// Quantidade dos dados lidos
		/// </summary>
		public int Read
		{
			get
			{
				return _read;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="read"></param>
		public BytesReadEventArgs(byte[] buffer, int offset, int read)
		{
			_buffer = buffer;
			_offset = offset;
			_read = read;
		}
	}
	/// <summary>
	/// Assinatura do evento acionado quando bytes forem escritos.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void BytesWrittenEventHandler (object sender, BytesWrittenEventArgs e);
	/// <summary>
	/// Assinatura do evento acionado quando bytes são lidos.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void BytesReadEventHandler (object sender, BytesReadEventArgs e);
	/// <summary>
	/// Implementação do Stream com rastreamento.
	/// </summary>
	public class TracingStream : System.IO.Stream
	{
		private byte[] _dataRead;

		private byte[] _dataWritten;

		private long _numBytesRead;

		private long _numBytesWritten;

		private System.Net.HttpWebResponse _response;

		private System.IO.Stream _stream;

		private bool _traceContent;

		/// <summary>
		/// Evento acionado quando bytes forem escritos.
		/// </summary>
		public event BytesWrittenEventHandler BytesWritten;

		/// <summary>
		/// Evento acionado quando bytes são lidos.
		/// </summary>
		public event BytesReadEventHandler BytesRead;

		/// <summary>
		/// Identifica se tem suporte para leitura.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return _stream.CanRead;
			}
		}

		/// <summary>
		/// Identifica se tem suporte para Seek.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return _stream.CanSeek;
			}
		}

		/// <summary>
		/// Identifica se tem suporte para escrita.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return _stream.CanWrite;
			}
		}

		/// <summary>
		/// Tamanho.
		/// </summary>
		public override long Length
		{
			get
			{
				return _stream.Length;
			}
		}

		/// <summary>
		/// Número de bytes já lidos.
		/// </summary>
		public long NumBytesRead
		{
			get
			{
				return _numBytesRead;
			}
		}

		/// <summary>
		/// Número de bytes escritos.
		/// </summary>
		public long NumBytesWritten
		{
			get
			{
				return _numBytesWritten;
			}
		}

		/// <summary>
		/// Posição.
		/// </summary>
		public override long Position
		{
			get
			{
				return _stream.Position;
			}
			set
			{
				_stream.Position = value;
			}
		}

		/// <summary>
		/// Cria a instancia com a stream que será adaptada.
		/// </summary>
		/// <param name="stream"></param>
		public TracingStream(System.IO.Stream stream) : this(stream, true)
		{
		}

		/// <summary>
		/// Cria a instancia com a stream que será adaptada.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="traceContent"></param>
		public TracingStream(System.IO.Stream stream, bool traceContent)
		{
			_dataRead = new byte[0];
			_dataWritten = new byte[0];
			_stream = stream;
			_traceContent = traceContent;
		}

		/// <summary>
		/// Cria a instancia para rastrear a resposta da web.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="traceContent"></param>
		public TracingStream(System.Net.HttpWebResponse response, bool traceContent) : this(response.GetResponseStream(), traceContent)
		{
			_response = response;
		}

		/// <summary>
		/// Inicia a leitura assincrona.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return _stream.BeginRead(buffer, offset, count, callback, state);
		}

		/// <summary>
		/// Inicia a escrita assincrona.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return _stream.BeginWrite(buffer, offset, count, callback, state);
		}

		/// <summary>
		/// Finaliza a leitura da assincrona.
		/// </summary>
		/// <param name="asyncResult"></param>
		/// <returns></returns>
		public override int EndRead(IAsyncResult asyncResult)
		{
			return _stream.EndRead(asyncResult);
		}

		/// <summary>
		/// Finaliza a escrita assincrona.
		/// </summary>
		/// <param name="asyncResult"></param>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			_stream.EndWrite(asyncResult);
		}

		/// <summary>
		/// Fecha a stream.
		/// </summary>
		public override void Close()
		{
			if(_response != null)
			{
				_response.Close();
				_response = null;
			}
			else if(_stream == null)
			{
				return;
			}
			_stream.Close();
			_stream = null;
		}

		/// <summary>
		/// Flash.
		/// </summary>
		public override void Flush()
		{
			_stream.Flush();
		}

		/// <summary>
		/// Lê os dados da stream.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int length = _stream.Read(buffer, offset, count);
			if(length > 0)
			{
				_numBytesRead += length;
				if(_traceContent)
				{
					byte[] destinationArray = new byte[_dataRead.Length + length];
					Array.Copy(_dataRead, destinationArray, _dataRead.Length);
					Array.Copy(buffer, offset, destinationArray, _dataRead.Length, length);
					_dataRead = destinationArray;
				}
				OnBytesRead(buffer, offset, length);
				OnNumBytesRead();
			}
			return length;
		}

		/// <summary>
		/// Lê um byte.
		/// </summary>
		/// <returns></returns>
		public override int ReadByte()
		{
			var result = _stream.ReadByte();
			OnBytesRead(new byte[] {
				(byte)result
			}, 0, 1);
			OnNumBytesRead();
			return result;
		}

		/// <summary>
		/// Salta para a posição informada.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			return _stream.Seek(offset, origin);
		}

		/// <summary>
		/// Define o tamanho da stream.
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value)
		{
			_stream.SetLength(value);
		}

		/// <summary>
		/// Escreve na stream.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if(count > 0)
			{
				_numBytesWritten += count;
				if(_traceContent)
				{
					byte[] destinationArray = new byte[_dataWritten.Length + count];
					Array.Copy(_dataWritten, destinationArray, _dataWritten.Length);
					Array.Copy(buffer, offset, destinationArray, _dataWritten.Length, count);
					_dataWritten = destinationArray;
				}
			}
			_stream.Write(buffer, offset, count);
			if(count > 0)
			{
				OnBytesWritten(buffer, offset, count);
				OnNumBytesWrittenUpdate();
			}
		}

		/// <summary>
		/// Escreve um byte na stream.
		/// </summary>
		/// <param name="value"></param>
		public override void WriteByte(byte value)
		{
			_numBytesWritten++;
			if(_traceContent)
			{
				byte[] destinationArray = new byte[_dataWritten.Length + 1];
				Array.Copy(_dataWritten, destinationArray, _dataWritten.Length);
				destinationArray[_dataWritten.Length] = value;
				_dataWritten = destinationArray;
			}
			_stream.WriteByte(value);
			OnBytesWritten(new byte[] {
				value
			}, 0, 1);
			OnNumBytesWrittenUpdate();
		}

		/// <summary>
		/// Método acionado quando bytes forem escritos.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		protected virtual void OnBytesWritten(byte[] buffer, int offset, int count)
		{
			if(BytesWritten != null)
				BytesWritten(this, new BytesWrittenEventArgs(buffer, offset, count));
		}

		/// <summary>
		/// Método acionado quando bytes são lidos.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="read"></param>
		protected virtual void OnBytesRead(byte[] buffer, int offset, int read)
		{
			if(BytesRead != null)
				BytesRead(this, new BytesReadEventArgs(buffer, offset, read));
		}

		/// <summary>
		/// Método acionado quando a quantidade de bytes escritos for atualizada.
		/// </summary>
		protected virtual void OnNumBytesWrittenUpdate()
		{
		}

		/// <summary>
		/// Método acionado quando a quantidade de bytes lidos for atualizada.
		/// </summary>
		protected virtual void OnNumBytesRead()
		{
		}
	}
}
