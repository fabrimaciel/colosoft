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
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server.Http
{
	/// <summary>
	/// Stream usada para recuperar o tamanho do conteúdo.
	/// </summary>
	class ContentLengthStream : Colosoft.IO.StreamWrapper
	{
		private readonly System.IO.Stream _stream;

		private readonly long _contentLength;

		private int _consumed;

		/// <summary>
		/// Identifica se pode escrever.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="contentLength"></param>
		public ContentLengthStream(System.IO.Stream stream, long contentLength) : base(stream)
		{
			_stream = stream;
			_contentLength = contentLength;
		}

		/// <summary>
		/// Lê os dados de forma assincrona.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if(_consumed >= _contentLength)
			{
				return 0;
			}
			var maxRead = (int)Math.Min(count, _contentLength - _consumed);
			int read = await _stream.ReadAsync(buffer, offset, maxRead, cancellationToken);
			_consumed += read;
			return read;
		}

		/// <summary>
		/// Lê os dados.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(_consumed >= _contentLength)
				return 0;
			var maxRead = (int)Math.Min(count, _contentLength - _consumed);
			int read = _stream.Read(buffer, offset, maxRead);
			_consumed += read;
			return read;
		}
	}
}
