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
	/// Representa um conteúdo que é vetor de bytes.
	/// </summary>
	public class ByteArrayContentParameter : MultipartFormDataParameter
	{
		private byte[] content;

		private int count;

		private int offset;

		/// <summary>
		/// Cria a instancia com o conteúdo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		public ByteArrayContentParameter(string name, byte[] content) : base(name)
		{
			content.Require("content").NotNull();
			if(content == null)
				throw new ArgumentNullException("content");
			this.content = content;
			this.offset = 0;
			this.count = content.Length;
		}

		/// <summary>
		/// Cria a isntancia com o conteúdo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public ByteArrayContentParameter(string name, byte[] content, int offset, int count) : base(name)
		{
			if(content == null)
				throw new ArgumentNullException("content");
			if((offset < 0) || (offset > content.Length))
				throw new ArgumentOutOfRangeException("offset");
			if((count < 0) || (count > (content.Length - offset)))
				throw new ArgumentOutOfRangeException("count");
			this.content = content;
			this.offset = offset;
			this.count = count;
		}

		/// <summary>
		/// Tenta calcular o tamanho do conteúdo.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public override bool TryComputeLength(out long length)
		{
			length = this.count;
			return true;
		}

		/// <summary>
		/// Escreve o conteúdo.
		/// </summary>
		/// <param name="stream"></param>
		public override void WriteContent(System.IO.Stream stream)
		{
			stream.Write(content, offset, count);
		}
	}
}
