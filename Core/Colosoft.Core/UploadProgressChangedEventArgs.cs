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
	/// Representa os eventos acioandos quando o progresso do upload for alterado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void UploadProgressEventHandler (object sender, UploadProgressChangedEventArgs e);
	/// <summary>
	/// Classe que armazena os dados do progresso de upload.
	/// </summary>
	public class UploadProgressChangedEventArgs : System.ComponentModel.ProgressChangedEventArgs
	{
		private long _bytesSent;

		private long _totalBytesToSend;

		/// <summary>
		/// Quantidade de bytes enviados.
		/// </summary>
		public long BytesSent
		{
			get
			{
				return _bytesSent;
			}
		}

		/// <summary>
		/// Quantidade total de bytes para serem enviados.
		/// </summary>
		public long TotalBytesToSend
		{
			get
			{
				return _totalBytesToSend;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="bytesSent"></param>
		/// <param name="totalBytesToSend"></param>
		public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend) : base((int)((100f / totalBytesToSend) * bytesSent), null)
		{
			_bytesSent = bytesSent;
			_totalBytesToSend = totalBytesToSend;
		}
	}
}
