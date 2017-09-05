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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Leitor de comunicação.
	/// </summary>
	public class CommunicationReader
	{
		private int _bufferIndex;

		private List<byte[]> _bufferList = new List<byte[]>();

		private byte[] _bufferRead = new byte[1024];

		private int _bufferReadIndex;

		private int _checkSumBlockSize = 2;

		private int _formatLengthBlockSize = 4;

		private object _formatLock = new object();

		private HeaderTerminator _headerTerminator;

		private IList<HeaderTerminator> _headerTerminatorList = new List<HeaderTerminator>();

		private int _leftOverSize;

		private int _maxDataLength = 0x2710;

		/// <summary>
		/// Identifica se o buffer está disponível.
		/// </summary>
		internal bool BufferAvailable
		{
			get
			{
				return ((1 < _bufferList.Count) || ((_bufferList.Count == 1) && (_bufferIndex < _bufferList[0].Length)));
			}
		}

		/// <summary>
		/// Tamanho do bloco co checksum.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal int CheckSumBlockSize
		{
			get
			{
				return _checkSumBlockSize;
			}
			set
			{
				_checkSumBlockSize = value;
			}
		}

		/// <summary>
		/// Tamanho do bloco de tamanho do formato.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal int FormatLengthBlockSize
		{
			get
			{
				return _formatLengthBlockSize;
			}
			set
			{
				_formatLengthBlockSize = value;
			}
		}

		/// <summary>
		/// Cabeçalho.
		/// </summary>
		private byte[] Header
		{
			get
			{
				return _headerTerminatorList[0].Header;
			}
		}

		/// <summary>
		/// Tamanho máximo dos dados recebidos.
		/// </summary>
		internal int MaxDataLength
		{
			get
			{
				return _maxDataLength;
			}
			set
			{
				_maxDataLength = value;
			}
		}

		/// <summary>
		/// Terminador.
		/// </summary>
		private byte[] Terminator
		{
			get
			{
				return _headerTerminatorList[0].Terminators[0];
			}
		}

		/// <summary>
		/// Identifica se é para usar o checksum.
		/// </summary>
		internal bool UseCheckSum
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para usar o tamanho do formato.
		/// </summary>
		internal bool UseFormatLength
		{
			get;
			set;
		}

		/// <summary>
		/// Verifica se os dados informados terminam com o terminador informado.
		/// </summary>
		/// <param name="data">Buffer.</param>
		/// <param name="startIndex">Indice de inicio para leitura do buffer.</param>
		/// <param name="length">Tamanho dos dados que devem ser lidos.</param>
		/// <param name="headerTerminator"></param>
		/// <returns></returns>
		private static bool EndsWithTerminator(byte[] data, int startIndex, int length, HeaderTerminator headerTerminator)
		{
			if(headerTerminator != null)
			{
				foreach (byte[] buffer in headerTerminator.Terminators)
				{
					if(IsNone(buffer))
					{
						return false;
					}
					if(length < (startIndex + GetTerminatorLength(buffer)))
					{
						return false;
					}
					if(Matches(buffer, 0, GetTerminatorLength(buffer), data, length, true))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera o tamanho do cabeçalho.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static int GetHeaderLength(byte[] data)
		{
			if(IsNone(data))
				return 0;
			return data.Length;
		}

		/// <summary>
		/// Recupera o terminador com base nos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private HeaderTerminator GetHeaderTerminator(byte[] data, int length)
		{
			HeaderTerminator terminator = null;
			lock (_formatLock)
			{
				foreach (HeaderTerminator terminator2 in this._headerTerminatorList)
				{
					byte[] header = terminator2.Header;
					if((IsNone(header) || Matches(header, 0, header.Length, data, length, false)) && ((terminator == null) || (GetHeaderLength(terminator.Header) < GetHeaderLength(header))))
					{
						terminator = terminator2;
					}
				}
			}
			return terminator;
		}

		/// <summary>
		/// Recupera o tamanho do terminador.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static int GetTerminatorLength(byte[] data)
		{
			if(IsNone(data))
				return 0;
			return data.Length;
		}

		/// <summary>
		/// Verifica se o buffer de dados é inválido.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static bool IsNone(byte[] data)
		{
			if(data != null)
			{
				if(data.Length != 1)
				{
					return false;
				}
				if(data[0] != 0xff)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Compara os buffers informados.
		/// </summary>
		/// <param name="pattern">Padrão que será comparado.</param>
		/// <param name="patternIndex"></param>
		/// <param name="patternLength"></param>
		/// <param name="data">Dados que será comparados</param>
		/// <param name="length"></param>
		/// <param name="fromEnd"></param>
		/// <returns></returns>
		private static bool Matches(byte[] pattern, int patternIndex, int patternLength, byte[] data, int length, bool fromEnd)
		{
			if(length < patternLength)
			{
				return false;
			}
			int num = 0;
			if(fromEnd)
			{
				num = length - patternLength;
			}
			for(int i = 0; i < patternLength; i++)
			{
				if(data[num + i] != pattern[patternIndex + i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Adiciona o terminador do cabeçalho.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="terminator"></param>
		public void AddHeaderTerminator(byte[] header, byte[] terminator)
		{
			lock (_formatLock)
			{
				foreach (HeaderTerminator terminator2 in this._headerTerminatorList)
				{
					if(Matches(header, 0, header.Length, terminator2.Header, GetHeaderLength(header), false))
					{
						foreach (byte[] buffer in terminator2.Terminators)
						{
							if(Matches(terminator, 0, terminator.Length, buffer, GetTerminatorLength(buffer), false))
								return;
						}
						terminator2.Terminators.Add(terminator);
					}
				}
				HeaderTerminator item = new HeaderTerminator {
					Header = header
				};
				item.Terminators.Add(terminator);
				_headerTerminatorList.Add(item);
			}
		}

		/// <summary>
		/// Limpa os buffers.
		/// </summary>
		public void CleanBuffers()
		{
			_bufferList.Clear();
			_bufferIndex = 0;
			_bufferReadIndex = 0;
			_leftOverSize = 0;
			_headerTerminator = null;
		}

		/// <summary>
		/// Limpa os terminadores do cabeçalho.
		/// </summary>
		public void CleanHeaderTermiators()
		{
			lock (_formatLock)
				_headerTerminatorList.Clear();
		}

		/// <summary>
		/// Recupera os dados do buffer informado.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="dataSize">Tamanho dos dados no buffer.</param>
		/// <returns></returns>
		public byte[] GetRawData(byte[] data, int dataSize)
		{
			int headerLength = 0;
			int terminatorLength = 0;
			int formatLengthBlockSize = 0;
			int checkSumBlockSize = 0;
			headerLength = GetHeaderLength(this.Header);
			terminatorLength = GetTerminatorLength(this.Terminator);
			if(this.UseFormatLength)
				formatLengthBlockSize = this.FormatLengthBlockSize;
			if(this.UseCheckSum)
				checkSumBlockSize = this.CheckSumBlockSize;
			int length = (((dataSize - headerLength) - formatLengthBlockSize) - checkSumBlockSize) - terminatorLength;
			if(length < 0)
			{
				return data;
			}
			byte[] destinationArray = new byte[length];
			Array.Copy(data, headerLength + formatLengthBlockSize, destinationArray, 0, length);
			return destinationArray;
		}

		/// <summary>
		/// Lê o buffer.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool ReadBuffer(out byte[] data)
		{
			data = null;
			try
			{
				int startIndex = 0;
				while (0 < _bufferList.Count)
				{
					byte[] buffer = _bufferList[0];
					if(buffer != null)
					{
						while (_bufferIndex < buffer.Length)
						{
							if(this._bufferRead.Length <= _bufferReadIndex)
							{
								Array.Resize<byte>(ref _bufferRead, _bufferRead.Length * 2);
							}
							_bufferRead[_bufferReadIndex] = buffer[_bufferIndex];
							_bufferReadIndex++;
							_bufferIndex++;
							if((_headerTerminator == null) && (_bufferReadIndex <= 5))
							{
								_headerTerminator = GetHeaderTerminator(_bufferRead, _bufferReadIndex);
								if(_headerTerminator != null)
								{
									startIndex = GetHeaderLength(_headerTerminator.Header);
								}
							}
							bool flag = EndsWithTerminator(_bufferRead, startIndex, _bufferReadIndex, _headerTerminator);
							if(flag)
							{
								_headerTerminator = null;
							}
							if((flag || (MaxDataLength <= _bufferReadIndex)) || UseFormatLength)
							{
								int num2;
								if(0 < _leftOverSize)
								{
									return ReadBuffer(out data, _leftOverSize);
								}
								if(UseFormatLength && ReadDataLength(out num2))
								{
									return ReadBuffer(out data, num2);
								}
								if(!UseFormatLength && (flag || (MaxDataLength <= _bufferReadIndex)))
								{
									Array.Resize<byte>(ref _bufferRead, _bufferReadIndex);
									data = this._bufferRead;
									_bufferRead = new byte[1024];
									_bufferReadIndex = 0;
									return true;
								}
							}
						}
					}
					_bufferList.RemoveAt(0);
					_bufferIndex = 0;
				}
			}
			catch(ArgumentOutOfRangeException)
			{
				return false;
			}
			return false;
		}

		/// <summary>
		/// Lê o buffer.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="expectedSize">Tamanho esperado.</param>
		/// <returns></returns>
		public bool ReadBuffer(out byte[] data, int expectedSize)
		{
			_headerTerminator = null;
			int maxDataLength = expectedSize;
			if(MaxDataLength < maxDataLength)
				maxDataLength = MaxDataLength;
			data = null;
			try
			{
				while (0 < _bufferList.Count)
				{
					byte[] buffer = _bufferList[0];
					if(buffer != null)
					{
						while (_bufferIndex < buffer.Length)
						{
							if(maxDataLength <= _bufferReadIndex)
								break;
							if(_bufferRead.Length <= _bufferReadIndex)
							{
								Array.Resize<byte>(ref _bufferRead, _bufferRead.Length * 2);
							}
							_bufferRead[_bufferReadIndex] = buffer[_bufferIndex];
							_bufferReadIndex++;
							_bufferIndex++;
						}
						if(maxDataLength <= _bufferReadIndex)
						{
							Array.Resize<byte>(ref _bufferRead, _bufferReadIndex);
							data = _bufferRead;
							_bufferRead = new byte[1024];
							_bufferReadIndex = 0;
							_leftOverSize = expectedSize - data.Length;
							return true;
						}
					}
					_bufferList.RemoveAt(0);
					_bufferIndex = 0;
				}
			}
			catch(ArgumentOutOfRangeException)
			{
				return false;
			}
			return false;
		}

		/// <summary>
		/// Lê o tamanho dos dados.
		/// </summary>
		/// <param name="dataLength"></param>
		/// <returns></returns>
		public bool ReadDataLength(out int dataLength)
		{
			int num3;
			dataLength = 0;
			int start = 0;
			int terminatorLength = 0;
			start = GetHeaderLength(Header);
			terminatorLength = GetTerminatorLength(this.Terminator);
			if(_bufferReadIndex < ((start + FormatLengthBlockSize) + terminatorLength))
				return false;
			if(!Colosoft.Serialization.BinaryTransformer.TryGetInteger(_bufferRead, start, FormatLengthBlockSize, Encoding.ASCII, out num3))
				return false;
			dataLength = (start + num3) + terminatorLength;
			return true;
		}

		/// <summary>
		/// Lê a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public CommunicationResult ReadStream(Stream stream, out byte[] data)
		{
			data = null;
			try
			{
				if(stream == null)
					return CommunicationResult.Closed;
				if(this.ReadBuffer(out data))
					return CommunicationResult.Success;
				int bufferSize = 0;
				byte[] buffer = new byte[0x400];
				var stopwatch = new System.Diagnostics.Stopwatch();
				stopwatch.Start();
				while ((bufferSize = stream.Read(buffer, 0, 0x400)) != -1)
				{
					if(bufferSize <= 0)
					{
						if(stopwatch.Elapsed.TotalMilliseconds >= stream.ReadTimeout)
						{
							LogAccessor.Log.Verbose("<COMMUNICATION READER> Reading Timeout => Exiting Loop");
							return CommunicationResult.Timeout;
						}
					}
					else
					{
						CommunicationResult result = RegisterBuffer(buffer, bufferSize);
						if(result == 0)
							return result;
						if(ReadBuffer(out data))
							return CommunicationResult.Success;
					}
				}
			}
			catch(ArgumentNullException)
			{
				return CommunicationResult.ReceiveFailed;
			}
			catch(ArgumentOutOfRangeException)
			{
				return CommunicationResult.ReceiveFailed;
			}
			catch(UnauthorizedAccessException)
			{
				return CommunicationResult.Closed;
			}
			catch(ObjectDisposedException)
			{
				return CommunicationResult.Closed;
			}
			catch(IOException exception)
			{
				var innerException = exception.InnerException as System.Net.Sockets.SocketException;
				if(innerException != null)
				{
					return new CommunicationResult(innerException.ErrorCode);
				}
				return CommunicationResult.Timeout;
			}
			catch(NotSupportedException)
			{
				return CommunicationResult.Closed;
			}
			catch(System.Net.Sockets.SocketException exception3)
			{
				return new CommunicationResult(exception3.ErrorCode);
			}
			catch(TimeoutException)
			{
				return CommunicationResult.Timeout;
			}
			return CommunicationResult.Timeout;
		}

		/// <summary>
		/// Lê a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="data"></param>
		/// <param name="expectedSize"></param>
		/// <returns></returns>
		public CommunicationResult ReadStream(Stream stream, out byte[] data, int expectedSize)
		{
			data = null;
			try
			{
				if(stream == null)
					return CommunicationResult.Closed;
				if(ReadBuffer(out data, expectedSize))
					return CommunicationResult.Success;
				int bufferSize = 0;
				byte[] buffer = new byte[1024];
				var stopwatch = new System.Diagnostics.Stopwatch();
				stopwatch.Start();
				while ((bufferSize = stream.Read(buffer, 0, 0x400)) != -1)
				{
					if(bufferSize <= 0)
					{
						if(stopwatch.Elapsed.TotalMilliseconds >= stream.ReadTimeout)
						{
							LogAccessor.Log.Verbose("<COMMUNICATION READER> Reading Timeout => Exiting Loop");
							return CommunicationResult.Timeout;
						}
					}
					else
					{
						CommunicationResult result = RegisterBuffer(buffer, bufferSize);
						if(result == 0)
							return result;
						if(ReadBuffer(out data, expectedSize))
							return CommunicationResult.Success;
					}
				}
			}
			catch(ArgumentNullException)
			{
				return CommunicationResult.ReceiveFailed;
			}
			catch(ArgumentOutOfRangeException)
			{
				return CommunicationResult.ReceiveFailed;
			}
			catch(UnauthorizedAccessException)
			{
				return CommunicationResult.Closed;
			}
			catch(ObjectDisposedException)
			{
				return CommunicationResult.Closed;
			}
			catch(IOException exception)
			{
				var innerException = exception.InnerException as System.Net.Sockets.SocketException;
				if(innerException != null)
				{
					return new CommunicationResult(innerException.ErrorCode);
				}
				return CommunicationResult.Timeout;
			}
			catch(NotSupportedException)
			{
				return CommunicationResult.Closed;
			}
			catch(System.Net.Sockets.SocketException exception3)
			{
				return new CommunicationResult(exception3.ErrorCode);
			}
			catch(TimeoutException)
			{
				return CommunicationResult.Timeout;
			}
			return CommunicationResult.ReceiveFailed;
		}

		/// <summary>
		/// Registra o buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <param name="bufferSize">Tamanho do buffer.</param>
		/// <returns></returns>
		public CommunicationResult RegisterBuffer(byte[] buffer, int bufferSize)
		{
			try
			{
				if(buffer.Length != bufferSize)
				{
					Array.Resize<byte>(ref buffer, bufferSize);
				}
				_bufferList.Add(buffer);
				return CommunicationResult.Success;
			}
			catch(ArgumentOutOfRangeException)
			{
			}
			return CommunicationResult.ReceiveFailed;
		}

		/// <summary>
		/// Armazena o cabeçalho e os terminadores.
		/// </summary>
		class HeaderTerminator
		{
			private List<byte[]> _terminatorList = new List<byte[]>();

			/// <summary>
			/// Cabeçalho.
			/// </summary>
			public byte[] Header
			{
				get;
				set;
			}

			/// <summary>
			/// Terminadores.
			/// </summary>
			public IList<byte[]> Terminators
			{
				get
				{
					return this._terminatorList;
				}
			}
		}
	}
}
