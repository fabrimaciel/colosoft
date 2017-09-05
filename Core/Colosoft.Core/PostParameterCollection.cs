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
	/// Colecao dos parametros.
	/// </summary>
	public class PostParameterCollection : List<PostParameter>, IRequestParameter
	{
		private System.Text.Encoding _defaultEncoding = Encoding.Default;

		private string _contentType;

		/// <summary>
		/// Evento acionado quando o progresso de escrita for alterado.
		/// </summary>
		public event EventHandler<Progress.ProgressChangedEventArgs> WriteProgressChanged;

		/// <summary>
		/// Codificação que será utilizada.
		/// </summary>
		public System.Text.Encoding DefaultEncoding
		{
			get
			{
				return _defaultEncoding;
			}
			set
			{
				_defaultEncoding = value;
			}
		}

		/// <summary>
		/// Tamanho do conteudo que sera postado.
		/// </summary>
		public virtual long ContentLength
		{
			get
			{
				if(Count == 0)
					return 0;
				var letterLenght = DefaultEncoding.GetByteCount("&");
				return DefaultEncoding.GetPreamble().Length + (letterLenght * (Count - 1)) + this.Select(f => DefaultEncoding.GetByteCount(HttpUtility.UrlEncode(f.Name) ?? "") + letterLenght + DefaultEncoding.GetByteCount(HttpUtility.UrlEncode(f.Value) ?? "")).Sum();
			}
		}

		/// <summary>
		/// Tipo de conteúdo.
		/// </summary>
		public string ContentType
		{
			get
			{
				return _contentType ?? "application/x-www-form-urlencoded";
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
			WriteOutput(outputStream);
		}

		/// <summary>
		/// Escreve a saída do parametro.
		/// </summary>
		/// <param name="outputStream"></param>
		protected virtual void WriteOutput(System.IO.Stream outputStream)
		{
			var writer = new System.IO.StreamWriter(outputStream, DefaultEncoding);
			for(int i = 0; i < Count; i++)
			{
				if(i > 0)
					writer.Write("&");
				writer.Write("{0}={1}", HttpUtility.UrlEncode(this[i].Name), HttpUtility.UrlEncode(this[i].Value));
			}
			writer.Flush();
		}
	}
}
