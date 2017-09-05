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
using Colosoft.Owin.Server.Infrastructure;
using System.Globalization;

namespace Colosoft.Owin.Server.Http
{
	/// <summary>
	/// Implementação de uma stream dividida.
	/// </summary>
	class ChunkedStream : Colosoft.IO.StreamWrapper
	{
		private readonly System.IO.Stream _stream;

		private int _consumed;

		private int? _chunkLength;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream"></param>
		public ChunkedStream(System.IO.Stream stream) : base(stream)
		{
			_stream = stream;
		}

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
		/// Realiza a leitura de forma assincrona.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if(_chunkLength == null)
			{
				string rawLength = _stream.ReadLine();
				int length = Int32.Parse(rawLength, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				if(length == 0)
				{
					return 0;
				}
				_chunkLength = length;
			}
			int maxRead = Math.Min(count - offset, _chunkLength.Value - _consumed);
			int read = await _stream.ReadAsync(buffer, offset, maxRead, cancellationToken);
			_consumed += read;
			if(_consumed >= _chunkLength)
			{
				_stream.ReadLine();
				_chunkLength = null;
				_consumed = 0;
			}
			return read;
		}

		/// <summary>
		/// Realiza a leitura.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(_chunkLength == null)
			{
				string rawLength = _stream.ReadLine();
				int length = Int32.Parse(rawLength, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				if(length == 0)
				{
					return 0;
				}
				_chunkLength = length;
			}
			int maxRead = Math.Min(count - offset, _chunkLength.Value - _consumed);
			int read = _stream.Read(buffer, offset, maxRead);
			_consumed += read;
			if(_consumed >= _chunkLength)
			{
				_stream.ReadLine();
				_chunkLength = null;
				_consumed = 0;
			}
			return read;
		}
	}
}
