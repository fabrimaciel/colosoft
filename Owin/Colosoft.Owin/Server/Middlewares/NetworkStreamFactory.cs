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
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server.Middlewares
{
	/// <summary>
	/// Implementação da factory de streans para rede.
	/// </summary>
	class NetworkStreamFactory : IStreamFactory
	{
		private static readonly SslProtocols _sslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls;

		/// <summary>
		/// Cria a stream para a uri informada.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public async Task<System.IO.Stream> CreateStream(Uri uri)
		{
			System.Net.Sockets.Socket socket = null;
			System.Net.IPAddress hostAddress;
			if(System.Net.IPAddress.TryParse(uri.Host, out hostAddress))
			{
				socket = await Connect(hostAddress, uri.Port);
				if(socket.Connected)
				{
					return await CreateStream(uri, socket);
				}
			}
			System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(uri.Host);
			foreach (System.Net.IPAddress address in hostEntry.AddressList)
			{
				socket = await Connect(address, uri.Port);
				if(socket.Connected)
				{
					return await CreateStream(uri, socket);
				}
			}
			return null;
		}

		/// <summary>
		/// Cria a stream para a uri no socket informado.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="socket"></param>
		/// <returns></returns>
		private async Task<System.IO.Stream> CreateStream(Uri uri, System.Net.Sockets.Socket socket)
		{
			System.IO.Stream stream = new System.Net.Sockets.NetworkStream(socket);
			if(uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				var sslStream = new System.Net.Security.SslStream(stream);
				await sslStream.AuthenticateAsClientAsync(uri.Host, new System.Security.Cryptography.X509Certificates.X509CertificateCollection(), _sslProtocols, checkCertificateRevocation: false);
				stream = sslStream;
			}
			return stream;
		}

		/// <summary>
		/// Conecta no endereço informado.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		private static Task<System.Net.Sockets.Socket> Connect(System.Net.IPAddress address, int port)
		{
			var ipe = new System.Net.IPEndPoint(address, port);
			var socket = new System.Net.Sockets.Socket(ipe.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
			var tcs = new TaskCompletionSource<System.Net.Sockets.Socket>();
			var sea = new System.Net.Sockets.SocketAsyncEventArgs();
			sea.RemoteEndPoint = ipe;
			sea.Completed += (sender, e) =>  {
				if(e.SocketError != System.Net.Sockets.SocketError.Success)
				{
					tcs.TrySetException(e.ConnectByNameError);
				}
				else
				{
					tcs.TrySetResult(socket);
				}
			};
			socket.ConnectAsync(sea);
			return tcs.Task;
		}
	}
}
