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
using System.Net;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Implementação base de uma cmunicação de rde.
	/// </summary>
	public abstract class NetworkCommunication : ICommunication, IDisposable
	{
		/// <summary>
		/// Identifica se a comunicação está aberta.
		/// </summary>
		public abstract bool IsOpen
		{
			get;
		}

		/// <summary>
		/// EndPoint remoto.
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected NetworkCommunication()
		{
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~NetworkCommunication()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Inicia o processo assincrono de recebimento dos dados.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public abstract CommunicationResult BeginReceive(Action<byte[], IPEndPoint> action);

		/// <summary>
		/// Fecha a comunicação.
		/// </summary>
		/// <returns></returns>
		public abstract CommunicationResult Close();

		/// <summary>
		/// Finaliza o recebimento de dados.
		/// </summary>
		/// <returns></returns>
		public abstract CommunicationResult EndReceive();

		/// <summary>
		/// Abre a comunicação.
		/// </summary>
		/// <returns></returns>
		public abstract CommunicationResult Open();

		/// <summary>
		/// Recebe os dados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public abstract CommunicationResult Receive(out byte[] data);

		/// <summary>
		/// Recebe os dados de forma assincrona.
		/// </summary>
		/// <param name="receiveAction"></param>
		/// <param name="timeoutAction"></param>
		/// <param name="expectResponse"></param>
		/// <returns></returns>
		public abstract CommunicationResult ReceiveAsync(Action<byte[], IPEndPoint> receiveAction, Action<IPEndPoint> timeoutAction, bool expectResponse);

		/// <summary>
		/// Envia os dados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public abstract CommunicationResult Send(byte[] data, int size);

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected abstract void Dispose(bool disposing);
	}
}
