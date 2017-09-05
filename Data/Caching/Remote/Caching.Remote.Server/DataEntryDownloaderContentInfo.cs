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
using System.ServiceModel;

namespace Colosoft.Data.Caching.Remote.Server
{
	/// <summary>
	/// Informações do conteúdo do Downloader.
	/// </summary>
	[MessageContract]
	public class DataEntryDownloaderContentInfo : IDisposable
	{
		/// <summary>
		/// Evento acionado quando a instancia for liberada.
		/// </summary>
		internal event EventHandler Disposed;

		/// <summary>
		/// Nome do conteúdo.
		/// </summary>
		[MessageHeader(MustUnderstand = true)]
		public string Name;

		/// <summary>
		/// Tamanho do conteúdo.
		/// </summary>
		[MessageHeader(MustUnderstand = true)]
		public long Length;

		/// <summary>
		/// Stream dos dados do conteúdo.
		/// </summary>
		[MessageBodyMember(Order = 1)]
		public System.IO.Stream Data;

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if(Data != null)
			{
				Data.Close();
				Data = null;
			}
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
	}
}
