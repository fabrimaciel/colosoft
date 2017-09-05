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

namespace Colosoft.Web
{
	/// <summary>
	/// Parametro usadao para enviar uma stream na requisição.
	/// </summary>
	public class StreamContentParameter : MultipartFormDataParameter, IDisposable
	{
		private System.IO.Stream _stream;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="stream"></param>
		public StreamContentParameter(string name, System.IO.Stream stream) : base(name)
		{
			stream.Require("stream").NotNull();
			_stream = stream;
		}

		/// <summary>
		/// Destrutor padrão.
		/// </summary>
		~StreamContentParameter()
		{
			Dispose(false);
		}

		/// <summary>
		/// Tenta recuperar o tamanho do conteúdo.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public override bool TryComputeLength(out long length)
		{
			try
			{
				length = _stream.Length;
				return true;
			}
			catch
			{
				length = 0;
				return false;
			}
		}

		/// <summary>
		/// Escreve o conteúdo do parametro.
		/// </summary>
		/// <param name="stream"></param>
		public override void WriteContent(System.IO.Stream stream)
		{
			if(_stream == null)
				throw new ObjectDisposedException("stream");
			var buffer = new byte[1024];
			var read = 0;
			while ((read = _stream.Read(buffer, 0, buffer.Length)) > 0)
				stream.Write(buffer, 0, read);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_stream != null)
				try
				{
					_stream.Dispose();
				}
				catch
				{
				}
				finally
				{
					_stream = null;
				}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
