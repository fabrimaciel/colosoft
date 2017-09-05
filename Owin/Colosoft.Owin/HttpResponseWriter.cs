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

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação do escritor da reposta.
	/// </summary>
	class HttpResponseWriter : System.IO.TextWriter
	{
		private Microsoft.Owin.IOwinResponse _response;

		private System.Text.Encoding _encoding;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="response"></param>
		public HttpResponseWriter(Microsoft.Owin.IOwinResponse response, System.Text.Encoding encoding)
		{
			_response = response;
			_encoding = encoding;
		}

		/// <summary>
		/// Codificação.
		/// </summary>
		public override Encoding Encoding
		{
			get
			{
				return _encoding;
			}
		}

		/// <summary>
		/// Libera o conteúdo.
		/// </summary>
		public override void Flush()
		{
			_response.Body.Flush();
		}

		/// <summary>
		/// Escreve o texto.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public override void Write(char[] buffer, int index, int count)
		{
			var buffer2 = Encoding.GetBytes(buffer, index, count);
			_response.Write(buffer2, 0, buffer2.Length);
		}
	}
}
