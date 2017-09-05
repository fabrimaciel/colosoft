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

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// Armazena as informações do recurso da stream.
	/// </summary>
	class StreamResourceInfo
	{
		private string _contentType;

		private System.IO.Stream _stream;

		/// <summary>
		/// Tipo do conteúdo.
		/// </summary>
		public string ContentType
		{
			get
			{
				return _contentType;
			}
		}

		/// <summary>
		/// Stream com o conteúdo.
		/// </summary>
		public System.IO.Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="contentType"></param>
		public StreamResourceInfo(System.IO.Stream stream, string contentType)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			_contentType = contentType;
			_stream = stream;
		}
	}
}
