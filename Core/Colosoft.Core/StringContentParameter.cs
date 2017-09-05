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
	/// Representa o conteúdo do tipo string.
	/// </summary>
	public class StringContentParameter : ByteArrayContentParameter
	{
		/// <summary>
		/// Cria a instancia com o nome do parametro e o conteúdo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		public StringContentParameter(string name, string content) : this(name, content, null, null)
		{
		}

		/// <summary>
		/// Cria a instancia com o nome do parametro e o conteúdo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <param name="encoding"></param>
		public StringContentParameter(string name, string content, Encoding encoding) : this(name, content, encoding, null)
		{
		}

		/// <summary>
		/// Cria a instancia com o nome do parametro e o conteído.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <param name="encoding"></param>
		/// <param name="mediaType"></param>
		public StringContentParameter(string name, string content, Encoding encoding, string mediaType) : base(name, GetContentByteArray(content, encoding))
		{
			this.ContentType = (mediaType == null) ? "text/plain" : mediaType;
		}

		/// <summary>
		/// Recupera os bytes.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		private static byte[] GetContentByteArray(string content, Encoding encoding)
		{
			if(content == null)
				return new byte[0];
			if(encoding == null)
				encoding = System.Text.Encoding.UTF8;
			return encoding.GetBytes(content);
		}
	}
}
