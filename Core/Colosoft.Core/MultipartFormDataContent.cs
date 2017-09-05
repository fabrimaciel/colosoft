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
	/// Representa um contúdo multipart.
	/// </summary>
	public class MultipartFormDataContent : List<MultipartFormDataParameter>, IRequestParameter, IDisposable
	{
		private readonly string _boundary;

		private string _subtype;

		private System.Text.Encoding _defaultEncoding = Encoding.Default;

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
		/// Tamanho do conteúdo.
		/// </summary>
		public long ContentLength
		{
			get
			{
				if(Count == 0)
					return 0;
				var letterLenght = DefaultEncoding.GetByteCount("&");
				var boundrayLenght = DefaultEncoding.GetByteCount(_boundary);
				return DefaultEncoding.GetPreamble().Length + this.Select(f => ComputeParameterLength(f)).Sum() + DefaultEncoding.GetByteCount("--") + boundrayLenght + DefaultEncoding.GetByteCount("--") + (letterLenght * 2);
			}
		}

		/// <summary>
		/// Calcula o tamanho do parametro.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private long ComputeParameterLength(MultipartFormDataParameter parameter)
		{
			var letterLenght = DefaultEncoding.GetByteCount("&");
			var boundrayLenght = DefaultEncoding.GetByteCount(_boundary);
			long length = 0;
			return DefaultEncoding.GetByteCount("--") + boundrayLenght + (letterLenght * 2) + DefaultEncoding.GetByteCount(string.Format("Content-Type: {0}", parameter.ContentType)) + (letterLenght * 2) + DefaultEncoding.GetByteCount("Content-Disposition: form-data; name=\"") + DefaultEncoding.GetByteCount(HttpUtility.UrlEncode(parameter.Name) ?? "") + letterLenght + (parameter is IFileMultipartFormDataParameter ? string.Format("; filename=\"{0}\"", (((IFileMultipartFormDataParameter)parameter).FileName)).Length : 0) + (letterLenght * 2) + (letterLenght * 2) + (parameter.TryComputeLength(out length) ? length : 0) + (letterLenght * 2);
		}

		/// <summary>
		/// Tipo do conteúdo.
		/// </summary>
		public string ContentType
		{
			get
			{
				return string.Format("multipart/{0}; boundary=\"{1}\"", _subtype, _boundary);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public MultipartFormDataContent() : this("form-data")
		{
		}

		/// <summary>
		/// Cria a instancia com o sub tipo.
		/// </summary>
		/// <param name="subtype"></param>
		public MultipartFormDataContent(string subtype) : this(subtype, Guid.NewGuid().ToString("D", System.Globalization.CultureInfo.InvariantCulture))
		{
		}

		/// <summary>
		/// Cria a instancia como subtipo e o delimitador.
		/// </summary>
		/// <param name="subtype"></param>
		/// <param name="boundary"></param>
		public MultipartFormDataContent(string subtype, string boundary)
		{
			if(string.IsNullOrWhiteSpace(subtype))
				throw new ArgumentException("boundary");
			if(string.IsNullOrWhiteSpace(boundary))
				throw new ArgumentException("boundary");
			if(boundary.Length > 70)
				throw new ArgumentOutOfRangeException("boundary");
			if(boundary.Last() == ' ' || !IsValidRFC2049(boundary))
				throw new ArgumentException("boundary");
			_boundary = boundary;
			_subtype = subtype;
		}

		/// <summary>
		/// Destrutor padrão.
		/// </summary>
		~MultipartFormDataContent()
		{
			Dispose(false);
		}

		private static bool IsValidRFC2049(string s)
		{
			foreach (char c in s)
			{
				if((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
					continue;
				switch(c)
				{
				case '\'':
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
				case ':':
				case '=':
				case '?':
					continue;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Escreve a saída.
		/// </summary>
		/// <param name="outputStream"></param>
		protected void WriteOutput(System.IO.Stream outputStream)
		{
			var writer = new System.IO.StreamWriter(outputStream, DefaultEncoding);
			foreach (var i in this)
			{
				writer.Write("--");
				writer.WriteLine(_boundary);
				writer.WriteLine("Content-Type: {0}", i.ContentType);
				writer.Write("Content-Disposition: form-data; name=\"");
				writer.Write(HttpUtility.UrlEncode(i.Name));
				writer.Write("\"");
				if(i is IFileMultipartFormDataParameter)
				{
					writer.Write("; filename=\"");
					writer.Write(((IFileMultipartFormDataParameter)i).FileName);
					writer.Write("\"");
				}
				writer.WriteLine();
				writer.WriteLine();
				writer.Flush();
				i.WriteContent(outputStream);
				outputStream.Flush();
				writer.WriteLine();
			}
			writer.Write("--");
			writer.Write(_boundary);
			writer.Write("--");
			writer.Write("\r\n");
			writer.Flush();
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
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			foreach (var i in this)
				if(i is IDisposable)
					((IDisposable)i).Dispose();
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
