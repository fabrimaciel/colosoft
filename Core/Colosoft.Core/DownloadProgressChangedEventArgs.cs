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

namespace Colosoft.Net
{
	/// <summary>
	/// Representa os eventos acioandos quando o progresso do download for alterado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void DownloadProgressEventHandler (object sender, DownloadProgressChangedEventArgs e);
	/// <summary>
	/// Prover dados para o evento <see cref="DownloadProgressEventHandler"/>
	/// </summary>
	public class DownloadProgressChangedEventArgs : System.ComponentModel.ProgressChangedEventArgs
	{
		private long _bytesReceived;

		private long _totalBytesToReceive;

		/// <summary>
		/// Quantidade de bytes recebidos.
		/// </summary>
		public long BytesReceived
		{
			get
			{
				return _bytesReceived;
			}
		}

		/// <summary>
		/// Total de bytes para serem recebidos.
		/// </summary>
		public long TotalBytesToReceive
		{
			get
			{
				return _totalBytesToReceive;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="bytesReceived"></param>
		/// <param name="totalBytesToReceive"></param>
		public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive) : base((int)((100f / totalBytesToReceive) * bytesReceived), null)
		{
			_bytesReceived = bytesReceived;
			_totalBytesToReceive = totalBytesToReceive;
		}
	}
}
