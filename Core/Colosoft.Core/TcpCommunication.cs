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

using Colosoft.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Implementação da comunicação via TCP.
	/// </summary>
	public class TcpCommunication : NetworkCommunication, IDisposable
	{
		private Action<byte[]> _actionCallBack;

		private object _actionLock = new object();

		private bool _disposed;

		private CommunicationReader _reader = new CommunicationReader();

		private IAsyncResult _readResult;

		private bool _receiving;

		private System.Net.Sockets.TcpClient _tcpClient;

		private object _tcpLock = new object();

		private System.IO.Stream _tcpStream;

		private int _tcpTimeout;

		/// <summary>
		/// Evento acionado quando ocorre um erro na comunicação.
		/// </summary>
		public event EventHandler<CommunicationErrorEventArgs> ErrorOccurred;

		/// <summary>
		/// Tamanho do bloco de CheckSum.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int CheckSumBlockSize
		{
			get
			{
				return _reader.CheckSumBlockSize;
			}
			set
			{
				_reader.CheckSumBlockSize = value;
			}
		}

		/// <summary>
		/// Identifica se possui dados disponíveis.
		/// </summary>
		private bool DataAvailable
		{
			get
			{
				return (_reader.BufferAvailable || ((System.Net.Sockets.NetworkStream)Stream).DataAvailable);
			}
		}

		/// <summary>
		/// Tamanho do block do tamanho do formato.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int FormatLengthBlockSize
		{
			get
			{
				return _reader.FormatLengthBlockSize;
			}
			set
			{
				_reader.FormatLengthBlockSize = value;
			}
		}

		/// <summary>
		/// Identifica se a comunicação está aberta.
		/// </summary>
		public override bool IsOpen
		{
			get
			{
				if(_tcpClient != null && _tcpClient.Client != null)
				{
					try
					{
						bool flag = _tcpClient.Client.Poll(1000, System.Net.Sockets.SelectMode.SelectRead);
						bool flag2 = _tcpClient.Client.Poll(1000, System.Net.Sockets.SelectMode.SelectWrite);
						bool flag3 = _tcpClient.Client.Poll(1000, System.Net.Sockets.SelectMode.SelectError);
						if((flag || flag2) && !flag3)
							return _tcpClient.Connected;
					}
					catch(NotSupportedException)
					{
					}
					catch(System.Net.Sockets.SocketException)
					{
					}
					catch(ObjectDisposedException)
					{
					}
					catch(NullReferenceException)
					{
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Último erro ocorrido.
		/// </summary>
		public CommunicationResult LastError
		{
			get;
			private set;
		}

		/// <summary>
		/// Tamanho máximo dos dados.
		/// </summary>
		public int MaxDataLength
		{
			get
			{
				return _reader.MaxDataLength;
			}
			set
			{
				_reader.MaxDataLength = value;
			}
		}

		/// <summary>
		/// Stream.
		/// </summary>
		public System.IO.Stream Stream
		{
			get
			{
				return _tcpStream;
			}
		}

		/// <summary>
		/// Tempo de espera do TCP.
		/// </summary>
		public int TcpTimeout
		{
			get
			{
				return _tcpTimeout;
			}
			set
			{
				_tcpTimeout = value;
				lock (_tcpLock)
				{
					if(_tcpClient != null)
					{
						try
						{
							_tcpStream = _tcpClient.GetStream();
							if(_tcpStream != null)
							{
								_tcpStream.WriteTimeout = _tcpTimeout;
								_tcpStream.ReadTimeout = _tcpTimeout;
							}
						}
						catch(ObjectDisposedException)
						{
						}
						catch(InvalidOperationException)
						{
						}
					}
				}
			}
		}

		/// <summary>
		/// Identifica se é para usar o CheckSum.
		/// </summary>
		public bool UseCheckSum
		{
			get
			{
				return _reader.UseCheckSum;
			}
			set
			{
				_reader.UseCheckSum = value;
			}
		}

		/// <summary>
		/// Identifica se é para usar o tamanho do formato.
		/// </summary>
		public bool UseFormatLength
		{
			get
			{
				return _reader.UseFormatLength;
			}
			set
			{
				_reader.UseFormatLength = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TcpCommunication()
		{
			TcpTimeout = -1;
			LastError = CommunicationResult.Success;
		}

		/// <summary>
		/// Método acionado quando dados forem recebidos da stream da conexão.
		/// </summary>
		/// <param name="result"></param>
		private void TcpStreamReceived(IAsyncResult result)
		{
			CommunicationResult success = CommunicationResult.Success;
			try
			{
				int length = 0;
				try
				{
					lock (_tcpLock)
					{
						_receiving = false;
						if(Stream == null)
							return;
						length = Stream.EndRead(result);
						if(length == 0)
						{
							if(base.RemoteEndPoint != null)
								LogAccessor.Log.Verbose("<TCP>Connection closed by remote host [{0}]", new object[] {
									base.RemoteEndPoint.ToString()
								});
							success = CommunicationResult.Closed;
							return;
						}
						TcpCommunicationState asyncState = (TcpCommunicationState)result.AsyncState;
						if(_actionCallBack != null)
						{
							byte[] data = asyncState.GetData();
							LogAccessor.Log.Verbose("<TCP> Packet: {0}", new object[] {
								BinaryTransformer.GetString(data, 0, length, Encoding.ASCII)
							});
							success = _reader.RegisterBuffer(data, length);
						}
					}
				}
				catch(ArgumentNullException)
				{
					success = CommunicationResult.ReceiveFailed;
					return;
				}
				catch(ArgumentException)
				{
					success = CommunicationResult.ReceiveFailed;
					return;
				}
				catch(System.IO.IOException)
				{
					BeginReceive(_actionCallBack);
					return;
				}
				catch(ObjectDisposedException)
				{
					return;
				}
				if(success != 0)
				{
					ReadBuffer();
					BeginReceive(_actionCallBack);
				}
			}
			finally
			{
				HandleResult(success);
			}
		}

		/// <summary>
		/// Método base para recebimento dos dados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private CommunicationResult ReceiveBase(out byte[] data)
		{
			CommunicationResult receiveFailed = CommunicationResult.ReceiveFailed;
			data = null;
			try
			{
				lock (_tcpLock)
				{
					if(_tcpClient == null)
						receiveFailed = CommunicationResult.Closed;
					else
					{
						_tcpClient.Client.Blocking = true;
						receiveFailed = _reader.ReadStream(this.Stream, out data);
					}
				}
			}
			catch(System.Net.Sockets.SocketException exception)
			{
				receiveFailed = new CommunicationResult(exception.ErrorCode);
			}
			catch(ObjectDisposedException)
			{
				receiveFailed = CommunicationResult.Closed;
			}
			if((receiveFailed != 0) && (base.RemoteEndPoint != null) && data != null)
			{
				LogAccessor.Log.Verbose("<TCP>Received from [{0}]: {1}", new object[] {
					base.RemoteEndPoint.ToString(),
					BinaryTransformer.GetString(data, 0, data.Length)
				});
			}
			return receiveFailed;
		}

		/// <summary>
		/// Tenta abrir a comunicação.
		/// </summary>
		/// <param name="asyncResult"></param>
		/// <returns></returns>
		private CommunicationResult TryOpen(IAsyncResult asyncResult)
		{
			try
			{
				if(_tcpClient != null && _tcpClient.Client != null)
				{
					_tcpClient.EndConnect(asyncResult);
					if(_tcpClient.Connected)
					{
						LogAccessor.Log.Info("<TCP>Connected to [{0}]", new object[] {
							base.RemoteEndPoint.ToString()
						});
						return CommunicationResult.Success;
					}
					return CommunicationResult.OpenFailed;
				}
				return CommunicationResult.OpenFailed;
			}
			catch(ArgumentNullException)
			{
				return CommunicationResult.OpenFailed;
			}
			catch(ArgumentException)
			{
				return CommunicationResult.OpenFailed;
			}
			catch(ObjectDisposedException)
			{
				return CommunicationResult.OpenFailed;
			}
			catch(NullReferenceException)
			{
				return CommunicationResult.OpenFailed;
			}
			catch(InvalidOperationException)
			{
				return CommunicationResult.OpenFailed;
			}
			catch(System.Net.Sockets.SocketException exception)
			{
				if(exception.ErrorCode == 0x2748)
					return CommunicationResult.AlreadyOpen;
				return new CommunicationResult(exception.ErrorCode);
			}
		}

		/// <summary>
		/// Tranta o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private CommunicationResult HandleResult(CommunicationResult result)
		{
			if(result == 0)
			{
				this.LastError = result;
				LogAccessor.Log.Info(result.GetDebugInfo());
				if(ErrorOccurred != null)
					ErrorOccurred(this, new CommunicationErrorEventArgs(CommunicationProtocol.Tcp, result));
			}
			return result;
		}

		/// <summary>
		/// Lê o buffer.
		/// </summary>
		private void ReadBuffer()
		{
			object obj2;
			while (true)
			{
				System.Threading.Monitor.Enter(obj2 = this._actionLock);
				try
				{
					byte[] buffer;
					if(_reader.ReadBuffer(out buffer))
					{
						if(buffer != null)
						{
							if(base.RemoteEndPoint != null)
								LogAccessor.Log.Verbose("<TCP>Received from [{0}]: {1}", base.RemoteEndPoint.ToString(), BinaryTransformer.GetString(buffer, 0, buffer.Length));
							try
							{
								if(_actionCallBack != null)
									_actionCallBack(buffer);
							}
							catch(NullReferenceException)
							{
								LogAccessor.Log.Verbose("<TCP>ReadBuffer aborted (null action).");
								return;
							}
							catch(ObjectDisposedException)
							{
								return;
							}
						}
						continue;
					}
					break;
				}
				finally
				{
					System.Threading.Monitor.Exit(obj2);
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
		protected override void Dispose(bool disposing)
		{
			if(!_disposed)
			{
				if(disposing)
					Close();
				_disposed = true;
			}
		}

		/// <summary>
		/// Adiciona um terminador do cabeçalho.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="terminator"></param>
		public void AddHeaderTerminator(byte[] header, byte[] terminator)
		{
			_reader.AddHeaderTerminator(header, terminator);
		}

		/// <summary>
		/// Inicia o processo assincrono para recebimento dos dados.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public CommunicationResult BeginReceive(Action<byte[]> action)
		{
			try
			{
				lock (_actionLock)
					_actionCallBack = action;
				LogAccessor.Log.Verbose("<TCP> Begin Receive");
				this.ReadBuffer();
				CommunicationResult success = CommunicationResult.Success;
				lock (_tcpLock)
				{
					if(_receiving || ((_readResult != null) && !_readResult.IsCompleted))
						LogAccessor.Log.Verbose("<TCP> Already Receiving=>Skip Begin Receive");
					else if(this.Stream == null)
					{
						success = CommunicationResult.Closed;
						LogAccessor.Log.Verbose("<TCP> Connection is Closed=>Skip Begin Receive");
					}
					else
					{
						byte[] data = new byte[1024];
						TcpCommunicationState state = new TcpCommunicationState(data);
						_receiving = true;
						_readResult = Stream.BeginRead(data, 0, data.Length, new AsyncCallback(TcpStreamReceived), state);
					}
				}
				return HandleResult(success);
			}
			catch(System.IO.IOException)
			{
				_receiving = false;
				return this.HandleResult(CommunicationResult.Closed);
			}
			catch(ObjectDisposedException)
			{
				_receiving = false;
				return HandleResult(CommunicationResult.Closed);
			}
			catch(ArgumentException)
			{
				_receiving = false;
			}
			catch(NotSupportedException)
			{
				_receiving = false;
				return HandleResult(CommunicationResult.Closed);
			}
			catch(InvalidOperationException)
			{
				_receiving = false;
				return HandleResult(CommunicationResult.Closed);
			}
			return HandleResult(CommunicationResult.ReceiveFailed);
		}

		/// <summary>
		/// Inicia o processo assincrono para recebimento dos dados.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public override CommunicationResult BeginReceive(Action<byte[], System.Net.IPEndPoint> action)
		{
			return HandleResult(CommunicationResult.ReceiveFailed);
		}

		/// <summary>
		/// Remove os terminadores do cabeçalho.
		/// </summary>
		public void CleanHeaderTermiators()
		{
			_reader.CleanHeaderTermiators();
		}

		/// <summary>
		/// Limpa o buffer.
		/// </summary>
		public void ClearBuffer()
		{
			lock (_tcpLock)
			{
				if(Stream != null)
				{
					byte[] buffer = new byte[1024];
					try
					{
						while ((_tcpClient.Available > 0) && (Stream.Read(buffer, 0, 1024) != 0))
						{
						}
					}
					catch(ArgumentNullException)
					{
					}
					catch(ArgumentOutOfRangeException)
					{
					}
					catch(ArgumentException)
					{
					}
					catch(System.IO.IOException)
					{
					}
					catch(NotSupportedException)
					{
					}
					catch(ObjectDisposedException)
					{
					}
					catch(System.Net.Sockets.SocketException)
					{
					}
					catch(NullReferenceException)
					{
					}
				}
				_reader.CleanBuffers();
			}
		}

		/// <summary>
		/// Fecha a comunicação.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult Close()
		{
			lock (_tcpLock)
				return Close(false);
		}

		/// <summary>
		/// Fecha a comunicação.
		/// </summary>
		/// <param name="force">Identifica se é para força o fechamento.</param>
		/// <returns></returns>
		internal CommunicationResult Close(bool force)
		{
			System.Net.Sockets.TcpClient client = _tcpClient;
			this._receiving = false;
			if(client != null)
			{
				var socket = client.Client;
				try
				{
					if(socket != null)
						socket.Shutdown(System.Net.Sockets.SocketShutdown.Send);
					if(!force)
						ClearBuffer();
					if(socket != null)
					{
						socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
						socket.Close();
					}
				}
				catch(System.Net.Sockets.SocketException)
				{
				}
				catch(ObjectDisposedException)
				{
				}
				if(base.RemoteEndPoint != null)
					LogAccessor.Log.Info("<TCP>Closed connection [{0}]", new object[] {
						base.RemoteEndPoint.ToString()
					});
			}
			lock (_tcpLock)
			{
				if(_tcpClient != null)
					_tcpClient.Close();
				_tcpClient = null;
				_tcpStream = null;
			}
			return CommunicationResult.Success;
		}

		/// <summary>
		/// Finaliza o recebimento assincrono dos dados.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult EndReceive()
		{
			try
			{
				lock (_tcpLock)
				{
					_actionCallBack = null;
					if(_readResult == null)
						return CommunicationResult.Success;
					if(_readResult.AsyncWaitHandle == null)
						return CommunicationResult.Success;
					TcpCommunicationState asyncState = (TcpCommunicationState)_readResult.AsyncState;
					_readResult.AsyncWaitHandle.Close();
					LogAccessor.Log.Verbose("<TCP> End Receive");
				}
				return CommunicationResult.Success;
			}
			catch(ObjectDisposedException)
			{
				return CommunicationResult.Success;
			}
		}

		/// <summary>
		/// Recupera o dados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="dataSize">Tamanho dos dados.</param>
		/// <returns></returns>
		public byte[] GetRawData(byte[] data, int dataSize)
		{
			return _reader.GetRawData(data, dataSize);
		}

		/// <summary>
		/// Abrea a comunicação.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult Open()
		{
			CommunicationResult result = CommunicationResult.OpenFailed;
			try
			{
				lock (_tcpLock)
				{
					LastError = CommunicationResult.Success;
					if((_tcpClient == null) || (_tcpClient.Client == null))
					{
						_tcpClient = new System.Net.Sockets.TcpClient();
						_receiving = false;
						_readResult = null;
					}
					AsyncCallback callback2 = null;
					using (var resetEvent = new System.Threading.ManualResetEvent(false))
					{
						if(callback2 == null)
						{
							callback2 = delegate(IAsyncResult asyncResult) {
								result = TryOpen(asyncResult);
								try
								{
									resetEvent.Set();
								}
								catch(NullReferenceException)
								{
								}
								catch(ObjectDisposedException)
								{
								}
							};
						}
						AsyncCallback requestCallback = callback2;
						if((_tcpClient == null) || (_tcpClient.Client == null))
							return CommunicationResult.OpenFailed;
						_tcpClient.BeginConnect(RemoteEndPoint.Address, base.RemoteEndPoint.Port, requestCallback, _tcpClient);
						if(!resetEvent.WaitOne(TcpTimeout, true))
						{
							try
							{
								_tcpClient.Close();
							}
							finally
							{
								result = CommunicationResult.OpenFailed;
							}
						}
					}
				}
			}
			catch(ArgumentNullException)
			{
				result = CommunicationResult.OpenFailed;
			}
			catch(ArgumentOutOfRangeException)
			{
				result = CommunicationResult.OpenFailed;
			}
			catch(ObjectDisposedException)
			{
				result = CommunicationResult.OpenFailed;
			}
			catch(System.Net.Sockets.SocketException exception)
			{
				if(exception.ErrorCode == 0x2748)
				{
					result = CommunicationResult.AlreadyOpen;
				}
				else
				{
					result = new CommunicationResult(exception.ErrorCode);
				}
			}
			catch(InvalidOperationException)
			{
				result = CommunicationResult.OpenFailed;
			}
			catch(System.Security.SecurityException)
			{
				result = CommunicationResult.OpenFailed;
			}
			catch(System.Threading.AbandonedMutexException)
			{
				result = CommunicationResult.OpenFailed;
			}
			finally
			{
				if((result.ErrorCode == 1) && !this.IsOpen)
					result = CommunicationResult.OpenFailed;
				if((result != 0) || (result.ErrorCode == 1))
				{
					try
					{
						_tcpStream = _tcpClient.GetStream();
						if(_tcpStream != null)
						{
							_tcpStream.WriteTimeout = TcpTimeout;
							_tcpStream.ReadTimeout = TcpTimeout;
						}
					}
					catch(ObjectDisposedException)
					{
						result = CommunicationResult.Closed;
					}
					catch(InvalidOperationException)
					{
						result = CommunicationResult.Closed;
					}
				}
				else
				{
					_tcpClient = null;
					HandleResult(result);
				}
			}
			return result;
		}

		/// <summary>
		/// Recebe os dados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override CommunicationResult Receive(out byte[] data)
		{
			return this.HandleResult(ReceiveBase(out data));
		}

		/// <summary>
		/// Recebe os dados.
		/// </summary>
		/// <param name="data">Buffer dos dados recebidos.</param>
		/// <param name="expectedSize">Tamanho esperado.</param>
		/// <returns></returns>
		public CommunicationResult Receive(out byte[] data, int expectedSize)
		{
			CommunicationResult receiveFailed = CommunicationResult.ReceiveFailed;
			lock (_tcpLock)
				receiveFailed = _reader.ReadStream(this.Stream, out data, expectedSize);
			if(receiveFailed == 0)
				return HandleResult(receiveFailed);
			if(base.RemoteEndPoint != null)
				LogAccessor.Log.Verbose("<TCP>Received from [{0}]: {1}", base.RemoteEndPoint.ToString(), BinaryTransformer.GetString(data, 0, data.Length));
			return receiveFailed;
		}

		/// <summary>
		/// Recebe os dados de forma assincrona.
		/// </summary>
		/// <param name="receiveAction"></param>
		/// <param name="timeoutAction"></param>
		/// <param name="expectResponse"></param>
		/// <returns></returns>
		public override CommunicationResult ReceiveAsync(Action<byte[], System.Net.IPEndPoint> receiveAction, Action<System.Net.IPEndPoint> timeoutAction, bool expectResponse)
		{
			System.Threading.ThreadStart start = delegate {
				byte[] data = null;
				CommunicationResult result = CommunicationResult.ReceiveFailed;
				int tcpTimeout = this.TcpTimeout;
				while (true)
				{
					lock (_tcpLock)
					{
						if(Stream == null)
						{
							result = CommunicationResult.Closed;
							break;
						}
						if(this.DataAvailable)
						{
							result = this.ReceiveBase(out data);
							break;
						}
					}
					if(tcpTimeout <= 0)
					{
						result = CommunicationResult.Timeout;
						break;
					}
					System.Threading.Thread.Sleep(Math.Min(10, tcpTimeout));
					tcpTimeout = Math.Max(tcpTimeout - 10, 0);
				}
				if(result == 0)
				{
					if((result == CommunicationResult.Timeout) || (result == 0x274c))
					{
						timeoutAction(RemoteEndPoint);
					}
					if(expectResponse)
						HandleResult(result);
				}
				else
				{
					receiveAction(data, this.RemoteEndPoint);
				}
			};
			try
			{
				new System.Threading.Thread(start).Start();
			}
			catch(ArgumentNullException)
			{
				return this.HandleResult(CommunicationResult.ReceiveFailed);
			}
			catch(System.Threading.ThreadStateException)
			{
				return this.HandleResult(CommunicationResult.ReceiveFailed);
			}
			return CommunicationResult.Success;
		}

		/// <summary>
		/// Envia os dados.
		/// </summary>
		/// <param name="data">Dados que serão enviados.</param>
		/// <param name="size">Tamanho dos dados.</param>
		/// <returns></returns>
		public override CommunicationResult Send(byte[] data, int size)
		{
			CommunicationResult success = CommunicationResult.Success;
			try
			{
				lock (_tcpLock)
				{
					if(Stream != null)
					{
						Stream.Write(data, 0, size);
						Stream.Flush();
					}
					else
					{
						success = CommunicationResult.Closed;
					}
				}
				if((success != 0) && (base.RemoteEndPoint != null))
				{
					LogAccessor.Log.Verbose("<TCP>Sent to [{0}]: {1}", base.RemoteEndPoint.ToString(), BinaryTransformer.GetString(data, 0, size));
				}
			}
			catch(ArgumentNullException)
			{
				return this.HandleResult(CommunicationResult.SendFailed);
			}
			catch(ArgumentOutOfRangeException)
			{
				return this.HandleResult(CommunicationResult.SendFailed);
			}
			catch(ArgumentException)
			{
				return this.HandleResult(CommunicationResult.SendFailed);
			}
			catch(NotSupportedException)
			{
				return this.HandleResult(CommunicationResult.SendFailed);
			}
			catch(ObjectDisposedException)
			{
				return this.HandleResult(CommunicationResult.Closed);
			}
			catch(System.IO.IOException exception)
			{
				var innerException = exception.InnerException as System.Net.Sockets.SocketException;
				if(innerException != null)
					return HandleResult(new CommunicationResult(innerException.ErrorCode));
				return HandleResult(CommunicationResult.Closed);
			}
			return HandleResult(success);
		}

		/// <summary>
		/// Estado da comunicação.
		/// </summary>
		class TcpCommunicationState
		{
			private byte[] _data;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="data"></param>
			public TcpCommunicationState(byte[] data)
			{
				_data = data;
			}

			/// <summary>
			/// Recupera os dados.
			/// </summary>
			/// <returns></returns>
			public byte[] GetData()
			{
				return _data;
			}
		}
	}
}
