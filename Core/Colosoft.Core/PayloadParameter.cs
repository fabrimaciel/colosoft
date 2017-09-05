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
	/// Implementacao do parametro da requisicao Payload.
	/// </summary>
	public class PayloadParameter : IRequestParameter
	{
		private string _content;

		private string _contentType;

		/// <summary>
		/// Evento acionado quando o progresso de escrita for alterado.
		/// </summary>
		public event EventHandler<Progress.ProgressChangedEventArgs> WriteProgressChanged;

		/// <summary>
		/// Conteúdo do parametro.
		/// </summary>
		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		/// <summary>
		/// Construtor padrao.
		/// </summary>
		/// <param name="content">Conteúdo do parametro.</param>
		public PayloadParameter(string content)
		{
			_content = content;
		}

		/// <summary>
		/// Tamanho do conteudo que sera postado.
		/// </summary>
		long IRequestParameter.ContentLength
		{
			get
			{
				return string.IsNullOrEmpty(_content) ? 0 : System.Text.Encoding.UTF8.GetByteCount(_content);
			}
		}

		/// <summary>
		/// Tipo de conteúdo.
		/// </summary>
		public string ContentType
		{
			get
			{
				return _contentType ?? "text/plain";
			}
			set
			{
				_contentType = value;
			}
		}

		/// <summary>
		/// Escreve a saída do parametro.
		/// </summary>
		/// <param name="outputStream"></param>
		void IRequestParameter.WriteOutput(System.IO.Stream outputStream)
		{
			var length = string.IsNullOrEmpty(_content) ? 0 : System.Text.Encoding.UTF8.GetByteCount(_content);
			if(length > 0)
				outputStream.Write(System.Text.Encoding.UTF8.GetBytes(_content), 0, length);
		}
	}
}
