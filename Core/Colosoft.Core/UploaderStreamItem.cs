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

namespace Colosoft.Net
{
	/// <summary>
	/// Implemetação do item do uploader para uma Stream.
	/// </summary>
	public class UploaderStreamItem : IUploaderItem, IDisposable
	{
		private UploaderItemAttributeSet _attributes = new UploaderItemAttributeSet();

		private System.IO.Stream _stream;

		private long _numberBytesSent;

		/// <summary>
		/// Atributos do item.
		/// </summary>
		public UploaderItemAttributeSet Attributes
		{
			get
			{
				return _attributes;
			}
		}

		/// <summary>
		/// Tamanho do item.
		/// </summary>
		public long Length
		{
			get
			{
				return _stream != null ? _stream.Length : 0;
			}
		}

		/// <summary>
		/// Número de bytes enviados.
		/// </summary>
		public long NumberBytesSent
		{
			get
			{
				return _numberBytesSent;
			}
			set
			{
				_numberBytesSent = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream"></param>
		public UploaderStreamItem(System.IO.Stream stream)
		{
			stream.Require("stream").NotNull();
			_stream = stream;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~UploaderStreamItem()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o conteúdo do item.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream GetContent()
		{
			return _stream;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
