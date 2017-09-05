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
using System.Net.Sockets;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Implementação da comunicação sobre UDP.
	/// </summary>
	public class UdpCommunication : NetworkCommunication
	{
		private Action<byte[], System.Net.IPEndPoint> _actionCallBack;

		private bool _disposed;

		private bool _isReceiving;

		private System.Net.IPEndPoint _localEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);

		private object _lockObject = new object();

		private bool _needBinding;

		private UdpClient _udpClient = new UdpClient();

		/// <summary>
		/// Evento acionado quando ocorrer um erro na comunicação.
		/// </summary>
		public event EventHandler<CommunicationErrorEventArgs> ErrorOccurred;

		/// <summary>
		/// Identifica se a comunicação está aberta.
		/// </summary>
		public override bool IsOpen
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// EndPoint local.
		/// </summary>
		public System.Net.IPEndPoint LocalEndPoint
		{
			get
			{
				return _localEndPoint;
			}
			set
			{
				if(!_localEndPoint.Equals(value))
				{
					_localEndPoint = value;
					_needBinding = true;
				}
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~UdpCommunication()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método acionado quando for recebido uma mensagem do cliente UDP.
		/// </summary>
		/// <param name="result"></param>
		private void UdpClientReceive(IAsyncResult result)
		{
			CommunicationResult success = CommunicationResult.Success;
			try
			{
				byte[] buffer;
				var remoteEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
				lock (_lockObject)
				{
					if(base.RemoteEndPoint != null)
						remoteEP = base.RemoteEndPoint;
					buffer = _udpClient.EndReceive(result, ref remoteEP);
				}
				_actionCallBack(buffer, remoteEP);
				lock (_lockObject)
				{
					if(_isReceiving)
					{
						if(_udpClient == null)
							success = CommunicationResult.Closed;
						else
							_udpClient.BeginReceive(new AsyncCallback(UdpClientReceive), _udpClient);
					}
				}
			}
			catch(ArgumentNullException)
			{
				success = CommunicationResult.Closed;
			}
			catch(ArgumentException)
			{
				success = CommunicationResult.Closed;
			}
			catch(ObjectDisposedException)
			{
				success = CommunicationResult.Closed;
			}
			catch(InvalidOperationException)
			{
				success = CommunicationResult.Closed;
			}
			catch(SocketException exception)
			{
				success = new CommunicationResult(exception.ErrorCode);
			}
			catch(NullReferenceException)
			{
				success = CommunicationResult.Closed;
			}
			finally
			{
				HandleResult(success);
			}
		}

		/// <summary>
		/// Vincula o EndPoint.
		/// </summary>
		/// <param name="localEndPoint"></param>
		/// <returns></returns>
		private CommunicationResult Bind(System.Net.IPEndPoint localEndPoint)
		{
			CommunicationResult success = CommunicationResult.Success;
			lock (this._lockObject)
			{
				if(_udpClient != null)
				{
					if(!this._needBinding)
					{
						return CommunicationResult.Success;
					}
					_udpClient.Close();
					_udpClient = null;
				}
				try
				{
					_udpClient = new UdpClient(localEndPoint);
					_needBinding = false;
				}
				catch(SocketException exception)
				{
					_udpClient = new UdpClient();
					success = new CommunicationResult(exception.ErrorCode);
				}
			}
			return HandleResult(success);
		}

		/// <summary>
		/// Despacha os dados para o endPoint informado.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="size"></param>
		/// <param name="remoteEndPoint"></param>
		/// <returns></returns>
		private CommunicationResult Dispatch(byte[] data, int size, System.Net.IPEndPoint remoteEndPoint)
		{
			CommunicationResult success = CommunicationResult.Success;
			lock (_lockObject)
			{
				CommunicationResult result2 = Bind(LocalEndPoint);
				if(result2 == 0)
					return result2;
				try
				{
					_udpClient.Send(data, size, remoteEndPoint);
				}
				catch(ArgumentNullException)
				{
					success = CommunicationResult.SendFailed;
				}
				catch(ObjectDisposedException)
				{
					success = CommunicationResult.Closed;
				}
				catch(InvalidOperationException)
				{
					success = CommunicationResult.SendFailed;
				}
				catch(SocketException exception)
				{
					success = new CommunicationResult(exception.ErrorCode);
				}
			}
			return HandleResult(success);
		}

		/// <summary>
		/// Trata o resultado da comunicação.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private CommunicationResult HandleResult(CommunicationResult result)
		{
			if(result == 0)
			{
				if((result != CommunicationResult.Closed) || this._isReceiving)
					LogAccessor.Log.Info(result.GetDebugInfo());
				if(ErrorOccurred != null)
					ErrorOccurred(this, new CommunicationErrorEventArgs(CommunicationProtocol.Udp, result));
			}
			return result;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			lock (_lockObject)
			{
				if(!_disposed)
				{
					if(disposing && (_udpClient != null))
					{
						try
						{
							_udpClient.Close();
							_udpClient = null;
						}
						catch(SocketException)
						{
						}
					}
					_disposed = true;
				}
			}
		}

		/// <summary>
		/// Inicia o recebimento de dados assincrono.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public override CommunicationResult BeginReceive(Action<byte[], System.Net.IPEndPoint> action)
		{
			CommunicationResult success = CommunicationResult.Success;
			lock (this._lockObject)
			{
				CommunicationResult result2 = Bind(LocalEndPoint);
				if(result2 == 0)
					return result2;
				_isReceiving = true;
				_actionCallBack = action;
				try
				{
					_udpClient.BeginReceive(new AsyncCallback(UdpClientReceive), _udpClient);
				}
				catch(SocketException exception)
				{
					success = new CommunicationResult(exception.ErrorCode);
				}
				catch(InvalidOperationException)
				{
					success = CommunicationResult.Closed;
				}
			}
			return HandleResult(success);
		}

		/// <summary>
		/// Dispara um broadcast dos dados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="size"></param>
		/// <param name="broadcastEndPoint"></param>
		/// <returns></returns>
		public CommunicationResult Broadcast(byte[] data, int size, System.Net.IPEndPoint broadcastEndPoint)
		{
			return Dispatch(data, size, broadcastEndPoint);
		}

		/// <summary>
		/// Fecha a comunicação.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult Close()
		{
			try
			{
				lock (_lockObject)
				{
					_isReceiving = false;
					if(_udpClient != null)
					{
						_udpClient.Close();
						_udpClient = null;
					}
				}
			}
			catch(SocketException exception)
			{
				return HandleResult(new CommunicationResult(exception.ErrorCode));
			}
			return CommunicationResult.Success;
		}

		/// <summary>
		/// Finaliza o recebimento dos dados.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult EndReceive()
		{
			CommunicationResult success;
			try
			{
				lock (_lockObject)
				{
					_isReceiving = false;
					if(_udpClient != null)
					{
						_udpClient.Close();
						_udpClient = null;
						return Bind(LocalEndPoint);
					}
					success = CommunicationResult.Success;
				}
			}
			catch(SocketException exception)
			{
				success = HandleResult(new CommunicationResult(exception.ErrorCode));
			}
			return success;
		}

		/// <summary>
		/// Abre a comunicação.
		/// </summary>
		/// <returns></returns>
		public override CommunicationResult Open()
		{
			lock (_lockObject)
				return Bind(LocalEndPoint);
		}

		/// <summary>
		/// Recebe os dados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override CommunicationResult Receive(out byte[] data)
		{
			data = null;
			return CommunicationResult.Success;
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
			return CommunicationResult.Success;
		}

		/// <summary>
		/// Envia os dados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public override CommunicationResult Send(byte[] data, int size)
		{
			return this.Dispatch(data, size, base.RemoteEndPoint);
		}
	}
}
